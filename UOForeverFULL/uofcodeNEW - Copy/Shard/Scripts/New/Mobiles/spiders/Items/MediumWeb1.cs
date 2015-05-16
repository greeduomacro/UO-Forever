using System;

namespace Server.Items 
{
[Flipable( 0x10D2, 0x10D3 )]
	public class Mediumweb1 : Item
	{
		public override string DefaultName{ get{ return "a Medium Spider Web"; }  }

		[Constructable]
		public Mediumweb1() : base( 4306 )
		{
			Weight = 1.0;
			Hue = 0;	
		}

		public Mediumweb1( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}