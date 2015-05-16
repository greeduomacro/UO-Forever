using System;
using Server;

namespace Server.Items
{
	public class MummyCostume : HalloweenMask
	{
		public override int BodyMod{ get{ return 154; } }
		public override int HueMod{ get{ return 0; } }
		public override string DefaultName{ get{ return "a mummy costume"; } }

		[Constructable]
		public MummyCostume() : base ()
		{
		}

		public MummyCostume( Serial serial ) : base ( serial )
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