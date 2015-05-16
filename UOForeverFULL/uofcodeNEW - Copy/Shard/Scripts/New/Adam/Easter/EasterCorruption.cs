#region References

using System;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    public class EasterCorruption
    {
        /*public static Map[] Maps = {Map.Felucca, Map.Ilshenar};

        public static void Corrupt(BaseCreature bc)
        {
            if (bc.IsCorrupt || EasterEventController.Instance == null)
            {
                return;
            }

            SkillName[] skills = ((SkillName) 0).GetValues<SkillName>();

            bc.SpecialTitle = "[Corrupted]";

            int hue = Utility.RandomBool() ? 1266 : 1166;

            bc.Hue = hue;

            bc.SolidHueOverride = hue;

            bc.TitleHue = hue;

            bc.BloodHue = hue;

            if (bc.HitsMaxSeed >= 0)
            {
                bc.HitsMaxSeed = (int) (bc.HitsMaxSeed * EasterEventController.HitsBuff);
            }

            bc.RawStr = (int) (bc.RawStr * EasterEventController.StrBuff);
            bc.RawInt = (int) (bc.RawInt * EasterEventController.IntBuff);
            bc.RawDex = (int) (bc.RawDex * EasterEventController.DexBuff);

            bc.Hits = bc.HitsMax;
            bc.Mana = bc.ManaMax;
            bc.Stam = bc.StamMax;

            foreach (SkillName skill in skills)
            {
                bc.SetSkill(skill, 100.0, 120.0);
            }

            bc.PassiveSpeed /= EasterEventController.SpeedBuff;
            bc.ActiveSpeed /= EasterEventController.SpeedBuff;
            bc.CurrentSpeed = bc.PassiveSpeed;

            bc.DamageMin += EasterEventController.DamageBuff;
            bc.DamageMax += EasterEventController.DamageBuff;

            if (bc.Fame > 0)
            {
                bc.Fame = (int) (bc.Fame * EasterEventController.FameBuff);
            }

            if (bc.Fame > 32000)
            {
                bc.Fame = 32000;
            }

            if (bc.Karma == 0)
            {
                return;
            }

            bc.Karma = (int) (bc.Karma * EasterEventController.KarmaBuff);

            if (Math.Abs(bc.Karma) > 32000)
            {
                bc.Karma = 32000 * Math.Sign(bc.Karma);
            }
        }

        public static void Uncorrupt(BaseCreature bc)
        {
            if (!bc.IsCorrupt)
            {
                return;
            }

            bc.SpecialTitle = "";

            bc.Hue = 0;

            bc.TitleHue = 0;

            bc.SolidHueOverride = -1;

            if (bc.HitsMaxSeed >= 0)
            {
                bc.HitsMaxSeed = (int) (bc.HitsMaxSeed / EasterEventController.HitsBuff);
            }

            bc.RawStr = (int) (bc.RawStr / EasterEventController.StrBuff);
            bc.RawInt = (int) (bc.RawInt / EasterEventController.IntBuff);
            bc.RawDex = (int) (bc.RawDex / EasterEventController.DexBuff);

            bc.Hits = bc.HitsMax;
            bc.Mana = bc.ManaMax;
            bc.Stam = bc.StamMax;

            bc.PassiveSpeed *= EasterEventController.SpeedBuff;
            bc.ActiveSpeed *= EasterEventController.SpeedBuff;
            bc.CurrentSpeed = bc.PassiveSpeed;

            bc.DamageMin -= EasterEventController.DamageBuff;
            bc.DamageMax -= EasterEventController.DamageBuff;

            if (bc.Fame > 0)
            {
                bc.Fame = (int) (bc.Fame / EasterEventController.FameBuff);
            }

            if (bc.Karma != 0)
            {
                bc.Karma = (int) (bc.Karma / EasterEventController.KarmaBuff);
            }
        }

        public static bool CheckCorrupt(BaseCreature bc)
        {
            return CheckCorrupt(bc, bc.Location, bc.Map);
        }

        public static bool CheckCorrupt(BaseCreature bc, Point3D location, Map m)
        {
            if (bc is BaseEscortable || bc is BaseVendor)
                return false;
            if (!(bc is BaseChampion))
            {
                return EasterEventController.Instance != null && bc.HitsMax > 50 &&
                       EasterEventController.EasterCorruptChance >= Utility.RandomDouble();
            }
            else
            {
                return true;
            }
        }

        public static double EggChance(BaseCreature bc)
        {
            if (EasterEventController.Instance == null)
            {
                return 0;
            }

            if (bc is BaseChampion)
            {
                return EasterEventController.EasterChampDropChance;
            }

            if (bc.HitsMax <= 100)
            {
                return EasterEventController.EasterVeryEasyDropChance;
            }
            if (bc.HitsMax <= 200)
            {
                return EasterEventController.EasterEasyDropChance;
            }
            if (bc.HitsMax <= 500)
            {
                return EasterEventController.EasterMediumDropChance;
            }

            return EasterEventController.EasterHardDropChance;
        }*/
    }
}