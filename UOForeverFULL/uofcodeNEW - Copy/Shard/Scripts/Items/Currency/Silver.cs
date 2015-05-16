namespace Server.Items
{
    [TypeAlias("Server.Factions.Silver")]
	public class Silver : Item, ICurrency
	{
		[CommandProperty(AccessLevel.Counselor)]
		public override bool ExpansionChangeAllowed { get { return false; } }

		public override double DefaultWeight { get { return (EraML ? (0.02 / 3) : 0.02); } }

		[Constructable]
		public Silver()
			: this(1)
		{ }

		[Constructable]
		public Silver(int amountFrom, int amountTo)
			: this(Utility.RandomMinMax(amountFrom, amountTo))
		{ }

		[Constructable] 
		public Silver(int amount)
			: base(0xEF0)
		{
			Stackable = true;
			Amount = amount;
		}

		public Silver(Serial serial)
			: base(serial)
		{ }

		public override int GetDropSound()
		{
			if (Amount <= 1)
			{
				return 0x2E4;
			}

			if (Amount <= 5)
			{
				return 0x2E5;
			}

			return 0x2E6;
		}

		protected override void OnAmountChange(int oldValue)
		{
			if (Expansion != Expansion.T2A)
			{
				base.OnAmountChange(oldValue);
				return;
			}

			int newValue = Amount;

			UpdateTotal(this, TotalType.Gold, newValue - oldValue);
		}

		public override int GetTotal(TotalType type)
		{
			if (Expansion != Expansion.T2A)
			{
				return base.GetTotal(type);
			}

			int baseTotal = base.GetTotal(type);

			if (type == TotalType.Gold)
			{
				baseTotal += Amount;
			}

			return baseTotal;
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