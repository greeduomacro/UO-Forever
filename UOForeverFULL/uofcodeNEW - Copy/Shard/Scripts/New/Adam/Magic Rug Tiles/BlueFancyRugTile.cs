using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    public class BlueFancyRugTile : BaseRugTile
    {
        public override int[] TileIds { get { return new[] { 0x0AD1, 0x0AD2, 0x0AD3, 0x0AD4, 0x0AD5, 0x0AD6, 0x0AD7, 0x0AD8, 0x0AD9}; } }

        [Constructable]
        public BlueFancyRugTile()
            : base(0x0AD1)
        {
        }

        public BlueFancyRugTile(Serial serial)
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
