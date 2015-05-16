#region References
using System;
using System.Globalization;
using System.Reflection;

using Server.Commands;
using Server.Mobiles;
#endregion

namespace Server.Items
{
	public class VendStone : Item
	{
		private string m_ItemName;
		private string m_Parameters;
		private int m_Price;

		[CommandProperty(AccessLevel.GameMaster)]
		public string ItemName
		{
			get { return m_ItemName; }
			set
			{
				m_ItemName = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string ItemType { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string Parameters
		{
			get { return m_Parameters; }
			set
			{
				m_Parameters = value;
				CreateParameters();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Price
		{
			get { return m_Price; }
			set
			{
				m_Price = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int TextHue { get; set; }

		private string[] m_ParamList;

		public string FullName { get { return String.Format(m_ItemName, 0); } }

		public override int LabelNumber { get { return 0; } }
		public override string DefaultName { get { return "Vending Stone"; } }

		public virtual Type TypeOfCurrency { get { return Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold); } }

		[Constructable]
		public VendStone()
			: this(0x2D1, "BagOfReagents", "Bag of 60 Reagents", 2500)
		{
			Parameters = "60";
		}

		[Constructable]
		public VendStone(int hue, string itype, string tname, int price)
			: this(hue, itype, tname, price, String.Empty)
		{ }

		[Constructable]
		public VendStone(int hue, string itype, string tname, int price, string parameters)
			: base(0xED4)
		{
			Movable = false;
			ItemType = itype;
			m_ItemName = tname;
			m_Price = price;
			Hue = hue;
			TextHue = 1346;
			Parameters = parameters;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (ItemType != null)
			{
				list.Add(1060661, "Item\t{0}", m_ItemName);
			}
			else
			{
				list.Add(1060661, "Item\tNothing");
			}

			if (m_Price > 0)
			{
				list.Add(1060659, "Price\t{0:#,0}", m_Price);
			}
			else
			{
				list.Add(1060659, "Price\tFree");
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			LabelToExpansion(from);

			bool free = m_Price <= 0;
			LabelTo(
				from,
				TextHue,
				"{0} for {1}{2}",
				FullName,
				free ? "free" : m_Price.ToString("#,0"),
				free ? String.Empty : " " + TypeOfCurrency.Name);
		}

		private void CreateParameters()
		{
			m_ParamList = String.IsNullOrEmpty(m_Parameters) ? new string[0] : m_Parameters.Trim().Split(' ');
		}

		public override void OnDoubleClick(Mobile from)
		{
			Type type = SpawnerType.GetType(ItemType);

			if (type != null && type.IsEqualOrChildOf<Item>())
			{
				if (m_Price < 0)
				{
					m_Price = 0;
				}

				if (from.AccessLevel < AccessLevel.GameMaster && m_Price > 0)
				{
					int totalGold = 0;

					if (from.Backpack != null)
					{
						totalGold += from.Backpack.GetAmount(TypeOfCurrency, true);
					}

					totalGold += Banker.GetBalance(from, TypeOfCurrency);

					if (totalGold < m_Price)
					{
						from.SendMessage(1153, "You lack the funds to purchase this.");
						return;
					}
				}

				object o = null;

				try
				{
					ConstructorInfo[] ctors = type.GetConstructors();

					foreach (ConstructorInfo ctor in ctors)
					{
						if (!Add.IsConstructable(ctor, AccessLevel.GameMaster))
						{
							continue;
						}

						ParameterInfo[] paramList = ctor.GetParameters();

						if (m_ParamList.Length != paramList.Length)
						{
							continue;
						}

						object[] paramValues = Add.ParseValues(paramList, m_ParamList);

						if (paramValues == null)
						{
							continue;
						}

						o = ctor.Invoke(paramValues);
						break;
					}
				}
				catch
				{
					Console.WriteLine(
						"VendStone: Invalid constructor or parameters for {0}: {1} {2}", Serial, ItemType, m_Parameters);
				}

				var item = o as Item;

				if (item != null && from.Backpack != null)
				{
					if (from.AddToBackpack(item))
					{
						from.SendMessage("You place the {0} into your backpack.", FullName);
						from.PlaySound(from.Backpack.GetDroppedSound(item));

						int leftPrice = m_Price;

						if (from.Backpack != null)
						{
							leftPrice -= from.Backpack.ConsumeUpTo(TypeOfCurrency, leftPrice, true);
						}

						if (leftPrice > 0)
						{
							Banker.Withdraw(from, TypeOfCurrency, leftPrice);
						}
					}
					else
					{
						from.SendMessage("You do not have room for this item in your backpack.");
						int sound = item.GetDropSound();
						from.PlaySound(sound == -1 ? 0x42 : sound);
					}
				}
			}
			else
			{
				from.SendMessage("The magic from this stone has faded.");
			}
		}

		public VendStone(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(1); // version

			writer.Write(m_Parameters);
			writer.Write(TextHue);

			writer.Write(m_ItemName);
			writer.Write(ItemType);
			writer.Write(m_Price);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();

			switch (version)
			{
				case 1:
					{
						Parameters = reader.ReadString();
						TextHue = reader.ReadInt();
						goto case 0;
					}
				case 0:
					{
						m_ItemName = reader.ReadString();
						ItemType = reader.ReadString();
						m_Price = reader.ReadInt();

						if (version < 1)
						{
							int amount = reader.ReadInt();

							Parameters = amount.ToString(CultureInfo.InvariantCulture);

							if (amount > 0)
							{
								m_ItemName = String.Format(m_ItemName, amount);
							}
						}

						break;
					}
			}
		}
	}
}