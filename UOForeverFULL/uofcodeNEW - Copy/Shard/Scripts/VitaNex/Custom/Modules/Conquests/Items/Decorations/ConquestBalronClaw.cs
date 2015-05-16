using System;

namespace Server.Items
{
    public class ConquestBalronClaw : Item
    {

        [Constructable]
        public ConquestBalronClaw()
            : base(22305)
        {
            Name = "a balron claw";
            Movable = true;
            Stackable = false;
            LootType = LootType.Blessed;
            Hue = 1921;
        }

        public ConquestBalronClaw(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "a balron claw", 1100);

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
