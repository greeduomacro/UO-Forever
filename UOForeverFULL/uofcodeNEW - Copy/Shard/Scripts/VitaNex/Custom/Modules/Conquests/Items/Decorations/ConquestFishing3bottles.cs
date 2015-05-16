using System;

namespace Server.Items
{
    public class ConquestFishing3bottles : Item
    {

        [Constructable]
        public ConquestFishing3bottles()
            : base(2465)
        {
            Name = "empty bottles";
            Movable = true;
            Stackable = false;
            LootType = LootType.Blessed;
        }

        public ConquestFishing3bottles(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
