#region References
using System.Drawing;

using Server;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public sealed class RootPage : GridPage
	{
		public RootPage()
			: base(null)
		{ }

		public RootPage(GenericReader reader)
			: base(null, reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Title = ShardInfo.DisplayName;
			TitleColor = KnownColor.Gold;
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