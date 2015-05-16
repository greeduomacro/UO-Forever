using System;
using Server;

namespace Server.Items
{
	public class BagOfPaganReagents : Bag
	{
		[Constructable]
		public BagOfPaganReagents() : this( 50 )
		{
		}

		[Constructable]
		public BagOfPaganReagents( int amount )
		{
			DropItem( new BatWing      ( amount ) );
			DropItem( new GraveDust    ( amount ) );
			DropItem( new DaemonBlood  ( amount ) );
			DropItem( new NoxCrystal   ( amount ) );
			DropItem( new PigIron      ( amount ) );
			DropItem( new EyeofNewt	   ( amount ) );
			DropItem( new Pumice	   ( amount ) );
			DropItem( new Bloodspawn   ( amount ) );
			DropItem( new ExecutionersCap( amount ) );
			DropItem( new Brimstone	   ( amount ) );
			DropItem( new DeadWood	   ( amount ) );
			DropItem( new WormsHeart   ( amount ) );
		}

		public BagOfPaganReagents( Serial serial ) : base( serial )
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