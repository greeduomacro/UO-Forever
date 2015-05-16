#region References
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Engines.CustomTitles
{
	public sealed class ConquestHueScrollOrange: ConquestHueScroll
	{
        [Constructable]
        public ConquestHueScrollOrange()
            : base(1260)
        {
        }

        public ConquestHueScrollOrange(Serial serial)
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