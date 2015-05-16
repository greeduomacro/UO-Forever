using System;
using Server;

namespace Server.Items
{
	public class HarvestWine : BeverageBottle
	{
		public override string DefaultName{ get{ return "harvest wine"; } }

		[Constructable]
		public HarvestWine() : base( BeverageType.Wine )
		{
			Quantity = 5;
			Hue = 1170;
		}

		public HarvestWine( Serial serial ) : base( serial )
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