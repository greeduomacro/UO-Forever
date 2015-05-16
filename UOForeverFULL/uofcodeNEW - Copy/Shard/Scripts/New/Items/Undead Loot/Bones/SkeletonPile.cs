using System;

namespace Server.Items
{
	public class SkeletonPile : BaseBone
	{
		public override string DefaultName{ get{ return "a skeleton"; } }

		[Constructable]
		public SkeletonPile() : this( BoneType.Regular )
		{
		}

		[Constructable]
		public SkeletonPile( BoneType type ) : base( 0xECA + Utility.Random( 9 ), type )
		{
			Weight = 10.0;
		}

		public SkeletonPile( Serial serial ) : base( serial )
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