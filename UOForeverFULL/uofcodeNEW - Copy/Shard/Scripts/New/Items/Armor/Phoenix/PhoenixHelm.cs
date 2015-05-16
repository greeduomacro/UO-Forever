using System;
using Server;

namespace Server.Items
{
	public class PhoenixHelm : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 45; } }
		public override int InitMaxHits{ get{ return 60; } }

		
		public override int OldStrReq{ get{ return 40; } }

		public override int ArmorBase{ get{ return 32; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }
		public override int LabelNumber{ get{ return Identified ? 1041609 : base.LabelNumber; } } // norse helm of the phoenix

		[Constructable]
		public PhoenixHelm() : base( 0x140E )
		{
			Weight = 5.0;
			Hue = 242;
		}

		public PhoenixHelm( Serial serial ) : base( serial )
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