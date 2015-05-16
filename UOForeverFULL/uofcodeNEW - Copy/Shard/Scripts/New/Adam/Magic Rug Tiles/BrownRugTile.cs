using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    public class BrownRugTile : BaseRugTile
    {
        public override int[] TileIds { get { return new[] { 0x0AE3, 0x0AE4, 0x0AE5, 0x0AE6, 0x0AE7, 0x0AE8, 0x0AE9, 0x0AEA, 0x0AEB }; } }

        [Constructable]
        public BrownRugTile()
            : base(0x0AE3)
        {
        }

        public BrownRugTile(Serial serial)
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
