namespace Server.Items
{
	public class FarmableHops : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3237;
		}

		public override Item GetCropObject()
		{
			return new Hops();
		}

		public override int GetPickedID()
		{
			return Utility.Random(3378, 2);
		}

		[Constructable]
		public FarmableHops()
			: base(GetCropID())
		{
			Hue = 780;
			Name = "hops";
		}

		public FarmableHops(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadEncodedInt();
		}
	}
}