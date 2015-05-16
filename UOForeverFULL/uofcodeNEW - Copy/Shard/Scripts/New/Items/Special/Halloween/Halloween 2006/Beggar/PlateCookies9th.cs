using System;
using Server;

namespace Server.Items
{
	public class PlateCookies9th : Food
	{
		public override int LabelNumber{ get{ return 1076771; } } // 9th Anniversary Plate of Cookies

		[Constructable]
		public PlateCookies9th() : base( 0x160C )
		{
			Weight = 1.0;
			FillFactor = 6;
		}

		public PlateCookies9th( Serial serial ) : base( serial )
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