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
    public sealed class DonationEntry : PropertyObject
    {
        [CommandProperty(DonationsTracker.Access, true)]
        public DonationEntrySerial UID { get; private set; }

        [CommandProperty(DonationsTracker.Access, true)]
        public string Character { get; set; }

        [CommandProperty(DonationsTracker.Access, true)]
        public int AmountDonated { get; set; }

        [CommandProperty(DonationsTracker.Access, true)]
        public int CoinsGiven { get; set; }

        [CommandProperty(DonationsTracker.Access, true)]
        public DateTime Date { get; set; }

        public DonationEntry(string character, int amount, int coins, DateTime date)
        {
            UID = new DonationEntrySerial();

            Character = character;
            AmountDonated = amount;
            CoinsGiven = coins;
            Date = date;
        }

        public DonationEntry(GenericReader reader)
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
                    writer.Write(Character);
                    writer.Write(AmountDonated);
                    writer.Write(CoinsGiven);
                    writer.Write(Date);
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            UID = new DonationEntrySerial(reader);

            switch (version)
            {
                case 0:
                {
                    Character = reader.ReadString();
                    AmountDonated = reader.ReadInt();
                    CoinsGiven = reader.ReadInt();
                    Date = reader.ReadDateTime();
                }
                    break;
            }
        }
    }
}