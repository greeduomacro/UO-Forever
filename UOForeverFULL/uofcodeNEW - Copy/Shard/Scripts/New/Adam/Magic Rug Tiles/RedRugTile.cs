using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    public class RedRugTile : BaseRugTile
    {
        public override int[] TileIds { get { return new[] { 0x0AC6, 0x0AC7, 0x0AC8, 0x0AC9, 0x0ACA, 0x0ACB, 0x0ACC, 0x0ACD, 0x0ACE, 0x0ACF, 0x0AD0 }; } }

        [Constructable]
        public RedRugTile()
            : base(0x0AC6)
        {
        }

        public RedRugTile(Serial serial)
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
