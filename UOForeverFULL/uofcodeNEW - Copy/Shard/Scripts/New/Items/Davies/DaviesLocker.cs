// **********
// UOForever - DaviesLocker.cs
// **********

#region References

using System;
using System.Collections;
using Server.Gumps;
using Server.Multis;
using Server.Network;

#endregion

namespace Server.Items
{
    public enum TableType
    {
        North,
        East
    }

    public class DaviesLocker : Item, IAddon, IChopable
    {
        private ArrayList m_Components;
        private Mobile m_Placer;
        public ArrayList m_Entries;
        public int m_DefaultIndex;


        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Placer
        {
            get { return m_Placer; }
            set { m_Placer = value; }
        }

        public ArrayList Entries
        {
            get { return m_Entries; }
        }

        public DaviesLockerEntry Default
        {
            get
            {
                if (m_DefaultIndex >= 0 && m_DefaultIndex < m_Entries.Count)
                    return (DaviesLockerEntry) m_Entries[m_DefaultIndex];

                return null;
            }
            set
            {
                if (value == null)
                    m_DefaultIndex = -1;
                else
                    m_DefaultIndex = m_Entries.IndexOf(value);
            }
        }

        private class LockerPiece : Item, IChopable
        {
            private DaviesLocker m_Table;

            public override int LabelNumber
            {
                get { return 1153534; }
            } // Davies Locker

            public LockerPiece(DaviesLocker table, int itemID) : base(itemID)
            {
                Movable = false;
                MoveToWorld(table.Location, table.Map);

                m_Table = table;
            }

            public LockerPiece(Serial serial) : base(serial)
            {
            }

            public override void OnDoubleClick(Mobile from)
            {
                if (m_Table != null)
                    m_Table.OnDoubleClick(from);
            }


            public void OnChop(Mobile from)
            {
                if (m_Table != null && !m_Table.Deleted)
                    m_Table.OnChop(from);
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write(0); // version

                writer.Write(m_Table);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch (version)
                {
                    case 0:
                    {
                        m_Table = reader.ReadItem() as DaviesLocker;

                        if (m_Table == null)
                            Delete();

                        break;
                    }
                }
            }
        }

        public override int LabelNumber
        {
            get { return 1153534; }
        } // Davies Locker

        public DaviesLocker(Mobile from, TableType type, Point3D loc) : base(1)
        {
            Movable = false;
            MoveToWorld(loc, from.Map);
            m_Entries = new ArrayList();
            m_DefaultIndex = -1;

            m_Placer = from;
            m_Components = new ArrayList();

            switch (type)
            {
                case TableType.East:
                {
                    ItemID = 0x4BF6;

                    AddItem(0, -1, 0, new LockerPiece(this, 0x4BF7));
                    AddItem(0, -2, 0, new LockerPiece(this, 0x4BF8));
                    AddItem(-1, -2, 0, new LockerPiece(this, 0x4BF9));
                    AddItem(-1, -1, 0, new LockerPiece(this, 0x4BFA));
                    AddItem(-1, 0, 0, new LockerPiece(this, 0x4BFB));

                    break;
                }
                case TableType.North:
                {
                    ItemID = 0x4BFE;

                    AddItem(-1, 0, 0, new LockerPiece(this, 0x4BFC));
                    AddItem(-2, 0, 0, new LockerPiece(this, 0x4BFD));
                    AddItem(-2, -1, 0, new LockerPiece(this, 0x4BFF));
                    AddItem(-1, -1, 0, new LockerPiece(this, 0x4C00));
                    AddItem(0, -1, 0, new LockerPiece(this, 0x4C01));

                    break;
                }
            }
        }

        public override void OnAfterDelete()
        {
            foreach (object t in m_Components)
                ((Item) t).Delete();
        }

        private void AddItem(int x, int y, int z, Item item)
        {
            item.MoveToWorld(new Point3D(Location.X + x, Location.Y + y, Location.Z + z), Map);

            m_Components.Add(item);
        }

        public DaviesLocker(Serial serial) : base(serial)
        {
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            return map.CanFit((Point3D) p, 20);
        }

        Item IAddon.Deed
        {
            get
            {
                DaviesLockerDeed deed = new DaviesLockerDeed();
                for (int i = 0; i < m_Entries.Count; ++i)
                {
                    //BountyBoardEntry entry = (BountyBoardEntry) BountyBoard.Entries[i];
                    DaviesLockerEntry e = (DaviesLockerEntry) Entries[i];
                    deed.m_Entries.Add(new DaviesDeedEntry(e.type, e.Level, e.Decoder, e.Map, e.Location2d, e.Location3d,
                        e.Bounds, e.Mapnumber));
                }


                return deed;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Placer);
            writer.Write(m_DefaultIndex);
            writer.Write(m_Entries.Count);

            foreach (object t in m_Entries)
            {
                ((DaviesLockerEntry) t).Serialize(writer);
            }

            writer.Write(m_Components.Count);

            foreach (object t in m_Components)
            {
                writer.Write((Item) t);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Placer = reader.ReadMobile();
            m_DefaultIndex = reader.ReadInt();
            int count2 = reader.ReadInt();
            m_Entries = new ArrayList(count2);
            for (int j = 0; j < count2; ++j)
            {
                m_Entries.Add(new DaviesLockerEntry(reader));
            }

            int count = reader.ReadInt();
            m_Components = new ArrayList(count);

            for (int i = 0; i < count; ++i)
            {
                Item item = reader.ReadItem();
                if (item != null)
                    m_Components.Add(item);
            }

            Timer.DelayCall(TimeSpan.Zero, ValidatePlacement);
        }

        public void ValidatePlacement()
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null)
            {
                DaviesLockerDeed deed = new DaviesLockerDeed();
                deed.MoveToWorld(Location, Map);
                Delete();
            }
        }

        public void DropTMap(Mobile from, DaviesLockerEntry e, int index)
        {
            if (m_DefaultIndex == index)
                m_DefaultIndex = -1;

            m_Entries.RemoveAt(index);

            if (e.type == 1)
            {
                TreasureMap tmap = new TreasureMap(e.Level, e.Map)
                {
                    Decoder = e.Decoder,
                    ChestLocation = e.Location2d,
                    ChestMap = e.Map,
                    Bounds = e.Bounds
                };

                tmap.ClearPins();
                tmap.AddWorldPin(e.Location2d.X, e.Location2d.Y);

                from.AddToBackpack(tmap);

                from.SendMessage("You have removed the Treasure Map");
            }
            else if (e.type == 2)
            {
                SOS sos = new SOS(e.Map, e.Level) {TargetLocation = e.Location3d};

                from.AddToBackpack(sos);
                from.SendMessage("You have removed the S.O.S.");
            }
            else
            {
                MessageInABottle mib = new MessageInABottle(e.Map, e.Level);

                from.AddToBackpack(mib);
                from.SendMessage("You have removed the message in a bottle");
            }
        }

        public void ViewMap(Mobile from, DaviesLockerEntry e, int index)
        {
            if (m_DefaultIndex == index)
                m_DefaultIndex = -1;

            from.CloseGump(typeof (DaviesMapGump));
            from.SendGump(new DaviesMapGump(e.Location2d.X, e.Location2d.Y));
        }

        public bool CheckAccess(Mobile m)
        {
            return true;
        }

        //private TMList[] m_Lists;

        public class TMEntry
        {
            private Point3D m_Location;
            private string m_Text;

            public Point3D Location
            {
                get { return m_Location; }
            }

            public string Text
            {
                get { return m_Text; }
            }

            public TMEntry(Point3D loc, string text)
            {
                m_Location = loc;
                m_Text = text;
            }
        }

        public class TMList
        {
            private TMEntry[] m_Entries;


            public TMEntry[] Entries
            {
                get { return m_Entries; }
            }

            public TMList(TMEntry[] entries)
            {
                m_Entries = entries;
            }

            public static readonly TMList MapLocations1 =
                new TMList(new[]
                {
                    new TMEntry(new Point3D(961, 506, 0), "Map 1"),
                    new TMEntry(new Point3D(1162, 189, 6), "Map 2"),
                    new TMEntry(new Point3D(1314, 317, 22), "Map 3"),
                    new TMEntry(new Point3D(1470, 229, 16), "Map 4"),
                    new TMEntry(new Point3D(1504, 366, 0), "Map 5"),
                    new TMEntry(new Point3D(2672, 392, 15), "Map 6"),
                    new TMEntry(new Point3D(2740, 435, 15), "Map 7"),
                    new TMEntry(new Point3D(2770, 345, 15), "Map 8"),
                    new TMEntry(new Point3D(2781, 289, 15), "Map 9"),
                    new TMEntry(new Point3D(2836, 233, 0), "Map 10"),
                    new TMEntry(new Point3D(3014, 250, 0), "Map 11"),
                    new TMEntry(new Point3D(3082, 202, 4), "Map 12"),
                    new TMEntry(new Point3D(1027, 1180, 0), "Map 13"),
                    new TMEntry(new Point3D(1319, 889, 0), "Map 14"),
                    new TMEntry(new Point3D(1414, 771, 0), "Map 15"),
                    new TMEntry(new Point3D(1529, 753, 16), "Map 16"),
                    new TMEntry(new Point3D(1555, 806, 0), "Map 17"),
                    new TMEntry(new Point3D(1511, 967, 0), "Map 18"),
                    new TMEntry(new Point3D(1562, 1058, 0), "Map 19"),
                    new TMEntry(new Point3D(1510, 1071, 0), "Map 20"),
                    new TMEntry(new Point3D(2340, 645, 0), "Map 21"),
                    new TMEntry(new Point3D(2350, 688, 0), "Map 22"),
                    new TMEntry(new Point3D(2395, 723, 0), "Map 23"),
                    new TMEntry(new Point3D(2433, 767, 0), "Map 24"),
                    new TMEntry(new Point3D(2643, 851, 0), "Map 25"),
                    new TMEntry(new Point3D(2458, 1042, 0), "Map 26"),
                    new TMEntry(new Point3D(2517, 1066, 0), "Map 27"),
                    new TMEntry(new Point3D(2337, 1159, 5), "Map 28"),
                    new TMEntry(new Point3D(2392, 1154, 0), "Map 29"),
                    new TMEntry(new Point3D(3246, 245, 4), "Map 30"),
                    new TMEntry(new Point3D(3404, 238, 0), "Map 31"),
                    new TMEntry(new Point3D(3375, 458, 9), "Map 32"),
                    new TMEntry(new Point3D(3369, 637, 5), "Map 33"),
                    new TMEntry(new Point3D(199, 1461, 0), "Map 34"),
                    new TMEntry(new Point3D(208, 1444, 0), "Map 35"),
                    new TMEntry(new Point3D(358, 1337, 0), "Map 36"),
                    new TMEntry(new Point3D(581, 1455, 0), "Map 37"),
                    new TMEntry(new Point3D(348, 1565, 0), "Map 38"),
                    new TMEntry(new Point3D(620, 1706, 0), "Map 39"),
                    new TMEntry(new Point3D(962, 1858, 0), "Map 40"),
                    new TMEntry(new Point3D(980, 1849, 0), "Map 41"),
                    new TMEntry(new Point3D(969, 1893, 0), "Map 42"),
                    new TMEntry(new Point3D(968, 1884, 0), "Map 43"),
                    new TMEntry(new Point3D(977, 1879, 0), "Map 44"),
                    new TMEntry(new Point3D(1018, 1859, 0), "Map 45"),
                    new TMEntry(new Point3D(1034, 1877, 0), "Map 46"),
                    new TMEntry(new Point3D(1042, 1903, 0), "Map 47"),
                    new TMEntry(new Point3D(1042, 1960, 0), "Map 48"),
                    new TMEntry(new Point3D(1037, 1975, 0), "Map 49"),
                    new TMEntry(new Point3D(1024, 1990, 0), "Map 50"),
                    new TMEntry(new Point3D(974, 1992, 0), "Map 51"),
                    new TMEntry(new Point3D(989, 1992, 0), "Map 52"),
                    new TMEntry(new Point3D(451, 2053, 0), "Map 53"),
                    new TMEntry(new Point3D(477, 2044, 8), "Map 54"),
                    new TMEntry(new Point3D(492, 2027, 3), "Map 55"),
                    new TMEntry(new Point3D(468, 2087, 8), "Map 56"),
                    new TMEntry(new Point3D(465, 2100, 5), "Map 57"),
                    new TMEntry(new Point3D(1658, 2030, 0), "Map 58"),
                    new TMEntry(new Point3D(1689, 1993, 0), "Map 59"),
                    new TMEntry(new Point3D(1709, 1964, 0), "Map 60"),
                    new TMEntry(new Point3D(1726, 1998, 0), "Map 61"),
                    new TMEntry(new Point3D(1731, 2016, 0), "Map 62"),
                    new TMEntry(new Point3D(1743, 2028, 0), "Map 63"),
                    new TMEntry(new Point3D(1754, 2020, 0), "Map 64"),
                    new TMEntry(new Point3D(2033, 1942, 0), "Map 65"),
                    new TMEntry(new Point3D(2054, 1962, 0), "Map 66"),
                    new TMEntry(new Point3D(2066, 1979, 0), "Map 67"),
                    new TMEntry(new Point3D(2058, 1992, 0), "Map 68"),
                    new TMEntry(new Point3D(2071, 2007, 0), "Map 69"),
                    new TMEntry(new Point3D(2062, 1962, 0), "Map 70"),
                    new TMEntry(new Point3D(2097, 1975, 0), "Map 71"),
                    new TMEntry(new Point3D(2089, 1987, 0), "Map 72"),
                    new TMEntry(new Point3D(2093, 2006, 0), "Map 73"),
                    new TMEntry(new Point3D(2187, 1991, 5), "Map 74"),
                    new TMEntry(new Point3D(1427, 2405, 5), "Map 75"),
                    new TMEntry(new Point3D(1433, 2381, 5), "Map 76"),
                    new TMEntry(new Point3D(1471, 2340, 5), "Map 77"),
                    new TMEntry(new Point3D(1450, 2301, 0), "Map 78"),
                    new TMEntry(new Point3D(1437, 2294, 0), "Map 79"),
                    new TMEntry(new Point3D(1439, 2217, 0), "Map 80"),
                    new TMEntry(new Point3D(1466, 2181, 0), "Map 81"),
                    new TMEntry(new Point3D(1464, 2245, 5), "Map 82"),
                    new TMEntry(new Point3D(1478, 2273, 5), "Map 83"),
                    new TMEntry(new Point3D(1562, 2312, 5), "Map 84"),
                    new TMEntry(new Point3D(1546, 2221, 10), "Map 85"),
                    new TMEntry(new Point3D(1519, 2214, 5), "Map 86"),
                    new TMEntry(new Point3D(1534, 2189, 5), "Map 87"),
                    new TMEntry(new Point3D(1523, 2150, 0), "Map 88"),
                    new TMEntry(new Point3D(1541, 2115, 5), "Map 89"),
                    new TMEntry(new Point3D(1595, 2193, 0), "Map 90"),
                    new TMEntry(new Point3D(1619, 2236, 0), "Map 91"),
                    new TMEntry(new Point3D(1654, 2268, 5), "Map 92"),
                    new TMEntry(new Point3D(1724, 2288, 5), "Map 93"),
                    new TMEntry(new Point3D(1772, 2321, 5), "Map 94"),
                    new TMEntry(new Point3D(1758, 2333, 5), "Map 95"),
                    new TMEntry(new Point3D(1765, 2430, 5), "Map 96"),
                    new TMEntry(new Point3D(1703, 2318, 0), "Map 97"),
                    new TMEntry(new Point3D(1655, 2304, 0), "Map 98"),
                    new TMEntry(new Point3D(2062, 2144, 0), "Map 99"),
                    new TMEntry(new Point3D(2104, 2123, 0), "Map 100"),
                    new TMEntry(new Point3D(2098, 2101, 0), "Map 101"),
                    new TMEntry(new Point3D(2130, 2108, 0), "Map 102"),
                    new TMEntry(new Point3D(2153, 2120, 0), "Map 103"),
                    new TMEntry(new Point3D(2186, 2144, 0), "Map 104"),
                    new TMEntry(new Point3D(2178, 2151, 0), "Map 105"),
                    new TMEntry(new Point3D(2162, 2148, 0), "Map 106"),
                    new TMEntry(new Point3D(2129, 2132, 0), "Map 107"),
                    new TMEntry(new Point3D(2123, 2120, 0), "Map 108"),
                    new TMEntry(new Point3D(2648, 2167, 5), "Map 109"),
                    new TMEntry(new Point3D(2629, 2221, 6), "Map 110"),
                    new TMEntry(new Point3D(2642, 2289, 7), "Map 111"),
                    new TMEntry(new Point3D(2682, 2290, 5), "Map 112"),
                    new TMEntry(new Point3D(2727, 2309, 0), "Map 113"),
                    new TMEntry(new Point3D(2782, 2293, 6), "Map 114"),
                    new TMEntry(new Point3D(2804, 2255, 0), "Map 115"),
                    new TMEntry(new Point3D(2850, 2252, 5), "Map 116"),
                    new TMEntry(new Point3D(2957, 2150, 53), "Map 117"),
                    new TMEntry(new Point3D(2968, 2170, 36), "Map 118"),
                    new TMEntry(new Point3D(2951, 2177, 52), "Map 119"),
                    new TMEntry(new Point3D(2956, 2200, 46), "Map 120"),
                    new TMEntry(new Point3D(2932, 2240, 5), "Map 121"),
                    new TMEntry(new Point3D(958, 2504, 0), "Map 122"),
                    new TMEntry(new Point3D(1025, 2702, 0), "Map 123"),
                    new TMEntry(new Point3D(1290, 2735, 0), "Map 124"),
                    new TMEntry(new Point3D(1383, 2840, 0), "Map 125"),
                    new TMEntry(new Point3D(1388, 2984, 0), "Map 126"),
                    new TMEntry(new Point3D(1415, 3059, 0), "Map 127"),
                    new TMEntry(new Point3D(1647, 2641, 5), "Map 128"),
                    new TMEntry(new Point3D(1562, 2705, 0), "Map 129"),
                    new TMEntry(new Point3D(1670, 2808, 0), "Map 130"),
                    new TMEntry(new Point3D(1602, 3012, 0), "Map 131"),
                    new TMEntry(new Point3D(1664, 3062, 0), "Map 132"),
                    new TMEntry(new Point3D(1068, 3182, 0), "Map 133"),
                    new TMEntry(new Point3D(1076, 3156, 0), "Map 134"),
                    new TMEntry(new Point3D(1073, 3133, 0), "Map 135"),
                    new TMEntry(new Point3D(1090, 3110, 0), "Map 136"),
                    new TMEntry(new Point3D(1094, 3131, 0), "Map 137"),
                    new TMEntry(new Point3D(1096, 3178, 0), "Map 138"),
                    new TMEntry(new Point3D(1129, 3403, 0), "Map 139"),
                    new TMEntry(new Point3D(1162, 3469, 0), "Map 140"),
                    new TMEntry(new Point3D(1128, 3500, 0), "Map 141"),
                    new TMEntry(new Point3D(1135, 3445, 0), "Map 142"),
                    new TMEntry(new Point3D(2013, 3269, 0), "Map 143"),
                    new TMEntry(new Point3D(2039, 3427, 0), "Map 144"),
                    new TMEntry(new Point3D(2097, 3384, 0), "Map 145"),
                    new TMEntry(new Point3D(2149, 3362, 10), "Map 146"),
                    new TMEntry(new Point3D(2370, 3428, 3), "Map 147"),
                    new TMEntry(new Point3D(2341, 3482, 3), "Map 148"),
                    new TMEntry(new Point3D(2360, 3507, 3), "Map 149"),
                    new TMEntry(new Point3D(2387, 3505, 3), "Map 150"),
                    new TMEntry(new Point3D(2467, 3581, 3), "Map 151"),
                    new TMEntry(new Point3D(2482, 3624, 3), "Map 152"),
                    new TMEntry(new Point3D(2527, 3585, 0), "Map 153"),
                    new TMEntry(new Point3D(2535, 3608, 0), "Map 154"),
                    new TMEntry(new Point3D(2797, 3452, 0), "Map 155"),
                    new TMEntry(new Point3D(2803, 3488, 0), "Map 156"),
                    new TMEntry(new Point3D(2793, 3519, 0), "Map 157"),
                    new TMEntry(new Point3D(2831, 3511, 0), "Map 158"),
                    new TMEntry(new Point3D(2989, 3606, 15), "Map 159"),
                    new TMEntry(new Point3D(3035, 3603, 0), "Map 160"),
                    new TMEntry(new Point3D(2154, 3983, 3), "Map 161"),
                    new TMEntry(new Point3D(2143, 3986, 0), "Map 162"),
                    new TMEntry(new Point3D(2140, 3940, 3), "Map 163"),
                    new TMEntry(new Point3D(2157, 3924, 3), "Map 164"),
                    new TMEntry(new Point3D(2152, 3950, 3), "Map 165"),
                    new TMEntry(new Point3D(2162, 3988, 3), "Map 166"),
                    new TMEntry(new Point3D(2453, 3942, 0), "Map 167"),
                    new TMEntry(new Point3D(2422, 3928, 3), "Map 168"),
                    new TMEntry(new Point3D(2415, 3920, 3), "Map 169"),
                    new TMEntry(new Point3D(2421, 3902, 0), "Map 170"),
                    new TMEntry(new Point3D(2480, 3908, 6), "Map 171"),
                    new TMEntry(new Point3D(2513, 3901, 3), "Map 172"),
                    new TMEntry(new Point3D(2512, 3918, 0), "Map 173"),
                    new TMEntry(new Point3D(2513, 3962, 6), "Map 174"),
                    new TMEntry(new Point3D(2528, 3982, 0), "Map 175"),
                    new TMEntry(new Point3D(2516, 3999, 3), "Map 176"),
                    new TMEntry(new Point3D(4477, 3282, 0), "Map 177"),
                    new TMEntry(new Point3D(4477, 3230, 0), "Map 178"),
                    new TMEntry(new Point3D(4466, 3209, 0), "Map 179"),
                    new TMEntry(new Point3D(4424, 3152, 0), "Map 180"),
                    new TMEntry(new Point3D(4419, 3117, 0), "Map 181"),
                    new TMEntry(new Point3D(4448, 3130, 0), "Map 182"),
                    new TMEntry(new Point3D(4453, 3148, 0), "Map 183"),
                    new TMEntry(new Point3D(4500, 3108, 0), "Map 184"),
                    new TMEntry(new Point3D(4513, 3103, 0), "Map 185"),
                    new TMEntry(new Point3D(4471, 3188, 0), "Map 186"),
                    new TMEntry(new Point3D(4507, 3227, 0), "Map 187"),
                    new TMEntry(new Point3D(4496, 3241, 0), "Map 188"),
                    new TMEntry(new Point3D(4642, 3369, 0), "Map 189"),
                    new TMEntry(new Point3D(4694, 3485, 0), "Map 190"),
                    new TMEntry(new Point3D(3476, 2761, 31), "Map 191"),
                    new TMEntry(new Point3D(3425, 2723, 49), "Map 192"),
                    new TMEntry(new Point3D(3418, 2675, 50), "Map 193"),
                    new TMEntry(new Point3D(3533, 2471, 10), "Map 194"),
                    new TMEntry(new Point3D(3511, 2421, 48), "Map 195"),
                    new TMEntry(new Point3D(3568, 2402, 11), "Map 196"),
                    new TMEntry(new Point3D(3702, 2825, 21), "Map 197"),
                    new TMEntry(new Point3D(3594, 2825, 44), "Map 198"),
                    new TMEntry(new Point3D(3557, 2820, 24), "Map 199"),
                    new TMEntry(new Point3D(3541, 2784, 6), "Map 200")
                });

            public static readonly TMList[] UORLists = {MapLocations1};
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is TreasureMap)
            {
                if (m_Entries.Count < 500)
                {
                    TreasureMap tmap = (TreasureMap) dropped;
                    if (tmap.Completed)
                    {
                        from.SendMessage("This map is completed and can not be stored in this book");
                        InvalidateProperties();
                        dropped.Delete();
                        return false;
                    }
                    if (tmap.ChestMap != null)
                    {
                        bool matched = false;

                        Point2D p2d = tmap.ChestLocation;
                        Point3D p3d = Point3D.Zero;
                        string mapnumber = "B";


                        Console.WriteLine("Searching for TMap point matching {0}", p2d);


                        foreach (TMList list in TMList.UORLists)
                        {
                            foreach (TMEntry entry in list.Entries)
                            {
                                p3d = entry.Location;
                                mapnumber = entry.Text;

                                if (p2d.X == p3d.X && p2d.Y == p3d.Y)
                                {
                                    matched = true;
                                    //Setting of Map #

                                    break; // Escape from TMEntry loop.
                                }
                            }

                            if (matched)
                            {
                                break; // Escape from TMList loop.
                            }
                        }

                        if (matched)
                        {
                            Console.WriteLine("Matched TMap point: {0} - {1}", p3d, mapnumber);
                        }
                        else
                        {
                            Console.WriteLine("Unable to find valid spot @ {0}", p3d);
                            mapnumber = "Unknown Spot";
                        }


                        Point3D p3D = new Point3D(0, 0, 0);
                        m_Entries.Add(new DaviesLockerEntry(1, tmap.Level, tmap.Decoder, tmap.ChestMap,
                            tmap.ChestLocation, p3D, tmap.Bounds, mapnumber));
                        InvalidateProperties();
                        dropped.Delete();
                        from.Send(new PlaySound(0x42, GetWorldLocation()));

                        from.CloseGump(typeof (DaviesLockerGump));
                        from.SendGump(new DaviesLockerGump(this));

                        return true;
                    }
                    @from.SendMessage("This map is invalid");
                }
                else
                {
                    from.SendMessage("This TMap Book is full");
                }
            }

            else if (dropped is SOS)
            {
                if (m_Entries.Count < 500)
                {
                    SOS sos = (SOS) dropped;
                    Point2D p2D = new Point2D(0, 0);
                    m_Entries.Add(new DaviesLockerEntry(2, sos.Level, null, sos.TargetMap, p2D, sos.TargetLocation,
                        new Rectangle2D(0, 0, 0, 0), "Blank"));
                    InvalidateProperties();
                    dropped.Delete();
                    from.Send(new PlaySound(0x42, GetWorldLocation()));

                    from.CloseGump(typeof (DaviesLockerGump));
                    from.SendGump(new DaviesLockerGump(this));

                    return true;
                }
                @from.SendMessage("This TMap Book is full");
            }
            else if (dropped is MessageInABottle)
            {
                if (m_Entries.Count < 500)
                {
                    MessageInABottle bottle = (MessageInABottle) dropped;
                    Point2D p2D = new Point2D(0, 0);
                    Point3D p3D = new Point3D(0, 0, 0);
                    m_Entries.Add(new DaviesLockerEntry(3, bottle.Level, null, bottle.TargetMap, p2D, p3D,
                        new Rectangle2D(0, 0, 0, 0), "Blank"));
                    InvalidateProperties();
                    dropped.Delete();
                    from.Send(new PlaySound(0x42, GetWorldLocation()));

                    from.CloseGump(typeof (DaviesLockerGump));
                    from.SendGump(new DaviesLockerGump(this));

                    return true;
                }
                @from.SendMessage("This TMap Book is full");
            }


            return false;
        }

        public void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsCoOwner(from))
            {
                DaviesLockerDeed deed = new DaviesLockerDeed();


                for (int i = 0; i < m_Entries.Count; ++i)
                {
                    DaviesLockerEntry e = (DaviesLockerEntry) Entries[i];
                    deed.m_Entries.Add(new DaviesDeedEntry(e.type, e.Level, e.Decoder, e.Map, e.Location2d, e.Location3d,
                        e.Bounds, e.Mapnumber));
                }

                from.AddToBackpack(deed);


                Delete();

                BaseHouse house2 = BaseHouse.FindHouseAt(this);

                if (house2 != null && house2.Addons.Contains(this))
                {
                    house2.Addons.Remove(this);
                }
                from.SendMessage("Davies Locker Deed has been placed in your backpack");
            }
        }


        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsOwner(from) && house.Addons.Contains(this))
            {
                if (from.InRange(GetWorldLocation(), 3))
                {
                    from.CloseGump(typeof (DaviesLockerGump));
                    from.SendGump(new DaviesLockerGump(this));
                }
                else
                {
                    from.SendMessage("You are too far away");
                }
            }
            else
                from.SendMessage("You dont not have access to this");
        }
    }

    public class DaviesLockerEntry
    {
        private int m_type;
        private string m_Mapnumber;
        private Rectangle2D m_Bounds;
        private int m_Level;
        private Mobile m_Decoder;
        private Map m_Map;
        private Point2D m_Location2d;
        private Point3D m_Location3d;

        public int type
        {
            get { return m_type; }
        }

        public string Mapnumber
        {
            get { return m_Mapnumber; }
        }

        public int Level
        {
            get { return m_Level; }
        }

        public Mobile Decoder
        {
            get { return m_Decoder; }
        }

        public Map Map
        {
            get { return m_Map; }
        }

        public Rectangle2D Bounds
        {
            get { return m_Bounds; }
        }

        public Point2D Location2d
        {
            get { return m_Location2d; }
        }

        public Point3D Location3d
        {
            get { return m_Location3d; }
        }


        public DaviesLockerEntry(int type, int level, Mobile decoder, Map map, Point2D loc2d, Point3D loc3d,
            Rectangle2D bounds, string number)
        {
            m_type = type;
            m_Level = level;
            m_Decoder = decoder;
            m_Map = map;
            m_Location2d = loc2d;
            m_Location3d = loc3d;
            m_Bounds = bounds;
            m_Mapnumber = number;
        }

        public DaviesLockerEntry(GenericReader reader)
        {
            int version = reader.ReadByte();
            m_type = reader.ReadInt();
            m_Level = reader.ReadInt();
            m_Decoder = reader.ReadMobile();
            m_Map = reader.ReadMap();
            m_Location2d = reader.ReadPoint2D();
            m_Location3d = reader.ReadPoint3D();
            m_Bounds = reader.ReadRect2D();
            m_Mapnumber = reader.ReadString();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((byte) 0); // version
            writer.Write(m_type);
            writer.Write(m_Level);
            writer.Write(m_Decoder);
            writer.Write(m_Map);
            writer.Write(m_Location2d);
            writer.Write(m_Location3d);
            writer.Write(m_Bounds);
            writer.Write(m_Mapnumber);
        }
    }
}