#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
#endregion

namespace Server.Items
{
	public class Runebook : Item, ISecurable, ICraftable
	{
		//public static readonly TimeSpan UseDelay = TimeSpan.FromSeconds( 7.0 );
		public static readonly TimeSpan UseDelay = TimeSpan.FromSeconds(3.0);

		private string m_Description;
		private int m_DefaultIndex;
		private Mobile m_Crafter;
		private BookQuality m_Quality;

        private List<Mobile> m_Openers = new List<Mobile>();

		public List<RunebookEntry> Entries { get; private set; }
        public List<Mobile> Openers
        {
            get
            {
                return m_Openers;
            }
            set
            {
                m_Openers = value;
            }
        }

		public RunebookEntry Default
		{
			get
			{
				if (m_DefaultIndex >= 0 && m_DefaultIndex < Entries.Count)
				{
					return Entries[m_DefaultIndex];
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
					m_DefaultIndex = Entries.IndexOf(value);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public BookQuality Quality
		{
			get { return m_Quality; }
			set
			{
				m_Quality = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime NextUse { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Crafter
		{
			get { return m_Crafter; }
			set
			{
				m_Crafter = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public SecureLevel Level { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string Description
		{
			get { return m_Description; }
			set
			{
				m_Description = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int CurCharges { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxCharges { get; set; }

		public override Type DyeType { get { return typeof(RunebookDyeTub); } }
		public override bool DisplayDyable { get { return false; } }

		//public override bool DisplayLootType{ get{ return EraAOS; } }
		public override bool DisplayLootType { get { return false; } }

		public override int LabelNumber { get { return 1041267; } } // runebook

		[Constructable]
		public Runebook()
			: this(6)
		{ }

		[Constructable]
		public Runebook(int maxCharges)
			: base(0x22C5)
		{
			Entries = new List<RunebookEntry>();
			Openers = new List<Mobile>();

			m_DefaultIndex = -1;
			MaxCharges = maxCharges;

			Level = SecureLevel.CoOwners;

			Weight = 3.0;
			Hue = 0x461;

			LootType = LootType.Blessed;
			Layer = Layer.OneHanded;

			Dyable = true;
		}

		public Runebook(Serial serial)
			: base(serial)
		{ }

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			Layer = EraAOS ? Layer.Invalid : Layer.OneHanded;

			if (EraSE)
			{
				Weight = 1.0;

				if (MaxCharges < 12)
				{
					MaxCharges = 12;
				}
			}
			else
			{
				Weight = 3.0;

				if (MaxCharges < 6)
				{
					MaxCharges = 6;
				}
			}
		}

		public override bool AllowEquippedCast(Mobile from)
		{
			return true;
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			SetSecureLevelEntry.AddTo(from, this, list);
		}

		public void DropRune(Mobile from, RunebookEntry e, int index)
		{
			if (m_DefaultIndex > index)
			{
				m_DefaultIndex -= 1;
			}
			else if (m_DefaultIndex == index)
			{
				m_DefaultIndex = -1;
			}

			Entries.RemoveAt(index);

			from.AddToBackpack(
				new RecallRune
				{
					Target = e.Location,
					TargetMap = e.Map,
					Description = e.Description,
					House = e.House,
					Marked = true,
                    Hue = e.Hue
				});

			from.SendLocalizedMessage(502421); // You have removed the rune.
		}

		public bool IsOpen(Mobile toCheck)
		{
			NetState ns = toCheck.NetState;

			if (ns != null)
			{
				return ns.Gumps.OfType<RunebookGump>().Any(bookGump => bookGump.Book == this);
			}

			return false;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (m_Quality == BookQuality.Exceptional)
			{
				list.Add(1063341); // exceptional
			}

			if (m_Crafter != null)
			{
				list.Add(1050043, m_Crafter.RawName); // crafted by ~1_NAME~
			}

			if (!String.IsNullOrEmpty(m_Description))
			{
				list.Add(m_Description);
			}
		}

		public override bool OnDragLift(Mobile from)
		{
			if (from.HasGump(typeof(RunebookGump)))
			{
				from.SendLocalizedMessage(500169); // You cannot pick that up.
				return false;
			}

			foreach (Mobile m in Openers.Where(IsOpen))
			{
				m.CloseGump(typeof(RunebookGump));
			}

			Openers.Clear();

			return true;
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (!String.IsNullOrEmpty(m_Description))
			{
				LabelTo(from, m_Description);
			}

			if (m_Crafter != null)
			{
				LabelTo(from, 1050043, m_Crafter.RawName);
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			// ADD SPECIAL GUMP HERE
			if (RootParent is PlayerVendor)
			{
				from.CloseGump(typeof(RunebookSellingGump));
				from.SendGump(new RunebookSellingGump(this));
				return;
			}

			if (!from.InRange(GetWorldLocation(), (EraML ? 3 : 2)) /* || !CheckAccess(from)*/)
			{
				return;
			}

			if (RootParent is BaseCreature)
			{
				from.SendLocalizedMessage(502402); // That is inaccessible.
				return;
			}

			if (DateTime.UtcNow < NextUse)
			{
				from.SendLocalizedMessage(502406); // This book needs time to recharge.
				return;
			}

			from.CloseGump(typeof(RunebookGump));
			from.SendGump(new RunebookGump(from, this));

			Openers.Add(from);
		}

		public virtual void OnTravel()
		{
			if (!EraSA)
			{
				NextUse = DateTime.UtcNow + UseDelay;
			}
		}

		public override void OnAfterDuped(Item newItem)
		{
			var book = newItem as Runebook;

			if (book == null)
			{
				return;
			}

			book.Entries = new List<RunebookEntry>();

			for (int i = 0; i < Entries.Count; i++)
			{
				RunebookEntry entry = Entries[i];

				book.Entries.Add(new RunebookEntry(entry.Location, entry.Map, entry.Description, entry.House, entry.Hue));
			}
		}

		public bool CheckAccess(Mobile m)
		{
			if (!IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster)
			{
				return true;
			}

			BaseHouse house = BaseHouse.FindHouseAt(this);

			if (house != null && house.IsAosRules && (house.Public ? house.IsBanned(m) : !house.HasAccess(m)))
			{
				return false;
			}

			return house != null && house.HasSecureAccess(m, Level);
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (dropped is RecallRune)
			{
				if (IsLockedDown && from.AccessLevel < AccessLevel.GameMaster)
				{
					// That cannot be done while the book is locked down.
					from.SendLocalizedMessage(502413, null, 0x35);
				}
				else if (IsOpen(from))
				{
					// You cannot place objects in the book while viewing the contents.
					from.SendLocalizedMessage(1005571);
				}
				else if (Entries.Count < 16)
				{
					var rune = (RecallRune)dropped;

					if (rune.Marked && rune.TargetMap != null)
					{
						Entries.Add(new RunebookEntry(rune.Target, rune.TargetMap, rune.Description, rune.House, rune.Hue));

						dropped.Delete();

						from.Send(new PlaySound(0x42, GetWorldLocation()));

						string desc = rune.Description;

						if (desc == null || (desc = desc.Trim()).Length == 0)
						{
							desc = "(indescript)";
						}

						from.SendMessage(desc);

						return true;
					}

					from.SendLocalizedMessage(502409); // This rune does not have a marked location.
				}
				else
				{
					from.SendLocalizedMessage(502401); // This runebook is full.
				}
			}
			else if (dropped is RecallScroll)
			{
				if (CurCharges < MaxCharges)
				{
					from.Send(new PlaySound(0x249, GetWorldLocation()));

					int amount = dropped.Amount;

					if (amount > (MaxCharges - CurCharges))
					{
						dropped.Consume(MaxCharges - CurCharges);
						CurCharges = MaxCharges;
					}
					else
					{
						CurCharges += amount;
						dropped.Delete();

						return true;
					}
				}
				else
				{
					from.SendLocalizedMessage(502410); // This book already has the maximum amount of charges.
				}
			}

			return false;
		}

		#region ICraftable Members
		public int OnCraft(
			int quality,
			bool makersMark,
			Mobile from,
			CraftSystem craftSystem,
			Type typeRes,
			IBaseTool tool,
			CraftItem craftItem,
			int resHue)
		{
			int charges = 5 + quality + (int)(from.Skills[SkillName.Inscribe].Value / 30);

			if (charges > 10)
			{
				charges = 10;
			}

			MaxCharges = charges;

			if (makersMark)
			{
				Crafter = from;
			}

			m_Quality = (BookQuality)(quality - 1);

			return quality;
		}
		#endregion
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(3);

			writer.Write((byte)m_Quality);

			writer.Write(m_Crafter);

			writer.Write((int)Level);

			writer.Write(Entries.Count);

			foreach (RunebookEntry e in Entries)
			{
				e.Serialize(writer);
			}

			writer.Write(m_Description);
			writer.Write(CurCharges);
			writer.Write(MaxCharges);
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

			int version = reader.ReadInt();

			switch (version)
			{
				case 3:
					m_Quality = (BookQuality)reader.ReadByte();
					goto case 2;
				case 2:
					m_Crafter = reader.ReadMobile();
					goto case 1;
				case 1:
					Level = (SecureLevel)reader.ReadInt();
					goto case 0;
				case 0:
					{
						int count = reader.ReadInt();

						Entries = new List<RunebookEntry>(count);

						for (int i = 0; i < count; ++i)
						{
							Entries.Add(new RunebookEntry(reader));
						}

						m_Description = reader.ReadString();
						CurCharges = reader.ReadInt();
						MaxCharges = reader.ReadInt();
						m_DefaultIndex = reader.ReadInt();
					}
					break;
			}
		}
	}

	public class RunebookEntry
	{
		public Point3D Location { get; private set; }
		public Map Map { get; private set; }

        public int Hue { get; private set; }

		public string Description { get; private set; }

		public BaseHouse House { get; private set; }

		public RunebookEntry(Point3D loc, Map map, string desc, BaseHouse house)
			: this(loc, map, desc, house, 0)
		{ }

		public RunebookEntry(Point3D loc, Map map, string desc, BaseHouse house, int hue)
		{
			Location = loc;
			Map = map;
			Description = desc;
			House = house;
		    Hue = hue;
		}

		public RunebookEntry(GenericReader reader)
		{
			Deserialize(reader);
		}

		public void Serialize(GenericWriter writer)
		{
			if (House != null && !House.Deleted)
			{
				writer.Write((byte)1);

                writer.Write(Hue);

				writer.Write(House);
			}
			else
			{
				writer.Write((byte)0);

                writer.Write(Hue);
			}

			writer.Write(Location);
			writer.Write(Map);
			writer.Write(Description);
		}

		public void Deserialize(GenericReader reader)
		{
			if (reader.ReadByte() > 0)
			{
			    Hue = reader.ReadInt();
				House = reader.ReadItem<BaseHouse>();
			}
			else
			{
                Hue = reader.ReadInt();
			}
			
			Location = reader.ReadPoint3D();
			Map = reader.ReadMap();
			Description = reader.ReadString();
		}
	}
}