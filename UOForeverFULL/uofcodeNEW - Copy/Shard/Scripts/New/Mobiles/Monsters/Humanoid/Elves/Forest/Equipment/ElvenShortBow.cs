using System;

namespace Server.Items
{
	[FlipableAttribute( 0x2D1F, 0x2D2B )]
	public class ElvenShortBow : Bow
	{
		public override int InitMinHits{ get{ return 75; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override string DefaultName{ get{ return "an elven shortbow"; } }

		[Constructable]
		public ElvenShortBow() : base()
		{
			Weight = 8.0;
			Identified = true;
		}

		public ElvenShortBow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}