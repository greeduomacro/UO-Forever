using System;

namespace Server.Items
{
	[Flipable( 0x3158 , 0x3159 )]
	public class MountedDreadHorn : Item
	{
		[Constructable]
		public MountedDreadHorn() : this( 0 )
		{
		}

		[Constructable]
		public MountedDreadHorn( int hue ) : base( 0x3158 )
		{
			Weight = 1.0;
			Name = "a mounted dread horn";
		}

		public MountedDreadHorn( Serial serial ) : base( serial )
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