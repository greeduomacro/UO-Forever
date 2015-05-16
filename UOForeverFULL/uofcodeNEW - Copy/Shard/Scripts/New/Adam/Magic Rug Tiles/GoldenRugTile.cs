using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    public class GoldenRugTile : BaseRugTile
    {
        public override int[] TileIds { get { return new[] { 0x0ADA, 0x0ADB, 0x0ADC, 0x0ADD, 0x0ADE, 0x0ADF, 0x0AE0, 0x0AE1, 0x0AE2 }; } }

        [Constructable]
        public GoldenRugTile()
            : base(0x0ADA)
        {
        }

        public GoldenRugTile(Serial serial)
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
