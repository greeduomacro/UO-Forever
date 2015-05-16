using System;
using Server;

namespace Server.Items
{
	public class ExcellentIronMaiden : Item
	{
		public override int LabelNumber{ get{ return 1036149; } } // excellent iron maiden

		[Constructable]
		public ExcellentIronMaiden() : base( 0x1249 )
		{
		}

		public ExcellentIronMaiden( Serial serial ) : base( serial )
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