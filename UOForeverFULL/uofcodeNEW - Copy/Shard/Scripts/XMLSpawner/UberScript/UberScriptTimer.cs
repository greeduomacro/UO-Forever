#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public sealed class UberScriptTimer : Timer
	{
		private static readonly UberScriptTimer[] _Pool;

		static UberScriptTimer()
		{
			_Pool = new[] {//
				new UberScriptTimer(TimerPriority.EveryTick), //
				new UberScriptTimer(TimerPriority.TenMS), //
				new UberScriptTimer(TimerPriority.TwentyFiveMS), //
				new UberScriptTimer(TimerPriority.FiftyMS), //
				new UberScriptTimer(TimerPriority.TwoFiftyMS), //
				new UberScriptTimer(TimerPriority.OneSecond), //
				new UberScriptTimer(TimerPriority.FiveSeconds), //
				new UberScriptTimer(TimerPriority.OneMinute) //
			};
		}

		public static void Configure()
		{
			_Pool.ForEach(t => t.Running = true); //Starts all timers
		}

		public static TimeSpan DelayFromPriority(TimerPriority p)
		{
			switch (p)
			{
				case TimerPriority.EveryTick:
					return TimeSpan.FromMilliseconds(1);
				case TimerPriority.TenMS:
					return TimeSpan.FromMilliseconds(10);
				case TimerPriority.TwentyFiveMS:
					return TimeSpan.FromMilliseconds(25);
				case TimerPriority.FiftyMS:
					return TimeSpan.FromMilliseconds(50);
				case TimerPriority.TwoFiftyMS:
					return TimeSpan.FromMilliseconds(250);
				case TimerPriority.OneSecond:
					return TimeSpan.FromSeconds(1);
				case TimerPriority.FiveSeconds:
					return TimeSpan.FromSeconds(5);
				case TimerPriority.OneMinute:
					return TimeSpan.FromMinutes(1);
			}

			//default:
			return TimeSpan.FromMinutes(1);
		}

		private readonly TimerPriority _InternalPriority;

		private UberScriptTimer(TimerPriority p)
			: base(DelayFromPriority(p), DelayFromPriority(p))
		{
			_InternalPriority = p;
		}

		protected override void OnTick()
		{
			base.OnTick();

			UberScriptTimedScripts.OnTick(_InternalPriority);
		}
	}

	public class PausedUberScript : Timer
	{
		public TriggerObject TrigObject;
		public bool TriggersOnly = false;
		public bool RelativeDelay = false; // if true, then server down time doesn't count towards the time
		public DateTime ExpectedEndTime = DateTime.MinValue;

		public const string END_DICTIONARY_DESERIALIZATION = "!!!!!!!END!!!!!!";

		public PausedUberScript(TriggerObject trigObject, bool triggersOnly, double milliseconds)
			: base(TimeSpan.FromMilliseconds(milliseconds))
		{
			TriggersOnly = triggersOnly;
			TrigObject = trigObject;
			ExpectedEndTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(milliseconds);
			Start();
		}

		protected override void OnTick()
		{
			TrigObject.Script.Execute(TrigObject, TriggersOnly);
			Stop();
		}
	}

	public class UberScriptTimedScripts
	{
		public static List<XmlScript> EveryTick = new List<XmlScript>();
		public static List<XmlScript> EveryTenMS = new List<XmlScript>();
		public static List<XmlScript> EveryTwentyFiveMS = new List<XmlScript>();
		public static List<XmlScript> EveryFiftyMS = new List<XmlScript>();
		public static List<XmlScript> EveryTwoFiftyMS = new List<XmlScript>();
		public static List<XmlScript> EveryOneSecond = new List<XmlScript>();
		public static List<XmlScript> EveryFiveSeconds = new List<XmlScript>();
		public static List<XmlScript> EveryOneMinute = new List<XmlScript>();

		public static void ClearSubscriptions()
		{
			EveryTick.Clear();
			EveryTenMS.Clear();
			EveryTwentyFiveMS.Clear();
			EveryFiftyMS.Clear();
			EveryTwoFiftyMS.Clear();
			EveryOneSecond.Clear();
			EveryFiveSeconds.Clear();
			EveryOneMinute.Clear();
		}

		public static void OnTick(TimerPriority p)
		{
			switch (p)
			{
				case TimerPriority.EveryTick:
					{
						if (EveryTick.Count > 0)
						{
							Trigger(EveryTick.ToArray(), TriggerName.onTick);
						}
					}
					break;
				case TimerPriority.TenMS:
					{
						if (EveryTenMS.Count > 0)
						{
							Trigger(EveryTenMS.ToArray(), TriggerName.onTenMS);
						}
					}
					break;
				case TimerPriority.TwentyFiveMS:
					{
						if (EveryTwentyFiveMS.Count > 0)
						{
							Trigger(EveryTwentyFiveMS.ToArray(), TriggerName.onTwentyFiveMS);
						}
					}
					break;
				case TimerPriority.FiftyMS:
					{
						if (EveryFiftyMS.Count > 0)
						{
							Trigger(EveryFiftyMS.ToArray(), TriggerName.onFiftyMS);
						}
					}
					break;
				case TimerPriority.TwoFiftyMS:
					{
						if (EveryTwoFiftyMS.Count > 0)
						{
							Trigger(EveryTwoFiftyMS.ToArray(), TriggerName.onTwoFiftyMS);
						}
					}
					break;
				case TimerPriority.OneSecond:
					{
						if (EveryOneSecond.Count > 0)
						{
							Trigger(EveryOneSecond.ToArray(), TriggerName.onOneSecond);
						}
					}
					break;
				case TimerPriority.FiveSeconds:
					{
						if (EveryFiveSeconds.Count > 0)
						{
							Trigger(EveryFiveSeconds.ToArray(), TriggerName.onFiveSeconds);
						}
					}
					break;
				case TimerPriority.OneMinute:
					{
						if (EveryOneMinute.Count > 0)
						{
							Trigger(EveryOneMinute.ToArray(), TriggerName.onOneMinute);
						}
					}
					break;
			}
		}

		public static void Trigger(XmlScript[] scripts, TriggerName triggerName)
		{
			foreach (XmlScript script in scripts)
			{
				script.Execute(
					new TriggerObject
					{
						TrigName = triggerName,
						This = script.AttachedTo
					},
					true);
			}
		}

		public static void UnsubscribeScript(XmlScript toRemove)
		{
			EveryTick.Remove(toRemove);
			EveryTenMS.Remove(toRemove);
			EveryTwentyFiveMS.Remove(toRemove);
			EveryFiftyMS.Remove(toRemove);
			EveryTwoFiftyMS.Remove(toRemove);
			EveryOneSecond.Remove(toRemove);
			EveryFiveSeconds.Remove(toRemove);
			EveryOneMinute.Remove(toRemove);
		}

		public static void SubscribeScript(XmlScript script, List<string> timerTriggers)
		{
			TriggerName triggerName;

			foreach (string timerTrigger in timerTriggers)
			{
				if (!Enum.TryParse(timerTrigger, out triggerName))
				{
					continue;
				}

				//if (timerTrigger == UberScriptTriggers.ON_TICK && !EveryTick.Contains(script)) EveryTick.Add(script);
				if (triggerName == TriggerName.onTick && !EveryTick.Contains(script))
				{
					EveryTick.Add(script);
				}
				else if (triggerName == TriggerName.onTenMS && !EveryTenMS.Contains(script))
				{
					EveryTenMS.Add(script);
				}
				else if (triggerName == TriggerName.onTwentyFiveMS && !EveryTwentyFiveMS.Contains(script))
				{
					EveryTwentyFiveMS.Add(script);
				}
				else if (triggerName == TriggerName.onFiftyMS && !EveryFiftyMS.Contains(script))
				{
					EveryFiftyMS.Add(script);
				}
				else if (triggerName == TriggerName.onTwoFiftyMS && !EveryTwoFiftyMS.Contains(script))
				{
					EveryTwoFiftyMS.Add(script);
				}
				else if (triggerName == TriggerName.onOneSecond && !EveryOneSecond.Contains(script))
				{
					EveryOneSecond.Add(script);
				}
				else if (triggerName == TriggerName.onFiveSeconds && !EveryFiveSeconds.Contains(script))
				{
					EveryFiveSeconds.Add(script);
				}
				else if (triggerName == TriggerName.onOneMinute && !EveryOneMinute.Contains(script))
				{
					EveryOneMinute.Add(script);
				}
			}
		}

		public static void SubscribeScript(XmlScript script, XmlScript.TimerSubscriptionFlag flags)
		{
			if (flags.HasFlag(XmlScript.TimerSubscriptionFlag.EveryTick) && !EveryTick.Contains(script))
			{
				EveryTick.Add(script);
			}

			if (flags.HasFlag(XmlScript.TimerSubscriptionFlag.TenMS) && !EveryTenMS.Contains(script))
			{
				EveryTenMS.Add(script);
			}

			if (flags.HasFlag(XmlScript.TimerSubscriptionFlag.TwentyFiveMS) && !EveryTwentyFiveMS.Contains(script))
			{
				EveryTwentyFiveMS.Add(script);
			}

			if (flags.HasFlag(XmlScript.TimerSubscriptionFlag.FiftyMS) && !EveryFiftyMS.Contains(script))
			{
				EveryFiftyMS.Add(script);
			}

			if (flags.HasFlag(XmlScript.TimerSubscriptionFlag.TwoFiftyMS) && !EveryTwoFiftyMS.Contains(script))
			{
				EveryTwoFiftyMS.Add(script);
			}

			if (flags.HasFlag(XmlScript.TimerSubscriptionFlag.OneSecond) && !EveryOneSecond.Contains(script))
			{
				EveryOneSecond.Add(script);
			}

			if (flags.HasFlag(XmlScript.TimerSubscriptionFlag.FiveSeconds) && !EveryFiveSeconds.Contains(script))
			{
				EveryFiveSeconds.Add(script);
			}

			if (flags.HasFlag(XmlScript.TimerSubscriptionFlag.OneMinute) && !EveryOneMinute.Contains(script))
			{
				EveryOneMinute.Add(script);
			}
		}
	}
}