#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Factions;
using Server.Mobiles;
using VitaNex;
using VitaNex.IO;
using VitaNex.Notify;

#endregion

namespace Server.Engines.EventInvasions
{
    public static partial class EventInvasions
    {
        public const AccessLevel Access = AccessLevel.EventMaster;

        public static EventInvasionsOptions CSOptions { get; private set; }

        public static BinaryDataStore<InvasionSerial, Invasion> Invasions { get; private set; }
        public static BinaryDataStore<PlayerMobile, PlayerInvasionProfile> PlayerProfiles { get; private set; }

        public static PlayerInvasionProfile EnsureProfile(PlayerMobile pm)
        {
            PlayerInvasionProfile p;

            if (!PlayerProfiles.TryGetValue(pm, out p))
            {
                PlayerProfiles.Add(pm, p = new PlayerInvasionProfile(pm));
            }
            else if (p == null)
            {
                PlayerProfiles[pm] = p = new PlayerInvasionProfile(pm);
            }

            return p;
        }

        public static void GenerateInvasion(string invasionname, string regionname, List<Level> levels, bool gates)
        {
            var invasion = new Invasion(invasionname, regionname, DateTime.UtcNow, levels, gates);
            Invasions.Add(invasion.UID, invasion);
        }

        public static void GenerateInvasion(Invasion invasion)
        {
            var invasionnew = new Invasion(invasion);
            Invasions.Add(invasionnew.UID, invasionnew);
        }

        public static Invasion GetInvasion(InvasionSerial uid)
        {
            if (Invasions.ContainsKey(uid))
                return Invasions[uid];
            return null;
        }

        public static Invasion GetInvasionByName(string name)
        {
            var invasion = Invasions.Values.Where(x => x.InvasionName == name).FirstOrDefault();
            return invasion;
        }

        public static int AsHue(this InvasionStatus status)
        {
            switch (status)
            {
                case InvasionStatus.Waiting:
                    return 1258;
                case InvasionStatus.Running:
                    return 63;
                case InvasionStatus.Finished:
                    return 137;
                default:
                    return 0;
            }
        }
    }
}