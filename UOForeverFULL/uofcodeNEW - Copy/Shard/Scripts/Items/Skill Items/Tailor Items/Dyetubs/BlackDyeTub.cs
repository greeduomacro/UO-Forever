using System;

namespace Server.Items
{
	public class BlackDyeTub : DyeTub
	{
		[Constructable]
		public BlackDyeTub() : base( 0x1, false, -1 )
		{
		}

		public BlackDyeTub( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

		    UsesRemaining = -1;
		}
	}
}