#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Multis
{
	public class DynamicDecay
	{
		public static bool Enabled = true;

		private static readonly Dictionary<DecayLevel, DecayStageInfo> m_Stages;

		static DynamicDecay()
		{
			m_Stages = new Dictionary<DecayLevel, DecayStageInfo>();

			Register(DecayLevel.LikeNew, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
			Register(DecayLevel.Slightly, TimeSpan.FromDays(7.3), TimeSpan.FromDays(7.3));
			Register(DecayLevel.Somewhat, TimeSpan.FromDays(7.5), TimeSpan.FromDays(7.5));
			Register(DecayLevel.Fairly, TimeSpan.FromDays(7.5), TimeSpan.FromDays(7.5));
			Register(DecayLevel.Greatly, TimeSpan.FromDays(6), TimeSpan.FromDays(6));
			Register(DecayLevel.IDOC, TimeSpan.FromHours(2), TimeSpan.FromHours(4));
		}

		public static void Register(DecayLevel level, TimeSpan min, TimeSpan max)
		{
			var info = new DecayStageInfo(min, max);

			if (m_Stages.ContainsKey(level))
			{
				m_Stages[level] = info;
			}
			else
			{
				m_Stages.Add(level, info);
			}
		}

		public static bool Decays(DecayLevel level)
		{
			return m_Stages.ContainsKey(level);
		}

		public static TimeSpan GetRandomDuration(DecayLevel level)
		{
			if (!m_Stages.ContainsKey(level))
			{
				return TimeSpan.Zero;
			}

			DecayStageInfo info = m_Stages[level];
			long min = info.MinDuration.Ticks;
			long max = info.MaxDuration.Ticks;

			return TimeSpan.FromTicks(min + (long)(Utility.RandomDouble() * (max - min)));
		}
	}

	public class DecayStageInfo
	{
		private readonly TimeSpan m_MinDuration;
		private readonly TimeSpan m_MaxDuration;

		public TimeSpan MinDuration { get { return m_MinDuration; } }

		public TimeSpan MaxDuration { get { return m_MaxDuration; } }

		public DecayStageInfo(TimeSpan min, TimeSpan max)
		{
			m_MinDuration = min;
			m_MaxDuration = max;
		}
	}
}