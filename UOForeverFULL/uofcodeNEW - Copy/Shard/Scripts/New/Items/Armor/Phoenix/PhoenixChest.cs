using System;

namespace Server.Items
{
	[FlipableAttribute( 0x13ec, 0x13ed )]
	public class PhoenixChest : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 40; } }
		public override int InitMaxHits{ get{ return 50; } }

		
		public override int OldStrReq{ get{ return 20; } }

		//public override int OldDexBonus{ get{ return -2; } }

		public override int ArmorBase{ get{ return 24; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Ringmail; } }
		public override int LabelNumber{ get{ return Identified ? 1041606 : base.LabelNumber; } } // ringmail tunic of the phoenix

		[Constructable]
		public PhoenixChest() : base( 0x13EC )
		{
			Weight = 15.0;
			Hue = 242;
		}

		public PhoenixChest( Serial serial ) : base( serial )
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