using System;
using Server;

namespace Server.Items
{
	public class LernaHydraScale : Item
	{
		[Constructable]
		public LernaHydraScale() : base( 0x26b4 )
		{
			Name = "Lerna Hydra Scale";
			Weight = 1.0;
			Hue = 0x4f2;
		}
		
		public LernaHydraScale( Serial serial ) : base( serial )
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