namespace Server.Items
{
    public class EasterEggsCorrupted: Item
    {
        [Constructable]
        public EasterEggsCorrupted()
            : base(0x9B5)
        {
            Name = "corrupted easter eggs";
            Weight = 2;
            Hue = 1164;
            Stackable = true;
        }

        public EasterEggsCorrupted(Serial serial)
            : base(serial)
        {}

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);
            LabelTo(m, "These eggs smell putrid and a vile energy emanates from them.", 1162);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class EasterEggsPurified : Item
    {
        [Constructable]
        public EasterEggsPurified()
            : base(0x9B5)
        {
            Name = "purified easter eggs";
            Weight = 2;
            Hue = 2498;
            Stackable = true;
        }

        public EasterEggsPurified(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);
            LabelTo(m, "These eggs are warm to the touch and you feel at ease as you hold them in your hands.", 2498);
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