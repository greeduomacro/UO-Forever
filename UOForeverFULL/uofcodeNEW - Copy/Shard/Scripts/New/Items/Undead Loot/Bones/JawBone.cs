using System;

namespace Server.Items
{
	public class JawBone : BaseBone
	{
		public override string DefaultName{ get{ return "a jaw bone"; } }

		[Constructable]
		public JawBone() : this( BoneType.Regular )
		{
		}

		[Constructable]
		public JawBone( BoneType type ) : base( 0x1B13 + Utility.Random( 2 ), type )
		{
			Weight = 1.0;
		}

		public JawBone( Serial serial ) : base( serial )
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