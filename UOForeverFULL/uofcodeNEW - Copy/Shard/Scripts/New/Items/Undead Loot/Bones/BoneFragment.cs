using System;

namespace Server.Items
{
	public class BoneFragement : BaseBone
	{
		public override string DefaultName{ get{ return "bone fragment"; } }

		[Constructable]
		public BoneFragement() : this( BoneType.Regular )
		{
		}

		[Constructable]
		public BoneFragement( BoneType type ) : base( 0x1B19 + Utility.Random( 2 ), type )
		{
			Weight = 1.0;
		}

		public BoneFragement( Serial serial ) : base( serial )
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