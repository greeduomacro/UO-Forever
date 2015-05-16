namespace Server.Items
{
	public class CurePotion : BaseCurePotion
	{
		private static readonly CureLevelInfo[] m_OldLevelInfo = new[]
		{
			new CureLevelInfo(Poison.Lesser, 1.00), // 100% chance to cure lesser poison
			new CureLevelInfo(Poison.Regular, 0.95), //  75% chance to cure regular poison
			new CureLevelInfo(Poison.Greater, 0.40), //  50% chance to cure greater poison
			new CureLevelInfo(Poison.Deadly, 0.25) //  15% chance to cure deadly poison
		};

		private static readonly CureLevelInfo[] m_AosLevelInfo = new[]
		{
			new CureLevelInfo(Poison.Lesser, 1.00), new CureLevelInfo(Poison.Regular, 0.95),
			new CureLevelInfo(Poison.Greater, 0.45), new CureLevelInfo(Poison.Deadly, 0.25),
			new CureLevelInfo(Poison.Lethal, 0.15)
		};

		public override CureLevelInfo[] LevelInfo { get { return EraAOS ? m_AosLevelInfo : m_OldLevelInfo; } }

		[Constructable]
		public CurePotion()
			: base(PotionEffect.Cure)
		{ }

		public CurePotion(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}