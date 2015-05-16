using System;

namespace Server.Items
{
	[FlipableAttribute( 0x2B69, 0x3160 )]
	public class DragonGorget : BaseArmor
	{
		public override int InitMinHits{ get{ return 55; } }
		public override int InitMaxHits{ get{ return 75; } }

		public override int OldStrReq{ get{ return 30; } }

		//public override int OldDexBonus{ get{ return -1; } }

		public override int ArmorBase{ get{ return 40; } }

		public override string DefaultName{ get{ return "dragon scale gorget"; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Dragon; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RedScales; } }

		[Constructable]
		public DragonGorget() : base( 0x2B69 )
		{
			Weight = 2.0;
		}

		public DragonGorget( Serial serial ) : base( serial )
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