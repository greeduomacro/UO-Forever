#region References
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Targeting;

using VitaNex.Targets;
#endregion

namespace Server.Items
{
	[Furniture]
	[Flipable(16706, 16708)]
	public class Mailbox : Container, ISecurable, IChopable
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public bool PublicCanOpen { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool PublicCanDrop { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public SecureLevel Level { get; set; }

		public override int LabelNumber { get { return 1113927; } } // Mailbox
		public override string DefaultName { get { return "Mailbox"; } }

		public override int DefaultGumpID { get { return 0x11A; } }
		public override int DefaultDropSound { get { return 0x42; } }

		[Constructable]
		public Mailbox()
			: base(16706)
		{
			Level = SecureLevel.Anyone;
		}

		public Mailbox(Serial serial)
			: base(serial)
		{ }

		public virtual void OnChop(Mobile m)
		{
			if(!this.CheckDoubleClick(m, false, false, 8))
			{
				return;
			}

			var house = BaseHouse.FindHouseAt(this);

			if (m.AccessLevel < AccessLevel.Counselor && (house == null || !house.IsOwner(m)))
			{
				return;
			}

			Effects.PlaySound(Location, Map, 284);

			m.SendLocalizedMessage(500461); // You destroy the item.
			m.AddToBackpack(new MailboxDeed());

			Delete();
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!this.CheckDoubleClick(m, true, false, 2))
			{
				return;
			}

			if (m.AccessLevel < AccessLevel.Counselor)
			{
				var house = BaseHouse.FindHouseAt(this);

				if (house == null && !PublicCanOpen)
				{
					m.SendMessage("The {0} is not public, you can't open it.", this.ResolveName(m));
					return;
				}
			}

			base.OnDoubleClick(m);

			switch(ItemID)
			{
				case 16705:
				case 16707:
					++ItemID;
					break;
			}

			m.SendSound(45);
		}

		public override bool OnDragDrop(Mobile m, Item dropped)
		{
			if(!base.OnDragDrop(m, dropped) || !this.CheckDoubleClick(m, false, false, 2))
			{
				return false;
			}

			if(m.AccessLevel < AccessLevel.Counselor)
			{
				var house = BaseHouse.FindHouseAt(this);

				if(house == null && !PublicCanDrop)
				{
					m.SendMessage("The {0} is not public, you can't use it.", this.ResolveName(m));
					return false;
				}
			}

			switch(ItemID)
			{
				case 16705:
				case 16707:
					break;
				case 16706:
				case 16708:
					--ItemID;
					break;
				default:
					m.SendMessage("The {0} is full.", this.ResolveName(m));
					return false;
			}

			m.SendMessage("You put the item in the {0}.", this.ResolveName(m));
			return true;
		}

		public override void OnSingleClick(Mobile m)
		{
			base.OnSingleClick(m);

			var house = BaseHouse.FindHouseAt(this);

			if (house == null || house.Owner == null)
			{
				return;
			}

			LabelTo(m, "Owner: {0}", house.Owner.RawName); // Owner: ~1_OWNER~
			LabelTo(m, "{0} can drop items into the {1}.", Level, this.ResolveName(m));
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			var house = BaseHouse.FindHouseAt(this);

			if (house == null || house.Owner == null)
			{
				return;
			}

			list.Add("Owner: {0}", house.Owner.RawName); // Owner: ~1_OWNER~
			list.Add("{0} can drop items into the {1}.", Level, this.ResolveName(house.Owner));
		}

		public override void GetContextMenuEntries(Mobile m, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(m, list);

			SetSecureLevelEntry.AddTo(m, this, list);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(2); // version

			writer.WriteFlag(Level);

			writer.Write(PublicCanOpen);
			writer.Write(PublicCanDrop);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();

			switch(version)
			{
				case 2:
				case 1:
					{
						if(version < 2)
						{
							Level = (SecureLevel)reader.ReadInt();

							reader.ReadItem<BaseHouse>(); // Removed in version 2
						}
						else
						{
							Level = reader.ReadFlag<SecureLevel>();
						}

						PublicCanOpen = reader.ReadBool();
						PublicCanDrop = reader.ReadBool();
					}
					break;
			}
		}
	}

	public class MailboxDeed : Item
	{
		[Constructable]
		public MailboxDeed()
			: base(0x14F0)
		{
			Name = "mailbox deed";
			Weight = 1.0;
		}

		public MailboxDeed(Serial serial)
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

			reader.ReadInt(); // version
		}

		public override void OnDoubleClick(Mobile m)
		{
			if(!IsChildOf(m.Backpack))
			{
				m.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}

			m.SendMessage(
				1150,
				"Place the mailbox where you want it and remember to use the command: \"I wish to lock this down\". After locking it down remember to click on it to set the security. Default security is \"Anyone\".");
			m.Target = new InternalTarget(this);
		}

		private class InternalTarget : GenericSelectTarget<IPoint3D>
		{
			private readonly MailboxDeed _Deed;

			public InternalTarget(MailboxDeed deed)
				: base(null, null, -1, true, TargetFlags.None)
			{
				_Deed = deed;

				CheckLOS = false;
			}

			protected override void OnTarget(Mobile m, IPoint3D p)
			{
				if(m == null || m.Map == null || p == null || _Deed.Deleted)
				{
					return;
				}

				if(!_Deed.IsChildOf(m.Backpack))
				{
					m.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
					return;
				}

				SpellHelper.GetSurfaceTop(ref p);

				new Mailbox().MoveToWorld(p.Clone3D(), m.Map);

				_Deed.Delete();
			}
		}
	}
}