#region References



#endregion

namespace Server.Items
{
    public class MinotaurStatuePortal : Item
    {
        [Constructable]
        public MinotaurStatuePortal() : base(0x2D89)
        {
            Name = "Statue of the Minotaur";
            Movable = true;
            Hue = 1196;
        }

        public MinotaurStatuePortal(Serial serial)
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