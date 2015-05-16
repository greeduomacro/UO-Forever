namespace Server.Items
{
    public class AACasinoRules : BaseBook
    {
        public static readonly BookContent Content = new BookContent
            (
            "AA Casino Rules", "AA Casino",
            new BookPageInfo
                (
                "GENERAL RULES & GAMEPLAY",
                "How to Play: To play at",
                "the Casino, please fully",
                "read and understand the",
                "gameplay & rules of the",
                "game being played. Once",
                "you’re ready, let the",
                "Dealer or Pit Boss know"
                ),
            new BookPageInfo
                (
                "you’re ready and a seat",
                "number will be given to",
                "you. Please refer to the",
                "books on the teleporters",
                "for the Seat Numbers. To",
                "place a bet: Simply",
                "place your Check on the",
                "table in front of you"
                ),
            new BookPageInfo
                (
                "and a dealer will record",
                "your bet. Anyone who is",
                "not cooperative will be",
                "warned and eventually",
                "banned. There is",
                "absolutely no PvPing or",
                "stealing allowed. Anyone",
                "who intends to do so"
                ),
            new BookPageInfo
                (
                "will be permanently",
                "banned from the Casino.",
                "Good Luck & Have Fun.",
                "BLACKJACK RULES &",
                "GAMEPLAY This game is",
                "very similar to",
                "BlackJack or 21 as some",
                "may call it, please read"
                ),
            new BookPageInfo
                (
                "this entirely as some",
                "rules may differ. How",
                "this works: Once you",
                "place your bet, you",
                "simply roll the dice cup",
                "twice when instructed by",
                "the dealer. You simply",
                "add up the outcome of"
                ),
            new BookPageInfo
                (
                "your rolls. Once you’ve",
                "rolled twice, you have",
                "the option to “Hit” or",
                "“Stand”. If you wish to",
                "“Hit”, simply roll",
                "again, if you wish to",
                "“Stand”, say “Stand”.",
                "(Much like the actual"
                ),
            new BookPageInfo
                (
                "game, if you total over",
                "21, the hand results in",
                "a bust thus losing your",
                "bet). When the player is",
                "finished with his/ her",
                "roll, it is the dealer’s",
                "turn. The dealer must",
                "“Hit” till he/ she gets"
                ),
            new BookPageInfo
                (
                "a total of 17 or higher,",
                "exceeding the total past",
                "21 results in a bust.",
                "The persons with the",
                "number closest to 21",
                "without exceeding it",
                "will win that hand.",
                "(Busted hands"
                ),
            new BookPageInfo
                (
                "automatically lose)",
                "Player BlackJacks: If a",
                "player gets a total of",
                "21 within their first",
                "two rolls, they win 2X",
                "their bet. (Dealers",
                "cannot get Black Jacks",
                "so players will be paid"
                ),
            new BookPageInfo
                (
                "as soon as they get a",
                "BlackJack). ALL ties and",
                "pushes results in a",
                "Player Lose. Minimum",
                "Wager per bet: 5K",
                "Maximum Wager per bet:",
                "100K Good Luck & Have",
                "Fun!"
                )
            );

        public override BookContent DefaultContent { get { return Content; } }

        [Constructable]
        public AACasinoRules()
            : base(0x1C11, false)
        {
            Hue = 0;
        }

        public AACasinoRules(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}