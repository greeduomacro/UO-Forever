#region References
using Server.Items;
using Server.Misc;
#endregion

namespace Server.Mobiles
{
	[CorpseName("an orcish corpse")]
	public class OrcCaptain : BaseCreature
	{
		public override InhumanSpeech SpeechType { get { return InhumanSpeech.Orc; } }

		[Constructable]
		public OrcCaptain()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = NameList.RandomName("orc");
			Body = 7;
			BaseSoundID = 0x45A;

            Alignment = Alignment.Orc;

			SetStr(111, 145);
			SetDex(101, 135);
			SetInt(86, 110);

			SetHits(67, 87);

			SetDamage(5, 15);

			SetSkill(SkillName.MagicResist, 70.1, 85.0);
			SetSkill(SkillName.Swords, 70.1, 95.0);
			SetSkill(SkillName.Tactics, 85.1, 100.0);

			Fame = 2500;
			Karma = -2500;

			VirtualArmor = 34;

			// TODO: Skull?
			switch (Utility.Random(7))
			{
				case 0:
					PackItem(new Arrow());
					break;
				case 1:
					PackItem(new Lockpick());
					break;
				case 2:
					PackItem(new Shaft());
					break;
				case 3:
					PackItem(new Ribs());
					break;
				case 4:
					PackItem(new Bandage());
					break;
				case 5:
					PackItem(new BeverageBottle(BeverageType.Wine));
					break;
				case 6:
					PackItem(new Jug(BeverageType.Cider));
					break;
			}
		}

		public override bool OnBeforeDeath()
		{
			if (base.OnBeforeDeath())
			{
				if (EraAOS)
				{
					PackItem(Loot.RandomNecromancyReagent());
				}

				return true;
			}

			return false;
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Meager, 2);
		}

		public override bool CanRummageCorpses { get { return true; } }
		public override int Meat { get { return 1; } }
		public override int DefaultBloodHue { get { return -2; } }
		public override int BloodHueTemplate { get { return Utility.RandomBlackHue(); } }

		public override OppositionGroup OppositionGroup { get { return OppositionGroup.SavagesAndOrcs; } }

		public override bool IsEnemy(Mobile m)
		{
			if (m.Player && m.FindItemOnLayer(Layer.Helm) is OrcishKinMask)
			{
				return false;
			}

			return base.IsEnemy(m);
		}

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);

            Item item = aggressor.FindItemOnLayer(Layer.Helm);

            if (item is OrcishKinMask)
            {
                aggressor.Damage(50);
                item.Delete();
                aggressor.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                aggressor.PlaySound(0x307);
            }
        }

		public OrcCaptain(Serial serial)
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
		}
	}
}