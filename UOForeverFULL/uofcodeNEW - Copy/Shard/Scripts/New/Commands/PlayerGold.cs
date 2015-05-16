using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Multis;
using Server.Targeting;
using System.Text;

namespace Server.Misc
{
    public class PlayerResourceAmount
    {
        public static List<PlayerResourceAmount> ProcessedPlayers = new List<PlayerResourceAmount>();
        
        public PlayerMobile Player;
        public long Owned;

        public PlayerResourceAmount(PlayerMobile pm, long amount)
        {
            Player = pm;
            Owned = amount;
        }

        public static void AddToSortedList(PlayerResourceAmount playerResource)
        {
            for (int i = 0; i < PlayerResourceAmount.ProcessedPlayers.Count; i++)
            {
                if (playerResource.Owned > PlayerResourceAmount.ProcessedPlayers[i].Owned)
                {
                    PlayerResourceAmount.ProcessedPlayers.Insert(i, playerResource);
                    return;
                }
            }
            PlayerResourceAmount.ProcessedPlayers.Add(playerResource);
        }
    }
    

    public class PlayerGold
	{
        public static void Initialize()
		{
			CommandSystem.Register( "PlayerGold", AccessLevel.Developer, new CommandEventHandler( PlayerGold_OnCommand ) );
		}

        public static void PlayerGold_OnCommand(CommandEventArgs e)
        {
            long goldcount;

            if (e.Length > 0 && e.Arguments[0].ToLower() == "target")
            {
                e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(PlayerGoldTarget));
            }
            else
            {
                goldcount = GoldOnPlayers(e.Mobile);
                e.Mobile.SendMessage("There is {0} gold owned by players.", goldcount);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("======================== " + DateTime.Now + " =====================");
            foreach (PlayerResourceAmount playerGold in PlayerResourceAmount.ProcessedPlayers)
            {
                sb.Append(playerGold.Player + "\t" + playerGold.Owned + "\n");
            }
			LoggingCustom.Log("LOG_PlayerGold.txt", sb.ToString());
            PlayerResourceAmount.ProcessedPlayers = new List<PlayerResourceAmount>();
        }

        private static void PlayerGoldTarget(Mobile from, object o)
        {
            PlayerMobile pm = o as PlayerMobile;
            if (from == null || pm == null) return;

            long goldcount = GoldOnPlayer(pm);
            from.SendMessage("There is {0} gold owned by " + pm.Name + ".", goldcount);
        }

        public static long GoldOnPlayer(PlayerMobile pm)
        {
            long gold = 0;
            if (pm.BankBox != null)
            {
                gold += SearchForGold(pm.BankBox);
                gold += (int) pm.BankBox.Credit;
            }
            if (pm.Backpack != null)
                gold += SearchForGold(pm.Backpack);
            List<BaseHouse> houses = BaseHouse.GetHouses(pm);

            foreach (BaseHouse house in houses)
            {
                List<Item> houseitems = house.GetItems();
                foreach (Item item in houseitems)
                {
                    if (item is Container)
                        gold += SearchForGold((Container)item);
                    else if (item is Gold)
                        gold += ((Gold)item).Amount;
                    else if (item is BankCheck)
                        gold += ((BankCheck)item).Worth;
                }
            }
            if (gold > 0)
                PlayerResourceAmount.AddToSortedList(new PlayerResourceAmount(pm, gold));
            return gold;
        }

		public static long GoldOnPlayers( Mobile m )
		{
			long gold = 0;

			foreach( Mobile mob in World.Mobiles.Values )
			{
				if ( mob is PlayerMobile && mob.AccessLevel == AccessLevel.Player )
				{
					PlayerMobile pm = (PlayerMobile)mob;
                    gold += GoldOnPlayer(pm);
				}
			}

			return gold;
		}

		public static long SearchForGold( Container c )
		{
			long gold = 0;

			foreach( Item item in c.Items )
				if ( item is Container )
					gold += SearchForGold( (Container)item );
				else if ( item is Gold )
					gold += ((Gold)item).Amount;
				else if ( item is BankCheck )
					gold += ((BankCheck)item).Worth;

			return gold;
		}
	}

    public class PlayerIngots
	{
		public static void Initialize()
		{
			CommandSystem.Register( "PlayerIngots", AccessLevel.Developer, new CommandEventHandler( PlayerIngots_OnCommand ) );
		}

        public static void PlayerIngots_OnCommand(CommandEventArgs e)
        {
            long BaseIngotcount;

            if (e.Length > 0 && e.Arguments[0].ToLower() == "target")
            {
                e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(PlayerBaseIngotTarget));
            }
            else
            {
                BaseIngotcount = BaseIngotOnPlayers(e.Mobile);
                e.Mobile.SendMessage("There is {0} BaseIngot owned by players.", BaseIngotcount);
            }

			LoggingCustom.Log("LOG_PlayerIngots.txt", "======================== " + DateTime.Now + " =====================");
            StringBuilder sb = new StringBuilder();
            foreach (PlayerResourceAmount playerResource in PlayerResourceAmount.ProcessedPlayers)
            {
                sb.Append(playerResource.Player + "\t" + playerResource.Owned + "\n");
            }
			LoggingCustom.Log("LOG_PlayerIngots.txt", sb.ToString());
            PlayerResourceAmount.ProcessedPlayers = new List<PlayerResourceAmount>();
        }

        private static void PlayerBaseIngotTarget(Mobile from, object o)
        {
            PlayerMobile pm = o as PlayerMobile;
            if (from == null || pm == null) return;

            long BaseIngotcount = BaseIngotOnPlayer(pm);
            from.SendMessage("There is {0} BaseIngot owned by " + pm.Name + ".", BaseIngotcount);
        }

        public static long BaseIngotOnPlayer(PlayerMobile pm)
        {
            long BaseIngots = 0;
            if (pm.BankBox != null)
                BaseIngots += SearchForBaseIngot(pm.BankBox);
            if (pm.Backpack != null)
                BaseIngots += SearchForBaseIngot(pm.Backpack);
            List<BaseHouse> houses = BaseHouse.GetHouses(pm);

            foreach (BaseHouse house in houses)
            {
                List<Item> houseitems = house.GetItems();
                foreach (Item item in houseitems)
                {
                    if (item is Container)
                        BaseIngots += SearchForBaseIngot((Container)item);
                    else if (item is BaseIngot)
                        BaseIngots += ((BaseIngot)item).Amount;
                    else if (item is CommodityDeed && ((CommodityDeed)item).CommodityItem is BaseIngot)
                        BaseIngots += ((CommodityDeed)item).CommodityItem.Amount;
                }
            }
            if (BaseIngots > 0)
                PlayerResourceAmount.AddToSortedList(new PlayerResourceAmount(pm, BaseIngots));
            return BaseIngots;
        }

		public static long BaseIngotOnPlayers( Mobile m )
		{
            long BaseIngots = 0;

			foreach( Mobile mob in World.Mobiles.Values )
			{
				if ( mob is PlayerMobile && mob.AccessLevel == AccessLevel.Player )
				{
					PlayerMobile pm = (PlayerMobile)mob;

                    BaseIngots += BaseIngotOnPlayer(pm);
				}
			}

            return BaseIngots;
		}

		public static long SearchForBaseIngot( Container c )
		{
			long BaseIngots = 0;

			foreach( Item item in c.Items )
				if ( item is Container )
                    BaseIngots += SearchForBaseIngot((Container)item);
				else if ( item is BaseIngot )
                    BaseIngots += ((BaseIngot)item).Amount;
				else if ( item is CommodityDeed && ((CommodityDeed)item).CommodityItem is BaseIngot )
                    BaseIngots += ((CommodityDeed)item).CommodityItem.Amount;

            return BaseIngots;
		}
	}

    public class PlayerWood
    {
        public static void Initialize()
        {
            CommandSystem.Register("PlayerWood", AccessLevel.Developer, new CommandEventHandler(PlayerWood_OnCommand));
        }

        public static void PlayerWood_OnCommand(CommandEventArgs e)
        {
            long Woodcount;

            if (e.Length > 0 && e.Arguments[0].ToLower() == "target")
            {
                e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(PlayerWoodTarget));
            }
            else
            {
                Woodcount = WoodOnPlayers(e.Mobile);
                e.Mobile.SendMessage("There is {0} Wood owned by players.", Woodcount);
            }

			LoggingCustom.Log("LOG_PlayerWood.txt", "======================== " + DateTime.Now + " =====================");
            StringBuilder sb = new StringBuilder();
            foreach (PlayerResourceAmount playerResource in PlayerResourceAmount.ProcessedPlayers)
            {
                sb.Append(playerResource.Player + "\t" + playerResource.Owned + "\n");
            }
			LoggingCustom.Log("LOG_PlayerWood.txt", sb.ToString());
            PlayerResourceAmount.ProcessedPlayers = new List<PlayerResourceAmount>();
        }

        private static void PlayerWoodTarget(Mobile from, object o)
        {
            PlayerMobile pm = o as PlayerMobile;
            if (from == null || pm == null) return;

            long Woodcount = WoodOnPlayer(pm);
            from.SendMessage("There is {0} Wood owned by " + pm.Name + ".", Woodcount);
        }

        public static long WoodOnPlayer(PlayerMobile pm)
        {
            long Wood = 0;
            if (pm.BankBox != null)
                Wood += SearchForWood(pm.BankBox);
            if (pm.Backpack != null)
                Wood += SearchForWood(pm.Backpack);
            List<BaseHouse> houses = BaseHouse.GetHouses(pm);

            foreach (BaseHouse house in houses)
            {
                List<Item> houseitems = house.GetItems();
                foreach (Item item in houseitems)
                {
                    if (item is Container)
                        Wood += SearchForWood((Container)item);
                    else if (item is BaseLog || item is Board)
                        Wood += item.Amount;
                    else if (item is CommodityDeed && (((CommodityDeed)item).CommodityItem is BaseLog || ((CommodityDeed)item).CommodityItem is Board))
                        Wood += ((CommodityDeed)item).CommodityItem.Amount;
                }
            }
            if (Wood > 0)
                PlayerResourceAmount.AddToSortedList(new PlayerResourceAmount(pm, Wood));
            return Wood;
        }

        public static long WoodOnPlayers(Mobile m)
        {
            long Wood = 0;

            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob is PlayerMobile && mob.AccessLevel == AccessLevel.Player)
                {
                    PlayerMobile pm = (PlayerMobile)mob;

                    Wood += WoodOnPlayer(pm);
                }
            }

            return Wood;
        }

        public static long SearchForWood(Container c)
        {
            long Wood = 0;

            foreach (Item item in c.Items)
                if (item is Container)
                    Wood += SearchForWood((Container)item);
                else if (item is BaseLog || item is Board)
                    Wood += item.Amount;
                else if (item is CommodityDeed && (((CommodityDeed)item).CommodityItem is BaseLog || ((CommodityDeed)item).CommodityItem is Board))
                    Wood += ((CommodityDeed)item).CommodityItem.Amount;

            return Wood;
        }
    }

    public class PlayerPlatinum
    {
        public static void Initialize()
        {
            CommandSystem.Register("PlayerPlatinum", AccessLevel.Developer, new CommandEventHandler(PlayerPlatinum_OnCommand));
        }

        public static void PlayerPlatinum_OnCommand(CommandEventArgs e)
        {
            long Platinumcount;

            if (e.Length > 0 && e.Arguments[0].ToLower() == "target")
            {
                e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(PlayerPlatinumTarget));
            }
            else
            {
                Platinumcount = PlatinumOnPlayers(e.Mobile);
                e.Mobile.SendMessage("There is {0} Platinum owned by players.", Platinumcount);
            }

			LoggingCustom.Log("LOG_PlayerPlatinum.txt", "======================== " + DateTime.Now + " =====================");
            StringBuilder sb = new StringBuilder();
            foreach (PlayerResourceAmount playerResource in PlayerResourceAmount.ProcessedPlayers)
            {
                sb.Append(playerResource.Player + "\t" + playerResource.Owned + "\n");
            }
			LoggingCustom.Log("LOG_PlayerPlatinum.txt", sb.ToString());
            PlayerResourceAmount.ProcessedPlayers = new List<PlayerResourceAmount>();
        }

        private static void PlayerPlatinumTarget(Mobile from, object o)
        {
            PlayerMobile pm = o as PlayerMobile;
            if (from == null || pm == null) return;

            long Platinumcount = PlatinumOnPlayer(pm);
            from.SendMessage("There is {0} Platinum owned by " + pm.Name + ".", Platinumcount);
        }

        public static long PlatinumOnPlayer(PlayerMobile pm)
        {
            long Platinums = 0;
            if (pm.BankBox != null)
                Platinums += SearchForPlatinum(pm.BankBox);
            if (pm.Backpack != null)
                Platinums += SearchForPlatinum(pm.Backpack);
            List<BaseHouse> houses = BaseHouse.GetHouses(pm);

            foreach (BaseHouse house in houses)
            {
                List<Item> houseitems = house.GetItems();
                foreach (Item item in houseitems)
                {
                    if (item is Container)
                        Platinums += SearchForPlatinum((Container)item);
                    else if (item is Platinum)
                        Platinums += ((Platinum)item).Amount;
                }
            }
            if (Platinums > 0)
                PlayerResourceAmount.AddToSortedList(new PlayerResourceAmount(pm, Platinums));
            return Platinums;
        }

        public static long PlatinumOnPlayers(Mobile m)
        {
            long Platinums = 0;

            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob is PlayerMobile && mob.AccessLevel == AccessLevel.Player)
                {
                    PlayerMobile pm = (PlayerMobile)mob;

                    Platinums += PlatinumOnPlayer(pm);
                }
            }

            return Platinums;
        }

        public static long SearchForPlatinum(Container c)
        {
            long Platinums = 0;

            foreach (Item item in c.Items)
                if (item is Container)
                    Platinums += SearchForPlatinum((Container)item);
                else if (item is Platinum)
                    Platinums += ((Platinum)item).Amount;

            return Platinums;
        }
    }

    public class PlayerDonationCoin
    {
        public static void Initialize()
        {
            CommandSystem.Register("PlayerDonationCoin", AccessLevel.Developer, new CommandEventHandler(PlayerDonationCoin_OnCommand));
        }

        public static void PlayerDonationCoin_OnCommand(CommandEventArgs e)
        {
            long DonationCoincount;

            if (e.Length > 0 && e.Arguments[0].ToLower() == "target")
            {
                e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(PlayerDonationCoinTarget));
            }
            else
            {
                DonationCoincount = DonationCoinOnPlayers(e.Mobile);
                e.Mobile.SendMessage("There is {0} DonationCoin owned by players.", DonationCoincount);
            }

			LoggingCustom.Log("LOG_PlayerDonationCoin.txt", "======================== " + DateTime.Now + " =====================");
            StringBuilder sb = new StringBuilder();
            foreach (PlayerResourceAmount playerResource in PlayerResourceAmount.ProcessedPlayers)
            {
                sb.Append(playerResource.Player + "\t" + playerResource.Owned + "\n");
            }
            LoggingCustom.Log("LOG_PlayerDonationCoin.txt", sb.ToString());
            PlayerResourceAmount.ProcessedPlayers = new List<PlayerResourceAmount>();
        }

        private static void PlayerDonationCoinTarget(Mobile from, object o)
        {
            PlayerMobile pm = o as PlayerMobile;
            if (from == null || pm == null) return;

            long DonationCoincount = DonationCoinOnPlayer(pm);
            from.SendMessage("There is {0} DonationCoin owned by " + pm.Name + ".", DonationCoincount);
        }

        public static long DonationCoinOnPlayer(PlayerMobile pm)
        {
            long DonationCoins = 0;
            if (pm.BankBox != null)
                DonationCoins += SearchForDonationCoin(pm.BankBox);
            if (pm.Backpack != null)
                DonationCoins += SearchForDonationCoin(pm.Backpack);
            List<BaseHouse> houses = BaseHouse.GetHouses(pm);

            foreach (BaseHouse house in houses)
            {
                List<Item> houseitems = house.GetItems();
                foreach (Item item in houseitems)
                {
                    if (item is Container)
                        DonationCoins += SearchForDonationCoin((Container)item);
                    else if (item is DonationCoin)
                        DonationCoins += ((DonationCoin)item).Amount;
                }
            }
            if (DonationCoins > 0)
                PlayerResourceAmount.AddToSortedList(new PlayerResourceAmount(pm, DonationCoins));
            return DonationCoins;
        }

        public static long DonationCoinOnPlayers(Mobile m)
        {
            long DonationCoins = 0;

            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob is PlayerMobile && mob.AccessLevel == AccessLevel.Player)
                {
                    PlayerMobile pm = (PlayerMobile)mob;

                    DonationCoins += DonationCoinOnPlayer(pm);
                }
            }

            return DonationCoins;
        }

        public static long SearchForDonationCoin(Container c)
        {
            long DonationCoins = 0;

            foreach (Item item in c.Items)
                if (item is Container)
                    DonationCoins += SearchForDonationCoin((Container)item);
                else if (item is DonationCoin)
                    DonationCoins += ((DonationCoin)item).Amount;

            return DonationCoins;
        }
    }
}
