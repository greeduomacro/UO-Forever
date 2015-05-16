#region References
using System;
using System.Drawing;
#endregion

namespace Server.Items
{
	public class MoonglowMead : PitcherOfMead
	{
		[Constructable]
		public MoonglowMead()
		{
			Kick = 5;
			ContentName = "Moonglow Mead";
		}

		public MoonglowMead(Serial serial)
			: base(serial)
		{ }

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			LabelTo(from, 51, "\"Mixed with organic Moonglow reagents, best when served chilled.\"");
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add("\"Mixed with organic Moonglow reagents,\nbest when served chilled.\"".WrapUOHtmlColor(Color.Gold));
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					break;
			}
		}
	}
}