using System;
using Server;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Misc
{
	public class TokunoArtifactDeed : Item // Create the item class which is derived from the base item class
	{
		[Constructable]
		public TokunoArtifactDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "Tokuno Artifact Deed";
		}
		
		public TokunoArtifactDeed( Serial serial ) : base( serial )
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
		
		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendGump( new TokunoArtifactGump( from, this ) );
			}
		}
	}
	public class TokunoArtifactGump : Gump
	{
		private Item m_Deed;
		private Mobile m_Mobile;
		
		private const int LabelColor = 0x7FFF;
		private const int SelectedColor = 0x421F;
		private const int DisabledColor = 0x4210;
		
		private const int LabelColor32 = 0xFFFFFF;
		private const int SelectedColor32 = 0x8080FF;
		private const int DisabledColor32 = 0x808080;
		
		private const int LabelHue = 0x480;
		private const int YellowHue = 1161;
		private const int RedHue = 0x20;
		
		public TokunoArtifactGump( Mobile from, Item deed ) : base( 300,100 )
		{
			m_Mobile = from;
			m_Deed = deed;
			
			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;
			AddPage(0);
			AddBackground(116, 25, 514, 526, 9200);
			AddLabel(282, 58, 1149, @"Please choose your reward!");
			AddItem(410, 313, 10211);
			AddItem(410, 111, 10109);
			AddItem(147, 288, 10146);
			AddItem(153, 188, 10232);
			AddItem(150, 377, 10149);
			AddItem(151, 96, 10153);
			AddItem(410, 205, 10205);			
			AddItem(410, 421, 10147);
			AddItem(161, 487, 3834, 2219);
			AddLabel(200, 81, 0, @"Swords of Prosperity");
			AddLabel(200, 263, 0, @"Sword of the Stampede");
			AddLabel(222, 177, 0, @"Darkened Sky");
			AddLabel(234, 363, 0, @"Horselord");
			AddLabel(479, 402, 0, @"Winds Edge");
			AddLabel(466, 303, 0, @"Kasa of the Raj-In");
			AddLabel(481, 202, 0, @"Stormgrip");
			AddLabel(460, 102, 0, @"Runebeetle Carapace");
			AddLabel(190, 465, 0, @"Tome of Lost Knowledge");
			AddButton(249, 109, 4023, 248, 1, GumpButtonType.Reply, 0);
			AddButton(250, 208, 4023, 248, 2, GumpButtonType.Reply, 0);
			AddButton(250, 301, 4023, 248, 3, GumpButtonType.Reply, 0);
			AddButton(249, 398, 4023, 248, 4, GumpButtonType.Reply, 0);
			AddButton(500, 424, 4023, 248, 5, GumpButtonType.Reply, 0);
			AddButton(500, 324, 4023, 248, 6, GumpButtonType.Reply, 0);
			AddButton(500, 224, 4023, 248, 7, GumpButtonType.Reply, 0);
			AddButton(500, 124, 4023, 248, 8, GumpButtonType.Reply, 0);
			AddButton(252, 497, 4023, 248, 9, GumpButtonType.Reply, 0);
			AddButton(489, 495, 2119, 248, 0, GumpButtonType.Reply, 0);
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			switch ( info.ButtonID )
			{
				case 0:
					{
						from.CloseGump( typeof( TokunoArtifactGump ) );
						break;
					}
				case 1:
					{
						Item item = new SwordsOfProsperity();
						item.LootType = LootType.Regular;
						from.AddToBackpack( item );
						from.CloseGump( typeof( TokunoArtifactGump ) );
						m_Deed.Delete();
						
						break;
					}
					
				case 3:
					{
						Item item = new SwordOfTheStampede();
						item.LootType = LootType.Regular;
						from.AddToBackpack( item );
						from.CloseGump( typeof( TokunoArtifactGump ) );
						m_Deed.Delete();
						
						break;
					}
				case 2:
					{
						Item item = new DarkenedSky();
						item.LootType = LootType.Regular;
						from.AddToBackpack( item );
						from.CloseGump( typeof( TokunoArtifactGump ) );
						m_Deed.Delete();
						
						break;
					}
					
				case 4:
					{
						Item item = new SwordOfTheStampede();
						item.LootType = LootType.Regular;
						from.AddToBackpack( item );
						from.CloseGump( typeof( TokunoArtifactGump ) );
						m_Deed.Delete();
						
						break;
					}
				case 5:
					{
						Item item = new WindsEdge();
						item.LootType = LootType.Regular;
						from.AddToBackpack( item );
						from.CloseGump( typeof( TokunoArtifactGump ) );
						m_Deed.Delete();
						
						break;
					}
				case 6:
					{
						Item item = new SwordOfTheStampede();
						item.LootType = LootType.Blessed;
						from.AddToBackpack( item );
						from.CloseGump( typeof( TokunoArtifactGump ) );
						m_Deed.Delete();
						
						break;
					}
				case 7:
					{
						Item item = new Stormgrip();
						item.LootType = LootType.Regular;
						from.AddToBackpack( item );
						from.CloseGump( typeof( TokunoArtifactGump ) );
						m_Deed.Delete();
						
						break;
					}
				case 8:
					{
						Item item = new RuneBeetleCarapace();
						item.LootType = LootType.Regular;
						from.AddToBackpack( item );
						from.CloseGump( typeof( TokunoArtifactGump ) );
						m_Deed.Delete();
						
						break;
					}
				case 9:
					{
						Item item = new TomeOfLostKnowledge();
						item.LootType = LootType.Regular;
						from.AddToBackpack( item );
						from.CloseGump( typeof( TokunoArtifactGump ) );
						m_Deed.Delete();
						
						break;
					}
			}
		}
	}
}
