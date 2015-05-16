using System;
using Server;

namespace Server.Items
{
	public class AmazonJadeNecklace : JadeNecklace
	{
		public override string DefaultName{ get{ return "an exceptionally crafted amazon jade necklace"; } }
		[Constructable]
		public AmazonJadeNecklace() : base()
		{
			Hue = 1437;
		}

		public AmazonJadeNecklace( Serial serial ) : base( serial )
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