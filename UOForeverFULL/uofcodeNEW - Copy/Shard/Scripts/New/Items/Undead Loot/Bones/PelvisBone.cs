using System;
namespace Server.Items
{
	public class PelvisBone : BaseBone
	{
		public override string DefaultName{ get{ return "a pelvis bone"; } }

		[Constructable]
		public PelvisBone() : this( BoneType.Regular )
		{
		}

		[Constructable]
		public PelvisBone( BoneType type ) : base( 0x1B15 + Utility.Random( 2 ), type )
		{
			Weight = 2.0;
		}

		public PelvisBone( Serial serial ) : base( serial )
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