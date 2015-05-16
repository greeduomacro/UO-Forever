#region References
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Engines.CustomTitles
{
	public sealed class ConquestHueScrollPurpleBlue: ConquestHueScroll
	{
        [Constructable]
        public ConquestHueScrollPurpleBlue()
            : base(1462)
        {
        }

        public ConquestHueScrollPurpleBlue(Serial serial)
			: base(serial)
		{ }

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