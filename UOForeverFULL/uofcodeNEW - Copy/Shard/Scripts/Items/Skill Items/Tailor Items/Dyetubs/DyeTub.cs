namespace Server.Items
{
	/// <summary>
	/// Vita-Nex: Core Compatibility
	/// </summary>
	public interface IDyable
	{
		bool Dye(Mobile from, DyeTub sender);
	}

	public class DyeTub : BaseDyeTub
	{
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			if (m_Version > 2)
			{
				int version = reader.ReadEncodedInt();
			}
		}

		[Constructable]
		public DyeTub()
			: this(0)
		{ }

		[Constructable]
		public DyeTub(int hue)
			: this(hue, true)
		{ }

		[Constructable]
		public DyeTub(int hue, bool redyable)
			: this(hue, redyable, -1)
		{ }

		[Constructable]
		public DyeTub(int hue, bool redyable, int uses)
			: base(hue, redyable, uses)
		{ }

		public DyeTub(Serial serial)
			: base(serial)
		{ }

		public override bool Dye(Mobile from, Item item)
		{
			if (item is IDyable)
			{
				if (((IDyable)item).Dye(from, this))
				{
					return true;
				}
			}

			return base.Dye(from, item);
		}
	}
}