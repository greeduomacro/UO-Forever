#region References

using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Engines.CustomTitles
{
    public class ConquestSigilKeeperTitle : ConquestTitleScroll
    {
        public ConquestSigilKeeperTitle(Serial serial)
            : base(serial)
        {}

        [Constructable]
        public ConquestSigilKeeperTitle()
            : base("The Sigil Keeper")
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