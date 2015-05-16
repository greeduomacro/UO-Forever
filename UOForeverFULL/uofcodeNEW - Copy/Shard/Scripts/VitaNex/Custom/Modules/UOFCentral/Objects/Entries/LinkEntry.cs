#region References
using System;
using System.Drawing;

using Server;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public class LinkEntry : Entry
	{
		private string _URL;

		[CommandProperty(UOFCentral.Access)]
		public string URL
		{
			get
			{
				if (!Uri.IsWellFormedUriString(_URL, UriKind.Absolute))
				{
					_URL = "http://www.uoforever.com";
				}

				return _URL;
			}
			set
			{
				if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
				{
					_URL = value;
				}
			}
		}

		public LinkEntry(Page parent)
			: base(parent)
		{ }

		public LinkEntry(Page parent, GenericReader reader)
			: base(parent, reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Label = "New Link";
			LabelColor = KnownColor.SkyBlue;

			_URL = "http://www.uoforever.com";
		}

		public override bool Valid(UOFCentralGump g)
		{
			return base.Valid(g) && !String.IsNullOrWhiteSpace(URL);
		}

		public override void Select(UOFCentralGump g)
		{
			base.Select(g);

			string url = URL;

			if (!String.IsNullOrWhiteSpace(url))
			{
				g.User.LaunchBrowser(url);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(_URL);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			_URL = reader.ReadString();
		}
	}
}