using System;
using Server;

namespace Server.Items
{
	public class BagOfReagents : Bag
	{
		[Constructable]
		public BagOfReagents() : this( 50 )
		{
		}

		[Constructable]
		public BagOfReagents( int amount )
		{
			DropItem( new BlackPearl   ( amount ) );
			DropItem( new Bloodmoss    ( amount ) );
			DropItem( new Garlic       ( amount ) );
			DropItem( new Ginseng      ( amount ) );
			DropItem( new MandrakeRoot ( amount ) );
			DropItem( new Nightshade   ( amount ) );
			DropItem( new SulfurousAsh ( amount ) );
			DropItem( new SpidersSilk  ( amount ) );
		}

        [Constructable]
        public BagOfReagents(int amount, int weight)
        {
            DropItem(new BlackPearl(amount){Weight = weight});
            DropItem(new Bloodmoss(amount) { Weight = weight });
            DropItem(new Garlic(amount) { Weight = weight });
            DropItem(new Ginseng(amount) { Weight = weight });
            DropItem(new MandrakeRoot(amount) { Weight = weight });
            DropItem(new Nightshade(amount) { Weight = weight });
            DropItem(new SulfurousAsh(amount) { Weight = weight });
            DropItem(new SpidersSilk(amount) { Weight = weight });
        }

		public BagOfReagents( Serial serial ) : base( serial )
		{
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