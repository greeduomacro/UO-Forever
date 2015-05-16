using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Scripts.VitaNex.Custom.Modules.Conquests.Items
{
    public class ConquestKiltCarpenter : ConquestBaseWerable
    {
        [Constructable]
        public ConquestKiltCarpenter()
            : base(5431, Layer.OuterLegs)
        {
            Weight = 1.0;
            Name = "Kilt of the Master Carpenter";
            Hue = 1462;
        }

        public ConquestKiltCarpenter(Serial serial)
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
