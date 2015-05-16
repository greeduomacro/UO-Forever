using System;
using Server;

namespace Server.Items
{
	public abstract class BaseMarkCoin : Item
	{

		public override double DefaultWeight{ get { return 0.01; } }

	 [Constructable]
		public BaseMarkCoin() : this( 1 )
		{
		}

		 [Constructable]
		public BaseMarkCoin( int amount ) : base( 0xEED )
		{
			Stackable = true;
			Movable = true;
            LootType = LootType.Blessed;
			Amount = amount;
			Hue = 1153;
		}

		public BaseMarkCoin( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}