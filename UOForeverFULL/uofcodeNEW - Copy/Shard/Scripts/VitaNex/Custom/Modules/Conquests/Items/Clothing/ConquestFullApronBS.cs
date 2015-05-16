using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Scripts.VitaNex.Custom.Modules.Conquests.Items
{
    public class ConquestFullApronBS : ConquestBaseWerable
    {
        [Constructable]
        public ConquestFullApronBS()
            : base(5437, Layer.Waist)
        {
            Weight = 1.0;
            Name = "Apron of the Master Blacksmith";
            Hue = 1462;
        }

        public ConquestFullApronBS(Serial serial)
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
