#region References



#endregion

namespace Server.Items
{
    public class CocoonWebbingArtifact : Item
    {
        [Constructable]
        public CocoonWebbingArtifact()
            : base(4317)
        {
            Name = "cocoon webbing";
            Weight = 2;
            Movable = true;
            Hue = 1461;
        }

        public CocoonWebbingArtifact(Serial serial)
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