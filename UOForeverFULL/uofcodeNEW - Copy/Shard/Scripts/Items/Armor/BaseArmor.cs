#region References

using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Engines.XmlSpawner2;
using Server.Ethics;
using Server.Factions;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using AMA = Server.Items.ArmorMeditationAllowance;
using AMT = Server.Items.ArmorMaterialType;
using ABT = Server.Items.ArmorBodyType;

#endregion

namespace Server.Items
{
    public abstract class BaseArmor
        : Item, IScissorable, IFactionItem, ICraftable, IWearableDurability, IEthicsItem, IIdentifiable, ISlayer
    {
        // BEGIN ALAN MOD
        private SlayerName m_Slayer;

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

        private SlayerName m_Slayer2;

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

        public virtual CheckSlayerResult CheckSlayers(Mobile attacker, Mobile defender)
        {
            if (attacker is PlayerMobile || attacker is HumanMob) // there are no slayer armor against them
            {
                return CheckSlayerResult.None;
            }

            SlayerEntry defSlayer = SlayerGroup.GetEntryByName(Slayer);
            SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName(Slayer2);

            if (Slayer == SlayerName.All)
            {
                return CheckSlayerResult.Slayer;
            }
            if (Slayer2 == SlayerName.All)
            {
                return CheckSlayerResult.Slayer;
            }

            if (defSlayer != null && defSlayer.Slays(attacker) || defSlayer2 != null && defSlayer2.Slays(attacker))
            {
                return CheckSlayerResult.Slayer;
            }

            return CheckSlayerResult.None;
        }

        // END ALAN MOD

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
                m_EthicState.IsRunic && m_EthicState.Ethic == parentState)
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
                m_EthicState.IsRunic && m_EthicState.Ethic == parentState)
            {
                return DeathMoveResult.MoveToBackpack;
            }
            else
            {
                return result;
            }
        }

        /* Armor internals work differently now (Jun 19 2003)
		 *
		 * The attributes defined below default to -1.
		 * If the value is -1, the corresponding virtual 'Aos/Old' property is used.
		 * If not, the attribute value itself is used. Here's the list:
		 *  - ArmorBase
		 *  - StrBonus
		 *  - DexBonus
		 *  - IntBonus
		 *  - StrReq
		 *  - DexReq
		 *  - IntReq
		 *  - MeditationAllowance
		 */

        // Instance values. These values must are unique to each armor piece.
        private int m_MaxHitPoints;
        private int m_HitPoints;
        private Mobile m_Crafter;
        private ArmorQuality m_Quality;
        private ArmorDurabilityLevel m_Durability;
        private ArmorProtectionLevel m_Protection;
        private CraftResource m_Resource;
        private bool m_Identified, m_PlayerConstructed;
        private bool m_NotScissorable; // Alan mod
        private bool m_Aesthetic;

        // Overridable values. These values are provided to override the defaults which get defined in the individual armor scripts.
        private int m_ArmorBase = -1;
        private int m_StrBonus = -1, m_DexBonus = -1, m_IntBonus = -1;
        private int m_StrReq = -1, m_DexReq = -1, m_IntReq = -1;
        private AMA m_Meditate = (AMA) (-1);

        public virtual bool AllowMaleWearer { get { return true; } }
        public virtual bool AllowFemaleWearer { get { return true; } }

        public abstract AMT MaterialType { get; }

        //		public virtual int RevertArmorBase{ get{ return ArmorBase; } }
        public virtual int ArmorBase { get { return 0; } }

        public virtual AMA DefMedAllowance { get { return AMA.None; } }
        public virtual AMA OldMedAllowance { get { return DefMedAllowance; } }

        public virtual int OldStrBonus { get { return 0; } }
        public virtual int OldDexBonus { get { return 0; } }
        public virtual int OldIntBonus { get { return 0; } }
        public virtual int OldStrReq { get { return 0; } }
        public virtual int OldDexReq { get { return 0; } }
        public virtual int OldIntReq { get { return 0; } }

        public virtual bool CanFortify { get { return true; } }

        public override void OnAfterDuped(Item newItem)
        {
            var armor = newItem as BaseArmor;

            if (armor == null)
            {
                return;
            }
        }

        #region Getters & Setters

        [CommandProperty(AccessLevel.GameMaster)] // Alan mod
        public bool NotScissorable
        {
            get { return m_NotScissorable; }
            set { m_NotScissorable = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)] // check to see if Aesthetic
        public bool IsAesthetic
        {
            get { return m_Aesthetic; }
            set
            {
                MeditationAllowance = AMA.All;
                StrRequirement = 0;
                DexRequirement = 0;
                DexBonus = 0;
                BaseArmorRating = 0;
                m_Aesthetic = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AMA MeditationAllowance
        {
            get { return (m_Meditate == (AMA) (-1) ? OldMedAllowance : m_Meditate); }
            set { m_Meditate = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseArmorRating
        {
            get
            {
                if (m_ArmorBase == -1)
                {
                    return ArmorBase;
                }
                else
                {
                    if (this is PlateChest || this is PlateHelm || this is PlateGorget || this is PlateArms ||
                        this is PlateGloves || this is PlateLegs)
                    {
                        var owner = this.RootParentEntity as Mobile;
                        if (owner != null && owner.IsT2A)
                            m_ArmorBase -= 10;
                    }
                    return m_ArmorBase;
                }
            }
            set
            {
                m_ArmorBase = value;
                Invalidate();
            }
        }

        public double BaseArmorRatingScaled { get { return (BaseArmorRating * ArmorScalar); } }

        public virtual double ArmorRating(Mobile attacker)
        {
            int ar = BaseArmorRating;

            Ethic attackerState = Ethic.Find(attacker);

            var owner = this.RootParentEntity as Mobile;

            int ethicar = 0;

            if ((owner == null || !owner.IsT2A) && m_EthicState != null && m_EthicState.IsRunic && attackerState != null &&
                attackerState != m_EthicState.Ethic)
                //Assume that Ethic is never null for items which have ethical states
            {
                ethicar = ar + 12;
            }
            else
            {
                if (m_Identified && m_Protection != ArmorProtectionLevel.Regular)
                {
                    ar += 10 + (5 * (int) m_Protection);
                }

                if ((owner == null || !owner.IsT2A))
                {
                    if (m_Resource != CraftResource.RegularLeather && m_Resource != CraftResource.SpinedLeather &&
                        m_Resource != CraftResource.HornedLeather && m_Resource != CraftResource.BarbedLeather)
                    {
                        switch (m_Quality)
                        {
                            case ArmorQuality.Exceptional:
                                ar += (int) Math.Floor(.2 * ar);
                                break;
                            case ArmorQuality.Low:
                                ar -= (int) Math.Floor(.2 * ar);
                                break;
                        }
                    }
                    switch (m_Resource)
                    {
                        case CraftResource.DullCopper:
                            ar += 2;
                            break;
                        case CraftResource.ShadowIron:
                            ar += 4;
                            break;
                        case CraftResource.Copper:
                            ar += 6;
                            break;
                        case CraftResource.Bronze:
                            ar += 8;
                            break;
                        case CraftResource.Gold:
                            ar += 10;
                            break;
                        case CraftResource.Agapite:
                            ar += 12;
                            break;
                        case CraftResource.Verite:
                            ar += 14;
                            break;
                        case CraftResource.Valorite:
                            ar += 16;
                            break;
                        case CraftResource.SpinedLeather:
                            ar += 8;
                            break;
                        case CraftResource.HornedLeather:
                            ar += 11;
                            break;
                        case CraftResource.BarbedLeather:
                            ar += 13;
                            break;
                    }
                    /*if (!IsAesthetic)
                {
                    ar += -4 + (4 * (int) m_Quality);
                }*/
                    if (m_Resource == CraftResource.RegularLeather || m_Resource == CraftResource.SpinedLeather ||
                        m_Resource == CraftResource.HornedLeather || m_Resource == CraftResource.BarbedLeather)
                    {
                        switch (m_Quality)
                        {
                            case ArmorQuality.Exceptional:
                                ar += (int) Math.Floor(.2 * ar);
                                break;
                            case ArmorQuality.Low:
                                ar -= (int) Math.Floor(.2 * ar);
                                break;
                        }
                    }
                }
                else
                {
                    switch (m_Quality)
                    {
                        case ArmorQuality.Exceptional:
                            ar += (int)Math.Floor(.2 * ar);
                            break;
                        case ArmorQuality.Low:
                            ar -= (int)Math.Floor(.2 * ar);
                            break;
                    }
                }
            }
            return ScaleArmorByDurability(Math.Max(ar, ethicar));
        }

        public double ArmorRatingScaled(Mobile attacker)
        {
            return (ArmorRating(attacker) * ArmorScalar);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StrBonus
        {
            get { return (m_StrBonus == -1 ? OldStrBonus : m_StrBonus); }
            set
            {
                m_StrBonus = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DexBonus
        {
            get { return (m_DexBonus == -1 ? OldDexBonus : m_DexBonus); }
            set
            {
                m_DexBonus = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int IntBonus
        {
            get { return (m_IntBonus == -1 ? OldIntBonus : m_IntBonus); }
            set
            {
                m_IntBonus = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StrRequirement
        {
            get { return (m_StrReq == -1 ? OldStrReq : m_StrReq); }
            set
            {
                m_StrReq = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DexRequirement
        {
            get { return (m_DexReq == -1 ? OldDexReq : m_DexReq); }
            set
            {
                m_DexReq = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int IntRequirement
        {
            get { return (m_IntReq == -1 ? OldIntReq : m_IntReq); }
            set
            {
                m_IntReq = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Identified
        {
            //No protection, durability, special hue, (dyability or not special craft material)
            get
            {
                return (ProtectionLevel == 0 && Durability == 0 &&
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

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Lead)]
        public bool PlayerConstructed { get { return m_PlayerConstructed; } set { m_PlayerConstructed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                if (m_Resource != value)
                {
                    UnscaleDurability();

                    m_Resource = value;

                    if (DefTailoring.IsColorable(GetType()))
                    {
                        Hue = CraftResources.GetHue(m_Resource);
                    }

                    Invalidate();
                    InvalidateProperties();

                    ScaleDurability();
                }
            }
        }

        #endregion

        public override Type DyeType
        {
            get
            {
                switch (MaterialType)
                {
                    default:
                        return typeof(DyeTub);
                    case AMT.Plate:
                    case AMT.Chainmail:
                    case AMT.Ringmail:
                        return typeof(RewardMetalDyeTub);
                    case AMT.Leather:
                    case AMT.Studded:
                    case AMT.Barbed:
                    case AMT.Spined:
                    case AMT.Horned:
                        return typeof(LeatherDyeTub);
                }
            }
        }

        public override bool DisplayDyable { get { return DyeType != typeof(LeatherDyeTub); } }

        public override bool Dye(Mobile from, IDyeTub sender)
        {
            if (m_Resource == CraftResource.None ||
                (m_Resource > CraftResource.RegularWood && m_Resource <= CraftResource.Frostwood))
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

        public virtual double ArmorScalar
        {
            get
            {
                var pos = (int) BodyPosition;

                if (pos >= 0 && pos < m_ArmorScalars.Length)
                {
                    return m_ArmorScalars[pos];
                }

                return 1.0;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get { return m_MaxHitPoints; }
            set
            {
                m_MaxHitPoints = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get { return m_HitPoints; }
            set
            {
                if (value != m_HitPoints && MaxHitPoints > 0)
                {
                    m_HitPoints = value;

                    if (m_HitPoints < 0)
                    {
                        Delete();
                    }
                    else if (m_HitPoints > MaxHitPoints)
                    {
                        m_HitPoints = MaxHitPoints;
                    }

                    InvalidateProperties();
                }
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
        public ArmorQuality Quality
        {
            get { return m_Quality; }
            set
            {
                UnscaleDurability();
                m_Quality = value;
                Invalidate();
                InvalidateProperties();
                ScaleDurability();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArmorDurabilityLevel Durability
        {
            get { return m_Durability; }
            set
            {
                UnscaleDurability();
                m_Durability = value;
                ScaleDurability();
                InvalidateProperties();
            }
        }

        public virtual int ArtifactRarity { get { return 0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArmorProtectionLevel ProtectionLevel
        {
            get { return m_Protection; }
            set
            {
                if (m_Protection != value)
                {
                    m_Protection = value;

                    Invalidate();
                    InvalidateProperties();
                }
            }
        }

        public virtual int InitMinHits { get { return 0; } }
        public virtual int InitMaxHits { get { return 0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArmorBodyType BodyPosition
        {
            get
            {
                switch (Layer)
                {
                    default:
                    case Layer.Neck:
                        return ArmorBodyType.Gorget;
                    case Layer.TwoHanded:
                        return ArmorBodyType.Shield;
                    case Layer.Gloves:
                        return ArmorBodyType.Gloves;
                    case Layer.Helm:
                        return ArmorBodyType.Helmet;
                    case Layer.Arms:
                        return ArmorBodyType.Arms;

                    case Layer.InnerLegs:
                    case Layer.OuterLegs:
                    case Layer.Pants:
                        return ArmorBodyType.Legs;

                    case Layer.InnerTorso:
                    case Layer.OuterTorso:
                    case Layer.Shirt:
                        return ArmorBodyType.Chest;
                }
            }
        }

        public CraftAttributeInfo GetResourceAttrs()
        {
            CraftResourceInfo info = CraftResources.GetInfo(m_Resource);

            if (info == null)
            {
                return CraftAttributeInfo.Blank;
            }

            return info.AttributeInfo;
        }

        public int GetProtOffset()
        {
            switch (m_Protection)
            {
                case ArmorProtectionLevel.Guarding:
                    return 1;
                case ArmorProtectionLevel.Hardening:
                    return 2;
                case ArmorProtectionLevel.Fortification:
                    return 3;
                case ArmorProtectionLevel.Invulnerability:
                    return 4;
            }

            return 0;
        }

        public void UnscaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_HitPoints = ((m_HitPoints * 100) + (scale - 1)) / scale;
            m_MaxHitPoints = ((m_MaxHitPoints * 100) + (scale - 1)) / scale;
            InvalidateProperties();
        }

        public void ScaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_HitPoints = ((m_HitPoints * scale) + 99) / 100;
            m_MaxHitPoints = ((m_MaxHitPoints * scale) + 99) / 100;
            InvalidateProperties();
        }

        public int GetDurabilityBonus()
        {
            int bonus = 0;

            if (m_Quality == ArmorQuality.Exceptional)
            {
                bonus += 20;
            }

            switch (m_Durability)
            {
                case ArmorDurabilityLevel.Durable:
                    bonus += 20;
                    break;
                case ArmorDurabilityLevel.Substantial:
                    bonus += 50;
                    break;
                case ArmorDurabilityLevel.Massive:
                    bonus += 70;
                    break;
                case ArmorDurabilityLevel.Fortified:
                    bonus += 100;
                    break;
                case ArmorDurabilityLevel.Indestructible:
                    bonus += 120;
                    break;
            }

            return bonus;
        }

        public bool Scissor(Mobile from, Scissors scissors)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(502437); // Items you wish to cut must be in your backpack.
                return false;
            }

            if (Ethic.IsImbued(this) || NotScissorable)
            {
                from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
                return false;
            }

            if (TestCenter.Enabled && !String.IsNullOrEmpty(Name) && Name.ToLower().IndexOf("beta") > -1)
            {
                from.SendMessage("This item is too special to be cut.");
                return false;
            }

            CraftSystem system = DefTailoring.CraftSystem;

            CraftItem item = system.CraftItems.SearchFor(GetType());

            if (item != null && item.Resources.Count == 1 && item.Resources.GetAt(0).Amount >= 2)
            {
                try
                {
                    var res = (Item) Activator.CreateInstance(CraftResources.GetInfo(m_Resource).ResourceTypes[0]);

                    ScissorHelper(from, res, m_PlayerConstructed ? (item.Resources.GetAt(0).Amount / 2) : 1);
                    return true;
                }
                catch
                {}
            }

            from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
            return false;
        }

        private static double[] m_ArmorScalars = {0.07, 0.07, 0.1375, 0.15, 0.22, 0.35};

        public static double[] ArmorScalars { get { return m_ArmorScalars; } set { m_ArmorScalars = value; } }

        public static void ValidateMobile(Mobile m)
        {
            for (int i = m.Items.Count - 1; i >= 0; --i)
            {
                if (i >= m.Items.Count)
                {
                    continue;
                }

                Item item = m.Items[i];

                if (item is BaseArmor)
                {
                    var armor = (BaseArmor) item;

                    if (armor.RequiredRace != null && m.Race != armor.RequiredRace)
                    {
                        if (armor.RequiredRace == Race.Elf)
                        {
                            m.SendLocalizedMessage(1072203); // Only Elves may use this.
                        }
                        else
                        {
                            m.SendMessage("Only {0} may use this.", armor.RequiredRace.PluralName);
                        }

                        m.AddToBackpack(armor);
                    }
                    else if (!armor.AllowMaleWearer && !m.Female && m.AccessLevel < AccessLevel.GameMaster)
                    {
                        if (armor.AllowFemaleWearer)
                        {
                            m.SendLocalizedMessage(1010388); // Only females can wear this.
                        }
                        else
                        {
                            m.SendMessage("You may not wear this.");
                        }

                        m.AddToBackpack(armor);
                    }
                    else if (!armor.AllowFemaleWearer && m.Female && m.AccessLevel < AccessLevel.GameMaster)
                    {
                        if (armor.AllowMaleWearer)
                        {
                            m.SendLocalizedMessage(1063343); // Only males can wear this.
                        }
                        else
                        {
                            m.SendMessage("You may not wear this.");
                        }

                        m.AddToBackpack(armor);
                    }
                }
            }
        }

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                var from = (Mobile) parent;
                from.Delta(MobileDelta.Armor); // Tell them armor rating has changed
            }
        }

        public virtual double ScaleArmorByDurability(double armor)
        {
            int scale = 100;

            if (m_MaxHitPoints > 0 && m_HitPoints < m_MaxHitPoints)
            {
                scale = 50 + ((50 * m_HitPoints) / m_MaxHitPoints);
            }

            return (armor * scale) / 100;
        }

        protected void Invalidate()
        {
            if (Parent is Mobile)
            {
                ((Mobile) Parent).Delta(MobileDelta.Armor); // Tell them armor rating has changed
            }
        }

        public BaseArmor(Serial serial)
            : base(serial)
        {}

        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
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

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            Attributes = 0x00000001,
            ArmorAttributes = 0x00000002,
            PhysicalBonus = 0x00000004,
            FireBonus = 0x00000008,
            ColdBonus = 0x00000010,
            PoisonBonus = 0x00000020,
            EnergyBonus = 0x00000040,
            Identified = 0x00000080,
            MaxHitPoints = 0x00000100,
            HitPoints = 0x00000200,
            Crafter = 0x00000400,
            Quality = 0x00000800,
            Durability = 0x00001000,
            Protection = 0x00002000,
            Resource = 0x00004000,
            BaseArmor = 0x00008000,
            StrBonus = 0x00010000,
            DexBonus = 0x00020000,
            IntBonus = 0x00040000,
            StrReq = 0x00080000,
            DexReq = 0x00100000,
            IntReq = 0x00200000,
            MedAllowance = 0x00400000,
            SkillBonuses = 0x00800000,
            PlayerConstructed = 0x01000000,
            Slayer = 0x02000000,
            Slayer2 = 0x04000000,
            NotScissorable = 0x08000000,
            Aesthetic = 0x10000000
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

            writer.Write(7); // version

            var flags = SaveFlag.None;

            //SetSaveFlag( ref flags, SaveFlag.Attributes,		false ); //obsolete
            //SetSaveFlag( ref flags, SaveFlag.ArmorAttributes,	false ); //obsolete
            SetSaveFlag(ref flags, SaveFlag.Identified, m_Identified);
            SetSaveFlag(ref flags, SaveFlag.MaxHitPoints, m_MaxHitPoints != 0);
            SetSaveFlag(ref flags, SaveFlag.HitPoints, m_HitPoints != 0);
            SetSaveFlag(ref flags, SaveFlag.Crafter, m_Crafter != null);
            SetSaveFlag(ref flags, SaveFlag.Quality, m_Quality != ArmorQuality.Regular);
            SetSaveFlag(ref flags, SaveFlag.Durability, m_Durability != ArmorDurabilityLevel.Regular);
            SetSaveFlag(ref flags, SaveFlag.Protection, m_Protection != ArmorProtectionLevel.Regular);
            SetSaveFlag(ref flags, SaveFlag.Resource, m_Resource != DefaultResource);
            SetSaveFlag(ref flags, SaveFlag.BaseArmor, m_ArmorBase != -1);
            SetSaveFlag(ref flags, SaveFlag.StrBonus, m_StrBonus != -1);
            SetSaveFlag(ref flags, SaveFlag.DexBonus, m_DexBonus != -1);
            SetSaveFlag(ref flags, SaveFlag.IntBonus, m_IntBonus != -1);
            SetSaveFlag(ref flags, SaveFlag.StrReq, m_StrReq != -1);
            SetSaveFlag(ref flags, SaveFlag.DexReq, m_DexReq != -1);
            SetSaveFlag(ref flags, SaveFlag.IntReq, m_IntReq != -1);
            SetSaveFlag(ref flags, SaveFlag.MedAllowance, m_Meditate != (AMA) (-1));

            //SetSaveFlag( ref flags, SaveFlag.SkillBonuses,		false ); //obsolete
            SetSaveFlag(ref flags, SaveFlag.PlayerConstructed, m_PlayerConstructed);
            SetSaveFlag(ref flags, SaveFlag.Slayer2, m_Slayer2 != SlayerName.None); // Alan Mod
            SetSaveFlag(ref flags, SaveFlag.Slayer, m_Slayer != SlayerName.None); // Alan Mod
            SetSaveFlag(ref flags, SaveFlag.NotScissorable, m_NotScissorable); // Alan Mod
            SetSaveFlag(ref flags, SaveFlag.Aesthetic, m_Aesthetic); // Aesthetic

            writer.WriteEncodedInt((int) flags);

            // alan mod
            if (GetSaveFlag(flags, SaveFlag.Slayer))
            {
                writer.WriteEncodedInt((int) m_Slayer);
            }

            if (GetSaveFlag(flags, SaveFlag.Slayer2))
            {
                writer.WriteEncodedInt((int) m_Slayer2);
            }
            // end alan mod

            // obsolete
            //if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
            //	m_AosAttributes.Serialize( writer );

            // obsolete
            //if ( GetSaveFlag( flags, SaveFlag.ArmorAttributes ) )
            //	m_AosArmorAttributes.Serialize( writer );

            if (GetSaveFlag(flags, SaveFlag.MaxHitPoints))
            {
                writer.WriteEncodedInt(m_MaxHitPoints);
            }

            if (GetSaveFlag(flags, SaveFlag.HitPoints))
            {
                writer.WriteEncodedInt(m_HitPoints);
            }

            if (GetSaveFlag(flags, SaveFlag.Crafter))
            {
                writer.Write(m_Crafter);
            }

            if (GetSaveFlag(flags, SaveFlag.Quality))
            {
                writer.WriteEncodedInt((int) m_Quality);
            }

            if (GetSaveFlag(flags, SaveFlag.Durability))
            {
                writer.WriteEncodedInt((int) m_Durability);
            }

            if (GetSaveFlag(flags, SaveFlag.Protection))
            {
                writer.WriteEncodedInt((int) m_Protection);
            }

            if (GetSaveFlag(flags, SaveFlag.Resource))
            {
                writer.WriteEncodedInt((int) m_Resource);
            }

            if (GetSaveFlag(flags, SaveFlag.BaseArmor))
            {
                writer.WriteEncodedInt(m_ArmorBase);
            }

            if (GetSaveFlag(flags, SaveFlag.StrBonus))
            {
                writer.WriteEncodedInt(m_StrBonus);
            }

            if (GetSaveFlag(flags, SaveFlag.DexBonus))
            {
                writer.WriteEncodedInt(m_DexBonus);
            }

            if (GetSaveFlag(flags, SaveFlag.IntBonus))
            {
                writer.WriteEncodedInt(m_IntBonus);
            }

            if (GetSaveFlag(flags, SaveFlag.StrReq))
            {
                writer.WriteEncodedInt(m_StrReq);
            }

            if (GetSaveFlag(flags, SaveFlag.DexReq))
            {
                writer.WriteEncodedInt(m_DexReq);
            }

            if (GetSaveFlag(flags, SaveFlag.IntReq))
            {
                writer.WriteEncodedInt(m_IntReq);
            }

            if (GetSaveFlag(flags, SaveFlag.MedAllowance))
            {
                writer.WriteEncodedInt((int) m_Meditate);
            }

            // obsolete
            //if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
            //	m_AosSkillBonuses.Serialize( writer );
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 7:
                case 6:
                case 5:
                {
                    var flags = (SaveFlag) reader.ReadEncodedInt();

                    if (GetSaveFlag(flags, SaveFlag.Aesthetic)) // Alan Mod
                    {
                        m_Aesthetic = true;
                    }
                    // alan mod
                    if (GetSaveFlag(flags, SaveFlag.NotScissorable)) // Alan Mod
                    {
                        m_NotScissorable = true;
                    }

                    // alan mod
                    if (GetSaveFlag(flags, SaveFlag.Slayer))
                    {
                        m_Slayer = (SlayerName) reader.ReadEncodedInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.Slayer2))
                    {
                        m_Slayer2 = (SlayerName) reader.ReadEncodedInt();
                    }
                    // end alan mod

                    // BEGIN OBSOLETE (removed AOS stuff)... after first save, this will never happen
                    if (GetSaveFlag(flags, SaveFlag.Attributes))
                    {
                        new AosAttributes(this, reader);
                    }
                    //else
                    //	m_AosAttributes = new AosAttributes( this );

                    if (GetSaveFlag(flags, SaveFlag.ArmorAttributes))
                    {
                        new AosArmorAttributes(this, reader);
                    }
                    //else
                    //	m_AosArmorAttributes = new AosArmorAttributes( this );

                    if (GetSaveFlag(flags, SaveFlag.PhysicalBonus))
                    {
                        reader.ReadEncodedInt(); //m_PhysicalBonus = reader.ReadEncodedInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.FireBonus))
                    {
                        reader.ReadEncodedInt(); //m_FireBonus = reader.ReadEncodedInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.ColdBonus))
                    {
                        reader.ReadEncodedInt(); //m_ColdBonus = reader.ReadEncodedInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.PoisonBonus))
                    {
                        reader.ReadEncodedInt(); //m_PoisonBonus = reader.ReadEncodedInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.EnergyBonus))
                    {
                        reader.ReadEncodedInt(); //m_EnergyBonus = reader.ReadEncodedInt();
                    }
                    // END OBSOLETE
                    if (GetSaveFlag(flags, SaveFlag.Identified))
                    {
                        m_Identified = (version >= 7 || reader.ReadBool());
                    }

                    if (GetSaveFlag(flags, SaveFlag.MaxHitPoints))
                    {
                        m_MaxHitPoints = reader.ReadEncodedInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.HitPoints))
                    {
                        m_HitPoints = reader.ReadEncodedInt();
                    }

                    if (GetSaveFlag(flags, SaveFlag.Crafter))
                    {
                        m_Crafter = reader.ReadMobile();
                    }

                    if (GetSaveFlag(flags, SaveFlag.Quality))
                    {
                        m_Quality = (ArmorQuality) reader.ReadEncodedInt();
                    }
                    else
                    {
                        m_Quality = ArmorQuality.Regular;
                    }

                    if (version == 5 && m_Quality == ArmorQuality.Low)
                    {
                        m_Quality = ArmorQuality.Regular;
                    }

                    if (GetSaveFlag(flags, SaveFlag.Durability))
                    {
                        m_Durability = (ArmorDurabilityLevel) reader.ReadEncodedInt();

                        if (m_Durability > ArmorDurabilityLevel.Indestructible)
                        {
                            m_Durability = ArmorDurabilityLevel.Durable;
                        }
                    }

                    if (GetSaveFlag(flags, SaveFlag.Protection))
                    {
                        m_Protection = (ArmorProtectionLevel) reader.ReadEncodedInt();

                        if (m_Protection > ArmorProtectionLevel.Invulnerability)
                        {
                            m_Protection = ArmorProtectionLevel.Defense;
                        }
                    }

                    if (GetSaveFlag(flags, SaveFlag.Resource))
                    {
                        m_Resource = (CraftResource) reader.ReadEncodedInt();
                    }
                    else
                    {
                        m_Resource = DefaultResource;
                    }

                    if (m_Resource == CraftResource.None)
                    {
                        m_Resource = DefaultResource;
                    }

                    if (GetSaveFlag(flags, SaveFlag.BaseArmor))
                    {
                        m_ArmorBase = reader.ReadEncodedInt();
                    }
                    else
                    {
                        m_ArmorBase = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.StrBonus))
                    {
                        m_StrBonus = reader.ReadEncodedInt();
                    }
                    else
                    {
                        m_StrBonus = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.DexBonus))
                    {
                        m_DexBonus = reader.ReadEncodedInt();
                    }
                    else
                    {
                        m_DexBonus = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.IntBonus))
                    {
                        m_IntBonus = reader.ReadEncodedInt();
                    }
                    else
                    {
                        m_IntBonus = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.StrReq))
                    {
                        m_StrReq = reader.ReadEncodedInt();
                    }
                    else
                    {
                        m_StrReq = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.DexReq))
                    {
                        m_DexReq = reader.ReadEncodedInt();
                    }
                    else
                    {
                        m_DexReq = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.IntReq))
                    {
                        m_IntReq = reader.ReadEncodedInt();
                    }
                    else
                    {
                        m_IntReq = -1;
                    }

                    if (GetSaveFlag(flags, SaveFlag.MedAllowance))
                    {
                        m_Meditate = (AMA) reader.ReadEncodedInt();
                    }
                    else
                    {
                        m_Meditate = (AMA) (-1);
                    }

                    if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                    {
                        new AosSkillBonuses(this, reader); // eventually will be unnecessary
                    }

                    if (GetSaveFlag(flags, SaveFlag.PlayerConstructed))
                    {
                        m_PlayerConstructed = true;
                    }

                    break;
                }
                case 4:
                {
                    new AosAttributes(this, reader); // evenutally won't be necessary
                    new AosArmorAttributes(this, reader); // evenutally won't be necessary
                    goto case 3;
                }
                case 3:
                {
                    reader.ReadInt(); //m_PhysicalBonus = 
                    reader.ReadInt(); // m_FireBonus = 
                    reader.ReadInt(); // m_ColdBonus = 
                    reader.ReadInt(); // m_PoisonBonus = 
                    reader.ReadInt(); // m_EnergyBonus = 
                    goto case 2;
                }
                case 2:
                case 1:
                {
                    m_Identified = reader.ReadBool();
                    goto case 0;
                }
                case 0:
                {
                    m_ArmorBase = reader.ReadInt();
                    m_MaxHitPoints = reader.ReadInt();
                    m_HitPoints = reader.ReadInt();
                    m_Crafter = reader.ReadMobile();
                    m_Quality = (ArmorQuality) reader.ReadInt();
                    m_Durability = (ArmorDurabilityLevel) reader.ReadInt();
                    m_Protection = (ArmorProtectionLevel) reader.ReadInt();

                    var mat = (AMT) reader.ReadInt();

                    //					if ( m_ArmorBase == RevertArmorBase )
                    //						m_ArmorBase = -1;

                    /*m_BodyPos = (ArmorBodyType)*/
                    reader.ReadInt();

                    //if ( version < 3 && m_Quality == ArmorQuality.Exceptional )
                    //	DistributeBonuses( 6 );

                    if (version >= 2)
                    {
                        m_Resource = (CraftResource) reader.ReadInt();
                    }
                    else
                    {
                        OreInfo info;

                        switch (reader.ReadInt())
                        {
                            default:
                            case 0:
                                info = OreInfo.Iron;
                                break;
                            case 1:
                                info = OreInfo.DullCopper;
                                break;
                            case 2:
                                info = OreInfo.ShadowIron;
                                break;
                            case 3:
                                info = OreInfo.Copper;
                                break;
                            case 4:
                                info = OreInfo.Bronze;
                                break;
                            case 5:
                                info = OreInfo.Gold;
                                break;
                            case 6:
                                info = OreInfo.Agapite;
                                break;
                            case 7:
                                info = OreInfo.Verite;
                                break;
                            case 8:
                                info = OreInfo.Valorite;
                                break;
                        }

                        m_Resource = CraftResources.GetFromOreInfo(info, mat);
                    }

                    m_StrBonus = reader.ReadInt();
                    m_DexBonus = reader.ReadInt();
                    m_IntBonus = reader.ReadInt();
                    m_StrReq = reader.ReadInt();
                    m_DexReq = reader.ReadInt();
                    m_IntReq = reader.ReadInt();

                    if (m_StrBonus == OldStrBonus)
                    {
                        m_StrBonus = -1;
                    }

                    if (m_DexBonus == OldDexBonus)
                    {
                        m_DexBonus = -1;
                    }

                    if (m_IntBonus == OldIntBonus)
                    {
                        m_IntBonus = -1;
                    }

                    if (m_StrReq == OldStrReq)
                    {
                        m_StrReq = -1;
                    }

                    if (m_DexReq == OldDexReq)
                    {
                        m_DexReq = -1;
                    }

                    if (m_IntReq == OldIntReq)
                    {
                        m_IntReq = -1;
                    }

                    m_Meditate = (AMA) reader.ReadInt();

                    if (m_Meditate == OldMedAllowance)
                    {
                        m_Meditate = (AMA) (-1);
                    }

                    if (m_Resource == CraftResource.None)
                    {
                        if (mat == ArmorMaterialType.Studded || mat == ArmorMaterialType.Leather)
                        {
                            m_Resource = CraftResource.RegularLeather;
                        }
                        else if (mat == ArmorMaterialType.Spined)
                        {
                            m_Resource = CraftResource.SpinedLeather;
                        }
                        else if (mat == ArmorMaterialType.Horned)
                        {
                            m_Resource = CraftResource.HornedLeather;
                        }
                        else if (mat == ArmorMaterialType.Barbed)
                        {
                            m_Resource = CraftResource.BarbedLeather;
                        }
                        else
                        {
                            m_Resource = CraftResource.Iron;
                        }
                    }

                    if (m_MaxHitPoints == 0 && m_HitPoints == 0)
                    {
                        m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);
                    }

                    break;
                }
            }

            if (Parent is Mobile && (StrBonus != 0 || DexBonus != 0 || IntBonus != 0))
            {
                var m = (Mobile) Parent;

                string modName = Serial.ToString();

                if (StrBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Str, modName + "Str", StrBonus, TimeSpan.Zero));
                }

                if (DexBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", DexBonus, TimeSpan.Zero));
                }

                if (IntBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Int, modName + "Int", IntBonus, TimeSpan.Zero));
                }
            }

            if (Parent is Mobile)
            {
                ((Mobile) Parent).CheckStatTimers();
            }

            if (version < 7)
            {
                m_PlayerConstructed = true; // we don't know, so, assume it's crafted
            }
        }

        public virtual CraftResource DefaultResource { get { return CraftResource.Iron; } }

        public BaseArmor(int itemID)
            : base(itemID)
        {
            m_Quality = ArmorQuality.Regular;
            m_Durability = ArmorDurabilityLevel.Regular;
            m_Crafter = null;

            m_Resource = DefaultResource;
            Hue = CraftResources.GetHue(m_Resource);

            m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);

            Layer = (Layer) ItemData.Quality;
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

        public override bool CanEquip(Mobile from)
        {
            if (!Ethic.CheckEquip(from, this))
            {
                return false;
            }

            if (from.AccessLevel < AccessLevel.GameMaster)
            {
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
                else if (!AllowMaleWearer && !from.Female)
                {
                    if (AllowFemaleWearer)
                    {
                        from.SendLocalizedMessage(1010388); // Only females can wear this.
                    }
                    else
                    {
                        from.SendMessage("You may not wear this.");
                    }

                    return false;
                }
                else if (!AllowFemaleWearer && from.Female)
                {
                    if (AllowMaleWearer)
                    {
                        from.SendLocalizedMessage(1063343); // Only males can wear this.
                    }
                    else
                    {
                        from.SendMessage("You may not wear this.");
                    }

                    return false;
                }
                else
                {
                    if (from.Dex < DexRequirement || (from.Dex + DexBonus) < 1)
                    {
                        from.SendLocalizedMessage(502077); // You do not have enough dexterity to equip this item.
                        return false;
                    }
                    else if (from.Str < StrRequirement || (from.Str + StrBonus) < 1)
                    {
                        from.SendLocalizedMessage(500213); // You are not strong enough to equip that.
                        return false;
                    }
                    else if (from.Int < IntRequirement || (from.Int + IntBonus) < 1)
                    {
                        from.SendMessage("You are not smart enough to equip that.");
                        return false;
                    }
                }
            }
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

        public override bool CheckPropertyConfliction(Mobile m)
        {
            if (base.CheckPropertyConfliction(m))
            {
                return true;
            }

            if (Layer == Layer.Pants)
            {
                return (m.FindItemOnLayer(Layer.InnerLegs) != null);
            }

            if (Layer == Layer.Shirt)
            {
                return (m.FindItemOnLayer(Layer.InnerTorso) != null);
            }

            return false;
        }

        public override bool OnEquip(Mobile from)
        {
            from.CheckStatTimers();

            if (StrBonus != 0 || DexBonus != 0 || IntBonus != 0)
            {
                string modName = Serial.ToString();

                if (StrBonus != 0)
                {
                    from.AddStatMod(new StatMod(StatType.Str, modName + "Str", StrBonus, TimeSpan.Zero));
                }

                if (DexBonus != 0)
                {
                    from.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", DexBonus, TimeSpan.Zero));
                }

                if (IntBonus != 0)
                {
                    from.AddStatMod(new StatMod(StatType.Int, modName + "Int", IntBonus, TimeSpan.Zero));
                }
            }

            if (!Identified && Resource == CraftResource.BlackRock)
            {
                from.ApplyPoison(from, Poison.Deadly);
                from.SendMessage("The armor has infected you.");
            }

            // XmlAttachment check for OnEquip
            XmlAttach.CheckOnEquip(this, from);

            // true if return override
            if (XmlScript.HasTrigger(this, TriggerName.onEquip) &&
                UberScriptTriggers.Trigger(this, from, TriggerName.onEquip))
            {
                return false;
            }

            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                var m = (Mobile) parent;
                string modName = Serial.ToString();

                m.RemoveStatMod(modName + "Str");
                m.RemoveStatMod(modName + "Dex");
                m.RemoveStatMod(modName + "Int");

                ((Mobile) parent).Delta(MobileDelta.Armor); // Tell them armor rating has changed
                m.CheckStatTimers();

                if (XmlScript.HasTrigger(this, TriggerName.onUnequip))
                {
                    UberScriptTriggers.Trigger(this, (Mobile) parent, TriggerName.onUnequip);
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

            // XmlAttachment check for OnRemoved... forget it now that we got uberscript
            // Server.Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent);

            base.OnRemoved(parent);
        }

        public override void OnDelete()
        {
            if (XmlScript.HasTrigger(this, TriggerName.onDelete))
            {
                UberScriptTriggers.Trigger(this, RootParentEntity as Mobile, TriggerName.onDelete);
            }
            base.OnDelete();
        }

        public virtual int OnHit(BaseWeapon weapon, Mobile attacker, int damageTaken)
        {
            var defender = Parent as Mobile;
            double armor = ArmorRating(attacker);
            //double halfAR = ArmorRating(attacker) / 2.0;
            /*if (defender != null && defender.Skills[SkillName.Magery].Value >= 75 &&
                defender.Skills[SkillName.EvalInt].Value >= 75)
            {
                armor += 9.0;
            }*/
            double halfAR = armor / 2.0;

            if (halfAR < 7.80)
                halfAR = 7.80;

            // absorb between 0.5 and 1.0 of halfAR's value
            //var absorbed = (int)(halfAR + (halfAR/4 * Utility.RandomDouble()));//even though this formula is UOR accurate, seems to negate too much
            //var absorbed = (int)(thirdAR + ((thirdAR * Utility.RandomDouble())));
            var absorbed = (int) ((halfAR) + (halfAR * Utility.RandomDouble()));
            //attacker.SendMessage(54, "Absorbed using regular formula is " + absorbed);
            if (absorbed > damageTaken - 10)
            {
                absorbed = (int) ((((halfAR * 2.0) * 0.0092) + (Utility.RandomDouble() * (0.1))) * damageTaken);
                //attacker.SendMessage(54, "Absorbed using percentage formula is " + absorbed);
            }
            damageTaken -= absorbed;

            if (damageTaken < 0)
            {
                damageTaken = 0;
            }

            if (absorbed < 2)
            {
                absorbed = 2;
            }

            if ((weapon.Type == WeaponType.Bashing) && !(weapon is BaseWand) ||
                0.20 > Utility.RandomDouble() && LootType != LootType.Blessed)
                // 20% chance to lower durability IF not blessed
            {
                int wear;

                if ((weapon.Type == WeaponType.Bashing) && !(weapon is BaseWand))
                {
                    wear = absorbed / 5;
                }
                else
                {
                    wear = Utility.Random(2);
                }

                if (wear > 0 && m_MaxHitPoints > 0 && !IsAesthetic)
                {
                    if (m_HitPoints >= wear)
                    {
                        HitPoints -= wear;
                        wear = 0;
                    }
                    else
                    {
                        wear -= HitPoints;
                        HitPoints = 0;
                    }

                    if (wear > 0)
                    {
                        if (m_MaxHitPoints > wear)
                        {
                            MaxHitPoints -= wear;

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
                }
            }

            // Alan Mod
            if (Parent is Mobile)
            {
                if (CheckSlayers(attacker, (Mobile) Parent) == CheckSlayerResult.Slayer)
                {
                    ((Mobile) Parent).FixedEffect(0x37B9, 10, 5);
                    damageTaken = damageTaken / 2 + 1;
                    if (damageTaken <= 0)
                    {
                        damageTaken = 1;
                    }
                }
            }
            // end Alan Mod

            if (XmlScript.HasTrigger(this, TriggerName.onArmorHit))
            {
                UberScriptTriggers.Trigger(this, attacker, TriggerName.onArmorHit, weapon, null, null, damageTaken);
            }

            return damageTaken;
        }

        private string GetNameString()
        {
            string name = Name;

            if (name == null)
            {
                name = String.Format("#{0}", LabelNumber);
            }

            return name;
        }

        public int GetLeatherLabel(int baselabel)
        {
            if (Identified && Resource >= CraftResource.SpinedLeather && Resource <= CraftResource.BarbedLeather)
            {
                return baselabel + (26 * (Resource - CraftResource.SpinedLeather));
            }
            else
            {
                return base.LabelNumber;
            }
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

            if (m_Quality == ArmorQuality.Exceptional)
            {
                if (oreType != 0)
                {
                    list.Add(1053100, "#{0}\t{1}", oreType, GetNameString()); // exceptional ~1_oretype~ ~2_armortype~
                }
                else
                {
                    list.Add(1050040, GetNameString()); // exceptional ~1_ITEMNAME~
                }
            }
            else
            {
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
            }
        }

        public override bool AllowEquippedCast(Mobile from)
        {
            if (base.AllowEquippedCast(from))
            {
                return true;
            }

            return false;
        }

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

            return attrInfo.ArmorLuck;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Crafter != null)
            {
                list.Add(1050043, m_Crafter.RawName); // crafted by ~1_NAME~
            }
            if (m_Quality == ArmorQuality.Exceptional &&
                (m_Resource >= CraftResource.RegularWood && m_Resource <= CraftResource.Frostwood))
            {
                list.Add(1060636); // exceptional
            }

            #region Factions

            if (m_FactionState != null)
            {
                list.Add(1041350); // faction item
            }

            #endregion

            if (RequiredRace == Race.Elf)
            {
                list.Add(1075086); // Elves Only
            }

            //m_AosSkillBonuses.GetProperties( list );

            int prop;

            if ((prop = ArtifactRarity) > 0)
            {
                list.Add(1061078, prop.ToString()); // artifact rarity ~1_val~
            }

            if ((prop = GetDurabilityBonus()) > 0)
            {
                list.Add(1060410, prop.ToString()); // durability ~1_val~%
            }

            if ((prop = StrRequirement) > 0)
            {
                list.Add(1061170, prop.ToString()); // strength requirement ~1_val~
            }

            if (m_HitPoints >= 0 && m_MaxHitPoints > 0)
            {
                list.Add(1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints); // durability ~1_val~ / ~2_val~
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Deleted || !from.CanSee(this))
            {
                return;
            }

            LabelToExpansion(from);


            int number;
            string name = Name;

            if (IsAesthetic)
                name = name + " [Aesthetic]";
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
                //Custom metals for loot
                if (m_Resource > CraftResource.Valorite && m_Resource < CraftResource.RegularLeather)
                {
                    StringList stringlist = StringList.Default;

                    string labelname = String.Empty;

                    if (from.NetState != null && from.NetState.LTable != null)
                    {
                        stringlist = from.NetState.LTable;
                    }

                    if (stringlist[LabelNumber] != null)
                    {
                        labelname = stringlist[LabelNumber].Text;
                    }

                    if (labelname.StartsWith("a ") && labelname.Length > 2)
                    {
                        labelname = labelname.Substring(2, labelname.Length - 2);
                    }
                    else if (labelname.StartsWith("an ") && labelname.Length > 3)
                    {
                        labelname = labelname.Substring(3, labelname.Length - 3);
                    }

                    number = 1041000;

                    if (Identified)
                    {
                        LabelTo(from, String.Format("{0} {1}", CraftResources.GetName(m_Resource).ToLower(), labelname),
                            1153);
                    }
                    else if (m_Resource == CraftResource.BlackRock)
                    {
                        LabelTo(from,
                            String.Format("infected {1}", CraftResources.GetName(m_Resource).ToLower(), labelname), 1272);
                    }
                    else
                    {
                        number = LabelNumber;
                    }
                }
                else
                {
                    number = LabelNumber;
                }
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

            if (attrs.Count > 0 || Crafter != null || ismagical || number != 1041000)
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

            if (m_Quality != ArmorQuality.Regular)
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

            if (m_Durability != ArmorDurabilityLevel.Regular)
            {
                attrs.Add(new EquipInfoAttribute(1038000 + (int) m_Durability));
                ismagical = true;
            }

            if (m_Protection > ArmorProtectionLevel.Regular && m_Protection <= ArmorProtectionLevel.Invulnerability)
            {
                attrs.Add(new EquipInfoAttribute(1038005 + (int) m_Protection));
                ismagical = true;
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

            return ismagical;
        }

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
            Quality = (ArmorQuality) quality;

            if (makersMark)
            {
                Crafter = from;
            }

            Type resourceType = typeRes;

            if (resourceType == null)
            {
                resourceType = craftItem.Resources.GetAt(0).ItemType;
            }

            Resource = CraftResources.GetFromType(resourceType);
            PlayerConstructed = true;
            Identified = true;

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

                    switch (thisResource)
                    {
                        case CraftResource.SpinedLeather:
                        {
                            Identified = true;
                            ProtectionLevel = ArmorProtectionLevel.Defense;
                            Durability = ArmorDurabilityLevel.Durable;
                            break;
                        }
                        case CraftResource.HornedLeather:
                        {
                            Identified = true;
                            ProtectionLevel = ArmorProtectionLevel.Guarding;
                            Durability = ArmorDurabilityLevel.Substantial;

                            break;
                        }
                        case CraftResource.BarbedLeather:
                        {
                            Identified = true;
                            ProtectionLevel = ArmorProtectionLevel.Hardening;
                            Durability = ArmorDurabilityLevel.Massive;
                            break;
                        }
                    }
                }
            }
            return quality;
        }

        #endregion
    }
}