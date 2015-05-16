namespace Server.Items
{
	public abstract class DecoTalisman : Item
	{
		public override string DefaultName { get { return "Talisman"; } }

		public DecoTalisman(int itemID)
			: base(itemID)
		{
			Hue = 0;
			Weight = 1.0;
			Layer = Layer.Talisman;
		}

		public DecoTalisman(Serial serial)
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