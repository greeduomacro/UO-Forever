namespace Server.Items
{
	public class WarDecoTalisman : DecoTalisman
	{
		public override string DefaultName { get { return "War Talisman"; } }

		[Constructable]
		public WarDecoTalisman()
			: base(12123)
		{ }

		public WarDecoTalisman(Serial serial)
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