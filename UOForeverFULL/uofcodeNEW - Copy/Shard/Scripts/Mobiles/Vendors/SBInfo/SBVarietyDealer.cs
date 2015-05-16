#region References
using System;
using System.Collections.Generic;

using Server.Items;
using Server.Misc;
#endregion

namespace Server.Mobiles
{
	public class SBVarietyDealer : SBInfo
	{
		private readonly List<IBuyItemInfo> m_BuyInfo;
		private readonly IShopSellInfo m_SellInfo;

		public SBVarietyDealer(Expansion e)
		{
			m_BuyInfo = new InternalBuyInfo(e);
			m_SellInfo = new InternalSellInfo(e);
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo(Expansion e)
			{
				Add(new GenericBuyInfo(typeof(Bandage), 5, 20, 0xE21, 0));

				Add(new GenericBuyInfo(typeof(BlankScroll), 5, 80, 0x0E34, 0));

				Add(new GenericBuyInfo(typeof(NightSightPotion), 15, 10, 0xF06, 0));
				Add(new GenericBuyInfo(typeof(AgilityPotion), 15, 10, 0xF08, 0));
				Add(new GenericBuyInfo(typeof(StrengthPotion), 15, 10, 0xF09, 0));
				Add(new GenericBuyInfo(typeof(RefreshPotion), 15, 10, 0xF0B, 0));
				Add(new GenericBuyInfo(typeof(LesserCurePotion), 15, 10, 0xF07, 0));
				Add(new GenericBuyInfo(typeof(LesserHealPotion), 15, 10, 0xF0C, 0));
				Add(new GenericBuyInfo(typeof(LesserPoisonPotion), 15, 10, 0xF0A, 0));
				Add(new GenericBuyInfo(typeof(LesserExplosionPotion), 21, 10, 0xF0D, 0));

				Add(new GenericBuyInfo(typeof(Bolt), 6, Utility.Random(30, 60), 0x1BFB, 0));
				Add(new GenericBuyInfo(typeof(Arrow), 3, Utility.Random(30, 60), 0xF3F, 0));

				Add(new GenericBuyInfo(typeof(BlackPearl), 3, 30, 0xF7A, 0));
				Add(new GenericBuyInfo(typeof(Bloodmoss), 3, 30, 0xF7B, 0));
				Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, 30, 0xF86, 0));
				Add(new GenericBuyInfo(typeof(Garlic), 3, 30, 0xF84, 0));
				Add(new GenericBuyInfo(typeof(Ginseng), 3, 30, 0xF85, 0));
				Add(new GenericBuyInfo(typeof(Nightshade), 3, 30, 0xF88, 0));
				Add(new GenericBuyInfo(typeof(SpidersSilk), 3, 30, 0xF8D, 0));
				Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, 30, 0xF8C, 0));

				Add(new GenericBuyInfo(typeof(BreadLoaf), 7, 10, 0x103B, 0));
				Add(new GenericBuyInfo(typeof(Backpack), 15, 20, 0x9B2, 0));

				Type[] types = Loot.RegularScrollTypes;

				int circles = 3;

				for (int i = 0; i < circles * 8 && i < types.Length; ++i)
				{
					int itemID = 0x1F2E + i;

					if (i == 6)
					{
						itemID = 0x1F2D;
					}
					else if (i > 6)
					{
						--itemID;
					}

					Add(new GenericBuyInfo(types[i], 12 + ((i / 8) * 10), 20, itemID, 0));
				}

				if (e >= Expansion.AOS)
				{
					Add(new GenericBuyInfo(typeof(BatWing), 3, 999, 0xF78, 0));
					Add(new GenericBuyInfo(typeof(GraveDust), 3, 999, 0xF8F, 0));
					Add(new GenericBuyInfo(typeof(DaemonBlood), 6, 999, 0xF7D, 0));
					Add(new GenericBuyInfo(typeof(NoxCrystal), 6, 999, 0xF8E, 0));
					Add(new GenericBuyInfo(typeof(PigIron), 5, 999, 0xF8A, 0));

					Add(new GenericBuyInfo(typeof(NecromancerSpellbook), 115, 10, 0x2253, 0));
				}

				Add(new GenericBuyInfo(typeof(RecallRune), 15, 10, 0x1f14, 0));
				Add(new GenericBuyInfo(typeof(Spellbook), 150, 10, 0xEFA, 0));

				Add(new GenericBuyInfo("1041072", typeof(MagicWizardsHat), 11, 10, 0x1718, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo(Expansion e)
			{
				Add(typeof(Bandage), 1);

				Add(typeof(BlankScroll), 1);

				if (!TestCenter.Enabled)
				{
					Add(typeof(NightSightPotion), 3);
					Add(typeof(AgilityPotion), 3);
					Add(typeof(StrengthPotion), 3);
					Add(typeof(RefreshPotion), 3);
					Add(typeof(LesserCurePotion), 3);
					Add(typeof(LesserHealPotion), 3);
					Add(typeof(LesserPoisonPotion), 3);
					Add(typeof(LesserExplosionPotion), 5);
				}

				Add(typeof(Bolt), 3);
				Add(typeof(Arrow), 2);

				if (!TestCenter.Enabled)
				{
					Add(typeof(BlackPearl), 1);
					Add(typeof(Bloodmoss), 1);
					Add(typeof(MandrakeRoot), 1);
					Add(typeof(Garlic), 1);
					Add(typeof(Ginseng), 1);
					Add(typeof(Nightshade), 1);
					Add(typeof(SpidersSilk), 1);
					Add(typeof(SulfurousAsh), 1);
				}

				Add(typeof(BreadLoaf), 3);
				Add(typeof(Backpack), 7);
				Add(typeof(RecallRune), 8);
				Add(typeof(Spellbook), 9);
				Add(typeof(BlankScroll), 3);

				if (e >= Expansion.AOS)
				{
					Add(typeof(BatWing), 2);
					Add(typeof(GraveDust), 2);
					Add(typeof(DaemonBlood), 3);
					Add(typeof(NoxCrystal), 3);
					Add(typeof(PigIron), 3);
				}

				Type[] types = Loot.RegularScrollTypes;

				for (int i = 0; i < types.Length; ++i)
				{
					Add(types[i], ((i / 8) + 2) * 5);
				}
			}
		}
	}
}