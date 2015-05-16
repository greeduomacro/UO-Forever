using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBLeatherWorker: SBInfo
	{
		private List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBLeatherWorker()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( Hides ), 6, 20, 0x1078, 0 ) );
				Add( new GenericBuyInfo( typeof( ThighBoots ), 56, 10, 0x1711, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Hides ), 2 );
				Add( typeof( ThighBoots ), 8 );
			}
		}
	}
}