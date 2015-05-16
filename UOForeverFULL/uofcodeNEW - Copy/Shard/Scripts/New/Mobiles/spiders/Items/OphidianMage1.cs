using System;

namespace Server.Items 
{
	public class ophidianMage1 : Item
	{
		public override string DefaultName{ get{ return "a Ophidian Mage Statue"; }  }

		[Constructable]
		public ophidianMage1() : base( 9643 )
		{
			Weight = 1.0;
			Hue = 0;	
		}

		public ophidianMage1( Serial serial ) : base( serial )
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