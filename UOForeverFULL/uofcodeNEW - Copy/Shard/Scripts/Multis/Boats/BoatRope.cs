using System;
using System.Collections;
using Server;
using Server.Multis;

namespace Server.Items
{
    public class BoatRope : BoatComponent, ILockable
    {
        private bool m_Locked;
        private uint m_KeyValue;

        private Timer m_CloseTimer;

        public override void SetFacing(Direction dir)
        {
            if (Boat == null) { if (!this.Deleted) { Delete(); } }
            return;
        }

        public BoatRope(BaseBoat boat, Point3D offset, uint keyValue)
            : base(boat, offset, null) // direction doesn't matter
        {
            Boat = boat;
            m_KeyValue = keyValue;
            m_Locked = true;
            Offset = offset;
            
            ItemID = 0x14FA;
        }

        public BoatRope(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version

            writer.Write(m_Locked);
            writer.Write(m_KeyValue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Locked = reader.ReadBool();
                        m_KeyValue = reader.ReadUInt();

                        break;
                    }
            }

            if (!Deleted && IsOpen)
            {
                m_CloseTimer = new CloseTimer(this);
                m_CloseTimer.Start();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Locked 
        { 
            get { return m_Locked; } set { m_Locked = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public uint KeyValue { get { return m_KeyValue; } set { m_KeyValue = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsOpen { get { return (Hue == OpenHue); } }

        public const int OpenHue = 38;

        public void Open()
        {
            if (IsOpen || Deleted)
                return;

            if (m_CloseTimer != null)
                m_CloseTimer.Stop();

            m_CloseTimer = new CloseTimer(this);
            m_CloseTimer.Start();

            Hue = OpenHue;

            if (Boat != null)
                Boat.Refresh();
        }

        public override bool OnMoveOver(Mobile from)
        {
            if (IsOpen)
            {
                if ((from.Direction & Direction.Running) != 0 || (Boat != null && !Boat.Contains(from)))
                    return true;

                Map map = Map;

                if (map == null)
                    return false;

                int rx = 0, ry = 0;

                if (from.Direction == Direction.East)
                    rx = 1;
                else if (from.Direction == Direction.West)
                    rx = -1;
                else if (from.Direction == Direction.South)
                    ry = 1;
                else if (from.Direction == Direction.North)
                    ry = -1;

                int lowerTestBound = -8;
                int upperTestBound = 8;
                if (Boat is GargoyleBoat) { lowerTestBound -= 13; upperTestBound -= 13; }
                if (Boat is OrcBoat) { lowerTestBound -= 11; upperTestBound -= 11; }
                if (Boat is TokunoBoat) { lowerTestBound -= 4; upperTestBound -= 4; }

                for (int i = 1; i <= 7; ++i)
                {
                    int x = X + (i * rx);
                    int y = Y + (i * ry);
                    int z;

                    for (int j = lowerTestBound; j <= upperTestBound; ++j) //for (int j = -8; j <= 8; ++j)  // map it down some z levels in order to mimic the accessibility of regular boats (where you stand -13 z levels lower)
                    {
                        z = from.Z + j;

                        if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                        {
                            if (i == 1 && j >= -2 && j <= 2)
                                return true;

                            from.Location = new Point3D(x, y, z);
                            return false;
                        }
                    }

                    z = map.GetAverageZ(x, y);

                    if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                    {
                        if (i == 1)
                            return true;

                        from.Location = new Point3D(x, y, z);
                        return false;
                    }
                }

                return true;
            }
            return true;
        }

        public bool CanClose()
        {
            Map map = Map;

            if (map == null || Deleted)
                return false;

            if (Boat.Hits < BaseBoat.CriticalHits)
            {
                return false;
            }

            return true;
        }

        public void Close()
        {
            if (!IsOpen || !CanClose() || Deleted)
                return;

            if (m_CloseTimer != null)
                m_CloseTimer.Stop();

            m_CloseTimer = null;

            Hue = 0;

            if (Boat != null)
                Boat.Refresh();
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Boat == null)
                return;

            if (from.InRange(GetWorldLocation(), 8))
            {
                if (Boat.Contains(from))
                {
                    if (IsOpen)
                        Close();
                    else
                        Open();
                }
                else
                {
                    if (!IsOpen)
                    {
                        if (!Locked)
                        {
                            Open();
                        }
                        else if (from.AccessLevel >= AccessLevel.GameMaster)
                        {
                            from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502502); // That is locked but your godly powers allow access
                            Open();
                        }
                        else
                        {
                            from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502503); // That is locked.
                        }
                    }
                    else if (!Locked || Boat.Hits < BaseBoat.CriticalHits)
                    {
                        from.Location = new Point3D(this.X, this.Y, this.Z + 3);
                    }
                    else if (from.AccessLevel >= AccessLevel.GameMaster)
                    {
                        from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502502); // That is locked but your godly powers allow access
                        from.Location = new Point3D(this.X, this.Y, this.Z + 3);
                    }
                    else
                    {
                        from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502503); // That is locked.
                    }
                }
            }
        }

        private class CloseTimer : Timer
        {
            private BoatRope BoatRope;

            public CloseTimer(BoatRope boatRope)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                BoatRope = boatRope;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                BoatRope.Close();
            }
        }
    }
}