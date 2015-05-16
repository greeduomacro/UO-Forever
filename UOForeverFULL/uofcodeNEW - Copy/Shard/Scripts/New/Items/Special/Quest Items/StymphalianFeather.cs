using System;
using Server;

namespace Server.Items
{
	public class StymphalianFeather : Item
	{
		[Constructable]
		public StymphalianFeather() : base( 0x1BD1 )
		{
			Name = "Stymphalian Marsh Bird Feather";
			Hue = 0x674;
			Stackable = true;
			Weight = 0.1;
		}
		
		public StymphalianFeather( Serial serial ) : base( serial )
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