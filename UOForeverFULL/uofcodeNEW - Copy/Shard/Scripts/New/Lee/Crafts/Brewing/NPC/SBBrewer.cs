#region References
using System.Collections.Generic;

using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class SBBrewer : SBInfo
	{
		private List<IBuyItemInfo> _BuyInfo { get; set; }
		private IShopSellInfo _SellInfo { get; set; }

		public override IShopSellInfo SellInfo { get { return _SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return _BuyInfo; } }

		public SBBrewer(bool moonglow = false)
		{
			_BuyInfo = new InternalBuyInfo(moonglow);
			_SellInfo = new InternalSellInfo();
		}

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo(bool moonglow = false)
			{
				Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Milk, 7, 20, 0x9F0, 0));
				Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Water, 11, 20, 0x1F9D, 0));
				Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Ale, 11, 20, 0x1F95, 0));
				Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Cider, 11, 20, 0x1F97, 0));
				Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Wine, 11, 20, 0x1F9B, 0));
				Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Mead, 11, 20, 0x1F95, 0));
				Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Liquor, 11, 20, 0x1F99, 0));

				if (moonglow)
				{
					Add(new GenericBuyInfo("Moonglow Mead", typeof(MoonglowMead), 20, 20, 0x1F95, 0));
				}

				//Add(new GenericBuyInfo("a Sack of Yeast", typeof(SackYeast), 4, 20, 0x1039, 0));
				//Add(new GenericBuyInfo("a Sack of Sugar", typeof(SackSugar), 4, 20, 0x1039, 0));
				Add(new GenericBuyInfo("a Jar of Honey", typeof(JarHoney), 4, 20, 0x9EC, 0));
				//Add(new GenericBuyInfo("a Jar of Syrup", typeof(JarSyrup), 8, 20, 0x9EC, 0));
				Add(new GenericBuyInfo("a Brewing Kit", typeof(BrewingKit), 50, 20, 6464, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add(typeof(Pitcher), 1);
				Add(typeof(SackYeast), 1);
				Add(typeof(SackSugar), 1);
				Add(typeof(JarHoney), 1);
				Add(typeof(JarSyrup), 1);
			}
		}
	}
}