using System;
using Server;

namespace Server.Items
{
	public class NemeanTooth : Item
	{
		[Constructable]
		public NemeanTooth(): base( 0xF26 )
		{
			Name = "A Nemean Lion's Tooth";
			Weight = 0.1;
			Hue= 1153;
		}

		public NemeanTooth( Serial serial ) : base( serial )
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