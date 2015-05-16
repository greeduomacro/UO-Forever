using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Scripts.VitaNex.Custom.Modules.Conquests.Items
{
    public class ConquestBandanaMining : ConquestBaseWerable
    {
        [Constructable]
        public ConquestBandanaMining()
            : base(5440, Layer.Helm)
        {
            Weight = 1.0;
            Name = "Bandana of the Master Miner";
            Hue = 1462;
        }

        public ConquestBandanaMining(Serial serial)
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
