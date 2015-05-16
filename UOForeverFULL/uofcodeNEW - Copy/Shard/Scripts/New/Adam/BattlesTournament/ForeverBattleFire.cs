using System;
using System.Collections.Generic;
using Server.Mobiles;
using VitaNex.Items;
using VitaNex.Network;

namespace Server.Items
{
    public class BattlesFire : Item
    {
        [Constructable]
        public BattlesFire()
            : base(6571)
        {
            Name = "Forever Battles Fire";
            Movable = false;

            Light = LightType.Circle150;
            Weight = 1.0;
        }

        public BattlesFire(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}