#region References
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Accounting;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

using VitaNex.IO;
using VitaNex.SuperGumps;
#endregion

namespace Server.Engines.Conquests
{
	public static partial class Conquests
	{
		public const AccessLevel Access = AccessLevel.EventMaster;

		private static ConquestsOptions _CMOptions = new ConquestsOptions();

		public static ConquestsOptions CMOptions { get { return _CMOptions ?? (_CMOptions = new ConquestsOptions()); } }

		public static Type[] ConquestTypes { get; private set; }
		public static Type[] ConquestCompleteGumpTypes { get; private set; }

		public static BinaryDataStore<ConquestSerial, Conquest> ConquestRegistry { get; private set; }

		public static BinaryDataStore<PlayerMobile, ConquestProfile> Profiles { get; private set; }

        public static ConquestProfile EnsureProfile(PlayerMobile pm)
        {
            ConquestProfile p;

            var account = pm.Account as Account;

            if (account == null)
            {
                Console.WriteLine("NULL ACCOUNT?!");
                return null;
            }

            if (!Profiles.TryGetValue(pm, out p))
            {
                foreach (PlayerMobile mobile in account.Mobiles.OfType<PlayerMobile>())
                {
                    if (Profiles.TryGetValue(mobile, out p))
                    {
                        break;
                    }
                }
                if (p == null)
                {
                    Profiles.Add(pm, p = new ConquestProfile(pm));
                }
            }
            else if (p == null)
            {
                Profiles[pm] = p = new ConquestProfile(pm);
            }

            return p;
        }

		public static TCon Create<TCon>() where TCon : Conquest
		{
			return Create(typeof(TCon)) as TCon;
		}

		public static Conquest Create(Type t)
		{
			Conquest c = null;

			if (t != null && t.IsConstructableFrom<Conquest>())
			{
				Register(c = t.CreateInstanceSafe<Conquest>());
			}

			return c;
		}

		public static void Register(Conquest c)
		{
			if (c == null || c.Deleted)
			{
				return;
			}

			if (ConquestRegistry.ContainsKey(c.UID))
			{
				ConquestRegistry[c.UID] = c;
			}
			else
			{
				ConquestRegistry.Add(c.UID, c);
			}
		}

		public static TCon[] FindConquests<TCon>(Predicate<TCon> predicate = null) where TCon : Conquest
		{
			return ConquestRegistry.Values.OfType<TCon>().Where(c => predicate == null || predicate(c)).ToArray();
		}

		public static ConquestState FindWorldFirst(Conquest c)
		{
			if (c == null || c.Deleted)
			{
				return null;
			}

			ConquestState state = null;

			if (Profiles.Values.Any(p => p != null && p.Owner != null && p.TryGetState(c, out state) && state.WorldFirst))
			{
				return state;
			}

			return null;
		}

		public static ConquestState FindReplacementWorldFirst(Conquest c)
		{
			if (c == null || c.Deleted || !c.Enabled)
			{
				return null;
			}

			List<ConquestState> states = new List<ConquestState>();

			foreach (var p in Profiles.Values.Where(p => p != null && p.Owner != null))
			{
				ConquestState state;

				if (p.TryGetState(c, out state) && state.Completed && !state.WorldFirst)
				{
					states.Add(state);
				}
			}

			return states.OrderBy(s => s.CompletedDate.Ticks).FirstOrDefault();
		}

        public static void ConsolidateConquests()
        {
            var tempprofiles = Profiles.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            foreach (var profile in tempprofiles)
            {
                Console.WriteLine("Consolidating Profile: " + profile.Key.RawName);
                if (!Profiles.ContainsKey(profile.Key))
                {
                    continue;
                }

                foreach (var prof in tempprofiles)
                {
                    if (profile.Value != prof.Value && profile.Value.Owner.Account == prof.Value.Owner.Account)
                    {
                        foreach (var state in prof.Value.Registry)
                        {
                            ConquestState s = profile.Value.EnsureState(state.Conquest);

                            if (state.Tier > s.Tier || state.Progress > s.Progress || state.Completed && s.Completed && state.CompletedDate < s.CompletedDate || state.WorldFirst)
                            {
                                s.CopyState(state);
                            }
                        }
                        Console.WriteLine("Consolidated Profile: " + profile.Key.RawName);
                        Profiles.Remove(prof.Value.Owner);
                    }
                }
            }
        }

	    public static void PruneConquestProfiles()
	    {
	        int count = 0;
	        foreach (var profile in Profiles.ToList())
	        {
	            var account = profile.Key.Account as Account;
	            if (account != null && account.Inactive)
	            {
	                int count1 = 0;
                    foreach (var state in profile.Value.Registry)
                    {
                        if (state.Completed)
                            count1++;
                    }
	                if (count1 == 0)
	                {
	                    Profiles.Remove(profile.Key);
	                    count++;
	                }
	            }
	        }
            Console.WriteLine("Pruned " + count + " conquest profiles.");
	    }

		public static void InvalidateRecursiveConquests(PlayerMobile pm)
		{
			ConquestProfile p;

			if (!Profiles.TryGetValue(pm, out p) || p == null)
			{
				return;
			}

			foreach (var s in p.ToArray())
			{
				if (s.Progress > 0)
				{
					CheckProgress<ConquestProgressConquest>(pm, new ConquestProgressContainer(s, 0));
				}
				
				if (s.Tier > 0)
				{
					for (int t = 0; t < s.Tier; t++)
					{
                        CheckProgress<ConquestTierCompletedConquest>(pm, new ConquestTierCompletedContainer(s, t + 1));
					}
				}

				if (s.Completed)
				{
                    CheckProgress<ConquestCompletedConquest>(pm, new ConquestCompletedContainer(s));
				}
			}
		}

		public static bool Validate(Conquest c, PlayerMobile pm)
		{
			return c != null && !c.Deleted && c.Enabled && (pm == null || (!c.Young || pm.Young));
		}

		public static void CheckProgress<TCon>(PlayerMobile pm, object args, params object[] argsN) where TCon : Conquest
		{
			if (!CMOptions.ModuleEnabled || pm == null)
			{
				return;
			}

			ConquestProfile p = EnsureProfile(pm);

			if (p == null)
			{
				return;
			}

			if (argsN != null && argsN.Length > 0)
			{
				args = new[] {args}.Merge(argsN);
			}

			bool refresh = false;

			foreach (var c in FindConquests<TCon>(c => Validate(c, pm)))
			{
				var state = p.EnsureState(c);

				if (state == null || state.Conquest != c || state.Owner.Account != pm.Account || state.Completed)
				{
					continue;
				}

			    state.User = pm;

				int progress = c.GetProgress(state, args);

				if (progress != 0)
				{
					refresh = true;
				}

				state.Progress += progress;
			}

			if (refresh)
			{
				RefreshGumps(pm);
			}
		}

		public static void RefreshGumps(PlayerMobile pm)
		{
			if (!CMOptions.ModuleEnabled || pm == null)
			{
				return;
			}

			if (CMOptions.UseCategories)
			{
				foreach (var g in SuperGump.GetInstances<ConquestStatesGump>(pm, true).Where(g => !g.IsDisposed && g.IsOpen))
				{
					g.Refresh(true);
				}
			}
			else
			{
				foreach (var g in SuperGump.GetInstances<ConquestStateListGump>(pm, true).Where(g => !g.IsDisposed && g.IsOpen))
				{
					g.Refresh(true);
				}
			}

			foreach (var g in SuperGump.GetInstances<ConquestStateGump>(pm, true).Where(g => !g.IsDisposed && g.IsOpen))
			{
				g.Refresh(true);
			}
		}

		public static void SendConquestsGump(PlayerMobile pm)
		{
			SendConquestsGump(pm, pm);
		}
		
		public static void SendConquestsGump(PlayerMobile source, PlayerMobile target)
		{
			if (source == null)
			{
				if (target == null)
				{
					return;
				}

				source = target;
			}

			if (target == null)
			{
				target = source;
			}

			if (source.Deleted || target.Deleted || (!CMOptions.ModuleEnabled && source.AccessLevel < Access))
			{
				return;
			}

			ConquestProfile p = EnsureProfile(target);

			if (p == null)
			{
				return;
			}

			p.Sync();
			
			if (CMOptions.UseCategories)
			{
                new ConquestStatesGump(source, null, p).Send();
			}
			else
			{
				new ConquestStateListGump(source, null, p).Send();
			}
		}

		/*public static void SendConquestProfilesGump(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted || (!CMOptions.ModuleEnabled && pm.AccessLevel < Access))
			{
				return;
			}

			ConquestProfile p = EnsureProfile(pm);

			if (p == null)
			{
				return;
			}

			p.Sync();

			new ConquestProfileListGump(pm).Send();
		}*/

		public static void SendConquestAdminGump(PlayerMobile pm)
		{
			if (pm != null && !pm.Deleted)
			{
				new ConquestAdminListGump(pm).Send();
			}
		}
	}
}