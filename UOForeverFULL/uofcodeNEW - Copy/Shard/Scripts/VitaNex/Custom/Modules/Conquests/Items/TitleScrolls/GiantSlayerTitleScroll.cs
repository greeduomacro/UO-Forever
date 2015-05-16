#region References

using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Engines.CustomTitles
{
    public class ConquestGiantSlayerTitle : ConquestTitleScroll
    {
        public ConquestGiantSlayerTitle(Serial serial)
            : base(serial)
        {}

        [Constructable]
        public ConquestGiantSlayerTitle()
            : base("Giant Slayer")
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();
        }
    }
}