using System;
using Server;

namespace Server.Items
{
	public class RockCandy : BaseCandy
	{
		public override int LabelNumber{ get{ return 1076762; } } // Rock Candy

		[Constructable]
		public RockCandy() : this( 1 )
		{
		}

		[Constructable]
		public RockCandy( int amount ) : base( amount, 0x105E )
		{
			Hue = 3 + (Utility.Random( 20 ) * 5);
		}

		public RockCandy( Serial serial ) : base( serial )
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