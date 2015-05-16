#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Items;
using Server.Multis;
#endregion

namespace Server.Mobiles
{
	public class VendorInventory
	{
		public static readonly TimeSpan GracePeriod = TimeSpan.FromDays(7.0);

		private readonly Timer m_ExpireTimer;

		public List<Item> Items { get; private set; }
		public DateTime ExpireTime { get; private set; }

		public BaseHouse House { get; set; }
		public Mobile Owner { get; set; }
		public int Currency { get; set; }
		public string VendorName { get; set; }
		public string ShopName { get; set; }

		public Type TypeOfCurrency { get { return House != null && House.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold); } }

		public VendorInventory(BaseHouse house, Mobile owner, string vendorName, string shopName)
		{
			House = house;
			Owner = owner;
			VendorName = vendorName;
			ShopName = shopName;

			Items = new List<Item>();

			ExpireTime = DateTime.UtcNow + GracePeriod;

			m_ExpireTimer = new ExpireTimer(this, GracePeriod);
			m_ExpireTimer.Start();
		}

		public void AddItem(Item item)
		{
			item.Internalize();
			Items.Add(item);
		}

		public void Delete()
		{
			foreach (Item item in Items)
			{
				item.Delete();
			}

			Items.Clear();
			Currency = 0;

			if (House != null)
			{
				House.VendorInventories.Remove(this);
			}

			m_ExpireTimer.Stop();
		}

		public void Serialize(GenericWriter writer)
		{
			writer.WriteEncodedInt(0); // version

			writer.Write(Owner);
			writer.Write(VendorName);
			writer.Write(ShopName);

			writer.Write(Items, true);
			writer.Write(Currency);

			writer.WriteDeltaTime(ExpireTime);
		}

		public VendorInventory(BaseHouse house, GenericReader reader)
		{
			House = house;

			reader.ReadEncodedInt();

			Owner = reader.ReadMobile();
			VendorName = reader.ReadString();
			ShopName = reader.ReadString();

			Items = reader.ReadStrongItemList();
			Currency = reader.ReadInt();

			ExpireTime = reader.ReadDeltaTime();

			if (Items.Count == 0 && Currency == 0)
			{
				Timer.DelayCall(TimeSpan.Zero, Delete);
			}
			else
			{
				TimeSpan delay = ExpireTime - DateTime.UtcNow;

				m_ExpireTimer = new ExpireTimer(this, delay > TimeSpan.Zero ? delay : TimeSpan.Zero);
				m_ExpireTimer.Start();
			}
		}

		private class ExpireTimer : Timer
		{
			private readonly VendorInventory m_Inventory;

			public ExpireTimer(VendorInventory inventory, TimeSpan delay)
				: base(delay)
			{
				m_Inventory = inventory;

				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				BaseHouse house = m_Inventory.House;

				if (house != null)
				{
					if (m_Inventory.Currency > 0)
					{
						if (house.MovingCrate == null)
						{
							house.MovingCrate = new MovingCrate(house);
						}

						Banker.Deposit(house.MovingCrate, m_Inventory.TypeOfCurrency, m_Inventory.Currency);
					}

					foreach (Item item in m_Inventory.Items.Where(item => !item.Deleted))
					{
						house.DropToMovingCrate(item);
					}

					m_Inventory.Currency = 0;
					m_Inventory.Items.Clear();
				}

				m_Inventory.Delete();
			}
		}
	}
}