using System;
using Server;

namespace Server.Items
{
	public class BrownDrakeCostume : HalloweenMask
	{
		public override int BodyMod{ get{ return 60; } }
		public override int HueMod{ get{ return 0; } }
		public override string DefaultName{ get{ return "a drake costume"; } }

		[Constructable]
		public BrownDrakeCostume() : base ()
		{
		}

		public BrownDrakeCostume( Serial serial ) : base ( serial )
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