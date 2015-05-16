using System;

namespace Server.Items
{
    public class ConquestFishing250kDeco : Item
    {

        [Constructable]
        public ConquestFishing250kDeco()
            : base(17639)
        {
            Name = "a trophy fish";
            Movable = true;
            Stackable = false;
            LootType = LootType.Blessed;
        }

        public ConquestFishing250kDeco(Serial serial)
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
