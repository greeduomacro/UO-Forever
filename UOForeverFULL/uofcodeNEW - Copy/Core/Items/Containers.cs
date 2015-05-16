#region References
using System;

using Server.Network;
#endregion

namespace Server.Items
{
	public class BankBox : Container
	{
		public static readonly ulong MaxCredit = UInt64.MaxValue;

		public static bool SendDeleteOnClose { get; set; }

		[CommandProperty(AccessLevel.Counselor)]
		public override bool ExpansionChangeAllowed { get { return false; } }

		public override int DefaultMaxWeight { get { return 0; } }
		public override bool IsVirtualItem { get { return true; } }

		public override string DefaultName { get { return "Bank [" + Expansion + "]"; } }

		public Mobile Owner { get; private set; }
		public bool Opened { get; private set; }

		[CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
		public ulong Credit { get; set; }

		public BankBox(Mobile owner)
			: this(owner, Expansion.None)
		{ }

		public BankBox(Mobile owner, Expansion e)
			: base(0xE7C)
		{
			Owner = owner;
			Expansion = e;

			Movable = false;
			Layer = Layer.Bank;
		}

		public BankBox(Serial serial)
			: base(serial)
		{ }

		public void Open()
		{
			Opened = true;

			if (Owner == null)
			{
				return;
			}

			Owner.PrivateOverheadMessage(
				MessageType.Regular,
				0x3B2,
				true,
				String.Format("Bank container has {0} items, {1} stones", TotalItems, TotalWeight),
				Owner.NetState);
			Owner.Send(new EquipUpdate(this));

			DisplayTo(Owner);
		}

		public void Close()
		{
			Opened = false;

			if (Owner != null && SendDeleteOnClose)
			{
				Owner.Send(RemovePacket);
			}
		}

		public override void OnSingleClick(Mobile from)
		{ }

		public override void OnDoubleClick(Mobile from)
		{ }

		public override DeathMoveResult OnParentDeath(Mobile parent)
		{
			return DeathMoveResult.RemainEquipped;
		}

		public override bool IsAccessibleTo(Mobile check)
		{
			return ((check == Owner && Opened) || check.AccessLevel >= AccessLevel.GameMaster) && base.IsAccessibleTo(check);
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			return ((from == Owner && Opened) || from.AccessLevel >= AccessLevel.GameMaster) && base.OnDragDrop(from, dropped);
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			return ((from == Owner && Opened) || from.AccessLevel >= AccessLevel.GameMaster) &&
				   base.OnDragDropInto(from, item, p);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write(Credit);

			writer.Write(Owner);
			writer.Write(Opened);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					Credit = reader.ReadULong();
					goto case 0;
				case 0:
					{
						Owner = reader.ReadMobile();
						Opened = reader.ReadBool();

						if (Owner == null)
						{
							Delete();
						}
					}
					break;
			}
		}
	}
}