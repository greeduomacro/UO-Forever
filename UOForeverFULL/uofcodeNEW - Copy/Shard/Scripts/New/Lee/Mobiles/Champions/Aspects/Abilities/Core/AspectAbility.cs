#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Spells;

using VitaNex;
#endregion

namespace Server.Mobiles
{
	public abstract class AspectAbility
	{
		public sealed class State
		{
			public AspectAbility Ability { get; private set; }

			public BaseAspect Aspect { get; set; }
			public Mobile Target { get; set; }

			public bool Expires { get; set; }
			public DateTime Expire { get; set; }

			public bool TargetDeathPersist { get; set; }

			public bool IsValid
			{
				get
				{
					return Ability != null && Aspect != null && !Aspect.Deleted && Aspect.Alive && Target != null && !Target.Deleted &&
						   (Target.Alive || (TargetDeathPersist && (Target.Player || Target.IsDeadBondedPet)));
				}
			}

			public bool IsExpired { get { return CheckExpired(DateTime.UtcNow); } }

			public State(AspectAbility ability, BaseAspect aspect, Mobile target, TimeSpan duration)
			{
				Ability = ability;
				Aspect = aspect;
				Target = target;

				Expire = DateTime.UtcNow + duration;
				Expires = true;
			}

			public bool CheckExpired(DateTime utcNow)
			{
				return Expires && Expire < utcNow;
			}
		}

		private static PollTimer _Timer;

		public static AspectAbility[] Abilities { get; private set; }

		public static Dictionary<AspectAbility, Dictionary<Mobile, State>> States { get; private set; }

		static AspectAbility()
		{
			Abilities =
				typeof(AspectAbility).GetConstructableChildren()
									 .Select(t => t.CreateInstanceSafe<AspectAbility>())
									 .Where(a => a != null)
									 .ToArray();

			States = Abilities.ToDictionary(a => a, a => new Dictionary<Mobile, State>());
		}

		public static void Configure()
		{
			if (_Timer == null)
			{
				_Timer = PollTimer.FromMilliseconds(
					250.0, DefragmentStates, () => States.Values.Any(states => states != null && states.Count > 0), false);
			}
		}

		public static void Initialize()
		{
			if (_Timer != null)
			{
				_Timer.Start();
			}
		}

		public static void DefragmentStates()
		{
			DateTime now = DateTime.UtcNow;

			foreach (Dictionary<Mobile, State> states in States.Values)
			{
				states.RemoveValueRange(
					s =>
					{
						if (s == null)
						{
							return true;
						}

						if (!s.IsValid || s.CheckExpired(now))
						{
							if (s.Ability != null)
							{
								s.Ability.OnRemoved(s);
							}

							return true;
						}

						return false;
					});

				states.RemoveKeyRange(m => m == null || m.Deleted);
			}
		}

		public static AspectAbility[] GetAbilities(BaseAspect aspect, bool checkLock)
		{
			return Abilities.Where(a => a.CanInvoke(aspect)).ToArray();
		}

		public static bool HasAbility<TAbility>(BaseAspect aspect) where TAbility : AspectAbility
		{
			return Abilities.OfType<TAbility>().Any(a => a.HasFlags(aspect));
		}

		public abstract string Name { get; }

		public abstract Aspects Aspects { get; }

		public abstract TimeSpan Lockdown { get; }
		public abstract TimeSpan Cooldown { get; }

		public virtual TimeSpan Duration { get { return TimeSpan.Zero; } }

		public virtual int DamageMin { get { return 0; } }
		public virtual int DamageMax { get { return 0; } }

		public void Damage(BaseAspect aspect, Mobile target)
		{
			int damage = Utility.RandomMinMax(DamageMin, DamageMax);

			if (damage > 0)
			{
				OnDamage(aspect, target, ref damage);
			}

			if (damage > 0)
			{
				target.Damage(damage, aspect);
			}
		}

		protected virtual void OnDamage(BaseAspect aspect, Mobile target, ref int damage)
		{ }

		protected virtual void OnAdded(State state)
		{ }

		protected virtual void OnRemoved(State state)
		{ }

		public bool HasFlags(BaseAspect aspect)
		{
			return aspect != null && (aspect.Aspects.Equals(Aspects) || aspect.Aspects.HasFlag(Aspects));
		}

		public bool CheckLock(BaseAspect aspect, bool locked)
		{
			return aspect != null && (locked ? !aspect.CanBeginAction(this) : aspect.CanBeginAction(this));
		}

		public void SetLock(BaseAspect aspect, bool locked)
		{
			if (aspect == null)
			{
				return;
			}

			if (locked)
			{
				aspect.BeginAction(this);
				OnLocked(aspect);
			}
			else
			{
				aspect.EndAction(this);
				OnUnlocked(aspect);
			}
		}

		protected virtual TMobile[] AcquireTargets<TMobile>(
			BaseAspect aspect, Point3D p, int range, bool cache = true, Func<TMobile, bool> filter = null) where TMobile : Mobile
		{
			if (aspect == null || aspect.Deleted || aspect.Map == null || aspect.Map == Map.Internal)
			{
				return new TMobile[0];
			}

			var targets =
				p.GetMobilesInRange(aspect.Map, range)
				 .OfType<TMobile>()
				 .Where(m => m != null && !m.Deleted && m != aspect && m.AccessLevel <= aspect.AccessLevel && m.Alive)
				 .Where(m => aspect.CanBeHarmful(m, false, true) && SpellHelper.ValidIndirectTarget(aspect, m))
				 .Where(m => m.Party == null || m.Party != aspect.Party)
				 .Where(
					 m =>
					 m.Player || aspect.Combatant == m || aspect.FocusMob == m ||
					 (m is BaseCreature && (m as BaseCreature).GetMaster<PlayerMobile>() != null))
				 .ToArray();

			if (cache && Duration > TimeSpan.Zero)
			{
				foreach (var t in targets)
				{
					SetTargetState(aspect, t, Duration);
				}
			}

			return targets;
		}

		public State GetTargetState(Mobile m)
		{
			Dictionary<Mobile, State> states;

			if (States.TryGetValue(this, out states) && states != null)
			{
				return states.GetValue(m);
			}

			return null;
		}

		public void SetTargetState(BaseAspect aspect, Mobile target, TimeSpan duration)
		{
			Dictionary<Mobile, State> states;

			if (!States.TryGetValue(this, out states) || states == null)
			{
				States.AddOrReplace(this, val => val ?? (states = new Dictionary<Mobile, State>()));
			}

			states.AddOrReplace(
				target,
				state =>
				{
					if (state == null)
					{
						state = new State(this, aspect, target, duration);
					}
					else
					{
						OnRemoved(state);

						state.Aspect = aspect;
						state.Target = target;
						state.Expire = DateTime.UtcNow + duration;
					}

					OnAdded(state);

					return state;
				});
		}

		public virtual bool CanInvoke(BaseAspect aspect)
		{
			return aspect != null && !aspect.Deleted && aspect.Alive && !aspect.Blessed && //
				   aspect.InCombat() && HasFlags(aspect) && CheckLock(aspect, false);
		}

		public bool TryInvoke(BaseAspect aspect)
		{
			if (CanInvoke(aspect))
			{
				return VitaNexCore.TryCatchGet(
					() =>
					{
						OnInvoke(aspect);

						SetLock(aspect, true);

						Timer.DelayCall(Lockdown, () => SetLock(aspect, false));

						return true;
					},
					x => x.ToConsole(true));
			}

			return false;
		}

		protected abstract void OnInvoke(BaseAspect aspect);

		protected virtual void OnLocked(BaseAspect aspect)
		{ }

		protected virtual void OnUnlocked(BaseAspect aspect)
		{ }
	}
}