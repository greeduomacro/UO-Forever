using System;
using Server;

namespace Server.Items
{
	public class CacheChestmap : MapItem
	{
        public CacheChest CacheChest { get; set; }

		[Constructable]
		public CacheChestmap()
		{
		    Name = " a map to a cache chest";
			SetDisplay( 5121, 2305, 6141, 4093, 700, 700 );
		    Protected = true;
		}

        public CacheChestmap(Serial serial)
            : base(serial)
		{
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

            writer.Write(CacheChest);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

		    CacheChest = reader.ReadItem<CacheChest>();
		}

        public override void SetDisplay(int x1, int y1, int x2, int y2, int w, int h)
        {
            Width = w;
            Height = h;

            Bounds = new Rectangle2D(5121 ,2305, 1020, 1788);
        }

        public override void DisplayTo(Mobile from)
        {
            ClearPins();

            if (CacheChest != null && !(CacheChest.RootParentEntity is Mobile))
            {
                AddWorldPin(from.X, from.Y);
                AddWorldPin(CacheChest.X, CacheChest.Y);
            }
            else if (CacheChest != null && CacheChest.RootParentEntity is Mobile)
            {
                AddWorldPin(from.X, from.Y);
                AddWorldPin(CacheChest.RootParentEntity.X, CacheChest.RootParentEntity.Y);
            }

            base.DisplayTo(from);
        }

        public override bool ValidateEdit(Mobile from)
        {
            return false;
        }
	}
}