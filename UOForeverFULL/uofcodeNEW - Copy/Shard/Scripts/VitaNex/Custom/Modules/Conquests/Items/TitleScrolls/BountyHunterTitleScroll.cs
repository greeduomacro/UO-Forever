#region References

using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Engines.CustomTitles
{
    public class ConquestBountyHunterrTitle : ConquestTitleScroll
    {
        public ConquestBountyHunterrTitle(Serial serial)
            : base(serial)
        {}

        [Constructable]
        public ConquestBountyHunterrTitle()
            : base("Bounty Hunter")
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