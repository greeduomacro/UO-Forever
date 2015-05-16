#region References
using Server;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public sealed class TextPageEntry : PageEntry<TextPage>
	{
		public TextPageEntry(Page parent)
			: base(parent)
		{ }

		public TextPageEntry(Page parent, GenericReader reader)
			: base(parent, reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Label = "New Text Page";

			if (Page == null)
			{
				Page = new TextPage(this);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}