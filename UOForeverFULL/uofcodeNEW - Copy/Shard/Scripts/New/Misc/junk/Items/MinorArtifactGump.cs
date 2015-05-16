//Written By WeedGod of WeedGods Workshop
using System;
using System.Net; 
using Server; 
using Server.Accounting; 
using Server.Gumps; 
using Server.Items; 
using Server.Mobiles; 
using Server.Network; 

namespace Server.Gumps 
{ 
   public class MinorArtifactGump : Gump 
   { 
      private Mobile m_Mobile;
      private Item m_Deed;
 

      public MinorArtifactGump( Mobile from, Item deed ) : base( 30, 20 ) 
      { 
         m_Mobile = from;
	     m_Deed = deed; 
	
	 AddPage( 1 ); 

		 AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5054 ); 

         AddLabel( 40, 12, 37, "Minor Artifact List" );  

         Account a = from.Account as Account; 


         AddLabel( 52, 40, 0, "Weapons" ); 
         AddButton( 12, 40, 4005, 4007, 0, GumpButtonType.Page, 2 ); 
         AddLabel( 52, 60, 0, "Armor" ); 
         AddButton( 12, 60, 4005, 4007, 0, GumpButtonType.Page, 3 ); 
         AddLabel( 52, 80, 0, "Misc" ); 
         AddButton( 12, 80, 4005, 4007, 10, GumpButtonType.Page, 4 ); 
         AddLabel( 52, 100, 0, "Shields" ); 
         AddButton( 12, 100, 4005, 4007, 0, GumpButtonType.Page, 5 );
         AddLabel( 52, 320, 0, "Close" ); 
         AddButton( 12, 320, 4005, 4007, 0, GumpButtonType.Reply, 0 ); 
	

	 AddPage( 2 ); 

         AddBackground( 0, 0, 300, 400, 5054 ); 
         AddBackground( 8, 8, 284, 384, 3000 ); 

	 AddLabel( 40, 12, 0, "Weapons" );
        	
          

         AddLabel( 52, 40, 37, "Weapons Menu" ); 
         AddButton( 12, 40, 4005, 4007, 0, GumpButtonType.Page, 2 ); 
         AddLabel( 52, 60, 37, "Misc" ); 
         AddButton( 12, 60, 4005, 4007, 0, GumpButtonType.Page, 3 ); 
         AddLabel( 52, 80, 37, "Jewelery Menu" ); 
         AddButton( 12, 80, 4005, 4007, 10, GumpButtonType.Page, 4 ); 
         AddLabel( 52, 100, 37, "Shields Menu" ); 
         AddButton( 12, 100, 4005, 4007, 0, GumpButtonType.Page, 5 );
         AddLabel( 52, 360, 37, "Close" ); 
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Reply, 0 ); 
	

	 AddPage( 2 ); 

         AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5054 ); 

	 AddLabel( 40, 12, 37, "Weapons List" );
        	
          

         AddLabel( 52, 40, 37, "Arctic Death Dealer" ); 
         AddButton( 12, 40, 4005, 4007, 1, GumpButtonType.Reply, 1 ); 
         AddLabel( 52, 60, 37, "Blaze of Death" ); 
         AddButton( 12, 60, 4005, 4007, 2, GumpButtonType.Reply, 2 ); 
         AddLabel( 52, 80, 37, "Bow of the Juka King" ); 
         AddButton( 12, 80, 4005, 4007, 3, GumpButtonType.Reply, 3 ); 
         AddLabel( 52, 100, 37, "Captain Quackle bushs Cutlass" ); 
         AddButton( 12, 100, 4005, 4007, 4, GumpButtonType.Reply, 4 ); 
         AddLabel( 52, 120, 37, "Cavorting Club" ); 
         AddButton( 12, 120, 4005, 4007, 5, GumpButtonType.Reply, 5 ); 
         AddLabel( 52, 140, 37, "Cold Blood" ); 
         AddButton( 12, 140, 4005, 4007, 6, GumpButtonType.Reply, 6 ); 
         AddLabel( 52, 160, 37, "Enchanted Titan Leg Bone" ); 
         AddButton( 12, 160, 4005, 4007, 7, GumpButtonType.Reply, 7 ); 
         AddLabel( 52, 180, 37, "Luna Lance" ); 
         AddButton( 12, 180, 4005, 4007, 8, GumpButtonType.Reply, 8 ); 
         AddLabel( 52, 200, 37, "Nights Kiss" ); 
         AddButton( 12, 200, 4005, 4007, 9, GumpButtonType.Reply, 9 ); 
         AddLabel( 52, 220, 37, "Nox Rangers Heavy Crossbow" ); 
         AddButton( 12, 220, 4005, 4007, 10, GumpButtonType.Reply, 10 ); 
         AddLabel( 52, 240, 37, "Pixie Swatter" ); 
         AddButton( 12, 240, 4005, 4007, 11, GumpButtonType.Reply, 11 );  
         AddLabel( 52, 260, 37, "Wrath Of The Dryad" ); 
         AddButton( 12, 260, 4005, 4007, 12, GumpButtonType.Reply, 12 );
         AddLabel( 52, 280, 37, "Staff of Power" );
		 AddButton( 12, 280, 4005, 4007, 13, GumpButtonType.Reply, 13 );
         
		 AddLabel( 52, 360, 37, "Main Menu" );
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Page, 1 ); 
	

         AddPage( 3 ); 

         AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5054 ); 

         AddLabel( 40, 12, 37, "Armor List" ); 

         
         AddLabel( 52, 40, 37, "Burglars Bandana" ); 
         AddButton( 12, 40, 4005, 4007, 14, GumpButtonType.Reply, 1 ); 
         AddLabel( 52, 60, 37, "Dread Pirate Hat" ); 
         AddButton( 12, 60, 4005, 4007, 15, GumpButtonType.Reply, 2 ); 
         AddLabel( 52, 80, 37, "Gloves Of The Pugilist" ); 
         AddButton( 12, 80, 4005, 4007, 24, GumpButtonType.Reply, 3 ); 
         AddLabel( 52, 100, 37, "Heart Of The Lion" ); 
         AddButton( 12, 100, 4005, 4007, 16, GumpButtonType.Reply, 4 ); 
         AddLabel( 52, 120, 37, "Orcish Visage" ); 
         AddButton( 12, 120, 4005, 4007, 17, GumpButtonType.Reply, 5 ); 
         AddLabel( 52, 140, 37, "Polar Bear Mask" ); 
         AddButton( 12, 140, 4005, 4007, 18, GumpButtonType.Reply, 6 ); 
         AddLabel( 52, 160, 37, "Violet Courage" ); 
         AddButton( 12, 160, 4005, 4007, 19, GumpButtonType.Reply, 7 );  
         

         AddLabel( 52, 360, 37, "Main Menu" ); 
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Page, 1 );
	 

	 AddPage( 4 ); 

         AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5054 );  

         AddLabel( 40, 12, 37, "Misc List" ); 

         

         AddLabel( 52, 40, 37, "Alchemists Bauble" ); 
         AddButton( 12, 40, 4005, 4007, 20, GumpButtonType.Reply, 1 ); 
         AddLabel( 52, 60, 37, "Iolos Lute" ); 
         AddButton( 12, 60, 4005, 4007, 21, GumpButtonType.Reply, 2 ); 
         AddLabel( 52, 80, 37, "Gwennos Harp" ); 
         AddButton( 12, 80, 4005, 4007, 22, GumpButtonType.Reply, 3 ); 

         AddLabel( 52, 360, 37, "Main Menu" ); 
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Page, 1 );
	  

	 AddPage( 5 ); 
  
         AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5054 );

         AddLabel( 40, 12, 37, "Shields List" ); 

         

         AddLabel( 52, 40, 37, "Shield Of Invulnerability" ); 
         AddButton( 12, 40, 4005, 4007, 23, GumpButtonType.Reply, 1 );  

         AddLabel( 52, 360, 37, "Main Menu" ); 
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Page, 1 );
      } 


      public override void OnResponse( NetState state, RelayInfo info ) 
      { 
         Mobile from = state.Mobile; 

         switch ( info.ButtonID ) 
         { 
            case 0: //Close Gump 
            { 
               from.CloseGump( typeof( MinorArtifactGump ) );	 
               break; 
            } 
             case 1: // Artic Death Dealer
            { 
		Item item = new ArcticDeathDealer();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 2: // Blaze of Death
            { 
		Item item = new BlazeOfDeath();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 3: //Bow Of The Juka King
            { 
		Item item = new BowOfTheJukaKing();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 4: //Captain Quackle bushs Cutlass
            { 
		Item item = new CaptainQuacklebushsCutlass();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 5: //Cavorting Club
            { 
		Item item = new CavortingClub();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 6: //Cold Blood
            { 
		Item item = new ColdBlood();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 7: //Enchanted Titan Leg Bone
            { 
		Item item = new EnchantedTitanLegBone();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 8: //Luna Lance
            { 
		Item item = new LunaLance();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
	    case 9: //Nights Kiss
	    { 
		Item item = new NightsKiss();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 10: //Nox Rangers Heavy Crossbow
            { 
		Item item = new NoxRangersHeavyCrossbow();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 11: //Pixie Swatter
            { 
		Item item = new PixieSwatter();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 12: //Wrath Of The Dryad
            { 
		Item item = new WrathOfTheDryad();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 13: //Staff Of Power
            { 
		Item item = new StaffOfPower();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
	    case 14: //Burglars Bandana
            { 
		Item item = new BurglarsBandana();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
        case 15: //Dread Pirate Hat
            {
        Item item = new DreadPirateHat();
        item.LootType = LootType.Blessed;
        from.AddToBackpack( item );
        from.CloseGump( typeof( MinorArtifactGump ) );
        m_Deed.Delete();
        break;
            } 
	    case 16: //Heart Of The Lion
            { 
		Item item = new HeartOfTheLion();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
	    case 17: //Orcish Visage
            { 
		Item item = new OrcishVisage();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
 	    case 18: //Polar Bear Mask
            { 
		Item item = new PolarBearMask();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 19: //Violet Courage
            { 
		Item item = new VioletCourage();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 20: //Alchemists Bauble
            { 
		Item item = new AlchemistsBauble();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 21: //Iolos Lute
            { 
		Item item = new IolosLute();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 22: //Gwennos Harp
            { 
		Item item = new GwennosHarp();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 23: //Shield Of Invulnerability
            { 
		Item item = new ShieldOfInvulnerability();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( MinorArtifactGump ) );
		m_Deed.Delete();
		break;
            }
    	case 24: //GlovesOfThePugilist
            {
        Item item = new GlovesOfThePugilist();
        item.LootType = LootType.Blessed;
        from.AddToBackpack( item );
        from.CloseGump( typeof( MinorArtifactGump ) );
        m_Deed.Delete();
        break;
            } 
         }    
      } 
   } 
} 
