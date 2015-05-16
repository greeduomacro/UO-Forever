using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public enum HousePlacementItemBlocking : byte
    {
        None = 0,
        Impassable = 1,
        All = 2
    }
    
    class HouseSystemController : Item
    {   
        private static HouseSystemController m_Instance;
        public static HouseSystemController Instance { get { return m_Instance; } }

        [Constructable]
        public HouseSystemController()
            : base(0xEDC)
        {
            this.Name = "House System Controller";
            this.Movable = false;
            this.Visible = false;

            if (m_Instance != null)
            {
                // there can only be one HouseSystemController game stone in the world
                m_Instance.Location = this.Location;
                Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.Administrator, 0x489,
                    "Existing HouseSystemController has been moved to this location (DON'T DELETE IT!).");
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback<HouseSystemController>(UpdateInstancePosition), this); 
            }
            else
                m_Instance = this;
        }

        public static void UpdateInstancePosition(HouseSystemController attemptedConstruct)
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

        public HouseSystemController(Serial serial) : base(serial) { }

        public override void Delete()
        {
            return; // can't delete it!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
            // version 0
            //global attributes
            writer.Write((byte)_ItemsBlockPlacement);
            writer.Write((bool)_MobsBlockPlacement);
            writer.Write((bool)_MobsBlockPlacement);
            writer.Write((int)_MinPlaceDelay);
            writer.Write((int)_MaxPlaceDelay);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    //_PoisonChancePenaltyPerFollower = reader.ReadDouble();
                    _ItemsBlockPlacement = (HousePlacementItemBlocking)reader.ReadByte();
                    _MobsBlockPlacement = reader.ReadBool();
                    _PetsBlockPlacement = reader.ReadBool();
                    _MinPlaceDelay = reader.ReadInt();
                    _MaxPlaceDelay = reader.ReadInt();
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

        public static HousePlacementItemBlocking _ItemsBlockPlacement = HousePlacementItemBlocking.Impassable;
        [CommandProperty(AccessLevel.Administrator)]
        public HousePlacementItemBlocking ItemsBlockPlacement { get { return _ItemsBlockPlacement; } set { _ItemsBlockPlacement = value; } }

        public static bool _MobsBlockPlacement = true;
        [CommandProperty(AccessLevel.Administrator)]
        public bool MobsBlockPlacement { get { return _MobsBlockPlacement; } set { _MobsBlockPlacement = value; } }

        public static bool _PetsBlockPlacement = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool PetsBlockPlacement { get { return _PetsBlockPlacement; } set { _PetsBlockPlacement = value; } }

        public static int _MinPlaceDelay = 2;
        [CommandProperty(AccessLevel.Administrator)]
        public int MinPlaceDelay { get { return _MinPlaceDelay; } set { _MinPlaceDelay = value; } }

        public static int _MaxPlaceDelay = 6;
        [CommandProperty(AccessLevel.Administrator)]
        public int MaxPlaceDelay { get { return _MaxPlaceDelay; } set { _MaxPlaceDelay = value; } }

        public static TimeSpan _OwnHouseMinGameTime = TimeSpan.FromHours(24.0);
        [CommandProperty(AccessLevel.Administrator)]
        public TimeSpan OwnHouseMinGameTime { get { return _OwnHouseMinGameTime; } set { _OwnHouseMinGameTime = value; } }

        public static int _OwnHouseMinSkillsOnAccount = 1200;
        [CommandProperty(AccessLevel.Administrator)]
        public int OwnHouseMinSkillsOnAccount { get { return _OwnHouseMinSkillsOnAccount; } set { _OwnHouseMinSkillsOnAccount = value; } }

        
    }
}