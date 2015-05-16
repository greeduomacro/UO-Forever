using System;

namespace Server.Items
{
    public class ConquestSoSMap : Item
    {

        [Constructable]
        public ConquestSoSMap()
            : base(5358)
        {
            Name = "an antediluvian SOS";
            Movable = true;
            Stackable = false;
            LootType = LootType.Blessed;
            Hue = 2966;
        }

        public ConquestSoSMap(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "an antediluvian SOS", 2966);

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
