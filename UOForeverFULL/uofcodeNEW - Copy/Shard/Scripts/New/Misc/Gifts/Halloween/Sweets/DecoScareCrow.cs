using System;

namespace Server.Items
{
[Flipable( 0x469B, 0x469C)]
	public class DecoScareCrow : Item
	{
		public override string DefaultName{ get{ return "2012 ScareCrow"; }  }

		[Constructable]
		public DecoScareCrow() : base( 18075 )
		{
			Weight = 1.0;
			Hue = 0;	
		}

		public DecoScareCrow( Serial serial ) : base( serial )
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