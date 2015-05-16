namespace Server.Items
{
	public class HealPotion : BaseHealPotion
	{
		public override int MinHeal { get { return EraAOS ? 13 : 6; } }
		public override int MaxHeal { get { return EraAOS ? 16 : 20; } }
		public override double Delay { get { return EraAOS ? 8.0 : 10.0; } }

		[Constructable]
		public HealPotion()
			: base(PotionEffect.Heal)
		{ }

		public HealPotion(Serial serial)
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

			reader.ReadInt();
		}
	}
}