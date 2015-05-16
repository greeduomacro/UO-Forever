// **********
// RunUO Shard - LordOaks.cs
// **********

#region References

using System;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.CustomTitles;
using Server.Items;

#endregion

namespace Server.Mobiles
{
	public class LordOaks : BaseChampion
	{
		private Mobile m_Queen;
		private bool m_SpawnedQueen;

		public override ChampionSkullType SkullType { get { return ChampionSkullType.Enlightenment; } }

		[Constructable]
		public LordOaks()
			: base(AIType.AI_Mage, FightMode.Evil)
		{
			Body = 175;
			Name = "Lord Oaks";

            SpecialTitle = "The Fae King";
            TitleHue = 1174;

            Alignment = Alignment.Inhuman;

			SetStr(603, 1050);
			SetDex(125, 190);
			SetInt(750, 950);

			SetHits(7950, 8590);
			SetStam(465, 690);

			SetDamage(35, 45);

			SetSkill(SkillName.Anatomy, 75.1, 100.0);
			SetSkill(SkillName.EvalInt, 120.1, 130.0);
			SetSkill(SkillName.Magery, 120.0);
			SetSkill(SkillName.Meditation, 120.1, 130.0);
			SetSkill(SkillName.MagicResist, 100.5, 150.0);
			SetSkill(SkillName.Tactics, 100.0);
			SetSkill(SkillName.Wrestling, 100.0);

			Fame = 22500;
			Karma = 22500;

			VirtualArmor = 30;
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.UltraRich, 5);
		}

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
        {

            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;
            if (0.3 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    MuzzlePacker muzzlepacker = new MuzzlePacker();
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(muzzlepacker);
                        eligibleMobs[i].SendMessage(54, "You have received a cannon muzzle packer!");
                        break;
                    }
                }
            }

            currentTestValue = 0.0;
            roll = Utility.RandomDouble() * totalScores;
            if (0.2 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    Random rand = new Random();
                    int value = rand.Next(0, 3);
                    Item item = null;
                    switch (value)
                    {
                        case 0:
                            {
                                item = new LordOaksStatueArtifact();
                            }
                            break;
                        case 1:
                            {
                                item = new PierArtifact();
                            }
                            break;
                        case 2:
                            {
                                item = new MountedPixieArtifact();
                            }
                            break;
                    }
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null && item != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(item);
                        eligibleMobs[i].SendMessage(54, "You have received a " + item.Name + ".");
                        break;
                    }
                }
            }

            if (0.2 > Utility.RandomDouble())
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
                        eligibleMobs[i].Backpack.DropItem(new TitleScroll("Renaissance Man"));
                        eligibleMobs[i].SendMessage(54, "You have received a title scroll.");
                        return;
                    }
                }
            }
        }

		public override bool AutoDispel { get { return true; } }
		public override bool BardImmune { get { return !EraSE; } }
		public override bool Unprovokable { get { return EraSE; } }
		public override bool Uncalmable { get { return EraSE; } }
		public override OppositionGroup OppositionGroup { get { return OppositionGroup.FeyAndUndead; } }
		public override Poison PoisonImmune { get { return Poison.Deadly; } }

		public void SpawnPixies(Mobile target)
		{
			Map map = Map;

			if (map == null)
			{
				return;
			}

			Say(1042154); // You shall never defeat me as long as I have my queen!

			int newPixies = Utility.RandomMinMax(3, 6);

			for (int i = 0; i < newPixies; ++i)
			{
				var pixie = new Pixie {
					Team = Team,
					FightMode = FightMode.Closest
				};

				Point3D loc = Location;

				for (int j = 0; j < 10; ++j)
				{
					int x = X + Utility.Random(3) - 1;
					int y = Y + Utility.Random(3) - 1;
					int z = map.GetAverageZ(x, y);

					if (false == map.CanFit(x, y, Z, 16, false, false))
					{
						loc = new Point3D(x, y, Z);
					}
					else if (false == map.CanFit(x, y, z, 16, false, false))
					{
						loc = new Point3D(x, y, z);
					}
				}

				pixie.MoveToWorld(loc, map);
				pixie.Combatant = target;
			}
		}

		public override int GetAngerSound()
		{
			return 0x2F8;
		}

		public override int GetIdleSound()
		{
			return 0x2F8;
		}

		public override int GetAttackSound()
		{
			return Utility.Random(0x2F5, 2);
		}

		public override int GetHurtSound()
		{
			return 0x2F9;
		}

		public override int GetDeathSound()
		{
			return 0x2F7;
		}

		public void CheckQueen()
		{
			if (Map == null)
			{
				return;
			}

			if (!m_SpawnedQueen)
			{
				Say(1042153); // Come forth my queen!

				m_Queen = new Silvani();

				((BaseCreature)m_Queen).Team = Team;

				m_Queen.MoveToWorld(Location, Map);

				m_SpawnedQueen = true;
			}
			else if (m_Queen != null && m_Queen.Deleted)
			{
				m_Queen = null;
			}
		}

		public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
		{
			CheckQueen();

			if (m_Queen == null)
			{
				return;
			}
			scalar *= 0.1;

			if (0.1 >= Utility.RandomDouble())
			{
				SpawnPixies(caster);
			}
		}

		public override void OnGaveMeleeAttack(Mobile defender)
		{
			base.OnGaveMeleeAttack(defender);

			defender.Stam -= Utility.Random(20, 10);
			defender.Mana -= Utility.Random(20, 10);
			defender.Damage(Utility.Random(20, 10), this);
		}

		public override void OnGotMeleeAttack(Mobile attacker)
		{
			base.OnGotMeleeAttack(attacker);

			CheckQueen();

			if (m_Queen != null && 0.1 >= Utility.RandomDouble())
			{
				SpawnPixies(attacker);
			}

			attacker.Stam -= Utility.Random(20, 10);
			attacker.Mana -= Utility.Random(20, 10);
			attacker.Damage(Utility.Random(20, 10), this);
		}

		public LordOaks(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(m_Queen);
			writer.Write(m_SpawnedQueen);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_Queen = reader.ReadMobile();
						m_SpawnedQueen = reader.ReadBool();

						break;
					}
			}
		}
	}
}