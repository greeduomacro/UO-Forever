using System;

namespace Server.Items
{
	[FlipableAttribute( 0x13d5, 0x13dd )]
	public class RangerGloves : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 42; } }
		public override int InitMaxHits{ get{ return 58; } }

		
		public override int OldStrReq{ get{ return 25; } }

		public override int ArmorBase{ get{ return 20; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Studded; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.Half; } }

		public override int LabelNumber{ get{ return 1041494; } } // studded gloves, ranger armor

		[Constructable]
		public RangerGloves() : base( 0x13D5 )
		{
			Weight = 1.0;
			Hue = 0x59C;
			Identified = true; //Otherwise hue will not show up.
		}

		public RangerGloves( Serial serial ) : base( serial )
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