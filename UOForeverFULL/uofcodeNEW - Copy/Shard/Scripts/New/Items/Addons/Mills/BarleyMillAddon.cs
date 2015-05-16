namespace Server.Items
{

	#region East
	public class BarleyMillEastAddon : BaseMillAddon<BarleySheaf, SackBarley>
	{
		private static readonly int[][] _StageTable = new[]
		{
			//
			new[] {0x1920, 0x1921, 0x1925}, //
			new[] {0x1922, 0x1923, 0x1926}, //
			new[] {0x1924, 0x1924, 0x1928} //
		};

		public override int[][] StageTable { get { return _StageTable; } }

		public override BaseAddonDeed Deed { get { return new BarleyMillEastDeed(); } }

		[Constructable]
		public BarleyMillEastAddon()
			: this(0)
		{ }

		[Constructable]
		public BarleyMillEastAddon(int hue)
			: base(hue)
		{
			AddComponent(
				new AddonComponent(0x1920)
				{
					Name = "barley mill"
				},
				-1,
				0,
				0);
			AddComponent(
				new AddonComponent(0x1922)
				{
					Name = "barley mill"
				},
				0,
				0,
				0);
			AddComponent(
				new AddonComponent(0x1924)
				{
					Name = "barley mill"
				},
				1,
				0,
				0);
		}

		public BarleyMillEastAddon(Serial serial)
			: base(serial)
		{ }

		public override SackBarley TryCreateProduct(Mobile user)
		{
			SackBarley sack = base.TryCreateProduct(user);

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
				user.SendMessage("You need more barley to make a sack of barley.");
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

	public class BarleyMillEastDeed : BaseAddonDeed
	{
		public override string DefaultName { get { return "barley mill (east)"; } }

		public override BaseAddon Addon { get { return new BarleyMillEastAddon(Hue); } }

		[Constructable]
		public BarleyMillEastDeed()
		{ }

		public BarleyMillEastDeed(Serial serial)
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
	public class BarleyMillSouthAddon : BaseMillAddon<BarleySheaf, SackBarley>
	{
		private static readonly int[][] _StageTable = new[]
		{
			//
			new[] {0x192C, 0x192D, 0x1931}, //
			new[] {0x192E, 0x192F, 0x1932}, //
			new[] {0x1930, 0x1930, 0x1934} //
		};

		public override int[][] StageTable { get { return _StageTable; } }

		public override BaseAddonDeed Deed { get { return new BarleyMillSouthDeed(); } }

		[Constructable]
		public BarleyMillSouthAddon()
			: this(0)
		{ }

		[Constructable]
		public BarleyMillSouthAddon(int hue)
			: base(hue)
		{
			AddComponent(
				new AddonComponent(0x192C)
				{
					Name = "barley mill"
				},
				0,
				-1,
				0);
			AddComponent(
				new AddonComponent(0x192E)
				{
					Name = "barley mill"
				},
				0,
				0,
				0);
			AddComponent(
				new AddonComponent(0x1930)
				{
					Name = "barley mill"
				},
				0,
				1,
				0);
		}

		public BarleyMillSouthAddon(Serial serial)
			: base(serial)
		{ }

		public override SackBarley TryCreateProduct(Mobile user)
		{
			SackBarley sack = base.TryCreateProduct(user);

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
				user.SendMessage("You need more barley to make a sack of barley.");
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

	public class BarleyMillSouthDeed : BaseAddonDeed
	{
		public override string DefaultName { get { return "barley mill (south)"; } }

		public override BaseAddon Addon { get { return new BarleyMillSouthAddon(Hue); } }

		[Constructable]
		public BarleyMillSouthDeed()
		{ }

		public BarleyMillSouthDeed(Serial serial)
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