using System;

namespace Server.Items
{
	public class StatuePillarSouth : Item
	{
		[Constructable]
		public StatuePillarSouth() : base(0x12D9)
		{
			Weight = 10;
		}

		public StatuePillarSouth(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class StatuePillarEast : Item
	{
		[Constructable]
		public StatuePillarEast() : base(0x12D8)
		{
			Weight = 10;
		}

		public StatuePillarEast(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class StatueCatSouth : Item
	{
		[Constructable]
		public StatueCatSouth() : base(0x1947)
		{
			Weight = 10;
		}

		public StatueCatSouth(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class StatueCatSouth2 : Item
	{
		[Constructable]
		public StatueCatSouth2() : base(0x1949)
		{
			Weight = 10;
		}

		public StatueCatSouth2(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class StatueCatEast : Item
	{
		[Constructable]
		public StatueCatEast() : base(0x1948)
		{
			Weight = 10;
		}

		public StatueCatEast(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class StatueCatEast2 : Item
	{
		[Constructable]
		public StatueCatEast2() : base(0x194A)
		{
			Weight = 10;
		}

		public StatueCatEast2(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class ArcaneStatueEast : Item
	{
		[Constructable]
		public ArcaneStatueEast() : base(0x2D0E)
		{
			Weight = 10;
		}

		public ArcaneStatueEast(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class ArcaneStatueSouth : Item
	{
		[Constructable]
		public ArcaneStatueSouth() : base(0x2D0F)
		{
			Weight = 10;
		}

		public ArcaneStatueSouth(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class SquirrelStatueEast : Item
	{
		[Constructable]
		public SquirrelStatueEast() : base(0x2D10)
		{
			Weight = 10;
		}

		public SquirrelStatueEast(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class SquirrelStatueSouth : Item
	{
		[Constructable]
		public SquirrelStatueSouth() : base(0x2D11)
		{
			Weight = 10;
		}

		public SquirrelStatueSouth(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class WarriorStatueEast : Item
	{
		[Constructable]
		public WarriorStatueEast() : base(0x2D12)
		{
			Weight = 10;
		}

		public WarriorStatueEast(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class WarriorStatueSouth : Item
	{
		[Constructable]
		public WarriorStatueSouth() : base(0x2D13)
		{
			Weight = 10;
		}

		public WarriorStatueSouth(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class CrystalStatueSouth : Item
	{
		[Constructable]
		public CrystalStatueSouth() : base(0x35F8)
		{
			Weight = 10;
		}

		public CrystalStatueSouth(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class CrystalStatueSouth2 : Item
	{
		[Constructable]
		public CrystalStatueSouth2() : base(0x35FB)
		{
			Weight = 10;
		}

		public CrystalStatueSouth2(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class CrystalStatueSouth3 : Item
	{
		[Constructable]
		public CrystalStatueSouth3() : base(0x35FD)
		{
			Weight = 10;
		}

		public CrystalStatueSouth3(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class CrystalStatueEast : Item
	{
		[Constructable]
		public CrystalStatueEast() : base(0x35F9)
		{
			Weight = 10;
		}

		public CrystalStatueEast(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class CrystalStatueEast2 : Item
	{
		[Constructable]
		public CrystalStatueEast2() : base(0x35FA)
		{
			Weight = 10;
		}

		public CrystalStatueEast2(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class CrystalStatueEast3 : Item
	{
		[Constructable]
		public CrystalStatueEast3() : base(0x35FC)
		{
			Weight = 10;
		}

		public CrystalStatueEast3(Serial serial) : base(serial)
		{
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled && EraAOS; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}
}