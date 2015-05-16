//
//  Written by Haazen May 2006
//

#region References
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server.ContextMenus;
using Server.Gumps;
using Server.Network;
using Server.Prompts;
#endregion

namespace Server.Items
{
	public class TMapBook : Item
	{
		private ArrayList m_Entries;
		private int m_DefaultIndex;

		[Constructable]
		public TMapBook()
			: base(0x2252)
		{
			Weight = (1.0);
			LootType = LootType.Blessed;
			Hue = 1265;
			Name = "TMap Book";

			Layer = Layer.OneHanded;
			m_Entries = new ArrayList();
			m_DefaultIndex = -1;
		}

		public ArrayList Entries { get { return m_Entries; } }

		public TMapBookEntry Default
		{
			get
			{
				if (m_DefaultIndex >= 0 && m_DefaultIndex < m_Entries.Count)
				{
					return (TMapBookEntry)m_Entries[m_DefaultIndex];
				}

				return null;
			}
			set
			{
				if (value == null)
				{
					m_DefaultIndex = -1;
				}
				else
				{
					m_DefaultIndex = m_Entries.IndexOf(value);
				}
			}
		}

		public TMapBook(Serial serial)
			: base(serial)
		{ }

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from.CheckAlive() && IsChildOf(from.Backpack))
			{
				list.Add(new NameBookEntry(from, this));
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);

			writer.Write(m_Entries.Count);

			foreach (TMapBookEntry e in m_Entries.OfType<TMapBookEntry>())
			{
				e.Serialize(writer);
			}

			writer.Write(m_DefaultIndex);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			LootType = LootType.Blessed;

			if (EraSE && Weight == 3.0)
			{
				Weight = 1.0;
			}

			reader.ReadInt(); // Version

			int count = reader.ReadInt();

			m_Entries = new ArrayList(count);

			for (int i = 0; i < count; ++i)
			{
				m_Entries.Add(new TMapBookEntry(reader));
			}

			m_DefaultIndex = reader.ReadInt();
		}

		public void DropTMap(Mobile from, TMapBookEntry e, int index)
		{
			if (m_DefaultIndex == index)
			{
				m_DefaultIndex = -1;
			}

			m_Entries.RemoveAt(index);

			var tmap = new TreasureMap(e.Level, e.Map) {
				Decoder = e.Decoder,
				ChestLocation = e.Location,
				ChestMap = e.Map,
				Bounds = e.Bounds
			};

			tmap.ClearPins();
			tmap.AddWorldPin(e.Location.X, e.Location.Y);

			from.AddToBackpack(tmap);

			from.SendMessage("You have removed the Treasure Map");
		}

		public void ViewMap(Mobile from, TMapBookEntry e, int index)
		{
			if (m_DefaultIndex == index)
			{
				m_DefaultIndex = -1;
			}

			from.CloseGump(typeof(MapDisplayGump));
			from.SendGump(new MapDisplayGump(from, e.Location.X, e.Location.Y));
		}

		public override bool DisplayLootType { get { return EraAOS; } }

		public override void OnDoubleClick(Mobile from)
		{
			if (from.InRange(GetWorldLocation(), 1))
			{
				from.CloseGump(typeof(TMapBookGump));
				from.SendGump(new TMapBookGump(from, this));
			}
		}

		public bool CheckAccess(Mobile m)
		{
			return true;
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (dropped is TreasureMap)
			{
				if (!CheckAccess(from))
				{
					from.SendLocalizedMessage(502413); // That cannot be done while the book is locked down.
				}
				/*
				else if (IsOpen(from))
				{
					from.SendLocalizedMessage( 1005571 ); // You cannot place objects in the book while viewing the contents.
				}
				*/
				else if (m_Entries.Count < 32)
				{
					var tmap = (TreasureMap)dropped;
					
					if (tmap.Completed)
					{
						from.SendMessage("This map is completed and can not be stored in this book");
						InvalidateProperties();
						dropped.Delete();
						return false;
					}

					if (tmap.ChestMap != null)
					{
						m_Entries.Add(new TMapBookEntry(tmap.Level, tmap.Decoder, tmap.ChestMap, tmap.ChestLocation, tmap.Bounds));
						InvalidateProperties();
						dropped.Delete();
						from.Send(new PlaySound(0x42, GetWorldLocation()));
						from.CloseGump(typeof(TMapBookGump));
						return true;
					}
					
					from.SendMessage("This map is invalid");
				}
				else
				{
					from.SendMessage("This TMap Book is full");
				}
			}

			return false;
		}

		private class NameBookEntry : ContextMenuEntry
		{
			private readonly Mobile m_From;
			private readonly TMapBook m_Book;

			public NameBookEntry(Mobile from, TMapBook book)
				: base(6216)
			{
				m_From = from;
				m_Book = book;
			}

			public override void OnClick()
			{
				if (m_From.CheckAlive() && m_Book.IsChildOf(m_From.Backpack))
				{
					m_From.Prompt = new NameBookPrompt(m_Book);
					m_From.SendLocalizedMessage(1062479); // Type in the new name of the book:
				}
			}
		}

		private class NameBookPrompt : Prompt
		{
			private readonly TMapBook m_Book;

			public NameBookPrompt(TMapBook book)
			{
				m_Book = book;
			}

			public override void OnResponse(Mobile from, string text)
			{
				if (text.Length > 40)
				{
					text = text.Substring(0, 40);
				}

				if (!from.CheckAlive() || !m_Book.IsChildOf(from.Backpack))
				{
					return;
				}

				m_Book.Name = Utility.FixHtml(text.Trim());

				from.SendMessage("This TMap Book name has been changed");
			}

			public override void OnCancel(Mobile from)
			{ }
		}
	}

	public class TMapBookEntry
	{
		public int Level { get; private set; }
		public Mobile Decoder { get; private set; }
		public Map Map { get; private set; }
		public Point2D Location { get; private set; }
		public Rectangle2D Bounds { get; private set; }

		public TMapBookEntry(int level, Mobile decoder, Map map, Point2D loc, Rectangle2D bounds)
		{
			Level = level;
			Decoder = decoder;
			Map = map;
			Location = loc;
			Bounds = bounds;
		}

		public TMapBookEntry(GenericReader reader)
		{
			Deserialize(reader);
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write((byte)0); // version

			writer.Write(Level);
			writer.Write(Decoder);
			writer.Write(Map);
			writer.Write(Location);
			writer.Write(Bounds);
		}

		public void Deserialize(GenericReader reader)
		{
			reader.ReadByte();

			Level = reader.ReadInt();
			Decoder = reader.ReadMobile();
			Map = reader.ReadMap();
			Location = reader.ReadPoint2D();
			Bounds = reader.ReadRect2D();
		}
	}
}