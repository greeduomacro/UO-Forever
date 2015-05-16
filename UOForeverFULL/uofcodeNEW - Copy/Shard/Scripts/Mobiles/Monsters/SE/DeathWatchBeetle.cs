#region References
using Server.Items;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a deathwatchbeetle corpse")]
	[TypeAlias("Server.Mobiles.DeathWatchBeetle")]
	public class DeathwatchBeetle : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.CrushingBlow;
		}

		public override string DefaultName { get { return "a deathwatch beetle"; } }

		[Constructable]
		public DeathwatchBeetle()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Body = 242;
			BaseSoundID = 0x4F0;

			SetStr(136, 160);
			SetDex(41, 52);
			SetInt(31, 40);

			SetHits(121, 145);
			SetMana(20);

			SetDamage(5, 10);

			SetSkill(SkillName.MagicResist, 50.1, 58.0);
			SetSkill(SkillName.Tactics, 67.1, 77.0);
			SetSkill(SkillName.Wrestling, 50.1, 60.0);
			SetSkill(SkillName.Anatomy, 30.1, 34.0);

			Fame = 1400;
			Karma = -1400;

			Tamable = false;
			MinTameSkill = 41.1;
			ControlSlots = 1;
		}

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			FightMode = EraML ? FightMode.Aggressor : FightMode.Closest;
		}

		public override int GetAngerSound()
		{
			return 0x4F3;
		}

		public override int GetIdleSound()
		{
			return 0x4F2;
		}

		public override int GetAttackSound()
		{
			return 0x4F1;
		}

		public override int GetHurtSound()
		{
			return 0x4F4;
		}

		public override int GetDeathSound()
		{
			return 0x4F0;
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.LowScrolls, 1);
			AddLoot(LootPack.Potions, 1);
			AddLoot(LootPack.Meager);
		}

		public override int Feathers { get { return 50; } }
		public override int Meat { get { return 1; } }

		public override void AlterMeleeDamageTo(Mobile to, ref int damage)
		{
			if (Utility.RandomBool() && (Mana > 14) && to != null)
			{
				damage = (damage + (damage / 2));
				to.SendLocalizedMessage(1060091); // You take extra damage from the crushing attack!
				to.PlaySound(0x1E1);
				to.FixedParticles(0x377A, 1, 32, 0x26da, 0, 0, 0);
				Mana -= 15;
			}
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			Mobile combatant = Combatant;

			if (combatant == null || combatant.Deleted || combatant.Map != Map || !InRange(combatant, 12) ||
				!CanBeHarmful(combatant) || !InLOS(combatant))
			{
				return;
			}

			if (Utility.Random(10) == 0)
			{
				PoisonAttack(combatant);
			}

			base.OnDamage(amount, from, willKill);
		}

		public void PoisonAttack(Mobile m)
		{
			DoHarmful(m);
			MovingParticles(m, 0x36D4, 1, 0, false, false, 0x3F, 0, 0x1F73, 1, 0, (EffectLayer)255, 0x100);
			m.ApplyPoison(this, Poison.Regular);
			m.SendLocalizedMessage(1070821, Name); // %s spits a poisonous substance at you!
		}

		public DeathwatchBeetle(Serial serial)
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
			int version = reader.ReadInt();
		}
	}
}