using System;
using Server;

namespace Server.Items
{
	public class EvilWispCostume : HalloweenMask
	{
		public override int BodyMod{ get{ return 165; } }
		public override int HueMod{ get{ return 0; } }
		public override int Rarity{ get{ return 3; } }
		public override string DefaultName{ get{ return "an evil wisp costume"; } }

		[Constructable]
		public EvilWispCostume() : base ()
		{
		}

		public EvilWispCostume( Serial serial ) : base ( serial )
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