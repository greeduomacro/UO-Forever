using System;
using Server;

namespace Server.Items
{
	public class TwilightJacket : LeatherNinjaJacket
	{
		public override int LabelNumber{ get{ return 1078183; } } // Twilight Jacket

		[Constructable]
		public TwilightJacket()
		{
		}

		public TwilightJacket( Serial serial ) : base( serial )
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
