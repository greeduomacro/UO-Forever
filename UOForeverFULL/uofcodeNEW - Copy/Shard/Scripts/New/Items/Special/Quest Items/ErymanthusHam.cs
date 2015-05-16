using System;
using Server;

namespace Server.Items
{
	public class ErymanthusHam : Item
	{
		[Constructable]
		public ErymanthusHam() : base( 0x9C9 )
		{
			Name = "Ham From The Mt. Erymanthus Boar";
			Weight = 1.0;
			Hue = 0x234;
		}
		
		public ErymanthusHam( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}