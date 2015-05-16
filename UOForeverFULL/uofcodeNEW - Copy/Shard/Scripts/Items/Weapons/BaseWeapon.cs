#region References

using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Engines.XmlSpawner2;
using Server.Ethics;
using Server.Factions;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Spells;

#endregion

namespace Server.Items
{
    public interface ISlayer
    {
        SlayerName Slayer { get; set; }
        SlayerName Slayer2 { get; set; }
    }

    public abstract class BaseWeapon
        : Item, IWeapon, IFactionItem, ICraftable, ISlayer, IDurability, IEthicsItem, IIdentifiable
    {
        private string m_EngravedText;

        [CommandProperty(AccessLevel.GameMaster)]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set
            {
                m_EngravedText = value;
                InvalidateProperties();
            }
        }

        #region Ethics

        private EthicsItem m_EthicState;

        public EthicsItem EthicsItemState { get { return m_EthicState; } set { m_EthicState = value; } }

        #endregion

        #region Factions

        private FactionItem m_FactionState;

        public FactionItem FactionItemState { get { return m_FactionState; } set { m_FactionState = value; } }

        #endregion

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            DeathMoveResult result = base.OnParentDeath(parent);

            Ethic parentState = Ethic.Find(parent);

            if (IsAesthetic || parentState != null && result == DeathMoveResult.MoveToCorpse && m_EthicState != null &&
                m_EthicState.IsRunic && parentState == m_EthicState.Ethic)
            {
                return DeathMoveResult.MoveToBackpack;
            }
            else
            {
                return result;
            }
        }

        public override DeathMoveResult OnInventoryDeath(Mobile parent)
        {
            DeathMoveResult result = base.OnParentDeath(parent);
            Ethic parentState = Ethic.Find(parent);

            if (parentState != null && result == DeathMoveResult.MoveToCorpse && m_EthicState != null &&
                m_EthicState.IsRunic && parentState == m_EthicState.Ethic)
            {
                return DeathMoveResult.MoveToBackpack;
            }
            else
            {
                return result;
            }
        }

        /* Weapon internals work differently now (Mar 13 2003)
		 *
		 * The attributes defined below default to -1.
		 * If the value is -1, the corresponding virtual 'Aos/Old' property is used.
		 * If not, the attribute value itself is used. Here's the list:
		 *  - MinDamage
		 *  - MaxDamage
		 *  - Speed
		 *  - HitSound
		 *  - MissSound
		 *  - StrRequirement, DexRequirement, IntRequirement
		 *  - WeaponType
		 *  - WeaponAnimation
		 *  - MaxRange
		 */

        #region Var declarations

        // Instance values. These values are unique to each weapon.
        private WeaponDamageLevel m_DamageLevel;
        private WeaponAccuracyLevel m_AccuracyLevel;
        private WeaponDurabilityLevel m_DurabilityLevel;
        private WeaponQuality m_Quality;
        private Mobile m_Crafter;
        private Poison m_Poison;
        private int m_PoisonCharges;
        private bool m_Identified;
        private int m_Hits;
        private int m_MaxHits;
        private SlayerName m_Slayer;
        private SlayerName m_Slayer2;
        private SkillMod m_SkillMod, m_MageMod;
        private CraftResource m_Resource;
        private bool m_PlayerConstructed;

        //private bool m_Cursed; // Is this weapon cursed via Curse Weapon necromancer spell? Temporary; not serialized.
        //private bool m_Consecrated; // Is this weapon blessed via Consecrate Weapon paladin ability? Temporary; not serialized.

        //private AosAttributes m_AosAttributes;
        //private AosWeaponAttributes m_AosWeaponAttributes;
        //private AosSkillBonuses m_AosSkillBonuses;
        //private AosElementAttributes m_AosElementDamages;

        // Overridable values. These values are provided to override the defaults which get defined in the individual weapon scripts.
        private int m_StrReq, m_DexReq, m_IntReq;
        private int m_DamageMin, m_DamageMax;
        private int[] m_DiceDamage;
        private int m_HitSound, m_MissSound;
        private float m_Speed;
        private int m_MaxRange;
        private SkillName m_Skill;
        private WeaponType m_Type;
        private WeaponAnimation m_Animation;

        #endregion

        #region Virtual Properties

        public virtual WeaponAbility PrimaryAbility { get { return null; } }
        public virtual WeaponAbility SecondaryAbility { get { return null; } }

        public virtual int DefMaxRange { get { return 1; } }
        public virtual int DefHitSound { get { return 0; } }
        public virtual int DefMissSound { get { return 0; } }
        public virtual SkillName DefSkill { get { return SkillName.Swords; } }
        public virtual WeaponType DefType { get { return WeaponType.Slashing; } }
        public virtual WeaponAnimation DefAnimation { get { return WeaponAnimation.Slash1H; } }

        public virtual int OldStrengthReq { get { return 0; } }
        public virtual int OldDexterityReq { get { return 0; } }
        public virtual int OldIntelligenceReq { get { return 0; } }
        public virtual int NewMinDamage { get { return 0; } }
        public virtual int NewMaxDamage { get { return 0; } }
        public virtual int OldSpeed { get { return 0; } }

        public virtual int OldMaxRange { get { return DefMaxRange; } }
        public virtual int OldHitSound { get { return DefHitSound; } }
        public virtual int OldMissSound { get { return DefMissSound; } }
        public virtual SkillName OldSkill { get { return DefSkill; } }
        public virtual WeaponType OldType { get { return DefType; } }
        public virtual WeaponAnimation OldAnimation { get { return DefAnimation; } }

        public virtual int InitMinHits { get { return 0; } }
        public virtual int InitMaxHits { get { return 0; } }

        public virtual bool CanFortify { get { return true; } }

        public virtual SkillName AccuracySkill { get { return SkillName.Tactics; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DiceDamage { get { return Utility.Dice(1, 3, 0); } } // changed from 1 1 0

        #endregion

        #region Getters & Setters

        private bool m_Aesthetic;

        [CommandProperty(AccessLevel.GameMaster)] // check to see if Aesthetic
        public bool IsAesthetic
        {
            get { return m_Aesthetic; }
            set
            {
                StrRequirement = 0;
                DexRequirement = 0;
                DamageMin = 0;
                DamageMax = 0;
                m_Speed = 30;
                m_Aesthetic = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int OriginalItemID { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int OriginalHue { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAnimation OriginalAnimation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Identified
        {
            get
            {
                return (DamageLevel == 0 && DurabilityLevel == 0 && AccuracyLevel == 0 && Slayer == SlayerName.None &&
                        Slayer2 == SlayerName.None &&
                        (base.Hue == CraftResources.GetHue(m_Resource) ||
                         (Dyable && !(m_Resource > CraftResource.Valorite && m_Resource < CraftResource.RegularLeather)))) ||
                       m_Identified;
            }
            set
            {
                m_Identified = value;
                OnIdentified();
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get { return m_Hits; }
            set
            {
                if (m_Hits == value)
                {
                    return;
                }

                if (value > m_MaxHits)
                {
                    value = m_MaxHits;
                }

                m_Hits = value;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get { return m_MaxHits; }
            set
            {
                m_MaxHits = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonCharges
        {
            get { return m_PoisonCharges; }
            set
            {
                m_PoisonCharges = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get { return m_Poison; }
            set
            {
                m_Poison = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponQuality Quality
        {
            get { return m_Quality; }
            set
            {
                UnscaleDurability();
                m_Quality = value;
                ScaleDurability();
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer
        {
            get { return m_Slayer; }
            set
            {
                m_Slayer = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer2
        {
            get { return m_Slayer2; }
            set
            {
                m_Slayer2 = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                UnscaleDurability();
                m_Resource = value;
                Hue = CraftResources.GetHue(m_Resource);
                InvalidateProperties();
                ScaleDurability();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponDamageLevel DamageLevel
        {
            get { return m_DamageLevel; }
            set
            {
                m_DamageLevel = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponDurabilityLevel DurabilityLevel
        {
            get { return m_DurabilityLevel; }
            set
            {
                UnscaleDurability();
                m_DurabilityLevel = value;
                InvalidateProperties();
                ScaleDurability();
            }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Lead)]
        public bool PlayerConstructed { get { return m_PlayerConstructed; } set { m_PlayerConstructed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRange
        {
            get { return (m_MaxRange == -1 ? OldMaxRange : m_MaxRange); }
            set
            {
                m_MaxRange = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAnimation Animation
        {
            get { return (m_Animation == (WeaponAnimation) (-1) ? OldAnimation : m_Animation); }
            set { m_Animation = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponType Type
        {
            get { return (m_Type == (WeaponType) (-1) ? OldType : m_Type); }
            set { m_Type = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get { return (m_Skill == (SkillName) (-1) ? OldSkill : m_Skill); }
            set
            {
                m_Skill = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitSound
        {
            get { return (m_HitSound == -1 ? OldHitSound : m_HitSound); }
            set { m_HitSound = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MissSound
        {
            get { return (m_MissSound == -1 ? OldMissSound : m_MissSound); }
            set { m_MissSound = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageMin
        {
            get { return (m_DamageMin == -1 ? NewMinDamage : m_DamageMin); }
            set
            {
                m_DamageMin = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageMax
        {
            get { return (m_DamageMax == -1 ? NewMaxDamage : m_DamageMax); }
            set
            {
                m_DamageMax = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageDiceNumber { get { return m_DiceDamage[0]; } set { m_DiceDamage[0] = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageDiceSides { get { return m_DiceDamage[1]; } set { m_DiceDamage[1] = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageDiceBonus { get { return m_DiceDamage[2]; } set { m_DiceDamage[2] = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public float Speed
        {
            get
            {
                if (m_Speed != -1)
                {
                    return m_Speed;
                }

                return OldSpeed;
            }
            set
            {
                m_Speed = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StrRequirement
        {
            get { return (m_StrReq == -1 ? OldStrengthReq : m_StrReq); }
            set
            {
                m_StrReq = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DexRequirement
        {
            get { return (m_DexReq == -1 ? OldDexterityReq : m_DexReq); }
            set { m_DexReq = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int IntRequirement
        {
            get { return (m_IntReq == -1 ? OldIntelligenceReq : m_IntReq); }
            set { m_IntReq = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAccuracyLevel AccuracyLevel
        {
            get { return m_AccuracyLevel; }
            set
            {
                if (m_AccuracyLevel != value)
                {
                    m_AccuracyLevel = value;

                    if (UseSkillMod && m_Identified)
                    {
                        if (m_AccuracyLevel == WeaponAccuracyLevel.Regular)
                        {
                            if (m_SkillMod != null)
                            {
                                m_SkillMod.Remove();
                            }

                            m_SkillMod = null;
                        }
                        else if (m_SkillMod == null && Parent is Mobile)
                        {
                            m_SkillMod = new DefaultSkillMod(AccuracySkill, true, (int) m_AccuracyLevel * 5);
                            ((Mobile) Parent).AddSkillMod(m_SkillMod);
                        }
                        else if (m_SkillMod != null)
                        {
                            m_SkillMod.Value = (int) m_AccuracyLevel * 5;
                        }
                    }

                    InvalidateProperties();
                }
            }
        }

        #endregion

        public override Type DyeType { get { return typeof(RewardMetalDyeTub); } }

        public override bool Dye(Mobile from, IDyeTub sender)
        {
            if (!(m_Resource == CraftResource.None || m_Resource == CraftResource.Iron))
            {
                return false;
            }

            return base.Dye(from, sender);
        }

        public virtual void OnIdentified()
        {
            ReleaseWorldPackets();
            Delta(ItemDelta.Update);
        }

        public override void OnAfterDuped(Item newItem)
        {
            var weap = newItem as BaseWeapon;

            if (weap == null)
            {
                return;
            }
        }

        public virtual void UnscaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_Hits = ((m_Hits * 100) + (scale - 1)) / scale;
            m_MaxHits = ((m_MaxHits * 100) + (scale - 1)) / scale;
            InvalidateProperties();
        }

        public virtual void ScaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_Hits = ((m_Hits * scale) + 99) / 100;
            m_MaxHits = ((m_MaxHits * scale) + 99) / 100;
            InvalidateProperties();
        }

        public int GetDurabilityBonus()
        {
            int bonus = 0;

            if (m_Quality == WeaponQuality.Exceptional)
            {
                bonus += 20;
            }

            switch (m_DurabilityLevel)
            {
                case WeaponDurabilityLevel.Durable:
                    bonus += 20;
                    break;
                case WeaponDurabilityLevel.Substantial:
                    bonus += 50;
                    break;
                case WeaponDurabilityLevel.Massive:
                    bonus += 70;
                    break;
                case WeaponDurabilityLevel.Fortified:
                    bonus += 100;
                    break;
                case WeaponDurabilityLevel.Indestructible:
                    bonus += 120;
                    break;
            }

            return bonus;
        }

        public static void BlockEquip(Mobile m, TimeSpan duration)
        {
            if (m.BeginAction(typeof(BaseWeapon)))
            {
                new ResetEquipTimer(m, duration).Start();
            }
        }

        private class ResetEquipTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public ResetEquipTimer(Mobile m, TimeSpan duration)
                : base(duration)
            {
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                m_Mobile.EndAction(typeof(BaseWeapon));
            }
        }

        public override bool CheckConflictingLayer(Mobile m, Item item, Layer layer)
        {
            if (base.CheckConflictingLayer(m, item, layer))
            {
                return true;
            }

            if (Layer == Layer.TwoHanded && layer == Layer.OneHanded)
            {
                m.SendLocalizedMessage(500214); // You already have something in both hands.
                return true;
            }
            else if (Layer == Layer.OneHanded && layer == Layer.TwoHanded && !(item is BaseShield) &&
                     !(item is BaseEquipableLight))
            {
                m.SendLocalizedMessage(500215); // You can only wield one weapon at a time.
                return true;
            }

            return false;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            if (!Ethic.CheckTrade(from, to, newOwner, this))
            {
                return false;
            }

            return base.AllowSecureTrade(from, to, newOwner, accepted);
        }

        public virtual Race RequiredRace { get { return null; } }
        //On OSI, there are no weapons with race requirements, this is for custom stuff

        public override bool CanEquip(Mobile from)
        {
            if (!Ethic.CheckEquip(from, this))
            {
                return false;
            }

            if (RequiredRace != null && from.Race != RequiredRace)
            {
                if (RequiredRace == Race.Elf)
                {
                    from.SendLocalizedMessage(1072203); // Only Elves may use this.
                }
                else
                {
                    from.SendMessage("Only {0} may use this.", RequiredRace.PluralName);
                }

                return false;
            }
            else if (from.Dex < DexRequirement)
            {
                from.SendMessage("You are not nimble enough to equip that.");
                return false;
            }
            else if (from.Str < StrRequirement)
            {
                from.SendLocalizedMessage(500213); // You are not strong enough to equip that.
                return false;
            }
            else if (from.Int < IntRequirement)
            {
                from.SendMessage("You are not smart enough to equip that.");
                return false;
            }
            else if (!from.CanBeginAction(typeof(BaseWeapon)))
            {
                return false;
            }
            else
            {
                // XmlAttachment check for CanEquip
                if (!XmlAttach.CheckCanEquip(this, from))
                {
                    return false;
                }
                else
                {
                    return base.CanEquip(from);
                }
            }
        }

        public virtual bool UseSkillMod { get { return !EraAOS; } }

        public override bool OnEquip(Mobile from)
        {
            if (!from.IsT2A)
                from.NextCombatTime = DateTime.UtcNow + GetDelay(from);

            if (UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular ||
                UseSkillMod && m_EthicState != null && m_EthicState.IsRunic)
            {
                if (m_SkillMod != null)
                {
                    m_SkillMod.Remove();
                }

                if (m_Identified && !(this is BaseRanged))
                {
                    if (m_EthicState == null || m_EthicState != null && !m_EthicState.IsRunic)
                    {
                        m_SkillMod = new DefaultSkillMod(AccuracySkill, true, (int) m_AccuracyLevel * 5);
                    }
                    else if (m_EthicState != null && m_EthicState.IsRunic)
                    {
                        m_SkillMod = new DefaultSkillMod(AccuracySkill, true, (int) WeaponAccuracyLevel.Exceedingly * 5);
                    }
                    from.AddSkillMod(m_SkillMod);
                }
                else if (m_Identified && this is BaseRanged)
                {
                    if (m_EthicState == null || m_EthicState != null && !m_EthicState.IsRunic)
                    {
                        m_SkillMod = new DefaultSkillMod(SkillName.Archery, true, (int)m_AccuracyLevel * 5);
                    }
                    else if (m_EthicState != null && m_EthicState.IsRunic)
                    {
                        m_SkillMod = new DefaultSkillMod(SkillName.Archery, true, (int)WeaponAccuracyLevel.Exceedingly * 5);
                    }
                    from.AddSkillMod(m_SkillMod);
                }
            }

            // XmlAttachment check for OnEquip
            XmlAttach.CheckOnEquip(this, from);

            // true if return override
            if (XmlScript.HasTrigger(this, TriggerName.onEquip) &&
                UberScriptTriggers.Trigger(this, from, TriggerName.onEquip))
            {
                return false;
            }

            return true;
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
            {
                var from = (Mobile) parent;

                from.CheckStatTimers();
                from.Delta(MobileDelta.WeaponDamage);

                var talisman = from.FindItemOnLayer(Layer.Talisman) as TransmuteTalisman;
                if (talisman != null)
                {
                    if (OriginalItemID == 0)
                    {
                        OriginalItemID = ItemID;
                        OriginalHue = Hue;
                        OriginalAnimation = Animation;
                    }

                    if (talisman.TransmuteID != 0)
                    {
                        ItemID = talisman.TransmuteID;
                    }

                    if (talisman.TransmuteHue != 0)
                    {
                        Hue = talisman.TransmuteHue;
                    }

                    if (talisman.TransmuteAnimation != Animation)
                    {
                        Animation = talisman.TransmuteAnimation;
                    }
                }
            }
            else if (parent is Item)
            {
                var parentItem = (Item) parent;
                if (XmlScript.HasTrigger(this, TriggerName.onAdded))
                {
                    UberScriptTriggers.Trigger(this, parentItem.RootParentEntity as Mobile, TriggerName.onAdded,
                        parentItem);
                }
            }
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                var m = (Mobile) parent;
                var weapon = m.Weapon as BaseWeapon;

                string modName = Serial.ToString();

                m.RemoveStatMod(modName + "Str");
                m.RemoveStatMod(modName + "Dex");
                m.RemoveStatMod(modName + "Int");

                if (weapon != null && !m.IsT2A)
                {
                    m.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(m);
                }

                if (UseSkillMod && m_SkillMod != null)
                {
                    m_SkillMod.Remove();
                    m_SkillMod = null;
                }

                if (m_MageMod != null)
                {
                    m_MageMod.Remove();
                    m_MageMod = null;
                }

                m.CheckStatTimers();

                m.Delta(MobileDelta.WeaponDamage);

                if (XmlScript.HasTrigger(this, TriggerName.onUnequip))
                {
                    UberScriptTriggers.Trigger(this, (Mobile) parent, TriggerName.onUnequip);
                }

                var talisman = m.FindItemOnLayer(Layer.Talisman) as TransmuteTalisman;
                if (talisman != null)
                {
                    ItemID = OriginalItemID;
                    Hue = OriginalHue;
                    Animation = OriginalAnimation;
                }
            }
            else if (parent is Item)
            {
                var parentItem = (Item) parent;
                if (XmlScript.HasTrigger(this, TriggerName.onRemove))
                {
                    UberScriptTriggers.Trigger(this, parentItem.RootParentEntity as Mobile, TriggerName.onRemove,
                        parentItem);
                }
            }
            // XmlAttachment check for OnRemoved
            //Server.Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent); // Don't think we need this with Uberscript - Alan
        }

        public override void OnDelete()
        {
            if (XmlScript.HasTrigger(this, TriggerName.onDelete))
            {
                UberScriptTriggers.Trigger(this, RootParentEntity as Mobile, TriggerName.onDelete);
            }
            base.OnDelete();
        }

        public virtual SkillName GetUsedSkill(Mobile m, bool checkSkillAttrs)
        {
            SkillName sk;
            /*
			if ( checkSkillAttrs && m_AosWeaponAttributes.UseBestSkill != 0 )
			{
				double swrd = m.Skills[SkillName.Swords].Value;
				double fenc = m.Skills[SkillName.Fencing].Value;
				double mcng = m.Skills[SkillName.Macing].Value;
				double val;

				sk = SkillName.Swords;
				val = swrd;

				if ( fenc > val ){ sk = SkillName.Fencing; val = fenc; }
				if ( mcng > val ){ sk = SkillName.Macing; val = mcng; }
			}
			else if ( m_AosWeaponAttributes.MageWeapon != 0 )
			{
				if ( m.Skills[SkillName.Magery].Value > m.Skills[Skill].Value )
					sk = SkillName.Magery;
				else
					sk = Skill;
			}
			else */
            {
                sk = Skill;

                if (sk != SkillName.Wrestling && !m.Player && !m.Body.IsHuman &&
                    m.Skills[SkillName.Wrestling].Value > m.Skills[sk].Value)
                {
                    sk = SkillName.Wrestling;
                }
            }

            return sk;
        }

        public virtual double GetAttackSkillValue(Mobile attacker, Mobile defender)
        {
            return attacker.Skills[GetUsedSkill(attacker, true)].Value;
        }

        public virtual double GetDefendSkillValue(Mobile attacker, Mobile defender)
        {
            return defender.Skills[GetUsedSkill(defender, true)].Value;
        }

        public virtual bool CheckHit(Mobile attacker, Mobile defender)
        {
            var atkWeapon = attacker.Weapon as BaseWeapon;
            var defWeapon = defender.Weapon as BaseWeapon;

            Skill atkSkill = attacker.Skills[atkWeapon.Skill];

            double atkValue = atkWeapon.GetAttackSkillValue(attacker, defender);
            double defValue = defWeapon.GetDefendSkillValue(attacker, defender);

            double ourValue, theirValue;

            int bonus = GetHitChanceBonus(attacker);

            ourValue = (atkValue + 50.0);
            theirValue = (defValue + 50.0);

            double chance = ourValue / (theirValue * SpecialMovesController._HitChanceFormulaDenomScalar);

            chance *= 1.0 + (bonus / 100.0);

            if (chance < 0.10)
            {
                chance = 0.10;
            }

            return attacker.CheckSkill(atkSkill.SkillName, chance);
        }

        public virtual TimeSpan GetDelay(Mobile m)
        {
            double speed;

            if (m.IsArcade)
            {
                speed = 0;
            }
            else
            {
                speed = Speed;
            }

            if (speed == 0)
            {
                return TimeSpan.FromMinutes(1.0);
            }

            double delayInSeconds;


            double v = ((Math.Max(m.Stam, 10) + 100) * speed);

            if (v <= 1300.0)
            {
                v = 1300.0;
            }

            delayInSeconds = 15000.0 / v;
            
            return TimeSpan.FromSeconds(delayInSeconds);
        }

        public virtual void OnBeforeSwing(Mobile attacker, Mobile defender)
        {
            WeaponAbility a = WeaponAbility.GetCurrentAbility(attacker);

            if (a != null && !a.OnBeforeSwing(attacker, defender))
            {
                WeaponAbility.ClearCurrentAbility(attacker);
            }
        }

        public virtual TimeSpan OnSwing(Mobile attacker, Mobile defender)
        {
            return OnSwing(attacker, defender, 1.0);
        }

        public virtual bool CanSwing(Mobile attacker, Mobile defender)
        {
            if (IsAesthetic)
            {
                attacker.SendMessage(61, "Attacking someone with this weapon would cause it to break!");
                return false;
            }
            var sp = attacker.Spell as Spell;
            var p = attacker as PlayerMobile;
            return !( /*attacker.Paralyzed ||*/ attacker.Frozen) && (sp == null || !sp.IsCasting || !sp.BlocksMovement) &&
                   (p == null || p.PeacedUntil <= DateTime.UtcNow);
        }

        public virtual TimeSpan OnSwing(Mobile attacker, Mobile defender, double damageBonus)
        {
            bool canSwing = CanSwing(attacker, defender);

            if (attacker is PlayerMobile)
            {
                var pm = (PlayerMobile) attacker;

                if (pm.DuelContext != null && !pm.DuelContext.CheckItemEquip(attacker, this))
                {
                    canSwing = false;
                }
            }

            if (canSwing && attacker.HarmfulCheck(defender))
            {
                // true if return override is encountered
                if (XmlScript.HasTrigger(this, TriggerName.onSwing) &&
                    UberScriptTriggers.Trigger(this, defender, TriggerName.onSwing))
                {
                    return GetDelay(attacker);
                }

                attacker.DisruptiveAction();

                if (attacker.NetState != null)
                {
                    attacker.Send(new Swing(0, attacker, defender));
                }

                if (attacker is BaseCreature)
                {
                    var bc = (BaseCreature) attacker;
                    WeaponAbility ab = bc.GetWeaponAbility();

                    if (ab != null)
                    {
                        if (bc.WeaponAbilityChance > Utility.RandomDouble())
                        {
                            WeaponAbility.SetCurrentAbility(bc, ab);
                        }
                        else
                        {
                            WeaponAbility.ClearCurrentAbility(bc);
                        }
                    }
                }

                if (CheckHit(attacker, defender))
                {
                    OnHit(attacker, defender, damageBonus);
                }
                else
                {
                    OnMiss(attacker, defender);
                }
            }

            return GetDelay(attacker);
        }

        #region Sounds

        public virtual int GetHitAttackSound(Mobile attacker, Mobile defender)
        {
            int sound = attacker.GetAttackSound();

            if (sound == -1)
            {
                sound = HitSound;
            }

            return sound;
        }

        public virtual int GetHitDefendSound(Mobile attacker, Mobile defender)
        {
            return defender.GetHurtSound();
        }

        public virtual int GetMissAttackSound(Mobile attacker, Mobile defender)
        {
            if (attacker.GetAttackSound() == -1)
            {
                return MissSound;
            }
            else
            {
                return -1;
            }
        }

        public virtual int GetMissDefendSound(Mobile attacker, Mobile defender)
        {
            return -1;
        }

        #endregion

        public static bool CheckParry(Mobile defender)
        {
            if (defender == null)
            {
                return false;
            }

            var shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

            double parry = defender.Skills[SkillName.Parry].Value;
            /*double bushidoNonRacial = defender.Skills[SkillName.Bushido].NonRacialValue;
			double bushido = defender.Skills[SkillName.Bushido].Value;*/

            if (shield != null)
            {
                double chance = (parry /*- bushidoNonRacial*/) / 1000.0;
                // As per OSI, no negitive effect from the Racial stuffs, ie, 120 parry and '0' bushido with humans

                if (chance < 0) // chance shouldn't go below 0
                {
                    chance = 0;
                }

                // Parry/Bushido over 100 grants a 5% bonus.
                if (parry >= 100.0 /*|| bushido >= 100.0*/)
                {
                    chance += 0.05;
                }

                /*// Evasion grants a variable bonus post ML. 50% prior.
				if ( Evasion.IsEvading( defender ) )
					chance *= Evasion.GetParryScalar( defender );*/

                // Low dexterity lowers the chance.
                if (defender.Dex < 80)
                {
                    chance = chance * (20 + defender.Dex) / 100;
                }

                return defender.CheckSkill(SkillName.Parry, chance);
            }
            /*else if ( !(defender.Weapon is Fists) && !(defender.Weapon is BaseRanged) )
			{
				BaseWeapon weapon = defender.Weapon as BaseWeapon;

				double divisor = (weapon.Layer == Layer.OneHanded) ? 48000.0 : 41140.0;

				double chance = (parry * bushido) / divisor;

				double aosChance = parry / 800.0;

				// Parry or Bushido over 100 grant a 5% bonus.
				if( parry >= 100.0 )
				{
					chance += 0.05;
					aosChance += 0.05;
				}
				else if( bushido >= 100.0 )
				{
					chance += 0.05;
				}

				// Evasion grants a variable bonus post ML. 50% prior.
				if( Evasion.IsEvading( defender ) )
					chance *= Evasion.GetParryScalar( defender );

				// Low dexterity lowers the chance.
//				if( defender.Dex < 80 )
//					chance = chance * (20 + defender.Dex) / 100;

				if ( chance > aosChance )
					return defender.CheckSkill( SkillName.Parry, chance );
				else
					return (aosChance > Utility.RandomDouble()); // Only skillcheck if wielding a shield & there's no effect from Bushido
			}*/

            return false;
        }

        public virtual int AbsorbDamageAOS(Mobile attacker, Mobile defender, int damage)
        {
            bool blocked = false;

            if (defender.Player || defender.Body.IsHuman)
            {
                blocked = CheckParry(defender);

                if (blocked)
                {
                    defender.FixedEffect(0x37B9, 10, 16);
                    damage = 0;

                    var shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

                    if (shield != null)
                    {
                        shield.OnHit(this, attacker, damage);
                    }
                }
            }

            if (!blocked)
            {
                double positionChance = Utility.RandomDouble();
                double positionchance2 = positionChance;

                Item armorItem = null;

                positionchance2 -= 0.44;
                if (positionchance2 <= 0)
                {
                    armorItem = defender.ChestArmor;
                }

                positionchance2 -= 0.14;
                if (positionchance2 <= 0)
                {
                    armorItem = defender.LegsArmor;
                }

                positionchance2 -= 0.14;
                if (positionchance2 <= 0)
                {
                    armorItem = defender.HeadArmor;
                }

                positionchance2 -= 0.14;
                if (positionchance2 <= 0)
                {
                    armorItem = defender.ArmsArmor;
                }

                positionchance2 -= 0.07;
                if (positionchance2 <= 0)
                {
                    armorItem = defender.HandArmor;
                }

                if (armorItem == null)
                {
                    armorItem = defender.NeckArmor;
                }

                var armor = armorItem as IWearableDurability;

                if (armor != null)
                {
                    armor.OnHit(this, attacker, damage); // call OnHit to lose durability
                }
            }

            return damage;
        }

        public virtual int AbsorbDamage(Mobile attacker, Mobile defender, int damage)
        {
            //if ( EraAOS )
            //	return AbsorbDamageAOS( attacker, defender, damage );
            bool blocked = false;
            double chance = Utility.RandomDouble();
            BaseShield shield = null;
            var bc = defender as BaseCreature;

            // if player or pseudoseer controlled mob, allow parry
            if (defender.Player || defender.Body.IsHuman || (bc != null && !bc.Deleted && bc.NetState != null))
            {
                blocked = CheckParry(defender);

                if (blocked)
                {
                    int origdamage = damage;
                    defender.FixedEffect(0x37B9, 10, 16);
                    damage = 0;
                    defender.SendMessage("You parry your attackers blow!");

                    shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

                    if (shield != null)
                    {
                        shield.OnHit(this, attacker, origdamage);
                    }
                }
            }

            if (!blocked)
            {
                double positionChance = Utility.RandomDouble();
                double positionchance2 = positionChance;

                Item armorItem;
                if (chance < 0.07)
                    armorItem = defender.NeckArmor;
                else if (chance < 0.14)
                    armorItem = defender.HandArmor;
                else if (chance < 0.28)
                    armorItem = defender.ArmsArmor;
                else if (chance < 0.43)
                    armorItem = defender.HeadArmor;
                else if (chance < 0.65)
                    armorItem = defender.LegsArmor;
                else
                    armorItem = defender.ChestArmor;

                var armor = armorItem as IWearableDurability;

                if (armor != null)
                {
                    damage = armor.OnHit(this, attacker, damage);
                }

                shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;
                if (shield != null)
                {
                    damage = shield.OnHit(this, attacker, damage);
                }
            }

            int virtualArmor = defender.VirtualArmor + defender.VirtualArmorMod;

            if (virtualArmor > 0)
            {
                double scalar;

                if (chance < 0.14)
                {
                    scalar = 0.07;
                }
                else if (chance < 0.28)
                {
                    scalar = 0.14;
                }
                else if (chance < 0.43)
                {
                    scalar = 0.15;
                }
                else if (chance < 0.65)
                {
                    scalar = 0.22;
                }
                else
                {
                    scalar = 0.35;
                }

                int from = (int) (virtualArmor * scalar) / 2;
                var to = (int) (virtualArmor * scalar);

                damage -= Utility.Random(from, (to - from) + 1);
            }
            // XmlAttachment check for OnArmorHit -- NOT USED ANYMORE (use uberscript instead) -Alan
            //damage -= Server.Engines.XmlSpawner2.XmlAttach.OnArmorHit(attacker, defender, armorItem, this, damage);
            //damage -= Server.Engines.XmlSpawner2.XmlAttach.OnArmorHit(attacker, defender, shield, this, damage);
            return damage;
        }

        public virtual int GetPackInstinctBonus(Mobile attacker, Mobile defender)
        {
            int bonus = 0;

            var bc = attacker as BaseCreature;

            if (bc != null && bc.PackInstinct != PackInstinct.None && (bc.Controlled || bc.Summoned))
            {
                Mobile master = bc.ControlMaster;

                if (master == null)
                {
                    master = bc.SummonMaster;
                }
                if (master == null) // still possible if controlled set by GM
                {
                    return 0;
                }

                var pm = master as PlayerMobile;

                if (pm != null)
                {
                    foreach (Mobile m in pm.AllFollowers)
                    {
                        if (m != attacker && m is BaseCreature)
                        {
                            var tc = (BaseCreature) m;

                            if (!tc.IsStabled && tc.InRange(master, 3) && (tc.PackInstinct & bc.PackInstinct) != 0 &&
                                (tc.Controlled || tc.Summoned) && tc.Combatant == defender)
                            {
                                bonus += 25;
                            }
                        }
                    }
                }
                else
                {
                    foreach (Mobile m in master.GetMobilesInRange(3))
                    {
                        if (m != attacker && m is BaseCreature)
                        {
                            var tc = (BaseCreature) m;

                            if ((tc.PackInstinct & bc.PackInstinct) != 0 && (tc.Controlled || tc.Summoned))
                            {
                                Mobile tcMaster = tc.GetMaster();

                                if (tcMaster == master && tc.Combatant == defender)
                                {
                                    bonus += 25;
                                }
                            }
                        }
                    }
                }
            }

            return bonus;
        }

        private static bool m_InDoubleStrike;

        public static bool InDoubleStrike { get { return m_InDoubleStrike; } set { m_InDoubleStrike = value; } }

        public void OnHit(Mobile attacker, Mobile defender)
        {
            OnHit(attacker, defender, 1.0);
        }

        public virtual void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            if (!attacker.IsT2A)
            {
                PlaySwingAnimation(attacker);
                PlayHurtAnimation(defender);
            }

            attacker.PlaySound(GetHitAttackSound(attacker, defender));
            defender.PlaySound(GetHitDefendSound(attacker, defender));

            if (m_EthicState != null && Notoriety.Compute(attacker, defender) == Notoriety.Innocent)
            {
                Ethic.DestoryItem(attacker, this);
            }

            if (m_FactionState != null && Notoriety.Compute(attacker, defender) == Notoriety.Innocent)
            {
                FactionItemState.Detach();
                attacker.SendMessage(54,
                    "Attacking an innocent with this faction weapon has caused it to lose its faction blessing.");
            }

            int damage = ComputeDamage(attacker, defender);

            if (m_EthicState != null && m_EthicState.IsRunic && (Notoriety.Compute(attacker, defender) == Notoriety.Innocent || defender is BaseCreature))
            //Assume that Ethic is never null for items which have ethical states
            {
                attacker.SendMessage(54, "Your ethic weapon does not appear to be very effective against this being.");
                damage -= 6; //weapon is less effective against same ethic
            }

            #region Damage Multipliers

            /*
			 * The following damage bonuses multiply damage by a factor.
			 * Capped at x3 (300%).
			 */
            //double factor = 1.0;
            int percentageBonus = 0;

            WeaponAbility a = WeaponAbility.GetCurrentAbility(attacker);
            if (a != null)
            {
                //factor *= a.DamageScalar;
                percentageBonus += (int) (a.DamageScalar * 100) - 100;
            }

            percentageBonus += (int) (damageBonus * 100) - 100;

            CheckSlayerResult cs = CheckSlayers(attacker, defender);

            if (cs != CheckSlayerResult.None)
            {
                if (cs == CheckSlayerResult.Slayer)
                {
                    defender.FixedEffect(0x37B9, 10, 5);
                }

                //factor *= 2.0;
                percentageBonus += 100;
            }

            int packInstinctBonus = GetPackInstinctBonus(attacker, defender);

            percentageBonus += packInstinctBonus;

            if (m_InDoubleStrike)
            {
                //factor *= 0.9; // 10% loss when attacking with double-strike
                percentageBonus -= 10;
            }

            //if ( factor > 3.0 )
            //	factor = 3.0;
            percentageBonus = Math.Min(percentageBonus, 300);

            //damage = (int)(damage * factor);
            damage = AOS.Scale(damage, 100 + percentageBonus);

            #endregion

            // NOTE: Currently these do nothing except for particular mobs
            // e.g. SavageShaman, Savage, SavageRider, ChaoseDragoon, ChaosDragoonElite, etc., 
            // do 3x damage to dragons, nightmares, hiryus, and what-not
            // Deathwatchbeetle has a 50% chance to use 14 mana to do a "crushing attack"
            // MadSorceress & EmeraldOoze does 10x damage to other basecreatures
            // Glacial Giant does 2x damage to other basecreatures
            // Undead Captain does 15x damage to other basecreatures
            // Undead Pirate does 4x damage to other basecreatures
            // Basilisk does 2x damage to other basecreatures (if NOT tamed)

            // NOTE: it could be interesting to make it so mobs can extra damage to tamed creatures
            // to make tamers less effective (Amazon Queen does 2x damage to tames / summons)

            if (attacker is BaseCreature)
            {
                ((BaseCreature) attacker).AlterMeleeDamageTo(defender, ref damage);
            }

            if (defender is BaseCreature)
            {
                ((BaseCreature) defender).AlterMeleeDamageFrom(attacker, ref damage);
            }

            // Global Damage controller
            if (WeaponDamageController.Instance != null)
            {
                if ((attacker.Player || attacker.NetState != null) && defender is BaseCreature &&
                    !((BaseCreature) defender).TakesNormalDamage)
                {
                    // Player attacking BaseCreature
                    damage = (int) Math.Round(damage * WeaponDamageController._PlayerVsMobMultiplier);
                }

                if (!attacker.Player && attacker.NetState == null && defender.Player)
                {
                    // BaseCreature attacking Player
                    damage = (int) Math.Round(damage * WeaponDamageController._MobVsPlayerMultiplier);
                    damage -= WeaponDamageController._MobVsPlayerReduction;
                }
            }

            // Reduce damage with Armor / Parry
            damage = AbsorbDamage(attacker, defender, damage);

            // Halve the computed damage and return
            // .... all right we'll have the multiplier by 0.5 by default
            damage = (int) Math.Ceiling(damage * WeaponDamageController._AllWeaponDamageMultiplier);

            if (damage < 1)
            {
                damage = 1;
            }

            AddBlood(attacker, defender, damage);

            int damageGiven = damage;

            if (a != null && !a.OnBeforeDamage(attacker, defender))
            {
                WeaponAbility.ClearCurrentAbility(attacker);
                a = null;
            }
            // return if return override has been encountered
            // NOTE this would skip an XmlOnHit attachment's action
            // also, armor damage isn't skipped.
            if (XmlScript.HasTrigger(this, TriggerName.onWeaponHit))
            {
                UberScriptTriggers.Trigger(this, attacker, TriggerName.onWeaponHit, null, null, null, damageGiven);
            }

            if ((XmlScript.HasTrigger(attacker, TriggerName.onGivenHit) &&
                 UberScriptTriggers.Trigger(attacker, defender, TriggerName.onGivenHit, this, null, null, damageGiven)) ||
                (XmlScript.HasTrigger(defender, TriggerName.onTakenHit) &&
                 UberScriptTriggers.Trigger(defender, attacker, TriggerName.onTakenHit, this, null, null, damageGiven)))
            {
                return;
            }

            defender.Damage(damageGiven, attacker);

            if (attacker.IsT2A)
            {
                PlaySwingAnimation(attacker);
                PlayHurtAnimation(defender);
            }


            bool acidblood = (MaxRange <= 1 && (defender is Slime || defender is ToxicElemental));

            // this adjustment is scaling katana speed (58) by halberd speed (25)
            double durabilityLossAdjustment = 1.0 + (58.0 - (Speed <= 0 ? 1 : Speed)) / (58.0 - 25.0);
            // = 1 for katana, 2 for halberd
            durabilityLossAdjustment *= WeaponDamageController._WeaponDurabilitySpeedAdjustment;
            if (defender is BaseCreature)
            {
                durabilityLossAdjustment *= WeaponDamageController._WeaponDurabilityLossVsMobs;
            }

            if (!IsAesthetic && m_MaxHits > 0 &&
                (acidblood ||
                 Utility.RandomDouble() <
                 (WeaponDamageController._WeaponDurabilityLossPercentage * durabilityLossAdjustment)))
                // Stratics says 50% chance, seems more like 4%..
            {
                if (acidblood)
                {
                    attacker.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500263);
                        // *Splashing acid blood scars your weapon!*
                }

                if (m_Hits > 0)
                {
                    --HitPoints;
                }
                else if (m_MaxHits > 1)
                {
                    --MaxHitPoints;

                    if (Parent is Mobile)
                    {
                        ((Mobile) Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121);
                            // Your equipment is severely damaged.
                    }
                }
                else
                {
                    Delete();
                }
            }

            // OnGaveMeleeAttack used (for example) for life drain and other effects
            if (attacker is BaseCreature)
            {
                ((BaseCreature) attacker).OnGaveMeleeAttack(defender);
            }
            if (defender is BaseCreature)
            {
                ((BaseCreature) defender).OnGotMeleeAttack(attacker);
            }

            if (a != null)
            {
                a.OnHit(attacker, defender, damage);
            }

            // hook for attachment OnWeaponHit method
            //Server.Engines.XmlSpawner2.XmlAttach.OnWeaponHit(this, attacker, defender, damageGiven);
        }

        public virtual CheckSlayerResult CheckSlayers(Mobile attacker, Mobile defender)
        {
            // ==== Alan Mod =====
            if (defender is PlayerMobile || defender is HumanMob) // there are no slayers that should slay them
            {
                return CheckSlayerResult.None;
            }
            // first check for XmlSlayer attachments
            List<XmlSlayer> slayerattachments = XmlAttach.GetSlayerAttachments(attacker);
            if (slayerattachments != null)
            {
                foreach (XmlSlayer slayerattachment in slayerattachments)
                {
                    if (slayerattachment.Slayer == SlayerName.None)
                    {
                        continue;
                    }
                    else if (slayerattachment.Slayer == SlayerName.All)
                    {
                        return CheckSlayerResult.Slayer;
                    }
                    SlayerEntry attachmentSlayer = SlayerGroup.GetEntryByName(slayerattachment.Slayer);

                    if (attachmentSlayer != null && attachmentSlayer.Slays(defender))
                    {
                        return CheckSlayerResult.Slayer;
                    }
                }
            }
            // ==== End Alan Mod ====

            var atkWeapon = attacker.Weapon as BaseWeapon;
            SlayerEntry atkSlayer = SlayerGroup.GetEntryByName(atkWeapon.Slayer);
            SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName(atkWeapon.Slayer2);

            if (atkWeapon.Slayer == SlayerName.All)
            {
                return CheckSlayerResult.Slayer;
            }
            if (atkWeapon.Slayer2 == SlayerName.All)
            {
                return CheckSlayerResult.Slayer;
            }

            if (atkSlayer != null && atkSlayer.Slays(defender) || atkSlayer2 != null && atkSlayer2.Slays(defender))
            {
                return CheckSlayerResult.Slayer;
            }

            var talisman = attacker.Talisman as BaseTalisman;

            if (talisman != null && TalismanSlayer.Slays(talisman.Slayer, defender))
            {
                return CheckSlayerResult.Slayer;
            }

            ISlayer defISlayer = Spellbook.FindEquippedSpellbook(defender);

            if (defISlayer == null)
            {
                defISlayer = defender.Weapon as ISlayer;
            }

            if (defISlayer != null)
            {
                SlayerEntry defSlayer = SlayerGroup.GetEntryByName(defISlayer.Slayer);
                SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName(defISlayer.Slayer2);

                if (defSlayer != null && defSlayer.Group.OppositionSuperSlays(attacker) ||
                    defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays(attacker))
                {
                    return CheckSlayerResult.Opposition;
                }
            }

            return CheckSlayerResult.None;
        }

        /*
		public virtual void AddBlood( Mobile attacker, Mobile defender, int damage )
		{
			if ( damage > 0 )
			{
				new Blood().MoveToWorld( defender.Location, defender.Map );

				int extraBlood = (EraSE ? Utility.RandomMinMax( 3, 4 ) : Utility.RandomMinMax( 0, 1 ) );

				for( int i = 0; i < extraBlood; i++ )
				{
					new Blood().MoveToWorld( new Point3D(
						defender.X + Utility.RandomMinMax( -1, 1 ),
						defender.Y + Utility.RandomMinMax( -1, 1 ),
						defender.Z ), defender.Map );
				}
			}
		}
*/

        public virtual void AddBlood(Mobile attacker, Mobile defender, int damage)
        {
            int hue = defender.BloodHue;

            if (damage <= 2 || hue < 0)
            {
                return;
            }

            int bloodcount = Math.Min(damage / 50 + 1, 5);

            for (int i = 0; i < bloodcount; i++)
            {
                CreateBlood(defender.Location, defender.Map, hue, i > 3);
            }
        }

        public static void CreateBlood(Point3D loc, Map map, int hue, bool delayed)
        {
            new Blood(hue, delayed).MoveToWorld(new Point3D(BloodOffset(loc.X), BloodOffset(loc.Y), loc.Z), map);
        }

        public static int BloodOffset(int coord)
        {
            return coord + Utility.RandomMinMax(-1, 1);
        }

        private int ApplyCraftAttributeElementDamage(int attrDamage, ref int element, int totalRemaining)
        {
            if (totalRemaining <= 0)
            {
                return 0;
            }

            if (attrDamage <= 0)
            {
                return totalRemaining;
            }

            int appliedDamage = attrDamage;

            if ((appliedDamage + element) > 100)
            {
                appliedDamage = 100 - element;
            }

            if (appliedDamage > totalRemaining)
            {
                appliedDamage = totalRemaining;
            }

            element += appliedDamage;

            return totalRemaining - appliedDamage;
        }

        public virtual void OnMiss(Mobile attacker, Mobile defender)
        {
            PlaySwingAnimation(attacker);
            attacker.PlaySound(GetMissAttackSound(attacker, defender));
            defender.PlaySound(GetMissDefendSound(attacker, defender));

            WeaponAbility ability = WeaponAbility.GetCurrentAbility(attacker);

            if (ability != null)
            {
                ability.OnMiss(attacker, defender);
            }

            if (XmlScript.HasTrigger(attacker, TriggerName.onMiss))
            {
                UberScriptTriggers.Trigger(attacker, defender, TriggerName.onMiss, this);
            }
            if (XmlScript.HasTrigger(defender, TriggerName.onDodge))
            {
                UberScriptTriggers.Trigger(defender, attacker, TriggerName.onDodge, this);
            }
        }

        public virtual void GetBaseDamageRange(Mobile attacker, out int min, out int max)
        {
            if (attacker is BaseCreature && ((BaseCreature) attacker).WeaponDamage == false)
            {
                var c = (BaseCreature) attacker;

                if (c.DamageMin >= 0)
                {
                    min = c.DamageMin;
                    max = c.DamageMax;
                    return;
                }

                if (this is Fists && !attacker.Body.IsHuman)
                {
                    min = attacker.Str / 28;
                    max = attacker.Str / 28;
                    return;
                }
            }

            min = DamageMin;
            max = DamageMax;
        }

        public virtual double GetBaseDamage(Mobile attacker)
        {
            if (attacker is BaseCreature)
            {
                int min, max;

                GetBaseDamageRange(attacker, out min, out max);

                return Utility.RandomMinMax(min, max);
            }

            //return Utility.RandomMinMax(DamageMin, DamageMax);


            if (m_DiceDamage[0] != -1 && m_DiceDamage[1] != -1 && m_DiceDamage[2] != -1)
            {
                return Utility.Dice(m_DiceDamage[0], m_DiceDamage[1], m_DiceDamage[2]);
            }
            else
            {
                return DiceDamage;
            }
        }

        public virtual double GetBonus(double value, double scalar, double threshold, double offset)
        {
            double bonus = value * scalar;

            if (value >= threshold)
            {
                bonus += offset;
            }

            return bonus / 100;
        }

        public virtual int GetHitChanceBonus(Mobile attacker)
        {
            int bonus = 0;

            Ethic attackerState = Ethic.Find(attacker);

            int ethicbonus = 0;

            if (m_EthicState != null && m_EthicState.IsRunic && attackerState != null &&
                attackerState != m_EthicState.Ethic)
                //Assume that Ethic is never null for items which have ethical states
            {
                ethicbonus = 6;
            }

            if (m_Identified)
            {
                switch (m_AccuracyLevel)
                {
                    case WeaponAccuracyLevel.Accurate:
                        bonus += SpecialMovesController._AccuracyHitBonusMaxPercent / 5;
                        break;
                    case WeaponAccuracyLevel.Surpassingly:
                        bonus += SpecialMovesController._AccuracyHitBonusMaxPercent / 5 * 2;
                        break;
                    case WeaponAccuracyLevel.Eminently:
                        bonus += SpecialMovesController._AccuracyHitBonusMaxPercent / 5 * 3;
                        break;
                    case WeaponAccuracyLevel.Exceedingly:
                        bonus += SpecialMovesController._AccuracyHitBonusMaxPercent / 5 * 4;
                        break;
                    case WeaponAccuracyLevel.Supremely:
                        bonus += SpecialMovesController._AccuracyHitBonusMaxPercent;
                        break;
                }
            }

            return Math.Max(bonus, ethicbonus);
        }

        public virtual int GetDamageBonus(Mobile attacker)
        {
            int bonus = VirtualDamageBonus;

            switch (m_Quality)
            {
                case WeaponQuality.Low:
                    bonus -= 20;
                    break;
                case WeaponQuality.Exceptional:
                    bonus += 20;
                    break;
            }

            Ethic attackerState = Ethic.Find(attacker);

            int ethicbonus = 0;

            if (m_EthicState != null && m_EthicState.IsRunic && attackerState != null &&
                attackerState != m_EthicState.Ethic)
                //Assume that Ethic is never null for items which have ethical states
            {
                ethicbonus = bonus + 25;
            }
            else if (m_Identified)
            {
                switch (m_DamageLevel)
                {
                    case WeaponDamageLevel.Ruin:
                        bonus += 15;
                        break;
                    case WeaponDamageLevel.Might:
                        bonus += 20;
                        break;
                    case WeaponDamageLevel.Force:
                        bonus += 25;
                        break;
                    case WeaponDamageLevel.Power:
                        bonus += 30;
                        break;
                    case WeaponDamageLevel.Vanq:
                        bonus += 35;
                        break;
                }
            }

            return Math.Max(bonus, ethicbonus);
        }

        public virtual void GetStatusDamage(Mobile from, out int min, out int max)
        {
            int baseMin, baseMax;

            GetBaseDamageRange(from, out baseMin, out baseMax);

            min = Math.Max((int) ScaleDamageOld(from, baseMin, false), 1);
            max = Math.Max((int) ScaleDamageOld(from, baseMax, false), 1);
        }

        public virtual int VirtualDamageBonus { get { return 0; } }

        public virtual double ScaleDamageOld(Mobile attacker, double damage, bool checkSkills)
        {
            if (checkSkills)
            {
                attacker.CheckSkill(SkillName.Tactics, 0.0, 120.0); // Passively check tactics for gain
                attacker.CheckSkill(SkillName.Anatomy, 0.0, 120.0); // Passively check Anatomy for gain

                if (Type == WeaponType.Axe)
                {
                    attacker.CheckSkill(SkillName.Lumberjacking, 0.0, 100.0); // Passively check Lumberjacking for gain
                }
            }

            /* Apply damage level offset
 * : Regular 	: 0
* : Ruin		: 1
* : Might   	: 3
* : Force   	: 5
* : Power   	: 7
* : Vanq		: 9
*/
            Ethic attackerState = Ethic.Find(attacker);

            if (m_EthicState != null && m_EthicState.IsRunic && attackerState != null &&
                attackerState == m_EthicState.Ethic)
            //Assume that Ethic is never null for items which have ethical states
            {
                damage += (2.0 * (int)WeaponDamageLevel.Power) - 1.0;
            }
            else if (m_DamageLevel != WeaponDamageLevel.Regular)
            {
                damage += (2.0 * (int)m_DamageLevel) - 1.0;
                if (Quality == WeaponQuality.Exceptional)
                {
                    damage += ((int)m_Quality - 1) * 4;
                }
            }
            else
            {
                damage += ((int)m_Quality - 1) * 4;
            }

            /* Compute tactics modifier
			 * :   0.0 = 50% loss
			 * :  50.0 = unchanged
			 * : 100.0 = 50% bonus
			 */

            double bonus = 0.0;
            double tacticsValue = attacker.Skills[SkillName.Tactics].Value;

            if (attacker is BaseCreature && attacker.IsControlled() && attacker.Combatant is PlayerMobile && tacticsValue > 100)
            {
                tacticsValue = 100;
            }

            bonus += (Math.Min(tacticsValue, 100) - 50.0) / 100.0;

            if (tacticsValue > 100.0) // 100 to 120 for powerscrolls up to 10%, with accuracy bonus its 22.5%
            {
                bonus += (tacticsValue - 100.0) / 300.0;
            }

            /* Compute strength modifier
			 * : 1% bonus for every 5 strength
			 */
            bonus += Math.Min(100.0, attacker.Str) / 500.0;

            if (attacker.Str > 100) // 100 to 120 for powerscrolls up to +5%
            {
                bonus += (attacker.Str - 100) / 500.0;
            }

            /* Compute anatomy modifier
			 * : 1% bonus for every 5 points of anatomy
			 * : +10% bonus at Grandmaster or higher
			 */
            double anatomyValue = attacker.Skills[SkillName.Anatomy].Value;

            if (attacker is BaseCreature && attacker.IsControlled() && attacker.Combatant is PlayerMobile && anatomyValue > 100)
            {
                anatomyValue = 100;
            }

            bonus += Math.Min(anatomyValue, 100.0) / 500.0;

            if (anatomyValue == 100.0)
            {
                bonus += 0.1;
            }
            else if (anatomyValue > 100.0) // 100+ for powerscrolls up to +12.5%
            {
                bonus += 0.1 + ((anatomyValue - 100.0) / 800.0);
            }

            /* Compute lumberjacking bonus
			 * : 1% bonus for every 5 points of lumberjacking
			 * : +10% bonus at Grandmaster or higher
			 */
            if (Type == WeaponType.Axe)
            {
                double lumberValue = attacker.Skills[SkillName.Lumberjacking].Value;
                var jackbonus = Math.Round(lumberValue / 285, 2);
                
                //compensate for lj powerscroll
                if (jackbonus >= 0.35)
                {
                    jackbonus = 0.35;
                }
                bonus += jackbonus;
            }

            // Apply bonuses
            damage += (damage * bonus) + ((damage * VirtualDamageBonus) / 100);
            //damage += (damage * tacticsBonus) + (damage * strBonus) + (damage * anatomyBonus) + (damage * lumberBonus) + (damage * qualityBonus) + ((damage * VirtualDamageBonus) / 100);

            return ScaleDamageByDurability((int) damage);
        }

        public virtual int ScaleDamageByDurability(int damage)
        {
            int scale = 100;

            if (m_MaxHits > 0 && m_Hits < m_MaxHits)
            {
                scale = 75 + ((25 * m_Hits) / m_MaxHits);
            }

            return AOS.Scale(damage, scale);
        }

        public virtual int ComputeDamage(Mobile attacker, Mobile defender)
        {
            return (int) ScaleDamageOld(attacker, GetBaseDamage(attacker), true);
        }

        public virtual void PlayHurtAnimation(Mobile from)
        {
            int action;
            int frames;

            switch (from.Body.Type)
            {
                case BodyType.Sea:
                case BodyType.Animal:
                {
                    action = 7;
                    frames = 5;
                    break;
                }
                case BodyType.Monster:
                {
                    action = 10;
                    frames = 4;
                    break;
                }
                case BodyType.Human:
                {
                    action = 20;
                    frames = 5;
                    break;
                }
                default:
                    return;
            }

            if (from.Mounted)
            {
                return;
            }

            from.Animate(action, frames, 1, true, false, 0);
        }

        public virtual void PlaySwingAnimation(Mobile from)
        {
            int action;

            switch (from.Body.Type)
            {
                case BodyType.Sea:
                case BodyType.Animal:
                {
                    action = Utility.Random(5, 2);
                    break;
                }
                case BodyType.Monster:
                {
                    switch (Animation)
                    {
                        default:
                        case WeaponAnimation.Wrestle:
                        case WeaponAnimation.Bash1H:
                        case WeaponAnimation.Pierce1H:
                        case WeaponAnimation.Slash1H:
                        case WeaponAnimation.Bash2H:
                        case WeaponAnimation.Pierce2H:
                        case WeaponAnimation.Slash2H:
                            // Alan mod: I want archery to still swing // action = Utility.Random( 4, 3 ); break;
                        case WeaponAnimation.ShootBow: // Alan Mod return; // 7
                        case WeaponAnimation.ShootXBow: // Alan Mod return; // 8
                            action = Utility.Random(4, 3);
                            break; // Alan Mod
                    }

                    break;
                }
                case BodyType.Human:
                {
                    if (!from.Mounted)
                    {
                        action = (int) Animation;
                    }
                    else
                    {
                        switch (Animation)
                        {
                            default:
                            case WeaponAnimation.Wrestle:
                            case WeaponAnimation.Bash1H:
                            case WeaponAnimation.Pierce1H:
                            case WeaponAnimation.Slash1H:
                                action = 26;
                                break;
                            case WeaponAnimation.Bash2H:
                            case WeaponAnimation.Pierce2H:
                            case WeaponAnimation.Slash2H:
                                action = 29;
                                break;
                            case WeaponAnimation.ShootBow:
                                action = 27;
                                break;
                            case WeaponAnimation.ShootXBow:
                                action = 28;
                                break;
                        }
                    }

                    break;
                }
                default:
                    return;
            }

            from.Animate(action, 7, 1, true, false, 0);
        }

        #region Serialization/Deserialization

        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
            {
                flags |= toSet;
            }
        }

        private static void SetSaveFlag(ref SaveFlag2 flags, SaveFlag2 toSet, bool setIf)
        {
            if (setIf)
            {
                flags |= toSet;
            }
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

        private static bool GetSaveFlag(SaveFlag2 flags, SaveFlag2 toGet)
        {
            return ((flags & toGet) != 0);
        }

        public override void Serialize(GenericWriter writer)
        {
            #region Ethics

            if (m_EthicState != null && m_EthicState.HasExpired)
            {
                m_EthicState.Detach();
            }

            #endregion

            #region Factions

            if (m_FactionState != null && m_FactionState.HasExpired)
            {
                m_FactionState.Detach();
            }

            #endregion

            base.Serialize(writer);

            writer.Write(11); // version

            writer.Write(OriginalItemID);
            writer.Write(OriginalHue);
            writer.Write((int)OriginalAnimation);

            var flags = SaveFlag.None;
            var flags2 = SaveFlag2.None;

            //SaveFlag
            SetSaveFlag(ref flags, SaveFlag.DamageLevel, m_DamageLevel != WeaponDamageLevel.Regular);
            SetSaveFlag(ref flags, SaveFlag.AccuracyLevel, m_AccuracyLevel != WeaponAccuracyLevel.Regular);
            SetSaveFlag(ref flags, SaveFlag.DurabilityLevel, m_DurabilityLevel != WeaponDurabilityLevel.Regular);
            SetSaveFlag(ref flags, SaveFlag.Quality, m_Quality != WeaponQuality.Regular);
            SetSaveFlag(ref flags, SaveFlag.Hits, m_Hits != 0);
            SetSaveFlag(ref flags, SaveFlag.MaxHits, m_MaxHits != 0);
            SetSaveFlag(ref flags, SaveFlag.Slayer, m_Slayer != SlayerName.None);
            SetSaveFlag(ref flags, SaveFlag.Poison, m_Poison != null);
            SetSaveFlag(ref flags, SaveFlag.PoisonCharges, m_PoisonCharges != 0);
            SetSaveFlag(ref flags, SaveFlag.Crafter, m_Crafter != null);
            SetSaveFlag(ref flags, SaveFlag.Identified, m_Identified);
            SetSaveFlag(ref flags, SaveFlag.StrReq, m_StrReq != -1);
            SetSaveFlag(ref flags, SaveFlag.DexReq, m_DexReq != -1);
            SetSaveFlag(ref flags, SaveFlag.IntReq, m_IntReq != -1);
            SetSaveFlag(ref flags, SaveFlag.MinDamage, m_DamageMin != -1);
            SetSaveFlag(ref flags, SaveFlag.MaxDamage, m_DamageMax != -1);
            SetSaveFlag(ref flags, SaveFlag.HitSound, m_HitSound != -1);
            SetSaveFlag(ref flags, SaveFlag.MissSound, m_MissSound != -1);
            SetSaveFlag(ref flags, SaveFlag.Speed, m_Speed != -1);
            SetSaveFlag(ref flags, SaveFlag.MaxRange, m_MaxRange != -1);
            SetSaveFlag(ref flags, SaveFlag.Skill, m_Skill != (SkillName) (-1));
            SetSaveFlag(ref flags, SaveFlag.Type, m_Type != (WeaponType) (-1));
            SetSaveFlag(ref flags, SaveFlag.Animation, m_Animation != (WeaponAnimation) (-1));
            SetSaveFlag(ref flags, SaveFlag.Resource, m_Resource != CraftResource.Iron);
            //SetSaveFlag( ref flags, SaveFlag.xAttributes,		!m_AosAttributes.IsEmpty );
            //SetSaveFlag( ref flags, SaveFlag.xWeaponAttributes,	!m_AosWeaponAttributes.IsEmpty );
            SetSaveFlag(ref flags, SaveFlag.PlayerConstructed, m_PlayerConstructed);
            //SetSaveFlag( ref flags, SaveFlag.SkillBonuses,		!m_AosSkillBonuses.IsEmpty );
            SetSaveFlag(ref flags, SaveFlag.Slayer2, m_Slayer2 != SlayerName.None);
            //SetSaveFlag( ref flags, SaveFlag.ElementalDamages,	!m_AosElementDamages.IsEmpty );
            //SetSaveFlag( ref flags, SaveFlag.EngravedText,		!String.IsNullOrEmpty( m_EngravedText ) );

            //SaveFlag2
            SetSaveFlag(
                ref flags2, SaveFlag2.DiceDamage,
                m_DiceDamage[0] != -1 && m_DiceDamage[1] != -1 && m_DiceDamage[2] != -1);

            writer.Write((int) flags2);

            if (GetSaveFlag(flags2, SaveFlag2.DiceDamage))
            {
                writer.WriteEncodedInt(m_DiceDamage[0]);
                writer.WriteEncodedInt(m_DiceDamage[1]);
                writer.WriteEncodedInt(m_DiceDamage[2]);
            }

            if (GetSaveFlag(flags2, SaveFlag2.Aesthetic))
            {
                writer.Write(m_Aesthetic);
            }

            writer.Write((int) flags);

            if (GetSaveFlag(flags, SaveFlag.DamageLevel))
            {
                writer.Write((int) m_DamageLevel);
            }

            if (GetSaveFlag(flags, SaveFlag.AccuracyLevel))
            {
                writer.Write((int) m_AccuracyLevel);
            }

            if (GetSaveFlag(flags, SaveFlag.DurabilityLevel))
            {
                writer.Write((int) m_DurabilityLevel);
            }

            if (GetSaveFlag(flags, SaveFlag.Quality))
            {
                writer.Write((int) m_Quality);
            }

            if (GetSaveFlag(flags, SaveFlag.Hits))
            {
                writer.Write(m_Hits);
            }

            if (GetSaveFlag(flags, SaveFlag.MaxHits))
            {
                writer.Write(m_MaxHits);
            }

            if (GetSaveFlag(flags, SaveFlag.Slayer))
            {
                writer.Write((int) m_Slayer);
            }

            if (GetSaveFlag(flags, SaveFlag.Poison))
            {
                Poison.Serialize(m_Poison, writer);
            }

            if (GetSaveFlag(flags, SaveFlag.PoisonCharges))
            {
                writer.Write(m_PoisonCharges);
            }

            if (GetSaveFlag(flags, SaveFlag.Crafter))
            {
                writer.Write(m_Crafter);
            }

            if (GetSaveFlag(flags, SaveFlag.StrReq))
            {
                writer.Write(m_StrReq);
            }

            if (GetSaveFlag(flags, SaveFlag.DexReq))
            {
                writer.Write(m_DexReq);
            }

            if (GetSaveFlag(flags, SaveFlag.IntReq))
            {
                writer.Write(m_IntReq);
            }

            if (GetSaveFlag(flags, SaveFlag.MinDamage))
            {
                writer.Write(m_DamageMin);
            }

            if (GetSaveFlag(flags, SaveFlag.MaxDamage))
            {
                writer.Write(m_DamageMax);
            }

            if (GetSaveFlag(flags, SaveFlag.HitSound))
            {
                writer.Write(m_HitSound);
            }

            if (GetSaveFlag(flags, SaveFlag.MissSound))
            {
                writer.Write(m_MissSound);
            }

            if (GetSaveFlag(flags, SaveFlag.Speed))
            {
                writer.Write(m_Speed);
            }

            if (GetSaveFlag(flags, SaveFlag.MaxRange))
            {
                writer.Write(m_MaxRange);
            }

            if (GetSaveFlag(flags, SaveFlag.Skill))
            {
                writer.Write((int) m_Skill);
            }

            if (GetSaveFlag(flags, SaveFlag.Type))
            {
                writer.Write((int) m_Type);
            }

            if (GetSaveFlag(flags, SaveFlag.Animation))
            {
                writer.Write((int) m_Animation);
            }

            if (GetSaveFlag(flags, SaveFlag.Resource))
            {
                writer.Write((int) m_Resource);
            }

            //if ( GetSaveFlag( flags, SaveFlag.xAttributes ) )
            //	m_AosAttributes.Serialize( writer );

            //if ( GetSaveFlag( flags, SaveFlag.xWeaponAttributes ) )
            //	m_AosWeaponAttributes.Serialize( writer );

            //if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
            //	m_AosSkillBonuses.Serialize( writer );

            if (GetSaveFlag(flags, SaveFlag.Slayer2))
            {
                writer.Write((int) m_Slayer2);
            }

            //if( GetSaveFlag( flags, SaveFlag.ElementalDamages ) )
            //	m_AosElementDamages.Serialize( writer );

            if (GetSaveFlag(flags, SaveFlag.EngravedText))
            {
                writer.Write(m_EngravedText);
            }
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            DamageLevel = 0x00000001,
            AccuracyLevel = 0x00000002,
            DurabilityLevel = 0x00000004,
            Quality = 0x00000008,
            Hits = 0x00000010,
            MaxHits = 0x00000020,
            Slayer = 0x00000040,
            Poison = 0x00000080,
            PoisonCharges = 0x00000100,
            Crafter = 0x00000200,
            Identified = 0x00000400,
            StrReq = 0x00000800,
            DexReq = 0x00001000,
            IntReq = 0x00002000,
            MinDamage = 0x00004000,
            MaxDamage = 0x00008000,
            HitSound = 0x00010000,
            MissSound = 0x00020000,
            Speed = 0x00040000,
            MaxRange = 0x00080000,
            Skill = 0x00100000,
            Type = 0x00200000,
            Animation = 0x00400000,
            Resource = 0x00800000,
            xAttributes = 0x01000000,
            xWeaponAttributes = 0x02000000,
            PlayerConstructed = 0x04000000,
            SkillBonuses = 0x08000000,
            Slayer2 = 0x10000000,
            ElementalDamages = 0x20000000,
            EngravedText = 0x40000000,
        }

        [Flags]
        private enum SaveFlag2
        {
            None = 0x00000000,
            DiceDamage = 0x00000001,
            Aesthetic = 0x00000002
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 11:
                {
                    OriginalItemID = reader.ReadInt();
                    OriginalHue = reader.ReadInt();
                    OriginalAnimation = (WeaponAnimation)reader.ReadInt();
                        goto case 10;
                    }
                case 10:
                {
                    var flags2 = (SaveFlag2) reader.ReadInt();

                    if (GetSaveFlag(flags2, SaveFlag2.Aesthetic)) // Alan Mod
                    {
                        m_Aesthetic = true;
                    }

                    m_DiceDamage = new[] {-1, -1, -1};

                    if (GetSaveFlag(flags2, SaveFlag2.DiceDamage))
                    {
                        m_DiceDamage[0] = reader.ReadEncodedInt();
                        m_DiceDamage[1] = reader.ReadEncodedInt();
                        m_DiceDamage[2] = reader.ReadEncodedInt();
                    }

                    goto case 9;
                }
                case 9:
                case 8:
                case 7:
                case 6:
                case 5:
                {
                    var flags = (SaveFlag) reader.ReadInt();


                    if (GetSaveFlag(flags, SaveFlag.DamageLevel))
                    {
                        m_DamageLevel = (WeaponDamageLevel) reader.ReadInt();

                        if (m_DamageLevel > WeaponDamageLevel.Vanq)
                        {
                            m_DamageLevel = WeaponDamageLevel.Ruin;
                        }
                    }

                    if (GetSaveFlag(flags, SaveFlag.AccuracyLevel))
                    {
                        m_AccuracyLevel = (WeaponAccuracyLevel) reader.ReadInt();

                        if (m_AccuracyLevel > WeaponAccuracyLevel.Supremely)
                        {
                            m_AccuracyLevel = WeaponAccuracyLevel.Accurate;
                        }
                    }

                    if (GetSaveFlag(flags, SaveFlag.DurabilityLevel))
                    {
                        m_DurabilityLevel = (WeaponDurabilityLevel) reader.ReadInt();

                        if (m_DurabilityLevel > WeaponDurabilityLevel.Indestructible)
                        {
                            m_DurabilityLevel = WeaponDurabilityLevel.Durable;
                        }
                    }

                    if (GetSaveFlag(flags, SaveFlag.Quality))
                    {
                        m_Quality = (WeaponQuality) reader.ReadInt();
                    }
                    else
                    {
                        m_Quality = WeaponQuality.Regular;
                    }

                    if (GetSaveFlag(flags, SaveFlag.Hits))
                    {
                        m_Hits = reader.ReadInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.MaxHits))
                    {
                        m_MaxHits = reader.ReadInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.Slayer))
                    {
                        m_Slayer = (SlayerName) reader.ReadInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.Poison))
                    {
                        m_Poison = Poison.Deserialize(reader);
                    }

                    if (GetSaveFlag(flags, SaveFlag.PoisonCharges))
                    {
                        m_PoisonCharges = reader.ReadInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.Crafter))
                    {
                        m_Crafter = reader.ReadMobile();
                    }

                    if (GetSaveFlag(flags, SaveFlag.Identified))
                    {
                        m_Identified = (version >= 6 || reader.ReadBool());
                    }

                    if (GetSaveFlag(flags, SaveFlag.StrReq))
                    {
                        m_StrReq = reader.ReadInt();
                    }
                    else
                    {
                        m_StrReq = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.DexReq))
                    {
                        m_DexReq = reader.ReadInt();
                    }
                    else
                    {
                        m_DexReq = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.IntReq))
                    {
                        m_IntReq = reader.ReadInt();
                    }
                    else
                    {
                        m_IntReq = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.MinDamage))
                    {
                        m_DamageMin = reader.ReadInt();
                    }
                    else
                    {
                        m_DamageMin = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.MaxDamage))
                    {
                        m_DamageMax = reader.ReadInt();
                    }
                    else
                    {
                        m_DamageMax = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.HitSound))
                    {
                        m_HitSound = reader.ReadInt();
                    }
                    else
                    {
                        m_HitSound = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.MissSound))
                    {
                        m_MissSound = reader.ReadInt();
                    }
                    else
                    {
                        m_MissSound = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.Speed))
                    {
                        if (version < 9)
                        {
                            m_Speed = reader.ReadInt();
                        }
                        else
                        {
                            m_Speed = reader.ReadFloat();
                        }
                    }
                    else
                    {
                        m_Speed = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.MaxRange))
                    {
                        m_MaxRange = reader.ReadInt();
                    }
                    else
                    {
                        m_MaxRange = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.Skill))
                    {
                        m_Skill = (SkillName) reader.ReadInt();
                    }
                    else
                    {
                        m_Skill = (SkillName) (-1);
                    }

                    if (GetSaveFlag(flags, SaveFlag.Type))
                    {
                        m_Type = (WeaponType) reader.ReadInt();
                    }
                    else
                    {
                        m_Type = (WeaponType) (-1);
                    }

                    if (GetSaveFlag(flags, SaveFlag.Animation))
                    {
                        m_Animation = (WeaponAnimation) reader.ReadInt();
                    }
                    else
                    {
                        m_Animation = (WeaponAnimation) (-1);
                    }

                    if (GetSaveFlag(flags, SaveFlag.Resource))
                    {
                        m_Resource = (CraftResource) reader.ReadInt();
                    }
                    else
                    {
                        m_Resource = CraftResource.Iron;
                    }

                    if (GetSaveFlag(flags, SaveFlag.xAttributes)) //obsolete
                    {
                        new AosAttributes(this, reader);
                    }
                    //else
                    //	m_AosAttributes = new AosAttributes( this );

                    if (GetSaveFlag(flags, SaveFlag.xWeaponAttributes)) //obsolete
                    {
                        new AosWeaponAttributes(this, reader);
                    }
                    //else
                    //	m_AosWeaponAttributes = new AosWeaponAttributes( this );

                    if (UseSkillMod && m_Identified && m_AccuracyLevel != WeaponAccuracyLevel.Regular &&
                        Parent is Mobile)
                    {
                        m_SkillMod = new DefaultSkillMod(AccuracySkill, true, (int) m_AccuracyLevel * 5);
                        ((Mobile) Parent).AddSkillMod(m_SkillMod);
                    }

                    //if ( version < 7 && m_AosWeaponAttributes.MageWeapon != 0 )
                    //	m_AosWeaponAttributes.MageWeapon = 30 - m_AosWeaponAttributes.MageWeapon;

                    if (GetSaveFlag(flags, SaveFlag.PlayerConstructed))
                    {
                        m_PlayerConstructed = true;
                    }

                    if (GetSaveFlag(flags, SaveFlag.SkillBonuses)) //obsolete
                    {
                        new AosSkillBonuses(this, reader);
                    }
                    //else
                    //	m_AosSkillBonuses = new AosSkillBonuses( this );

                    if (GetSaveFlag(flags, SaveFlag.Slayer2))
                    {
                        m_Slayer2 = (SlayerName) reader.ReadInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.ElementalDamages)) //obsolete
                    {
                        new AosElementAttributes(this, reader);
                    }
                    //else
                    //	m_AosElementDamages = new AosElementAttributes( this );

                    if (GetSaveFlag(flags, SaveFlag.EngravedText))
                    {
                        m_EngravedText = reader.ReadString();
                    }

                    if (version < 10)
                    {
                        m_DiceDamage = new[] {-1, -1, -1};
                    }

                    break;
                }
                case 4:
                {
                    m_Slayer = (SlayerName) reader.ReadInt();

                    goto case 3;
                }
                case 3:
                {
                    m_StrReq = reader.ReadInt();
                    m_DexReq = reader.ReadInt();
                    m_IntReq = reader.ReadInt();

                    goto case 2;
                }
                case 2:
                {
                    m_Identified = reader.ReadBool();

                    goto case 1;
                }
                case 1:
                {
                    m_MaxRange = reader.ReadInt();

                    goto case 0;
                }
                case 0:
                {
                    if (version == 0)
                    {
                        m_MaxRange = 1; // default
                    }

                    if (version < 5)
                    {
                        //m_Resource = CraftResource.Iron;
                        //m_AosAttributes = new AosAttributes( this );
                        //m_AosWeaponAttributes = new AosWeaponAttributes( this );
                        //m_AosElementDamages = new AosElementAttributes( this );
                        //m_AosSkillBonuses = new AosSkillBonuses( this );
                    }

                    m_DamageMin = reader.ReadInt();
                    m_DamageMax = reader.ReadInt();

                    m_Speed = reader.ReadInt();

                    m_HitSound = reader.ReadInt();
                    m_MissSound = reader.ReadInt();

                    m_Skill = (SkillName) reader.ReadInt();
                    m_Type = (WeaponType) reader.ReadInt();
                    m_Animation = (WeaponAnimation) reader.ReadInt();
                    m_DamageLevel = (WeaponDamageLevel) reader.ReadInt();
                    m_AccuracyLevel = (WeaponAccuracyLevel) reader.ReadInt();
                    m_DurabilityLevel = (WeaponDurabilityLevel) reader.ReadInt();
                    m_Quality = (WeaponQuality) reader.ReadInt();

                    m_Crafter = reader.ReadMobile();

                    m_Poison = Poison.Deserialize(reader);
                    m_PoisonCharges = reader.ReadInt();

                    if (m_StrReq == OldStrengthReq)
                    {
                        m_StrReq = -1;
                    }

                    if (m_DexReq == OldDexterityReq)
                    {
                        m_DexReq = -1;
                    }

                    if (m_IntReq == OldIntelligenceReq)
                    {
                        m_IntReq = -1;
                    }

                    if (m_DamageMin == NewMinDamage)
                    {
                        m_DamageMin = -1;
                    }

                    if (m_DamageMax == NewMaxDamage)
                    {
                        m_DamageMax = -1;
                    }

                    if (m_HitSound == OldHitSound)
                    {
                        m_HitSound = -1;
                    }

                    if (m_MissSound == OldMissSound)
                    {
                        m_MissSound = -1;
                    }

                    if (m_Speed == OldSpeed)
                    {
                        m_Speed = -1;
                    }

                    if (m_MaxRange == OldMaxRange)
                    {
                        m_MaxRange = -1;
                    }

                    if (m_Skill == OldSkill)
                    {
                        m_Skill = (SkillName) (-1);
                    }

                    if (m_Type == OldType)
                    {
                        m_Type = (WeaponType) (-1);
                    }

                    if (m_Animation == OldAnimation)
                    {
                        m_Animation = (WeaponAnimation) (-1);
                    }

                    if (UseSkillMod && m_Identified && m_AccuracyLevel != WeaponAccuracyLevel.Regular &&
                        Parent is Mobile)
                    {
                        m_SkillMod = new DefaultSkillMod(AccuracySkill, true, (int) m_AccuracyLevel * 5);
                        ((Mobile) Parent).AddSkillMod(m_SkillMod);
                    }

                    break;
                }
            }

            if (Parent is Mobile)
            {
                ((Mobile) Parent).CheckStatTimers();
            }

            if (m_Hits <= 0 && m_MaxHits <= 0)
            {
                m_Hits = m_MaxHits = Utility.RandomMinMax(InitMinHits, InitMaxHits);
            }

            if (version < 6)
            {
                m_PlayerConstructed = true; // we don't know, so, assume it's crafted
            }
        }

        #endregion

        public BaseWeapon(int itemID)
            : base(itemID)
        {
            Layer = (Layer) ItemData.Quality;

            m_Quality = WeaponQuality.Regular;
            m_StrReq = -1;
            m_DexReq = -1;
            m_IntReq = -1;
            m_DamageMin = -1;
            m_DamageMax = -1;
            m_HitSound = -1;
            m_MissSound = -1;
            m_Speed = -1;
            m_MaxRange = -1;
            m_Skill = (SkillName) (-1);
            m_Type = (WeaponType) (-1);
            m_Animation = (WeaponAnimation) (-1);

            m_DiceDamage = new[] {-1, -1, -1};

            m_Hits = m_MaxHits = Utility.RandomMinMax(InitMinHits, InitMaxHits);

            m_Resource = CraftResource.Iron;
        }

        public BaseWeapon(Serial serial)
            : base(serial)
        {}

        private string GetNameString()
        {
            string name = Name;

            if (name == null)
            {
                name = String.Format("#{0}", LabelNumber);
            }

            return name;
        }

        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return Identified ? base.Hue : (Resource == CraftResource.BlackRock ? 1155 : 0); }
            set
            {
                base.Hue = value;
                InvalidateProperties();
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            int oreType;

            switch (m_Resource)
            {
                case CraftResource.DullCopper:
                    oreType = 1053108;
                    break; // dull copper
                case CraftResource.ShadowIron:
                    oreType = 1053107;
                    break; // shadow iron
                case CraftResource.Copper:
                    oreType = 1053106;
                    break; // copper
                case CraftResource.Bronze:
                    oreType = 1053105;
                    break; // bronze
                case CraftResource.Gold:
                    oreType = 1053104;
                    break; // golden
                case CraftResource.Agapite:
                    oreType = 1053103;
                    break; // agapite
                case CraftResource.Verite:
                    oreType = 1053102;
                    break; // verite
                case CraftResource.Valorite:
                    oreType = 1053101;
                    break; // valorite
                case CraftResource.SpinedLeather:
                    oreType = 1061118;
                    break; // spined
                case CraftResource.HornedLeather:
                    oreType = 1061117;
                    break; // horned
                case CraftResource.BarbedLeather:
                    oreType = 1061116;
                    break; // barbed
                case CraftResource.RedScales:
                    oreType = 1060814;
                    break; // red
                case CraftResource.YellowScales:
                    oreType = 1060818;
                    break; // yellow
                case CraftResource.BlackScales:
                    oreType = 1060820;
                    break; // black
                case CraftResource.GreenScales:
                    oreType = 1060819;
                    break; // green
                case CraftResource.WhiteScales:
                    oreType = 1060821;
                    break; // white
                case CraftResource.BlueScales:
                    oreType = 1060815;
                    break; // blue
                default:
                    oreType = 0;
                    break;
            }

            if (oreType != 0)
            {
                list.Add(1053099, "#{0}\t{1}", oreType, GetNameString()); // ~1_oretype~ ~2_armortype~
            }
            else if (Name == null)
            {
                list.Add(LabelNumber);
            }
            else
            {
                list.Add(Name);
            }

            /*
			 * Want to move this to the engraving tool, let the non-harmful 
			 * formatting show, and remove CLILOCs embedded: more like OSI
			 * did with the books that had markup, etc.
			 * 
			 * This will have a negative effect on a few event things imgame 
			 * as is.
			 * 
			 * If we cant find a more OSI-ish way to clean it up, we can 
			 * easily put this back, and use it in the deserialize
			 * method and engraving tool, to make it perm cleaned up.
			 */

            if (!String.IsNullOrEmpty(m_EngravedText))
            {
                list.Add(1062613, m_EngravedText);
            }

            /* list.Add( 1062613, Utility.FixHtml( m_EngravedText ) ); */
        }

        public override bool AllowEquippedCast(Mobile from)
        {
            if (base.AllowEquippedCast(from))
            {
                return true;
            }

            return false;
        }

        public virtual int ArtifactRarity { get { return 0; } }

        public virtual int GetLuckBonus()
        {
            CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);

            if (resInfo == null)
            {
                return 0;
            }

            CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

            if (attrInfo == null)
            {
                return 0;
            }

            return attrInfo.WeaponLuck;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Crafter != null)
            {
                list.Add(1050043, m_Crafter.RawName); // crafted by ~1_NAME~
            }

            #region Factions

            if (m_FactionState != null)
            {
                list.Add(1041350); // faction item
            }

            #endregion

            //if ( m_AosSkillBonuses != null )
            //	m_AosSkillBonuses.GetProperties( list );

            if (m_Quality == WeaponQuality.Exceptional)
            {
                list.Add(1060636); // exceptional
            }

            if (RequiredRace == Race.Elf)
            {
                list.Add(1075086); // Elves Only
            }

            if (ArtifactRarity > 0)
            {
                list.Add(1061078, ArtifactRarity.ToString()); // artifact rarity ~1_val~
            }

            if (this is IUsesRemaining && ((IUsesRemaining) this).ShowUsesRemaining)
            {
                list.Add(1060584, ((IUsesRemaining) this).UsesRemaining.ToString()); // uses remaining: ~1_val~
            }

            if (m_Poison != null && m_PoisonCharges > 0)
            {
                list.Add(1062412 + m_Poison.Level, m_PoisonCharges.ToString());
            }

            if (m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer);
                if (entry != null)
                {
                    list.Add(entry.GetTitle(EraAOS));
                }
            }

            if (m_Slayer2 != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer2);
                if (entry != null)
                {
                    list.Add(entry.GetTitle(EraAOS));
                }
            }

            int prop;

            if ((prop = (GetDamageBonus(null))) > 0)
            {
                list.Add(1060401, prop.ToString()); // damage increase ~1_val~%
            }

            if ((prop = (GetHitChanceBonus(null))) > 0)
            {
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%
            }

            list.Add(1061168, "{0}\t{1}", DamageMin.ToString(), DamageMax.ToString());
                // weapon damage ~1_val~ - ~2_val~

            if (EraML)
            {
                list.Add(1061167, String.Format("{0}s", Speed)); // weapon speed ~1_val~
            }
            else
            {
                list.Add(1061167, Speed.ToString());
            }

            if (MaxRange > 1)
            {
                list.Add(1061169, MaxRange.ToString()); // range ~1_val~
            }

            int strReq = StrRequirement;

            if (strReq > 0)
            {
                list.Add(1061170, strReq.ToString()); // strength requirement ~1_val~
            }

            if (Layer == Layer.TwoHanded)
            {
                list.Add(1061171); // two-handed weapon
            }
            else
            {
                list.Add(1061824); // one-handed weapon
            }

            if (m_Hits >= 0 && m_MaxHits > 0)
            {
                list.Add(1060639, "{0}\t{1}", m_Hits, m_MaxHits); // durability ~1_val~ / ~2_val~
            }
        }

        /*
		public void DisplayDurabilityTo( Mobile m, int uses )
		{
			LabelToAffix( m, 1017323, AffixType.Append, ": " + uses.ToString() ); // Durability
		}
*/

        public override void OnSingleClick(Mobile from)
        {
            if (Deleted || !from.CanSee(this))
            {
                return;
            }

            LabelToExpansion(from);

            //			if ( this is IUsesRemaining && ((IUsesRemaining)this).ShowUsesRemaining )
            //				DisplayDurabilityTo( from, ((IUsesRemaining)this).UsesRemaining );

            if (IsAesthetic)
            {
                LabelTo(from, "[Aesthetic]", 2049);
            }

            int number;
            string name = Name;
            if (IsAesthetic)
                name = name + "[Aesthetic]";
            /*
			if ( m_Resource != CraftResource.None && m_Resource != CraftResource.RegularWood && m_Resource != CraftResource.Iron && m_Resource != CraftResource.RegularLeather && String.IsNullOrEmpty( name ) )
			{
				StringList stringlist = StringList.Default;

				if ( from.NetState != null && from.NetState.LTable != null )
					stringlist = from.NetState.LTable;

				if ( String.IsNullOrEmpty( name ) && stringlist[LabelNumber] != null )
					name = stringlist[LabelNumber].Text;

				if ( name.StartsWith( "a " ) && name.Length > 2 )
					name = name.Substring( 2, name.Length-2 );
				else if ( name.StartsWith( "an " ) && name.Length > 3 )
					name = name.Substring( 3, name.Length-3 );

				int oreType = 0;
				bool dragonscale = false;

				//dragon scale - 1053112

				switch ( m_Resource )
				{
					case CraftResource.DullCopper:	oreType = 1053108; break; // dull copper
					case CraftResource.ShadowIron:	oreType = 1053107; break; // shadow iron
					case CraftResource.Copper:		oreType = 1053106; break; // copper
					case CraftResource.Bronze:		oreType = 1053105; break; // bronze
					case CraftResource.Gold:			oreType = 1053104; break; // golden
					case CraftResource.Agapite:		oreType = 1053103; break; // agapite
					case CraftResource.Verite:		oreType = 1053102; break; // verite
					case CraftResource.Valorite:		oreType = 1053101; break; // valorite
					case CraftResource.SpinedLeather:	oreType = 1061118; break; // spined
					case CraftResource.HornedLeather:	oreType = 1061117; break; // horned
					case CraftResource.BarbedLeather:	oreType = 1061116; break; // barbed
					case CraftResource.RedScales:		oreType = 1060814; dragonscale = true; break; // red
					case CraftResource.YellowScales:	oreType = 1060818; dragonscale = true; break; // yellow
					case CraftResource.BlackScales:	oreType = 1060820; dragonscale = true; break; // black
					case CraftResource.GreenScales:	oreType = 1060819; dragonscale = true; break; // green
					case CraftResource.WhiteScales:	oreType = 1060821; dragonscale = true; break; // white
					case CraftResource.BlueScales:	oreType = 1060815; dragonscale = true; break; // blue
					case CraftResource.OakWood:		oreType = 1072533; break;
					case CraftResource.AshWood:		oreType = 1072534; break;
					case CraftResource.YewWood:		oreType = 1072535; break;
					case CraftResource.Bloodwood:		oreType = 1072538; break;
					case CraftResource.Heartwood:		oreType = 1072536; break;
					case CraftResource.Frostwood:		oreType = 1072539; break;
				}

				string resource = stringlist[oreType].Text;
				if ( dragonscale )
					resource = String.Format( "{0} {1}", resource, stringlist[1053112].Text );

				name = String.Format( "{0} {1}", resource, name );
			}
			else
				name = Name;
*/
            if (String.IsNullOrEmpty(name))
            {
                number = LabelNumber;
            }
            else
            {
                LabelTo(from, name, 1153);
                number = 1041000;
            }

            if (DisplayDyable)
            {
                LabelDyableTo(from);
            }

            var attrs = new List<EquipInfoAttribute>();

            bool ismagical = AddEquipInfoAttributes(from, attrs) ||
                             (m_Resource > CraftResource.Valorite && m_Resource < CraftResource.RegularLeather) ||
                             base.Hue > 0;

            if (attrs.Count > 0 || Crafter != null || number != 1041000)
            {
                var eqInfo = new EquipmentInfo(
                    number, m_Crafter, /*from.AccessLevel < AccessLevel.GameMaster &&*/ ismagical && !Identified,
                    attrs.ToArray());
                from.Send(new DisplayEquipmentInfo(this, eqInfo));
            }
        }

        public virtual bool AddEquipInfoAttributes(Mobile from, List<EquipInfoAttribute> attrs)
        {
            bool ismagical = false;

            if (DisplayLootType)
            {
                if (LootType == LootType.Blessed)
                {
                    attrs.Add(new EquipInfoAttribute(1038021)); // blessed
                }
                else if (LootType == LootType.Cursed)
                {
                    attrs.Add(new EquipInfoAttribute(1049643)); // cursed
                }
            }

            #region Factions

            if (m_FactionState != null)
            {
                attrs.Add(new EquipInfoAttribute(1041350)); // faction item
            }

            #endregion

            if (m_Quality != WeaponQuality.Regular)
            {
                attrs.Add(new EquipInfoAttribute(1018305 - (int) m_Quality));
            }

            if (m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer);
                if (entry != null)
                {
                    attrs.Add(new EquipInfoAttribute(entry.GetTitle(EraAOS)));
                }

                ismagical = true;
            }

            if (m_Slayer2 != SlayerName.None && m_Slayer2 != m_Slayer)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer2);
                if (entry != null)
                {
                    attrs.Add(new EquipInfoAttribute(entry.GetTitle(EraAOS)));
                }

                ismagical = true;
            }

            if (m_DurabilityLevel != WeaponDurabilityLevel.Regular)
            {
                attrs.Add(new EquipInfoAttribute(1038000 + (int) m_DurabilityLevel));
                ismagical = true;
            }

            if (m_DamageLevel != WeaponDamageLevel.Regular)
            {
                attrs.Add(new EquipInfoAttribute(1038015 + (int) m_DamageLevel));
                ismagical = true;
            }

            if (m_AccuracyLevel != WeaponAccuracyLevel.Regular)
            {
                attrs.Add(new EquipInfoAttribute(1038010 + (int) m_AccuracyLevel));
                ismagical = true;
            }

            if (m_Poison != null && m_PoisonCharges > 0)
            {
                attrs.Add(new EquipInfoAttribute(m_Poison.LabelNumber > 0 ? m_Poison.LabelNumber : 1017383,
                    m_PoisonCharges));
            }

            if (m_EthicState != null)
            {
                if (m_EthicState.IsRunic)
                {
                    attrs.Add(new EquipInfoAttribute(m_EthicState.Ethic == Ethic.Evil ? 1045017 : 1045002));
                }
                else
                {
                    attrs.Add(new EquipInfoAttribute(m_EthicState.Ethic == Ethic.Evil ? 1042519 : 1042518));
                }

                ismagical = true;
            }

            var usesItem = this as IUsesRemaining;

            if (usesItem != null && usesItem.ShowUsesRemaining)
            {
                attrs.Add(new EquipInfoAttribute(1017323, usesItem.UsesRemaining));
            }

            return ismagical;
        }

        public static BaseWeapon Fists { get; set; }

        #region ICraftable Members

        public virtual int OnCraft(
            int quality,
            bool makersMark,
            Mobile from,
            CraftSystem craftSystem,
            Type typeRes,
            IBaseTool tool,
            CraftItem craftItem,
            int resHue)
        {
            Quality = (WeaponQuality) quality;

            if (makersMark)
            {
                Crafter = from;
            }

            PlayerConstructed = true;

            Type resourceType = typeRes;

            if (resourceType == null)
            {
                resourceType = craftItem.Resources.GetAt(0).ItemType;
            }

            Resource = CraftResources.GetFromType(resourceType);

            CraftContext context = craftSystem.GetContext(from);

            if (context != null && context.DoNotColor)
            {
                Hue = 0;
            }

            if (tool is BaseRunicTool)
            {
                CraftResource thisResource = CraftResources.GetFromType(resourceType);

                if (thisResource == ((BaseRunicTool) tool).Resource)
                {
                    Identified = true;
                    /*
					Resource = thisResource;

					CraftContext context = craftSystem.GetContext( from );

					if ( context != null && context.DoNotColor )
						Hue = 0;
*/
                    switch (thisResource)
                    {
                        case CraftResource.DullCopper:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Durable;
                            AccuracyLevel = WeaponAccuracyLevel.Accurate;
                            break;
                        }
                        case CraftResource.ShadowIron:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Durable;
                            DamageLevel = WeaponDamageLevel.Ruin;
                            break;
                        }
                        case CraftResource.Copper:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Fortified;
                            DamageLevel = WeaponDamageLevel.Ruin;
                            AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
                            break;
                        }
                        case CraftResource.Bronze:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Fortified;
                            DamageLevel = WeaponDamageLevel.Might;
                            AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
                            break;
                        }
                        case CraftResource.Gold:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                            DamageLevel = WeaponDamageLevel.Force;
                            AccuracyLevel = WeaponAccuracyLevel.Eminently;
                            break;
                        }
                        case CraftResource.Agapite:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                            DamageLevel = WeaponDamageLevel.Power;
                            AccuracyLevel = WeaponAccuracyLevel.Eminently;
                            break;
                        }
                        case CraftResource.Verite:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                            DamageLevel = WeaponDamageLevel.Power;
                            AccuracyLevel = WeaponAccuracyLevel.Exceedingly;
                            break;
                        }
                        case CraftResource.Valorite:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                            DamageLevel = WeaponDamageLevel.Vanq;
                            AccuracyLevel = WeaponAccuracyLevel.Supremely;
                            break;
                        }
                    }
                }
            }

            var region1 = from.Region as CustomRegion;

            if (region1 != null && region1.CraftRegion() && m_EthicState == null)
            {

                if (Quality == WeaponQuality.Exceptional)
                {                   

                    // Only care about types of wood higher than plain and upper bound it in case of additions
                    // Special woods start at 301 and go to 307
                    if (Resource > (CraftResource)301 && Resource <= (CraftResource)307)
                    {
                        var durabilityRoll = Utility.RandomDouble();
                        var damageRoll = Utility.RandomDouble();
                        var accuracyRoll = Utility.RandomDouble();                        

                        double modifierChance = 0.06;
                        bool addedModifier = false;

                        var woodQuality = (int)Resource - 300;                       

                        if (modifierChance > durabilityRoll)
                        {
                            var rolledDurability = Utility.Random(1, woodQuality - 1);
                            if (Enum.IsDefined(typeof(WeaponDurabilityLevel), rolledDurability))
                            {
                                DurabilityLevel = (WeaponDurabilityLevel)rolledDurability;                                
                                addedModifier = true;
                            }
                        }
                        if (modifierChance > damageRoll)
                        {
                            var rolledDamage = Utility.Random(1, woodQuality - 1);
                            if (Enum.IsDefined(typeof(WeaponDamageLevel), rolledDamage))
                            {
                                DamageLevel = (WeaponDamageLevel)rolledDamage;                               
                                addedModifier = true;
                            }
                        }
                        if (modifierChance > accuracyRoll)
                        {
                            var rolledAccuracy = Utility.Random(1, woodQuality - 1);

                            if (Enum.IsDefined(typeof(WeaponAccuracyLevel), rolledAccuracy))
                            {
                                AccuracyLevel = (WeaponAccuracyLevel)rolledAccuracy;                                
                                addedModifier = true;
                            }
                        }

                        if (addedModifier)
                        {
                            from.SendMessage("Your material and skill have added magical properties to this weapon.");                            
                        }                        
                    }                       
			    }
                

                if (Type == WeaponType.Ranged)
                {
                    double skillBaseFletching = from.Skills[SkillName.Fletching].Base;
                    double factor = (skillBaseFletching / 100.0) * 0.02;
                    if (factor >= Utility.RandomDouble())
                    {
                        from.SendMessage("You have successfully crafted a slayer weapon.");
                        Slayer = (SlayerName) Utility.Random(27) + 1;
                    }
                }
                if (Type == WeaponType.Staff)
                {
                    double skillBaseCarpentry = from.Skills[SkillName.Carpentry].Base;
                    double factor = (skillBaseCarpentry / 100.0) * 0.02;
                    if (factor >= Utility.RandomDouble())
                    {
                        from.SendMessage("You have successfully crafted a slayer weapon.");
                        Slayer = (SlayerName) Utility.Random(27) + 1;
                    }
                }
                else
                {
                    double skillBaseBS = from.Skills[SkillName.Blacksmith].Base;
                    double factor = (skillBaseBS / 100.0) * 0.02;
                    if (factor >= Utility.RandomDouble())
                    {
                        from.SendMessage("You have successfully crafted a slayer weapon.");
                        Slayer = (SlayerName) Utility.Random(27) + 1;
                    }
                }
            }
            return quality;
        }

        #endregion
    }

    public enum CheckSlayerResult
    {
        None,
        Slayer,
        Opposition
    }
}