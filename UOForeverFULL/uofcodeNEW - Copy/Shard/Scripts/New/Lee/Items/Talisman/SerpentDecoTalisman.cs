namespace Server.Items
{
	public class SerpentDecoTalisman : DecoTalisman
	{
		public override string DefaultName { get { return "Serpent Talisman"; } }

		[Constructable]
		public SerpentDecoTalisman()
			: base(12121)
		{ }

		public SerpentDecoTalisman(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}