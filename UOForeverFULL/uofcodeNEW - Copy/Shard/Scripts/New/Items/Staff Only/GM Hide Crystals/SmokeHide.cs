using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class SmokeHide : BaseGMJewel
	{
		public override bool CastHide{ get{ return true; } }

		public override void HideEffects( Mobile from )
		{
			from.Hidden = !from.Hidden;
			Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z + 4 ), from.Map, 0x3728, 13, 1, 1149, 4 );
			Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z ), from.Map, 0x3728, 13, 1, 1149, 4  );
			Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z - 4 ), from.Map, 0x3728, 13, 1, 1149, 4  );
			Effects.SendLocationEffect( new Point3D( from.X, from.Y + 1, from.Z + 4 ), from.Map, 0x3728, 13, 1, 1149, 4  );
			Effects.SendLocationEffect( new Point3D( from.X, from.Y + 1, from.Z ), from.Map, 0x3728, 13, 1, 1149, 4  );
			Effects.SendLocationEffect( new Point3D( from.X, from.Y + 1, from.Z - 4 ), from.Map, 0x3728, 13, 1, 1149, 4  );

			Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z + 11 ), from.Map, 0x3728, 13, 1, 1149, 4  );
			Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z + 7 ), from.Map, 0x3728, 13, 1, 1149, 4  );
			Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z + 3 ), from.Map, 0x3728, 13, 1, 1149, 4  );
			Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z - 1 ), from.Map, 0x3728, 13, 1, 1149, 4  );

			from.PlaySound( 0x228 );
		}

		[Constructable]
		public SmokeHide() : base(AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 2119;
			Name = "GM Smoke Ball";
		}
		public SmokeHide( Serial serial ) : base( serial )
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