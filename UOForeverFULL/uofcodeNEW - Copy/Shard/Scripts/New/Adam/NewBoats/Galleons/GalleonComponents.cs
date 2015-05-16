using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public interface IHelm
    {
        void RefreshItemID(int mod);
    }

    public interface ICannonPlace
    {
        void RefreshItemID(int mod);
    }
	
	public interface ICannon
	{
		void RefreshItemID(int mod);
	}

    public class SingleHelm : BaseGalleonItem, IHelm
    {
	
		private BaseGalleon m_Galleon;
	
		[CommandProperty( AccessLevel.GameMaster )]
		public BaseGalleon Galleon{ get{ return m_Galleon; } }		
	
        public SingleHelm(BaseGalleon galleon, int northItemID, Point3D initOffset)
            : base(galleon, northItemID, initOffset)
        {
			m_Galleon = galleon;
			Name = "Wheel";
		
        }

        public SingleHelm(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Transport.IsDriven)
                Transport.LeaveCommand(from);
            else
				if (m_Galleon != null)
					if ( from == m_Galleon.Owner )
					{			
						from.SendMessage( "You take control of the ship" );
						Transport.TakeCommand(from);
					}
					else if (m_Galleon.PlayerAccess != null)
						if (m_Galleon.PlayerAccess.ContainsKey((PlayerMobile)from))
							if ( m_Galleon.PlayerAccess[(PlayerMobile)from] == 2 )					
							{
								from.SendMessage( "You take control of the ship" );
								Transport.TakeCommand(from);
							}
							else if ( m_Galleon.PlayerAccess[(PlayerMobile)from] == 3 )					
							{
								from.SendMessage( "You take control of the ship" );
								Transport.TakeCommand(from);
							}
							else if ( m_Galleon.PlayerAccess[(PlayerMobile)from] == 4 )					
							{
								from.SendMessage( "You take control of the ship" );
								Transport.TakeCommand(from);
							}																		
					else if (( from.Guild == m_Galleon.Owner.Guild ) && ( from.Guild != null ))
					{
						if ( m_Galleon.Guild == 2 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Guild == 3 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Guild == 4 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
					}
					else if (( from.Party == m_Galleon.Owner.Party) && ( from.Party != null ))
					{
						if ( m_Galleon.Party == 2 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Party == 3 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Party == 4 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
					}
					else
					{
						if ( m_Galleon.Public == 2 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Public == 3 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Public == 4 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
					}								 
        }

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch ( version )
			{		
				case 1:
				{
					m_Galleon = reader.ReadItem() as BaseGalleon;
					break;
				}
			}							
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); //current version is 1
			
			// version 1
			writer.Write((BaseGalleon)m_Galleon);			
        }
        #endregion
    }

    public class MultiHelm : BaseGalleonMultiItem, IHelm
    {
	
		private BaseGalleon m_Galleon;

		[CommandProperty( AccessLevel.GameMaster )]
		public BaseGalleon Galleon{ get{ return m_Galleon; } }	
		
        public MultiHelm(BaseGalleon galleon, int northItemID, Point3D initOffset, int sxNorthItemId, int dxNorthItemId)
            : base(galleon, northItemID, initOffset)
        {
			m_Galleon = galleon;
            AddComponent(new GalleonMultiComponent(sxNorthItemId, this, new Point3D(-1, 0, 0)));
            AddComponent(new GalleonMultiComponent(dxNorthItemId, this, new Point3D(1, 0, 0)));
        }

        public MultiHelm(BaseGalleon galleon, int northItemID, Point3D initOffset, int relatedNorthItemId, Point3D relatedOffset)
            : base(galleon, northItemID, initOffset)
        {
			m_Galleon = galleon;
            AddComponent(new GalleonMultiComponent(relatedNorthItemId, this, relatedOffset));
        }

        public MultiHelm(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Transport.IsDriven)
                Transport.LeaveCommand(from);
            else
				if (m_Galleon != null)
					if ( from == m_Galleon.Owner )
					{			
						from.SendMessage( "You take control of the ship" );
						Transport.TakeCommand(from);
					}
					else if (m_Galleon.PlayerAccess != null)
						if (m_Galleon.PlayerAccess.ContainsKey((PlayerMobile)from))
							if ( m_Galleon.PlayerAccess[(PlayerMobile)from] == 2 )					
							{
								from.SendMessage( "You take control of the ship" );
								Transport.TakeCommand(from);
							}
							else if ( m_Galleon.PlayerAccess[(PlayerMobile)from] == 3 )					
							{
								from.SendMessage( "You take control of the ship" );
								Transport.TakeCommand(from);
							}
							else if ( m_Galleon.PlayerAccess[(PlayerMobile)from] == 4 )					
							{
								from.SendMessage( "You take control of the ship" );
								Transport.TakeCommand(from);
							}																		
					else if (( from.Guild == m_Galleon.Owner.Guild ) && ( from.Guild != null ))
					{
						if ( m_Galleon.Guild == 2 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Guild == 3 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Guild == 4 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
					}
					else if (( from.Party == m_Galleon.Owner.Party) && ( from.Party != null ))
					{
						if ( m_Galleon.Party == 2 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Party == 3 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Party == 4 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
					}
					else
					{
						if ( m_Galleon.Public == 2 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Public == 3 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
						else if ( m_Galleon.Public == 4 )					
						{
							from.SendMessage( "You take control of the ship" );
							Transport.TakeCommand(from);
						}
					}								 
        }

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch ( version )
			{		
				case 1:
				{
					m_Galleon = reader.ReadItem() as BaseGalleon;
					break;
				}
			}							
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); //current version is 1
			
			// version 1
			writer.Write((BaseGalleon)m_Galleon);			
        }
        #endregion
    }

    public class GalleonHold : BaseGalleonMultiContainer
    {
	
		private BaseGalleon m_Galleon;

		[CommandProperty( AccessLevel.GameMaster )]
		public BaseGalleon Galleon{ get{ return m_Galleon; } }	
		
        public GalleonHold(BaseGalleon galleon, int northItemId, Point3D initOffset, List<Tuple<int, Point3D>> componentList, int HoldMaxWeight = 16000)
            : base(galleon, northItemId, initOffset)
        {
			m_Galleon = galleon;
			Name = "Cargo Hold";
			m_MaxWeight = HoldMaxWeight;
            foreach (Tuple<int, Point3D> comp in componentList)
                AddComponent(new GalleonMultiComponent(comp.Item1, this, comp.Item2));
            LiftOverride = true;
        }

        public GalleonHold(Serial serial)
            : base(serial)
        {
        }

		
		private int m_MaxWeight;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public override int MaxWeight{ get{ return m_MaxWeight; } } 		

		public override void OnDoubleClick(Mobile from)
        {
			if (m_Galleon != null)
				if ( from == m_Galleon.Owner )						
					base.OnDoubleClick( from );			
				else if (m_Galleon.PlayerAccess != null)
					if (m_Galleon.PlayerAccess.ContainsKey((PlayerMobile)from))
						if ( m_Galleon.PlayerAccess[(PlayerMobile)from] == 3 )					
							base.OnDoubleClick( from );
						else if ( m_Galleon.PlayerAccess[(PlayerMobile)from] == 4 )					
							base.OnDoubleClick( from );																	
				else if (( from.Guild == m_Galleon.Owner.Guild ) && ( from.Guild != null ))			
					if ( m_Galleon.Guild == 3 )					
						base.OnDoubleClick( from );
					else if ( m_Galleon.Guild == 4 )					
						base.OnDoubleClick( from );			
				else if (( from.Party == m_Galleon.Owner.Party) && ( from.Party != null ))			
					if ( m_Galleon.Party == 3 )					
						base.OnDoubleClick( from );
					else if ( m_Galleon.Party == 4 )					
						base.OnDoubleClick( from );			
				else
					if ( m_Galleon.Public == 3 )					
						base.OnDoubleClick( from );
					else if ( m_Galleon.Public == 4 )					
						base.OnDoubleClick( from );								 
        }
		
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // current version is 2

			// version 2
			writer.Write((BaseGalleon)m_Galleon);
			
			// version 1
			writer.Write((int)m_MaxWeight);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch ( version )
			{
				case 2:
				{
					m_Galleon = reader.ReadItem() as BaseGalleon;
					goto case 1;
				}			
			
				case 1:
				{
					m_MaxWeight = (int) reader.ReadInt();
					break;
				}
			}						
        }
    }

    public class SingleCannonPlace : BaseGalleonItem, ICannonPlace
    {
        public SingleCannonPlace(BaseGalleon galleon, int northItemId, Point3D initOffset)
            : base(galleon, northItemId, initOffset)
        {
			Name = " ";
			m_Galleon = galleon;
        }

        public SingleCannonPlace(Serial serial)
            : base(serial)
        {
        }
		
		private BaseGalleon m_Galleon;
		
		private LightShipCannon m_LinkedCannon;
		private HeavyShipCannon m_LinkedHeavyCannon;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public BaseGalleon Galleon { get{ return m_Galleon; } } 
		
		[CommandProperty( AccessLevel.GameMaster )]
		public LightShipCannon LinkedCannon { get{ return m_LinkedCannon; } set{ m_LinkedCannon = value; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public HeavyShipCannon LinkedHeavyCannon { get{ return m_LinkedHeavyCannon; } set{ m_LinkedHeavyCannon = value; } }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);
			
			//version 3
			writer.Write((HeavyShipCannon)m_LinkedHeavyCannon);

			//version 2
			writer.Write((LightShipCannon)m_LinkedCannon);
				
			// version 1
			writer.Write((BaseGalleon)m_Galleon);		
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch ( version )
			{
				case 3:
				{
					m_LinkedHeavyCannon = reader.ReadItem() as HeavyShipCannon;
					goto case 2;
				}	
				case 2:
				{
					m_LinkedCannon = reader.ReadItem() as LightShipCannon;
					goto case 1;
				}			
				case 1:
				{
					m_Galleon = reader.ReadItem() as BaseGalleon;
					break;
				}
			}	
        }
        #endregion
    }

    public class MultiCannonPlace : BaseGalleonMultiItem, ICannonPlace
    {
        public MultiCannonPlace(BaseGalleon galleon, int northItemId, Point3D initOffset, int sxNorthItemId)
            : base(galleon, northItemId, initOffset)
        {
			Name = " ";
			m_Galleon = galleon;
            AddComponent(new GalleonMultiComponent(sxNorthItemId, this, new Point3D(0, 1, 0)));
        }

        public MultiCannonPlace(BaseGalleon galleon, int northItemId, Point3D initOffset, int sxNorthItemId, int dxNorthItemId)
            : base(galleon, northItemId, initOffset)
        {
			Name = " ";
			m_Galleon = galleon;
            AddComponent(new GalleonMultiComponent(sxNorthItemId, this, new Point3D(0, 1, 0)));
            AddComponent(new GalleonMultiComponent(dxNorthItemId, this, new Point3D(0, -1, 0)));
        }

        public MultiCannonPlace(Serial serial)
            : base(serial)
        {
        }
		
		private BaseGalleon m_Galleon;		
		private LightShipCannon m_LinkedCannon;
		private HeavyShipCannon m_LinkedHeavyCannon;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public BaseGalleon Galleon { get{ return m_Galleon; } } 
		
		[CommandProperty( AccessLevel.GameMaster )]
		public LightShipCannon LinkedCannon { get{ return m_LinkedCannon; } set{ m_LinkedCannon = null; } } 
		
		[CommandProperty( AccessLevel.GameMaster )]
		public HeavyShipCannon LinkedHeavyCannon { get{ return m_LinkedHeavyCannon; } set{ m_LinkedHeavyCannon = value; } }
		

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);
			
			//version 3
			writer.Write((HeavyShipCannon)m_LinkedHeavyCannon);
			
			//version 2
			writer.Write((LightShipCannon)m_LinkedCannon);			
			
			// version 1
			writer.Write((BaseGalleon)m_Galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch ( version )
			{
				case 3:
				{
					m_LinkedHeavyCannon = reader.ReadItem() as HeavyShipCannon;
					goto case 2;
				}	
				case 2:
				{
					m_LinkedCannon = reader.ReadItem() as LightShipCannon;
					goto case 1;
				}				
				
				case 1:
				{
					m_Galleon = reader.ReadItem() as BaseGalleon;
					break;
				}
			}
        }
        #endregion
    }

    public class MainMast : BaseGalleonItem
    {
        #region Properties
        public BaseGalleon Galleon { get { return Transport as BaseGalleon; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set
            {
                if (base.Hue == value)
                    return;

                base.Hue = value;
                if (IsEmbarked)
                    Transport.Hue = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Anchored
        {
            get { if (Galleon != null) return Galleon.Anchored; return false; }
            set { if (Galleon != null) Galleon.Anchored = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HullDurability
        {
            get { if (Galleon != null) return Galleon.HullDurability; return 0; }
            set { if (Galleon != null) Galleon.HullDurability = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHullDurability
        {
            get { if (Galleon != null) return Galleon.MaxHullDurability; return 0; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SailsDurability
        {
            get { if (Galleon != null) return Galleon.SailDurability; return 0; }
            set { if (Galleon != null) Galleon.SailDurability = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSailsDurability
        {
            get { if (Galleon != null) return Galleon.MaxSailDurability; return 0; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShipStatus Status
        {
            get { if (Galleon != null) return Galleon.Status; return ShipStatus.Low; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SpeedCode CurrentSpeed
        {
            get { if (Galleon != null) return Galleon.CurrentSpeed; return SpeedCode.Stop; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing
        {
            get { if (Galleon != null) return Galleon.Direction; return Direction.North; }
            set { if (Galleon != null) Galleon.Facing = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsDriven
        {
            get { if (Galleon != null) return Galleon.IsDriven; return false; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsMoving
        {
            get { if (Galleon != null) return Galleon.IsMoving; return false; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Moving
        {
            get { if (Galleon != null) return Galleon.Moving; return Direction.North; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Pilot
        {
            get { if (Galleon != null) return Galleon.Pilot; return null; }
            set { if (Galleon != null) Galleon.Pilot = value; }
        }
        #endregion

        public MainMast(BaseGalleon galleon, int baseNorthId, Point3D initOffset)
            : base(galleon, baseNorthId, initOffset)
        {
        }

        public MainMast(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon != null)
                Galleon.OnDoubleClick(from);
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
        #endregion
    }

    public class BritainHull : BaseGalleonItem
    {
        private static int[,] m_InternalItemIDMods = new int[,]
        {
            {1536, 1260, 1812, 2088 },
            {0, -276, 276, 552 },
            {0, -276, 276, 552 },
        };

        public BritainHull(BaseGalleon galleon, int northItemId)
            : base(galleon, northItemId)
        { }

        public BritainHull(Serial serial)
            : base(serial)
        { }

        public override void RefreshItemID(int itemIDModifier)
        {
            if(Transport is BaseGalleon)
            {
                BaseGalleon trans = (BaseGalleon)Transport;
                SetItemIDOnSmooth(BaseItemID + m_InternalItemIDMods[(int)trans.Status, (int)trans.Facing / 2]);
            }            
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
        #endregion
    }
	
	public enum BoatRopeSide{ Port, Starboard }

	public class BoatRope2 : BaseGalleonItem, ILockable
	{
		private BaseGalleon m_Boat;
		private BoatRopeSide m_Side;
		private bool m_Locked;
		private uint m_KeyValue;

		[CommandProperty(AccessLevel.GameMaster, true)]
        public override bool ShareHue
        {
            get { return false; }
        }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public BaseGalleon Boat{ get{ return m_Boat; } set{ m_Boat = value; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public BoatRopeSide Side{ get{ return m_Side; } set{ m_Side = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Locked{ get{ return m_Locked; } set{ m_Locked = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public uint KeyValue{ get{ return m_KeyValue; } set{ m_KeyValue = value; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Starboard{ get{ return ( m_Side == BoatRopeSide.Starboard ); } }
			
		public BoatRope2( BaseGalleon boat,  int northItemID, Point3D initOffset, BoatRopeSide side, uint keyValue )
			: base(boat, northItemID, initOffset)
		{
			m_Boat = boat;
			m_KeyValue = keyValue;
			m_Side = side;
			m_Locked = true;
			Movable = false;
			Name = "Mooring Line";
		}

        public BoatRope2(Serial serial)
            : base(serial)
		{
		}
		
		public override void OnDoubleClickDead( Mobile from )
		{
			base.OnDoubleClick( from );
			from.MoveToWorld( this.Location, this.Map );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Boat != null && !m_Boat.Contains( from ) )
			{
                if (m_Boat.Status == ShipStatus.Low)
			    {
                    from.MoveToWorld(this.Location, this.Map);
                    from.SendMessage(61, "You have boarded the ship.");
			        return;
			    }

				if ( from == m_Boat.Owner )
				{			
					from.SendMessage( "Welcome aboard, Owner !" );
					base.OnDoubleClick( from );
					from.MoveToWorld( this.Location, this.Map );
				}
				else if (m_Boat.PlayerAccess != null)
					if (m_Boat.PlayerAccess.ContainsKey((PlayerMobile)from))
						if (m_Boat.PlayerAccess[(PlayerMobile)from] == 1)
						{
							from.SendMessage( "Welcome aboard, you are now a passenger !" );
							base.OnDoubleClick( from );
							from.MoveToWorld( this.Location, this.Map );
						}
						else if ( m_Boat.PlayerAccess[(PlayerMobile)from] == 2 )					
						{
							from.SendMessage( "Welcome aboard, you are now a crew !" );
							base.OnDoubleClick( from );
							from.MoveToWorld( this.Location, this.Map );
						}
						else if ( m_Boat.PlayerAccess[(PlayerMobile)from] == 3 )					
						{
							from.SendMessage( "Welcome aboard, you are now an officer !" );
							base.OnDoubleClick( from );
							from.MoveToWorld( this.Location, this.Map );
						}
						else if ( m_Boat.PlayerAccess[(PlayerMobile)from] == 4 )					
						{
							from.SendMessage( "Welcome aboard, you are now a captain !" );
							base.OnDoubleClick( from );
							from.MoveToWorld( this.Location, this.Map );
						}																		
				else if (( from.Guild == m_Boat.Owner.Guild ) && ( from.Guild != null ))
				{
					if ( m_Boat.Guild == 1 )
					{
						from.SendMessage( "Welcome aboard, you are now a passenger !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
					else if ( m_Boat.Guild == 2 )					
					{
						from.SendMessage( "Welcome aboard, you are now a crew !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
					else if ( m_Boat.Guild == 3 )					
					{
						from.SendMessage( "Welcome aboard, you are now an officer !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
					else if ( m_Boat.Guild == 4 )					
					{
						from.SendMessage( "Welcome aboard, you are now a captain !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
				}
				else if (( from.Party == m_Boat.Owner.Party) && ( from.Party != null ))
				{
					if ( m_Boat.Party == 1 )
					{
						from.SendMessage( "Welcome aboard, you are now a passenger !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
					else if ( m_Boat.Party == 2 )					
					{
						from.SendMessage( "Welcome aboard, you are now a crew !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
					else if ( m_Boat.Party == 3 )					
					{
						from.SendMessage( "Welcome aboard, you are now an officer !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
					else if ( m_Boat.Party == 4 )					
					{
						from.SendMessage( "Welcome aboard, you are now a captain !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
				}
				else
				{
					if ( m_Boat.Public == 1 )
					{
						from.SendMessage( "Welcome aboard, you are now a passenger !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
					else if ( m_Boat.Public == 2 )					
					{
						from.SendMessage( "Welcome aboard, you are now a crew !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
					else if ( m_Boat.Public == 3 )					
					{
						from.SendMessage( "Welcome aboard, you are now an officer !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
					else if ( m_Boat.Public == 4 )					
					{
						from.SendMessage( "Welcome aboard, you are now a captain !" );
						base.OnDoubleClick( from );
						from.MoveToWorld( this.Location, this.Map );
					}
				}								
			}
			else if ( m_Boat != null && m_Boat.Contains( from ) )
			{
				Map map = Map;

				if ( map == null )
					return;

				int rx = 0, ry = 0;

				if (m_Side == BoatRopeSide.Port)
				{
					if ( m_Boat.Facing == Direction.North )
						rx = 1;
					else if ( m_Boat.Facing == Direction.South )
						rx = -1;
					else if ( m_Boat.Facing == Direction.East )
						ry = 1;
					else if ( m_Boat.Facing == Direction.West )
						ry = -1;
				}
				else if (m_Side == BoatRopeSide.Starboard)
				{
					if ( m_Boat.Facing == Direction.North )
						rx = -1;
					else if ( m_Boat.Facing == Direction.South )
						rx = 1;
					else if ( m_Boat.Facing == Direction.East )
						ry = -1;
					else if ( m_Boat.Facing == Direction.West )
						ry = 1;
				}

				for ( int i = 1; i <= 6; ++i )
				{
					int x = X + (i*rx);
					int y = Y + (i*ry);
					int z;

					for ( int j = -16; j <= 16; ++j )
					{
						z = from.Z + j;

						if ( map.CanFit( x, y, z, 16, false, false ) && !Server.Spells.SpellHelper.CheckMulti( new Point3D( x, y, z ), map ) && !Region.Find( new Point3D( x, y, z ), map ).IsPartOf( typeof( Factions.StrongholdRegion ) ) )
						{
							if ( i == 1 && j >= -2 && j <= 2 )
								return;

							from.Location = new Point3D( x, y, z );
							return;
						}
					}

					z = map.GetAverageZ( x, y );

					if ( map.CanFit( x, y, z, 16, false, false ) && !Server.Spells.SpellHelper.CheckMulti( new Point3D( x, y, z ), map ) && !Region.Find( new Point3D( x, y, z ), map ).IsPartOf( typeof( Factions.StrongholdRegion ) ) )
					{
						if ( i == 1 )
							return;

						from.Location = new Point3D( x, y, z );
						return;
					}	
				}
			}
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );//version

			writer.Write( m_Boat );
			writer.Write( (int) m_Side );
			writer.Write( m_Locked );
			writer.Write( m_KeyValue );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Boat = reader.ReadItem() as BaseGalleon;
					m_Side = (BoatRopeSide) reader.ReadInt();
					m_Locked = reader.ReadBool();
					m_KeyValue = reader.ReadUInt();
					
					if ( m_Boat == null )
						Delete();

					break;
				}
			}
		}
	}
}
