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

namespace Server.Engines.Portals
{
    public static partial class Portals
    {
        public const AccessLevel Access = AccessLevel.Administrator;

        public static PortalsOptions CSOptions { get; private set; }

        public static BinaryDataStore<PortalSerial, Portal> PortalList { get; private set; }
        public static BinaryDataStore<PlayerMobile, PlayerPortalProfile> PlayerProfiles { get; private set; }

        public static PlayerPortalProfile EnsureProfile(PlayerMobile pm)
        {
            PlayerPortalProfile p;

            if (!PlayerProfiles.TryGetValue(pm, out p))
            {
                PlayerProfiles.Add(pm, p = new PlayerPortalProfile(pm));
            }
            else if (p == null)
            {
                PlayerProfiles[pm] = p = new PlayerPortalProfile(pm);
            }

            return p;
        }

        public static void GeneratePortal()
        {
            PortalType type = PortalType.Undead;
            Portal temp;
            bool validtype = false;
            int count = 0;

            while (!validtype)
            {
                int random = Utility.Random(0, 6);
                type = (PortalType) random;
                temp = PortalList.Values.FirstOrDefault(x => x.Status == PortalStatus.Running && x.PortalType == type);
                if (temp == null)
                {
                    validtype = true;
                }
                count++;
                if (count >= 100)
                {
                    Console.WriteLine("Failed to create valid portal!");
                    return;
                }

            }
            var portal = new Portal(type, DateTime.UtcNow);
            PortalList.Add(portal.UID, portal);
        }

        public static Portal GetPortal(PortalSerial uid)
        {
            if (PortalList.ContainsKey(uid))
                return PortalList[uid];
            return null;
        }

        public static void StartPortals()
        {
            int count = PortalList.Values.Count(portal => portal.Status == PortalStatus.Running);
            for (int i = count; i < CSOptions.NumberofPortals; i++)
            {
                GeneratePortal();
            }            
        }

        public static void StopPortals()
        {
            foreach (Portal portal in PortalList.Values.Where(portal => portal.Status == PortalStatus.Running).ToList())
            {
                portal.StopPortal();
            }
        }

        public static int AsHue(this PortalStatus status)
        {
            switch (status)
            {
                case PortalStatus.Running:
                    return 63;
                case PortalStatus.Finished:
                    return 137;
                default:
                    return 0;
            }
        }
    }
}