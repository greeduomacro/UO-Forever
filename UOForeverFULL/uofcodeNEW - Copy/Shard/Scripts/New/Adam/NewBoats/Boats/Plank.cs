using System;
using Server.Multis;

namespace Server.Items
{
    public class NewPlank : BaseShipItem, ILockable, IFacingChange
    {
        private PlankSide m_Side;
        private bool m_Locked;
        private uint m_KeyValue;
        private Timer m_CloseTimer;

        #region Properties
        [CommandProperty(AccessLevel.GameMaster)]
        public PlankSide Side { get { return m_Side; } set { m_Side = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Locked { get { return m_Locked; } set { m_Locked = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public uint KeyValue { get { return m_KeyValue; } set { m_KeyValue = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsOpen { get { return (ItemID == 0x3ED5 || ItemID == 0x3ED4 || ItemID == 0x3E84 || ItemID == 0x3E89); } }

        private bool Starboard { get { return (m_Side == PlankSide.Starboard); } }
        #endregion

        public NewPlank(NewBaseBoat boat, Point3D initOffset, PlankSide side, uint keyValue)
            : base(boat, 0x3EB1 + (int)side, initOffset)
        {
            m_Side = side;
            m_KeyValue = keyValue;
            m_Locked = true;
        }

        public NewPlank(Serial serial)
            : base(serial)
        {
        }

        public void SetFacing(Direction oldFacing, Direction newFacing)
        {
            if (IsOpen)
            {
                switch (newFacing)
                {
                    case Direction.North: SetItemIDOnSmooth(Starboard ? 0x3ED4 : 0x3ED5); break;
                    case Direction.East: SetItemIDOnSmooth(Starboard ? 0x3E84 : 0x3E89); break;
                    case Direction.South: SetItemIDOnSmooth(Starboard ? 0x3ED5 : 0x3ED4); break;
                    case Direction.West: SetItemIDOnSmooth(Starboard ? 0x3E89 : 0x3E84); break;
                }
            }
            else
            {
                switch (newFacing)
                {
                    case Direction.North: SetItemIDOnSmooth(Starboard ? 0x3EB2 : 0x3EB1); break;
                    case Direction.East: SetItemIDOnSmooth(Starboard ? 0x3E85 : 0x3E8A); break;
                    case Direction.South: SetItemIDOnSmooth(Starboard ? 0x3EB1 : 0x3EB2); break;
                    case Direction.West: SetItemIDOnSmooth(Starboard ? 0x3E8A : 0x3E85); break;
                }
            }
        }

        public void Open()
        {
            if (IsOpen || Deleted)
                return;

            if (m_CloseTimer != null)
                m_CloseTimer.Stop();

            m_CloseTimer = new CloseTimer(this);
            m_CloseTimer.Start();

            switch (ItemID)
            {
                case 0x3EB1: ItemID = 0x3ED5; break;
                case 0x3E8A: ItemID = 0x3E89; break;
                case 0x3EB2: ItemID = 0x3ED4; break;
                case 0x3E85: ItemID = 0x3E84; break;
            }
        }

        public override bool OnMoveOver(Mobile from)
        {
            if (IsOpen)
            {
                if ((from.Direction & Direction.Running) != 0 || (Transport != null && !Transport.IsOnBoard(from)))
                {
                    Transport.Embark(from);
                    return true;
                }

                Map map = Map;

                if (map == null)
                    return false;

                int rx = 0, ry = 0;

                if (ItemID == 0x3ED4)
                    rx = 1;
                else if (ItemID == 0x3ED5)
                    rx = -1;
                else if (ItemID == 0x3E84)
                    ry = 1;
                else if (ItemID == 0x3E89)
                    ry = -1;

                for (int i = 1; i <= 6; ++i)
                {
                    int x = X + (i * rx);
                    int y = Y + (i * ry);
                    int z;

                    for (int j = -8; j <= 8; ++j)
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
            else
            {
                return false;
            }
        }

        public bool CanClose()
        {
            Map map = Map;

            if (map == null || Deleted)
                return false;

            foreach (object o in this.GetObjectsInRange(0))
            {
                if (o != this)
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

            switch (ItemID)
            {
                case 0x3ED5: ItemID = 0x3EB1; break;
                case 0x3E89: ItemID = 0x3E8A; break;
                case 0x3ED4: ItemID = 0x3EB2; break;
                case 0x3E84: ItemID = 0x3E85; break;
            }
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Transport == null)
                return;

            if (from.InRange(GetWorldLocation(), 8))
            {
                if (Transport.IsOnBoard(from))
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
                            Open();
                        else if (from.AccessLevel >= AccessLevel.GameMaster)
                        {
                            from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502502); // That is locked but your godly powers allow access
                            Open();
                        }
                        else
                            from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502503); // That is locked.
                    }
                    else if (!Locked)
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

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version

            writer.Write((int)m_Side);
            writer.Write(m_Locked);
            writer.Write(m_KeyValue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Side = (PlankSide)reader.ReadInt();
            m_Locked = reader.ReadBool();
            m_KeyValue = reader.ReadUInt();

            if (IsOpen)
            {
                m_CloseTimer = new CloseTimer(this);
                m_CloseTimer.Start();
            }
        }
        #endregion

        private class CloseTimer : Timer
        {
            private NewPlank m_Plank;

            public CloseTimer(NewPlank plank)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                m_Plank = plank;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Plank.Close();
            }
        }
    }
}