#region References
using Server.Items;
#endregion

namespace Server.Scripts.Engines.ValorSystem
{
	public class ValorItem : Item
	{
		public Item ValorItemInfo { get; private set; }

		public int Cost { get; set; }
		public string Cat { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		public ValorItem()
		{
			ValorItemInfo = new Item();
			Cost = 0;
			Cat = null;
			Title = "";
			Description = "";
		}

		public ValorItem(Item i)
		{
			ValorItemInfo = i;
			Cost = 0;
			Cat = null;
			Title = i.ResolveName();
			Description = "Enter description here.";
		}

		public ValorItem(Item i, int cost)
		{
			ValorItemInfo = i;
			Cost = cost;
			Cat = null;
			Title = i.ResolveName();
			Description = "Enter description here.";
		}

		public ValorItem(Item i, int cost, string cat)
		{
			ValorItemInfo = i;
			Cost = cost;
			Cat = cat;
			Title = i.ResolveName();
			Description = "Enter description here.";
		}

		public ValorItem(Item i, int cost, string cat, string title)
		{
			ValorItemInfo = i;
			Cost = cost;
			Cat = cat;
			Title = title;
			Description = "Enter description here.";
		}

		public ValorItem(Item i, int cost, string cat, string title, string desc)
		{
			ValorItemInfo = i;
			Cost = cost;
			Cat = cat;
			Title = title;
			Description = desc;
		}

		public void Edit(int cost, string cat, string title, string desc, bool istitle)
		{
			if (cost < 0 || title == null || desc == null)
			{
				return;
			}

			Cost = cost;
			Cat = cat;
			Title = title;
			Description = desc;
		}

		public Item GetValorItem()
		{
			return ItemClone.Clone(ValorItemInfo, false);
		}

		public ValorItem(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);

			writer.WriteItem(ValorItemInfo);
			writer.Write(Cost);
			writer.Write(Cat);
			writer.Write(Title);
			writer.Write(Description);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						ValorItemInfo = reader.ReadItem();
						Cost = reader.ReadInt();
						Cat = reader.ReadString();
						Title = reader.ReadString();
						Description = reader.ReadString();

						break;
					}
			}
		}
	}
}