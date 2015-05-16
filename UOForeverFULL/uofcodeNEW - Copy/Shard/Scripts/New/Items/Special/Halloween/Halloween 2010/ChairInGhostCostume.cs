using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0xC17, 0xC18 )]
	public class ChairInGhostCostume : Item
	{
		public override int LabelNumber{ get{ return 1036166; } } // chair in a ghost costume

		[Constructable]
		public ChairInGhostCostume() : base( 0xC17 )
		{
		}

		public ChairInGhostCostume( Serial serial ) : base( serial )
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