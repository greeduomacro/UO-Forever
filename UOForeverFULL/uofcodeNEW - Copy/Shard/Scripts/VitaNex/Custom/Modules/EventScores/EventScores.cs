#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Accounting;
using Server.Engines.XmlSpawner2;
using Server.Factions;
using Server.Mobiles;
using VitaNex;
using VitaNex.IO;
using VitaNex.Notify;

#endregion

namespace Server.Engines.EventScores
{
    public static partial class EventScores
    {
        public const AccessLevel Access = AccessLevel.EventMaster;

        public static EventScoresOptions CSOptions { get; private set; }

        public static BinaryDataStore<PlayerMobile, PlayerEventScoreProfile> PlayerProfiles { get; private set; }

        public static PlayerEventScoreProfile EnsureProfile(PlayerMobile pm)
        {
            PlayerEventScoreProfile p;

            var account = pm.Account as Account;

            if (account == null)
            {
                Console.WriteLine("NULL ACCOUNT?!");
                return null;
            }

            if (!PlayerProfiles.TryGetValue(pm, out p))
            {
                foreach (PlayerMobile mobile in account.Mobiles.OfType<PlayerMobile>())
                {
                    if (PlayerProfiles.TryGetValue(mobile, out p))
                    {
                        break;
                    }
                }
                if (p == null)
                {
                    PlayerProfiles.Add(pm, p = new PlayerEventScoreProfile(pm));
                }
            }
            else if (p == null)
            {
                PlayerProfiles[pm] = p = new PlayerEventScoreProfile(pm);
            }

            return p;
        }

        public static List <PlayerEventScoreProfile> SortedProfiles()
        {
            List<PlayerEventScoreProfile> profiles = PlayerProfiles.Values.OrderByDescending(x => x.OverallScore).ToList();

            return profiles;
        }
    }
}