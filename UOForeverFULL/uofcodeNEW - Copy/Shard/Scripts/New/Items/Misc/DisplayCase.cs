using System;

namespace Server.Items
{
	[Furniture]
	[Flipable(0x2FEA,0x2FEB)]
	public class DisplayCase : Item
	{
		[Constructable]
		public DisplayCase() : base(0x2FEA)
		{
			Weight = 0.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public DisplayCase(Serial serial) : base(serial)
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

			if ( Weight == 4.0 )
				Weight = 1.0;
		}
	}
}