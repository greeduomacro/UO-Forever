using System;
using System.Collections.Generic;
using Server.Items;
using Server.Engines.Apiculture;

namespace Server.Mobiles
{
	public class SBBeekeeper : SBInfo
	{
		private List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBBeekeeper()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( JarHoney ), 3, 20, 0x9EC, 0 ) );
				Add( new GenericBuyInfo( typeof( Beeswax ), 2, 20, 0x1422, 0 ) );
				Add( new GenericBuyInfo( typeof( BeehiveDeed ), 2000, 10, 2330, 0 ) );
				Add( new GenericBuyInfo( typeof( HiveTool ), 100, 20, 2549, 0 ) );
				Add( new GenericBuyInfo( typeof( SmallWaxPot ), 250, 20, 2532, 0 ) );
				Add( new GenericBuyInfo( typeof( LargeWaxPot ), 400, 20, 2541, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( JarHoney ), 1 );
				Add( typeof( Beeswax ), 1 );
				Add( typeof( BeehiveDeed ), 100 );
				Add( typeof( HiveTool ), 20 );
				Add( typeof( SmallWaxPot ), 25 );
				Add( typeof( LargeWaxPot ), 50 );
			}
		}
	}
}