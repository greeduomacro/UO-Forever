using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	//[FlipableAttribute( 0xD0B, 0xDBE )]
	public class Reeds : Item
	{

		[Constructable]
		public Reeds() : base( 0xD05 )
		{
			Weight = 1.0;
			Stackable = false;
			Name = "Reeds";
		}

		public Reeds( Serial serial ) : base( serial )
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

