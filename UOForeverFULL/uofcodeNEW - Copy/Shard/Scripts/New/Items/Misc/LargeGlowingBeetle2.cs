using System;
using Server;

namespace Server.Items
{
	public class LargeGlowingBeetle2 : BaseLight
	{
		public override int LitItemID{ get { return 0x2D04; } }
		public override int UnlitItemID{ get { return 0x2D03; } }

		[Constructable]
		public LargeGlowingBeetle2() : base( 0x2D03 )
		{
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = false;
			Light = LightType.Circle225;
			Weight = 1.0;
		}

		public LargeGlowingBeetle2( Serial serial ) : base( serial )
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