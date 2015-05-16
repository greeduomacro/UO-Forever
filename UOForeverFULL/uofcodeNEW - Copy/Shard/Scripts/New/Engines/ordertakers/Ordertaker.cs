
using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;

namespace Server.Mobiles
{
	
	public class DismissCRVendorEntry : ContextMenuEntry
	{
		private CraftRequestVendor m_vendor;

		public DismissCRVendorEntry( CraftRequestVendor vendor ) : base( 6129, 12 ) 
		{
			m_vendor = vendor;
		}

		public override void OnClick()
		{
			m_vendor.Delete();
		}
	}
	
	public class CraftRequestVendor : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }
		
		public override bool CanTeach { get { return false; } }
		public override bool IsActiveVendor { get { return false; } }
		private Mobile m_owner;
		private Dictionary<Mobile, string> m_orders = new Dictionary<Mobile, string>();
		private bool m_neworders = false;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Owner
		{
			get{ return m_owner; }
			set{ m_owner = value; }
		}
		
		public Dictionary<Mobile, string> Orders
		{
			get {return m_orders; }
			set {m_orders = value;}
		}
		public bool NewOrders
		{
			get{ return m_neworders; }
			set{ m_neworders = value; }
		}
		
		
		[Constructable]
		public CraftRequestVendor() : base( "the order taker" )
		{
			Frozen = true;
			
		}
		
		public override void InitSBInfo()
		{
			
		}
		
		
		public override void OnDoubleClick( Mobile from )
		{
			if (from.HasGump( typeof (CraftRequestGump) ) )
				from.CloseGump( typeof (CraftRequestGump) );
			
			from.SendGump( new CraftRequestGump( 1, this, from, ref m_orders ));
		}
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( ( from == m_owner ) && from.InLOS( this ) )
				list.Add( new DismissCRVendorEntry( this ) );
		}
		
		 public override bool HandlesOnSpeech( Mobile from )
        {
            if ( from.InRange( this.Location, 5 ) )
            return true;
            
            return base.HandlesOnSpeech( from );
        }

        public override void OnSpeech(SpeechEventArgs args)  
        {
            if (XmlScript.HasTrigger(this, TriggerName.onSpeech) && UberScriptTriggers.Trigger(this, args.Mobile, TriggerName.onSpeech, null, args.Speech))
            {
                return;
            }
			string said = args.Speech.ToLower();
            Mobile from = args.Mobile;
            if ( (said.ToLower().IndexOf( "new orders" ) >= 0) && (m_owner == from) )
            {
            	if ( m_neworders )
            		this.Say( "You have some new orders!" );
            	else
            		this.Say( "You have no new orders at this time." );
            }
        }
		public CraftRequestVendor( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			
			writer.Write( (Mobile) m_owner );
			writer.Write( (int) m_orders.Count );
			foreach( KeyValuePair<Mobile, string> de in m_orders )
			{
				writer.Write( (Mobile)de.Key);
				writer.Write( (string)de.Value );
			}
			writer.Write( (bool) m_neworders );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			m_orders = new Dictionary<Mobile, string>();

			int version = reader.ReadInt();
			
			m_owner = reader.ReadMobile();
			int count = reader.ReadInt();
			for ( int i = 0; i < count; i++ )
			{
				Mobile mob = reader.ReadMobile();
				if ( mob == null )
				{
					string bad = reader.ReadString();
					continue;
				}
				m_orders.Add( mob, reader.ReadString() );
				
			}
			m_neworders = reader.ReadBool();
			Frozen = true;
		}
	}
	
	public class CraftRequestGump : Gump
	{
		private CraftRequestVendor m_vendor;
		private Mobile m_asker;
		private Dictionary<Mobile, string> order;
		private int m_page;
		
		public CraftRequestGump( int page, CraftRequestVendor vendor, Mobile asker, ref Dictionary<Mobile, string> Order ) : base( 0, 0 )
		{
			m_asker = asker;
			m_vendor = vendor;
			order = Order;
			m_page = page;
			
			
			AddBackground( 50, 50, 540, 350, 2600 );
			AddPage( 0 );
		
			
			if ( m_asker != m_vendor.Owner )
			{
				AddHtml( 264, 80, 200, 24, "Order Request", false, false );
				AddHtml( 120, 108, 420, 48, "Please enter your request", false, false );
				
				AddBackground( 100, 148, 440, 200, 3500 );
				AddTextEntry( 120, 168, 400, 200, 1153, 0, "" );
	
				AddButton( 175, 355, 2074, 2075, 1, GumpButtonType.Reply, 0 ); // Okay
				AddButton( 405, 355, 2073, 2072, 0, GumpButtonType.Reply, 0 ); // Cancel
			}
			
			else
			{
				m_vendor.NewOrders = false;
				int buttonindex = 100;
				int textvert = 122;
				int buttonvert = 122;
				int pages;
				List<Mobile> names = new List<Mobile>( order.Keys );
				pages = ( names.Count / 4 );
				if ( ( names.Count % 4) > 0 )
					pages += 1;
				AddLabel( 276, 82, 1160, "Orders" );
							
				if ( order != null && names != null && names.Count > 0 )
				{
					int counter = ((m_page - 1)*4);
					for ( int i = counter; ( (i < (counter + 4)) && (i < names.Count)); i++ )
					     {
							if ( i == counter )
							{
								AddButton( 76, buttonvert, 4017, 4018, ( i + buttonindex ), GumpButtonType.Reply, 0 );
								AddHtml( 114, textvert, 422, 51, String.Format( "{0} - {1}", names[i].Name, order[names[i]]), false, true );
							}
							else
							{
								buttonvert += 60;
								textvert += 62;
								AddButton( 76, buttonvert, 4017, 4018, ( i + buttonindex ), GumpButtonType.Reply, 0 );
								AddHtml( 114, textvert, 422, 51, String.Format( "{0} - {1}", names[i].Name, order[names[i]]), false, true );
							}
							
							
					     }
					if ( ( names.Count > 4 ) && ( m_page != pages ) )
					{
						AddButton( 448, 84, 2469, 2470, 2, GumpButtonType.Reply, 0 );
					}
					if ( m_page > 1 )
					{
						AddButton( 103, 84, 2466, 2467, 3, GumpButtonType.Reply, 0 );
					}
				}
			}
		
		}
		
			
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			
			if ( info.ButtonID == 0 )
			{
				if ( m_vendor.Owner != from )
					from.SendMessage( "Order Cancelled" );
									
			}
			if ( info.ButtonID == 1 )//DO Send Request
			{
				TextRelay entry = info.GetTextEntry( 0 );
				string text = ( entry == null ? "" : entry.Text.Trim() );
				
				if ( text.Length == 0 )
					from.SendMessage( "Nothing Ordered" );
				
				else if ( order.ContainsKey( from ) )
				{
					order[from] += String.Format( ". {0}.", text );
					from.SendMessage( "Order added to your existing order." );
				}
				else
				{
					order.Add( from, text );
					m_vendor.NewOrders = true;
					from.SendMessage( "Order Submitted." );
				}
			}
			if ( info.ButtonID == 2 )//Send next page
			{
				from.SendGump( new CraftRequestGump( m_page + 1, m_vendor, m_asker, ref order ) );
			}
			if ( info.ButtonID == 3 ) //Send Previous page
			{
				from.SendGump( new CraftRequestGump( m_page - 1, m_vendor, m_asker, ref order ) );
			}
			if ( info.ButtonID >= 100 )//Delete record
			{
				int i = ( info.ButtonID - 100 );
				List<Mobile> names = new List<Mobile>( order.Keys );
				Mobile mob = names[i];
				if ( order.ContainsKey( mob ) )
				{
					order.Remove( mob );
					from.CloseGump( typeof (CraftRequestGump) );
					from.SendGump( new CraftRequestGump( 1, m_vendor, m_asker, ref order ));
					from.SendMessage( "Order Removed" );
				}
				
				
			}
		}
	}
	
}

namespace Server.Items
{
	public class CraftRequestVendorDeed : Item
	{
		[Constructable]
		public CraftRequestVendorDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "an order taker deed";
		}
		
		public CraftRequestVendorDeed( Serial serial ) : base( serial )
		{
			
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.AccessLevel >= AccessLevel.GameMaster )
			{
				from.SendLocalizedMessage( 503248 ); // Your godly powers allow you to place this vendor whereever you wish.

				CraftRequestVendor v = new CraftRequestVendor();

				v.Direction = from.Direction & Direction.Mask;
				v.MoveToWorld( from.Location, from.Map );
				v.Owner = from;

				v.Say( "I am ready to take your orders!" );

				this.Delete();
			}
			else
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( house == null )
				{
					from.SendLocalizedMessage( 503240 ); // Vendors can only be placed in houses.	
				}
				else if ( !BaseHouse.NewVendorSystem && !house.IsFriend( from ) )
				{
					from.SendLocalizedMessage( 503242 ); // You must ask the owner of this building to name you a friend of the household in order to place a vendor here.
				}
				else if ( BaseHouse.NewVendorSystem && !house.IsOwner( from ) )
				{
					from.SendLocalizedMessage( 1062423 ); // Only the house owner can directly place vendors.  Please ask the house owner to offer you a vendor contract so that you may place a vendor in this house.
				}
				else if ( !house.Public || !house.CanPlaceNewVendor() ) 
				{
					from.SendLocalizedMessage( 503241 ); // You cannot place this vendor or barkeep.  Make sure the house is public and has sufficient storage available.
				}
				else
				{
					bool vendor, contract;
					BaseHouse.IsThereVendor( from.Location, from.Map, out vendor, out contract );

					if ( vendor )
					{
						from.SendLocalizedMessage( 1062677 ); // You cannot place a vendor or barkeep at this location.
					}
					else if ( contract )
					{
						from.SendLocalizedMessage( 1062678 ); // You cannot place a vendor or barkeep on top of a rental contract!
					}
					else
					{
						CraftRequestVendor v = new CraftRequestVendor();

						v.Direction = from.Direction & Direction.Mask;
						v.MoveToWorld( from.Location, from.Map );
						v.Owner = from;
		
						v.Say( "I am ready to take your orders!" );

						this.Delete();
					}
				}
			
			}
		}
		
		
	}
}