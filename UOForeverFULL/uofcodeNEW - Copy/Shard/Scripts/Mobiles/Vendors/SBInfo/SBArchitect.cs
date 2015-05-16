#region References
using System.Collections.Generic;

using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class SBArchitect : SBInfo
	{
		private readonly List<IBuyItemInfo> m_BuyInfo;
		private readonly IShopSellInfo m_SellInfo;

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public SBArchitect(Expansion e)
		{
			m_BuyInfo = new InternalBuyInfo(e);
			m_SellInfo = new InternalSellInfo(e);
		}

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo(Expansion e)
			{
				Add(new GenericBuyInfo("1041280", typeof(InteriorDecorator), 10001, 20, 0xFC1, 0));

				Add(
					e >= Expansion.UOR
						? new GenericBuyInfo("1060651", typeof(HousePlacementTool), 627, 20, 0x14F6, 0)
						: new GenericBuyInfo("House Survey Tool", typeof(HouseSurveyTool), 1000, 10, 0x14F6, 1072, new object[] {10}));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo(Expansion e)
			{
				Add(typeof(InteriorDecorator), 5000);

				if (e >= Expansion.UOR)
				{
					Add(typeof(HousePlacementTool), 301);
				}
			}
		}
	}
}