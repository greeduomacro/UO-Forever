using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBFisherman : SBInfo
	{
		private List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBFisherman()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( RawFishSteak ), 3, 20, 0x97A, 0 ) );
				//TODO: Add( new GenericBuyInfo( typeof( SmallFish ), 3, 20, 0xDD6, 0 ) );
				//TODO: Add( new GenericBuyInfo( typeof( SmallFish ), 3, 20, 0xDD7, 0 ) );
				Add( new GenericBuyInfo( typeof( Fish ), 6, 80, 0x9CC, 0 ) );
				Add( new GenericBuyInfo( typeof( Fish ), 6, 80, 0x9CD, 0 ) );
				Add( new GenericBuyInfo( typeof( Fish ), 6, 80, 0x9CE, 0 ) );
				Add( new GenericBuyInfo( typeof( Fish ), 6, 80, 0x9CF, 0 ) );
				Add( new GenericBuyInfo( typeof( FishingPole ), 15, 20, 0xDC0, 0 ) );

				#region Mondain's Legacy
				Add( new GenericBuyInfo( typeof( AquariumFishNet ), 250, 5, 0xDC8, 0x240 ) );
				Add( new GenericBuyInfo( typeof( AquariumFood ), 62, 5, 0xEFC, 0 ) );
				Add( new GenericBuyInfo( typeof( FishBowl ), 6312, 5, 0x241C, 0x482 ) );
				Add( new GenericBuyInfo( typeof( VacationWafer ), 67, 5, 0x971, 0 ) );
				Add( new GenericBuyInfo( typeof( AquariumNorthDeed ), 250000, 1, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( typeof( AquariumEastDeed ), 250000, 1, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( typeof( NewAquariumBook ), 15, 5, 0xFF2, 0 ) );
				#endregion
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				//Add( typeof( RawFishSteak ), 1 );
				Add( typeof( Fish ), 1 );
				//TODO: Add( typeof( SmallFish ), 1 );
				Add( typeof( FishingPole ), 3 );
			}
		}
	}
}