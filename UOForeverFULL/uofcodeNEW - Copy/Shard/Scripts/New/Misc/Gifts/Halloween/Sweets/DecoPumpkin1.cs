using System;

namespace Server.Items
{
[Flipable( 0x4693, 0x4694)]
	public class DecoPumpkin1 : Item
	{
		public override string DefaultName{ get{ return "Decorative Pumpkin 2012"; }  }

		[Constructable]
		public DecoPumpkin1() : base( 18067 )
		{
			Weight = 1.0;
			Hue = 0;	
		}

		public DecoPumpkin1( Serial serial ) : base( serial )
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