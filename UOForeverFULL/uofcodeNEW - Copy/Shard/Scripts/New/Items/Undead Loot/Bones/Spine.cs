using System;

namespace Server.Items
{
	public class Spine : BaseBone
	{
		public override string DefaultName{ get{ return "a spine"; } }

		[Constructable]
		public Spine() : this( BoneType.Regular )
		{
		}

		[Constructable]
		public Spine( BoneType type ) : base( 0x1B1B + Utility.Random( 2 ), type )
		{
			Weight = 2.5;
		}

		public Spine( Serial serial ) : base( serial )
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