#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Server.ContextMenus;
using Server.Engines.BulkOrders;
using Server.Engines.XmlSpawner2;
using Server.Factions;
using Server.Games;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;
#endregion

namespace Server.Mobiles
{
	public enum VendorShoeType
	{
		None,
		Shoes,
		Boots,
		Sandals,
		ThighBoots
	}

	public abstract class BaseVendor : BaseCreature, IVendor
	{
		private const int MaxSell = 500;

		protected abstract List<SBInfo> SBInfos { get; }

		private readonly List<IBuyItemInfo> m_BuyInfo = new List<IBuyItemInfo>();
		private readonly List<IShopSellInfo> m_SellInfo = new List<IShopSellInfo>();

		public override bool CanTeach { get { return true; } }

		public override bool BardImmune { get { return true; } }

		public override bool PlayerRangeSensitive { get { return true; } }

		public virtual Type TypeOfCurrency { get { return Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold); } }

		public virtual bool IsActiveVendor { get { return true; } }
		public virtual bool IsActiveBuyer { get { return IsActiveVendor; } } // response to vendor SELL
		public virtual bool IsActiveSeller { get { return IsActiveVendor; } } // repsonse to vendor BUY

		public virtual NpcGuild NpcGuild { get { return NpcGuild.None; } }

		public virtual bool IsInvulnerable { get { return false; } }

		public override bool ShowFameTitle { get { return false; } }
		/*
		[CommandProperty( AccessLevel.GameMaster )]
		public override Faction FactionAllegiance
		{
			get
			{
				Town town = Town.FromRegion( Region );
				if ( town != null )
					return town.Owner;
				return null;
			}
		}
		*/

		public virtual bool IsValidBulkOrder(Item item)
		{
			return false;
		}

		public virtual Item CreateBulkOrder(Mobile from, bool fromContextMenu)
		{
			return null;
		}

		public virtual bool SupportsBulkOrders(Mobile from)
		{
			return false;
		}

		public virtual TimeSpan GetNextBulkOrder(Mobile from)
		{
			return TimeSpan.Zero;
		}

		public virtual void OnSuccessfulBulkOrderReceive(Mobile from, Item dropped)
		{ }

		#region Faction
		public virtual int GetPriceScalar()
		{
			Town town = Town.FromRegion(Region);

			if (town != null)
			{
				return (100 + town.Tax);
			}

			return 100;
		}

		public void UpdateBuyInfo()
		{
			int priceScalar = GetPriceScalar();

			IBuyItemInfo[] buyinfo = m_BuyInfo.ToArray();

			foreach (IBuyItemInfo info in buyinfo)
			{
				info.PriceScalar = priceScalar;
			}
		}
		#endregion

		#region Begging
		public override bool CanBeBegged(Mobile from)
		{
			return Backpack != null || BaseHalloweenGiftGiver.IsHalloween();
		}

		//Its probably easier to just steal from them
		public override void OnBegged(Mobile beggar)
		{
			/*if (TrickorTreatPersistence.TrickTreat(this, beggar))
			{
				return;
			}*/

			Container theirPack = Backpack;

			double badKarmaChance = 0.5 - ((double)beggar.Karma / 8570);

			if (theirPack != null)
			{
				if (beggar.Karma < 0 && badKarmaChance > Utility.RandomDouble())
				{
					PublicOverheadMessage(
						MessageType.Regular,
						SpeechHue,
						true,
						String.Format("Thou dost not look trustworthy... no {0} for thee today!", TypeOfCurrency.Name));
				}
				else if (beggar.CheckTargetSkill(SkillName.Begging, this, 0.0, 100.0))
				{
					int toConsume = theirPack.GetAmount(TypeOfCurrency, true) / 10;
					int max = Math.Max(Math.Min(10 + (beggar.Fame / 2500), 14), 10);

					if (toConsume > 0)
					{
						if (toConsume > max)
						{
							toConsume = max;
						}

						int consumed = theirPack.ConsumeUpTo(TypeOfCurrency, toConsume, true);

						if (consumed > 0)
						{
							PublicOverheadMessage(MessageType.Regular, SpeechHue, 500405); // I feel sorry for thee...

							var currency = TypeOfCurrency.CreateInstanceSafe<Item>();

							currency.Stackable = true;
							currency.Amount = consumed;

							beggar.AddToBackpack(currency);
							beggar.PlaySound(currency.GetDropSound());

							if (beggar.Karma > -3000)
							{
								int toLose = beggar.Karma + 3000;

								if (toLose > 40)
								{
									toLose = 40;
								}

								Titles.AwardKarma(beggar, -toLose, true);
							}
						}
						else
						{
							PublicOverheadMessage(MessageType.Regular, SpeechHue, 500407); // I have not enough money to give thee any!
						}
					}
					else
					{
						PublicOverheadMessage(MessageType.Regular, SpeechHue, 500407); // I have not enough money to give thee any!
					}
				}

				beggar.NextSkillTime = DateTime.UtcNow + TimeSpan.FromSeconds(7.0);
			}
			else
			{
				PublicOverheadMessage(MessageType.Regular, SpeechHue, 500407); // I have not enough money to give thee any!
			}
		}
		#endregion

		private class BulkOrderInfoEntry : ContextMenuEntry
		{
			private readonly Mobile m_From;
			private readonly BaseVendor m_Vendor;

			public BulkOrderInfoEntry(Mobile from, BaseVendor vendor)
				: base(6152)
			{
				m_From = from;
				m_Vendor = vendor;
			}

			public override void OnClick()
			{
				if (m_Vendor.SupportsBulkOrders(m_From))
				{
					if (m_From is PlayerMobile && ((PlayerMobile)m_From).Young)
					{
						m_From.SendMessage("Young players cannot receive bulk order requests.");
						return;
					}

					TimeSpan ts = m_Vendor.GetNextBulkOrder(m_From);

					var totalSeconds = (int)ts.TotalSeconds;
					int totalHours = (totalSeconds + 3599) / 3600;
					int totalMinutes = (totalSeconds + 59) / 60;

					if (((m_Vendor.EraSE) ? totalMinutes == 0 : totalHours == 0))
					{
						m_From.SendLocalizedMessage(1049038); // You can get an order now.

						if (m_Vendor.EraAOS)
						{
							Item bulkOrder = m_Vendor.CreateBulkOrder(m_From, true);

							if (bulkOrder is LargeBOD)
							{
								m_From.SendGump(new LargeBODAcceptGump(m_From, (LargeBOD)bulkOrder));
							}
							else if (bulkOrder is SmallBOD)
							{
								m_From.SendGump(new SmallBODAcceptGump(m_From, (SmallBOD)bulkOrder));
							}
						}
					}
					else
					{
						int oldSpeechHue = m_Vendor.SpeechHue;
						m_Vendor.SpeechHue = 0x3B2;

						if (m_Vendor.EraSE)
						{
							m_Vendor.SayTo(m_From, 1072058, totalMinutes.ToString(CultureInfo.InvariantCulture));
							// An offer may be available in about ~1_minutes~ minutes.
						}
						else
						{
							m_Vendor.SayTo(m_From, 1049039, totalHours.ToString(CultureInfo.InvariantCulture));
								// An offer may be available in about ~1_hours~ hours.
						}

						m_Vendor.SpeechHue = oldSpeechHue;
					}
				}
			}
		}

		public BaseVendor(string title)
			: base(
				AIType.AI_Vendor, FightMode.None, 2, 1, (Utility.RandomDouble() * 0.1) + 0.5, (Utility.RandomDouble() * 0.2) + 2)
		{
			//LoadSBInfo();

			Title = title;
			InitBody();
			InitOutfit();

			//these packs MUST exist, or the client will crash when the packets are sent
			AddItem(
				new Backpack
				{
					Layer = Layer.ShopBuy,
					Movable = false,
					Visible = false
				});
			AddItem(
				new Backpack
				{
					Layer = Layer.ShopResale,
					Movable = false,
					Visible = false
				});

			LastRestock = DateTime.UtcNow;

			SetSkill(SkillName.Wrestling, 100.0);
			SetSkill(SkillName.Tactics, 100.0);
		}

		public BaseVendor(Serial serial)
			: base(serial)
		{ }

		[CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
		public DateTime LastRestock { get; set; }

		public virtual TimeSpan RestockDelay { get { return TimeSpan.FromHours(1); } }

		public Container BuyPack
		{
			get
			{
				var pack = FindItemOnLayer(Layer.ShopBuy) as Container;

				if (pack == null)
				{
					AddItem(
						pack = new Backpack
						{
							Layer = Layer.ShopBuy,
							Visible = false
						});
				}

				return pack;
			}
		}

		public abstract void InitSBInfo();

		public virtual bool IsTokunoVendor { get { return Map == Map.Tokuno; } }

		protected void LoadSBInfo()
		{
			LastRestock = DateTime.UtcNow;

			foreach (GenericBuyInfo buy in m_BuyInfo.OfType<GenericBuyInfo>())
			{
				buy.DeleteDisplayEntity();
			}

			SBInfos.Clear();

			InitSBInfo();

			m_BuyInfo.Clear();
			m_SellInfo.Clear();

			foreach (SBInfo sbInfo in SBInfos)
			{
				m_BuyInfo.AddRange(sbInfo.BuyInfo);
				m_SellInfo.Add(sbInfo.SellInfo);
			}
		}

		public virtual bool GetGender()
		{
			return Utility.RandomBool();
		}

		public virtual void InitBody()
		{
			InitStats(100, 100, 25);

			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();

			if (IsInvulnerable && !EraAOS)
			{
				NameHue = 0x35;
			}

			Female = GetGender();

			if (Female)
			{
				Body = 0x191;
				Name = NameList.RandomName("female");
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName("male");
			}
		}

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (IsInvulnerable && !EraAOS)
			{
				NameHue = 0x35;
			}
		}

		public virtual int GetRandomHue()
		{
			int hue = 0;

			do
			{
				switch (Utility.Random(5))
				{
					case 0:
						hue = Utility.RandomBlueHue();
						break;
					case 1:
						hue = Utility.RandomGreenHue();
						break;
					case 2:
						hue = Utility.RandomRedHue();
						break;
					case 3:
						hue = Utility.RandomYellowHue();
						break;
					case 4:
						hue = Utility.RandomNeutralHue();
						break;
				}
			}
			while (IsResourceHue(hue));

			return hue;
		}

		public static bool IsResourceHue(int hue)
		{
			return CraftResources.m_MetalInfo.Any(t => hue == t.Hue) //
				   || CraftResources.m_LeatherInfo.Any(t => hue == t.Hue) //
				   || CraftResources.m_ScaleInfo.Any(t => hue == t.Hue);
		}

		public virtual int GetShoeHue()
		{
			if (0.1 > Utility.RandomDouble())
			{
				return 0;
			}

			return Utility.RandomNeutralHue();
		}

		public virtual VendorShoeType ShoeType { get { return VendorShoeType.Shoes; } }

		public virtual int RandomBrightHue()
		{
			if (0.1 > Utility.RandomDouble())
			{
				return Utility.RandomList(0x62, 0x71);
			}

			return Utility.RandomList(0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59);
		}

		public virtual void CheckMorph()
		{
			if (CheckGargoyle())
			{
				return;
			}

			if (CheckNecromancer())
			{
				return;
			}

			CheckTokuno();
		}

		public virtual bool CheckTokuno()
		{
			if (Map != Map.Tokuno)
			{
				return false;
			}

			NameList n = NameList.GetNameList(Female ? "tokuno female" : "tokuno male");

			if (!n.ContainsName(Name))
			{
				TurnToTokuno();
			}

			return true;
		}

		public virtual void TurnToTokuno()
		{
			Name = NameList.RandomName(Female ? "tokuno female" : "tokuno male");
		}

		public virtual bool CheckGargoyle()
		{
			Map map = Map;

			if (map != Map.Ilshenar)
			{
				return false;
			}

			if (!Region.IsPartOf("Gargoyle City"))
			{
				return false;
			}

			if (Body != 0x2F6 || (Hue & 0x8000) == 0)
			{
				TurnToGargoyle();
			}

			return true;
		}

		public virtual bool CheckNecromancer()
		{
			Map map = Map;

			if (map != Map.Malas)
			{
				return false;
			}

			if (!Region.IsPartOf("Umbra"))
			{
				return false;
			}

			if (Hue != 0x83E8)
			{
				TurnToNecromancer();
			}

			return true;
		}

		public override void OnAfterSpawn()
		{
			CheckMorph();
		}

		protected override void OnMapChange(Map oldMap)
		{
			base.OnMapChange(oldMap);

			CheckMorph();

			LoadSBInfo();
		}

		public virtual int GetRandomNecromancerHue()
		{
			switch (Utility.Random(20))
			{
				case 0:
					return 0;
				case 1:
					return 0x4E9;
				default:
					return Utility.RandomList(0x485, 0x497);
			}
		}

		public virtual void TurnToNecromancer()
		{
			foreach (Item item in Items)
			{
				if (item is Hair || item is Beard)
				{
					item.Hue = 0;
				}
				else if (item is BaseClothing || item is BaseWeapon || item is BaseArmor || item is BaseTool)
				{
					item.Hue = GetRandomNecromancerHue();
				}
			}

			HairHue = 0;
			FacialHairHue = 0;

			Hue = 0x83E8;
		}

		public virtual void TurnToGargoyle()
		{
			foreach (Item item in Items)
			{
				if (item is BaseClothing || item is Hair || item is Beard)
				{
					item.Delete();
				}
			}

			HairItemID = 0;
			FacialHairItemID = 0;

			Body = 0x2F6;
			Hue = RandomBrightHue() | 0x8000;
			Name = NameList.RandomName("gargoyle vendor");

			CapitalizeTitle();
		}

		public virtual void CapitalizeTitle()
		{
			string title = Title;

			if (title == null)
			{
				return;
			}

			string[] split = title.Split(' ');

			for (int i = 0; i < split.Length; ++i)
			{
				if (Insensitive.Equals(split[i], "the"))
				{
					continue;
				}

				if (split[i].Length > 1)
				{
					split[i] = Char.ToUpper(split[i][0]) + split[i].Substring(1);
				}
				else if (split[i].Length > 0)
				{
					split[i] = Char.ToUpper(split[i][0]).ToString(CultureInfo.InvariantCulture);
				}
			}

			Title = String.Join(" ", split);
		}

		public virtual int GetHairHue()
		{
			return Utility.RandomHairHue();
		}

		public virtual void InitOutfit()
		{
			switch (Utility.Random(3))
			{
				case 0:
					AddItem(new FancyShirt(GetRandomHue()));
					break;
				case 1:
					AddItem(new Doublet(GetRandomHue()));
					break;
				case 2:
					AddItem(new Shirt(GetRandomHue()));
					break;
			}

			switch (ShoeType)
			{
				case VendorShoeType.Shoes:
					AddItem(new Shoes(GetShoeHue()));
					break;
				case VendorShoeType.Boots:
					AddItem(new Boots(GetShoeHue()));
					break;
				case VendorShoeType.Sandals:
					AddItem(new Sandals(GetShoeHue()));
					break;
				case VendorShoeType.ThighBoots:
					AddItem(new ThighBoots(GetShoeHue()));
					break;
			}

			int hairHue = GetHairHue();

			Utility.AssignRandomHair(this, hairHue);
			Utility.AssignRandomFacialHair(this, hairHue);

			if (Female)
			{
				switch (Utility.Random(6))
				{
					case 0:
						AddItem(new ShortPants(GetRandomHue()));
						break;
					case 1:
					case 2:
						AddItem(new Kilt(GetRandomHue()));
						break;
					case 3:
					case 4:
					case 5:
						AddItem(new Skirt(GetRandomHue()));
						break;
				}
			}
			else
			{
				switch (Utility.Random(2))
				{
					case 0:
						AddItem(new LongPants(GetRandomHue()));
						break;
					case 1:
						AddItem(new ShortPants(GetRandomHue()));
						break;
				}
			}

			PackGold(10, 25);
		}

		public virtual void Restock()
		{
			LastRestock = DateTime.UtcNow;

			IBuyItemInfo[] buyInfo = GetBuyInfo();

			foreach (IBuyItemInfo bii in buyInfo)
			{
				bii.OnRestock();
			}
		}

		private static readonly TimeSpan InventoryDecayTime = TimeSpan.FromHours(1.0);
		private const int InventoryBuyPackLimit = 2500;

		public virtual void VendorBuy(Mobile buyer)
		{
			if (!IsActiveSeller)
			{
				return;
			}

			if (!buyer.CheckAlive())
			{
				return;
			}

			if (Home != Point3D.Zero)
			{
				if (Math.Abs(Location.X - Home.X) > 30 || Math.Abs(Location.Y - Home.Y) > 30)
				{
					Location = Home;
					return;
				}
			}
			//Faction fact = Faction.Find( from );

			if (!CheckVendorAccess(buyer))
			{
				Say(501522); // I shall not treat with scum like thee!
				return;
			}
			/*else if ( FactionAllegiance != null && fact != null && FactionAllegiance != fact )
			{
				Say( "I will not do business with the enemy!" );
				return;
			}*/

			if (DateTime.UtcNow - LastRestock > RestockDelay)
			{
				Restock();
			}

			UpdateBuyInfo();

			//int count = 0;
			IBuyItemInfo[] buyInfo = GetBuyInfo();
			IShopSellInfo[] sellInfo = GetSellInfo();

			var list = new List<BuyItemState>(buyInfo.Length);
			Container cont = BuyPack;

			List<ObjectPropertyList> opls = null;

			foreach (IBuyItemInfo buyItem in buyInfo)
			{
				if (buyItem.Amount <= 0 || list.Count >= 250)
				{
					continue;
				}

				// NOTE: Only GBI supported; if you use another implementation of IBuyItemInfo, this will crash
				var gbi = (GenericBuyInfo)buyItem;
				IEntity disp = gbi.GetDisplayEntity();

				if (disp is Item && ((Item)disp).IsPhased)
				{
					continue;
				}

				list.Add(
					new BuyItemState(
						buyItem.Name,
						cont.Serial,
						disp == null ? (Serial)0x7FC0FFEE : disp.Serial,
						buyItem.Price,
						buyItem.Amount,
						buyItem.ItemID,
						buyItem.Hue));
				//count++;

				if (opls == null)
				{
					opls = new List<ObjectPropertyList>();
				}

				if (disp is Item)
				{
					opls.Add(((Item)disp).PropertyList);
				}
				else if (disp is Mobile)
				{
					opls.Add(((Mobile)disp).PropertyList);
				}
			}

			List<Item> playerItems = cont.Items;

			for (int i = playerItems.Count - 1; i >= 0; --i)
			{
				if (i >= playerItems.Count)
				{
					continue;
				}

				Item item = playerItems[i];

				if ((item.LastMoved + InventoryDecayTime) <= DateTime.UtcNow)
				{
					item.Delete();
				}
			}

			foreach (Item item in playerItems)
			{
				if (item == null || item.IsPhased)
				{
					continue;
				}

				int price = 0;
				string name = null;

				foreach (IShopSellInfo ssi in sellInfo.Where(ssi => ssi.IsSellable(item)))
				{
					price = ssi.GetBuyPriceFor(item);
					name = ssi.GetNameFor(item);
					break;
				}

				if (name == null || list.Count >= 250)
				{
					continue;
				}

				list.Add(new BuyItemState(name, cont.Serial, item.Serial, price, item.Amount, item.ItemID, item.Hue));
				//count++;

				if (opls == null)
				{
					opls = new List<ObjectPropertyList>();
				}

				opls.Add(item.PropertyList);
			}

			//one (not all) of the packets uses a byte to describe number of items in the list.  Osi = dumb.
			//if ( list.Count > 255 )
			//	Console.WriteLine( "Vendor Warning: Vendor {0} has more than 255 buy items, may cause client errors!", this );

			if (list.Count <= 0)
			{
				return;
			}

			list.Sort(new BuyItemStateComparer());

			SendPacksTo(buyer);

			NetState ns = buyer.NetState;

			if (ns == null)
			{
				return;
			}

			if (ns.ContainerGridLines)
			{
				buyer.Send(new VendorBuyContent6017(list));
			}
			else
			{
				buyer.Send(new VendorBuyContent(list));
			}

			buyer.Send(new VendorBuyList(this, list));

			if (ns.HighSeas)
			{
				buyer.Send(new DisplayBuyListHS(this));
			}
			else
			{
				buyer.Send(new DisplayBuyList(this));
			}

			buyer.Send(new MobileStatusExtended(buyer)); //make sure their gold amount is sent

			if (opls != null)
			{
				foreach (ObjectPropertyList opl in opls)
				{
					buyer.Send(opl);
				}
			}

			SayTo(buyer, 500186); // Greetings.  Have a look around.
		}

		public virtual void SendPacksTo(Mobile from)
		{
			Item pack = FindItemOnLayer(Layer.ShopBuy);

			if (pack == null)
			{
				AddItem(
					new Backpack
					{
						Layer = Layer.ShopBuy,
						Movable = false,
						Visible = false
					});
			}

			from.Send(new EquipUpdate(pack));

			pack = FindItemOnLayer(Layer.ShopSell);

			if (pack != null)
			{
				from.Send(new EquipUpdate(pack));
			}

			pack = FindItemOnLayer(Layer.ShopResale);

			if (pack == null)
			{
				AddItem(
					new Backpack
					{
						Layer = Layer.ShopResale,
						Movable = false,
						Visible = false
					});
			}

			from.Send(new EquipUpdate(pack));
		}

		public virtual void VendorSell(Mobile from)
		{
			if (!IsActiveBuyer)
			{
				return;
			}

			if (!from.CheckAlive())
			{
				return;
			}

			if (Home != Point3D.Zero)
			{
				if (Math.Abs(Location.X - Home.X) > 30 || Math.Abs(Location.Y - Home.Y) > 30)
				{
					Location = Home;
					return;
				}
			}

			// fact = Faction.Find( from );

			if (!CheckVendorAccess(from))
			{
				Say(501522); // I shall not treat with scum like thee!
				return;
			}
			/*else if ( FactionAllegiance != null && fact != null && FactionAllegiance != fact )
			{
				Say( "I will not do business with the enemy!" );
				return;
			}*/

			Container cont = BuyPack;
			List<Item> playerItems = cont.Items;

			for (int i = playerItems.Count - 1; i >= 0; --i)
			{
				if (i >= playerItems.Count)
				{
					continue;
				}

				Item item = playerItems[i];

				if ((item.LastMoved + InventoryDecayTime) <= DateTime.UtcNow)
				{
					item.Delete();
				}
			}

			if (playerItems.Count > InventoryBuyPackLimit)
			{
				Say(true, "I cannot purchase more at this time.");
			}
			else
			{
				Container pack = from.Backpack;

				if (pack != null)
				{
					IShopSellInfo[] info = GetSellInfo();

					var table = new Dictionary<Item, SellItemState>();

					foreach (IShopSellInfo ssi in info)
					{
						Item[] items = pack.FindItemsByType(ssi.Types);

						foreach (Item item in
							items.Where(item => !(item is Container) || item.Items.Count == 0)
								 .Where(item => item.IsStandardLoot() && item.Movable && ssi.IsSellable(item)))
						{
							table[item] = new SellItemState(item, ssi.GetSellPriceFor(item), ssi.GetNameFor(item));
						}
					}

					if (table.Count > 0)
					{
						SendPacksTo(from);

						from.Send(new VendorSellList(this, table));
					}
					else
					{
						Say(true, "You have nothing I would be interested in.");
					}
				}
			}
		}

		public override bool OnDragDrop(Mobile m, Item dropped)
		{
			// trigger returns true if returnoverride
			if (XmlScript.HasTrigger(this, TriggerName.onDragDrop) &&
				UberScriptTriggers.Trigger(this, m, TriggerName.onDragDrop, dropped))
			{
				return true;
			}

			/* TODO: Thou art giving me? and fame/karma for gold gifts */

			if (dropped is SmallBOD || dropped is LargeBOD)
			{
				var pm = m as PlayerMobile;

				if (EraML && pm != null && pm.NextBODTurnInTime > DateTime.UtcNow)
				{
					SayTo(m, 1079976); // You'll have to wait a few seconds while I inspect the last order.
				}
				else
				{
					var sbod = dropped as SmallBOD;
					var lbod = dropped as LargeBOD;

					if (!IsValidBulkOrder(dropped) || !SupportsBulkOrders(m))
					{
						SayTo(m, 1045130); // That order is for some other shopkeeper.
					}
					else if ((sbod != null && !sbod.Complete) || (lbod != null && !lbod.Complete))
					{
						SayTo(m, 1045131); // You have not completed the order yet.
					}
					else
					{
						Item reward;
						int currency, fame;

						if (sbod != null)
						{
							sbod.GetRewards(out reward, out currency, out fame);
						}
						else
						{
							lbod.GetRewards(out reward, out currency, out fame);
						}

						if (reward != null || currency > 0)
						{
							Item rewardCurrency = null;

							if (currency > PseudoSeerStone.MaxBODGoldRewardAllowed)
							{
								currency = PseudoSeerStone.MaxBODGoldRewardAllowed;
							}

							if (currency > 1000)
							{
								rewardCurrency = new BankCheck(currency);
							}
							else if (currency > 0)
							{
								rewardCurrency = new Gold(currency);
							}

							/*if ( ( reward != null && !from.Backpack.CheckHold( from, reward, false ) ) || ( rewardgold != null && !from.Backpack.CheckHold( from, rewardgold, false ) ) )*/

							var items = new List<Item>();

							if (reward != null)
							{
								m.SendMessage(String.Format("Attempting to drop a reward {0} ({1})", reward.GetType(), reward.PileWeight));
								items.Add(reward);
							}

							if (rewardCurrency != null)
							{
								m.SendMessage(
									String.Format("Attempting to drop a reward {0} ({1})", rewardCurrency.GetType(), rewardCurrency.PileWeight));
								items.Add(rewardCurrency);
							}

							if (items.Count > 0 && m.Backpack.TryDropItems(m, false, items.ToArray()))
							{
								m.SendSound(0x3D);
								SayTo(m, 1045132); // Thank you so much!  Here is a reward for your effort.

								Titles.AwardFame(m, fame, true);

								OnSuccessfulBulkOrderReceive(m, dropped);

								if (EraML && pm != null)
								{
									pm.NextBODTurnInTime = DateTime.UtcNow + TimeSpan.FromSeconds(10.0);
								}

								dropped.Delete();
								return true;
							}

							SayTo(m, 1045129); // You do not have enough room in your backpack for the bulk request's reward.

							if (reward != null)
							{
								reward.Delete();
							}

							if (rewardCurrency != null)
							{
								rewardCurrency.Delete();
							}
						}
					}
				}

				return false;
			}

			return base.OnDragDrop(m, dropped);
		}

		private GenericBuyInfo LookupDisplayObject(object obj)
		{
			return GetBuyInfo().OfType<GenericBuyInfo>().FirstOrDefault(gbi => gbi.GetDisplayEntity() == obj);
		}

		private static void ProcessSinglePurchase(
			BuyItemResponse buy,
			IBuyItemInfo bii,
			List<BuyItemResponse> validBuy,
			ref int controlSlots,
			ref bool fullPurchase,
			ref int totalCost)
		{
			int amount = buy.Amount;

			if (amount > bii.Amount)
			{
				amount = bii.Amount;
			}

			if (amount <= 0)
			{
				return;
			}

			int slots = bii.ControlSlots * amount;

			if (controlSlots >= slots)
			{
				controlSlots -= slots;
			}
			else
			{
				fullPurchase = false;
				return;
			}

			totalCost += bii.Price * amount;
			validBuy.Add(buy);
		}

		private static void ProcessValidPurchase(int amount, IBuyItemInfo bii, Mobile buyer, Container cont)
		{
			if (amount > bii.Amount)
			{
				amount = bii.Amount;
			}

			if (amount < 1)
			{
				return;
			}

			bii.Amount -= amount;

			IEntity o = bii.GetEntity();

			if (o is Item)
			{
				var item = (Item)o;

				if (item.Stackable)
				{
					item.Amount = amount;

					if (cont is BankBox)
					{
						buyer.LocalOverheadMessage(
							MessageType.Regular, 0x38, false, "Given the cost of the items, they will be delivered to your bank.");
					}

					if (cont == null || !cont.TryDropItem(buyer, item, false))
					{
						// try to put it in the bank
						cont = buyer.BankBox;
						if (cont == null || !cont.TryDropItem(buyer, item, false))
						{
							//buyer.SendMessage(38, "You don't have enough room for that in your pack OR in your bank. It has been placed on the ground!");
							buyer.LocalOverheadMessage(
								MessageType.Regular,
								38,
								false,
								"You don't have enough room for that in your pack OR in your bank. It has been placed on the ground!");
							item.MoveToWorld(buyer.Location, buyer.Map);
						}
						else
						{
							//buyer.SendMessage(0x38, "Since you could not carry the item, it has been delivered to your bank.");   
							buyer.LocalOverheadMessage(
								MessageType.Regular, 38, false, "Since you could not carry the item, it has been delivered to your bank.");
						}
					}
				}
				else
				{
					item.Amount = 1;

					if (cont is BankBox)
					{
						buyer.LocalOverheadMessage(
							MessageType.Regular, 0x38, false, "Given the cost of the items, they will be delivered to your bank.");
					}

					if (cont == null || !cont.TryDropItem(buyer, item, false))
					{
						// try to put it in the bank
						cont = buyer.BankBox;

						if (cont == null || !cont.TryDropItem(buyer, item, false))
						{
							buyer.LocalOverheadMessage(
								MessageType.Regular,
								38,
								false,
								"You don't have enough room for that in your pack OR in your bank. It has been placed on the ground!");
							//buyer.SendMessage(38, );
							item.MoveToWorld(buyer.Location, buyer.Map);
						}
						else
						{
							buyer.LocalOverheadMessage(
								MessageType.Regular, 38, false, "Since you could not carry the item, it has been delivered to your bank.");
							//buyer.SendMessage(0x38, "Since you could not carry the item, it has been delivered to your bank.");
						}
					}

					for (int i = 1; i < amount; i++)
					{
						item = bii.GetEntity() as Item;

						if (item == null)
						{
							continue;
						}

						item.Amount = 1;

						if (cont != null && cont.TryDropItem(buyer, item, false))
						{
							continue;
						}

						// try to put it in the bank
						cont = buyer.BankBox;

						if (cont == null || !cont.TryDropItem(buyer, item, false))
						{
							//buyer.SendMessage(38, "You don't have enough room for that in your pack OR in your bank. It has been placed on the ground!");
							buyer.LocalOverheadMessage(
								MessageType.Regular,
								38,
								false,
								"You don't have enough room for that in your pack OR in your bank. It has been placed on the ground!");
							item.MoveToWorld(buyer.Location, buyer.Map);
						}
						else
						{
							//buyer.SendMessage(0x38, "Since you could not carry the item, it has been delivered to your bank.");
							buyer.LocalOverheadMessage(
								MessageType.Regular, 38, false, "Since you could not carry the item, it has been delivered to your bank.");
						}
					}

					if (item is BaseBoatDeed && buyer is PlayerMobile)
					{
						Timer.DelayCall(TimeSpan.FromSeconds(3.0), ((PlayerMobile)buyer).BoatDeedWarning);
					}
				}
			}
			else if (o is Mobile)
			{
				var m = (Mobile)o;

				m.Direction = (Direction)Utility.Random(8);
				m.MoveToWorld(buyer.Location, buyer.Map);
				m.PlaySound(m.GetIdleSound());

				if (m is BaseCreature)
				{
					((BaseCreature)m).SetControlMaster(buyer);
				}

				for (int i = 1; i < amount; ++i)
				{
					m = bii.GetEntity() as Mobile;

					if (m == null)
					{
						continue;
					}

					m.Direction = (Direction)Utility.Random(8);
					m.MoveToWorld(buyer.Location, buyer.Map);

					if (m is BaseCreature)
					{
						((BaseCreature)m).SetControlMaster(buyer);
					}
				}
			}
		}

		public virtual bool OnBuyItems(Mobile buyer, List<BuyItemResponse> list)
		{
			if (!IsActiveSeller)
			{
				return false;
			}

			if (!buyer.CheckAlive())
			{
				return false;
			}

			//Faction fact = Faction.Find( buyer );

			if (!CheckVendorAccess(buyer))
			{
				Say(501522); // I shall not treat with scum like thee!
				return false;
			}
			/*else if ( FactionAllegiance != null && fact != null && FactionAllegiance != fact )
			{
				Say( "I will not do business with the enemy!" );
				return false;
			}*/

			UpdateBuyInfo();

			//IBuyItemInfo[] buyInfo = GetBuyInfo();
			IShopSellInfo[] info = GetSellInfo();
			int totalCost = 0;
			var validBuy = new List<BuyItemResponse>(list.Count);
			Container cont;
			//bool fromBank = false;
			bool fullPurchase = true;
			int controlSlots = buyer.FollowersMax - buyer.Followers;

			foreach (BuyItemResponse buy in list)
			{
				Serial ser = buy.Serial;
				int amount = buy.Amount;

				if (ser.IsItem)
				{
					Item item = World.FindItem(ser);

					if (item == null)
					{
						continue;
					}

					GenericBuyInfo gbi = LookupDisplayObject(item);

					if (gbi != null)
					{
						ProcessSinglePurchase(buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost);
					}
					else if (item != BuyPack && item.IsChildOf(BuyPack))
					{
						if (amount > item.Amount)
						{
							amount = item.Amount;
						}

						if (amount <= 0)
						{
							continue;
						}

						foreach (IShopSellInfo ssi in info.Where(ssi => ssi.IsSellable(item) && ssi.IsResellable(item)))
						{
							totalCost += ssi.GetBuyPriceFor(item) * amount;
							validBuy.Add(buy);
							break;
						}
					}
				}
				else if (ser.IsMobile)
				{
					Mobile mob = World.FindMobile(ser);

					if (mob == null)
					{
						continue;
					}

					GenericBuyInfo gbi = LookupDisplayObject(mob);

					if (gbi != null)
					{
						ProcessSinglePurchase(buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost);
					}
				}
			} //foreach

			if (fullPurchase && validBuy.Count == 0)
			{
				SayTo(buyer, 500190); // Thou hast bought nothing!
			}
			else if (validBuy.Count == 0)
			{
				SayTo(buyer, 500187); // Your order cannot be fulfilled, please try again.
			}

			if (validBuy.Count == 0)
			{
				return false;
			}

			bool bought = buyer.AccessLevel >= AccessLevel.GameMaster;

			if (!bought)
			{
				if (Banker.WithdrawPackAndBank(buyer, TypeOfCurrency, totalCost))
				{
					bought = true;
				}
				else
				{
					SayTo(buyer, 500192); //Begging thy pardon, but thou casnt afford that.
				}
			}

			if (bought)
			{
				buyer.PlaySound(0x32);

				cont = buyer.Backpack;

				if (cont == null || totalCost > 5000)
				{
					cont = buyer.BankBox;
				}

				foreach (BuyItemResponse buy in validBuy)
				{
					Serial ser = buy.Serial;
					int amount = buy.Amount;

					if (amount < 1)
					{
						continue;
					}

					if (ser.IsItem)
					{
						Item item = World.FindItem(ser);

						if (item == null)
						{
							continue;
						}

						GenericBuyInfo gbi = LookupDisplayObject(item);

						if (gbi != null)
						{
							ProcessValidPurchase(amount, gbi, buyer, cont);
						}
						else
						{
							if (amount > item.Amount)
							{
								amount = item.Amount;
							}

							if (info.Any(ssi => ssi.IsSellable(item) && ssi.IsResellable(item)))
							{
								Item buyItem;

								if (amount >= item.Amount)
								{
									buyItem = item;
								}
								else
								{
									buyItem = LiftItemDupe(item, item.Amount - amount) ?? item;
								}

								if (cont == null || !cont.TryDropItem(buyer, buyItem, false))
								{
									buyItem.MoveToWorld(buyer.Location, buyer.Map);
								}
							}
						}
					}
					else if (ser.IsMobile)
					{
						Mobile mob = World.FindMobile(ser);

						if (mob == null)
						{
							continue;
						}

						GenericBuyInfo gbi = LookupDisplayObject(mob);

						if (gbi != null)
						{
							ProcessValidPurchase(amount, gbi, buyer, cont);
						}
					}
				} //foreach

				if (fullPurchase)
				{
					if (buyer.AccessLevel >= AccessLevel.GameMaster)
					{
						SayTo(buyer, true, "I would not presume to charge thee anything.  Here are the goods you requested.");
					}
					else
					{
						SayTo(
							buyer,
							true,
							"The total of thy purchase is {0} {1}.  My thanks for the patronage.",
							totalCost,
							TypeOfCurrency.Name);
					}
				}
				else
				{
					if (buyer.AccessLevel >= AccessLevel.GameMaster)
					{
						SayTo(
							buyer,
							true,
							"I would not presume to charge thee anything.  Unfortunately, I could not sell you all the goods you requested.");
					}
					else
					{
						SayTo(
							buyer,
							true,
							"The total of thy purchase is {0} {1}.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.",
							totalCost,
							TypeOfCurrency.Name);
					}
				}

				return true;
			}

			return false;
		}

		public virtual bool CheckVendorAccess(Mobile from)
		{
			var reg = (GuardedRegion)Region.GetRegion(typeof(GuardedRegion));

			if (reg != null && !reg.CheckVendorAccess(this, from))
			{
				return false;
			}

			if (Region != from.Region)
			{
				reg = (GuardedRegion)from.Region.GetRegion(typeof(GuardedRegion));

				if (reg != null && !reg.CheckVendorAccess(this, from))
				{
					return false;
				}
			}

			//Faction fact = Faction.Find( from );

			//if ( FactionAllegiance != null && fact != null && FactionAllegiance != fact )
			//	return false;

			return true;
		}

		public virtual bool OnSellItems(Mobile seller, List<SellItemResponse> list)
		{
			if (!IsActiveBuyer)
			{
				return false;
			}

			if (!seller.CheckAlive())
			{
				return false;
			}

			//Faction fact = Faction.Find( seller );

			if (!CheckVendorAccess(seller))
			{
				Say(501522); // I shall not treat with scum like thee!
				return false;
			}
			/*else if ( FactionAllegiance != null && fact != null && FactionAllegiance != fact )
			{
				Say( "I will not do business with the enemy!" );
				return false;
			}*/

			seller.PlaySound(0x32);

			IShopSellInfo[] info = GetSellInfo();
			IBuyItemInfo[] buyInfo = GetBuyInfo();
			int giveCurrency = 0;
			Container cont;

			int sold =
				list.Where(r => r.Item.RootParent == seller && r.Amount > 0)
					.Where(r => r.Item.IsStandardLoot() && r.Item.Movable)
					.Where(r => !(r.Item is Container) || r.Item.Items.Count == 0)
					.Count(r => info.Any(ssi => ssi.IsSellable(r.Item)));

			if (sold > MaxSell)
			{
				SayTo(seller, true, "You may only sell {0} items at a time!", MaxSell);
				return false;
			}

			if (sold == 0)
			{
				return true;
			}

			foreach (SellItemResponse r in
				list.Where(r => r.Item.RootParent == seller && r.Amount > 0)
					.Where(r => r.Item.IsStandardLoot() && r.Item.Movable)
					.Where(r => !(r.Item is Container) || r.Item.Items.Count == 0))
			{
				foreach (IShopSellInfo ssi in info)
				{
					if (!ssi.IsSellable(r.Item))
					{
						continue;
					}

					int amount = r.Amount;

					if (amount > r.Item.Amount)
					{
						amount = r.Item.Amount;
					}

					if (ssi.IsResellable(r.Item))
					{
						bool found = false;

						if (buyInfo.Any(bii => bii.Restock(r.Item, amount)))
						{
							r.Item.Consume(amount);
							found = true;
						}

						if (!found)
						{
							cont = BuyPack;

							if (amount < r.Item.Amount)
							{
								Item item = LiftItemDupe(r.Item, r.Item.Amount - amount);

								if (item != null)
								{
									item.SetLastMoved();
									cont.DropItem(item);
								}
								else
								{
									r.Item.SetLastMoved();
									cont.DropItem(r.Item);
								}
							}
							else
							{
								r.Item.SetLastMoved();
								cont.DropItem(r.Item);
							}
						}
					}
					else
					{
						if (amount < r.Item.Amount)
						{
							r.Item.Amount -= amount;
						}
						else
						{
							r.Item.Delete();
						}
					}

					giveCurrency += ssi.GetSellPriceFor(r.Item) * amount;
					break;
				}
			}

			if (giveCurrency > 0)
			{
				Item currency;

				while (giveCurrency >= 60000)
				{
					currency = TypeOfCurrency.CreateInstanceSafe<Item>();

					currency.Stackable = true;
					currency.Amount = 60000;

					seller.AddToBackpack(currency);
					giveCurrency -= 60000;
				}

				if (giveCurrency > 0)
				{
					currency = TypeOfCurrency.CreateInstanceSafe<Item>();

					currency.Stackable = true;
					currency.Amount = giveCurrency;

					seller.AddToBackpack(currency);
				}

				seller.PlaySound(0x0037);

				if (SupportsBulkOrders(seller))
				{
					Item bulkOrder = CreateBulkOrder(seller, false);

					if (bulkOrder is LargeBOD)
					{
						seller.SendGump(new LargeBODAcceptGump(seller, (LargeBOD)bulkOrder));
					}
					else if (bulkOrder is SmallBOD)
					{
						seller.SendGump(new SmallBODAcceptGump(seller, (SmallBOD)bulkOrder));
					}
				}
			}

			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			List<SBInfo> sbInfos = SBInfos;

			for (int i = 0; sbInfos != null && i < sbInfos.Count; ++i)
			{
				SBInfo sbInfo = sbInfos[i];
				List<IBuyItemInfo> buyInfo = sbInfo.BuyInfo;

				for (int j = 0; buyInfo != null && j < buyInfo.Count; ++j)
				{
					var gbi = (GenericBuyInfo)buyInfo[j];

					int maxAmount = gbi.MaxAmount;
					int doubled = 0;

					switch (maxAmount)
					{
						case 40:
							doubled = 1;
							break;
						case 80:
							doubled = 2;
							break;
						case 160:
							doubled = 3;
							break;
						case 320:
							doubled = 4;
							break;
						case 640:
							doubled = 5;
							break;
						case 999:
							doubled = 6;
							break;
					}

					if (doubled > 0)
					{
						writer.WriteEncodedInt(1 + ((j * sbInfos.Count) + i));
						writer.WriteEncodedInt(doubled);
					}
				}
			}

			writer.WriteEncodedInt(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			LoadSBInfo();

			List<SBInfo> sbInfos = SBInfos;

			switch (version)
			{
				case 1:
					{
						int index;

						while ((index = reader.ReadEncodedInt()) > 0)
						{
							int doubled = reader.ReadEncodedInt();

							if (sbInfos == null)
							{
								continue;
							}

							index -= 1;
							int sbInfoIndex = index % sbInfos.Count;
							int buyInfoIndex = index / sbInfos.Count;

							if (sbInfoIndex < 0 || sbInfoIndex >= sbInfos.Count)
							{
								continue;
							}

							SBInfo sbInfo = sbInfos[sbInfoIndex];
							List<IBuyItemInfo> buyInfo = sbInfo.BuyInfo;

							if (buyInfo == null || buyInfoIndex < 0 || buyInfoIndex >= buyInfo.Count)
							{
								continue;
							}

							var gbi = (GenericBuyInfo)buyInfo[buyInfoIndex];

							int amount = 20;

							switch (doubled)
							{
								case 1:
									amount = 40;
									break;
								case 2:
									amount = 80;
									break;
								case 3:
									amount = 160;
									break;
								case 4:
									amount = 320;
									break;
								case 5:
									amount = 640;
									break;
								case 6:
									amount = 960;
									break;
							}

							gbi.Amount = gbi.MaxAmount = amount;
						}
					}
					break;
			}

			if (IsParagon)
			{
				IsParagon = false;
			}

			Timer.DelayCall(TimeSpan.Zero, CheckMorph);
		}

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (from.Alive && IsActiveVendor)
			{
				if (SupportsBulkOrders(from))
				{
					list.Add(new BulkOrderInfoEntry(from, this));
				}

				if (IsActiveSeller)
				{
					list.Add(new VendorBuyEntry(from, this));
				}

				if (IsActiveBuyer)
				{
					list.Add(new VendorSellEntry(from, this));
				}
			}

			base.AddCustomContextEntries(from, list);
		}

		public virtual IShopSellInfo[] GetSellInfo()
		{
			return m_SellInfo.ToArray();
		}

		public virtual IBuyItemInfo[] GetBuyInfo()
		{
			return m_BuyInfo.ToArray();
		}

		public override bool CanBeDamaged()
		{
			return !IsInvulnerable;
		}
	}
}

namespace Server.ContextMenus
{
	public class VendorBuyEntry : ContextMenuEntry
	{
		private readonly BaseVendor m_Vendor;

		public VendorBuyEntry(Mobile from, BaseVendor vendor)
			: base(6103, 8)
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess(from);
		}

		public override void OnClick()
		{
			m_Vendor.VendorBuy(Owner.From);
		}
	}

	public class VendorSellEntry : ContextMenuEntry
	{
		private readonly BaseVendor m_Vendor;

		public VendorSellEntry(Mobile from, BaseVendor vendor)
			: base(6104, 8)
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess(from);
		}

		public override void OnClick()
		{
			m_Vendor.VendorSell(Owner.From);
		}
	}
}

namespace Server
{
	public interface IShopSellInfo
	{
		//get display name for an item
		string GetNameFor(Item item);

		//get price for an item which the player is selling
		int GetSellPriceFor(Item item);

		//get price for an item which the player is buying
		int GetBuyPriceFor(Item item);

		//can we sell this item to this vendor?
		bool IsSellable(Item item);

		//What do we sell?
		Type[] Types { get; }

		//does the vendor resell this item?
		bool IsResellable(Item item);
	}

	public interface IBuyItemInfo
	{
		//get a new instance of an object (we just bought it)
		IEntity GetEntity();

		int ControlSlots { get; }

		int PriceScalar { get; set; }

		//display price of the item
		int Price { get; }

		//display name of the item
		string Name { get; }

		//display hue
		int Hue { get; }

		//display id
		int ItemID { get; }

		//amount in stock
		int Amount { get; set; }

		//max amount in stock
		int MaxAmount { get; }

		//Attempt to restock with item, (return true if restock sucessful)
		bool Restock(Item item, int amount);

		//called when its time for the whole shop to restock
		void OnRestock();
	}
}