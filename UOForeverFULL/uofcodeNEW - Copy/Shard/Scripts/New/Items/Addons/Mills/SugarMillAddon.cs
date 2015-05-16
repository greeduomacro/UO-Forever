namespace Server.Items
{

	#region East
	public class SugarMillEastAddon : BaseMillAddon<SugarcaneSheaf, SackSugar>
	{
		private static readonly int[][] _StageTable = new[]
		{
			//
			new[] {0x1920, 0x1921, 0x1925}, //
			new[] {0x1922, 0x1923, 0x1926}, //
			new[] {0x1924, 0x1924, 0x1928} //
		};

		public override int[][] StageTable { get { return _StageTable; } }

		public override BaseAddonDeed Deed { get { return new SugarMillEastDeed(); } }

		[Constructable]
		public SugarMillEastAddon()
			: this(0)
		{ }

		[Constructable]
		public SugarMillEastAddon(int hue)
			: base(hue)
		{
			AddComponent(
				new AddonComponent(0x1920)
				{
					Name = "sugar mill"
				},
				-1,
				0,
				0);
			AddComponent(
				new AddonComponent(0x1922)
				{
					Name = "sugar mill"
				},
				0,
				0,
				0);
			AddComponent(
				new AddonComponent(0x1924)
				{
					Name = "sugar mill"
				},
				1,
				0,
				0);
		}

		public SugarMillEastAddon(Serial serial)
			: base(serial)
		{ }

		public override SackSugar TryCreateProduct(Mobile user)
		{
			SackSugar sack = base.TryCreateProduct(user);

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
				user.SendMessage("You need more sugarcane to make a sack of sugar.");
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

	public class SugarMillEastDeed : BaseAddonDeed
	{
		public override string DefaultName { get { return "sugar mill (east)"; } }

		public override BaseAddon Addon { get { return new SugarMillEastAddon(Hue); } }

		[Constructable]
		public SugarMillEastDeed()
		{ }

		public SugarMillEastDeed(Serial serial)
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
	public class SugarMillSouthAddon : BaseMillAddon<SugarcaneSheaf, SackSugar>
	{
		private static readonly int[][] _StageTable = new[]
		{
			//
			new[] {0x192C, 0x192D, 0x1931}, //
			new[] {0x192E, 0x192F, 0x1932}, //
			new[] {0x1930, 0x1930, 0x1934} //
		};

		public override int[][] StageTable { get { return _StageTable; } }

		public override BaseAddonDeed Deed { get { return new SugarMillSouthDeed(); } }

		[Constructable]
		public SugarMillSouthAddon()
			: this(0)
		{ }

		[Constructable]
		public SugarMillSouthAddon(int hue)
			: base(hue)
		{
			AddComponent(
				new AddonComponent(0x192C)
				{
					Name = "sugar mill"
				},
				0,
				-1,
				0);
			AddComponent(
				new AddonComponent(0x192E)
				{
					Name = "sugar mill"
				},
				0,
				0,
				0);
			AddComponent(
				new AddonComponent(0x1930)
				{
					Name = "sugar mill"
				},
				0,
				1,
				0);
		}

		public SugarMillSouthAddon(Serial serial)
			: base(serial)
		{ }

		public override SackSugar TryCreateProduct(Mobile user)
		{
			SackSugar sack = base.TryCreateProduct(user);

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
				user.SendMessage("You need more sugarcane to make a sack of sugar.");
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

	public class SugarMillSouthDeed : BaseAddonDeed
	{
		public override string DefaultName { get { return "sugar mill (south)"; } }

		public override BaseAddon Addon { get { return new SugarMillSouthAddon(Hue); } }

		[Constructable]
		public SugarMillSouthDeed()
		{ }

		public SugarMillSouthDeed(Serial serial)
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