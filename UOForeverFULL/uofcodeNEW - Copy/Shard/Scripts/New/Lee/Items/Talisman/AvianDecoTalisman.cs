namespace Server.Items
{
	public class AvianDecoTalisman : DecoTalisman
	{
		public override string DefaultName { get { return "Avian Talisman"; } }

		[Constructable]
		public AvianDecoTalisman()
			: base(12122)
		{ }

		public AvianDecoTalisman(Serial serial)
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