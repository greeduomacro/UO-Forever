using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Scripts.VitaNex.Custom.Modules.Conquests.Items
{
    public class ConquestRobeTailoring : ConquestBaseWerable
    {
        [Constructable]
        public ConquestRobeTailoring()
            : base(7939, Layer.OuterTorso)
        {
            Weight = 1.0;
            Name = "Robe of the Master Tailor";
            Hue = 1462;
        }

        public ConquestRobeTailoring(Serial serial)
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
