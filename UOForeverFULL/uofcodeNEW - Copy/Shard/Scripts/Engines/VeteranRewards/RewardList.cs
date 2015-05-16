using System;

namespace Server.Engines.VeteranRewards
{
	public class RewardList
	{
		private TimeSpan m_Age;
		private TimeSpan m_GameTime;
		private RewardEntry[] m_Entries;

		public TimeSpan Age{ get{ return m_Age; } }
		public TimeSpan GameTime{ get{ return m_GameTime; } }
		public RewardEntry[] Entries{ get{ return m_Entries; } }

		public RewardList( int index, RewardEntry[] entries ) : this( TimeSpan.FromTicks( RewardSystem.RewardInterval.Ticks * index ), TimeSpan.FromTicks( RewardSystem.RewardGameTimeInterval.Ticks * index ), entries )
		{
		}

		public RewardList( TimeSpan interval, TimeSpan gametime, RewardEntry[] entries )
		{
			m_Age = interval;
			m_GameTime = gametime;
			m_Entries = entries;

			for ( int i = 0; i < entries.Length; ++i )
				entries[i].List = this;
		}
	}
}