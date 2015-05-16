using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Spells.Fourth
{
	public class GreaterHealSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Greater Heal", "In Vas Mani",
				204,
				9061,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public GreaterHealSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( m is BaseCreature && ((BaseCreature)m).IsAnimatedDead )
			{
				Caster.SendLocalizedMessage( 1061654 ); // You cannot heal that which is not alive.
			}
			else if ( m.IsDeadBondedPet )
			{
				Caster.SendLocalizedMessage( 1060177 ); // You cannot heal a creature that is already dead!
			}
            else if (m is Golem || (m is BaseCreature &&
                (((BaseCreature)m).Pseu_CanBeHealed == false || ((BaseCreature)m).ChampSpawn != null)))
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500951 ); // You cannot heal that.
			}
			else if ( m.Poisoned && !m.IsT2A|| Server.Items.MortalStrike.IsWounded( m ) )
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x22, (Caster == m) ? 1005000 : 1010398 );
			}
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				// Algorithm: (40% of magery) + (1-10)

				int toHeal = Utility.Random( 1, 10 ) + (int)(Caster.Skills[SkillName.Magery].Value * 0.4);

				int healmessage = Math.Min( m.HitsMax - m.Hits, toHeal );

				if ( healmessage > 0 )
				{
					m.PrivateOverheadMessage( MessageType.Regular, 0x42, false, healmessage.ToString(), m.NetState );
					if ( Caster != m )
						m.PrivateOverheadMessage( MessageType.Regular, 0x42, false, healmessage.ToString(), Caster.NetState );
				}

				Caster.DoBeneficial( m );
				SpellHelper.Heal( toHeal, m, Caster );

				m.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				m.PlaySound( 0x202 );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private GreaterHealSpell m_Owner;

			public InternalTarget(GreaterHealSpell owner)
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