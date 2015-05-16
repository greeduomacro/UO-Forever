using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x2B12, 0x2B13 )]
	public class RoyalPlateBoots : BaseShoes
	{
		public override CraftResource DefaultResource{ get{ return CraftResource.Iron; } }

		public override string DefaultName{ get{ return "royal platemail boots"; } }

		[Constructable]
		public RoyalPlateBoots() : this( 2407 )
		{
		}

		[Constructable]
		public RoyalPlateBoots( int hue ) : base( 0x2B12, hue )
		{
			Weight = 3.0;
		}

		public RoyalPlateBoots( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}