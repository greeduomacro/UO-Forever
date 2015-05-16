#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Commands;
using Server.Ethics;
using Server.Factions;
using Server.Games;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.SkillHandlers;
#endregion

namespace Server.Gumps
{
	public class ReportMurdererGump : Gump
	{
		private int m_Idx;
		private readonly List<Mobile> m_Killers;
		private Mobile m_Victum;
		private readonly Point3D m_Location;
		private readonly Map m_Map;

		public static void Initialize()
		{
			//AggressorInfo.ExpireDelay = TimeSpan.FromMinutes(5.0);

			EventSink.PlayerDeath += EventSink_PlayerDeath;
		}

		public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
		{
			Mobile m = e.Mobile;

			if (m.LastKiller != null &&
				(m.LastKiller == m || m.LastKiller is BaseGuard ||
				 m.LastKiller is BaseCreature && ((BaseCreature)m.LastKiller).GetMaster() == m))
			{
				return;
			}

			var killers = new List<Mobile>();
			var toGive = new List<Mobile>();

			DateTime now = DateTime.UtcNow;

			foreach (AggressorInfo ai in m.Aggressors)
			{
				//Allow people of the same ethic to flag each other as murderers?  Factioners can?
				if (ai.Attacker.Player && ai.CanReportMurder && !ai.Reported &&
					(!m.EraSE || !((PlayerMobile)m).RecentlyReported.Contains(ai.Attacker)))
				{
					killers.Add(ai.Attacker);
					ai.Reported = true;
					ai.CanReportMurder = false;
				}

				if (ai.Attacker.Player && now - ai.LastCombatTime < TimeSpan.FromSeconds(30.0) && !toGive.Contains(ai.Attacker))
				{
					toGive.Add(ai.Attacker);
				}
			}

			foreach (AggressorInfo ai in
				m.Aggressed.Where(
					ai => ai.Defender.Player && now - ai.LastCombatTime < TimeSpan.FromSeconds(30.0) && !toGive.Contains(ai.Defender)))
			{
				toGive.Add(ai.Defender);
			}

			foreach (Mobile g in toGive)
			{
				int n = Notoriety.Compute(g, m);

				//int theirKarma = m.Karma;
				int ourKarma = g.Karma;
				bool innocent = (n == Notoriety.Innocent);
				bool criminal = (n == Notoriety.Criminal || n == Notoriety.Murderer);

				int fameAward = m.Fame / 200;
				int karmaAward = 0;

				if (innocent)
				{
					karmaAward = (ourKarma > -2500 ? -850 : -110 - (m.Karma / 100));
				}
				else if (criminal)
				{
					karmaAward = 50;
				}

				Titles.AwardFame(g, fameAward, false);
				Titles.AwardKarma(g, karmaAward, true);
			}

			if (m is PlayerMobile && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild)
			{
				return;
			}

			if (killers.Count == 0)
			{
				return;
			}

			if (m is PlayerMobile && ((PlayerMobile)m).Young && MurderSystemController._MurderYoungFreezeSeconds > 0)
			{
				new ParaTimer(m, killers).Start();
			}

			new GumpTimer(m, killers, m.Location, m.Map).Start();

			foreach (Mobile killer in killers)
			{
				m.RemoveAggressor(killer);

				if (killer is PlayerMobile)
				{
					var playerKiller = (PlayerMobile)killer;

					foreach (Mobile pet in playerKiller.AllFollowers)
					{
						m.RemoveAggressor(pet);
					}
				}

				killer.RemoveAggressed(m);
			}
		}

		private class ParaTimer : Timer
		{
			private Mobile m_Victim;
			private readonly List<Mobile> m_Killers;
			private int m_TickCount;

			public ParaTimer(Mobile victim, List<Mobile> killers)
				: base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
			{
				m_Victim = victim;
				m_Killers = killers;

				foreach (Mobile m in m_Killers)
				{
					m.SendMessage(
						38,
						"You have been frozen for " + MurderSystemController._MurderYoungFreezeSeconds +
						" seconds for murdering a young innocent.");
				}
			}

			protected override void OnTick()
			{
				m_TickCount += 1;

				if (m_TickCount == MurderSystemController._MurderYoungFreezeSeconds)
				{
					foreach (Mobile m in m_Killers)
					{
						m.Frozen = false;
						m.SendMessage("You have been released.");
					}

					Stop();
				}
				else
				{
					foreach (Mobile m in m_Killers)
					{
						m.Frozen = true;
						m.RevealingAction();
					}
				}
			}
		}

		private class GumpTimer : Timer
		{
			private readonly Mobile m_Victim;
			private readonly List<Mobile> m_Killers;
			private readonly Point3D m_Location;
			private readonly Map m_Map;

			public GumpTimer(Mobile victim, List<Mobile> killers, Point3D loc, Map map)
				: base(TimeSpan.FromSeconds(4.0))
			{
				m_Victim = victim;
				m_Killers = killers;
				m_Location = loc;
				m_Map = map;
			}

			protected override void OnTick()
			{
				m_Victim.SendGump(new ReportMurdererGump(m_Victim, m_Killers, m_Location, m_Map));
			}
		}

		public ReportMurdererGump(Mobile victum, List<Mobile> killers, Point3D loc, Map map)
			: this(victum, killers, loc, map, 0)
		{ }

		private ReportMurdererGump(Mobile victum, List<Mobile> killers, Point3D loc, Map map, int idx)
			: base(0, 0)
		{
			m_Killers = killers;
			m_Victum = victum;
			m_Location = loc;
			m_Map = map;
			m_Idx = idx;

			BuildGump();
		}

		private void BuildGump()
		{
			Closable = false;
			Resizable = false;

			AddBackground(265, 205, 320, 290, 5054);

			AddPage(0);

			AddImageTiled(225, 175, 50, 45, 0xCE); //Top left corner
			AddImageTiled(267, 175, 315, 44, 0xC9); //Top bar
			AddImageTiled(582, 175, 43, 45, 0xCF); //Top right corner
			AddImageTiled(225, 219, 44, 270, 0xCA); //Left side
			AddImageTiled(582, 219, 44, 270, 0xCB); //Right side
			AddImageTiled(225, 489, 44, 43, 0xCC); //Lower left corner
			AddImageTiled(267, 489, 315, 43, 0xE9); //Lower Bar
			AddImageTiled(582, 489, 43, 43, 0xCD); //Lower right corner

			AddPage(1);

			AddHtml(260, 234, 300, 140, m_Killers[m_Idx].Name, false, false); // Player's Name
			AddHtmlLocalized(260, 254, 300, 140, 1049066, false, false); // Would you like to report...

			AddButton(260, 300, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(300, 300, 300, 50, 1046362, false, false); // Yes

			AddButton(360, 300, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
			AddHtmlLocalized(400, 300, 300, 50, 1046363, false, false); // No
		}

		public static void ReportedListExpiry_Callback(Mobile[] states)
		{
			var from = (PlayerMobile)states[0];
			Mobile killer = states[1];

			if (from.RecentlyReported.Contains(killer))
			{
				from.RecentlyReported.Remove(killer);
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			switch (info.ButtonID)
			{
				case 1:
					{
						Mobile killer = m_Killers[m_Idx];

						if (killer != null && !killer.Deleted)
						{
							if (from is PlayerMobile && ((PlayerMobile)from).Young && MurderSystemController._KillsPerYoungMurder > 1)
							{
								killer.Kills += MurderSystemController._KillsPerYoungMurder;
								killer.SendMessage(
									"The murder report resulted in " + MurderSystemController._KillsPerYoungMurder +
									" kill counts due to murderering a young player.");
							}
							else
							{
								killer.Kills++;
							}

							Region reg = Region.Find(m_Location, m_Map);
							GuardedRegion greg = null;

							if (reg != null)
							{
								greg = reg.GetRegion(typeof(GuardedRegion)) as GuardedRegion;
							}

							if (greg == null || !greg.IsDisabled())
							{
								killer.ShortTermMurders++;
							}

							//if(from.EraSE)
							//{
							((PlayerMobile)from).RecentlyReported.Add(killer);

							Timer.DelayCall(
								TimeSpan.FromMinutes(PseudoSeerStone.JustMurderedMinutesTracked),
								ReportedListExpiry_Callback,
								new[] {from, killer});
							//}

							if (killer is PlayerMobile)
							{
								((PlayerMobile)killer).ResetKillTime();
							}

							killer.SendLocalizedMessage(1049067); //You have been reported for murder!

							/*Player killerEPL = Player.Find(killer);

							if (killer.Kills == Mobile.MurderCount)
							{
								killer.SendLocalizedMessage(502134); //You are now known as a murderer!
								PlayerState pl = PlayerState.Find(killer);
								Faction factionf = Faction.Find(killer);

								if (pl != null)
								{
									if ((factionf.Definition.FriendlyName == "True Britannians" ||
										 factionf.Definition.FriendlyName == "Council of Mages"))
									{
										pl.Faction.RemoveMember(killer);
										killer.SendMessage("You have been kicked from your faction for being a murderer.");
										LoggingCustom.Log("LOG_FactionPoints.txt", DateTime.Now + "\t" + killer.Name + "\tKicked for murdering.");
									}
								}

								if (killerEPL != null)
								{
									killer.SendMessage("You have been stripped of your life force due to your dastardly actions.");

									killerEPL.Power = 0;

								}
							}
							else */if (Stealing.SuspendOnMurder && killer.Kills == 1 && killer is PlayerMobile &&
									 ((PlayerMobile)killer).NpcGuild == NpcGuild.ThievesGuild)
							{
								killer.SendLocalizedMessage(501562); // You have been suspended by the Thieves Guild.
							}

							/*if ( killer.AccessLevel == AccessLevel.Player ) // Can't put bounties on staff.
								from.SendGump( new BountyGump(from, killer) );*/
						}
					}
					break;
				case 2:
					break;
			}

			m_Idx++;

			if (m_Idx < m_Killers.Count)
			{
				from.SendGump(new ReportMurdererGump(from, m_Killers, m_Location, m_Map, m_Idx));
			}
		}
	}
}