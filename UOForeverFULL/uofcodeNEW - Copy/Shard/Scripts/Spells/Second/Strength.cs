using System;
using Server.Targeting;
using Server.Network;
using Server.Engines.XmlSpawner2;

namespace Server.Spells.Second
{
	public class StrengthSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Strength", "Uus Mani",
				212,
				9061,
				Reagent.MandrakeRoot,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public StrengthSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( Engines.ConPVP.DuelContext.CheckSuddenDeath( Caster ) )
			{
				Caster.SendMessage( 0x22, "You cannot cast this spell when in sudden death." );
				return false;
			}

			return base.CheckCast();
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.AddStatBonus( Caster, m, StatType.Str, true );

				m.FixedParticles( 0x375A, 10, 15, 5017, EffectLayer.Waist );
				m.PlaySound( 0x1EE );

				int percentage = (int)(SpellHelper.GetOffsetScalar( Caster, m, false )*100);
				TimeSpan length = SpellHelper.GetDuration( Caster, m );

				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Strength, 1075845, length, m, percentage.ToString() ) );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private StrengthSpell m_Owner;

			public InternalTarget(StrengthSpell owner)
				: base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.Beneficial)
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
                IEntity entity = o as IEntity; if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) && UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, m_Owner))
                {
                    return;
                }
                if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}