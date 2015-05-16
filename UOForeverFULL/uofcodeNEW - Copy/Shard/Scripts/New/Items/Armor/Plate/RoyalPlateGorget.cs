using System;

namespace Server.Items
{
	[FlipableAttribute( 0x2B0E, 0x2B0F )]
	public class RoyalPlateGorget : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		
		public override int OldStrReq{ get{ return 30; } }

		//public override int OldDexBonus{ get{ return -1; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		public override string DefaultName{ get{ return "royal platemail gorget"; } }

		[Constructable]
		public RoyalPlateGorget() : base( 0x2B0E )
		{
			Weight = 2.0;
			Hue = 2407;
		}

		public RoyalPlateGorget( Serial serial ) : base( serial )
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