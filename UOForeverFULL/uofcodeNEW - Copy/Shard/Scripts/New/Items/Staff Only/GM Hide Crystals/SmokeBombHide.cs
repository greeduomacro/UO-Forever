using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class SmokeBombHide : BaseGMJewel
	{
		public override bool CastHide{ get{ return false; } }

		public override void HideEffects( Mobile from )
		{
			from.Hidden = !from.Hidden;

			Entity entity = new Entity( from.Serial, from.Location, from.Map );

			if ( from.Hidden )
				Effects.SendLocationParticles( entity, 0x3709, 1, 30, 1108, 6, 9904, 0 );
			else
				from.FixedParticles( 0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot );

			Effects.PlaySound( entity.Location, entity.Map, 0x22F );
		}

		[Constructable]
		public SmokeBombHide() : base( AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 1175;
			Name = "GM Smoke Bomb Ball";
		}
		public SmokeBombHide( Serial serial ) : base( serial )
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