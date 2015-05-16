using System;

namespace Server.Items
{

    [FlipableAttribute(0xC6A, 0xC6B)]
    public class HalloweenPumpkin : Food
    {
        [Constructable]
        public HalloweenPumpkin()
            : this(1)
        {
        }

        [Constructable]
        public HalloweenPumpkin(int amount)
            : base(amount, 0xC6A)
        {
            this.Weight = 1.0;
            this.FillFactor = 8;
        }

        public HalloweenPumpkin(Serial serial)
            : base(serial)
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
            {
                if (FillFactor == 4)
                    FillFactor = 8;

                if (Weight == 5.0)
                    Weight = 1.0;
            }
        }
    }
}
