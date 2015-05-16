#region References
using System;
using System.Collections;

using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
#endregion

namespace Server.Gumps
{
	public class VendorInventoryGump : Gump
	{
		private readonly BaseHouse m_House;
		private readonly ArrayList m_Inventories;

		public VendorInventoryGump(BaseHouse house, Mobile from)
			: base(50, 50)
		{
			m_House = house;
			m_Inventories = new ArrayList(house.VendorInventories);

			AddBackground(0, 0, 420, 50 + 20 * m_Inventories.Count, 0x13BE);

			AddImageTiled(10, 10, 400, 20, 0xA40);
			AddHtmlLocalized(15, 10, 200, 20, 1062435, 0x7FFF, false, false); // Reclaim Vendor Inventory
			AddHtmlLocalized(330, 10, 50, 20, 1062465, 0x7FFF, false, false); // Expires

			AddImageTiled(10, 40, 400, 20 * m_Inventories.Count, 0xA40);

			for (int i = 0; i < m_Inventories.Count; i++)
			{
				var inventory = (VendorInventory)m_Inventories[i];

				int y = 40 + 20 * i;

				if (inventory.Owner == from)
				{
					AddButton(10, y, 0xFA5, 0xFA7, i + 1, GumpButtonType.Reply, 0);
				}

				AddLabel(45, y, 0x481, String.Format("{0} ({1})", inventory.ShopName, inventory.VendorName));

				TimeSpan expire = inventory.ExpireTime - DateTime.UtcNow;
				var hours = (int)expire.TotalHours;

				AddLabel(320, y, 0x481, hours.ToString("#,0"));
				AddHtmlLocalized(350, y, 50, 20, 1062466, 0x7FFF, false, false); // hour(s)
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 0)
			{
				return;
			}

			Mobile from = sender.Mobile;
			HouseSign sign = m_House.Sign;

			if (m_House.Deleted || sign == null || sign.Deleted || !from.CheckAlive())
			{
				return;
			}

			if (from.Map != sign.Map || !from.InRange(sign, 5))
			{
				// You must be within five paces of the house sign to use this option.
				from.SendLocalizedMessage(1062429);
				return;
			}

			int index = info.ButtonID - 1;
			if (index < 0 || index >= m_Inventories.Count)
			{
				return;
			}

			var inventory = (VendorInventory)m_Inventories[index];

			if (inventory.Owner != from || !m_House.VendorInventories.Contains(inventory))
			{
				return;
			}

			int totalItems = 0;
			int givenToBackpack = 0;
			int givenToBankBox = 0;

			for (int i = inventory.Items.Count - 1; i >= 0; i--)
			{
				Item item = inventory.Items[i];

				if (item.Deleted)
				{
					inventory.Items.RemoveAt(i);
					continue;
				}

				totalItems += 1 + item.TotalItems;

				if (from.PlaceInBackpack(item))
				{
					inventory.Items.RemoveAt(i);
					givenToBackpack += 1 + item.TotalItems;
				}
				else
				{
					BankBox box = from.FindBank(m_House.Expansion) ?? from.BankBox;

					if (box.TryDropItem(from, item, false))
					{
						inventory.Items.RemoveAt(i);
						givenToBankBox += 1 + item.TotalItems;
					}
				}
			}

			from.SendMessage(
				"The vendor you selected had {0:#,0} items in its inventory, and {1:#,0} {2} in its account.",
				totalItems,
				inventory.Currency,
				inventory.TypeOfCurrency.Name);

			int givenGold = Banker.DepositUpTo(from, inventory.TypeOfCurrency, inventory.Currency);
			inventory.Currency -= givenGold;

			from.SendMessage("{0:#,0} {1} has been deposited into your bank box.", givenGold, inventory.TypeOfCurrency.Name);

			// ~1_COUNT~ items have been removed from the shop inventory and placed in your backpack.  
			// ~2_BANKCOUNT~ items were removed from the shop inventory and placed in your bank box.
			from.SendLocalizedMessage(1062437, givenToBackpack.ToString("#,0") + "\t" + givenToBankBox.ToString("#,0"));

			if (inventory.Currency > 0 || inventory.Items.Count > 0)
			{
				// Some of the shop inventory would not fit in your backpack or bank box.  
				// Please free up some room and try again.
				from.SendLocalizedMessage(1062440);
			}
			else
			{
				inventory.Delete();

				// The shop is now empty of inventory and funds, so it has been deleted.
				from.SendLocalizedMessage(1062438);
			}
		}
	}
}