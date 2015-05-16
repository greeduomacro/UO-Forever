using System;

namespace Server.Items
{
	public class GruesomeStandard1 : Item
	{
		[Constructable]
		public GruesomeStandard1() : base( 0x41F )
		{
		}

		public GruesomeStandard1( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}