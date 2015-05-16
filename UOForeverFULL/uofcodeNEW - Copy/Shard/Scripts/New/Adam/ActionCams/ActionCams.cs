#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server.Mobiles;
using Server.Network;
using Server.Regions;

using VitaNex.Modules.AutoPvP;
using VitaNex.Notify;
using VitaNex.SuperGumps;
#endregion

namespace Server.Twitch
{
	public static partial class ActionCams
	{
		private static readonly FileInfo _PersistenceFile;

		public static Dictionary<PlayerMobile, DateTime> DeathCams { get; private set; }
		public static Dictionary<PlayerMobile, DateTime> DeathCamsEvents { get; private set; }

		public static Dictionary<PlayerMobile, PlayerMobile> CurrentlyViewing { get; private set; }

		public static Dictionary<PlayerMobile, int> PlayerMurderers { get; private set; }
		public static Dictionary<BaseCreature, int> MonsterMurderers { get; private set; }

		public static List<Type> GumpWhitelist { get; private set; }
		public static List<string> RegionBlackList { get; private set; }

		public static ulong CurrentDeathCount { get; set; }
		public static ulong CurrentPlayerMurders { get; set; }
		public static ulong CurrentMonsterMurders { get; set; }

		public static PlayerMobile TopPlayerMurderer { get; set; }
		public static BaseCreature TopMonsterMurderer { get; set; }

		public static void Defragment()
		{
			DeathCams.RemoveKeyRange(cam => cam.Deleted || !cam.IsOnline());
			DeathCamsEvents.RemoveKeyRange(cam => cam.Deleted || !cam.IsOnline());

			CurrentlyViewing.RemoveKeyRange(cam => cam.Deleted || !cam.IsOnline() || !IsCamera(cam));
			CurrentlyViewing.RemoveValueRange(target => target.Deleted || !target.IsOnline() || IsCamera(target));

			PlayerMurderers.RemoveKeyRange(player => player.Deleted);
			MonsterMurderers.RemoveKeyRange(mob => mob.Deleted);
		}

		public static bool IsCamera(PlayerMobile pm)
		{
			return pm != null && (DeathCams.ContainsKey(pm) || DeathCamsEvents.ContainsKey(pm));
		}

		private static void OnNotify(NotifyGump obj)
		{
			if (obj == null || obj.AutoClose || !IsCamera(obj.User))
			{
				return;
			}

			obj.PauseDuration = TimeSpan.FromSeconds(30.0);
			obj.AutoClose = true;
		}

		private static void OnPlayerDeath(PlayerDeathEventArgs e)
		{
			if (e == null || e.Mobile == null || !(e.Mobile is PlayerMobile))
			{
				return;
			}

			var player = (PlayerMobile)e.Mobile;

			if (IsCamera(player))
			{
				return;
			}

			if (player.LastKiller != null && player != player.LastKiller)
			{
				CurrentDeathCount++;

				if (player.LastKiller is PlayerMobile)
				{
					PlayerKillerCheck(player.LastKiller as PlayerMobile);
				}
				else if (player.LastKiller is BaseCreature && ((BaseCreature)player.LastKiller).Controlled &&
						 ((BaseCreature)player.LastKiller).ControlMaster is PlayerMobile)
				{
					PlayerKillerCheck(((BaseCreature)player.LastKiller).ControlMaster as PlayerMobile);
				}
				else if (player.LastKiller is BaseCreature && !((BaseCreature)player.LastKiller).Controlled)
				{
					MonsterKillerCheck((BaseCreature)player.LastKiller);
				}
			}

			foreach (PlayerMobile cam in GetCams())
			{
				cam.PlaySound(100);
				RefreshUI(cam);
			}
		}

		public static void PlayerKillerCheck(PlayerMobile pm)
		{
			if (pm == null || IsCamera(pm))
			{
				return;
			}

			++CurrentPlayerMurders;

			if ((TopPlayerMurderer == null || TopPlayerMurderer.Deleted || !PlayerMurderers.ContainsKey(TopPlayerMurderer)) &&
				PlayerMurderers.Count > 0)
			{
				TopPlayerMurderer = PlayerMurderers.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
			}

			int count;

			if (!PlayerMurderers.TryGetValue(pm, out count))
			{
				PlayerMurderers.Add(pm, count = 1);
			}
			else
			{
				count = ++PlayerMurderers[pm];
			}

			if (count >= PlayerMurderers.Values.Max())
			{
				TopPlayerMurderer = pm;
			}
		}

		public static void MonsterKillerCheck(BaseCreature m)
		{
			if (m == null)
			{
				return;
			}

			++CurrentMonsterMurders;

			if ((TopMonsterMurderer == null || TopMonsterMurderer.Deleted || !MonsterMurderers.ContainsKey(TopMonsterMurderer)) &&
				MonsterMurderers.Count > 0)
			{
				TopMonsterMurderer = MonsterMurderers.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
			}

			int count;

			if (!MonsterMurderers.TryGetValue(m, out count))
			{
				MonsterMurderers.Add(m, count = 1);
			}
			else
			{
				count = ++MonsterMurderers[m];
			}

			if (count >= MonsterMurderers.Values.Max())
			{
				TopMonsterMurderer = m;
			}
		}

		public static bool IsViewing(PlayerMobile target)
		{
			return target != null && !IsCamera(target) && CurrentlyViewing.ContainsValue(target);
		}

		public static bool CanView(PlayerMobile target)
		{
			return target != null && !target.Deleted // Sanity
				   && !IsCamera(target) // No Cams
				   && target.Map != null && target.Map != Map.Internal // No Invalid Maps
				   && target.IsOnline() // Online Only
				   && target.InCombat(TimeSpan.FromSeconds(60.0)) // Combat Heat Only
				   && (DateTime.UtcNow - target.LastMoveTime).TotalSeconds < 60.0 // Has Moved
				   && !target.InRegion<HouseRegion>() && !target.InRegion<Jail>() // No Houses or Jail
				   && !RegionBlackList.Any(target.InRegion); // No Blacklisted Regions (By Name)
		}

		public static PlayerMobile[] GetCamsViewing(PlayerMobile target)
		{
			return target != null && !IsCamera(target)
					   ? CurrentlyViewing.Where(kv => kv.Value == target).Select(kv => kv.Key).ToArray()
					   : new PlayerMobile[0];
		}

		public static IEnumerable<PlayerMobile> GetCams()
		{
			Defragment();

			return DeathCams.Keys.With(DeathCamsEvents.Keys);
		}

		public static void MoveCams(PlayerMobile target, PlayerMobile[] cams)
		{
			if (target == null || cams == null || cams.Length == 0)
			{
				return;
			}

			foreach (
				PlayerMobile cam in cams.Not(cam => cam == null || cam.Deleted || (cam.Map == target.Map && cam.InRange(target, 0)))
				)
			{
				bool refreshUI = cam.Map != target.Map || cam.Region != target.Region;

				cam.Map = target.Map;
				cam.SetLocation(target.Location, true);

				if (refreshUI)
				{
					RefreshUI(cam);
				}
			}
		}

		public static void AssignCams(PlayerMobile target, bool idleCams)
		{
			if (target == null || !CanView(target))
			{
				return;
			}

			DateTime now = DateTime.UtcNow;

			// Process battle cams and return, if they are in the pvp region.
			if (target.InRegion<PvPBattleRegion>())
			{
				const double idleSeconds = 5.0;

				foreach (KeyValuePair<PlayerMobile, DateTime> kv in DeathCamsEvents.ToArray())
				{
					PlayerMobile cam = kv.Key;
					DateTime time = kv.Value;

					if (now > time && !idleCams)
					{
						CurrentlyViewing.AddOrReplace(cam, target);
						DeathCamsEvents[cam] = now.AddSeconds(idleSeconds);
					}
					else if (now > time.AddSeconds(idleSeconds) && idleCams)
					{
						// if cam has been idling for longer than normal

						CurrentlyViewing.AddOrReplace(cam, target);
						DeathCamsEvents[cam] = now; // assign right away if primary threshhold is met
					}
				}
			}
			else
			{
				const double idleSeconds = 15.0;

				// Normal (non-battle)
				foreach (KeyValuePair<PlayerMobile, DateTime> kv in DeathCams.ToArray())
				{
					PlayerMobile cam = kv.Key;
					DateTime time = kv.Value;

					if (now > time && !idleCams)
					{
						CurrentlyViewing.AddOrReplace(cam, target);
						DeathCams[cam] = now.AddSeconds(idleSeconds);
					}
					else if (now > time.AddSeconds(idleSeconds) && idleCams)
					{
						// if cam has been idling for longer than normal

						CurrentlyViewing.AddOrReplace(cam, target);
						DeathCams[cam] = now; // assign right away if primary threshhold is met
					}
				}
			}
		}

		public static void OnPlayerMove(PlayerMobile target, Point3D oldLocation)
		{
			if (target == null || !IsViewing(target))
			{
				return;
			}

			if (!CanView(target))
			{
				CurrentlyViewing.RemoveValueRange(v => v == target);
				return;
			}

			PlayerMobile[] cams = GetCamsViewing(target);

			MoveCams(target, cams);
		}

		public static void OnPlayerHitsChange(PlayerMobile target, int oldValue)
		{
			if (target == null || !target.Alive)
			{
				return;
			}

			int offset = target.Hits - oldValue;

			// If we're not viewing the target, try to assign cams.
			if (!IsViewing(target))
			{
				if (target.Hits <= 25)
				{
					AssignCams(target, false);
				}
				else if (target.Hits <= 40)
				{
					AssignCams(target, true); //assign idle cams at a less strict threshold
				}
			}

			if (offset < 0)
			{
				OnPlayerDamage(target, Math.Abs(offset));
			}
			else if (offset > 0)
			{
				OnPlayerHeal(target, offset);
			}
		}

		private static void OnPlayerHeal(PlayerMobile target, int heal)
		{
			if (target == null || !IsViewing(target))
			{
				return;
			}

			if (!CanView(target))
			{
				CurrentlyViewing.RemoveValueRange(v => v == target);
				return;
			}

			PlayerMobile[] cams = GetCamsViewing(target);

			DisplayStatus(target, heal, cams);
		}

		private static void OnPlayerDamage(PlayerMobile target, int damage)
		{
			if (target == null)
			{
				return;
			}

			if (!CanView(target))
			{
				CurrentlyViewing.RemoveValueRange(v => v == target);
				return;
			}

			PlayerMobile[] cams = GetCamsViewing(target);

			MoveCams(target, cams);

			DisplayDamage(target, damage, cams);
			DisplayStatus(target, -damage, cams);
		}

		private static void DisplayDamage(PlayerMobile target, int damage, PlayerMobile[] cams)
		{
			if (target == null || cams == null || cams.Length == 0)
			{
				return;
			}

			Packet pNew = null;
			Packet pOld = null;

			foreach (PlayerMobile cam in cams.Where(cam => cam != null && !cam.Deleted && cam.NetState != null))
			{
				if (cam.NetState.DamagePacket)
				{
					if (pNew == null)
					{
						pNew = Packet.Acquire(new DamagePacket(target, damage));
					}

					cam.Send(pNew);
				}
				else
				{
					if (pOld == null)
					{
						pOld = Packet.Acquire(new DamagePacketOld(target, damage));
					}

					cam.Send(pOld);
				}
			}
		}

		private static void DisplayStatus(PlayerMobile target, int offset, PlayerMobile[] cams)
		{
			if (target == null || !target.Alive || cams == null || cams.Length == 0)
			{
				return;
			}

			const int red = 44; // lower than green
			const int yellow = 54; // somewhere inbetween green and red
			const int green = 69; // greater than red

			string hLabel = String.Format("{0} / {1}", target.Hits, target.HitsMax); // hits
			int hHue = offset < 0 ? red : offset > 0 ? green : yellow;
				// negative offset = red, positive = green, neutral = yellow

			/*
			double percent = target.Hits / (double)target.HitsMax;

			string pLabel = String.Format("[{0}%]", percent * 100); // percent
			int pHue = (int)(red + (green - red) * percent); // interpolate between two hues
			*/

			if (offset != 0)
			{
				hLabel += String.Format(" ({0}{1:#,0})", offset > 0 ? "+" : String.Empty, offset);
			}

			Timer.DelayCall(
				TimeSpan.FromSeconds(0.5),
				() =>
				{
					foreach (PlayerMobile cam in cams.Where(cam => cam != null && !cam.Deleted && cam.NetState != null))
					{
						target.PrivateOverheadMessage(MessageType.Regular, hHue, true, hLabel, cam.NetState); // hits
						//target.PrivateOverheadMessage(MessageType.Regular, pHue, true, pLabel, cam.NetState); // percent
					}
				});
		}

		public static void CloseUI(PlayerMobile cam)
		{
			foreach (ActionCamUI g in SuperGump.GetInstances<ActionCamUI>(cam, true))
			{
				g.Close(true);
			}
		}

		public static void RefreshUI(PlayerMobile cam)
		{
			if (cam == null)
			{
				return;
			}

			if (!IsCamera(cam))
			{
				CloseUI(cam);
				return;
			}

			bool refreshed = false;

			foreach (ActionCamUI g in SuperGump.GetInstances<ActionCamUI>(cam, true).Where(g => !g.IsDisposed))
			{
				g.Refresh(true);
				refreshed = true;
			}

			if (!refreshed)
			{
				new ActionCamUI(cam).Send();
			}
		}

		public static void ForcetoBattleCams()
		{
			foreach (PlayerMobile cam in DeathCams.Keys.ToArray())
			{
				JoinDeathCamEvent(cam);
			}
		}

		public static void ForceToNormalCams()
		{
			foreach (PlayerMobile cam in DeathCamsEvents.Keys.ToArray())
			{
				JoinDeathCam(cam);
			}
		}

		private static void JoinDeathCam(PlayerMobile cam)
		{
			if (cam == null)
			{
				return;
			}

			if (DeathCamsEvents.Remove(cam))
			{
				cam.SendMessage("You have been removed from the battle death cameras list.");
			}

			if (DeathCams.ContainsKey(cam))
			{
				return;
			}

			DeathCams.Add(cam, DateTime.UtcNow);

			cam.BodyValue = 0;
			cam.Blessed = true;
			cam.SendMessage("You are now a regular death camera.  Type [DCquit to stop.");

			cam.CloseAllGumps();

			RefreshUI(cam);
		}

		private static void JoinDeathCamEvent(PlayerMobile cam)
		{
			if (cam == null)
			{
				return;
			}

			if (DeathCams.Remove(cam))
			{
				cam.SendMessage("You have been removed from the regular death cameras list.");
			}

			if (DeathCamsEvents.ContainsKey(cam))
			{
				return;
			}

			DeathCamsEvents.Add(cam, DateTime.UtcNow);

			cam.BodyValue = 0;
			cam.Blessed = true;
			cam.SendMessage("You are now a battle death camera.  Type [DCquit to stop.");

			cam.CloseAllGumps();

			RefreshUI(cam);
		}

        private static void DeathCamClearStats(PlayerMobile cam)
        {
            if (cam == null)
            {
                return;
            }

            if (PlayerMurderers != null)
            {
                PlayerMurderers.Clear();
            }

            if (MonsterMurderers != null)
            {
                MonsterMurderers.Clear();
            }

            CurrentDeathCount = 0;
            CurrentPlayerMurders = 0;
            CurrentMonsterMurders = 0;

            TopMonsterMurderer = null;
            TopPlayerMurderer = null;

            cam.SendMessage("Action Camera stats have been cleared.");
        }

		private static void QuitDeathCam(PlayerMobile cam)
		{
			if (cam == null)
			{
				return;
			}

			if (DeathCams.Remove(cam))
			{
				cam.BodyValue = cam.Race.Body(cam);
				cam.SendMessage("You have been removed from the regular death cameras list.");
			}

			if (DeathCamsEvents.Remove(cam))
			{
				cam.BodyValue = cam.Race.Body(cam);
				cam.SendMessage("You have been removed from the battles death cameras list.");
			}

			CurrentlyViewing.Remove(cam);

			CloseUI(cam);
		}
	}
}