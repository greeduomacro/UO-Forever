using System;
using Server;

namespace Server.Items
{
	public class BagOfRecallReagents : Pouch
	{
		[Constructable]
		public BagOfRecallReagents() : this( 50 )
		{
		}

		[Constructable]
		public BagOfRecallReagents( int amount )
		{
			DropItem( new BlackPearl   ( amount ) );
			DropItem( new Bloodmoss    ( amount ) );
			DropItem( new MandrakeRoot ( amount ) );
		}

		public BagOfRecallReagents( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}