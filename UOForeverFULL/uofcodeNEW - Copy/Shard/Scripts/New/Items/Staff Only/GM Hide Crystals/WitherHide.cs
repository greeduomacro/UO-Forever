using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class WitherHide : BaseGMJewel
	{
		public override bool CastHide{ get{ return false; } }

		public override void HideEffects(Mobile from)
		{
			from.Hidden = !from.Hidden;
			if (from.Hidden)
			{
				Effects.SendLocationParticles( (IEntity)from, 0x37CC, 1, 40, 3, 9917, 97, 0 );
				Effects.SendLocationParticles( (IEntity)from, 0x374A, 1, 15, 97, 3, 9502, 0 );
			}
			else
			{
				from.FixedParticles( 0x37CC, 1, 40, 97, 3, 9917, EffectLayer.Waist );
				from.FixedParticles( 0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255 );
			}
			from.PlaySound( 0x10B );
		}

		[Constructable]
		public WitherHide() : base(AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 1152;
			Name = "GM Wither Ball";
		}
		public WitherHide( Serial serial ) : base( serial )
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