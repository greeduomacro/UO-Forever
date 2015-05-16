using System;
using Server;

namespace Server.Items
{
	public class SilverSerpentBlade : Kryss
	{
		public override int LabelNumber{ get{ return 1078163; } } // Silver Serpent Blade

		[Constructable]
		public SilverSerpentBlade()
		{
		}

		public SilverSerpentBlade( Serial serial ) : base( serial )
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
