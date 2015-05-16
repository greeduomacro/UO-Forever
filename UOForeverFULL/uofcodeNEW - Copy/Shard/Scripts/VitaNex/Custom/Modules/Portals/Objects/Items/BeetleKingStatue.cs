#region References



#endregion

namespace Server.Items
{
    public class BeetleKingStatue : Item
    {
        [Constructable]
        public BeetleKingStatue() : base(0x276F)
        {
            Name = "Statue of the Beetle King";
            Movable = true;
        }

        public BeetleKingStatue(Serial serial)
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