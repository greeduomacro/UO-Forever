#region References



#endregion

namespace Server.Items
{
    public class BahamutStatue : Item
    {
        [Constructable]
        public BahamutStatue() : base(9781)
        {
            Name = "Statue of Bahamut";
            Movable = true;
        }

        public BahamutStatue(Serial serial)
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