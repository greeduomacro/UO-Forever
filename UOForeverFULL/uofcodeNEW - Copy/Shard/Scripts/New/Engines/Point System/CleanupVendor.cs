using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.BulkOrders;
using Server.Network;
using Server.Commands;

namespace Server.Engines.Cleanup
{


	public class DonationVendor : BaseVendor
	{

		private class DonationItemEntry
		{	
			private Type m_Type;
			private int m_Price;
			private string m_Name;

			public Type Type{ get{ return m_Type; } }
			public int Price{ get{ return m_Price; } }
			public string Name{ get{ return m_Name; } }

			public DonationItemEntry( Type t, int price)
			{
				m_Type = t;
				m_Price = price;
			    m_Name = String.Empty;
			}
		}

		private static DonationItemEntry[] DonationItemDeeds = new DonationItemEntry[]
		{
			//new DonationItemEntry(typeof(HairRestylingDeed),500),
			//new DonationItemEntry(typeof(BeardRestylingDeed),500),
			new DonationItemEntry(typeof(GenderChangeDeed),500),
			new DonationItemEntry(typeof(PetBondDeed),1000),
			new DonationItemEntry(typeof(BallOfSummoning),1500),
			new DonationItemEntry(typeof(PowderOfTranslocation),100),
			//new DonationItemEntry(typeof(CommodityDeedBox),1000),
			new DonationItemEntry(typeof(SashLayerDeed),2500),			
			new DonationItemEntry(typeof(BlackDyeTub),2000),
		    new DonationItemEntry(typeof(LeatherDyeTub),2000),
		    new DonationItemEntry(typeof(FurnitureDyeTub),2000),
			new DonationItemEntry(typeof(RunebookDyeTub),2000),			
			new DonationItemEntry(typeof(SpecialDyeTub),2000),
			new DonationItemEntry(typeof(RewardStatue1AddonDeed),5000),
			//new DonationItemEntry(typeof(HorseFlagAddonDeed),2000),
			//new DonationItemEntry(typeof(BloodSwordFlagAddonDeed),2000),
			//new DonationItemEntry(typeof(PirateFlagAddonDeed),2000),
			//new DonationItemEntry(typeof(SpellbookFlagAddonDeed),2000),
			new DonationItemEntry(typeof(HaunterMirrorDeed),2000),
			//new DonationItemEntry(typeof(garishbannerAddonDeed),1000),
			new DonationItemEntry(typeof(LilacBushAddonDeed),2000),
			new DonationItemEntry(typeof(SacrificialAltarDeed),4000),
			//new DonationItemEntry(typeof(longtableAddonDeed),2000),
            new DonationItemEntry(typeof(CoconutPalmTreeAddonDeed),3000),
            new DonationItemEntry(typeof(DatePalmTreeAddonDeed),3000),
            new DonationItemEntry(typeof(TallBananaTreeAddonDeed),3000),
            new DonationItemEntry(typeof(SmallBananaTreeAddonDeed),2000),
            new DonationItemEntry(typeof(BigWillowTreeAddonDeed),2000),
            new DonationItemEntry(typeof(CypressTreeAddonDeed),2000),
            new DonationItemEntry(typeof(DeadTreeAddonDeed),2000),
            new DonationItemEntry(typeof(AmethystTreeAddonDeed),2000),
            new DonationItemEntry(typeof(TreasurePileAddonDeed),3000),
            new DonationItemEntry(typeof(BannerDeed),2000),
            new DonationItemEntry(typeof(WallBannerDeed),3500),
            new DonationItemEntry(typeof(CurtainsDeed),3500),
            new DonationItemEntry(typeof(PianoAddonDeed),5000),
			
		};

		private static DonationItemEntry[] DonationItemEthereal = new DonationItemEntry[]
		{
			new DonationItemEntry(typeof(EtherealHorse),1000),
			//new DonationItemEntry(typeof(EtherealLlama),1000),
			new DonationItemEntry(typeof(EtherealOstard),1000),
			//new DonationItemEntry(typeof(EtherealFrenziedOstard),2000),
			//new DonationItemEntry(typeof(EtherealKirin),3000),
			//new DonationItemEntry(typeof(EtherealRidgeback),2500),
			new DonationItemEntry(typeof(EtherealBeetle),4000),
			//new DonationItemEntry(typeof(EtherealUnicorn),4000),
			new DonationItemEntry(typeof(EtherealSwampDragon),3000),
			//new DonationItemEntry(typeof(EtherealPolarBear),2500),
			new DonationItemEntry(typeof(EtherealCuSidhe),4000),
			//new DonationItemEntry(typeof(EtherealHiryu),4000),
			new DonationItemEntry(typeof(EtherealReptalon),5000),
			//new DonationItemEntry(typeof(EtherealSkeletalSteed),5000 ) REMOVED FOREVER
		}; 


		private static DonationItemEntry[]DonationItemWears = new DonationItemEntry[]
		{
			new DonationItemEntry(typeof (SpecialHairDye),1000),
			new DonationItemEntry(typeof (SpecialBeardDye),1000), 
		    //new DonationItemEntry (typeof (DonationRed), 2000),
			new DonationItemEntry(typeof (AnkhNecklace), 2000),
			//new DonationItemEntry(typeof (DonationNeonBlue), 3500),
			new DonationItemEntry(typeof (DonationNeonGreen), 3500),
			//new DonationItemEntry (typeof (DonationNeonPurple), 3500),
            new DonationItemEntry (typeof (DonationNeonPink), 3500),
			//new DonationItemEntry (typeof (DonationFalonSandal), 6000),
			new DonationItemEntry (typeof (DonationVesperSandal), 5000),
			//new DonationItemEntry(typeof(DonationBlack), 2500),
			//new DonationItemEntry(typeof(DonationBlazeSandal), 9000), REMOVED FOREVER
			//new DonationItemEntry(typeof(DonationDeerShadow), 4500),
			//new DonationItemEntry (typeof (DonationNeonRed), 2500), REMOVED FOREVER,
			//new DonationItemEntry (typeof (DonationFalonMask), 4500),
			//new DonationItemEntry (typeof (DonationAcid), 3000),
			new DonationItemEntry (typeof (DonationCharcoalBoneHelm), 9000),
            new DonationItemEntry (typeof (DonationBearCharcoal), 4000),
            new DonationItemEntry (typeof (DonationTribalCharcoal), 4500),
			//new DonationItemEntry (typeof (DonationTribalVesper), 8000),
			new DonationItemEntry (typeof (DonationWhite), 3500),
			new DonationItemEntry (typeof (DonationPureWhiteMask), 4500),
			//new DonationItemEntry (typeof (DonationCharcoalNMask), 4500),
			new DonationItemEntry (typeof (DonationPurpleNMask), 4500),
			new DonationItemEntry (typeof (DonationDeerVerite), 6000),
			new DonationItemEntry (typeof (ClothingBlessDeed), 1000),
            new DonationItemEntry (typeof (SpellbookBlessDeed), 1000),
			new DonationItemEntry (typeof (HouseLadderDeed), 2000),
			new DonationItemEntry (typeof (HearthOfHomeFireDeed), 2000),
			new DonationItemEntry (typeof (BloodyTableAddonDeed), 2500),
			new DonationItemEntry (typeof (GargishbenchAddonDeed), 1000),
			//new DonationItemEntry (typeof (DawnsMusicBox),2000),
			new DonationItemEntry (typeof (BloodyAnkhAddonDeed), 2000),
			new DonationItemEntry (typeof (CannonDeed), 1000),
			new DonationItemEntry (typeof (BloodyPentagramDeed), 2000),
			new DonationItemEntry (typeof (ArcaneCircleDeed), 1000),
			//new DonationItemEntry (typeof (hex_FirepSouthAddonDeed), 2000),
			//new DonationItemEntry (typeof (GarishBedEastAddonDeed), 1000),
			//new DonationItemEntry (typeof (hex_StoveEastAddonDeed), 1000),
			new DonationItemEntry (typeof (FirePillarAddonDeed), 1000),
			new DonationItemEntry (typeof (PottedPlantDeed), 1000),
			new DonationItemEntry (typeof (AppleTreeDeed), 2000),
			new DonationItemEntry (typeof (PeachTreeDeed), 2000),
			new DonationItemEntry (typeof (CherryBlossomTreeDeed), 3000),
			new DonationItemEntry (typeof (BoneSetAddonDeed), 1000),
			new DonationItemEntry (typeof (DisplayCaseAddonDeed), 1000),
		};
				
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public DonationVendor( Serial serial ) : base( serial )
		{
			m_SBInfos = new List<SBInfo>();
		}

		public override string DefaultName{ get{ return "Donation System"; } }

		[Constructable]
		public DonationVendor() : base("")
		{
			CantWalk = true;
			Blessed = true;
			m_SBInfos = new List<SBInfo>();
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

		public override void VendorBuy( Mobile buyer )
		{
			buyer.CloseGump( typeof ( DonationVendorGump ));
			buyer.SendGump( new DonationVendorGump(buyer) );
		}

		public override void VendorSell( Mobile from )
		{
		}

		public override void InitSBInfo()
		{
		}

		private class DonationVendorGump : Gump
		{
			private Mobile m_Player;

			public DonationVendorGump(Mobile player) : base(50, 200)//Mobile from, DonationVendor vend) : base(50, 50)
			{
				m_Player = player;

				AddPage(0);
		
				//AddBackground(0,0,200,200,0x53);
				AddBackground(0,0,360,135,3600);
				AddBackground(15,15,330,105,3000);

				// deeds
				AddImageTiledButton(30,30,0x918,0x919,1,GumpButtonType.Reply,0,0x14F0,0,14,11);//0xbbc,0xbbc,1,GumpButtonType.Reply,0,0x22e0,0,50,50);
				AddHtml(30,95,80,20,"<CENTER>Deeds</CENTER>", false, false);
				// deco
				AddImageTiledButton(140,30,0x918,0x919,2,GumpButtonType.Reply,0,0x20DD,0,20,15);//0xbbc,0xbbc,1,GumpButtonType.Reply,0,0x22e0,0,50,50);
				AddHtml(140,95,80,20,"<CENTER>Ethereal</CENTER>", false, false);
				// resources
				AddImageTiledButton(250,30,0x918,0x919,3,GumpButtonType.Reply,0,0x170D,0,30,15);//0xbbc,0xbbc,1,GumpButtonType.Reply,0,0x22e0,0,50,50);
				AddHtml(250,95,80,20,"<CENTER>Wearables</CENTER>", false, false);
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				Mobile from = sender.Mobile;
                try
                {
                    switch (info.ButtonID)
                    {
                        case 1:
                            from.CloseGump(typeof(DonationVendorGump));
                            from.SendGump(new DonationItemsGump(1, m_Player));
                            break;
                        case 2:
                            from.CloseGump(typeof(DonationVendorGump));
                            from.SendGump(new DonationItemsGump(2, m_Player));
                            break;
                        case 3:
                            from.CloseGump(typeof(DonationVendorGump));
                            from.SendGump(new DonationItemsGump(3, m_Player));
                            break;
                        case 4:
                            from.CloseGump(typeof(DonationVendorGump));
                            from.SendGump(new DonationItemsGump(4, m_Player));
                            break;
                        default:
                            from.SendMessage("You Decide not to purchase at this time");
                            break;
                    }
                }
                catch (Exception e)
                {
					LoggingCustom.Log("ERROR_DonationVendor", "Mobile: " + sender.Mobile + "Error with donation vendor: " + e.Message + "\n" + e.StackTrace);
                }
			}
		}

		internal class DonationItemsGump : Gump
		{
			private DonationItemEntry[] m_Set;
			private Mobile m_Player;

			public DonationItemsGump(int set, Mobile player) : base( 50, 200 )
			{
                try
                {
                    m_Player = player;

                    switch (set)
                    {
                        case 1:
                            m_Set = DonationItemDeeds;
                            break;
                        case 2:
                            m_Set = DonationItemEthereal;
                            break;
                        case 3:
                            m_Set = DonationItemWears;
                            break;
                        default:
                            m_Set = new DonationItemEntry[0];
                            break;
                    }
                    LoadMe();
                }
                catch (Exception e)
                {
					LoggingCustom.Log("ERROR_DonationVendor", "Mobile: " + player + "Error with donation vendor: " + e.Message + "\n" + e.StackTrace);
                }
			}

			private void LoadMe()
			{
				AddPage(0);
		
				int height = m_Set.Length * 80 + 40;
				int next = 30;

				//AddBackground(0,0,200,200,0x53);
				AddBackground(0,0,300,height,3600);//135,3600);
				AddBackground(15,15,270,height-30,3000);//105,3000);

				//AddImageTiledButton(30,30,0x918,0x919,1,GumpButtonType.Reply,0,0x14F0,0,14,11);//0xbbc,0xbbc,1,GumpButtonType.Reply,0,0x22e0,0,50,50);
				//AddHtml(30,95,80,20,"<CENTER>Deeds</CENTER>", false, false);
				if (m_Set.Length > 0)
				{
					for (int i=0;i<m_Set.Length;i++)
					{
						Item x = Activator.CreateInstance(m_Set[i].Type) as Item;

						string name = m_Set[i].Name;
						int price = m_Set[i].Price;

						AddImageTiledButton(30, next, 0x918, 0x919, i+300, GumpButtonType.Reply, 0, x.ItemID, x.Hue, 12, 10);
   	//				AddLabelCropped(120, next, 80, 60, 1149, String.Format("{0}\n{1} points", x.Name, 10000));
						AddHtml(120, next, 150, 60, String.Format("{0}<br>{1} Donation Coins", GetNameString(x), price), false, false);

						x.Delete();

						next += 80;
					}
				}
			}

		private string GetNameString(Item item)
			{
				/*if (item is BagOfReagents)
					return "50 each regeant";
				
				if (item is DAxeEastDeed)
					return "decorative axes";

				if (item is DSwordEastDeed)
					return "decorative swords";

				if (item is BottleDeed)
					return "1000 empty bottles";
				

				if (item is ScrollDeed)
					return "1000 blank scrolls";
            */
                if (item == null)
                {
                    return "null";
                }
				if (item.Name != null)
				{
					return item.Name;
				}
				else if (item.DefaultName != null)
				{
					return item.DefaultName;
				}
				else if (item.LabelNumber > 0)
				{
					return String.Format("{0}",StringList.Default[item.LabelNumber].Text);
				}
				else
				{
					return "no name";
				}
			} 

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				Mobile from = sender.Mobile;

                try
                {
                    if (info.ButtonID >= 300)
                    {
                        int pr = m_Set[info.ButtonID - 300].Price;
                        int amnt = 0;
                        List<Item> dd = new List<Item>();

                        for (int i = 0; i < from.BankBox.Items.Count; i++)
                        {
                            if (from.BankBox.Items[i] is DonationCoin)
                            {
                                amnt += from.BankBox.Items[i].Amount;
                                dd.Add(from.BankBox.Items[i]);
                            }
                        }

                        if (dd.Count == 0)
                        {
                            from.SendMessage("You don't have any Donation Coins!");
                        }
                        else if (dd.Count > 0)
                        {
                            if (pr > amnt)
                            {
                                from.SendMessage("You cannot afford that!");
                            }
                            else
                            {
                                for (int i = 0; i < dd.Count; i++)
                                {
                                    if (dd[i].Amount < pr)
                                    {
                                        pr -= dd[i].Amount;
                                        dd[i].Consume(dd[i].Amount);
                                    }
                                    else
                                    {
                                        dd[i].Consume(pr);
                                        pr = 0;
                                    }
                                }

                                Item d = Activator.CreateInstance(m_Set[info.ButtonID - 300].Type) as Item;

                                if (d is Spellbook)
                                {
                                    d.LootType = LootType.Newbied;
                                }
                                else if (!(d is BaseContainer))
                                    d.LootType = LootType.Blessed;
                                BankBox bank = from.BankBox;
                                from.SendMessage("The item has been placed in your bank and the Donation Coins was deducted from your bank account.");
                                bank.DropItem(d);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    from.SendMessage("Invalid Option Selected.");
					LoggingCustom.Log("ERROR_DonationVendor", "Error with donation vendor: " + e.Message + "\n" + e.StackTrace);
                }
			}
		}
	}
}

 /*    public class BottleDeed : CommodityDeed
	{
		[Constructable]
		public BottleDeed() : this( 1000 )
		{
		}

		[Constructable]
		public BottleDeed( int amount ) : base(new Bottle( amount ))
		{
		}

		public BottleDeed(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class ScrollDeed : CommodityDeed
	{
		[Constructable]
		public ScrollDeed() : this( 1000 )
		{
		}

		[Constructable]
		public ScrollDeed( int amount ) : base(new BlankScroll( amount ))
		{
		}

		public ScrollDeed(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}*/