using System;
using System.Collections.Generic;
using Server.Misc;
using Server.Multis.Deeds;

namespace Server.Mobiles
{
	public class SBHouseDeed: SBInfo
	{
		private List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBHouseDeed()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( "deed to a stone-and-plaster house", typeof( StonePlasterHouseDeed ), 63800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a field stone house", typeof( FieldStoneHouseDeed ), 63800, 20, 0x14F0, 0 ) );
				if ( !TestCenter.Enabled )
					Add( new GenericBuyInfo( "deed to a small brick house", typeof( SmallBrickHouseDeed ), 63800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a wooden house", typeof( WoodHouseDeed ), 63800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a wood-and-plaster house", typeof( WoodPlasterHouseDeed ), 63800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a thatched-roof cottage", typeof( ThatchedRoofCottageDeed ), 63800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a brick house", typeof( BrickHouseDeed ), 164500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a two-story wood-and-plaster house", typeof( TwoStoryWoodPlasterHouseDeed ), 192400, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a tower", typeof( TowerDeed ), 612200, 20, 0x14F0, 0 ) );
				//Add( new GenericBuyInfo( "deed to a small stone keep", typeof( KeepDeed ), 665200, 20, 0x14F0, 0 ) );
				//Add( new GenericBuyInfo( "deed to a castle", typeof( CastleDeed ), 1022800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a large house with patio", typeof( LargePatioDeed ), 172800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a marble house with patio", typeof( LargeMarbleDeed ), 212000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small stone tower", typeof( SmallTowerDeed ), 100500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a two story log cabin", typeof( LogCabinDeed ), 119800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a sandstone house with patio", typeof( SandstonePatioDeed ), 90900, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a two story villa", typeof( VillaDeed ), 156500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small stone workshop", typeof( StoneWorkshopDeed ), 80600, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small marble workshop", typeof( MarbleWorkshopDeed ), 83000, 20, 0x14F0, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				// can't sell deeds to architects
                /*
                if ( !TestCenter.Enabled )
				{
					Add( typeof( StonePlasterHouseDeed ), 21900 );
					Add( typeof( FieldStoneHouseDeed ), 21900 );
					Add( typeof( SmallBrickHouseDeed ), 21900 );
					Add( typeof( WoodHouseDeed ), 21900 );
					Add( typeof( WoodPlasterHouseDeed ), 21900 );
					Add( typeof( ThatchedRoofCottageDeed ), 21900 );
					Add( typeof( BrickHouseDeed ), 72250 );
					Add( typeof( TwoStoryWoodPlasterHouseDeed ), 96200 );
					Add( typeof( TowerDeed ), 216600 );
					//Add( typeof( KeepDeed ), 665200 );
					//Add( typeof( CastleDeed ), 1022800 );
					Add( typeof( LargePatioDeed ), 76400 );
					Add( typeof( LargeMarbleDeed ), 96400 );
					Add( typeof( SmallTowerDeed ), 44250 );
					Add( typeof( LogCabinDeed ), 48900 );
					Add( typeof( SandstonePatioDeed ), 45450 );
					Add( typeof( VillaDeed ), 68250 );
					Add( typeof( StoneWorkshopDeed ), 30300 );
					Add( typeof( MarbleWorkshopDeed ), 31500 );
					Add( typeof( SmallBrickHouseDeed ), 21900 );
				}
                 * */
			}
		}
	}
}