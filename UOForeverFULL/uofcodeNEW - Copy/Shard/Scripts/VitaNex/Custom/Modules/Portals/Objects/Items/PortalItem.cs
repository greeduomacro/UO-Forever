#region References

using System;
using Server.Engines.CannedEvil;
using Server.Engines.Portals;
using Server.Factions;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Spells;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Items
{
    public class PortalItem : Item
    {
        private Point3D m_Target;
        private Map m_TargetMap;
        public PortalSerial _PortalSerial;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Target { get { return m_Target; } set { m_Target = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map TargetMap { get { return m_TargetMap; } set { m_TargetMap = value; } }

        public virtual bool ShowFeluccaWarning { get { return false; } }

        [Constructable]
        public PortalItem(Point3D target, Map targetMap)
            : base(0xF6C)
        {
            Name = "a mysterious gate";
            Movable = false;
            DoesNotDecay = true;
            Light = LightType.Circle300;
            Hue = 1161;

            m_Target = target;
            m_TargetMap = targetMap;
        }

        public PortalItem(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Player ||
                (from is BaseCreature && !from.Deleted && from.NetState != null &&
                 ((BaseCreature) from).Pseu_CanUseGates))
                // pseudoseer controlled basecreature
            {
                if (from.InRange(GetWorldLocation(), 1))
                {
                    CheckGate(from, 1);
                }
                else
                {
                    from.SendLocalizedMessage(500446); // That is too far away.
                }
            }
            else
            {
                from.SendMessage("You aren't allowed to use that.");
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Player ||
                (m is BaseCreature && !m.Deleted && m.NetState != null && ((BaseCreature) m).Pseu_CanUseGates))
                // pseudoseer controlled basecreature
            {
                CheckGate(m, 0);
            }

            return true;
        }

        public virtual void CheckGate(Mobile m, int range)
        {
            new DelayTimer(m, this, range).Start();
        }

        public virtual void OnGateUsed(Mobile m)
        {
            if (m == null || m.Deleted)
            {
                return;
            }

            Point3D newTarget;

            BaseHouse house = BaseHouse.FindHouseAt(m);

            if (house != null && !house.Public && !house.IsFriend(m))
            {
                newTarget = house.BanLocation;

                BaseCreature.TeleportPets(m, newTarget, m_TargetMap);
                m.MoveToWorld(newTarget, m_TargetMap);
            }
        }

        public virtual void UseGate(Mobile m)
        {
            //ClientFlags flags = m.NetState == null ? ClientFlags.None : m.NetState.Flags;
            Region region = Region.Find(m_Target, m_TargetMap);
            var champregion = region.GetRegion(typeof(ChampionSpawn)) as ChampionSpawnRegion;
            var customRegion = region as CustomRegion;

            if (Sigil.ExistsOn(m))
            {
                m.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (SpellHelper.CheckCombat(m))
            {
                m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
            }
            else if (m.Spell != null)
            {
                m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
            }
            else if (m.IsYoung())
            {
                m.SendMessage(54, "You are too young to go in such a dangerous place!"); // That location is blocked.
            }
            else if (m.Mounted)
            {
                m.SendMessage("Your mount refuses to go into the gate!");
            }
            else if (m_TargetMap != null && m_TargetMap != Map.Internal)
            {
                BaseCreature.TeleportPets(m, m_Target, m_TargetMap);

                m.MoveToWorld(m_Target, m_TargetMap);

                if (m.AccessLevel == AccessLevel.Player || !m.Hidden)
                {
                    m.PlaySound(0x1FE);
                }

                OnGateUsed(m);
            }
            else
            {
                m.SendMessage("This gate does not seem to go anywhere.");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Target);
            writer.Write(m_TargetMap);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Target = reader.ReadPoint3D();
            m_TargetMap = reader.ReadMap();

        }

        public virtual bool ValidateUse(Mobile from, bool message)
        {
            if (from.Deleted || Deleted)
            {
                return false;
            }

            if (from.Criminal)
            {
                from.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }

            if (from.Map != Map || !from.InRange(this, 1))
            {
                if (message)
                {
                    from.SendLocalizedMessage(500446); // That is too far away.
                }

                return false;
            }

            return true;
        }

        public virtual void BeginConfirmation(Mobile from)
        {
            if (IsInTown(from.Location, from.Map) && !IsInTown(m_Target, m_TargetMap) ||
                (from.Map != Map.Felucca && TargetMap == Map.Felucca && ShowFeluccaWarning))
            {
                if (from.AccessLevel == AccessLevel.Player || !from.Hidden)
                {
                    from.Send(new PlaySound(0x20E, from.Location));
                }
                from.CloseGump(typeof(PortalConfirmGump));
                from.SendGump(new PortalConfirmGump(from, this));
            }
            else
            {
                EndConfirmation(from);
            }
        }

        public virtual void EndConfirmation(Mobile from)
        {
            if (!ValidateUse(from, true))
            {
                return;
            }

            UseGate(from);
        }

        public virtual void DelayCallback(Mobile from, int range)
        {
            if (!ValidateUse(from, false) || !from.InRange(this, range))
            {
                return;
            }

            if (m_TargetMap != null)
            {
                BeginConfirmation(from);
            }
            else
            {
                from.SendMessage("This moongate does not seem to go anywhere.");
            }
        }

        public static bool IsInTown(Point3D p, Map map)
        {
            if (map == null)
            {
                return false;
            }

            var reg = (GuardedRegion) Region.Find(p, map).GetRegion(typeof(GuardedRegion));

            return (reg != null && !reg.IsDisabled());
        }

        private class DelayTimer : Timer
        {
            private readonly Mobile m_From;
            private readonly PortalItem m_Gate;
            private readonly int m_Range;

            public DelayTimer(Mobile from, PortalItem gate, int range)
                : base(TimeSpan.FromSeconds(1.0))
            {
                m_From = from;
                m_Gate = gate;
                m_Range = range;
            }

            protected override void OnTick()
            {
                m_Gate.DelayCallback(m_From, m_Range);
            }
        }
    }

    public class PortalConfirmGump : Gump
    {
        private readonly Mobile m_From;
        private readonly PortalItem m_Gate;

        public PortalConfirmGump(Mobile from, PortalItem gate)
            : base(gate.EraAOS ? 110 : 20, gate.EraAOS ? 100 : 30)
        {
            m_From = from;
            m_Gate = gate;

            if (gate.EraAOS)
            {
                Closable = false;

                AddPage(0);

                AddBackground(0, 0, 420, 280, 5054);

                AddImageTiled(10, 10, 400, 20, 2624);
                AddAlphaRegion(10, 10, 400, 20);

                AddHtmlLocalized(10, 10, 400, 20, 1062051, 30720, false, false); // Gate Warning

                AddImageTiled(10, 40, 400, 200, 2624);
                AddAlphaRegion(10, 40, 400, 200);

                if (from.Map != Map.Felucca && gate.TargetMap == Map.Felucca && gate.ShowFeluccaWarning)
                {
                    AddHtmlLocalized(10, 40, 400, 200, 1062050, 32512, false, true);
                    // This Gate goes to Felucca... Continue to enter the gate, Cancel to stay here
                }
                else
                {
                    AddHtmlLocalized(10, 40, 400, 200, 1062049, 32512, false, true);
                    // Dost thou wish to step into the moongate? Continue to enter the gate, Cancel to stay here
                }

                AddImageTiled(10, 250, 400, 20, 2624);
                AddAlphaRegion(10, 250, 400, 20);

                AddButton(10, 250, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 250, 170, 20, 1011036, 32767, false, false); // OKAY

                AddButton(210, 250, 4005, 4007, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(240, 250, 170, 20, 1011012, 32767, false, false); // CANCEL
            }
            else
            {
                AddPage(0);

                AddBackground(0, 0, 420, 400, 5054);
                AddBackground(10, 10, 400, 380, 3000);

                AddHtml(
                    20,
                    40,
                    380,
                    60,
                    @"Dost thou wish to step into the moongate? Continue to enter the gate, Cancel to stay here",
                    false,
                    false);

                AddHtmlLocalized(55, 110, 290, 20, 1011012, false, false); // CANCEL
                AddButton(20, 110, 4005, 4007, 0, GumpButtonType.Reply, 0);

                AddHtmlLocalized(55, 140, 290, 40, 1011011, false, false); // CONTINUE
                AddButton(20, 140, 4005, 4007, 1, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                m_Gate.EndConfirmation(m_From);
            }
        }
    }
}