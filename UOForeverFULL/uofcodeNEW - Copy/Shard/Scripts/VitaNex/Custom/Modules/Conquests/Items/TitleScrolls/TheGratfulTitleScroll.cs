#region References

using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Engines.CustomTitles
{
    public class Conquestgratefulitle : ConquestTitleScroll
    {
        public Conquestgratefulitle(Serial serial)
            : base(serial)
        {}

        [Constructable]
        public Conquestgratefulitle()
            : base("The Grateful")
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