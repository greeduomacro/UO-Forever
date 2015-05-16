using System;

namespace Server.Items
{
	[FlipableAttribute( 0x2B0A, 0x2B0B )]
	public class RoyalPlateArms : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		
		public override int OldStrReq{ get{ return 40; } }

		//public override int OldDexBonus{ get{ return -2; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		public override string DefaultName{ get{ return "royal platemail arms"; } }

		[Constructable]
		public RoyalPlateArms() : base( 0x2B0A )
		{
			Weight = 5.0;
			Hue = 2407;
		}

		public RoyalPlateArms( Serial serial ) : base( serial )
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