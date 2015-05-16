#region References



#endregion

namespace Server.Items
{
    public class SphinxStatue : Item
    {
        [Constructable]
        public SphinxStatue() : base(9752)
        {
            Name = "Statue of the Sphinx";
            Movable = true;
            Hue = 1196;
        }

        public SphinxStatue(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}