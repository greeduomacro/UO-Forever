#region References
using Server.Items;
#endregion

namespace Server.Mobiles
{
	[CorpseName("corpse of lord kruzk")]
	public class Kruzk : BaseCreature
	{
		[Constructable]
		public Kruzk()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "Necromancer Kruzk";
			Body = Utility.RandomList(125, 126);

			PackItem(new Robe(Utility.RandomMetalHue()));
			PackItem(new WizardsHat(Utility.RandomMetalHue()));

			SetStr(81, 105);
			SetDex(191, 215);
			SetInt(126, 150);

			SetHits(49, 63);

			SetDamage(5, 10);

			SetSkill(SkillName.EvalInt, 80.2, 100.0);
			SetSkill(SkillName.Magery, 95.1, 100.0);
			SetSkill(SkillName.Meditation, 27.5, 50.0);
			SetSkill(SkillName.MagicResist, 77.5, 100.0);
			SetSkill(SkillName.Tactics, 65.0, 87.5);
			SetSkill(SkillName.Wrestling, 20.3, 80.0);

			Fame = 10500;
			Karma = -10500;

			VirtualArmor = 16;
			PackReg(23);
			if (Utility.RandomBool())
			{
				PackItem(new Shoes());
			}
			else
			{
				PackItem(new Sandals());
			}
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Average);
			AddLoot(LootPack.Meager);
			AddLoot(LootPack.MedScrolls, 2);
		}

		public override bool CanRummageCorpses { get { return true; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override int Meat { get { return 1; } }
		public override int TreasureMapLevel { get { return EraAOS ? 2 : 0; } }

		public Kruzk(Serial serial)
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