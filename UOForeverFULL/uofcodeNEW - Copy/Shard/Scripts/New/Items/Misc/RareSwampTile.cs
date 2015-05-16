using System;
using Server;

namespace Server.Items
{
	public class RareSwampTile : Item
	{
		[Constructable]
		public RareSwampTile() : this( 0x3213 )
		{
		}

		[Constructable]
		public RareSwampTile( int itemID ) : base( itemID )
		{
			Movable = true;
			Weight = 11;
			Name = "a rare swamp tile";
		}

		public RareSwampTile( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}


	}
}