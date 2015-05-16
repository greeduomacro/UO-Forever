#region References
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a blade spirit corpse")]
	public class BladeSpirits : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return Summoned; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool IsHouseSummonable { get { return true; } }
		public override double DispelDifficulty { get { return 0.0; } }
		public override double DispelFocus { get { return 20.0; } }
		public override string DefaultName { get { return "a blade spirit"; } }

		public override double GetFightModeRanking(Mobile m, FightMode acqType, bool bPlayerOnly)
		{
			return (m.Str + m.Skills[SkillName.Tactics].Value) / Math.Max(GetDistanceToSqrt(m), 1.0);
		}

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (EraSE)
			{
				SetHits(160);
				ControlSlots = 2;
			}
			else
			{
				SetHits(80);
				ControlSlots = 1;
			}
		}

		[Constructable]
		public BladeSpirits()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.6)
		{
			Body = 574;

			SetStr(150);
			SetDex(150);
			SetInt(100);

			SetHits(80);
			SetStam(250);
			SetMana(0);

			SetDamage(10, 14);

			SetSkill(SkillName.MagicResist, 70.0);
			SetSkill(SkillName.Tactics, 90.0);
			SetSkill(SkillName.Wrestling, 90.0);

			Fame = 0;
			Karma = 0;

			VirtualArmor = 40;
			ControlSlots = 1;
		}

		public override bool BleedImmune { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override Poison HitPoison { get { return 0.75 > Utility.RandomDouble() ? Poison.Greater : Poison.Regular; } }
		public override int DefaultBloodHue { get { return -1; } }

		public override int GetAngerSound()
		{
			return 0x23A;
		}

		public override int GetAttackSound()
		{
			return 0x3B8;
		}

		public override int GetHurtSound()
		{
			return 0x23A;
		}

		public override void OnThink()
		{
			if (EraSE && Summoned)
			{
				IPooledEnumerable list = GetMobilesInRange(5);

				List<BaseCreature> spirtsOrVortexes =
					list.OfType<BaseCreature>().Where(m => m.Summoned && (m is EnergyVortex || m is BladeSpirits)).ToList();

				list.Free();

				while (spirtsOrVortexes.Count > 6)
				{
					int index = Utility.Random(spirtsOrVortexes.Count);

					Dispel(spirtsOrVortexes[index]);
					spirtsOrVortexes.RemoveAt(index);
				}
			}

			base.OnThink();
		}

		public BladeSpirits(Serial serial)
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
	}
}