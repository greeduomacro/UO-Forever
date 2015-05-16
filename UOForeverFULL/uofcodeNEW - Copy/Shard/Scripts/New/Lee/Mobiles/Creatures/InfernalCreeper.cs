namespace Server.Mobiles
{
	[CorpseName("an infernal corpse")]
	public class InfernalCreeper : BaseCreature
	{
		public override string DefaultName { get { return "infernal creeper"; } }

		public override int DefaultBloodHue { get { return -1; } }

		public override bool AlwaysMurderer { get { return true; } }
		public override bool BleedImmune { get { return true; } }

		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		
		[Constructable]
		public InfernalCreeper()
			: base(AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4)
		{
			Body = 776;
			Hue = Utility.RandomBlackHue();
			BaseSoundID = 357;

			SetStr(110, 130);
			SetDex(10, 120);
			SetInt(30, 50);

			SetHits(65, 80);

			SetDamage(7, 10);

			SetSkill(SkillName.MagicResist, 75.1, 100.0);
			SetSkill(SkillName.Tactics, 55.1, 80.0);
			SetSkill(SkillName.Wrestling, 55.1, 75.0);

			Fame = 450;
			Karma = -450;

			VirtualArmor = 38;
		}

		public InfernalCreeper(Serial serial)
			: base(serial)
		{ }

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Meager);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}