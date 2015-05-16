#region References
using System;
using System.Linq;

using Server.Games;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class Paragon
	{
		public static void RevertParagons()
		{
			if (PseudoSeerStone.ParagonRevertInHours == 0.0)
			{
				return;
			}

			TimeSpan timeBetweenReverts = TimeSpan.FromHours(PseudoSeerStone.ParagonRevertInHours);
			DateTime now = DateTime.UtcNow;

			foreach (BaseCreature bc in
				World.Mobiles.Values.OfType<BaseCreature>()
					 .Where(bc => bc.IsParagon)
					 .Where(bc => now - bc.CreationTime > timeBetweenReverts)
					 .Where(bc => bc.Aggressors.Count == 0 && bc.Aggressed.Count == 0))
			{
				bc.IsParagon = false;
			}
		}

		// Chance that a paragon will carry a paragon chest
		public static double ChestChance { get { return PseudoSeerStone.Instance == null ? 0.1 : PseudoSeerStone.Instance._ParagonChestChance; } }

		public static Map[] Maps = {Map.Felucca, Map.Ilshenar};

		public static Type[] Artifacts =
		{
			typeof(GoldBricks), typeof(PhillipsWoodenSteed), typeof(AlchemistsBauble),
			typeof(ArcticDeathDealer), typeof(BlazeOfDeath), typeof(BowOfTheJukaKing), typeof(BurglarsBandana),
			typeof(CavortingClub), typeof(EnchantedTitanLegBone), typeof(GwennosHarp), typeof(IolosLute), typeof(LunaLance),
			typeof(NightsKiss), typeof(NoxRangersHeavyCrossbow), typeof(OrcishVisage), typeof(PolarBearMask),
			typeof(ShieldOfInvulnerability), typeof(StaffOfPower), typeof(VioletCourage), typeof(HeartOfTheLion),
			typeof(WrathOfTheDryad), typeof(PixieSwatter), typeof(GlovesOfThePugilist)
		};

		// Paragon hue
		public static int Hue = 1157;

		// Buffs
		public static double HitsBuff = 5.0;
		public static double StrBuff = 1.05;
		public static double IntBuff = 1.20;
		public static double DexBuff = 1.20;
		public static double SkillsBuff = 1.20;
		public static double SpeedBuff = 1.20;
		public static double FameBuff = 1.40;
		public static double KarmaBuff = 1.40;
		public static int DamageBuff = 5;

		public static void Convert(BaseCreature bc)
		{
			if (bc.IsParagon)
			{
				return;
			}

			bc.SolidHueOverride = Hue;

			if (bc.HitsMaxSeed >= 0)
			{
				bc.HitsMaxSeed = (int)(bc.HitsMaxSeed * HitsBuff);
			}

			bc.RawStr = (int)(bc.RawStr * StrBuff);
			bc.RawInt = (int)(bc.RawInt * IntBuff);
			bc.RawDex = (int)(bc.RawDex * DexBuff);

			bc.Hits = bc.HitsMax;
			bc.Mana = bc.ManaMax;
			bc.Stam = bc.StamMax;

			foreach (Skill skill in bc.Skills.Where(skill => skill.Base > 0.0))
			{
				skill.Base *= SkillsBuff;
			}

			bc.PassiveSpeed /= SpeedBuff;
			bc.ActiveSpeed /= SpeedBuff;
			bc.CurrentSpeed = bc.PassiveSpeed;

			bc.DamageMin += DamageBuff;
			bc.DamageMax += DamageBuff;

			if (bc.Fame > 0)
			{
				bc.Fame = (int)(bc.Fame * FameBuff);
			}

			if (bc.Fame > 32000)
			{
				bc.Fame = 32000;
			}

			if (bc.Karma == 0)
			{
				return;
			}

			bc.Karma = (int)(bc.Karma * KarmaBuff);

			if (Math.Abs(bc.Karma) > 32000)
			{
				bc.Karma = 32000 * Math.Sign(bc.Karma);
			}
		}

		public static void UnConvert(BaseCreature bc)
		{
			if (!bc.IsParagon)
			{
				return;
			}

			bc.SolidHueOverride = 0;

			if (bc.HitsMaxSeed >= 0)
			{
				bc.HitsMaxSeed = (int)(bc.HitsMaxSeed / HitsBuff);
			}

			bc.RawStr = (int)(bc.RawStr / StrBuff);
			bc.RawInt = (int)(bc.RawInt / IntBuff);
			bc.RawDex = (int)(bc.RawDex / DexBuff);

			bc.Hits = bc.HitsMax;
			bc.Mana = bc.ManaMax;
			bc.Stam = bc.StamMax;

			foreach (Skill skill in bc.Skills.Where(skill => skill.Base > 0.0))
			{
				skill.Base /= SkillsBuff;
			}

			bc.PassiveSpeed *= SpeedBuff;
			bc.ActiveSpeed *= SpeedBuff;
			bc.CurrentSpeed = bc.PassiveSpeed;

			bc.DamageMin -= DamageBuff;
			bc.DamageMax -= DamageBuff;

			if (bc.Fame > 0)
			{
				bc.Fame = (int)(bc.Fame / FameBuff);
			}

			if (bc.Karma != 0)
			{
				bc.Karma = (int)(bc.Karma / KarmaBuff);
			}
		}

		public static bool CheckConvert(BaseCreature bc)
		{
			return CheckConvert(bc, bc.Location, bc.Map);
		}

		public static bool CheckConvert(BaseCreature bc, Point3D location, Map m)
		{
			if (Array.IndexOf(Maps, m) == -1)
			{
				return false;
			}

			if (bc is BaseChampion || bc is BaseVendor || bc is BaseEscortable || bc.IsParagon)
			{
				return false;
			}

			int fame = bc.Fame;

			if (fame < 3000 || bc.TreasureMapLevel <= 0) // ogre is 3000
			{
				return false;
			}

			if (fame > 30000)
			{
				fame = 30000;
			}

			double ratioOfMaxRange = (fame - 3000.0) / (27000.0);

			double chance;

			if (PseudoSeerStone.Instance == null)
			{
				chance = 1.0 / Math.Round(20.0 - fame / 3000.0); // ~5-10%
			}
			else
			{
				chance = PseudoSeerStone.Instance._ParagonMinChance +
						 ratioOfMaxRange * (PseudoSeerStone.Instance._ParagonMaxChance - PseudoSeerStone.Instance._ParagonMinChance);
			}

			return chance > Utility.RandomDouble();
		}

		public static bool CheckArtifactChance(Mobile m, BaseCreature bc)
		{
			if (bc == null || !bc.EraAOS)
			{
				return false;
			}

			double fame = bc.Fame;

			if (fame > 32000)
			{
				fame = 32000;
			}

			double chance = 1.0 /
							(Math.Max(10.0, 100.0 * (0.83 - Math.Round(Math.Log(Math.Round(fame / 6000.0, 3) + 0.001, 10), 3))) * 100.0 /
							 100.0);

			return chance > Utility.RandomDouble();
		}

		public static void GiveArtifactTo(Mobile m)
		{
			var item = (Item)Activator.CreateInstance(Artifacts[Utility.Random(Artifacts.Length)]);

			m.SendMessage(
				m.AddToBackpack(item)
					? "As a reward for slaying the mighty paragon, an artifact has been placed in your backpack."
					: "As your backpack is full, your reward for destroying the legendary paragon has been placed at your feet.");
		}
	}
}