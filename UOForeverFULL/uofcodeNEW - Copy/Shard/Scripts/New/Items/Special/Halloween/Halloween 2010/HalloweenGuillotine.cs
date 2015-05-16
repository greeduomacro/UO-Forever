using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x1230, 0x125E )]
	public class HalloweenGuillotine : Item
	{
		public override int LabelNumber{ get{ return 1036167; } } // halloween guillotine

		[Constructable]
		public HalloweenGuillotine() : base( 0x1230 )
		{
		}

		public HalloweenGuillotine( Serial serial ) : base( serial )
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