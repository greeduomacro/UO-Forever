using System;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Misc;

namespace Server.Spells.Fifth
{
	public interface ILinkDispel
	{
		Item Link{ get; set; }
	}

	public class DispelFieldSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
			"Dispel Field", "An Grav",
			206,
			9002,
			Reagent.BlackPearl,
			Reagent.SpidersSilk,
			Reagent.SulfurousAsh,
			Reagent.Garlic
		);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public DispelFieldSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Item item )
		{
			Type t = item.GetType();

			if ( !Caster.CanSee( item ) )
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			else if ( !t.IsDefined( typeof( DispellableFieldAttribute ), false ) )
				Caster.SendLocalizedMessage( 1005049 ); // That cannot be dispelled.
			else if ( item is Moongate && !((Moongate)item).Dispellable )
				Caster.SendLocalizedMessage( 1005047 ); // That magic is too chaotic
			else if ( CheckSequence() )
			{
				SpellHelper.Turn( Caster, item );

				if ( item is ILinkDispel )
				{
					Item second = ((ILinkDispel)item).Link;
					Effects.SendLocationParticles( EffectItem.Create( second.Location, second.Map, EffectItem.DefaultDuration ), 0x376A, 9, 20, 5042 );
					Effects.PlaySound( second.GetWorldLocation(), second.Map, 0x201 );

					second.Delete();
				}

				Effects.SendLocationParticles( EffectItem.Create( item.Location, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 20, 5042 );
				Effects.PlaySound( item.GetWorldLocation(), item.Map, 0x201 );

				item.Delete();
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private DispelFieldSpell m_Owner;

			public InternalTarget(DispelFieldSpell owner)
				: base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.None)
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Item )
				{
					m_Owner.Target( (Item)o );
				}
				else
				{
                    if (o != null && o == m_Owner.Caster)
                    {
                        //remove RA or Magic Reflect
                        if (m_Owner.Caster.MagicDamageAbsorb > 0)
                        {
                            m_Owner.Caster.MagicDamageAbsorb = 0;
                            m_Owner.Caster.SendMessage("You let your magic reflection dissipate.");
                            DefensiveSpell.Nullify(m_Owner.Caster);
                        }
                        else if (m_Owner.Caster.MeleeDamageAbsorb > 0)
                        {
                            m_Owner.Caster.MeleeDamageAbsorb = 0;
                            m_Owner.Caster.SendMessage("You let your reactive armor dissipate.");
                            DefensiveSpell.Nullify(m_Owner.Caster);
                        }
                        else
                        {
                            m_Owner.Caster.SendLocalizedMessage(1005049); // That cannot be dispelled.
                        }
                    }
                    else
                    {
                        m_Owner.Caster.SendLocalizedMessage(1005049); // That cannot be dispelled.
                    }
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}