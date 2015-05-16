#region References

using System;
using Server.Network;
using VitaNex.FX;

#endregion
namespace Server.Items
{
    public class VialofVitriol : Item
    {
        [Constructable]
        public VialofVitriol() : base(22307)
        {
            Name = "vial of vitriol";
        }

        public VialofVitriol(Serial serial)
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