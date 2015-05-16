#region References

using System;
using Server.Mobiles;

#endregion

namespace Server.Items
{
    public class CharacterStatueMaker : Item
    {
        public override int LabelNumber { get { return 1076173; } } // Character Statue Maker

        private StatueType m_Type;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool NoHouse { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public StatueType StatueType
        {
            get { return m_Type; }
            set
            {
                m_Type = value;
                InvalidateHue();
            }
        }

        [Constructable]
        public CharacterStatueMaker(StatueType type)
            : base(0x32F0)
        {
            m_Type = type;

            InvalidateHue();

            Name = "a character statue maker";

            LootType = LootType.Blessed;
            Weight = 5.0;
        }

        public CharacterStatueMaker(Serial serial)
            : base(serial)
        {}

        public void InvalidateHue()
        {
            switch (m_Type)
            {
                case StatueType.Minax:
                {
                    Hue = 1645;
                    break;
                }
                case StatueType.TB:
                {
                    Hue = 2214;
                    break;
                }
                case StatueType.CoM:
                {
                    Hue = 1325;
                    break;
                }
                case StatueType.SL:
                {
                    Hue = 1109;
                    break;
                }
                case StatueType.None:
                {
                    Hue = 0;
                    break;
                }
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            string label = String.Empty;
            int labelhue = 0;
            switch (m_Type)
            {
                case StatueType.Minax:
                {
                    label = "Minax";
                    labelhue = 1645;
                    break;
                }
                case StatueType.TB:
                {
                    label = "True Britannians";
                    labelhue = 2214;
                    break;
                }
                case StatueType.CoM:
                {
                    label = "Council of Mages";
                    labelhue = 1325;
                    break;
                }
                case StatueType.SL:
                {
                    label = "Shadowlords";
                    labelhue = 1109;
                    break;
                }
            }
            if (label != String.Empty)
            {
                LabelTo(from, "[" + label + "]", labelhue);
            }
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (!from.IsBodyMod)
                {
                    from.SendLocalizedMessage(1076194); // Select a place where you would like to put your statue.
                    from.Target = new CharacterStatueTarget(this, m_Type);
                }
                else
                {
                    from.SendLocalizedMessage(1073648); // You may only proceed while in your original state...
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((int) m_Type);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadEncodedInt();

            m_Type = (StatueType) reader.ReadInt();

        }
    }

    public class MinaxStatueMaker : CharacterStatueMaker
    {
        [Constructable]
        public MinaxStatueMaker()
            : base(StatueType.Minax)
        {}

        public MinaxStatueMaker(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadEncodedInt();
        }
    }

    public class TBStatueMaker : CharacterStatueMaker
    {
        [Constructable]
        public TBStatueMaker()
            : base(StatueType.TB)
        {}

        public TBStatueMaker(Serial serial)
            : base(serial)
        {}

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
    }

    public class ComStatueMaker : CharacterStatueMaker
    {
        [Constructable]
        public ComStatueMaker()
            : base(StatueType.CoM)
        {}

        public ComStatueMaker(Serial serial)
            : base(serial)
        {}

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
    }

    public class NoHueStatueMaker : CharacterStatueMaker
    {
        [Constructable]
        public NoHueStatueMaker()
            : base(StatueType.None)
        {}

        public NoHueStatueMaker(Serial serial)
            : base(serial)
        {}

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
    }

    public class SLStatueMaker : CharacterStatueMaker
    {
        [Constructable]
        public SLStatueMaker()
            : base(StatueType.SL)
        {}

        public SLStatueMaker(Serial serial)
            : base(serial)
        {}

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
    }
}