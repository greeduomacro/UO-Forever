#region References
using System;

using Server;
using Server.Gumps;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Text;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public class GridPage : Page
	{
		public GridPageGrid Grid { get; private set; }

		public GridPage(Entry parent)
			: base(parent)
		{ }

		public GridPage(Entry parent, GenericReader reader)
			: base(parent, reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Title = "New Grid Page";

			Grid = new GridPageGrid(this);
		}

		public override void CompileLayout(UOFCentralGump g, SuperGumpLayout layout)
		{
			base.CompileLayout(g, layout);

			if (g != null && layout != null)
			{
				Grid.ForEach((c, x, y) => CompileEntry(g, layout, x, y, c));
			}
		}

		private void SendCreateMenu(UOFCentralGump g, int gx, int gy)
		{
			var o = new MenuGumpOptions();

			foreach (Type t in UOFCentral.EntryTypes)
			{
				o.AppendEntry(
					new ListGumpEntry(
						"New " + t.Name.Replace("Entry", String.Empty).SpaceWords(),
						() =>
						{
							Grid.SetContent(gx, gy, t.CreateInstanceSafe<Entry>(this));
							g.Refresh(true);
						}));
			}

			new MenuGump(g.User, g.Refresh(), o).Send();
		}

		public virtual void CompileEntry(UOFCentralGump g, SuperGumpLayout layout, int gx, int gy, Entry entry)
		{
			if (g == null || layout == null)
			{
				return;
			}

			bool valid = entry != null && entry.Valid(g);

			if (!g.Edit && !valid)
			{
				return;
			}

			int x = 120 + (gx * 104);
			int y = 160 + (gy * 104);

			string root = "page/grid/entry/" + (entry != null ? entry.UID.Value : gx + "," + gy);

			if (entry != null)
			{
				if (g.Edit)
				{
					layout.Add(root + "/select", () => g.AddButton(x + 26, y + 4, 2297, 2297, b => entry.Select(g)));
					g.AddTooltip(ButtonIconInfo.GetTooltip(ButtonIcon.Edit));
				}
				else
				{
					layout.Add(root + "/select", () => g.AddButton(x + 4, y + 4, 5104, 5104, b => entry.Select(g)));
				}
			}
			else if (g.Edit)
			{
				layout.Add(
					root + "/select",
					() =>
					{
						g.AddButton(x + 4, y + 4, 5104, 5104, b => SendCreateMenu(g, gx, gy));
						g.AddTooltip(ButtonIconInfo.GetTooltip(ButtonIcon.Add));
					});
			}

			if (entry != null || g.Edit)
			{
				int borderID;

				if (entry == null)
				{
                    borderID = 30072; // Red 30073
				}
				else if (valid)
				{
                    borderID = 30072; // Green
				}
				else
				{
					borderID = 30072; // Blue
				}

				layout.Add(
					root + "/bg",
					() =>
					{
						g.AddImageTiled(x, y, 100, 100, 2624);

						g.AddImageTiled(x, y, 100, 2, borderID); //TL -> TR
						g.AddImageTiled(x + 98, y, 2, 100, borderID); //TR -> BR
						g.AddImageTiled(x, y, 2, 100, borderID); //TL -> BL
						g.AddImageTiled(x, y + 98, 100, 2, borderID); //BL -> BR
					});
			}

			if (entry == null)
			{
				if (g.Edit)
				{
					layout.Add(
						root + "/add",
						() =>
						{
							g.AddImageTiled(x + 2, y + 2, 20, 20, 2624);
							g.AddButton(x + 4, y + 4, 55, 55, b => SendCreateMenu(g, gx, gy));
							g.AddTooltip(ButtonIconInfo.GetTooltip(ButtonIcon.Add));
						});
				}
			}
			else
			{
				if (g.Edit)
				{
					if (entry.ArtID > 0)
					{
						layout.Add(
							root + "/icon", () => g.AddItem(x + 2 + entry.ArtOffset.X, y + 2 + entry.ArtOffset.Y, entry.ArtID, entry.ArtHue));
					}

					layout.Add(
						root + "/delete",
						() =>
						{
							g.AddImageTiled(x + 2, y + 2, 20, 20, 2624);
							g.AddButton(
								x + 4,
								y + 4,
								56,
								56,
								b =>
								{
									g.Refresh();

									new ConfirmDialogGump(g.User)
									{
										Title = "Confirm Clear?",
										Html =
											"Removing this entry will delete all data associated with it, " +
											"including any changes you have made.\nThis action can not be undone!\nClick OK to confirm.",
										AcceptHandler = cb =>
										{
											Grid.SetContent(gx, gy, null);
											g.Refresh(true);
										}
									}.Send();
								});
							g.AddTooltip(ButtonIconInfo.GetTooltip(ButtonIcon.Remove));
						});

					layout.Add(
						root + "/props",
						() =>
						{
							g.AddImageTiled(x + 78, y + 2, 20, 20, 2624);
							g.AddButton(
								x + 80,
								y + 4,
								4033,
								4032,
								b =>
								{
									g.Refresh();
									g.User.SendGump(new PropertiesGump(g.User, entry));
								});
							g.AddTooltip(ButtonIconInfo.GetTooltip(ButtonIcon.Properties));
						});

					layout.Add(
						root + "/label",
						() =>
						{
							g.AddImageTiled(x + 5, y + 55, 90, 40, 2624);
							g.AddTextEntryLimited(
								x + 5, y + 55, 90, 40, g.TextHue, entry.Label, 50, (e, t) => entry.Label = t ?? String.Empty);
						});
				}
				else
				{
					bool icon = entry.ArtID > 0;

					if (icon)
					{
						layout.Add(
							root + "/icon", () => g.AddItem(x + 2 + entry.ArtOffset.X, y + 2 + entry.ArtOffset.Y, entry.ArtID, entry.ArtHue));
					}

					layout.Add(
						root + "/label",
						() =>
						{
							string label =
								entry.Label.ParseBBCode(entry.LabelColor.ToColor())
									 .WrapUOHtmlTag("CENTER")
									 .WrapUOHtmlColor(entry.LabelColor, false);

							g.AddImageTiled(x + 5, y + (icon ? 55 : 5), 90, icon ? 40 : 90, 2624);
							g.AddHtml(x + 5, y + (icon ? 55 : 5), 90, icon ? 40 : 90, label, false, false);
						});
				}
			}

			if (g.Edit)
			{
				// Move entries around cells.
				// Left/Right/Up/Down
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			Grid.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Grid = new GridPageGrid(this, reader);
		}
	}

	public sealed class GridPageGrid : Grid<Entry>
	{
		public GridPage Parent { get; private set; }

		public GridPageGrid(GridPage parent)
			: base(4, 2)
		{
			Parent = parent;

			DefaultValue = null;
		}

		public GridPageGrid(GridPage parent, GenericReader reader)
			: base(reader)
		{
			Parent = parent;
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

		public override void SerializeContent(GenericWriter writer, Entry content, int x, int y)
		{
			writer.WriteType(
				content,
				t =>
				{
					if (t != null)
					{
						content.Serialize(writer);
					}
				});
		}

		public override Entry DeserializeContent(GenericReader reader, Type type, int x, int y)
		{
			return reader.ReadTypeCreate<Entry>(Parent, reader);
		}
	}
}