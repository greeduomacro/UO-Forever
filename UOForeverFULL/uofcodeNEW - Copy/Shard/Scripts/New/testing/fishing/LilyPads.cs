using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xD0B, 0xDBE )]
	public class LilyPads : Item
	{

		[Constructable]
		public LilyPads() : base( 0xD0B )
		{
			Weight = 1.0;
			Stackable = false;
			Name = "Lily Pads";
		}

		public LilyPads( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}

