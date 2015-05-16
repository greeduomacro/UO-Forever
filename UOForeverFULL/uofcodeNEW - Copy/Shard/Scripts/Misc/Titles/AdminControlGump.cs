#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using Server.Gumps;
using Server.Network;
#endregion

namespace Server.Scripts.Engines.ValorSystem
{
	public class ValorTitleRewardAdmin : Gump
	{
		private readonly int _mListPageCat;
		private readonly int _iListPageItem;
		private readonly List<ValorItem> _vList;
		private readonly string _scurrcat;
		private readonly string _soldcat;
		private readonly ValorRewardController _MValorRc;

		public ValorTitleRewardAdmin(
			ValorRewardController mValorRewardController,
			List<ValorItem> vlist,
			int listPageCat,
			int listPageItem,
			string scurrcat,
			string soldcat)
			: base(820, 300)
		{
			Closable = true;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			_mListPageCat = listPageCat;
			_vList = vlist;
			_iListPageItem = listPageItem;
			_MValorRc = mValorRewardController;
			_scurrcat = scurrcat;
			_soldcat = soldcat;

			if (_scurrcat == null && _MValorRc.Categories != null)
			{
				_scurrcat = _MValorRc.Categories[0];
			}

			if (_scurrcat != "Titles" && _vList != null && !_vList.Exists(x => x.Cat == _scurrcat) ||
				_vList == null && _scurrcat != "Titles")
			{
				List<ValorItem> a = _MValorRc.GetValorItems.Where(x => x.Cat == _scurrcat).ToList();
				_vList = a;
			}
			//Main container
			AddBackground(0, 0, 300, 425, 2620);
			AddLabel(40, 20, 5, @"Titles & Valor Items Administration");

			AddLabel(10, 45, 52, @"Categories");
			CatGen();

			if (_scurrcat == "Titles")
			{
				TitleGen();
			}
			else
			{
				ItemGen();
			}
			PageGen();

			//H and V bars
			AddBackground(140, 45, 2, 196, 9300);
			AddBackground(3, 63, 293, 2, 9300);
			AddBackground(3, 240, 293, 2, 9300);

			AddLabel(10, 247, 32, @"Add Category");
			AddBackground(5, 280, 95, 25, 9350);
			AddLabel(7, 263, 55, @"Category Name");
			AddTextEntry(7, 285, 90, 20, 2622, 0, ""); //cat name
			AddButton(6, 307, 4005, 4006, 21, GumpButtonType.Reply, 0);

			AddLabel(10, 327, 32, @"Delete Category");
			AddBackground(5, 360, 95, 25, 9350);
			AddLabel(7, 343, 55, @"Category Name");
			AddTextEntry(7, 365, 90, 20, 2622, 1, ""); //cat delete name
			AddButton(6, 387, 4005, 4006, 22, GumpButtonType.Reply, 0);

			AddLabel(150, 247, 32, @"Add Item");
			AddButton(210, 246, 4005, 4006, 23, GumpButtonType.Reply, 0);

			AddLabel(150, 287, 32, @"Add Title");
			AddButton(210, 286, 4005, 4006, 24, GumpButtonType.Reply, 0);
		}

		//generate category list
		public void CatGen()
		{
			for (int i = 0, index = _mListPageCat * 8; i < 8 && index < _MValorRc.Categories.Count; i++, ++index)
			{
				int offsetlbl = 67 + (i * 20);
				int offsetbtn = 70 + (i * 20);
				int l = i + 1; //offset for available buttons
				AddButton(10, offsetbtn, 0x846, 0x845, l, GumpButtonType.Reply, 0);
				AddLabel(28, offsetlbl, 88, _MValorRc.Categories[index]);
			}
		}

		public void TitleGen()
		{
			AddLabel(150, 45, 52, @"Titles");
			for (int i = 0, index = _iListPageItem * 8; i < 8 && index < _MValorRc.TitleCount; i++, ++index)
			{
				int offsetlbl = 67 + (i * 20);
				int offsetbtn = 70 + (i * 20);
				int l = (i + 8) + 1; //offset for available buttons
				AddButton(147, offsetbtn, 0x846, 0x845, l, GumpButtonType.Reply, 0);
				AddLabel(165, offsetlbl, 88, _MValorRc.GetTitles[index].Title);
			}
		}

		public void ItemGen()
		{
			AddLabel(150, 45, 52, @"Items");
			for (int i = 0, index = _iListPageItem * 8; i < 8 && index < _vList.Count; i++, ++index)
			{
				if (_scurrcat != _vList[index].Cat)
				{
					continue;
				}
				int offsetlbl = 67 + (i * 20);
				int offsetbtn = 70 + (i * 20);
				int l = (i + 8) + 1; //offset for available buttons
				AddButton(147, offsetbtn, 0x846, 0x845, l, GumpButtonType.Reply, 0);
				AddLabel(165, offsetlbl, 88, _vList[index].Title);
			}
		}

		public void PageGen()
		{
			if (_mListPageCat > 0)
			{
				AddButton(107, 220, 0x15e3, 0x15e7, 30, GumpButtonType.Reply, 0);
			}

			if ((_mListPageCat + 1) * 8 < _MValorRc.Categories.Count)
			{
				AddButton(117, 220, 0x15e1, 0x15e5, 31, GumpButtonType.Reply, 0);
			}

			if (_iListPageItem > 0)
			{
				AddButton(255, 220, 0x15e3, 0x15e7, 32, GumpButtonType.Reply, 0);
			}

			int r = _scurrcat == "Titles" ? _MValorRc.TitleCount : _vList.Count;

			if ((_iListPageItem + 1) * 8 < r)
			{
				AddButton(275, 220, 0x15e1, 0x15e5, 33, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID >= 1 && info.ButtonID <= 8)
			{
				state.Mobile.CloseGump(typeof(ValorTitleRewardAdmin));
				int temp = (info.ButtonID + (_mListPageCat * 8)) - 1;
				state.Mobile.SendGump(
					new ValorTitleRewardAdmin(_MValorRc, _vList, _mListPageCat, _iListPageItem, _MValorRc.Categories[temp], _soldcat));
			}
			else if (info.ButtonID >= 9 && info.ButtonID <= 18)
			{
				state.Mobile.SendGump(
					_scurrcat == "Titles"
						? new ValorTitleItemEdit(_MValorRc, _MValorRc.GetTitles[((info.ButtonID - 8) + (_iListPageItem * 8)) - 1], null, false)
						: new ValorTitleItemEdit(_MValorRc, null, _vList[((info.ButtonID - 8) + (_iListPageItem * 8)) - 1], false));
			}
			else if (info.ButtonID == 21)
			{
				TextRelay messageText = info.GetTextEntry(0);
				if (messageText != null && Regex.IsMatch(messageText.Text, @"^[a-zA-Z]+$") &&
					!_MValorRc.Categories.ContainsValue(messageText.Text))
				{
					_MValorRc.Categories.Add(_MValorRc.Categories.Count, messageText.Text);
				}
				else if (messageText != null)
				{
					state.Mobile.SendMessage(
						32, "The category: " + messageText.Text + " already exists or the category was improperly formatted.");
				}
				state.Mobile.SendGump(new ValorTitleRewardAdmin(_MValorRc, _vList, _mListPageCat, _iListPageItem, _scurrcat, _soldcat));
			}
			else if (info.ButtonID == 22)
			{
				TextRelay messageText = info.GetTextEntry(1);
				if (messageText != null && messageText.Text != "Titles" && Regex.IsMatch(messageText.Text, @"^[a-zA-Z]+$") &&
					_MValorRc.Categories.ContainsValue(messageText.Text))
				{
					KeyValuePair<int, string> item = _MValorRc.Categories.First(kvp => kvp.Value == messageText.Text);
					_MValorRc.Categories.Remove(item.Key);
					state.Mobile.SendMessage(32, "The category: " + messageText.Text + " was successfully removed.");
				}
				else if (messageText != null && messageText.Text == "Titles")
				{
					state.Mobile.SendMessage(32, "The Titles category is a protected category and cannot be deleted.");
				}
				else if (messageText != null)
				{
					state.Mobile.SendMessage(32, "The category: " + messageText.Text + " does not exist");
				}
				state.Mobile.SendGump(new ValorTitleRewardAdmin(_MValorRc, _vList, _mListPageCat, _iListPageItem, _scurrcat, _soldcat));
			}

			else if (info.ButtonID == 23)
			{
				state.Mobile.CloseGump(typeof(ValorTitleRewardAdmin));
				state.Mobile.Target = new ValorRewardController.AddRewardTarget(_MValorRc);
			}
			else if (info.ButtonID == 24)
			{
				state.Mobile.CloseGump(typeof(ValorTitleRewardAdmin));
				state.Mobile.SendGump(new ValorTitleItemEdit(_MValorRc, null, null, true));
			}
			else if (info.ButtonID == 30)
			{
				if (_mListPageCat > 0)
				{
					state.Mobile.SendGump(new ValorTitleRewardAdmin(_MValorRc, _vList, _mListPageCat - 1, _iListPageItem, _scurrcat, _soldcat));
				}
			}
			else if (info.ButtonID == 31)
			{
				if ((_mListPageCat + 1) * 8 < _MValorRc.Categories.Count)
				{
					state.Mobile.SendGump(new ValorTitleRewardAdmin(_MValorRc, _vList, _mListPageCat + 1, _iListPageItem, _scurrcat, _soldcat));
				}
			}
			else if (info.ButtonID == 32)
			{
				if (_iListPageItem > 0)
				{
					state.Mobile.SendGump(new ValorTitleRewardAdmin(_MValorRc, _vList, _mListPageCat, _iListPageItem - 1, _scurrcat, _soldcat));
				}
			}
			else if (info.ButtonID == 33)
			{
				int r = _scurrcat != "Titles" ? _vList.Count : _MValorRc.TitleCount;
				if ((_iListPageItem + 1) * 8 < r)
				{
					state.Mobile.SendGump(new ValorTitleRewardAdmin(_MValorRc, _vList, _mListPageCat, _iListPageItem + 1, _scurrcat, _soldcat));
				}
			}
		}
	}

	public class ValorTitleItemEdit : Gump
	{
		//private readonly ValorBoard _iValorBoard;
		// private readonly ValorItem _mValorItem;

		private readonly bool _mIsAdd;
		private readonly ValorRewardController _MValorRc;
		private readonly ValorTitleItem _MValorTitleItem;
		private readonly ValorItem _mVItem;

		public ValorTitleItemEdit(ValorRewardController mValorRewardController, ValorTitleItem mtitle, ValorItem mVItem, bool addingItem)
			: base(820, 300)
		{
			_mIsAdd = addingItem;
			_MValorRc = mValorRewardController;
			_MValorTitleItem = mtitle;
			_mVItem = mVItem;

			Closable = true;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			AddBackground(0, 0, 500, 450, 2620);
			if (mVItem == null)
			{
				EditTitle();
			}
			else
			{
				EditItem();
			}

			AddButton(350, 400, 4023, 4024, 1, GumpButtonType.Reply, 0); //ok
			AddButton(430, 400, 4017, 4018, 0, GumpButtonType.Reply, 0); //cancel
		}

		public void EditItem()
		{
			AddLabel(360, 10, 2499, @"Add or Edit Item");

			AddImage(315, 30, 0x42, 0);
			AddItem(380, 175, _mVItem.ValorItemInfo.ItemID, _mVItem.ValorItemInfo.Hue);

			AddLabel(10, 10, 2499, @"Enter an item name:");
			AddBackground(5, 27, 190, 44, 3500);
			AddTextEntry(24, 38, 140, 44, 1879, 0, _mVItem.Title);

			AddLabel(10, 75, 2499, @"Enter a category that exists:");
			AddBackground(5, 92, 190, 44, 3500);
			AddTextEntry(24, 103, 140, 44, 1879, 1, _mVItem.Cat);

			AddLabel(10, 141, 2499, @"Enter a valor cost:");
			AddBackground(5, 158, 190, 44, 3500);
			AddTextEntry(24, 169, 140, 44, 1879, 2, _mVItem.Cost.ToString(CultureInfo.InvariantCulture));

			AddLabel(10, 223, 2499, @"Enter a description:");
			AddBackground(5, 240, 300, 200, 3500);
			AddTextEntry(24, 250, 261, 200, 1879, 3, _mVItem.Description);

			if (_mIsAdd)
			{
				return;
			}
			AddLabel(350, 300, 32, @"DELETE ITEM?");
			AddButton(450, 300, 4023, 4024, 2, GumpButtonType.Reply, 0); //ok
		}

		public void EditTitle()
		{
			AddLabel(360, 10, 2499, @"Add or Edit Title");

			AddLabel(10, 10, 2499, @"Enter a title:");
			AddBackground(5, 27, 190, 44, 3500);
			AddTextEntry(24, 38, 140, 44, 1879, 0, _MValorTitleItem == null ? "" : _MValorTitleItem.Title);

			AddLabel(10, 70, 2499, @"Enter a equivalent female title (OPTIONAL):");
			AddBackground(5, 87, 190, 44, 3500);
			AddTextEntry(24, 98, 140, 44, 1879, 3, _MValorTitleItem != null && _MValorTitleItem.fTitle != null ? _MValorTitleItem.fTitle : "");

			AddLabel(10, 141, 2499, @"Enter a valor cost (ONLY SPECIFY IF THIS IS A VALOR TITLE):");
			AddBackground(5, 158, 190, 44, 3500);

			AddTextEntry(
				24, 169, 140, 44, 1879, 1, _MValorTitleItem == null ? "0" : _MValorTitleItem.ValorCost.ToString(CultureInfo.InvariantCulture));

			AddLabel(10, 223, 2499, @"Enter a description:");
			AddBackground(5, 240, 300, 200, 3500);
			AddTextEntry(24, 250, 261, 200, 1879, 2, _MValorTitleItem == null ? "Default Description" : _MValorTitleItem.Description);

			if (_mIsAdd)
			{
				return;
			}
			AddLabel(350, 300, 32, @"DELETE ITEM?");
			AddButton(450, 300, 4023, 4024, 2, GumpButtonType.Reply, 0); //ok
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			switch (info.ButtonID)
			{
				case 0:
					{
						state.Mobile.SendGump(new ValorTitleRewardAdmin(_MValorRc, null, 0, 0, null, null));
						//cancel
						break;
					}
				case 1: //ok
					{
						try
						{
							if (_mVItem != null)
							{
								//Name
								TextRelay entry0 = info.GetTextEntry(0);
								string name = (entry0 == null ? "" : entry0.Text.Trim());

								//category
								TextRelay entry1 = info.GetTextEntry(1);
								string cat = (entry1 == null ? "" : entry1.Text.Trim());

								//Cost
								TextRelay entry2 = info.GetTextEntry(2);
								string costtext = (entry2 == null ? "%Error" : entry2.Text.Trim());
								int cost = Int32.Parse(costtext);

								//Description
								TextRelay entry3 = info.GetTextEntry(3);
								string desc = (entry3 == null ? "None." : entry3.Text.Trim());

								//check for add to make sure cat exists and item does not already exist
								if (_MValorRc.GetValorItems.Exists((r => r.Title == name)) && _mIsAdd || !_MValorRc.Categories.ContainsValue(cat) ||
									cat == "Titles")
								{
									state.Mobile.SendMessage(
										"Please make sure that the specified category exists and the Item does not already exist.");
									break;
								}
								if (_mIsAdd)
								{
									_MValorRc.AddValorItem(_mVItem, state.Mobile);
								}

								_mVItem.Edit(cost, cat, name, desc, false);
							}
							else
							{
								//Name
								TextRelay entry0 = info.GetTextEntry(0);
								string name = (entry0 == null ? "" : entry0.Text.Trim());

								//Female Name
								TextRelay entry3 = info.GetTextEntry(3);
								string namef = (entry3 == null ? "" : entry3.Text.Trim());

								//Cost
								TextRelay entry1 = info.GetTextEntry(1);
								string costtext = (entry1 == null ? "0" : entry1.Text.Trim());
								int cost = Int32.Parse(costtext);

								//Description
								TextRelay entry2 = info.GetTextEntry(2);
								string desc = (entry2 == null ? "None." : entry2.Text.Trim());
								if (_MValorRc.GetTitles.Exists((r => r.Title == name)) && _mIsAdd)
								{
									state.Mobile.SendMessage(
										"Please make sure all fields are filled correctly, the category exists or the item/title does not already exist.");
									break;
								}

								if (_mIsAdd)
								{
									_MValorRc.GetTitles.Add(new ValorTitleItem(name, namef, cost, desc));
								}
								else
								{
									_MValorTitleItem.Edit(name, namef, cost, desc);
								}

								state.Mobile.SendGump(new ValorTitleRewardAdmin(_MValorRc, null, 0, 0, null, null));
							}
						}
						catch
						{
							state.Mobile.SendMessage(
								"Please make sure all fields are filled correctly, the category exists or the item/title does not already exist.");
						}

						break;
					}
				case 2:
					TextRelay del = info.GetTextEntry(0);
					try
					{
						if (_MValorTitleItem != null)
						{
							_MValorRc.GetTitles.RemoveAll((r => r.Title == del.Text));
						}
						else
						{
							_MValorRc.GetValorItems.RemoveAll((r => r.Title == del.Text));
						}
					}
					catch (Exception)
					{
						state.Mobile.SendMessage("Failed to delete!!!");
						throw;
					}
					state.Mobile.SendGump(new ValorTitleRewardAdmin(_MValorRc, null, 0, 0, null, null));

					break;
			}
		}
	}
}