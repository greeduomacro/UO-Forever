using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class PoisonCrystal : Item
	{
		[Constructable]
		public PoisonCrystal() : base( 0xF8E )
		{
			Weight = 11.0;
			Name = "Poison Crystal";
			Hue = 64;
		}

		public PoisonCrystal( Serial serial ) : base( serial )
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