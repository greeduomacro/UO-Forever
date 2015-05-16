using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    class SpellDamageControllerT2A : Item
    {   
        private static SpellDamageControllerT2A m_Instance;
        public static SpellDamageControllerT2A Instance { get { return m_Instance; } }

        [Constructable]
        public SpellDamageControllerT2A()
            : base(0xEDC)
        {
            this.Name = "Spell Damage Controller T2A";
            this.Movable = false;
            this.Visible = false;

            if (m_Instance != null)
            {
                // there can only be one WeaponDamageController game stone in the world
                m_Instance.Location = this.Location;
                Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.Administrator, 0x489,
                    "Existing SpellDamageController has been moved to this location (DON'T DELETE IT!).");
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback<SpellDamageControllerT2A>(UpdateInstancePosition), this); 
            }
            else
                m_Instance = this;
        }

        public static void UpdateInstancePosition(SpellDamageControllerT2A attemptedConstruct)
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

        public SpellDamageControllerT2A(Serial serial) : base(serial) { }

        public override void Delete()
        {
            return; // can't delete it!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)5);//version

            //5
            writer.Write(_MagicArrowDamageDelay);
            writer.Write(_EnergyBoltDamageDelay);
            writer.Write(_FireballDamageDelay);
            //version 4
            writer.Write(_MindblastDamageDivisor);
            writer.Write(_NewTravellingSpellDelayValue);
            writer.Write(_OldTravellingSpellDelay);
            // version 3
            writer.Write((double)_SlayerMultiplier);
            writer.Write((bool)_CanStackExplosion);
            // version 2
            writer.Write((double)_PoisonSkillResistModifier);
            // version 1
            writer.Write((double)_ManaDrainResistChance);
            writer.Write((double)_ManaVampireResistChance);
            // version 0
            writer.Write((double)__MaxFirstPercent );
            writer.Write((double)__MaxSecondPercentMageryAffect );
            writer.Write((double)__SecondPercentCircleAffect );
            writer.Write((double)__GlobalResistChanceMultiplier );
            writer.Write((double)_FlameStrikeResistMultiplier );
            writer.Write((int)_FlameStrikeDamageMin );
            writer.Write((int)_FlameStrikeDamageMax );
            writer.Write((double)_FlameStrikeDelaySeconds );
            writer.Write((double)_ExplosionResistMultiplier );
            writer.Write((int)_ExplosionDamageMin );
            writer.Write((int)_ExplosionDamageMax );
            writer.Write((double)_ExplosionBaseDelay );
            writer.Write((int)_ExplosionDelayRandomTenths );
            writer.Write((double)_EnergyBoltResistMultiplier );
            writer.Write((int)_EnergyBoltDamageMin );
            writer.Write((int)_EnergyBoltDamageMax );
            writer.Write((double)_LightningResistMultiplier );
            writer.Write((int)_LightningDamageMin );
            writer.Write((int)_LightningDamageMax );
            writer.Write((double)_FireballResistMultiplier );
            writer.Write((int)_FireballDamageMin );
            writer.Write((int)_FireballDamageMax );
            writer.Write((double)_HarmResistMultiplier );
            writer.Write((int)_HarmDamageMin );
            writer.Write((int)_HarmDamageMax );
            writer.Write((double)_HarmFarDistanceMultiplier );
            writer.Write((double)_HarmMediumDistanceMultiplier );
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 5:
                    {
                        _MagicArrowDamageDelay = reader.ReadDouble();
                        _EnergyBoltDamageDelay = reader.ReadDouble();
                        _FireballDamageDelay = reader.ReadDouble();
                    }
                    goto case 4;
                case 4:
                {
                    _MindblastDamageDivisor = reader.ReadDouble();
                    _NewTravellingSpellDelayValue = reader.ReadDouble();
                    _OldTravellingSpellDelay = reader.ReadBool();
                }
                    goto case 3;
                case 3:
                    _SlayerMultiplier = reader.ReadDouble();
                    _CanStackExplosion = reader.ReadBool();
                    goto case 2;
                case 2:
                    _PoisonSkillResistModifier = reader.ReadDouble();
                    goto case 1;
                case 1:
                    _ManaDrainResistChance = reader.ReadDouble();
                    _ManaVampireResistChance = reader.ReadDouble();
                    goto case 0;
                case 0:
                    __MaxFirstPercent = reader.ReadDouble();
                    __MaxSecondPercentMageryAffect = reader.ReadDouble();
                    __SecondPercentCircleAffect = reader.ReadDouble();
                    __GlobalResistChanceMultiplier = reader.ReadDouble();
                    _FlameStrikeResistMultiplier = reader.ReadDouble();
                    _FlameStrikeDamageMin = reader.ReadInt();
                    _FlameStrikeDamageMax = reader.ReadInt();
                    _FlameStrikeDelaySeconds = reader.ReadDouble();
                    _ExplosionResistMultiplier = reader.ReadDouble();
                    _ExplosionDamageMin = reader.ReadInt();
                    _ExplosionDamageMax = reader.ReadInt();
                    _ExplosionBaseDelay = reader.ReadDouble();
                    _ExplosionDelayRandomTenths = reader.ReadInt();
                    _EnergyBoltResistMultiplier = reader.ReadDouble();
                    _EnergyBoltDamageMin = reader.ReadInt();
                    _EnergyBoltDamageMax = reader.ReadInt();
                    _LightningResistMultiplier = reader.ReadDouble();
                    _LightningDamageMin = reader.ReadInt();
                    _LightningDamageMax = reader.ReadInt();
                    _FireballResistMultiplier = reader.ReadDouble();
                    _FireballDamageMin = reader.ReadInt();
                    _FireballDamageMax = reader.ReadInt();
                    _HarmResistMultiplier = reader.ReadDouble();
                    _HarmDamageMin = reader.ReadInt();
                    _HarmDamageMax = reader.ReadInt();
                    _HarmFarDistanceMultiplier = reader.ReadDouble();
                    _HarmMediumDistanceMultiplier = reader.ReadDouble();
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

        public static double __MaxFirstPercent = 0.20;
        [CommandProperty(AccessLevel.Administrator)]
        public double _MaxFirstPercent { get { return __MaxFirstPercent; } set { __MaxFirstPercent = value; } }

        public static double __MaxSecondPercentMageryAffect = 0.16;
        [CommandProperty(AccessLevel.Administrator)]
        public double _MaxSecondPercentMageryAffect { get { return __MaxSecondPercentMageryAffect; } set { __MaxSecondPercentMageryAffect = value; } }

        public static double __SecondPercentCircleAffect = 0.05;
        [CommandProperty(AccessLevel.Administrator)]
        public double _SecondPercentCircleAffect { get { return __SecondPercentCircleAffect; } set { __SecondPercentCircleAffect = value; } }

        public static double __GlobalResistChanceMultiplier = 0.5;
        [CommandProperty(AccessLevel.Administrator)]
        public double _GlobalResistChanceMultiplier { get { return __GlobalResistChanceMultiplier; } set { __GlobalResistChanceMultiplier = value; } }

        
        
        public static double _ManaDrainResistChance = 0.85;
        [CommandProperty(AccessLevel.Administrator)]
        public double ManaDrainResistChance { get { return _ManaDrainResistChance; } set { _ManaDrainResistChance = value; } }

        public static double _ManaVampireResistChance = 0.98;
        [CommandProperty(AccessLevel.Administrator)]
        public double ManaVampireResistChance { get { return _ManaVampireResistChance; } set { _ManaVampireResistChance = value; } }

        public static double _SlayerMultiplier = 1.5;
        [CommandProperty(AccessLevel.Administrator)]
        public double SlayerMultiplier { get { return _SlayerMultiplier; } set { _SlayerMultiplier = value; } }

        public static bool _CanStackExplosion = true;
        [CommandProperty(AccessLevel.Administrator)]
        public bool CanStackExplosion { get { return _CanStackExplosion; } set { _CanStackExplosion = value; } }
        

        public static double _PoisonSkillResistModifier = 0.2;
        [CommandProperty(AccessLevel.Administrator)]
        public double PoisonSkillResistModifier { get { return _PoisonSkillResistModifier; } set { _PoisonSkillResistModifier = value; } }


        public static double _FlameStrikeResistMultiplier = 0.75;
        [CommandProperty(AccessLevel.Administrator)]
        public double FlameStrikeResistMultiplier { get { return _FlameStrikeResistMultiplier; } set { _FlameStrikeResistMultiplier = value; } }

        public static int _FlameStrikeDamageMin = 30;
        [CommandProperty(AccessLevel.Administrator)]
        public int FlameStrikeDamageMin { get { return _FlameStrikeDamageMin; } set { _FlameStrikeDamageMin = value; } }

        public static int _FlameStrikeDamageMax = 35;
        [CommandProperty(AccessLevel.Administrator)]
        public int FlameStrikeDamageMax { get { return _FlameStrikeDamageMax; } set { _FlameStrikeDamageMax = value; } }

        public static double _FlameStrikeDelaySeconds = 0.5;
        [CommandProperty(AccessLevel.Administrator)]
        public double FlameStrikeDelaySeconds { get { return _FlameStrikeDelaySeconds; } set { _FlameStrikeDelaySeconds = value; } }





        public static double _ExplosionResistMultiplier = 0.75;
        [CommandProperty(AccessLevel.Administrator)]
        public double ExplosionResistMultiplier { get { return _ExplosionResistMultiplier; } set { _ExplosionResistMultiplier = value; } }

        public static int _ExplosionDamageMin = 24;
        [CommandProperty(AccessLevel.Administrator)]
        public int ExplosionDamageMin { get { return _ExplosionDamageMin; } set { _ExplosionDamageMin = value; } }

        public static int _ExplosionDamageMax = 30;
        [CommandProperty(AccessLevel.Administrator)]
        public int ExplosionDamageMax { get { return _ExplosionDamageMax; } set { _ExplosionDamageMax = value; } }

        public static double _ExplosionBaseDelay = 2.7;
        [CommandProperty(AccessLevel.Administrator)]
        public double ExplosionBaseDelay { get { return _ExplosionBaseDelay; } set { _ExplosionBaseDelay = value; } }

        public static int _ExplosionDelayRandomTenths = 4;
        [CommandProperty(AccessLevel.Administrator)]
        public int ExplosionDelayRandomTenths { get { return _ExplosionDelayRandomTenths; } set { _ExplosionDelayRandomTenths = value; } }



        public static double _EnergyBoltResistMultiplier = 0.75;
        [CommandProperty(AccessLevel.Administrator)]
        public double EnergyBoltResistMultiplier { get { return _EnergyBoltResistMultiplier; } set { _EnergyBoltResistMultiplier = value; } }

        public static int _EnergyBoltDamageMin = 24;
        [CommandProperty(AccessLevel.Administrator)]
        public int EnergyBoltDamageMin { get { return _EnergyBoltDamageMin; } set { _EnergyBoltDamageMin = value; } }

        public static int _EnergyBoltDamageMax = 30;
        [CommandProperty(AccessLevel.Administrator)]
        public int EnergyBoltDamageMax { get { return _EnergyBoltDamageMax; } set { _EnergyBoltDamageMax = value; } }



        public static double _LightningResistMultiplier = 0.5;
        [CommandProperty(AccessLevel.Administrator)]
        public double LightningResistMultiplier { get { return _LightningResistMultiplier; } set { _LightningResistMultiplier = value; } }

        public static int _LightningDamageMin = 14;
        [CommandProperty(AccessLevel.Administrator)]
        public int LightningDamageMin { get { return _LightningDamageMin; } set { _LightningDamageMin = value; } }

        public static int _LightningDamageMax = 18;
        [CommandProperty(AccessLevel.Administrator)]
        public int LightningDamageMax { get { return _LightningDamageMax; } set { _LightningDamageMax = value; } }



        public static double _FireballResistMultiplier = 0.65;
        [CommandProperty(AccessLevel.Administrator)]
        public double FireballResistMultiplier { get { return _FireballResistMultiplier; } set { _FireballResistMultiplier = value; } }

        public static int _FireballDamageMin = 9;
        [CommandProperty(AccessLevel.Administrator)]
        public int FireballDamageMin { get { return _FireballDamageMin; } set { _FireballDamageMin = value; } }

        public static int _FireballDamageMax = 12;
        [CommandProperty(AccessLevel.Administrator)]
        public int FireballDamageMax { get { return _FireballDamageMax; } set { _FireballDamageMax = value; } }



        public static double _HarmResistMultiplier = 0.5;
        [CommandProperty(AccessLevel.Administrator)]
        public double HarmResistMultiplier { get { return _HarmResistMultiplier; } set { _HarmResistMultiplier = value; } }

        public static int _HarmDamageMin = 7;
        [CommandProperty(AccessLevel.Administrator)]
        public int HarmDamageMin { get { return _HarmDamageMin; } set { _HarmDamageMin = value; } }

        public static int _HarmDamageMax = 13;
        [CommandProperty(AccessLevel.Administrator)]
        public int HarmDamageMax { get { return _HarmDamageMax; } set { _HarmDamageMax = value; } }

        public static double _HarmFarDistanceMultiplier = 0.33;
        [CommandProperty(AccessLevel.Administrator)]
        public double HarmFarDistanceMultiplier { get { return _HarmFarDistanceMultiplier; } set { _HarmFarDistanceMultiplier = value; } }

        public static double _HarmMediumDistanceMultiplier = 0.66;
        [CommandProperty(AccessLevel.Administrator)]
        public double HarmMediumDistanceMultiplier { get { return _HarmMediumDistanceMultiplier; } set { _HarmMediumDistanceMultiplier = value; } }

        public static double _MindblastDamageDivisor = 2;
        [CommandProperty(AccessLevel.Administrator)]
        public double MindblastDamageDivisor { get { return _MindblastDamageDivisor; } set { _MindblastDamageDivisor = value; } }

        public static bool _OldTravellingSpellDelay = true;
        [CommandProperty(AccessLevel.Administrator)]
        public bool OldTravellingSpellDelay { get { return _OldTravellingSpellDelay; } set { _OldTravellingSpellDelay = value; } }

        public static double _NewTravellingSpellDelayValue = 1.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double NewTravellingSpellDelayValue { get { return _NewTravellingSpellDelayValue; } set { _NewTravellingSpellDelayValue = value; } }

        public static double _MagicArrowDamageDelay = 1.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double MagicArrowDamageDelay { get { return _MagicArrowDamageDelay; } set { _MagicArrowDamageDelay = value; } }

        public static double _EnergyBoltDamageDelay = 1.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double EnergyBoltDamageDelay { get { return _EnergyBoltDamageDelay; } set { _EnergyBoltDamageDelay = value; } }

        public static double _FireballDamageDelay = 1.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double FireballDamageDelay { get { return _FireballDamageDelay; } set { _FireballDamageDelay = value; } }



    }
}