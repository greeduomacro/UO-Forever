#region References
using System;
using System.Collections.Generic;
using System.Drawing;

using Server;
using Server.Gumps;

using VitaNex.Crypto;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Text;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public abstract class Page : PropertyObject, IEquatable<Page>
	{
		[CommandProperty(UOFCentral.Access, true)]
		public Entry ParentEntry { get; private set; }

		[CommandProperty(UOFCentral.Access, true)]
		public CryptoHashCode UID { get; private set; }

		[CommandProperty(UOFCentral.Access)]
		public string Title { get; set; }

		[CommandProperty(UOFCentral.Access)]
		public KnownColor TitleColor { get; set; }

		public Page(Entry parent)
		{
			ParentEntry = parent;

			UID = new CryptoHashCode(
				CryptoHashType.MD5, GetType().Name + "+" + TimeStamp.UtcNow.Stamp + "+" + Utility.RandomDouble());

			EnsureDefaults();
		}

		public Page(Entry parent, GenericReader reader)
			: base(reader)
		{
			ParentEntry = parent;
		}

		public virtual void EnsureDefaults()
		{
			Title = "New Page";
			TitleColor = KnownColor.White;
		}

		public override void Clear()
		{
			EnsureDefaults();
		}

		public override void Reset()
		{
			EnsureDefaults();
		}

		public virtual void CompileToolbar(UOFCentralGump g, Dictionary<string, ToolbarButton> buttons)
		{
			if (g == null || buttons == null)
			{
				return;
			}

			if (g.History.Count > 0)
			{
				buttons.Add("prev", new ToolbarButton(ButtonIcon.Previous, g.PreviousPage));
			}

			if (g.User.AccessLevel < UOFCentral.CMOptions.EditAccess)
			{
				return;
			}

			if (g.Edit)
			{
				// Clear
				buttons.Add(
					"clear",
					new ToolbarButton(
						ButtonIcon.Clear,
						() =>
						{
							g.Refresh();

							new ConfirmDialogGump(g.User)
							{
								Title = "Confirm Clear?",
								Html =
									"Clearing this page will delete all data associated with it, " +
									"including any changes you have made.\nThis action can not be undone!\nClick OK to confirm.",
								AcceptHandler = b =>
								{
									Clear();
									g.Refresh(true);
								}
							}.Send();
						}));

				// Properties
				buttons.Add(
					"props",
					new ToolbarButton(
						ButtonIcon.Properties,
						() =>
						{
							g.Refresh();
							g.User.SendGump(new PropertiesGump(g.User, this));
						}));

				// Done
				buttons.Add(
					"okay",
					new ToolbarButton(
						ButtonIcon.Okay,
						() =>
						{
							g.Edit = false;
							g.Refresh(true);
						}));
			}
			else
			{
				// Edit
				buttons.Add(
					"edit",
					new ToolbarButton(
						ButtonIcon.Edit,
						() =>
						{
							g.Edit = true;
							g.Refresh(true);

							if (g != UOFCentralGump.Editing)
							{
								new NoticeDialogGump(g.User)
								{
									Title = "Inaccessible",
									Html =
										String.Format(
											"The Central Gump is currently being edited by {0}, please wait until they have finished.",
											UOFCentralGump.Editing.User.RawName)
								}.Send();
							}
						}));
			}
		}

		public virtual void CompileLayout(UOFCentralGump g, SuperGumpLayout layout)
		{
			if (g == null || layout == null)
			{
				return;
			}

			// Page Bounds: (110, 120 -> 540, 370) (430 x 250)

			if (g.Edit)
			{
				layout.Add(
					"page/title",
					() =>
					{
						g.AddImageTiled(120, 130, 410, 25, 2624);
						g.AddButton(
							120,
							130,
							2640,
							2641,
							b =>
							{
								Title = String.Empty;
								g.Refresh(true);
							});
						g.AddTooltip(ButtonIconInfo.GetTooltip(ButtonIcon.Clear));
						g.AddImageTiled(150, 153, 380, 2, 30072);
						g.AddTextEntryLimited(155, 130, 370, 25, g.TextHue, Title, 100, (e, t) => Title = t ?? String.Empty);
					});
			}
			else
			{
				layout.Add(
					"page/title",
					() =>
					{
						string title = Title.ParseBBCode(TitleColor.ToColor()).WrapUOHtmlTag("CENTER").WrapUOHtmlColor(TitleColor, false);

						g.AddImageTiled(120, 130, 410, 25, 2624);
						g.AddImageTiled(120, 153, 410, 2, 30072);
						g.AddHtml(125, 130, 400, 40, title, false, false);
					});
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			UID.Serialize(writer);

			switch (version)
			{
				case 0:
					{
						writer.Write(Title);
						writer.WriteFlag(TitleColor);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			UID = new CryptoHashCode(reader);

			switch (version)
			{
				case 0:
					{
						Title = reader.ReadString();
						TitleColor = reader.ReadFlag<KnownColor>();
					}
					break;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Page && Equals((Page)obj);
		}

		public bool Equals(Page other)
		{
			return !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || UID.Equals(other.UID));
		}

		public override int GetHashCode()
		{
			return UID.GetHashCode();
		}

		public override string ToString()
		{
			return Title;
		}

		public static bool operator ==(Page left, Page right)
		{
			return ReferenceEquals(null, left) ? ReferenceEquals(null, right) : left.Equals(right);
		}

		public static bool operator !=(Page left, Page right)
		{
			return ReferenceEquals(null, left) ? !ReferenceEquals(null, right) : !left.Equals(right);
		}
	}
}