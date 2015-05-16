using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Arcade
{
    public static class Arcade
    {
        #region swingeffect
        public static void SwingDirection(Mobile mob, Direction direction)
        {
            if (mob != null && mob.Weapon != null)
            {
                SwingDirection(mob, direction, mob.Weapon.MaxRange);
            }
        }

        public static void SwingDirection(Mobile mob, Direction direction, int range)
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
                        var pm = (PlayerMobile)mob;

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
                    if (((BaseRanged)weapon).OnFired(mob, null))
                    {
                        Effect(mob, ((BaseRanged)weapon).EffectID, 18, mob.X, mob.Y, mob.Z, currentLoc.X,
                            currentLoc.Y, currentLoc.Z, false, false);
                        Effects.PlaySound(mob, mob.Map, Utility.RandomMinMax(0x538, 0x53a));
                        SwingAnimation(mob);
                    }
                }
                else
                {
                    Effects.PlaySound(mob, mob.Map, Utility.RandomMinMax(0x538, 0x53a));
                    SwingAnimation(mob);
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

        public static void SwingAnimation(Mobile mob)
        {
            if (mob == null || mob.Weapon == null || !(mob.Weapon is BaseWeapon))
            {
                return;
            }

            int action;
            WeaponAnimation animation = ((BaseWeapon)mob.Weapon).Animation;

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
                            action = (int)animation;
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


        public static void Effect(Mobile m,
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
    }
}
