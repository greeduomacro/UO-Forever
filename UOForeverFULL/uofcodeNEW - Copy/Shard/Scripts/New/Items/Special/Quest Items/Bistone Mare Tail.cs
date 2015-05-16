using System;
using Server;

namespace Server.Items
{
	public class MareTail : Item
	{
		[Constructable]
		public MareTail() : base( 0x166F )
		{
           	Name = "A Mares Tail";
			Weight = 1.0;
			Hue = 1646;
		}
		
		public MareTail( Serial serial ) : base( serial )
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