#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.Craft;
using Server.Engines.Quests.Haven;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace Server.Items
{
    public class NewFireableCannonAddonComponent : AddonComponent
    {
        public NewFireableCannonAddonComponent(int itemID) : base(itemID)
        {
            Name = "an operational cannon";
            LootType = LootType.Blessed;
        }

        public NewFireableCannonAddonComponent(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class NewFireableCannonAddon : BaseAddon
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                var deed = new NewFireableCannonDeed();
                return deed;
            }
        }

        private CannonDirection m_CannonDirection;

        [CommandProperty(AccessLevel.GameMaster)]
        public CannonDirection CannonDirection { get { return m_CannonDirection; } }

        private DateTime NextUse { get; set; }

        [Constructable]
        public NewFireableCannonAddon(CannonDirection direction)
        {
            m_CannonDirection = direction;

            switch (direction)
            {
                case CannonDirection.North:
                {
                    AddComponent(new NewFireableCannonAddonComponent(0xE8D), 0, 0, 0);
                    AddComponent(new NewFireableCannonAddonComponent(0xE8C), 0, 1, 0);
                    AddComponent(new NewFireableCannonAddonComponent(0xE8B), 0, 2, 0);

                    break;
                }
                case CannonDirection.East:
                {
                    AddComponent(new NewFireableCannonAddonComponent(0xE96), 0, 0, 0);
                    AddComponent(new NewFireableCannonAddonComponent(0xE95), -1, 0, 0);
                    AddComponent(new NewFireableCannonAddonComponent(0xE94), -2, 0, 0);

                    break;
                }
                case CannonDirection.South:
                {
                    AddComponent(new NewFireableCannonAddonComponent(0xE91), 0, 0, 0);
                    AddComponent(new NewFireableCannonAddonComponent(0xE92), 0, -1, 0);
                    AddComponent(new NewFireableCannonAddonComponent(0xE93), 0, -2, 0);

                    break;
                }
                default:
                {
                    AddComponent(new NewFireableCannonAddonComponent(0xE8E), 0, 0, 0);
                    AddComponent(new NewFireableCannonAddonComponent(0xE8F), 1, 0, 0);
                    AddComponent(new NewFireableCannonAddonComponent(0xE90), 2, 0, 0);

                    break;
                }
            }
        }

        public NewFireableCannonAddon(Serial serial) : base(serial)
        {}

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(Location, 2))
            {
                var cannonball = from.Backpack.FindItemByType<CannonBall2>();
                var gunpowder = from.Backpack.FindItemByType<Gunpowder2>();
                if (from.Backpack != null && cannonball != null && gunpowder != null)
                {
                    from.Target = new InternalTarget(this, cannonball, gunpowder);
                    if (DateTime.UtcNow > NextUse)
                    {
                        from.Target = new InternalTarget(this, cannonball, gunpowder);
                    }
                    else
                    {
                        from.SendMessage(54, "You must let the cannon cool down before you can use it again.");
                    }
                }
                else
                {
                    if (gunpowder == null && cannonball == null)
                    {
                        from.SendMessage(54, "You cannot fire a cannon without a cannonball and gunpowder!");
                    }
                    else if (cannonball == null)
                    {
                        from.SendMessage(54, "You cannot fire a cannon without a cannonball!");
                    }
                    else if (gunpowder == null)
                    {
                        from.SendMessage(54, "You cannot fire a cannon without gunpowder!");
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(1076766); // That is too far away.
            }
        }

        public void DoFireEffect(IPoint3D target)
        {
            Map map = Map;

            if (target == null || map == null)
            {
                return;
            }

            NextUse = DateTime.UtcNow + TimeSpan.FromMinutes(1);

            var startloc = new Point3D(Location);
            IPoint3D point = target;

            switch (CannonDirection)
            {
                case CannonDirection.South:
                {
                    Effects.SendLocationEffect(new Point3D(X, Y + 1, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                    break;
                }
                case CannonDirection.North:
                {
                    Effects.SendLocationEffect(new Point3D(X, Y - 1, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                    break;
                }
                case CannonDirection.East:
                {
                    Effects.SendLocationEffect(new Point3D(X + 1, Y, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                    break;
                }
                case CannonDirection.West:
                {
                    Effects.SendLocationEffect(new Point3D(X - 1, Y + 1, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                    break;
                }
            }
            Effects.PlaySound(target, map, Utility.RandomList(0x11B, 0x11C, 0x11D));

            var queue = new EffectQueue();
            queue.Deferred = false;

            queue.Add(
                new MovingEffectInfo(
                    startloc,
                    point,
                    Map,
                    3699,
                    0,
                    10,
                    EffectRender.Normal,
                    TimeSpan.FromSeconds(0.1),
                    () =>
                    {
                        for (int count = 8; count > 0; count--)
                        {
                            IPoint3D location = new Point3D(target.X + Utility.RandomMinMax(-1, 1),
                                target.Y + Utility.RandomMinMax(-1, 1), target.Z);
                            int effect = Utility.RandomList(0x36B0);
                            Effects.SendLocationEffect(location, map, effect, 25, 1);
                            Effects.PlaySound(target, map, Utility.RandomList(0x11B, 0x11C, 0x11D));
                        }
                        foreach (Mobile player in AcquireAllTargets(new Point3D(point), 3))
                        {
                            var damage = player.Hits * 0.4;
                            player.Damage((int)Math.Round(damage));
                        }
                    }));
            queue.Process();
        }

        public List<Mobile> AcquireAllTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.Alive &&
                            (m.Player ||
                             (m is BaseCreature && ((BaseCreature) m).GetMaster() is PlayerMobile && !m.IsDeadBondedPet)))
                    .ToList();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((int) m_CannonDirection);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_CannonDirection = (CannonDirection) reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            private readonly NewFireableCannonAddon m_Cannon;
            private readonly CannonBall2 _Cannonball;
            private readonly Gunpowder2 _Gunpowder;

            public InternalTarget(NewFireableCannonAddon cannon, CannonBall2 cannonball, Gunpowder2 gunpowder)
                : base(12, true, TargetFlags.None)
            {
                m_Cannon = cannon;
                _Cannonball = cannonball;
                _Gunpowder = gunpowder;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Cannon == null || m_Cannon.Deleted)
                {
                    return;
                }

                var p = targeted as IPoint3D;

                if (p == null)
                {
                    return;
                }

                if (from.InLOS(new Point3D(p)))
                {
                    if (!Utility.InRange(new Point3D(p), m_Cannon.Location, 2))
                    {
                        bool allow = false;

                        int x = p.X - m_Cannon.X;
                        int y = p.Y - m_Cannon.Y;

                        switch (m_Cannon.CannonDirection)
                        {
                            case CannonDirection.North:
                                if (y < 0 && Math.Abs(x) <= -y / 3)
                                {
                                    allow = true;
                                }

                                break;
                            case CannonDirection.East:
                                if (x > 0 && Math.Abs(y) <= x / 3)
                                {
                                    allow = true;
                                }

                                break;
                            case CannonDirection.South:
                                if (y > 0 && Math.Abs(x) <= y / 3)
                                {
                                    allow = true;
                                }

                                break;
                            case CannonDirection.West:
                                if (x < 0 && Math.Abs(y) <= -x / 3)
                                {
                                    allow = true;
                                }

                                break;
                        }

                        if (allow && Utility.InRange(new Point3D(p), m_Cannon.Location, 25))
                        {
                            m_Cannon.DoFireEffect(p);
                            _Cannonball.Delete();
                            _Gunpowder.Delete();
                        }
                        else
                        {
                            from.SendLocalizedMessage(1076203); // Target out of range.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1076215); // Cannon must be aimed farther away.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1049630); // You cannot see that target!
                }
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                from.SendLocalizedMessage(1076203); // Target out of range.
            }
        }
    }

    public class NewFireableCannonDeed : BaseAddonDeed, IRewardItem, IRewardOption
    {
        public override int LabelNumber { get { return 1076195; } } // A deed for a cannon

        public override BaseAddon Addon
        {
            get
            {
                var addon = new NewFireableCannonAddon(m_Direction);

                return addon;
            }
        }

        private CannonDirection m_Direction;
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public NewFireableCannonDeed() : base()
        {
            LootType = LootType.Blessed;
        }

        public NewFireableCannonDeed(Serial serial) : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                return;
            }

            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
            {
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((bool) m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int) CannonDirection.South, 1075386); // South
            list.Add((int) CannonDirection.East, 1075387); // East
            list.Add((int) CannonDirection.North, 1075389); // North
            list.Add((int) CannonDirection.West, 1075390); // West
        }

        public void OnOptionSelected(Mobile from, int option)
        {
            m_Direction = (CannonDirection) option;

            if (!Deleted)
            {
                base.OnDoubleClick(from);
            }
        }
    }
}