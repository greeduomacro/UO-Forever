#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Accounting;
using Server.Ethics;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using VitaNex.IO;
using VitaNex.Items;

#endregion

namespace Server.Engines.ZombieEvent
{
    public static partial class ZombieEvent
    {
        public const AccessLevel Access = AccessLevel.EventMaster;

        public static ZombieEventOptions CSOptions { get; private set; }

        public static BinaryDataStore<ZombieInstanceSerial, ZombieInstance> ZombieEvents { get; private set; }
        public static BinaryDataStore<PlayerMobile, PlayerZombieProfile> PlayerProfiles { get; private set; }

        public static PlayerZombieProfile EnsureProfile(PlayerMobile pm)
        {
            PlayerZombieProfile p;

            if (!PlayerProfiles.TryGetValue(pm, out p))
            {
                PlayerProfiles.Add(pm, p = new PlayerZombieProfile(pm));
            }
            else if (p == null)
            {
                PlayerProfiles[pm] = p = new PlayerZombieProfile(pm);
            }

            return p;
        }

        public static ZombieInstance GetInstance(ZombieInstanceSerial uid)
        {
            if (uid != null)
            {
                if (ZombieEvents.ContainsKey(uid))
                {
                    return ZombieEvents[uid];
                }
            }
            return null;
        }

        public static List<PlayerZombieProfile> SortedProfiles()
        {
            List<PlayerZombieProfile> profiles = PlayerProfiles.Values.OrderByDescending(x => x.OverallScore).ToList();

            return profiles;
        }

        public static ZombieInstance GetInstance()
        {
            ZombieInstance instance = ZombieEvents.Values.FirstOrDefault(x => x.Status == ZombieEventStatus.Running);

            return instance;
        }

        public static ZombieInstance GetPausedInstance()
        {
            ZombieInstance instance = ZombieEvents.Values.FirstOrDefault(x => x.Status == ZombieEventStatus.Paused);

            return instance;
        }

        public static void CleanUpAvatars()
        {
            foreach (var profile in PlayerProfiles.Values.Where(profile => profile.ZombieAvatar != null))
            {
                profile.ZombieAvatar.Delete();
            }
        }

        public static void PauseEvent()
        {
            foreach (var profile in PlayerProfiles.Values.Where(profile => profile.ZombieAvatar != null))
            {
                profile.ZombieAvatar.CantWalk = true;
                profile.ZombieAvatar.Hidden = true;
                profile.ZombieAvatar.IgnoreMobiles = true;
                profile.ZombieAvatar.Blessed = true;

                if (profile.ZombieAvatar.NetState != null)
                {
                    profile.ZombieAvatar.NetState.Dispose();
                }
            }
            var instance = GetInstance();
            if (instance != null)
                instance.Pause();
        }

        public static void UnpauseEvent()
        {
            var instance = GetPausedInstance();
            if (instance != null)
                instance.Unpause();
        }

        public static int GetParticipantCount()
        {
            return NetState.Instances.Count(state => state.Mobile is ZombieAvatar && (state.Mobile).Map == Map.ZombieLand);
        }

        public static int AsHue(this ZombieEventStatus status)
        {
            switch (status)
            {
                case ZombieEventStatus.Paused:
                    return 1258;
                case ZombieEventStatus.Running:
                    return 63;
                case ZombieEventStatus.Finished:
                    return 137;
                default:
                    return 0;
            }
        }

        #region Zombie Event Loot
        public static void AddItem(Container c)
        {
            int items = 0;
            double roll = Utility.RandomDouble();

            if (roll > 0.95)
            {
                items = 5;
            }
            else if (roll > 0.7)
            {
                items = 3;
            }
            else if (roll > 0.3)
            {
                items = 2;
            }
            else
            {
                items = 1;
            }

            for (int i = 0; i < items; i++)
            {
                roll = Utility.RandomDouble();
                if (roll > 0.4) // random utility item
                {
                    // potions, bandages, other basic resources
                    if (roll > 0.85)
                    {
                        c.DropItem(new Bandage {Amount = Utility.Random(3, 10)});
                    } // 15% chance of bandages
                    else if (roll > 0.75) // 10% chance for a potion
                    {
                        var PotionTypes = new[]
                        {
                            typeof(AgilityPotion), typeof(GreaterAgilityPotion), typeof(StrengthPotion),
                            typeof(GreaterStrengthPotion), typeof(RefreshPotion), typeof(TotalRefreshPotion),
                            typeof(LesserCurePotion), typeof(GreaterCurePotion), typeof(LesserHealPotion),
                            typeof(GreaterHealPotion), typeof(LesserPoisonPotion), typeof(PoisonPotion),
                            typeof(GreaterPoisonPotion)
                        };
                        c.DropItem(Loot.Construct(PotionTypes));
                    }
                    else if (roll > 0.55) // 20% chance for reagents
                    {
                        Item reags = Loot.RandomReagent();
                        reags.Amount = Utility.Random(3, 21);
                        c.DropItem(reags);
                    }
                    else if (roll > 0.5) // 5% chance for bag of some kind
                    {
                        var BagTypes = new[]
                        {
                            typeof(TPouch), typeof(Pouch), typeof(Bag),
                            typeof(Backpack), typeof(CacheChest)
                        };
                        c.DropItem(Loot.Construct(BagTypes));
                    }
                    else if (roll > 0.4) // chance for random item (bowl needed to mix in)
                    {
                        if (roll > 0.49)
                        {
                            c.DropItem(new Arrow(10));
                        }
                        else
                        {
                            var Garbage = new[]
                            {
                                typeof(ThrowawbleFootstool), typeof(ThrowawbleSpoon), typeof(ThrowawbleFork),
                                typeof(PewterBowlZombie), typeof(ThrowawblePlate), typeof(ZombieEventKnife), typeof(MortarPestleZombie)
                            };
                            c.DropItem(Loot.Construct(Garbage));
                        }
                    }
                }
                else if (roll > 0.3) // 10% chance for a weapon
                {
// ============= WEAPONS LIST ==========================================
                    BaseWeapon weapon = Loot.RandomWeapon();


                    double powerroll = Utility.RandomDouble();

                    if (powerroll > 0.20)
                    {
                        // normal
                    }
                    else if (powerroll > 0.12)
                    {
                        weapon.AccuracyLevel = WeaponAccuracyLevel.Accurate;
                        weapon.DamageLevel = WeaponDamageLevel.Ruin;
                    }
                    else if (powerroll > 0.06)
                    {
                        weapon.AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
                        weapon.DamageLevel = WeaponDamageLevel.Might;
                    }
                    else if (powerroll > 0.03)
                    {
                        weapon.AccuracyLevel = WeaponAccuracyLevel.Eminently;
                        weapon.DamageLevel = WeaponDamageLevel.Force;
                    }
                    else if (powerroll > 0.01)
                    {
                        weapon.AccuracyLevel = WeaponAccuracyLevel.Exceedingly;
                        weapon.DamageLevel = WeaponDamageLevel.Power;
                    }
                    else
                    {
                        weapon.AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
                        weapon.DamageLevel = WeaponDamageLevel.Vanq;
                    }
                    c.DropItem(weapon);

                    weapon.Speed = 1;

                    weapon.MaxHitPoints = 25;
                    weapon.HitPoints = 25;
                }
// ============= ARMOR LIST ==========================================
                else if (roll > 0.2) // 10% chance for a piece of armor
                {
                    BaseArmor armor = Loot.RandomArmorOrShield();
                    double powerRoll = Utility.RandomDouble();
                    armor.MaxHitPoints = 25;
                    armor.HitPoints = 25;

                    if (powerRoll > 0.20)
                    {
                        // normal
                    }
                    else if (powerRoll > 0.12)
                    {
                        armor.ProtectionLevel = ArmorProtectionLevel.Defense;
                    }
                    else if (powerRoll > 0.06)
                    {
                        armor.ProtectionLevel = ArmorProtectionLevel.Guarding;
                    }
                    else if (powerRoll > 0.03)
                    {
                        armor.ProtectionLevel = ArmorProtectionLevel.Hardening;
                    }
                    else if (powerRoll > 0.01)
                    {
                        armor.ProtectionLevel = ArmorProtectionLevel.Fortification;
                    }
                    else
                    {
                        armor.ProtectionLevel = ArmorProtectionLevel.Invulnerability;
                    }
                    c.DropItem(armor);
                }
                else if (roll > 0.1) // 10% chance for a tool of some kind
                {
                    Type[] types =
                    {
                        typeof(TinkerTools), typeof(Tongs), typeof(ZombieEventShovel),
                        typeof(Hammer), typeof(FletcherTools),
                        typeof(MortarPestle)
                    };
                    c.DropItem(Loot.Construct(types));
                }
                else if (roll > 0.02) // 8% chance for a scroll or spellbook
                {
                    if (roll > 0.09) // 1% chance (total) for spellbook
                    {
                        c.DropItem(new Spellbook {LootType = LootType.Regular});
                    }
                    else // spawn a scroll
                    {
                        Type[] types =
                        {
                            typeof(DispelFieldScroll), typeof(MagicReflectScroll), typeof(MindBlastScroll),
                            typeof(ParalyzeScroll), typeof(ClumsyScroll),
                            typeof(CreateFoodScroll), typeof(FeeblemindScroll), typeof(HealScroll),
                            typeof(MagicArrowScroll), typeof(ReactiveArmorScroll), typeof(WeakenScroll),
                            typeof(ArchCureScroll), typeof(ArchProtectionScroll), typeof(CurseScroll),
                            typeof(FireFieldScroll), typeof(GreaterHealScroll), typeof(LightningScroll),
                            typeof(ManaDrainScroll), typeof(AgilityScroll), typeof(CunningScroll), typeof(CureScroll),
                            typeof(HarmScroll), typeof(MagicTrapScroll), typeof(ProtectionScroll),
                            typeof(StrengthScroll), typeof(ChainLightningScroll), typeof(EnergyFieldScroll), typeof(ManaVampireScroll),
                            typeof(MeteorSwarmScroll), typeof(EnergyBoltScroll), typeof(ExplosionScroll), typeof(MassCurseScroll), typeof(ParalyzeFieldScroll),
                            typeof(BlessScroll), typeof(FireballScroll), typeof(PoisonScroll), typeof(TeleportScroll),
                            typeof(WallOfStoneScroll)
                        };
                        c.DropItem(Loot.Construct(types));
                    }
                }

                // always a 5% chance at an extra bonus pouch and magic trap scroll
                if (roll > 0.95)
                {
                    c.DropItem(new MagicTrapScroll());
                    c.DropItem(new TPouch());
                }
            }
        }
        #endregion

        #region swingeffect
        public static void ZombieSwingDirection(Mobile mob, Direction direction)
        {
            if (mob != null && mob.Weapon != null)
            {
                ZombieSwingDirection(mob, direction, mob.Weapon.MaxRange);
            }
        }

        public static void ZombieSwingDirection(Mobile mob, Direction direction, int range)
        {
            if (mob == null || mob.Weapon == null)
            {
                return;
            }

            IWeapon weapon = mob.Weapon;

            mob.Direction = direction; // don't do this b/c it kind of causes rubber band to the client

            UpdateNearbyClients(mob, direction);

            Point2D directionVector = Point2D.Zero;

            switch (direction & Direction.Mask)
            {
                case Direction.North:
                    directionVector = new Point2D(0, -1);
                    break;
                case Direction.Right:
                    directionVector = new Point2D(1, -1);
                    break;
                case Direction.East:
                    directionVector = new Point2D(1, 0);
                    break;
                case Direction.Down:
                    directionVector = new Point2D(1, 1);
                    break;
                case Direction.South:
                    directionVector = new Point2D(0, 1);
                    break;
                case Direction.Left:
                    directionVector = new Point2D(-1, 1);
                    break;
                case Direction.West:
                    directionVector = new Point2D(-1, 0);
                    break;
                case Direction.Up:
                    directionVector = new Point2D(-1, -1);
                    break;
            }

            var possibleTargets = new List<Mobile>();
            Point3D currentLoc = mob.Location;

            if (range <= 1 || directionVector == Point2D.Zero)
            {
                //IPooledEnumerable mobsOnHitSpot = mob.Map.GetMobilesInRange(new Point3D(currentLoc.X + directionVector.X, currentLoc.Y + directionVector.Y, mob.Location.Z));

                currentLoc.X += directionVector.X;
                currentLoc.Y += directionVector.Y;

                Sector newSector = mob.Map.GetSector(currentLoc);

                possibleTargets.AddRange(
                    newSector.Mobiles.Where(
                        m =>
                            (m.X == currentLoc.X && m.Y == currentLoc.Y && m != mob && mob.CanBeHarmful(m)) ||
                            m.X == mob.X && m.Y == mob.Y && m != mob));
            }
            else
            {
                for (int i = 0; i < range; i++)
                {
                    currentLoc.X += directionVector.X;
                    currentLoc.Y += directionVector.Y;

                    Sector newSector = mob.Map.GetSector(currentLoc);

                    possibleTargets.AddRange(
                        newSector.Mobiles.Where(
                            m =>
                                m.X == currentLoc.X && m.Y == currentLoc.Y && m != mob && mob.CanBeHarmful(m) &&
                                mob.InLOS(m)));

                    if (possibleTargets.Count > 0)
                    {
                        break; // we found our mark
                    }
                }
            }

            if (possibleTargets.Count > 0)
            {
                // TODO: maybe I should add a check for friends? (less likely to hit a friend?)
                Mobile target = possibleTargets[Utility.Random(possibleTargets.Count)];

                if (weapon is BaseRanged)
                {
                    var ranged = weapon as BaseRanged;
                    bool canSwing = ranged.CanSwing(mob, target);

                    if (mob is PlayerMobile)
                    {
                        var pm = (PlayerMobile) mob;

                        if (pm.DuelContext != null && !pm.DuelContext.CheckItemEquip(mob, ranged))
                        {
                            canSwing = false;
                        }
                    }

                    if (canSwing && mob.HarmfulCheck(target))
                    {
                        mob.DisruptiveAction();
                        mob.Send(new Swing(0, mob, target));

                        if (ranged.OnFired(mob, target))
                        {
                            if (ranged.CheckHit(mob, target))
                            {
                                ranged.OnHit(mob, target);
                            }
                            else
                            {
                                ranged.OnMiss(mob, target);
                            }
                        }
                    }

                    mob.RevealingAction();

                    //GetDelay(mob);
                }
                else
                {
                    weapon.OnSwing(mob, target);
                }
            }
            else
            {
                if (weapon is BaseRanged)
                {
                    if (((BaseRanged) weapon).OnFired(mob, null))
                    {
                        ZombieEffect(mob, ((BaseRanged) weapon).EffectID, 18, mob.X, mob.Y, mob.Z, currentLoc.X,
                            currentLoc.Y, currentLoc.Z, false, false);
                        Effects.PlaySound(mob, mob.Map, Utility.RandomMinMax(0x538, 0x53a));
                        ZombieSwingAnimation(mob);
                    }
                }
                else
                {
                    Effects.PlaySound(mob, mob.Map, Utility.RandomMinMax(0x538, 0x53a));
                    ZombieSwingAnimation(mob);
                }
            }
        }

        public static void UpdateNearbyClients(Mobile m, Direction d)
        {
            if (m == null)
            {
                return;
            }

            IPooledEnumerable eable = m.Map.GetClientsInRange(m.Location);

            foreach (NetState state in eable.OfType<NetState>().Where(ns => ns != m.NetState))
            {
                Mobile beholder = state.Mobile;

                if (state.StygianAbyss)
                {
                    int noto = Notoriety.Compute(beholder, m);

                    Packet p = Packet.Acquire(new MobileDirectionToOthersOnly(m, noto, d));

                    state.Send(p);
                }
                else
                {
                    int noto = Notoriety.Compute(beholder, m);

                    Packet p = Packet.Acquire(new MobileDirectionToOthersOnly(m, noto, d));

                    state.Send(p);
                }
            }

            eable.Free();
        }

        public static void ZombieSwingAnimation(Mobile mob)
        {
            if (mob == null || mob.Weapon == null || !(mob.Weapon is BaseWeapon))
            {
                return;
            }

            int action;
            WeaponAnimation animation = ((BaseWeapon) mob.Weapon).Animation;

            switch (mob.Body.Type)
            {
                case BodyType.Sea:
                case BodyType.Animal:
                    action = Utility.Random(5, 2);
                    break;
                case BodyType.Monster:
                {
                    switch (animation)
                    {
                        default:
                            action = Utility.Random(4, 3);
                            break;
                    }

                    break;
                }
                case BodyType.Human:
                {
                    if (!mob.Mounted)
                    {
                        action = (int) animation;
                    }
                    else
                    {
                        switch (animation)
                        {
                            default:
                                /* default case makes all these cases redundant
                                    case WeaponAnimation.Wrestle:
                                    case WeaponAnimation.Bash1H:
                                    case WeaponAnimation.Pierce1H:
                                    case WeaponAnimation.Slash1H:
                                    */
                                action = 26;
                                break;
                            case WeaponAnimation.Bash2H:
                            case WeaponAnimation.Pierce2H:
                            case WeaponAnimation.Slash2H:
                                action = 29;
                                break;
                            case WeaponAnimation.ShootBow:
                                action = 27;
                                break;
                            case WeaponAnimation.ShootXBow:
                                action = 28;
                                break;
                        }
                    }

                    break;
                }
                default:
                    return;
            }

            mob.Animate(action, 7, 1, true, false, 0);
        }


        public static void ZombieEffect(Mobile m,
            int effect,
            int speed,
            int x,
            int y,
            int z,
            int x2,
            int y2,
            int z2,
            bool fixedDirection,
            bool explosion)
        {
            if (effect <= 0)
            {
                return;
            }

            //syntax is MEFFECT,itemid,speed,x,y,z,x2,y2,z2
            int duration = 10;
            var eloc1 = new Point3D(x, y, z + 10);

            // offset by 10 because it always looks so bad going out of anythign--it drags along the floor
            var eloc2 = new Point3D(x2, y2, z2 + 10);

            Map emap = Map.Felucca;

            if (m != null)
            {
                emap = m.Map;
            }

            if (effect >= 0 && emap != Map.Internal)
            {
                // might want to implement those last booleans!
                Effects.SendPacket(
                    eloc1,
                    emap,
                    new HuedEffect(EffectType.Moving, -1, -1, effect, eloc1, eloc2, speed, duration, fixedDirection,
                        explosion, 0, 0));
            }
        }
        #endregion

        #region DeathAnimation
        public static void ZombieDeathAnim(Mobile mob, Corpse c)
        {
            if (mob == null || mob.Map == null)
            {
                return;
            }

            Packet animPacket = null; //new DeathAnimation( this, c );
            Packet remPacket = null; //this.RemovePacket;

            IPooledEnumerable eable = mob.Map.GetClientsInRange(mob.Location);

            foreach (NetState state in eable.OfType<NetState>().Where(state => state != mob.NetState))
            {
                if (animPacket == null)
                {
                    animPacket = Packet.Acquire(new DeathAnimation(mob, c));
                }

                state.Send(animPacket);

                if (state.Mobile.CanSee(mob))
                {
                    continue;
                }

                if (remPacket == null)
                {
                    remPacket = mob.RemovePacket;
                }

                state.Send(remPacket);
            }

            Packet.Release(animPacket);

            eable.Free();
        }
        #endregion

        public static void RandomClothing( Mobile mob)
        {
            if (mob == null)
            {
                return;
            }

            switch (Utility.Random(3))
            {
                case 0:
                    mob.AddItem(new FancyShirt(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                    break;
                case 1:
                    mob.AddItem(new Doublet(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                    break;
                case 2:
                    mob.AddItem(new Shirt(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                    break;
            }

            switch (Utility.Random(4))
            {
                case 0:
                    mob.AddItem(new Shoes(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                    break;
                case 1:
                    mob.AddItem(new Boots(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                    break;
                case 2:
                    mob.AddItem(new Sandals(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                    break;
                case 3:
                    mob.AddItem(new ThighBoots(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                    break;
            }

            if (mob.Female)
            {
                switch (Utility.Random(6))
                {
                    case 0:
                        mob.AddItem(new ShortPants(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                        break;
                    case 1:
                    case 2:
                        mob.AddItem(new Kilt(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                        break;
                    case 3:
                    case 4:
                    case 5:
                        mob.AddItem(new Skirt(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                        break;
                }
            }
            else
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        mob.AddItem(new LongPants(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                        break;
                    case 1:
                        mob.AddItem(new ShortPants(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
                        break;
                }
            }
        }
    }
}