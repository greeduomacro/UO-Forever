#region References

using Server;

#endregion

namespace Knives.TownHouses
{
    public class RentalContractCopy : Item
    {
        private readonly RentalContract c_Contract;

        public RentalContractCopy(RentalContract contract)
        {
            Name = "rental contract copy";
            ItemID = 0x14F0;
            c_Contract = contract;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (c_Contract == null || c_Contract.Deleted)
            {
                Delete();
                return;
            }

            c_Contract.OnDoubleClick(m);
        }

        public RentalContractCopy(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}