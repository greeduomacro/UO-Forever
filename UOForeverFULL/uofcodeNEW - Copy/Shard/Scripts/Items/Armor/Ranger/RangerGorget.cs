using System;

namespace Server.Items
{
	public class RangerGorget : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 42; } }
		public override int InitMaxHits{ get{ return 58; } }

		
		public override int OldStrReq{ get{ return 25; } }

		public override int ArmorBase{ get{ return 20; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Studded; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.Half; } }

		public override int LabelNumber{ get{ return 1041495; } } // studded gorget, ranger armor

		[Constructable]
		public RangerGorget() : base( 0x13D6 )
		{
			Weight = 1.0;
			Hue = 0x59C;
			Identified = true; //Otherwise hue will not show up.
		}

		public RangerGorget( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}