using VitaNex;

namespace Server
{
	public sealed class PlayerScoresOptions : CoreServiceOptions
	{
		[CommandProperty(PlayerScores.Access)]
		public double MeleeVsChampMod { get; set; }

        [CommandProperty(PlayerScores.Access)]
        public double MeleeMod { get; set; }

		[CommandProperty(PlayerScores.Access)]
		public double ArcherVsChampMod { get; set; }

		[CommandProperty(PlayerScores.Access)]
		public double ArcherMod { get; set; }

		[CommandProperty(PlayerScores.Access)]
		public double BardMod { get; set; }

		[CommandProperty(PlayerScores.Access)]
		public double TamerMod { get; set; }

		[CommandProperty(PlayerScores.Access)]
		public double SummonMod { get; set; }

		[CommandProperty(PlayerScores.Access)]
		public double ChampionMod { get; set; }

		[CommandProperty(PlayerScores.Access)]
		public double MaxPoints { get; set; }
		
		public PlayerScoresOptions()
			: base(typeof(PlayerScores))
		{
			EnsureDefaults();
		}

		public PlayerScoresOptions(GenericReader reader)
			: base(reader)
		{ }

		public void EnsureDefaults()
		{
			MeleeVsChampMod = 2.0;
            MeleeMod = 2.0;
			ArcherVsChampMod = 0.8;
			ArcherMod = 0.9;
			BardMod = 0.25;
			TamerMod = 0.25;
			SummonMod = 0.20;
			ChampionMod = 2.0;
			MaxPoints = 1000000;
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

            switch(version)
		    {
		        case 1:
                    writer.Write(MeleeMod);
		            goto case 0;
                case 0:
		        {
                    writer.Write(MeleeVsChampMod);
                    writer.Write(ArcherVsChampMod);
                    writer.Write(ArcherMod);
                    writer.Write(BardMod);
                    writer.Write(TamerMod);
                    writer.Write(SummonMod);
                    writer.Write(ChampionMod);
                    writer.Write(MaxPoints);		            
		        }
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
                    MeleeMod = reader.ReadDouble();
                    goto case 0;
                case 0:
                    {
                        MeleeVsChampMod = reader.ReadDouble();
                        ArcherVsChampMod = reader.ReadDouble();
                        ArcherMod = reader.ReadDouble();
                        BardMod = reader.ReadDouble();
                        TamerMod = reader.ReadDouble();
                        SummonMod = reader.ReadDouble();
                        ChampionMod = reader.ReadDouble();
                        MaxPoints = reader.ReadDouble();
                    }
                    break;
            }
		}
	}
}