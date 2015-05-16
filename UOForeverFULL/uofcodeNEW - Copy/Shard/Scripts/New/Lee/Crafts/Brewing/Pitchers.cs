
namespace Server.Items
{
	public class PitcherOfAle : Pitcher
	{
		public override bool CanChangeContent { get { return false; } }

		[Constructable]
		public PitcherOfAle()
			: base(BeverageType.Ale)
		{ }

		public PitcherOfAle(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					break;
			}
		}
	}

	public class PitcherOfCider : Pitcher
	{
		public override bool CanChangeContent { get { return false; } }

		[Constructable]
		public PitcherOfCider()
			: base(BeverageType.Cider)
		{ }

		public PitcherOfCider(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					break;
			}
		}
	}

	public class PitcherOfWine : Pitcher
	{
		public override bool CanChangeContent { get { return false; } }

		[Constructable]
		public PitcherOfWine()
			: base(BeverageType.Wine)
		{ }

		public PitcherOfWine(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					break;
			}
		}
	}

	public class PitcherOfLiquor : Pitcher
	{
		public override bool CanChangeContent { get { return false; } }

		[Constructable]
		public PitcherOfLiquor()
			: base(BeverageType.Liquor)
		{ }

		public PitcherOfLiquor(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					break;
			}
		}
	}

	public class PitcherOfMead : Pitcher
	{
		public override bool CanChangeContent { get { return false; } }

		[Constructable]
		public PitcherOfMead()
			: base(BeverageType.Mead)
		{ }

		public PitcherOfMead(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					break;
			}
		}
	}
}