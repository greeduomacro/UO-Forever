#region References
using Server.Items;
#endregion

namespace Server.Mobiles
{
	[CorpseName("an acid elemental corpse")]
	public class ToxicElemental : BaseCreature
	{
		public override double DispelDifficulty { get { return 98.5; } }
		public override double DispelFocus { get { return 65.0; } }
		public override string DefaultName { get { return "a toxic elemental"; } }

		[Constructable]
		public ToxicElemental()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Body = 0x9E;
			BaseSoundID = 278;

            Alignment = Alignment.Elemental;

			SetStr(326, 355);
			SetDex(66, 85);
			SetInt(271, 295);

			SetHits(196, 213);

			SetDamage(9, 15);

			SetSkill(SkillName.Anatomy, 30.3, 60.0);
			SetSkill(SkillName.EvalInt, 70.1, 85.0);
			SetSkill(SkillName.Magery, 70.1, 85.0);
			SetSkill(SkillName.MagicResist, 60.1, 75.0);
			SetSkill(SkillName.Tactics, 80.1, 90.0);
			SetSkill(SkillName.Wrestling, 70.1, 90.0);

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 40;
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Rich, 3);
			AddLoot(LootPack.Average);

			if (0.06 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
			{
				var scroll = new SkillScroll();
				scroll.Randomize();
				PackItem(scroll);
			}
		}

		public override bool BleedImmune { get { return true; } }
		public override Poison HitPoison { get { return Poison.Deadly; } }
		public override double HitPoisonChance { get { return 0.6; } }
		public override int TreasureMapLevel { get { return EraAOS ? 2 : 3; } }
		public override int DefaultBloodHue { get { return 60; } }

		public ToxicElemental(Serial serial)
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
			int version = reader.ReadInt();

			if (BaseSoundID == 263)
			{
				BaseSoundID = 278;
			}

			if (Body == 13)
			{
				Body = 0x9E;
			}

			if (Hue == 0x4001)
			{
				Hue = 0;
			}
		}
	}
}