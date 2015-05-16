#region References
using System;
using System.Linq;
#endregion

namespace Server.Mobiles
{
	[CorpseName("an energy vortex corpse")]
	public class EnergyVortex : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return Summoned; } }
		public override bool AlwaysMurderer { get { return Body == 164; } } // Or Llama vortices will appear gray.
		public override double DispelDifficulty { get { return 60.0; } }
		public override double DispelFocus { get { return 20.0; } }
		public override string DefaultName { get { return "an energy vortex"; } }

		public override double GetFightModeRanking(Mobile m, FightMode acqType, bool bPlayerOnly)
		{
			return (m.Int + m.Skills[SkillName.Magery].Value) / Math.Max(GetDistanceToSqrt(m), 1.0);
		}

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (EraSE)
			{
				SetHits(140);
				ControlSlots = 2;
			}
			else
			{
				SetHits(500);
				ControlSlots = 1;
			}
		}

		[Constructable]
		public EnergyVortex()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			if (0.02 > Utility.RandomDouble()) // Tested on OSI, but is this right? Who knows.
			{
				// Llama vortex!
				Body = 0xDC;
				Hue = 0x76;
			}
			else
			{
				Body = 164;
			}

			SetStr(200);
			SetDex(200);
			SetInt(100);

			SetHits(500);
			SetStam(250);
			SetMana(0);

			SetDamage(14, 17);

			SetSkill(SkillName.MagicResist, 99.9);
			SetSkill(SkillName.Tactics, 100.0);
			SetSkill(SkillName.Wrestling, 120.0);

			Fame = 0;
			Karma = 0;

			VirtualArmor = 40;
			ControlSlots = 1;
		}

		public override bool BleedImmune { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override Poison HitPoison { get { return 0.75 > Utility.RandomDouble() ? Poison.Deadly : Poison.Greater; } }
		public override int DefaultBloodHue { get { return -1; } }

		public override int GetAngerSound()
		{
			return 0x15;
		}

		public override int GetAttackSound()
		{
			return 0x28;
		}

		public override void OnThink()
		{
			if (EraSE && Summoned)
			{
				var list = GetMobilesInRange(5);

				var spirtsOrVortexes =
					list.OfType<BaseCreature>().Where(m => m.Summoned && (m is EnergyVortex || m is BladeSpirits)).ToList();

				list.Free();

				while (spirtsOrVortexes.Count > 6)
				{
					// TODO: Confim if it's the dispel with all the pretty effects or just a Deletion of it.

					int index = Utility.Random(spirtsOrVortexes.Count);

					Dispel(spirtsOrVortexes[index]);
					spirtsOrVortexes.RemoveAt(index);
				}
			}

			base.OnThink();
		}

		public EnergyVortex(Serial serial)
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

			if (BaseSoundID == 263)
			{
				BaseSoundID = 0;
			}
		}
	}
}