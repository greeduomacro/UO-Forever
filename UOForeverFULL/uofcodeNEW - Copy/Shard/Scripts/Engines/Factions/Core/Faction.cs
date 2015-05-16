#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Accounting;
using Server.Commands;
using Server.Commands.Generic;
using Server.Engines.CannedEvil;
using Server.Engines.ConPVP;
using Server.Engines.Conquests;
using Server.Ethics;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;
using Server.Prompts;
using Server.Regions;
using Server.Targeting;

using VitaNex.Modules.AutoPvP;
#endregion

namespace Server.Factions
{
	[CustomEnum(new[] {"Minax", "Council of Mages", "True Britannians", "Shadowlords"})]
	public abstract class Faction : IComparable
	{
		private static readonly bool m_Enabled = true;
		public static bool Enabled { get { return m_Enabled; } }

		public int ZeroRankOffset;

		private FactionDefinition m_Definition;
		private FactionState m_State;
		private StrongholdRegion m_StrongholdRegion;

		public StrongholdRegion StrongholdRegion { get { return m_StrongholdRegion; } set { m_StrongholdRegion = value; } }

		public FactionDefinition Definition
		{
			get { return m_Definition; }
			set
			{
				m_Definition = value;
				m_StrongholdRegion = new StrongholdRegion(this);
			}
		}

		public FactionState State { get { return m_State; } set { m_State = value; } }

		public Election Election { get { return m_State.Election; } set { m_State.Election = value; } }

		public Mobile Commander { get { return m_State.Commander; } set { m_State.Commander = value; } }

		public int Tithe { get { return m_State.Tithe; } set { m_State.Tithe = value; } }
		public int Silver { get { return m_State.Silver; } set { m_State.Silver = value; } }

		public List<PlayerState> Members { get { return m_State.Members; } set { m_State.Members = value; } }

		public int OwnedTowns { get { return Town.Towns.Count(town => town != null && town.Owner == this); } }

		public static readonly TimeSpan LeavePeriod = TimeSpan.FromDays(1.0);

		public bool FactionMessageReady { get { return m_State.FactionMessageReady; } }

		public void Broadcast(int number)
		{
			Broadcast(0x3B2, number);
		}

		public void Broadcast(int hue, int number)
		{
			foreach (PlayerState s in Members.Where(s => s != null && s.Mobile != null))
			{
				s.Mobile.SendLocalizedMessage(number, hue);
			}
		}

		public void Broadcast(string format, params object[] args)
		{
			Broadcast(0x3B2, format, args);
		}

		public void Broadcast(int hue, string format, params object[] args)
		{
			string text = String.Format(format, args);

			foreach (PlayerState s in Members.Where(s => s != null && s.Mobile != null))
			{
				s.Mobile.SendMessage(hue, text);
			}
		}

		public void BeginBroadcast(Mobile from)
		{
			from.SendLocalizedMessage(1010265); // Enter Faction Message
			from.Prompt = new BroadcastPrompt(this);
		}

		public void EndBroadcast(Mobile from, string text)
		{
			if (from.AccessLevel == AccessLevel.Player)
			{
				m_State.RegisterBroadcast();
			}

			Broadcast(Definition.HueBroadcast, "{0} [Commander] {1} : {2}", from.Name, Definition.FriendlyName, text);
		}

		private class BroadcastPrompt : Prompt
		{
			private readonly Faction m_Faction;

			public BroadcastPrompt(Faction faction)
			{
				m_Faction = faction;
			}

			public override void OnResponse(Mobile from, string text)
			{
				m_Faction.EndBroadcast(from, text);
			}
		}

		public static void HandleAtrophy()
		{
			if (Factions.Any(f => !f.State.IsAtrophyReady))
			{
				return;
			}

			var activePlayers = new List<PlayerState>();

			foreach (Faction f in Factions)
			{
				activePlayers.AddRange(f.Members.Where(ps => ps.KillPoints > 0 && ps.IsActive));
			}

			int distrib = Factions.Sum(f => f.State.CheckAtrophy());

			if (activePlayers.Count == 0)
			{
				return;
			}

			for (int i = 0; i < distrib; ++i)
			{
				activePlayers[Utility.Random(activePlayers.Count)].KillPoints++;
			}
		}

		public static void DistributePoints(int distrib)
		{
			var activePlayers = new List<PlayerState>();

			foreach (Faction f in Factions)
			{
				activePlayers.AddRange(f.Members.Where(ps => ps.KillPoints > 0 && ps.IsActive));
			}

			if (activePlayers.Count == 0)
			{
				return;
			}

			for (int i = 0; i < distrib; ++i)
			{
				activePlayers[Utility.Random(activePlayers.Count)].KillPoints++;
			}
		}

		public void BeginHonorLeadership(Mobile from)
		{
			from.SendLocalizedMessage(502090); // Click on the player whom you wish to honor.
			from.BeginTarget(12, false, TargetFlags.None, HonorLeadership_OnTarget);
		}

		public void HonorLeadership_OnTarget(Mobile from, object obj)
		{
			if (!(obj is Mobile))
			{
				from.SendLocalizedMessage(1042496); // You may only honor another player.
				return;
			}

			var recv = (Mobile)obj;

			PlayerState giveState = PlayerState.Find(@from);
			PlayerState recvState = PlayerState.Find(recv);

			if (giveState == null)
			{
				return;
			}

			if (recvState == null || recvState.Faction != giveState.Faction)
			{
				from.SendLocalizedMessage(1042497); // Only faction mates can be honored this way.
			}
			else if (giveState.KillPoints < 5)
			{
				from.SendLocalizedMessage(1042499); // You must have at least five kill points to honor them.
			}
			else
			{
				recvState.LastHonorTime = DateTime.UtcNow;
				giveState.KillPoints -= 5;
				recvState.KillPoints += 4;

				from.SendMessage(
					38,
					"Be warned that faction members who don't kill enemy faction members for a long time may be booted from their faction!");

				// TODO: Confirm no message sent to giver
				recv.SendLocalizedMessage(1042500); // You have been honored with four kill points.
				recv.SendMessage(
					38,
					"Be warned that faction members who don't kill enemy faction members for a long time may be booted from their faction!");
			}
		}

		public virtual void AddMember(Mobile mob)
		{
			Members.Insert(ZeroRankOffset, new PlayerState(mob, this, Members));

			mob.AddToBackpack(FactionItem.Imbue(new HoodedShroudOfShadows(), this, false, Definition.HuePrimary));
			mob.SendLocalizedMessage(1010374); // You have been granted a robe which signifies your faction

			mob.InvalidateProperties();
			mob.Delta(MobileDelta.Noto);

			mob.FixedEffect(0x373A, 10, 30);
			mob.PlaySound(0x209);
		}

		public static bool IsNearType(Mobile mob, Type type, int range)
		{
			bool mobs = type.IsSubclassOf(typeof(Mobile));
			bool items = type.IsSubclassOf(typeof(Item));

			IPooledEnumerable eable;

			if (mobs)
			{
				eable = mob.GetMobilesInRange(range);
			}
			else if (items)
			{
				eable = mob.GetItemsInRange(range);
			}
			else
			{
				return false;
			}

			if (eable.OfType<IEntity>().Any(e => e.TypeEquals(type)))
			{
				eable.Free();
				return true;
			}

			eable.Free();
			return false;
		}

		public static bool IsNearType(Mobile mob, Type[] types, int range)
		{
			IPooledEnumerable eable = mob.GetObjectsInRange(range);

			if (eable.OfType<IEntity>().Any(e => types.Any(t => e.TypeEquals(t))))
			{
				eable.Free();
				return true;
			}

			eable.Free();
			return false;
		}

		public void RemovePlayerState(PlayerState pl)
		{
			if (pl == null || !Members.Contains(pl))
			{
				return;
			}

			int killPoints = pl.KillPoints;

			if (pl.RankIndex != -1)
			{
				while ((pl.RankIndex + 1) < ZeroRankOffset)
				{
					PlayerState pNext = Members[pl.RankIndex + 1];
					Members[pl.RankIndex + 1] = pl;
					Members[pl.RankIndex] = pNext;
					pl.RankIndex++;
					pNext.RankIndex--;
				}

				ZeroRankOffset--;
			}

			Members.Remove(pl);

			var pm = (PlayerMobile)pl.Mobile;
			if (pm == null)
			{
				return;
			}

			Mobile mob = pl.Mobile;
			if (pm.FactionPlayerState == pl)
			{
				pm.FactionPlayerState = null;

				mob.InvalidateProperties();
				mob.Delta(MobileDelta.Noto);

				if (Election.IsCandidate(mob))
				{
					Election.RemoveCandidate(mob);
				}

				if (pl.Finance != null)
				{
					pl.Finance.Finance = null;
				}

				if (pl.Sheriff != null)
				{
					pl.Sheriff.Sheriff = null;
				}

				Election.RemoveVoter(mob);

				if (Commander == mob)
				{
					Commander = null;
				}

				pm.ValidateEquipment();
			}

			if (killPoints > 0)
			{
				LoggingCustom.Log(
					"LOG_FactionPoints.txt",
					DateTime.Now + "\t" + pm.Name + "\tFaction.RemovePlayerState: DistributePoints\t" + killPoints);
				DistributePoints(killPoints);
			}
		}

		public void RemoveMember(Mobile mob)
		{
			PlayerState pl = PlayerState.Find(mob);

			if (pl == null || !Members.Contains(pl))
			{
				return;
			}

			int killPoints = pl.KillPoints;

			if (mob.Backpack != null)
			{
				//Ordinarily, through normal faction removal, this will never find any sigils.
				//Only with a leave delay less than the ReturnPeriod or a Faction Kick/Ban, will this ever do anything
				Item[] sigils = mob.Backpack.FindItemsByType(typeof(Sigil));

				foreach (Sigil s in sigils.OfType<Sigil>())
				{
					s.ReturnHome();
				}
			}

			if (pl.RankIndex != -1)
			{
				while ((pl.RankIndex + 1) < ZeroRankOffset)
				{
					PlayerState pNext = Members[pl.RankIndex + 1];
					Members[pl.RankIndex + 1] = pl;
					Members[pl.RankIndex] = pNext;
					pl.RankIndex++;
					pNext.RankIndex--;
				}

				ZeroRankOffset--;
			}

			Members.Remove(pl);

			if (mob is PlayerMobile)
			{
				((PlayerMobile)mob).FactionPlayerState = null;
			}

			mob.InvalidateProperties();
			mob.Delta(MobileDelta.Noto);

			if (Election.IsCandidate(mob))
			{
				Election.RemoveCandidate(mob);
			}

			Election.RemoveVoter(mob);

			if (pl.Finance != null)
			{
				pl.Finance.Finance = null;
			}

			if (pl.Sheriff != null)
			{
				pl.Sheriff.Sheriff = null;
			}

			if (Commander == mob)
			{
				Commander = null;
			}

			if (mob is PlayerMobile)
			{
				((PlayerMobile)mob).ValidateEquipment();
			}

			if (killPoints > 0)
			{
				LoggingCustom.Log(
					"LOG_FactionPoints.txt", DateTime.Now + "\t" + mob.Name + "\tFaction.RemoveMember: DistributePoints\t" + killPoints);
				DistributePoints(killPoints);
			}

			Player epl = Player.Find(mob);
			if (Ethic.Enabled && epl != null)
			{
				epl.Detach();
			}
		}

		public void JoinGuilded(PlayerMobile mob, Guild guild)
		{
			Faction faction = this;
			if (mob.Young)
			{
				guild.RemoveMember(mob);
				mob.SendLocalizedMessage(1042283);
				// You have been kicked out of your guild!  Young players may not remain in a guild which is allied with a faction.
			}
			else if (AlreadyHasCharInFaction(mob, faction))
			{
				guild.RemoveMember(mob);
				mob.SendLocalizedMessage(1005281); // You have been kicked out of your guild due to factional overlap
			}
			else if (IsFactionBanned(mob))
			{
				guild.RemoveMember(mob);
				mob.SendLocalizedMessage(1005052); // You are currently banned from the faction system
			}
			else if (mob.SkillsTotal < 7000)
			{
				guild.RemoveMember(mob);
				mob.SendMessage("You are not skilled enough to join a faction.");
			}
			else
			{
				AddMember(mob);
				Player pl = Player.Find(mob);
				if (Ethic.Enabled && pl == null)
				{
					switch (m_Definition.FriendlyName)
					{
						case "Shadowlords":
						case "Minax":
							pl = new Player(Ethic.Evil, mob);
							break;
						case "Council of Mages":
						case "True Britannians":
							pl = new Player(Ethic.Hero, mob);
							break;
					}
					if (pl != null)
					{
						pl.Attach();
					}
				}
				mob.SendLocalizedMessage(1042756, true, " " + m_Definition.FriendlyName);
				// You are now joining a faction:
			}
		}

		public void JoinAlone(Mobile mob)
		{
			AddMember(mob);

			Player pl = Player.Find(mob);
			if (Ethic.Enabled && pl == null)
			{
				switch (m_Definition.FriendlyName)
				{
					case "Shadowlords":
					case "Minax":
						pl = new Player(Ethic.Evil, mob);
						break;
					case "Council of Mages":
					case "True Britannians":
						pl = new Player(Ethic.Hero, mob);
						break;
				}
				if (pl != null)
				{
					pl.Attach();
				}
			}
			mob.SendLocalizedMessage(1005058); // You have joined the faction

			if (mob is PlayerMobile)
			{
				Conquests.CheckProgress<FactionStateConquest>(mob as PlayerMobile, this);
			}
		}

		public static Faction FindFactionOnAccount(Mobile mob)
		{
			var acct = mob.Account as Account;

			if (acct != null)
			{
				for (int i = 0; i < acct.Length; ++i)
				{
					Mobile c = acct[i];
					Faction f = Find(c);

					if (f != null)
					{
						return f;
					}
				}
			}

			return null;
		}

		public static bool AlreadyHasCharInFaction(Mobile mob, Faction faction)
		{
			var acct = mob.Account as Account;

			if (acct != null)
			{
				for (int i = 0; i < acct.Length; ++i)
				{
					Mobile c = acct[i];
					if (Find(c) != null)
					{
						if (((PlayerMobile)c).FactionPlayerState.Faction != faction)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public static bool IsFactionBanned(Mobile mob)
		{
			var acct = mob.Account as Account;

			if (acct == null)
			{
				return false;
			}

			return acct.GetTag("FactionBanned") != null;
		}

		public bool HasEnoughSkills(Mobile mob)
		{
			int totalStats = mob.RawStr + mob.RawDex + mob.RawInt;
			int gms = mob.Skills.Count(t => t.Base >= 100.0);

			return gms >= 5 && totalStats >= 185;
		}

		public void OnJoinAccepted(Mobile mob)
		{
			var pm = mob as PlayerMobile;
			Faction faction = this;

			if (pm == null)
			{
				return; // sanity
			}

			PlayerState pl = PlayerState.Find(pm);

			if (pm.Young)
			{
				pm.SendLocalizedMessage(1010104); // You cannot join a faction as a young player
			}
			else if (pl != null && pl.IsLeaving)
			{
				pm.SendLocalizedMessage(1005051);
				// You cannot use the faction stone until you have finished quitting your current faction
			}
			else if (AlreadyHasCharInFaction(pm, faction))
			{
				pm.SendLocalizedMessage(1005059);
				// You cannot join a faction because you already declared your allegiance with another character
			}
			else if (IsFactionBanned(mob))
			{
				pm.SendLocalizedMessage(1005052); // You are currently banned from the faction system
			}
				//else if ( mob.SkillsTotal < 7000 )
				//	pm.SendMessage ("You are not skilled enough to join a faction.");
			else if (pm.Guild is Guild)
			{
				var guild = (Guild)pm.Guild;

				if (guild.Leader != pm)
				{
					// ALAN MOD: allow guild members to join the same faction if the guildmaster is in that faction or if the guildmaster is null
					var leader = guild.Leader as PlayerMobile;

					if (leader == null)
					{
						if (HasEnoughSkills(pm))
						{
							JoinAlone(pm);
						}
						else
						{
							pm.SendMessage("You do not meet the skill or stat requirements to join a faction.");
						}
					}
					else if (leader.FactionPlayerState != null && leader.FactionPlayerState.Faction == this)
					{
						if (HasEnoughSkills(pm))
						{
							JoinAlone(pm);
						}
						else
						{
							pm.SendMessage("You do not meet the skill or stat requirements to join a faction.");
						}
					}
					else
					{
						pm.SendMessage(
							38,
							"You cannot join a faction that your guildmaster is not already in. Either resign from your guild or join the same faction that your guild is in!");
						//(1005057); // You cannot join a faction because you are in a guild and not the guildmaster
					}
				}
				else if (guild.Type != GuildType.Regular && !Guild.NewGuildSystem)
				{
					pm.SendLocalizedMessage(1042161);
					// You cannot join a faction because your guild is an Order or Chaos type.
				}
				else if (!Guild.NewGuildSystem && guild.Enemies != null && guild.Enemies.Count > 0)
					//CAN join w/wars in new system
				{
					pm.SendLocalizedMessage(1005056); // You cannot join a faction with active Wars
				}
				else if (Guild.NewGuildSystem && guild.Alliance != null)
				{
					pm.SendLocalizedMessage(1080454);
					// Your guild cannot join a faction while in alliance with non-factioned guilds.
				}
				else if (!CanHandleInflux(guild.Members.Count))
				{
					pm.SendLocalizedMessage(1018031);
					// In the interest of faction stability, this faction declines to accept new members for now.
				}
				else
				{
					var members = new List<Mobile>(guild.Members);

					if (members.OfType<PlayerMobile>().Any(member => !HasEnoughSkills(member)))
					{
						pm.SendMessage("One or more of your guildmates does not meet the skill or stat requirements to join a faction.");
						return;
					}

					foreach (PlayerMobile member in members.OfType<PlayerMobile>())
					{
						JoinGuilded(member, guild);
					}
				}
			}
			else if (!CanHandleInflux(1))
			{
				pm.SendLocalizedMessage(1018031);
				// In the interest of faction stability, this faction declines to accept new members for now.
			}
			else if (HasEnoughSkills(mob))
			{
				JoinAlone(mob);
			}
			else
			{
				pm.SendMessage("You do not meet the skill or stat requirements to join a faction.");
			}
		}

		public bool IsCommander(Mobile mob)
		{
			return mob != null && (mob.AccessLevel >= AccessLevel.GameMaster || mob == Commander);
		}

		public Faction()
		{
			m_State = new FactionState(this);
		}

		public override string ToString()
		{
			return m_Definition.FriendlyName;
		}

		public int CompareTo(object obj)
		{
			return m_Definition.Sort - ((Faction)obj).m_Definition.Sort;
		}

		public static bool CheckLeaveTimer(Mobile mob)
		{
			PlayerState pl = PlayerState.Find(mob);

			if (pl == null || !pl.IsLeaving)
			{
				return false;
			}

			if ((pl.Leaving + LeavePeriod) >= DateTime.UtcNow)
			{
				return false;
			}

			LoggingCustom.Log("LOG_FactionPoints.txt", DateTime.Now + "\t" + mob.Name + "\tLeave timer up " + pl.Leaving);
			mob.SendLocalizedMessage(1005163); // You have now quit your faction

			pl.Faction.RemoveMember(mob);

			if (mob is PlayerMobile)
			{
				Conquests.CheckProgress<FactionStateConquest>((PlayerMobile)mob, null);
			}

			return true;
		}

		public static void Initialize()
		{
			CleanUpFactions();

			EventSink.Login += EventSink_Login;
			EventSink.Logout += EventSink_Logout;

			Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(10.0), HandleAtrophy);

			Timer.DelayCall(TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(30.0), ProcessTick);

			CommandSystem.Register("FactionElection", AccessLevel.Administrator, FactionElection_OnCommand);
			CommandSystem.Register("FactionCommander", AccessLevel.Administrator, FactionCommander_OnCommand);
			CommandSystem.Register("FactionItemReset", AccessLevel.Administrator, FactionItemReset_OnCommand);
			CommandSystem.Register("FactionReset", AccessLevel.Administrator, FactionReset_OnCommand);
			CommandSystem.Register("FactionTownReset", AccessLevel.Administrator, FactionTownReset_OnCommand);
		}

		private static readonly TimeSpan MustKillThisOften = TimeSpan.FromDays(2000.0);
		// 90.0 ); // was 90 but this is just dumb-catches people off guard or if they are away a long time.. just let atrophy do its thing (Alan)

		public static void CleanUpFactions()
		{
			//Console.Write( "Cleaning factions..." );
			int count = 0;

			// loop through every mobile that's a faction member
			List<Faction> factions = Factions;

			foreach (Faction f in factions)
			{
				var list = new List<PlayerState>(f.Members);

				foreach (PlayerState ps in list)
				{
					Mobile m = ps.Mobile;

					if ((DateTime.UtcNow - ps.JoinDate) <= MustKillThisOften || (DateTime.UtcNow - ps.LastKill) <= MustKillThisOften ||
						MerchantTitles.HasMerchantQualifications(m))
					{
						continue;
					}

					LoggingCustom.Log(
						"LOG_FactionPoints.txt",
						DateTime.Now + "\t" + m.Name + "\tFailed requirements: JoinDate: " + ps.JoinDate + "\tLastKill: " + ps.LastKill);

					f.RemoveMember(m);
					count ++;
				}
			}

			if (count > 0)
			{
				Console.WriteLine("Factions: Removed {0} inactive members...", count);
			}
		}

		public static void FactionTownReset_OnCommand(CommandEventArgs e)
		{
			foreach (BaseMonolith m in BaseMonolith.Monoliths.Where(m => m != null).ToArray())
			{
				m.Sigil = null;
			}

			foreach (Town t in Town.Towns.Where(t => t != null).ToArray())
			{
				t.Silver = 0;
				t.Sheriff = null;
				t.Finance = null;
				t.Tax = 0;
				t.Owner = null;
			}

			foreach (Sigil s in Sigil.Sigils.Where(s => s != null).ToArray())
			{
				s.Corrupted = null;
				s.Corrupting = null;
				s.LastStolen = DateTime.MinValue;
				s.GraceStart = DateTime.MinValue;
				s.CorruptionStart = DateTime.MinValue;
				s.PurificationStart = DateTime.MinValue;
				s.LastMonolith = null;
				s.ReturnHome();
			}

			foreach (
				FactionItem fi in Factions.SelectMany(fa => fa.State.FactionItems.Where(s => s != null && s.Item != null)).ToArray()
				)
			{
				if (fi.Expiration == DateTime.MinValue)
				{
					fi.Item.Delete();
				}
				else
				{
					fi.Detach();
				}
			}
		}

		public static void FactionReset_OnCommand(CommandEventArgs e)
		{
			foreach (BaseMonolith m in BaseMonolith.Monoliths.Where(m => m != null).ToArray())
			{
				m.Sigil = null;
			}

			foreach (Town t in Town.Towns.Where(t => t != null).ToArray())
			{
				t.Silver = 0;
				t.Sheriff = null;
				t.Finance = null;
				t.Tax = 0;
				t.Owner = null;
			}

			foreach (Sigil s in Sigil.Sigils.Where(s => s != null).ToArray())
			{
				s.Corrupted = null;
				s.Corrupting = null;
				s.LastStolen = DateTime.MinValue;
				s.GraceStart = DateTime.MinValue;
				s.CorruptionStart = DateTime.MinValue;
				s.PurificationStart = DateTime.MinValue;
				s.LastMonolith = null;
				s.ReturnHome();
			}

			List<Faction> factions = Factions;

			foreach (Faction f in factions)
			{
				foreach (PlayerState s in f.Members.Where(s => s != null && s.Mobile != null).ToArray())
				{
					f.RemoveMember(s.Mobile);
				}

				f.Members.Clear();
				f.ZeroRankOffset = 0;

				foreach (
					FactionItem fi in
						Factions.SelectMany(fa => fa.State.FactionItems.Where(s => s != null && s.Item != null)).ToArray())
				{
					if (fi.Expiration == DateTime.MinValue)
					{
						fi.Item.Delete();
					}
					else
					{
						fi.Detach();
					}
				}

				foreach (BaseFactionTrap t in f.Traps.Where(t => t != null).ToArray())
				{
					t.Delete();
				}
			}
		}

		public static void FactionItemReset_OnCommand(CommandEventArgs e)
		{
			List<Item> items =
				World.Items.Values.AsParallel()
					 .OfType<IFactionItem>()
					 .Where(item => !(item is HoodedShroudOfShadows))
					 .OfType<Item>()
					 .ToList();

			var hues = new int[Factions.Count * 2];

			for (int i = 0; i < Factions.Count; ++i)
			{
				hues[0 + (i * 2)] = Factions[i].Definition.HuePrimary;
				hues[1 + (i * 2)] = Factions[i].Definition.HueSecondary;
			}

			int count = 0;

			foreach (Item item in items)
			{
				var fci = (IFactionItem)item;

				if (fci.FactionItemState != null || item.LootType != LootType.Blessed)
				{
					continue;
				}

				bool isHued = hues.Any(t => item.Hue == t);

				if (!isHued)
				{
					continue;
				}

				if (fci.FactionItemState != null)
				{
					fci.FactionItemState.Detach();
				}

				++count;
			}

			e.Mobile.SendMessage("{0} items reset", count);
		}

		public static void FactionCommander_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendMessage("Target a player to make them the faction commander.");
			e.Mobile.BeginTarget(-1, false, TargetFlags.None, FactionCommander_OnTarget);
		}

		public static void FactionCommander_OnTarget(Mobile from, object obj)
		{
			if (obj is PlayerMobile)
			{
				var targ = (Mobile)obj;
				PlayerState pl = PlayerState.Find(targ);

				if (pl != null)
				{
					pl.Faction.Commander = targ;
					from.SendMessage("You have appointed them as the faction commander.");
				}
				else
				{
					from.SendMessage("They are not in a faction.");
				}
			}
			else
			{
				from.SendMessage("That is not a player.");
			}
		}

		public static void FactionElection_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendMessage("Target a faction stone to open its election properties.");
			e.Mobile.BeginTarget(-1, false, TargetFlags.None, FactionElection_OnTarget);
		}

		public static void FactionElection_OnTarget(Mobile from, object obj)
		{
			if (!(obj is FactionStone))
			{
				from.SendMessage("That is not a faction stone.");
				return;
			}

			Faction faction = ((FactionStone)obj).Faction;

			if (faction != null)
			{
				from.SendGump(new ElectionManagementGump(faction.Election));
				//from.SendGump( new Gumps.PropertiesGump( from, faction.Election ) );
			}
			else
			{
				from.SendMessage("That stone has no faction assigned.");
			}
		}

		public static void FactionKick_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendMessage("Target a player to remove them from their faction.");
			e.Mobile.BeginTarget(-1, false, TargetFlags.None, FactionKick_OnTarget);
		}

		public static void FactionKick_OnTarget(Mobile from, object obj)
		{
			if (!(obj is Mobile))
			{
				from.SendMessage("That is not a player.");
				return;
			}

			var mob = (Mobile)obj;
			PlayerState pl = PlayerState.Find(mob);

			if (pl != null)
			{
				pl.Faction.RemoveMember(mob);

				mob.SendMessage("You have been kicked from your faction.");
				from.SendMessage("They have been kicked from their faction.");
			}
			else
			{
				from.SendMessage("They are not in a faction.");
			}
		}

		public static void ProcessTick()
		{
			foreach (Sigil sigil in Sigil.Sigils.Where(s => s != null).ToArray())
			{
				if (!sigil.IsBeingCorrupted && sigil.GraceStart != DateTime.MinValue &&
					(sigil.GraceStart + Sigil.CorruptionGrace) < DateTime.UtcNow)
				{
					if (sigil.LastMonolith is StrongholdMonolith &&
						(sigil.Corrupted == null || sigil.LastMonolith.Faction != sigil.Corrupted))
					{
						sigil.Corrupting = sigil.LastMonolith.Faction;
						sigil.CorruptionStart = DateTime.UtcNow;
					}
					else
					{
						sigil.Corrupting = null;
						sigil.CorruptionStart = DateTime.MinValue;
					}

					sigil.GraceStart = DateTime.MinValue;
				}

				if (sigil.LastMonolith == null || sigil.LastMonolith.Sigil == null)
				{
					if ((sigil.LastStolen + Sigil.ReturnPeriod) < DateTime.UtcNow)
					{
						sigil.ReturnHome();
					}
				}
				else
				{
					if (sigil.IsBeingCorrupted && (sigil.CorruptionStart + Sigil.CorruptionPeriod) < DateTime.UtcNow)
					{
						sigil.Corrupted = sigil.Corrupting;
						sigil.Corrupting = null;
						sigil.CorruptionStart = DateTime.MinValue;
						sigil.GraceStart = DateTime.MinValue;
					}
					else if (sigil.IsPurifying && (sigil.PurificationStart + Sigil.PurificationPeriod) < DateTime.UtcNow)
					{
						sigil.PurificationStart = DateTime.MinValue;
						sigil.Corrupted = null;
						sigil.Corrupting = null;
						sigil.CorruptionStart = DateTime.MinValue;
						sigil.GraceStart = DateTime.MinValue;
					}
				}
			}
		}

		public static void HandleDeath(Mobile mob)
		{
			HandleDeath(mob, null);
		}

		#region Skill Loss
		public const double SkillLossFactor = 1.0 / 3.0;
		public static readonly TimeSpan SkillLossPeriod = TimeSpan.FromMinutes(2.0);

		public static readonly Dictionary<Mobile, SkillLossContext> m_SkillLoss = new Dictionary<Mobile, SkillLossContext>();

		public class SkillLossContext
		{
			public Timer m_Timer;
			public List<SkillMod> m_Mods;
		}

		public static bool InSkillLoss(Mobile mob)
		{
			return m_SkillLoss.ContainsKey(mob);
		}

		public static void ApplySkillLoss(Mobile mob)
		{
			SkillLossContext context;

			if (!m_SkillLoss.TryGetValue(mob, out context))
			{
				m_SkillLoss.Add(mob, context = new SkillLossContext());
			}
			else if (context == null)
			{
				m_SkillLoss[mob] = context = new SkillLossContext();
			}
			else
			{
				return;
			}

			List<SkillMod> mods = context.m_Mods = new List<SkillMod>();

			foreach (Skill sk in mob.Skills)
			{
				double baseValue = sk.Base;

				if (baseValue <= 0)
				{
					continue;
				}

				SkillMod mod = new DefaultSkillMod(sk.SkillName, true, -(baseValue * SkillLossFactor));

				mods.Add(mod);
				mob.AddSkillMod(mod);
			}

			//PlayerState ps = PlayerState.Find( mob );

		    context.m_Timer = Timer.DelayCall(SkillLossPeriod, ClearSkillLoss_Callback, mob);
		}

		private static void ClearSkillLoss_Callback(Mobile mob)
		{
			ClearSkillLoss(mob);
		}

		public static bool ClearSkillLoss(Mobile mob)
		{
			SkillLossContext context;
			m_SkillLoss.TryGetValue(mob, out context);

			if (context != null)
			{
				m_SkillLoss.Remove(mob);

				List<SkillMod> mods = context.m_Mods;

				foreach (SkillMod sm in mods)
				{
					mob.RemoveSkillMod(sm);
				}

				context.m_Timer.Stop();

				return true;
			}

			return false;
		}
		#endregion

		public int AwardSilver(Mobile mob, int silver)
		{
			if (silver <= 0 || mob is BaseVendor)
			{
				return 0;
			}

			int tithed = (silver * Tithe) / 100;

			Silver += tithed;

			silver = silver - tithed;

			if (silver > 0)
			{
				var s = new Silver(silver);

				mob.AddToBackpack(s);

				if (mob is PlayerMobile)
				{
					Conquests.CheckProgress<ItemConquest>((PlayerMobile)mob, s);
				}
			}

			return silver;
		}

		public virtual int MaximumTraps { get { return 15; } }

		public List<BaseFactionTrap> Traps { get { return m_State.Traps; } set { m_State.Traps = value; } }

		public const int StabilityFactor = 20; // 20% greater than second largest faction

		public const int StabilityActivation = 10000;
		// Stablity code goes into effect when largest faction has > 200 people

		private static int GetMemberCount(Faction faction)
		{
			var accountlist = new List<Account>();

			foreach (Account acc in faction.Members.Select(ps => ps.Mobile.Account as Account).Where(acc => acc != null))
			{
				accountlist.AddOrReplace(acc);
			}

			return accountlist.Count;
		}

		/*public static Faction FindSmallestFaction()
        {
            List<Faction> factions = Factions;
            Faction smallest = null;
            int smallestmembers = 0;

            for (int i = 0; i < factions.Count; ++i)
            {
                Faction faction = factions[i];
                int count = GetMemberCount(faction);

                if (smallest == null || count < smallestmembers)
                {
                    smallest = faction;
                    smallestmembers = count;
                }
            }

            return smallest;
        }*/

		public static Faction FindLargestFaction()
		{
			Faction largest = null;

			foreach (Faction faction in Factions)
			{
				if (largest == null)
				{
					largest = faction;
				}
				else if (GetMemberCount(faction) > GetMemberCount(largest))
				{
					largest = faction;
				}
			}

			return largest;
		}

		/*public static Faction FindSecondLargestFaction()
        {
            List<Faction> factions = Factions;
            Faction largest = null;
            Faction secondlargest = null;

            foreach (Faction faction in factions)
            {
                if (largest == null)
                {
                    largest = faction;
                }
                else if (GetMemberCount(faction) > GetMemberCount(largest))
                {
                    if (secondlargest != null && GetMemberCount(largest) > GetMemberCount(secondlargest) || secondlargest == null)
                    {
                        secondlargest = largest;
                    }
                    largest = faction;
                }

                if (secondlargest == null && largest != faction)
                {
                    secondlargest = faction;
                }
                else if (secondlargest != null && GetMemberCount(faction) > GetMemberCount(secondlargest) &&
                         faction != largest)
                {
                    secondlargest = faction;
                }
            }
            return secondlargest;
        }*/

		public static bool StabilityActive()
		{
			return Factions.Any(faction => faction.Members.Count > StabilityActivation);
		}

		public bool CanHandleInflux(int influx)
		{
			if (!StabilityActive())
			{
				return true;
			}

			Faction largestfaction = FindLargestFaction();

			if (largestfaction == null)
			{
				return true; // sanity
			}

			if (largestfaction == this)
			{
				return false;
			}

			double factor = ((double)largestfaction.Members.Count / 100 * (20));
			int balancenumber = largestfaction.Members.Count + (int)Math.Round(factor);

			if (balancenumber == 0)
			{
				balancenumber = 30;
			}

			return (GetMemberCount(this) + influx) <= balancenumber;
		}

		public static void HandleDeath(Mobile victim, Mobile killer)
		{
			if (killer == null)
			{
				killer = victim.FindMostRecentDamager(true);
			}

			PlayerState victimState = PlayerState.Find(victim);
			PlayerState killerState = PlayerState.Find(killer);

			Container pack = victim.Backpack;

			if (pack != null)
			{
				Container killerPack = (killer == null ? null : killer.Backpack);

				foreach (Sigil sigil in pack.FindItemsByType<Sigil>())
				{
					if (killerState != null && killerPack != null)
					{
						if (killer.GetDistanceToSqrt(victim) > 64)
						{
							sigil.ReturnHome();
							killer.SendLocalizedMessage(1042230); // The sigil has gone back to its home location.
						}
						else if (Sigil.ExistsOn(killer))
						{
							sigil.ReturnHome();
							killer.SendLocalizedMessage(1010258);
							// The sigil has gone back to its home location because you already have a sigil.
						}
						else if (!killerPack.TryDropItem(killer, sigil, false))
						{
							sigil.ReturnHome();
							killer.SendLocalizedMessage(1010259);
							// The sigil has gone home because your backpack is full.
						}
						else if (killer.Region.IsPartOf<HouseRegion>())
						{
							sigil.ReturnHome();
							killer.SendLocalizedMessage(1042230); // The sigil has gone back to its home location.
						}
					}
					else
					{
						sigil.ReturnHome();
					}
				}
			}

			#region Dueling
			if (victim.Region.IsPartOf<SafeZone>() || victim.Region.IsPartOf<PvPBattleRegion>())
			{
				return;
			}
			#endregion

			if (victim is PlayerMobile && victimState != null &&
				(killer == victim || (killer is BaseFactionGuard && ((BaseFactionGuard)killer).Faction != victimState.Faction) ||
				 (killerState != null && killerState.Faction != victimState.Faction)))
			{
				((PlayerMobile)victim).FactionDeath = true;

				ApplySkillLoss(victim);
			}

			if (killerState != null && killer != null)
			{
				if (victim is BaseCreature && !(victim is BaseVendor))
				{
					var bc = (BaseCreature)victim;
					Faction victimFaction = bc.FactionAllegiance;

					if (IsFactionFacet(bc.Map) && victimFaction != null && killerState.Faction != victimFaction)
					{
						int silver = killerState.Faction.AwardSilver(killer, bc.FactionSilverWorth);

						if (silver > 0)
						{
							killer.SendLocalizedMessage(1042748, silver.ToString("N0"));
							// Thou hast earned ~1_AMOUNT~ silver for vanquishing the vile creature.
						}
					}

					#region Ethics
					if (Ethic.Enabled && IsFactionFacet(bc.Map) && bc.GetEthicAllegiance(killer) == BaseCreature.Allegiance.Enemy)
					{
						Player killerEPL = Player.Find(killer);

						if (killerEPL != null && killerEPL.Power < Player.MaxPower)
						{
							killerEPL.Power += 1;

							if (0.02 >= Utility.RandomDouble())
							{
								killerEPL.Sphere += 1;
								killer.SendMessage(
									"You have gained a sphere of influence for slaying a minion of {0}.",
									killerEPL.Ethic == Ethic.Evil ? "justice" : "evil");
							}

							killer.SendMessage(
								"You gain a little life force for slaying a minion of {0}.", killerEPL.Ethic == Ethic.Evil ? "justice" : "evil");
						}
					}
					#endregion

					return;
				}
			}

			if (killer == null || victimState == null || killerState == null || killerState.Faction == victimState.Faction)
			{
				return;
			}

			if (victimState.KillPoints <= -6)
			{
				killer.SendLocalizedMessage(501693); // This victim is not worth enough to get kill points from.
			}
			else
			{
				int award = Math.Max(victimState.KillPoints / 10, 1);

				if (award > 40)
				{
					award = 40;
				}

				if (victimState.CanGiveSilverTo(killer))
				{
					if (Ethic.Enabled)
					{
						Player killerEPL = Player.Find(killer);
						Player victimEPL = Player.Find(victim);

					    if (killerEPL == null || victimEPL == null)
					    {
					        return;
					    }

						int powerTransfer = Math.Min(Math.Max(1, victimEPL.Power / 5), Player.MaxPower - killerEPL.Power);

						if (powerTransfer > 0)
						{
							victimEPL.Power -= powerTransfer;
							killerEPL.Power += powerTransfer;

							killer.FixedEffect(0x373A, 10, 30);
							killer.PlaySound(0x209);

							killer.SendMessage("You have gained {0} life force for killing {1}.", powerTransfer, victim.Name);
							victim.SendMessage("You have lost {0} life force for falling victim to {1}.", powerTransfer, killer.Name);
						}

						if (killerEPL.Ethic != victimEPL.Ethic &&
							(victimEPL.Sphere > 0 && (victimEPL.Rank > 3 || (killerEPL.Rank - victimEPL.Rank < 3))))
						{
							int sphereTransfer = Math.Max(1, 1 + (victimEPL.Rank - killerEPL.Rank));
							//Always at least 1pt
							victimEPL.Sphere -= sphereTransfer;
							killerEPL.Sphere += sphereTransfer;
							killer.SendMessage(
								"You have gained a sphere of influence for slaying an agent of {0}.",
								killerEPL.Ethic == Ethic.Evil ? "justice" : "evil");
							victim.SendMessage(
								"You have lost a sphere of influence for being slain by an agent of {0}.",
								killerEPL.Ethic == Ethic.Hero ? "justice" : "evil");
						}
					}

					if (victimState.KillPoints > 0)
					{
						victimState.IsActive = true;

						if (1 > Utility.Random(3))
						{
							killerState.IsActive = true;
						}

						int silver = killerState.Faction.AwardSilver(killer, award * 40);

						if (silver > 0)
						{
							killer.SendLocalizedMessage(1042736, String.Format("{0:N0} silver\t{1}", silver, victim.Name));
							// You have earned ~1_SILVER_AMOUNT~ pieces for vanquishing ~2_PLAYER_NAME~!
						}
					}

					victimState.KillPoints -= award;
					killerState.KillPoints += award;
					killerState.LastKill = DateTime.UtcNow;

					int offset = (award != 1 ? 0 : 2); // for pluralization

					string args = String.Format("{0}\t{1}\t{2}", award, victim.Name, killer.Name);

					killer.SendLocalizedMessage(1042737 + offset, args);
					// Thou hast been honored with ~1_KILL_POINTS~ kill point(s) for vanquishing ~2_DEAD_PLAYER~!
					victim.SendLocalizedMessage(1042738 + offset, args);
					// Thou has lost ~1_KILL_POINTS~ kill point(s) to ~3_ATTACKER_NAME~ for being vanquished!

					victimState.OnGivenSilverTo(killer);
				}
				else
				{
					killer.SendLocalizedMessage(1042231);
					// You have recently defeated this enemy and thus their death brings you no honor.
				}
			}
			//ethics
		}

		private static void EventSink_Logout(LogoutEventArgs e)
		{
			Mobile mob = e.Mobile;

			Container pack = mob.Backpack;

			if (pack == null)
			{
				return;
			}

			foreach (Sigil s in pack.FindItemsByType<Sigil>())
			{
				s.ReturnHome();
			}
		}

		private static void EventSink_Login(LoginEventArgs e)
		{
			Mobile mob = e.Mobile;

			if (mob is BaseCreature)
			{
				return; // for pseudoseer controlled mobs
			}

			/*PlayerState pl = PlayerState.Find(mob);
            Faction factionf = Find(mob);

            if (mob.Kills >= 5)
            {
                if (pl != null)
                {
                    if ((factionf.Definition.FriendlyName == "True Britannians" ||
                         factionf.Definition.FriendlyName == "Council of Mages"))
                    {
                        LoggingCustom.Log("LOG_FactionPoints.txt",
                            DateTime.Now + "\t" + mob.Name + "\tKicked for being murderer");
                        pl.Faction.RemoveMember(mob);
                        mob.SendMessage("You have been kicked from your faction for being a murderer.");
                    }
                }
            }*/

			CheckLeaveTimer(mob);
		}

		public static readonly Map Facet = Map.Felucca;

		public static bool IsFactionFacet(Map map)
		{
			return true; //All maps are faction maps.
		}

		public static void WriteReference(GenericWriter writer, Faction fact)
		{
			int idx = Factions.IndexOf(fact);

			writer.WriteEncodedInt(idx + 1);
		}

		public static List<Faction> Factions { get { return Reflector.Factions; } }

		public static Faction ReadReference(GenericReader reader)
		{
			int idx = reader.ReadEncodedInt() - 1;

			if (idx >= 0 && idx < Factions.Count)
			{
				return Factions[idx];
			}

			return null;
		}

		public static Faction Find(Mobile mob)
		{
			return Find(mob, false, false);
		}

		public static Faction Find(Mobile mob, bool inherit)
		{
			return Find(mob, inherit, false);
		}

		public static Faction Find(Mobile mob, bool inherit, bool creatureAllegiances)
		{
			PlayerState pl = PlayerState.Find(mob);

			if (pl != null)
			{
				return pl.Faction;
			}

			if (inherit && mob is BaseCreature)
			{
				var bc = (BaseCreature)mob;

				if (bc.Controlled)
				{
					return Find(bc.ControlMaster, false);
				}

				if (bc.Summoned)
				{
					return Find(bc.SummonMaster, false);
				}

				if (creatureAllegiances && mob is BaseFactionGuard)
				{
					return ((BaseFactionGuard)mob).Faction;
				}

				if (creatureAllegiances)
				{
					return bc.FactionAllegiance;
				}
			}

			return null;
		}

		public static Faction Parse(string name)
		{
			return Factions.FirstOrDefault(faction => Insensitive.Equals(faction.Definition.FriendlyName, name));
		}
	}

	public enum FactionKickType
	{
		Kick,
		Ban,
		Unban
	}

	public class FactionKickCommand : BaseCommand
	{
		private readonly FactionKickType m_KickType;

		public FactionKickCommand(FactionKickType kickType)
		{
			m_KickType = kickType;

			AccessLevel = AccessLevel.Lead;
			Supports = CommandSupport.AllMobiles;
			ObjectTypes = ObjectTypes.Mobiles;

			switch (m_KickType)
			{
				case FactionKickType.Kick:
					{
						Commands = new[] {"FactionKick"};
						Usage = "FactionKick";
						Description = "Kicks the targeted player out of his current faction. This does not prevent them from rejoining.";
						break;
					}
				case FactionKickType.Ban:
					{
						Commands = new[] {"FactionBan"};
						Usage = "FactionBan";
						Description =
							"Bans the account of a targeted player from joining factions. All players on the account are removed from their current faction, if any.";
						break;
					}
				case FactionKickType.Unban:
					{
						Commands = new[] {"FactionUnban"};
						Usage = "FactionUnban";
						Description = "Unbans the account of a targeted player from joining factions.";
						break;
					}
			}
		}

		public override void Execute(CommandEventArgs e, object obj)
		{
			var mob = (Mobile)obj;

			switch (m_KickType)
			{
				case FactionKickType.Kick:
					{
						PlayerState pl = PlayerState.Find(mob);

						if (pl != null)
						{
							pl.Faction.RemoveMember(mob);
							mob.SendMessage("You have been kicked from your faction.");
							AddResponse("They have been kicked from their faction.");
							LoggingCustom.Log("LOG_FactionPoints.txt", DateTime.Now + "\t" + mob.Name + "\tKicked by GM " + e.Mobile);
						}
						else
						{
							LogFailure("They are not in a faction.");
						}

						break;
					}
				case FactionKickType.Ban:
					{
						var acct = mob.Account as Account;

						if (acct != null)
						{
							if (acct.GetTag("FactionBanned") == null)
							{
								acct.SetTag("FactionBanned", "true");
								AddResponse("The account has been banned from joining factions.");
							}
							else
							{
								AddResponse("The account is already banned from joining factions.");
							}

							for (int i = 0; i < acct.Length; ++i)
							{
								mob = acct[i];

								if (mob == null)
								{
									continue;
								}

								PlayerState pl = PlayerState.Find(mob);

								if (pl == null)
								{
									continue;
								}

								LoggingCustom.Log("LOG_FactionPoints.txt", DateTime.Now + "\t" + mob.Name + "\tBanned by GM " + e.Mobile);
								pl.Faction.RemoveMember(mob);
								mob.SendMessage("You have been kicked from your faction.");
								AddResponse("They have been kicked from their faction.");
							}
						}
						else
						{
							LogFailure("They have no assigned account.");
						}

						break;
					}
				case FactionKickType.Unban:
					{
						var acct = mob.Account as Account;

						if (acct != null)
						{
							if (acct.GetTag("FactionBanned") == null)
							{
								AddResponse("The account is not already banned from joining factions.");
							}
							else
							{
								acct.RemoveTag("FactionBanned");
								AddResponse("The account may now freely join factions.");
							}
						}
						else
						{
							LogFailure("They have no assigned account.");
						}

						break;
					}
			}
		}
	}
}