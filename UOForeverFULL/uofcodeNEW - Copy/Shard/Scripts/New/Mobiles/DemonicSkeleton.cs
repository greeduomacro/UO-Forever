#region References
using Server.Engines.Craft;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a skeletal corpse")]
	public class DemonicSkeleton : BaseCreature
	{
		public override string DefaultName { get { return "a demonic skeleton"; } }

		[Constructable]
		public DemonicSkeleton()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Body = 57;
			BaseSoundID = 451;
			Hue = 32;

		    Alignment = Alignment.Undead;

			SetStr(196, 250);
			SetDex(76, 95);
			SetInt(36, 60);

			SetHits(118, 150);

			SetDamage(8, 18);

			SetSkill(SkillName.MagicResist, 65.1, 80.0);
			SetSkill(SkillName.Tactics, 85.1, 100.0);
			SetSkill(SkillName.Wrestling, 85.1, 95.0);

			Fame = 3000;
			Karma = -3000;

			VirtualArmor = 40;
		}

		public override bool OnBeforeDeath()
		{
			AddLoot(LootPack.FilthyRich, 2);
			AddLoot(LootPack.Rich, 2);
			AddLoot(LootPack.Average, 2);
			AddLoot(LootPack.Gems, 5);

			PackItem(new Gold(1400, 1900));

			if (0.05 >= Utility.RandomDouble())
			{
				var scroll = new SkillScroll();
				scroll.Randomize();
				PackItem(scroll);
			}

			if (0.1 >= Utility.RandomDouble())
			{
				PackItem(new CultistPaint());
			}

			if (0.01 > Utility.RandomDouble())
			{
			    int ItemID = 0;
                switch (Utility.Random(7))
                {
                    case 0: ItemID = 7026; break;
                    case 1: ItemID = 5102; break;
                    case 2: ItemID = 5104; break;
                    case 3: ItemID = 5099; break;
                    case 4: ItemID = 5100; break;
                    case 5: ItemID = 5134; break;
                    case 6: ItemID = 5139; break;
                }
				PackItem(new AncientArmorNew(ItemID));
			}

			return true;
		}

		public override bool BleedImmune { get { return true; } }
		public override int DefaultBloodHue { get { return -1; } }
		public override int TreasureMapLevel { get { return 6; } }

		public DemonicSkeleton(Serial serial)
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
			
			reader.ReadInt();
		}
	}
}