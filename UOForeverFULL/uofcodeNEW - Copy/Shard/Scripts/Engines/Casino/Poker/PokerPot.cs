#region References

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Server.Poker
{
    public class PokerPot
    {
        public Dictionary<PokerPlayer, int> ContributionToPot;
        public int MaxContributionAmountPerPlayer { get; set; }
        public int PotCurrency;
        public bool IsMainPot;
        public bool AllIn;

        public List<PokerPlayer> winners;

        public PokerPot()
        {
            ContributionToPot = new Dictionary<PokerPlayer, int>();
            IsMainPot = true;
        }

        public PokerPot(Dictionary<PokerPlayer, int> createlist)
        {
            ContributionToPot = new Dictionary<PokerPlayer, int>();
            if (createlist != null)
            {
                foreach (KeyValuePair<PokerPlayer, int> entry in createlist)
                {
                    ContributionToPot.Add(entry.Key, entry.Value);
                    PotCurrency += entry.Value;
                }
            }
        }


        public Dictionary<PokerPlayer, int> SplitPot()
        {
            if (ContributionToPot.Count == 1)
                return null;
            
            Dictionary<PokerPlayer, int> toreturn = new Dictionary<PokerPlayer, int>();
            foreach (var entry in ContributionToPot)
            {
                if (MaxContributionAmountPerPlayer == 0 && entry.Key.Currency == 0)
                {
                    MaxContributionAmountPerPlayer = entry.Value;
                }
                else if (MaxContributionAmountPerPlayer > 0 && entry.Value < MaxContributionAmountPerPlayer && entry.Key.Currency == 0)
                {
                    MaxContributionAmountPerPlayer = entry.Value;
                }
            }


            if (MaxContributionAmountPerPlayer > 0)
            {
                foreach (var entry in ContributionToPot.ToArray())
                {
                    if (entry.Value > MaxContributionAmountPerPlayer && MaxContributionAmountPerPlayer > 0)
                    {
                        var diff = entry.Value - MaxContributionAmountPerPlayer;
                        toreturn.Add(entry.Key, diff);
                        ContributionToPot[entry.Key] -= diff;
                        PotCurrency -= diff;
                    }
                }
            }

            if (toreturn.Count > 0)
                return toreturn;

            return null;
        }

        /*public Dictionary<PokerPlayer, int> CheckAllIn(int maxamount)
        {
            MaxContributionAmountPerPlayer = maxamount;

            return CheckPotDivision();
        }

        public Dictionary<PokerPlayer, int> CheckPotDivision()
        {
            var toreturn = new Dictionary<PokerPlayer, int>();

            foreach (KeyValuePair<PokerPlayer, int> entry in ContributionToPot.ToArray())
            {
                if (entry.Value > MaxContributionAmountPerPlayer)
                {
                    int diff = entry.Value - MaxContributionAmountPerPlayer;
                    toreturn.Add(entry.Key, diff);
                    ContributionToPot[entry.Key] -= diff;
                    PotCurrency -= diff;
                }
            }

            return toreturn;
        }

        public Dictionary<PokerPlayer, int> DivisionAdjustment(Dictionary<PokerPlayer, int> input)
        {
            var toreturn = new Dictionary<PokerPlayer, int>();

            foreach (KeyValuePair<PokerPlayer, int> entry in input)
            {
                if (!ContributionToPot.ContainsKey(entry.Key))
                {
                    if (MaxContributionAmountPerPlayer == 0 && entry.Key.Currency == 0)
                    {
                        MaxContributionAmountPerPlayer = entry.Value;
                    }
                    else if (MaxContributionAmountPerPlayer > 0 && entry.Key.Currency == 0 &&
                             entry.Value < MaxContributionAmountPerPlayer)
                    {
                        MaxContributionAmountPerPlayer = entry.Value;
                    }
                }
                else
                {
                    if (MaxContributionAmountPerPlayer == 0 && entry.Key.Currency == 0)
                    {
                        MaxContributionAmountPerPlayer = entry.Value + ContributionToPot[entry.Key];
                    }
                    else if (MaxContributionAmountPerPlayer > 0 && entry.Key.Currency == 0 &&
                             entry.Value + ContributionToPot[entry.Key] < MaxContributionAmountPerPlayer)
                    {
                        MaxContributionAmountPerPlayer = entry.Value + ContributionToPot[entry.Key];
                    }
                }
            }

            foreach (KeyValuePair<PokerPlayer, int> entry in input)
            {
                int current = 0;
                if (ContributionToPot.ContainsKey(entry.Key))
                {
                    current = ContributionToPot[entry.Key];
                }

                int allowedcontribution = MaxContributionAmountPerPlayer - current;

                if (MaxContributionAmountPerPlayer == 0 || allowedcontribution > 0 && entry.Value < allowedcontribution)
                {
                    ContributionToPot.Add(entry.Key, entry.Value);
                    PotCurrency += entry.Value;
                }
                else if (entry.Value > allowedcontribution && allowedcontribution > 0)
                {
                    int diff = entry.Value - allowedcontribution;
                    ContributionToPot.Add(entry.Key, allowedcontribution);
                    PotCurrency += allowedcontribution;
                    toreturn.Add(entry.Key, diff);
                }
                else
                {
                    toreturn.Add(entry.Key, entry.Value);
                }
            }

            foreach (KeyValuePair<PokerPlayer, int> contributer in ContributionToPot.ToArray())
            {
                if (MaxContributionAmountPerPlayer > 0 && contributer.Value > MaxContributionAmountPerPlayer)
                {
                    int diff = contributer.Value - MaxContributionAmountPerPlayer;
                    PotCurrency -= diff;
                    ContributionToPot[contributer.Key] -= diff;
                    if (toreturn.ContainsKey(contributer.Key))
                    {
                        toreturn[contributer.Key] += diff;
                    }
                    else
                    {
                        toreturn.Add(contributer.Key, contributer.Value);
                    }
                }
            }

            return toreturn;
        }

        public int CheckContribution(int amount, PokerPlayer player)
        {
            int leftover = 0;

            if (MaxContributionAmountPerPlayer == 0)
            {
                if (ContributionToPot.ContainsKey(player))
                {
                    ContributionToPot[player] += amount;
                }
                else
                {
                    ContributionToPot.Add(player, amount);
                }
                PotCurrency += amount;
            }
            else if (MaxContributionAmountPerPlayer > 0)
            {
                if (!ContributionToPot.ContainsKey(player))
                {
                    int allowablecontribution = MaxContributionAmountPerPlayer;

                    if (amount >= allowablecontribution)
                    {
                        ContributionToPot.Add(player, MaxContributionAmountPerPlayer);
                        leftover = amount - allowablecontribution;
                        PotCurrency += MaxContributionAmountPerPlayer;
                    }
                    else
                    {
                        ContributionToPot.Add(player, amount);
                        PotCurrency += amount;
                    }
                }
                else
                {
                    int currcontribution = ContributionToPot[player];

                    if (currcontribution == MaxContributionAmountPerPlayer)
                    {
                        leftover = amount;
                    }
                    else
                    {
                        int allowablecontribution = MaxContributionAmountPerPlayer - currcontribution;

                        if (amount >= allowablecontribution)
                        {
                            ContributionToPot[player] = MaxContributionAmountPerPlayer;
                            leftover = amount - allowablecontribution;
                            PotCurrency += allowablecontribution;
                        }
                        else
                        {
                            ContributionToPot[player] += amount;
                            PotCurrency += amount;
                        }
                    }
                }
            }
            player.Mobile.SendMessage(61, "There is currently: " + PotCurrency);
            return leftover;
        }*/

        public void AddtoPot(int amount, PokerPlayer player)
        {
            if (ContributionToPot.ContainsKey(player))
            {
                ContributionToPot[player] += amount;
                PotCurrency += amount;
            }
            else
            {
                ContributionToPot.Add(player, amount);
                PotCurrency += amount;
            }
        }
    }
}