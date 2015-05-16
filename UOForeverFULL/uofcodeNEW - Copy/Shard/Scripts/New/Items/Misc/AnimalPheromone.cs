using System;
using Server;

namespace Server.Items
{
	public class AnimalPheromone : Item
	{
		public override int LabelNumber{ get{ return 1071200; } }

		[Constructable]
		public AnimalPheromone() : base( 0xF8E )
		{
			Weight = 1.0;
		}

		public AnimalPheromone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}