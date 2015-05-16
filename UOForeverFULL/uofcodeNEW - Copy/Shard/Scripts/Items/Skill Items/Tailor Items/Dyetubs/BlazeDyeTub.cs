using System;
using Server;

namespace Server.Items
{
	public class BlazeDyeTub : DyeTub
	{
		[Constructable]
		public BlazeDyeTub() : base( 0x489, false, 5 )
		{
		}

		public BlazeDyeTub( Serial serial ) : base( serial )
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
		}
	}
}