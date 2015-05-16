using System;

namespace Server.Items
{
    public class ConquestTMap : Item
    {

        [Constructable]
        public ConquestTMap()
            : base(5356)
        {
            Name = "an antediluvian treasure map";
            Movable = true;
            Stackable = false;
            LootType = LootType.Blessed;
            Hue = 2966;
        }

        public ConquestTMap(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "an antediluvian treasure map", 2966);

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
