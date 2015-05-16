namespace Server.Items
{
    public class SnowHouseTile : BaseRugTile
    {
        public override int[] TileIds
        {
            get { return new[] { 0x17BD, 0x17C1, 0x17C2, 0x17C3, 0x17C4, 0x17C5, 0x17C6, 0x17C7, 0x17C8, 0x17C9, 0x17CA, 0x17CB, 0x17CC }; }
        }

        [Constructable]
        public SnowHouseTile()
            : base(0x17BD)
        { }

        public SnowHouseTile(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }


        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}