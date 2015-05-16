

using System;

namespace Server.Items
{
	public class BoneTable : Item
	{
		[Constructable]
		public BoneTable() : base(0x2A5C)
		{
			Name = "Bone Table";
			Weight = 1.0;
		}

		public BoneTable(Serial serial) : base(serial)
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