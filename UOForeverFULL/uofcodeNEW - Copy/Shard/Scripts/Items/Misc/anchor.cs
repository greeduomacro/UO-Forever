namespace Server.Items
{
	public class Anchor : Item
	{
		[Constructable]
		public Anchor()
			: base(Utility.RandomList(0x14F7, 0x14F9))
		{
			Movable = true;
			Stackable = false;
		}

		public Anchor(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}