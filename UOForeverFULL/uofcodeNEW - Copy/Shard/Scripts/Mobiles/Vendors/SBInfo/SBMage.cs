#region References
using System;
using System.Collections.Generic;

using Server.Items;
using Server.Misc;
#endregion

namespace Server.Mobiles
{
	public class SBMage : SBInfo
	{
		private readonly List<IBuyItemInfo> m_BuyInfo;
		private readonly IShopSellInfo m_SellInfo;

		public SBMage(Expansion e)
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
				Add(new GenericBuyInfo(typeof(Spellbook), 125, 10, 0xEFA, 0));

				Add(new GenericBuyInfo(typeof(ScribesPen), 8, 10, 0xFBF, 0));

				Add(new GenericBuyInfo(typeof(BlankScroll), 5, 20, 0x0E34, 0));

				Add(new GenericBuyInfo("1041072", typeof(MagicWizardsHat), 11, 10, 0x1718, Utility.RandomDyedHue()));

				Add(new GenericBuyInfo(typeof(RecallRune), 15, 10, 0x1F14, 0));

				Add(new GenericBuyInfo(typeof(RefreshPotion), 15, 10, 0xF0B, 0));
				Add(new GenericBuyInfo(typeof(AgilityPotion), 15, 10, 0xF08, 0));
				Add(new GenericBuyInfo(typeof(NightSightPotion), 15, 10, 0xF06, 0));
				Add(new GenericBuyInfo(typeof(LesserHealPotion), 15, 10, 0xF0C, 0));
				Add(new GenericBuyInfo(typeof(StrengthPotion), 15, 10, 0xF09, 0));
				Add(new GenericBuyInfo(typeof(LesserPoisonPotion), 15, 10, 0xF0A, 0));
				Add(new GenericBuyInfo(typeof(LesserCurePotion), 15, 10, 0xF07, 0));
				Add(new GenericBuyInfo(typeof(LesserExplosionPotion), 21, 10, 0xF0D, 0));

				Add(new GenericBuyInfo(typeof(BlackPearl), 5, 100, 0xF7A, 0));
				Add(new GenericBuyInfo(typeof(Bloodmoss), 5, 100, 0xF7B, 0));
				Add(new GenericBuyInfo(typeof(Garlic), 3, 100, 0xF84, 0));
				Add(new GenericBuyInfo(typeof(Ginseng), 3, 100, 0xF85, 0));
				Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, 100, 0xF86, 0));
				Add(new GenericBuyInfo(typeof(Nightshade), 3, 100, 0xF88, 0));
				Add(new GenericBuyInfo(typeof(SpidersSilk), 3, 100, 0xF8D, 0));
				Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, 100, 0xF8C, 0));

				if (e >= Expansion.AOS)
				{
					Add(new GenericBuyInfo(typeof(BatWing), 3, 20, 0xF78, 0));
					Add(new GenericBuyInfo(typeof(DaemonBlood), 6, 20, 0xF7D, 0));
					Add(new GenericBuyInfo(typeof(PigIron), 5, 20, 0xF8A, 0));
					Add(new GenericBuyInfo(typeof(NoxCrystal), 6, 20, 0xF8E, 0));
					Add(new GenericBuyInfo(typeof(GraveDust), 3, 20, 0xF8F, 0));
				}
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo(Expansion e)
			{
				Add(typeof(WizardsHat), 15);

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

					if (e >= Expansion.AOS)
					{
						Add(typeof(BatWing), 1);
						Add(typeof(DaemonBlood), 1);
						Add(typeof(PigIron), 1);
						Add(typeof(NoxCrystal), 1);
						Add(typeof(GraveDust), 1);
					}
				}

				Add(typeof(RecallRune), 8);
				Add(typeof(Spellbook), 11);

				Type[] types = Loot.RegularScrollTypes;

				for (int i = 0; i < types.Length; ++i)
				{
					Add(types[i], (i / 8) * 2);
				}
			}
		}
	}
}