using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    class MurderSystemController : Item
    {   
        private static MurderSystemController m_Instance;
        public static MurderSystemController Instance { get { return m_Instance; } }

        [Constructable]
        public MurderSystemController()
            : base(0xEDC)
        {
            this.Name = "Murder System Controller";
            this.Movable = false;
            this.Visible = false;

            if (m_Instance != null)
            {
                // there can only be one MurderSystemController game stone in the world
                m_Instance.Location = this.Location;
                Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.Administrator, 0x489,
                    "Existing MurderSystemController has been moved to this location (DON'T DELETE IT!).");
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback<MurderSystemController>(UpdateInstancePosition), this); 
            }
            else
                m_Instance = this;
        }

        public static void UpdateInstancePosition(MurderSystemController attemptedConstruct)
        {
            if (attemptedConstruct == null) return;
            if (m_Instance == null) // should never happen, but if it does, make this the instance
            {
                m_Instance = attemptedConstruct;
            }
            else if (attemptedConstruct.Location != new Point3D(0,0,0)) // move the instance to it's location and delete it
            {
                m_Instance.Location = attemptedConstruct.Location;
                attemptedConstruct.Delete();
            }
        }

        public MurderSystemController(Serial serial) : base(serial) { }

        public override void Delete()
        {
            return; // can't delete it!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2);//version

            // version 2
            writer.Write((bool)_YoungProtectionRegionsEnabled);

            // version 1
            writer.Write((int)_MurderYoungFreezeSeconds);

            // version 0
            //global attributes
            writer.Write((bool)_MurderersCanJoinAnyFaction);
            writer.Write((bool)_FactionHealRedIsCriminal);
            writer.Write((bool)_FactionHealRedIsGuardWhackable);
            writer.Write((bool)_FactionHealRedCrimIsCriminal);
            writer.Write((bool)_FactionHealRedCrimIsGuardWhackable);
            writer.Write((bool)_RedHealRedIsCriminal);
            writer.Write((bool)_RedHealRedIsGuardWhackable);
            writer.Write((bool)_RedHealRedCrimIsCriminal);
            writer.Write((bool)_RedHealRedCrimIsGuardWhackable);
            writer.Write((bool)_RedAllowHarmfulToBluesInTown);
            writer.Write((int)_HeadRansomCap);
            writer.Write((int)_KillsPerYoungMurder);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 2:
                    _YoungProtectionRegionsEnabled = reader.ReadBool();
                    goto case 1;
                case 1:
                    _MurderYoungFreezeSeconds = reader.ReadInt();
                    goto case 0;
                case 0:
                    //_PoisonChancePenaltyPerFollower = reader.ReadDouble();
                    _MurderersCanJoinAnyFaction = reader.ReadBool();
                    _FactionHealRedIsCriminal = reader.ReadBool();
                    _FactionHealRedIsGuardWhackable = reader.ReadBool();
                    _FactionHealRedCrimIsCriminal = reader.ReadBool();
                    _FactionHealRedCrimIsGuardWhackable  = reader.ReadBool();
                    _RedHealRedIsCriminal = reader.ReadBool();
                    _RedHealRedIsGuardWhackable = reader.ReadBool();
                    _RedHealRedCrimIsCriminal = reader.ReadBool();
                    _RedHealRedCrimIsGuardWhackable = reader.ReadBool();
                    _RedAllowHarmfulToBluesInTown = reader.ReadBool();
                    _HeadRansomCap = reader.ReadInt();
                    _KillsPerYoungMurder = reader.ReadInt();
                    break;
            }
            m_Instance = this;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.Administrator)
            {
                from.SendGump( new PropertiesGump( from, this));
            }
            else
            {
                from.SendMessage("Sorry, but you don't have permission access this.");
            }
            base.OnDoubleClick(from);
        }

        public static bool _MurderersCanJoinAnyFaction = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool MurderersCanJoinAnyFaction { get { return _MurderersCanJoinAnyFaction; } set { _MurderersCanJoinAnyFaction = value; } }

        public static bool _FactionHealRedIsCriminal = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool FactionHealRedIsCriminal { get { return _FactionHealRedIsCriminal; } set { _FactionHealRedIsCriminal = value; } }

        public static bool _FactionHealRedIsGuardWhackable = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool FactionHealRedIsGuardWhackable { get { return _FactionHealRedIsGuardWhackable; } set { _FactionHealRedIsGuardWhackable = value; } }

        public static bool _FactionHealRedCrimIsCriminal = true;
        [CommandProperty(AccessLevel.Administrator)]
        public bool FactionHealRedCrimIsCriminal { get { return _FactionHealRedCrimIsCriminal; } set { _FactionHealRedCrimIsCriminal = value; } }

        public static bool _FactionHealRedCrimIsGuardWhackable = true;
        [CommandProperty(AccessLevel.Administrator)]
        public bool FactionHealRedCrimIsGuardWhackable { get { return _FactionHealRedCrimIsGuardWhackable; } set { _FactionHealRedCrimIsGuardWhackable = value; } }
        
        public static bool _RedHealRedIsCriminal = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool RedHealRedIsCriminal { get { return _RedHealRedIsCriminal; } set { _RedHealRedIsCriminal = value; } }

        public static bool _RedHealRedIsGuardWhackable = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool RedHealRedIsGuardWhackable { get { return _RedHealRedIsGuardWhackable; } set { _RedHealRedIsGuardWhackable = value; } }


        public static bool _RedHealRedCrimIsCriminal = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool RedHealRedCrimIsCriminal { get { return _RedHealRedCrimIsCriminal; } set { _RedHealRedCrimIsCriminal = value; } }

        public static bool _RedHealRedCrimIsGuardWhackable = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool RedHealRedCrimIsGuardWhackable { get { return _RedHealRedCrimIsGuardWhackable; } set { _RedHealRedCrimIsGuardWhackable = value; } }

        public static int _HeadRansomCap = 50000;
        [CommandProperty(AccessLevel.Administrator)]
        public int HeadRansomCap { get { return _HeadRansomCap; } set { _HeadRansomCap = value; } }

        public static bool _RedAllowHarmfulToBluesInTown = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool RedAllowHarmfulToBlues { get { return _RedAllowHarmfulToBluesInTown; } set { _RedAllowHarmfulToBluesInTown = value; } }

        public static int _KillsPerYoungMurder = 5;
        [CommandProperty(AccessLevel.Administrator)]
        public int KillsPerYoungMurder { get { return _KillsPerYoungMurder; } set { _KillsPerYoungMurder = value; } }

        public static int _MurderYoungFreezeSeconds = 60;
        [CommandProperty(AccessLevel.Administrator)]
        public int MurderYoungFreezeSeconds { get { return _MurderYoungFreezeSeconds; } set { _MurderYoungFreezeSeconds = value; } }

        public static bool _YoungProtectionRegionsEnabled = true;
        [CommandProperty(AccessLevel.Administrator)]
        public bool YoungProtectionRegionsEnabled { get { return _YoungProtectionRegionsEnabled; } set { _YoungProtectionRegionsEnabled = value; } }
    }
}