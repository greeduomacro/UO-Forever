#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a meer's corpse")]
	public class MeerEternal : BaseCreature
	{
		public override string DefaultName { get { return "a meer eternal"; } }

		[Constructable]
		public MeerEternal()
			: base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
		{
			Body = 772;

			SetStr(416, 505);
			SetDex(146, 165);
			SetInt(566, 655);

			SetHits(250, 303);

			SetDamage(11, 13);

			SetSkill(SkillName.EvalInt, 90.1, 100.0);
			SetSkill(SkillName.Magery, 90.1, 100.0);
			SetSkill(SkillName.Meditation, 90.1, 100.0);
			SetSkill(SkillName.MagicResist, 150.5, 200.0);
			SetSkill(SkillName.Tactics, 50.1, 70.0);
			SetSkill(SkillName.Wrestling, 60.1, 80.0);

			Fame = 18000;
			Karma = 18000;

			VirtualArmor = 34;

			m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 5));
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.FilthyRich, 2);
			AddLoot(LootPack.MedScrolls, 2);
			AddLoot(LootPack.HighScrolls, 2);
		}

		public override bool AutoDispel { get { return true; } }
		//public override bool BardImmune{ get{ return !EraAOS; } }
		public override bool CanRummageCorpses { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override int TreasureMapLevel { get { return EraAOS ? 5 : 4; } }

		public override bool InitialInnocent { get { return true; } }

		public override int GetHurtSound()
		{
			return 0x167;
		}

		public override int GetDeathSound()
		{
			return 0xBC;
		}

		public override int GetAttackSound()
		{
			return 0x28B;
		}

		private DateTime m_NextAbilityTime;

		private void DoAreaLeech()
		{
			m_NextAbilityTime += TimeSpan.FromSeconds(2.5);

			Say(true, "Beware, mortals!  You have provoked my wrath!");
			FixedParticles(0x376A, 10, 10, 9537, 33, 0, EffectLayer.Waist);

			Timer.DelayCall(TimeSpan.FromSeconds(5.0), DoAreaLeech_Finish);
		}

		private void DoAreaLeech_Finish()
		{
			var list = new List<Mobile>();

			foreach (Mobile m in GetMobilesInRange(6))
			{
				if (CanBeHarmful(m) && IsEnemy(m))
				{
					list.Add(m);
				}
			}

			if (list.Count == 0)
			{
				Say(true, "Bah! You have escaped my grasp this time, mortal!");
			}
			else
			{
				double scalar;

				if (list.Count == 1)
				{
					scalar = 0.75;
				}
				else if (list.Count == 2)
				{
					scalar = 0.50;
				}
				else
				{
					scalar = 0.25;
				}

				for (int i = 0; i < list.Count; ++i)
				{
					Mobile m = list[i];

					var damage = (int)(m.Hits * scalar);

					damage += Utility.RandomMinMax(-5, 5);

					if (damage < 1)
					{
						damage = 1;
					}

					m.MovingParticles(this, 0x36F4, 1, 0, false, false, 32, 0, 9535, 1, 0, (EffectLayer)255, 0x100);
					m.MovingParticles(this, 0x0001, 1, 0, false, true, 32, 0, 9535, 9536, 0, (EffectLayer)255, 0);

					DoHarmful(m);
					Hits += damage;
				}

				Say(true, "If I cannot cleanse thy soul, I will destroy it!");
			}
		}

		private void DoFocusedLeech(Mobile combatant, string message)
		{
			Say(true, message);

			Timer.DelayCall(TimeSpan.FromSeconds(0.5), DoFocusedLeech_Stage1, combatant);
		}

		private void DoFocusedLeech_Stage1(Mobile combatant)
		{
			if (CanBeHarmful(combatant))
			{
				MovingParticles(combatant, 0x36FA, 1, 0, false, false, 1108, 0, 9533, 1, 0, (EffectLayer)255, 0x100);
				MovingParticles(combatant, 0x0001, 1, 0, false, true, 1108, 0, 9533, 9534, 0, (EffectLayer)255, 0);
				PlaySound(0x1FB);

				Timer.DelayCall(TimeSpan.FromSeconds(1.0), DoFocusedLeech_Stage2, combatant);
			}
		}

		private void DoFocusedLeech_Stage2(Mobile combatant)
		{
			if (CanBeHarmful(combatant))
			{
				combatant.MovingParticles(this, 0x36F4, 1, 0, false, false, 32, 0, 9535, 1, 0, (EffectLayer)255, 0x100);
				combatant.MovingParticles(this, 0x0001, 1, 0, false, true, 32, 0, 9535, 9536, 0, (EffectLayer)255, 0);

				PlaySound(0x209);
				DoHarmful(combatant);
				int damageDone = Utility.RandomMinMax(20, 30);
				combatant.Damage(damageDone, this);
				Hits += damageDone;
			}
		}

		public override void OnThink()
		{
			if (DateTime.UtcNow >= m_NextAbilityTime)
			{
				Mobile combatant = Combatant;

				if (combatant != null && combatant.Map == Map && combatant.InRange(this, 12))
				{
					m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15));

					int ability = Utility.Random(4);

					switch (ability)
					{
						case 0:
							DoFocusedLeech(combatant, "Thine essence will fill my withering body with strength!");
							break;
						case 1:
							DoFocusedLeech(combatant, "I rebuke thee, worm, and cleanse thy vile spirit of its tainted blood!");
							break;
						case 2:
							DoFocusedLeech(combatant, "I devour your life's essence to strengthen my resolve!");
							break;
						case 3:
							DoAreaLeech();
							break;
							// TODO: Resurrect ability
					}
				}
			}

			base.OnThink();
		}

		public MeerEternal(Serial serial)
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