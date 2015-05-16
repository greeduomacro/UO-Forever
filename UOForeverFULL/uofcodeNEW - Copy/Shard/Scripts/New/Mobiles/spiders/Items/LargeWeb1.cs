using System;

namespace Server.Items 
{
[Flipable( 0x10CF, 0x10CB )]
	public class Largeweb1 : Item
	{
		public override string DefaultName{ get{ return "a Large Spider Web"; }  }

		[Constructable]
		public Largeweb1() : base( 4303 )
		{
			Weight = 1.0;
			Hue = 0;	
		}

		public Largeweb1( Serial serial ) : base( serial )
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