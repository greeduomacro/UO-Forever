using System;
using Server;

namespace Server.Items
{
	public class BloodCrystal : Item
	{
		[Constructable]
		public BloodCrystal() : base( 0xF8E )
		{
			Weight = 11.0;
			Name = "Blood Crystal";
			Hue = 38;
		}

		public BloodCrystal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}