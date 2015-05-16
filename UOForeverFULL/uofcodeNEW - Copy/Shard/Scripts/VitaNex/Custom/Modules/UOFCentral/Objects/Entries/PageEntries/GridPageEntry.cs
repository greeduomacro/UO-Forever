#region References
using Server;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public sealed class GridPageEntry : PageEntry<GridPage>
	{
		public GridPageEntry(Page parent)
			: base(parent)
		{ }

		public GridPageEntry(Page parent, GenericReader reader)
			: base(parent, reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Label = "New Grid Page";

			if (Page == null)
			{
				Page = new GridPage(this);
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