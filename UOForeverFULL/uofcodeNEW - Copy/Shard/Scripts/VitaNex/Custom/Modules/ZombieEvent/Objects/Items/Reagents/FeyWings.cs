#region References

using System;
using Server.Network;
using VitaNex.FX;

#endregion
namespace Server.Items
{
    public class DaemonClaw : Item
    {
        [Constructable]
        public DaemonClaw() : base(22305)
        {
            Name = "daemon claw";
        }

        public DaemonClaw(Serial serial)
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