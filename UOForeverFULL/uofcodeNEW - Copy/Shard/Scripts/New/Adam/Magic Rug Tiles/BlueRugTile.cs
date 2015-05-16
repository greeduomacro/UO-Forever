using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    public class BlueRugTile : BaseRugTile
    {
        public override int[] TileIds { get { return new[] { 0x0ABD, 0x0ABE, 0x0AC0, 0x0AC1, 0x0AC2, 0x0AC3, 0x0AC4, 0x0AC5, 0x0AF6, 0x0AF7, 0x0AF8, 0x0AF9 }; } }

        [Constructable]
        public BlueRugTile()
            : base(0x0ABD)
        {
        }

        public BlueRugTile(Serial serial)
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
