#region References
using System;

using Server.Engines.Craft;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a skeletal dragon corpse")]
	public class SkeletalDragon : BaseCreature
	{
		public override string DefaultName { get { return "a skeletal dragon"; } }

		[Constructable]
		public SkeletalDragon()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Body = 104;
			BaseSoundID = 0x488;

            Alignment = Alignment.Undead;

			SetStr(828, 1030);
			SetDex(68, 200);
			SetInt(488, 620);

			SetHits(528, 549);

			SetDamage(29, 34);

			SetSkill(SkillName.EvalInt, 80.1, 100.0);
			SetSkill(SkillName.Magery, 80.1, 100.0);
			SetSkill(SkillName.MagicResist, 100.3, 130.0);
			SetSkill(SkillName.Tactics, 97.6, 100.0);
			SetSkill(SkillName.Wrestling, 97.6, 100.0);
			//SetSkill( SkillName.Necromancy, 120.1, 130.0 );
			SetSkill(SkillName.SpiritSpeak, 120.1, 130.0);

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 55;
		}

		public override void GenerateLoot(bool spawning)
		{
			base.GenerateLoot(spawning);
			if (!spawning && Utility.Random(1000) == 0)
			{
				PackItem(new SkeletalDragonScale());
			}
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.FilthyRich, 4);
			AddLoot(LootPack.Gems, 5);

			if (0.04 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
			{
				var scroll = new SkillScroll();
				scroll.Randomize();
				PackItem(scroll);
			}
		}

		public override bool ReacquireOnMovement { get { return true; } }
		public override bool HasBreath { get { return true; } } // fire breath enabled
		public override int BreathFireDamage { get { return 0; } }
		public override int BreathColdDamage { get { return 100; } }
		public override int BreathEffectHue { get { return 0x480; } }
		public override double BonusPetDamageScalar { get { return EraSE ? 3.0 : 1.0; } }
		public override int DefaultBloodHue { get { return -1; } }

		public override bool AutoDispel { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override bool BleedImmune { get { return true; } }
		//public override int Meat{ get{ return 19; } } // where's it hiding these? :)
		//public override int Hides{ get{ return 20; } }
		//public override HideType HideType{ get{ return HideType.Barbed; } }

		public SkeletalDragon(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}

		private DateTime m_NextAttack;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if (combatant == null || combatant.Deleted || combatant.Map != Map || !InRange(combatant, 12) ||
				!CanBeHarmful(combatant) || !InLOS(combatant))
			{
				return;
			}

			if (Paralyzed || DateTime.UtcNow < m_NextAttack || Utility.Random(5) != 0)
			{
				return;
			}

			SummonUndead(combatant);

			m_NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds(8.0 + (8.0 * Utility.RandomDouble()));
		}

		public override bool OnBeforeDeath()
		{
			switch (Utility.Random(500))
			{
				case 0:
					PackItem(new LeatherDyeTub());
					break;
				case 1:
					PackItem(new DragonHead());
					break;
			}

            if (0.05 > Utility.RandomDouble())
            {
                PackItem(new DragonBoneShards());
            }
            if (0.001 > Utility.RandomDouble())
            {
                PackItem(new DragonHeart());
            }

			return base.OnBeforeDeath();
		}

		public void SummonUndead(Mobile target)
		{
			BaseCreature summon = null;

			switch (Utility.Random(11))
			{
				default:
					//case 0:
					summon = new Skeleton();
					break;
				case 1:
					summon = new Zombie();
					break;
				case 2:
					summon = new Wraith();
					break;
				case 3:
					summon = new Spectre();
					break;
				case 4:
					summon = new Ghoul();
					break;
				case 5:
					summon = new Mummy();
					break;
				case 6:
					summon = new Bogle();
					break;
				case 7:
					summon = new BoneKnight();
					break;
				case 8:
					summon = new SkeletalKnight();
					break;
				case 9:
					summon = new Lich();
					break;
				case 10:
					summon = new SkeletalMage();
					break;
			}

			summon.Team = Team;
			summon.FightMode = FightMode.Closest;
			summon.MoveToWorld(target.Location, target.Map);

			Effects.SendLocationEffect(summon.Location, summon.Map, 0x3728, 10, 10, 0, 0);

			summon.Combatant = target;
			summon.PlaySound(summon.GetAttackSound());
		}
	}
}