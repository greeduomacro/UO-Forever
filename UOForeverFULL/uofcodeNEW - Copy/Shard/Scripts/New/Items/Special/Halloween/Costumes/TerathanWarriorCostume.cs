using System;
using Server;

namespace Server.Items
{
	public class TerathanWarriorCostume : HalloweenMask
	{
		public override int BodyMod{ get{ return 70; } }
		public override int HueMod{ get{ return 0; } }
		public override string DefaultName{ get{ return "a terathan warrior costume"; } }

		[Constructable]
		public TerathanWarriorCostume() : base ()
		{
		}

		public TerathanWarriorCostume( Serial serial ) : base ( serial )
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