using System;
using Server;

namespace Server.Items
{
	public class RedLicorice : BaseCandy
	{
		public override int LabelNumber{ get{ return 1076763; } } // Licorice

		[Constructable]
		public RedLicorice() : this( 1 )
		{
		}

		[Constructable]
		public RedLicorice( int amount ) : base( amount, 0xF8A )
		{
			Hue = 0xE4;
		}

		public RedLicorice( Serial serial ) : base( serial )
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