#region References

using Server.Targeting;

#endregion

namespace Server.Items
{
    public class RuneDyeTub : DyeTub
    {
        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.LeatherDyeTub; } }

        public int Uses { get; set; }

        [Constructable]
        public RuneDyeTub() : this(0)
        {
            Uses = 10;
        }

        [Constructable]
        public RuneDyeTub(int hue) : this(hue, true)
        {
            Uses = 10;
        }

        [Constructable]
        public RuneDyeTub(int hue, bool redyable) : this(hue, true, -1)
        {
            Uses = 10;
        }

        [Constructable]
        public RuneDyeTub(int hue, bool redyable, int uses) : base(hue, redyable, uses)
        {
            Uses = 10;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Uses == 0)
            {
                from.SendMessage("This item cannot be used anymore as it has run out of charges.");
            }
            else
            {
                if (from.InRange(GetWorldLocation(), 1))
                {
                    from.SendMessage("Select the rune you wish to dye.");
                    from.Target = new InternalTarget(this);
                }
                else
                {
                    from.SendLocalizedMessage(500446); // That is too far away.
                }
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
                    var item = (Item) targeted;
                    if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                    {
                        from.SendLocalizedMessage(500446); // That is too far away.
                    }
                    else if (!item.Deleted &&
                             (item.Dyable ||
                              (m_Tub.UsesRemaining >= 0 && m_Tub.Redyable && item is BaseDyeTub &&
                               ((BaseDyeTub) item).Redyable &&
                               ((BaseDyeTub) item).UsesRemaining >= 0)) && m_Tub.IsDyable(item) && item is RecallRune)
                    {
                        m_Tub.Dye(from, item);
                    }
                    else
                    {
                        from.SendMessage("You cannot dye that.");
                    }
                }
                else
                {
                    from.SendMessage("You cannot dye that.");
                }
            }
        }

        public override bool Dye(Mobile from, Item item)
        {
            if (!item.Movable)
            {
                from.SendMessage("You may not dye runes that are locked down.");
            }
            else if (item.Dye(from, this))
            {
                Uses--;
                from.PlaySound(0x23E);
                return true;
            }

            return false;
        }

        public RuneDyeTub(Serial serial)
            : base(serial)
        {}

        public override void OnSingleClick(Mobile m)
        {
            LabelToExpansion(m);

            LabelTo(m, "a rune dyeing tub", 0);

            LabelTo(m, "Charges left: " + Uses, 54);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 1); // version

            writer.Write(Uses);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                {
                    Uses = reader.ReadInt();
                    break;
                }
            }
        }
    }
}