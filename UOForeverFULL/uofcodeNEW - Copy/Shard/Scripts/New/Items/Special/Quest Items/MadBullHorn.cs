using System;
using Server;

namespace Server.Items
{
	public class MadBullHorn : Item
	{
		[Constructable]
		public MadBullHorn() : base( 0x1084 )
		{
			Name = "Mad Bull's Horn";
			Weight = 1.0;
			Hue = 0x481;
		}

		public MadBullHorn( Serial serial ) : base( serial )
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