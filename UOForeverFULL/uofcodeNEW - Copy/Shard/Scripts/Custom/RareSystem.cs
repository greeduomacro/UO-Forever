// **********
// UOForever - RareSystem.cs
// **********

#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Commands;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;

#endregion

namespace Server.Items
{
    public class RareSystem
    {
        public const string RaresFolder = "Rares";
        public static Dictionary<string, RaresFile> ParsedFiles = null;

        public static RaresFile GetRaresFile(string sourcefile)
        {
            RaresFile value;
            if (ParsedFiles.TryGetValue(sourcefile, out value))
            {
                return value;
            }
            return null;
        }

        public static void Initialize()
        {
            CommandSystem.Register("ReloadRareConfigs", AccessLevel.GameMaster, ReloadRareConfigs_Command);
            CommandSystem.Register("SpawnRareFromFile", AccessLevel.GameMaster, SpawnRareFromFile_Command);
            if (ParsedFiles == null)
                // might not be null b/c it will load it in the RareAdddon deserialize function if needed
            {
                LoadRareConfigs();
            }
        }

        public static void SpawnRareFromFile_Command(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            RaresFile value;
            if (ParsedFiles.TryGetValue(e.ArgString, out value))
            {
                RaresFile file = value;
                Item spawned = file.GetRandomRareEntry();
                if (spawned != null)
                {
                    from.Backpack.AddItem(spawned);
                }
                else
                {
                    from.SendMessage("There was some sort of problem so nothing was spawned. Check the file!");
                }
            }
            else
            {
                from.SendMessage("file did not exist... existingfiles: ");
                foreach (var pair in ParsedFiles)
                {
                }
            }
        }

        public static void ReloadRareConfigs_Command(CommandEventArgs e)
        {
            LoadRareConfigs();
        }

        public static void LoadRareConfigs()
        {
            ParsedFiles = new Dictionary<string, RaresFile>();
            try
            {
                var files = Directory.GetFiles(RaresFolder, "*.*", SearchOption.TopDirectoryOnly);
                foreach (string filename in files)
                {
                    var filePathParts = filename.Split(Path.DirectorySeparatorChar);
                    ParsedFiles.Add(filePathParts[filePathParts.Length - 1],
                        new RaresFile(filePathParts[filePathParts.Length - 1]));
                }
            }
            catch (Exception e)
            {
                string msg = DateTime.UtcNow + ": ERROR loading file: " + e.Message;
                Console.WriteLine(msg);
                //LoggingCustom.Log("ERROR-RareSystem.txt", msg);
            }
        }

        public static RareAddonEntry GetRareAddonEntry(string name, string sourcefile)
        {
            // go through and see if there is an addon by that name
            string msg = null;
            RaresFile value;
            if (ParsedFiles.TryGetValue(sourcefile, out value))
            {
                RaresFile file = value;
                foreach (RareAddonEntry entry in file.RareAddons)
                {
                    if (entry.Name == name)
                    {
                        return entry;
                    }
                }
                msg = DateTime.UtcNow + ": Rare system ERROR: name=" + name + " sourcfile=" + sourcefile +
                      " ... did not match existing file and addon name!";
            }
            else
            {
                msg = DateTime.UtcNow + ": Rare system ERROR: name=" + name + " sourcfile=" + sourcefile +
                      " ... did not match any existing file!";
            }
            Console.WriteLine(msg);
            //LoggingCustom.Log("ERROR-RareSystem.txt", msg);
            return null;
        }
    }

    public class RareItem : Item
    {
        [Constructable]
        public RareItem()
            : base(0x9D7)
        {
            Weight = 1.0;
        }

        public RareItem(RareEntry entry)
            : base(entry.ItemID)
        {
            Hue = entry.Hue;
            Name = entry.Name;
            RaresFile.AddScriptAttachments(this, entry.ScriptFiles);
        }

        public RareItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class RareAddonDeed : BaseAddonDeed
    {
        public string SourceFile;

        public override BaseAddon Addon
        {
            get { return new RareAddon(Name, SourceFile); }
        }

        [Constructable]
        public RareAddonDeed(string name, string sourcefile)
        {
            Name = name;
            SourceFile = sourcefile;
            RareAddonEntry test = RareSystem.GetRareAddonEntry(name, sourcefile);
            if (test == null)
            {
                // specification in file not found
                Delete();
            }
        }

        public RareAddonDeed(RareAddonEntry entry)
        {
            SourceFile = entry.SourceFile;
            Name = entry.Name;
        }

        public RareAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            // version 0
            writer.Write(SourceFile);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            // must have all files parsed first
            if (RareSystem.ParsedFiles == null)
            {
                RareSystem.LoadRareConfigs();
            }

            int version = reader.ReadEncodedInt();

            SourceFile = reader.ReadString();
            RareAddonEntry rareAddonEntry = RareSystem.GetRareAddonEntry(Name, SourceFile);
            if (rareAddonEntry == null)
            {
                // specification in file not found
                Delete();
            }
        }
    }

    public class RareAddon : BaseAddon
    {
        public string SourceFile;

        public override BaseAddonDeed Deed
        {
            get { return new RareAddonDeed(Name, SourceFile); }
        }

        [Constructable]
        public RareAddon(string name, string sourcefile)
        {
            Name = name;
            SourceFile = sourcefile;
            RareAddonEntry rareAddonEntry = RareSystem.GetRareAddonEntry(name, sourcefile);
            if (rareAddonEntry == null)
            {
                // specification in file not found
                Delete();
                return;
            }
            foreach (RareAddonComponentEntry component in rareAddonEntry.Components)
            {
                AddonComponent newAddonComponent = new AddonComponent(component.ItemID)
                {
                    Hue = component.Hue,
                    Visible = component.Visible
                };
                AddComponent(newAddonComponent, component.X, component.Y, component.Z);
            }
            RaresFile.AddScriptAttachments(this, rareAddonEntry.ScriptFiles);
        }

        [Constructable]
        public RareAddon(string name, string sourcefile, int hue)
            : this(name, sourcefile)
        {
            if (!Deleted)
            {
                Hue = hue;
            }
        }

        public RareAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            // version 0
            writer.Write(SourceFile);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            // must have all files parsed first
            if (RareSystem.ParsedFiles == null)
            {
                RareSystem.LoadRareConfigs();
            }

            int version = reader.ReadEncodedInt();

            SourceFile = reader.ReadString();
            RareAddonEntry rareAddonEntry = RareSystem.GetRareAddonEntry(Name, SourceFile);
            if (rareAddonEntry == null)
            {
                // specification in file not found
                Delete();
            }
        }
    }

    public class RaresFile
    {
        public static void AddScriptAttachments(object target, List<string> scriptfiles)
        {
            foreach (string scriptFile in scriptfiles)
            {
                if (scriptFile.Length == 0)
                {
                    continue;
                }

                string scriptName = null;
                string[] splitScriptFileNames = null;
                if (scriptFile.Contains("/"))
                {
                    splitScriptFileNames = scriptFile.Split('/');
                }
                else if (scriptFile.Contains("\\"))
                {
                    splitScriptFileNames = scriptFile.Split('\\');
                }
                else
                {
                    scriptName = scriptFile;
                }

                if (splitScriptFileNames != null)
                {
                    scriptName = splitScriptFileNames[splitScriptFileNames.Length - 1];
                }

                // get everything before the first period (so like test.txt would name it test)
                int periodIndex = scriptName.IndexOf('.');
                if (periodIndex > 0)
                {
                    scriptName = scriptName.Substring(0, periodIndex);
                }
                XmlScript newScript = new XmlScript(scriptFile) {Name = scriptName};
                XmlAttach.AttachTo(target as IEntity, newScript);
            }
        }

        public string SourceFile;
        public List<RareEntry> Rares = new List<RareEntry>();
        public List<RareAddonEntry> RareAddons = new List<RareAddonEntry>();

        public Item GetRandomRareEntry()
        {
            int totalCount = 0;
            foreach (RareEntry entry in Rares)
            {
                totalCount += entry.SpawnProbability;
            }
            foreach (RareAddonEntry entry in RareAddons)
            {
                totalCount += entry.SpawnProbability;
            }

            int roll = Utility.Random(totalCount);
            foreach (RareEntry entry in Rares)
            {
                if (roll < entry.SpawnProbability)
                {
                    return new RareItem(entry);
                }
                roll = roll - entry.SpawnProbability;
            }
            foreach (RareAddonEntry entry in RareAddons)
            {
                if (roll < entry.SpawnProbability)
                {
                    return new RareAddonDeed(entry);
                }
                roll = roll - entry.SpawnProbability;
            }
            // should never get here
            return null;
        }

        public Item GetRareByName(string name)
        {
            foreach (RareEntry entry in Rares)
            {
                if (entry.Name == name)
                {
                    return new RareItem(entry);
                }
            }
            return (from entry in RareAddons where entry.Name == name select new RareAddonDeed(entry)).FirstOrDefault();
        }

        public RaresFile(string sourceFile)
        {
            SourceFile = sourceFile;
            try
            {
                StreamReader file = new StreamReader(RareSystem.RaresFolder + Path.DirectorySeparatorChar + SourceFile);
                string line = null;
                int lineNumber = 0;
                while ((line = file.ReadLine()) != null)
                {
                    lineNumber++;
                    line = line.Trim();
                    if (line.StartsWith("#") || line == String.Empty || line.Length == 0)
                    {
                        continue;
                    }
                    int commentIndex = line.IndexOf('#');
                    if (commentIndex != -1)
                    {
                        line = line.Substring(0, commentIndex);
                    }

                    int entrySpawnProbability = 1;
                    var scriptFiles = new List<string>();

                    int scriptIndex = line.IndexOf('$');
                    // |5234 0x38 $10 alan\ps\go.txt #apple  ... 10 is the entrySpawnProbability (10x as probable spawning as no weighting or empty, must come first after $, and then scripts)
                    if (scriptIndex != -1)
                    {
                        string scriptLine = line.Substring(scriptIndex + 1);
                        line = line.Substring(0, scriptIndex);

                        var split = scriptLine.Split();
                        int newSpawnProbability = 1;
                        foreach (string t in split)
                        {
                            if (t == string.Empty)
                            {
                                continue;
                            }
                            if (newSpawnProbability == 1) // not assigned yet
                            {
                                try
                                {
                                    newSpawnProbability = int.Parse(t);
                                    // successfully got an int
                                }
                                catch // it's a script string
                                {
                                    scriptFiles.Add(t);
                                }
                            }
                            else
                            {
                                // assume it's a script string if newSpawnProbability already been found
                                scriptFiles.Add(t);
                            }
                        }
                        entrySpawnProbability = newSpawnProbability;
                    }

                    var args = line.Split('|');
                    try
                    {
                        if (args.Length == 1)
                        {
                            throw new Exception(
                                "Rares entry \"" + line + "\" in " + SourceFile + " line number " + lineNumber +
                                " did not have at least a single | character!");
                        }
                        string name = args[0].Trim();
                        if (name == "")
                        {
                            name = null;
                        }

                        if (args.Length == 2)
                        {
                            // it's a single item rare
                            var itemArgs = (args[1].Trim()).Split();
                            int itemID = itemArgs[0].StartsWith("0x")
                                ? Convert.ToInt32(itemArgs[0].Substring(2), 16)
                                : int.Parse(itemArgs[0]);
                            int hue = itemArgs.Length > 1
                                ? (itemArgs[1].StartsWith("0x")
                                    ? Convert.ToInt32(itemArgs[1].Substring(2), 16)
                                    : int.Parse(itemArgs[1]))
                                : 0;
                            Rares.Add(new RareEntry(name, itemID, hue, entrySpawnProbability, scriptFiles));
                        }
                        else
                        {
                            // it's an addon
                            RareAddonEntry newAddonEntry = new RareAddonEntry(name, SourceFile,
                                entrySpawnProbability, scriptFiles);
                            for (int i = 1; i < args.Length; i++)
                            {
                                var componentArgs = (args[i].Trim()).Split();
                                newAddonEntry.AddAddonComponentEntry(componentArgs);
                            }
                            RareAddons.Add(newAddonEntry);
                        }
                    }
                    catch (Exception e)
                    {
                        string msg = DateTime.UtcNow + ": ERROR on line number " + lineNumber + ": " + e.Message;
                        Console.WriteLine(msg);
                        //LoggingCustom.Log("ERROR-RareSystem.txt", msg);
                    }
                }
                file.Close();
            }
            catch
            {
                //LoggingCustom.Log("ERROR-RareSystem.txt", DateTime.UtcNow + ": ERROR: " + e.Message);
            }
        }
    }

    public class RareEntry
    {
        public int ItemID = 0;
        public int Hue = 0;
        public string Name = null;
        public int SpawnProbability = 1;
        public List<string> ScriptFiles = null;

        public RareEntry(string name, int itemID, int hue, int spawnProbability, List<string> scriptFiles)
        {
            Name = name;
            ItemID = itemID;
            Hue = hue;
            SpawnProbability = spawnProbability;
            ScriptFiles = scriptFiles;
        }
    }

    public class RareAddonEntry
    {
        public string SourceFile = null;
        public string Name = null;
        public List<RareAddonComponentEntry> Components = new List<RareAddonComponentEntry>();
        public int SpawnProbability = 1;
        public List<string> ScriptFiles = null;

        public RareAddonEntry(string name, string sourceFile, int spawnProbability, List<string> scriptFiles)
        {
            Name = name;
            SourceFile = sourceFile;
            SpawnProbability = spawnProbability;
            ScriptFiles = scriptFiles;
        }

        public void AddAddonComponentEntry(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("AddonComponentEntry error: expected at least 4 args but had " + args.Length + ": " +
                                  args);
                return;
            }

            // format is x y z visible hue
            int itemID = args[0].StartsWith("0x") ? Convert.ToInt32(args[0].Substring(2), 16) : int.Parse(args[0]);
            int x = args[1].StartsWith("0x") ? Convert.ToInt32(args[1].Substring(2), 16) : int.Parse(args[1]);
            int y = args[2].StartsWith("0x") ? Convert.ToInt32(args[2].Substring(2), 16) : int.Parse(args[2]);
            int z = args[3].StartsWith("0x") ? Convert.ToInt32(args[3].Substring(2), 16) : int.Parse(args[3]);
            int hue = 0;
            bool visible = true;
            if (args.Length > 4)
            {
                visible = args[4] == "1";
            }
            if (args.Length > 5)
            {
                hue = args[5].StartsWith("0x") ? Convert.ToInt32(args[5].Substring(2), 16) : int.Parse(args[5]);
            }
            RareAddonComponentEntry newEntry = new RareAddonComponentEntry(itemID, x, y, z, hue, visible);
            Components.Add(newEntry);
        }
    }

    public class RareAddonComponentEntry
    {
        // format is x y z visible hue
        public int ItemID = 0;
        public int X = 0;
        public int Y = 0;
        public int Z = 0;
        public int Hue = 0;
        public bool Visible = true;

        public RareAddonComponentEntry(int itemID, int x, int y, int z)
            : this(itemID, x, y, z, 0, true)
        {
        }

        public RareAddonComponentEntry(int itemID, int x, int y, int z, int hue)
            : this(itemID, x, y, z, hue, true)
        {
        }

        public RareAddonComponentEntry(int itemID, int x, int y, int z, int hue, bool visible)
        {
            ItemID = itemID;
            X = x;
            Y = y;
            Z = z;
            Hue = hue;
            Visible = visible;
        }
    }

    public class WriteAddon
    {
        private class TileEntry
        {
            public readonly int ID;
            public readonly int X;
            public readonly int Y;
            public readonly int Z;

            public TileEntry(int id, int x, int y, int z)
            {
                ID = id;
                X = x;
                Y = y;
                Z = z;
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register("WriteAddon", XmlSpawner.DiskAccessLevel, WriteAddon_OnCommand);
        }

        [Usage(
            "WriteAddon <fileToAppendTo> <nameOfAddon> [zmin zmax][-noitems][-nostatics][-nomultis][-noaddons][-invisible]"
            )]
        [Description(
            "Creates a multi text file from the objects within the targeted area.  The min/max z range can also be specified."
            )]
        public static void WriteAddon_OnCommand(CommandEventArgs e)
        {
            if (e == null || e.Mobile == null)
            {
                return;
            }

            if (e.Mobile.AccessLevel < XmlSpawner.DiskAccessLevel)
            {
                e.Mobile.SendMessage("You do not have rights to perform this command.");
                return;
            }

            if (e.Arguments != null && e.Arguments.Length < 2)
            {
                e.Mobile.SendMessage(
                    "Usage:  {0} <fileToAppendTo> <nameOfAddon> [zmin zmax][-noitems][-nostatics][-nomultis][-noaddons][-invisible]",
                    e.Command);
                return;
            }

            string filename = e.Arguments[0];
            string addonName = e.Arguments[1];

            int zmin = int.MinValue;
            int zmax = int.MinValue;
            bool includeitems = true;
            bool includestatics = true;
            bool includemultis = true;
            bool includeaddons = true;
            bool includeinvisible = false;

            if (e.Arguments.Length > 2)
            {
                int index = 2;
                while (index < e.Arguments.Length)
                {
                    if (e.Arguments[index] == "-noitems")
                    {
                        includeitems = false;
                        index++;
                    }
                    else if (e.Arguments[index] == "-nostatics")
                    {
                        includestatics = false;
                        index++;
                    }
                    else if (e.Arguments[index] == "-nomultis")
                    {
                        includemultis = false;
                        index++;
                    }
                    else if (e.Arguments[index] == "-noaddons")
                    {
                        includeaddons = false;
                        index++;
                    }
                    else if (e.Arguments[index] == "-invisible")
                    {
                        includeinvisible = true;
                        index++;
                    }
                    else
                    {
                        try
                        {
                            zmin = int.Parse(e.Arguments[index++]);
                            zmax = int.Parse(e.Arguments[index++]);
                        }
                        catch
                        {
                            e.Mobile.SendMessage("{0} : Invalid zmin zmax arguments", e.Command);
                            return;
                        }
                    }
                }
            }

            string dirname;
            if (Directory.Exists(RareSystem.RaresFolder) && filename != null && !filename.StartsWith("/") &&
                !filename.StartsWith("\\"))
            {
                // put it in the defaults directory if it exists
                dirname = String.Format("{0}/{1}", RareSystem.RaresFolder, filename);
            }
            else
            {
                // otherwise just put it in the main installation dir
                dirname = filename;
            }

            DefineMultiArea(
                e.Mobile,
                dirname,
                addonName,
                zmin,
                zmax,
                includeitems,
                includestatics,
                includemultis,
                includeinvisible,
                includeaddons);
        }

        public static void DefineMultiArea(
            Mobile m,
            string dirname,
            string addonName,
            int zmin,
            int zmax,
            bool includeitems,
            bool includestatics,
            bool includemultis,
            bool includeinvisible,
            bool includeaddons)
        {
            var multiargs = new object[9];
            multiargs[0] = dirname;
            multiargs[1] = zmin;
            multiargs[2] = zmax;
            multiargs[3] = includeitems;
            multiargs[4] = includestatics;
            multiargs[5] = includemultis;
            multiargs[6] = includeinvisible;
            multiargs[7] = includeaddons;
            multiargs[8] = addonName;

            BoundingBoxPicker.Begin(m, DefineMultiArea_Callback, multiargs);
        }

        //public const MaxItemId = 16383;
        public const int MaxItemId = 39212;

        private static void DefineMultiArea_Callback(Mobile from, Map map, Point3D start, Point3D end, object state)
        {
            var multiargs = (object[]) state;

            if (from != null && multiargs != null && map != null)
            {
                string dirname = (string) multiargs[0];
                int zmin = (int) multiargs[1];
                int zmax = (int) multiargs[2];
                bool includeitems = (bool) multiargs[3];
                bool includestatics = (bool) multiargs[4];
                bool includemultis = (bool) multiargs[5];
                bool includeinvisible = (bool) multiargs[6];
                bool includeaddons = (bool) multiargs[7];
                string addonName = (string) multiargs[8];

                ArrayList itemlist = new ArrayList();
                ArrayList staticlist = new ArrayList();
                ArrayList tilelist = new ArrayList();

                int sx = (start.X > end.X) ? end.X : start.X;
                int sy = (start.Y > end.Y) ? end.Y : start.Y;
                int ex = (start.X < end.X) ? end.X : start.X;
                int ey = (start.Y < end.Y) ? end.Y : start.Y;

                // find all of the world-placed items within the specified area
                if (includeitems)
                {
                    // make the first pass for items only
                    IPooledEnumerable eable = map.GetItemsInBounds(new Rectangle2D(sx, sy, ex - sx + 1, ey - sy + 1));

                    foreach (Item item in eable)
                    {
                        // is it within the bounding area
                        if (item.Parent == null &&
                            (zmin == int.MinValue || (item.Location.Z >= zmin && item.Location.Z <= zmax)))
                        {
                            // add the item
                            if ((includeinvisible || item.Visible) && (item.ItemID <= MaxItemId))
                            {
                                itemlist.Add(item);
                            }
                        }
                    }

                    eable.Free();

                    int searchrange = 100;

                    // make the second expanded pass to pick up addon components and multi components
                    eable =
                        map.GetItemsInBounds(
                            new Rectangle2D(sx - searchrange, sy - searchrange, ex - sy + searchrange*2 + 1,
                                ey - sy + searchrange*2 + 1));

                    foreach (Item item in eable)
                    {
                        // is it within the bounding area
                        if (item.Parent == null)
                        {
                            if (item is BaseAddon && includeaddons)
                            {
                                // go through all of the addon components
                                foreach (AddonComponent c in ((BaseAddon) item).Components)
                                {
                                    int x = c.X;
                                    int y = c.Y;
                                    int z = c.Z;

                                    if ((includeinvisible || item.Visible) &&
                                        (item.ItemID <= MaxItemId || includemultis) &&
                                        (x >= sx && x <= ex && y >= sy && y <= ey &&
                                         (zmin == int.MinValue || (z >= zmin && z <= zmax))))
                                    {
                                        itemlist.Add(c);
                                    }
                                }
                            }

                            if (item is BaseMulti && includemultis)
                            {
                                // go through all of the multi components
                                MultiComponentList mcl = ((BaseMulti) item).Components;
                                if (mcl != null && mcl.List != null)
                                {
                                    foreach (MultiTileEntry t in mcl.List)
                                    {
                                        int x = t.m_OffsetX + item.X;
                                        int y = t.m_OffsetY + item.Y;
                                        int z = t.m_OffsetZ + item.Z;
                                        int itemID = t.m_ItemID & 0x3FFF;

                                        if (x >= sx && x <= ex && y >= sy && y <= ey &&
                                            (zmin == int.MinValue || (z >= zmin && z <= zmax)))
                                        {
                                            tilelist.Add(new TileEntry(itemID, x, y, z));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    eable.Free();
                }

                // find all of the static tiles within the specified area
                if (includestatics)
                {
                    // count the statics
                    for (int x = sx; x < ex; x++)
                    {
                        for (int y = sy; y < ey; y++)
                        {
                            var statics = map.Tiles.GetStaticTiles(x, y, false);

                            foreach (StaticTile t in statics)
                            {
                                if ((zmin == int.MinValue || (t.Z >= zmin && t.Z <= zmax)))
                                {
                                    staticlist.Add(new TileEntry(t.ID & 0x3FFF, x, y, t.Z));
                                }
                            }
                        }
                    }
                }

                int nstatics = staticlist.Count;
                int nitems = itemlist.Count;
                int ntiles = tilelist.Count;

                int ntotal = nitems + nstatics + ntiles;

                int ninvisible = 0;
                int nmultis = ntiles;
                int naddons = 0;

                foreach (Item item in itemlist)
                {
                    int x = item.X - from.X;
                    int y = item.Y - from.Y;
                    int z = item.Z - from.Z;

                    if (item.ItemID > 16383)
                    {
                        nmultis++;
                    }
                    if (!item.Visible)
                    {
                        ninvisible++;
                    }
                    if (item is BaseAddon || item is AddonComponent)
                    {
                        naddons++;
                    }
                }

                try
                {
                    // open the file, overwrite any previous contents
                    StreamWriter op = new StreamWriter(dirname, true);

                    op.Write(addonName);
                    // write out the items
                    foreach (Item item in itemlist)
                    {
                        int x = item.X - @from.X;
                        int y = item.Y - @from.Y;
                        int z = item.Z - @from.Z;

                        if (item.Hue > 0)
                        {
                            // format is x y z visible hue
                            op.Write("|{0} {1} {2} {3} {4} {5}", item.ItemID, x, y, z, item.Visible ? 1 : 0,
                                item.Hue);
                        }
                        else
                        {
                            // format is x y z visible
                            op.Write("|{0} {1} {2} {3} {4}", item.ItemID, x, y, z, item.Visible ? 1 : 0);
                        }
                    }

                    if (includestatics)
                    {
                        foreach (TileEntry s in staticlist)
                        {
                            int x = s.X - @from.X;
                            int y = s.Y - @from.Y;
                            int z = s.Z - @from.Z;
                            int ID = s.ID;
                            op.Write("|{0} {1} {2} {3} {4}", ID, x, y, z, 1);
                        }
                    }

                    if (includemultis)
                    {
                        foreach (TileEntry s in tilelist)
                        {
                            int x = s.X - @from.X;
                            int y = s.Y - @from.Y;
                            int z = s.Z - @from.Z;
                            int ID = s.ID;
                            op.Write("|{0} {1} {2} {3} {4}", ID, x, y, z, 1);
                        }
                    }
                    op.WriteLine();

                    op.Close();
                }
                catch
                {
                    from.SendMessage("Error writing multi file {0}", dirname);
                    return;
                }

                from.SendMessage(66, "WriteAddon results:");

                if (includeitems)
                {
                    from.SendMessage(66, "Included {0} items", nitems);

                    if (includemultis)
                    {
                        from.SendMessage("{0} multis", nmultis);
                    }
                    else
                    {
                        from.SendMessage(33, "Ignored multis");
                    }

                    if (includeinvisible)
                    {
                        from.SendMessage("{0} invisible", ninvisible);
                    }
                    else
                    {
                        from.SendMessage(33, "Ignored invisible");
                    }

                    if (includeaddons)
                    {
                        from.SendMessage("{0} addons", naddons);
                    }
                    else
                    {
                        from.SendMessage(33, "Ignored addons");
                    }
                }
                else
                {
                    from.SendMessage(33, "Ignored items");
                }

                if (includestatics)
                {
                    from.SendMessage(66, "Included {0} statics", nstatics);
                }
                else
                {
                    from.SendMessage(33, "Ignored statics");
                }

                from.SendMessage(66, "Saved {0} components to {1}", ntotal, dirname);
            }
        }
    }
}