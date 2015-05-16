using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Scripts.VitaNex.Custom.Modules.Conquests.Items
{
    public class ConquestJesterTinkerer : ConquestBaseWerable
    {
        [Constructable]
        public ConquestJesterTinkerer()
            : base(8095, Layer.MiddleTorso)
        {
            Weight = 1.0;
            Name = "Tunic of the Master Tinkerer";
            Hue = 1462;
        }

        public ConquestJesterTinkerer(Serial serial)
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
