using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x2B06, 0x2B07 )]
	public class RoyalPlateLegs : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		

		public override int OldStrReq{ get{ return 60; } }
		//public override int OldDexBonus{ get{ return -6; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		public override string DefaultName{ get{ return "royal platemail leggings"; } }

		[Constructable]
		public RoyalPlateLegs() : base( 0x2B06 )
		{
			Weight = 7.0;
			Hue = 2407;
		}

		public RoyalPlateLegs( Serial serial ) : base( serial )
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