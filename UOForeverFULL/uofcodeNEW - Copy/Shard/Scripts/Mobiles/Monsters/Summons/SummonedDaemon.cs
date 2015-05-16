namespace Server.Mobiles
{
	[CorpseName("a daemon corpse")]
	public class SummonedDaemon : BaseCreature
	{
		public override double DispelDifficulty { get { return 125.0; } }
		public override double DispelFocus { get { return 45.0; } }
		public override bool AlwaysMurderer { get { return true; } }

		[Constructable]
		public SummonedDaemon()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = NameList.RandomName("daemon");
			Body = 9;
			BaseSoundID = 357;

			SetStr(200);
			SetDex(110);
			SetInt(150);

			SetDamage(14, 21);

			SetSkill(SkillName.EvalInt, 90.1, 100.0);
			SetSkill(SkillName.Meditation, 90.1, 100.0);
			SetSkill(SkillName.Magery, 90.1, 100.0);
			SetSkill(SkillName.MagicResist, 90.1, 100.0);
			SetSkill(SkillName.Tactics, 100.0);
			SetSkill(SkillName.Wrestling, 98.1, 99.0);

			VirtualArmor = 58;
			ControlSlots = 4;
		}

		public override Poison PoisonImmune { get { return Poison.Regular; } } // TODO: Immune to poison?

		public SummonedDaemon(Serial serial)
			: base(serial)
		{ }

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			Body = EraAOS ? 10 : 9;
		}

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
}