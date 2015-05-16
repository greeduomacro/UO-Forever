using System;
using Server;

namespace Server.Items
{
	public class SkullsPike : Item
	{
		public override int LabelNumber{ get{ return 1095949; } } // skulls on pike

		[Constructable]
		public SkullsPike() : base( 0x42B5 )
		{
		}

		public SkullsPike( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt((int) 0); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}