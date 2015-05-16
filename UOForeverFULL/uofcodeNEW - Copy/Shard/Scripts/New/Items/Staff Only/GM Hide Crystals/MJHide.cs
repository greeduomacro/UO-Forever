using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class MJHide : BaseGMJewel
	{
		public Timer m_Timer;

		public override bool CastHide{ get{ return false; } }

		[Constructable]
		public MJHide() : base(AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 2119;
			Name = "GM Michael Jackson Hide Ball";
		}
		public MJHide( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			base.OnDoubleClick( from );
			if (Deleted)
				return;
			else if ( from.Hidden )
			{
				from.Hidden = false;
				from.Animate( 1, 5, 1, true, false, 0 );
				//from.Emote( "*" + from.Name + " appears while doing the moonwalk!*" );
				from.FixedParticles( 0x3709, 1, 30, 9965, 5, 7, EffectLayer.Waist );
				from.FixedParticles( 0x376A, 1, 30, 9502, 5, 3, EffectLayer.Waist );

				from.PlaySound( 0x244 );

			}
			else
			{
				m_Timer = new Countdown(from, this);
				m_Timer.Start();
			}
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

		public class Countdown: Timer
		{
			private int m_Ticker;
			private Mobile m_Mobile;
			private MJHide m_Item;

			public Countdown( Mobile mobile, MJHide item ): base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = mobile;
				m_Item = item;
				Priority = TimerPriority.TwoFiftyMS;
				m_Ticker = 2;
			}

			private void Emoo( Mobile from )
			{
				from.BoltEffect( 0 );
				from.Animate( 22, 5, 1, true, false, 0 );
				//from.Emote( "*" + from.Name + " was banned to hell for doing the moonwalk...*" );
			}

			protected override void OnTick()
			{
				switch (m_Ticker)
				{
					case 2: Emoo( m_Mobile ); break;
					case 1: m_Mobile.Hidden = true; break;
					case 0: Stop(); m_Item.m_Timer = null; break;
				}
				m_Ticker--;
			}
		}
	}
}