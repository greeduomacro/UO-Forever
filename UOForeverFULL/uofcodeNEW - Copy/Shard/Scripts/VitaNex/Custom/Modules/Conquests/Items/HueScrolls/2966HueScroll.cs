#region References
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Engines.CustomTitles
{
	public sealed class ConquestHueScrollJade : ConquestHueScroll
	{
        [Constructable]
        public ConquestHueScrollJade()
            : base(2966)
        {
        }

        public ConquestHueScrollJade(Serial serial)
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