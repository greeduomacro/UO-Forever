using System;

namespace Server.Items
{
	public class Skull : BaseBone
	{
		public override string DefaultName{ get{ return "a skull"; } }

		[Constructable]
		public Skull() : this( BoneType.Regular )
		{
		}

		[Constructable]
		public Skull( BoneType type ) : base( 0x1AE0 + Utility.Random( 5 ), type )
		{
			Weight = 1.0;
		}

		public Skull( Serial serial ) : base( serial )
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