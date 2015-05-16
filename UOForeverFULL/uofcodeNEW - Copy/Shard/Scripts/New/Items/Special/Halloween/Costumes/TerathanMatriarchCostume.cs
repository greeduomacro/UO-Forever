using System;
using Server;

namespace Server.Items
{
	public class TerathanMatriarchCostume : HalloweenMask
	{
		public override int BodyMod{ get{ return 72; } }
		public override int HueMod{ get{ return 0; } }
		public override int Rarity{ get{ return 3; } }
		public override string DefaultName{ get{ return "a terathan matriarch costume"; } }

		[Constructable]
		public TerathanMatriarchCostume() : base ()
		{
		}

		public TerathanMatriarchCostume( Serial serial ) : base ( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}