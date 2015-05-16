using System;

namespace Server.Items
{
	[FlipableAttribute( 0x2B0C, 0x2B0D )]
	public class RoyalPlateGloves : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		
		public override int OldStrReq{ get{ return 30; } }

		//public override int OldDexBonus{ get{ return -2; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		public override string DefaultName{ get{ return "royal platemail gauntlets"; } }

		[Constructable]
		public RoyalPlateGloves() : base( 0x2B0C )
		{
			Weight = 2.0;
			Hue = 2407;
		}

		public RoyalPlateGloves( Serial serial ) : base( serial )
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