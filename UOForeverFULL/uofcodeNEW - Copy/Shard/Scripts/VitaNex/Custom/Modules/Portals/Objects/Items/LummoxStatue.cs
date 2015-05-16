#region References



#endregion

namespace Server.Items
{
    public class LummoxStatue : Item
    {
        [Constructable]
        public LummoxStatue() : base(0x281B)
        {
            Name = "Lummox Statue";
            Movable = true;
        }

        public LummoxStatue(Serial serial)
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