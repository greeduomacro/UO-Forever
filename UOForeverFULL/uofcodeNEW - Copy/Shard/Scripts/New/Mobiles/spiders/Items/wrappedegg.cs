using System;

namespace Server.Items 
{
	public class WrappedEgg : Item
	{
		public override string DefaultName{ get{ return "A child of the Queen"; }  }

		[Constructable]
		public WrappedEgg() : base( 4313 )
		{
			Weight = 1.0;
			Hue = 0;	
		}

		public WrappedEgg( Serial serial ) : base( serial )
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