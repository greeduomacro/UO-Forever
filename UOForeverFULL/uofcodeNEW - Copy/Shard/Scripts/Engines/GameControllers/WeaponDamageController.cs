using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    class WeaponDamageController : Item
    {   
        private static WeaponDamageController m_Instance;
        public static WeaponDamageController Instance { get { return m_Instance; } }

        [Constructable]
        public WeaponDamageController()
            : base(0xEDC)
        {
            this.Name = "Weapon Damage Controller";
            this.Movable = false;
            this.Visible = false;

            if (m_Instance != null)
            {
                // there can only be one WeaponDamageController game stone in the world
                m_Instance.Location = this.Location;
                Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.GameMaster, 0x489,
                    "Existing WeaponDamageController has been moved to this location (DON'T DELETE IT!).");
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback<WeaponDamageController>(UpdateInstancePosition), this); 
            }
            else
                m_Instance = this;
        }

        public static void UpdateInstancePosition(WeaponDamageController attemptedConstruct)
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

        public WeaponDamageController(Serial serial) : base(serial) { }

        public override void Delete()
        {
            return; // can't delete it!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2);//version

            // version 2
            writer.Write((double)_WeaponDurabilityLossVsMobs);

            // version 1
            //global attributes
            writer.Write((double)m_AllWeaponDamageMultiplier);
            writer.Write((double)m_PlayerVsMobMultiplier); 
            writer.Write((double)m_MobVsPlayerMultiplier);
            writer.Write((int)m_MobVsPlayerReduction);
            

            // individual weapon attributes
            writer.Write((int)m_AxeDamageMin); writer.Write((int)m_AxeDamageMax);
            writer.Write((int)m_BattleAxeDamageMin); writer.Write((int)m_BattleAxeDamageMax);
            writer.Write((int)m_DoubleAxeDamageMin); writer.Write((int)m_DoubleAxeDamageMax);
            writer.Write((int)m_ExecutionersAxeDamageMin); writer.Write((int)m_ExecutionersAxeDamageMax);
            writer.Write((int)m_HatchetDamageMin); writer.Write((int)m_HatchetDamageMax);
            writer.Write((int)m_LargeBattleAxeDamageMin); writer.Write((int)m_LargeBattleAxeDamageMax);
            writer.Write((int)m_PickaxeDamageMin); writer.Write((int)m_PickaxeDamageMax);
            writer.Write((int)m_TwoHandedAxeDamageMin); writer.Write((int)m_TwoHandedAxeDamageMax);
            writer.Write((int)m_WarAxeDamageMin); writer.Write((int)m_WarAxeDamageMax);
            writer.Write((int)m_ButcherKnifeDamageMin); writer.Write((int)m_ButcherKnifeDamageMax);
            writer.Write((int)m_CleaverDamageMin); writer.Write((int)m_CleaverDamageMax);
            writer.Write((int)m_DaggerDamageMin); writer.Write((int)m_DaggerDamageMax);
            writer.Write((int)m_SkinningKnifeDamageMin); writer.Write((int)m_SkinningKnifeDamageMax);
            writer.Write((int)m_ClubDamageMin); writer.Write((int)m_ClubDamageMax);
            writer.Write((int)m_HammerPickDamageMin); writer.Write((int)m_HammerPickDamageMax);
            writer.Write((int)m_MaceDamageMin); writer.Write((int)m_MaceDamageMax);
            writer.Write((int)m_WarHammerDamageMin); writer.Write((int)m_WarHammerDamageMax);
            writer.Write((int)m_WarMaceDamageMin); writer.Write((int)m_WarMaceDamageMax);
            writer.Write((int)m_BardicheDamageMin); writer.Write((int)m_BardicheDamageMax);
            writer.Write((int)m_HalberdDamageMin); writer.Write((int)m_HalberdDamageMax);
            writer.Write((int)m_BowDamageMin); writer.Write((int)m_BowDamageMax);
            writer.Write((int)m_CrossbowDamageMin); writer.Write((int)m_CrossbowDamageMax);
            writer.Write((int)m_HeavyCrossbowDamageMin); writer.Write((int)m_HeavyCrossbowDamageMax);
            writer.Write((int)m_PitchforkDamageMin); writer.Write((int)m_PitchforkDamageMax);
            writer.Write((int)m_ShortSpearDamageMin); writer.Write((int)m_ShortSpearDamageMax);
            writer.Write((int)m_SpearDamageMin); writer.Write((int)m_SpearDamageMax);
            writer.Write((int)m_TribalSpearDamageMin); writer.Write((int)m_TribalSpearDamageMax);
            writer.Write((int)m_WarForkDamageMin); writer.Write((int)m_WarForkDamageMax);
            writer.Write((int)m_BlackStaffDamageMin); writer.Write((int)m_BlackStaffDamageMax);
            writer.Write((int)m_GnarledStaffDamageMin); writer.Write((int)m_GnarledStaffDamageMax);
            writer.Write((int)m_QuarterStaffDamageMin); writer.Write((int)m_QuarterStaffDamageMax);
            writer.Write((int)m_ShepherdsCrookDamageMin); writer.Write((int)m_ShepherdsCrookDamageMax);
            writer.Write((int)m_BroadswordDamageMin); writer.Write((int)m_BroadswordDamageMax);
            writer.Write((int)m_CutlassDamageMin); writer.Write((int)m_CutlassDamageMax);
            writer.Write((int)m_KatanaDamageMin); writer.Write((int)m_KatanaDamageMax);
            writer.Write((int)m_KryssDamageMin); writer.Write((int)m_KryssDamageMax);
            writer.Write((int)m_LongswordDamageMin); writer.Write((int)m_LongswordDamageMax);
            writer.Write((int)m_ScimitarDamageMin); writer.Write((int)m_ScimitarDamageMax);
            writer.Write((int)m_VikingSwordDamageMin); writer.Write((int)m_VikingSwordDamageMax);

            // version 1
            writer.Write((double)m_WeaponDurabilityLossPercentage);
            writer.Write((double)m_WeaponDurabilitySpeedAdjustment);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 2:
                    _WeaponDurabilityLossVsMobs = reader.ReadDouble();
                    goto case 1;
                case 1:
                    // global attributes
                    m_AllWeaponDamageMultiplier = reader.ReadDouble();
                    m_PlayerVsMobMultiplier = reader.ReadDouble();
                    m_MobVsPlayerMultiplier = reader.ReadDouble();
                    m_MobVsPlayerReduction = reader.ReadInt();

                    // individual weapon attributes
                    m_AxeDamageMin = reader.ReadInt(); m_AxeDamageMax = reader.ReadInt();
                    m_BattleAxeDamageMin = reader.ReadInt(); m_BattleAxeDamageMax = reader.ReadInt();
                    m_DoubleAxeDamageMin = reader.ReadInt(); m_DoubleAxeDamageMax = reader.ReadInt();
                    m_ExecutionersAxeDamageMin = reader.ReadInt(); m_ExecutionersAxeDamageMax = reader.ReadInt();
                    m_HatchetDamageMin = reader.ReadInt(); m_HatchetDamageMax = reader.ReadInt();
                    m_LargeBattleAxeDamageMin = reader.ReadInt(); m_LargeBattleAxeDamageMax = reader.ReadInt();
                    m_PickaxeDamageMin = reader.ReadInt(); m_PickaxeDamageMax = reader.ReadInt();
                    m_TwoHandedAxeDamageMin = reader.ReadInt(); m_TwoHandedAxeDamageMax = reader.ReadInt();
                    m_WarAxeDamageMin = reader.ReadInt(); m_WarAxeDamageMax = reader.ReadInt();
                    m_ButcherKnifeDamageMin = reader.ReadInt(); m_ButcherKnifeDamageMax = reader.ReadInt();
                    m_CleaverDamageMin = reader.ReadInt(); m_CleaverDamageMax = reader.ReadInt();
                    m_DaggerDamageMin = reader.ReadInt(); m_DaggerDamageMax = reader.ReadInt();
                    m_SkinningKnifeDamageMin = reader.ReadInt(); m_SkinningKnifeDamageMax = reader.ReadInt();
                    m_ClubDamageMin = reader.ReadInt(); m_ClubDamageMax = reader.ReadInt();
                    m_HammerPickDamageMin = reader.ReadInt(); m_HammerPickDamageMax = reader.ReadInt();
                    m_MaceDamageMin = reader.ReadInt(); m_MaceDamageMax = reader.ReadInt();
                    m_WarHammerDamageMin = reader.ReadInt(); m_WarHammerDamageMax = reader.ReadInt();
                    m_WarMaceDamageMin = reader.ReadInt(); m_WarMaceDamageMax = reader.ReadInt();
                    m_BardicheDamageMin = reader.ReadInt(); m_BardicheDamageMax = reader.ReadInt();
                    m_HalberdDamageMin = reader.ReadInt(); m_HalberdDamageMax = reader.ReadInt();
                    m_BowDamageMin = reader.ReadInt(); m_BowDamageMax = reader.ReadInt();
                    m_CrossbowDamageMin = reader.ReadInt(); m_CrossbowDamageMax = reader.ReadInt();
                    m_HeavyCrossbowDamageMin = reader.ReadInt(); m_HeavyCrossbowDamageMax = reader.ReadInt();
                    m_PitchforkDamageMin = reader.ReadInt(); m_PitchforkDamageMax = reader.ReadInt();
                    m_ShortSpearDamageMin = reader.ReadInt(); m_ShortSpearDamageMax = reader.ReadInt();
                    m_SpearDamageMin = reader.ReadInt(); m_SpearDamageMax = reader.ReadInt();
                    m_TribalSpearDamageMin = reader.ReadInt(); m_TribalSpearDamageMax = reader.ReadInt();
                    m_WarForkDamageMin = reader.ReadInt(); m_WarForkDamageMax = reader.ReadInt();
                    m_BlackStaffDamageMin = reader.ReadInt(); m_BlackStaffDamageMax = reader.ReadInt();
                    m_GnarledStaffDamageMin = reader.ReadInt(); m_GnarledStaffDamageMax = reader.ReadInt();
                    m_QuarterStaffDamageMin = reader.ReadInt(); m_QuarterStaffDamageMax = reader.ReadInt();
                    m_ShepherdsCrookDamageMin = reader.ReadInt(); m_ShepherdsCrookDamageMax = reader.ReadInt();
                    m_BroadswordDamageMin = reader.ReadInt(); m_BroadswordDamageMax = reader.ReadInt();
                    m_CutlassDamageMin = reader.ReadInt(); m_CutlassDamageMax = reader.ReadInt();
                    m_KatanaDamageMin = reader.ReadInt(); m_KatanaDamageMax = reader.ReadInt();
                    m_KryssDamageMin = reader.ReadInt(); m_KryssDamageMax = reader.ReadInt();
                    m_LongswordDamageMin = reader.ReadInt(); m_LongswordDamageMax = reader.ReadInt();
                    m_ScimitarDamageMin = reader.ReadInt(); m_ScimitarDamageMax = reader.ReadInt();
                    m_VikingSwordDamageMin = reader.ReadInt(); m_VikingSwordDamageMax = reader.ReadInt();

                    m_WeaponDurabilityLossPercentage = reader.ReadDouble();
                    m_WeaponDurabilitySpeedAdjustment = reader.ReadDouble();
                    break;
            }
            m_Instance = this;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                from.SendGump( new PropertiesGump( from, this));
            }
            else
            {
                from.SendMessage("Sorry, but you don't have permission access this.");
            }
            base.OnDoubleClick(from);
        }

        public static double _WeaponDurabilityLossVsMobs = 0.15;
        [CommandProperty(AccessLevel.GameMaster)]
        public double __WeaponDurabilityLossVsMobs { get { return _WeaponDurabilityLossVsMobs; } set { _WeaponDurabilityLossVsMobs = value; } }

        public static double _WeaponDurabilityLossPercentage { get { if (WeaponDamageController.Instance == null) return .15; else return WeaponDamageController.Instance.__WeaponDurabilityLossPercentage; } }
        private double m_WeaponDurabilityLossPercentage = .20;
        [CommandProperty(AccessLevel.Developer)]
        public double __WeaponDurabilityLossPercentage { get { return m_WeaponDurabilityLossPercentage; } set { m_WeaponDurabilityLossPercentage = value > 0 ? value : 0; } }

        public static double _WeaponDurabilitySpeedAdjustment { get { if (WeaponDamageController.Instance == null) return 1.0; else return WeaponDamageController.Instance.__WeaponDurabilitySpeedAdjustment; } }
        private double m_WeaponDurabilitySpeedAdjustment = 1.0;
        [CommandProperty(AccessLevel.Developer)]
        public double __WeaponDurabilitySpeedAdjustment { get { return m_WeaponDurabilitySpeedAdjustment; } set { m_WeaponDurabilitySpeedAdjustment = value > 1.0 ? value : 1.0; } }
    
        // ========= BEGIN GLOBAL ATTRIBUTES ======================================================================
        public static double _PlayerVsMobMultiplier { get { if (WeaponDamageController.Instance == null) return 1.0; else return WeaponDamageController.Instance.__PlayerVsMobMultiplier; } }
        private double m_PlayerVsMobMultiplier = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double __PlayerVsMobMultiplier { get { return m_PlayerVsMobMultiplier; } set { m_PlayerVsMobMultiplier = value > 0 ? value : 0; } }

        public static double _MobVsPlayerMultiplier { get { if (WeaponDamageController.Instance == null) return 1.0; else return WeaponDamageController.Instance.__MobVsPlayerMultiplier; } }
        private double m_MobVsPlayerMultiplier = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double __MobVsPlayerMultiplier { get { return m_MobVsPlayerMultiplier; } set { m_MobVsPlayerMultiplier = value > 0 ? value : 0; } }

        public static int _MobVsPlayerReduction { get { if (WeaponDamageController.Instance == null) return 0; else return WeaponDamageController.Instance.__MobVsPlayerReduction; } }
        private int m_MobVsPlayerReduction = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int __MobVsPlayerReduction { get { return m_MobVsPlayerReduction; } set { m_MobVsPlayerReduction = value; } }

        public static double _AllWeaponDamageMultiplier { get { if (WeaponDamageController.Instance == null) return 0.5; else return WeaponDamageController.Instance.__AllWeaponDamageMultiplier; } }
        private double m_AllWeaponDamageMultiplier = 0.5;
        [CommandProperty(AccessLevel.GameMaster)]
        public double __AllWeaponDamageMultiplier { get { return m_AllWeaponDamageMultiplier; } set { m_AllWeaponDamageMultiplier = value; } }
       
        // ========= BEGIN INDIVIDUAL WEAPON ATTRIBUTES ======================================================================
        public static int _AxeDamageMin { get { if (WeaponDamageController.Instance == null) return 6; else return WeaponDamageController.Instance.AxeDamageMin; } }
        private int m_AxeDamageMin = 6;
        [CommandProperty(AccessLevel.GameMaster)]
        public int AxeDamageMin { get { return m_AxeDamageMin; } set { m_AxeDamageMin = value; } }

        public static int _AxeDamageMax { get { if (WeaponDamageController.Instance == null) return 33; else return WeaponDamageController.Instance.AxeDamageMax; } }
        private int m_AxeDamageMax = 33;
        [CommandProperty(AccessLevel.GameMaster)]
        public int AxeDamageMax { get { return m_AxeDamageMax; } set { m_AxeDamageMax = value; } }

        public static int _BattleAxeDamageMin { get { if (WeaponDamageController.Instance == null) return 6; else return WeaponDamageController.Instance.BattleAxeDamageMin; } }
        private int m_BattleAxeDamageMin = 6;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BattleAxeDamageMin { get { return m_BattleAxeDamageMin; } set { m_BattleAxeDamageMin = value; } }

        public static int _BattleAxeDamageMax { get { if (WeaponDamageController.Instance == null) return 38; else return WeaponDamageController.Instance.BattleAxeDamageMax; } }
        private int m_BattleAxeDamageMax = 38;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BattleAxeDamageMax { get { return m_BattleAxeDamageMax; } set { m_BattleAxeDamageMax = value; } }

        public static int _DoubleAxeDamageMin { get { if (WeaponDamageController.Instance == null) return 5; else return WeaponDamageController.Instance.DoubleAxeDamageMin; } }
        private int m_DoubleAxeDamageMin = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DoubleAxeDamageMin { get { return m_DoubleAxeDamageMin; } set { m_DoubleAxeDamageMin = value; } }

        public static int _DoubleAxeDamageMax { get { if (WeaponDamageController.Instance == null) return 35; else return WeaponDamageController.Instance.DoubleAxeDamageMax; } }
        private int m_DoubleAxeDamageMax = 35;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DoubleAxeDamageMax { get { return m_DoubleAxeDamageMax; } set { m_DoubleAxeDamageMax = value; } }

        public static int _ExecutionersAxeDamageMin { get { if (WeaponDamageController.Instance == null) return 6; else return WeaponDamageController.Instance.ExecutionersAxeDamageMin; } }
        private int m_ExecutionersAxeDamageMin = 6;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ExecutionersAxeDamageMin { get { return m_ExecutionersAxeDamageMin; } set { m_ExecutionersAxeDamageMin = value; } }

        public static int _ExecutionersAxeDamageMax { get { if (WeaponDamageController.Instance == null) return 33; else return WeaponDamageController.Instance.ExecutionersAxeDamageMax; } }
        private int m_ExecutionersAxeDamageMax = 33;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ExecutionersAxeDamageMax { get { return m_ExecutionersAxeDamageMax; } set { m_ExecutionersAxeDamageMax = value; } }

        public static int _HatchetDamageMin { get { if (WeaponDamageController.Instance == null) return 2; else return WeaponDamageController.Instance.HatchetDamageMin; } }
        private int m_HatchetDamageMin = 2;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HatchetDamageMin { get { return m_HatchetDamageMin; } set { m_HatchetDamageMin = value; } }

        public static int _HatchetDamageMax { get { if (WeaponDamageController.Instance == null) return 17; else return WeaponDamageController.Instance.HatchetDamageMax; } }
        private int m_HatchetDamageMax = 17;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HatchetDamageMax { get { return m_HatchetDamageMax; } set { m_HatchetDamageMax = value; } }

        public static int _LargeBattleAxeDamageMin { get { if (WeaponDamageController.Instance == null) return 6; else return WeaponDamageController.Instance.LargeBattleAxeDamageMin; } }
        private int m_LargeBattleAxeDamageMin = 6;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LargeBattleAxeDamageMin { get { return m_LargeBattleAxeDamageMin; } set { m_LargeBattleAxeDamageMin = value; } }

        public static int _LargeBattleAxeDamageMax { get { if (WeaponDamageController.Instance == null) return 38; else return WeaponDamageController.Instance.LargeBattleAxeDamageMax; } }
        private int m_LargeBattleAxeDamageMax = 38;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LargeBattleAxeDamageMax { get { return m_LargeBattleAxeDamageMax; } set { m_LargeBattleAxeDamageMax = value; } }

        public static int _PickaxeDamageMin { get { if (WeaponDamageController.Instance == null) return 1; else return WeaponDamageController.Instance.PickaxeDamageMin; } }
        private int m_PickaxeDamageMin = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PickaxeDamageMin { get { return m_PickaxeDamageMin; } set { m_PickaxeDamageMin = value; } }

        public static int _PickaxeDamageMax { get { if (WeaponDamageController.Instance == null) return 15; else return WeaponDamageController.Instance.PickaxeDamageMax; } }
        private int m_PickaxeDamageMax = 15;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PickaxeDamageMax { get { return m_PickaxeDamageMax; } set { m_PickaxeDamageMax = value; } }

        public static int _TwoHandedAxeDamageMin { get { if (WeaponDamageController.Instance == null) return 5; else return WeaponDamageController.Instance.TwoHandedAxeDamageMin; } }
        private int m_TwoHandedAxeDamageMin = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TwoHandedAxeDamageMin { get { return m_TwoHandedAxeDamageMin; } set { m_TwoHandedAxeDamageMin = value; } }

        public static int _TwoHandedAxeDamageMax { get { if (WeaponDamageController.Instance == null) return 39; else return WeaponDamageController.Instance.TwoHandedAxeDamageMax; } }
        private int m_TwoHandedAxeDamageMax = 39;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TwoHandedAxeDamageMax { get { return m_TwoHandedAxeDamageMax; } set { m_TwoHandedAxeDamageMax = value; } }

        public static int _WarAxeDamageMin { get { if (WeaponDamageController.Instance == null) return 9; else return WeaponDamageController.Instance.WarAxeDamageMin; } }
        private int m_WarAxeDamageMin = 9;
        [CommandProperty(AccessLevel.GameMaster)]
        public int WarAxeDamageMin { get { return m_WarAxeDamageMin; } set { m_WarAxeDamageMin = value; } }

        public static int _WarAxeDamageMax { get { if (WeaponDamageController.Instance == null) return 27; else return WeaponDamageController.Instance.WarAxeDamageMax; } }
        private int m_WarAxeDamageMax = 27;
        [CommandProperty(AccessLevel.GameMaster)]
        public int WarAxeDamageMax { get { return m_WarAxeDamageMax; } set { m_WarAxeDamageMax = value; } }

        public static int _ButcherKnifeDamageMin { get { if (WeaponDamageController.Instance == null) return 2; else return WeaponDamageController.Instance.ButcherKnifeDamageMin; } }
        private int m_ButcherKnifeDamageMin = 2;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ButcherKnifeDamageMin { get { return m_ButcherKnifeDamageMin; } set { m_ButcherKnifeDamageMin = value; } }

        public static int _ButcherKnifeDamageMax { get { if (WeaponDamageController.Instance == null) return 14; else return WeaponDamageController.Instance.ButcherKnifeDamageMax; } }
        private int m_ButcherKnifeDamageMax = 14;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ButcherKnifeDamageMax { get { return m_ButcherKnifeDamageMax; } set { m_ButcherKnifeDamageMax = value; } }

        public static int _CleaverDamageMin { get { if (WeaponDamageController.Instance == null) return 2; else return WeaponDamageController.Instance.CleaverDamageMin; } }
        private int m_CleaverDamageMin = 2;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CleaverDamageMin { get { return m_CleaverDamageMin; } set { m_CleaverDamageMin = value; } }

        public static int _CleaverDamageMax { get { if (WeaponDamageController.Instance == null) return 13; else return WeaponDamageController.Instance.CleaverDamageMax; } }
        private int m_CleaverDamageMax = 13;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CleaverDamageMax { get { return m_CleaverDamageMax; } set { m_CleaverDamageMax = value; } }

        public static int _DaggerDamageMin { get { if (WeaponDamageController.Instance == null) return 3; else return WeaponDamageController.Instance.DaggerDamageMin; } }
        private int m_DaggerDamageMin = 3;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DaggerDamageMin { get { return m_DaggerDamageMin; } set { m_DaggerDamageMin = value; } }

        public static int _DaggerDamageMax { get { if (WeaponDamageController.Instance == null) return 15; else return WeaponDamageController.Instance.DaggerDamageMax; } }
        private int m_DaggerDamageMax = 15;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DaggerDamageMax { get { return m_DaggerDamageMax; } set { m_DaggerDamageMax = value; } }

        public static int _SkinningKnifeDamageMin { get { if (WeaponDamageController.Instance == null) return 1; else return WeaponDamageController.Instance.SkinningKnifeDamageMin; } }
        private int m_SkinningKnifeDamageMin = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SkinningKnifeDamageMin { get { return m_SkinningKnifeDamageMin; } set { m_SkinningKnifeDamageMin = value; } }

        public static int _SkinningKnifeDamageMax { get { if (WeaponDamageController.Instance == null) return 10; else return WeaponDamageController.Instance.SkinningKnifeDamageMax; } }
        private int m_SkinningKnifeDamageMax = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SkinningKnifeDamageMax { get { return m_SkinningKnifeDamageMax; } set { m_SkinningKnifeDamageMax = value; } }

        public static int _ClubDamageMin { get { if (WeaponDamageController.Instance == null) return 8; else return WeaponDamageController.Instance.ClubDamageMin; } }
        private int m_ClubDamageMin = 8;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ClubDamageMin { get { return m_ClubDamageMin; } set { m_ClubDamageMin = value; } }

        public static int _ClubDamageMax { get { if (WeaponDamageController.Instance == null) return 24; else return WeaponDamageController.Instance.ClubDamageMax; } }
        private int m_ClubDamageMax = 24;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ClubDamageMax { get { return m_ClubDamageMax; } set { m_ClubDamageMax = value; } }

        public static int _HammerPickDamageMin { get { if (WeaponDamageController.Instance == null) return 6; else return WeaponDamageController.Instance.HammerPickDamageMin; } }
        private int m_HammerPickDamageMin = 6;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HammerPickDamageMin { get { return m_HammerPickDamageMin; } set { m_HammerPickDamageMin = value; } }

        public static int _HammerPickDamageMax { get { if (WeaponDamageController.Instance == null) return 33; else return WeaponDamageController.Instance.HammerPickDamageMax; } }
        private int m_HammerPickDamageMax = 33;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HammerPickDamageMax { get { return m_HammerPickDamageMax; } set { m_HammerPickDamageMax = value; } }

        public static int _MaceDamageMin { get { if (WeaponDamageController.Instance == null) return 8; else return WeaponDamageController.Instance.MaceDamageMin; } }
        private int m_MaceDamageMin = 8;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaceDamageMin { get { return m_MaceDamageMin; } set { m_MaceDamageMin = value; } }

        public static int _MaceDamageMax { get { if (WeaponDamageController.Instance == null) return 32; else return WeaponDamageController.Instance.MaceDamageMax; } }
        private int m_MaceDamageMax = 32;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaceDamageMax { get { return m_MaceDamageMax; } set { m_MaceDamageMax = value; } }

        public static int _MagicWandDamageMin { get { if (WeaponDamageController.Instance == null) return 2; else return WeaponDamageController.Instance.MagicWandDamageMin; } }
        private int m_MagicWandDamageMin = 2;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MagicWandDamageMin { get { return m_MagicWandDamageMin; } set { m_MagicWandDamageMin = value; } }

        public static int _MagicWandDamageMax { get { if (WeaponDamageController.Instance == null) return 6; else return WeaponDamageController.Instance.MagicWandDamageMax; } }
        private int m_MagicWandDamageMax = 6;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MagicWandDamageMax { get { return m_MagicWandDamageMax; } set { m_MagicWandDamageMax = value; } }

        public static int _MaulDamageMin { get { if (WeaponDamageController.Instance == null) return 10; else return WeaponDamageController.Instance.MaulDamageMin; } }
        private int m_MaulDamageMin = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaulDamageMin { get { return m_MaulDamageMin; } set { m_MaulDamageMin = value; } }

        public static int _MaulDamageMax { get { if (WeaponDamageController.Instance == null) return 30; else return WeaponDamageController.Instance.MaulDamageMax; } }
        private int m_MaulDamageMax = 30;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaulDamageMax { get { return m_MaulDamageMax; } set { m_MaulDamageMax = value; } }

        public static int _WarHammerDamageMin { get { if (WeaponDamageController.Instance == null) return 12; else return WeaponDamageController.Instance.WarHammerDamageMin; } }
        private int m_WarHammerDamageMin = 12;
        [CommandProperty(AccessLevel.GameMaster)]
        public int WarHammerDamageMin { get { return m_WarHammerDamageMin; } set { m_WarHammerDamageMin = value; } }

        public static int _WarHammerDamageMax { get { if (WeaponDamageController.Instance == null) return 40; else return WeaponDamageController.Instance.WarHammerDamageMax; } }
        private int m_WarHammerDamageMax = 40;
        [CommandProperty(AccessLevel.GameMaster)]
        public int WarHammerDamageMax { get { return m_WarHammerDamageMax; } set { m_WarHammerDamageMax = value; } }

        public static int _WarMaceDamageMin { get { if (WeaponDamageController.Instance == null) return 10; else return WeaponDamageController.Instance.WarMaceDamageMin; } }
        private int m_WarMaceDamageMin = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int WarMaceDamageMin { get { return m_WarMaceDamageMin; } set { m_WarMaceDamageMin = value; } }

        public static int _WarMaceDamageMax { get { if (WeaponDamageController.Instance == null) return 30; else return WeaponDamageController.Instance.WarMaceDamageMax; } }
        private int m_WarMaceDamageMax = 30;
        [CommandProperty(AccessLevel.GameMaster)]
        public int WarMaceDamageMax { get { return m_WarMaceDamageMax; } set { m_WarMaceDamageMax = value; } }

        public static int _BardicheDamageMin { get { if (WeaponDamageController.Instance == null) return 5; else return WeaponDamageController.Instance.BardicheDamageMin; } }
        private int m_BardicheDamageMin = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BardicheDamageMin { get { return m_BardicheDamageMin; } set { m_BardicheDamageMin = value; } }

        public static int _BardicheDamageMax { get { if (WeaponDamageController.Instance == null) return 43; else return WeaponDamageController.Instance.BardicheDamageMax; } }
        private int m_BardicheDamageMax = 43;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BardicheDamageMax { get { return m_BardicheDamageMax; } set { m_BardicheDamageMax = value; } }

        public static int _HalberdDamageMin { get { if (WeaponDamageController.Instance == null) return 9; else return WeaponDamageController.Instance.HalberdDamageMin; } }
        private int m_HalberdDamageMin = 9;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HalberdDamageMin { get { return m_HalberdDamageMin; } set { m_HalberdDamageMin = value; } }

        public static int _HalberdDamageMax { get { if (WeaponDamageController.Instance == null) return 49; else return WeaponDamageController.Instance.HalberdDamageMax; } }
        private int m_HalberdDamageMax = 49;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HalberdDamageMax { get { return m_HalberdDamageMax; } set { m_HalberdDamageMax = value; } }

        public static int _BowDamageMin { get { if (WeaponDamageController.Instance == null) return 9; else return WeaponDamageController.Instance.BowDamageMin; } }
        private int m_BowDamageMin = 9;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BowDamageMin { get { return m_BowDamageMin; } set { m_BowDamageMin = value; } }

        public static int _BowDamageMax { get { if (WeaponDamageController.Instance == null) return 41; else return WeaponDamageController.Instance.BowDamageMax; } }
        private int m_BowDamageMax = 41;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BowDamageMax { get { return m_BowDamageMax; } set { m_BowDamageMax = value; } }

        public static int _CrossbowDamageMin { get { if (WeaponDamageController.Instance == null) return 8; else return WeaponDamageController.Instance.CrossbowDamageMin; } }
        private int m_CrossbowDamageMin = 8;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CrossbowDamageMin { get { return m_CrossbowDamageMin; } set { m_CrossbowDamageMin = value; } }

        public static int _CrossbowDamageMax { get { if (WeaponDamageController.Instance == null) return 43; else return WeaponDamageController.Instance.CrossbowDamageMax; } }
        private int m_CrossbowDamageMax = 43;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CrossbowDamageMax { get { return m_CrossbowDamageMax; } set { m_CrossbowDamageMax = value; } }

        public static int _HeavyCrossbowDamageMin { get { if (WeaponDamageController.Instance == null) return 11; else return WeaponDamageController.Instance.HeavyCrossbowDamageMin; } }
        private int m_HeavyCrossbowDamageMin = 11;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HeavyCrossbowDamageMin { get { return m_HeavyCrossbowDamageMin; } set { m_HeavyCrossbowDamageMin = value; } }

        public static int _HeavyCrossbowDamageMax { get { if (WeaponDamageController.Instance == null) return 56; else return WeaponDamageController.Instance.HeavyCrossbowDamageMax; } }
        private int m_HeavyCrossbowDamageMax = 56;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HeavyCrossbowDamageMax { get { return m_HeavyCrossbowDamageMax; } set { m_HeavyCrossbowDamageMax = value; } }

        public static int _PitchforkDamageMin { get { if (WeaponDamageController.Instance == null) return 4; else return WeaponDamageController.Instance.PitchforkDamageMin; } }
        private int m_PitchforkDamageMin = 4;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PitchforkDamageMin { get { return m_PitchforkDamageMin; } set { m_PitchforkDamageMin = value; } }

        public static int _PitchforkDamageMax { get { if (WeaponDamageController.Instance == null) return 16; else return WeaponDamageController.Instance.PitchforkDamageMax; } }
        private int m_PitchforkDamageMax = 16;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PitchforkDamageMax { get { return m_PitchforkDamageMax; } set { m_PitchforkDamageMax = value; } }

        public static int _ShortSpearDamageMin { get { if (WeaponDamageController.Instance == null) return 4; else return WeaponDamageController.Instance.ShortSpearDamageMin; } }
        private int m_ShortSpearDamageMin = 4;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ShortSpearDamageMin { get { return m_ShortSpearDamageMin; } set { m_ShortSpearDamageMin = value; } }

        public static int _ShortSpearDamageMax { get { if (WeaponDamageController.Instance == null) return 32; else return WeaponDamageController.Instance.ShortSpearDamageMax; } }
        private int m_ShortSpearDamageMax = 32;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ShortSpearDamageMax { get { return m_ShortSpearDamageMax; } set { m_ShortSpearDamageMax = value; } }

        public static int _SpearDamageMin { get { if (WeaponDamageController.Instance == null) return 5; else return WeaponDamageController.Instance.SpearDamageMin; } }
        private int m_SpearDamageMin = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SpearDamageMin { get { return m_SpearDamageMin; } set { m_SpearDamageMin = value; } }

        public static int _SpearDamageMax { get { if (WeaponDamageController.Instance == null) return 32; else return WeaponDamageController.Instance.SpearDamageMax; } }
        private int m_SpearDamageMax = 32;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SpearDamageMax { get { return m_SpearDamageMax; } set { m_SpearDamageMax = value; } }

        public static int _TribalSpearDamageMin { get { if (WeaponDamageController.Instance == null) return 10; else return WeaponDamageController.Instance.TribalSpearDamageMin; } }
        private int m_TribalSpearDamageMin = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TribalSpearDamageMin { get { return m_TribalSpearDamageMin; } set { m_TribalSpearDamageMin = value; } }

        public static int _TribalSpearDamageMax { get { if (WeaponDamageController.Instance == null) return 37; else return WeaponDamageController.Instance.TribalSpearDamageMax; } }
        private int m_TribalSpearDamageMax = 37;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TribalSpearDamageMax { get { return m_TribalSpearDamageMax; } set { m_TribalSpearDamageMax = value; } }

        public static int _WarForkDamageMin { get { if (WeaponDamageController.Instance == null) return 4; else return WeaponDamageController.Instance.WarForkDamageMin; } }
        private int m_WarForkDamageMin = 4;
        [CommandProperty(AccessLevel.GameMaster)]
        public int WarForkDamageMin { get { return m_WarForkDamageMin; } set { m_WarForkDamageMin = value; } }

        public static int _WarForkDamageMax { get { if (WeaponDamageController.Instance == null) return 32; else return WeaponDamageController.Instance.WarForkDamageMax; } }
        private int m_WarForkDamageMax = 32;
        [CommandProperty(AccessLevel.GameMaster)]
        public int WarForkDamageMax { get { return m_WarForkDamageMax; } set { m_WarForkDamageMax = value; } }

        public static int _BlackStaffDamageMin { get { if (WeaponDamageController.Instance == null) return 8; else return WeaponDamageController.Instance.BlackStaffDamageMin; } }
        private int m_BlackStaffDamageMin = 8;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BlackStaffDamageMin { get { return m_BlackStaffDamageMin; } set { m_BlackStaffDamageMin = value; } }

        public static int _BlackStaffDamageMax { get { if (WeaponDamageController.Instance == null) return 33; else return WeaponDamageController.Instance.BlackStaffDamageMax; } }
        private int m_BlackStaffDamageMax = 33;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BlackStaffDamageMax { get { return m_BlackStaffDamageMax; } set { m_BlackStaffDamageMax = value; } }

        public static int _GnarledStaffDamageMin { get { if (WeaponDamageController.Instance == null) return 10; else return WeaponDamageController.Instance.GnarledStaffDamageMin; } }
        private int m_GnarledStaffDamageMin = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int GnarledStaffDamageMin { get { return m_GnarledStaffDamageMin; } set { m_GnarledStaffDamageMin = value; } }

        public static int _GnarledStaffDamageMax { get { if (WeaponDamageController.Instance == null) return 30; else return WeaponDamageController.Instance.GnarledStaffDamageMax; } }
        private int m_GnarledStaffDamageMax = 30;
        [CommandProperty(AccessLevel.GameMaster)]
        public int GnarledStaffDamageMax { get { return m_GnarledStaffDamageMax; } set { m_GnarledStaffDamageMax = value; } }

        public static int _QuarterStaffDamageMin { get { if (WeaponDamageController.Instance == null) return 8; else return WeaponDamageController.Instance.QuarterStaffDamageMin; } }
        private int m_QuarterStaffDamageMin = 8;
        [CommandProperty(AccessLevel.GameMaster)]
        public int QuarterStaffDamageMin { get { return m_QuarterStaffDamageMin; } set { m_QuarterStaffDamageMin = value; } }

        public static int _QuarterStaffDamageMax { get { if (WeaponDamageController.Instance == null) return 28; else return WeaponDamageController.Instance.QuarterStaffDamageMax; } }
        private int m_QuarterStaffDamageMax = 28;
        [CommandProperty(AccessLevel.GameMaster)]
        public int QuarterStaffDamageMax { get { return m_QuarterStaffDamageMax; } set { m_QuarterStaffDamageMax = value; } }

        public static int _ShepherdsCrookDamageMin { get { if (WeaponDamageController.Instance == null) return 3; else return WeaponDamageController.Instance.ShepherdsCrookDamageMin; } }
        private int m_ShepherdsCrookDamageMin = 3;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ShepherdsCrookDamageMin { get { return m_ShepherdsCrookDamageMin; } set { m_ShepherdsCrookDamageMin = value; } }

        public static int _ShepherdsCrookDamageMax { get { if (WeaponDamageController.Instance == null) return 12; else return WeaponDamageController.Instance.ShepherdsCrookDamageMax; } }
        private int m_ShepherdsCrookDamageMax = 12;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ShepherdsCrookDamageMax { get { return m_ShepherdsCrookDamageMax; } set { m_ShepherdsCrookDamageMax = value; } }

        public static int _BroadswordDamageMin { get { if (WeaponDamageController.Instance == null) return 5; else return WeaponDamageController.Instance.BroadswordDamageMin; } }
        private int m_BroadswordDamageMin = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BroadswordDamageMin { get { return m_BroadswordDamageMin; } set { m_BroadswordDamageMin = value; } }

        public static int _BroadswordDamageMax { get { if (WeaponDamageController.Instance == null) return 29; else return WeaponDamageController.Instance.BroadswordDamageMax; } }
        private int m_BroadswordDamageMax = 29;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BroadswordDamageMax { get { return m_BroadswordDamageMax; } set { m_BroadswordDamageMax = value; } }

        public static int _CutlassDamageMin { get { if (WeaponDamageController.Instance == null) return 6; else return WeaponDamageController.Instance.CutlassDamageMin; } }
        private int m_CutlassDamageMin = 6;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CutlassDamageMin { get { return m_CutlassDamageMin; } set { m_CutlassDamageMin = value; } }

        public static int _CutlassDamageMax { get { if (WeaponDamageController.Instance == null) return 28; else return WeaponDamageController.Instance.CutlassDamageMax; } }
        private int m_CutlassDamageMax = 28;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CutlassDamageMax { get { return m_CutlassDamageMax; } set { m_CutlassDamageMax = value; } }

        public static int _KatanaDamageMin { get { if (WeaponDamageController.Instance == null) return 5; else return WeaponDamageController.Instance.KatanaDamageMin; } }
        private int m_KatanaDamageMin = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int KatanaDamageMin { get { return m_KatanaDamageMin; } set { m_KatanaDamageMin = value; } }

        public static int _KatanaDamageMax { get { if (WeaponDamageController.Instance == null) return 26; else return WeaponDamageController.Instance.KatanaDamageMax; } }
        private int m_KatanaDamageMax = 26;
        [CommandProperty(AccessLevel.GameMaster)]
        public int KatanaDamageMax { get { return m_KatanaDamageMax; } set { m_KatanaDamageMax = value; } }

        public static int _KryssDamageMin { get { if (WeaponDamageController.Instance == null) return 3; else return WeaponDamageController.Instance.KryssDamageMin; } }
        private int m_KryssDamageMin = 3;
        [CommandProperty(AccessLevel.GameMaster)]
        public int KryssDamageMin { get { return m_KryssDamageMin; } set { m_KryssDamageMin = value; } }

        public static int _KryssDamageMax { get { if (WeaponDamageController.Instance == null) return 28; else return WeaponDamageController.Instance.KryssDamageMax; } }
        private int m_KryssDamageMax = 28;
        [CommandProperty(AccessLevel.GameMaster)]
        public int KryssDamageMax { get { return m_KryssDamageMax; } set { m_KryssDamageMax = value; } }

        public static int _LongswordDamageMin { get { if (WeaponDamageController.Instance == null) return 5; else return WeaponDamageController.Instance.LongswordDamageMin; } }
        private int m_LongswordDamageMin = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LongswordDamageMin { get { return m_LongswordDamageMin; } set { m_LongswordDamageMin = value; } }

        public static int _LongswordDamageMax { get { if (WeaponDamageController.Instance == null) return 33; else return WeaponDamageController.Instance.LongswordDamageMax; } }
        private int m_LongswordDamageMax = 33;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LongswordDamageMax { get { return m_LongswordDamageMax; } set { m_LongswordDamageMax = value; } }

        public static int _ScimitarDamageMin { get { if (WeaponDamageController.Instance == null) return 4; else return WeaponDamageController.Instance.ScimitarDamageMin; } }
        private int m_ScimitarDamageMin = 4;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ScimitarDamageMin { get { return m_ScimitarDamageMin; } set { m_ScimitarDamageMin = value; } }

        public static int _ScimitarDamageMax { get { if (WeaponDamageController.Instance == null) return 30; else return WeaponDamageController.Instance.ScimitarDamageMax; } }
        private int m_ScimitarDamageMax = 30;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ScimitarDamageMax { get { return m_ScimitarDamageMax; } set { m_ScimitarDamageMax = value; } }

        public static int _VikingSwordDamageMin { get { if (WeaponDamageController.Instance == null) return 6; else return WeaponDamageController.Instance.VikingSwordDamageMin; } }
        private int m_VikingSwordDamageMin = 6;
        [CommandProperty(AccessLevel.GameMaster)]
        public int VikingSwordDamageMin { get { return m_VikingSwordDamageMin; } set { m_VikingSwordDamageMin = value; } }

        public static int _VikingSwordDamageMax { get { if (WeaponDamageController.Instance == null) return 34; else return WeaponDamageController.Instance.VikingSwordDamageMax; } }
        private int m_VikingSwordDamageMax = 34;
        [CommandProperty(AccessLevel.GameMaster)]
        public int VikingSwordDamageMax { get { return m_VikingSwordDamageMax; } set { m_VikingSwordDamageMax = value; } }
    }
}