using System;

namespace Server.Items
{
[Flipable( 0x4697, 0x4698)]
	public class DecoPumpkin2 : Item
	{
		public override string DefaultName{ get{ return "Decorative Pumpkin 2012"; }  }

		[Constructable]
		public DecoPumpkin2() : base( 18071 )
		{
			Weight = 1.0;
			Hue = 0;	
		}

		public DecoPumpkin2( Serial serial ) : base( serial )
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