using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    class ExplosionPotionController : Item
    {   
        private static ExplosionPotionController m_Instance;
        public static ExplosionPotionController Instance { get { return m_Instance; } }

        [Constructable]
        public ExplosionPotionController()
            : base(0xEDC)
        {
            this.Name = "Explosion Potion Controller";
            this.Movable = false;
            this.Visible = false;

            if (m_Instance != null)
            {
                // there can only be one ExplosionPotionController game stone in the world
                m_Instance.Location = this.Location;
                Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.Administrator, 0x489,
                    "Existing ExplosionPotionController has been moved to this location (DON'T DELETE IT!).");
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback<ExplosionPotionController>(UpdateInstancePosition), this); 
            }
            else
                m_Instance = this;
        }

        public static void UpdateInstancePosition(ExplosionPotionController attemptedConstruct)
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

        public ExplosionPotionController(Serial serial) : base(serial) { }

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
            //writer.Write((double)m_AllWeaponDamageMultiplier);

            writer.Write((double)_AlchemyBonusPercentageOfSkill);
            writer.Write((int)_AlchemyBonusMax);
            writer.Write((int)_GreaterExplosionPotDmgMin);
            writer.Write((int)_GreaterExplosionPotDmgMax);
            writer.Write((double)_GreaterExplosionPotTimeDelay);
            writer.Write((int)_ExplosionPotDmgMin);
            writer.Write((int)_ExplosionPotDmgMax);
            writer.Write((double)_ExplosionPotTimeDelay);
            writer.Write((int)_LesserExplosionPotDmgMin);
            writer.Write((int)_LesserExplosionPotDmgMax);
            writer.Write((double)_LesserExplosionPotTimeDelay);
            writer.Write((double)_ExplosionPotionHandsNotFreeDelay);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    _AlchemyBonusPercentageOfSkill = reader.ReadDouble();
                    _AlchemyBonusMax = reader.ReadInt();
                    _GreaterExplosionPotDmgMin = reader.ReadInt();
                    _GreaterExplosionPotDmgMax = reader.ReadInt();
                    _GreaterExplosionPotTimeDelay = reader.ReadDouble();
                    _ExplosionPotDmgMin = reader.ReadInt();
                    _ExplosionPotDmgMax = reader.ReadInt();
                    _ExplosionPotTimeDelay = reader.ReadDouble();
                    _LesserExplosionPotDmgMin = reader.ReadInt();
                    _LesserExplosionPotDmgMax = reader.ReadInt();
                    _LesserExplosionPotTimeDelay = reader.ReadDouble();
                    _ExplosionPotionHandsNotFreeDelay = reader.ReadDouble();
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

        public static double _AlchemyBonusPercentageOfSkill = 10.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double AlchemyBonusPercentageOfSkill { get { return _AlchemyBonusPercentageOfSkill; } set { _AlchemyBonusPercentageOfSkill = value; } }

        public static int _AlchemyBonusMax = 8;
        [CommandProperty(AccessLevel.Administrator)]
        public int AlchemyBonusMax { get { return _AlchemyBonusMax; } set { _AlchemyBonusMax = value; } }

        public static int _GreaterExplosionPotDmgMin = 8;
        [CommandProperty(AccessLevel.Administrator)]
        public int GreaterExplosionPotDmgMin { get { return _GreaterExplosionPotDmgMin; } set { _GreaterExplosionPotDmgMin = value; } }

        public static int _GreaterExplosionPotDmgMax = 14;
        [CommandProperty(AccessLevel.Administrator)]
        public int GreaterExplosionPotDmgMax { get { return _GreaterExplosionPotDmgMax; } set { _GreaterExplosionPotDmgMax = value; } }

        public static double _GreaterExplosionPotTimeDelay = 6.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double GreaterExplosionPotTimeDelay { get { return _GreaterExplosionPotTimeDelay; } set { _GreaterExplosionPotTimeDelay = value; } }

        public static int _ExplosionPotDmgMin = 8;
        [CommandProperty(AccessLevel.Administrator)]
        public int ExplosionPotDmgMin { get { return _ExplosionPotDmgMin; } set { _ExplosionPotDmgMin = value; } }

        public static int _ExplosionPotDmgMax = 10;
        [CommandProperty(AccessLevel.Administrator)]
        public int ExplosionPotDmgMax { get { return _ExplosionPotDmgMax; } set { _ExplosionPotDmgMax = value; } }

        public static double _ExplosionPotTimeDelay = 5.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double ExplosionPotTimeDelay { get { return _ExplosionPotTimeDelay; } set { _ExplosionPotTimeDelay = value; } }

        public static int _LesserExplosionPotDmgMin = 5;
        [CommandProperty(AccessLevel.Administrator)]
        public int LesserExplosionPotDmgMin { get { return _LesserExplosionPotDmgMin; } set { _LesserExplosionPotDmgMin = value; } }

        public static int _LesserExplosionPotDmgMax = 7;
        [CommandProperty(AccessLevel.Administrator)]
        public int LesserExplosionPotDmgMax { get { return _LesserExplosionPotDmgMax; } set { _LesserExplosionPotDmgMax = value; } }

        public static double _LesserExplosionPotTimeDelay = 4.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double LesserExplosionPotTimeDelay { get { return _LesserExplosionPotTimeDelay; } set { _LesserExplosionPotTimeDelay = value; } }

        public static double _ExplosionPotionHandsNotFreeDelay = 0.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double ExplosionPotionHandsNotFreeDelay { get { return _ExplosionPotionHandsNotFreeDelay; } set { _ExplosionPotionHandsNotFreeDelay = value; } }
    }
}