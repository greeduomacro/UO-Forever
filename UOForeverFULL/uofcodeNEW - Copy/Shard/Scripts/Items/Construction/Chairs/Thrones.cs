using System;

namespace Server.Items
{
	[Furniture]
	[Flipable(0xB32, 0xB33)]
	public class Throne : Item
	{
		[Constructable]
		public Throne() : base(0xB33)
		{
			Weight = 1.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public Throne(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if ( Weight == 6.0 )
				Weight = 1.0;
		}
	}

	[Furniture]
	[Flipable( 0xB2E, 0xB2F, 0xB31, 0xB30 )]
	public class WoodenThrone : Item
	{
		[Constructable]
		public WoodenThrone() : base(0xB2F)
		{
			Weight = 15.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public WoodenThrone(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if ( Weight == 6.0 )
				Weight = 15.0;
		}
	}
}