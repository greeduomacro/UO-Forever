using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Concurrent;
using Server;
using Server.Network;
using Server.Mobiles;


namespace Server.Items
{
    public enum SpeedCode
    {
        Stop = 0x0,
        One = 0x1,
        Slow = 0x2,
        Medium = 0x3,
        Fast = 0x4,
        FastForward = 0x5
    }

    public enum TurnCode
    {
        Left = -2,
        Right = 2,
        Around = -4,
    }

    public interface IFacingChange
    {
        void SetFacing(Direction oldFacing, Direction newFacing);
    }

    public interface IShareHue
    {
        bool ShareHue { get; }
    }

    public interface ICrew
    {
        void Delete();
    }

    public abstract class BaseSmoothMulti : BaseMulti, IMount
    {
        #region Statics Fields
        private static List<BaseSmoothMulti> m_Instances = new List<BaseSmoothMulti>();
        private static Dictionary<SpeedCode, TimeSpan> m_TimespanDictionary;

        static BaseSmoothMulti()
        {
            m_TimespanDictionary = new Dictionary<SpeedCode, TimeSpan>();
            m_TimespanDictionary.Add(SpeedCode.One, TimeSpan.FromSeconds(0.50));
            m_TimespanDictionary.Add(SpeedCode.Fast, TimeSpan.FromSeconds(0.25));
            m_TimespanDictionary.Add(SpeedCode.FastForward, TimeSpan.FromSeconds(0.125));
            m_TimespanDictionary.Add(SpeedCode.Medium, TimeSpan.FromSeconds(0.50));
            m_TimespanDictionary.Add(SpeedCode.Slow, TimeSpan.FromSeconds(1));
        }
        #endregion

        private Direction m_Facing;
        private Direction m_Moving;
        private SpeedCode m_Speed;
        private DynamicComponentList m_ContainedObjects;
        //private virtual List<Item> m_StaticSurfaces { get; }

        private Mobile m_Pilot;
        private SmoothMultiMountItem m_VirtualMount;

        private TurnTimer m_TurnTimer; // timer for changing facing
        private MoveTimer m_MoveTimer; // timer for movement animation

        private Packet m_ContainerPacket;

        #region Properties      		
        [CommandProperty(AccessLevel.GameMaster)]
        public static List<BaseSmoothMulti> Instances { get { return m_Instances; } }					

        [CommandProperty(AccessLevel.GameMaster)]
        public TurnTimer CurrentTurnTimer { get { return m_TurnTimer; } }

		[CommandProperty(AccessLevel.GameMaster)]
        public MoveTimer CurrentMoveTimer { get { return m_MoveTimer; } }		
		
        [CommandProperty(AccessLevel.GameMaster)]
        public SpeedCode CurrentSpeed { get { return m_Speed; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing { get { return m_Facing; } set { SetFacing(value); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Moving { get { return m_Moving; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsMoving { get { return m_Speed != SpeedCode.Stop; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsDriven { get { return m_Pilot != null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Pilot { get { return m_Pilot; } set { SetPilot(value); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set
            {
                if (base.Hue == value)
                    return;

                base.Hue = value;
                m_ContainedObjects.ForEachItem(item =>
                {
                    if (item is IShareHue && ((IShareHue)item).ShareHue)
                        item.Hue = value;
                });
            }
        }

        public override Packet WorldPacketHS
        {
            get
            {
                if (m_ContainerPacket == null)
                {
                    m_ContainerPacket = new Packet.ContainerMultiList(this, m_ContainedObjects);
                    m_ContainerPacket.SetStatic();
                }

                return m_ContainerPacket;
            }
        }

        public Mobile Rider { get { return Pilot; } set { Pilot = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
        public DynamicComponentList ContainedObjects { get { return m_ContainedObjects; } }
        #endregion

        [Constructable]
        protected BaseSmoothMulti(int itemID)
            : base(itemID)
        {
            m_Facing = Direction.North;
            m_Moving = Direction.North;
            m_Speed = SpeedCode.Stop;
            m_ContainedObjects = new DynamicComponentList();
            m_VirtualMount = new SmoothMultiMountItem(this);
            m_Instances.Add(this);
        }

        public BaseSmoothMulti(Serial serial)
            : base(serial)
        {
        }

        #region Movement
        protected virtual bool BeginMove(Direction dir, SpeedCode speed)
        {
            if (speed == SpeedCode.Stop)
                return false;

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            m_Moving = dir;
            m_Speed = speed;

            m_MoveTimer = new MoveTimer(this, m_TimespanDictionary[speed], speed);
            m_MoveTimer.Start();

            return true;
        }

        private bool Move(Direction dir, SpeedCode speed)
        {
            Map map = Map;

            if (map == null || Deleted)
                return false;

            int rx = 0, ry = 0;
            Movement.Movement.Offset(dir, ref rx, ref ry);

            Point3D newLocation = new Point3D(X + rx, Y + ry, Z);
            if (!CanFit(newLocation, Map, ItemID))
                return false;

            SetLocationOnSmooth(newLocation);
            return true;
        }

        protected virtual bool EndMove()
        {
            if (m_Speed == SpeedCode.Stop)
                return false;

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            m_Speed = SpeedCode.Stop;
            return true;
        }

        public override void SetLocationOnSmooth(Point3D newLocation)
        {
            Point3D oldLocation = Location;
            base.SetLocationOnSmooth(newLocation);

            int rx = newLocation.X - oldLocation.X;
            int ry = newLocation.Y - oldLocation.Y;

            m_ContainedObjects.ForEachObject(
                item => item.SetLocationOnSmooth(new Point3D(item.X + rx, item.Y + ry, item.Z)),
                mob => mob.SetLocationOnSmooth(new Point3D(mob.X + rx, mob.Y + ry, mob.Z)));

            Packet.SmoothMovement movementPacket = new Packet.SmoothMovement(this, m_ContainedObjects);
            movementPacket.SetStatic();

            IPooledEnumerable eable = Map.GetClientsInRange(Location, GetMaxUpdateRange());
            foreach (NetState state in eable)
            {
                Mobile m = state.Mobile;

                if (!m.CanSee(this))
                    continue;

                if (m.InRange(Location, GetUpdateRange(m)))
                {
                    state.Send(movementPacket);

                    if (!m.InRange(oldLocation, GetUpdateRange(m)))
                        SendInfoTo(state);
                }

                if (Utility.InUpdateRange(Location, m.Location) && !Utility.InUpdateRange(oldLocation, m.Location))
                    SendInfoTo(state);
            }
            eable.Free();
            movementPacket.Release();

            m_ContainedObjects.ForEachMobile(mob => mob.NotifyLocationChangeOnSmooth(new Point3D(mob.Location.X - rx, mob.Location.Y - ry, mob.Location.Z)));
        }
        #endregion


        #region Turn
        protected bool BeginTurn(TurnCode turnDir)
        {
            return BeginTurn((Direction)(((int)m_Facing + (int)turnDir) & 0x7));
        }

        protected virtual bool BeginTurn(Direction newDirection)
        {
            if (m_TurnTimer != null)
                m_TurnTimer.Stop();

            m_TurnTimer = new TurnTimer(this, newDirection);
            m_TurnTimer.Start();

            return true;
        }

        private bool Turn(Direction newDirection)
        {
            if (m_TurnTimer != null)
            {
                m_TurnTimer.Stop();
                m_TurnTimer = null;
            }

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            Direction oldFacing = m_Facing;
            if (SetFacing((int)newDirection % 2 != 0 ? newDirection - 1 : newDirection))
            {
                if (m_Speed != SpeedCode.Stop && m_MoveTimer != null)
                {
                    m_Moving = IsDriven ? newDirection : (Direction)((int)m_Moving + (m_Facing - oldFacing)) & Server.Direction.Mask;
                    m_MoveTimer.Start();
                }
                return true;
            }

            if (m_Speed != SpeedCode.Stop && m_MoveTimer != null) // if boat can't turn, restart movement if it was on moving
                m_MoveTimer.Start();

            return false;
        }

        protected abstract int GetMultiId(Direction newFacing);

        private bool SetFacing(Direction facing)
        {
            if (Parent != null || this.Map == null || m_Facing == facing)
                return false;

            int? NewFacingItemID = GetMultiId(facing);

            // check if the multi can fit in new Location
            if (!NewFacingItemID.HasValue || !CanFit(Location, Map, NewFacingItemID.Value))
                return false;

            this.Map.OnLeave(this);

            Direction oldFacing = m_Facing;
            m_Facing = facing;

            int xOffset = 0, yOffset = 0;
            Movement.Movement.Offset(facing, ref xOffset, ref yOffset);
            int count = ((int)(m_Facing - oldFacing) & 0x7) / 2;

            UpdateContainedItemsFacing(oldFacing, facing, count);    // update contained items
            SetItemIDOnSmooth(NewFacingItemID.Value);                // update multi id
            UpdateContainedMobilesFacing(oldFacing, facing, count);  // update contained mobiles

            this.Map.OnEnter(this);
            return true;
        }

        public override void SetItemIDOnSmooth(int itemID)
        {
            base.SetItemIDOnSmooth(itemID);

            IPooledEnumerable eable = GetClientsInRange(GetMaxUpdateRange());
            foreach (NetState state in eable)
            {
                Mobile mob = state.Mobile;
                if (mob.CanSee(this) && mob.InRange(Location, GetUpdateRange(mob)))
                    SendInfoTo(state);
            }
            eable.Free();
        }

        protected virtual void UpdateContainedItemsFacing(Direction oldFacing, Direction newFacing, int count)
        {
            m_ContainedObjects.ForEachItem(item =>
            {
                if (item is IFacingChange)
                    ((IFacingChange)item).SetFacing(oldFacing, newFacing);

                item.Location = Rotate(item, count);
            });
        }

        protected virtual void UpdateContainedMobilesFacing(Direction oldFacing, Direction facing, int count)
        {
            m_ContainedObjects.ForEachMobile(mob =>
            {
                mob.SetDirection((facing + (mob.Direction - oldFacing)) & Direction.Mask);
                mob.SetLocation(Rotate(mob, count), true);
            });
        }

        protected Point3D Rotate(IEntity e, int count)
        {
            Point3D toRotate = e.Location;
            int rx = toRotate.X - Location.X;
            int ry = toRotate.Y - Location.Y;

            for (int i = 0; i < count; ++i)
            {
                int temp = rx;
                rx = -ry;
                ry = temp;
            }

            return new Point3D(Location.X + rx, Location.Y + ry, toRotate.Z);
        }
        #endregion


        #region Checks
        public bool CanFit(Point3D loc, Map map, int itemID)
        {
            if (map == null || map == Map.Internal || Deleted)
                return false;

            MultiComponentList newComponents = MultiData.GetComponents(itemID);		
			
            for (int x = 0; x < newComponents.Width; ++x)
            {
                for (int y = 0; y < newComponents.Height; ++y)
                {			
					if (itemID <= 0x20 && itemID >= 0x18)
					{
						if (m_Facing == Direction.North)
						{
							if ((x >= 0) && (x <= 4)) 
								continue;
							if ((x >= newComponents.Width - 5) && (x < newComponents.Width))
								continue;
						}
						else if (m_Facing == Direction.South)
						{
							if ((x >= 0) && (x <= 4)) 
								continue;
							if ((x >= newComponents.Width - 5) && (x < newComponents.Width))
								continue;
						}
						else if (m_Facing == Direction.East)
						{
							if ((y >= 0) && (y <= 4))
								continue;	
							if ((y >= newComponents.Height - 5) && (y < newComponents.Height))
								continue;
						}
						else
						{
							if ((y >= 0) && (y <= 4))
								continue;	
							if ((y >= newComponents.Height - 5) && (y < newComponents.Height))
								continue;
						}						
					}					
				
                    int tx = loc.X + newComponents.Min.X + x;
                    int ty = loc.Y + newComponents.Min.Y + y;					
					
                    if (newComponents.Tiles[x][y].Length == 0 || Contains(tx, ty))
                        continue;

                    bool isWalkable = false;

                    // landTile check
                    LandTile landTile = map.Tiles.GetLandTile(tx, ty);
                    if (landTile.Z == loc.Z && IsEnabledLandID(landTile.ID))
                        isWalkable = true;
						
                    // staticTiles check
                    foreach (StaticTile tile in map.Tiles.GetStaticTiles(tx, ty, true))
                    {
                        if (IsEnabledStaticID(tile.ID) && (tile.Z == loc.Z))
							isWalkable = true;
                        else if (!IsEnabledStaticID(tile.ID) && (tile.Z >= loc.Z))//else if (!isBridgeEnabledTile(tile, loc.Z, maxMultiZ))
                            return false;
                    }

                    if (!isWalkable)
                        return false;
                }
            }

            // controllo items ( da aggiungere controllo ponti se previsti )
            IPooledEnumerable eable = map.GetItemsInBounds(new Rectangle2D(loc.X + newComponents.Min.X, loc.Y + newComponents.Min.Y, newComponents.Width, newComponents.Height));

            foreach (Item item in eable)
            {
                if (item is BaseSmoothMulti || item.ItemID > TileData.MaxItemValue || item.Z < loc.Z || !item.Visible)
                    continue;

                int x = item.X - loc.X + newComponents.Min.X;
                int y = item.Y - loc.Y + newComponents.Min.Y;

                if (x >= 0 && x < newComponents.Width && y >= 0 && y < newComponents.Height && newComponents.Tiles[x][y].Length == 0)
                    continue;
                else if (IsOnBoard(item))
                    continue;

                eable.Free();
                return false;
            }

            eable.Free();

            return true;
        }

        protected abstract bool IsEnabledStaticID(int staticID);

        protected abstract bool IsEnabledLandID(int landID);

        //protected abstract bool isBridgeEnabledTile(StaticTile tile, int z, int maxMultiZ);

        public virtual bool IsOnBoard(Mobile mob)
        {
            return m_ContainedObjects.Contains(mob);
        }

        public virtual bool IsOnBoard(Item item)
        {
            return m_ContainedObjects.Contains(item);
        }

        public override bool Contains(int x, int y)
        {
            if (base.Contains(x, y))
                return true;

            return m_ContainedObjects.Contains(x, y);
        }

        #endregion


        #region Mouse Movement
        public bool TakeCommand(Mobile from)
        {
            return SetPilot(from);
        }

        public void LeaveCommand(Mobile from)
        {
            SetPilot(null);
        }

        private bool CanPilot(Mobile from)
        {
            if (from == null || !from.CheckAlive() || !IsOnBoard(from) || from.Mounted)
                return false;

            return true;
        }

        private bool SetPilot(Mobile from)
        {
            if (from == null) // no new pilot
            {
                m_Pilot = null;
                m_VirtualMount.Internalize();
            }
            else
            {
                if (!CanPilot(from))
                    return false;

                //if (m_Pilot != from && from.AccessLevel < AccessLevel.GameMaster)
                //    return false;

                m_Pilot = from;
                from.Direction = m_Facing;
                from.AddItem(m_VirtualMount);
            }           
                        
            if (m_MoveTimer != null) // stop boat if moving before mouse control
                m_MoveTimer.Stop();

            return true;
        }

        public void OnMousePilotCommand(Mobile from, Direction movementDir, int rawSpeed)
        {
            if (rawSpeed != 0)
            {
                if (!BeginMove(movementDir, ComputeMouseSpeed(rawSpeed)))
                {
                    EndMove();
                    return;
                }

                if (rawSpeed > 1 && ((m_Facing - movementDir) & 0x7) > 1)
                    BeginTurn(movementDir);
            }
            else
                EndMove();
        }

        private SpeedCode ComputeMouseSpeed(int rawSpeed)
        {
            switch (rawSpeed)
            {
                case 1:
                    return SpeedCode.Slow;
                case 2:
                    return SpeedCode.Fast;
                default:
                    return SpeedCode.Stop;
            }
        }
        #endregion


        #region OnBoard Object Management
        public void Embark(Item item)
        {
            if (item == null || item.Deleted || item == this)
                return;

            if (item.Transport != null)
            {
                if (item.Transport == this)
                    return;

                item.Transport.Disembark(item);
            }

            item.Transport = this;
            m_ContainedObjects.Add(item);
            ReleaseWorldPackets();
        }

        public void Embark(Mobile mob)
        {
            if (mob == null || mob.Deleted)
                return;

            if (mob.Transport != null)
            {
                if (mob.Transport == this)
                    return;

                mob.Transport.Disembark(mob);
            }

            mob.Transport = this;
            m_ContainedObjects.Add(mob);
            ReleaseWorldPackets();
        }

        public void Disembark(Item item)
        {
            if (item != null && item.Transport == this)
            {
                item.Transport = null;
                m_ContainedObjects.Remove(item);
                ReleaseWorldPackets();
            }
        }

        public void Disembark(Mobile mob)
        {
            if (mob != null && mob.Transport == this)
            {
                mob.Transport = null;
                m_ContainedObjects.Remove(mob);
                ReleaseWorldPackets();
            }
        }
        #endregion

        public override void OnMapChange()
        {
            m_ContainedObjects.ForEachObject(
                item => item.Map = Map,
                mob => mob.Map = Map);
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            WorldItemHS packet = new WorldItemHS(this);
            packet.SetStatic();

            IPooledEnumerable eable = GetClientsInRange(GetMaxUpdateRange());
            foreach (NetState state in eable)
            {
                Mobile mob = state.Mobile;
                if (mob.CanSee(this) && mob.InRange(Location, GetUpdateRange(mob)))
                    state.Send(packet);
            }
            eable.Free();
            packet.Release();

            int rx = X - oldLoc.X;
            int ry = Y - oldLoc.Y;
            int rz = Z - oldLoc.Z;

            m_ContainedObjects.ForEachObject(
                item => item.Location = new Point3D(item.X + rx, item.Y + ry, item.Z + rz),
                mob => mob.Location = new Point3D(mob.X + rx, mob.Y + ry, mob.Z + rz));
        }

        public override void ReleaseWorldPackets()
        {
            //base.ReleaseWorldPackets(); commented for HS use only
            if (m_ContainerPacket != null)
            {
                m_ContainerPacket.Release();
                m_ContainerPacket = null;
            }
        }

        public override void OnAfterDelete()
        {
            foreach (IEntity obj in m_ContainedObjects.ToList()) // toList necessary for enumeration modification
            {
                if (obj is Item)
                    ((Item)obj).Delete();
                else if (obj is ICrew)
                    ((ICrew)obj).Delete();
                else if (obj is Mobile)
                    ((Mobile)obj).Transport = null;
            }

            if (m_MoveTimer != null)
                m_MoveTimer.Stop();

            if (m_TurnTimer != null)
                m_TurnTimer.Stop();

            m_Instances.Remove(this);
        }

        public virtual void OnRiderDamaged(int amount, Mobile from, bool willKill) { }
		
		public override bool AllowsRelativeDrop
		{
			get { return true; }
		}

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((byte)m_Facing);
            writer.Write((Item)m_VirtualMount);
            m_ContainedObjects.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Facing = (Direction)reader.ReadByte();
            m_VirtualMount = reader.ReadItem() as SmoothMultiMountItem;
            m_Moving = m_Facing;
            m_Speed = SpeedCode.Stop;
            m_ContainedObjects = new DynamicComponentList(reader);

            if (m_VirtualMount == null)
                Delete();
        }
        #endregion

        public class TurnTimer : Timer
        {
            private BaseSmoothMulti m_Multi;
            private Direction m_TurnDir;

            public TurnTimer(BaseSmoothMulti multi, Direction turnDir)
                : base(TimeSpan.FromSeconds(0.5))
            {
                m_Multi = multi;
                m_TurnDir = turnDir;

                Priority = TimerPriority.TenMS;
            }

            protected override void OnTick()
            {
                if (!m_Multi.Deleted)
                    m_Multi.Turn(m_TurnDir);
            }
        }

        public class MoveTimer : Timer
        {
            private BaseSmoothMulti m_Multi;

            public MoveTimer(BaseSmoothMulti multi, TimeSpan interval, SpeedCode speed)
                : base(TimeSpan.Zero, interval, speed == SpeedCode.One ? 1 : 0)
            {
                m_Multi = multi;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (!m_Multi.Move(m_Multi.Moving, m_Multi.CurrentSpeed) || m_Multi.CurrentSpeed == SpeedCode.One)
                    m_Multi.EndMove();
            }
        }

        private class SmoothMultiMountItem : Item, IMountItem
        {
            private BaseSmoothMulti m_Mount;

            public IMount Mount { get { return m_Mount; } set{}}

            public SmoothMultiMountItem(BaseSmoothMulti mount)
                : base(0x3E96)
            {
                Layer = Layer.Mount;

                Movable = false;
                m_Mount = mount;
            }

            public SmoothMultiMountItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write((Item)m_Mount);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                m_Mount = reader.ReadItem() as BaseSmoothMulti;

                if (m_Mount == null)
                    Delete();
                else
                    Internalize();
            }
        }
    }

    public class DynamicComponentList
    {
        private Dictionary<Serial, IEntity> m_InternalList;
        private object m_Locker;

        public int Count { get { return m_InternalList.Values.Count; } }
        
        public DynamicComponentList()
        {
            m_InternalList = new Dictionary<Serial, IEntity>(new InternalComparer());
            m_Locker = ((ICollection)m_InternalList).SyncRoot;
        }

        public DynamicComponentList(GenericReader reader)
        {
            Deserialize(reader);
        }

        public void Add(Item i)
        {
            lock (m_Locker)
            {
                if (!m_InternalList.ContainsKey(i.Serial))
                    m_InternalList.Add(i.Serial, i);
            }
        }

        public void Add(Mobile m)
        {
            lock (m_Locker)
            {
                if (!m_InternalList.ContainsKey(m.Serial))
                    m_InternalList.Add(m.Serial, m);
            }
        }

        public void Remove(Item i)
        {
            lock (m_Locker)
            {
                m_InternalList.Remove(i.Serial);
            }
        }

        public void Remove(Mobile m)
        {
            lock (m_Locker)
            {
                m_InternalList.Remove(m.Serial);
            }
        }

        public bool Contains(Item item)
        {
            return m_InternalList.ContainsKey(item.Serial);
        }

        public bool Contains(Mobile mob)
        {
            return m_InternalList.ContainsKey(mob.Serial);
        }

        public bool Contains(int x, int y)
        {
            return m_InternalList.Values.OfType<Item>().FirstOrDefault(item => item.X == x && item.Y == y) != null;
        }

        public void ForEachItem(Action<Item> itemCmd)
        {
            lock (m_Locker)
            {
                foreach (IEntity obj in m_InternalList.Values)
                {
                    if (obj is Item)
                        itemCmd.Invoke((Item)obj);
                }
            }
        }

        public void ForEachMobile(Action<Mobile> mobCmd)
        {
            lock (m_Locker)
            {
                foreach (IEntity obj in m_InternalList.Values)
                {
                    if (obj is Mobile)
                        mobCmd.Invoke((Mobile)obj);
                }
            }
        }

        public void ForEachObject(Action<Item> itemCmd, Action<Mobile> mobCmd)
        {
            lock (m_Locker)
            {
                foreach (IEntity obj in m_InternalList.Values)
                {
                    if (obj is Item)
                        itemCmd.Invoke((Item)obj);
                    else
                        mobCmd.Invoke((Mobile)obj);
                }
            }
        }

        public List<IEntity> ToList()
        {
            return new List<IEntity>(m_InternalList.Values);
        }

        #region Serialization
        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            int itemSize = m_InternalList.Values.Count(obj => obj is Item);
            writer.Write((int)itemSize);

            foreach (Item obj in m_InternalList.Values.OfType<Item>())
                writer.Write((Item)obj);

            writer.Write((int)m_InternalList.Values.Count - itemSize);

            foreach (Mobile mob in m_InternalList.Values.OfType<Mobile>())
                writer.Write((Mobile)mob);
        }

        public void Deserialize(GenericReader reader)
        {
            reader.ReadInt();

            int itemSize = reader.ReadInt();
            m_InternalList = new Dictionary<Serial, IEntity>(new InternalComparer());
            m_Locker = ((ICollection)m_InternalList).SyncRoot;

            Item it;
            for (int i = 0; i < itemSize; i++)
            {
                it = reader.ReadItem();
                m_InternalList.Add(it.Serial, it);
            }

            int mobSize = reader.ReadInt();

            Mobile mob;
            for (int i = 0; i < mobSize; i++)
            {
                mob = reader.ReadMobile();
                m_InternalList.Add(mob.Serial, mob);
            }
        }
        #endregion

        private class InternalComparer : IEqualityComparer<Serial>
        {
            public bool Equals(Serial a, Serial b) { return a == b; }

            public int GetHashCode(Serial e) { return e; }
        }
    }
}
