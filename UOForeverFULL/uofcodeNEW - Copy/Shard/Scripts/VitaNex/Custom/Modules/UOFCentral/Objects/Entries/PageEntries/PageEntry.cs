#region References
using System;

using Server;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public abstract class PageEntry<TPage> : Entry
		where TPage : Page
	{
		[CommandProperty(UOFCentral.Access)]
		public TPage Page { get; set; }

		public PageEntry(Page parent)
			: base(parent)
		{ }

		public PageEntry(Page parent, GenericReader reader)
			: base(parent, reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Label = "New Page";

			if (Page == null)
			{
				Page = typeof(TPage).CreateInstanceSafe<TPage>(this);
			}
		}

		public override bool Valid(UOFCentralGump g)
		{
			return base.Valid(g) && Page != null;
		}

		public override void Select(UOFCentralGump g)
		{
			g.SetPage(Page);

			base.Select(g);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.WriteType(
				Page,
				t =>
				{
					if (t != null)
					{
						Page.Serialize(writer);
					}
				});
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Page = reader.ReadTypeCreate<TPage>(this, reader);
		}
	}
}