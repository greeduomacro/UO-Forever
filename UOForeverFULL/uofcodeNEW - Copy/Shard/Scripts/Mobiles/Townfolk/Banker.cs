#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.ContextMenus;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Network;
#endregion

namespace Server.Mobiles
{
	public class Banker : BaseVendor
	{
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		public override NpcGuild NpcGuild { get { return NpcGuild.MerchantsGuild; } }

		[Constructable]
		public Banker()
			: base("the banker")
		{ }

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBBanker());
		}

		public static ulong GetFullBalance(Mobile from, Type currencyType)
		{
			if (from == null)
			{
				return 0;
			}

			Item[] currency;
			BankCheck[] checks;
			ulong credit;

			return GetBalance(from, currencyType, out currency, out checks, out credit);
		}

		public static int GetBalance(Mobile from, Type currencyType)
		{
			if (from == null)
			{
				return 0;
			}

			Item[] currency;
			BankCheck[] checks;

			return GetBalance(from, currencyType, out currency, out checks);
		}

		public static ulong GetBalance(Mobile from, Type currencyType, out Item[] currency, out ulong credit)
		{
			ulong balance = 0;

			BankBox bank = from.FindBankNoCreate();
			
			if (bank != null)
			{
				Type cType = bank.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

				credit = cType == currencyType ? bank.Credit : 0;

				balance += credit;

				currency = bank.FindItemsByType(currencyType, true).ToArray();

				balance = currency.Aggregate(balance, (current, t) => current + (ulong)t.Amount);
			}
			else
			{
				currency = new Item[0];
				credit = 0;
			}

			return balance;
		}

		public static int GetBalance(Mobile from, Type currencyType, out Item[] currency, out BankCheck[] checks)
		{
			int balance = 0;

			BankBox bank = from.FindBankNoCreate();

			if (bank != null)
			{
				currency = bank.FindItemsByType(currencyType, true).ToArray();
				checks = bank.FindItemsByType<BankCheck>(true).Where(c => c.TypeOfCurrency == currencyType).ToArray();

				balance += currency.Sum(t => t.Amount);
				balance += checks.Sum(t => t.Worth);
			}
			else
			{
				currency = new Item[0];
				checks = new BankCheck[0];
			}

			return balance;
		}

		public static ulong GetBalance(Mobile from, Type currencyType, out Item[] currency, out BankCheck[] checks, out ulong credit)
		{
			ulong balance = 0;

			BankBox bank = from.FindBankNoCreate();

			if (bank != null)
			{
				Type cType = bank.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

				credit = cType == currencyType ? bank.Credit : 0;

				balance += credit;

				currency = bank.FindItemsByType(currencyType, true).ToArray();
				checks = bank.FindItemsByType<BankCheck>(true).Where(c => c.TypeOfCurrency == currencyType).ToArray();

				balance = currency.Aggregate(balance, (current, t) => current + (ulong)t.Amount);
				balance = checks.Aggregate(balance, (current, t) => current + (ulong)t.Worth);
			}
			else
			{
				currency = new Item[0];
				checks = new BankCheck[0];
				credit = 0;
			}

			return balance;
		}

		public static bool Withdraw(Mobile from, Type currencyType, int amount)
		{
			if (amount > 0)
			{
				Item[] currency;
				BankCheck[] checks;
				int balance = GetBalance(from, currencyType, out currency, out checks);

				if (balance >= amount)
				{
					WithdrawUpTo(from, amount, currency, checks);
					return true;
				}

				return false;
			}

			return true;
		}

		public static bool WithdrawCredit(Mobile from, Type currencyType, int amount)
		{
			if (amount > 0)
			{
				Item[] currency;
				ulong credit;

				ulong balance = GetBalance(from, currencyType, out currency, out credit);

				if (balance >= (ulong)amount)
				{
					WithdrawUpTo(from, amount, currency, new BankCheck[0], credit);
					return true;
				}

				return false;
			}

			return true;
		}

		public static bool WithdrawChecksCredit(Mobile from, Type currencyType, int amount)
		{
			if (amount > 0)
			{
				Item[] currency;
				BankCheck[] checks;
				ulong credit;

				ulong balance = GetBalance(from, currencyType, out currency, out checks, out credit);

				if (balance >= (ulong)amount)
				{
					WithdrawUpTo(from, amount, currency, checks, credit);
					return true;
				}

				return false;
			}

			return true;
		}

		public static void WithdrawUpTo(Mobile from, int amount, Item[] currency, BankCheck[] checks)
		{
			WithdrawUpTo(from, amount, currency, checks, 0);
		}

		public static void WithdrawUpTo(Mobile from, int amount, Item[] currency, BankCheck[] checks, ulong credit)
		{
			for (int i = 0; amount > 0 && i < currency.Length; ++i)
			{
				int consume = Math.Min(currency[i].Amount, amount);
				currency[i].Consume(consume);
				amount -= consume;
			}

			for (int i = 0; amount > 0 && i < checks.Length; ++i)
			{
				BankCheck check = checks[i];

				int consume = Math.Min(check.Worth, amount);
				check.ConsumeWorth(consume);
				amount -= consume;
			}

			BankBox bank = from.FindBankNoCreate();

			if (bank != null) // sanity check?
			{
				ulong cons = Math.Min((ulong)amount, credit);
				bank.Credit -= cons;
				amount -= (int)cons;
			}
		}

		public static bool WithdrawPackAndBank(Mobile from, Type currencyType, int amount)
		{
			ulong totalCurrency = 0;
			Item[] packCurrency;
			Item[] bankCurrency;
			BankCheck[] bankChecks;
			ulong credit;
			
			if (from.Backpack != null)
			{
				packCurrency = from.Backpack.FindItemsByType(currencyType, true).ToArray();

				totalCurrency = packCurrency.Aggregate(totalCurrency, (current, t) => current + (ulong)t.Amount);
			}
			else
			{
				packCurrency = new Item[0];
			}

			if (totalCurrency < (ulong)amount)
			{
				totalCurrency += GetBalance(from, currencyType, out bankCurrency, out bankChecks, out credit);
			}
			else
			{
				bankCurrency = new Item[0];
				bankChecks = new BankCheck[0];
				credit = 0;
			}

			if (amount > 0)
			{
				if (totalCurrency >= (ulong)amount)
				{
					for (int i = 0; amount > 0 && i < packCurrency.Length; ++i)
					{
						int consume = Math.Min(packCurrency[i].Amount, amount);
						packCurrency[i].Consume(consume);
						amount -= consume;
					}

					if (amount > 0)
					{
						WithdrawUpTo(from, amount, bankCurrency, bankChecks, credit);
					}

					return true;
				}

				return false;
			}

			return true;
		}

		public static bool Deposit(Mobile from, Type currencyType, int amount)
		{
			BankBox box = from.FindBankNoCreate();

			if (box != null)
			{
				var items = new List<Item>();

				while (amount > 0)
				{
					Item item;

					if (amount < 5000)
					{
						item = currencyType.CreateInstanceSafe<Item>();

						item.Stackable = true;
						item.Amount = amount;

						amount = 0;
					}
					else if (amount <= 1000000)
					{
						item = new BankCheck(amount);
						amount = 0;
					}
					else
					{
						item = new BankCheck(1000000);
						amount -= 1000000;
					}

				    BankCheck check = item as BankCheck;

				    if (currencyType.Equals(typeof(DonationCoin)) && item is BankCheck)
				    {
				        check.IsDonation = true;
				        check.Hue = 1153;
				    }
					if (box.TryDropItem(from, item, false))
					{
						items.Add(item);
					}
					else
					{
						item.Delete();

						foreach (Item curItem in items)
						{
							curItem.Delete();
						}

						return false;
					}
				}

				return true;
			}

			return false;
		}

		public static int DepositUpTo(Mobile from, Type currencyType, int amount)
		{
			BankBox box = from.FindBankNoCreate();

			if (box != null)
			{
				int amountLeft = amount;

				while (amountLeft > 0)
				{
					Item item;
					int amountGiven;

					if (amountLeft < 5000)
					{
						item = currencyType.CreateInstanceSafe<Item>();

						item.Stackable = true;
						item.Amount = amountLeft;

						amountGiven = amountLeft;
					}
					else if (amountLeft <= 1000000)
					{
						item = new BankCheck(amountLeft);
						amountGiven = amountLeft;
					}
					else
					{
						item = new BankCheck(1000000);
						amountGiven = 1000000;
					}

					if (box.TryDropItem(from, item, false))
					{
						amountLeft -= amountGiven;
					}
					else
					{
						item.Delete();
						break;
					}
				}

				return amount - amountLeft;
			}

			return 0;
		}

		public static void Deposit(Container cont, Type currencyType, int amount)
		{
			while (amount > 0)
			{
				Item item;

				if (amount < 5000)
				{
					item = currencyType.CreateInstanceSafe<Item>();

					item.Stackable = true;
					item.Amount = amount;

					amount = 0;
				}
				else if (amount <= 1000000)
				{
					item = new BankCheck(amount);
					amount = 0;
				}
				else
				{
					item = new BankCheck(1000000);
					amount -= 1000000;
				}

				cont.DropItem(item);
			}
		}

		public Banker(Serial serial)
			: base(serial)
		{ }

		public override bool HandlesOnSpeech(Mobile m)
		{
			return m.InRange(Location, 12) || base.HandlesOnSpeech(m);
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			Mobile m = e.Mobile;

			if (XmlScript.HasTrigger(this, TriggerName.onSpeech) &&
				UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
			{
				return;
			}

			if (!e.Handled && m.InRange(Location, 12))
			{
				string speech = e.Speech.Trim().ToLower();

				if (e.HasKeyword(0x00)) // *withdraw*
				{
					e.Handled = true;

					if (!CheckVendorAccess(m))
					{
						Say(500389); // I will not do business with a criminal!
					}
					else
					{
						string[] split = e.Speech.Split(' ');

						if (split.Length >= 2)
						{
							int amount;

							Container pack = m.Backpack;

							if (Int32.TryParse(split[1], out amount))
							{
								if (amount > 60000)
								{
									Say(500381); // Thou canst not withdraw so much at one time!
								}
								else if (amount > 0)
								{
									BankBox box = m.FindBankNoCreate();

									if (box == null || !WithdrawCredit(m, TypeOfCurrency, amount))
									{
										Say("Ah, art thou trying to fool me? Thou hast not so much {0}!", TypeOfCurrency.Name);
									}
									else
									{
										Item currency = TypeOfCurrency.CreateInstanceSafe<Item>();

										currency.Stackable = true;
										currency.Amount = amount;

										if (pack != null && !pack.Deleted && pack.TryDropItem(m, currency, false))
										{
											Say("Thou hast withdrawn {0} from thy account.", TypeOfCurrency.Name);
											//Say(1010005); // Thou hast withdrawn gold from thy account.
										}
										else
										{
											currency.Delete();

											box.Credit += (ulong)amount;

											Say(1048147); // Your backpack can't hold anything else.
										}
									}
								}
							}
						}
					}
				}
				else if (e.HasKeyword(0x01)) // *balance*
				{
					e.Handled = true;

					if (!CheckVendorAccess(m))
					{
						Say(500389); // I will not do business with a criminal!
					}
					else
					{
						BankBox box = m.FindBankNoCreate();

						if (box != null)
						{
							Say("Thy current bank balance is {0:#,0} {1}.", (ulong)GetBalance(m, TypeOfCurrency) + box.Credit, TypeOfCurrency.Name);
						}
						else
						{
							Say("Thy current bank balance is 0 {0}.", TypeOfCurrency.Name);
						}
					}
				}
				else if (e.HasKeyword(0x02)) // *bank*
				{
					e.Handled = true;

					if (!CheckVendorAccess(m))
					{
						Say(500378); // Thou art a criminal and cannot access thy bank box.
					}
					else
					{
						m.BankBox.Open();
					}
				}
				else if (e.HasKeyword(0x03)) // *check*
				{
					e.Handled = true;

					if (!CheckVendorAccess(m))
					{
						Say(500389); // I will not do business with a criminal!
					}
					else
					{
						string[] split = e.Speech.Split(' ');

						if (split.Length >= 2)
						{
							int amount;

							if (int.TryParse(split[1], out amount))
							{
								if (amount < 5000)
								{
									Say("We cannot create checks for such a paltry amount of {0}!", TypeOfCurrency.Name);
									//Say(1010006); // We cannot create checks for such a paltry amount of gold!
								}
								else if (amount > 1000000)
								{
									Say(1010007); // Our policies prevent us from creating checks worth that much!
								}
								else
								{
									var check = new BankCheck(amount);

									BankBox box = m.BankBox;

									if (!box.TryDropItem(m, check, false))
									{
										Say(500386); // There's not enough room in your bankbox for the check!
										check.Delete();
									}
									else if (!WithdrawCredit(m, TypeOfCurrency, amount))
									{
										Say("Ah, art thou trying to fool me? Thou hast not so much {0}!", TypeOfCurrency.Name);
										//Say(500384); // Ah, art thou trying to fool me? Thou hast not so much gold!
										check.Delete();
									}
									else
									{
										// Into your bank box I have placed a check in the amount of:
										Say(1042673, AffixType.Append, Utility.MoneyFormat(amount, m), "");
									}
								}
							}
						}
					}
				}
				else if (speech.IndexOf("deposit", StringComparison.Ordinal) > -1 || speech.IndexOf("credit", StringComparison.Ordinal) > -1)
				{
					e.Handled = true;

					if (!CheckVendorAccess(m))
					{
						Say(500389); // I will not do business with a criminal!
					}
					else
					{
						BankBox box = m.FindBankNoCreate();

						if (box == null || box.Items.Count == 0)
						{
							Say("Ah, art thou trying to fool me? Thou hast nothing to deposit!");
						}
						else
						{
							Item[] currency = box.FindItemsByType(TypeOfCurrency, true).ToArray();
							BankCheck[] checks = box.FindItemsByType<BankCheck>(true).ToArray();

							foreach (Item c in currency)
							{
								ulong amount = Math.Min(box.Credit + (ulong)c.Amount, BankBox.MaxCredit);
								c.Consume((int)(amount - box.Credit));
								box.Credit = amount;
							}

							foreach (BankCheck c in checks)
							{
								ulong amount = Math.Min(box.Credit + (ulong)c.Worth, BankBox.MaxCredit);
								c.ConsumeWorth((int)(amount - box.Credit));

								box.Credit = amount;
							}

							Say(
								box.Credit == BankBox.MaxCredit
									? "You have reached the maximum limit.  We do not have the facilities to deposit more {0} and bank checks."
									: "I have deposited all of the {0} and bank checks from your bank box.",
								TypeOfCurrency.Name);

							Say("You currently have {0:#,0} {1} stored in credit.", box.Credit, TypeOfCurrency.Name);
						}
					}
				}
			}

			base.OnSpeech(e);
		}

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (from.Alive)
			{
				list.Add(new OpenBankEntry(this));
			}

			list.Add(new CheckBalanceEntry(this));

			base.AddCustomContextEntries(from, list);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}