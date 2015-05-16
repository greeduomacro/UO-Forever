using System;
using Server;

namespace Server.Items
{
	public class Pizza9th : Food
	{
		public override int LabelNumber{ get{ return 1076772; } } // 9th Anniversary Pizza

		[Constructable]
		public Pizza9th() : base( 0x1040 )
		{
			Stackable = false;
			Weight = 1.0;
			FillFactor = 10;
		}

		public Pizza9th( Serial serial ) : base( serial )
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