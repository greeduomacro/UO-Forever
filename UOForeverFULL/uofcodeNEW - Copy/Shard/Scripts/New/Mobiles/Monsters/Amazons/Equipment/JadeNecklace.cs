using System;
using Server;

namespace Server.Items
{
	public class JadeNecklace : GoldNecklace
	{
		public override string DefaultName{ get{ return "a jade necklace"; } }
		public override int LabelNumber{ get{ return 0; } }

		[Constructable]
		public JadeNecklace() : base()
		{
			Hue = 2128;
		}

		public JadeNecklace( Serial serial ) : base( serial )
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