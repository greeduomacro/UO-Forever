//Author: krazeykow
//Version: 1.0.1

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Server.Commands;
using Server.ContextMenus;
using Server.Engines.CustomTitles;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Scripts.Items.Deeds;
using Server.Targeting;
#endregion

namespace Server.Items
{
	[TypeAlias("Currency")]
	public class Currency : Item
	{
		private Payment m_Payment;
		private PropertyInfo m_PropertyInfo;

		public Type XMLType { get; private set; }
		public Type PaymentType { get; private set; }
		public string PayName { get; private set; }
		public int PayID { get; private set; }
		public int CurrHue { get; private set; }

		public enum Payment
		{
			ByGold,
			BySilver,
			ByItem,
			ByProperty,
			ByXMLAttachment
		}

		//default
		public Currency()
		{
			m_Payment = Payment.ByGold;
			PaymentType = typeof(Gold);
			m_PropertyInfo = null;
			PayID = 3823;
			PayName = "Gold";
		}

		public Currency(object bindedTo, Type attachType, PropertyInfo attachProp)
		{
			XMLType = attachType;
			m_Payment = Payment.ByXMLAttachment;
			PaymentType = bindedTo.GetType();
			m_PropertyInfo = attachProp;

			if (bindedTo is Mobile)
			{
				PayID = 8461;
				PayName = "Player " + attachProp.Name;
			}
			else if (bindedTo is Item)
			{
				PayID = ((Item)bindedTo).ItemID;
				PayName = GetName((Item)bindedTo) + "'s " + attachProp.Name;
				CurrHue = ((Item)bindedTo).Hue;
			}
		}

		//consume by item
		public Currency(Item i)
		{
			Type t = i.GetType();

			if (t.IsEqual<Gold>())
			{
				m_Payment = Payment.ByGold;
			}
			else if (t.IsEqual<Silver>())
			{
				m_Payment = Payment.BySilver;
			}
			else
			{
				m_Payment = Payment.ByItem;
			}

			PaymentType = i.GetType();
			m_PropertyInfo = null;
			PayID = CoinID(i.ItemID);
			PayName = GetName(i);
			CurrHue = i.Hue;
		}

		//consume property on player
		public Currency(Mobile m, PropertyInfo pi)
		{
			m_Payment = Payment.ByProperty;
			PaymentType = m.GetType();
			m_PropertyInfo = pi;
			PayID = 8461;
			PayName = "Player " + pi.Name;
		}

		//consume by item's number based property
		public Currency(Item i, PropertyInfo pi)
		{
			m_Payment = Payment.ByProperty;
			PaymentType = i.GetType();
			m_PropertyInfo = pi;
			PayID = CoinID(i.ItemID);
			PayName = GetName(i) + "'s " + pi.Name;
			CurrHue = i.Hue;
		}

		public void Modify(Type t, string append)
		{
			if (t.IsEqual<PlayerMobile>() || append == null)
			{
				return;
			}

			Item i = Activator.CreateInstance(t, null) as Item;

			if (i == null)
			{
				return;
			}

			PayName = GetName(i) + append;
			CurrHue = i.Hue;
			PayID = CoinID(PayID);
		}

		public int CoinID(int itemID)
		{
			switch (itemID)
			{
				case 3821:
					return 3823;
				case 3824:
					return 3826;
				default:
					return itemID;
			}
		}

		public void VOneUpdate()
		{
			switch (m_Payment)
			{
				case Payment.ByGold:
				case Payment.ByItem:
					{
						Modify(PaymentType, "");
						break;
					}
				case Payment.ByProperty:
					{
						if (!PaymentType.IsEqual<PlayerMobile>())
						{
							Modify(PaymentType, "'s " + m_PropertyInfo.Name);
						}
						break;
					}
			}
		}

		public string GetName(Item i)
		{
			return i.ResolveName();
		}

		public bool Purchase(Mobile m, int cost)
		{
			int remain = Value(m) - cost;

			if (remain < 0 || m.Backpack == null)
			{
				return false;
			}

			Charge(m, cost, remain);

			return true;
		}

		public void Charge(Container pack, Container bank, Type t, int cost)
		{
			if (pack == null || bank == null)
			{
				return;
			}

			cost = cost - pack.ConsumeUpTo(t, cost);

			if (cost > 0)
			{
				bank.ConsumeUpTo(t, cost);
			}
		}

		public void Charge(Mobile m, Type t, PropertyInfo pi, int cost, int remain)
		{
			try
			{
				if (t.BaseType.IsEqual<Mobile>() || t.IsEqual<PlayerMobile>())
				{
					SetPropertyValue(m, remain);
				}
				else
				{
					if (m.Backpack == null || m.FindBankNoCreate() == null)
					{
						return;
					}

					Item[] packItems = m.Backpack.FindItemsByType(t);
					Item[] bankItems = m.BankBox.FindItemsByType(t);

					var items = new Item[packItems.Length + bankItems.Length];
					packItems.CopyTo(items, 0);
					bankItems.CopyTo(items, packItems.Length);

					//none found in backpack
					if (items.Length == 0)
					{
						return;
					}

					int i = 0;
					int value;
					int dif;

					//account for multiple objects
					while (cost > 0 && i < items.Length)
					{
						value = GetPropertyValue(items[i]);

						dif = value - cost;

						if (dif >= 0)
						{
							//one item has enough

							SetPropertyValue(items[i], dif);
						}
						else // assert (dif < 0)
						{
							//take what amount item has (the amount has already been confirmed by Value(m))

							cost -= GetPropertyValue(items[i]);
							SetPropertyValue(items[i], 0);
						}

						i++;
					}
				}
			}
			catch
			{ }
		}

		public void Charge(Mobile m, Container pack, Container bank, Type currencyType, int cost)
		{
			if (pack == null || bank == null)
			{
				return;
			}

			cost = cost - pack.ConsumeUpTo(currencyType, cost);

			if (cost > 0)
			{
				Banker.Withdraw(m, currencyType, cost);
			}
		}

		public void Charge(Mobile m, int cost, int remain)
		{
			switch (m_Payment)
			{
				case Payment.ByGold:
					Charge(m, m.Backpack, m.FindBankNoCreate(), typeof(Gold), cost);
					break;
				case Payment.BySilver:
					Charge(m, m.Backpack, m.FindBankNoCreate(), typeof(Silver), cost);
					break;
				case Payment.ByItem:
					Charge(m.Backpack, m.FindBankNoCreate(), PaymentType, cost);
					break;
				case Payment.ByXMLAttachment:
				case Payment.ByProperty:
					Charge(m, PaymentType, m_PropertyInfo, cost, remain);
					break;
			}
		}

		public int Value(Mobile m)
		{
			switch (m_Payment)
			{
				case Payment.ByGold:
					return ProcessByGold(m);
				case Payment.BySilver:
					return ProcessBySilver(m);
				case Payment.ByItem:
					return ProcessByItem(m, PaymentType);
				case Payment.ByXMLAttachment:
				case Payment.ByProperty:
					return ProcessByProperty(m, PaymentType, m_PropertyInfo);
			}
			return 0;
		}

		public static int Value(Mobile m, Currency currency)
		{
			switch (currency.m_Payment)
			{
				case Payment.ByGold:
					return currency.ProcessByGold(m);
				case Payment.BySilver:
					return currency.ProcessBySilver(m);
				case Payment.ByItem:
					return currency.ProcessByItem(m, currency.PaymentType);
				case Payment.ByXMLAttachment:
				case Payment.ByProperty:
					return currency.ProcessByProperty(m, currency.PaymentType, currency.m_PropertyInfo);
			}
			return 0;
		}

		public int ProcessByGold(Mobile m)
		{
			if (m.Backpack == null)
			{
				return 0;
			}

			int totalGold = 0;

			totalGold += m.Backpack.GetAmount(typeof(Gold), true);
			totalGold += Banker.GetBalance(m, typeof(Gold));

			return totalGold;
		}

		public int ProcessBySilver(Mobile m)
		{
			if (m.Backpack == null)
			{
				return 0;
			}

			int totalSilver = 0;

			totalSilver += m.Backpack.GetAmount(typeof(Silver), true);
			totalSilver += Banker.GetBalance(m, typeof(Silver));

			return totalSilver;
		}

		public int ProcessByItem(Mobile m, Type t)
		{
			Container pack = m.Backpack;
			Container bank = m.FindBankNoCreate();

			if (pack == null || bank == null)
			{
				return 0;
			}

			int amount = pack.GetAmount(t, true) + bank.GetAmount(t, true);

			return amount;
		}

		public int ProcessByProperty(Container pack, Container bank, Type t, PropertyInfo pi)
		{
			if (pack == null || bank == null)
			{
				return 0;
			}

			Item[] packItems = pack.FindItemsByType(t);
			Item[] bankItems = bank.FindItemsByType(t);

			var items = new Item[packItems.Length + bankItems.Length];
			packItems.CopyTo(items, 0);
			bankItems.CopyTo(items, packItems.Length);

			return items.Sum(o => GetPropertyValue(o));
		}

		public int GetPropertyValue(object o)
		{
			int val;

			if (m_Payment == Payment.ByXMLAttachment)
			{
				val = GetXMLValue(o);
			}
			else
			{
				val = (int)m_PropertyInfo.GetValue(o, null);
			}

			return val;
		}

		public void SetPropertyValue(object o, int val)
		{
			if (m_Payment == Payment.ByXMLAttachment)
			{
				SetXMLValue(o, val);
			}
			else
			{
				m_PropertyInfo.SetValue(o, val, null);
			}
		}

		public int GetXMLValue(object o)
		{
			int val = 0;

			if (XMLType == null)
			{
				return val;
			}

			XmlAttachment attach = XmlAttach.FindAttachment(o as IEntity, XMLType);

			if (attach != null)
			{
				val = (int)m_PropertyInfo.GetValue(attach, null);
			}
			return val;
		}

		public void SetXMLValue(object o, int val)
		{
			if (XMLType == null)
			{
				return;
			}

			XmlAttachment attach = XmlAttach.FindAttachment(o as IEntity, XMLType);

			if (attach != null)
			{
				m_PropertyInfo.SetValue(attach, val, null);
			}
		}

		//Master
		public int ProcessByProperty(Mobile m, Type t, PropertyInfo pi)
		{
			try
			{
				if (t.BaseType.IsEqual<Mobile>() || t.IsEqual<Mobile>())
				{
					return GetPropertyValue(m);
				}
				else
				{
					return ProcessByProperty(m.Backpack, m.FindBankNoCreate(), t, pi);
				}
			}
			catch
			{ }

			return 0;
		}

		public Currency(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(2);

			//version 2
			writer.Write((XMLType == null ? "" : XMLType.Name));

			//version 1
			writer.Write(CurrHue);

			//explicit casts are given for clarification        
			writer.Write((int)m_Payment);
			writer.Write(PaymentType.Name);
			writer.Write((m_PropertyInfo == null ? "" : m_PropertyInfo.Name));
			writer.Write(PayID);
			writer.Write(PayName);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
					{
						try
						{
							string name = reader.ReadString();
							
							XMLType = name.Equals("") ? null : ScriptCompiler.FindTypeByName(name);
						}
						catch
						{ }

						goto case 1;
					}
				case 1:
					{
						CurrHue = reader.ReadInt();
						goto case 0;
					}
				case 0:
					{
						m_Payment = (Payment)reader.ReadInt();

						try
						{
							string name = reader.ReadString();
							PaymentType = ScriptCompiler.FindTypeByName(name);
						}
						catch
						{ }

						try
						{
							string propName = reader.ReadString();
							if (propName.Equals("")) //never set
							{
								m_PropertyInfo = null;
							}
							else if (m_Payment == Payment.ByXMLAttachment && XMLType != null)
								//XML requires find property by the XML Class type (m_XML_Type)
							{
								m_PropertyInfo = XMLType.GetProperty(propName);
							}
							else
							{
								m_PropertyInfo = PaymentType.GetProperty(propName);
							}
						}
						catch
						{ }

						PayID = reader.ReadInt();
						PayName = reader.ReadString();

						break;
					}
			}
			if (version == 0)
			{
				try
				{
					VOneUpdate();
				}
				catch
				{ }
			}
		}
	}

	[TypeAlias("Reward")]
	public class Reward : Item, ICloneable
	{
		private ObjectPropertyList m_Display;

		public int BuyCount { get; set; }
		public RestockInfo Restock { get; set; }

		public ObjectPropertyList Display
		{
			get
			{
				if (m_Display == null)
				{
					m_Display = new ObjectPropertyList(this);

					RewardInfo.GetProperties(m_Display);
					RewardInfo.AppendChildProperties(m_Display);

					m_Display.Terminate();
					m_Display.SetStatic();
				}

				return m_Display;
			}
		}

		public void Void_RestockInfo()
		{
			Restock = null;
		}

		//returns ((-1) -> no restockInfo) or current count
		public int Try_Restock()
		{
			if (Restock == null)
			{
				return -1;
			}

			TimeSpan dif = DateTime.UtcNow - Restock.LastRestock;
			int cycles = 0;

			if (dif > (Restock.RestockRate) && Restock.RestockRate.TotalMinutes > 0)
			{
				cycles = (int)(dif.TotalMinutes / Restock.RestockRate.TotalMinutes);

				for (int i = 0; i < cycles; ++i)
				{
					if (Restock.Restock())
					{
						continue;
					}
				}
			}

			return Restock.Count;
		}

		public bool InStock(int amount)
		{
			return Restock == null || Restock.Count - amount >= 0;
		}

		public void RegisterBuy(int amount)
		{
			if (Restock != null && InStock(amount))
			{
				Restock.Count -= amount;
			}

			BuyCount += amount;
		}

		//for retrieving information only
		public Item RewardInfo { get; private set; }

		//use only when creating Reward for player
		public Item RewardCopy { get { return GetReward(); } }

		public int Cost { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		object ICloneable.Clone()
		{
			return new Reward(this);
		}

		public Reward()
		{
			RewardInfo = new Item();
			Cost = 0;
			Title = "";
			Description = "";
			Restock = null;
		}

		public Reward(Reward r)
		{
			RewardInfo = r.RewardCopy;
			Cost = r.Cost;
			Title = r.Title;
			Description = r.Description;
			Restock = null;
		}

		public Reward(Item i)
		{
			RewardInfo = i;
			Cost = 0;
			Title = i.ResolveName();
			Description = "None.";
			Restock = null;
		}

		public Reward(Item i, int cost)
		{
			RewardInfo = i;
			Cost = cost;
			Title = i.ResolveName();
			Description = "None.";
			Restock = null;
		}

		public Reward(Item i, int cost, string title)
		{
			RewardInfo = i;
			Cost = cost;
			Title = title;
			Description = "None.";
			Restock = null;
		}

		public Reward(Item i, int cost, string title, string desc)
		{
			RewardInfo = i;
			Cost = cost;
			Title = title;
			Description = desc;
			Restock = null;
		}

		public class RestockInfo
		{
			private int? m_Maximum;

			public TimeSpan RestockRate { get; private set; }
			public DateTime LastRestock { get; private set; }

			public int RestockAmnt { get; private set; }
			public int Count { get; set; }
			public int Maximum { get { return m_Maximum.GetValueOrDefault(-1); } }

			public int Hours { get { return RestockRate.Hours; } }
			public int Minutes { get { return RestockRate.Minutes; } }

			public RestockInfo()
			{
				RestockRate = new TimeSpan();
				RestockAmnt = 0;
				Count = 0;
				m_Maximum = null;
				LastRestock = DateTime.UtcNow;
			}

			public RestockInfo(int count)
				: this()
			{
				Count = count;
			}

			public RestockInfo(int hour, int minute, int count, int RestockNum)
			{
				RestockRate = new TimeSpan(hour, minute, 0);
				Count = count;
				RestockAmnt = RestockNum;
				m_Maximum = null;
				LastRestock = DateTime.UtcNow;
			}

			public RestockInfo(int hour, int minute, int count, int RestockNum, int maxNum)
				: this(hour, minute, count, RestockNum)
			{
				m_Maximum = maxNum;
			}

			public bool Equals(RestockInfo info)
			{
				return (RestockRate.Equals(info.RestockRate) && (RestockAmnt == info.RestockAmnt) && (Count == info.Count) &&
						(m_Maximum == info.Maximum));
			}

			public bool Restock()
			{
				LastRestock = DateTime.UtcNow;

				if (m_Maximum != null) //Restock to a limit
				{
					if ((Count + RestockAmnt) > m_Maximum)
					{
						Count = m_Maximum.Value;
						return false;
					}

					Count += RestockAmnt;
					return true;
				}

				Count += RestockAmnt;
				return true;
			}

			public void Serialize(GenericWriter writer)
			{
				writer.Write(RestockRate);
				writer.Write(RestockAmnt);
				writer.Write(Count);
				writer.Write(m_Maximum.GetValueOrDefault(-1));
			}

			public RestockInfo(GenericReader reader)
			{
				RestockRate = reader.ReadTimeSpan();
				LastRestock = DateTime.UtcNow;
				RestockAmnt = reader.ReadInt();
				Count = reader.ReadInt();

				int? max = reader.ReadInt();

				m_Maximum = ((max < 0) ? null : max);
			}
		}

		public void Edit(int cost, string title, string desc)
		{
			if (cost < 0 || title == null || desc == null)
			{
				return;
			}

			Cost = cost;
			Title = title;
			Description = desc;
		}

		public Item GetReward()
		{
			return ItemClone.Clone(RewardInfo, false);
		}

		public Reward(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1);

			writer.Write(BuyCount);

			if (Restock == null)
			{
				writer.Write(false);
			}
			else
			{
				writer.Write(true);
				Restock.Serialize(writer);
			}

			writer.WriteItem(RewardInfo);
			writer.Write(Cost);
			writer.Write(Title);
			writer.Write(Description);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					{
						BuyCount = reader.ReadInt();

						if (reader.ReadBool())
						{
							Restock = new RestockInfo(reader);
						}

						goto case 0;
					}
				case 0:
					{
						RewardInfo = reader.ReadItem();
						Cost = reader.ReadInt();
						Title = reader.ReadString();
						Description = reader.ReadString();

						break;
					}
			}
		}
	}

	public class RewardCollection : List<Reward>, ICloneable
	{
		object ICloneable.Clone()
		{
			var clone = new RewardCollection();

			foreach (Reward r in this)
			{
                if (r.RewardInfo != null)
				    clone.Add(((Reward)(r as ICloneable).Clone()));
			}
			return clone;
		}
	}

	public static class WorldRewardVendors
	{
		private static readonly List<IRewardVendor> m_Vendors = new List<IRewardVendor>();

		public static List<IRewardVendor> Vendors { get { return m_Vendors; } }

		public static void RegisterVendor(IRewardVendor vendor)
		{
			m_Vendors.Add(vendor);
		}

		public static void RemoveVendor(IRewardVendor vendor)
		{
			m_Vendors.Remove(vendor);
		}
	}

	public class EditItemGump : Gump
	{
		private readonly IRewardVendor m_Vendor;
		private readonly Reward m_Reward;

		private readonly Reward.RestockInfo m_Restock;
		private readonly bool m_RestockOpen;

		private readonly bool m_IsAdd;

		private readonly Storage m_Entries;

		public EditItemGump(IRewardVendor vendor, Reward r, Storage entries, bool openRestockInfo, bool addingItem)
			: base(0, 0)
		{
			m_Vendor = vendor;
			m_Reward = r;

			m_Restock = r.Restock;
			m_RestockOpen = openRestockInfo;
			m_IsAdd = addingItem;

			m_Entries = entries;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			AddImageTiled(227, 46, 329, 518, 2702);
			AddImageTiled(252, 291, 280, 217, 5104);
			AddLabel(253, 266, 2124, @"Description:");
			AddImageTiled(334, 147, 192, 23, 5104);
			AddLabel(258, 150, 2124, @"Cost:");
			AddImageTiled(336, 107, 192, 23, 5104);
			AddLabel(258, 108, 2124, @"Name:");
			AddImage(354, 199, 2328);
			AddItem(373, 208, r.RewardInfo.ItemID, r.RewardInfo.Hue);
			AddImageTiled(330, 54, 138, 16, 5214);
			AddLabel(373, 51, 1324, @"Edit Item");
			AddButton(465, 222, 5601, 5605, 2, GumpButtonType.Reply, 0); //restock
			AddLabel(447, 198, 2120, @"Restock ");
			AddLabel(447, 246, 2120, @"(Optional)");
			AddButton(324, 523, 4023, 4024, 1, GumpButtonType.Reply, 0); //ok
			AddButton(421, 523, 4017, 4018, 0, GumpButtonType.Reply, 0); //cancel

			if (entries == null)
			{
				InitialText();
			}
			else
			{
				PacketText();
			}

			if (openRestockInfo || r.Restock != null)
			{
				AddRestockMenu();
				m_RestockOpen = true;
			}
		}

		public void InitialText()
		{
			AddTextEntry(252, 292, 277, 215, 1879, 0, m_Reward.Description);
			AddTextEntry(334, 148, 188, 20, 1879, 1, m_Reward.Cost.ToString());
			AddTextEntry(336, 108, 188, 20, 1879, 2, m_Reward.Title);
		}

		public void PacketText()
		{
			AddTextEntry(252, 292, 277, 215, 1879, 0, m_Entries[0].ToString()); //desc
			AddTextEntry(334, 148, 188, 20, 1879, 1, m_Entries[1].ToString()); //cost 
			AddTextEntry(336, 108, 188, 20, 1879, 2, m_Entries[2].ToString()); //title
		}

		public void AddRestockMenu()
		{
			AddPage(1);

			AddImageTiled(601, 47, 181, 469, 2624);
			AddLabel(648, 60, 2129, @"Restock Info");
			AddLabel(637, 98, 1418, @"Available");
			AddImageTiled(633, 123, 120, 16, 5214);
			AddTextEntry(637, 121, 107, 20, 547, 3, (m_Restock == null ? "Enter." : m_Restock.Count.ToString()));
			//3 - beginning amount
			AddLabel(660, 161, 1418, @"Maximum");
			AddImageTiled(631, 186, 120, 16, 5214);
			AddTextEntry(
				635, 184, 107, 20, 547, 4, ((m_Restock == null || m_Restock.Maximum < 0) ? "None." : m_Restock.Maximum.ToString()));
			//4 - Maxinum
			AddLabel(648, 227, 1418, @"Restock Rate");
			AddImageTiled(635, 252, 72, 16, 5214);
			AddTextEntry(638, 250, 64, 20, 547, 5, (m_Restock == null ? "0" : m_Restock.Hours.ToString())); //5 - hour
			AddLabel(714, 250, 602, @"hours");
			AddImageTiled(635, 292, 72, 16, 5214);
			AddLabel(716, 290, 602, @"mins");
			AddLabel(632, 323, 602, @"amount to restock");
			AddImageTiled(636, 355, 72, 16, 5214);
			AddTextEntry(638, 290, 64, 20, 547, 6, (m_Restock == null ? "0" : m_Restock.Minutes.ToString())); //6 - minute
			AddTextEntry(639, 353, 64, 20, 547, 7, (m_Restock == null ? "0" : m_Restock.RestockAmnt.ToString()));
			//7 - Restock Amount      
			AddImage(655, 377, 109);
			AddButton(636, 474, 4022, 4021, 3, GumpButtonType.Reply, 0);
			AddLabel(679, 475, 132, @"Void Restock");
		}

		public Reward.RestockInfo GetRestockInfo(RelayInfo info)
		{
			if (m_RestockOpen)
			{
				int numMinutes = 0, numHours = 0;
				int numBegin, numMax, numRestockAmnt;

				TextRelay entry3 = info.GetTextEntry(3);
				string beginAmount = (entry3.Text.Trim());

				TextRelay entry4 = info.GetTextEntry(4);
				string max = (entry4.Text.Trim());

				TextRelay entry5 = info.GetTextEntry(5);
				string hour = (entry5.Text.Trim());

				TextRelay entry6 = info.GetTextEntry(6);
				string minute = (entry6.Text.Trim());

				TextRelay entry7 = info.GetTextEntry(7);
				string restockAmount = (entry7.Text.Trim());

				Int32.TryParse(hour, out numHours);
				Int32.TryParse(minute, out numMinutes);
				Int32.TryParse(beginAmount, out numBegin);
				Int32.TryParse(max, out numMax);
				Int32.TryParse(restockAmount, out numRestockAmnt);

				if (numBegin > 0)
				{
					if ((numMinutes + numHours) > 0 && numRestockAmnt > 0)
					{
						if (numMax > 0)
						{
							return new Reward.RestockInfo(numHours, numMinutes, numBegin, numRestockAmnt, numMax);
						}
						else
						{
							return new Reward.RestockInfo(numHours, numMinutes, numBegin, numRestockAmnt);
						}
					}

					return new Reward.RestockInfo(numBegin);
				}
			}
			return m_Restock; //default
		}

		public Storage GetEntries(RelayInfo info)
		{
			//Description
			TextRelay entry0 = info.GetTextEntry(0);
			string desc = (entry0 == null ? "None." : entry0.Text.Trim());

			//Cost
			TextRelay entry1 = info.GetTextEntry(1);
			string costtext = (entry1 == null ? "0" : entry1.Text.Trim());

			//Name
			TextRelay entry2 = info.GetTextEntry(2);
			string name = (entry2 == null ? "" : entry2.Text.Trim());

			return new Storage(desc, costtext, name);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			if (m.AccessLevel < MobileRewardVendor.FullAccessStaffLevel)
			{
				LoggingCustom.Log(
					"RewardsVendor.txt", "ERROR: non-staff in " + GetType() + " gump!" + "\t" + m.Name + "\t" + m.Account);
				return;
			}

			switch (info.ButtonID)
			{
				case 0:
					{
						//cancel
						break;
					}
				case 1: //ok
					{
						try
						{
							//Description
							TextRelay entry0 = info.GetTextEntry(0);
							string desc = (entry0 == null ? "None." : entry0.Text.Trim());

							//Cost
							TextRelay entry1 = info.GetTextEntry(1);
							string costtext = (entry1 == null ? "%Error" : entry1.Text.Trim());
							int cost = Int32.Parse(costtext);

							//Name
							TextRelay entry2 = info.GetTextEntry(2);
							string name = (entry2 == null ? "" : entry2.Text.Trim());

							if (m_IsAdd)
							{
								m_Vendor.AddReward(m_Reward);
							}

							m_Reward.Edit(cost, name, desc);

							if (m_RestockOpen)
							{
								Reward.RestockInfo input = GetRestockInfo(info);

								if (input != null)
								{
									if (m_Restock != null && !(m_Restock.Equals(input))) //do not reassign same object
									{
										m_Reward.Restock = input;
									}
									else
									{
										m_Reward.Restock = input;
									}
								}
							}

							m.SendMessage("{0} has been modified in {1}'s collection", name, m_Vendor.GetName());
						}
						catch
						{
							m.SendMessage("Please make sure all fields are filled correctly, and the title has not been previously used.");
						}

						break;
					}
				case 2: //open restock options
					{
						m.SendGump(new EditItemGump(m_Vendor, m_Reward, GetEntries(info), true, m_IsAdd));

						return; //override display 
					}
				case 3: //delete restock options
					{
						m_Reward.Void_RestockInfo();

						m.SendGump(new EditItemGump(m_Vendor, m_Reward, GetEntries(info), false, m_IsAdd));

						return; //override display 
					}
			}
			MenuUploader.Display(m_Vendor.Menu, m, m_Vendor, true); //send back display
		}
	}

	[TypeAlias("Exhibit")]
	public class Exhibit : Item
	{
		private IRewardVendor m_Vendor;
		private Reward m_Reward;
		private ObjectPropertyList m_PropertyList;

		public Exhibit(IRewardVendor vendor, Reward r)
		{
			m_Vendor = vendor;
			m_Reward = r;

			SetOPL(r);

			ItemID = r.RewardInfo.ItemID;
			Hue = r.RewardInfo.Hue;
		}

		public override void SendPropertiesTo(Mobile from)
		{
			if (m_PropertyList == null)
			{
				m_PropertyList = new ObjectPropertyList(this);
				m_PropertyList.Add("Display [Broken]");
			}

			from.Send(m_PropertyList);
		}

		public void SetOPL(Reward r)
		{
			Item i = r.RewardInfo;

			m_PropertyList = new ObjectPropertyList(this);

			m_PropertyList.Add("Cost: " + m_Reward.Cost.ToString() + " " + m_Vendor.Payment.PayName + " [Click for more]");

			i.GetProperties(m_PropertyList);
			i.AppendChildProperties(m_PropertyList);

			m_PropertyList.Terminate();
			m_PropertyList.SetStatic();
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (m_Vendor == null || m_Vendor.IsRemoved() || m_Reward == null || !m_Vendor.Display.Contains(m_Reward))
			{
				m_Vendor = null; // free up resource
				m_Reward = null;
				m.SendMessage("This item is no longer available to purchase.");
			}
			else
			{
				m.SendGump(new ViewItemGump(m, m_Vendor, m_Reward, false));
			}
		}

		public Exhibit(Serial serial)
			: base(serial)
		{ }

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if (reader.ReadBool())
			{
				m_Reward = (Reward)reader.ReadItem();

				m_Vendor = (IRewardVendor)reader.ReadMobile();

				try
				{
					SetOPL(m_Reward);
				}
				catch
				{ }
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version  

			if (m_Vendor == null || m_Vendor.IsRemoved() || m_Reward == null || m_Reward.Deleted)
			{
				m_Vendor = null;
				m_Reward = null;

				writer.Write(false); //do not deserialize
			}
			else
			{
				writer.Write(true);

				writer.WriteItem(m_Reward);

				writer.Write(m_Vendor.GetMobile());
			}
		}
	}

	public class ViewItemGump : Gump
	{
		private readonly IRewardVendor m_Vendor;
		private readonly Reward m_Reward;

		public ViewItemGump(Mobile m, IRewardVendor vendor, Reward r, bool viewItem)
			: base(0, 0)
		{
			m_Vendor = vendor;
			m_Reward = r;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImageTiled(149, 207, 548, 277, 2624);
			AddImage(184, 300, 2328);
			AddImage(101, 222, 10400);

			AddHtml(428, 290, 243, 140, r.Description, true, true);
			AddItem(201, 311, r.RewardInfo.ItemID, r.RewardInfo.Hue);

			AddLabel(430, 263, 2101, @"Description");
			AddButton(192, 442, 4029, 4030, 2, GumpButtonType.Reply, 0); //buy
			AddLabel(231, 444, 2125, "Buy (" + r.Cost + ")");
			AddButton(366, 442, 4020, 4021, 0, GumpButtonType.Reply, 0); //cancel
			AddLabel(407, 444, 2116, @"Cancel");
			AddImageTiled(184, 239, 510, 8, 9201);

			AddItem(182, 376, m_Vendor.Payment.PayID, m_Vendor.Payment.CurrHue);
			AddLabel(248, 369, 83, @"Pay By: " + m_Vendor.Payment.PayName);
			AddLabel(184, 268, 43, @"Vendor: " + m_Vendor.GetName());
			AddLabel(363, 216, 2123, r.Title);

			if (m.AccessLevel >= MobileRewardVendor.FullAccessStaffLevel) //GM capabilities
			{
				if (!viewItem)
				{
					AddLabel(363, 216, 2123, r.Title);
				}
				else
				{
					AddLabel(336, 296, 2101, @"Edit Item");
					AddButton(294, 293, 4011, 4012, 4, GumpButtonType.Reply, 0); //edit item

					if (m_Vendor.GetMobile() != null)
					{
						AddLabel(336, 348, 2101, @"Get Display");
						AddButton(294, 345, 4011, 4012, 3, GumpButtonType.Reply, 0); //get display   
					}
				}
			}
			else
			{
				OpenDisplay(m); //prevent build up of box display during editing
			}

			//Edit gump for display object
			if (viewItem) //not display
			{
				AddLabel(336, 322, 2101, @"View Item");
				AddButton(294, 319, 4011, 4012, 1, GumpButtonType.Reply, 0); //view item
			}
			else //display
			{
				AddLabel(279, 320, 38, @"Cost: " + m_Reward.Cost.ToString());
				AddLabel(249, 398, 69, @"You have: " + m_Vendor.Payment.Value(m));

				if (m_Reward.Restock != null)
				{
					m_Reward.Try_Restock();
					AddLabel(279, 340, 4, "In Stock: " + m_Reward.Restock.Count.ToString());
				}

				if (m_Reward.RewardInfo is Container)
				{
					OpenDisplay(m);
				}
			}
		}

		public void OpenDisplay(Mobile m)
		{
			DisplayBox dbox = m_Vendor.Display;

			if (dbox != null)
			{
				dbox.DisplayTo(m, m_Reward);
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (m_Vendor.IsRemoved())
			{
				return;
			}

			switch (info.ButtonID)
			{
				case 0:
					{
						break;
					}
				case 1:
					{
						try
						{
							OpenDisplay(m);
							m.SendGump(this);
						}
						catch
						{
							m.SendMessage("This Reward can no longer be found.");
						}
						break;
					}
				case 2:
					{
						Container bank = m.BankBox;
						Container pack = m.Backpack;

						if (bank == null || pack == null)
						{
							break;
						}

						Currency curr = m_Vendor.Payment;

						if (m_Reward.InStock(1))
						{
							var sb = new StringBuilder();
							sb.Append(
								DateTime.Now + "\t" + m + "\t" + m.Account + "\t" + curr.PaymentType + "\tbefore: " + Currency.Value(m, curr));

							if (curr.Purchase(m, m_Reward.Cost))
							{
								m_Reward.RegisterBuy(1);

								Item i = m_Reward.RewardCopy;

								if (!m.PlaceInBackpack(i))
								{
									bank.DropItem(i);
									m.SendMessage("You are overweight, the Reward was added to your bank");
								}

								m.SendMessage("You bought {0} for {1} {2}.", m_Reward.Title, m_Reward.Cost, curr.PayName);
								sb.Append("\tafter: " + Currency.Value(m, curr) + "\tCost: " + m_Reward.Cost + "\t" + i.GetType());
								LoggingCustom.Log("RewardsVendor.txt", sb.ToString());
							}
							else
							{
								m.SendMessage("You cannot afford {0}", m_Reward.Title);
								sb.Append("\tCan't afford\tCost: " + m_Reward.Cost);
								LoggingCustom.Log("RewardsVendor.txt", sb.ToString());
							}
						}
						else
						{
							m.SendMessage("{0} is no longer in stock.", m_Reward.Title);
						}

						break;
					}
				case 3:
					{
						if (m.AccessLevel < MobileRewardVendor.FullAccessStaffLevel)
						{
							LoggingCustom.Log(
								"RewardsVendor.txt", "ERROR: non-staff in " + GetType() + " gump case 3!" + "\t" + m.Name + "\t" + m.Account);
							return;
						}
						if (m.Backpack != null)
						{
							m.AddToBackpack(new Exhibit(m_Vendor, m_Reward));
						}
						break;
					}
				case 4:
					{
						if (m.AccessLevel < MobileRewardVendor.FullAccessStaffLevel)
						{
							LoggingCustom.Log(
								"RewardsVendor.txt", "ERROR: non-staff in " + GetType() + " gump case 4!" + "\t" + m.Name + "\t" + m.Account);
							return;
						}
						m.SendGump(new EditItemGump(m_Vendor, m_Reward, null, false, false));
						break;
					}
			}
		}
	}

	//Created for the usage of Warning Gump delegate 
	public class Storage
	{
		private readonly object[] m_Objs;

		public object this[int index] { get { return m_Objs[index]; } }

		public Storage(params object[] input)
		{
			m_Objs = input;
		}
	}

	public class WorldVendorsGump : Gump
	{
		private readonly IRewardVendor m_Vendor; //used in case of copy

		public WorldVendorsGump(IRewardVendor vendor)
			: base(0, 0)
		{
			m_Vendor = vendor;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			//background
			AddImageTiled(50, 3, 430, 556, 2702);
			AddImageTiled(93, 78, 387, 9, 10101);
			AddImage(33, 58, 10420);
			AddImageTiled(47, 2, 3, 556, 10004);
			AddImageTiled(0, 58, 82, 414, 10440);
			AddImageTiled(47, 558, 436, 3, 10001);
			AddImageTiled(47, 0, 435, 3, 10001);
			AddImageTiled(480, 2, 3, 556, 10004);

			AddPage(1);

			CreateVendors(); //fill out entries
		}

		public static void CopyVendor_Callback(Mobile from, bool okay, object state)
		{
			if (from.AccessLevel < MobileRewardVendor.FullAccessStaffLevel)
			{
				return;
			}

			if (okay)
			{
				//indexes -> (vendor (0), vendor's copy target(1))
				var store = (Storage)state;

				var source = (IRewardVendor)store[0];
				var target = (IRewardVendor)store[1];

				try
				{
					source.CopyVendor(target);
				}
				catch
				{
					from.SendMessage("Error occured while copying, please insure backpack is on vendors.");
					return;
				}

				from.SendMessage("{0} has copied {1}'s Reward collection", source.GetName(), target.GetName());
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID == 0)
			{
				m.CloseGump(typeof(ControlPanelGump));
				return;
			}

			if (m.AccessLevel < MobileRewardVendor.FullAccessStaffLevel)
			{
				LoggingCustom.Log(
					"RewardsVendor.txt", "ERROR: non-staff in " + GetType() + " gump!" + "\t" + m.Name + "\t" + m.Account);
				return;
			}

			MenuUploader.Display(m_Vendor.Menu, m, m_Vendor, false);

			try
			{
				if (info.ButtonID % 2 == 0) //even ID: Copy Vendor
				{
					IRewardVendor selected = WorldRewardVendors.Vendors[(info.ButtonID / 2) - 1];

					if (m_Vendor.GetName().Equals(selected.GetName()))
					{
						m.SendMessage("A vendor cannot copy itself.");
					}
					else
					{
						//params -> (vendor, vendor's copy target)
						var store = new Storage(m_Vendor, selected);

						m.SendGump(
							new WarningGump(
								1060635,
								30720,
								"Warning: You will lose all saved items on " + m_Vendor.GetName(),
								0xFFC000,
								420,
								400,
								CopyVendor_Callback,
								store));
					}
				}
				else //assert odd ID: Goto 
				{
					IRewardVendor v = WorldRewardVendors.Vendors[((info.ButtonID + 1) / 2) - 1];

					m.Location = v.GetLocation();
					m.Map = v.GetMap();
				}
			}
			catch
			{
				m.SendMessage("Vendor cannot be found.");
			}
		}

		public void CreateVendors()
		{
			int yPos = 101, pageNum = 1, buttonNum = 0;

			for (int i = 0; i < WorldRewardVendors.Vendors.Count; ++i)
			{
				AddButton(175, yPos + 38, 4005, 4006, ++buttonNum, GumpButtonType.Reply, pageNum); //goto button - odd ID
				AddButton(279, yPos + 38, 4011, 4012, ++buttonNum, GumpButtonType.Reply, pageNum); //copy vendor - even ID
				AddLabel(86, yPos, 1849, WorldRewardVendors.Vendors[i].GetName());
				AddImageTiled(86, yPos + 93, 359, 2, 96);

				if (WorldRewardVendors.Vendors[i].GetMobile() != null)
				{
					AddItem(110, yPos + 27, 8461); // person
				}
				else
				{
					AddItem(
						110, yPos + 27, WorldRewardVendors.Vendors[i].GetItem().ItemID, WorldRewardVendors.Vendors[i].GetItem().Hue);
					//stone ID
				}

				AddLabel(213, yPos + 43, 2209, @"Go To");
				AddLabel(318, yPos + 43, 2204, @"Copy Vendor");

				yPos += 99;

				//add new page every four entries
				if ((i + 1) % 4 == 0)
				{
					AddButton(417, 531, 9903, 248, -1, GumpButtonType.Page, (pageNum + 1));

					AddPage(++pageNum);

					AddButton(92, 532, 9909, 248, -1, GumpButtonType.Page, (pageNum - 1));

					//reset to top of page
					yPos = 101;
				}
			}
		}
	}

	public class InfoGump : Gump
	{
		public InfoGump()
			: base(0, 0)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			AddImageTiled(0, 0, 452, 516, 2702);
			AddHtml(21,
					78,
					407,
					105,
					@"Consume Item:  Vendor counts the number of the chosen item present in player's backpack.  

        Property(Item/Player):  Vendor takes the total of the given number based property (example: PlayerPoints, Charges)
        (Items): Adds up from every item present.
        (Player): Simply takes from what is present.
        ",
					true,
					true);

			AddLabel(23, 52, 2000, @"Choosing How Player Pays:");
			AddHtml(
				21, 242, 407, 105, @"You can create your own rewards in game, all properties will be copied over.  ", true, true);
			AddLabel(24, 216, 2000, @"Adding Items");
			AddLabel(239, 479, 37, @"Author: krazeykow1102");
			AddItem(391, 467, 8451);
			AddLabel(24, 368, 2000, @"How Displays Work");
			AddHtml(
				24,
				394,
				410,
				78,
				@"Displays allow you to create your own interactive Reward room.  The display is a simple mirror to the item on the vendor.  Payment and cost will reflect what is on the vendor.  Vendor must stay in tact for display to function",
				true,
				true);
		}
	}

	public class ControlPanelGump : Gump
	{
		private readonly IRewardVendor m_Vendor;

		public ControlPanelGump(Mobile m, IRewardVendor vendor)
			: base(0, 0)
		{
			m_Vendor = vendor;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			//background
			AddImageTiled(4, 3, 215, 571, 2702);
			AddImageTiled(3, 0, 217, 3, 10001);
			AddImageTiled(4, 572, 216, 3, 10001);
			AddImageTiled(3, 1, 3, 571, 10004);
			AddImageTiled(217, 1, 3, 574, 10004);

			AddImage(40, 8, 5214);
			AddLabel(41, 7, 47, @"Control Panel");

			AddLabel(109, 62, 55, @"Manage Rewards");
			AddButton(72, 60, 4008, 4009, 1, GumpButtonType.Reply, 0); //PlayerView
			AddItem(31, 63, 5366); //scope
			AddImageTiled(19, 160, 132, 2, 96);

			AddLabel(79, 123, 1324, @"Add Item or Container");
			AddButton(52, 126, 2117, 2118, 2, GumpButtonType.Reply, 0); //AddItem
			AddItem(0, 113, 8192); //box  

			AddLabel(11, 232, 55, @"Target Payment Source:");
			AddButton(170, 232, 2117, 2118, 3, GumpButtonType.Reply, 0); //Payment
			AddLabel(12, 173, 55, @"Pay By: " + m_Vendor.Payment.PayName);
			AddItem(61, 197, m_Vendor.Payment.PayID, m_Vendor.Payment.CurrHue);

			AddLabel(12, 32, 55, @"Vendor: " + m_Vendor.GetName());
			AddButton(77, 533, 4026, 4027, 4, GumpButtonType.Reply, 0);
			AddLabel(78, 553, 104, @"Help");

			if (m_Vendor.GetMobile() != null)
			{
				AddButton(72, 91, 4011, 4012, 5, GumpButtonType.Reply, 0); //Displays
				AddLabel(109, 93, 55, @"Get All Displays");
			}
			else
			{
				AddLabel(12, 93, 55, @"Use Mobile Vendor for displays.");
			}

			AddLabel(51, 258, 43, @"XML Attachment");
			AddRadio(15, 258, 9720, 9724, false, 6);
			AddLabel(51, 303, 43, @"Consume Item");
			AddRadio(15, 300, 9720, 9724, false, 7);
			AddLabel(50, 349, 43, @"[props Name of Item/Player");
			AddRadio(15, 342, 9720, 9724, false, 8);
			AddImageTiled(12, 382, 156, 22, 5058);
			AddTextEntry(12, 382, 149, 20, 41, 0, @"[prop/XML Name Here");

			AddPage(1);

			CreateGumps(); //fill out entries
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (m_Vendor.IsRemoved())
			{
				m.SendMessage("This vendor has been deleted.");
				return;
			}

			if (m.AccessLevel < MobileRewardVendor.FullAccessStaffLevel)
			{
				LoggingCustom.Log(
					"RewardsVendor.txt", "ERROR: non-staff in " + GetType() + " gump!" + "\t" + m.Name + "\t" + m.Account);
				return;
			}

			switch (info.ButtonID)
			{
				case 0:
					{
						m.CloseGump(typeof(WorldVendorsGump));

						//close
						break;
					}
				case 1: //PlayerView
					{
						m.CloseGump(typeof(WorldVendorsGump));

						MenuUploader.Display(m_Vendor.Menu, m, m_Vendor, true);
						break;
					}
				case 2: //AddItem
					{
						m.CloseGump(typeof(WorldVendorsGump));

						m.Target = new AddItemTarget(m_Vendor);
						break;
					}
				case 3: //Payment
					{
						try
						{
							if (info.IsSwitched(6))
							{
								m.SendMessage("Target the player or item you wish to get the XMLAttribute from.");
								TextRelay entry0 = info.GetTextEntry(0);
								string propName = (entry0 == null ? "" : entry0.Text.Trim());

								m.Target = new PaymentTarget(m_Vendor, propName, true);
							}
							else if (info.IsSwitched(7)) // Consume by item amount
							{
								m.SendMessage("Warning: payment will be taken as the unmodified created item.");
								m.Target = new PaymentTarget(m_Vendor);
							}
							else if (info.IsSwitched(8)) //Consume by item's property
							{
								m.SendMessage("Target yourself or an item.");
								TextRelay entry0 = info.GetTextEntry(0);
								string propName = (entry0 == null ? "" : entry0.Text.Trim());

								m.Target = new PaymentTarget(m_Vendor, propName, false);
							}
							else
							{
								MenuUploader.Display(m_Vendor.Menu, m, m_Vendor, false);
								m.SendMessage("Please select consume or property.");
							}
						}

						catch
						{ }

						break;
					}
				case 4:
					{
						MenuUploader.Display(m_Vendor.Menu, m, m_Vendor, false);
						m.SendGump(new InfoGump());
						break;
					}
				case 5: //Displays
					{
						if (m.Backpack == null)
						{
							return;
						}

						var c = new MetalBox();
						c.Name = m_Vendor.GetName() + "'s Reward Displays";

						foreach (Reward r in m_Vendor.Rewards)
						{
							c.DropItem(new Exhibit(m_Vendor, r));
						}

						m.AddToBackpack(c);

						break;
					}

				default: //Set Menu - IDs are >= 7
					{
						try
						{
							Type menuType = MenuUploader.Menus[info.ButtonID - 7];

							if (m_Vendor.GetItem() != null && menuType.Equals(typeof(ClassicVendorGump)))
							{
								m.SendMessage("Only Mobile Reward Vendors can have classic menus.");
							}
							else
							{
								m_Vendor.Menu = menuType;
								m.SendMessage("Display changed to " + menuType.Name);
							}
						}
						catch
						{ }

						MenuUploader.Display(m_Vendor.Menu, m, m_Vendor, false);

						break;
					}
			}
		}

		public void CreateGumps()
		{
			int yPos = 433, pageNum = 1;

			for (int i = 0; i < MenuUploader.Menus.Count; ++i)
			{
				AddLabel(50, yPos, 100, MenuUploader.Menus[i].Name);
				AddButton(12, yPos, 4005, 4006, (i + 7), GumpButtonType.Reply, pageNum);

				yPos += 26;

				//add new page every four entries
				if ((i + 1) % 4 == 0)
				{
					AddButton(144, 542, 9702, 9703, -1, GumpButtonType.Page, (pageNum + 1));

					AddPage(++pageNum);

					AddButton(10, 542, 9706, 9707, -1, GumpButtonType.Page, (pageNum - 1));

					//reset to top of page
					yPos = 433;
				}
			}
		}
	}

	public static class MenuUploader
	{
		private static readonly List<Type> m_Menus = new List<Type>();

		public static List<Type> Menus { get { return m_Menus; } }

		public static void RegisterMenu<G>(G menu) where G : Gump, IRewardVendorGump
		{
			m_Menus.Add(menu.GetType());
		}

		public static void Display(IRewardVendorGump menu, Mobile m, IRewardVendor vendor)
		{
			m.CloseGump(typeof(IRewardVendorGump));

			//create 'canvas'
			menu.CreateBackground();

			//create Reward displays
			foreach (Reward r in vendor.Rewards)
			{
				//refresh restock  
				if (r.RewardInfo == null || r.Try_Restock() == 0 && !(menu is ManageItemsGump))
				{
				    vendor.Rewards.Remove(r);
					continue;
				}

				menu.AddEntry(r);
			}

			//send finished product
			menu.Send(m);
		}

		public static void Display(Type menu, Mobile m, IRewardVendor vendor, bool playerView)
			//Note: Playerview is only relevant for Staff
		{
			try
			{
				if (m.AccessLevel >= MobileRewardVendor.FullAccessStaffLevel)
				{
					if (playerView)
					{
						Display(new ManageItemsGump(vendor, m), m, vendor);
					}
					else //control panel
					{
						m.CloseGump(typeof(WorldVendorsGump));
						m.CloseGump(typeof(ControlPanelGump));

						m.SendGump(new WorldVendorsGump(vendor));
						m.SendGump(new ControlPanelGump(m, vendor));
					}
				}
				else //player
				{
					Display((IRewardVendorGump)Activator.CreateInstance(menu, new object[] {vendor, m}), m, vendor);
					//create fresh instance
				}
			}
			catch
			{
				if (m.AccessLevel >= MobileRewardVendor.FullAccessStaffLevel)
				{
					m.SendMessage("The current display is invalid, try setting gump back to default, JewlRewardGump.");
				}
				else
				{
					m.SendMessage("The current display is invalid, please notify the staff.");
				}
			}
		}
	}

	public class ManageItemsGump : JewlRewardGump
	{
		private int m_ButtonNum;

		public ManageItemsGump(IRewardVendor vendor, Mobile m)
			: base(vendor, m)
		{
			m_ButtonNum = 0;
		}

		public override void AddEntryControl(Reward r)
		{
			ImageTileButtonInfo b = new ItemTileButtonInfo(r.RewardInfo);

			//begin time entries
			if (r.Restock != null)
			{
				if (r.Restock.RestockRate.TotalMinutes == 0)
				{
					AddLabel(324, (PosY + 22), 5, "Limited");
				}
				else
				{
					AddLabel(
						324,
						(PosY + 22),
						5,
						(r.Restock.RestockRate.Hours).ToString() + " hours " + (r.Restock.RestockRate.Minutes).ToString() + " min");
				}

				if (r.Restock.Maximum > 0)
				{
					AddLabel(
						324,
						(PosY + 68),
						4,
						"In Stock: " + r.Restock.Count.ToString() + " / " + r.Restock.Maximum.ToString() + ", Purchased: " +
						r.BuyCount.ToString());
				}
				else
				{
					AddLabel(324, (PosY + 68), 4, "In Stock: " + r.Restock.Count.ToString() + ", Purchased: " + r.BuyCount.ToString());
				}
			}
			else
			{
				AddLabel(324, (PosY + 68), 4, "Purchased: " + r.BuyCount.ToString());
			}
			//end 

			//create entry
			AddLabel(227, PosY, 2123, r.Title);
			AddLabel(324, (PosY + 45), 2115, "Cost: " + r.Cost);
			//odd numbers
			AddImageTiledButton(
				227,
				(PosY + 26),
				2328,
				2329,
				++m_ButtonNum,
				GumpButtonType.Reply,
				PageNum,
				b.ItemID,
				b.Hue,
				15,
				10,
				r.Display.Header);
			AddImageTiled(215, (PosY + 95), 359, 2, 96);
			//odd numbers (same as above)
			AddButton(470, (PosY + 45), 4011, 4012, m_ButtonNum, GumpButtonType.Reply, PageNum);
			AddLabel(508, (PosY + 47), 1882, @"Options");
			//even numbers
			AddButton(470, (PosY + 19), 4020, 4021, ++m_ButtonNum, GumpButtonType.Reply, PageNum);
			AddLabel(509, (PosY + 21), 1882, @"Delete");
			PosY += 102;

			//add new page every four entries
			if (EntryNum % 4 == 0)
			{
				AddButton(546, 562, 9903, 9904, -1, GumpButtonType.Page, PageNum + 1);

				AddPage(++PageNum);

				AddButton(221, 563, 9909, 9910, -1, GumpButtonType.Page, PageNum - 1);

				//reset to top of page
				PosY = 130;
			}

			EntryNum++;
		}

		public static void DeleteReward_Callback(Mobile from, bool okay, object state)
		{
			if (from.AccessLevel < MobileRewardVendor.FullAccessStaffLevel)
			{
				return;
			}

			if (okay)
			{
				//indexes -> (Vendor(0), Reward(1))
				var store = (Storage)state;

				var vendor = (IRewardVendor)store[0];
				var Reward = (Reward)store[1];

				try
				{
					vendor.RemoveReward(Reward);
				}
				catch
				{
					from.SendMessage("An error ocurred in the removal of this item.");
				}

				MenuUploader.Display(vendor.Menu, from, vendor, true);
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (m.AccessLevel < MobileRewardVendor.FullAccessStaffLevel)
			{
				LoggingCustom.Log(
					"RewardsVendor.txt", "ERROR: non-staff in " + GetType() + " gump!" + "\t" + m.Name + "\t" + m.Account);
				return;
			}

			if (info.ButtonID == 0)
			{
				MenuUploader.Display(Vendor.Menu, m, Vendor, false);
				return;
			}

			if (info.ButtonID % 2 == 0) //even
			{
				try
				{
					Reward r = Vendor.Rewards[(info.ButtonID / 2) - 1];

					//params -> (vendor, Reward)
					var store = new Storage(Vendor, r);

					m.SendGump(
						new WarningGump(
							1060635,
							30720,
							"Warning: Are you sure you want to remove " + r.Title,
							0xFFC000,
							420,
							400,
							DeleteReward_Callback,
							store));
				}
				catch
				{
					m.SendMessage("This item could not be found.");
				}
			}
			else //assert - odd
			{
				try
				{
					MenuUploader.Display(Vendor.Menu, m, Vendor, true);
					m.SendGump(new ViewItemGump(m, Vendor, Vendor.Rewards[((info.ButtonID + 1) / 2) - 1], true));
				}
				catch
				{
					m.SendMessage("Vendor could not be found");
				}
			}
		}
	}

	public class XMLIntPropertyGump : Gump
	{
		private readonly Type m_XML_Type;
		private readonly object m_Target;

		private readonly IRewardVendor m_Vendor;
		private readonly List<PropertyInfo> m_Props;

		private int m_PageNum;
		private int m_PosY;
		private int m_EntryNum;

		public XMLIntPropertyGump(Type xmlType, object target, IRewardVendor vendor, List<PropertyInfo> props)
			: base(0, 0)
		{
			m_XML_Type = xmlType;
			m_Target = target;
			m_Vendor = vendor;
			m_Props = props;

			m_PageNum = 1;
			m_PosY = 134;
			m_EntryNum = 1;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			AddImageTiled(284, 90, 168, 384, 2702);
			AddLabel(315, 99, 49, xmlType.Name);
			AddImageTiled(292, 123, 151, 3, 9151);

			AddPage(1);

			FillEntries();
		}

		public void FillEntries()
		{
			foreach (PropertyInfo pi in m_Props)
			{
				AddButton(295, m_PosY, 5601, 5605, m_EntryNum, GumpButtonType.Reply, 0);
				AddLabel(319, m_PosY, 43, pi.Name);

				m_PosY += 26;

				if (m_EntryNum % 10 == 0)
				{
					AddButton(423, 444, 9903, 9904, -1, GumpButtonType.Page, m_PageNum + 1);

					AddPage(++m_PageNum);

					AddButton(292, 444, 9909, 9910, -1, GumpButtonType.Page, m_PageNum - 1);

					//reset to top of page
					m_PosY = 134;
				}
				m_EntryNum++;
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (m.AccessLevel < MobileRewardVendor.FullAccessStaffLevel)
			{
				LoggingCustom.Log(
					"RewardsVendor.txt", "ERROR: non-staff in " + GetType() + " gump!" + "\t" + m.Name + "\t" + m.Account);
				return;
			}

			if (info.ButtonID == 0)
			{
				return;
			}

			m_Vendor.Payment = new Currency(m_Target, m_XML_Type, m_Props[info.ButtonID - 1]);
			m.SendMessage(
				"{0}'s payment method set to {1}.{2}", m_Vendor.GetName(), m_XML_Type.Name, m_Props[info.ButtonID - 1].Name);
		}
	}

	public class JewlRewardGump : Gump, IRewardVendorGump
	{
		private int m_PageNum;
		private int m_EntryNum;
		private int m_PosY;
		private IRewardVendor m_Vendor;
		private int m_CurrencyAmnt;

		public static void Initialize()
		{
			MenuUploader.RegisterMenu(new JewlRewardGump());
		}

		public JewlRewardGump()
			: base(0, 0)
		{
			m_PageNum = 1;
			m_EntryNum = 1;
			m_PosY = 130;
			m_Vendor = null;
			Mobile = null;
			m_CurrencyAmnt = 0;
		}

		public JewlRewardGump(IRewardVendor vendor, Mobile m)
			: this()
		{
			m_Vendor = vendor;
			Mobile = m;
			m_CurrencyAmnt = vendor.Payment.Value(m);
		}

		//Properties
		public int PageNum { get { return m_PageNum; } set { m_PageNum = value; } }
		public int EntryNum { get { return m_EntryNum; } set { m_EntryNum = value; } }
		public int PosY { get { return m_PosY; } set { m_PosY = value; } }
		public IRewardVendor Vendor { get { return m_Vendor; } set { m_Vendor = value; } }
		public Mobile Mobile { get; set; }
		public int CurrencyAmnt { get { return m_CurrencyAmnt; } set { m_CurrencyAmnt = value; } }

		//Interface Explicit Implementation 
		void IRewardVendorGump.Send(Mobile m)
		{
			SendControl(m);
		}

		void IRewardVendorGump.CreateBackground()
		{
			CreateBackgroundControl();
		}

		void IRewardVendorGump.AddEntry(Reward r)
		{
			AddEntryControl(r);
		}

		public virtual void SendControl(Mobile m)
		{
			m.SendGump(this);
		}

		public virtual void CreateBackgroundControl()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			AddImageTiled(179, 34, 430, 556, 2702);
			AddImageTiled(222, 109, 387, 9, 10101);
			AddImage(162, 89, 10420);
			AddImageTiled(176, 33, 3, 556, 10004);
			AddImageTiled(129, 89, 82, 414, 10440);
			AddImageTiled(176, 589, 436, 3, 10001);
			AddImageTiled(176, 31, 435, 3, 10001);
			AddImageTiled(609, 33, 3, 556, 10004);
			AddLabel(348, 52, 2124, m_Vendor.GetName());
			AddImage(210, 52, 9818);
			AddImage(534, 52, 9818);
			AddLabel(404, 119, 2125, @"You have:");
			AddItem(348, 126, m_Vendor.Payment.PayID, m_Vendor.Payment.CurrHue);
			AddLabel(404, 137, 2125, (m_CurrencyAmnt.ToString() + " " + m_Vendor.Payment.PayName));

			AddPage(1);
		}

		public virtual void AddEntryControl(Reward r)
		{
			ImageTileButtonInfo b = new ItemTileButtonInfo(r.RewardInfo);

			//create entry

			//begin time entries
			if (r.Restock != null)
			{
				if (r.Restock.RestockRate.TotalMinutes == 0)
				{
					AddLabel(324, (PosY + 22), 5, "Limited");
				}

				if (r.Restock.Maximum > 0)
				{
					AddLabel(324, (PosY + 68), 4, "In Stock: " + r.Restock.Count.ToString());
				}
				else
				{
					AddLabel(324, (PosY + 68), 4, "In Stock: " + r.Restock.Count.ToString());
				}
			}
			//end 

			AddLabel(227, m_PosY, 2123, r.Title);
			AddLabel(324, (m_PosY + 45), 2115, "Cost: " + r.Cost);
			AddImageTiledButton(
				227,
				(m_PosY + 26),
				2328,
				2329,
				(m_EntryNum),
				GumpButtonType.Reply,
				m_PageNum,
				b.ItemID,
				b.Hue,
				15,
				10,
				b.LocalizedTooltip);
			AddImageTiled(215, (m_PosY + 95), 359, 2, 96);
			AddButton(470, (PosY + 45), 4011, 4012, (m_EntryNum), GumpButtonType.Reply, m_PageNum);
			AddLabel(508, (PosY + 47), 745, @"View Item");
			m_PosY += 102;

			//add new page every four entries
			if (m_EntryNum % 4 == 0)
			{
				AddButton(546, 562, 9903, 9904, -1, GumpButtonType.Page, m_PageNum + 1);

				AddPage(++m_PageNum);

				AddButton(221, 563, 9909, 9910, -1, GumpButtonType.Page, m_PageNum - 1);

				//reset to top of page
				m_PosY = 130;
			}

			m_EntryNum++;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID == 0)
			{
				return;
			}

			MenuUploader.Display(m_Vendor.Menu, m, m_Vendor, true);

			try
			{
				m.SendGump(new ViewItemGump(m, m_Vendor, m_Vendor.Rewards[info.ButtonID - 1], true));
			}
			catch
			{
				m.SendMessage("Vendor could not be found");
			}
		}
	}

	[TypeAlias("DisplayBox")]
	public class DisplayBox : LargeCrate, IEnumerable
	{
		private Dictionary<Reward, MetalBox> m_Boxes;

		public override bool IsPublicContainer { get { return true; } }

		public MetalBox this[Reward r]
		{
			get
			{
				MetalBox box = null; //not found

				m_Boxes.TryGetValue(r, out box);

				return box;
			}
		}

		[Constructable]
		public DisplayBox()
		{
			Name = "Display Box [DO NOT REMOVE]";
			LiftOverride = true;
			m_Boxes = new Dictionary<Reward, MetalBox>();
		}

		public DisplayBox(RewardCollection rc)
			: this()
		{
			CreateEntries(rc);
		}

		public bool Contains(Reward r)
		{
			return m_Boxes.ContainsKey(r);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_Boxes.GetEnumerator();
		}

		public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
		{
			reject = 0;
			return false;
		}

		public void DisplayTo(Mobile m, Reward r)
		{
			string invalid = "This item can no longer be displayed.";
			bool staff = m.AccessLevel >= MobileRewardVendor.FullAccessStaffLevel;

			try
			{
				var containers = new[] {(Container)Parent, this}; //cast exception if parent isn't container

				foreach (Container c in containers)
				{
					if (m.NetState != null && m.NetState.ContainerGridLines)
					{
						m.Send(new ContainerContent6017(m, c));
					}
					else
					{
						m.Send(new ContainerContent(m, c));
					}
				}

				m_Boxes[r].DisplayTo(m); //throws null ref if not found

				m.SendMessage("A container has been opened for you to view the item.");
			}
			catch (NullReferenceException)
			{
				if (staff)
				{
					m.SendMessage("Display error - please insure Reward is still on this vendor.");
				}
				else
				{
					m.SendMessage(invalid);
				}
			}
			catch (InvalidCastException)
			{
				if (staff)
				{
					m.SendMessage("Display error - the current vendor does not have a DisplayBox in backpack.");
				}
				else
				{
					m.SendMessage(invalid);
				}
			}
			catch (Exception)
			{
				if (staff)
				{
					m.SendMessage("Unknown error occured with display.");
				}
				else
				{
					m.SendMessage(invalid);
				}
			}
		}

		//Warning: must catch exception in methods
		public void CreateEntries(RewardCollection rc)
		{
			foreach (Reward r in rc)
			{
				AddDisplay(r, r.RewardInfo);
			}
		}

		public void AddDisplay(Reward key, Item i)
		{
			if (m_Boxes.ContainsKey(key))
			{
				throw new Exception("Key already in use.");
			}

			var container = new MetalBox();

			container.DropItem(i);

			DropItem(container);

			m_Boxes.Add(key, container);
		}

		public void AddDisplay(Reward r)
		{
			AddDisplay(r, r.RewardInfo);
		}

		public void RemoveDisplay(Reward r)
		{
			(m_Boxes[r]).Delete(); //remove from displaybox
			m_Boxes.Remove(r); //remove from hashtable      
		}

		//End Warning

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			return false; //do nothing
		}

		public DisplayBox(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			int count = 0;
			try
			{
				count = m_Boxes.Count;
			}
			catch
			{
				m_Boxes = new Dictionary<Reward, MetalBox>();
				m_Boxes.Add(new Reward(), new MetalBox());
				count = m_Boxes.Count;
			}
			writer.Write(count);

			foreach (KeyValuePair<Reward, MetalBox> kvp in m_Boxes)
			{
				writer.Write(kvp.Key);
				writer.WriteItem(kvp.Value);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			int count = reader.ReadInt();

			m_Boxes = new Dictionary<Reward, MetalBox>();
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					var r = (Reward)reader.ReadItem();
					var box = (MetalBox)reader.ReadItem();

					m_Boxes.Add(r, box);
				}
			}
		}
	}

	public class LoadGumpEntry : ContextMenuEntry
	{
		private readonly IRewardVendor m_Vendor;
		private readonly Mobile m_Mobile;

		public LoadGumpEntry(Mobile from, IRewardVendor vendor)
			: base(6103, 8)
		{
			m_Vendor = vendor;
			m_Mobile = from;
			Enabled = true;
		}

		public override void OnClick()
		{
			try
			{
				MenuUploader.Display(
					(IRewardVendorGump)Activator.CreateInstance(m_Vendor.Menu, new object[] {m_Vendor, m_Mobile}), m_Mobile, m_Vendor);
				//create fresh instance
			}
			catch
			{
				m_Mobile.SendMessage("The current display is invalid.");
			}
		}
	}

	public class ClassicVendorGump : Gump, IRewardVendorGump
	{
		private readonly IRewardVendor m_Vendor;
		private readonly BaseVendor m_BVendor;

		private readonly Mobile m_Mobile;

		private readonly List<ObjectPropertyList> m_Opls;
		private readonly List<BuyItemState> m_List;

		public List<ObjectPropertyList> ObjPropLists { get { return m_Opls; } }
		public List<BuyItemState> RewardList { get { return m_List; } }

		public static void Initialize()
		{
			MenuUploader.RegisterMenu(new ClassicVendorGump());
		}

		public ClassicVendorGump()
			: base(0, 0)
		{
			m_Vendor = null;
			m_Mobile = null;
			m_BVendor = null;
			m_Opls = null;
			m_List = null;
		}

		public ClassicVendorGump(IRewardVendor vendor, Mobile m)
			: this()
		{
			m_Vendor = vendor;
			m_Mobile = m;
			m_BVendor = Parent(vendor);
			m_Opls = new List<ObjectPropertyList>();
			m_List = new List<BuyItemState>();
		}

		private BaseVendor Parent(IRewardVendor vendor)
		{
			BaseVendor validVendor = null;

			if (vendor != null) //only available on mobiles inherited from BaseVendor
			{
				validVendor = vendor as BaseVendor;
			}

			return validVendor;
		}

		void IRewardVendorGump.Send(Mobile m)
		{
			if (!m_Vendor.Payment.PaymentType.Equals(typeof(Gold))) //need custom payment display
			{
				m.SendGump(this);
			}

			if (m_BVendor != null)
			{
				if (m_List.Count > 0)
				{
					m_List.Sort(new BuyItemStateComparer());

					m_BVendor.SendPacksTo(m);

					if (m.NetState == null)
					{
						return;
					}

					if (m.NetState.ContainerGridLines)
					{
						m.Send(new VendorBuyContent6017(m_List));
					}
					else
					{
						m.Send(new VendorBuyContent(m_List));
					}
					m.Send(new VendorBuyList(m_BVendor, m_List));
					if (m.NetState.HighSeas)
					{
						m.Send(new DisplayBuyListHS(m_BVendor));
					}
					else
					{
						m.Send(new DisplayBuyList(m_BVendor));
					}
					// m.Send(new DisplayBuyList(m_BVendor));

					m.Send(new MobileStatusExtended(m)); //make sure their gold amount is sent

					if (m_Opls != null)
					{
						for (int i = 0; i < m_Opls.Count; ++i)
						{
							m.Send(m_Opls[i]);
						}
					}

					m_BVendor.SayTo(m, 500186); // Greetings.  Have a look around.
				}
			}
		}

		void IRewardVendorGump.AddEntry(Reward r)
		{
			m_List.Add(
				new BuyItemState(
					r.Title,
					m_BVendor.BuyPack.Serial,
					r.Serial,
					r.Cost,
					(r.Restock == null ? 20 : r.Restock.Count),
					r.RewardInfo.ItemID,
					r.RewardInfo.Hue));

			m_Opls.Add(r.Display);
		}

		void IRewardVendorGump.CreateBackground()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImage(36, 105, 2162);
			AddItem(111, 217, m_Vendor.Payment.PayID, m_Vendor.Payment.CurrHue);
			AddLabel(110, 172, 2122, @"Pay By:  " + m_Vendor.Payment.PayName);
			AddLabel(110, 299, 2120, @"You have: " + m_Vendor.Payment.Value(m_Mobile));
			AddImage(71, 302, 57);
			AddImage(71, 174, 57);
		}
	}

	[TypeAlias("MobileRewardVendor")]
	public class MobileRewardVendor : Banker, IRewardVendor
	{
		public const AccessLevel FullAccessStaffLevel = AccessLevel.Administrator;

		private Currency m_Currency;
		private RewardCollection m_Rewards;
		private Type m_Menu;
		private DisplayBox m_Box;

		private bool m_IsBanker;

		[CommandProperty(FullAccessStaffLevel)]
		public bool IsBanker { get { return m_IsBanker; } set { m_IsBanker = value; } }

		Currency IRewardVendor.Payment { get { return m_Currency; } set { m_Currency = value; } }
		RewardCollection IRewardVendor.Rewards { get { return m_Rewards; } set { m_Rewards = value; } }
		Type IRewardVendor.Menu { get { return m_Menu; } set { m_Menu = value; } }
		DisplayBox IRewardVendor.Display { get { return m_Box; } }

		[Constructable]
		public MobileRewardVendor()
		{
			m_IsBanker = true;

			//default is Gold
			m_Currency = new Currency();
			m_Rewards = new RewardCollection();
			m_Menu = typeof(ClassicVendorGump);
			m_Box = new DisplayBox();

			//add to world collection
			WorldRewardVendors.RegisterVendor(this);
			//must be brought to world for client to view
			AddToBackpack(m_Box);

			Title = "";
		}

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (from.Alive)
			{
				list.Add(new LoadGumpEntry(from, this));

				if (m_IsBanker && this is Banker)
				{
					list.Add(new OpenBankEntry(this));
				}
			}
		}

		Mobile IRewardVendor.GetMobile()
		{
			return this;
		}

		Item IRewardVendor.GetItem()
		{
			return null;
		}

		bool IRewardVendor.IsRemoved()
		{
			return Deleted;
		}

		Container IRewardVendor.GetContainer()
		{
			return Backpack;
		}

		string IRewardVendor.GetName()
		{
			return Name;
		}

		Map IRewardVendor.GetMap()
		{
			return Map;
		}

		Point3D IRewardVendor.GetLocation()
		{
			return Location;
		}

		//begin bank override
		public override bool HandlesOnSpeech(Mobile from)
		{
			if (!m_IsBanker)
			{
				return false;
			}

			base.HandlesOnSpeech(from);

			return true;
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (XmlScript.HasTrigger(this, TriggerName.onSpeech) &&
				UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
			{
				return;
			}
			if (m_IsBanker)
			{
				base.OnSpeech(e);
			}
		}

		//end

		//Warning: Following methods will throw Exceptions
		void IRewardVendor.AddReward(Reward r)
		{
			m_Box.AddDisplay(r);
			m_Rewards.Add(r);
		}

		void IRewardVendor.RemoveReward(Reward r)
		{
			m_Rewards.Remove(r);
			m_Box.RemoveDisplay(r);
		}

		void IRewardVendor.CopyVendor(IRewardVendor vendor)
		{
			Item currBox = Backpack.FindItemByType(typeof(DisplayBox));
			currBox.Delete();

			m_Currency = vendor.Payment;
			m_Menu = vendor.Menu;

			m_Rewards = (RewardCollection)((vendor.Rewards as ICloneable).Clone());
			m_Box = new DisplayBox(m_Rewards);

			AddToBackpack(m_Box);
		}

		//End Warning
		public override void OnDelete()
		{
			WorldRewardVendors.RemoveVendor(this);
			base.OnDelete();
		}

		//response to ClassicVendorGump - (must be inside BaseVendor)
		public override bool OnBuyItems(Mobile buyer, List<BuyItemResponse> list)
		{
			int totalCost = 0;

			var purchases = new Dictionary<Reward, int>();

			Container bank = buyer.BankBox;
			Container pack = buyer.Backpack;

			if (pack == null || bank == null)
			{
				return false;
			}

			foreach (BuyItemResponse buy in list)
			{
				Serial ser = buy.Serial;
				int amount = buy.Amount;

				if (ser.IsItem)
				{
					Item item = World.FindItem(ser);

					if (item == null)
					{
						continue;
					}

					var r = item as Reward;

					if (r == null || !r.InStock(amount))
					{
						continue;
					}

					totalCost += (r.Cost * amount);

					purchases.Add(r, amount);
				}
			}

			if (purchases.Count == 0)
			{
				SayTo(buyer, 500190); // Thou hast bought nothing! 
				return false;
			}

			var sb = new StringBuilder();
			sb.Append(
				DateTime.Now + "\t" + buyer + "\t" + buyer.Account + "\t" + m_Currency.PaymentType + "\tbefore: " +
				Currency.Value(buyer, m_Currency));
			if (!m_Currency.Purchase(buyer, totalCost)) //cannot afford
			{
				SayTo(buyer, 500192); //Begging thy pardon, but thou casnt afford that.
				sb.Append("\tCan't afford\tCost: " + totalCost);
				LoggingCustom.Log("RewardsVendor.txt", sb.ToString());
				return false;
			}
			// they bought it
			sb.Append("\tafter: " + Currency.Value(buyer, m_Currency) + "\tCost: " + totalCost);

			foreach (KeyValuePair<Reward, int> kvp in purchases)
			{
				kvp.Key.RegisterBuy(kvp.Value);

				for (int index = 0; index < kvp.Value; index++)
				{
					Item i = kvp.Key.RewardCopy;
					sb.Append("\t" + i.GetType());
					if (!buyer.PlaceInBackpack(i))
					{
						bank.DropItem(i);
						buyer.SendMessage("You are overweight, the Reward was added to your bank");
					}
				}

				buyer.SendMessage("You bought {0} {1}", kvp.Value, kvp.Key.Title);
			}
			LoggingCustom.Log("RewardsVendor.txt", sb.ToString());
			buyer.PlaySound(0x32);

			return true;
		}

		//respone to 'vendor buy'
		public override void VendorBuy(Mobile buyer)
		{
			MenuUploader.Display((IRewardVendorGump)Activator.CreateInstance(m_Menu, new object[] {this, buyer}), buyer, this);
			//create fresh instance
		}

		//end edit
		public override void OnDoubleClick(Mobile m)
		{
			if (InRange(m, 4) && InLOS(m))
			{
				MenuUploader.Display(m_Menu, m, this, false);
			}
		}

		public MobileRewardVendor(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write(m_IsBanker);

			int count = m_Rewards.Count;
			writer.Write(count);

			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					writer.WriteItem(m_Rewards[i]);
				}
			}

			writer.WriteItem(m_Currency);
			writer.Write(m_Menu.Name);
			writer.WriteItem(m_Box);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			WorldRewardVendors.RegisterVendor(this);

			int version = reader.ReadInt();

			m_Rewards = new RewardCollection();

			m_IsBanker = true; //for conversion

			switch (version)
			{
				case 1:
					{
						m_IsBanker = reader.ReadBool();

						goto case 0;
					}
				case 0:
					{
						int count = reader.ReadInt();
						if (count > 0)
						{
							for (int i = 0; i < count; i++)
							{
								var r = (Reward)reader.ReadItem();
								m_Rewards.Add(r);
							}
						}

						m_Currency = (Currency)reader.ReadItem();

						try
						{
							string name = reader.ReadString();
							m_Menu = ScriptCompiler.FindTypeByName(name);
						}
						catch
						{
							m_Menu = typeof(JewlRewardGump);
						}

						m_Box = (DisplayBox)reader.ReadItem();

						break;
					}
			}
		}
	}

	[TypeAlias("StoneRewardHolder")]
	public class StoneRewardHolder : MetalBox
	{
		private readonly StoneRewardVendor m_S_Vendor;

		public StoneRewardHolder(StoneRewardVendor sv)
		{
			m_S_Vendor = sv;
			Movable = false;
			Name = sv.Name + "'s Display Container [DO NOT DELETE]";
		}

		public override void OnItemLifted(Mobile m, Item item)
		{
			m.SendMessage("This item should not be moved away from the vendor.");
		}

		public void UpdateName()
		{
			if (m_S_Vendor == null)
			{
				return;
			}

			Name = m_S_Vendor.Name + "'s Display Container [DO NOT DELETE]";
		}

		public StoneRewardHolder(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	[TypeAlias("StoneRewardVendor")]
	public class StoneRewardVendor : Item, IRewardVendor
	{
		private Currency m_Currency;
		private RewardCollection m_Rewards;
		private Type m_Menu;
		private StoneRewardHolder m_Box_Holder;
		private DisplayBox m_Box;

		Currency IRewardVendor.Payment { get { return m_Currency; } set { m_Currency = value; } }
		RewardCollection IRewardVendor.Rewards { get { return m_Rewards; } set { m_Rewards = value; } }
		Type IRewardVendor.Menu { get { return m_Menu; } set { m_Menu = value; } }
		DisplayBox IRewardVendor.Display { get { return m_Box; } }

		[Constructable]
		public StoneRewardVendor()
		{
			m_Currency = new Currency();
			m_Rewards = new RewardCollection();
			m_Menu = typeof(JewlRewardGump);
			m_Box = new DisplayBox();

			ItemID = 0xEDC;
			Name = "Stone Vendor";

			WorldRewardVendors.RegisterVendor(this);

			m_Box_Holder = new StoneRewardHolder(this);
			m_Box_Holder.DropItem(m_Box);

			Movable = false;
		}

		public override void OnLocationChange(Point3D oldLocation)
		{
			base.OnLocationChange(oldLocation);

			if (m_Box_Holder != null)
			{
				m_Box_Holder.Location = Location;
				m_Box_Holder.Z = Z - 10;
			}
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			if (m_Box_Holder != null)
			{
				m_Box_Holder.Map = Map;
			}
		}

		Mobile IRewardVendor.GetMobile()
		{
			return null;
		}

		Item IRewardVendor.GetItem()
		{
			return this;
		}

		bool IRewardVendor.IsRemoved()
		{
			return Deleted;
		}

		Container IRewardVendor.GetContainer()
		{
			return m_Box_Holder;
		}

		string IRewardVendor.GetName()
		{
			return Name;
		}

		Map IRewardVendor.GetMap()
		{
			return Map;
		}

		Point3D IRewardVendor.GetLocation()
		{
			return Location;
		}

		void IRewardVendor.AddReward(Reward r)
		{
			m_Box.AddDisplay(r);
			m_Rewards.Add(r);
		}

		void IRewardVendor.RemoveReward(Reward r)
		{
			m_Rewards.Remove(r);
			m_Box.RemoveDisplay(r);
		}

		void IRewardVendor.CopyVendor(IRewardVendor vendor)
		{
			Item currBox = m_Box_Holder.FindItemByType(typeof(DisplayBox));
			currBox.Delete();

			m_Currency = vendor.Payment;
			m_Menu = vendor.Menu;

			m_Rewards = (RewardCollection)((vendor.Rewards as ICloneable).Clone());
			m_Box = new DisplayBox(m_Rewards);

			m_Box_Holder.DropItem(m_Box);
		}

		//End Warning
		public override void OnDelete()
		{
			if (m_Box_Holder != null)
			{
				m_Box_Holder.Delete();
			}

			WorldRewardVendors.RemoveVendor(this);
			base.OnDelete();
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (m_Box_Holder != null)
			{
				m_Box_Holder.MoveToWorld(Location, Map);
				m_Box_Holder.Z = Z - 10;
				m_Box_Holder.UpdateName();
			}
			MenuUploader.Display(m_Menu, m, this, false);
		}

		public StoneRewardVendor(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); //Version 0

			int count = m_Rewards.Count;
			writer.Write(count);

			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					writer.WriteItem(m_Rewards[i]);
				}
			}

			writer.WriteItem(m_Currency);
			writer.Write(m_Menu.Name);
			writer.WriteItem(m_Box);
			writer.WriteItem(m_Box_Holder);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			WorldRewardVendors.RegisterVendor(this);

			int version = reader.ReadInt();

			m_Rewards = new RewardCollection();

			int count = reader.ReadInt();
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					var r = (Reward)reader.ReadItem();
					m_Rewards.Add(r);
				}
			}

			m_Currency = (Currency)reader.ReadItem();

			try
			{
				string name = reader.ReadString();
				m_Menu = ScriptCompiler.FindTypeByName(name);
			}
			catch
			{
				m_Menu = typeof(JewlRewardGump);
			}

			m_Box = (DisplayBox)reader.ReadItem();
			m_Box_Holder = (StoneRewardHolder)reader.ReadItem();
		}
	}

	public interface IRewardVendor
	{
		//used to determine how player pays
		Currency Payment { get; set; }
		//to keep added items
		RewardCollection Rewards { get; set; }
		//to change gump
		Type Menu { get; set; }
		//to show player items
		DisplayBox Display { get; }
		//to return what holds 'DisplayBox' object
		Container GetContainer();
		//to return removal status
		bool IsRemoved();

		//serial /deserial
		Mobile GetMobile();
		Item GetItem();

		//used for 'world collection'
		string GetName();
		Point3D GetLocation();
		Map GetMap();

		//to manage RewardCollection
		void AddReward(Reward r);
		void RemoveReward(Reward r);

		//to create vendor clones
		void CopyVendor(IRewardVendor vendor);
	}

	public interface IRewardVendorGump
	{
		void Send(Mobile m);
		void AddEntry(Reward r);
		void CreateBackground();

		//add entry || background || 
	}

	public class PaymentTarget : Target
	{
		private readonly string m_Name;
		private readonly IRewardVendor m_Vendor;
		private readonly bool m_Is_XML;

		public PaymentTarget()
			: base(2, false, TargetFlags.None)
		{ }

		//by item
		public PaymentTarget(IRewardVendor vendor)
			: this()
		{
			m_Vendor = vendor;
			m_Name = null;
			m_Is_XML = false;
		}

		//by item property
		public PaymentTarget(IRewardVendor vendor, string name, bool isXML)
			: this()
		{
			m_Vendor = vendor;
			m_Name = name;
			m_Is_XML = isXML;
		}

		protected override void OnTarget(Mobile m, object targeted)
		{
			if (m_Is_XML && !String.IsNullOrEmpty(m_Name))
			{
				Type attachType = ScriptCompiler.FindTypeByName(m_Name);

				if (attachType != null && attachType.BaseType.Equals(typeof(XmlAttachment)))
				{
					var integerProps = new List<PropertyInfo>();

					foreach (PropertyInfo pi in attachType.GetProperties())
					{
						if ((pi.CanRead && pi.CanWrite) && pi.PropertyType.Equals(typeof(Int32)))
						{
							integerProps.Add(pi);
						}
					}

					if (integerProps.Count == 0)
					{
						m.SendMessage("{0} does not contain any integer based properties.", attachType.Name);
					}
					else
					{
						m.SendGump(new XMLIntPropertyGump(attachType, targeted, m_Vendor, integerProps));
					}
				}
				else
				{
					m.SendMessage("The XML Attachment, {0}, could not be found, the search IS case sensitive", m_Name);
				}
			}
			else if (targeted is Item)
			{
				//consume by item
				if (m_Name == null)
				{
					m_Vendor.Payment = new Currency((Item)targeted);
					m.SendMessage("{0} will now use {1} for payment.", m_Vendor.GetName(), targeted.GetType().Name);
				}
					//consume by item property
				else
				{
					SetByProperty(m, targeted);
				}
			}
			else if (targeted is PlayerMobile)
			{
				if (m_Name == null)
				{
					m.SendMessage("You can only use Property option for player.");
				}
				else
				{
					SetByProperty(m, null);
				}
			}
			else
			{
				m.SendMessage("Invalid, target must be Item or Player.");
			}

			MenuUploader.Display(m_Vendor.Menu, m, m_Vendor, false);
		}

		private void SetByProperty(Mobile m, object o)
		{
			string msg = "Please make sure you entered the name correctly, " + "it IS case sensitive.";
			try
			{
				if (o != null) //Item
				{
					PropertyInfo pi = o.GetType().GetProperty(m_Name);

					if (pi != null && pi.PropertyType.Equals(typeof(Int32)))
					{
						m_Vendor.Payment = new Currency((Item)o, pi);
						m.SendMessage(
							m_Vendor.GetName() + "will now use {1}.{2} for payment.", m_Vendor.GetName(), o.GetType().Name, pi.Name);
					}
					else
					{
						m.SendMessage(msg);
					}
				}
				else //Mobile
				{
					PropertyInfo pi = m.GetType().GetProperty(m_Name);

					if (pi != null && pi.PropertyType.Equals(typeof(Int32)))
					{
						m_Vendor.Payment = new Currency(m, pi);
						m.SendMessage("{0} will now use {1}.{2} for payment.", m_Vendor.GetName(), m.GetType().Name, pi.Name);
					}
					else
					{
						m.SendMessage(msg);
					}
				}
			}
			catch
			{
				m.SendMessage(msg);
			}
		}
	}

	public class AddItemTarget : Target
	{
		private readonly IRewardVendor m_Vendor;

		public AddItemTarget()
			: base(2, false, TargetFlags.None)
		{ }

		public AddItemTarget(IRewardVendor vendor)
			: this()
		{
			m_Vendor = vendor;
		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			var i = targeted as Item;

			if (m_Vendor == null || i == null)
			{
				return;
			}

			Item clone = ItemClone.Clone(i, true);

			if (clone != null) //copy did not fail
			{
				from.SendGump(new EditItemGump(m_Vendor, new Reward(clone), null, false, true));
			}
			else
			{
				from.SendMessage("This item cannot be copied.");
			}
		}
	}

	public class ItemClone
	{
		// InitialClone sets copied scripts Enabled to false, while the other Clone method sets it to true
		public static Item Clone(Item item, bool initialClone)
		{
			try
			{
				if (item is Container)
				{
					return CloneBag(
						((Container)item).FindItemsByType(typeof(Item), false),
						(Container)item,
						(Container)Activator.CreateInstance(item.GetType(), null),
						initialClone);
				}
				else
				{
					return Clone(item.GetType(), item, (Item)Activator.CreateInstance(item.GetType(), null), initialClone);
				}
			}
			catch
			{ }

			return null;
		}

		public static Container CloneBag(Item[] contents, Container old, Container copy, bool initialClone)
		{
			//emtpy new container
			foreach (Item i in copy.FindItemsByType(typeof(Item)))
			{
				i.Delete();
			}

			//'old' contents into 'new'
			foreach (Item i in contents)
			{
				copy.DropItem(Clone(i, initialClone));
			}

			copy.Name = old.Name; //requires manual set
			List<XmlScript> scripts = XmlAttach.GetScripts(old);
			if (scripts != null)
			{
				foreach (XmlScript script in scripts)
				{
					var copiedScript = new XmlScript(script.ScriptFile);
					copiedScript.Name = script.Name;
					copiedScript.Enabled = !initialClone; // don't let it be used at all if it is the first clone
					XmlAttach.AttachTo(copy, copiedScript);
				}
			}

			return copy;
		}

		public static Item Clone(Type type, Item old, Item copy, bool initialClone)
		{
			PropertyInfo[] props = type.GetProperties();
			foreach (PropertyInfo pi in props)
			{
				if (pi.CanRead && pi.CanWrite && ValidProperty(pi.Name))
				{
					try
					{
						if (pi.PropertyType.IsClass)
							// THIS IS TOO DANGEROUS TO BE ALLOWED!!! (it was changing Map.Internal's name to Felucca
						{
							continue;
							//CloneInnerClass(pi.PropertyType, pi.GetValue(old, null), pi.GetValue(copy, null)); //found object property
						}
						else
						{
							pi.SetValue(copy, pi.GetValue(old, null), null); //found value property
						}
					}
					catch
					{ }
				}
			}

			copy.Name = old.Name; //requires manual set
		    if (copy is TitleScroll && old is TitleScroll)
		        ((TitleScroll) copy).RawTitle = ((TitleScroll) old).RawTitle;

		    if (copy is TransmuteTalisman && old is TransmuteTalisman)
		    {
		        ((TransmuteTalisman) copy).TransmuteID = ((TransmuteTalisman) old).TransmuteID;
                ((TransmuteTalisman)copy).TransmuteHue = ((TransmuteTalisman)old).TransmuteHue;
                ((TransmuteTalisman)copy).TransmuteAnimation = ((TransmuteTalisman)old).TransmuteAnimation;
                ((TransmuteTalisman)copy).TransmuteName = ((TransmuteTalisman)old).TransmuteName;
		    }

		    if (copy is HueScroll && old is HueScroll)
                ((HueScroll)copy).RawTitleHue = ((HueScroll)old).RawTitleHue;

            if (copy is EtherealCharcoalDeed && old is EtherealCharcoalDeed)
                ((EtherealCharcoalDeed)copy).EtherealType = ((EtherealCharcoalDeed)old).EtherealType;

		    if (copy is EtherealDyeDeed && old is EtherealDyeDeed)
		    {
		        ((EtherealDyeDeed) copy).EtherealType = ((EtherealDyeDeed) old).EtherealType;
		    }

		    if (copy is StorageIncreaseDeed && old is StorageIncreaseDeed)
                ((StorageIncreaseDeed)copy).StorageIncrease = ((StorageIncreaseDeed)old).StorageIncrease;

            if (copy is VendorIncreaseDeed && old is VendorIncreaseDeed)
                ((VendorIncreaseDeed)copy).VendorIncrease = ((VendorIncreaseDeed)old).VendorIncrease;

		    if (copy is BoatDyeDeed && old is BoatDyeDeed)
		    {
		        ((BoatDyeDeed) copy).HueName = ((BoatDyeDeed) old).HueName;
                ((BoatDyeDeed)copy).DyeableHue = ((BoatDyeDeed)old).DyeableHue;
		    }


		    List<XmlScript> scripts = XmlAttach.GetScripts(old);
			if (scripts != null)
			{
				foreach (XmlScript script in scripts)
				{
					var copiedScript = new XmlScript(script.ScriptFile);
					copiedScript.Name = script.Name;
					copiedScript.Enabled = !initialClone; // don't let it be used at all if it is the first clone
					XmlAttach.AttachTo(copy, copiedScript);
				}
			}

			return copy;
		}

		public static void CloneInnerClass(Type type, object old, object copy)
		{
			// DON'T USE THIS--it's very dangerous!
			if (old == null) //not defined
			{
				return;
			}

			PropertyInfo[] props = type.GetProperties();
			foreach (PropertyInfo pi in props)
			{
				if (pi.CanRead && pi.CanWrite)
				{
					try
					{
						pi.SetValue(copy, pi.GetValue(old, null), null);
					}
					catch
					{ }
				}
			}
		}

		public static bool ValidProperty(string str)
		{
			return !(str.Equals("Parent") || str.Equals("TotalWeight") || str.Equals("TotalItems") || str.Equals("TotalGold"));
		}
	}
}