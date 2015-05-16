using System;
using System.Collections;
using System.Collections.Generic;
using Server.Targeting;
using Server.Network;
using Server.Engines.XmlSpawner2;

namespace Server.Spells.Second
{
	public class ProtectionSpell : MagerySpell
	{
		private static Dictionary<Mobile, double> m_Registry = new Dictionary<Mobile, double>();
		public static Dictionary<Mobile, double> Registry { get { return m_Registry; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Protection", "Uus Sanct",
				236,
				9011,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public ProtectionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( m_Registry.ContainsKey( Caster ) )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				return false;
			}
			else if ( !Caster.CanBeginAction( typeof( DefensiveSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if ( m_Registry.ContainsKey( Caster ) )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
			}
			else if ( !Caster.CanBeginAction( typeof( DefensiveSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
			}
			else if ( CheckSequence() )
			{
				if ( Caster.BeginAction( typeof( DefensiveSpell ) ) )
				{
					double value = (int)(Math.Min( Caster.Skills[SkillName.EvalInt].Value, 100.0 ) + Caster.Skills[SkillName.Meditation].Value);
				    if (!Caster.IsT2A)
				        value += Caster.Skills[SkillName.Inscribe].Value;
					value /= 4;

					if ( value < 0 )
						value = 0;
					else if ( value > 75 )
						value = 75.0;

					m_Registry.Add( Caster, value );
					new InternalTimer( Caster ).Start();

					Caster.FixedParticles( 0x375A, 9, 20, 5016, EffectLayer.Waist );
					Caster.PlaySound( 0x1ED );
				}
				else
				{
					Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
				}
			}

			FinishSequence();
			
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Caster;

			public InternalTimer( Mobile caster ) : base( TimeSpan.FromSeconds( 0 ) )
			{
				double val = caster.Skills[SkillName.Magery].Value * 2.0;
				if ( val < 15 )
					val = 15;
				else if ( val > 240 )
					val = 240;

				m_Caster = caster;
				Delay = TimeSpan.FromSeconds( val );
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				ProtectionSpell.Registry.Remove( m_Caster );
				DefensiveSpell.Nullify( m_Caster );
			}
		}
	}
}