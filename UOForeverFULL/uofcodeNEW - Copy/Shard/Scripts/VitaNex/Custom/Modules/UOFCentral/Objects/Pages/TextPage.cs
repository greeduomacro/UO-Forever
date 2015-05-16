#region References
using System;
using System.Drawing;

using Server;

using VitaNex.SuperGumps;
using VitaNex.Text;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public sealed class TextPage : Page
	{
		[CommandProperty(UOFCentral.Access)]
		public string Text { get; set; }

		[CommandProperty(UOFCentral.Access)]
		public KnownColor TextColor { get; set; }

		[CommandProperty(UOFCentral.Access)]
		public bool Scrollbar { get; set; }

		[CommandProperty(UOFCentral.Access)]
		public bool Background { get; set; }

		public TextPage(Entry parent)
			: base(parent)
		{ }

		public TextPage(Entry parent, GenericReader reader)
			: base(parent, reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Title = "New Text Page";

			Text = "Text, HTML or BBC...[br][br]Edit text with Notepad and paste it here for best results.";
			TextColor = KnownColor.White;

			Scrollbar = true;
			Background = false;
		}

		public override void CompileLayout(UOFCentralGump g, SuperGumpLayout layout)
		{
			base.CompileLayout(g, layout);

			if (g == null || layout == null)
			{
				return;
			}

			if (g.Edit)
			{
				layout.Add(
					"page/text",
					() =>
					{
						g.AddImageTiled(120, 160, 410, 200, 2624);
						g.AddButton(
							120,
							160,
							2640,
							2641,
							b =>
							{
								Text = String.Empty;
								g.Refresh(true);
							});
						g.AddTooltip(ButtonIconInfo.GetTooltip(ButtonIcon.Clear));
						g.AddImageTiled(150, 153, 385, 2, 30072);

						if (Background)
						{
							g.AddBackground(150, 160, 380, 200, 9350);
						}

						g.AddTextEntry(155, 165, 370, 190, g.TextHue, Text, (e, t) => Text = t);
					});
			}
			else
			{
				layout.Add(
					"page/text",
					() =>
					{
						string text = Text.ParseBBCode(TextColor.ToColor()).WrapUOHtmlColor(TextColor, false);

						g.AddImageTiled(120, 160, 410, 200, 2624);
						g.AddHtml(125, 165, 400, 190, text, Background, Scrollbar);
					});
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.Write(Text);
						writer.WriteFlag(TextColor);
						writer.Write(Scrollbar);
						writer.Write(Background);
					}
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
					{
						Text = reader.ReadString();
						TextColor = reader.ReadFlag<KnownColor>();
						Scrollbar = reader.ReadBool();
						Background = reader.ReadBool();
					}
					break;
			}
		}
	}
}