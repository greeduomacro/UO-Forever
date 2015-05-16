using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Server.Commands;
using System.Xml;

namespace Server.Engines.XmlSpawner2
{
    public class ParsedGumps
    {
        public static void Initialize()
        {
            CommandSystem.Register("UberGumpReset", AccessLevel.GameMaster, new CommandEventHandler(ResetScripts_Command));
        }

        public static void ResetScripts_Command(CommandEventArgs e)
        {
            GumpFileMap = new Dictionary<string, string>();
            Gumps = new Dictionary<string, UberGumpBase>();

            // should probably ensure all the timer stuff is restarted as appropriate
            UberScriptTimedScripts.ClearSubscriptions();

            List<XmlScript> deletedScripts = new List<XmlScript>();

            foreach (KeyValuePair<XmlScript, bool> scriptPair in XmlScript.AllScripts)
            {
                XmlScript script = scriptPair.Key;
                if (script.Deleted)
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
            }
            foreach (XmlScript script in deletedScripts)
            {
                XmlScript.AllScripts.Remove(script);
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
        public static Dictionary<string, string> GumpFileMap = new Dictionary<string, string>();
        // scripts used to map absolute path to absolute rootnode
        public static Dictionary<string, UberGumpBase> Gumps = new Dictionary<string, UberGumpBase>();

        public static void AddGump(string GumpFile)
        {
            //only add if it hasn't already been added
            try
            {
                string path;
                GumpFileMap.TryGetValue(GumpFile.ToLower(), out path);
                if (path == null)
                {
                    path = Path.Combine(Core.BaseDirectory, "UberScripts", GumpFile);
                    if (!File.Exists(path))
                    {
                        throw new UberScriptException("Script file did not exist at " + path);
                    }
                    GumpFileMap.Add(GumpFile.ToLower(), path);
                }

                if (!Gumps.ContainsKey(path))
                {
                    XmlDocument parsedXml = new XmlDocument(); //* create an xml document object.
                    parsedXml.Load(path); //* load the XML document from the specified file.
                    if (parsedXml == null) throw new Exception("Xml Document parsed to null!");
                    UberGumpBase parsedGump = new UberGumpBase(parsedXml);
                    Gumps.Add(path, parsedGump);
                }
            }
            catch (Exception general)
            {
                Console.WriteLine("Error Parsing Gump Xml file: " + GumpFile);
                Console.WriteLine(general.Message);
                Console.WriteLine(general.StackTrace);
            }
        }

        public static bool SendGump(TriggerObject trigObject, Mobile target, string GumpFile, bool closeGump )
        {
            if (GumpFile == null || target == null) return false;

            string path;
            GumpFileMap.TryGetValue(GumpFile.ToLower(), out path);
            if (path == null)
            {
                //path = Path.GetFullPath(ScriptFile);
                path = Path.Combine(Core.BaseDirectory, "UberScripts", GumpFile);
                if (!File.Exists(path))
                {
                    throw new UberScriptException("Script file did not exist at " + path);
                }
                GumpFileMap.Add(GumpFile.ToLower(), path);
            }

            UberGumpBase parsedGump;
            XmlDocument parsedXml;
            if (!Gumps.ContainsKey(path))
            {
                parsedXml = new XmlDocument(); //* create an xml document object.
                parsedXml.Load(path); //* load the XML document from the specified file.
                if (parsedXml == null) return false;
                parsedGump = new UberGumpBase(parsedXml.FirstChild);
                Gumps.Add(path, parsedGump);
            }
            else
            {
                parsedGump = Gumps[path];
            }

            if (closeGump && target.HasGump(typeof(UberScriptGump)))
                target.CloseGump(typeof(UberScriptGump));
            target.SendGump(new UberScriptGump(target, trigObject, parsedGump, GumpFile));
            return true;
        }
    }
}
