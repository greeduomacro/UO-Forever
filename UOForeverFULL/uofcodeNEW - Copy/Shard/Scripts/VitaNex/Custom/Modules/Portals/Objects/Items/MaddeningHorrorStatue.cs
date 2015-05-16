#region References



#endregion

namespace Server.Items
{
    public class MaddeninghorrorStatue : Item
    {
        [Constructable]
        public MaddeninghorrorStatue() : base(0x2116)
        {
            Name = "Maddening Horror Statue";
            Movable = true;
            Hue = 1196;
        }

        public MaddeninghorrorStatue(Serial serial)
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