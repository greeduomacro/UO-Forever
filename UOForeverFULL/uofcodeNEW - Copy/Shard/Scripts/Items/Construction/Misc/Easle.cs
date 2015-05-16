using System;

namespace Server.Items
{
	[Furniture]
	[Flipable(0xF65, 0xF67, 0xF69)]
	public class Easle : Item
	{
		[Constructable]
		public Easle() : base(0xF65)
		{
			Weight = 24.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public Easle(Serial serial) : base(serial)
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

			if ( Weight == 10.0 )
				Weight = 25.0;
		}
	}

	[Furniture]
	[Flipable( 0xF66, 0xF68, 0xF6A )]
	public class EasleWithCanvas : Item
	{
		[Constructable]
		public EasleWithCanvas() : base( 0xF66 )
		{
			Weight = 24.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public EasleWithCanvas(Serial serial) : base(serial)
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
		}
	}
}