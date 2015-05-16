using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x4688, 0x4689 )]
	public class HalloweenCatStatue : Item
	{
		public override int LabelNumber{ get{ return ( Hue == 43 ? 0 : 1096928 ); } }
		public override string DefaultName{ get{ return ( Hue == 43 ? "a halloween cat statue" : null ); } }

		[Constructable]
		public HalloweenCatStatue() : base( 0x4688 )
		{
			Hue = (0.02 > Utility.RandomDouble()) ? 43 : 0;
		}

		public HalloweenCatStatue( Serial serial ) : base( serial )
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