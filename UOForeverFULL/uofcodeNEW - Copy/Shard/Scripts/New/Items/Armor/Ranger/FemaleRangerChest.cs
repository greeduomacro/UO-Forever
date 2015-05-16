using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1c02, 0x1c03 )]
	public class FemaleRangerChest : BaseArmor
	{
		public override int InitMinHits{ get{ return 35; } }
		public override int InitMaxHits{ get{ return 45; } }

		public override int OldStrReq{ get{ return 35; } }

		public override int ArmorBase{ get{ return 20; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Studded; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.Half; } }

		public override bool AllowMaleWearer{ get{ return false; } }

		public override string DefaultName{ get{ return "female studded ranger armor"; } }

		[Constructable]
		public FemaleRangerChest() : base( 0x1C02 )
		{
			Weight = 6.0;
			Hue = 0x59C;
			Identified = true; //Otherwise hue will not show up.
		}

		public FemaleRangerChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}