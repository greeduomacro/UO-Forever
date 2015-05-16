#region References



#endregion

namespace Server.Items
{
    public class HydraStatue : Item
    {
        [Constructable]
        public HydraStatue()
            : base(0x2D8B)
        {
            Name = "Statue of the Hydra";
            Movable = true;
            Hue = 0x47e;
        }

        public HydraStatue(Serial serial)
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