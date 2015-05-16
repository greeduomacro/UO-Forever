using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.XmlSpawner2;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using VitaNex.FX;
using VitaNex.Network;

namespace Server.Items
{
    public class RepairKit : Item
    {
        private InternalTimer timer;

        [Constructable]
        public RepairKit()
			:base(7864)
        {
            Name = "a ship repair kit";
            Hue = 2127;
        }

        public RepairKit(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Mounted)
            {
                from.SendMessage(61, "You cannot use this repair kit while mounted.");
                return;
            }

            from.SendMessage(61, "What would you like to repair?");
            from.Target = new RepairTarget(this, from);
        }

        public void Animate(Mobile user)
        {
            if (user.Body.IsHuman)
            {
                user.Animate(9, 10, 1, true, true, 1);
            }
            else if (user.Body.IsMonster)
            {
                user.Animate(12, 7, 1, true, true, 1);
            }
        }

        private class RepairTarget : Target
        {
            private readonly RepairKit m_kit;
            private readonly Mobile m_from;
            public RepairTarget(RepairKit kit, Mobile from)
                : base(10, true, TargetFlags.None)
            {
                m_kit = kit;
                this.m_from = from;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_kit == null || m_kit.Deleted)
                {
                    return;
                }

                var t = targeted as Item;
                var p = targeted as IPoint3D;

                if (t != null )
                {
                    if (t is MainMast && from.InRange(t.Location, 2))
                    {
                        var mast = t as MainMast;
                        if (mast.SailsDurability < mast.MaxSailsDurability)
                        {
                            from.SendMessage(61, "You begin repairing the ship.");
                            m_kit.timer = new InternalTimer(m_from, from.Location, mast.Galleon, m_kit);
                            m_kit.timer.Start();
                        }
                        else
                        {
                            from.SendMessage(61, "This ship is already in full repair.");
                        }
                        return;
                    }
                    if (t is NewBaseBoat && from.InRange(t.Location, 5))
                    {
                        var baseboat = t as NewBaseBoat;
                        if (baseboat.HullDurability < baseboat.MaxHullDurability)
                        {
                            from.SendMessage(61, "You begin repairing the ship.");
                            m_kit.timer = new InternalTimer(m_from, from.Location, baseboat, m_kit);
                            m_kit.timer.Start();
                        }
                        else
                        {
                            from.SendMessage(61, "This ship is already in full repair.");
                        }
                        return;
                    }
                }

                if (p != null)
                {
                        foreach (Item item in from.Map.GetItemsInRange(new Point3D(p), 10))
                        {
                            if (item is MainMast && from.InRange(item.Location, 2))
                            {
                                var mast = item as MainMast;
                                if (mast.HullDurability < mast.MaxHullDurability)
                                {
                                    from.SendMessage(61, "You begin repairing the ship.");
                                    m_kit.timer = new InternalTimer(m_from, from.Location, mast.Galleon, m_kit);
                                    m_kit.timer.Start();
                                }
                                else
                                {
                                    from.SendMessage(61, "This ship is already in full repair.");
                                }
                                return;
                            }

                            if (item is NewBaseBoat && from.InRange(item.Location, 7))
                            {
                                var baseboat = item as NewBaseBoat;
                                if (baseboat.HullDurability < baseboat.MaxHullDurability)
                                {
                                    from.SendMessage(61, "You begin repairing the ship.");
                                    m_kit.timer = new InternalTimer(m_from, from.Location, baseboat, m_kit);
                                    m_kit.timer.Start();
                                }
                                else
                                {
                                    from.SendMessage(61, "This ship is already in full repair.");
                                }
                                return;
                            }
                        }
                }
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                from.SendLocalizedMessage(1076203); // Target out of range.
            }
        }

        private class InternalTimer : Timer
        {
            private Mobile m_user;
            private Point3D m_origination;
            private BaseShip m_boat;
            private RepairKit m_kit;

            public InternalTimer(Mobile user, Point3D origin, BaseShip boat, RepairKit kit)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_user = user;
                Priority = TimerPriority.OneSecond;
                m_origination = origin;
                m_boat = boat;
                m_kit = kit;
            }

            protected override void OnTick()
            {
                if (m_user.Location != m_origination || !m_user.Alive)
                {
                    m_user.SendMessage(61, "You have stopped repairing the ship.");
                    Stop();
                }
                else if (m_boat.HullDurability < m_boat.MaxHullDurability)
                {
                    m_kit.Animate(m_user);
                    m_boat.HullDurability += 1;
                }
                else
                {
                    m_user.SendMessage(61, "The ship has been fully repaired.");
                    Stop();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

    }
}