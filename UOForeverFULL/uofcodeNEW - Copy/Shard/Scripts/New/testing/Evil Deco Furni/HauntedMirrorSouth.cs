


using System;
using Server;

namespace Server.Items
{

	public class HauntedMirrorSouth : BaseLight
	{
		public override int LitItemID{ get { return 0x2A7C;  } }
		public override int UnlitItemID{ get { return 0x2A7B; } }

		[Constructable]
		public HauntedMirrorSouth() : base( 0x2A7B )
		{
			Name = "Haunted Mirror";
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = true;
			Light = LightType.Circle225;
			Weight = 3.0;
		}

		public HauntedMirrorSouth( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}