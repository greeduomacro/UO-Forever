using System;
using Server;

namespace Server.Items
{
	public class StableDung : Item
	{
		[Constructable]
		public StableDung() : base(Utility.RandomMinMax( 0xF3C, 0xF3B ))
		{
			Name = "Dung";
			Weight = 1.0;
			Stackable= true;
		}
		
		public StableDung( Serial serial ) : base( serial )
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