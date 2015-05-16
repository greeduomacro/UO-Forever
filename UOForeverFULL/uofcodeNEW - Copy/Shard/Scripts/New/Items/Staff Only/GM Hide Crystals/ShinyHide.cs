using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class ShinyHide : BaseGMJewel
	{
		public override bool CastHide{ get{ return false; } }

		public override void HideEffects(Mobile from)
		{
			from.Hidden = !from.Hidden;
			if (from.Hidden)
			{
				Effects.SendLocationParticles( (IEntity)from, 0x375A, 1, 30, 33, 2, 9966, 0 );
				Effects.SendLocationParticles( (IEntity)from, 0x37B9, 1, 30, 43, 3, 9502, 0 );
			}
			else
			{
				from.FixedParticles( 0x375A, 1, 30, 9966, 33, 2, EffectLayer.Waist );
				from.FixedParticles( 0x37B9, 1, 30, 9502, 43, 3, (EffectLayer)255 );
			}

			from.PlaySound( 0x0F5 );
			from.PlaySound( 0x1ED );
		}

		[Constructable]
		public ShinyHide() : base(AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 1150;
			Name = "GM Shiny Ball";
		}
		public ShinyHide( Serial serial ) : base( serial )
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