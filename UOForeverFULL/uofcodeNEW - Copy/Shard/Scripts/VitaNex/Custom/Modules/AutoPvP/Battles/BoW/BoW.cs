#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Third;

using VitaNex.FX;
using VitaNex.Network;
using VitaNex.Schedules;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class BoWBattle : UOF_PvPBattle
	{
		private static readonly Type[] _NoCarrySpellCasts = new[]
		{typeof(TeleportSpell), typeof(WallOfStoneSpell), typeof(EnergyFieldSpell)};

		private static readonly SkillName[] _NoCarrySkillUses = new[] {SkillName.Hiding, SkillName.Stealth};

		[CommandProperty(AutoPvP.Access)]
		public PvPTeam FirstMaxTeam { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public PvPTeam SecondMaxTeam { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int PointsToWin { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public BoWWinPortal WinGate { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public BoWCrystal Crystal { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public Point3D CrystalLoc { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public BoWBrazier Brazier1 { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public BoWBrazier Brazier2 { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public BoWBrazier Brazier3 { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public BoWBrazier Brazier4 { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public Point3D Brazier1Loc { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public Point3D Brazier2Loc { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public Point3D Brazier3Loc { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public Point3D Brazier4Loc { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool IsFaction { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public double DamageInc { get; set; }

		/*
		protected override void EnsureConstructDefaults()
		{
			base.EnsureConstructDefaults();
		}
		*/

		public BoWBattle()
		{
			Name = "The Battle of Wind";
			Category = "Daily Events";
			Description =
				"Steal the Crystal of Power and hold it to accumulate points.\nThe First team to reach the desired amount wins.";

			IsFaction = false;

			PointsToWin = 400;

			FirstMaxTeam = null;
			SecondMaxTeam = null;

			DamageInc = 0.25;

			AddTeam("Minax", 0, 7, 1645);
			AddTeam("The Shadowlords", 0, 7, 1109);
			AddTeam("The True Britannians", 0, 7, 1254);
			AddTeam("The Council of Mages", 0, 7, 1325);

			Schedule.Info.Months = ScheduleMonths.All;
			Schedule.Info.Days = ScheduleDays.All;
			Schedule.Info.Times = ScheduleTimes.EveryHalfHour;

			Options.Timing.PreparePeriod = TimeSpan.FromMinutes(5.0);
			Options.Timing.RunningPeriod = TimeSpan.FromMinutes(15.0);
			Options.Timing.EndedPeriod = TimeSpan.FromMinutes(5.0);

			Options.Rules.AllowSpawn = true;
			Options.Rules.CanDie = true;
			Options.Rules.CanResurrect = true;
		}

		public BoWBattle(GenericReader reader)
			: base(reader)
		{ }

		public override bool Validate(Mobile viewer, List<string> errors, bool pop = true)
		{
			if (!base.Validate(viewer, errors, pop) && pop)
			{
				return false;
			}

			if (PointsToWin < 1)
			{
				errors.Add("PointsToWin value must be equal to or greater than 1.");
				errors.Add("[Options] -> [Edit Options]");
			}

			if (Teams.Any(t => !(t is BoWTeam)))
			{
				errors.Add("One or more teams are not of the BoWTeam type.");
				errors.Add("[Options] -> [View Teams]");

				if (pop)
				{
					return false;
				}
			}

			return true;
		}

		public override bool CanEndBattle()
		{
			return base.CanEndBattle() ||
				   (State == PvPBattleState.Running && Teams.OfType<BoWTeam>().Any(t => t.Points >= PointsToWin));
		}

		public override GlobalJoinGump CreateJoinGump(PlayerMobile pm)
		{
			return new GlobalJoinGump(pm, this, 7057);
		}

		protected override void OnBattleStarted(DateTime when)
		{
			base.OnBattleStarted(when);

		    ToggleDoors(true);

			if (Crystal == null || Crystal.Deleted)
			{
				if (State != PvPBattleState.Running)
				{
					if (Crystal != null && !Crystal.Deleted)
					{
						Crystal.Delete();
					}

					Crystal = null;
					return;
				}

				Crystal = Crystal != null && !Crystal.Deleted ? Crystal : new BoWCrystal(this);
				Crystal.Carrier = null;
				Crystal.Reset();
			}
			else
			{
				if (Crystal != null)
				{
					Crystal.Delete();
				}

				Crystal = new BoWCrystal(this);
				Crystal.Reset();
			}

			//create new braziers
			if (Brazier1 != null || Brazier2 != null || Brazier3 != null || Brazier4 != null)
			{
				Cleanup();
			}

			Brazier1 = Brazier1 != null && !Brazier1.Deleted ? Brazier1 : new BoWBrazier(this);
			Brazier2 = Brazier2 != null && !Brazier2.Deleted ? Brazier2 : new BoWBrazier(this);
			Brazier3 = Brazier3 != null && !Brazier3.Deleted ? Brazier3 : new BoWBrazier(this);
			Brazier4 = Brazier4 != null && !Brazier4.Deleted ? Brazier4 : new BoWBrazier(this);

			//move them to locations
			Brazier1.MoveToWorld(Brazier1Loc, Map);
			Brazier2.MoveToWorld(Brazier2Loc, Map);
			Brazier3.MoveToWorld(Brazier3Loc, Map);
			Brazier4.MoveToWorld(Brazier4Loc, Map);

			if (WinGate != null)
			{
				WinGate.Delete();
			}
		}

        public override void ToggleDoors(bool secure)
        {
            Doors.RemoveAll(door => door == null || door.Deleted || door.Map != Map);

            Doors.Where(d => (d.Open && CanCloseDoor(d)) || (!d.Open && CanOpenDoor(d))).ForEach(
                door =>
                {
                    door.Open = false;
                    door.Locked = secure;
                });
        }

        public override void CloseDoors(bool secure)
        {
        }

        public override void OpendDoors(bool secure)
        {
        }

		public override BattleResultsGump CreateResultsGump(PlayerMobile pm)
		{
			return new BoWBattleResultsGump(pm, StatisticsCache, TeamStats, Winners);
		}

		public override void AwardTrophies()
		{
			base.AwardTrophies();

			PlayerMobile topwaller = null;
			long topwalls = 0;

			PlayerMobile topassaulter = null;
			long topassaults = 0;

			PlayerMobile topholder = null;
			long topheld = 0;

			PlayerMobile carrierkiller = null;
			long topcarrierkills = 0;

			foreach (KeyValuePair<PlayerMobile, PvPProfileHistoryEntry> kv in StatisticsCache)
			{
				if (kv.Value["Walls Cast"] > topwalls)
				{
					topwalls = kv.Value["Walls Cast"];
					topwaller = kv.Key;
				}

				if (kv.Value["Crystal Points"] > topheld)
				{
                    topheld = kv.Value["Crystal Points"];
					topholder = kv.Key;
				}

				if (kv.Value["Crystal Steals"] > topassaults)
				{
					topassaults = kv.Value["Crystal Steals"];
					topassaulter = kv.Key;
				}

				if (kv.Value["Crystal Carrier Kills"] > topcarrierkills)
				{
					topcarrierkills = kv.Value["Crystal Carrier Kills"];
					carrierkiller = kv.Key;
				}
			}

			if (topwaller != null)
			{
				topwaller.SendMessage(54, "You had the top walls cast in the {0}.", Name);
				topwaller.PublicOverheadMessage(MessageType.Label, 54, true, topwaller.Name + ": Top Walls Cast!");

				BankBox bank = topwaller.FindBank(Map.Expansion);

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

			if (topholder != null)
			{
				topholder.SendMessage(54, "You had the top crystal points in the {0}.", Name);
				topholder.PublicOverheadMessage(MessageType.Label, 54, true, topholder.Name + ": Top Crystal Points!");

				BankBox bank = topholder.FindBank(Map.Expansion);

				if (bank != null)
				{
					bank.DropItem(
						new BattlesTrophy(Name + " - Top Crystal Points: " + topheld, TrophyType.CrystalTime)
						{
							Owner = topholder
						});

					topholder.SendMessage(54, "A trophy has been placed in your bankbox.");
				}
			}

			if (topassaulter != null)
			{
				topassaulter.SendMessage(54, "You had the most crystal assaults in the {0}.", Name);
				topassaulter.PublicOverheadMessage(MessageType.Label, 54, true, topassaulter.Name + ": Top Crystal Assaults!");

				BankBox bank = topassaulter.FindBank(Map.Expansion);

				if (bank != null)
				{
					bank.DropItem(
						new BattlesTrophy(Name + " - Top Crystals Assaulted: " + topassaults, TrophyType.FlagAssaults)
						{
							Owner = topassaulter
						});

					topassaulter.SendMessage(54, "A trophy has been placed in your bankbox.");
				}
			}

			if (carrierkiller != null)
			{
				carrierkiller.SendMessage(54, "You had the top carrier kills in the {0}.", Name);
				carrierkiller.PublicOverheadMessage(MessageType.Label, 54, true, carrierkiller.Name + ": Top Carrier Kills!");

				BankBox bank = carrierkiller.FindBank(Map.Expansion);

				if (bank != null)
				{
					bank.DropItem(
						new BattlesTrophy(Name + " - Top Crystal Carrier Kills: " + topcarrierkills, TrophyType.FlagCaps)
						{
							Owner = carrierkiller
						});

					carrierkiller.SendMessage(54, "A trophy has been placed in your bankbox.");
				}
			}
		}

		public override int CompareTeamRank(PvPTeam a, PvPTeam b)
		{
			return CompareTeamRank(a as BoWTeam, b as BoWTeam);
		}

		public virtual int CompareTeamRank(BoWTeam a, BoWTeam b)
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

			if (a.Points > b.Points)
			{
				return -1;
			}

			if (a.Points < b.Points)
			{
				return 1;
			}

			return 0;
		}

		public override bool IsWinningTeam(PvPTeam team)
		{
			var t = team as BoWTeam;

			if (t == null || t.Deleted)
			{
				return false;
			}

			return t.Points >= PointsToWin;
		}

		public override void OnTeamWin(PvPTeam team)
		{
			WorldBroadcast("{0} has won the Battle of Wind!", team.Name);

			var t = team as BoWTeam;

			if (t == null)
			{
				return;
			}

			/*if (IsFaction)
			{
				if (WinGate != null)
				{
					WinGate.Delete();
				}

				WinGate = new BoWWinPortal(this)
				{
					Controller = t.Name,
					Hue = t.Color
				};

				WinGate.MoveToWorld(t.GatePoint, Map);
			}*/

			if (Crystal != null)
			{
				Crystal.Carrier = null;

				//For the lols
				if (!MyPeopleNeedMe())
				{
					//Aesthetic stuff for crystal
					Crystal.Hue = t.Color;

					if (t.Name == "Minax" || t.Name == "The Shadowlords")
					{
						Crystal.Name = "the corrupted Crystal of Power";
					}

					if (t.Name == "The True Britannians" || t.Name == "The Council of Mages")
					{
						Crystal.Name = "the purified Crystal of Power";
					}

					if (Crystal == null || Crystal.Deleted)
					{
						Crystal = new BoWCrystal(this);
					}

					Crystal.MoveToWorld(t.CrystalLoc, Map);
				}
			}

			Cleanup();

			base.OnTeamWin(team);
		}

		private bool MyPeopleNeedMe()
		{
			if (IsFaction || Crystal == null || Crystal.Deleted || !Crystal.Visible || !Crystal.Movable || Utility.RandomBool())
			{
				return false;
			}

			Crystal.PublicOverheadMessage(MessageType.Regular, 1287, false, "I MUST GO, MY PEOPLE NEED ME.");

			Point3D loc = Crystal.GetWorldLocation();

			// (Lee) The reason for using effects is efficiency,
			// since whenever the physical item's location (Z) is changed, 
			// it sends a bunch of update packets, I know it's trivial, 
			// but i thought you might like this example of usage of the VNc effects :)

			// Create a new deferred effect queue.
			// When the queue has finished processing, delete the crystal.
			var q = new EffectQueue(
				() =>
				{
					if (Crystal != null)
					{
						Crystal.Delete();
					}
				});

			// Add 10 effects to the queue,
			// each with a duration of 10 (default value),
			// which translates to 1000 milliseconds (10 * 100).
			// Since they are not moving effects and each effect lasts for 1 second,
			// it should produce the same illusion of the original implementation design.
			for (int i = 0; i < 10; i++)
			{
				q.Add(
					new EffectInfo(
						loc.Clone3D(0, 0, i), Crystal.Map, Crystal.ItemID, Crystal.Hue, 10, 10, EffectRender.Normal, TimeSpan.Zero));
			}

			// Process the effect queue after waiting 5 seconds.
			Timer.DelayCall(
				TimeSpan.FromSeconds(5.0),
				() =>
				{
					// Anything can happen in 5 seconds, so check it all again.
					if (Crystal != null && !Crystal.Deleted && Crystal.Visible && Crystal.Movable && !IsFaction)
					{
						// Hiding the crystal so the effects can handle the visuals
						Crystal.Visible = false;

						// Just in case anyone wants to try to mess with it, even when it's hidden.
						Crystal.Movable = false;

						q.Process();
					}

					// We don't need the queue, so explicitly dispose for efficiency (optional)
					q.Dispose();
				});

			return true;
		}

		public void Cleanup()
		{
			if (Brazier1 != null)
			{
				Brazier1.Delete();
				Brazier1 = null;
			}

			if (Brazier2 != null)
			{
				Brazier2.Delete();
				Brazier2 = null;
			}

			if (Brazier3 != null)
			{
				Brazier3.Delete();
				Brazier3 = null;
			}

			if (Brazier4 != null)
			{
				Brazier4.Delete();
				Brazier4 = null;
			}

			FirstMaxTeam = null;
			SecondMaxTeam = null;
		}

		public override void Enqueue(PlayerMobile pm, PvPTeam team = null, bool party = true)
		{
			if (IsFaction && pm != null && !pm.Deleted)
			{
				string faction = null;

				switch (pm.FactionName.ToUpper())
				{
					case "LORD BRITISH":
						faction = "The True Britannians";
						break;
					case "MINAX":
						faction = "Minax";
						break;
					case "SHADOWLORDS":
						faction = "The Shadowlords";
						break;
					case "COUNCIL OF MAGES":
						faction = "The Council of Mages";
						break;
				}

				if (team != null && faction != null && team.Name != null && faction != team.Name)
				{
					team = Teams.Find(x => x.Name == faction);
				}
				else if (team == null && faction != null)
				{
					team = Teams.Find(x => x.Name == faction);
				}
			}

			base.Enqueue(pm, team, party);
		}

		public override bool CanQueue(PlayerMobile pm)
		{
			if (!base.CanQueue(pm))
			{
				return false;
			}

			if (!IsFaction)
			{
				return true;
			}

			if (String.IsNullOrWhiteSpace(pm.FactionName))
			{
				pm.SendMessage(0x22, "You must be in a faction to join this event!");
				return false;
			}

			return true;
		}

		public override bool AddTeam(string name, int minCapacity, int capacity, int color)
		{
			return AddTeam(new BoWTeam(this, name, minCapacity, capacity, color));
		}

		public override bool AddTeam(PvPTeam team)
		{
			return team != null && !team.Deleted &&
				   (team is BoWTeam ? base.AddTeam(team) : AddTeam(team.Name, team.MinCapacity, team.MinCapacity, team.Color));
		}

		public override void OnDeath(Mobile m)
		{
			base.OnDeath(m);

			if (m == null || m.Deleted || !(m is PlayerMobile) || Crystal == null || Crystal.Deleted || Crystal.Carrier != m)
			{
				return;
			}

			var k = m.GetLastKiller<PlayerMobile>(true);

			if (k == null)
			{
				return;
			}

			if (IsParticipant(k))
			{
				EnsureStatistics(k)["Crystal Carrier Kills"]++;
			}
		}

		public override bool CheckSkillUse(Mobile user, int skill)
		{
			if (!base.CheckSkillUse(user, skill))
			{
				return false;
			}

			if (Crystal != null && !Crystal.Deleted && Crystal.Carrier == user && skill >= 0 && _NoCarrySkillUses.Contains((SkillName)skill))
			{
				return false;
			}

			return true;
		}

		protected override void OnSkillUseDeny(Mobile user, int skill)
		{
			if (Crystal != null && !Crystal.Deleted && Crystal.Carrier == user && skill >= 0)
			{
				var sn = (SkillName)skill;

				if (_NoCarrySkillUses.Contains(sn))
				{
					string name = sn.ToString().SpaceWords();

					user.SendMessage(33, "You cannot use {0} while holding the Crystal of Power!", name);
					return;
				}
			}

			base.OnSkillUseDeny(user, skill);
		}

		public override bool CheckSpellCast(Mobile caster, ISpell spell)
		{
			if (!base.CheckSpellCast(caster, spell))
			{
				return false;
			}

			if (Crystal != null && !Crystal.Deleted && Crystal.Carrier == caster && spell != null && _NoCarrySpellCasts.Contains(spell.GetType()))
			{
				return false;
			}

			return true;
		}

		protected override void OnSpellCastDeny(Mobile caster, ISpell spell)
		{
			if (Crystal != null && !Crystal.Deleted && Crystal.Carrier == caster && spell != null)
			{
				Type st = spell.GetType();

				if (_NoCarrySpellCasts.Contains(st))
				{
					string name = spell is Spell ? ((Spell)spell).Name : st.Name.Replace("Spell", String.Empty).SpaceWords();

					caster.SendMessage(33, "You cannot cast {0} while holding the Crystal of Power!", name);
					return;
				}
			}

			base.OnSpellCastDeny(caster, spell);
		}

		public virtual void OnStolen(PlayerMobile pm, BoWTeam captureteam)
		{
			PlaySound(748);

			if (captureteam != null && !captureteam.Deleted && State == PvPBattleState.Running)
			{
				LocalBroadcast("{0} have stolen the Crystal of Power!", captureteam.Name);
			}
		}

		public virtual void OnDrop(BoWTeam captureteam)
		{
			PlaySound(748);

			if (captureteam != null && !captureteam.Deleted && State == PvPBattleState.Running)
			{
				LocalBroadcast("{0} have lost the Crystal of Power.", captureteam.Name);
			}
		}

		public override void OnExit(PvPRegion region, Mobile m)
		{
			base.OnExit(region, m);

			if (!(region is PvPBattleRegion))
			{
				return;
			}

			if (Crystal != null && !Crystal.Deleted && Crystal.Carrier == m)
			{
				Crystal.Reset();
			}
		}

		protected override void OnEjected(PlayerMobile pm)
		{
			base.OnEjected(pm);

			if (Crystal != null && !Crystal.Deleted && Crystal.Carrier == pm)
			{
				Crystal.Reset();
			}
		}

		public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
		{
			if (!base.OnMoveInto(m, d, newLocation, oldLocation))
			{
				return false;
			}

			if (State != PvPBattleState.Running)
			{
				return true;
			}

			var pm = m as PlayerMobile;

			return pm == null || IsParticipant(pm);
		}

		protected override void OnBattleEnded(DateTime when)
		{
			base.OnBattleEnded(when);

            ToggleDoors(false);

			Cleanup();
		}

		protected override void OnBattleCancelled(DateTime when)
		{
			base.OnBattleCancelled(when);

            ToggleDoors(false);

			Cleanup();
		}

		public override bool CheckAllowHarmful(Mobile from, Mobile target)
		{
			if (!base.CheckAllowHarmful(from, target))
			{
				return false;
			}

			if (Crystal != null && !Crystal.Deleted && from is PlayerMobile && target is PlayerMobile)
			{
				var fromP = (PlayerMobile)from;
				var targetP = (PlayerMobile)target;

				if (IsParticipant(fromP) && IsParticipant(targetP) && Crystal.Carrier == fromP)
				{
					return false;
				}
			}

			return true;
		}

		protected override void OnAllowHarmfulDeny(Mobile from, Mobile target)
		{
			if (from == null || from.Deleted || target == null || target.Deleted || from == target)
			{
				return;
			}

			if (Crystal != null && !Crystal.Deleted && Crystal.Carrier == from)
			{
				from.SendMessage("You can not perform harmful actions while carrying the Crystal of Power.");
			}

			base.OnAllowHarmfulDeny(from, target);
		}

		protected override void OnDamageAccept(Mobile from, Mobile damaged, ref int damage)
		{
			base.OnDamageAccept(from, damaged, ref damage);

			if (damaged == null || damaged.Deleted)
			{
				return;
			}

			if (Crystal != null && !Crystal.Deleted && Crystal.Carrier == damaged)
			{
				damage += (int)(damage * DamageInc);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(7);

			switch (version)
			{
				case 7:
					writer.Write(DamageInc);
					goto case 6;
				case 6:
					writer.Write(GiveTrophies);
					goto case 5;
				case 5:
					writer.Write(BattleWell);
					goto case 4;
				case 4:
					writer.Write(IsFaction);
					goto case 3;
				case 3:
					{
						writer.Write(Brazier1);
						writer.Write(Brazier2);
						writer.Write(Brazier3);
						writer.Write(Brazier4);
					}
					goto case 2;
				case 2:
					{
						writer.Write(Brazier1Loc);
						writer.Write(Brazier2Loc);
						writer.Write(Brazier3Loc);
						writer.Write(Brazier4Loc);
						writer.Write(CrystalLoc);
					}
					goto case 1;
				case 1:
					{
						writer.Write(Crystal);
						writer.Write(WinGate);
					}
					goto case 0;
				case 0:
					writer.Write(PointsToWin);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 7:
					DamageInc = reader.ReadDouble();
					goto case 6;
				case 6:
					GiveTrophies = reader.ReadBool();
					goto case 5;
				case 5:
					BattleWell = reader.ReadItem<BattleWell>();
					goto case 4;
				case 4:
					IsFaction = reader.ReadBool();
					goto case 3;
				case 3:
					{
						Brazier1 = reader.ReadItem<BoWBrazier>();
						Brazier2 = reader.ReadItem<BoWBrazier>();
						Brazier3 = reader.ReadItem<BoWBrazier>();
						Brazier4 = reader.ReadItem<BoWBrazier>();
					}
					goto case 2;
				case 2:
					{
						Brazier1Loc = reader.ReadPoint3D();
						Brazier2Loc = reader.ReadPoint3D();
						Brazier3Loc = reader.ReadPoint3D();
						Brazier4Loc = reader.ReadPoint3D();
						CrystalLoc = reader.ReadPoint3D();
					}
					goto case 1;
				case 1:
					{
						Crystal = reader.ReadItem<BoWCrystal>();
						WinGate = reader.ReadItem<BoWWinPortal>();
					}
					goto case 0;
				case 0:
						PointsToWin = reader.ReadInt();
					break;
			}

			Teams.Where(t => !t.GetType().IsEqualOrChildOf<BoWTeam>()).ForEach(
				t =>
				{
					Teams.Remove(t);
					AddTeam(t);
				});
		}
	}

	#region Crystal Arrow
	public class BoWArrow : QuestArrow
	{
		private readonly bool _Closable;
		private readonly Timer _Timer;
		private Mobile _Owner;

		public BoWArrow(Mobile owner, IEntity target, int range, bool closable)
			: base(owner, target)
		{
			_Owner = owner;

			_Timer = new BoWArrowTimer(owner, target, range, this);
			_Timer.Start();

			_Closable = closable;
		}

		public override void OnClick(bool rightClick)
		{
			if (!_Closable || !rightClick)
			{
				return;
			}

			_Owner = null;

			Stop();
		}

		public override void OnStop()
		{
			_Timer.Stop();

			if (_Owner != null)
			{ }
		}
	}

	public class BoWArrowTimer : Timer
	{
		private readonly QuestArrow _Arrow;
		private readonly Mobile _Owner;
		private readonly int _Range;
		private readonly IEntity _Target;
		private int _LastX, _LastY;

		public BoWArrowTimer(Mobile from, IEntity target, int range, QuestArrow arrow)
			: base(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(2.5))
		{
			_Owner = from;
			_Target = target;
			_Range = range;

			_Arrow = arrow;
		}

		protected override void OnTick()
		{
			if (!_Arrow.Running)
			{
				Stop();
				return;
			}

			if (_Owner.NetState == null || _Owner.Deleted || _Target.Deleted || _Owner.Map != _Target.Map ||
				(_Range != -1 && !_Owner.InRange(_Target, _Range)) ||
				(_Target is Mobile && ((Mobile)_Target).Hidden && ((Mobile)_Target).AccessLevel > _Owner.AccessLevel))
			{
				_Arrow.Stop();
				Stop();
				return;
			}

			if (_LastX == _Target.X && _LastY == _Target.Y)
			{
				return;
			}

			_LastX = _Target.X;
			_LastY = _Target.Y;

			_Arrow.Update();
		}
	}
	#endregion
}