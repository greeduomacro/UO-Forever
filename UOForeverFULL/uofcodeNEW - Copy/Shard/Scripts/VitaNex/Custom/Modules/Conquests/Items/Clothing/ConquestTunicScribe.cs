using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Scripts.VitaNex.Custom.Modules.Conquests.Items
{
    public class ConquestTunicScribe : ConquestBaseWerable
    {
        [Constructable]
        public ConquestTunicScribe()
            : base(8097, Layer.MiddleTorso)
        {
            Weight = 1.0;
            Name = "Tunic of the Master Scribe";
            Hue = 1462;
        }

        public ConquestTunicScribe(Serial serial)
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
