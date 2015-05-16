using System;

namespace Server.Items
{
	public class ThreeLiquorBottles : Item
	{
		[Constructable]
		public ThreeLiquorBottles() : base( 0x99D )
		{
		}

		public ThreeLiquorBottles( Serial serial ) : base( serial )
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