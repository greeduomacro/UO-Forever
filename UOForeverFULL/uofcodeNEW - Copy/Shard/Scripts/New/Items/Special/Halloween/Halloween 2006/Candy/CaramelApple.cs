using System;
using Server;

namespace Server.Items
{
	public class CaramelApple : BaseCandy
	{
		public override int LabelNumber{ get{ return 1076761; } } // Candy Apple

		[Constructable]
		public CaramelApple() : this( 1 )
		{
		}

		[Constructable]
		public CaramelApple( int amount ) : base( amount, 0x9D0 )
		{
			Hue = 0x420;
		}

		public CaramelApple( Serial serial ) : base( serial )
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