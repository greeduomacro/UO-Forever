using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Lord British's corpse")]
    public class LordBritish : Mobile 
    {
        [Constructable]
        public LordBritish()
        {
            Name = "Lord British";
            Body = 0x190;
            Hue = 0x83F3;
            Female = false;
            Blessed = true;

            AddItem(new LordBritishSuit());
            AddItem(new VikingSword());
            AddItem(new OrderShield());
        }

        public LordBritish(Serial serial)
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