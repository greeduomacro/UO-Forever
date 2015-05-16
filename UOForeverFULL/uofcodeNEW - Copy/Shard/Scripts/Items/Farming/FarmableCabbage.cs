namespace Server.Items
{
	public class FarmableCabbage : FarmableCrop
	{
		public static int GetCropID()
		{
			return 3254;
		}

		public override Item GetCropObject()
		{
			return new Cabbage {
				ItemID = Utility.Random(3195, 2)
			};
		}

		public override int GetPickedID()
		{
			return 3254;
		}

		[Constructable]
		public FarmableCabbage()
			: base(GetCropID())
		{ }

		public FarmableCabbage(Serial serial)
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