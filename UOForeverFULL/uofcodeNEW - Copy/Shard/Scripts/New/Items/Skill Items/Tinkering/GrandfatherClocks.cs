using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Spells;
using Server.Targeting;
using Server.Accounting;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public abstract class BaseGrandfatherClock : Clock, IChopable, IAddon
	{
		private static List<BaseGrandfatherClock> m_Clocks;
		private static ClockTimer m_Timer;

		public static List<BaseGrandfatherClock> Clocks{ get{ return m_Clocks; } }

		public void Add()
		{
			m_Clocks.Add( this );

			if ( m_Timer == null )
			{
				m_Timer = new ClockTimer();
				m_Timer.Start();
			}
		}

		public static void Configure()
		{
			m_Clocks = new List<BaseGrandfatherClock>();
		}

		public abstract Item Deed{ get; }

		public virtual void OnChop( Mobile from )
		{
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsOwner( from ) && house.Addons.Contains( this ) )
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

		public virtual bool CouldFit( IPoint3D p, Map map )
		{
			BaseHouse h = null;
			return ( CouldFit( new Point3D( p ), map, null, ref h ) == AddonFitResult.Valid );
		}

		public static AddonFitResult CouldFit( Point3D p, Map map, Mobile from, ref BaseHouse house )
		{
			if ( !map.CanFit( p.X, p.Y, p.Z, 5, true, true, true ) )
				return AddonFitResult.Blocked;
			else if ( !BaseAddon.CheckHouse( from, p, map, 5, ref house ) )
				return AddonFitResult.NotInHouse;
			else
				return CheckDoors( p, 5, house );
		}

		public static AddonFitResult CheckDoors( Point3D p, int height, BaseHouse house )
		{
			List<Item> doors = house.Doors;

			for ( int i = 0; i < doors.Count; i ++ )
			{
				Item door = doors[i];

				//if ( door != null && door.Open )
				//	return AddonFitResult.DoorsNotClosed;

				Point3D doorLoc = door.GetWorldLocation();
				int doorHeight = door.ItemData.CalcHeight;

				if ( Utility.InRange( doorLoc, p, 1 ) && (p.Z == doorLoc.Z || ((p.Z + height) > doorLoc.Z && (doorLoc.Z + doorHeight) > p.Z)) )
					return AddonFitResult.DoorTooClose;
			}

			return AddonFitResult.Valid;
		}

		public BaseGrandfatherClock( int itemID ) : base( itemID )
		{
			Movable = false;
			Add();
		}

		public BaseGrandfatherClock( Serial serial ) : base( serial )
		{
			Add();
		}

		public override void OnDelete()
		{
			base.OnDelete();

			m_Clocks.Remove( this );

			if ( m_Clocks.Count == 0 )
			{
				m_Timer.Stop();
				m_Timer = null;
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

		private class ClockTimer : Timer
		{
			public ClockTimer() : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				foreach ( BaseGrandfatherClock clock in BaseGrandfatherClock.Clocks )
				{
					//sanity checks
					if ( clock != null && !clock.Deleted && clock.Parent == null && (!clock.Movable || clock.IsLockedDown || clock.IsSecure) )
					{
						int hours, minutes;
						Server.Items.Clock.GetTime( clock.Map, clock.X, clock.Y, out hours, out minutes );

						if ( minutes == 0 )
							Effects.PlaySound( clock.Location, clock.Map, ( hours == 12 || hours == 0 ) ? 0x662 : 0x663 );
					}
				}
			}
		}
	}

	public enum GrandfatherClockType
	{
		Small,
		Large,
		White
	}

	public class GrandfatherClockDeed : Item, IRewardItem
	{
		private GrandfatherClockType m_ClockType;
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public GrandfatherClockType ClockType
		{
			get { return m_ClockType; }
			set { m_ClockType = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		[Constructable]
		public GrandfatherClockDeed( GrandfatherClockType type ) : base( 0x14F0 )
		{
			m_ClockType = type;

			LootType = LootType.Blessed;
			Weight = 1.0;
		}

		public GrandfatherClockDeed( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			switch ( m_ClockType )
			{
				case GrandfatherClockType.Small: list.Add( 1149901 ); break;
				case GrandfatherClockType.Large: list.Add( 1149902 ); break;
				case GrandfatherClockType.White: list.Add( 1149903 ); break;
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelToExpansion(from);

			string name = String.Empty;

			switch ( m_ClockType )
			{
				case GrandfatherClockType.Small: name = "deed to a small grandfather clock"; break;
				case GrandfatherClockType.Large: name = "deed to a large grandfather clock"; break;
				case GrandfatherClockType.White: name = "deed to a white grandfather clock"; break;
			}

			LabelTo( from, name );
		}

		public override void OnDoubleClick( Mobile from )
		{
			Account acct = from.Account as Account;

			if ( m_IsRewardItem && acct != null && from.AccessLevel == AccessLevel.Player && !Engines.VeteranRewards.RewardSystem.CheckIsUsableBy( from, this, null ) )
				return;

			if ( IsChildOf( from.Backpack ) )
			{
				from.SendMessage( "Where would you like to put this clock?" );
				from.Target = new GrandfatherClockTarget( this );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (int) m_ClockType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_ClockType = (GrandfatherClockType)reader.ReadInt();
		}
	}

	public class GrandfatherClockTarget : Target
	{
		private GrandfatherClockDeed m_Deed;

		public GrandfatherClockTarget( GrandfatherClockDeed deed ) : base( -1, true, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			IPoint3D p = targeted as IPoint3D;
			Map map = from.Map;

			if ( p == null || map == null || m_Deed == null || m_Deed.Deleted )
				return;

			if ( m_Deed.IsChildOf( from.Backpack ) )
			{
				SpellHelper.GetSurfaceTop( ref p );
				BaseHouse house = null;
				Point3D loc = new Point3D( p );

				if ( targeted is Item && !((Item) targeted).IsLockedDown && !((Item) targeted).IsSecure && !(targeted is AddonComponent) )
				{
					from.SendMessage( "Grandfather clocks can only be placed in houses." );
					return;
				}

				AddonFitResult result = CouldFit( loc, map, from, ref house );

				if ( result == AddonFitResult.Valid || from.AccessLevel >= AccessLevel.GameMaster )
				{
					BaseGrandfatherClock clock;

					switch ( m_Deed.ClockType )
					{
						default:
						case GrandfatherClockType.Small: clock = new SmallGrandfatherClock(); break;
						case GrandfatherClockType.Large: clock = new LargeGrandfatherClock(); break;
						case GrandfatherClockType.White: clock = new WhiteGrandfatherClock(); break;
					}

					if ( house != null )
						house.Addons.Add( clock );

					//if ( m_Deed is IRewardItem )
					//	clock.IsRewardItem = ( (IRewardItem) m_Deed).IsRewardItem;
					
					m_Deed.Delete();

					clock.MoveToWorld( loc, map );
				}
				else if ( result == AddonFitResult.Blocked )
					from.SendLocalizedMessage( 500269 ); // You cannot build that there.
				else if ( result == AddonFitResult.NotInHouse )
					from.SendMessage( "Grandfather clocks can only be placed in houses where you are the owner or co-owner" );
				else if ( result == AddonFitResult.DoorTooClose )
					from.SendLocalizedMessage( 500271 ); // You cannot build near the door.
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public static AddonFitResult CouldFit( Point3D p, Map map, Mobile from, ref BaseHouse house )
		{
			if ( !map.CanFit( p.X, p.Y, p.Z, 5, true, true, true ) )
				return AddonFitResult.Blocked;
			else if ( !BaseAddon.CheckHouse( from, p, map, 5, ref house ) )
				return AddonFitResult.NotInHouse;
			else
				return CheckDoors( p, 5, house );
		}

		public static AddonFitResult CheckDoors( Point3D p, int height, BaseHouse house )
		{
			List<Item> doors = house.Doors;

			for ( int i = 0; i < doors.Count; i ++ )
			{
				Item door = doors[i];

				Point3D doorLoc = door.GetWorldLocation();
				int doorHeight = door.ItemData.CalcHeight;

				if ( Utility.InRange( doorLoc, p, 1 ) && (p.Z == doorLoc.Z || ((p.Z + height) > doorLoc.Z && (doorLoc.Z + doorHeight) > p.Z)) )
					return AddonFitResult.DoorTooClose;
			}

			return AddonFitResult.Valid;
		}
	}

	public class SmallGrandfatherClock : BaseGrandfatherClock
	{
		public override Item Deed{ get{ return new GrandfatherClockDeed( GrandfatherClockType.Small ); } }

		public SmallGrandfatherClock() : base( 0x44D5 )
		{
		}

		public SmallGrandfatherClock( Serial serial ) : base( serial )
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
		}
	}

	public class LargeGrandfatherClock : BaseGrandfatherClock
	{
		public override Item Deed{ get{ return new GrandfatherClockDeed( GrandfatherClockType.Large ); } }

		public LargeGrandfatherClock() : base( 0x44DD )
		{
		}

		public LargeGrandfatherClock( Serial serial ) : base( serial )
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
		}
	}

	public class WhiteGrandfatherClock : BaseGrandfatherClock
	{
		public override Item Deed{ get{ return new GrandfatherClockDeed( GrandfatherClockType.White ); } }

		public WhiteGrandfatherClock() : base( 0x48D8 )
		{
		}

		public WhiteGrandfatherClock( Serial serial ) : base( serial )
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
		}
	}
}