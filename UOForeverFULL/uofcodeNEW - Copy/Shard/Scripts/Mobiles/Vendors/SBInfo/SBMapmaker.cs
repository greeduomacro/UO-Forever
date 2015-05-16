using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBMapmaker : SBInfo
	{
		private List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMapmaker()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( BlankMap ), 5, 40, 0x14EC, 0 ) );
				Add( new GenericBuyInfo( typeof( MapmakersPen ), 8, 20, 0x0FBF, 0 ) );
				Add( new GenericBuyInfo( typeof( BlankScroll ), 12, 40, 0xEF3, 0 ) );

				for ( int i = 0; i < PresetMapEntry.Table.Length; ++i )
					Add( new PresetMapBuyInfo( PresetMapEntry.Table[i], Utility.RandomMinMax( 7, 10 ), 20 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( BlankScroll ), 1 );
				Add( typeof( MapmakersPen ), 1 );
				Add( typeof( BlankMap ), 1 );
				Add( typeof( CityMap ), 1 );
				Add( typeof( LocalMap ), 1 );
				Add( typeof( WorldMap ), 1 );
				Add( typeof( PresetMapEntry ), 3 );
				//TODO: Buy back maps that the mapmaker sells!!!
			}
		}
	}
}