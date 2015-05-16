using System;

namespace Server.Items
{
	public class BoneContainer : BaseContainer
	{
		[Constructable]
		public BoneContainer() : base( 0xECA + Utility.Random( 9 ) )
		{
			Weight = 8.0;
		}

		public BoneContainer( Serial serial ) : base( serial )
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