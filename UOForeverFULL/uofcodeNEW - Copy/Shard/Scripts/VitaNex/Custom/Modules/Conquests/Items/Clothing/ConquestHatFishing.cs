using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Scripts.VitaNex.Custom.Modules.Conquests.Items
{
    public class ConquestHatFishing : ConquestBaseWerable
    {
        [Constructable]
        public ConquestHatFishing()
            : base(5907, Layer.Helm)
        {
            Weight = 1.0;
            Name = "Hat of the Master Fisherman";
            Hue = 1462;
        }

        public ConquestHatFishing(Serial serial)
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
