// **********
// UOForever - DaviesLockerDeed.cs
// **********

#region References

using System.Collections;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

#endregion

namespace Server.Items
{
    public class DaviesLockerDeed : Item
    {
        public ArrayList m_Entries;
        public int m_DefaultIndex;

        public override int LabelNumber
        {
            get { return 1153535; }
        } // a deed to davies locker

        [Constructable]
        public DaviesLockerDeed() : base(0x14F0)
        {
            Hue = 0x466;
            Weight = 1.0;
            LootType = LootType.Blessed;
            m_Entries = new ArrayList();
            m_DefaultIndex = -1;
        }

        public ArrayList Entries
        {
            get { return m_Entries; }
        }

        public DaviesDeedEntry Default
        {
            get
            {
                if (m_DefaultIndex >= 0 && m_DefaultIndex < m_Entries.Count)
                    return (DaviesDeedEntry) m_Entries[m_DefaultIndex];

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

        public DaviesLockerDeed(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Maps:  {0}", m_Entries.Count);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Entries.Count);

            foreach (object t in m_Entries)
                ((DaviesDeedEntry) t).Serialize(writer);
            writer.Write(m_DefaultIndex);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            LootType = LootType.Blessed;

            int version = reader.ReadInt();

            int count = reader.ReadInt();

            m_Entries = new ArrayList(count);

            for (int i = 0; i < count; ++i)
                m_Entries.Add(new DaviesDeedEntry(reader));

            m_DefaultIndex = reader.ReadInt();
        }

        public bool ValidatePlacement(Mobile from, Point3D loc, TableType type)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return false;
            }

            Map map = from.Map;

            if (map == null)
                return false;

            BaseHouse house = BaseHouse.FindHouseAt(loc, map, 20);

            if (house == null || !house.IsFriend(from))
            {
                from.SendLocalizedMessage(500269); // You cannot build that there.
                return false;
            }

            if (type == TableType.East)
            {
                int x = loc.X - 1;
                int y = loc.Y - 1;
                int y2 = loc.Y - 2;
                int z = loc.Z;
                if (!map.CanFit(x, y, z, 20, true, false, false))
                {
                    from.SendLocalizedMessage(500269); // You cannot build that there.
                    return false;
                }
                if (!map.CanFit(x, y2, z, 20, true, false, false))
                {
                    @from.SendLocalizedMessage(500269); // You cannot build that there.
                    return false;
                }
            }
            else
            {
                int x = loc.X - 1;
                int x2 = loc.X - 2;
                int y = loc.Y - 1;
                int z = loc.Z;
                if (!map.CanFit(x, y, z, 20, true, false, false))
                {
                    from.SendLocalizedMessage(500269); // You cannot build that there.
                    return false;
                }
                if (!map.CanFit(x2, y, z, 20, true, false, false))
                {
                    @from.SendLocalizedMessage(500269); // You cannot build that there.
                    return false;
                }
            }

            return true;
        }

        public void BeginPlace(Mobile from, TableType type)
        {
            from.BeginTarget(-1, true, TargetFlags.None, new TargetStateCallback(Placement_OnTarget), type);
        }

        public void Placement_OnTarget(Mobile from, object targeted, object state)
        {
            IPoint3D p = targeted as IPoint3D;

            if (p == null)
                return;

            Point3D loc = new Point3D(p);

            if (p is StaticTarget)
                loc.Z -= TileData.ItemTable[((StaticTarget) p).ItemID].CalcHeight;
            if (ValidatePlacement(from, loc, (TableType) state))
                EndPlace(from, (TableType) state, loc);
        }

        public void EndPlace(Mobile from, TableType type, Point3D loc)
        {
            DaviesLocker table = new DaviesLocker(from, type, loc);
            for (int i = 0; i < m_Entries.Count; ++i)
            {
                //BountyBoardEntry entry = (BountyBoardEntry) BountyBoard.Entries[i];
                DaviesDeedEntry e = (DaviesDeedEntry) Entries[i];
                table.m_Entries.Add(new DaviesLockerEntry(e.type, e.Level, e.Decoder, e.Map, e.Location2d, e.Location3d,
                    e.Bounds, e.Mapnumber));
            }
            Delete();
            BaseHouse house = BaseHouse.FindHouseAt(table);
            if (house != null)
                house.Addons.Add(table);
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.CloseGump(typeof (DaviesLockerTableChoiceGump));
            from.SendGump(new DaviesLockerTableChoiceGump(from, this));
        }
    }

    public class DaviesLockerTableChoiceGump : Gump
    {
        private Mobile m_From;
        private DaviesLockerDeed m_Deed;

        public DaviesLockerTableChoiceGump(Mobile from, DaviesLockerDeed deed) : base(200, 200)
        {
            m_From = from;
            m_Deed = deed;

            AddPage(0);

            AddBackground(0, 0, 220, 120, 5054);
            AddBackground(10, 10, 200, 100, 3000);

            AddButton(20, 35, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(55, 35, 145, @"Facing East"); // East

            AddButton(20, 65, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(55, 65, 145, @"Facing North"); // North
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Deed.Deleted)
                return;

            switch (info.ButtonID)
            {
                case 1:
                {
                    m_Deed.BeginPlace(m_From, TableType.East);
                    break;
                }
                case 2:
                {
                    m_Deed.BeginPlace(m_From, TableType.North);
                    break;
                }
            }
        }
    }

    public class DaviesDeedEntry
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


        public DaviesDeedEntry(int type, int level, Mobile decoder, Map map, Point2D loc2d, Point3D loc3d,
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

        public DaviesDeedEntry(GenericReader reader)
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