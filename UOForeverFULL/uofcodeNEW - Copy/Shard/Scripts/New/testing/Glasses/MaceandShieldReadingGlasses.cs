using System;
using Server.Items;

namespace Server.Items
{
	public class MaceandShieldReadingGlasses : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 25; } }
		public override int InitMaxHits{ get{ return 45; } }

		
		public override int OldStrReq{ get{ return 10; } }

		//public override int ArmorBase{ get{ return 3; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Chainmail; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

		[Constructable]
		public MaceandShieldReadingGlasses() : base( 0x3172 )
		{
			//ItemID = 0x3172;
			Weight = 1;
			Name = "Mace and Shield Reading Glasses";
			Hue = 1358;			
		}
		public MaceandShieldReadingGlasses( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}