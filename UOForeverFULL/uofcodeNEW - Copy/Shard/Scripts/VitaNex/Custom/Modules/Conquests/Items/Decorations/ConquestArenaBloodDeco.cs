using System;

namespace Server.Items
{
    public class ConquestArenaBloodDeco : Item
    {

        [Constructable]
        public ConquestArenaBloodDeco()
            : base(4655)
        {
            Name = "dried blood";
            Movable = true;
            Stackable = false;
            LootType = LootType.Blessed;
        }

        public ConquestArenaBloodDeco(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "dried blood", 1100);

            if (LootType == LootType.Blessed)
            {
                LabelTo(from, 1153, 1041362);              
            }
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
