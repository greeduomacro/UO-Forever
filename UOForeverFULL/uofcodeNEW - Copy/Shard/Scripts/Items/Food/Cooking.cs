#region References
using System;
#endregion

namespace Server.Items
{
	#region Jars
	// ********** JarHoney **********
	public class JarHoney : Item
	{
		public override string DefaultName { get { return "jar of honey"; } }

		[Constructable]
		public JarHoney()
			: base(0x9ec)
		{
			Weight = 1.0;
			Stackable = true;
		}

		public JarHoney(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	// ********** JarSyrup **********
	public class JarSyrup : Item
	{
		public override string DefaultName { get { return "jar of syrup"; } }

		[Constructable]
		public JarSyrup()
			: base(0x9ec)
		{
			Weight = 1.0;
			Stackable = true;
		}

		public JarSyrup(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
	#endregion

	#region Bowls
	// ********** BowlFlour **********
	public class BowlFlour : Item
	{
		[Constructable]
		public BowlFlour()
			: base(2590)
		{
			Weight = 1.0;
		}

		public BowlFlour(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	// ********** BowlSugar **********
	public class BowlSugar : Item
	{
		public override string DefaultName { get { return "Bowl of Sugar"; } }

		[Constructable]
		public BowlSugar()
			: base(2590)
		{
			Hue = 996;
			Weight = 1.0;
		}

		public BowlSugar(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	// ********** BowlYeast **********
	public class BowlYeast : Item
	{
		public override string DefaultName { get { return "bowl of yeast"; } }

		[Constructable]
		public BowlYeast()
			: base(2590)
		{
			Hue = 650;
			Weight = 1.0;
		}

		public BowlYeast(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	// ********** BowlBarley **********
	public class BowlBarley : Item
	{
		public override string DefaultName { get { return "bowl of barley"; } }

		[Constructable]
		public BowlBarley()
			: base(2590)
		{
			Hue = 46;
			Weight = 1.0;
		}

		public BowlBarley(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	// ********** WoodenBowl **********
	public class WoodenBowl : Item
	{
		[Constructable]
		public WoodenBowl()
			: base(5624)
		{
			Weight = 1.0;
		}

		public WoodenBowl(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
	#endregion

	#region Sacks
	// ********** SackSugar **********
	public class SackSugar : Item, IHasQuantity
	{
		public override string DefaultName { get { return "sack of sugar"; } }

		private int _Quantity;

		[CommandProperty(AccessLevel.GameMaster)]
		public int Quantity
		{
			get { return _Quantity; }
			set
			{
				_Quantity = Math.Max(0, Math.Min(20, value));

				if (_Quantity <= 0)
				{
					Delete();
				}
				else if (_Quantity < 20 && (ItemID == 4153 || ItemID == 4165))
				{
					ItemID++;
					Name = "open sack of sugar";
				}

				InvalidateProperties();
			}
		}

		[Constructable]
		public SackSugar()
			: base(4153)
		{
			_Quantity = 20;

			Hue = 996;
			Weight = 1.0;
		}

		public SackSugar(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile from)
		{
			if (!Movable || (ItemID != 4153 && ItemID != 4165))
			{
				return;
			}

			ItemID++;
			Name = "open sack of sugar";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(_Quantity);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					_Quantity = reader.ReadInt();
					break;
			}
		}
	}

	// ********** SackYeast **********
	public class SackYeast : Item, IHasQuantity
	{
		public override string DefaultName { get { return "sack of yeast"; } }

		private int _Quantity;

		[CommandProperty(AccessLevel.GameMaster)]
		public int Quantity
		{
			get { return _Quantity; }
			set
			{
				_Quantity = Math.Max(0, Math.Min(20, value));

				if (_Quantity <= 0)
				{
					Delete();
				}
				else if (_Quantity < 20 && (ItemID == 4153 || ItemID == 4165))
				{
					ItemID++;
					Name = "open sack of yeast";
				}

				InvalidateProperties();
			}
		}

		[Constructable]
		public SackYeast()
			: base(4153)
		{
			_Quantity = 20;

			Hue = 650;
			Weight = 1.0;
		}

		public SackYeast(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile from)
		{
			if (!Movable || (ItemID != 4153 && ItemID != 4165))
			{
				return;
			}

			ItemID++;
			Name = "open sack of yeast";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(_Quantity);
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
					_Quantity = reader.ReadInt();
					break;
			}
		}
	}

	// ********** SackBarley **********
	public class SackBarley : Item, IHasQuantity
	{
		public override string DefaultName { get { return "sack of barley"; } }

		private int _Quantity;

		[CommandProperty(AccessLevel.GameMaster)]
		public int Quantity
		{
			get { return _Quantity; }
			set
			{
				_Quantity = Math.Max(0, Math.Min(20, value));

				if (_Quantity <= 0)
				{
					Delete();
				}
				else if (_Quantity < 20 && (ItemID == 4153 || ItemID == 4165))
				{
					ItemID++;
					Name = "open sack of barley";
				}

				InvalidateProperties();
			}
		}

		[Constructable]
		public SackBarley()
			: base(4153)
		{
			_Quantity = 20;

			Hue = 46;
			Weight = 1.0;
		}

		public SackBarley(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile from)
		{
			if (!Movable || (ItemID != 4153 && ItemID != 4165))
			{
				return;
			}

			ItemID++;
			Name = "open sack of barley";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(_Quantity);
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
					_Quantity = reader.ReadInt();
					break;
			}
		}
	}

	// ********** SackFlour **********
	public class SackFlour : Item, IHasQuantity
	{
		private int _Quantity;

		[CommandProperty(AccessLevel.GameMaster)]
		public int Quantity
		{
			get { return _Quantity; }
			set
			{
				_Quantity = Math.Max(0, Math.Min(20, value));

				if (_Quantity == 0)
				{
					Delete();
				}
				else if (_Quantity < 20 && (ItemID == 4153 || ItemID == 4165))
				{
					++ItemID;
				}
			}
		}

		[Constructable]
		public SackFlour()
			: base(4153)
		{
			_Quantity = 20;

			Weight = 5.0;
		}

		public SackFlour(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile m)
		{
			if (Movable && (ItemID == 4153 || ItemID == 4165))
			{
				++ItemID;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(2); // version

			writer.Write(_Quantity);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
				case 1:
					_Quantity = reader.ReadInt();
					break;
				case 0:
					_Quantity = 20;
					break;
			}

			if (version < 2 && Weight == 1.0)
			{
				Weight = 5.0;
			}
		}
	}
	#endregion

	#region Sheaves
	// ********** WheatSheaf **********
	public class WheatSheaf : Item
	{
		[Constructable]
		public WheatSheaf()
			: this(1)
		{ }

		[Constructable]
		public WheatSheaf(int amount)
			: base(7869)
		{
			Weight = 1.0;
			Stackable = true;
			Amount = amount;
		}

		public WheatSheaf(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile m)
		{
			if (Movable && this.CheckDoubleClick(m, true, false, 2))
			{
				m.Target = new MillTarget<WheatSheaf, SackFlour>(this);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	// ********** BarleySheaf **********
	public class BarleySheaf : Item
	{
		public override string DefaultName { get { return "barley sheaf"; } }

		[Constructable]
		public BarleySheaf()
			: this(1)
		{ }

		[Constructable]
		public BarleySheaf(int amount)
			: base(7869)
		{
			Hue = 46;
			Weight = 1.0;
			Stackable = true;
			Amount = amount;
		}

		public BarleySheaf(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile m)
		{
			if (Movable && this.CheckDoubleClick(m, true, false, 2))
			{
				m.Target = new MillTarget<BarleySheaf, SackBarley>(this);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	// ********** SugarcaneSheaf **********
	public class SugarcaneSheaf : Item
	{
		public override string DefaultName { get { return "sugarcane sheaf"; } }

		[Constructable]
		public SugarcaneSheaf()
			: this(1)
		{ }

		[Constructable]
		public SugarcaneSheaf(int amount)
			: base(7869)
		{
			Hue = 768;
			Weight = 1.0;
			Stackable = true;
			Amount = amount;
		}

		public SugarcaneSheaf(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile m)
		{
			if (Movable && this.CheckDoubleClick(m, true, false, 2))
			{
				m.Target = new MillTarget<SugarcaneSheaf, SackSugar>(this);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
	#endregion

	// ********** Hops **********
	public class Hops : Item
	{
		public override string DefaultName { get { return "hops"; } }

		[Constructable]
		public Hops()
			: this(1)
		{ }

		[Constructable]
		public Hops(int amount)
			: base(12676)
		{
			Hue = 780;
			Weight = 1.0;
			Stackable = true;
			Amount = amount;
		}

		public Hops(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	// ********** Eggshells **********
	public class Eggshells : Item
	{
		[Constructable]
		public Eggshells()
			: base(0x9b4)
		{
			Weight = 0.5;
		}

		public Eggshells(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	// ********** Dough **********
	public class Dough : Item
	{
		[Constructable]
		public Dough()
			: base(0x103d)
		{
			Stackable = false;
			Weight = 1.0;
		}

		public Dough(Serial serial)
			: base(serial)
		{ }

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (EraML)
			{
				Stackable = true;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	// ********** SweetDough **********
	public class SweetDough : Item
	{
		public override int LabelNumber { get { return 1041340; } } // sweet dough

		[Constructable]
		public SweetDough()
			: base(0x103d)
		{
			Stackable = false;
			Weight = 1.0;
			Hue = 150;
		}

		public SweetDough(Serial serial)
			: base(serial)
		{ }

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (EraML)
			{
				Stackable = true;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			if (Hue == 51)
			{
				Hue = 150;
			}
		}
	}
}