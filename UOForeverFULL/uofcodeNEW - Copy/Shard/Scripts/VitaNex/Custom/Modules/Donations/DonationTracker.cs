#region References

using System;
using System.Collections.Generic;
using Server.Engines.EventInvasions;
using Server.Factions;
using Server.Mobiles;
using VitaNex;
using VitaNex.IO;
using VitaNex.Notify;

#endregion

namespace Server.Engines.DonationsTracker
{
    public static partial class DonationsTracker
    {
        public const AccessLevel Access = AccessLevel.Administrator;

        public static DonationsTrackerOptions CSOptions { get; private set; }

        public static BinaryDataStore<String, DonationProfile> DonationProfiles { get; private set; }

        public static DonationProfile EnsureProfile(string email)
        {
            DonationProfile p;

            if (!DonationProfiles.TryGetValue(email, out p))
            {
                DonationProfiles.Add(email, p = new DonationProfile(email));
            }
            else if (p == null)
            {
                DonationProfiles[email] = p = new DonationProfile(email);
            }

            return p;
        }
    }

}