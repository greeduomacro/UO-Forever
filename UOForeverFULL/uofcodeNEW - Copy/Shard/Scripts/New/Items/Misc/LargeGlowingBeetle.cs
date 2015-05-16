using System;
using Server;

namespace Server.Items
{
	public class LargeGlowingBeetle : BaseLight
	{
		public override int LitItemID{ get { return 0x2D02; } }
		public override int UnlitItemID{ get { return 0x2D01; } }

		[Constructable]
		public LargeGlowingBeetle() : base( 0x2D01 )
		{
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = false;
			Light = LightType.Circle225;
			Weight = 1.0;
		}

		public LargeGlowingBeetle( Serial serial ) : base( serial )
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