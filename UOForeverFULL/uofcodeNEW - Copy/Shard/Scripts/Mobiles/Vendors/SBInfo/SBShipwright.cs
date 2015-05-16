using System;
using System.Collections.Generic;
using Server.Items;
using Server.Multis;

namespace Server.Mobiles
{
	public class SBShipwright : SBInfo
	{
        private List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBShipwright()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

        /*// doesn't work for some reason, just have to create new shipwrights
         * public static void UpdateSBInfos()
        {
            foreach (KeyValuePair<Serial, Mobile> pair in World.Mobiles)
            {
                if (pair.Value is Shipwright)
                {
                    Shipwright shipWright = (Shipwright)pair.Value;
                    shipWright.UpdateSBInfo();
                }
            }
        }*/

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( "1041205", typeof( SmallBoatDeed ), BoatSystemController._SmallBoatCost, 20, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041206", typeof( SmallDragonBoatDeed ), BoatSystemController._SmallBoatCost, 20, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041207", typeof( MediumBoatDeed ), BoatSystemController._MediumBoatCost, 20, 0x14F2, 0 ) );
                Add(new GenericBuyInfo("1041208", typeof(MediumDragonBoatDeed), BoatSystemController._MediumBoatCost, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041209", typeof(LargeBoatDeed), BoatSystemController._LargeBoatCost, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041210", typeof(LargeDragonBoatDeed), BoatSystemController._LargeBoatCost, 20, 0x14F2, 0));
                // === Alan Mod ===
                 // uncomment this stuff for the new expansion (might need to adjust prices)
                /*
                Add(new GenericBuyInfo("1116739", typeof(GargoyleBoatDeed), BoatSystemController._GargoyleBoatCost, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1116740", typeof(TokunoBoatDeed), BoatSystemController._TokunoBoatCost, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1116738", typeof(OrcBoatDeed), BoatSystemController._OrcBoatCost, 20, 0x14F2, 0));
                */
                Add(new GenericBuyInfo("1011071", typeof(ShipRepairTools), BoatSystemController._ShipRepairToolsCost, 20, 0x0BB6, 0));
                Add(new GenericBuyInfo("1095790", typeof(SmallShipCannonDeed), BoatSystemController._ShipCannonCost, 20, 0x4218, 0));
                
                // === end Alan Mod ===
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				//You technically CAN sell them back, *BUT* the vendors do not carry enough money to buy with
			}
		}
	}
}