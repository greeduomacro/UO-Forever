using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;
using Server.Engines.BulkOrders;

namespace Server.Items
{    
    class BODSystemController : Item
    {   
        private static BODSystemController m_Instance;
        public static BODSystemController Instance { get { return m_Instance; } }

        [Constructable]
        public BODSystemController()
            : base(0xEDC)
        {
            this.Name = "Dynamic Settings Controller";
            this.Movable = false;
            this.Visible = false;

            if (m_Instance != null)
            {
                // there can only be one BODSystemController game stone in the world
                m_Instance.Location = this.Location;
                Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.Administrator, 0x489,
                    "Existing BODSystemController has been moved to this location (DON'T DELETE IT!).");
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback<BODSystemController>(UpdateInstancePosition), this); 
            }
            else
                m_Instance = this;
        }

        public static void UpdateInstancePosition(BODSystemController attemptedConstruct)
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

        public BODSystemController(Serial serial) : base(serial) { }

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
            
            writer.Write((double)_SmallBodChanceForNone);
            writer.Write((double)_SmallBodChanceForDullCopper);
            writer.Write((double)_SmallBodChanceForShadowIron);
            writer.Write((double)_SmallBodChanceForCopper);
            writer.Write((double)_SmallBodChanceForBronze);
            writer.Write((double)_SmallBodChanceForGold);
            writer.Write((double)_SmallBodChanceForAgapite);
            writer.Write((double)_SmallBodChanceForVerite);
            writer.Write((double)_SmallBodChanceForValorite);

            writer.Write((double)_LargeBodChanceForNone);
            writer.Write((double)_LargeBodChanceForDullCopper);
            writer.Write((double)_LargeBodChanceForShadowIron);
            writer.Write((double)_LargeBodChanceForCopper);
            writer.Write((double)_LargeBodChanceForBronze);
            writer.Write((double)_LargeBodChanceForGold);
            writer.Write((double)_LargeBodChanceForAgapite);
            writer.Write((double)_LargeBodChanceForVerite);
            writer.Write((double)_LargeBodChanceForValorite);

            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    //_PoisonChancePenaltyPerFollower = reader.ReadDouble();
                   // _ItemsBlockPlacement = (HousePlacementItemBlocking)reader.ReadByte();
                    _SmallBodChanceForNone = reader.ReadDouble();
                    _SmallBodChanceForDullCopper = reader.ReadDouble();
                    _SmallBodChanceForShadowIron = reader.ReadDouble();
                    _SmallBodChanceForCopper = reader.ReadDouble();
                    _SmallBodChanceForBronze = reader.ReadDouble();
                    _SmallBodChanceForGold = reader.ReadDouble();
                    _SmallBodChanceForAgapite = reader.ReadDouble();
                    _SmallBodChanceForVerite = reader.ReadDouble();
                    _SmallBodChanceForValorite = reader.ReadDouble();

                    _LargeBodChanceForNone = reader.ReadDouble();
                    _LargeBodChanceForDullCopper = reader.ReadDouble();
                    _LargeBodChanceForShadowIron = reader.ReadDouble();
                    _LargeBodChanceForCopper = reader.ReadDouble();
                    _LargeBodChanceForBronze = reader.ReadDouble();
                    _LargeBodChanceForGold = reader.ReadDouble();
                    _LargeBodChanceForAgapite = reader.ReadDouble();
                    _LargeBodChanceForVerite = reader.ReadDouble();
                    _LargeBodChanceForValorite = reader.ReadDouble();
                    break;
            }
            m_Instance = this;
            UpdateChances();
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


        public static void UpdateChances()
        {
            SmallSmithBOD.m_BlacksmithMaterialChances[0] = _SmallBodChanceForNone;
            SmallSmithBOD.m_BlacksmithMaterialChances[1] = _SmallBodChanceForDullCopper;
            SmallSmithBOD.m_BlacksmithMaterialChances[2] = _SmallBodChanceForShadowIron;
            SmallSmithBOD.m_BlacksmithMaterialChances[3] = _SmallBodChanceForCopper;
            SmallSmithBOD.m_BlacksmithMaterialChances[4] = _SmallBodChanceForBronze;
            SmallSmithBOD.m_BlacksmithMaterialChances[5] = _SmallBodChanceForGold;
            SmallSmithBOD.m_BlacksmithMaterialChances[6] = _SmallBodChanceForAgapite;
            SmallSmithBOD.m_BlacksmithMaterialChances[7] = _SmallBodChanceForVerite;
            SmallSmithBOD.m_BlacksmithMaterialChances[8] = _SmallBodChanceForValorite;

            LargeSmithBOD.m_BlacksmithMaterialChances[0] = _LargeBodChanceForNone;
            LargeSmithBOD.m_BlacksmithMaterialChances[1] = _LargeBodChanceForDullCopper;
            LargeSmithBOD.m_BlacksmithMaterialChances[2] = _LargeBodChanceForShadowIron;
            LargeSmithBOD.m_BlacksmithMaterialChances[3] = _LargeBodChanceForCopper;
            LargeSmithBOD.m_BlacksmithMaterialChances[4] = _LargeBodChanceForBronze;
            LargeSmithBOD.m_BlacksmithMaterialChances[5] = _LargeBodChanceForGold;
            LargeSmithBOD.m_BlacksmithMaterialChances[6] = _LargeBodChanceForAgapite;
            LargeSmithBOD.m_BlacksmithMaterialChances[7] = _LargeBodChanceForVerite;
            LargeSmithBOD.m_BlacksmithMaterialChances[8] = _LargeBodChanceForValorite;
        }

        public static double _SmallBodChanceForNone = 0.501953125;
        [CommandProperty(AccessLevel.Administrator)]
        public double SmallBodChanceForNone { get { return _SmallBodChanceForNone; } set { _SmallBodChanceForNone = value; UpdateChances(); } }

        public static double _SmallBodChanceForDullCopper = 0.250000000;
        [CommandProperty(AccessLevel.Administrator)]
        public double SmallBodChanceForDullCopper { get { return _SmallBodChanceForDullCopper; } set { _SmallBodChanceForDullCopper = value; UpdateChances(); } }

        public static double _SmallBodChanceForShadowIron = 0.125000000;
        [CommandProperty(AccessLevel.Administrator)]
        public double SmallBodChanceForShadowIron { get { return _SmallBodChanceForShadowIron; } set { _SmallBodChanceForShadowIron = value; UpdateChances(); } }

        public static double _SmallBodChanceForCopper = 0.062500000;
        [CommandProperty(AccessLevel.Administrator)]
        public double SmallBodChanceForCopper { get { return _SmallBodChanceForCopper; } set { _SmallBodChanceForCopper = value; UpdateChances(); } }

        public static double _SmallBodChanceForBronze = 0.031250000;
        [CommandProperty(AccessLevel.Administrator)]
        public double SmallBodChanceForBronze { get { return _SmallBodChanceForBronze; } set { _SmallBodChanceForBronze = value; UpdateChances(); } }

        public static double _SmallBodChanceForGold = 0.015625000;
        [CommandProperty(AccessLevel.Administrator)]
        public double SmallBodChanceForGold { get { return _SmallBodChanceForGold; } set { _SmallBodChanceForGold = value; UpdateChances(); } }

        public static double _SmallBodChanceForAgapite = 0.007812500;
        [CommandProperty(AccessLevel.Administrator)]
        public double SmallBodChanceForAgapite { get { return _SmallBodChanceForAgapite; } set { _SmallBodChanceForAgapite = value; UpdateChances(); } }

        public static double _SmallBodChanceForVerite = 0.003906250;
        [CommandProperty(AccessLevel.Administrator)]
        public double SmallBodChanceForVerite { get { return _SmallBodChanceForVerite; } set { _SmallBodChanceForVerite = value; UpdateChances(); } }

        public static double _SmallBodChanceForValorite = 0.001953125;
        [CommandProperty(AccessLevel.Administrator)]
        public double SmallBodChanceForValorite { get { return _SmallBodChanceForValorite; } set { _SmallBodChanceForValorite = value; UpdateChances(); } }





        public static double _LargeBodChanceForNone = 0.501953125;
        [CommandProperty(AccessLevel.Administrator)]
        public double LargeBodChanceForNone { get { return _LargeBodChanceForNone; } set { _LargeBodChanceForNone = value; UpdateChances(); } }

        public static double _LargeBodChanceForDullCopper = 0.250000000;
        [CommandProperty(AccessLevel.Administrator)]
        public double LargeBodChanceForDullCopper { get { return _LargeBodChanceForDullCopper; } set { _LargeBodChanceForDullCopper = value; UpdateChances(); } }

        public static double _LargeBodChanceForShadowIron = 0.125000000;
        [CommandProperty(AccessLevel.Administrator)]
        public double LargeBodChanceForShadowIron { get { return _LargeBodChanceForShadowIron; } set { _LargeBodChanceForShadowIron = value; UpdateChances(); } }

        public static double _LargeBodChanceForCopper = 0.062500000;
        [CommandProperty(AccessLevel.Administrator)]
        public double LargeBodChanceForCopper { get { return _LargeBodChanceForCopper; } set { _LargeBodChanceForCopper = value; UpdateChances(); } }

        public static double _LargeBodChanceForBronze = 0.031250000;
        [CommandProperty(AccessLevel.Administrator)]
        public double LargeBodChanceForBronze { get { return _LargeBodChanceForBronze; } set { _LargeBodChanceForBronze = value; UpdateChances(); } }

        public static double _LargeBodChanceForGold = 0.015625000;
        [CommandProperty(AccessLevel.Administrator)]
        public double LargeBodChanceForGold { get { return _LargeBodChanceForGold; } set { _LargeBodChanceForGold = value; UpdateChances(); } }

        public static double _LargeBodChanceForAgapite = 0.007812500;
        [CommandProperty(AccessLevel.Administrator)]
        public double LargeBodChanceForAgapite { get { return _LargeBodChanceForAgapite; } set { _LargeBodChanceForAgapite = value; UpdateChances(); } }

        public static double _LargeBodChanceForVerite = 0.003906250;
        [CommandProperty(AccessLevel.Administrator)]
        public double LargeBodChanceForVerite { get { return _LargeBodChanceForVerite; } set { _LargeBodChanceForVerite = value; UpdateChances(); } }

        public static double _LargeBodChanceForValorite = 0.001953125;
        [CommandProperty(AccessLevel.Administrator)]
        public double LargeBodChanceForValorite { get { return _LargeBodChanceForValorite; } set { _LargeBodChanceForValorite = value; UpdateChances(); } }

    }
}