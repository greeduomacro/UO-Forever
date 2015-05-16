using System;

namespace Server.Items 
{
	public class WrappedBody : Item
	{
		public override string DefaultName{ get{ return "Cocoon"; }  }

		[Constructable]
		public WrappedBody() : base( 4314 )
		{
			Weight = 1.0;
			Hue = 0;	
		}

		public WrappedBody( Serial serial ) : base( serial )
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