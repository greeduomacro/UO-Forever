using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Regions;

namespace Server.Mobiles
{    
    public abstract class BaseMount : BaseCreature, IMount
	{
		private static TimeSpan CombatHeatDelay = TimeSpan.FromSeconds( 15.0 );
		private Mobile m_Rider;
		private IMountItem m_InternalItem;
        protected Item InternalItem { get { return m_InternalItem as Item; } }

		private DateTime m_NextMountAbility;

		public virtual TimeSpan MountAbilityDelay { get { return TimeSpan.Zero; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime NextMountAbility
		{
			get { return m_NextMountAbility; }
			set { m_NextMountAbility = value; }
		}

        public void NewInternalItem()
        {
            if (m_InternalItem != null)
            {
                m_InternalItem.Mount = null;
            }
            m_InternalItem = new MountItem(this, InternalItemItemID);
            if (m_InternalItem is Item)
                ((Item)m_InternalItem).Hue = this.Hue;
        }

        public virtual int InternalItemItemID { get { return 0; } }

		public virtual bool AllowMaleRider{ get{ return true; } }
		public virtual bool AllowFemaleRider{ get{ return true; } }

		public BaseMount( /*string name,*/ int bodyID, AIType aiType, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed ) : base ( aiType, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed )
		{
			//Name = name;
			Body = bodyID;

			m_InternalItem = new MountItem( this, InternalItemItemID );
		}

		public BaseMount( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( m_NextMountAbility );

			writer.Write( m_Rider );
			writer.Write( m_InternalItem as Item );
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get
			{
				return base.Hue;
			}
			set
			{
				base.Hue = value;

				if ( m_InternalItem != null && m_InternalItem is Item)
					((Item)m_InternalItem).Hue = value;
			}
		}

		public override bool OnBeforeDeath()
		{
			Rider = null;

			return base.OnBeforeDeath();
		}

		public override void OnAfterDelete()
		{
			if ( m_InternalItem != null && m_InternalItem is Item)
				((Item)m_InternalItem).Delete();

			m_InternalItem = null;

			base.OnAfterDelete();
		}

		public override void OnDelete()
		{
			Rider = null;

			base.OnDelete();
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_NextMountAbility = reader.ReadDateTime();
					goto case 0;
				}
				case 0:
				{
					m_Rider = reader.ReadMobile();
					m_InternalItem = reader.ReadItem() as IMountItem;

                    if (m_InternalItem == null || !(m_InternalItem is IMountItem))
                        //Delete(); // don't delete it anymore, make a new one
                        NewInternalItem();
					break;
				}
			}
		}

		public virtual void OnDisallowedRider( Mobile m )
		{
			m.SendMessage( "You may not ride this creature." );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsDeadPet )
				return;

			if ( from.IsBodyMod && !from.Body.IsHuman )
			{
				if (from.EraAOS) // You cannot ride a mount in your current form.
					PrivateOverheadMessage( Network.MessageType.Regular, 0x3B2, 1062061, from.NetState );
				else
					from.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.

				return;
			}

			if ( !CheckMountAllowed( from, true ) )
				return;

		    if (from.IsT2A)
		    {
		        from.SendMessage(61, "You cannot ride mounts in T2A zones.");
		        return;
		    }

			if ( from.Mounted )
			{
				from.SendLocalizedMessage( 1005583 ); // Please dismount first.
				return;
			}
			
			for ( int i = 0; i < this.Aggressors.Count; ++i )
			{
				AggressorInfo info = this.Aggressors[i];

				if ((DateTime.UtcNow - info.LastCombatTime) < CombatHeatDelay)
				{
					from.SendMessage( "You cannot ride this mount while it is in combat." );
					return;				
				}
			}
			for ( int i = 0; i < this.Aggressed.Count; ++i )
			{
				AggressorInfo info = this.Aggressed[i];

				if ((DateTime.UtcNow - info.LastCombatTime) < CombatHeatDelay)
				{
					from.SendMessage( "You cannot ride this mount while it is in combat." );
					return;				
				}

						
			}

			if ( from.Female ? !AllowFemaleRider : !AllowMaleRider )
			{
				OnDisallowedRider( from );
				return;
			}

			if ( !Multis.DesignContext.Check( from ) )
				return;

			if ( from.HasTrade )
			{
				from.SendLocalizedMessage( 1042317, "", 0x41 ); // You may not ride at this time
				return;
			}

			if ( from.InRange( this, 1 ) )
			{
				bool canAccess = ( from.AccessLevel >= AccessLevel.GameMaster )
					|| ( Controlled && ControlMaster == from )
					|| ( Summoned && SummonMaster == from );

				if ( canAccess )
				{
					if ( this.Poisoned )
						PrivateOverheadMessage( Network.MessageType.Regular, 0x3B2, 1049692, from.NetState ); // This mount is too ill to ride.
					else
						Rider = from;
				}
				else if ( !Controlled && !Summoned )
				{
					// That mount does not look broken! You would have to tame it to ride it.
					PrivateOverheadMessage( Network.MessageType.Regular, 0x3B2, 501263, from.NetState );
				}
				else
				{
					// This isn't your mount; it refuses to let you ride.
					PrivateOverheadMessage( Network.MessageType.Regular, 0x3B2, 501264, from.NetState );
				}
			}
			else
			{
				from.SendLocalizedMessage( 500206 ); // That is too far away to ride.
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ItemID
		{
			get
			{
				if ( m_InternalItem != null && m_InternalItem is Item )
					return ((Item)m_InternalItem).ItemID;
				else
					return InternalItemItemID;
			}
			set
			{
				if ( m_InternalItem != null && m_InternalItem is Item)
                    ((Item)m_InternalItem).ItemID = value;
			}
		}

		public static void Dismount( Mobile m )
		{
			IMount mount = m.Mount;

			if ( mount != null )
				mount.Rider = null;
		}

        public static void DismountStable(Mobile m)
        {
            IMount mount = m.Mount;

            if (mount != null)
                mount.Rider = null;

            if (m is PlayerMobile)
            {
                var player = m as PlayerMobile;
                player.AutoStablePets();
            }
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Rider
		{
			get
			{
				return m_Rider;
			}
			set
			{
				if ( m_Rider != value )
				{
					if ( value == null )
					{
						Point3D loc = m_Rider.Location;
						Map map = m_Rider.Map;

						if ( map == null || map == Map.Internal )
						{
							loc = m_Rider.LogoutLocation;
							map = m_Rider.LogoutMap;
						}

						Direction = m_Rider.Direction;
						Location = loc;
						Map = map;

						if ( m_InternalItem != null && m_InternalItem is Item)
							((Item)m_InternalItem).Internalize();
					}
					else
					{
						if ( m_Rider != null )
							Dismount( m_Rider );

						Dismount( value );

                        if (m_InternalItem != null && m_InternalItem is Item)
							value.AddItem( (Item)m_InternalItem );

						value.Direction = this.Direction;

						Internalize();
					}

					m_Rider = value;
				}
			}
		}

		private class BlockEntry
		{
			public BlockMountType m_Type;
			public DateTime m_Expiration;

			public bool IsExpired{ get{ return ( DateTime.UtcNow >= m_Expiration ); } }

			public BlockEntry( BlockMountType type, DateTime expiration )
			{
				m_Type = type;
				m_Expiration = expiration;
			}
		}

		private static Dictionary<Mobile, BlockEntry> m_Table = new Dictionary<Mobile, BlockEntry>();

		public static void SetMountPrevention( Mobile mob, BlockMountType type, TimeSpan duration )
		{
			if ( mob == null )
				return;

			DateTime expiration = DateTime.UtcNow + duration;

			BlockEntry entry;
			m_Table.TryGetValue( mob, out entry );

			if ( entry != null )
			{
				entry.m_Type = type;
				entry.m_Expiration = expiration;
			}
			else
			{
				m_Table[mob] = new BlockEntry( type, expiration );
			}
		}

		public static void ClearMountPrevention( Mobile mob )
		{
			if ( mob != null )
				m_Table.Remove( mob );
		}

        public class ExemptedOutsideFeluccaBounds // Alan hacky RDA fix (see ExemptedOutsideFeluccaLocations below)
        {
            public Rectangle2D Bound { get; set; }
            public Map Map { get; set; }
            public ExemptedOutsideFeluccaBounds(Rectangle2D bound, Map map)
            {
                Map = map;
                Bound = bound;
            }
        }

        // hacky Alan Mod (for RDA purposes, since there are only a few locations outside of Felucca that should be accessible to playermobiles)
        public static ExemptedOutsideFeluccaBounds[] ExemptedOutsideFeluccaLocations = new ExemptedOutsideFeluccaBounds[]  
        {
            new ExemptedOutsideFeluccaBounds( new Rectangle2D(new Point2D(1097,400), new Point2D(1207, 496)), Map.Malas),
            new ExemptedOutsideFeluccaBounds( new Rectangle2D(new Point2D(2078,20), new Point2D(2247, 190)), Map.Ilshenar),
            new ExemptedOutsideFeluccaBounds( new Rectangle2D(new Point2D(1752,948), new Point2D(1873, 999)), Map.Ilshenar),
            new ExemptedOutsideFeluccaBounds( new Rectangle2D(new Point2D(1484,866), new Point2D(1527, 895)), Map.Ilshenar),
            new ExemptedOutsideFeluccaBounds( new Rectangle2D(new Point2D(5125,759), new Point2D(5373, 1033)), Map.Trammel),
            new ExemptedOutsideFeluccaBounds( new Rectangle2D(new Point2D(5169,1084), new Point2D(5229, 1125)), Map.Trammel),
            new ExemptedOutsideFeluccaBounds( new Rectangle2D(new Point2D(1802,1778), new Point2D(1853, 1829)), Map.Malas)
        };

        public static bool IsInExemptArea(IPoint2D location, Map map)
        {
            foreach (ExemptedOutsideFeluccaBounds bound in ExemptedOutsideFeluccaLocations)
            {
                if (map == bound.Map)
                {
                    if (bound.Bound.Contains(location))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

		public static BlockMountType GetMountPrevention( Mobile mob )
		{
			if ( mob == null )
				return BlockMountType.None;

            // Alan Mod
            if (DynamicSettingsController.MountFeluccaOnly && mob.Player && mob.Map != Map.Felucca && mob.Map != Map.Internal)
            {
                if (!IsInExemptArea(mob, mob.Map))
                    return BlockMountType.MapRestricted;
            }

            if (mob.Region is CustomRegion)
            {
                CustomRegion region = (CustomRegion)mob.Region;
                if (region.Controller != null && region.Controller.NoMounts)
                {
                    return BlockMountType.MapRestricted;
                }
            }
            // end Alan Mod

			BlockEntry entry;
			m_Table.TryGetValue( mob, out entry );

			if ( entry == null )
				return BlockMountType.None;

			if ( entry.IsExpired )
			{
				m_Table.Remove( mob );
				return BlockMountType.None;
			}

			return entry.m_Type;
		}

		public static bool CheckMountAllowed( Mobile mob, bool message )
		{
			BlockMountType type = GetMountPrevention( mob );

			if ( type == BlockMountType.None )
				return true;

			if ( message )
			{
				switch ( type )
				{
					case BlockMountType.Dazed:
					{
						mob.SendLocalizedMessage( 1040024 ); // You are still too dazed from being knocked off your mount to ride!
						break;
					}
					case BlockMountType.BolaRecovery:
					{
						mob.SendLocalizedMessage( 1062910 ); // You cannot mount while recovering from a bola throw.
						break;
					}
					case BlockMountType.DismountRecovery:
					{
						mob.SendLocalizedMessage( 1070859 ); // You cannot mount while recovering from a dismount special maneuver.
						break;
					}
                    case BlockMountType.MapRestricted:
                    {
                        mob.SendMessage("Your mount refuses to let you ride here!");
                        break;
                    }
				}
			}

			return false;
		}

		public virtual void OnRiderDamaged( int amount, Mobile from, bool willKill )
		{
			if( m_Rider == null )
				return;

			Mobile attacker = from;
			if( attacker == null )
				attacker = m_Rider.FindMostRecentDamager( true );

			if( !(attacker == this || attacker == m_Rider || willKill || DateTime.UtcNow < m_NextMountAbility) )
			{
				if( DoMountAbility( amount, from ) )
					m_NextMountAbility = DateTime.UtcNow + MountAbilityDelay;

			}
		}

		public virtual bool DoMountAbility( int damage, Mobile attacker )
		{
			return false;
		}
	}

	public class MountItem : Item, IMountItem
	{
		private BaseMount m_Mount;

		public override double DefaultWeight { get { return 0; } }

		public MountItem( BaseMount mount, int itemID ) : base( itemID )
		{
			Layer = Layer.Mount;
			Movable = false;

			m_Mount = mount;
		}

		public MountItem( Serial serial ) : base( serial )
		{
		}

		public override void OnAfterDelete()
		{
			if ( m_Mount != null )
				m_Mount.Delete();

			m_Mount = null;

			base.OnAfterDelete();
		}

		public override DeathMoveResult OnParentDeath(Mobile parent)
		{
			if ( m_Mount != null )
				m_Mount.Rider = null;

			return DeathMoveResult.RemainEquipped;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Mount );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Mount = reader.ReadMobile() as BaseMount;

					if ( m_Mount == null )
						Delete();

					break;
				}
			}
		}

		public IMount Mount
		{
			get
			{
				return m_Mount;
			}
            set
            {
                m_Mount = value as BaseMount;
                if (m_Mount == null)
                {
                    Delete();
                }
            }
		}
	}

	public enum BlockMountType
	{
		None = -1,
		Dazed,
		BolaRecovery,
		DismountRecovery,
        MapRestricted
	}
}