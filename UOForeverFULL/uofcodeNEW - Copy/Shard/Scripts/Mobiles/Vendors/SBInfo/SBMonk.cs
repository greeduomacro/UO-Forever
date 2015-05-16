using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBMonk : SBInfo
	{
		private List<IBuyItemInfo> m_BuyInfo = new List<IBuyItemInfo>();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMonk()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( MonkRobe ), 136, 20, 0x2687, 0x21E ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
			}
		}
	}
}