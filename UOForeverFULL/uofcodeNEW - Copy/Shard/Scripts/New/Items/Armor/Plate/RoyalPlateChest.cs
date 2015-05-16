using System;

namespace Server.Items
{
	[FlipableAttribute( 0x2B08, 0x2B09 )]
	public class RoyalPlateChest : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		
		public override int OldStrReq{ get{ return 60; } }

		//public override int OldDexBonus{ get{ return -8; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		public override string DefaultName{ get{ return "royal platemail breastplate"; } }

		[Constructable]
		public RoyalPlateChest() : base( 0x2B08 )
		{
			Weight = 10.0;
			Hue = 2407;
		}

		public RoyalPlateChest( Serial serial ) : base( serial )
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