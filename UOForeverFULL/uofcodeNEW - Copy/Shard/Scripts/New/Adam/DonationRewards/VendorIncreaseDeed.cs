#region References

using System.Linq;
using Server.Multis;

#endregion

namespace Server.Scripts.Items.Deeds
{
    public class VendorIncreaseDeed : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public double VendorIncrease { get; set; }

        [Constructable]
        public VendorIncreaseDeed() : base(0x14F0)
        {
            Name = "a vendor increase deed";
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 1175;
        }

        public VendorIncreaseDeed(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(VendorIncrease);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;

            reader.ReadInt();

            VendorIncrease = reader.ReadDouble();
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "Vendor Increase Multiplier: " + VendorIncrease * 100 + "%", 54);
        }

        public override bool DisplayLootType { get { return false; } }

        public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target
        {
            BaseHouse house = BaseHouse.GetHouses(from).FirstOrDefault();
            if (house == null)
            {
                from.SendMessage(54, "You must own a house before using this deed!");
            }
            else
            {
                if (house.VendorMultiplier >= 1.5)
                {
                    from.SendMessage(54, "You cannot increase your house vendor multiplier beyond 50%.");
                }
                else
                {
                    if (house.VendorMultiplier < 1.0)
                    {
                        house.VendorMultiplier = 1.0;
                    }
                    house.VendorMultiplier = house.VendorMultiplier + VendorIncrease;
                    if (house.VendorMultiplier > 1.5)
                    {
                        house.VendorMultiplier = 1.5;
                    }
                    from.SendMessage(54,
                        "Your house now has a vendor multipler of: " + house.VendorMultiplier * 100 + "%");
                    Consume();
                }
            }
        }
    }
}