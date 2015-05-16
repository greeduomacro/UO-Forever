using System;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class StoneFaceAddon : Item, IAddon, IChopable
	{
		private DateTime m_NextPassiveTrigger, m_NextActiveTrigger;
		public bool East{ get{ return FaceType == StoneFaceTrapType.WestWall; } }

		public bool Breathing
		{
			get{ return ( ItemID == GetFireID( this.FaceType ) ); }
			set
			{
				if ( value )
					ItemID = GetFireID( this.FaceType );
				else
					ItemID = GetBaseID( this.FaceType );
			}
		}

		public static int GetBaseID( StoneFaceTrapType type )
		{
			switch ( type )
			{
				case StoneFaceTrapType.NorthWestWall: return 0x10F5;
				case StoneFaceTrapType.NorthWall: return 0x10FC;
				case StoneFaceTrapType.WestWall: return 0x110F;
			}

			return 0;
		}

		public static int GetFireID( StoneFaceTrapType type )
		{
			switch ( type )
			{
				case StoneFaceTrapType.NorthWestWall: return 0x10F7;
				case StoneFaceTrapType.NorthWall: return 0x10FE;
				case StoneFaceTrapType.WestWall: return 0x1111;
			}

			return 0;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public StoneFaceTrapType FaceType
		{
			get
			{
				switch ( ItemID )
				{
					case 0x10F5: case 0x10F6: case 0x10F7: return StoneFaceTrapType.NorthWestWall;
					case 0x10FC: case 0x10FD: case 0x10FE: return StoneFaceTrapType.NorthWall;
					case 0x110F: case 0x1110: case 0x1111: return StoneFaceTrapType.WestWall;
				}

				return StoneFaceTrapType.NorthWestWall;
			}
			set
			{
				bool breathing = this.Breathing;

				ItemID = ( breathing ? GetFireID( value ) : GetBaseID( value ) );
			}
		}

		public void OnChop( Mobile from )
		{
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( from.AccessLevel >= AccessLevel.GameMaster || ( house != null && house.IsOwner( from ) && house.Addons.Contains( this ) ) )
			{
				Effects.PlaySound( GetWorldLocation(), Map, 0x3B3 );
				from.SendLocalizedMessage( 500461 ); // You destroy the item.

				Delete();

				house.Addons.Remove( this );

				Item deed = Deed;

				if ( deed != null )
					from.AddToBackpack( deed );
			}
		}

		public bool CouldFit( IPoint3D p, Map map )
		{
			if ( !map.CanFit( p.X, p.Y, p.Z, this.ItemData.Height ) )
				return false;

			if ( this.ItemID == 0x232C )
				return BaseAddon.IsWall( p.X, p.Y - 1, p.Z, map ); // North wall
			else
				return BaseAddon.IsWall( p.X - 1, p.Y, p.Z, map ); // West wall
		}

		public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

		public void OnTrigger( Mobile from )
		{
			if ( !from.Alive || from.AccessLevel > AccessLevel.Player )
				return;

			Effects.PlaySound( Location, Map, 0x359 );

			Breathing = true;

			Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerCallback( FinishBreath ) );
			//Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( TriggerDamage ) );
		}

		public bool CheckRange( Point3D loc, Point3D oldLoc, int range )
		{
			return CheckRange( loc, range ) && !CheckRange( oldLoc, range );
		}

		public bool CheckRange( Point3D loc, int range )
		{
			return ( (this.Z + 8) >= loc.Z && (loc.Z + 16) > this.Z )
				&& Utility.InRange( GetWorldLocation(), loc, range );
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			base.OnMovement( m, oldLocation );

			if ( m.Location == oldLocation )
				return;

			if ( !m.Alive || m.AccessLevel > AccessLevel.Player )
				return;

			if ( CheckRange( m.Location, oldLocation, 0 ) && DateTime.UtcNow >= m_NextActiveTrigger )
			{
				m_NextActiveTrigger = m_NextPassiveTrigger = DateTime.UtcNow;

				OnTrigger( m );
			}
			else if ( CheckRange( m.Location, oldLocation, 2 ) && DateTime.UtcNow >= m_NextPassiveTrigger )
			{
				m_NextPassiveTrigger = DateTime.UtcNow;

				OnTrigger( m );
			}
		}

		public void FinishBreath()
		{
			Breathing = false;
		}

		public Item Deed
		{
			get{ return new StoneFaceAddonDeed(); }
		}

		[Constructable]
		public StoneFaceAddon( int itemID ) : base( itemID )
		{
			Light = LightType.Circle225;
		}

		public StoneFaceAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			Breathing = false;
		}
	}

	public class StoneFaceAddonDeed : Item
	{
		public override string DefaultName{ get{ return "a stone face addon deed"; } }

		[Constructable]
		public StoneFaceAddonDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
		}

		public StoneFaceAddonDeed( Serial serial ) : base( serial )
		{
		}

		public void Placement_OnTarget( Mobile from, object targeted, object state )
		{
			IPoint3D p = targeted as IPoint3D;

			if ( p == null )
				return;

			Point3D loc = new Point3D( p );

			BaseHouse house = BaseHouse.FindHouseAt( loc, from.Map, 16 );

			if ( (house != null && house.IsCoOwner( from )) || from.AccessLevel >= AccessLevel.GameMaster )
			{
				bool northWall = BaseAddon.IsWall( loc.X, loc.Y - 1, loc.Z, from.Map );
				bool westWall = BaseAddon.IsWall( loc.X - 1, loc.Y, loc.Z, from.Map );

				if ( northWall && westWall )
					from.SendGump( new StoneFaceDeedGump( from, loc, this ) );
				else
					PlaceAddon( from, loc, northWall, westWall );
			}
			else
				from.SendLocalizedMessage( 1042036 ); // That location is not in your house.
		}

		private void PlaceAddon( Mobile from, Point3D loc, bool northWall, bool westWall )
		{
			if ( Deleted )
				return;

			BaseHouse house = BaseHouse.FindHouseAt( loc, from.Map, 16 );

			if ( (house == null || !house.IsCoOwner( from )) || from.AccessLevel > AccessLevel.GameMaster )
				from.SendLocalizedMessage( 1042036 ); // That location is not in your house.
			else
			{
				int itemID = 0;

				if ( northWall )
					itemID = 0x10FC;
				else if ( westWall )
					itemID = 0x110F;
				else
					from.SendLocalizedMessage( 1062840 ); // The decoration must be placed next to a wall.

				if ( itemID > 0 )
				{
					Item addon = new StoneFaceAddon( itemID );

					//addon.ItemID = itemID;
					addon.MoveToWorld( loc, from.Map );

					house.Addons.Add( addon );
					Delete();
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( ( house != null && house.IsCoOwner( from ) ) || from.AccessLevel >= AccessLevel.GameMaster )
				{
					from.SendLocalizedMessage( 1062838 ); // Where would you like to place this decoration?
					from.BeginTarget( -1, true, TargetFlags.None, new TargetStateCallback( Placement_OnTarget ), null );
				}
				else
				{
					from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
				}
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		private class StoneFaceDeedGump : Gump
		{
			private Mobile m_From;
			private Point3D m_Loc;
			private StoneFaceAddonDeed m_Deed;

			public StoneFaceDeedGump( Mobile from, Point3D loc, StoneFaceAddonDeed deed ) : base( 150, 50 )
			{
				m_From = from;
				m_Loc = loc;
				m_Deed = deed;

				AddBackground( 0, 0, 300, 150, 0xA28 );

				AddPage( 0 );

				AddItem( 90, 30, 0x110F );
				AddItem( 180, 30, 0x10FC );
				AddButton( 50, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0 );
				AddButton( 145, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0 );
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted )
					return;

				switch( info.ButtonID )
				{
					case 1:
						m_Deed.PlaceAddon( m_From, m_Loc, false, true );
						break;
					case 2:
						m_Deed.PlaceAddon( m_From, m_Loc, true, false );
						break;
				}
			}
		}
	}
}