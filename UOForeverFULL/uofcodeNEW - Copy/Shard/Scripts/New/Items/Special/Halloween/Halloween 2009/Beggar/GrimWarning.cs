using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x4687, 0x42BD )]
	public class GrimWarning : Item
	{
		public override int LabelNumber{ get{ return 1095957; } } // grim warning

		[Constructable]
		public GrimWarning() : base( 0x4687 )
		{
		}

		public GrimWarning( Serial serial ) : base( serial )
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