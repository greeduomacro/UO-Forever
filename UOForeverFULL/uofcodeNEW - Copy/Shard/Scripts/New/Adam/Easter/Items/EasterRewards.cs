using Server.Items;
using Server.Network;

namespace Server.Scripts.New.Adam.Easter.Items
{
    public class EasterBracelet : BaseBracelet
    {
        [Constructable]
        public EasterBracelet(int hue)
            : base(0x1086)
        {
            Name = "an easter bracelet";
            LootType = LootType.Blessed;
            Hue = hue;
            Weight = 0.1;
        }

        public EasterBracelet(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);
            LabelTo(m, "Saviour of Easter - 2014", Hue);
        }

        public override void OnDoubleClick(Mobile m)
        {
            base.OnDoubleClick(m);
            Effects.SendIndividualFlashEffect(m, (FlashType)2);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class EasterEggsEvent : Item
    {
        public override int LabelNumber { get { return 1016105; } } // Easter Eggs

        [Constructable]
        public EasterEggsEvent()
            : base(0x9B5)
        {
            Weight = 0.5;
            Hue = 3 + (Utility.Random(20) * 5);
        }

        public EasterEggsEvent(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);
            LabelTo(m, "Have a happy Easter!", Hue);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

    }
}
