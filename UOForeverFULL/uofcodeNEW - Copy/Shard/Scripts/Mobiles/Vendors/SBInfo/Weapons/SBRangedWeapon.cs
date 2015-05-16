#region References
using System.Collections.Generic;

using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class SBRangedWeapon : SBInfo
	{
		private readonly List<IBuyItemInfo> m_BuyInfo;
		private readonly IShopSellInfo m_SellInfo;

		public SBRangedWeapon(Expansion e)
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
				Add(new GenericBuyInfo(typeof(Crossbow), 55, 20, 0xF50, 0));
				Add(new GenericBuyInfo(typeof(HeavyCrossbow), 55, 20, 0x13FD, 0));

				if (e >= Expansion.AOS)
				{
					Add(new GenericBuyInfo(typeof(RepeatingCrossbow), 46, 20, 0x26C3, 0));
					Add(new GenericBuyInfo(typeof(CompositeBow), 45, 20, 0x26C2, 0));
				}

				Add(new GenericBuyInfo(typeof(Bolt), 2, Utility.Random(30, 60), 0x1BFB, 0));
				Add(new GenericBuyInfo(typeof(Bow), 40, 20, 0x13B2, 0));
				Add(new GenericBuyInfo(typeof(Arrow), 4, Utility.Random(30, 60), 0xF3F, 0));
				Add(new GenericBuyInfo(typeof(Feather), 2, Utility.Random(30, 60), 0x1BD1, 0));
				Add(new GenericBuyInfo(typeof(Shaft), 2, Utility.Random(30, 60), 0x1BD4, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo(Expansion e)
			{
				Add(typeof(Bolt), 1);
				Add(typeof(Arrow), 1);
				Add(typeof(Shaft), 1);
				Add(typeof(Feather), 1);

				Add(typeof(HeavyCrossbow), 11);
				Add(typeof(Bow), 7);
				Add(typeof(Crossbow), 11);

				if (e < Expansion.AOS)
				{
					return;
				}

				Add(typeof(CompositeBow), 11);
				Add(typeof(RepeatingCrossbow), 11);
			}
		}
	}
}