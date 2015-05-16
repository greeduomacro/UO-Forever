#region References
using System;
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Multis;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server.Items
{
	public abstract class BaseDyeTub : Item, IDyeTub
	{
		private bool m_Redyable;
		private int m_DyedHue;
		private SecureLevel m_SecureLevel;
		private int m_UsesRemaining;

		[CommandProperty(AccessLevel.Administrator)]
		public int UsesRemaining { get { return m_UsesRemaining; } set { m_UsesRemaining = value; } }

		public virtual CustomHuePicker CustomHuePicker { get { return null; } }

		public override bool DisplayDyable { get { return false; } }

		public virtual bool IsDyable(Item item)
		{
			//return item.DyeType == GetType();
			return item.DyeType.IsAssignableFrom(GetType());
		}

		public virtual bool Dye(Mobile from, Item item)
		{
			if (item.Parent is Mobile)
			{
				from.SendMessage("You decided not to dye this while it is worn."); // Can't Dye clothing that is being worn.
			}
			else if (item.Dye(from, this))
			{
				from.PlaySound(0x23E);
				return true;
			}
			else
			{
				TextDefinition.SendMessageTo(from, FailMessage);
			}

			return false;
		}

		public virtual void DisplayDurabilityTo(Mobile m)
		{
			LabelToAffix(m, 1041367, AffixType.Append, " " + m_UsesRemaining.ToString()); // charges:
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (m_UsesRemaining > -1)
			{
				DisplayDurabilityTo(from);
			}
		}

		public override bool Dye(Mobile from, IDyeTub sender)
		{
			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(3); // version

			writer.WriteEncodedInt(m_UsesRemaining);

			writer.Write((int)m_SecureLevel);
			writer.Write(m_Redyable);
			writer.Write(m_DyedHue);
		}

		internal int m_Version;

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			m_Version = version;

			switch (version)
			{
				case 3:
				case 2:
					{
						m_UsesRemaining = reader.ReadEncodedInt();
						goto case 1;
					}
				case 1:
					{
						m_SecureLevel = (SecureLevel)reader.ReadInt();
						goto case 0;
					}
				case 0:
					{
						m_Redyable = reader.ReadBool();
						m_DyedHue = reader.ReadInt();

						break;
					}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Redyable { get { return m_Redyable; } set { m_Redyable = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DyedHue
		{
			get { return m_DyedHue; }
			set
			{
				if (m_Redyable)
				{
					m_DyedHue = value;
					Hue = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public SecureLevel Level { get { return m_SecureLevel; } set { m_SecureLevel = value; } }

		//[Constructable]
		public BaseDyeTub()
			: this(0)
		{
			Weight = 10.0;
			m_Redyable = true;
		}

		/// [Constructable]
		public BaseDyeTub(int hue)
			: this(hue, true)
		{ }

		//[Constructable]
		public BaseDyeTub(int hue, bool redyable)
			: this(hue, true, -1)
		{ }

		//[Constructable]
		public BaseDyeTub(int hue, bool redyable, int uses)
			: base(0xFAB)
		{
			m_DyedHue = Hue = hue;
			Weight = 10.0;
			m_Redyable = redyable;
			m_UsesRemaining = uses;
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);
			SetSecureLevelEntry.AddTo(from, this, list);
		}

		public BaseDyeTub(Serial serial)
			: base(serial)
		{ }

		// Select the clothing to dye.
		public virtual TextDefinition TargetMessage { get { return new TextDefinition(500859); } }

		// You cannot dye that.
		public virtual TextDefinition FailMessage { get { return new TextDefinition(1042083); } }

		public override void OnDoubleClick(Mobile from)
		{
			if (from.InRange(GetWorldLocation(), 1))
			{
				TextDefinition.SendMessageTo(from, TargetMessage);
				from.Target = new InternalTarget(this);
			}
			else
			{
				from.SendLocalizedMessage(500446); // That is too far away.
			}
		}

		private class InternalTarget : Target
		{
			private readonly BaseDyeTub m_Tub;

			public InternalTarget(BaseDyeTub tub)
				: base(1, false, TargetFlags.None)
			{
				m_Tub = tub;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (targeted is Item)
				{
					Item item = (Item)targeted;
					if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
					{
						from.SendLocalizedMessage(500446); // That is too far away.
					}
                    else if (targeted is MetalKiteShield && m_Tub is DyeTub)
                        m_Tub.Dye(from, item);
                    else if (targeted is HouseToHouseTeleporter && m_Tub is FurnitureDyeTub)
                        m_Tub.Dye(from, item);
                    else if (!item.Deleted &&
                             (item.Dyable ||
                              (m_Tub.UsesRemaining >= 0 && m_Tub.Redyable && item is BaseDyeTub && ((BaseDyeTub)item).Redyable &&
                               ((BaseDyeTub)item).UsesRemaining >= 0)) && m_Tub.IsDyable(item))
					{
					    if (m_Tub.Dye(from, item))
					    {
                            if (m_Tub.UsesRemaining > 0)
                            {
                                m_Tub.UsesRemaining--;
                                if (m_Tub.UsesRemaining == 0)
                                {
                                    m_Tub.Delete();
                                    from.SendMessage(54, "Your dye tub has run out of charges!");
                                }
                            }
					    }
					}
					else
					{
						TextDefinition.SendMessageTo(from, m_Tub.FailMessage);
					}
				}
				else
				{
					TextDefinition.SendMessageTo(from, m_Tub.FailMessage);
				}
			}
		}
	}
}