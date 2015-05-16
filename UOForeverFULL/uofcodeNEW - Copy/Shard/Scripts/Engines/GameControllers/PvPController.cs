using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{    
    class PvPController : Item
    {   
        private static PvPController m_Instance;
        public static PvPController Instance { get { return m_Instance; } }

        [Constructable]
        public PvPController()
            : base(0xEDC)
        {
            this.Name = "PvP Controller";
            this.Movable = false;
            this.Visible = false;

            if (m_Instance != null)
            {
                // there can only be one PvPController game stone in the world
                m_Instance.Location = this.Location;
                Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.Administrator, 0x489,
                    "Existing PvPController has been moved to this location (DON'T DELETE IT!).");
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback<PvPController>(UpdateInstancePosition), this); 
            }
            else
                m_Instance = this;
        }

        public static void UpdateInstancePosition(PvPController attemptedConstruct)
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

        public PvPController(Serial serial) : base(serial) { }

        public override void Delete()
        {
            return; // can't delete it!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);//version

            // version 1
            writer.Write((bool)_AnybodyAllowedInStronghold);

            // version 0
            //global attributes
            
            writer.Write((bool)_CriminalCanUseOthersGates);
            writer.Write((byte)_SigilAnnounceStolen);
            writer.Write((TimeSpan)_SigilCorruptionGrace);
            writer.Write((TimeSpan)_SigilCorruptionPeriod);
            writer.Write((TimeSpan)_SigilReturnPeriod);
            writer.Write((TimeSpan)_SigilPurificationPeriod);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    _AnybodyAllowedInStronghold = reader.ReadBool();
                    goto case 0;

                case 0:
                    //_PoisonChancePenaltyPerFollower = reader.ReadDouble();
                    
                    _CriminalCanUseOthersGates = reader.ReadBool();
                    _SigilAnnounceStolen = (SigilStolenAnnouncing)reader.ReadByte();
                    _SigilCorruptionGrace = reader.ReadTimeSpan();
                    _SigilCorruptionPeriod = reader.ReadTimeSpan();
                    _SigilReturnPeriod = reader.ReadTimeSpan();
                    _SigilPurificationPeriod = reader.ReadTimeSpan();
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

        public static bool _CriminalCanUseOthersGates = true;
        [CommandProperty(AccessLevel.Administrator)]
        public bool CriminalCanUseOthersGates { get { return _CriminalCanUseOthersGates; } set { _CriminalCanUseOthersGates = value; } }

        public enum SigilStolenAnnouncing : byte
        {
            None = 0,
            All = 1,
            Owner = 2
        }

        public static SigilStolenAnnouncing _SigilAnnounceStolen = SigilStolenAnnouncing.All;
        [CommandProperty(AccessLevel.Administrator)]
        public SigilStolenAnnouncing SigilAnnounceStolen { get { return _SigilAnnounceStolen; } set { _SigilAnnounceStolen = value; } }

        // ?? time corrupting faction has to return the sigil before corruption time resets ?
        public static TimeSpan _SigilCorruptionGrace = TimeSpan.FromMinutes(15.0);
        [CommandProperty(AccessLevel.Administrator)]
        public TimeSpan SigilCorruptionGrace { get { return _SigilCorruptionGrace; } set { _SigilCorruptionGrace = value; } }

        // Sigil must be held at a stronghold for this amount of time in order to become corrupted
        public static TimeSpan _SigilCorruptionPeriod = TimeSpan.FromHours(4.0);
        [CommandProperty(AccessLevel.Administrator)]
        public TimeSpan SigilCorruptionPeriod { get { return _SigilCorruptionPeriod; } set { _SigilCorruptionPeriod = value; } }

        // After a sigil has been corrupted it must be returned to the town within this period of time
        public static TimeSpan _SigilReturnPeriod = TimeSpan.FromMinutes(20.0);
        [CommandProperty(AccessLevel.Administrator)]
        public TimeSpan SigilReturnPeriod { get { return _SigilReturnPeriod; } set { _SigilReturnPeriod = value; } }

        // Once it's been returned the corrupting faction owns the town for this period of time
        public static TimeSpan _SigilPurificationPeriod = TimeSpan.FromDays( 3.0 );
        [CommandProperty(AccessLevel.Administrator)]
        public TimeSpan SigilPurificationPeriod { get { return _SigilPurificationPeriod; } set { _SigilPurificationPeriod = value; } }

        public static bool _AnybodyAllowedInStronghold = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool AnybodyAllowedInStronghold { get { return _AnybodyAllowedInStronghold; } set { _AnybodyAllowedInStronghold = value; } }
    }
}