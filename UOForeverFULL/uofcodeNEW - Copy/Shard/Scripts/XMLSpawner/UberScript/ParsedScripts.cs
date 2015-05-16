using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Server.Commands;
using Server.Targeting;

namespace Server.Engines.XmlSpawner2
{
    public class ParsedScripts
    {
        public const string UberScriptDirectory = "UberScripts";
        
        public static void Initialize()
        {
            CommandSystem.Register("UberReset", AccessLevel.GameMaster, new CommandEventHandler(ResetScripts_Command));
            CommandSystem.Register("UberDebugLevel", AccessLevel.GameMaster, new CommandEventHandler(DebugLevel_Command));
            CommandSystem.Register("AddScript", AccessLevel.GameMaster, new CommandEventHandler(AddScript_Command));
        }

        public static void AddScript_Command(CommandEventArgs e)
        {
            if (e.Arguments.Length == 0)
            {
                e.Mobile.SendMessage("You must specify an existing uberscript file name, e.g. [addscript alan\test.us");
            }
            else
            {
                string fileName = e.Arguments[0];
                string scriptName = null;
                if (e.Arguments.Length > 1) { scriptName = e.Arguments[1]; }
                string path = Path.GetFullPath(Path.Combine(Core.BaseDirectory, ParsedScripts.UberScriptDirectory, fileName));
                if (!File.Exists(path))
                {
                    e.Mobile.SendMessage("There is no file at path: " + fileName);
                }
                else
                {
                    e.Mobile.SendMessage("Target something to attach that script to.");
                    e.Mobile.Target = new AddScriptTarget(fileName, scriptName);
                }
            }
        }

        private class AddScriptTarget : Target
        {
            public string FilePath;
            public string ScriptName;

            public AddScriptTarget(string filepath, string scriptname = null)
                : base(-1, false, TargetFlags.None)
            {
                FilePath = filepath;
                ScriptName = scriptname;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                Mobile mob = target as Mobile;
                Item item = target as Item;
                if (mob != null)
                {
                    from.SendMessage(0x38, "You have attached the script to " + target);
                    XmlScript script = new XmlScript(FilePath);
                    script.Name = ScriptName;
                    XmlAttach.AttachTo(mob, script);
                }
                else if (item != null)
                {
                    from.SendMessage(0x38, "You have attached the script to " + target);
                    XmlScript script = new XmlScript(FilePath);
                    script.Name = ScriptName;
                    XmlAttach.AttachTo(item, script);
                }
                else
                {
                    from.SendMessage(38, "You can't attach a script to that!");
                }
            }
        }

        public static void ResetScripts_Command(CommandEventArgs e)
        {
            // first reset the gumps
            ParsedGumps.GumpFileMap = new Dictionary<string, string>();
            ParsedGumps.Gumps = new Dictionary<string, UberGumpBase>();
            
            List<XmlScript> deletedScripts = new List<XmlScript>();
            if (e.Arguments.Length == 0)
            {
                ScriptFileMap = new Dictionary<string, string>();
                Scripts = new Dictionary<string, RootNode>();

                // should probably ensure all the timer stuff is restarted as appropriate
                UberScriptTimedScripts.ClearSubscriptions();
				UberScriptFunctions.Methods.StopAllLineEffectTimers();
				UberScriptFunctions.Methods.StopAllLineScriptTimers();

                foreach (KeyValuePair<XmlScript, bool> scriptPair in XmlScript.AllScripts)
                {
                    XmlScript script = scriptPair.Key;
                    if (script.Deleted || script.AttachedTo == null || script.AttachedTo.Deleted)
                    {
                        deletedScripts.Add(script);
                        continue;
                    }
                    // hacky way to resubscribe the timers if at all possible
                    XmlScript.TimerSubscriptionFlag temp = script.TimerSubscriptions;
                    // unsubscribe them
                    script.TimerSubscriptions = XmlScript.TimerSubscriptionFlag.None;
                    // resubscribe them to what they were previously subscribed to
                    script.TimerSubscriptions = temp;

                    script.UpdateScriptTriggerLookup();
                }
                foreach (XmlScript script in deletedScripts)
                {
                    XmlScript.AllScripts.Remove(script);
                }
                if (e.Mobile != null)
                {
                    e.Mobile.SendMessage("All scripts have been reset.");
                }
            }
            else
            {
                try
                {
                    // first, always clear out deleted scripts
                    foreach (KeyValuePair<XmlScript, bool> scriptPair in XmlScript.AllScripts)
                    {
                        XmlScript script = scriptPair.Key;
                        if (script.Deleted)
                        {
                            deletedScripts.Add(script);
                            continue;
                        }
                    }
                    foreach (XmlScript script in deletedScripts)
                    {
                        XmlScript.AllScripts.Remove(script);
                    }

                    string fileName = e.Arguments[0].ToLower();
                    string path = Path.GetFullPath(Path.Combine(Core.BaseDirectory, ParsedScripts.UberScriptDirectory, fileName)); ;

                    if (Directory.Exists(path))
                    {
                        // reset all uberscripts in that directory, allow to toggle subdirectory reset too
                        bool resetSubdirectories = (e.Arguments.Length > 1 && e.Arguments[1].ToLower() == "true");
                        foreach (string file in Directory.GetFiles(path, "*.*", resetSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                        {
                            ResetScriptFile(file, out path);
                            e.Mobile.SendMessage("All scripts connected to " + path + " were reset.");
                        }
                        if (e.Mobile != null)
                        {
                            e.Mobile.SendMessage("All scripts INSIDE the DIRECTORY " + fileName + (resetSubdirectories ? " AND its SUBDIRECTORIES" : "") + " were reset.");
                        }
                    }
                    else
                    {
                        ResetScriptFile(fileName, out path);
                        if (e.Mobile != null)
                        {
                            e.Mobile.SendMessage("All scripts connected to " + path + " were reset.");
                        }
                    }
                }
                catch (Exception except)
                {
                    if (e.Mobile != null)
                    {
                        e.Mobile.SendMessage("Exception caught: " + except.Message);
                    }
                }
            }
        }

        public static void ResetScriptFile(string fileName, out string path)
        {
            ScriptFileMap.TryGetValue(fileName, out path);
            if (path == null)
            {
                path = Path.GetFullPath(Path.Combine(Core.BaseDirectory, ParsedScripts.UberScriptDirectory, fileName));

                if (!File.Exists(path))
                {
                    throw new UberScriptException("Script file did not exist at " + path);
                }
            }
            if (Scripts.ContainsKey(path))
            {
                Scripts.Remove(path);
            }
            foreach (KeyValuePair<string, string> filePair in ScriptFileMap)
            {
                foreach (KeyValuePair<XmlScript, bool> scriptPair in XmlScript.AllScripts)
                {
                    XmlScript script = scriptPair.Key;
                    if (ScriptFileMap.ContainsKey(script.ScriptFile) && ScriptFileMap[script.ScriptFile] == path)
                    {
                        // hacky way to resubscribe the timers if at all possible
                        XmlScript.TimerSubscriptionFlag temp = script.TimerSubscriptions;
                        // unsubscribe them
                        script.TimerSubscriptions = XmlScript.TimerSubscriptionFlag.None;
                        // resubscribe them to what they were previously subscribed to
                        script.TimerSubscriptions = temp;

                        script.UpdateScriptTriggerLookup();
                    }
                }
            }
        }

        public enum DebugLevels : int
        {
            NoMessages = 0,
            ScriptDebugMessages = 1,
            ScriptDebugMessagesAndStackTrace = 2
        }

        public static int DebugLevel = 2;

        public static void DebugLevel_Command(CommandEventArgs e)
        {
            string[] args = e.Arguments;
            if (args != null)
            {
                bool error = false;
                if (args.Length > 0)
                {
                    try
                    {
                        string val = args[0].ToLower().Trim();
                        if (val == "0")
                        {
                            DebugLevel = (int)DebugLevels.NoMessages;
                            e.Mobile.SendMessage("Successfully set to 0.  Scripts will no longer print any debug messages in console!");
                        }
                        else if (val == "1")
                        {
                            DebugLevel = (int)DebugLevels.ScriptDebugMessages;
                            e.Mobile.SendMessage("Successfully set to 1.  Scripts will print debug messages in console!");
                        }
                        else if (val == "2")
                        {
                            DebugLevel = (int)DebugLevels.ScriptDebugMessagesAndStackTrace;
                            e.Mobile.SendMessage("Successfully set to 2.  Scripts will print debug messages AND stack traces in console!");
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    catch (Exception) { error = true; }
                }
                else
                {
                    error = true;
                }
                if (error)
                {
                    e.Mobile.SendMessage("You must provide either 0 (No Message), 1 (Debug Messages), or 2 (Debug AND Stack Traces)");
                }
            }
        }

        // ScriptFileMap used to map relative paths to absolute paths
        public static Dictionary<string, string> ScriptFileMap = new Dictionary<string, string>();
        // scripts used to map absolute path to absolute rootnode
        public static Dictionary<string, RootNode> Scripts = new Dictionary<string, RootNode>();

        public static void AddScript(string ScriptFile)
        {
            //only add if it hasn't already been added
            try
            {
                string path;
                ScriptFileMap.TryGetValue(ScriptFile.ToLower(), out path);
                if (path == null)
                {
                    path = Path.GetFullPath(Path.Combine(Core.BaseDirectory, ParsedScripts.UberScriptDirectory, ScriptFile));
                    if (!File.Exists(path))
                    {
                        throw new UberScriptException("Script file did not exist at " + path);
                    }
                    ScriptFileMap.Add(ScriptFile.ToLower(), path);
                }

                if (!Scripts.ContainsKey(path))
                {
                    RootNode parsedScriptRoot = UberTreeParser.ParseFile(path);
                    if (parsedScriptRoot == null) throw new UberScriptException("UberTreeParser.ParseFile returned null node!");
                    Scripts.Add(path, parsedScriptRoot);
                }
            }
            catch (UberScriptException e)
            {
                if (DebugLevel >= (int)DebugLevels.ScriptDebugMessages)
                {
                    Console.WriteLine("Error Parsing script file: " + ScriptFile);
                    Exception innerMostException = e;
                    Console.WriteLine(e.Message);
                    int level = 0;
                    while (true)
                    {
                        if (innerMostException.InnerException == null) break;
                        level++;
                        innerMostException = innerMostException.InnerException;
                        for (int i = 0; i < level; i++)
                        {
                            Console.Write("\t");
                        }
                        Console.WriteLine(innerMostException.Message);
                    }
                    if (DebugLevel >= (int)DebugLevels.ScriptDebugMessagesAndStackTrace)
                        Console.WriteLine(innerMostException.StackTrace);
                }
            }
            catch (Exception general)
            {
                Console.WriteLine("Error Parsing script file: " + ScriptFile);
                Console.WriteLine("GENERAL UNCAUGHT ERROR: this should never happen!");
                Console.WriteLine(general.Message);
                Console.WriteLine(general.StackTrace);
            }
        }

        public static XmlScript.TimerSubscriptionFlag GetTimerTriggers(string ScriptFile, List<int> rootNodeIndeces)
        {
            try
            {
                RootNode parsedScriptRoot = GetRootNode(ScriptFile, rootNodeIndeces);
                if (parsedScriptRoot == null) return XmlScript.TimerSubscriptionFlag.None;
                return parsedScriptRoot.TimerTriggerNodes;
            }
            catch (Exception general)
            {
                Console.WriteLine("Error Executing script file: " + ScriptFile);
                Console.WriteLine("GENERAL UNCAUGHT ERROR: this should never happen!");
                Console.WriteLine(general.Message);
                Console.WriteLine(general.StackTrace);
                return XmlScript.TimerSubscriptionFlag.None;
            }
        }

        public static bool TryExecuteScript(string ScriptFile, TriggerObject trigObject, List<int> rootNodeIndeces, bool triggerScriptsOnly)
        {
            bool signalToOverride = false;

            RootNode parsedScriptRoot = GetRootNode(ScriptFile, rootNodeIndeces);
            if (parsedScriptRoot == null) return false;

            if (triggerScriptsOnly)
            {
                foreach (TriggerNode trigNode in parsedScriptRoot.TriggerNodes)
                {
                    if (trigNode.TrigName == trigObject.TrigName)
                    {
                        ProcessResult result = trigNode.Process(ProcessResult.None, trigObject);
                        if (result == ProcessResult.ReturnOverride)
                        {
                            signalToOverride = true;
                        }
                    }
                }
            }
            else if (trigObject.Script.ProceedToNextStage)
            {
                // NOTE: signalToOverride is overloaded--it works for both NEXTSTAGE function (sequences)
                // AND triggers
                if (parsedScriptRoot.Process(ProcessResult.None, trigObject) == ProcessResult.EndOfSequence)
                    signalToOverride = true;
            }
            else
            {
                parsedScriptRoot.Process(ProcessResult.None, trigObject);
            }
            return signalToOverride;
        }

        public static RootNode GetRootNode(string ScriptFile, List<int> rootNodeIndeces)
        {
            string path;
            ScriptFileMap.TryGetValue(ScriptFile.ToLower(), out path);
            if (path == null)
            {
                //path = Path.GetFullPath(ScriptFile);
                path = Path.GetFullPath(Path.Combine(Core.BaseDirectory, ParsedScripts.UberScriptDirectory, ScriptFile));
                if (!File.Exists(path))
                {
                    throw new UberScriptException("Script file did not exist at " + path);
                }
                ScriptFileMap.Add(ScriptFile.ToLower(), path);
            }

            RootNode parsedScriptRoot;
            if (!Scripts.ContainsKey(path))
            {
                parsedScriptRoot = UberTreeParser.ParseFile(path);
                if (parsedScriptRoot == null) return null;
                Scripts.Add(path, parsedScriptRoot);
            }
            else
            {
                parsedScriptRoot = Scripts[path];
            }
            try
            {
                // in the case where there are child root (i.e. spawn) nodes
                // we need to traverse the tree down to the node that pertains to 
                // that particular spawn... e.g the following sequence has two onDeath trigger handlers,
                // so we need to travel to the spawn node under which the trigger handler is pertinent
                // orc
                // {
                //    onDeath
                //    {
                //      troll
                //      {
                //          onDeath
                //          {
                //              MSG(TRIGMOB(),You did it!)
                //          }
                //      }
                //    }
                // }
                foreach (int index in rootNodeIndeces)
                {
                    parsedScriptRoot = parsedScriptRoot.ChildRoots[index];
                }
            }
            catch
            {
                throw new UberScriptException("ChildRoots of script " + path + " did not match the input indeces... "
                                            + "a script has been reloaded without the same amount/sequence of root nodes "
                                            + "and an existing spawned mob or item was created with that script before the change was made!");
            }
            return parsedScriptRoot;
        }
    }
}
