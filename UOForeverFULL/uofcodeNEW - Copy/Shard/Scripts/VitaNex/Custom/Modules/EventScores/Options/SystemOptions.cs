using VitaNex;

namespace Server.Engines.EventScores
{
	public sealed class EventScoresOptions : CoreServiceOptions
	{
        [CommandProperty(PlayerScores.Access)]
        public double MeleeMod { get; set; }

        [CommandProperty(PlayerScores.Access)]
        public double ArcherMod { get; set; }

        [CommandProperty(PlayerScores.Access)]
        public double BardMod { get; set; }

        [CommandProperty(PlayerScores.Access)]
        public double TamerMod { get; set; }

        [CommandProperty(PlayerScores.Access)]
        public double SummonMod { get; set; }

		public EventScoresOptions()
			: base(typeof(EventScores))
		{
			EnsureDefaults();
		}

        public EventScoresOptions(GenericReader reader)
			: base(reader)
		{ }

		public void EnsureDefaults()
		{
            MeleeMod = 2.0;
            ArcherMod = 0.7;
            BardMod = 0.5;
            TamerMod = 0.5;
            SummonMod = 0.5;
		}

		public override void Clear()
		{
			base.Clear();

			EnsureDefaults();
		}

		public override void Reset()
		{
			base.Reset();

			EnsureDefaults();
		}
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(1);

            switch (version)
            {
                case 1:
                {
                    writer.Write(ArcherMod);
                    writer.Write(BardMod);
                    writer.Write(TamerMod);
                    writer.Write(SummonMod);
                }
                    goto case 0;
                case 0:
                    break;
            }

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        ArcherMod = reader.ReadDouble();
                        BardMod = reader.ReadDouble();
                        TamerMod = reader.ReadDouble();
                        SummonMod = reader.ReadDouble();
                    }
                    goto case 0;
                case 0:
                    break;
            }
        }
	}
}