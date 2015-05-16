using System;
using Server;

namespace Server.Items
{
	public class AncientFruitBowl : Item
	{
		public override string DefaultName{ get{ return "a fruit bowl"; } }

		[Constructable]
		public AncientFruitBowl() : base( 0x2D4F )
		{
		}

		public AncientFruitBowl( Serial serial ) : base( serial )
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