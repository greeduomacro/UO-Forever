using System;
using Server;

namespace Server.Items
{
	public class CrystalBrazier : BaseLight
	{
		public override int LitItemID{ get { return 0x35EF; } }

		[Constructable]
		public CrystalBrazier() : base( 0x35EF )
		{
			Movable = false;
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = true;
			Light = LightType.Circle300;
			Weight = 25.0;
		}

		public CrystalBrazier( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}