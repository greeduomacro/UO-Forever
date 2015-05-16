namespace Server.Items
{

	#region East
	public class FlourMillEastAddon : BaseMillAddon<WheatSheaf, SackFlour>
	{
		private static readonly int[][] _StageTable = new[]
		{
			//
			new[] {0x1920, 0x1921, 0x1925}, //
			new[] {0x1922, 0x1923, 0x1926}, //
			new[] {0x1924, 0x1924, 0x1928} //
		};

		public override int[][] StageTable { get { return _StageTable; } }

		public override BaseAddonDeed Deed { get { return new FlourMillEastDeed(); } }

		[Constructable]
		public FlourMillEastAddon()
			: this(0)
		{ }

		[Constructable]
		public FlourMillEastAddon(int hue)
			: base(hue)
		{
			AddComponent(
				new AddonComponent(0x1920)
				{
					Name = "flour mill"
				},
				-1,
				0,
				0);
			AddComponent(
				new AddonComponent(0x1922)
				{
					Name = "flour mill"
				},
				0,
				0,
				0);
			AddComponent(
				new AddonComponent(0x1924)
				{
					Name = "flour mill"
				},
				1,
				0,
				0);
		}

		public FlourMillEastAddon(Serial serial)
			: base(serial)
		{ }

		public override SackFlour TryCreateProduct(Mobile user)
		{
			SackFlour sack = base.TryCreateProduct(user);

			if (sack != null && !sack.Deleted)
			{
				sack.ItemID = Utility.RandomBool() ? 4153 : 4165;
			}

			return sack;
		}

		protected override void OnUseNotFull(Mobile user)
		{
			if (user != null && !user.Deleted)
			{
				// You need more wheat to make a sack of flour.
				user.SendLocalizedMessage(500997);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class FlourMillEastDeed : BaseAddonDeed
	{
		public override int LabelNumber { get { return 1044347; } } // flour mill (east)

		public override BaseAddon Addon { get { return new FlourMillEastAddon(Hue); } }

		[Constructable]
		public FlourMillEastDeed()
		{ }

		public FlourMillEastDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
	#endregion

	#region South
	public class FlourMillSouthAddon : BaseMillAddon<WheatSheaf, SackFlour>
	{
		private static readonly int[][] _StageTable = new[]
		{
			//
			new[] {0x192C, 0x192D, 0x1931}, //
			new[] {0x192E, 0x192F, 0x1932}, //
			new[] {0x1930, 0x1930, 0x1934} //
		};

		public override int[][] StageTable { get { return _StageTable; } }

		public override BaseAddonDeed Deed { get { return new FlourMillSouthDeed(); } }

		[Constructable]
		public FlourMillSouthAddon()
			: this(0)
		{ }

		[Constructable]
		public FlourMillSouthAddon(int hue)
			: base(hue)
		{
			AddComponent(
				new AddonComponent(0x192C)
				{
					Name = "flour mill"
				},
				0,
				-1,
				0);
			AddComponent(
				new AddonComponent(0x192E)
				{
					Name = "flour mill"
				},
				0,
				0,
				0);
			AddComponent(
				new AddonComponent(0x1930)
				{
					Name = "flour mill"
				},
				0,
				1,
				0);
		}

		public FlourMillSouthAddon(Serial serial)
			: base(serial)
		{ }

		public override SackFlour TryCreateProduct(Mobile user)
		{
			SackFlour sack = base.TryCreateProduct(user);

			if (sack != null && !sack.Deleted)
			{
				sack.ItemID = Utility.RandomBool() ? 4153 : 4165;
			}

			return sack;
		}

		protected override void OnUseNotFull(Mobile user)
		{
			if (user != null && !user.Deleted)
			{
				// You need more wheat to make a sack of flour.
				user.SendLocalizedMessage(500997);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class FlourMillSouthDeed : BaseAddonDeed
	{
		public override int LabelNumber { get { return 1044348; } } // flour mill (south)

		public override BaseAddon Addon { get { return new FlourMillSouthAddon(Hue); } }

		[Constructable]
		public FlourMillSouthDeed()
		{ }

		public FlourMillSouthDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
	#endregion
}