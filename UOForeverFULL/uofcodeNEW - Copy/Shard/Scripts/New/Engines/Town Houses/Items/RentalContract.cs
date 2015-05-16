using System;
using System.Collections;
using System.Linq;
using Server;
using Server.Multis;
using Server.Items;

namespace Knives.TownHouses
{
	public class RentalContract : TownHouseSign
	{
	    private Mobile c_RentalClient;
	    private bool c_Completed, c_EntireHouse;

	    public BaseHouse ParentHouse { get; private set; }
	    public Mobile RentalClient{ get{ return c_RentalClient; } set{ c_RentalClient = value; InvalidateProperties(); } }
	    public Mobile RentalMaster { get; private set; }
	    public bool Completed{ get{ return c_Completed; } set{ c_Completed = value; } }
		public bool EntireHouse{ get{ return c_EntireHouse; } set{ c_EntireHouse = value; } }

		public RentalContract()
		{
			ItemID = 0x14F0;
			Movable = true;
			RentByTime = TimeSpan.FromDays( 1 );
			RecurRent = true;
			MaxZ = MinZ;
		}

		public bool HasContractedArea( Rectangle2D rect, int z )
		{
			foreach( TownHouseSign item in AllSigns )
				if ( item is RentalContract && item != this && item.Map == Map && ParentHouse == ((RentalContract)item).ParentHouse )
					foreach( Rectangle2D rect2 in item.Blocks )
						for( int x = rect.Start.X; x < rect.End.X; ++x )
							for( int y = rect.Start.Y; y < rect.End.Y; ++y )
								if ( rect2.Contains( new Point2D( x, y ) ) )
									if ( item.MinZ <= z && item.MaxZ >= z )
										return true;

			return false;
		}

		public bool HasContractedArea( int z )
		{
		    return AllSigns.Cast<Item>().Where(item => item is RentalContract && item != this && item.Map == Map && ParentHouse == ((RentalContract) item).ParentHouse).Any(item => ((RentalContract) item).MinZ <= z && ((RentalContract) item).MaxZ >= z);
		}

	    private void DepositTo( Mobile m )
		{
			if ( m == null )
				return;

			if ( Free )
			{
				m.SendMessage( "Since this home is free, you do not receive the deposit." );
				return;
			}

			m.BankBox.DropItem( new Gold( Price ) );
			m.SendMessage( "You have received a {0} gold deposit from your town house.", Price );
		}

		public override void ValidateOwnership()
		{
			if ( c_Completed && RentalMaster == null )
			{
				Delete();
				return;
			}

			if ( c_RentalClient != null && ( ParentHouse == null || ParentHouse.Deleted ) )
			{
				Delete();
				return;
			}

			if ( c_RentalClient != null && !Owned )
			{
				Delete();
				return;
			}

			if ( ParentHouse == null )
				return;

			if ( !ValidateLocSec() )
			{
				if ( DemolishTimer == null )
					BeginDemolishTimer( TimeSpan.FromHours( 48 ) );
			}
			else
				ClearDemolishTimer();
		}

		protected override void DemolishAlert()
		{
			if ( ParentHouse == null || RentalMaster == null || c_RentalClient == null )
				return;

			RentalMaster.SendMessage( "You have begun to use lockdowns reserved for {0}, and their rental unit will collapse in {1}.", c_RentalClient.Name, Math.Round( (DemolishTime-DateTime.Now).TotalHours, 2 ) );
			c_RentalClient.SendMessage( "Alert your land lord, {0}, they are using storage reserved for you.  They have violated the rental agreement, which will end in {1} if nothing is done.", RentalMaster.Name, Math.Round( (DemolishTime-DateTime.Now).TotalHours, 2 ) );
		}

		public void FixLocSec()
		{
			int count = 0;

			if ( (count = General.RemainingSecures( ParentHouse )+Secures) < Secures )
				Secures = count;

			if ( (count = General.RemainingLocks( ParentHouse )+Locks) < Locks )
				Locks = count;
		}

		public bool ValidateLocSec()
		{
			if ( General.RemainingSecures( ParentHouse )+Secures < Secures )
				return false;

			return General.RemainingLocks( ParentHouse )+Locks >= Locks;
		}

	    protected override void ConvertItems( bool keep )
		{
			if ( House == null || ParentHouse == null || RentalMaster == null )
				return;

			foreach (BaseDoor door in new ArrayList( ParentHouse.Doors ).Cast<BaseDoor>().Where(door => door.Map == House.Map && House.Region.Contains( door.Location )))
			    ConvertDoor( door );

			foreach (SecureInfo info in new ArrayList( ParentHouse.Secures ).Cast<SecureInfo>().Where(info => info.Item.Map == House.Map && House.Region.Contains( info.Item.Location )))
			    ParentHouse.Release( RentalMaster, info.Item );

			foreach (Item item in new ArrayList( ParentHouse.LockDowns ).Cast<Item>().Where(item => item.Map == House.Map && House.Region.Contains( item.Location )))
			    ParentHouse.Release( RentalMaster, item );
		}

	    protected override void UnconvertDoors( )
		{
			if ( House == null || ParentHouse == null )
				return;

			foreach( BaseDoor door in new ArrayList( House.Doors ) )
				House.Doors.Remove( door );
		}

		protected override void OnRentPaid()
		{
			if ( RentalMaster == null || c_RentalClient == null )
				return;

			if ( Free )
				return;

			RentalMaster.BankBox.DropItem( new Gold( Price ) );
			RentalMaster.SendMessage( "The bank has transfered your rent from {0}.", c_RentalClient.Name );
		}

		public override void ClearHouse()
		{
			if ( !Deleted )
				Delete();

			base.ClearHouse();
		}

		public override void OnDoubleClick( Mobile m )
		{
			ValidateOwnership();

			if ( Deleted )
				return;

			if ( RentalMaster == null )
				RentalMaster = m;

			BaseHouse house = BaseHouse.FindHouseAt( m );

			if ( ParentHouse == null )
				ParentHouse = house;

			if ( house == null || ( house != ParentHouse && house != House ) )
			{
				m.SendMessage( "You must be in the home to view this contract." );
				return;
			}

			if ( m == RentalMaster
			 && !c_Completed
			 && house is TownHouse
			 && ((TownHouse)house).ForSaleSign.PriceType != "Sale" )
			{
				ParentHouse = null;
				m.SendMessage( "You can only rent property you own." );
				return;
			}

			if ( m == RentalMaster && !c_Completed && General.EntireHouseContracted( ParentHouse ) )
			{
				m.SendMessage( "This entire house already has a rental contract." );
				return;
			}

			if ( c_Completed )
				new ContractConfirmGump( m, this );
			else if ( m == RentalMaster )
				new ContractSetupGump( m, this );
			else
				m.SendMessage( "This rental contract has not yet been completed." );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			if ( c_RentalClient != null )
				list.Add( "a house rental contract with " + c_RentalClient.Name );
			else if ( c_Completed )
				list.Add( "a completed house rental contract" );
			else
				list.Add( "an uncompleted house rental contract" );
		}

		public override void Delete()
		{
			if ( ParentHouse == null )
			{
				base.Delete();
				return;
			}

			if ( !Owned && !ParentHouse.IsFriend( c_RentalClient ) )
			{
				if ( c_RentalClient != null && RentalMaster != null )
				{
					RentalMaster.SendMessage( "{0} has ended your rental agreement.  Because you revoked their access, their last payment will be refunded.", RentalMaster.Name );
					c_RentalClient.SendMessage( "You have ended your rental agreement with {0}.  Because your access was revoked, your last payment is refunded.", c_RentalClient.Name );
				}

				DepositTo( c_RentalClient );
			}
			else if ( Owned )
			{
				if ( c_RentalClient != null && RentalMaster != null )
				{
					c_RentalClient.SendMessage( "{0} has ended your rental agreement.  Since they broke the contract, your are refunded the last payment.", RentalMaster.Name );
					RentalMaster.SendMessage( "You have ended your rental agreement with {0}.  They will be refunded their last payment.", c_RentalClient.Name );
				}

				DepositTo( c_RentalClient );

				PackUpHouse();
			}
			else
			{
				if ( c_RentalClient != null && RentalMaster != null )
				{
					RentalMaster.SendMessage( "{0} has ended your rental agreement.", c_RentalClient.Name );
					c_RentalClient.SendMessage( "You have ended your rental agreement with {0}.", RentalMaster.Name );
				}

				DepositTo( RentalMaster );
			}

			ClearRentTimer();
			base.Delete();
		}

		public RentalContract( Serial serial ) : base( serial )
		{
			RecurRent = true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 ); // version

			// Version 1

			writer.Write( c_EntireHouse );

			writer.Write( RentalMaster );
			writer.Write( c_RentalClient );
			writer.Write( ParentHouse );
			writer.Write( c_Completed );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version >= 1 )
				c_EntireHouse = reader.ReadBool();

			RentalMaster = reader.ReadMobile();
			c_RentalClient = reader.ReadMobile();
			ParentHouse = reader.ReadItem() as BaseHouse;
			c_Completed = reader.ReadBool();
		}
	}
}