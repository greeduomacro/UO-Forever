using System;
using System.Collections.Generic;
using Server;
using Server.Network;

namespace Server.Items
{
    public class FleshboundTome : Spellbook
    {
        [Constructable]
        public FleshboundTome()
        {
            Name = "Flesh-Bound Tome";
            Hue = 138;
            Slayer = SlayerName.Repond;
            Slayer2 = SlayerName.Silver;
            Content = ulong.MaxValue;
            LootType = LootType.Blessed;
        }

        public FleshboundTome(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "This ancient tome is bound with the flesh of men.", 137);
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