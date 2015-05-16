using System;
using Server;

namespace Server.Items
{
	public class GazerCostume : HalloweenMask
	{
		public override int BodyMod{ get{ return 22; } }
		public override int HueMod{ get{ return 0; } }
		public override string DefaultName{ get{ return "a gazer costume"; } }

		[Constructable]
		public GazerCostume() : base ()
		{
		}

		public GazerCostume( Serial serial ) : base ( serial )
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