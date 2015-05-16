using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    class BoatSystemController : Item
    {
        private static BoatSystemController m_Instance;
        public static BoatSystemController Instance { get { return m_Instance; } }

        [Constructable]
        public BoatSystemController()
            : base(0xEDC)
        {
            this.Name = "Boat System Controller";
            this.Movable = false;
            this.Visible = false;

            if (m_Instance != null)
            {
                // there can only be one BoatSytemController game stone in the world
                m_Instance.Location = this.Location;
                Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.Administrator, 0x489,
                    "Existing BoatSytemController has been moved to this location (DON'T DELETE IT!).");
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback<BoatSystemController>(UpdateInstancePosition), this);
            }
            else
                m_Instance = this;
        }

        public static void UpdateInstancePosition(BoatSystemController attemptedConstruct)
        {
            if (attemptedConstruct == null) return;
            if (m_Instance == null) // should never happen, but if it does, make this the instance
            {
                m_Instance = attemptedConstruct;
            }
            else if (attemptedConstruct.Location != new Point3D(0, 0, 0)) // move the instance to it's location and delete it
            {
                m_Instance.Location = attemptedConstruct.Location;
                attemptedConstruct.Delete();
            }
        }

        public BoatSystemController(Serial serial) : base(serial) { }

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
            writer.Write((int)_SmallBoatCost);
            writer.Write((int)_MediumBoatCost);
            writer.Write((int)_LargeBoatCost);
            writer.Write((int)_GargoyleBoatCost);
            writer.Write((int)_TokunoBoatCost);
            writer.Write((int)_OrcBoatCost);
            writer.Write((int)_ShipRepairToolsCost);
            writer.Write((int)_ShipCannonCost);
            
            writer.Write((double)_BoatRansomCostFraction);
            writer.Write((double)_BoatRansomGoldSinkFraction); // gold sink
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    _SmallBoatCost = reader.ReadInt();
                    _MediumBoatCost = reader.ReadInt();
                    _LargeBoatCost = reader.ReadInt();
                    _GargoyleBoatCost = reader.ReadInt();
                    _TokunoBoatCost = reader.ReadInt();
                    _OrcBoatCost = reader.ReadInt();
                    _ShipRepairToolsCost = reader.ReadInt();
                    _ShipCannonCost = reader.ReadInt();
                    _BoatRansomCostFraction = reader.ReadDouble();
                    _BoatRansomGoldSinkFraction = reader.ReadDouble();
                    break;
            }
            m_Instance = this;
            //SBShipwright.UpdateSBInfos();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.Administrator)
            {
                from.SendGump(new PropertiesGump(from, this));
            }
            else
            {
                from.SendMessage("Sorry, but you don't have permission access this.");
            }
            base.OnDoubleClick(from);
        }

        //50177
        //55552
        //60927

        public static int _SmallBoatCost = 35000;
        [CommandProperty(AccessLevel.Administrator)]
        public int SmallBoatCost { get { return _SmallBoatCost; } set { _SmallBoatCost = value; } }

        public static int _MediumBoatCost = 40000;
        [CommandProperty(AccessLevel.Administrator)]
        public int MediumBoatCost { get { return _MediumBoatCost; } set { _MediumBoatCost = value; } }

        public static int _LargeBoatCost = 45000;
        [CommandProperty(AccessLevel.Administrator)]
        public int LargeBoatCost { get { return _LargeBoatCost; } set { _LargeBoatCost = value; } }

        public static int _GargoyleBoatCost = 20000;
        [CommandProperty(AccessLevel.Administrator)]
        public int GargoyleBoatCost { get { return _GargoyleBoatCost; } set { _GargoyleBoatCost = value; } }

        public static int _TokunoBoatCost = 20000;
        [CommandProperty(AccessLevel.Administrator)]
        public int TokunoBoatCost { get { return _TokunoBoatCost; } set { _TokunoBoatCost = value; } }

        public static int _OrcBoatCost = 20000;
        [CommandProperty(AccessLevel.Administrator)]
        public int OrcBoatCost { get { return _OrcBoatCost; } set { _OrcBoatCost = value; } }

        public static int _ShipRepairToolsCost = 500;
        [CommandProperty(AccessLevel.Administrator)]
        public int ShipRepairTools { get { return _ShipRepairToolsCost; } set { _ShipRepairToolsCost = value; } }

        public static int _ShipCannonCost = 500;
        [CommandProperty(AccessLevel.Administrator)]
        public int ShipCannonCost { get { return _ShipCannonCost; } set { _ShipCannonCost = value; } }

        public static double _BoatRansomCostFraction = 0.1;
        [CommandProperty(AccessLevel.Administrator)]
        public double BoatRansomCostFraction { get { return _BoatRansomCostFraction; } set { _BoatRansomCostFraction = value; } }

        public static double _BoatRansomGoldSinkFraction = 0.2; // 20% of the ransom is lost (i.e. 0.1 * 0.2 = 2% of the original cost of the boat)
        [CommandProperty(AccessLevel.Administrator)]
        public double BoatRansomGoldSinkFraction { get { return _BoatRansomGoldSinkFraction; } set { _BoatRansomGoldSinkFraction = value; } }
    }
}