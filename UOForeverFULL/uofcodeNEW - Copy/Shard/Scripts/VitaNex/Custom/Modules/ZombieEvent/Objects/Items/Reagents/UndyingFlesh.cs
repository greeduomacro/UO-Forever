#region References

using System;
using Server.Network;
using VitaNex.FX;

#endregion
namespace Server.Items
{
    public class UndyingFlesh : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ChoppedUp { get; set; }

        [Constructable]
        public UndyingFlesh()
            : base(22321)
        {
            Name = "undying flesh";
        }

        public UndyingFlesh(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(ChoppedUp);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            ChoppedUp = reader.ReadBool();
        }
    }
}