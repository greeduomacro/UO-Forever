using System;
using Server;

namespace Server.Items
{
	public class StagAntler : Item
	{
		[Constructable]
		public StagAntler() : base( 0x1B9C )
		{
			Name = "Antler of The Arcadia Stag";
			Weight = 1.0;
			Hue = 0x221;
            Stackable = true;
		}

		public StagAntler( Serial serial ) : base( serial )
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