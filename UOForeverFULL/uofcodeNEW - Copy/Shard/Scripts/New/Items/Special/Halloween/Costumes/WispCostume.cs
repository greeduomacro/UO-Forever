using System;
using Server;

namespace Server.Items
{
	public class WispCostume : HalloweenMask
	{
		public override int BodyMod{ get{ return 58; } }
		public override int HueMod{ get{ return 0; } }
		public override string DefaultName{ get{ return "a wisp costume"; } }

		[Constructable]
		public WispCostume() : base ()
		{
		}

		public WispCostume( Serial serial ) : base ( serial )
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