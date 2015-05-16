using System;
using Server.Multis;
using Server.Network;

namespace Server.Items {
    public class BoatTeleporter : Item {
        private bool m_Active, m_Creatures;
        private BaseBoat m_BoatDest;
        private bool m_SourceEffect;
        private bool m_DestEffect;
        private int m_SoundID;
        private TimeSpan m_Delay;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SourceEffect {
            get { return m_SourceEffect; }
            set { m_SourceEffect = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DestEffect {
            get { return m_DestEffect; }
            set { m_DestEffect = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SoundID {
            get { return m_SoundID; }
            set { m_SoundID = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Delay {
            get { return m_Delay; }
            set { m_Delay = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active {
            get { return m_Active; }
            set { m_Active = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat BoatDest {
            get { return m_BoatDest; }
            set { m_BoatDest = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Creatures {
            get { return m_Creatures; }
            set { m_Creatures = value; InvalidateProperties(); }
        }

        public override int LabelNumber { get { return 1026095; } } // teleporter

        [Constructable]
        public BoatTeleporter ()
            : this(false) {
        }

        [Constructable]
        public BoatTeleporter (bool creatures)
            : base(0x1BC3) {
            Movable = false;
            Visible = false;

            m_Active = true;
            m_Creatures = creatures;
        }

        public override void GetProperties (ObjectPropertyList list) {
            base.GetProperties(list);
            if (m_BoatDest != null)
                list.Add("Boat\t" + m_BoatDest.ShipName);
            if (m_Active)
                list.Add(1060742); // active
            else
                list.Add(1060743); // inactive

            list.Add(1060660, "Creatures\t{0}", m_Creatures ? "Yes" : "No");
        }

        public override void OnSingleClick (Mobile from) {
            base.OnSingleClick(from);
            if (m_BoatDest != null)
                LabelTo(from, m_BoatDest.ShipName);
            if (m_Active) {
                LabelTo(from, "(active)");
            } else {
                LabelTo(from, "(inactive)");
            }
        }

        public virtual void StartTeleport (Mobile m) {
            if (m_Delay == TimeSpan.Zero)
                DoTeleport(m);
            else
                Timer.DelayCall(m_Delay, new TimerStateCallback(DoTeleport_Callback), m);
        }

        private void DoTeleport_Callback (object state) {
            DoTeleport((Mobile)state);
        }

        public virtual void DoTeleport (Mobile m) {
            if (m_BoatDest == null) return;
            
            Map map = m_BoatDest.Map;

            if (map == null || map == Map.Internal) {
                m.SendLocalizedMessage(501942); // That location is blocked.
                return;
            }

            Point3D p = m_BoatDest.GetMarkedLocation();

            if (p == Point3D.Zero) {
                m.SendLocalizedMessage(501942); // That location is blocked.
                return;
            }

            Server.Mobiles.BaseCreature.TeleportPets(m, p, map);

            bool sendEffect = (!m.Hidden || m.AccessLevel == AccessLevel.Player);

            if (m_SourceEffect && sendEffect)
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            m.MoveToWorld(p, map);

            if (m_DestEffect && sendEffect)
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            if (m_SoundID > 0 && sendEffect)
                Effects.PlaySound(m.Location, m.Map, m_SoundID);
        }

        public override bool OnMoveOver (Mobile m) {
            if (m_Active) {
                if (!m_Creatures && !m.Player)
                    return true;

                StartTeleport(m);
                return false;
            }

            return true;
        }

        public BoatTeleporter (Serial serial)
            : base(serial) {
        }

        public override void Serialize (GenericWriter writer) {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_Active);
            writer.Write((Item)m_BoatDest);
            writer.Write(m_Creatures);
            writer.Write((bool)m_SourceEffect);
            writer.Write((bool)m_DestEffect);
            writer.Write((TimeSpan)m_Delay);
            writer.WriteEncodedInt((int)m_SoundID);
        }

        public override void Deserialize (GenericReader reader) {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Active = reader.ReadBool();
            m_BoatDest = (BaseBoat)reader.ReadItem();
            m_Creatures = reader.ReadBool();
            m_SourceEffect = reader.ReadBool();
            m_DestEffect = reader.ReadBool();
            m_Delay = reader.ReadTimeSpan();
            m_SoundID = reader.ReadEncodedInt();

        }

    }
}