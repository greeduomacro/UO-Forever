#region References

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Poker
{
    public class GameBackup //Provides a protection for players so that if server crashes, they will be refunded money
    {
        public static List<PokerGame> PokerGames; //List of all poker games with players
    }

    public class PokerGame
    {
        public static void Initialize()
        {
            GameBackup.PokerGames = new List<PokerGame>();
        }

        public static void EventSink_Crashed(CrashedEventArgs e)
        {
            foreach (PokerGame game in GameBackup.PokerGames)
            {
                List<PokerPlayer> toRemove = game.Players.Players.Where(player => player.Mobile != null).ToList();

                foreach (PokerPlayer player in toRemove)
                {
                    player.SendMessage(
                        0x22,
                        "The server has crashed, and you are now being removed from the poker game and being refunded the money that you currently have.");
                    if (player.RoundCurrency > 0)
                    {
                        player.Currency += player.RoundCurrency;
                        game.CommunityCurrency -= player.RoundCurrency;
                    }
                    game.RemovePlayer(player);
                }
            }
        }

        public bool NeedsGumpUpdate { get; set; }
        public List<Card> CommunityCards { get; set; }
        public int CommunityCurrency { get; set; }
        public Deck Deck { get; set; }
        public PokerGameState State { get; set; }
        public PokerDealer Dealer { get; set; }
        public PokerGameTimer Timer { get; set; }
        public PokerPlayer DealerButton { get; private set; }
        public PokerPlayer SmallBlind { get; private set; }
        public PokerPlayer BigBlind { get; private set; }
        public PlayerStructure Players { get; private set; }
        public PokerHandObject PokerHand { get; set; }
        public List<PokerActionObject> PokerActions { get; set; }
        public List<PokerPlayerObject> PokerPlayers { get; set; }
        public int Steps { get; set; }
        public List<PokerPot> PokerPots { get; set; }
        public int CurrentBet { get; set; }
        public bool IsBettingRound { get { return ((int) State % 2 == 0); } }
        public int NextRaise { get; set; }

        public Type TypeOfCurrency
        {
            get { return Dealer != null && Dealer.IsDonation ? typeof(DonationCoin) : typeof(Gold); }
        }

        public PokerGame(PokerDealer dealer)
        {
            Dealer = dealer;
            NeedsGumpUpdate = false;
            CommunityCards = new List<Card>();
            State = PokerGameState.Inactive;
            Deck = new Deck();
            Timer = new PokerGameTimer(this);
            Players = new PlayerStructure(this);
        }

        public void PokerMessage(Mobile from, string message)
        {
            from.PublicOverheadMessage(MessageType.Regular, 0x9A, true, message);

            for (int i = 0; i < Players.Count; ++i)
            {
                if (Players[i].Mobile != null)
                {
                    Players[i].Mobile.SendMessage(0x9A, "[{0}]: {1}", from.Name, message);
                }
            }
        }

        public void CheckPot(int amount, PokerPlayer player, bool allin = false)
        {
            /*if (allin)
            {
                foreach (PokerPot pokerPot in PokerPots)
                {
                    if (pokerPot.MaxContributionAmountPerPlayer == 0)
                    {
                        pokerPot.MaxContributionAmountPerPlayer = amount + player.RoundCurrency;
                        break;
                    }

                    if (amount + player.RoundCurrency < pokerPot.MaxContributionAmountPerPlayer)
                    {
                        Dictionary<PokerPlayer, int> newpot = pokerPot.CheckAllIn(amount + player.RoundCurrency);

                        foreach (PokerPlayer p in Players.Players)
                        {
                            p.Mobile.SendMessage(61, "POT DIVISION OCCURRED");
                        }

                        newpot = PokerPots.Aggregate(newpot, (current, pot) => pot.DivisionAdjustment(current));

                        PokerPots.Add(new PokerPot(newpot));
                        break;
                    }
                }
            }
            amount = PokerPots.Aggregate(amount, (current, pokerPot) => pokerPot.CheckContribution(current, player));

            if (amount > 0)
            {
                foreach (PokerPlayer p in Players.Players)
                {
                    p.Mobile.SendMessage(61, "SPLIT POT CREATED: " + amount + "GP ADDED TO IT");
                }

                PokerPots.Add(new PokerPot(amount, player));
            }*/

            var mainpot = PokerPots.FirstOrDefault();

            if (mainpot != null)
            {
                mainpot.AddtoPot(amount, player);
            }
        }

        public void SplitPots()
        {
            var firstpot = PokerPots.FirstOrDefault();
            if (firstpot != null)
            {
                var newpot = firstpot;

                while (newpot != null)
                {
                    var tempdict = newpot.SplitPot();

                    if (tempdict == null || tempdict.Count <= 0)
                        newpot = null;
                    else
                    {
                        newpot = new PokerPot(tempdict);

                        PokerPots.Add(newpot);
                    }
                }
            }
        }

        public void PokerGame_PlayerMadeDecision(PokerPlayer player)
        {
            if (Players.Peek() != player)
            {
                return;
            }

            if (player.Mobile == null)
            {
                return;
            }

            bool resetTurns = false;

            Steps++;

            var pobj = new PokerActionObject(PokerHand.PokerGameId, Steps,
                PokerPlayers.Find(x => x.Serial == player.Mobile.Serial).PokerPlayerId, (int) State / 2,
                (int) player.Action, 0);

            switch (player.Action)
            {
                case PlayerAction.None:
                    break;
                case PlayerAction.Bet:
                {
                    NextRaise = player.Bet;
                    CheckPot(player.Bet, player);
                    PokerPlayers.Find(x => x.Serial == player.Mobile.Serial).Debit += player.Bet;
                    PokerMessage(player.Mobile, String.Format("I bet {0}.", player.Bet));
                    CurrentBet = player.Bet;
                    player.RoundBet = player.Bet;
                    player.Currency -= player.Bet;
                    player.RoundCurrency += player.Bet;
                    CommunityCurrency += player.Bet;
                    resetTurns = true;
                    pobj.Amount = player.Bet;

                    break;
                }
                case PlayerAction.Raise:
                {

                    PokerMessage(player.Mobile, String.Format("I raise by {0}.", player.Bet));

                    NextRaise = player.Bet;

                    CurrentBet += player.Bet;

                    int diff = CurrentBet - player.RoundBet;

                    CheckPot(diff, player);
                    PokerPlayers.Find(x => x.Serial == player.Mobile.Serial).Debit += diff;
                    player.Currency -= diff;
                    player.RoundCurrency += diff;
                    player.RoundBet += diff;
                    CommunityCurrency += diff;
                    player.Bet = diff;
                    resetTurns = true;
                    pobj.Amount = diff;


                    break;
                }
                case PlayerAction.Call:
                {

                    PokerMessage(player.Mobile, "I call.");

                    int diff = CurrentBet - player.RoundBet; //how much they owe in the pot

                    CheckPot(diff, player);
                    player.Bet = diff;
                    PokerPlayers.Find(x => x.Serial == player.Mobile.Serial).Debit += player.Bet;
                    player.Currency -= diff;
                    player.RoundCurrency += diff;
                    player.RoundBet += diff;
                    CommunityCurrency += diff;

                    pobj.Amount = player.Bet;


                    break;
                }
                case PlayerAction.Check:
                {
                    if (!player.LonePlayer)
                    {
                        PokerMessage(player.Mobile, "Check.");
                    }
                    pobj.Amount = 0;
                    break;
                }
                case PlayerAction.Fold:
                {
                    PokerMessage(player.Mobile, "I fold.");
                    pobj.Amount = 0;

                    PokerPlayers.Find(x => x.Serial == player.Mobile.Serial).Folded = 1;

                    if (Players.Round.Contains(player))
                    {
                        Players.Round.Remove(player);
                    }

                    if (Players.Turn.Contains(player))
                    {
                        Players.Turn.Remove(player);
                    }

                    if (Players.Round.Count == 1)
                    {
                        DoShowdown(true);
                        return;
                    }

                    foreach (var pokerPot in PokerPots)
                    {
                        pokerPot.ContributionToPot.Remove(player);
                    }

                    break;
                }
                case PlayerAction.AllIn:
                {
                    if (!player.IsAllIn)
                    {
                        PokerMessage(player.Mobile, player.Forced ? "I call: all-in." : "All in.");


                        int diff = player.Currency - CurrentBet;

                        if (diff > 0)
                        {
                            CurrentBet += diff;
                            NextRaise += diff;
                        }

                        player.Bet = player.Currency;
                        CheckPot(player.Bet, player, true);
                        PokerPlayers.Find(x => x.Serial == player.Mobile.Serial).Debit += player.Currency;
                        player.RoundCurrency += player.Currency;
                        player.RoundBet += player.Currency;
                        CommunityCurrency += player.Currency;
                        pobj.Amount = player.Currency;
                        player.Currency = 0;


                        //We need to check to see if this is a follow up action, or a first call
                        //before we reset the turns
                        if (Players.Prev() != null)
                        {
                            resetTurns = (Players.Prev().Action == PlayerAction.Check);

                            PokerPlayer prev = Players.Prev();

                            if (prev.Action == PlayerAction.Check ||
                                (prev.Action == PlayerAction.Bet && prev.Bet < player.Bet) ||
                                (prev.Action == PlayerAction.AllIn && prev.Bet < player.Bet) ||
                                (prev.Action == PlayerAction.Call && prev.Bet < player.Bet) ||
                                (prev.Action == PlayerAction.Raise && prev.Bet < player.Bet))
                            {
                                resetTurns = true;
                            }
                        }
                        else
                        {
                            resetTurns = true;
                        }

                        PokerActions.Add(pobj);
                        player.IsAllIn = true;

                        player.Forced = false;
                    }

                    break;
                }
            }

            PokerLogging.PokerPlayerAction(player, player.Action, Dealer.TableName, Dealer.IsDonation);

            if (!player.IsAllIn)
            {
                PokerActions.Add(pobj);
            }

            if (resetTurns)
            {
                Players.Turn.Clear();
                Players.Push(player);
            }

            Timer.m_LastPlayer = null;
            Timer.hasWarned = false;

            if (Players.Turn.Count == Players.Round.Count)
            {
                State = (PokerGameState) ((int) State + 1);
            }
            else
            {
                AssignNextTurn();
            }

            NeedsGumpUpdate = true;
        }

        public void Begin()
        {
            Players.Clear();
            CurrentBet = 0;
            NextRaise = 0;
            Steps = 0;

            PokerLogging.createPokerLog(Dealer.TableName);
            foreach (PokerPlayer player in Players.Players)
            {
                if (player.PendingCredit > 0)
                {
                    int balance = Banker.GetBalance(player.Mobile, TypeOfCurrency);
                    if ((player.PendingCredit + player.Currency) > Dealer.MinBuyIn &&
                        (player.PendingCredit + player.Currency) < Dealer.MaxBuyIn && balance >= player.PendingCredit)
                    {
                        player.Currency += player.PendingCredit;
                        Banker.Withdraw(player.Mobile, TypeOfCurrency, player.PendingCredit);
                        player.Mobile.SendMessage(61, "You have withdrawn " + player.PendingCredit + ".");
                        player.PendingCredit = 0;
                    }
                    else if ((player.PendingCredit + player.Currency) > Dealer.MaxBuyIn &&
                             balance >= player.PendingCredit)
                    {
                        int diff = Dealer.MaxBuyIn - player.Currency;

                        if (diff > 0)
                        {
                            player.Currency += diff;
                            Banker.Withdraw(player.Mobile, TypeOfCurrency, diff);
                            player.Mobile.SendMessage(61, "You have withdrawn " + diff + "gp.");
                            player.PendingCredit = 0;
                        }
                        else
                        {
                            player.Mobile.SendMessage(61,
                                "You cannot withdraw any further currency as you are at or above the max buy-in.");
                            player.PendingCredit = 0;
                        }
                    }
                    else if ((player.PendingCredit + player.Currency) < Dealer.MinBuyIn)
                    {
                        player.Mobile.SendMessage(61,
                            "Your current rebuy-in amount does not meet the minimum buy-in for this hand.");
                        player.PendingCredit = 0;
                    }
                }
            }

            List<PokerPlayer> dispose =
                Players.Players.Where(player => player.RequestLeave || !player.IsOnline() || player.Currency <= 0)
                    .ToList();

            foreach (PokerPlayer player in dispose.Where(player => Players.Contains(player)))
            {
                RemovePlayer(player);
            }


            foreach (PokerPlayer player in Players.Players)
            {
                player.ClearGame();
                player.Game = this;

                if (player.Currency >= Dealer.BigBlind && player.IsOnline())
                {
                    Players.Round.Add(player);
                }
            }

            if (DealerButton == null) //First round / more player
            {
                if (Players.Round.Count == 2) //Only use dealer button and small blind
                {
                    DealerButton = Players.Round[0];
                    SmallBlind = Players.Round[1];
                    BigBlind = null;
                }
                else if (Players.Round.Count > 2)
                {
                    DealerButton = Players.Round[0];
                    SmallBlind = Players.Round[1];
                    BigBlind = Players.Round[2];
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (Players.Round.Count == 2) //Only use dealer button and small blind
                {
                    if (DealerButton == Players.Round[0])
                    {
                        DealerButton = Players.Round[1];
                        SmallBlind = Players.Round[0];
                    }
                    else
                    {
                        DealerButton = Players.Round[0];
                        SmallBlind = Players.Round[1];
                    }

                    BigBlind = null;
                }
                else if (Players.Round.Count > 2)
                {
                    int index = Players.Round.IndexOf(DealerButton);

                    if (index == -1) //Old dealer button was lost :(
                    {
                        DealerButton = null;
                        Begin(); //Start over
                        return;
                    }

                    if (index == Players.Round.Count - 1)
                    {
                        DealerButton = Players.Round[0];
                        SmallBlind = Players.Round[1];
                        BigBlind = Players.Round[2];
                    }
                    else if (index == Players.Round.Count - 2)
                    {
                        DealerButton = Players.Round[Players.Round.Count - 1];
                        SmallBlind = Players.Round[0];
                        BigBlind = Players.Round[1];
                    }
                    else if (index == Players.Round.Count - 3)
                    {
                        DealerButton = Players.Round[Players.Round.Count - 2];
                        SmallBlind = Players.Round[Players.Round.Count - 1];
                        BigBlind = Players.Round[0];
                    }
                    else
                    {
                        DealerButton = Players.Round[index + 1];
                        SmallBlind = Players.Round[index + 2];
                        BigBlind = Players.Round[index + 3];
                    }
                }
                else
                {
                    return;
                }
            }

            PokerHand = new PokerHandObject();
            PokerHand.Community = new List<Card>();
            PokerHand.StartTime = DateTime.Now;
            PokerHand.PokerGameId = new PokerHandSerial();

            PokerActions = new List<PokerActionObject>();

            PokerPlayers = new List<PokerPlayerObject>();

            CommunityCards.Clear();
            Deck = new Deck();

            State = PokerGameState.DealHoleCards;

            PokerLogging.StartNewHand(Dealer.TableName);

            PokerHand.InitialPlayers = Players.Players.Count;
            foreach (PokerPlayer player in Players.Players)
            {
                var playerobject = new PokerPlayerObject();

                playerobject.PokerPlayerId = new PokerPlayerSerial();
                playerobject.Serial = player.Mobile.Serial;
                playerobject.charname = player.Mobile.RawName;
                playerobject.HoleCards = new List<Card>();
                playerobject.PokerGameId = PokerHand.PokerGameId;
                playerobject.Bankroll = player.Currency;
                PokerPlayers.Add(playerobject);
            }

            if (BigBlind != null)
            {
                BigBlind.Currency -= Dealer.BigBlind;
                CommunityCurrency += Dealer.BigBlind;
                BigBlind.RoundCurrency = Dealer.BigBlind;
                BigBlind.RoundBet = Dealer.BigBlind;
                BigBlind.Bet = Dealer.BigBlind;
            }
            else
            {
                DealerButton.Currency -= Dealer.BigBlind;
                CommunityCurrency += Dealer.BigBlind;
                DealerButton.RoundCurrency = Dealer.BigBlind;
                DealerButton.RoundBet = Dealer.BigBlind;
                DealerButton.Bet = Dealer.BigBlind;                
            }

            SmallBlind.Currency -= Dealer.SmallBlind;
            CommunityCurrency += Dealer.SmallBlind;
            SmallBlind.RoundCurrency = Dealer.SmallBlind;
            SmallBlind.RoundBet = Dealer.SmallBlind;
            SmallBlind.Bet = Dealer.SmallBlind;

            NextRaise = Dealer.BigBlind;
            if (PokerPots == null)
            {
                PokerPots = new List<PokerPot>();
            }

            var pokerpotobj = new PokerPot();

            if (BigBlind != null)
            {
                pokerpotobj.AddtoPot(BigBlind.Bet, BigBlind);
                pokerpotobj.AddtoPot(SmallBlind.Bet, SmallBlind);
            }
            else
            {
                pokerpotobj.AddtoPot(SmallBlind.Bet, SmallBlind);           
            }
            PokerPots.Add(pokerpotobj);

            if (BigBlind != null)
            {
                //m_Players.Push( m_BigBlind );
                BigBlind.SetBBAction();
                CurrentBet = Dealer.BigBlind;
            }
            else
            {
                //m_Players.Push( m_SmallBlind );
                SmallBlind.SetBBAction();
                CurrentBet = Dealer.BigBlind;
            }

            if (Players.Next() == null)
            {
                return;
            }

            NeedsGumpUpdate = true;
            Timer = new PokerGameTimer(this);
            Timer.Start();
        }

        public void End()
        {

            foreach (PokerPlayer player in Players.Players.ToList())
            {
                player.CloseGump(typeof(PokerTableGump));
                player.SendGump(new PokerTableGump(this, player));
            }

            State = PokerGameState.Inactive;

            PokerHand.EndTime = DateTime.Now;

            PokerLogging.EndHand(Dealer.TableName);

            PokerExport.HandsToExport.Add(PokerHand);
            foreach (PokerActionObject pokerActionObject in PokerActions)
            {
                PokerExport.ActionsToExport.Add(pokerActionObject);
            }
            foreach (PokerPlayerObject pokerplayer in PokerPlayers)
            {
                PokerExport.PlayersToExport.Add(pokerplayer);
            }

            PokerPots.Clear();

            if (Timer.Running)
            {
                Timer.Stop();
            }
        }

        public void DealHoleCards()
        {
            for (int i = 0; i < 2; ++i)
                //Simulate passing one card out at a time, going around the circle of players 2 times
            {
                foreach (PokerPlayer player in Players.Round)
                {
                    Card card = Deck.Pop();
                    player.AddCard(card);
                    PokerPlayers.Find(x => x.Serial == player.Mobile.Serial).HoleCards.Add(card);
                }
            }
        }

        public PokerPlayer AssignNextTurn()
        {
            PokerPlayer nextTurn = Players.Next();

            if (nextTurn == null)
            {
                return null;
            }

            if (nextTurn.RequestLeave)
            {
                Players.Push(nextTurn);
                nextTurn.BetStart = DateTime.UtcNow;
                nextTurn.Action = PlayerAction.Fold;
                return nextTurn;
            }

            if (nextTurn.IsAllIn)
            {
                Players.Push(nextTurn);
                nextTurn.BetStart = DateTime.UtcNow;
                nextTurn.Action = PlayerAction.AllIn;
                return nextTurn;
            }

            if (nextTurn.LonePlayer)
            {
                Players.Push(nextTurn);
                nextTurn.BetStart = DateTime.UtcNow;
                nextTurn.Action = PlayerAction.Check;
                return nextTurn;
            }

            bool canCall = false;

            PokerPlayer currentTurn = Players.Peek();

            if (currentTurn != null && currentTurn.Action != PlayerAction.Check &&
                currentTurn.Action != PlayerAction.Fold)
            {
                canCall = true;
            }
            if (currentTurn == null && State == PokerGameState.PreFlop)
            {
                canCall = true;
            }

            Players.Push(nextTurn);
            nextTurn.BetStart = DateTime.UtcNow;

            var entry = new ResultEntry(nextTurn);
            List<Card> bestCards;

            entry.Rank = nextTurn.GetBestHand(CommunityCards, out bestCards);
            entry.BestCards = bestCards;

            nextTurn.SendMessage(0x22, String.Format("You have {0}.", HandRanker.RankString(entry)));
            nextTurn.CloseGump(typeof(PokerBetGump));
            nextTurn.SendGump(new PokerBetGump(this, nextTurn, canCall));

            NeedsGumpUpdate = true;

            return nextTurn;
        }

        public void GetWinners(bool silent)
        {
            var results = new List<ResultEntry>();

            SplitPots();
            foreach (ResultEntry entry in Players.Round.Select(p => new ResultEntry(p)))
            {
                List<Card> bestCards;

                entry.Rank = HandRanker.GetBestHand(entry.Player.GetAllCards(CommunityCards), out bestCards);
                entry.BestCards = bestCards;

                results.Add(entry);
            }

            results.Sort();

            foreach (PokerPot pokerPot in PokerPots)
            {
                List<ResultEntry> elligiblehands = results;
                elligiblehands.RemoveAll(x => !pokerPot.ContributionToPot.ContainsKey(x.Player));
                pokerPot.winners =
                    elligiblehands.Where(
                        t =>
                            (HandRanker.IsBetterThan(t, results[0]) == RankResult.Same ||
                             pokerPot.ContributionToPot.Count == 1) && pokerPot.ContributionToPot.ContainsKey(t.Player))
                        .Select(t => t.Player)
                        .ToList();
            }

            //IF NOT SILENT
            if (!silent)
            {
                /*//Only hands that have made it past the showdown may be considered for the jackpot
                foreach (ResultEntry r in results.Where(t => winners.Contains(t.Player)))
                {
                    if (PokerDealer.JackpotWinners != null)
                    {
                        if (HandRanker.IsBetterThan(r, PokerDealer.JackpotWinners.Hand) == RankResult.Better)
                        {
                            PokerDealer.JackpotWinners = null;
                            PokerDealer.JackpotWinners = new PokerDealer.JackpotInfo(winners, r, DateTime.UtcNow,
                                TypeOfCurrency);

                            break;
                        }
                    }
                    else
                    {
                        PokerDealer.JackpotWinners = new PokerDealer.JackpotInfo(winners, r, DateTime.UtcNow,
                            TypeOfCurrency);
                        break;
                    }
                }*/

                results.Reverse();

                foreach (ResultEntry entry in results)
                {
                    PokerMessage(entry.Player.Mobile, String.Format("I have {0}.", HandRanker.RankString(entry)));
                }
            }
        }

        public void AwardPotsToWinners(bool silent)
        {
            //** Casino Rake - Will take a percentage of each pot awarded and place it towards
            //**				the casino jackpot for the highest ranked hand.

            int totalrake = 0;
            if (!silent) //Only rake pots that have made it past the showdown.
            {
                foreach (PokerPot pokerPot in PokerPots)
                {
                    int rake = Math.Min((int) (pokerPot.PotCurrency * Dealer.Rake), Dealer.RakeMax);

                    if (rake > 0)
                    {
                        pokerPot.PotCurrency -= rake;
                        totalrake += rake;
                    }
                }
            }

            Dealer.TotalRake += totalrake;
            PokerLogging.TotalRaked(Dealer.TableName, totalrake, Dealer.IsDonation);
            //**

            /*int lowestBet = 0;

            foreach (PokerPlayer player in winners.Where(player => player.RoundCurrency < lowestBet || lowestBet == 0))
            {
                lowestBet = player.RoundCurrency;
            }

            foreach (PokerPlayer player in Players.Round)
            {
                int diff = player.RoundCurrency - lowestBet;

                if (diff <= 0)
                {
                    continue;
                }

                PokerPlayers.Find(x => x.Serial == player.Mobile.Serial).Credit += diff;
                player.Currency += diff;
                CommunityCurrency -= diff;
                PokerMessage(
                    Dealer,
                    String.Format("{0:#,0} {1} has been returned to {2}.", diff, TypeOfCurrency.Name, player.Mobile.Name));
            }*/

            Dictionary<PokerPlayer, int> winnerslog = new Dictionary<PokerPlayer, int>();
            foreach (PokerPot pokerpot in PokerPots)
            {
                if (pokerpot.winners.Count > 0)
                {
                    int splitPot = pokerpot.PotCurrency / pokerpot.winners.Count;
                    foreach (PokerPlayer player in pokerpot.winners)
                    {
                        PokerPlayers.Find(x => x.Serial == player.Mobile.Serial).Credit += splitPot;
                        player.Currency += splitPot;
                        if (winnerslog.ContainsKey(player))
                        {
                            winnerslog[player] += splitPot;
                        }
                        else
                        {
                            winnerslog.Add(player, splitPot);
                        }
                    }
                }
            }

            foreach (var winner in winnerslog)
            {
                PokerMessage(Dealer, String.Format("{0} has won {1:#,0} {2}.", winner.Key.Mobile.Name,
                            winner.Value,
                            (Dealer.IsDonation ? "donation coins" : "gold")));
                PokerLogging.HandWinnings(Dealer.TableName, winner.Key, winner.Value, Dealer.IsDonation);
            }

            foreach (var player in Players.Players)
            {
                PokerLogging.EndHandCurrency(Dealer.TableName, player, Dealer.IsDonation);
            }

            CommunityCurrency = 0;
        }

        public void DoShowdown(bool silent)
        {
            PokerHand.FinalPot = CommunityCurrency;
            PokerHand.FinalPlayers = Players.Round.Count;

            GetWinners(silent);

            AwardPotsToWinners(silent);


            End();

            Begin();
        }

        public void DoRoundAction() //Happens once State is changed (once per state)
        {
            if (State == PokerGameState.Showdown)
            {
                DoShowdown(false);
            }
            else if (State == PokerGameState.DealHoleCards)
            {
                DealHoleCards();
                State = PokerGameState.PreFlop;
                NeedsGumpUpdate = true;
            }
            else if (!IsBettingRound)
            {
                int numberOfCards = 0;
                string round = String.Empty;

                switch (State)
                {
                    case PokerGameState.Flop:
                    {
                        numberOfCards += 3;
                        round = "flop";
                        State = PokerGameState.PreTurn;
                    }
                        break;
                    case PokerGameState.Turn:
                    {
                        ++numberOfCards;
                        round = "turn";
                        State = PokerGameState.PreRiver;
                    }
                        break;
                    case PokerGameState.River:
                    {
                        ++numberOfCards;
                        round = "river";
                        State = PokerGameState.PreShowdown;
                    }
                        break;
                }

                if (numberOfCards != 0) //Pop the appropriate number of cards from the top of the deck
                {
                    var sb = new StringBuilder();

                    sb.Append("The " + round + " shows: ");

                    for (int i = 0; i < numberOfCards; ++i)
                    {
                        Card popped = Deck.Pop();

                        if (i == 2 || numberOfCards == 1)
                        {
                            sb.Append(popped.Name + ".");
                        }
                        else
                        {
                            sb.Append(popped.Name + ", ");
                        }

                        CommunityCards.Add(popped);
                        PokerHand.Community.Add(popped);
                    }

                    PokerMessage(Dealer, sb.ToString());
                    Players.Turn.Clear();
                    NeedsGumpUpdate = true;
                }
            }
            else
            {
                if (Players.Turn.Count == Players.Round.Count)
                {
                    switch (State)
                    {
                        case PokerGameState.PreFlop:
                            State = PokerGameState.Flop;
                            break;
                        case PokerGameState.PreTurn:
                            State = PokerGameState.Turn;
                            break;
                        case PokerGameState.PreRiver:
                            State = PokerGameState.River;
                            break;
                        case PokerGameState.PreShowdown:
                            State = PokerGameState.Showdown;
                            break;
                    }
                }
                else if (Players.Turn.Count == 0 && State != PokerGameState.PreFlop)
                    //We need to initiate betting for this round
                {
                    CurrentBet = Dealer.BigBlind;
                    NextRaise = 0;
                    ResetPlayerActions();
                    CheckLonePlayer();
                    AssignNextTurn();
                }
                else if (Players.Turn.Count == 0 && State == PokerGameState.PreFlop)
                {
                    CheckLonePlayer();
                    AssignNextTurn();
                }
            }
        }

        public void CheckLonePlayer()
        {
            int allInCount = Players.Round.Count(t => t.IsAllIn);

            PokerPlayer loner = null;

            if (allInCount == Players.Round.Count - 1)
            {
                foreach (PokerPlayer p in Players.Round.Where(t => !t.IsAllIn))
                {
                    loner = p;
                }
            }

            if (loner != null)
            {
                loner.LonePlayer = true;
            }
        }

        public void ResetPlayerActions()
        {
            for (int i = 0; i < Players.Count; ++i)
            {
                Players[i].Action = PlayerAction.None;
                Players[i].RoundBet = 0;
            }
        }

        public int GetIndexFor(Mobile from)
        {
            for (int i = 0; i < Players.Count; ++i)
            {
                if (Players[i].Mobile == null || from == null)
                {
                    continue;
                }

                if (Players[i].Mobile.Serial == from.Serial)
                {
                    return i;
                }
            }

            return -1;
        }

        public PokerPlayer GetPlayer(Mobile from)
        {
            return GetIndexFor(from) == -1 ? null : Players[GetIndexFor(from)];
        }

        public int GetIndexForPlayerInRound(Mobile from)
        {
            for (int i = 0; i < Players.Round.Count; ++i)
            {
                if (Players.Round[i].Mobile == null || from == null)
                {
                    continue;
                }

                if (Players.Round[i].Mobile.Serial == from.Serial)
                {
                    return i;
                }
            }

            return -1;
        }

        public void AddPlayer(PokerPlayer player)
        {
            Mobile from = player.Mobile;

            if (from == null)
            {
                return;
            }

            if (from is PlayerMobile)
            {
                var playermob = from as PlayerMobile;
                if (playermob.PokerJoinTimer > DateTime.UtcNow)
                {
                    TimeSpan nextuse = playermob.PokerJoinTimer - DateTime.UtcNow;
                    from.SendMessage("You cannot join another poker game for " + nextuse.Seconds + " seconds.");
                    return;
                }

                if (playermob.Aggressed.Any(info => (DateTime.UtcNow - info.LastCombatTime) < TimeSpan.FromSeconds(60)))
                {
                    playermob.SendMessage("You cannot join poker while you are in combat!");
                    return;
                }

                if (playermob.Aggressors.Any(info => (DateTime.UtcNow - info.LastCombatTime) < TimeSpan.FromSeconds(60)))
                {
                    playermob.SendMessage("You cannot join poker while you are in combat!");
                    return;
                }

                if (playermob.Party != null)
                {
                    playermob.SendMessage("You cannot join a poker game while in a party.");
                    return;
                }
            }


            if (!Dealer.InRange(from.Location, 8))
            {
                from.PrivateOverheadMessage(MessageType.Regular, 0x22, true, "I am too far away to do that",
                    from.NetState);
            }
            else if (GetIndexFor(from) != -1)
            {
                from.SendMessage(0x22, "You are already seated at this table");
            }
            else if (Players.Count >= Dealer.MaxPlayers)
            {
                from.SendMessage(0x22, "Sorry, that table is full");
            }
            else if (Banker.Withdraw(from, TypeOfCurrency, player.Currency))
            {
                Point3D seat = Point3D.Zero;

                foreach (Point3D seats in Dealer.Seats.Where(seats => !Dealer.SeatTaken(seats)))
                {
                    seat = seats;
                    break;
                }

                if (seat == Point3D.Zero)
                {
                    from.SendMessage(0x22, "Sorry, that table is full");
                    return;
                }

                player.Game = this;
                player.Seat = seat;
                player.TeleportToSeat();
                Players.Players.Add(player);

                ((PlayerMobile) from).PokerGame = this;
                from.SendMessage(0x22, "You have been seated at the table");

                if (Players.Count == 1 && !GameBackup.PokerGames.Contains(this))
                {
                    GameBackup.PokerGames.Add(this);
                }
                else if (State == PokerGameState.Inactive && Players.Count > 1 && !Dealer.TournamentMode)
                {
                    Begin();
                }
                else if (State == PokerGameState.Inactive && Players.Count >= Dealer.MaxPlayers && Dealer.TournamentMode)
                {
                    Dealer.TournamentMode = false;
                    Begin();
                }

                player.CloseGump(typeof(PokerTableGump));
                player.SendGump(new PokerTableGump(this, player));
                NeedsGumpUpdate = true;
                player.Chat = true;

                player.Mobile.Blessed = true;
            }
            else
            {
                from.SendMessage(0x22, "Your bank box lacks the funds to join this poker table");
            }
        }

        public void RemovePlayer(PokerPlayer player)
        {
            Mobile from = player.Mobile;

            if (from is PlayerMobile)
            {
                var playermob = from as PlayerMobile;
                playermob.PokerJoinTimer = DateTime.UtcNow + TimeSpan.FromMinutes(1);

                playermob.Blessed = false;
            }

            if (from == null || !Players.Contains(player))
            {
                return;
            }

            Players.Players.Remove(player);

            if (Players.Peek() == player) //It is currently their turn, fold them.
            {
                player.CloseGump(typeof(PokerBetGump));
                Timer.m_LastPlayer = null;
                player.Action = PlayerAction.Fold;
            }

            if (Players.Round.Contains(player))
            {
                Players.Round.Remove(player);
            }

            if (Players.Turn.Contains(player))
            {
                Players.Turn.Remove(player);
            }

            if (Players.Round.Count == 0)
            {
                if (PokerPlayers != null && PokerPlayers.Exists(x => x.Serial == player.Mobile.Serial))
                {
                    PokerPlayers.Find(x => x.Serial == player.Mobile.Serial).Credit += CommunityCurrency;
                }
                player.Currency += CommunityCurrency;
                CommunityCurrency = 0;

                if (GameBackup.PokerGames.Contains(this))
                {
                    GameBackup.PokerGames.Remove(this);
                }
            }

            if (player.Currency > 0)
            {
                if (from.BankBox == null) //Should NEVER happen, but JUST IN CASE!
                {
                    Utility.PushColor(ConsoleColor.Red);
                    Console.WriteLine(
                        "WARNING: Player \"{0}\" with account \"{1}\" had null bank box while trying to deposit {2:#,0} {3}. Player will NOT recieve their gold.",
                        from.Name,
                        from.Account == null ? "(-null-)" : from.Account.Username,
                        player.Currency,
                        (Dealer.IsDonation ? "donation coins" : "gold"));
                    Utility.PopColor();

                    try
                    {
                        using (var op = new StreamWriter("poker_error.log", true))
                        {
                            op.WriteLine(
                                "WARNING: Player \"{0}\" with account \"{1}\" had null bankbox while poker script was trying to deposit {2:#,0} {3}. Player will NOT recieve their gold.",
                                from.Name,
                                from.Account == null ? "(-null-)" : from.Account.Username,
                                player.Currency,
                                (Dealer.IsDonation ? "donation coins" : "gold"));
                        }
                    }
                    catch
                    {}

                    from.SendMessage(
                        0x22,
                        "WARNING: Could not find your bank box. All of your poker money has been lost in this error. Please contact a Game Master to resolve this issue.");
                }
                else
                {
                    if (Banker.Deposit(from, TypeOfCurrency, player.Currency))
                    {
                        from.SendMessage(0x22, "{0:#,0} {1} has been deposited into your bank box.", player.Currency,
                            (Dealer.IsDonation ? "donation coins" : "gold"));
                    }
                    else
                    {
                        BankCheck check;
                        if (Dealer.IsDonation)
                            check = new BankCheck(player.Currency, true);
                        else
                        {
                            check = new BankCheck(player.Currency); 
                        }
                        from.Backpack.DropItem(check);
                        from.SendMessage(0x22, "{0:#,0} {1} has been placed in your bag.", player.Currency,
                            (Dealer.IsDonation ? "donation coins" : "gold"));
                    }
                }
            }

            player.CloseAllGumps();
            ((PlayerMobile) from).PokerGame = null;
            from.Location = Dealer.ExitLocation;
            from.Map = Dealer.ExitMap;
            from.SendMessage(0x22, "You have left the table");

            NeedsGumpUpdate = true;
        }
    }

    public class ResultEntry : IComparable
    {
        public PokerPlayer Player { get; private set; }
        public List<Card> BestCards { get; set; }
        public HandRank Rank { get; set; }

        public ResultEntry(PokerPlayer player)
        {
            Player = player;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is ResultEntry)
            {
                var entry = (ResultEntry) obj;
                RankResult result = HandRanker.IsBetterThan(this, entry);

                if (result == RankResult.Better)
                {
                    return -1;
                }
                if (result == RankResult.Worse)
                {
                    return 1;
                }
            }

            return 0;
        }

        #endregion
    }

    public class PokerGameTimer : Timer
    {
        private readonly PokerGame m_Game;
        private PokerGameState m_LastState;
        public PokerPlayer m_LastPlayer;
        public bool hasWarned;

        public PokerGameTimer(PokerGame game)
            : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
        {
            m_Game = game;
            m_LastState = PokerGameState.Inactive;
            m_LastPlayer = null;
        }

        protected override void OnTick()
        {
            if (m_Game.State != PokerGameState.Inactive && m_Game.Players.Count < 2)
            {
                m_Game.End();
            }

            for (int i = 0; i < m_Game.Players.Count; ++i)
            {
                if (!m_Game.Players.Round.Contains(m_Game.Players[i]) && m_Game.Players[i].RequestLeave)
                {
                    m_Game.RemovePlayer(m_Game.Players[i]);
                }
            }

            if (m_Game.NeedsGumpUpdate)
            {
                foreach (PokerPlayer player in m_Game.Players.Players)
                {
                    player.CloseGump(typeof(PokerTableGump));
                    player.SendGump(new PokerTableGump(m_Game, player));
                }

                m_Game.NeedsGumpUpdate = false;
            }

            if (m_Game.State != m_LastState && m_Game.Players.Round.Count > 1)
            {
                m_LastState = m_Game.State;
                m_Game.DoRoundAction();
                m_LastPlayer = null;
            }

            if (m_Game.Players.Peek() == null)
            {
                return;
            }

            if (m_LastPlayer == null)
            {
                m_LastPlayer = m_Game.Players.Peek(); //Changed timer from 25.0 and 30.0 to 45.0 and 60.0
            }

            if (m_LastPlayer.BetStart.AddSeconds(15.0) <= DateTime.UtcNow && !hasWarned)
            {
                m_LastPlayer.SendMessage(
                    0x22,
                    "You have 15 seconds left to make a choice. (You will automatically fold if no choice is made)");
                hasWarned = true;
            }
            else if (m_LastPlayer.BetStart.AddSeconds(30.0) <= DateTime.UtcNow)
            {
                m_LastPlayer.KickNumber++;
                if (m_LastPlayer.KickNumber == 2)
                {
                    m_LastPlayer.RequestLeave = true;
                }
                else
                {
                    PokerPlayer temp = m_LastPlayer;
                    m_LastPlayer = null;

                    temp.CloseGump(typeof(PokerBetGump));
                    temp.Action = PlayerAction.Fold;
                    hasWarned = false;
                }
            }
        }
    }
}