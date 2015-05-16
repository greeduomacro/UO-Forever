using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BagOfArrows : Bag
	{
		[Constructable]
		public BagOfArrows() : this( 100 )
		{
		Name = "Bag of arrows";
		}

		[Constructable]
		public BagOfArrows( int amount )
		{
			DropItem( new Bolt    ( amount ) );
			DropItem( new Arrow    ( amount ) );
		}

		public BagOfArrows( Serial serial ) : base( serial )
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