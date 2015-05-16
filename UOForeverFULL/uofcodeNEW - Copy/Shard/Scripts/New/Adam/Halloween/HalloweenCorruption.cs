#region References

using System;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    public class HalloweenCorruption
    {
        public static Map[] Maps = {Map.Felucca, Map.Ilshenar};

        public static int Hue = Utility.RandomBool() ? 1358 : 1378;

        public static void Corrupt(BaseCreature bc)
        {
            if (bc.IsCorrupt || HalloweenEventController.Instance == null)
            {
                return;
            }

            SkillName[] skills = ((SkillName) 0).GetValues<SkillName>();

            bc.SpecialTitle = "[Spirit of Halloween]";

            int hue = Utility.RandomBool() ? 1358 : 1378;

            bc.Hue = hue;

            bc.SolidHueOverride = hue;

            bc.TitleHue = hue;

            bc.BloodHue = hue;

            if (bc.HitsMaxSeed >= 0)
            {
                bc.HitsMaxSeed = (int) (bc.HitsMaxSeed * HalloweenEventController.HitsBuff);
            }

            bc.RawStr = (int) (bc.RawStr * HalloweenEventController.StrBuff);
            bc.RawInt = (int) (bc.RawInt * HalloweenEventController.IntBuff);
            bc.RawDex = (int) (bc.RawDex * HalloweenEventController.DexBuff);

            bc.Hits = bc.HitsMax;
            bc.Mana = bc.ManaMax;
            bc.Stam = bc.StamMax;

            bc.Tamable = false;

            foreach (SkillName skill in skills)
            {
                bc.SetSkill(skill, 100.0, 120.0);
            }

            bc.PassiveSpeed /= HalloweenEventController.SpeedBuff;
            bc.ActiveSpeed /= HalloweenEventController.SpeedBuff;
            bc.CurrentSpeed = bc.PassiveSpeed;

            bc.DamageMin += HalloweenEventController.DamageBuff;
            bc.DamageMax += HalloweenEventController.DamageBuff;

            if (bc.Fame > 0)
            {
                bc.Fame = (int) (bc.Fame * HalloweenEventController.FameBuff);
            }

            if (bc.Fame > 32000)
            {
                bc.Fame = 32000;
            }

            if (bc.Karma == 0)
            {
                return;
            }

            bc.Karma = (int) (bc.Karma * HalloweenEventController.KarmaBuff);

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
                bc.HitsMaxSeed = (int) (bc.HitsMaxSeed / HalloweenEventController.HitsBuff);
            }

            bc.RawStr = (int) (bc.RawStr / HalloweenEventController.StrBuff);
            bc.RawInt = (int) (bc.RawInt / HalloweenEventController.IntBuff);
            bc.RawDex = (int) (bc.RawDex / HalloweenEventController.DexBuff);

            bc.Hits = bc.HitsMax;
            bc.Mana = bc.ManaMax;
            bc.Stam = bc.StamMax;

            bc.PassiveSpeed *= HalloweenEventController.SpeedBuff;
            bc.ActiveSpeed *= HalloweenEventController.SpeedBuff;
            bc.CurrentSpeed = bc.PassiveSpeed;

            bc.DamageMin -= HalloweenEventController.DamageBuff;
            bc.DamageMax -= HalloweenEventController.DamageBuff;

            if (bc.Fame > 0)
            {
                bc.Fame = (int) (bc.Fame / HalloweenEventController.FameBuff);
            }

            if (bc.Karma != 0)
            {
                bc.Karma = (int) (bc.Karma / HalloweenEventController.KarmaBuff);
            }
        }

        public static bool CheckCorrupt(BaseCreature bc)
        {
            return CheckCorrupt(bc, bc.Location, bc.Map);
        }

        public static bool CheckCorrupt(BaseCreature bc, Point3D location, Map m)
        {
            if (Array.IndexOf(Maps, m) == -1)
            {
                return false;
            }

            if (bc is BaseVendor || bc is BaseEscortable || bc.IsParagon || bc.IsCorrupt)
            {
                return false;
            }

            int fame = bc.Fame;

            if (fame < 3000 || bc.TreasureMapLevel <= 0) // ogre is 3000
            {
                return false;
            }

            return HalloweenEventController.Instance != null &&
                HalloweenEventController.HalloweenCorruptChance > Utility.RandomDouble();
        }
    }
}