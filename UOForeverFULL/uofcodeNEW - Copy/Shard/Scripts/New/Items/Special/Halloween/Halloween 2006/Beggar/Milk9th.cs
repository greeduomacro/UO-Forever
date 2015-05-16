using System;
using Server;

namespace Server.Items
{
	public class Milk9th : Pitcher
	{
		public override int LabelNumber{ get{ return 1076775; } } // 9th Anniversary Milk
		public override bool Fillable{ get{ return false; } }

		[Constructable]
		public Milk9th() : base( BeverageType.Milk )
		{
			Quantity = 5;
		}

		public Milk9th( Serial serial ) : base( serial )
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