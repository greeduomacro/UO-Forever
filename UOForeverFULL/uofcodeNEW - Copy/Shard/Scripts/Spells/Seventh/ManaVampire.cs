using System;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Engines.XmlSpawner2;

namespace Server.Spells.Seventh
{
	public class ManaVampireSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Mana Vampire", "Ort Sanct",
				221,
				9032,
				Reagent.BlackPearl,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public ManaVampireSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
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
			else if ( CheckHSequence( m ) )
			{
				Caster.DoHarmful( m );
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster, target = m;

				SpellHelper.CheckReflect( (int)this.Circle, ref source, ref target );

				if ( m.Spell != null )
					m.Spell.OnCasterHurt();

				target.Paralyzed = false;

				int toDrain = 0;

				if ( CheckResisted( target ) )
				{
					target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
					//toDrain = m.Mana / 2;
					toDrain = 0;
				}
				else
					toDrain = (int)(m.Mana * Math.Min( Math.Max( ((GetDamageSkill( Caster ) - GetResistSkill( m )) / 100.0), 0.25 ), 0.75 ) );
					//toDrain = (int)(m.Mana * (Utility.Random( 25, 50 ) / 100.0));
				

				//if ( toDrain > (Caster.ManaMax - Caster.Mana) )
				//	toDrain = Caster.ManaMax - Caster.Mana;

				target.Mana -= toDrain;

				if ( source == Caster )
					source.Mana += Math.Min( toDrain, source.ManaMax - source.Mana );
				
				target.FixedParticles( 0x374A, 10, 15, 5054, EffectLayer.Head );
				target.PlaySound( 0x1F9 );

				HarmfulSpell( m );
			}

			FinishSequence();
		}

		public override double GetResistPercent( Mobile target )
		{
			return SpellDamageController._ManaVampireResistChance;
		}

		private class InternalTarget : Target
		{
			private ManaVampireSpell m_Owner;

			public InternalTarget(ManaVampireSpell owner)
				: base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.Harmful)
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