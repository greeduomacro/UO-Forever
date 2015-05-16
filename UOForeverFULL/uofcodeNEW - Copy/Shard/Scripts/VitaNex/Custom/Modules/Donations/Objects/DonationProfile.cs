#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using VitaNex;
using VitaNex.Notify;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.DonationsTracker
{
    public sealed class DonationProfile : PropertyObject
    {
        [CommandProperty(DonationsTracker.Access, true)]
        public DonationProfileSerial UID { get; private set; }

        [CommandProperty(DonationsTracker.Access, true)]
        public string Email { get; set; }

        [CommandProperty(DonationsTracker.Access, true)]
        public int TotalDonations { get; set; }

        [CommandProperty(DonationsTracker.Access, true)]
        public List<DonationEntry> DonationEntries { get; set; }

        public DonationProfile(string email)
        {
            UID = new DonationProfileSerial();

            DonationEntries = new List<DonationEntry>();

            Email = email;
        }

        public DonationProfile(GenericReader reader)
            : base(reader)
        {}

        public override void Reset()
        {}

        public override void Clear()
        {}


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

            UID.Serialize(writer);

            switch (version)
            {
                case 0:
                {
                    writer.Write(Email);
                    writer.Write(TotalDonations);

                    writer.Write(DonationEntries.Count);

                    if (DonationEntries.Count > 0)
                    {
                        foreach (DonationEntry entry in DonationEntries)
                        {
                            entry.Serialize(writer);
                        }
                    }

                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            UID = new DonationProfileSerial(reader);

            DonationEntries = new List<DonationEntry>();

            switch (version)
            {
                case 0:
                {
                    Email = reader.ReadString();
                    TotalDonations = reader.ReadInt();

                    int count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var entry = new DonationEntry(reader);
                            DonationEntries.Add(entry);
                        }
                    }
                }
                    break;
            }
        }
    }
}