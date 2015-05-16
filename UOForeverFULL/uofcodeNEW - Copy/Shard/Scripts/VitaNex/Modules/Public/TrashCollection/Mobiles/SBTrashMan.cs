#region Header
//   Vorspire    _,-'/-'/  SBTrashMan.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System.Collections.Generic;

using Server;
using Server.Mobiles;

using VitaNex.Items;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public class SBTrashMan : SBInfo
	{
		// IBuyItemInfo for UOF compatibility
		private readonly List<IBuyItemInfo> _BuyInfo;
		private readonly IShopSellInfo _SellInfo;

		public SBTrashMan()
		{
			_BuyInfo = new InternalBuyInfo();
			_SellInfo = new InternalSellInfo();
		}

		public override IShopSellInfo SellInfo { get { return _SellInfo; } }

		/// <summary>
		/// IBuyItemInfo for UOF compatibility
		/// </summary>
		public override List<IBuyItemInfo> BuyInfo { get { return _BuyInfo; } }

		// IBuyItemInfo for UOF compatibility
		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add(new GenericBuyInfo("Trash Can", typeof(TrashCan), 100, 20, 3703, 0));
				Add(new GenericBuyInfo("Personal Trash Bag", typeof(PersonalTrashBag), 750, 20, 3702, 0));
				Add(new GenericBuyInfo("Septic Tank", typeof(SepticTank), 1000, 20, 3703, 1270));

				Add(new GenericBuyInfo("Stink Bomb", typeof(ThrowableStinkBomb), 20, 20, 10248, 1270));
				Add(new GenericBuyInfo("Reeking Rat", typeof(ThrowableRat), 20, 20, 8483, 0));
				Add(new GenericBuyInfo("Gigantic Boulder", typeof(ThrowableRock), 20, 20, 4534, 0));

				Add(new GenericBuyInfo("Rune Codex", typeof(RuneCodex), 25000, 20, 8901, 74));
				Add(new GenericBuyInfo("Strobe Lantern", typeof(StrobeLantern), 10000, 20, 2594, 0));

				Add(new GenericBuyInfo("Bobomb", typeof(ThrowableBomb), 200, 20, 8790, 2104));
				Add(new GenericBuyInfo("Healing Bobomb", typeof(ThrowableHealBomb), 200, 20, 8790, 55));
				Add(new GenericBuyInfo("Curing Bobomb", typeof(ThrowableCureBomb), 200, 20, 8790, 74));
				Add(new GenericBuyInfo("Mana Bobomb", typeof(ThrowableManaBomb), 200, 20, 8790, 2));

				Add(new GenericBuyInfo("B'Cast Scroll x1", typeof(BroadcastScroll), 10, 20, 3636, 0));
				Add(new GenericBuyInfo("B'Cast Scroll x3", typeof(BroadcastScroll_3Uses), 30, 20, 3636, 0));
				Add(new GenericBuyInfo("B'Cast Scroll x5", typeof(BroadcastScroll_5Uses), 50, 20, 3636, 0));
				Add(new GenericBuyInfo("B'Cast Scroll x10", typeof(BroadcastScroll_10Uses), 100, 20, 3636, 0));
				Add(new GenericBuyInfo("B'Cast Scroll x30", typeof(BroadcastScroll_30Uses), 300, 20, 3636, 0));
				Add(new GenericBuyInfo("B'Cast Scroll x50", typeof(BroadcastScroll_50Uses), 500, 20, 3636, 0));
				Add(new GenericBuyInfo("B'Cast Scroll x100", typeof(BroadcastScroll_100Uses), 1000, 20, 3636, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{ }
	}
}