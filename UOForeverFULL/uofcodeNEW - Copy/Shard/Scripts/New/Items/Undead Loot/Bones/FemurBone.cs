using System;

namespace Server.Items
{
	public class FemurBone : BaseBone
	{
		public override string DefaultName{ get{ return "a femur bone"; } }

		[Constructable]
		public FemurBone() : this( BoneType.Regular )
		{
		}

		[Constructable]
		public FemurBone( BoneType type ) : base( 0x1B11 + Utility.Random( 2 ), type )
		{
			Weight = 1.0;
		}

		public FemurBone( Serial serial ) : base( serial )
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