#region References



#endregion

namespace Server.Items
{
    public class DarkFatherStatue : Item
    {
        [Constructable]
        public DarkFatherStatue() : base(9778)
        {
            Name = "Statue of the Dark Father";
            Movable = true;
        }

        public DarkFatherStatue(Serial serial)
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