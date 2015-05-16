
using System;
using System.Net;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class RunicDeed : Item
	{
		[Constructable]
		public RunicDeed() : this( null )
		{
		}
		
		[Constructable]
		public RunicDeed( string name ) : base( 0x14F0 )
		{
			Name = "Runic Tool Deed";
			LootType = LootType.Blessed;
			Hue = 1169;
		}
		
		public RunicDeed( Serial serial ) : base( serial )
		{
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 );
			}
			else
			{
				from.SendGump( new RunicGump( from, this ) );
			}
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
	
			public class RunicBag : Bag 
		{ 
		[Constructable] 
		public RunicBag() : this( 5000 ) 
		{ 
		} 

		[Constructable] 
		public RunicBag( int amount ) 
		{ 
			Name = "A bag of runic tools";
			Hue = 934;
			
			DropItem( new RunicHammer( CraftResource.DullCopper, 100 ) );
			DropItem( new RunicHammer( CraftResource.ShadowIron, 100 ) );
			DropItem( new RunicHammer( CraftResource.Copper, 100 ) );
			DropItem( new RunicHammer( CraftResource.Bronze, 100 ) );
			DropItem( new RunicHammer( CraftResource.Gold, 100 ) );
			DropItem( new RunicHammer( CraftResource.Agapite, 100 ) );
			DropItem( new RunicHammer( CraftResource.Verite, 100 ) );
			DropItem( new RunicHammer( CraftResource.Valorite, 100 ) );
			
			DropItem( new RunicSewingKit( CraftResource.SpinedLeather, 100 ) );
			DropItem( new RunicSewingKit( CraftResource.HornedLeather, 100 ) );
			DropItem( new RunicSewingKit( CraftResource.BarbedLeather, 100 ) );

		} 

		public RunicBag( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 

			writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt(); 
		} 
	} 
	
			public class RunicGump : Gump
		{
		private Mobile m_Mobile;
		private Item m_Deed;
		
		public RunicGump( Mobile from, Item deed ) : base( 30, 20 )
		{
			m_Mobile = from;
			m_Deed = deed;
			
			AddPage( 1 );
			
			AddBackground( 0, 0, 300, 120, 9200 );
			
			AddLabel( 98, 8, 1168, "Runic Tool List" );
			
			Account a = from.Account as Account;
			
			AddLabel( 52, 30, 1166, "Blacksmith" );
			AddButton( 30, 34, 2224, 2223, 0, GumpButtonType.Page, 2 );
			AddLabel( 52, 50, 1166, "Tailor" );
			AddButton( 30, 54, 2224, 2223, 0, GumpButtonType.Page, 3 );
			AddLabel( 52, 70, 1166, "Bag of All Runics" );
			AddButton( 30, 74, 2224, 2223, 12, GumpButtonType.Reply, 1 );
			AddLabel( 40, 95, 1168, "Perilous" );
			AddButton( 269, 3, 1150, 1152, 0, GumpButtonType.Reply, 0 );
			
			AddPage( 2 );
			
			AddBackground( 0, 0, 250, 250, 9200 );
			
			AddLabel( 90, 8, 1152, "Blacksmith" );
			
			AddLabel( 52, 40, 2401, "Dull Copper" );
			AddButton( 30, 44, 2224, 2223, 1, GumpButtonType.Reply, 1 );
			AddLabel( 52, 60, 2405, "Shadow" );
			AddButton( 30, 64, 2224, 2223, 2, GumpButtonType.Reply, 2 );
			AddLabel( 52, 80, 2412, "Copper" );
			AddButton( 30, 84, 2224, 2223, 3, GumpButtonType.Reply, 3 );
			AddLabel( 52, 100, 2417, "Bronze" );
			AddButton( 30, 104, 2224, 2223, 4, GumpButtonType.Reply, 4 );
			AddLabel( 52, 120, 2214, "Gold" );
			AddButton( 30, 124, 2224, 2223, 5, GumpButtonType.Reply, 5 );
			AddLabel( 52, 140, 2424, "Agapite" );
			AddButton( 30, 144, 2224, 2223, 6, GumpButtonType.Reply, 6 );
			AddLabel( 52, 160, 2206, "Verite" );
			AddButton( 30, 164, 2224, 2223, 7, GumpButtonType.Reply, 7 );
			AddLabel( 52, 180, 2118, "Valorite" );
			AddButton( 30, 184, 2224, 2223, 8, GumpButtonType.Reply, 8 );
			
			AddItem( 203, 4, 4017 );
			AddItem( 168, 10, 4015 );
			
			AddLabel( 52, 220, 1152, "Main Menu" );
			AddButton( 20, 219, 4014, 4016, 0, GumpButtonType.Page, 1 );
			
			AddPage( 3 );
			
			AddBackground( 0, 0, 250, 130, 9200 );
			
			AddLabel( 98, 8, 1152, "Tailor" );
			
			AddLabel( 52, 30, 2118, "Spined" );
			AddButton( 30, 34, 2224, 2223, 9, GumpButtonType.Reply, 1 );
			AddLabel( 52, 50, 2117, "Horned" );
			AddButton( 30, 54, 2224, 2223, 10, GumpButtonType.Reply, 2 );
			AddLabel( 52, 70, 2128, "Barbed" );
			AddButton( 30, 74, 2224, 2223, 11, GumpButtonType.Reply, 3 );
			
			AddItem( 202, 7, 3997 );
			AddItem( 195, 15, 3999 );
			
			AddLabel( 52, 100, 1152, "Main Menu" );
			AddButton( 20, 99, 4014, 4016, 0, GumpButtonType.Page, 1 );
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			
			switch( info.ButtonID )
			{
				case 0:
					{
						from.CloseGump( typeof( RunicGump ) );
						break;
					}
				case 1:
					{
						Item item = new RunicHammer( CraftResource.DullCopper, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 2:
					{
						Item item = new RunicHammer( CraftResource.ShadowIron, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 3:
					{
						Item item = new RunicHammer( CraftResource.Copper, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 4:
					{
						Item item = new RunicHammer( CraftResource.Bronze, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 5:
					{
						Item item = new RunicHammer( CraftResource.Gold, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 6:
					{
						Item item = new RunicHammer( CraftResource.Agapite, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 7:
					{
						Item item = new RunicHammer( CraftResource.Verite, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 8:
					{
						Item item = new RunicHammer( CraftResource.Valorite, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 9:
					{
						Item item = new RunicSewingKit( CraftResource.SpinedLeather, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 10:
					{
						Item item = new RunicSewingKit( CraftResource.HornedLeather, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 11:
					{
						Item item = new RunicSewingKit( CraftResource.BarbedLeather, 100 );
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						break;
					}
				case 12:
					{
						Item item = new RunicBag();
						from.AddToBackpack( item );
						break;
					}
			}
		}
	}
}


