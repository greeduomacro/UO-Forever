// **********
// RunUO Shard - PlayersHouseTeleporter.cs
// **********

#region References

using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Spells;

#endregion

namespace Server.Items
{
    public class PlayersHouseTeleporter : Item, IDyable, ISecurable


    {
        private bool m_Active, m_Creatures, m_CombatCheck;
        private Point3D m_PointDest;
        private Map m_MapDest;
        private bool m_SourceEffect;
        private bool m_DestEffect;
        private int m_SoundID;
        private TimeSpan m_Delay;
        private Mobile m_Owner;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SourceEffect
        {
            get { return m_SourceEffect; }
            set
            {
                m_SourceEffect = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DestEffect
        {
            get { return m_DestEffect; }
            set
            {
                m_DestEffect = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SoundID
        {
            get { return m_SoundID; }
            set
            {
                m_SoundID = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Delay
        {
            get { return m_Delay; }
            set
            {
                m_Delay = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                m_Active = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D PointDest
        {
            get { return m_PointDest; }
            set
            {
                m_PointDest = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map MapDest
        {
            get { return m_MapDest; }
            set
            {
                m_MapDest = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Creatures
        {
            get { return m_Creatures; }
            set
            {
                m_Creatures = value;
                InvalidateProperties();
            }
        }


        [CommandProperty(AccessLevel.GameMaster)]
        public bool CombatCheck
        {
            get { return m_CombatCheck; }
            set
            {
                m_CombatCheck = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        public override int LabelNumber
        {
            get { return 1026095; }
        } // teleporter

        [Constructable]
        public PlayersHouseTeleporter() : this(new Point3D(0, 0, 0), null, false)
        {
        }

        [Constructable]
        public PlayersHouseTeleporter(Point3D pointDest, Map mapDest) : this(pointDest, mapDest, false)
        {
        }

        [Constructable]
        public PlayersHouseTeleporter(Point3D pointDest, Map mapDest, bool creatures) : base(0x181D) //( 0xAD1 )
        {
            Movable = true;
            Visible = true;
            Hue = 1150;
            Name = "House Teleporter";

            m_Active = true;
            m_PointDest = pointDest;
            m_MapDest = mapDest;
            m_Creatures = creatures;

            m_CombatCheck = false;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Movable)
                from.SendMessage("This must be secured in a house to mark it!");
            else
            {
                if (m_Owner != null)
                {
                    from.SendMessage("This teleporter has been marked already.");
                }
                else
                {
                    m_Owner = from;
                    m_MapDest = Map;
                    m_PointDest = Location;
                    from.SendMessage(
                        "You have marked the teleporter.  You can now unlock it and move it to the house you wish to teleport from. It must be locked down in that house to use it.");
                }
            }
        }

        public virtual void StartTeleport(Mobile m)
        {
            if (m_Delay == TimeSpan.Zero)
                DoTeleport(m);
            else
                Timer.DelayCall(m_Delay, new TimerStateCallback(DoTeleport_Callback), m);
        }

        private void DoTeleport_Callback(object state)
        {
            DoTeleport((Mobile) state);
        }

        public virtual void DoTeleport(Mobile m)
        {
            Map map = m_MapDest;

            if (map == null || map == Map.Internal)
                map = m.Map;

            Point3D p = m_PointDest;

            if (p == Point3D.Zero)
                p = m.Location;

            BaseCreature.TeleportPets(m, p, map);

            bool sendEffect = (!m.Hidden || m.AccessLevel == AccessLevel.Player);

            if (m_SourceEffect && sendEffect)
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            m.MoveToWorld(p, map);

            if (m_DestEffect && sendEffect)
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            if (m_SoundID > 0 && sendEffect)
                Effects.PlaySound(m.Location, m.Map, m_SoundID);
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (Movable)
                m.SendMessage("This must be locked down in a house to use!");
            else if (m_Active)
            {
                if (!m_Creatures && !m.Player)
                    return true;
                if (m_CombatCheck && SpellHelper.CheckCombat(m))
                {
                    m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                    return true;
                }

                StartTeleport(m);
                return false;
            }

            return true;
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted) return false;
            Hue = sender.DyedHue;
            return true;
        }


        public PlayersHouseTeleporter(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(3); // version

            writer.Write(m_CombatCheck);

            writer.Write(m_SourceEffect);
            writer.Write(m_DestEffect);
            writer.Write(m_Delay);
            writer.WriteEncodedInt(m_SoundID);

            writer.Write(m_Creatures);

            writer.Write(m_Active);
            writer.Write(m_PointDest);
            writer.Write(m_MapDest);
            writer.Write(m_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                {
                    m_CombatCheck = reader.ReadBool();
                    goto case 2;
                }
                case 2:
                {
                    m_SourceEffect = reader.ReadBool();
                    m_DestEffect = reader.ReadBool();
                    m_Delay = reader.ReadTimeSpan();
                    m_SoundID = reader.ReadEncodedInt();

                    goto case 1;
                }
                case 1:
                {
                    m_Creatures = reader.ReadBool();

                    goto case 0;
                }
                case 0:
                {
                    m_Active = reader.ReadBool();
                    m_PointDest = reader.ReadPoint3D();
                    m_MapDest = reader.ReadMap();
                    m_Owner = reader.ReadMobile();

                    break;
                }
            }
        }
    }
}