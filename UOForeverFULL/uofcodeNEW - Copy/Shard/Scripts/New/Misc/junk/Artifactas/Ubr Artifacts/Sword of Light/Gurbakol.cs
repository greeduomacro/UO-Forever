#region References
using Server.Items;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a corpse of Gurbakol")]
	public class Gurbakol : BaseCreature
	{
		public override bool BardImmune { get { return !EraSE; } }
		public override bool Unprovokable { get { return EraSE; } }
		public override bool AreaPeaceImmune { get { return EraSE; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override int TreasureMapLevel { get { return 1; } }

		[Constructable]
		public Gurbakol()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "Gurbakol";
			Body = 308;
			BaseSoundID = 0x48D;
			Hue = 1157;

			SetStr(1000);
			SetDex(151, 175);
			SetInt(171, 220);

			SetHits(3600);

			SetDamage(34, 36);

			SetSkill(SkillName.DetectHidden, 80.0);
			SetSkill(SkillName.EvalInt, 77.6, 87.5);
			SetSkill(SkillName.Magery, 77.6, 87.5);
			SetSkill(SkillName.Meditation, 100.0);
			SetSkill(SkillName.MagicResist, 50.1, 75.0);
			SetSkill(SkillName.Tactics, 100.0);
			SetSkill(SkillName.Wrestling, 100.0);

			Fame = 20000;
			Karma = -20000;

			VirtualArmor = 44;
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.FilthyRich, 8);
		}

		public override bool OnBeforeDeath()
		{
			switch (Utility.Random(10))
			{
				case 0:
					PackItem(new SwordOfLight());
					break;
			}
			return base.OnBeforeDeath();
		}

		public Gurbakol(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			
			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			
			reader.ReadInt();
		}
	}
}