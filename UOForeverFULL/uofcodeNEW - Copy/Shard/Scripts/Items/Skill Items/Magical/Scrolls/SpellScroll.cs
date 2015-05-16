#region References
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Factions;
using Server.Multis;
using Server.Spells;
using Server.Targeting;
#endregion

namespace Server.Items
{
	public class SpellScroll : Item, IFactionItem, ICommodity
	{
		private FactionItem m_FactionState;

		public FactionItem FactionItemState
		{
			get { return m_FactionState; }
			set
			{
				m_FactionState = value;

				LootType = (m_FactionState == null ? LootType.Regular : LootType.Blessed);
			}
		}

		private int m_SpellID;

		public int SpellID { get { return m_SpellID; } }
		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return EraML; } }

		public SpellScroll(Serial serial)
			: base(serial)
		{ }

		[Constructable]
		public SpellScroll(int spellID, int itemID)
			: this(spellID, itemID, 1)
		{ }

		[Constructable]
		public SpellScroll(int spellID, int itemID, int amount)
			: base(itemID)
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;

			m_SpellID = spellID;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(m_SpellID);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_SpellID = reader.ReadInt();

						break;
					}
			}
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from.Alive && Movable)
			{
				list.Add(new AddToSpellbookEntry());
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (m_FactionState != null)
			{
				LabelTo(from, "(faction item)");
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!DesignContext.Check(from))
			{
				return; // They are customizing
			}

			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
			else
			{
				if (m_FactionState != null)
				{
					if (from.Skills.Inscribe.Value >= 90.0)
					{
						PlayerState state = PlayerState.Find(from);

						if (state != null && state.Faction == m_FactionState.Faction)
						{
							from.SendLocalizedMessage(1010378); // Select a gnarled faction staff to charge
							from.Target = new SpellScrollTarget(this);
						}
						else
						{
							from.SendLocalizedMessage(1010377); // You may not use a scroll crafted by the other factions!
						}
					}
					else
					{
						from.SendLocalizedMessage(1005420); // You cannot use this.
					}
				}
				else
				{
					Spell spell = SpellRegistry.NewSpell(m_SpellID, from, this);

					if (spell != null)
					{
						spell.Cast();
					}
					else
					{
						from.SendLocalizedMessage(502345); // This spell has been temporarily disabled.
					}
				}
			}
		}

		private class SpellScrollTarget : Target
		{
			private readonly SpellScroll m_Scroll;

			public SpellScrollTarget(SpellScroll scroll)
				: base(12, false, TargetFlags.None)
			{
				m_Scroll = scroll;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				var staff = targeted as GnarledStaff;

				if (staff == null)
				{
					from.SendLocalizedMessage(1010387); // You cant use a faction scroll on that!
				}
				else if (staff.FactionItemState == null)
				{
					from.SendLocalizedMessage(1010386); // This staff is not faction made and thus may not be charged
				}
				else
				{
					PlayerState state = PlayerState.Find(from);

					if (state.Faction != staff.FactionItemState.Faction)
					{
						from.SendLocalizedMessage(1010385); // You may not charge enemy faction staves!
					}
					else
					{
						staff.Charge(from, m_Scroll);
					}
				}
			}
		}
	}
}