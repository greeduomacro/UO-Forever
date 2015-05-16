using System;
using Server;
using Server.Ethics;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Commands;

namespace Server.Factions
{
	public class PlayerState : IComparable
	{
		private Mobile m_Mobile;
		private Faction m_Faction;
		private List<PlayerState> m_Owner;
		private int m_KillPoints;
		private DateTime m_Leaving;
		private MerchantTitle m_MerchantTitle;
		private RankDefinition m_Rank;
		private List<SilverGivenEntry> m_SilverGiven;
		private bool m_IsActive;
		private DateTime m_LastKill;
		private DateTime m_JoinDate;

		private Town m_Sheriff;
		private Town m_Finance;

		private DateTime m_LastHonorTime;

		public Mobile Mobile{ get{ return m_Mobile; } }
		public Faction Faction{ get{ return m_Faction; } }
		public DateTime LastKill{ get{ return m_LastKill; } set{ m_LastKill = value; } }
		public DateTime JoinDate{ get{ return m_JoinDate; } }
		public List<PlayerState> Owner { get { return m_Owner; } }
		public MerchantTitle MerchantTitle{ get{ return m_MerchantTitle; } set{ m_MerchantTitle = value; Invalidate(); } }
		public Town Sheriff{ get{ return m_Sheriff; } set{ m_Sheriff = value; Invalidate(); } }
		public Town Finance{ get{ return m_Finance; } set{ m_Finance = value; Invalidate(); } }
		public List<SilverGivenEntry> SilverGiven { get { return m_SilverGiven; } }
		
		public DateTime m_NextM;

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime NextM
		{
			get{ return m_NextM; }
			set{ m_NextM = value; }
		}

		public int KillPoints
		{
			get { return m_KillPoints; }
			set
			{
				if ( m_KillPoints != value )
				{
					LoggingCustom.Log("LOG_FactionPoints.txt", DateTime.Now + "\t" + (m_Mobile != null ? m_Mobile.Name : "null") + "\tbefore: " + m_KillPoints + "\tafter: " + value);
                    if (value == -5)
                    {
                        try
                        {
							LoggingCustom.Log("LOG_FactionPoints.txt", DateTime.Now + "\tPossible problem setting to -5? " + (new System.Diagnostics.StackTrace()).ToString());
                        }
                        catch
                        {
							LoggingCustom.Log("LOG_FactionPoints.txt", DateTime.Now + " Stack trace error");
                        }
                    }
                    if ( value > m_KillPoints )
					{
						if ( m_KillPoints <= 0 )
						{
							if ( value <= 0 )
							{
								m_KillPoints = value;
								Invalidate();
								return;
							}

							m_Owner.Remove( this );
							m_Owner.Insert( m_Faction.ZeroRankOffset, this );

							m_RankIndex = m_Faction.ZeroRankOffset;
							m_Faction.ZeroRankOffset++;
						}
						while ( ( m_RankIndex - 1 ) >= 0 )
						{
							PlayerState p = m_Owner[m_RankIndex-1] as PlayerState;
							if ( value > p.KillPoints )
							{
								m_Owner[m_RankIndex] = p;
								m_Owner[m_RankIndex-1] = this;
								RankIndex--;
								p.RankIndex++;
							}
							else
								break;
						}
					}
					else
					{
						if ( value <= 0 )
						{
							if ( m_KillPoints <= 0 )
							{
								m_KillPoints = value;
								Invalidate();
								return;
							}

							while ( ( m_RankIndex + 1 ) < m_Faction.ZeroRankOffset )
							{
								PlayerState p = m_Owner[m_RankIndex+1] as PlayerState;
								m_Owner[m_RankIndex+1] = this;
								m_Owner[m_RankIndex] = p;
								RankIndex++;
								p.RankIndex--;
							}

							m_RankIndex = -1;
							m_Faction.ZeroRankOffset--;
						}
						else
						{
							while ( ( m_RankIndex + 1 ) < m_Faction.ZeroRankOffset )
							{
								PlayerState p = m_Owner[m_RankIndex+1] as PlayerState;
								if ( value < p.KillPoints )
								{
									m_Owner[m_RankIndex+1] = this;
									m_Owner[m_RankIndex] = p;
									RankIndex++;
									p.RankIndex--;
								}
								else
									break;
							}
						}
					}

					m_KillPoints = value;
					Invalidate();
				}
			}
		}

		private bool m_InvalidateRank = true;
		private int  m_RankIndex = -1;

		public int RankIndex { get { return m_RankIndex; } set { if ( m_RankIndex != value ) { m_RankIndex = value; m_InvalidateRank = true; } } }

		public RankDefinition Rank
		{
			get
			{
				if ( m_InvalidateRank )
				{
					RankDefinition[] ranks = m_Faction.Definition.Ranks;
					int percent;

					if ( m_Owner.Count == 1 )
						percent = 1000;
					else if ( m_RankIndex == -1 )
						percent = 0;
					else
						percent = ( ( m_Faction.ZeroRankOffset - m_RankIndex ) * 1000 ) / m_Faction.ZeroRankOffset;

					for ( int i = 0; i < ranks.Length; i++ )
					{
						RankDefinition check = ranks[i];

						if ( percent >= check.Required )
						{
							m_Rank = check;
							m_InvalidateRank = false;
							break;
						}
					}

					Invalidate();
				}

				return m_Rank;
			}
		}

		public DateTime LastHonorTime{ get{ return m_LastHonorTime; } set{ m_LastHonorTime = value; } }
		public DateTime Leaving{ get{ return m_Leaving; } set{ m_Leaving = value; } }
		public bool IsLeaving{ get{ return ( m_Leaving > DateTime.MinValue ); } }

		public bool IsActive{ get{ return m_IsActive; } set{ m_IsActive = value; } }

		public bool CanGiveSilverTo( Mobile mob )
		{
			if ( m_SilverGiven == null )
				return true;

		    if (mob.Address == Mobile.Address)
		        return false;

			for ( int i = 0; i < m_SilverGiven.Count; ++i )
			{
				SilverGivenEntry sge = m_SilverGiven[i];

				if ( sge.IsExpired )
					m_SilverGiven.RemoveAt( i-- );
				else if ( sge.GivenTo == mob )
					return false;
			}

			return true;
		}

		public void OnGivenSilverTo( Mobile mob )
		{
			if ( m_SilverGiven == null )
				m_SilverGiven = new List<SilverGivenEntry>();

			m_SilverGiven.Add( new SilverGivenEntry( mob ) );
		}

		public void Invalidate()
		{
			if ( m_Mobile is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m_Mobile;
				pm.InvalidateProperties();
                //pm.InvalidateLegends();
			}
		}

		public void Attach()
		{
			if ( m_Mobile is PlayerMobile )
				((PlayerMobile)m_Mobile).FactionPlayerState = this;
		}

		public PlayerState( Mobile mob, Faction faction, List<PlayerState> owner )
		{
			m_JoinDate = DateTime.UtcNow;
			m_Mobile = mob;
			m_Faction = faction;
			m_Owner = owner;

			Attach();
			Invalidate();
		}

		public PlayerState( GenericReader reader, Faction faction, List<PlayerState> owner )
		{
			m_Faction = faction;
			m_Owner = owner;

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 3:
				{
					m_JoinDate = reader.ReadDateTime();
					goto case 2;
				}
				case 2:
				{
					m_LastKill = reader.ReadDateTime();
					goto case 1;
				}
				case 1:
				{
					m_IsActive = reader.ReadBool();
					m_LastHonorTime = reader.ReadDateTime();
					goto case 0;
				}
				case 0:
				{
					m_Mobile = reader.ReadMobile();

					m_KillPoints = reader.ReadEncodedInt();
					m_MerchantTitle = (MerchantTitle)reader.ReadEncodedInt();

					m_Leaving = reader.ReadDateTime();

					if ( version < 3 )
						m_JoinDate = DateTime.UtcNow;

					break;
				}
			}

			Attach();
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 3 ); // version

			writer.Write( m_JoinDate );

			writer.Write( m_LastKill );

			writer.Write( m_IsActive );
			writer.Write( m_LastHonorTime );

			writer.Write( (Mobile) m_Mobile );

			writer.WriteEncodedInt( (int) m_KillPoints );
			writer.WriteEncodedInt( (int) m_MerchantTitle );

			writer.Write( (DateTime) m_Leaving );
		}

		public static PlayerState Find( Mobile mob )
		{
			if ( mob is PlayerMobile )
				return ((PlayerMobile)mob).FactionPlayerState;

			return null;
		}

		public int CompareTo( object obj )
		{
			return ((PlayerState)obj).m_KillPoints - m_KillPoints;
		}
	}
}