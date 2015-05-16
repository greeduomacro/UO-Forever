#region References
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Targeting;
#endregion

namespace Server.Spells.Second
{
	public class MagicTrapSpell : MagerySpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Magic Trap", "In Jux", 212, 9001, Reagent.Garlic, Reagent.SpidersSilk, Reagent.SulfurousAsh);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public MagicTrapSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{ }

		public override void OnCast()
		{
			Caster.Target = new InternalTarget(this);
		}

		public void Target(TrapableContainer item)
		{
			if (!Caster.CanSee(item))
			{
				Caster.SendLocalizedMessage(500237); // Target can not be seen.
			}
			else if (item.TrapType != TrapType.None && item.TrapType != TrapType.MagicTrap)
			{
				base.DoFizzle();
			}
			else if (CheckSequence())
			{
				SpellHelper.Turn(Caster, item);

                var pouch = item as ChargeableTrapPouch;

                if (pouch != null)
			    {
                    if (pouch.Charges == 0)
			        {
                        item.TrapType = TrapType.MagicTrap;
                        item.TrapPower = Caster.EraAOS ? Utility.RandomMinMax(10, 50) : 4; //Change to depend on magery skill?
                        item.TrapLevel = 0;
                        pouch.Charges++;
			        }
			        else if (pouch.Charges < 30)
			        {
			            pouch.Charges++;
			        }
			        else
			        {
			            Caster.SendMessage(54, "This pouch can only hold 30 charges.");
			        }
			    }
			    else
			    {
                    item.TrapType = TrapType.MagicTrap;
                    item.TrapPower = Caster.EraAOS ? Utility.RandomMinMax(10, 50) : 4; //Change to depend on magery skill?
                    item.TrapLevel = 0;			        
			    }

				Point3D loc = item.GetWorldLocation();

				Effects.SendLocationParticles(
					EffectItem.Create(new Point3D(loc.X + 1, loc.Y, loc.Z), item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 9502);
				Effects.SendLocationParticles(
					EffectItem.Create(new Point3D(loc.X, loc.Y - 1, loc.Z), item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 9502);
				Effects.SendLocationParticles(
					EffectItem.Create(new Point3D(loc.X - 1, loc.Y, loc.Z), item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 9502);
				Effects.SendLocationParticles(
					EffectItem.Create(new Point3D(loc.X, loc.Y + 1, loc.Z), item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 9502);
				Effects.SendLocationParticles(
					EffectItem.Create(new Point3D(loc.X, loc.Y, loc.Z), item.Map, EffectItem.DefaultDuration), 0, 0, 0, 5014);

				Effects.PlaySound(loc, item.Map, 0x1EF);
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private readonly MagicTrapSpell m_Owner;

			public InternalTarget(MagicTrapSpell owner)
				: base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.None)
			{
				m_Owner = owner;
			}

			protected override void OnTarget(Mobile from, object o)
			{
				var entity = o as IEntity;
				if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) &&
					UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, m_Owner))
				{
					return;
				}

				if (o is TrapableContainer)
				{
					m_Owner.Target((TrapableContainer)o);
				}
				else
				{
					from.SendMessage("You can't trap that");
				}
			}

			protected override void OnTargetFinish(Mobile from)
			{
				m_Owner.FinishSequence();
			}
		}
	}
}