namespace Server.Items
{
    public class RikktorsHead : Item
    {
        [Constructable]
        public RikktorsHead()
            : base(11700)
        {
            Name = "rikktor's head";
        }

        public RikktorsHead(Serial serial)
            : base(serial)
        {}

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);
            LabelTo(m,
                "[Champion Artifact]",
                134);
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
}