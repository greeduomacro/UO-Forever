#region References



#endregion

namespace Server.Items
{
    public class VileTentacles : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ChoppedUp { get; set; }

        [Constructable]
        public VileTentacles() : base(22311)
        {
            Name = "vile tentacles";
        }

        public VileTentacles(Serial serial)
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