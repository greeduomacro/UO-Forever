#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Accounting;
using Server.Engines.ZombieEvent;
using Server.Factions;
using Server.Games;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using VitaNex.Modules.AutoPvP;

#endregion

namespace Server.Misc
{
    public enum Stat
    {
        Str,
        Dex,
        Int
    }

    public class SkillCheck
    {
        //Change this to false to disable anti-macro code
        //private static readonly bool AntiMacroCode = false;		

        //How long do we remember targets/locations?
        public static TimeSpan AntiMacroExpire = TimeSpan.FromMinutes(5.0);

        //How many times may we use the same location/target for gain
        public const int Allowance = 3;

        //The size of each location, make this smaller so players dont have to move as far
        private const int LocationSize = 5;

        // true if this skill uses the anti-macro code, false if it does not
        private static bool[] UseAntiMacro = new[]
        {
            false, // Alchemy = 0,
            false, // Anatomy = 1,
            false, // AnimalLore = 2,
            false, // ItemID = 3,
            false, // ArmsLore = 4,
            false, // Parry = 5,
            true, // Begging = 6,
            false, // Blacksmith = 7,
            false, // Fletching = 8,
            true, // Peacemaking = 9,
            true, // Camping = 10,
            false, // Carpentry = 11,
            false, // Cartography = 12,
            false, // Cooking = 13,
            true, // DetectHidden = 14,
            true, // Discordance = 15,
            true, // EvalInt = 16,
            true, // Healing = 17,
            true, // Fishing = 18,
            true, // Forensics = 19,
            true, // Herding = 20,
            true, // Hiding = 21,
            true, // Provocation = 22,
            false, // Inscribe = 23,
            true, // Lockpicking = 24,
            true, // Magery = 25,
            true, // MagicResist = 26,
            false, // Tactics = 27,
            true, // Snooping = 28,
            true, // Musicianship = 29,
            true, // Poisoning = 30,
            false, // Archery = 31,
            true, // SpiritSpeak = 32,
            true, // Stealing = 33,
            false, // Tailoring = 34,
            true, // AnimalTaming = 35,
            true, // TasteID = 36,
            false, // Tinkering = 37,
            true, // Tracking = 38,
            true, // Veterinary = 39,
            false, // Swords = 40,
            false, // Macing = 41,
            false, // Fencing = 42,
            false, // Wrestling = 43,
            true, // Lumberjacking = 44,
            true, // Mining = 45,
            true, // Meditation = 46,
            true, // Stealth = 47,
            true, // RemoveTrap = 48,
            true, // Necromancy = 49,
            false, // Focus = 50,
            true, // Chivalry = 51
            true, // Bushido = 52
            true, // Ninjitsu = 53
            true, // Spellweaving = 54
            true, // Mysticism = 55
            true, // Imbuing = 56
            false // Throwing = 57
        };

        public static void Initialize()
        {
            Mobile.SkillCheckLocationHandler = Mobile_SkillCheckLocation;
            Mobile.SkillCheckDirectLocationHandler = Mobile_SkillCheckDirectLocation;

            Mobile.SkillCheckTargetHandler = Mobile_SkillCheckTarget;
            Mobile.SkillCheckDirectTargetHandler = Mobile_SkillCheckDirectTarget;

            // Begin mod to enable XmlSpawner skill triggering
            Mobile.SkillCheckLocationHandler = XmlSpawnerSkillCheck.Mobile_SkillCheckLocation;
            Mobile.SkillCheckDirectLocationHandler = XmlSpawnerSkillCheck.Mobile_SkillCheckDirectLocation;
            Mobile.SkillCheckTargetHandler = XmlSpawnerSkillCheck.Mobile_SkillCheckTarget;
            Mobile.SkillCheckDirectTargetHandler = XmlSpawnerSkillCheck.Mobile_SkillCheckDirectTarget;
            // End mod to enable XmlSpawner skill triggering

            SetDifficulty();
        }

        public static void SetDifficulty()
        {
            //Lower number is FASTER
            SkillInfo.Table[0].GainFactor = .75; // Alchemy
            SkillInfo.Table[1].GainFactor = .50; // Anatomy
            SkillInfo.Table[3].GainFactor = 0.25; // ItemID
            SkillInfo.Table[9].GainFactor = 0.50; // Peacemaking
            SkillInfo.Table[15].GainFactor = 0.50; // Discordance
            SkillInfo.Table[16].GainFactor = 0.25; // Eval Int
            SkillInfo.Table[17].GainFactor = 0.25; // Healing
            SkillInfo.Table[18].GainFactor = 0.30; // Camping
            SkillInfo.Table[19].GainFactor = 0.50; // Forensic Eval
            SkillInfo.Table[20].GainFactor = 0.50; // Herding
            SkillInfo.Table[22].GainFactor = 0.50; // Provocation
            SkillInfo.Table[23].GainFactor = 0.75; // inscribe
            SkillInfo.Table[24].GainFactor = 0.60; // Lock Picking
            SkillInfo.Table[25].GainFactor = 0.25; // Magery
            SkillInfo.Table[26].GainFactor = 0.25; // Resisting Spells
            SkillInfo.Table[29].GainFactor = 0.50; // Musicianship
            //SkillInfo.Table[30].GainFactor = 0.75; // Poisoning
            SkillInfo.Table[32].GainFactor = 0.50; // Spirit Speak
            SkillInfo.Table[33].GainFactor = 0.50; // Stealing
            SkillInfo.Table[35].GainFactor = 0.90; // Animal Taming
            SkillInfo.Table[38].GainFactor = 0.50; // Tracking
            SkillInfo.Table[48].GainFactor = 0.50; // Remove Trap

            //Adjusting Melee skills (NOT PARRYING)
            SkillInfo.Table[27].GainFactor = 0.25; // Tactics
            SkillInfo.Table[40].GainFactor = 0.25; // Swordsmanship
            SkillInfo.Table[41].GainFactor = 0.25; // Mace Fighting
            SkillInfo.Table[42].GainFactor = 0.25; // Fencing
            SkillInfo.Table[43].GainFactor = 0.25; // Wrestling
        }

        private static readonly SkillName[] m_CraftSupportSkills = new[]
        {
            SkillName.Mining, SkillName.Lumberjacking, SkillName.Tinkering, SkillName.TasteID, SkillName.Tailoring,
            SkillName.Poisoning,
            //	SkillName.Inscribe,
            SkillName.Fishing, SkillName.Cooking, SkillName.Carpentry, SkillName.Camping, SkillName.Fletching,
            SkillName.Blacksmith, SkillName.Begging, SkillName.ArmsLore, SkillName.ItemID,
            //	SkillName.Alchemy,
            SkillName.Cartography, SkillName.RemoveTrap
        };

        public static bool CanBePenalized(SkillName skillname)
        {
            return m_CraftSupportSkills.All(t => t != skillname);
        }

        public static bool Mobile_SkillCheckLocation(Mobile from, SkillName skillName, double minSkill, double maxSkill)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
            {
                return false;
            }

            double value = skill.Value;

            if (value < skill.Cap) //This allows players to gain stats still
            {
                if (value < minSkill)
                {
                    return false; // Too difficult
                }

                if (value >= maxSkill)
                {
                    return true; // No challenge
                }
            }

            double chance = (value - minSkill) / (maxSkill - minSkill);

            var loc = new Point2D(from.Location.X / LocationSize, from.Location.Y / LocationSize);
            return CheckSkill(from, skill, loc, chance);
        }

        public static bool Mobile_SkillCheckDirectLocation(Mobile from, SkillName skillName, double chance)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
            {
                return false;
            }

            double value = skill.Value;

            if (value < skill.Cap) //This allows players to gain stats still
            {
                if (chance < 0.0)
                {
                    return false; // Too difficult
                }

                if (chance >= 1.0)
                {
                    return true; // No challenge
                }
            }

            var loc = new Point2D(from.Location.X / LocationSize, from.Location.Y / LocationSize);
            return CheckSkill(from, skill, loc, chance);
        }

        public static bool CheckSkill(Mobile from, Skill skill, object amObj, double chance)
        {
            return CheckSkill(from, skill, amObj, chance, chance >= Utility.RandomDouble());
        }

        public static bool CheckSkill(Mobile from, Skill skill, object amObj, double chance, bool success)
        {
            if (from.Alive && AllowGain(from /*, skill, amObj*/))
            {
                if (from.Skills.Cap > 0 && skill.Base < skill.Cap)
                {
                    if (skill.Base < 50.0)
                    {
                        Gain(from, skill);
                    }
                    else
                    {
                        double thirst = 20.0 - from.Thirst;
                        double hunger = 20.0 - from.Hunger;

                        double gc = 50.0;

                        if (skill.BaseFixedPoint >= 1200)
                        {
                            gc = 44;
                        }
                        else if (skill.BaseFixedPoint >= 1150)
                        {
                            gc = 42;
                        }
                        else if (skill.BaseFixedPoint >= 1100)
                        {
                            gc = 39;
                        }
                        else if (skill.BaseFixedPoint >= 1050)
                        {
                            gc = 37;
                        }
                        else if (skill.BaseFixedPoint >= 1000)
                        {
                            gc = 30;
                        }
                        else if (skill.BaseFixedPoint >= 900)
                        {
                            gc = 25;
                        }
                        else if (skill.BaseFixedPoint >= 800)
                        {
                            gc = 20;
                        }
                        else if (skill.BaseFixedPoint >= 700)
                        {
                            gc = 15;
                        }
                        else if (skill.BaseFixedPoint >= 600)
                        {
                            gc = 10;
                        }
                        else if (skill.BaseFixedPoint >= 500)
                        {
                            gc = 5;
                        }
                        else
                        {
                            gc = 2;
                        }

                        double gainmod = 0.0;

                        if (from is PlayerMobile)
                        {
                            gainmod = ((PlayerMobile) from).GetSkillGainModBonus(skill.SkillName);
                        }

                        double malus = skill.Info.GainFactor + (0.08 * (thirst / 20.0)) + (0.08 * (hunger / 20.0)) +
                                       (0.09 * (from.BAC / 60.0)) - gainmod;
                            // + (0.25 * (from.Skills.Total / from.Skills.Cap));

                        gc *= malus;

                        if (skill.Info.UsesDifficulty) // Base gain on success rate
                        {
                            if (success)
                            {
                                if (chance <= 0.65)
                                {
                                    gc /= 1.65 - chance; // changed to 2
                                }
                            }
                            else if (chance < 0.35)
                            {
                                gc *= 5.0 - chance; // chanced to 5
                            }
                        }

                        /*
						 * NO CHANGE in towns
                         * -25% in houses or boats
						 * +50% dungeons (not despise)
						 */

                        //GuardedRegion town = from.Region.GetRegion(typeof(GuardedRegion)) as GuardedRegion;

                        // NOTE currently CanBePenalized returns FALSE for crafting skills... that seems wrong
                        // i'm just using the CreaturePossession function here, no relation to pseudoseer system
                        if (CreaturePossession.IsInHouseOrBoat(from.Location, from.Map))
                        {
                            gc *= 1.25; // -25%
                        }
                        //if ( CanBePenalized( skill.SkillName ) && town != null && !town.IsDisabled() )
                        //	gc *= 2.25;
                        //else
                        //{

                        var dungeon = from.Region.GetRegion(typeof(DungeonRegion)) as DungeonRegion;
                        if (dungeon != null)
                        {
                            gc *= 0.50;
                        }
                        //}

                        if (FactionObelisks.Obelisks != null && from is PlayerMobile)
                        {
                            var p = from as PlayerMobile;
                            var acct = p.Account as Account;
                            foreach (var obelisk in FactionObelisks.Obelisks)
                            {
                                if (obelisk.ObeliskType == ObeliskType.SkillGain && !String.IsNullOrEmpty(obelisk.OwningFaction) && acct != null)
                                {
                                    if (obelisk.ObeliskUsers != null && obelisk.ObeliskUsers.ContainsKey(acct))
                                    {
                                            gc *= 0.95;
                                            break;
                                    }
                                }
                            }
                        }

                        if (from is ZombieAvatar)
                        {
                            if (skill.SkillName == SkillName.Archery || skill.SkillName == SkillName.Fencing ||
                                skill.SkillName == SkillName.Hiding || skill.SkillName == SkillName.Stealth || skill.SkillName == SkillName.Macing ||
                                skill.SkillName == SkillName.Parry || skill.SkillName == SkillName.Swords ||
                                skill.SkillName == SkillName.Tactics || skill.SkillName == SkillName.Anatomy ||
                                skill.SkillName == SkillName.Magery || skill.SkillName == SkillName.EvalInt)
                            {
                                gc *= ZombieEvent.CSOptions.MeleeMod;
                            }
                        }

                        if (gc > 60)
                        {
                            gc = 30;
                        }

                        var bc = from as BaseCreature;

                        /*if (bc != null)
						{
							if (bc.Controlled)
							{
								if (bc.IsBonded)
								{
									gc /= 1.5;
								}
							}

							gc /= bc.SkillGainMultiplier; // can customize how well they gain
						}*/

                        // Dynamic control of all skill gain
                        gc /= DynamicSettingsController.SkillGainMultiplier;

                        //from.SendMessage( "Gain Chance: 1/{0}", gc );
                        if ((1.0 / gc) > Utility.RandomDouble())
                        {
                            Gain(from, skill);
                        }
                        if (0.025 > Utility.RandomDouble())
                        {
                            GainStat(from, skill);
                        }
                    }
                }
                else if (success && 0.25 > Utility.RandomDouble())
                {
                    GainStat(from, skill);
                }
            }

            return success;
        }

        public static bool Mobile_SkillCheckTarget(
            Mobile from, SkillName skillName, object target, double minSkill, double maxSkill)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
            {
                return false;
            }

            double value = skill.Value;

            if (value < skill.Cap) //This allows players to gain stats still
            {
                if (value < minSkill)
                {
                    return false; // Too difficult
                }

                if (value >= maxSkill)
                {
                    return true; // No challenge
                }
            }

            double chance = (value - minSkill) / (maxSkill - minSkill);

            return CheckSkill(from, skill, target, chance);
        }

        public static bool Mobile_SkillCheckDirectTarget(Mobile from, SkillName skillName, object target, double chance)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
            {
                return false;
            }

            if (chance < 0.0)
            {
                return false; // Too difficult
            }

            if (chance >= 1.0)
            {
                return true; // No challenge
            }

            return CheckSkill(from, skill, target, chance);
        }

        private static bool AllowGain(Mobile from /*, Skill skill, object obj*/)
        {
            var pm = from as PlayerMobile;
            PvPBattle battle = AutoPvP.FindBattle(pm);
            if (Faction.InSkillLoss(from)) //Changed some time between the introduction of AoS and SE.
            {
                return false;
            }

            if (pm != null && pm.InStat)
            {
                return false;
            }

            if (pm != null && battle != null && battle.IsParticipant(pm))
            {
                return false;
            }

            if (from is PlayerMobile && PvPTemplates.PvPTemplates.FetchProfile(from as PlayerMobile).Active)
            {
                return false;
            }

            /*if (AntiMacroCode && from is PlayerMobile && UseAntiMacro[skill.Info.SkillID])
			{
				return ((PlayerMobile)from).AntiMacroCheck(skill, obj);
			}*/

            return true;
        }

        public static void Gain(Mobile from, Skill skill)
        {
            if (from.Region.IsPartOf(typeof(Jail)))
            {
                return;
            }

            var bc = from as BaseCreature;

            if (bc != null && bc.IsDeadPet)
            {
                return;
            }

            if (skill.SkillName == SkillName.Focus && bc != null)
            {
                return;
            }

            if (skill.Base < skill.Cap && skill.Lock == SkillLock.Up)
            {
                int toGain = 1;

                if (skill.Base < 25.0)
                {
                    toGain = Utility.Random(1, 3);
                }

                if (bc != null && bc.SkillGainMultiplier != 1.0)
                {
                    if (skill.Base < 25.0)
                    {
                        toGain = (int) Math.Round(bc.SkillGainMultiplier) * 2;
                    }
                    else if (skill.Base < 50.0)
                    {
                        toGain = (int) Math.Round(bc.SkillGainMultiplier);
                    }
                    else if (skill.Base < 75.0)
                    {
                        toGain = (int) Math.Round(bc.SkillGainMultiplier) / 2;
                    }
                    else
                    {
                        toGain = (int) Math.Round(bc.SkillGainMultiplier) / 4;
                    }
                    if (toGain < 1)
                    {
                        toGain = 1;
                    }
                }

                Skills skills = from.Skills;

                if (from.Player && (skills.Total / skills.Cap) >= Utility.RandomDouble())
                {
                    foreach (
                        Skill toLower in
                            skills.Where(
                                toLower =>
                                    toLower != skill && toLower.Lock == SkillLock.Down &&
                                    toLower.BaseFixedPoint >= toGain))
                    {
                        toLower.BaseFixedPoint -= toGain;
                        break;
                    }
                }

                #region Scroll of Alacrity

                var pm = from as PlayerMobile;

                if (pm != null && skill.SkillName == pm.AcceleratedSkill && pm.AcceleratedEnd > DateTime.UtcNow)
                {
                    toGain *= Utility.RandomMinMax(2, 5);
                }

                #endregion

                if (!from.Player || (skills.Total + toGain) <= skills.Cap)
                {
                    skill.BaseFixedPoint += toGain;
                }
            }
            GainStat(from, skill);
        }

        private static readonly TimeSpan m_StatGainDelay = TimeSpan.FromMinutes(0.5);
        private static readonly TimeSpan m_PetStatGainDelay = TimeSpan.FromMinutes(10.0);

        public static void GainStat(Mobile from, Skill skill)
        {
            if (skill.Lock == SkillLock.Up)
            {
                SkillInfo info = skill.Info;

                var stats = new List<Tuple<double, int, Stat>>(3);

                if (CanRaise(from, Stat.Str))
                {
                    stats.Add(new Tuple<double, int, Stat>(info.StrGain, from.RawStr, Stat.Str));
                }

                if (CanRaise(from, Stat.Dex))
                {
                    stats.Add(new Tuple<double, int, Stat>(info.DexGain, from.RawDex, Stat.Dex));
                }

                if (CanRaise(from, Stat.Int))
                {
                    stats.Add(new Tuple<double, int, Stat>(info.IntGain, from.RawInt, Stat.Int));
                }

                if (stats.Count > 0)
                {
                    //from.SendMessage( "Success: Can Gain Something!" );
                    stats.Sort(new StatGainComparer());

                    //bool nostat = true;

                    foreach (Tuple<double, int, Stat> t in stats)
                    {
                        GainStat(from, t);
                    }
                }
            }
        }

        public static void GainStat(Mobile from, Tuple<double, int, Stat> tuple)
        {
            //from.SendMessage( "Attempting to gain {0}", tuple.Item3 );
            if ((tuple.Item1 / 20.0) + (0.16 * (100.0 - tuple.Item2) / 100.0) > Utility.RandomDouble())
            {
                //from.SendMessage( "Success: Gain!" );
                GainStat(from, tuple.Item3);
            }
        }

        public static void GainStat(Mobile from, Stat stat)
        {
            var bc = from as BaseCreature;
            bool pet = bc != null && bc.Controlled;

            switch (stat)
            {
                case Stat.Str:
                {
                    if (pet)
                    {
                        if ((from.LastStrGain + m_PetStatGainDelay) >= DateTime.UtcNow)
                        {
                            return;
                        }
                    }
                    else if ((from.LastStrGain + m_StatGainDelay) >= DateTime.UtcNow)
                    {
                        return;
                    }

                    from.LastStrGain = DateTime.UtcNow;
                    break;
                }
                case Stat.Dex:
                {
                    if (pet)
                    {
                        if ((from.LastDexGain + m_PetStatGainDelay) >= DateTime.UtcNow)
                        {
                            return;
                        }
                    }
                    else if ((from.LastDexGain + m_StatGainDelay) >= DateTime.UtcNow)
                    {
                        return;
                    }

                    from.LastDexGain = DateTime.UtcNow;
                    break;
                }
                case Stat.Int:
                {
                    if (pet)
                    {
                        if ((from.LastIntGain + m_PetStatGainDelay) >= DateTime.UtcNow)
                        {
                            return;
                        }
                    }
                    else if ((from.LastIntGain + m_StatGainDelay) >= DateTime.UtcNow)
                    {
                        return;
                    }

                    from.LastIntGain = DateTime.UtcNow;
                    break;
                }
            }

            double chance = ((from.StatCap + 5) - from.RawStatTotal) / (double) from.StatCap;
            //from.SendMessage( "Chance to atrophy: {0}", chance );
            IncreaseStat(from, stat, chance >= Utility.RandomDouble());
        }

        public static bool CanLower(Mobile from, Stat stat)
        {
            switch (stat)
            {
                case Stat.Str:
                    return (from.StrLock == StatLockType.Down && from.RawStr > 10);
                case Stat.Dex:
                    return (from.DexLock == StatLockType.Down && from.RawDex > 10);
                case Stat.Int:
                    return (from.IntLock == StatLockType.Down && from.RawInt > 10);
            }

            return false;
        }

        public static bool CanRaise(Mobile from, Stat stat)
        {
            return CanRaise(from, stat, false);
        }

        public static bool CanRaise(Mobile from, Stat stat, bool totalstats)
        {
            if (totalstats && !(from is BaseCreature && ((BaseCreature) from).Controlled) &&
                from.RawStatTotal >= from.StatCap)
            {
                return false;
            }

            int statcap = 100 + (from.StatCap - 225);

            switch (stat)
            {
                case Stat.Str:
                    return (from.StrLock == StatLockType.Up && from.RawStr < statcap);
                case Stat.Dex:
                    return (from.DexLock == StatLockType.Up && from.RawDex < statcap);
                case Stat.Int:
                    return (from.IntLock == StatLockType.Up && from.RawInt < statcap);
            }

            return false;
        }

        public static void IncreaseStat(Mobile from, Stat stat, bool atrophy)
        {
            atrophy = atrophy || (from.RawStatTotal >= from.StatCap);

            switch (stat)
            {
                case Stat.Str:
                {
                    if (atrophy)
                    {
                        if (CanLower(from, Stat.Dex) && (from.RawDex < from.RawInt || !CanLower(from, Stat.Int)))
                        {
                            --from.RawDex;
                        }
                        else if (CanLower(from, Stat.Int))
                        {
                            --from.RawInt;
                        }
                    }

                    if (CanRaise(from, Stat.Str, true))
                    {
                        ++from.RawStr;
                    }

                    break;
                }
                case Stat.Dex:
                {
                    if (atrophy)
                    {
                        if (CanLower(from, Stat.Str) && (from.RawStr < from.RawInt || !CanLower(from, Stat.Int)))
                        {
                            --from.RawStr;
                        }
                        else if (CanLower(from, Stat.Int))
                        {
                            --from.RawInt;
                        }
                    }

                    if (CanRaise(from, Stat.Dex, true))
                    {
                        ++from.RawDex;
                    }

                    break;
                }
                case Stat.Int:
                {
                    if (atrophy)
                    {
                        if (CanLower(from, Stat.Str) && (from.RawStr < from.RawDex || !CanLower(from, Stat.Dex)))
                        {
                            --from.RawStr;
                        }
                        else if (CanLower(from, Stat.Dex))
                        {
                            --from.RawDex;
                        }
                    }

                    if (CanRaise(from, Stat.Int, true))
                    {
                        ++from.RawInt;
                    }

                    break;
                }
            }
        }
    }

    public class StatGainComparer : IComparer<Tuple<double, int, Stat>>
    {
        public int Compare(Tuple<double, int, Stat> x, Tuple<double, int, Stat> y)
        {
            if (x == y || (x == null && y == null))
            {
                return 0;
            }

            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }

            return x.Item1.CompareTo(y.Item1);
        }
    }
}