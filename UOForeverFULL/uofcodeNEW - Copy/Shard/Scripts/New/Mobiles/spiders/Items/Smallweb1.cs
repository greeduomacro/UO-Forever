using System;

namespace Server.Items 
{
[Flipable( 0x10D6, 0x10D7 )]
	public class Smallweb1 : Item
	{
		public override string DefaultName{ get{ return "a small Spider Web"; }  }

		[Constructable]
		public Smallweb1() : base( 4310 )
		{
			Weight = 1.0;
			Hue = 0;	
		}

		public Smallweb1( Serial serial ) : base( serial )
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