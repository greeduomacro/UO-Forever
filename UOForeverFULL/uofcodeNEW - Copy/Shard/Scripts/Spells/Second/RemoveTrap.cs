using System;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Engines.XmlSpawner2;

namespace Server.Spells.Second
{
	public class RemoveTrapSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Remove Trap", "An Jux",
				212,
				9001,
				Reagent.Bloodmoss,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public RemoveTrapSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendMessage( "What do you wish to untrap?" );
		}

		public void Target( TrapableContainer item )
		{
			if ( !Caster.CanSee( item ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( item.TrapType != TrapType.None && item.TrapType != TrapType.MagicTrap )
			{
				base.DoFizzle();
			}
			else if ( CheckSequence() )
			{
				SpellHelper.Turn( Caster, item );

				Point3D loc = item.GetWorldLocation();

				Effects.SendLocationParticles( EffectItem.Create( loc, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5015 );
				Effects.PlaySound( loc, item.Map, 0x1F0 );

				TreasureMapChest chest = item as TreasureMapChest;

				double skill = Caster.Skills[SkillName.Magery].Value;

				if ( ( chest != null && skill >= 100.0 && chest.TrapLevel <= 2 ) || ( chest == null && ( ( item.TrapCreator == null && item.TrapLevel <= (int)(skill/33.3) ) || ( item.TrapCreator != null && item.TrapLevel <= (int)(skill/20.0) ) ) ) )
				{
					//Treasure Chests: Must be GM, up to level 2
					//Non Treasure Chests:  Up to level 3, Player constructed, up to level 5.
					item.TrapType = TrapType.None;
					item.TrapPower = 0;
					item.TrapLevel = 0;
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private RemoveTrapSpell m_Owner;

			public InternalTarget(RemoveTrapSpell owner)
				: base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.None)
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
                IEntity entity = o as IEntity; if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) && UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, m_Owner))
                {
                    return;
                }
                if ( o is TrapableContainer )
				{
					m_Owner.Target( (TrapableContainer)o );
				}
				else
				{
					from.SendMessage( "You can't disarm that" );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}