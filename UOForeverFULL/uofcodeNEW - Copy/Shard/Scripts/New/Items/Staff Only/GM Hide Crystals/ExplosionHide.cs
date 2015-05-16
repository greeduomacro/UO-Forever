using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class ExplosionHide : BaseGMJewel
	{
		public override bool CastHide{ get{ return false; } }

		public override void HideEffects(Mobile from)
		{
			from.Hidden = !from.Hidden;
			if (from.Hidden)
				Effects.SendLocationParticles( (IEntity)from, 0x36BD, 20, 10, 5044 );
			else
				from.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Waist );
			from.PlaySound( 0x307 );
		}

		[Constructable]
		public ExplosionHide() : base(AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 2113;
			Name = "GM Explosion Ball";
		}
		public ExplosionHide( Serial serial ) : base( serial )
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