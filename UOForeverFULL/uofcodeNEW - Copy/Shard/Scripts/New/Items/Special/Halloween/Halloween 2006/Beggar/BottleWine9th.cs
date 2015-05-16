using System;
using Server;

namespace Server.Items
{
	public class BottleWine9th : BeverageBottle
	{
		public override int LabelNumber{ get{ return 1076774; } } // 9th Anniversary Bottle of Wine

		[Constructable]
		public BottleWine9th() : base( BeverageType.Wine )
		{
			Quantity = 5;
		}

		public BottleWine9th( Serial serial ) : base( serial )
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