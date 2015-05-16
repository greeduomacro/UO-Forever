using System;
using Server;

namespace Server.Items
{
	public class BlackLicorice : BaseCandy
	{
		public override int LabelNumber{ get{ return 1076763; } } // Licorice

		[Constructable]
		public BlackLicorice() : this( 1 )
		{
		}

		[Constructable]
		public BlackLicorice( int amount ) : base( amount, 0xF8A )
		{
			Hue = 1109;
		}

		public BlackLicorice( Serial serial ) : base( serial )
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