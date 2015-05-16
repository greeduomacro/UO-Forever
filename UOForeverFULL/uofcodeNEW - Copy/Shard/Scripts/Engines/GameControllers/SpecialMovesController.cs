using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    class SpecialMovesController : Item
    {   
        private static SpecialMovesController m_Instance;
        public static SpecialMovesController Instance { get { return m_Instance; } }

        [Constructable]
        public SpecialMovesController()
            : base(0xEDC)
        {
            this.Name = "Special Moves Controller";
            this.Movable = false;
            this.Visible = false;

            if (m_Instance != null)
            {
                // there can only be one SpecialMovesController game stone in the world
                m_Instance.Location = this.Location;
                Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.Administrator, 0x489,
                    "Existing SpecialMovesController has been moved to this location (DON'T DELETE IT!).");
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback<SpecialMovesController>(UpdateInstancePosition), this); 
            }
            else
                m_Instance = this;
        }

        public static void UpdateInstancePosition(SpecialMovesController attemptedConstruct)
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

        public SpecialMovesController(Serial serial) : base(serial) { }

        public override void Delete()
        {
            return; // can't delete it!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);//version
            //version 3
            writer.Write(_Allow2HanderExplosionPots);
            writer.Write(_MaceStaminaDrainModifier);
            // version 2
            writer.Write((double)_StealingRehideDelay);
            // version 1
            writer.Write((int)_TrackingMinDistance);
            writer.Write((double)_TrackingDistancePerSkillPoint);
            writer.Write((double)_TrackingSuccessSkillTest);
            writer.Write((int)_AccuracyHitBonusMaxPercent);
            writer.Write((double)_HitChanceFormulaDenomScalar);
            writer.Write((double)_DefensiveWrestlingMaxValue);
            // version 0
            //global attributes
            //writer.Write((double)m_AllWeaponDamageMultiplier);
            writer.Write((double)_StunPunchSeconds);
            writer.Write((int)_StunStaminaRequired);
            writer.Write((int)_DisarmStaminaRequired);
            writer.Write((double)_SpecialMoveChanceAtGM_GM);
            writer.Write((double)_SpearStunMaxChance);
            writer.Write((double)_SpearStunDurationSeconds);
            writer.Write((double)_CrushingBlowMaxChance);
            writer.Write((double)_ConcussionPoleArmMaxChance);
            writer.Write((double)_ConcussionAxeMaxChance);
            writer.Write((double)_ConcussionDurationSeconds);
            writer.Write((double)_PoisonChanceSwords);
            writer.Write((double)_PoisonChanceSpears);
            writer.Write((double)_PoisonChanceKnives);
            writer.Write((double)_PoisonSkillChanceMaxBonus);
            writer.Write((double)_PoisonChancePenaltyPerFollower);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 3:
                {
                    _Allow2HanderExplosionPots = reader.ReadBool();
                    _MaceStaminaDrainModifier = reader.ReadDouble();
                }
                    goto case 2;
                case 2:
                    _StealingRehideDelay = reader.ReadDouble();
                    goto case 1;
                case 1:
                    _TrackingMinDistance = reader.ReadInt();
                    _TrackingDistancePerSkillPoint = reader.ReadDouble();
                    _TrackingSuccessSkillTest = reader.ReadDouble();

                    _AccuracyHitBonusMaxPercent = reader.ReadInt();
                    _HitChanceFormulaDenomScalar = reader.ReadDouble();
                    _DefensiveWrestlingMaxValue = reader.ReadDouble();
                    goto case 0;
                case 0:
                    _StunPunchSeconds = reader.ReadDouble();
                    _StunStaminaRequired = reader.ReadInt();
                    _DisarmStaminaRequired = reader.ReadInt();
                    _SpecialMoveChanceAtGM_GM = reader.ReadDouble();
                    _SpearStunMaxChance = reader.ReadDouble();
                    _SpearStunDurationSeconds = reader.ReadDouble();
                    _CrushingBlowMaxChance = reader.ReadDouble();
                    _ConcussionPoleArmMaxChance = reader.ReadDouble();
                    _ConcussionAxeMaxChance = reader.ReadDouble();
                    _ConcussionDurationSeconds = reader.ReadDouble();
                    _PoisonChanceSwords = reader.ReadDouble();
                    _PoisonChanceSpears = reader.ReadDouble();
                    _PoisonChanceKnives = reader.ReadDouble();
                    _PoisonSkillChanceMaxBonus = reader.ReadDouble();
                    _PoisonChancePenaltyPerFollower = reader.ReadDouble();
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

        public static double _HitChanceFormulaDenomScalar = 1.75;
        [CommandProperty(AccessLevel.Administrator)]
        public double HitChanceFormulaDenomScalar { get { return _HitChanceFormulaDenomScalar; } set { _HitChanceFormulaDenomScalar = value; } }

        public static double _DefensiveWrestlingMaxValue = 110.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double DefensiveWrestlingMaxValue { get { return _DefensiveWrestlingMaxValue; } set { _DefensiveWrestlingMaxValue = value; } }

        public static int _AccuracyHitBonusMaxPercent = 0; // percent
        [CommandProperty(AccessLevel.Administrator)]
        public int AccuracyHitBonusMaxPercent { get { return _AccuracyHitBonusMaxPercent; } set { _AccuracyHitBonusMaxPercent = value; } }

        public static double _StunPunchSeconds = 3.5;
        [CommandProperty(AccessLevel.Administrator)]
        public double StunPunchSeconds { get { return _StunPunchSeconds; } set { _StunPunchSeconds = value; } }

        public static int _StunStaminaRequired = 15;
        [CommandProperty(AccessLevel.Administrator)]
        public int StunStaminaRequired { get { return _StunStaminaRequired; } set { _StunStaminaRequired = value; } }

        public static int _DisarmStaminaRequired = 15;
        [CommandProperty(AccessLevel.Administrator)]
        public int DisarmStaminaRequired { get { return _DisarmStaminaRequired; } set { _DisarmStaminaRequired = value; } }

        public static double _SpecialMoveChanceAtGM_GM = 0.5;
        [CommandProperty(AccessLevel.Administrator)]
        public double SpecialMoveChanceAtGM_GM { get { return _SpecialMoveChanceAtGM_GM; } set { _SpecialMoveChanceAtGM_GM = value; } }

        public static double _SpearStunMaxChance = 0.22;
        [CommandProperty(AccessLevel.Administrator)]
        public double SpearStunMaxChance { get { return _SpearStunMaxChance; } set { _SpearStunMaxChance = value; } }

        public static double _SpearStunDurationSeconds = 2.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double SpearStunDurationSeconds { get { return _SpearStunDurationSeconds; } set { _SpearStunDurationSeconds = value; } }

        public static double _CrushingBlowMaxChance = 0.20;
        [CommandProperty(AccessLevel.Administrator)]
        public double CrushingBlowMaxChance { get { return _CrushingBlowMaxChance; } set { _CrushingBlowMaxChance = value; } }

        public static double _CrushingBlowDamageMultiplier = 1.5;
        [CommandProperty(AccessLevel.Administrator)]
        public double CrushingBlowDamageMultiplier { get { return _CrushingBlowDamageMultiplier; } set { _CrushingBlowDamageMultiplier = value; } }

        public static double _ConcussionPoleArmMaxChance = 0.20;
        [CommandProperty(AccessLevel.Administrator)]
        public double ConcussionPoleArmMaxChance { get { return _ConcussionPoleArmMaxChance; } set { _ConcussionPoleArmMaxChance = value; } }

        public static double _ConcussionAxeMaxChance = 0.182;
        [CommandProperty(AccessLevel.Administrator)]
        public double ConcussionAxeMaxChance { get { return _ConcussionAxeMaxChance; } set { _ConcussionAxeMaxChance = value; } }

        public static double _ConcussionDurationSeconds = 30.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double ConcussionDurationSeconds { get { return _ConcussionDurationSeconds; } set { _ConcussionDurationSeconds = value; } }

        public static double _PoisonChanceSwords = 0.20;
        [CommandProperty(AccessLevel.Administrator)]
        public double PoisonChanceSwords { get { return _PoisonChanceSwords; } set { _PoisonChanceSwords = value; } }

        public static double _PoisonChanceSpears = 0.20;
        [CommandProperty(AccessLevel.Administrator)]
        public double PoisonChanceSpears { get { return _PoisonChanceSpears; } set { _PoisonChanceSpears = value; } }

        public static double _PoisonChanceKnives = 0.50;
        [CommandProperty(AccessLevel.Administrator)]
        public double PoisonChanceKnives { get { return _PoisonChanceKnives; } set { _PoisonChanceKnives = value; } }

        public static double _PoisonSkillChanceMaxBonus = 0.30;
        [CommandProperty(AccessLevel.Administrator)]
        public double PoisonSkillChanceMaxBonus { get { return _PoisonSkillChanceMaxBonus; } set { _PoisonSkillChanceMaxBonus = value; } }

        public static double _PoisonChancePenaltyPerFollower = 0.05;
        [CommandProperty(AccessLevel.Administrator)]
        public double PoisonChancePenaltyPerFollower { get { return _PoisonChancePenaltyPerFollower; } set { _PoisonChancePenaltyPerFollower = value; } }

        public static int _TrackingMinDistance = 0;
        [CommandProperty(AccessLevel.Administrator)]
        public int TrackingMinDistance { get { return _TrackingMinDistance; } set { _TrackingMinDistance = value; } }

        public static double _TrackingDistancePerSkillPoint = 0.35;
        [CommandProperty(AccessLevel.Administrator)]
        public double TrackingDistancePerSkillPoint { get { return _TrackingDistancePerSkillPoint; } set { _TrackingDistancePerSkillPoint = value; } }

        public static double _TrackingSuccessSkillTest = 40.0;
        [CommandProperty(AccessLevel.Administrator)]
        public double TrackingSuccessSkillTest { get { return _TrackingSuccessSkillTest; } set { _TrackingSuccessSkillTest = value; } }

        public static double _StealingRehideDelay = 0.25;
        [CommandProperty(AccessLevel.Administrator)]
        public double StealingRehideDelay { get { return _StealingRehideDelay; } set { _StealingRehideDelay = value; } }

        public static double _MaceStaminaDrainModifier = 0.7;
        [CommandProperty(AccessLevel.Administrator)]
        public double MaceStaminaDrainModifier { get { return _MaceStaminaDrainModifier; } set { _MaceStaminaDrainModifier = value; } }

        public static bool _Allow2HanderExplosionPots = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool Allow2HanderExplosionPots { get { return _Allow2HanderExplosionPots; } set { _Allow2HanderExplosionPots = value; } }
    }
}