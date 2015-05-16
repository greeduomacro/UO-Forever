using System;
using Server;

namespace Server.Items
{
	public class RoyalShield : BaseShield
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		

		public override int ArmorBase{ get{ return 23; } }

		public override string DefaultName{ get{ return "royal shield"; } }

		[Constructable]
		public RoyalShield() : base( 0x2B01 )
		{
			Weight = 8.0;
			Hue = 2407;
		}

		public RoyalShield( Serial serial ) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}