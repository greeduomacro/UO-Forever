#region References
using System;

using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Items
{
	public sealed class StoneFragment : Item
	{
		[Constructable]
		public StoneFragment()
			: base(22328)
		{
			Name = "stone shards";
			Weight = 2;
			Hue = 2407;
		}

        public StoneFragment(Serial serial)
			: base(serial)
		{ }

		public override void OnSingleClick(Mobile m)
		{
		    base.OnSingleClick(m);
			LabelTo(m, "The stone shards pulse and quiver with latent energy.", 54);
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