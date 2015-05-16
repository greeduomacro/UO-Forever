#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Network;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace Server.Items
{
    public class SandMine : Item
    {
        private SpawnTimer m_Timer;

        [Constructable]
        public SandMine() : base(0x11EA)
        {
            Movable = false;
            Name = "sand mine";

            Hue = 0;

            m_Timer = new SpawnTimer(this);
            m_Timer.Start();
        }

        public SandMine(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Timer = new SpawnTimer(this);
            m_Timer.Start();
        }

        private class SpawnTimer : Timer
        {
            private readonly SandMine m_Item;

            private int TicCount;

            public SpawnTimer(SandMine item)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.1))
            {
                Priority = TimerPriority.FiftyMS;

                m_Item = item;
            }

            protected override void OnTick()
            {
                TicCount++;
                if (m_Item.Deleted)
                {
                    return;
                }

                m_Item.Hue = m_Item.Hue == 0 ? 33 : 0;

                int count = m_Item.AcquireTargets(m_Item.Location, 2).Count;
                if (count >= 1)
                {
                    m_Item.PublicOverheadMessage(MessageType.Label, 34, true, "*DETONATES*");
                    BaseExplodeEffect e = ExplodeFX.Fire.CreateInstance(
                        m_Item.Location, m_Item.Map, 2, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                        {
                            foreach (Mobile mobile in m_Item.AcquireAllTargets(m_Item.Location, 2))
                            {
                                if (mobile is BaseCreature && mobile.IsControlled())
                                {
                                    mobile.Damage(500);
                                }
                                else if (mobile is PlayerMobile)
                                {
                                    mobile.Damage(40);
                                }
                            }
                            m_Item.Delete();
                            Stop();
                        });
                    e.Send();
                }

                if (TicCount == 300)
                {
                    m_Item.Delete();
                    Stop();
                }
            }
        }

        public List<Mobile> AcquireTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.Alive && !m.Hidden &&
                            (m.Player || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile && !m.IsDeadBondedPet)))
                    .ToList();
        }

        public List<Mobile> AcquireAllTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.Alive &&
                            (m.Player || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile && !m.IsDeadBondedPet)))
                    .ToList();
        }
    }
}