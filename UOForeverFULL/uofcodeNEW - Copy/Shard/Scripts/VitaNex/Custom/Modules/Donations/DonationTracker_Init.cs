#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.EventInvasions;
using Server.Items;
using Server.Mobiles;
using VitaNex;
using VitaNex.IO;

#endregion

namespace Server.Engines.DonationsTracker
{
    [CoreService("Donations Tracker", "1.0.0", TaskPriority.High)]
    public static partial class DonationsTracker
    {
        static DonationsTracker()
        {
            CSOptions = new DonationsTrackerOptions();

            DonationProfiles = new BinaryDataStore<String, DonationProfile>(
                VitaNexCore.SavesDirectory + "/DonationsTracker", "DonationProfiles")
            {
                Async = true,
                OnSerialize = SerializeDonationProfiles,
                OnDeserialize = DeserializeDonationProfiles
            };
        }

        private static void CSSave()
        {
            VitaNexCore.TryCatchGet(DonationProfiles.Export, x => x.ToConsole());
        }

        private static void CSLoad()
        {
            VitaNexCore.TryCatchGet(DonationProfiles.Import, x => x.ToConsole());
        }

        private static void CSInvoke()
        {
            CommandUtility.Register(
                "Donations",
                AccessLevel.Developer,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }
                    new DonationsUI(e.Mobile as PlayerMobile).Send();
                });
        }

        private static bool SerializeDonationProfiles(GenericWriter writer)
        {
            writer.SetVersion(0);

            writer.WriteBlockDictionary(
                DonationProfiles,
                (pm, p) =>
                {
                    writer.Write(pm);

                    p.Serialize(writer);
                });

            return true;
        }

        private static bool DeserializeDonationProfiles(GenericReader reader)
        {
            reader.GetVersion();

            reader.ReadBlockDictionary(
                () =>
                {
                    var e = reader.ReadString();

                    var p = new DonationProfile(reader);

                    return new KeyValuePair<string, DonationProfile>(e, p);
                },
                DonationProfiles);

            return true;
        }
    }
}