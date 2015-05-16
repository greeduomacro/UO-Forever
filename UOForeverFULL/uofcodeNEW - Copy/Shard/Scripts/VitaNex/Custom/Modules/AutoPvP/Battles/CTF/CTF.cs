#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Engines.EventScores;
using Server.Mobiles;
using Server.Network;

using VitaNex.Schedules;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class UOF_CTFBattle : UOF_PvPBattle
	{
		private PollTimer _FlagEffectTimer;

		[CommandProperty(AutoPvP.Access)]
		public virtual int CapsToWin { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual double FlagDamageInc { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual double FlagDamageIncMax { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int FlagCapturePoints { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int FlagReturnPoints { get; set; }

		public UOF_CTFBattle()
		{
			Name = "Capture The Flag";
			Category = "Capture The Flag";
			Description =
				"Capture the enemy flag and return it to your podium to score points!\nDefend your flag from the enemy, you can only capture their flag when your flag is on your podium.";

			CapsToWin = 5;

			AddTeam(NameList.RandomName("daemon"), 1, 1, 0x22);
			AddTeam(NameList.RandomName("daemon"), 1, 1, 0x55);

			Schedule.Info.Months = ScheduleMonths.All;
			Schedule.Info.Days = ScheduleDays.All;
			Schedule.Info.Times = ScheduleTimes.EveryQuarterHour;

			Options.Timing.PreparePeriod = TimeSpan.FromMinutes(5.0);
			Options.Timing.RunningPeriod = TimeSpan.FromMinutes(15.0);
			Options.Timing.EndedPeriod = TimeSpan.FromMinutes(5.0);
		}

		public UOF_CTFBattle(GenericReader reader)
			: base(reader)
		{
			CapsToWin = 5;
		}

		public override bool Validate(Mobile viewer, List<string> errors, bool pop = true)
		{
			if (!base.Validate(viewer, errors, pop) && pop)
			{
				return false;
			}

			if (CapsToWin < 1)
			{
				errors.Add("CapsToWin value must be equal to or greater than 1.");
				errors.Add("[Options] -> [Edit Options]");
			}
			
			if (Teams.Any(t => !(t is UOF_CTFTeam)))
			{
				errors.Add("One or more teams are not of the UOF_CTFTeam type.");
				errors.Add("[Options] -> [View Teams]");

				if (pop)
				{
					return false;
				}
			}

			return true;
		}

		protected override void OnInit()
		{
			_FlagEffectTimer = PollTimer.FromMilliseconds(
				100.0,
				() =>
				Teams.OfType<UOF_CTFTeam>().Where(t => t != null && t.Flag != null).ForEach(t => t.Flag.InvalidateCarryEffect()),
				() => !Deleted && !Hidden && State == PvPBattleState.Running && Teams.Count > 0);

			base.OnInit();
		}

		protected override void RegisterSubCommands()
		{
			base.RegisterSubCommands();

			RegisterSubCommand(
				"scores",
				state =>
				{
					if (state == null || state.Mobile == null || state.Mobile.Deleted)
					{
						return false;
					}

					PlayerMobile pm = state.Mobile;

					if (pm == null || !IsParticipant(pm))
					{
						return false;
					}

					foreach (UOF_CTFTeam team in Teams.OfType<UOF_CTFTeam>())
					{
						pm.SendMessage(team.Color, "[{0}]: {1:#,0} / {2:#,0} flag captures.", team.Name, team.Caps, CapsToWin);
					}

					return true;
				},
				"Displays the current scores for each team.",
				"",
				AccessLevel.Player);

			RegisterSubCommandAlias("scores");
		}

		public override bool CanEndBattle()
		{
			return base.CanEndBattle() ||
				   (State == PvPBattleState.Running && Teams.OfType<UOF_CTFTeam>().Any(t => t.Caps >= CapsToWin));
		}

		public override int CompareTeamRank(PvPTeam a, PvPTeam b)
		{
			return CompareTeamRank(a as UOF_CTFTeam, b as UOF_CTFTeam);
		}

		public virtual int CompareTeamRank(UOF_CTFTeam a, UOF_CTFTeam b)
		{
			int result = 0;

			if (a.CompareNull(b, ref result))
			{
				return result;
			}

			if (a.Deleted && b.Deleted)
			{
				return 0;
			}

			if (a.Deleted)
			{
				return 1;
			}

			if (b.Deleted)
			{
				return -1;
			}

			if (a.Caps > b.Caps)
			{
				return -1;
			}

			if (a.Caps < b.Caps)
			{
				return 1;
			}

			return 0;
		}

		public override bool IsWinningTeam(PvPTeam team)
		{
			var t = team as UOF_CTFTeam;

			if (t == null || t.Deleted)
			{
				return false;
			}

			return t.Caps >= CapsToWin;
		}

		public override void OnTeamWin(PvPTeam team)
		{
			WorldBroadcast("Team {0} has won {1}!", team.Name, Name);

			base.OnTeamWin(team);
		}

		public override bool AddTeam(string name, int minCapacity, int capacity, int color)
		{
			return AddTeam(new UOF_CTFTeam(this, name, minCapacity, capacity, color));
		}

		public override bool AddTeam(PvPTeam team)
		{
			return team != null && !team.Deleted &&
				   (team is UOF_CTFTeam ? base.AddTeam(team) : AddTeam(team.Name, team.MinCapacity, team.MinCapacity, team.Color));
		}

		public virtual void OnFlagDropped(UOF_CTFFlag flag, PlayerMobile attacker, UOF_CTFTeam enemyTeam)
		{
			EnsureStatistics(attacker)["Flags Dropped"]++;

			PlaySound(746);

			LocalBroadcast("[{0}]: {1} has dropped the flag of team {2}!", enemyTeam.Name, attacker.Name, flag.Team.Name);
		}

		public virtual void OnFlagCaptured(UOF_CTFFlag flag, PlayerMobile attacker, UOF_CTFTeam enemyTeam)
		{
			EnsureStatistics(attacker)["Flags Captured"]++;

			if (FlagCapturePoints > 0)
			{
				AwardPoints(attacker, FlagCapturePoints);
			}

			PlaySound(747);

			LocalBroadcast("[{0}]: {1} has captured the flag of team {2}!", enemyTeam.Name, attacker.Name, flag.Team.Name);
			LocalBroadcast("Team {0} now has {1:#,0} / {2:#,0} flag captures!", enemyTeam.Name, enemyTeam.Caps, CapsToWin);
		}

		public virtual void OnFlagStolen(UOF_CTFFlag flag, PlayerMobile attacker, UOF_CTFTeam enemyTeam)
		{
			EnsureStatistics(attacker)["Flags Stolen"]++;

            AwardPoints(attacker, 5);

			PlaySound(748);

			LocalBroadcast("[{0}]: {1} has stolen the flag of team {2}!", enemyTeam.Name, attacker.Name, flag.Team.Name);
		}

		public virtual void OnFlagReturned(UOF_CTFFlag flag, PlayerMobile defender)
		{
			EnsureStatistics(defender)["Flags Returned"]++;

			if (FlagReturnPoints > 0)
			{
				AwardPoints(defender, FlagReturnPoints);
			}

			PlaySound(749);

			LocalBroadcast("[{0}]: {1} has returned the flag of team {0}!", flag.Team.Name, defender.Name);
		}

		public virtual void OnFlagTimeout(UOF_CTFFlag flag)
		{
			PlaySound(749);

			LocalBroadcast("[{0}]: Flag has been returned to the base!", flag.Team.Name);
		}

		protected override void OnDamageAccept(Mobile from, Mobile damaged, ref int damage)
		{
			base.OnDamageAccept(from, damaged, ref damage);

			var flag = damaged.Backpack.FindItemByType<UOF_CTFFlag>();

			if (flag != null)
			{
				damage += (int)(damage * flag.DamageInc);
			}
		}

		public override void AwardTrophies()
		{
			base.AwardTrophies();

			PlayerMobile topwaller = null;
			long topwalls = 0;

			PlayerMobile topdefender = null;
			long topdefends = 0;

			PlayerMobile topassaulter = null;
			long topassaults = 0;

			PlayerMobile topcapper = null;
			long topcaps = 0;

			foreach (KeyValuePair<PlayerMobile, PvPProfileHistoryEntry> kv in StatisticsCache)
			{
				if (kv.Value["Walls Cast"] > topwalls)
				{
					topwalls = kv.Value["Walls Cast"];
					topwaller = kv.Key;
				}

				if (kv.Value["Flags Returned"] > topdefends)
				{
					topdefends = kv.Value["Flags Returned"];
					topdefender = kv.Key;
				}

				if (kv.Value["Flags Stolen"] > topassaults)
				{
					topassaults = kv.Value["Flags Stolen"];
					topassaulter = kv.Key;
				}

				if (kv.Value["Flags Captured"] > topcaps)
				{
					topcaps = kv.Value["Flags Captured"];
					topcapper = kv.Key;
				}
			}

			if (topwaller != null)
			{
				topwaller.SendMessage(54, "You had the top walls cast in the {0}.", Name);
				topwaller.PublicOverheadMessage(MessageType.Label, 54, true, topwaller.Name + ": Top Walls Cast!");

				var bank = topwaller.FindBank(Map.Expansion);

				if (bank != null)
				{
					bank.DropItem(
						new BattlesTrophy(Name + " - Top Walls Cast: " + topwalls, TrophyType.Walls)
						{
							Owner = topwaller
						});

					topwaller.SendMessage(54, "A trophy has been placed in your bankbox.");
				}
			}

			if (topdefender != null)
			{
				topdefender.SendMessage(54, "You had the top flags defended in the {0}.", Name);
				topdefender.PublicOverheadMessage(MessageType.Label, 54, true, topdefender.Name + ": Top Flags Defended!");

				var bank = topdefender.FindBank(Map.Expansion);

				if (bank != null)
				{
					bank.DropItem(
						new BattlesTrophy(Name + " - Top Flags Defended: " + topdefends, TrophyType.FlagDefends)
						{
							Owner = topdefender
						});

					topdefender.SendMessage(54, "A trophy has been placed in your bankbox.");
				}
			}

			if (topassaulter != null)
			{
				topassaulter.SendMessage(54, "You had the top flags assaulted in the {0}.", Name);
				topassaulter.PublicOverheadMessage(MessageType.Label, 54, true, topassaulter.Name + ": Top Flag Assaults!");

				var bank = topassaulter.FindBank(Map.Expansion);

				if (bank != null)
				{
					bank.DropItem(
						new BattlesTrophy(Name + " - Top Flags Assaulted: " + topassaults, TrophyType.FlagAssaults)
						{
							Owner = topassaulter
						});

					topassaulter.SendMessage(54, "A trophy has been placed in your bankbox.");
				}
			}

			if (topcapper != null)
			{
				topcapper.SendMessage(54, "You had the top flags captured in the {0}.", Name);
				topcapper.PublicOverheadMessage(MessageType.Label, 54, true, topcapper.Name + ": Top Flag Captures!");

				var bank = topcapper.FindBank(Map.Expansion);

				if (bank != null)
				{
					bank.DropItem(
						new BattlesTrophy(Name + " - Top Flags Captured: " + topcaps, TrophyType.FlagCaps)
						{
							Owner = topcapper
						});

					topcapper.SendMessage(54, "A trophy has been placed in your bankbox.");
				}
			}
		}

		public override BattleResultsGump CreateResultsGump(PlayerMobile pm)
		{
		    return new CTFBattleResultsGump(pm, StatisticsCache, TeamStats, Winners);
		}

		public override GlobalJoinGump CreateJoinGump(PlayerMobile pm)
		{
			return new GlobalJoinGump(pm, this, 7031);
		}

		protected override void OnDeleted()
		{
			if (_FlagEffectTimer != null)
			{
				_FlagEffectTimer.Stop();
				_FlagEffectTimer = null;
			}

			base.OnDeleted();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(2);

			switch (version)
			{
				case 2:
					{
						writer.Write(FlagDamageInc);
						writer.Write(FlagDamageIncMax);
					}
					goto case 1;
				case 1:
					{
						writer.Write(FlagCapturePoints);
						writer.Write(FlagReturnPoints);
					}
					goto case 0;
				case 0:
					writer.Write(CapsToWin);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
					{
						FlagDamageInc = reader.ReadDouble();
						FlagDamageIncMax = reader.ReadDouble();
					}
					goto case 1;
				case 1:
					{
						FlagCapturePoints = reader.ReadInt();
						FlagReturnPoints = reader.ReadInt();
					}
					goto case 0;
				case 0:
					{
						CapsToWin = reader.ReadInt();

						Type type = typeof(UOF_CTFTeam);

						Teams.Where(t => !t.GetType().IsEqualOrChildOf(type)).ForEach(
							t =>
							{
								Teams.Remove(t);
								AddTeam(t);
							});
					}
					break;
			}
		}
	}
}