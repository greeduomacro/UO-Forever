using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Scripts.VitaNex.Custom.Modules.Conquests.Items
{
    public class ConquestBootsLJ : ConquestBaseWerable
    {
        [Constructable]
        public ConquestBootsLJ()
            : base(5905, Layer.OuterLegs)
        {
            Weight = 1.0;
            Name = "Boots of the Master Lumberjack";
            Hue = 1462;
        }

        public ConquestBootsLJ(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
