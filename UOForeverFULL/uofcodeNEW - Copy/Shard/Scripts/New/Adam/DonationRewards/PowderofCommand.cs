#region References

using Server.Network;
using Server.Targeting;

#endregion

namespace Server.Items
{
    public class PowderofCommand : Item
    {
        [Constructable]
        public PowderofCommand()
            : this(1)
        {}

        [Constructable]
        public PowderofCommand(int amount)
            : base(0x26B8)
        {
            Name = "powder of command";
            Stackable = true;
            Weight = 0.1;
            LootType = LootType.Blessed;
            Amount = amount;
            Hue = 1194;
        }

        public PowderofCommand(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 2))
            {
                from.SendMessage(61,"Target the wand of command which you would like to have absorb this powder.");
                from.Target = new InternalTarget(this);
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private class InternalTarget : Target
        {
            private readonly PowderofCommand m_Powder;

            public InternalTarget(PowderofCommand powder)
                : base(-1, false, TargetFlags.None)
            {
                m_Powder = powder;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Powder.Deleted)
                {
                    return;
                }

                if (!from.InRange(m_Powder.GetWorldLocation(), 2))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                }
                else if (targeted is WandofLoyalty)
                {
                    var wand = (WandofLoyalty) targeted;

                    wand.CommandCharges += m_Powder.Amount;
                    m_Powder.Delete();


                    // The ~1_translocationItem~ glows with green energy and absorbs magical power from the powder.
                    from.SendMessage(61, "The wand of command glows with green enrgy and absorbs the magic power from the powder.");
                }
                else
                {
                    from.SendMessage(61, "This powder is only usable on a wand of command.");
                }
            }
        }
    }
}