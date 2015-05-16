#region References
using Server.Engines.Plants;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a mummy corpse")]
	public class Mummy : BaseCreature
	{
		public override string DefaultName { get { return "a mummy"; } }

		[Constructable]
		public Mummy()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
		{
			Body = 154;
			BaseSoundID = 471;

		    Alignment = Alignment.Undead;

			SetStr(346, 370);
			SetDex(71, 90);
			SetInt(26, 40);

			SetHits(208, 222);

			SetDamage(13, 23);

			SetSkill(SkillName.MagicResist, 15.1, 40.0);
			SetSkill(SkillName.Tactics, 35.1, 50.0);
			SetSkill(SkillName.Wrestling, 35.1, 50.0);

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 50;

			PackItem(new Garlic(5));
			PackItem(new Bandage(10));
		}

		public override bool OnBeforeDeath()
		{
			if (base.OnBeforeDeath())
			{
				if (EraML && Utility.RandomDouble() < 0.33)
				{
					PackItem(Seed.RandomPeculiarSeed(2));
				}

				return true;
			}

			return false;
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Rich);
			AddLoot(LootPack.Gems);
			AddLoot(LootPack.Potions);
		}

		public override bool BleedImmune { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lesser; } }
		public override int DefaultBloodHue { get { return 1438; } }

		public Mummy(Serial serial)
			: base(serial)
		{ }

		public override OppositionGroup OppositionGroup { get { return OppositionGroup.FeyAndUndead; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}