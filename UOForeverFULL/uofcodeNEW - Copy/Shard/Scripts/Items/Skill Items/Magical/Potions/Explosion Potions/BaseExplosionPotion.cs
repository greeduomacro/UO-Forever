#region References
using System;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.Conquests;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Targeting;
#endregion

namespace Server.Items
{
	public abstract class BaseExplosionPotion : BasePotion
	{
		//public abstract int MinDamage { get; }
		//public abstract int MaxDamage { get; }
		public abstract int Damage { get; }
		public abstract double Delay { get; }

		public override bool RequireFreeHand { get { return SpecialMovesController._Allow2HanderExplosionPots; } }

		private static bool LeveledExplosion = true; // Should explosion potions explode other nearby potions?
		private static bool InstantExplosion = false; // Should explosion potions explode on impact?
		private static bool RelativeLocation = true; // Is the explosion target location relative for mobiles?
		private const int ExplosionRange = 2; // How long is the blast radius?

        private bool _CanThrow = true;

		public BaseExplosionPotion(PotionEffect effect)
			: base(0xF0D, effect)
		{ }

		public BaseExplosionPotion(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		public virtual IEntity FindParent(Mobile from)
		{
			Mobile m = HeldBy;

			if (m != null)
			{
				return m;
			}

			IEntity obj = RootParentEntity;

			if (obj != null)
			{
				return obj;
			}

			if (Map == Map.Internal)
			{
				return from;
			}

			return this;
		}

		private Timer m_Timer;
		private List<Mobile> m_Users;

		public override bool Drink(Mobile from)
		{
			if (EraAOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
			{
				from.SendLocalizedMessage(1062725); // You can not use that potion while paralyzed.
				return false;
			}
			else
			{
				var targ = from.Target as ThrowTarget;
				Stackable = false; // Scavenged explosion potions won't stack with those ones in backpack, and still will explode.

				if (targ == null || targ.Potion != this) //Already have a targeter from this potion
				{
                    if (_CanThrow || FindParent(from) is Mobile)
				    {
				        if (m_Timer != null)
				        {
				            Target(from);
				        }
				        else if (from.BeginAction(typeof(BaseExplosionPotion)))
				        {
				            Target(from);

				            from.SendLocalizedMessage(500236); // You should throw it now!

				            int count = Utility.Random(3, 2);

				            //if( Core.ML )
				            //m_Timer = Timer.DelayCall<ExplodeCount>( TimeSpan.FromSeconds( 0.75 ), TimeSpan.FromSeconds( 1.0 ), count+1, new TimerStateCallback<ExplodeCount>( Detonate_OnTick ), new ExplodeCount( from, count ) ); // 3.6 seconds explosion delay
				            //else
				            m_Timer = Timer.DelayCall(
				                TimeSpan.FromSeconds(0.75), TimeSpan.FromSeconds(1.0), count + 1, Detonate_OnTick,
				                new ExplodeCount(from, count));
				            // 2.6 seconds explosion delay

				            _CanThrow = false;
				            double delay = Delay;
				            if (from.FindItemOnLayer(Layer.OneHanded) != null || from.FindItemOnLayer(Layer.TwoHanded) != null)
				            {
				                delay += ExplosionPotionController._ExplosionPotionHandsNotFreeDelay;
				            }
				            Timer.DelayCall(TimeSpan.FromSeconds(delay), ReleaseExplosionLock, from);
				        }
				        else
				        {
				            from.LocalOverheadMessage(
				                MessageType.Regular, 0x22, false, "You must wait a moment before using another explosion potion.");
							return false;
				        }
				    }
				    else
				    {
				        from.SendMessage("You cannot do this again so soon!");
						return false;
				    }
				}
			}

			return true;
		}

		public void Target(Mobile from)
		{
			if (m_Users == null)
			{
				m_Users = new List<Mobile>();
			}

			if (!m_Users.Contains(from))
			{
				m_Users.Add(from);
			}

			from.RevealingAction();
			from.Target = new ThrowTarget(this);
		}

		private static void ReleaseExplosionLock(Mobile from)
		{
			from.EndAction(typeof(BaseExplosionPotion));
		}

		private class ExplodeCount
		{
			public readonly Mobile From;
			public int Count;

			public ExplodeCount(Mobile from, int count)
			{
				From = from;
				Count = count;
			}
		}

		private void Detonate_OnTick(ExplodeCount counter)
		{
			if (Deleted)
			{
				return;
			}

			Mobile from = counter.From;
			int timer = counter.Count--;

			IEntity parent = FindParent(from);

			if (timer <= 0)
			{
				//from.SendMessage( "Explode!" );
				Explode(from, true, parent.Location, parent.Map);
				m_Timer = null;
			}
			else
			{
				//from.SendMessage( "Tick!" );
				if (parent is Item)
				{
					((Item)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
				}
				else if (parent is Mobile)
				{
					((Mobile)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
				}
			}
		}

		private void Reposition_OnTick(IEntity[] states)
		{
			if (Deleted)
			{
				return;
			}

			//((Mobile)states[0]).SendMessage( "Moving!" );

			if (InstantExplosion)
			{
				Explode((Mobile)states[0], true, states[1].Location, states[1].Map);
			}
			else
			{
				MoveToWorld(states[1].Location, states[1].Map);
			}
		}

		private class ThrowTarget : Target
		{
			private readonly BaseExplosionPotion m_Potion;

			public BaseExplosionPotion Potion { get { return m_Potion; } }

			public ThrowTarget(BaseExplosionPotion potion)
				: base(12, true, TargetFlags.None)
			{
				m_Potion = potion;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (m_Potion != null && !m_Potion.Deleted && m_Potion.Map != Map.Internal)
				{
					var mob = targeted as Mobile;
					var p = targeted as IPoint3D;

					if (p != null)
					{
						Map map = from.Map;

						if (map != null)
						{
							SpellHelper.GetSurfaceTop(ref p);

							from.RevealingAction();

							IEntity to = new Entity(Serial.Zero, new Point3D(p), map);

							if (RelativeLocation && mob != null)
							{
								to = mob;
							}

							Effects.SendMovingEffect(from, to, m_Potion.ItemID, 7, 0, false, false, m_Potion.Hue, 0);

							var entity = targeted as IEntity;
							if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) &&
								UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, m_Potion))
							{
								Effects.PlaySound(p, map, 0x307);
								Effects.SendLocationEffect(p, map, 0x36BD, 9, 10, 0, 0);
								m_Potion.Consume();
								return;
							}

							if (m_Potion.Amount > 1)
							{
								Mobile.LiftItemDupe(m_Potion, 1);
							}

							m_Potion.Internalize();
							Timer.DelayCall(TimeSpan.FromSeconds(1.0), m_Potion.Reposition_OnTick, new[] {from, to});
						}
					}
				}
			}
		}

		public void Explode(Mobile from, bool direct, Point3D loc, Map map)
		{
			if (Deleted)
			{
				return;
			}

			for (int i = 0; m_Users != null && i < m_Users.Count; ++i)
			{
				Mobile m = m_Users[i];
				var targ = m.Target as ThrowTarget;

				if (targ != null && targ.Potion == this)
				{
					Targeting.Target.Cancel(m);
				}
			}

			Consume();

			if (map == null)
			{
				return;
			}

			Effects.PlaySound(loc, map, 0x307);
			Effects.SendLocationEffect(loc, map, 0x36BD, 9, 10, 0, 0);

			int alchemyBonus = 0;

			if (direct)
			{
				alchemyBonus = (int)(from.Skills.Alchemy.Value * ExplosionPotionController._AlchemyBonusPercentageOfSkill * 0.01);
			}
			if (alchemyBonus > ExplosionPotionController._AlchemyBonusMax)
			{
				alchemyBonus = ExplosionPotionController._AlchemyBonusMax;
			}

			IPooledEnumerable eable = LeveledExplosion
										  ? map.GetObjectsInRange(loc, ExplosionRange)
										  : map.GetMobilesInRange(loc, ExplosionRange);
			var toExplode = new ArrayList();

			int toDamage = 0;

			foreach (IEntity o in eable)
			{
				if (o is Mobile &&
					(from == null || (SpellHelper.ValidIndirectTarget(from, (Mobile)o) && from.CanBeHarmful((Mobile)o, false))))
				{
					toExplode.Add(o);
					++toDamage;
				}
				else if (o is BaseExplosionPotion && o != this)
				{
					toExplode.Add(o);
				}
			}

			eable.Free();

			//			int min = Scale( from, MinDamage );
			//			int max = Scale( from, MaxDamage );

			for (int i = 0; i < toExplode.Count; ++i)
			{
				object o = toExplode[i];

				if (o is Mobile)
				{
					var m = (Mobile)o;

					if (from != null)
					{
						from.DoHarmful(m);

						double damage = Scale(from, Damage);

						damage += alchemyBonus;

						if (damage > 30)
						{
							damage = 15;
						}

						if (toDamage > 2)
						{
							damage /= toDamage - 1;
						}

						if (m is BaseCreature && !((BaseCreature)m).TakesNormalDamage)
						{
							damage *= 1.4;
						}

						var iDamage = (int)damage;
						if ((XmlScript.HasTrigger(from, TriggerName.onGivenHit) &&
							 UberScriptTriggers.Trigger(from, m, TriggerName.onGivenHit, this, null, null, iDamage)) ||
							(XmlScript.HasTrigger(m, TriggerName.onTakenHit) &&
							 UberScriptTriggers.Trigger(m, from, TriggerName.onTakenHit, this, null, null, iDamage)))
						{
							return;
						}
						m.Damage(iDamage, from);
					}
				}
				else if (o is BaseExplosionPotion)
				{
					var pot = (BaseExplosionPotion)o;

					pot.Explode(from, false, pot.GetWorldLocation(), pot.Map);
				}
			}
		}
	}
}