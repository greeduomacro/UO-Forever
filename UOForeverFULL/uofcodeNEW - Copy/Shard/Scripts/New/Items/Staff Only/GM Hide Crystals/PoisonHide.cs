using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class PoisonHide : BaseGMJewel
	{
		public override bool CastHide{ get{ return false; } }

		public override void HideEffects(Mobile from)
		{
			from.Hidden = !from.Hidden;
			if (from.Hidden)
			{
				Effects.SendLocationParticles( (IEntity)from, 0x36CB, 1, 9, 67, 5, 9911, 0 );
				Effects.SendLocationParticles( (IEntity)from, 0x374A, 1, 17, 1108, 4, 9502, 0 );
			}
			else
			{
				from.FixedParticles( 0x36CB, 1, 9, 9911, 67, 5, EffectLayer.Waist );
				from.FixedParticles( 0x374A, 1, 17, 9502, 1108, 4, (EffectLayer)255 );
			}
			from.PlaySound( 0x22F );
		}

		[Constructable]
		public PoisonHide() : base(AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 1268;
			Name = "GM Poison Ball";
		}
		public PoisonHide( Serial serial ) : base( serial )
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