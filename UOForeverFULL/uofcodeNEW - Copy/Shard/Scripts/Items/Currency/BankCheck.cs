#region References
using System;

using Server.Engines.Quests;
using Server.Engines.Quests.Haven;
using Server.Engines.Quests.Necro;
using Server.Mobiles;
using Server.Network;

using CashBankCheckObjective = Server.Engines.Quests.Necro.CashBankCheckObjective;
#endregion

namespace Server.Items
{
	public class BankCheck : Item
	{
		[CommandProperty(AccessLevel.Counselor)]
		public override bool ExpansionChangeAllowed { get { return false; } }

		public virtual Type TypeOfCurrency { get { return IsDonation ? typeof(DonationCoin) : typeof(Gold); } }

        public virtual bool IsDonation { get; set; }

		private int m_Worth;

		[CommandProperty(AccessLevel.GameMaster)]
		public int Worth
		{
			get { return m_Worth; }
			set
			{
				m_Worth = value;
				InvalidateProperties();
			}
		}

		public BankCheck(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

            writer.Write(IsDonation);
			writer.Write(m_Worth);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			LootType = LootType.Blessed;

			int version = reader.ReadInt();

			switch (version)
			{
                case 1:
                    {
                        IsDonation = reader.ReadBool();
                    }
			        goto case 0;
				case 0:
					{
						m_Worth = reader.ReadInt();
						break;
					}
			}
		}

		[Constructable]
		public BankCheck(int worth)
			: base(0x14F0)
		{
			Weight = 1.0;
			Hue = 0x34;
			LootType = LootType.Blessed;

			m_Worth = worth;
		}

        [Constructable]
        public BankCheck(int worth, bool donation)
            : base(0x14F0)
        {
            Weight = 1.0;
            Hue = 0x34;
            LootType = LootType.Blessed;

            m_Worth = worth;

            IsDonation = donation;

            if (IsDonation)
                Hue = 1153;
        }

		public virtual void ConsumeWorth()
		{
			ConsumeWorth(1);
		}

		public virtual void ConsumeWorth(int worth)
		{
			Worth -= worth;

			if (Worth <= 0)
			{
				Delete();
			}
		}

		public override bool DisplayLootType { get { return EraAOS; } }

		public override int LabelNumber { get { return 1041361; } } // A bank check

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			string worth = m_Worth.ToString("#,0");

			list.Add(1060738, worth); // value: ~1_val~
		}

		public override void OnSingleClick(Mobile from)
		{
			LabelToExpansion(from);

			from.Send(
				new MessageLocalizedAffix(
					Serial,
					ItemID,
					MessageType.Label,
					0x3B2,
					3,
					1041361,
					"",
					AffixType.Append,
					String.Format(": {0}{1}", Utility.MoneyFormat(m_Worth, from), IsDonation ? " donation coins" : "gp"),
					"")); // A bank check:
		}

		public override void OnDoubleClick(Mobile from)
		{
			BankBox box = from.FindBankNoCreate();

			if (box != null && IsChildOf(box))
			{
				Delete();

				int deposited = 0;

				int toAdd = m_Worth;

				Item currency;

				while (toAdd >= 60000)
				{
					currency = TypeOfCurrency.CreateInstanceSafe<Item>();

					currency.Stackable = true;
					currency.Amount = 60000;

					if (box.TryDropItem(from, currency, false))
					{
						toAdd -= 60000;
						deposited += 60000;
					}
					else
					{
						currency.Delete();

						from.AddToBackpack(new BankCheck(toAdd));
						toAdd = 0;

						break;
					}
				}

				if (toAdd > 0)
				{
					currency = TypeOfCurrency.CreateInstanceSafe<Item>();

					currency.Stackable = true;
					currency.Amount = toAdd;

					if (box.TryDropItem(from, currency, false))
					{
						deposited += toAdd;
					}
					else
					{
						currency.Delete();

						from.AddToBackpack(new BankCheck(toAdd));
					}
				}

				// Gold was deposited in your account:
				from.SendLocalizedMessage(1042672, true, " " + deposited.ToString("#,0"));

				var pm = from as PlayerMobile;

				if (pm != null)
				{
					QuestSystem qs = pm.Quest;

					if (qs is DarkTidesQuest)
					{
						QuestObjective obj = qs.FindObjective(typeof(CashBankCheckObjective));

						if (obj != null && !obj.Completed)
						{
							obj.Complete();
						}
					}

					if (qs is UzeraanTurmoilQuest)
					{
						QuestObjective obj = qs.FindObjective(typeof(Engines.Quests.Haven.CashBankCheckObjective));

						if (obj != null && !obj.Completed)
						{
							obj.Complete();
						}
					}
				}
			}
			else
			{
				from.SendLocalizedMessage(1047026); // That must be in your bank box to use it.
			}
		}
	}
}