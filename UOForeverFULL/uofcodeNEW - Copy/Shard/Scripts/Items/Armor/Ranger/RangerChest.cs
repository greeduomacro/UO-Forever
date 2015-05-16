using System;

namespace Server.Items
{
	[FlipableAttribute( 0x13db, 0x13e2 )]
	public class RangerChest : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 42; } }
		public override int InitMaxHits{ get{ return 58; } }

		
		public override int OldStrReq{ get{ return 35; } }

		public override int ArmorBase{ get{ return 20; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Studded; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.Half; } }

		public override int LabelNumber{ get{ return 1041497; } } // studded tunic, ranger armor

		[Constructable]
		public RangerChest() : base( 0x13DB )
		{
			Weight = 8.0;
			Hue = 0x59C;
			Identified = true; //Otherwise hue will not show up.
		}

		public RangerChest( Serial serial ) : base( serial )
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