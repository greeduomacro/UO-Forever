using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    public class BaseRugTile : Item
	{     
        public virtual int[] TileIds { get {return new [] {0x0ABD}; } }

		[Constructable]
		public BaseRugTile() : base( 0x0ABD )
		{
		    Name = "magic rug tile";
		}

        [Constructable]
        public BaseRugTile(int itemId)
            : base(itemId)
        {
        }

        public BaseRugTile(Serial serial)
            : base(serial)
		{
		}
       
        public override void OnDoubleClick(Mobile from)
        {            
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("The magic rug tile must be in your backpack to transform.");
                return;
            }

            var index = Array.IndexOf(TileIds, ItemID);

            index += 1;

            if (index >= TileIds.Length)
            {
                index = 0;
            }

            ItemID = TileIds[index];          

            base.OnDoubleClick(from);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

       

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
