using System;
using Server;

namespace Server.Items
{
	public class Cake9th : Food
	{
		public override int LabelNumber{ get{ return 1076773; } } // 9th Anniversary Cake

		[Constructable]
		public Cake9th() : base( 0x9E9 )
		{
			Stackable = false;
			this.Weight = 1.0;
			this.FillFactor = 15;
		}

		public Cake9th( Serial serial ) : base( serial )
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