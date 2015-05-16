#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Server.Accounting;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Scripts.Engines.ValorSystem
{
	public class ValorBoardGump : Gump
	{
		private readonly List<Mobile> _List;
		private readonly List<Mobile> _Names;
		private int _Page;
		private int _Index;
		private readonly string _NameFind;
		private bool _Check;
		private readonly ValorRewardController _Controller;

		public ValorBoardGump(
			ValorRewardController controller, int listPage, List<Mobile> namelist, List<Mobile> list, string nameFind, bool check)
			: base(300, 300)
		{
			_Controller = controller;
			_List = list;
			_Names = namelist;
			_Page = listPage;
			_NameFind = nameFind;
			_Check = check;

			Closable = true;
			Disposable = false;
			Dragable = false;
			Resizable = false;

			AddPage(0);
			//container
			AddBackground(0, 0, 262, 280, 83);
			//vertical bar
			AddImageTiled(128, 7, 1, 193, 2620);
			//horizontal bar
			AddImageTiled(5, 25, 252, 2, 2620);
			AddLabel(10, 7, 200, @"Name");
			AddLabel(134, 7, 200, @"Valor Points");
			//horizontal bar
			AddImageTiled(5, 200, 252, 2, 2620);
			//sort by
			AddButton(9, 205, 4005, 4006, 6, GumpButtonType.Reply, 0);
			AddLabel(40, 205, 1149, @"Sort by Name");
			AddButton(134, 205, 4005, 4006, 7, GumpButtonType.Reply, 0);
			AddLabel(164, 205, 1149, @"Sort by Points");
			//search by name
			AddLabel(9, 225, 1149, @"Search by Name");
			AddButton(9, 245, 4005, 4006, 8, GumpButtonType.Reply, 0);
			AddBackground(40, 245, 150, 25, 3000);
			AddTextEntry(42, 248, 150, 20, 2622, 0, ""); //searchbyname

			if (_List == null)
			{
				//gather and sort all players with an active account, not a murderer and have valor points
				List<Mobile> a = (from person in World.Mobiles.Values.OfType<PlayerMobile>()
								  let gather = person
								  let acct = person.Account as Account
								  where acct != null
								  let inactive = DateTime.UtcNow - acct.LastLogin
								  where
									  gather.AccessLevel == AccessLevel.Player && gather.Kills <= 4 && gather.ValorPoints > 0 &&
									  inactive < TimeSpan.FromDays(60.0)
								  select gather).Cast<Mobile>().ToList();

				_List = a;
			}
			//generate a list from the gathered players with players matching name
			if (nameFind != null && _Check)
			{
				List<Mobile> b =
					_List.Where(
						person => person is PlayerMobile && person.Name.IndexOf(_NameFind, StringComparison.OrdinalIgnoreCase) >= 0)
						 .Cast<PlayerMobile>()
						 .Cast<Mobile>()
						 .ToList();

				_Names = b;
			}

			_Check = false; //check is used to ensure list doesn't generate constantly

			//page generation here
			if (listPage > 0)
			{
				AddButton(94, 177, 4014, 4015, 9, GumpButtonType.Reply, 0);
				AddLabel(34, 179, 1149, @"Last Page");
			}

			int k = nameFind == null ? _List.Count : _Names.Count;

			if ((listPage + 1) * 5 < k)
			{
				AddButton(134, 177, 4005, 4006, 10, GumpButtonType.Reply, 0);
				AddLabel(164, 179, 1149, @"Next Page");
			}

			//generate player list here, if namefind null, use full list of players

			for (int i = 0, j = 0, index = ((listPage * 5)); i < 5 && index < k && j >= 0; ++j, i++, ++index)
			{
				Mobile mob = nameFind == null ? _List[index] : _Names[index];

				if (!(mob is PlayerMobile))
				{
					continue;
				}

				var m = (PlayerMobile)mob;
				int offsetbg = 30 + (i * 30);
				int offsetlbl = 35 + (i * 30);
				int offsetbutton = 38 + (i * 30);
				int l = i + 1; //offset for available buttons

				//name container
				AddBackground(7, offsetbg, 120, 30, 9300);
				//points container
				AddBackground(131, offsetbg, 125, 30, 9300);
				AddLabel(10, offsetlbl, 1149, m.Name);
				AddButton(108, offsetbutton, 0x846, 0x845, l, GumpButtonType.Reply, 0);
				AddLabel(137, offsetlbl, 1149, m.ValorPoints.ToString(CultureInfo.InvariantCulture));
				_Index = index;
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			if (from == null)
			{
				return;
			}

			if (info.ButtonID == 0) //cloase
			{
				//close shit if main window closed
				from.CloseGump(typeof(ValorBoardGump));
				from.CloseGump(typeof(ValorBoardGumpBritish));
				from.CloseGump(typeof(ValorBoardGumpSide));
				from.CloseGump(typeof(ValorBoardGumpBuyMenu));
			}

			if (info.ButtonID >= 1 && info.ButtonID <= 5) // statistics
			{
				//show stats for clicked player
				_Index = (info.ButtonID + (_Page * 5)) - 1;
				Mobile mob = _NameFind == null ? _List[_Index] : _Names[_Index];
				from.CloseGump(typeof(ValorBoardGumpSide));
				from.SendGump(new ValorBoardGumpSide(mob, from, _Controller));
				from.SendGump(new ValorBoardGump(_Controller, _Page, _Names, _List, _NameFind, _Check));
			}

			if (info.ButtonID == 6) // Sort by Name
			{
				if (_NameFind == null)
				{
					_List.Sort(new NameComparer());
				}
				else
				{
					_Names.Sort(new NameComparer());
				}

				_Page = 0;
				from.SendGump(new ValorBoardGump(_Controller, _Page, _Names, _List, _NameFind, _Check));
			}

			if (info.ButtonID == 7) // Sort by Valor
			{
				if (_NameFind == null)
				{
					_List.Sort(new ValorComparer());
				}
				else
				{
					_Names.Sort(new ValorComparer());
				}
				_Page = 0;
				from.SendGump(new ValorBoardGump(_Controller, _Page, _Names, _List, _NameFind, _Check));
			}

			if (info.ButtonID == 8) // Search by Name
			{
				_Page = 0;
				_Check = true;
				TextRelay messageText = info.GetTextEntry(0);
				from.SendGump(new ValorBoardGump(_Controller, _Page, _Names, _List, messageText.Text, _Check));
			}

			if (info.ButtonID == 9) // Previous page
			{
				if (_Page > 0)
				{
					from.SendGump(new ValorBoardGump(_Controller, _Page - 1, _Names, _List, _NameFind, _Check));
				}
			}

			if (info.ButtonID != 10)
			{
				return;
			}

			if ((_Page + 1) * 5 < _List.Count)
			{
				from.SendGump(new ValorBoardGump(_Controller, _Page + 1, _Names, _List, _NameFind, _Check));
			}
		}
	}

	public class ValorComparer : IComparer<Mobile>
	{
		public int Compare(Mobile m1, Mobile m2)
		{
			var pm1 = (PlayerMobile)m1;
			var pm2 = (PlayerMobile)m2;

			if (pm1 == null || pm2 == null)
			{
				return 0;
			}

			return pm1.ValorPoints.CompareTo(pm2.ValorPoints) * -1;
		}
	}

	public class NameComparer : IComparer<Mobile>
	{
		public int Compare(Mobile m1, Mobile m2)
		{
			int res = 0;

			if (m1.CompareNull(m2, ref res))
			{
				return res;
			}

			return Insensitive.Compare(m1.Name, m2.Name);
		}
	}

	//too lazy to remake initial gump
	public class ValorBoardGumpBritish : Gump
	{
		public ValorBoardGumpBritish()
			: base(300, 100)
		{
			Closable = true;
			Disposable = false;
			Dragable = false;
			Resizable = false;
			//lord british
			//AddBackground(0, 0, 200, 210, 83);
			AddImage(40, -35, 0x3DE, 0);
			AddImage(40, -35, 0xc5d3, 0);
			AddImage(45, -35, 0xc730, 0);
			AddLabel(75, 185, 32, @"Valor Points Board");
			//close button
			AddButton(220, 10, 4017, 4018, 0, GumpButtonType.Reply, 0);
			//AddLabel(140, 10, 1149, @"Close");
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			if (info.ButtonID != 0 || from == null)
			{
				return;
			}

			//close shit
			from.CloseGump(typeof(ValorBoardGump));
			from.CloseGump(typeof(ValorBoardGumpBritish));
			from.CloseGump(typeof(ValorBoardGumpSide));
			from.CloseGump(typeof(ValorBoardGumpBuyMenu));
		}
	}

	public class ValorBoardGumpSide : Gump
	{
		private readonly string[] _Rating = {"Poor", "Mediocre", "Good", "Great", "Excellent", "Perfect"};
		private readonly int[] _RatingColor = {2411, 32, 55, 66, 77, 88, 99};
		private readonly ValorRewardController _Controller;

		public ValorBoardGumpSide(Mobile mob, Mobile from, ValorRewardController controller)
			: base(600, 300)
		{
			var pm = (PlayerMobile)mob;

			if (pm == null)
			{
				return;
			}

			_Controller = controller;

			Closable = true;
			Disposable = false;
			Dragable = true;
			Resizable = false;
			//Main container
			AddBackground(0, 0, 200, 275, 5170);
			AddLabel(27, 20, 88, @"Detailed Valor Statistics");

			AddLabel(24, 35, 55, @"Name");
			AddBackground(23, 50, 75, 25, 9350);
			AddLabel(26, 53, 0, pm.Name.Trim());

			AddLabel(104, 35, 55, @"Rank");
			AddBackground(103, 50, 75, 25, 9350);
			AddLabel(106, 53, 0, pm.ValorRank == 0 ? @"Unranked" : pm.ValorTitle);

			AddLabel(24, 75, 55, @"Valor Points");
			AddBackground(23, 90, 75, 25, 9350);
			AddLabel(26, 93, 0, pm.ValorPoints.ToString(CultureInfo.InvariantCulture));

			AddLabel(104, 75, 55, @"Valor Title");
			AddBackground(103, 90, 75, 25, 9350);
			AddLabel(106, 93, 0, pm.ValorTitle ?? @"Untitled");

			AddLabel(24, 115, 55, @"Reds Killed");
			AddBackground(23, 130, 75, 25, 9350);
			AddLabel(26, 133, 32, pm.MurderersKilled.ToString(CultureInfo.InvariantCulture));

			AddLabel(104, 115, 55, @"Valor Quests");
			AddBackground(103, 130, 75, 25, 9350);
			AddLabel(106, 133, 0, pm.ValorQuests.ToString(CultureInfo.InvariantCulture));

			AddLabel(24, 155, 55, @"Overall Valor Rating");
			AddBackground(23, 171, 155, 25, 9350);
			AddLabel(26, 174, _RatingColor[pm.ValorRating], pm.ValorRating == 0 ? @"Unrated" : _Rating[pm.ValorRating]);

			//if clicked mob is player, show options just for him
			if (mob != from)
			{
				return;
			}

			AddLabel(41, 202, 77, @"Redeem Valor Points");
			AddButton(24, 205, 0x15e1, 0x15e5, 1, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			switch (info.ButtonID)
			{
				case 1:
					state.Mobile.SendGump(new ValorBoardGumpBuyMenu(_Controller, null, null, state.Mobile, 0, 0, null));
					break;
			}
		}
	}

	public class ValorBoardGumpBuyMenu : Gump
	{
		private readonly List<ValorItem> _Items;
		private readonly List<ValorTitleItem> _Titles;
		private readonly int _CategoryPage;
		private readonly int _ItemPage;
		private readonly string _Category;
		private readonly ValorRewardController _Controller;
		private readonly PlayerMobile _User;

		public ValorBoardGumpBuyMenu(
			ValorRewardController mrc,
			List<ValorTitleItem> vtlist,
			List<ValorItem> vlist,
			Mobile user,
			int listCategoryPage,
			int listItemPage,
			string category)
			: base(600, 300)
		{
			Closable = true;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			_User = user as PlayerMobile;

			_Titles = vtlist;
			_Items = vlist;
			_Controller = mrc;
			_CategoryPage = listCategoryPage;
			_ItemPage = listItemPage;
			_Category = category;

			if (_Category == null && _Controller.Categories[0] != null)
			{
				_Category = _Controller.Categories[0];
			}

			if (_Category != "Titles" && _Items != null && !_Items.Exists(x => x.Cat == _Category) ||
				_Items == null && _Category != "Titles")
			{
				List<ValorItem> a = _Controller.GetValorItems.Where(x => x.Cat == _Category).ToList();
				_Items = a;
			}

			if (_Titles == null)
			{
				List<ValorTitleItem> a = _Controller.GetTitles.Where(x => x.ValorCost > 0).ToList();
				_Titles = a;
			}

			//Main container
			AddBackground(0, 0, 380, 325, 5170);
			AddLabel(130, 20, 5, @"Valor Rewards Menu");

			AddLabel(27, 45, 52, @"Categories");

			CatGen();
			PageGen();

			if (_Category == "Titles")
			{
				TitleGen();
			}
			else
			{
				ItemGen();
			}

			//H and V bars
			AddBackground(140, 45, 2, 201, 2620);
			AddBackground(18, 63, 345, 2, 2620);
			AddBackground(18, 245, 345, 2, 2620);

			if (_User == null)
			{
				return;
			}

			AddLabel(30, 260, 10, @"Current Valor Points:");
			AddLabel(165, 260, 88, _User.ValorPoints.ToString(CultureInfo.InvariantCulture));
		}

		//generate category list
		public void CatGen()
		{
			for (int i = 0, index = _CategoryPage * 8; i < 8 && index < _Controller.Categories.Count; i++, ++index)
			{
				int offsetlbl = 67 + (i * 20);
				int offsetbtn = 70 + (i * 20);
				int l = i + 1; //offset for available buttons

				AddButton(24, offsetbtn, 0x846, 0x845, l, GumpButtonType.Reply, 0);
				AddLabel(42, offsetlbl, 88, _Controller.Categories[index]);
			}
		}

		public void TitleGen()
		{
			AddLabel(150, 45, 52, @"Titles");
			AddLabel(290, 45, 52, @"Valor Cost");

			for (int i = 0, index = _ItemPage * 8; i < 8 && index < _Controller.TitleCount; i++, ++index)
			{
				if (_Controller.GetTitles[index].ValorCost > 0)
				{
					int offsetlbl = 67 + (i * 20);
					int offsetbtn = 70 + (i * 20);
					int l = (i + 8) + 1; //offset for available buttons

					AddButton(147, offsetbtn, 0x846, 0x845, l, GumpButtonType.Reply, 0);

					if (_User.Female && _Controller.GetTitles[index].fTitle != null)
					{
						AddLabel(165, offsetlbl, 88, _Controller.GetTitles[index].fTitle);
					}
					else
					{
						AddLabel(165, offsetlbl, 88, _Controller.GetTitles[index].Title);
					}

					AddLabel(290, offsetlbl, 88, _Controller.GetTitles[index].ValorCost.ToString(CultureInfo.InvariantCulture));
				}
				else
				{
					i--;
				}
			}
		}

		public void ItemGen()
		{
			AddLabel(150, 45, 52, @"Items");
			AddLabel(290, 45, 52, @"Valor Cost");

			for (int i = 0, index = _ItemPage * 8; i < 8 && index < _Items.Count; i++, ++index)
			{
				if (_Category != _Items[index].Cat)
				{
					continue;
				}

				int offsetlbl = 67 + (i * 20);
				int offsetbtn = 70 + (i * 20);
				int l = (i + 8) + 1; //offset for available buttons

				AddButton(147, offsetbtn, 0x846, 0x845, l, GumpButtonType.Reply, 0);
				AddLabel(165, offsetlbl, 88, _Items[index].Title);
				AddLabel(290, offsetlbl, 88, _Items[index].Cost.ToString(CultureInfo.InvariantCulture));
			}
		}

		public void PageGen()
		{
			if (_CategoryPage > 0)
			{
				AddButton(107, 220, 0x15e3, 0x15e7, 30, GumpButtonType.Reply, 0);
			}

			if ((_CategoryPage + 1) * 8 < _Controller.Categories.Count)
			{
				AddButton(117, 220, 0x15e1, 0x15e5, 31, GumpButtonType.Reply, 0);
			}

			if (_ItemPage > 0)
			{
				AddButton(255, 220, 0x15e3, 0x15e7, 32, GumpButtonType.Reply, 0);
			}

			int r = _Category == "Titles" ? _Controller.TitleCount : _Items.Count;

			if ((_ItemPage + 1) * 8 < r)
			{
				AddButton(275, 220, 0x15e1, 0x15e5, 33, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 0)
			{
				state.Mobile.SendGump(new ValorBoardGumpSide(state.Mobile, state.Mobile, _Controller));
			}

			if (info.ButtonID >= 1 && info.ButtonID <= 8)
			{
				state.Mobile.CloseGump(typeof(ValorBoardGumpBuyMenu));

				int temp = (info.ButtonID + (_CategoryPage * 8)) - 1;

				state.Mobile.SendGump(
					new ValorBoardGumpBuyMenu(
						_Controller, _Titles, _Items, _User, _CategoryPage, _ItemPage, _Controller.Categories[temp]));
			}
			else if (info.ButtonID >= 9 && info.ButtonID <= 18)
			{
				state.Mobile.SendGump(
					_Category == "Titles"
						? new ValorBoardGumpViewItem(
							  state.Mobile, _Controller, _Titles[((info.ButtonID - 8) + (_ItemPage * 8)) - 1], null)
						: new ValorBoardGumpViewItem(state.Mobile, _Controller, null, _Items[((info.ButtonID - 8) + (_ItemPage * 8)) - 1]));
			}
			else
			{
				switch (info.ButtonID)
				{
					case 30:
						{
							if (_CategoryPage > 0)
							{
								state.Mobile.SendGump(
									new ValorBoardGumpBuyMenu(_Controller, _Titles, _Items, _User, _CategoryPage - 1, _ItemPage, _Category));
							}
						}
						break;
					case 31:
						{
							if ((_CategoryPage + 1) * 8 < _Controller.Categories.Count)
							{
								state.Mobile.SendGump(
									new ValorBoardGumpBuyMenu(_Controller, _Titles, _Items, _User, _CategoryPage + 1, _ItemPage, _Category));
							}
						}
						break;
					case 32:
						{
							if (_ItemPage > 0)
							{
								state.Mobile.SendGump(
									new ValorBoardGumpBuyMenu(_Controller, _Titles, _Items, _User, _CategoryPage, _ItemPage - 1, _Category));
							}
						}
						break;
					case 33:
						{
							int r = _Category != "Titles" ? _Items.Count : _Controller.TitleCount;

							if ((_ItemPage + 1) * 8 < r)
							{
								state.Mobile.SendGump(
									new ValorBoardGumpBuyMenu(_Controller, _Titles, _Items, _User, _CategoryPage, _ItemPage + 1, _Category));
							}
						}
						break;
				}
			}
		}
	}

	public class ValorBoardGumpViewItem : Gump
	{
		private readonly ValorRewardController _Controller;
		private readonly ValorTitleItem _TitleItem;
		private readonly ValorItem _Item;
		private readonly PlayerMobile _User;

		public ValorBoardGumpViewItem(Mobile user, ValorRewardController controller, ValorTitleItem titleItem, ValorItem item)
			: base(820, 300)
		{
			_Controller = controller;
			_TitleItem = titleItem;
			_Item = item;

			_User = user as PlayerMobile;

			Closable = true;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			if (_Item != null)
			{
				ViewItem();
			}
			else if (_TitleItem != null)
			{
				ViewTitle();
			}
		}

		public void ViewItem()
		{
			AddBackground(0, 0, 320, 600, 5170);
			AddLabel(115, 25, 2499, @"Purchase Item");

			AddLabel(30, 60, 2499, ("Item you will receive: "));
			AddLabel(165, 60, 2499, _Item.Title);

			AddImage(70, 80, 0x42, 0);
			AddItem(140, 190, _Item.ValorItemInfo.ItemID, _Item.ValorItemInfo.Hue);

			AddLabel(30, 340, 2499, @"Valor cost: ");
			AddLabel(105, 340, 2499, _Item.Cost.ToString(CultureInfo.InvariantCulture));

			AddLabel(30, 360, 2499, @"Title description:");
			AddHtml(30, 380, 250, 100, _Item.Description, true, true);

			AddButton(190, 525, 4023, 4024, 1, GumpButtonType.Reply, 0); //ok
			AddButton(250, 525, 4017, 4018, 0, GumpButtonType.Reply, 0); //cancel

			if (_User == null)
			{
				return;
			}

			AddLabel(30, 480, 2499, @"Your current valor:");
			AddLabel(152, 480, 2499, _User.ValorPoints.ToString(CultureInfo.InvariantCulture));

			if (_User.ValorPoints - _Item.Cost < 0)
			{
				return;
			}

			AddLabel(30, 500, 2499, @"Valor after purchase: ");
			AddLabel(165, 500, 2499, (_User.ValorPoints - _Item.Cost).ToString(CultureInfo.InvariantCulture));
		}

		public void ViewTitle()
		{
			AddBackground(0, 0, 320, 350, 5170);
			AddLabel(115, 25, 2499, @"Purchase Title");

			AddLabel(30, 60, 2499, ("Title you will receive: "));

			if (_User.Female && _TitleItem.fTitle != null)
			{
				AddLabel(165, 60, 2499, _TitleItem.fTitle);
			}
			else
			{
				AddLabel(165, 60, 2499, _TitleItem.Title);
			}

			AddLabel(30, 90, 2499, @"Valor cost: ");
			AddLabel(105, 90, 2499, _TitleItem.ValorCost.ToString(CultureInfo.InvariantCulture));

			AddLabel(30, 120, 2499, @"Title description:");
			AddHtml(30, 140, 250, 100, _TitleItem.Description, true, true);

			if (_User == null)
			{
				return;
			}

			AddLabel(30, 250, 2499, @"Your current valor:");
			AddLabel(152, 250, 2499, _User.ValorPoints.ToString(CultureInfo.InvariantCulture));

			if (_User.ValorPoints - _TitleItem.ValorCost < 0)
			{
				return;
			}

			AddLabel(30, 270, 2499, @"Valor after purchase: ");
			AddLabel(165, 270, 2499, (_User.ValorPoints - _TitleItem.ValorCost).ToString(CultureInfo.InvariantCulture));

			AddButton(190, 295, 4023, 4024, 1, GumpButtonType.Reply, 0); //ok
			AddButton(250, 295, 4017, 4018, 0, GumpButtonType.Reply, 0); //cancel
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			switch (info.ButtonID)
			{
				case 0: //cancel
					state.Mobile.SendGump(new ValorBoardGumpBuyMenu(_Controller, null, null, state.Mobile, 0, 0, null));
					break;
				case 1: //ok
					{
						if (_Item != null)
						{
							if (_User != null && _User.ValorPoints >= _Item.Cost)
							{
								_User.BankBox.DropItem(_Item.GetValorItem());
								_User.SendMessage(44, _Item.Title + " has been placed in your bank box.");
								_User.ValorPoints = _User.ValorPoints - _Item.Cost;
							}
							else if (_User != null && _User.ValorPoints < _Item.Cost)
							{
								_User.SendMessage(44, "You do not have enough valor points to make this purchase.");
							}
						}
						else if (_TitleItem != null)
						{
							if (_User != null && _User.ValorPoints >= _TitleItem.ValorCost)
							{
								_User.BankBox.DropItem(new TitleScrollOld(_TitleItem.Title, _TitleItem.fTitle));

								if (!_User.Female || _TitleItem.fTitle == null)
								{
									_User.SendMessage(
										44, "A scroll containing the title: " + _TitleItem.Title + " has been deposted in your bank box.");
								}
								else
								{
									_User.SendMessage(
										44, "A scroll containing the title: " + _TitleItem.fTitle + " has been deposted in your bank box.");
								}
								_User.ValorPoints = _User.ValorPoints - _TitleItem.ValorCost;
							}
							else if (_User != null && _User.ValorPoints < _TitleItem.ValorCost)
							{
								_User.SendMessage(44, "You do not have enough valor points to make this purchase.");
							}
						}

						state.Mobile.SendGump(new ValorBoardGumpBuyMenu(_Controller, null, null, state.Mobile, 0, 0, null));
					}
					break;
			}
		}
	}

	public class ValorBoardGumpChangeTitle : Gump
	{
		private readonly int _Page;
		private readonly List<string> _Titles;

		public ValorBoardGumpChangeTitle(Mobile from, int page)
			: base(600, 300)
		{
			_Page = page;

			var pm = from as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			Closable = true;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			_Titles = pm.GetValorTitles();

			//Main container
			AddBackground(0, 0, 250, 280, 3500);
			AddLabel(23, 17, 0, @"|");
			AddLabel(24, 4, 0, @"_______");
			AddLabel(27, 19, 88, @"Change");
			AddLabel(78, 17, 0, @"|");
			AddLabel(23, 32, 0, @"|");
			AddLabel(24, 34, 0, @"_______");
			AddLabel(45, 33, 88, @"Titles");
			AddLabel(78, 32, 0, @"|");
			AddLabel(170, 25, 88, @"None");
			AddRadio(135, 20, 0x868, 0x86A, false, 1);

			AddButton(190, 230, 4023, 4024, 20, GumpButtonType.Reply, 0);

			AddLabel(20, 200, 44, @"Cost to change title:");
			AddLabel(155, 200, 0, @"25,000 gold");

			//title generation
			for (int i = 0, j = 0, index = page * 6; i < 6 && index < pm.ValorTitleCount && j >= 0; i++, ++index)
			{
				int offsetlbl = 65 + (j * 40);
				int offsetbutton = 60 + (j * 40);

				int l = i + 2; //used to offset 0 button

				if (index % 2 == 0)
				{
					AddRadio(20, offsetbutton, 0x868, 0x86A, false, l);
					AddLabel(55, offsetlbl, 88, _Titles[index]);
				}
				else
				{
					AddRadio(135, offsetbutton, 0x868, 0x86A, false, l);
					AddLabel(170, offsetlbl, 88, _Titles[index]);
					j++;
				}
			}

			//page generation
			if (page > 0)
			{
				AddButton(25, 230, 4014, 4015, 8, GumpButtonType.Reply, 0);
				AddLabel(27, 250, 1149, @"Last");
			}

			if ((page + 1) * 6 >= pm.ValorTitleCount)
			{
				return;
			}

			AddButton(60, 230, 4005, 4006, 9, GumpButtonType.Reply, 0);
			AddLabel(62, 250, 1149, @"Next");
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			var pm = state.Mobile as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			const int price = 25000;

			switch (info.ButtonID)
			{
				case 0:
					break;
				case 8:
					{
						if (_Page > 0)
						{
							pm.SendGump(new ValorBoardGumpChangeTitle(state.Mobile, _Page - 1));
						}
					}
					break;
				case 9:
					{
						if ((_Page + 1) * 6 < pm.ValorTitleCount)
						{
							pm.SendGump(new ValorBoardGumpChangeTitle(state.Mobile, _Page + 1));
						}
					}
					break;

				case 20:
					{
						Type cType = pm.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

						if (!info.IsSwitched(1))
						{
							if (!Banker.Withdraw(pm, cType, price))
							{
								pm.SendMessage(
									44, " You do not have the {0:#,0} {1} in your bank box required to change your title.", price, cType.Name);
								break;
							}

							if (Banker.Withdraw(pm, cType, price))
							{
								pm.SendMessage("{0:#,0} {1} has been withdrawn from your bank box.", price, cType.Name);
								pm.SendMessage("You have {0:#,0} {1} in cash remaining in your bank box.", price, cType.Name);
							}
						}

						if (info.IsSwitched(1))
						{
							pm.ValorTitle = null;
						}

						if (info.IsSwitched(2))
						{
							string temptitle = _Titles[(_Page * 6)];

							if (temptitle != null)
							{
								pm.ValorTitle = temptitle;
							}
						}

						if (info.IsSwitched(3))
						{
							string temptitle = _Titles[(_Page * 6) + 1];

							if (temptitle != null)
							{
								pm.ValorTitle = temptitle;
							}
						}

						if (info.IsSwitched(4))
						{
							string temptitle = _Titles[(_Page * 6) + 2];

							if (temptitle != null)
							{
								pm.ValorTitle = temptitle;
							}
						}

						if (info.IsSwitched(5))
						{
							string temptitle = _Titles[(_Page * 6) + 3];

							if (temptitle != null)
							{
								pm.ValorTitle = temptitle;
							}
						}

						if (info.IsSwitched(6))
						{
							string temptitle = _Titles[(_Page * 6) + 4];

							if (temptitle != null)
							{
								pm.ValorTitle = temptitle;
							}
						}

						if (info.IsSwitched(7))
						{
							string temptitle = _Titles[(_Page * 6) + 5];

							if (temptitle != null)
							{
								pm.ValorTitle = temptitle;
							}
						}
					}
					break;
			}
		}
	}
}