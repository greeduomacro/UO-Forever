#region Header
//   Vorspire    _,-'/-'/  MobileExt.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;

using Server.Mobiles;
using Server.Targeting;

using VitaNex;
using VitaNex.Notify;
using VitaNex.Targets;
#endregion

namespace Server
{
	public static class MobileExtUtility
	{
		public static TimeSpan CombatHeatDelay = TimeSpan.FromSeconds(5.0);

		public static bool InCombat(this Mobile m)
		{
			return InCombat(m, CombatHeatDelay);
		}

		public static bool InCombat(this Mobile m, TimeSpan heat)
		{
			var now = DateTime.Now;
			var utc = DateTime.UtcNow;

			return m != null &&
				   (m.Aggressed.Any(info => info.LastCombatTime + heat > (info.LastCombatTime.Kind == DateTimeKind.Utc ? utc : now)) ||
					m.Aggressors.Any(info => info.LastCombatTime + heat > (info.LastCombatTime.Kind == DateTimeKind.Utc ? utc : now)));
		}

		public static bool IsControlled(this Mobile m)
		{
			Mobile master;
			return IsControlled(m, out master);
		}

		public static bool IsControlled(this Mobile m, out Mobile master)
		{
			if (m is BaseCreature)
			{
				BaseCreature c = (BaseCreature)m;
				master = c.GetMaster();
				return c.Controlled;
			}

			master = null;
			return false;
		}

		public static void TryParalyze(this Mobile m, TimeSpan duration, TimerStateCallback<Mobile> callback = null)
		{
			m.Paralyze(duration);

			if (callback != null)
			{
				Timer.DelayCall(duration, callback, m);
			}
		}

		public static void TryFreeze(this Mobile m, TimeSpan duration, TimerStateCallback<Mobile> callback = null)
		{
			m.Freeze(duration);

			if (callback != null)
			{
				Timer.DelayCall(duration, callback, m);
			}
		}

		private static readonly MethodInfo _SleepImpl = typeof(Mobile).GetMethod("Sleep") ??
														typeof(Mobile).GetMethod("DoSleep");

		public static void TrySleep(this Mobile m, TimeSpan duration, TimerStateCallback<Mobile> callback = null)
		{
			if (_SleepImpl != null)
			{
				VitaNexCore.TryCatch(
					() =>
					{
						_SleepImpl.Invoke(m, new object[] {duration});

						if (callback != null)
						{
							Timer.DelayCall(duration, callback, m);
						}
					});
			}
		}

		public static void SendNotification(
			this Mobile m,
			string html,
			bool autoClose = true,
			double delay = 1.0,
			double pause = 3.0,
			Color? color = null,
			Action<NotifyGump> beforeSend = null,
			Action<NotifyGump> afterSend = null)
		{
			if (m is PlayerMobile)
			{
				Notify.Send((PlayerMobile)m, html, autoClose, delay, pause, color, beforeSend, afterSend);
			}
		}

		public static void SendNotification<TGump>(
			this Mobile m,
			string html,
			bool autoClose = true,
			double delay = 1.0,
			double pause = 3.0,
			Color? color = null,
			Action<TGump> beforeSend = null,
			Action<TGump> afterSend = null) where TGump : NotifyGump
		{
			if (m is PlayerMobile)
			{
				Notify.Send((PlayerMobile)m, html, autoClose, delay, pause, color, beforeSend, afterSend);
			}
		}

		public static int GetNotorietyHue(this Mobile source, Mobile target = null)
		{
			return ComputeNotoriety(source, target).GetHue();
		}

		public static Color GetNotorietyColor(this Mobile source, Mobile target = null)
		{
			return ComputeNotoriety(source, target).GetColor();
		}

		public static NotorietyType ComputeNotoriety(this Mobile source, Mobile target = null)
		{
			if (source == null && target != null)
			{
				source = target;
			}

			if (source != null)
			{
				return (NotorietyType)Notoriety.Compute(source, target ?? source);
			}

			return NotorietyType.None;
		}

		public static void Control(this BaseCreature pet, Mobile master)
		{
			if (pet == null || pet.Deleted || master == null || master.Deleted)
			{
				return;
			}

			pet.CurrentWayPoint = null;

			pet.ControlMaster = master;
			pet.Controlled = true;
			pet.ControlTarget = null;
			pet.ControlOrder = OrderType.Come;
			pet.Guild = null;

			pet.Delta(MobileDelta.Noto);
		}

		public static bool Stable(this BaseCreature pet, bool maxLoyalty = true, bool autoStable = true)
		{
			if (pet == null || pet.Deleted || pet.IsStabled || !(pet.ControlMaster is PlayerMobile))
			{
				return false;
			}

			var master = (PlayerMobile)pet.ControlMaster;

			pet.ControlTarget = null;
			pet.ControlOrder = OrderType.Stay;
			pet.Internalize();

			pet.SetControlMaster(null);
			pet.SummonMaster = null;

			pet.IsStabled = true;
		    pet.StabledDate = DateTime.UtcNow;

			if (maxLoyalty)
			{
				pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy
			}

			master.Stabled.Add(pet);

			if (autoStable)
			{
				master.AutoStabled.Add(pet);
			}

			return true;
		}

		/// <summary>
		///     Begin targeting for the specified Mobile with definded handlers
		/// </summary>
		/// <param name="m">Mobile owner of the new GenericSelectTarget instance</param>
		/// <param name="success">Success callback</param>
		/// <param name="fail">Failure callback</param>
		/// <param name="range">Maximum distance allowed</param>
		/// <param name="allowGround">Allow ground as valid target</param>
		/// <param name="flags">Target flags determine the target action</param>
		public static GenericSelectTarget<TObj> BeginTarget<TObj>(
			this Mobile m,
			Action<Mobile, TObj> success,
			Action<Mobile> fail,
			int range = -1,
			bool allowGround = false,
			TargetFlags flags = TargetFlags.None)
		{
			if (m == null || m.Deleted)
			{
				return null;
			}

			var t = new GenericSelectTarget<TObj>(success, fail, range, allowGround, flags);

			m.Target = t;

			return t;
		}

		public static TMobile GetLastKiller<TMobile>(this Mobile m, bool petMaster = false) where TMobile : Mobile
		{
			if (m == null || m.LastKiller == null)
			{
				return null;
			}

			var killer = m.LastKiller as TMobile;

			if (killer == null && petMaster && m.LastKiller is BaseCreature)
			{
				killer = ((BaseCreature)m.LastKiller).GetMaster<TMobile>();
			}

			return killer;
		}
		
		public static TItem FindItemOnLayer<TItem>(this Mobile m, Layer layer) where TItem : Item
		{
			return m.FindItemOnLayer(layer) as TItem;
		}

		public static bool HasItem<TItem>(this Mobile m, int amount = 1, bool children = true) where TItem : Item
		{
			return HasItem(m, typeof(TItem), amount, children);
		}

		public static bool HasItem(this Mobile m, Type type, int amount = 1, bool children = true)
		{
			if (m == null || type == null || amount < 1)
			{
				return false;
			}

			long sum = 0;

			sum += m.Items.Where(i => i != null && !i.Deleted && (children || i.GetType().IsEqual(type)))
					.Sum(i => (long)i.Amount);

			if (m.Backpack != null)
			{
				sum +=
					m.Backpack.FindItemsByType(type, true)
					 .Where(i => i != null && !i.Deleted && (children || i.GetType().IsEqual(type)))
					 .Sum(i => (long)i.Amount);
			}

			return sum >= amount;
		}

		public static bool HasItems(this Mobile m, Type[] types, int[] amounts = null, bool children = true)
		{
			if (m == null || types == null || types.Length == 0)
			{
				return false;
			}

			if (amounts == null)
			{
				amounts = new int[0];
			}

			int count = 0;

			types.For(
				(i, t) =>
				{
					int amount = amounts.InBounds(i) ? amounts[i] : 1;

					if (HasItem(m, t, amount, children))
					{
						++count;
					}
				});

			return count >= types.Length;
		}
	}
}