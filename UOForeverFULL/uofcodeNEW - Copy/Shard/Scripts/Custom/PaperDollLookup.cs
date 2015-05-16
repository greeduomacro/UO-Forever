// **********
// UOForever - PaperDollLookup.cs
// **********

#region References

using System;
using System.Collections.Generic;
using System.IO;
using Server.Commands;

#endregion

namespace Server.Gumps
{
    public class PaperDollLookup
    {
        public const string PapderdollDictionaryFileName = "PaperdollDictionary.txt";
        public static Dictionary<int, int> ItemIDtoGumpID = new Dictionary<int, int>();

        public static void Initialize()
        {
            CommandSystem.Register("UberScriptPaperdollReset", AccessLevel.GameMaster,
                UberScriptPaperdollReset_Command);
            LoadPaperdollDictionary();
        }

        public static void LoadPaperdollDictionary()
        {
            string path = "Data" + Path.DirectorySeparatorChar + PapderdollDictionaryFileName;
            ItemIDtoGumpID = new Dictionary<int, int>();
            int lineNumber = 0;
            try
            {
                StreamReader file =
                    new StreamReader(path);
                string line = null;

                while ((line = file.ReadLine()) != null)
                {
                    lineNumber++;
                    line = line.Trim();
                    //itemid<tab>gumpid<tab>name
                    if (line.StartsWith("#")) continue; // comments

                    var args = line.Split('\t');
                    if (args.Length != 3) continue; // needs all 3 elements
                    if (args[0].Length == 0 || args[1].Length == 0) continue;

                    int itemID = args[0].StartsWith("0x")
                        ? Convert.ToInt32(args[0].Substring(2), 16)
                        : int.Parse(args[0]);
                    int gumpID = args[1].StartsWith("0x")
                        ? Convert.ToInt32(args[1].Substring(2), 16)
                        : int.Parse(args[1]);
                    ItemIDtoGumpID.Add(itemID, gumpID);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading paperdoll dictionary line " + lineNumber + ": " + e.Message);
            }
        }

        public static void UberScriptPaperdollReset_Command(CommandEventArgs e)
        {
            LoadPaperdollDictionary();
        }
    }
}