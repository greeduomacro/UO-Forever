// **********
// UOForever - LostStabledPetRecorder.cs
// **********

#region References

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;

#endregion

namespace Server.Items
{
    public class LostStabledPetRecorder
    {
        public static List<PlayerMobile> PlayerMobiles = new List<PlayerMobile>();
        public static List<Mobile> ProblemMobs = new List<Mobile>();
        public static bool Enabled = false;

        public static void Initialize()
        {
            if (Enabled)
            {
                // try to write out a save file
                Console.WriteLine("Trying to write Problem Mobs");
                foreach (PlayerMobile pm in PlayerMobiles)
                {
                    foreach (Mobile mob in pm.Stabled)
                    {
                        BaseCreature bc = mob as BaseCreature;
                        if (bc.LastOwner != pm)
                        {
                            bc.Owners.Add(pm); // make this the last owner
                        }
                        /*LoggingCustom.Log("PetAdding.txt",
                            bc.Name + "\t" + bc.Serial + "\tDeleted:" + bc.Deleted + "\t" + bc.LastOwner + "\t" +
                            bc.LastOwner);*/
                        ProblemMobs.Add(bc);
                    }
                }

                SaveMobiles(ProblemMobs);
            }
            else
            {
                foreach (Mobile mob in ProblemMobs)
                {
                    Console.Write("LostStabledPet problem mob: " + mob.Serial + "\t" + mob.Name + "\t");
                    if (mob is BaseCreature && ((BaseCreature) mob).LastOwner != null)
                    {
                        Console.WriteLine(((BaseCreature) mob).LastOwner.ToString());
                    }
                    else
                    {
                        Console.WriteLine("");
                    }
                }
            }
        }

        private static string m_LoadingType;

        public static string LoadingType
        {
            get { return m_LoadingType; }
        }

        private interface IEntityEntry
        {
            Serial Serial { get; }
            int TypeID { get; }
            long Position { get; }
            int Length { get; }
        }

        private sealed class MobileEntry : IEntityEntry
        {
            private readonly Mobile m_Mobile;
            private readonly int m_TypeID;
            private readonly string m_TypeName;
            private readonly long m_Position;
            private readonly int m_Length;

            public Mobile Mobile
            {
                get { return m_Mobile; }
            }

            public Serial Serial
            {
                get { return m_Mobile == null ? Serial.MinusOne : m_Mobile.Serial; }
            }

            public int TypeID
            {
                get { return m_TypeID; }
            }

            public string TypeName
            {
                get { return m_TypeName; }
            }

            public long Position
            {
                get { return m_Position; }
            }

            public int Length
            {
                get { return m_Length; }
            }

            public MobileEntry(Mobile mobile, int typeID, string typeName, long pos, int length)
            {
                m_Mobile = mobile;
                m_TypeID = typeID;
                m_TypeName = typeName;
                m_Position = pos;
                m_Length = length;
            }
        }

        protected static void SaveMobiles(List<Mobile> moblist)
        {
            GenericWriter idx = new BinaryFileWriter(MobileIndexPath, false);
            GenericWriter tdb = new BinaryFileWriter(MobileTypesPath, false);
            GenericWriter bin = new BinaryFileWriter(MobileDataPath, true);

            idx.Write(moblist.Count);

            foreach (Mobile m in moblist)
            {
                long start = bin.Position;

                idx.Write(m.TypeRef);
                idx.Write(m.Serial);
                idx.Write(start);

                m.Serialize(bin);

                idx.Write((int) (bin.Position - start));

                m.FreeCache();
            }

            tdb.Write(World.MobileTypes.Count);

            foreach (Type t in World.MobileTypes)
            {
                tdb.Write(t.FullName);
            }

            idx.Close();
            tdb.Close();
            bin.Close();
        }

        public static readonly string MobileIndexPath = "Mobiles.idx";
        public static readonly string MobileTypesPath = "Mobiles.tdb";
        public static readonly string MobileDataPath = "Mobiles.bin";

        private static List<Tuple<ConstructorInfo, string>> ReadTypes(BinaryReader tdbReader)
        {
            int count = tdbReader.ReadInt32();

            var types = new List<Tuple<ConstructorInfo, string>>(count);

            for (int i = 0; i < count; ++i)
            {
                string typeName = tdbReader.ReadString();

                Type t = ScriptCompiler.FindTypeByFullName(typeName);

                if (t == null)
                {
                    Console.WriteLine("failed");

                    if (!Core.Service)
                    {
                        Console.WriteLine("Error: Type '{0}' was not found. Delete all of those types? (y/n)", typeName);

                        if (Console.ReadKey(true).Key == ConsoleKey.Y)
                        {
                            types.Add(null);
                            Console.Write("Loading lost pets...");
                            continue;
                        }

                        Console.WriteLine("Types will not be deleted. An exception will be thrown.");
                    }
                    else
                    {
                        Console.WriteLine("Error: Type '{0}' was not found.", typeName);
                    }

                    throw new Exception(String.Format("Bad type '{0}'", typeName));
                }

                ConstructorInfo ctor = t.GetConstructor(new[] {typeof (Serial)});

                if (ctor != null)
                {
                    types.Add(new Tuple<ConstructorInfo, string>(ctor, typeName));
                }
                else
                {
                    throw new Exception(String.Format("Type '{0}' does not have a serialization constructor", t));
                }
            }

            return types;
        }

        public static List<Mobile> ReadMobiles()
        {
            var output = new List<Mobile>();
            try
            {
                var mobiles = new List<MobileEntry>();

                if (File.Exists(MobileIndexPath) && File.Exists(MobileTypesPath))
                {
                    using (
                        FileStream idx = new FileStream(MobileIndexPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                        )
                    {
                        BinaryReader idxReader = new BinaryReader(idx);

                        using (
                            FileStream tdb = new FileStream(MobileTypesPath, FileMode.Open, FileAccess.Read,
                                FileShare.Read))
                        {
                            BinaryReader tdbReader = new BinaryReader(tdb);

                            var types = ReadTypes(tdbReader);

                            int mobileCount = idxReader.ReadInt32();

                            for (int i = 0; i < mobileCount; ++i)
                            {
                                int typeID = idxReader.ReadInt32();
                                int serial = idxReader.ReadInt32();
                                long pos = idxReader.ReadInt64();
                                int length = idxReader.ReadInt32();

                                var objs = types[typeID];

                                if (objs == null)
                                {
                                    continue;
                                }

                                ConstructorInfo ctor = objs.Item1;
                                string typeName = objs.Item2;

                                try
                                {
                                    Mobile value;
                                    Mobile m = null;
                                    if (World.Mobiles.TryGetValue(serial, out value))
                                    {
                                        Mobile sameSerialMob = value;
                                        BaseCreature bc = sameSerialMob as BaseCreature;
                                        // Don't use the real serial number, get a new serial number for it 
                                        serial = Serial.NewMobile;
                                        m = (Mobile) (ctor.Invoke(new object[] {(Serial) serial}));
                                        // this constructor gets a new, unused serial number

                                        if (m.GetType() != sameSerialMob.GetType())
                                        {
                                            // the serial has already been reused by a different type of mob
                                            // which means the original stabled mob is gone.  Therefore add the original mob
                                            output.Add(m);
                                            World.AddMobile(m);
                                            /*LoggingCustom.Log("PetReassignedSerials.txt",
                                                "serial was previously replaced by: " + sameSerialMob.Name + "\t" +
                                                sameSerialMob.Serial + "\tDeleted:" +
                                                sameSerialMob.Deleted + "\t|new serial assigned for pet:" + m.Serial);*/
                                        }
                                        else
                                            // it's a very safe bet that it's the same mob (though we can't be absolutely sure)...
                                        {
                                            /*LoggingCustom.Log("PetStillExists.txt",
                                                "Now Existing: " + sameSerialMob.Name + "\t" + sameSerialMob.Serial +
                                                "\tDeleted:" + sameSerialMob.Deleted);*/
                                            // don't add mob to output
                                        }
                                    }
                                    else
                                    {
                                        // construct mob with it's original serial number... this probably won't happen much
                                        m = (Mobile) (ctor.Invoke(new object[] {(Serial) serial}));
                                        // this constructor adds this mob into the World.Mobiles dictionary at this serial
                                        World.AddMobile(m);
                                        output.Add(m);
                                        //LoggingCustom.Log("PetSameSerialRestored.txt", m.Serial + "");
                                    }
                                    // always add it to this list regardless
                                    mobiles.Add(new MobileEntry(m, typeID, typeName, pos, length));
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("LostStabledPetRecorder error!: " + e.Message);
                                    Console.WriteLine(e.StackTrace);
                                }
                            }

                            tdbReader.Close();
                        }

                        idxReader.Close();
                    }
                }
                else
                {
                    Console.WriteLine("Lost stable files not found!  Should be at " + Path.GetFullPath(MobileIndexPath));
                }

                Serial failedSerial = Serial.Zero;
                if (File.Exists(MobileDataPath))
                {
                    using (
                        FileStream bin = new FileStream(MobileDataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        BinaryFileReader reader = new BinaryFileReader(new BinaryReader(bin));

                        for (int i = 0; i < mobiles.Count; ++i)
                        {
                            MobileEntry entry = mobiles[i];
                            Mobile m = entry.Mobile;

                            if (m != null)
                            {
                                reader.Seek(entry.Position, SeekOrigin.Begin);

                                try
                                {
                                    if (!output.Contains(m))
                                    {
                                        // same mob already exist in the world, but
                                        // m has been assigned a new serial number so it's ok to delete it
                                    }
                                    else
                                    {
                                        m_LoadingType = entry.TypeName;
                                        m.Deserialize(reader);

                                        m.Map = Map.Internal;
                                        if (m is Beetle || m is HordeMinion || m is PackHorse || m is PackLlama)
                                        {
                                            // pack animals: make sure they have their pack

                                            Container pack = m.Backpack;
                                            if (pack == null)
                                            {
                                                pack = new StrongBackpack {Movable = false};

                                                m.AddItem(pack);
                                            }
                                        }
                                        if (reader.Position != (entry.Position + entry.Length))
                                        {
                                            throw new Exception(String.Format("***** Bad serialize on {0} *****",
                                                m.GetType()));
                                        }
                                    }
                                }
                                catch
                                {
                                    mobiles.RemoveAt(i);

                                    Type failedType = m.GetType();
                                    int failedTypeID = entry.TypeID;
                                    failedSerial = m.Serial;
                                    Console.WriteLine(failedType);
                                    Console.WriteLine(failedTypeID);
                                    Console.WriteLine(failedSerial);
                                    break;
                                }
                            }
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            return output;
        }
    }

    public class PetRestorationStone : Item
    {
        private static PetRestorationStone m_Instance;

        public static PetRestorationStone Instance
        {
            get { return m_Instance; }
        }

        private List<Mobile> MobList = new List<Mobile>();

        [CommandProperty(AccessLevel.GameMaster)]
        public bool LoadMobs
        {
            get { return false; }
            set
            {
                if (value)
                {
                    MobList = LostStabledPetRecorder.ReadMobiles();
                }
            }
        }

        [Constructable]
        public PetRestorationStone()
            : base(0xEDC)
        {
            Name =
                "Pet Restoration Stone - Double click and pets lost in the recent update will be brought here (NOT to your stable) and you will have control of them.";
            Movable = false;
            Visible = false;

            if (m_Instance != null)
            {
                // there can only be one WeaponDamageController game stone in the world
                m_Instance.Location = Location;
                CommandHandlers.BroadcastMessage(
                    AccessLevel.Administrator,
                    0x489,
                    "Existing PetRestorationStone has been moved to this location (DON'T DELETE IT!).");
                Timer.DelayCall(TimeSpan.FromSeconds(1), UpdateInstancePosition, this);
            }
            else
            {
                m_Instance = this;
            }
        }

        public static void UpdateInstancePosition(PetRestorationStone attemptedConstruct)
        {
            if (attemptedConstruct == null)
            {
                return;
            }
            if (m_Instance == null) // should never happen, but if it does, make this the instance
            {
                m_Instance = attemptedConstruct;
            }
            else if (attemptedConstruct.Location != new Point3D(0, 0, 0))
                // move the instance to it's location and delete it
            {
                m_Instance.Location = attemptedConstruct.Location;
                attemptedConstruct.Delete();
            }
        }

        public PetRestorationStone(Serial serial)
            : base(serial)
        {
        }

        public override void OnDelete()
        {
            if (this == m_Instance)
            {
                m_Instance = null;
            }

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); //version

            writer.WriteMobileList(MobList, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            MobList = reader.ReadStrongMobileList();
            // global attributes
            m_Instance = this;
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (pm == null)
            {
                return;
            }
            if (pm.Followers > 0)
            {
                pm.SendMessage("You must have 0 followers to use the pet recovery stone!");
                return;
            }
            Mobile foundOwnedMob = null;
            Mobile mobToDelete = null;
            foreach (Mobile mob in MobList)
            {
                BaseCreature bc = mob as BaseCreature;
                if (bc == null)
                {
                    Console.WriteLine("Mobile wasn't base creature??");
                    continue;
                }
                if (bc.LastOwner == pm)
                {
                    if (bc.Deleted)
                    {
                        pm.SendMessage("A mob was already deleted... name = " + bc.Name + " serial = " + bc.Serial);
                        /*LoggingCustom.Log("PetStoneClicked.txt",
                            "player: " + pm.Name + "\tA mob was already deleted... name = " + bc.Name + " serial = " +
                            bc.Serial);
                        mobToDelete = mob;*/
                        break;
                    }
                    pm.SendMessage(
                        "Found a mob: name = " + bc.Name + " serial = " + bc.Serial +
                        " ... attempting to return it to you.");
                    pm.SendMessage(
                        "A pet previously owned by you has been returned to you.  If you have trouble with stable spots, say 'claim' and try again!");
                    /*LoggingCustom.Log("PetStoneClicked.txt",
                        "player: " + pm.Name + "\tReturning a mob: name=" + bc.Name + " serial=" + bc.Serial + " type=" +
                        bc.GetType() +
                        " bonded=" + bc.IsBonded + " bondBegin=" + bc.BondingBegin);*/
                    bc.SetControlMaster(pm);

                    foundOwnedMob = mob;

                    bc.MoveToWorld(pm.Location, Map.Felucca);
                    break;
                }
            }
            if (mobToDelete != null)
            {
                mobToDelete.Delete();
            }
            else if (foundOwnedMob != null)
            {
                MobList.Remove(foundOwnedMob);
            }
            else
            {
                pm.SendMessage("Sorry, but you do not have any lost pets to recover!");
            }

            if (from.AccessLevel >= AccessLevel.Administrator)
            {
                from.SendGump(new PropertiesGump(from, this));
            }
        }
    }
}