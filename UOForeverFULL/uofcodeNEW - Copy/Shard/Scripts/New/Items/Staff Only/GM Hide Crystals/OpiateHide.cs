using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class OpiateHide : BaseGMJewel
	{
		public override bool CastHide{ get{ return false; } }

		public override void HideEffects( Mobile from )
		{
			from.Hidden = !from.Hidden;
			Entity entity = new Entity( from.Serial, from.Location, from.Map );
			if ( from.Hidden )
			{
				Effects.SendLocationParticles( entity, 0x3709, 1, 30, 1150, 7, 9965, 0 );
				Effects.SendLocationParticles( entity, 0x376A, 1, 30, 1150, 3, 9502, 0 );
			}
			else
			{
				from.FixedParticles( 0x3709, 1, 30, 9965, 1150, 7, EffectLayer.Waist );
				from.FixedParticles( 0x376A, 1, 30, 9502, 1150, 3, (EffectLayer)255 );
			}

			Effects.PlaySound( entity.Location, entity.Map, 0x244 );
		}

		[Constructable]
		public OpiateHide() : base( AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 1151;
			Name = "GM Opiate Ball";
		}
		public OpiateHide( Serial serial ) : base( serial )
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