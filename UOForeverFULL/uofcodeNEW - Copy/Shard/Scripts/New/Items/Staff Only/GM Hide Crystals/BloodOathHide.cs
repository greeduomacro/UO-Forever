using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class BloodOathHide : BaseGMJewel
	{
		public override bool CastHide{ get{ return false; } }

		public override void HideEffects(Mobile from)
		{
			from.Hidden = !from.Hidden;
			if (from.Hidden)
			{
				Effects.SendLocationParticles( (IEntity)from, 0x375A, 1, 17, 33, 7, 9919, 0 );
				Effects.SendLocationParticles( (IEntity)from, 0x3728, 1, 13, 33, 7, 9502, 0 );
			}
			else
			{
				from.FixedParticles( 0x375A, 1, 17, 9919, 33, 7, EffectLayer.Waist );
				from.FixedParticles( 0x3728, 1, 13, 9502, 33, 7, (EffectLayer)255 );
			}
			from.PlaySound( 0x175 );
		}

		[Constructable]
		public BloodOathHide() : base(AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 2117;
			Name = "GM BloodOath Ball";
		}
		public BloodOathHide( Serial serial ) : base( serial )
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