#region References
using System;
using System.Collections.Generic;

using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public class UOFCentralGump : SuperGump
	{
		private static readonly int[] _Frames =
		{
			1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1410, 1411, 1412,
			1413, 1414
		};

		public static UOFCentralGump Editing { get; private set; }

		private bool _ForcedClose;

		private int _Frame;
		private bool? _State = true;
		private Page _Page;
		private bool _Edit;

		public Page Page { get { return _Page ?? (_Page = UOFCentral.State.Root); } }

		public Stack<Page> History { get; private set; }
		public Dictionary<string, ToolbarButton> Toolbar { get; private set; }

		public bool Edit
		{
			get
			{
				if (_Edit)
				{
					if (Editing == null || Editing.IsDisposed)
					{
						Editing = this;
					}
					else if (Editing != this)
					{
						if (User.AccessLevel > Editing.User.AccessLevel)
						{
							Editing.Close();
							Editing = this;
						}
						else
						{
							_Edit = false;
						}
					}
				}
				else if (Editing == this)
				{
					Editing = null;
				}

				return _Edit;
			}
			set
			{
				_Edit = value;

				if (_Edit)
				{
					if (Editing == null || Editing.IsDisposed)
					{
						Editing = this;
					}
					else if (Editing != this)
					{
						if (User.AccessLevel > Editing.User.AccessLevel)
						{
							Editing.Close();
							Editing = this;
						}
						else
						{
							_Edit = false;
						}
					}
				}
				else if (Editing == this)
				{
					Editing = null;
				}
			}
		}

		public UOFCentralGump(PlayerMobile user, Page page = null, bool edit = false)
			: base(user, null, UOFCentral.State.X, UOFCentral.State.Y)
		{
			_Page = page;

			History = new Stack<Page>();
			Toolbar = new Dictionary<string, ToolbarButton>();

			Edit = edit;

			ForceRecompile = true;

			AutoRefreshRate = TimeSpan.FromMilliseconds(100);
		}

		public void SetPage(Page page)
		{
			if (Page != page)
			{
				History.Push(Page);
			}

			_Page = page;

			Refresh(true);
		}

		public void PreviousPage()
		{
			if (History.Count > 0)
			{
				_Page = History.Pop();
			}

			Refresh(true);
		}

		protected override void Compile()
		{
			base.Compile();

			Toolbar.Clear();
			Page.CompileToolbar(this, Toolbar);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			if (IsDisposed)
			{
				return;
			}

			layout.Add("bg", () => AddBackground(0, 0, 655, 470, 2620));
			layout.Add("bg/alpha", () => AddAlphaRegion(10, 10, 635, 450));

			layout.Add("chest/bg", () => AddImage(8, 7, 1415, 0));
			layout.Add("chest/lid", () => AddImage(8, 9, _Frames[_Frame], 0));

			if (_State != null)
			{
				return;
			}

			layout.Add(
				"frame/close",
				() => AddButton(
					628,
					0,
					2640,
					2641,
					b =>
					{
						_ForcedClose = true;
						Close(b);
					}));

			if (Toolbar.Count > 0)
			{
				layout.Add("toolbar/bg", () => AddBackground(105, 70, 440, 40, 2620));
				layout.Add("toolbar/alpha", () => AddAlphaRegion(110, 75, 430, 30));

				Toolbar.ForRange(
					0,
					13,
					(i, k, v) =>
					{
						int xo = i * 34; //(i % 2 == 0 ? 34 : 33);

						layout.Add(
							"toolbar/button/" + k,
							() =>
							{
								AddButton(110 + xo, 80, v.Icon.Normal, v.Icon.Pressed, b => v.Callback());
								AddTooltip(v.Icon.Tooltip);
							});
					});
			}

			//layout.Add("page/bg", () => AddBackground(105, 115, 440, 260, 2620));
			//layout.Add("page/alpha", () => AddAlphaRegion(110, 120, 430, 250));

			Page.CompileLayout(this, layout);
		}

		protected override void OnAutoRefresh()
		{
			if (_State == null)
			{
				AutoRefresh = false;
				return;
			}

			if (_State.Value)
			{
				_Frame++;

				if (_Frame == _Frames.Length - 1)
				{
					AutoRefresh = true;
					_State = null;
				}
			}
			else
			{
				if (_Frame <= 0)
				{
					Close(true);
					return;
				}

				_Frame--;
			}

			base.OnAutoRefresh();
		}

		public override void Close(bool all = false)
		{
			if (!all && AutoRefresh && _State == false)
			{
				return;
			}

			if (IsOpen)
			{
				if (!all)
				{
					if (_ForcedClose)
					{
						_State = false;
						AutoRefresh = true;
						Refresh(true);
						return;
					}

					if (Edit)
					{
						AutoRefresh = false;
						_State = null;

						Edit = false;
						Refresh(true);
						return;
					}

					if (History.Count > 0)
					{
						AutoRefresh = false;
						_State = null;

						PreviousPage();
						return;
					}

					_State = false;
					AutoRefresh = true;
					Refresh(true);
					return;
				}

				AutoRefresh = false;
				_State = null;
			}

			Edit = false;

			_ForcedClose = false;

			History.Clear();

			base.Close(true);
		}

		protected override void OnSend()
		{
			if (_State != null)
			{
				AutoRefresh = true;
			}

			base.OnSend();
		}

		protected override void OnDispose()
		{
			base.OnDispose();

			if (Editing == this)
			{
				Editing = null;
			}
		}
	}
}