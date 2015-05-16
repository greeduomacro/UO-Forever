#region References

using System;
using Server.Network;
using VitaNex.FX;

#endregion
namespace Server.Items
{
    public class FeyWings : Item
    {
        [Constructable]
        public FeyWings()
            : base(22310)
        {
            Name = "fey wings";
        }

        public FeyWings(Serial serial)
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