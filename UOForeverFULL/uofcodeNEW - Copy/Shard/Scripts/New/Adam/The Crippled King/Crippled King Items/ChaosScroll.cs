#region References
using System;

using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Items
{
	public sealed class ChaosScroll : Item
	{
		[Constructable]
		public ChaosScroll()
			: base(0x2831)
		{
			Name = "an indecipherable chaos scroll";
			Weight = 2;
			Hue = 1194;
		}

        public ChaosScroll(Serial serial)
			: base(serial)
		{ }

		public override void OnSingleClick(Mobile m)
		{
		    base.OnSingleClick(m);
			LabelTo(m, "The archaic glyphs on this scroll are meaningless to you.", 54);
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