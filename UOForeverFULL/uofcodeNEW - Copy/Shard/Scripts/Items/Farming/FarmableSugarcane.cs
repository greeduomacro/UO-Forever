namespace Server.Items
{
	public class FarmableSugarcane : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3259;
		}

		public override Item GetCropObject()
		{
			return new SugarcaneSheaf();
		}

		public override int GetPickedID()
		{
			return 3270;
		}

		[Constructable]
		public FarmableSugarcane()
			: base(GetCropID())
		{
			Hue = 768;
			Name = "sugarcane";
		}

		public FarmableSugarcane(Serial serial)
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