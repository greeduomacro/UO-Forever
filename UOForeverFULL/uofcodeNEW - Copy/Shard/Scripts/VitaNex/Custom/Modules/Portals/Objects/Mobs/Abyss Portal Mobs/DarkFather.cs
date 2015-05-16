#region References
using System;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.CustomTitles;
using Server.Engines.LegendaryCrafting;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a demonic corpse")]
	public class DarkFatherPortal : BaseChampion
	{
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return false; } }
        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lesser; } }
        public override bool BardImmune
        {
            get { return true; }
        }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.Special; } }

        public override bool NoGoodies { get { return true; } }

        public override int PSToGive { get { return Utility.RandomDouble() >= 0.5 ? 2 : 1; } }

        public override int FactionPSToGive { get { return 0; } }

        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

		public static Mobile FindRandomPlayer(BaseCreature creature)
		{
			List<DamageStore> rights = GetLootingRights(creature.DamageEntries, creature.HitsMax);

			for (int i = rights.Count - 1; i >= 0; --i)
			{
				DamageStore ds = rights[i];

				if (!ds.m_HasRight)
				{
					rights.RemoveAt(i);
				}
			}

			if (rights.Count > 0)
			{
				return rights[Utility.Random(rights.Count)].m_Mobile;
			}

			return null;
		}

		public override WeaponAbility GetWeaponAbility()
		{
			switch (Utility.Random(3))
			{
				default:
					//case 0:
					return WeaponAbility.DoubleStrike;
				case 1:
					return WeaponAbility.WhirlwindAttack;
				case 2:
					return WeaponAbility.CrushingBlow;
			}
		}

		[Constructable]
		public DarkFatherPortal()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = NameList.RandomName("demon knight");
            SpecialTitle = "The Dark Father";
            TitleHue = 1174;
			Body = 318;
			BaseSoundID = 0x165;
		    Hue = 2655;

            Alignment = Alignment.Demon;

			SetStr(500);
			SetDex(100);
			SetInt(1000);

			SetHits(30000);
			SetMana(5000);

			SetDamage(17, 21);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 140.0);
            }


            SetSkill(SkillName.EvalInt, 140.0, 140.0);
            SetSkill(SkillName.Magery, 140.0, 140.0);

			Fame = 28000;
			Karma = -28000;

			VirtualArmor = 64;
		}

        public override void OnDeath(Container c)
        {
            var scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);
            c.DropItem(new Platinum { Amount = 50 });
            c.DropItem(new GargoyleRune());
            if (Utility.RandomDouble() < 0.5)
            {
                c.DropItem(new GargoyleRune());
            }
            if (Utility.RandomDouble() < 0.1)
            {
                c.DropItem(new GargoyleRune());
            }
            if (Utility.RandomDouble() < 0.01)
            {
                c.DropItem(new DarkFatherStatue{Hue = Hue});
            }

            base.OnDeath(c);
        }

        public override bool OnBeforeDeath()
        {
            if (0.5 > Utility.RandomDouble())
            {
                PackItem(new LegendaryHammer());
            }
            PackItem(new Gold(5000));
            PackItem(new Gold(5000));

            return base.OnBeforeDeath();
        }

		public override void GenerateLoot()
		{
			AddLoot(LootPack.SuperBoss, 2);
			AddLoot(LootPack.HighScrolls, Utility.RandomMinMax(6, 60));
		}

		private static bool m_InHere;

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			if (from == null || from == this || m_InHere)
			{
				return;
			}

			m_InHere = true;

			from.Damage(Utility.RandomMinMax(8, 20), this);

			MovingEffect(from, 0xECA, 10, 0, false, false, 0, 0);
			PlaySound(0x491);

			if (0.05 > Utility.RandomDouble())
			{
				Timer.DelayCall(TimeSpan.FromSeconds(1.0), CreateBones_Callback, from);
			}

			m_InHere = false;

            base.OnDamage(amount, from, willKill);
		}

		public virtual void CreateBones_Callback(Mobile from)
		{
			Map map = from.Map;

			if (map == null)
			{
				return;
			}

			int count = Utility.RandomMinMax(1, 3);

			for (int i = 0; i < count; ++i)
			{
				int x = from.X + Utility.RandomMinMax(-1, 1);
				int y = from.Y + Utility.RandomMinMax(-1, 1);
				int z = from.Z;

				if (!map.CanFit(x, y, z, 16, false, true))
				{
					z = map.GetAverageZ(x, y);

					if (z == from.Z || !map.CanFit(x, y, z, 16, false, true))
					{
						continue;
					}
				}

				var bone = new UnholyBone
				{
					Hue = 0,
					Name = "unholy bones",
					ItemID = Utility.Random(0xECA, 9)
				};

				bone.MoveToWorld(new Point3D(x, y, z), map);
			}
		}

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;
            if (0.05 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    var recipescroll = new RecipeScroll(Recipe.GetRandomRecipe());
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(recipescroll);
                        eligibleMobs[i].SendMessage(54, "You have received a recipe scroll!");
                        break;
                    }
                }
            }

            if (0.1 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(new TitleScroll("The Lightbringer"));
                        eligibleMobs[i].SendMessage(54, "You have received a title scroll.");
                        return;
                    }
                }
            }
        }

        public DarkFatherPortal(Serial serial)
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