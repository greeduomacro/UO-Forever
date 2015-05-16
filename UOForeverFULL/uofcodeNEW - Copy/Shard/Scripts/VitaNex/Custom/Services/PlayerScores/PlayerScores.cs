#region References
using System;
using System.Collections.Generic;

using Server.Engines.CannedEvil;
using Server.Items;
using Server.Mobiles;

using VitaNex;
using VitaNex.IO;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server
{
	public static partial class PlayerScores
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static PlayerScoresOptions CSOptions { get; private set; }

		/// <summary>
		///     Global PlayerScores storage collection.
		/// </summary>
		public static BinaryDataStore<IEntity, Dictionary<Mobile, double>> Registry { get; private set; }

		public static bool HasPlayerScores(IEntity e)
		{
			Dictionary<Mobile, double> list;

			return TryGetPlayerScores(e, out list);
		}

		public static bool TryGetPlayerScores(IEntity e, out Dictionary<Mobile, double> list)
		{
			list = null;

			if (e == null)
			{
				return false;
			}

			if (e.Deleted || !Registry.TryGetValue(e, out list) || list == null)
			{
				RemovePlayerScores(e);
				return false;
			}

			return true;
		}

		public static Dictionary<Mobile, double> GetPlayerScores(IEntity e, bool create)
		{
			Dictionary<Mobile, double> list;

			if (TryGetPlayerScores(e, out list))
			{
				return list;
			}

			if (create && e != null && !e.Deleted)
			{
				list = list ?? new Dictionary<Mobile, double>();

				Registry.Add(e, list);

				return list;
			}

			return null;
		}

		public static bool ClearPlayerScores(IEntity e)
		{
			Dictionary<Mobile, double> list;

			if (!TryGetPlayerScores(e, out list))
			{
				return false;
			}

			list.Clear();
			return true;
		}

		public static bool RemovePlayerScores(IEntity e)
		{
			return e != null && Registry.Remove(e);
		}

		public static void AwardScorePoints(IEntity e, Mobile m, bool champ, ref double points)
		{
			if (m == null)
			{
				return;
			}

			Dictionary<Mobile, double> list = GetPlayerScores(e, true);

			if (list == null)
			{
				return;
			}

			Mobile creditMob = null;

			var oneHandedWeapon = m.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;
			var twoHandedWeapon = m.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;
			var equipRanged = twoHandedWeapon as BaseRanged;

			if (m is BaseCreature)
			{
				var bc = (BaseCreature)m;

				if (bc.ControlMaster is PlayerMobile)
				{
					creditMob = bc.ControlMaster;
					points *= CSOptions.TamerMod;
				}
				else if (bc.SummonMaster is PlayerMobile)
				{
					creditMob = bc.SummonMaster;
					points *= CSOptions.SummonMod;
				}
				else if (bc.BardMaster is PlayerMobile)
				{
					creditMob = bc.BardMaster;
					points *= CSOptions.BardMod;
				}
			}
			else if (m is PlayerMobile)
			{
				creditMob = m;

				if (champ)
				{
					if (equipRanged != null)
					{
						points *= CSOptions.ArcherVsChampMod;
					}
					else if (oneHandedWeapon != null || twoHandedWeapon != null)
					{
						points *= CSOptions.MeleeVsChampMod;
					}
				}
				else
				{
					if (equipRanged != null)
					{
						points *= CSOptions.ArcherMod;
					}
                    else if (oneHandedWeapon != null || twoHandedWeapon != null)
                    {
                        points *= CSOptions.MeleeMod;
                    }
				}
			}

			if (creditMob == null)
			{
				return;
			}

			if (champ)
			{
				points *= CSOptions.ChampionMod;
			}

			if (!list.ContainsKey(creditMob))
			{
				list.Add(creditMob, points);
			}
			else
			{
                list[creditMob] += points;
			    list[creditMob] = Math.Min(CSOptions.MaxPoints, list[creditMob]);
			    var creature = e as BaseChampion;
                if (creature != null && creature.Invasion == null && creature.Portal == null && Engines.CentralGump.CentralGump.EnsureProfile(creditMob as PlayerMobile).MiniGump || e is ChampionSpawn && Engines.CentralGump.CentralGump.EnsureProfile(creditMob as PlayerMobile).MiniGump)
			    {
                    var scoregump = new ScoreGumpPlayerScores(creditMob as PlayerMobile, list[creditMob]).Send<ScoreGumpPlayerScores>();
			    }

			}
		}

		/*public static void DefragmentPlayerScores()
		{
			VitaNexCore.TryCatch(
				() =>
				{
					Registry.RemoveKeyRange(e =>
					{
						if (e == null)
						{
							return true;
						}

						return false;
					});
					Registry.RemoveValueRange(list => list == null || list.Count == 0);
					Registry.Values.ForEach(list => list.RemoveKeyRange(m => m == null));
				},
				x => x.ToConsole(true));
		}*/
	}
}