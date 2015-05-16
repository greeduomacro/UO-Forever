using VitaNex;

namespace Server.Mobiles
{
	public sealed class ChampionGlobalsOptions : CoreServiceOptions
	{
		[CommandProperty(PlayerScores.Access)]
		public bool InformChampSpawnRegionMobs { get; set; }
		
		[CommandProperty(PlayerScores.Access)]
		public int PowerScrollsToGive { get; set; }

		[CommandProperty(PlayerScores.Access)]
		public int PowerScrollMinimumDistance { get; set; }

		[CommandProperty(PlayerScores.Access)]
		public bool PowerScrollRequireAlive { get; set; }

		public ChampionGlobalsOptions()
			: base(typeof(ChampionGlobals))
		{
			EnsureDefaults();
		}

		public ChampionGlobalsOptions(GenericReader reader)
			: base(reader)
		{ }

		public void EnsureDefaults()
		{
			InformChampSpawnRegionMobs = true;
			PowerScrollsToGive = 3;
			PowerScrollMinimumDistance = 50;
			PowerScrollRequireAlive = true;
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

			writer.SetVersion(0);

			writer.Write(InformChampSpawnRegionMobs);
			writer.Write(PowerScrollsToGive);
			writer.Write(PowerScrollMinimumDistance);
			writer.Write(PowerScrollRequireAlive);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			InformChampSpawnRegionMobs = reader.ReadBool();
			PowerScrollsToGive = reader.ReadInt();
			PowerScrollMinimumDistance = reader.ReadInt();
			PowerScrollRequireAlive = reader.ReadBool();
		}
	}
}