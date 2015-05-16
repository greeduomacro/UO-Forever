using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x1db9, 0x1dba )]
	public class RangerCap : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 35; } }
		public override int InitMaxHits{ get{ return 45; } }

		
		public override int OldStrReq{ get{ return 25; } }

		public override int ArmorBase{ get{ return 20; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Studded; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.Half; } }

		public override string DefaultName{ get{ return "studded cap, ranger armor"; } }

		[Constructable]
		public RangerCap() : base( 0x1DB9 )
		{
			Weight = 2.0;
			Hue = 0x59C;
			Identified = true; //Otherwise hue will not show up.
		}

		public RangerCap( Serial serial ) : base( serial )
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