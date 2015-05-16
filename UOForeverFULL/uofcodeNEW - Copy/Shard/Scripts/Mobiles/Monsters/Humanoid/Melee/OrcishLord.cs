#region References
using Server.Items;
using Server.Misc;
#endregion

namespace Server.Mobiles
{
	[CorpseName("an orcish corpse")]
	public class OrcishLord : BaseCreature
	{
		public override InhumanSpeech SpeechType { get { return InhumanSpeech.Orc; } }
		public override string DefaultName { get { return "an orcish lord"; } }

		[Constructable]
		public OrcishLord()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Body = 138;
			BaseSoundID = 0x45A;

			SetStr(147, 215);
			SetDex(91, 115);
			SetInt(61, 85);

			SetHits(95, 123);

			SetDamage(4, 14);

			SetSkill(SkillName.MagicResist, 70.1, 85.0);
			SetSkill(SkillName.Swords, 60.1, 85.0);
			SetSkill(SkillName.Tactics, 75.1, 90.0);
			SetSkill(SkillName.Wrestling, 60.1, 85.0);

			Fame = 2500;
			Karma = -2500;

            Alignment = Alignment.Orc;

			switch (Utility.Random(5))
			{
				case 0:
					PackItem(new Lockpick());
					break;
				case 1:
					PackItem(new MortarPestle());
					break;
				case 2:
					PackItem(new Bottle());
					break;
				case 3:
					PackItem(new RawRibs());
					break;
				case 4:
					PackItem(new Shovel());
					break;
			}

			PackItem(new RingmailChest());

			if (0.2 > Utility.RandomDouble())
			{
				PackItem(new EvilOrcHelm(1175));
			}
		}

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (0.3 > Utility.RandomDouble())
			{
				PackItem(Loot.RandomPossibleReagent(Expansion));
			}
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Meager);
			AddLoot(LootPack.Average);
			AddPackedLoot(LootPack.PoorProvisions, typeof(Bag));
			AddPackedLoot(LootPack.MeagerProvisions, typeof(Bag));

			if (0.5 > Utility.RandomDouble())
			{
				AddPackedLoot(LootPack.AverageProvisions, typeof(Pouch));
			}

			if (0.05 > Utility.RandomDouble())
			{
				PackItem(new EvilOrcHelm());
			}
		}

		public override bool CanRummageCorpses { get { return true; } }
		public override int TreasureMapLevel { get { return 1; } }
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

		public OrcishLord(Serial serial)
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