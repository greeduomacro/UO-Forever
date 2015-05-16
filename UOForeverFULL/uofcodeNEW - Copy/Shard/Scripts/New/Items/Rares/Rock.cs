using System;

namespace Server.Items
{
	public class Rock : Item
	{
		[Constructable]
		public Rock() : base( 0x9DE )
		{
				Weight = 1.0;
			Stackable = false;
			Name = "Dirty Frying Pan";
		}

		public Rock( Serial serial ) : base( serial )
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