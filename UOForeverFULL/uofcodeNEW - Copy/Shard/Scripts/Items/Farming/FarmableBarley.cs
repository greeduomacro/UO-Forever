namespace Server.Items
{
	public class FarmableBarley : FarmableCrop
	{
		public static int GetCropID()
		{
			return Utility.Random(3157, 4);
		}

		public override Item GetCropObject()
		{
			return new BarleySheaf();
		}

		public override int GetPickedID()
		{
			return Utility.Random(3502, 2);
		}

		[Constructable]
		public FarmableBarley()
			: base(GetCropID())
		{
			Hue = 46;
			Name = "barley";
		}

		public FarmableBarley(Serial serial)
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