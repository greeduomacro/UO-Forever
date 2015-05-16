using System;

namespace Server.Items
{
	[FlipableAttribute( 0x13da, 0x13e1 )]
	public class RangerLegs : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 42; } }
		public override int InitMaxHits{ get{ return 58; } }

		
		public override int OldStrReq{ get{ return 35; } }

		public override int ArmorBase{ get{ return 20; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Studded; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.Half; } }

		public override int LabelNumber{ get{ return 1041496; } } // studded leggings, ranger armor

		[Constructable]
		public RangerLegs() : base( 0x13DA )
		{
			Weight = 3.0;
			Hue = 0x59C;
			Identified = true; //Otherwise hue will not show up.
		}

		public RangerLegs( Serial serial ) : base( serial )
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

			if ( Weight == 3.0 )
				Weight = 5.0;
		}
	}
}