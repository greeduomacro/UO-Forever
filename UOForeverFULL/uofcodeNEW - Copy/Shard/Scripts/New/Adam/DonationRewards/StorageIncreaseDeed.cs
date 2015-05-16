#region References

using System.Linq;
using Server.Multis;

#endregion

namespace Server.Scripts.Items.Deeds
{
    public class StorageIncreaseDeed : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public double StorageIncrease { get; set; }

        [Constructable]
        public StorageIncreaseDeed() : base(0x14F0)
        {
            Name = "a house storage increase deed";
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 1175;
        }

        public StorageIncreaseDeed(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(StorageIncrease);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();

            StorageIncrease = reader.ReadDouble();
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Storage Increase Multiplier: " + StorageIncrease * 100 + "%", 54);
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
                if (house.StorageMultiplier >= 1.5)
                {
                    from.SendMessage(54, "You cannot increase your house storage multiplier beyond 50%.");
                }
                else
                {
                    if (house.StorageMultiplier < 1.0)
                    {
                        house.StorageMultiplier = 1.0;
                    }
                    house.StorageMultiplier = house.StorageMultiplier + StorageIncrease;
                    if (house.StorageMultiplier > 1.5)
                    {
                        house.StorageMultiplier = 1.5;
                    }
                    from.SendMessage(54,
                        "Your house now has a storage multipler of: " + house.StorageMultiplier * 100 + "%");
                    Consume();
                }
            }
        }
    }
}