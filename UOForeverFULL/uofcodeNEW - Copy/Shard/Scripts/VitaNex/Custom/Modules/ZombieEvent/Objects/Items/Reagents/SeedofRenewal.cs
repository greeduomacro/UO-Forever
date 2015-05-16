#region References

using System;
using Server.Network;
using VitaNex.FX;

#endregion
namespace Server.Items
{
    public class SeedofRenewal : Item
    {
        [Constructable]
        public SeedofRenewal() : base(22326)
        {
            Name = "seed of renewal";
            Hue = 61;
        }

        public SeedofRenewal(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}