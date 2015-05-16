using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Movement;
using Server.Network;
using Server.Targeting;

namespace Server.Multis
{
    public enum BoatDirectionCommand
    {
        Forward = 0,
        ForwardLeft = -1,
        ForwardRight = +1,
        Left = -2,
        Right = +2,
        BackwardLeft = -3,
        BackwardRight = +3,
        Backward = 4
    }

    public enum ShipStatus
    {
        Full = 2,
        Half = 1,
        Low = 0
    }


    public abstract class BaseShip : BaseSmoothMulti
    {
        private ShipStatus m_Status;
        private bool m_Anchored;
        private int m_HullDurability;
        private int m_MaxHullDurability;
        private int m_SailDurability;
        private int m_MaxSailDurability;
		private Hold m_Hold;
		private TillerManHS m_TillerManMobile;
		private NewTillerMan m_TillerManItem;
		private Mobile m_Owner;
		private DateTime m_DecayTime;
		private string m_ShipName;
		private static TimeSpan BoatDecayDelay = TimeSpan.FromDays(13.0);
		private MapItem m_MapItem;
		private int m_NextNavPoint;
		private Timer m_TurnTimer;
		private Timer m_MoveTimer;
		private Direction m_Moving;
		private int m_Speed;
		private byte m_CanModifySecurity; //0: Never //1: Leader //2: Member
		private byte m_Public; //0 : N/A //1: Passenger //2: Crew //3: Officer //4: Captain //5: Deny Access
		private byte m_Party; //0 : N/A //1: Passenger //2: Crew //3: Officer //4: Captain //5: Deny Access
		private byte m_Guild; //0 : N/A //1: Passenger //2: Crew //3: Officer //4: Captain //5: Deny Access
		private Dictionary<PlayerMobile, byte> m_PlayerAccess; //0 : N/A //1: Passenger //2: Crew //3: Officer //4: Captain //5: Deny Access

        #region Properties
		[CommandProperty( AccessLevel.GameMaster )]
		public byte CanModifySecurity{ get{ return m_CanModifySecurity; } set{ m_CanModifySecurity = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShipStatus Status { get { return m_Status; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public byte Public{ get{ return m_Public; } set{ m_Public = value; } }	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public byte Party{ get{ return m_Party; } set{ m_Party = value; } }	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public byte Guild{ get{ return m_Guild; } set{ m_Guild = value; } }	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Direction MovingShip{ get{ return m_Moving; } set{ m_Moving = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsMovingShip{ get{ return ( m_MoveTimer != null ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Speed{ get{ return m_Speed; } set{ m_Speed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentHits { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHits { get; set; }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Anchored
        {
            get { return m_Anchored; }
            set
            {
                if (m_Anchored == value)
                    return;

                if (IsMovingShip)
                    EndMove();

                m_Anchored = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HullDurability
        {
            get { return m_HullDurability; }
            set { SetHullDurability(value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHullDurability 
        {
            get { return m_MaxHullDurability; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SailDurability
        {
            get { return m_SailDurability; }
            set { SetSailDurability(value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSailDurability
        {
            get { return m_MaxSailDurability; }
        }
        
        public override bool HandlesOnSpeech { get { return true; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Hold Hold { get { return m_Hold; } set { m_Hold = value; } }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public TillerManHS TillerManMobile { get { return m_TillerManMobile; } set { m_TillerManMobile = value; } }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public NewTillerMan TillerManItem { get { return m_TillerManItem; } set { m_TillerManItem = value; } }
				
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipName { get { return m_ShipName; } set { m_ShipName = value; if (m_TillerManMobile != null) m_TillerManMobile.InvalidateProperties(); } }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeOfDecay { get { return m_DecayTime; } set { m_DecayTime = value; if (m_TillerManMobile != null) m_TillerManMobile.InvalidateProperties(); } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public MapItem MapItem{ get{ return m_MapItem; } set{ m_MapItem = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int NextNavPoint{ get{ return m_NextNavPoint; } set{ m_NextNavPoint = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
        public virtual Point3D MarkOffset{ get{ return Point3D.Zero; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Dictionary<PlayerMobile, byte> PlayerAccess{ get{ return m_PlayerAccess; } set{ m_PlayerAccess = value; } }			
        #endregion
        
        protected BaseShip(int itemId, int maxhullDurability, int maxsaildurability)
            : base(itemId)
        {
			m_DecayTime = DateTime.UtcNow + BoatDecayDelay;
            m_Anchored = false;
            m_MaxHullDurability = maxhullDurability;
            m_HullDurability = maxhullDurability;
            m_MaxSailDurability = maxsaildurability;
            m_SailDurability = maxsaildurability;
			m_CanModifySecurity = 0;
			m_Public = 0;
			m_Party = 0;
			m_Guild = 0;
			m_PlayerAccess = new Dictionary<PlayerMobile,byte>();
            m_Status = ShipStatus.Full;
        }

        public BaseShip(Serial serial): base(serial)
        {
        }

		#region Commands
        public bool LowerAnchor(bool message)
		{
			if (this.CheckDecay())
				return false;
		   
			#region Frase do TillerMan
			if(this.m_Anchored)
			{
				if (message && this.m_TillerManMobile != null)
					this.m_TillerManMobile.TillerManSay(501445); // Ar, the anchor was already dropped sir.
				if (message && this.m_TillerManItem != null)
					this.m_TillerManItem.Say(501445); // Ar, the anchor was already dropped sir.					
					
				return false;
			}
			#endregion
	   
			EndMove();
	   
			m_Anchored = true;
	   
			#region Frase do TillerMan
			if (message && this.m_TillerManMobile != null)
				this.m_TillerManMobile.TillerManSay(501444); // Ar, anchor dropped sir.
			if (message && this.m_TillerManItem != null)
				this.m_TillerManItem.Say(501444); // Ar, anchor dropped sir.
			#endregion
	   
			return true;
		}
     
		public bool RaiseAnchor(bool message)
		{
			if (this.CheckDecay())
				return false;
	   
			#region Frase do TillerMan
			if (!this.m_Anchored)
			{
				if (message && this.m_TillerManMobile != null)
					this.m_TillerManMobile.TillerManSay(501447); // Ar, the anchor has not been dropped sir.
				if (message && this.m_TillerManItem != null)
					this.m_TillerManItem.Say(501447); // Ar, the anchor has not been dropped sir.
 
				return false;
			}
			#endregion
	   
			m_Anchored = false;
	   
			#region Frase do TillerMan
			if (message && this.m_TillerManMobile != null)
				this.m_TillerManMobile.TillerManSay(501446); // Ar, anchor raised sir.
			if (message && this.m_TillerManItem != null)
				this.m_TillerManItem.Say(501446); // Ar, anchor raised sir.
			#endregion
	   
			return true;
		}

        protected override bool BeginMove(Direction dir, SpeedCode speed)
        {
            if (Anchored)
                return false;

            if (base.BeginMove(dir, speed))
                return true;

            return false;
        }

        protected override bool BeginTurn(Direction newDirection)
        {
            if (Anchored)
                return false;

            return base.BeginTurn(newDirection);
        }

        protected override bool EndMove()
        {
            if (!base.EndMove())
                return false;

            return true;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (IsDriven)
                return;

            Mobile from = e.Mobile;

            if (IsOnBoard(from))
            {
                if (e.Speech.Contains("fast forward") && this is NewBaseBoat)
                {
                    BeginMove(ComputeDirection(BoatDirectionCommand.Forward), SpeedCode.FastForward);
                }
                else
                {
                    for (int i = 0; i < e.Keywords.Length; ++i)
                    {
                        int keyword;

                        {
                            keyword = e.Keywords[i];
                        }

                        if (keyword >= 0x42 && keyword <= 0x6B)
                        {
                            switch (keyword)
                            {
                                    //case 0x42: SetName(e); break;
                                    //case 0x43: RemoveName(e.Mobile); break;
                                    //case 0x44: GiveName(e.Mobile); break;
                                case 0x45:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Forward), SpeedCode.Fast);
                                    break;
                                case 0x46:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Backward), SpeedCode.Fast);
                                    break;
                                case 0x47:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Left), SpeedCode.Fast);
                                    break;
                                case 0x48:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Right), SpeedCode.Fast);
                                    break;
                                case 0x4B:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.ForwardLeft), SpeedCode.Fast);
                                    break;
                                case 0x4C:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.ForwardRight), SpeedCode.Fast);
                                    break;
                                case 0x4D:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.BackwardLeft), SpeedCode.Fast);
                                    break;
                                case 0x4E:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.BackwardRight), SpeedCode.Fast);
                                    break;
                                case 0x4F:
                                    EndMove();
                                    break;
                                case 0x50:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Left), SpeedCode.Slow);
                                    break;
                                case 0x51:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Right), SpeedCode.Slow);
                                    break;
                                case 0x52:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Forward), SpeedCode.Slow);
                                    break;
                                case 0x53:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Backward), SpeedCode.Slow);
                                    break;
                                case 0x54:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.ForwardLeft), SpeedCode.Slow);
                                    break;
                                case 0x55:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.ForwardRight), SpeedCode.Slow);
                                    break;
                                case 0x56:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.BackwardRight), SpeedCode.Slow);
                                    break;
                                case 0x57:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.BackwardLeft), SpeedCode.Slow);
                                    break;
                                case 0x58:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Left), SpeedCode.One);
                                    break;
                                case 0x59:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Right), SpeedCode.One);
                                    break;
                                case 0x5A:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Forward), SpeedCode.One);
                                    break;
                                case 0x5B:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Backward), SpeedCode.One);
                                    break;
                                case 0x5C:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.ForwardLeft), SpeedCode.One);
                                    break;
                                case 0x5D:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.ForwardRight), SpeedCode.One);
                                    break;
                                case 0x5E:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.BackwardRight), SpeedCode.One);
                                    break;
                                case 0x5F:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.BackwardLeft), SpeedCode.One);
                                    break;
                                case 0x49:
                                case 0x65:
                                    BeginTurn(TurnCode.Right);
                                    break; // turn right
                                case 0x4A:
                                case 0x66:
                                    BeginTurn(TurnCode.Left);
                                    break; // turn left
                                case 0x67:
                                    BeginTurn(TurnCode.Around);
                                    break; // turn around, come about
                                case 0x68:
                                    BeginMove(ComputeDirection(BoatDirectionCommand.Forward), SpeedCode.Fast);
                                    break;
                                case 0x69:
                                    EndMove();
                                    break;
                                case 0x6A:
                                    LowerAnchor(true);
                                    break;
                                case 0x6B:
                                    RaiseAnchor(true);
                                    break;
                                    //case 0x60: GiveNavPoint(); break; // nav
                                    //case 0x61: NextNavPoint = 0; StartCourse(false, true); break; // start
                                    //case 0x62: StartCourse(false, true); break; // continue
                                    //case 0x63: StartCourse(e.Speech, false, true); break; // goto*
                                    //case 0x64: StartCourse(e.Speech, true, true); break; // single*
                            }

                            break;
                        }
                    }
                }
            }
        }
        #endregion

        private void SetHullDurability(int newDurability)
        {
            int oldDurability = m_HullDurability;
            if (newDurability > MaxHullDurability)
                m_HullDurability = MaxHullDurability;
            else if (newDurability <= 0)
			{
                new DecayTimer(this).Start();
                m_Decaying = true;
			}
            else
                m_HullDurability = (ushort)newDurability;

            OnHullDurabilityChange(oldDurability);
        }

        private void SetSailDurability(int newDurability)
        {
            int oldDurability = m_SailDurability;
            if (newDurability > MaxHullDurability)
                m_SailDurability = MaxHullDurability;
            else
                m_SailDurability = (ushort)newDurability;

            OnSailDurabilityChange(oldDurability);
        }

        protected virtual void OnHullDurabilityChange(int oldDurability)
        {
            ShipStatus newStatus = (ShipStatus)(HullDurability / Math.Ceiling(MaxHullDurability / 3.0));
            if (newStatus != Status)
            {
                m_Status = newStatus;
                ItemID = GetMultiId(Facing);
                OnHullStatusChange();
            }
        }

        protected virtual void OnSailDurabilityChange(int oldDurability)
        {
        }

        protected void TransferOwnership()
        {
            
        }

        protected virtual void OnHullStatusChange()
        {}

        protected Direction ComputeDirection(BoatDirectionCommand cmd)
        {
            return (Direction)((int)Facing + (int)cmd) & Direction.Mask;
        }
		
		public uint CreateKeys( Mobile m )
		{
			uint value = Key.RandomValue();

			Key packKey = new Key( KeyType.Gold, value, this );
			Key bankKey = new Key( KeyType.Gold, value, this );
			
			packKey.ItemID = 4111;
			bankKey.ItemID = 4111;

			packKey.MaxRange = 10;
			bankKey.MaxRange = 10;

			packKey.Name = "a ship key";
			bankKey.Name = "a ship key";

			BankBox box = m.BankBox;

			if ( !box.TryDropItem( m, bankKey, false ) )
				bankKey.Delete();
			else
				m.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502484 ); // A ship's key is now in my safety deposit box.

			if ( m.AddToBackpack( packKey ) )
				m.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502485 ); // A ship's key is now in my backpack.
			else
				m.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502483 ); // A ship's key is now at my feet.

			return value;
		}	

        public Point3D GetMarkedLocation()
        {
            Point3D p = new Point3D(this.X + this.MarkOffset.X, this.Y + this.MarkOffset.Y, this.Z + this.MarkOffset.Z);

			int rx = p.X - Location.X;
			int ry = p.Y - Location.Y;
			int count = (int)Facing / 2;

			for ( int i = 0; i < count; ++i )
			{
				int temp = rx;
				rx = -ry;
				ry = temp;
			}

			return new Point3D( Location.X + rx, Location.Y + ry, p.Z );			
        }		
		
        public bool CheckKey(uint keyValue)
        {
			object objValue = this.GetPropertyValue("PPlank");
			NewPlank PPlank = objValue as NewPlank;
		
            if (PPlank != null)
				if (PPlank.KeyValue == keyValue)
					return true;
					

			objValue = this.GetPropertyValue("SPlank");
			NewPlank SPlank = objValue as NewPlank;
		
            if (SPlank != null)
				if (SPlank.KeyValue == keyValue)
					return true;
					
					
			objValue = this.GetPropertyValue("Ropes");
            List<BoatRope2> Ropes = objValue as List<BoatRope2>;
		
            if (Ropes != null)
				if (Ropes[0].KeyValue == keyValue)
					return true;

            return false;
        }

        public string GetOwnerName()
        {
            if (Owner == null)
                return "unknown";
            return Owner.RawName;
        }
		
        public void SetName(SpeechEventArgs e)
        {
            if (CheckDecay())
                return;

            if (e.Mobile.AccessLevel < AccessLevel.GameMaster && e.Mobile != m_Owner)
            {
                if (m_TillerManItem != null)
                    m_TillerManItem.Say(1042880); // Arr! Only the owner of the ship may change its name!
				if (m_TillerManMobile != null)
					m_TillerManMobile.TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!
                return;
            }
            else if (!e.Mobile.Alive)
            {
                if (m_TillerManItem != null)
                    m_TillerManItem.Say(502582); // You appear to be dead.
				if (m_TillerManMobile != null)
					m_TillerManMobile.TillerManSay(502582); // You appear to be dead.

                return;
            }

            if (e.Speech.Length > 8)
            {
                string newName = e.Speech.Substring(8).Trim();

                if (newName.Length == 0)
                    newName = null;

                Rename(newName);
            }
        }

        public void Rename(string newName)
        {
            if (CheckDecay())
                return;

            if (newName != null && newName.Length > 40)
                newName = newName.Substring(0, 40);

            if (m_ShipName == newName)
            {
                if (m_TillerManItem != null)
                    m_TillerManItem.Say(502531); // Yes, sir.
                if (m_TillerManMobile != null)
                    m_TillerManMobile.TillerManSay(502531); // Yes, sir.

                return;
            }

            ShipName = newName;

            if (m_TillerManItem != null && m_ShipName != null)
                m_TillerManItem.Say(1042885, m_ShipName); // This ship is now called the ~1_NEW_SHIP_NAME~.
            else if (m_TillerManItem != null)
                m_TillerManItem.Say(502534); // This ship now has no name.
				
            if (m_TillerManMobile != null && m_ShipName != null)
                m_TillerManMobile.TillerManSay(1042885, m_ShipName); // This ship is now called the ~1_NEW_SHIP_NAME~.
            else if (m_TillerManMobile != null)
                m_TillerManMobile.TillerManSay(502534); // This ship now has no name.
        }

        public void RemoveName(Mobile m)
        {
            if (CheckDecay())
                return;

            if (m.AccessLevel < AccessLevel.GameMaster && m != m_Owner)
            {
                if (m_TillerManItem != null)
                    m_TillerManItem.Say(1042880); // Arr! Only the owner of the ship may change its name!
                if (m_TillerManMobile != null)
                    m_TillerManMobile.TillerManSay(1042880); // Arr! Only the owner of the ship may change its name!
					
                return;
            }
            else if (!m.Alive)
            {
                if (m_TillerManItem != null)
                    m_TillerManItem.Say(502582); // You appear to be dead.
                if (m_TillerManMobile != null)
                    m_TillerManMobile.TillerManSay(502582); // You appear to be dead.
					
                return;
            }

            if (m_ShipName == null)
            {
                if (m_TillerManItem != null)
                    m_TillerManItem.Say(502526); // Ar, this ship has no name.
                if (m_TillerManMobile != null)
                    m_TillerManMobile.TillerManSay(502526); // Ar, this ship has no name.

                return;
            }

            ShipName = null;

            if (m_TillerManItem != null)
                m_TillerManItem.Say(502534); // This ship now has no name.
            if (m_TillerManMobile != null)
                m_TillerManMobile.TillerManSay(502534); // This ship now has no name.
        }

        public void GiveName(Mobile m)
        {
            if ((m_TillerManItem == null && m_TillerManMobile == null) || CheckDecay())
                return;

            if (m_ShipName == null)
			{
				if (m_TillerManItem != null)
					m_TillerManItem.Say(502526); // Ar, this ship has no name.
				if (m_TillerManMobile != null)
					m_TillerManMobile.TillerManSay(502526); // Ar, this ship has no name.
			}
            else
			{
				if (m_TillerManItem != null)
					m_TillerManItem.Say(1042881, m_ShipName); // This is the ~1_BOAT_NAME~.
				if (m_TillerManMobile != null)
					m_TillerManMobile.TillerManSay(1042881, m_ShipName); // This is the ~1_BOAT_NAME~.
			}
        }

        private bool m_Decaying;

        public void Refresh()
        {
            m_DecayTime = DateTime.UtcNow + BoatDecayDelay;

            if (m_TillerManMobile != null)
                m_TillerManMobile.InvalidateProperties();
        }
		
        private class DecayTimer : Timer
        {
            private BaseShip m_Boat;
            private int m_Count;

            public DecayTimer(BaseShip boat)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(5.0))
            {
                m_Boat = boat;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Count == 5)
                {
                    List<Mobile> tomove = new List<Mobile>();
                    MultiComponentList mcl = m_Boat.Components;

                    IPooledEnumerable eable = m_Boat.Map.GetObjectsInBounds(new Rectangle2D(m_Boat.X + mcl.Min.X, m_Boat.Y + mcl.Min.Y, mcl.Width, mcl.Height));

                    foreach (object o in eable)
                    {
                        if (o is Mobile && m_Boat.Contains((Mobile)o))
                        {
                            var mobile = o as Mobile;
                            if (mobile.Alive && !mobile.Blessed)
                            {
                                mobile.SendMessage(61, "You have drowned.");
                                mobile.Kill();
                            }

                            if (!mobile.Blessed)
                                tomove.Add(mobile);
                        }
                    }
                    eable.Free();

                    foreach (var mobile in tomove)
                    {
                        Strandedness.MoveStrandedMobile(mobile);
                    }

                    m_Boat.Delete();
                    Stop();
                }
                else
                {
                    m_Boat.Location = new Point3D(m_Boat.X, m_Boat.Y, m_Boat.Z - 1);

                    if (m_Boat.TillerManMobile != null)
                        m_Boat.TillerManMobile.TillerManSay(1007168 + m_Count);
                    if (m_Boat.TillerManItem != null)
                        m_Boat.TillerManItem.Say(1007168 + m_Count);

                    ++m_Count;
                }
            }
        }

        public bool CheckDecay()
        {
            if (m_Decaying)
                return true;

            if (!IsMovingShip && DateTime.UtcNow >= m_DecayTime)
            {
                new DecayTimer(this).Start();

                m_Decaying = true;

                return true;
            }

            return false;
        }
		
		public bool CanCommand( Mobile m )
		{
			return true;
		}
		
        public void BeginRename(Mobile from)
        {
            if (CheckDecay())
                return;

            if (from.AccessLevel < AccessLevel.GameMaster && from != m_Owner)
            {
                if (m_TillerManItem != null)
                    m_TillerManItem.Say(Utility.Random(1042876, 4)); // Arr, don't do that! | Arr, leave me alone! | Arr, watch what thour'rt doing, matey! | Arr! Do that again and I’ll throw ye overhead!
                if (m_TillerManMobile != null)
                    m_TillerManMobile.TillerManSay(Utility.Random(1042876, 4)); // Arr, don't do that! | Arr, leave me alone! | Arr, watch what thour'rt doing, matey! | Arr! Do that again and I’ll throw ye overhead!

                return;
            }

            if (m_TillerManMobile != null)
                m_TillerManMobile.TillerManSay(502580); // What dost thou wish to name thy ship?
            if (m_TillerManItem != null)
                m_TillerManItem.Say(502580); // What dost thou wish to name thy ship?

            from.Prompt = new NewRenameBoatPrompt(this);
        }
		
		public void EndRename( Mobile from, string newName )
		{
			if ( Deleted || CheckDecay() )
				return;

			if ( from.AccessLevel < AccessLevel.GameMaster && from != m_Owner )
			{
				if ( m_TillerManItem != null )
					m_TillerManItem.Say( 1042880 ); // Arr! Only the owner of the ship may change its name!
				if ( m_TillerManMobile != null )
					m_TillerManMobile.TillerManSay( 1042880 ); // Arr! Only the owner of the ship may change its name!

				return;
			}
			else if ( !from.Alive )
			{
				if ( m_TillerManItem != null )
					m_TillerManItem.Say( 502582 ); // You appear to be dead.
				if ( m_TillerManMobile != null )
					m_TillerManMobile.TillerManSay( 502582 ); // You appear to be dead.

				return;
			}

			newName = newName.Trim();

			if ( newName.Length == 0 )
				newName = null;

			Rename( newName );
		}
		
		public void AssociateMap( MapItem map )
		{
			if ( CheckDecay() )
				return;

			if ( map is BlankMap )
			{
				if ( TillerManItem != null )
					TillerManItem.Say( 502575 ); // Ar, that is not a map, tis but a blank piece of paper!
				if ( TillerManMobile != null )
					TillerManMobile.TillerManSay( 502575 ); // Ar, that is not a map, tis but a blank piece of paper!
			}
			else if ( map.Pins.Count == 0 )
			{
				if ( TillerManItem != null )
					TillerManItem.Say( 502576 ); // Arrrr, this map has no course on it!
				if ( TillerManMobile != null )
					TillerManMobile.TillerManSay( 502576 ); // Arrrr, this map has no course on it!
			}
			else
			{
				StopMove( false );

				MapItem = map;
				NextNavPoint = -1;

				if ( TillerManItem != null )
					TillerManItem.Say( 502577 ); // A map!
				if ( TillerManMobile != null )
					TillerManMobile.TillerManSay( 502577 ); // A map!
			}
		}
		
		public bool StopMove( bool message )
		{
			if ( CheckDecay() )
				return false;

			if ( m_MoveTimer == null )
			{
				if ( message && m_TillerManItem != null )
					m_TillerManItem.Say( 501443 ); // Er, the ship is not moving sir.
				if ( message && m_TillerManMobile != null )
					m_TillerManMobile.TillerManSay( 501443 ); // Er, the ship is not moving sir.

				return false;
			}

			m_Moving = Direction.North;
			m_Speed = 0;
			m_MoveTimer.Stop();
			m_MoveTimer = null;

			if ( message && m_TillerManItem != null )
				m_TillerManItem.Say( 501429 ); // Aye aye sir.
			if ( message && m_TillerManMobile != null )
				m_TillerManMobile.TillerManSay( 501429 ); // Aye aye sir.

			return true;
		}

        public class OfficerTarget : Target
        {
            private readonly BaseShip m_Ship;
            private readonly bool m_Add;

            public OfficerTarget(bool add, BaseShip ship)
                : base(12, false, TargetFlags.None)
            {
                CheckLOS = false;

                m_Ship = ship;
                m_Add = add;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!from.Alive || m_Ship.Deleted || m_Ship.Owner != from)
                {
                    return;
                }

                if (targeted is PlayerMobile && m_Ship.Owner != targeted as PlayerMobile)
                {
                    if (m_Add)
                    {
                        if (m_Ship.PlayerAccess != null)
                        {
                            if (m_Ship.PlayerAccess.ContainsKey(targeted as PlayerMobile))
                            {
                                m_Ship.PlayerAccess[targeted as PlayerMobile] = 3;
                            }
                            else
                                m_Ship.PlayerAccess.Add(targeted as PlayerMobile, 3);

                            var pm = targeted as PlayerMobile;
                            pm.SendMessage(61, "You have been made an officer of the " + m_Ship.ShipName + ".");
                        }

                        if (m_Ship.PlayerAccess == null)
                        {
                            var pm = targeted as PlayerMobile;
                            pm.SendMessage(61, "You have been made an officer of the " + m_Ship.ShipName + ".");
                            m_Ship.PlayerAccess = new Dictionary<PlayerMobile, byte>();
                            m_Ship.PlayerAccess.Add(targeted as PlayerMobile, 3);
                        }
                    }
                    else
                    {
                        if (m_Ship.PlayerAccess != null)
                            if (m_Ship.PlayerAccess.ContainsKey(targeted as PlayerMobile))
                            {
                                var pm = targeted as PlayerMobile;
                                pm.SendMessage(61, "You have been removed as an officer of the " + m_Ship.ShipName + ".");
                                m_Ship.PlayerAccess[targeted as PlayerMobile] = 0;
                            }
                            else
                                from.SendMessage(61, "This individual is not an officer on your ship.");
                    }
                }
                else
                {
                    from.SendMessage(61, "That can't be an officer.");
                }
            }
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2);//version
			
			//version 2
			writer.Write(m_CanModifySecurity);
			writer.Write(m_Public);
			writer.Write(m_Party);
			writer.Write(m_Guild);
			
            int listSize = 0;
			if (m_PlayerAccess != null)
				listSize = m_PlayerAccess.Count;
            writer.Write((int)listSize);

			if (listSize > 0)
				foreach(KeyValuePair<PlayerMobile, byte> entry in m_PlayerAccess)
				{		
					writer.Write((Mobile)entry.Key);
					writer.Write((byte)entry.Value);
				}										
			
			//version 1
			writer.Write(m_TillerManMobile);
			writer.Write(m_TillerManItem);
			writer.Write(m_Owner);
			writer.Write((Item)m_MapItem);
			writer.Write((int)m_NextNavPoint);
			writer.WriteDeltaTime(m_DecayTime);
			writer.Write((string)m_ShipName);
			
			//version 0
            writer.Write((bool)m_Anchored);
            writer.Write((int)m_MaxHullDurability);
            writer.Write((int)m_HullDurability);
            writer.Write((int)m_MaxSailDurability);
            writer.Write((int)m_SailDurability);
			
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
	
			switch ( version )
			{
				case 2:
				{	
					m_CanModifySecurity = reader.ReadByte();
					m_Public = reader.ReadByte();
					m_Party = reader.ReadByte();
					m_Guild = reader.ReadByte();
					
					int listSize = reader.ReadInt();
					m_PlayerAccess = new Dictionary<PlayerMobile, byte>();					
					PlayerMobile player;
					byte access;
					for (int i = 0; i < listSize; i++)
					{
						player = (PlayerMobile)reader.ReadMobile();
						access = (byte)reader.ReadByte();
						m_PlayerAccess.Add(player, access);
					}
					
					goto case 1;
				}			
			
				case 1:
				{	
					m_TillerManMobile = reader.ReadMobile() as TillerManHS;
					m_TillerManItem = reader.ReadItem() as NewTillerMan;
					m_Owner = reader.ReadMobile();
					m_MapItem = (MapItem) reader.ReadItem();
					m_NextNavPoint = reader.ReadInt();
					m_DecayTime = reader.ReadDeltaTime();
					m_ShipName = reader.ReadString();
					goto case 0;
				}
				
				case 0:
				{				
					m_Anchored = reader.ReadBool();
                    m_MaxHullDurability = reader.ReadInt();
                    m_HullDurability = reader.ReadInt();
				    m_MaxSailDurability = reader.ReadInt();
                    m_SailDurability = reader.ReadInt();
					break;
				}
			}
            m_Status = (ShipStatus)(MaxHullDurability / Math.Ceiling(MaxHullDurability / 3.0));
        }
        #endregion
    }
	
	public static class PropertyHelper
	{
		public static object GetPropertyValue<T>(this T classInstance, string propertyName)
		{
			PropertyInfo property = classInstance.GetType().GetProperty(propertyName);
			if (property != null)
				return property.GetValue(classInstance, null);
			return null;
		}
	}
}
