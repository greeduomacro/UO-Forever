using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.Conquests;
using Server.Engines.EventInvasions;
using Server.Engines.Portals;
using Server.Engines.ZombieEvent;
using Server.Mobiles.MetaPet;
using Server.Mobiles.MetaSkills;
using Server.Regions;
using Server.Network;
using Server.Multis;
using Server.Spells;
using Server.Misc;
using Server.Items;
using Server.ContextMenus;
using Server.Engines.Quests;
using Server.Engines.PartySystem;
using Server.Factions;
using Server.SkillHandlers;
using Server.Engines.CannedEvil;
using Server.Games;
using Server.Engines.XmlSpawner2;
using Server.Commands;
using System.Reflection;
using System.Text;
using System.IO;
using Server.Accounting;

namespace Server.Mobiles
{
	#region Enums
	/// <summary>
	/// Summary description for MobileAI.
	/// </summary>
	///
	public enum FightMode
	{
		None,			// Never focus on others
		Aggressor,		// Only attack aggressors
		Strongest,		// Attack the strongest
		Weakest,		// Attack the weakest
		Closest, 		// Attack the closest
		Evil			// Only attack aggressor -or- negative karma
	}

	public enum OrderType
	{
		None,			//When no order, let's roam
		Come,			//"(All/Name) come"  Summons all or one pet to your location.
		Drop,			//"(Name) drop"  Drops its loot to the ground (if it carries any).
		Follow,			//"(Name) follow"  Follows targeted being.
						//"(All/Name) follow me"  Makes all or one pet follow you.
		Friend,			//"(Name) friend"  Allows targeted player to confirm resurrection.
		Unfriend,		// Remove a friend
		Guard,			//"(Name) guard"  Makes the specified pet guard you. Pets can only guard their owner.
						//"(All/Name) guard me"  Makes all or one pet guard you.
		Attack,			//"(All/Name) kill",
						//"(All/Name) attack"  All or the specified pet(s) currently under your control attack the target.
		Patrol,			//"(Name) patrol"  Roves between two or more guarded targets.
		Release,		//"(Name) release"  Releases pet back into the wild (removes "tame" status).
		Stay,			//"(All/Name) stay" All or the specified pet(s) will stop and stay in current spot.
		Stop,			//"(All/Name) stop Cancels any current orders to attack, guard or follow.
		Transfer		//"(Name) transfer" Transfers complete ownership to targeted player.
	}

	[Flags]
	public enum FoodType
	{
		None				= 0x0000,
		Meat				= 0x0001,
		FruitsAndVeggies	= 0x0002,
		GrainsAndHay		= 0x0004,
		Fish				= 0x0008,
		Eggs				= 0x0010,
		Gold				= 0x0020,
		All					= Meat | FruitsAndVeggies | GrainsAndHay | Fish | Eggs | Gold
	}

	[Flags]
	public enum PackInstinct
	{
		None			= 0x0000,
		Canine			= 0x0001,
		Ostard			= 0x0002,
		Feline			= 0x0004,
		Arachnid		= 0x0008,
		Daemon			= 0x0010,
		Bear			= 0x0020,
		Equine			= 0x0040,
		Bull			= 0x0080
	}

	public enum ScaleType
	{
		Red,
		Yellow,
		Black,
		Green,
		White,
		Blue,
		Blood,
		All,
		Any
	}

	public enum MeatType
	{
		Ribs,
		Bird,
		LambLeg
	}

	public enum HideType
	{
		Regular,
		Spined,
		Horned,
		Barbed
	}

    public enum Alignment
    {
        None,
        Orc,
        Dragon,
        Undead,
        Demon,
        Elemental,
        Giantkin,
        Inhuman
    }

	#endregion

	public class DamageStore : IComparable
	{
		public Mobile m_Mobile;
		public int m_Damage;
		public bool m_HasRight;

		public DamageStore( Mobile m, int damage )
		{
			m_Mobile = m;
			m_Damage = damage;
		}

		public int CompareTo( object obj )
		{
			DamageStore ds = (DamageStore)obj;

			return ds.m_Damage - m_Damage;
		}
	}

	[AttributeUsage( AttributeTargets.Class )]
	public class FriendlyNameAttribute : Attribute
	{
		//future use: Talisman 'Protection/Bonus vs. Specific Creature
		private TextDefinition m_FriendlyName;

		public TextDefinition FriendlyName
		{
			get
			{
				return m_FriendlyName;
			}
		}

		public FriendlyNameAttribute( TextDefinition friendlyName )
		{
			m_FriendlyName = friendlyName;
		}

		public static TextDefinition GetFriendlyNameFor( Type t )
		{
			if( t.IsDefined( typeof( FriendlyNameAttribute ), false ) )
			{
				object[] objs = t.GetCustomAttributes( typeof( FriendlyNameAttribute ), false );

				if( objs != null && objs.Length > 0 )
				{
					FriendlyNameAttribute friendly = objs[0] as FriendlyNameAttribute;

					return friendly.FriendlyName;
				}
			}

			return t.Name;
		}
	}

    // Alan Mod: I created this class in order to deserialize a BaseCreature's that have previously
    // been pseudo-seer possessed (for the purpose of tracking who had control of a mob, especially for events)
    // .. and since accounts are deserialized AFTER mobs and items, we have to wait until the initialize function to
    // make the connections
    public class AccountDeserializeInfo
    {
        public BaseCreature Creature;
        public string Username;

        public AccountDeserializeInfo(BaseCreature creature, string username)
        {
            Creature = creature;
            Username = username;
        }
    }

	public class BaseCreature : Mobile, IBegged
	{
        #region Vita-Nex: Core Compatibility
        /// <summary>
        /// Vita-Nex: Core Compatibility, empty function.
        /// </summary>
        public void SetResistance(ResistanceType type, int value)
        { }

        /// <summary>
        /// Vita-Nex: Core Compatibility, empty function.
        /// </summary>
        public void SetDamageType(ResistanceType type, int value)
        { }
        #endregion

		public const int MaxLoyalty = 100;

        public static void Initialize()
        {
            CommandSystem.Register("PushTheRedButton", AccessLevel.Administrator, new CommandEventHandler(PushTheRedButton));

            // see AccountDeserializeInfo for more info about this
            foreach (AccountDeserializeInfo info in AccountDeserializateInfos)
            {
                if (info.Creature != null)
                {
                    info.Creature.Account = Accounts.GetAccount(info.Username);
                }
            }
        }

        // see AccountDeserializeInfo for more info about this
        public static List<AccountDeserializeInfo> AccountDeserializateInfos = new List<AccountDeserializeInfo>();
        
        /// <summary>
        /// Spawns every constructable (with 0 parameters) BaseCreature in the game on top of you.
        /// </summary>
        /// <param name="e"></param>
        public static void PushTheRedButton(CommandEventArgs e)
        {
            if (e.Mobile == null) return;
            bool print = false;
            if (e.Arguments.Length > 0 && e.Arguments[0].ToLower() == "print")
            {
                print = true;
            }
            Point3D mobLocation = e.Mobile.Location;
            Type type = null;
            Type baseCreatureType = typeof(BaseCreature);
            StringBuilder stringbuilder = new StringBuilder();
            if (print)
            {
                stringbuilder.Append("Type,");
                stringbuilder.Append("DamageMin,");
                stringbuilder.Append("DamageMax,");

                stringbuilder.Append("Hits,");
                stringbuilder.Append("Stam,");
                stringbuilder.Append("bc.Mana,");

                stringbuilder.Append("Str,");
                stringbuilder.Append("Int,");
                stringbuilder.Append("Dex,");

                stringbuilder.Append("BaseSoundID,");

                stringbuilder.Append("Magery,");
                stringbuilder.Append("EvalInt,");
                stringbuilder.Append("MagicResist,");
                stringbuilder.Append("Tactics,");
                stringbuilder.Append("Wrestling,");
                stringbuilder.Append("Archery,");
                stringbuilder.Append("Fencing,");
                stringbuilder.Append("Swords,");
                stringbuilder.Append("Macing,");
                stringbuilder.Append("Anatomy,");
                stringbuilder.Append("Healing,");
                stringbuilder.Append("Necromancy,");
                stringbuilder.Append("Bushido,");
                stringbuilder.Append("Chivalry,");
                stringbuilder.Append("Ninjitsu,");
                stringbuilder.Append("Mining,");
                stringbuilder.Append("Blacksmith,");
                stringbuilder.Append("Carpentry,");
                stringbuilder.Append("Tailoring,");
                stringbuilder.Append("ArmsLore,");
                stringbuilder.Append("ArmsLore\n");
            }
            
            for( int k = 0; type == null && k < ScriptCompiler.Assemblies.Length; ++k ) {
                Type[] types = ScriptCompiler.GetTypeCache( ScriptCompiler.Assemblies[k] ).Types;
                for (int j = 0; j < types.Length; j++)
                { 
                    try
                    {
                        Type typeToCheck = types[j];
                        if (baseCreatureType.IsAssignableFrom(typeToCheck) && typeToCheck != baseCreatureType)
                        {
                            ConstructorInfo[] ctors = typeToCheck.GetConstructors();
                            string[] typewordargs = new string[] { };
                            if (ctors == null) throw new Exception("AHHHH");
                            bool foundZeroLength = false;
                            // go through all the constructors for this type
                            for (int i = 0; i < ctors.Length; ++i)
                            {

                                ConstructorInfo ctor = ctors[i];

                                if (ctor == null) continue;

                                // IsConstructable(ctor) ?? should check?

                                // check the parameter list of the constructor
                                ParameterInfo[] paramList = ctor.GetParameters();

                                // and compare with the argument list provided
                                if (paramList != null)
                                {
                                    // this is a constructor that takes args and matches the number of args passed in to CreateObject
                                    if (paramList.Length > 0)
                                    {
                                        continue;
                                    }

                                    foundZeroLength = true;
                                    // zero argument constructor
                                    BaseCreature bc = (BaseCreature)Activator.CreateInstance(typeToCheck);

                                    // successfully constructed the object, otherwise try another matching constructor
                                    if (bc == null)
                                        throw new Exception("Null base creature???");
                                    bc.MoveToWorld(mobLocation, Map.Felucca);
                                    if (print)
                                    {
                                        stringbuilder.Append(typeToCheck.Name + ",");
                                        stringbuilder.Append(bc.DamageMin + ",");
                                        stringbuilder.Append(bc.DamageMax + ",");

                                        stringbuilder.Append(bc.Hits + ",");
                                        stringbuilder.Append(bc.Stam + ",");
                                        stringbuilder.Append(bc.Mana + ",");

                                        stringbuilder.Append(bc.Str + ",");
                                        stringbuilder.Append(bc.Int + ",");
                                        stringbuilder.Append(bc.Dex + ",");

                                        stringbuilder.Append(bc.BaseSoundID + ",");

                                        stringbuilder.Append(bc.Skills.Magery.Value + ",");
                                        stringbuilder.Append(bc.Skills.EvalInt.Value + ",");
                                        stringbuilder.Append(bc.Skills.MagicResist.Value + ",");
                                        stringbuilder.Append(bc.Skills.Tactics.Value + ",");
                                        stringbuilder.Append(bc.Skills.Wrestling.Value + ",");
                                        stringbuilder.Append(bc.Skills.Archery.Value + ",");
                                        stringbuilder.Append(bc.Skills.Fencing.Value + ",");
                                        stringbuilder.Append(bc.Skills.Swords.Value + ",");
                                        stringbuilder.Append(bc.Skills.Macing.Value + ",");
                                        stringbuilder.Append(bc.Skills.Anatomy.Value + ",");
                                        stringbuilder.Append(bc.Skills.Healing.Value + ",");
                                        stringbuilder.Append(bc.Skills.Necromancy.Value + ",");
                                        stringbuilder.Append(bc.Skills.Bushido.Value + ",");
                                        stringbuilder.Append(bc.Skills.Chivalry.Value + ",");
                                        stringbuilder.Append(bc.Skills.Ninjitsu.Value + ",");
                                        stringbuilder.Append(bc.Skills.Mining.Value + ",");
                                        stringbuilder.Append(bc.Skills.Blacksmith.Value + ",");
                                        stringbuilder.Append(bc.Skills.Carpentry.Value + ",");
                                        stringbuilder.Append(bc.Skills.Tailoring.Value + ",");
                                        stringbuilder.Append(bc.Skills.ArmsLore.Value + ",");
                                        stringbuilder.Append(bc.Skills.ArmsLore.Value + "\n");
                                    }
                                }
                            }
                            if (!foundZeroLength)
                                throw new Exception("No zero length constructor!");
                        }
                    }
                    catch 
                    { 
                    
                    }
                }
            }
            if (print)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter("AllBaseCreatureProps.csv", true))
                    {
                        writer.Write(stringbuilder.ToString());
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Failed to write BaseCreature Properties to file!: " + exception.Message);
                }
            }
        }

		#region Var declarations
		private BaseAI	m_AI;					// THE AI

		private AIType	m_CurrentAI;			// The current AI
		private AIType	m_DefaultAI;			// The default AI

		private Mobile	m_FocusMob;				// Use focus mob instead of combatant, maybe we don't whan to fight
		private FightMode m_FightMode;			// The style the mob uses

		private int		m_iRangePerception;		// The view area
		private int		m_iRangeFight;			// The fight distance

		private bool	m_bDebugAI;				// Show debug AI messages

		private int		m_iTeam;				// Monster Team

		private double	m_dActiveSpeed;			// Timer speed when active
		private double	m_dPassiveSpeed;		// Timer speed when not active
		private double	m_dCurrentSpeed;		// The current speed, lets say it could be changed by something;

		private Point3D m_Home;				// The home position of the creature, used by some AI
		private Map m_HomeMap;				// Used by grim reaper and guards that follow across maps!
		private int		m_iRangeHome = 10;		// The home range of the creature

		List<Type>		m_arSpellAttack;		// List of attack spell/power
		List<Type>		m_arSpellDefense;		// List of defensive spell/power

		private bool		m_bControlled;		// Is controlled
		private Mobile		m_ControlMaster;	// My master
		private Mobile		m_ControlTarget;	// My target mobile
		private Point3D		m_ControlDest;		// My target destination (patrol)
		private OrderType	m_ControlOrder;		// My order

		private int			m_Loyalty;

		private double		m_dMinTameSkill;
		private bool		m_bTamable;

		private bool		m_bSummoned = false;
		private DateTime	m_SummonEnd;
		private int			m_iControlSlots = 1;

		private bool		m_bBardProvoked = false;
		private bool		m_bBardPacified = false;
		private Mobile		m_bBardMaster = null;
		private Mobile		m_bBardTarget = null;
		private DateTime	m_BardEndTime;
        private WayPoint m_CurrentWayPoint = null;
		private IPoint2D	m_TargetLocation = Point2D.Zero;

		private Mobile		m_SummonMaster;

		private int			m_HitsMax = -1;
		private	int			m_StamMax = -1;
		private int			m_ManaMax = -1;
		private int			m_DamageMin = -1;
		private int			m_DamageMax = -1;

		private int			m_PhysicalResistance, m_PhysicalDamage = 100;
		private int			m_FireResistance, m_FireDamage;
		private int			m_ColdResistance, m_ColdDamage;
		private int			m_PoisonResistance, m_PoisonDamage;
		private int			m_EnergyResistance, m_EnergyDamage;
		//private int			m_ChaosDamage;
		//private int			m_DirectDamage;

		private List<Mobile> m_Owners;
		private List<Mobile> m_Friends;

		private bool		m_IsStabled;
		private DateTime	m_StabledDate;

		private bool		m_HasGeneratedLoot; // have we generated our loot yet?

		private bool		m_Paragon;

        private bool        m_Corrupt;

		private bool		m_IsPrisoner;

		private string		m_CorpseNameOverride;

		private ChampionSpawn	m_ChampSpawn;

		private int m_StepsTaken;

        private Dictionary<BaseMetaPet, int> evodragons; 

	    #endregion

		public virtual InhumanSpeech SpeechType{ get{ return null; } }

        private bool m_FreelyLootable = false; 
        [CommandProperty( AccessLevel.GameMaster )]
		public bool FreelyLootable
        {
            get { return m_FreelyLootable; }
            set { m_FreelyLootable = value; }
        }


		[CommandProperty( AccessLevel.GameMaster )]
		public string CorpseNameOverride
		{
			get { return m_CorpseNameOverride; }
			set { m_CorpseNameOverride = value; }
		}

        [Flags]
        public enum ExtSaveFlag : int
        {
           None = 0x00000000,
           KillCriminals = 0x00000001,
           KillMurderers = 0x00000002,
           InnocentDefault = 0x00000004,
           Pseu_KeepKillCredit = 0x00000008,
           CanBreathCustom = 0x00000010,
           BardImmuneCustom  = 0x00000020,
           Pseu_EQPlayerAllowed = 0x00000040,
           Pseu_AllowFizzle = 0x00000080,
           Pseu_AllowInterrupts = 0x00000100,
           Pseu_CanBeHealed = 0x00000200,
           WeaponDamage = 0x00000400,
           TakesNormalDamage = 0x00000800,
           PowerWords = 0x00001000,
           ClearHandsOnCast = 0x00002000,
           Pseu_ConsumeReagents = 0x00004000,
           Pseu_SpellBookRequired = 0x00008000,
           Speaks = 0x00010000,
           FreelyLootable = 0x00020000,
           AutoDispelCustom = 0x00040000,
           AlwaysMurdererCustom = 0x00080000,
           IsScaryToPetsCustom = 0x00100000,
           Pseu_CanUseGates = 0x00200000,
           Pseu_CanUseRecall = 0x00400000,
           Pseu_CanAttackInnocents = 0x00800000,
           Account = 0x01000000,
           ReduceSpeedWithDamage = 0x02000000,
           SkillGainMultiplier = 0x04000000
        }

        private static void SetExtSaveFlag(ref ExtSaveFlag flags, ExtSaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetExtSaveFlag(ExtSaveFlag flags, ExtSaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

		public ChampionSpawn ChampSpawn{ get{ return m_ChampSpawn; } set{ m_ChampSpawn = value; } }

        private int m_LastDamageAmount = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LastDamageAmount { get { return m_LastDamageAmount; } set { m_LastDamageAmount = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int StepsTaken
		{
			get{ return m_StepsTaken; }
			set{ m_StepsTaken = value; }
		}

		public virtual bool CanHaveLlamaTitle{ get{ return true; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public bool IsStabled
		{
			get{ return m_IsStabled; }
			set
			{
				m_IsStabled = value;

				if ( m_IsStabled )
					StopDeleteTimer();
			}
		}

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public DateTime StabledDate
		{
			get{ return m_StabledDate; }
			set{ m_StabledDate = value; }
		}

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsPrisoner
        {
            get{ return m_IsPrisoner; }
            set{ m_IsPrisoner = value; }
        }

		public virtual int BloodHueTemplate{ get{ return 0; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override int BloodHue
		{
			get
			{
				if ( base.BloodHue == -2 )
					return BloodHueTemplate;
				else
					return base.BloodHue;
			}
			set{ base.BloodHue = value; }
		}

		public static Item Identify( Item item )
		{
			if ( item is BaseArmor )
				((BaseArmor)item).Identified = true;
			else if ( item is BaseWeapon )
				((BaseWeapon)item).Identified = true;
			else if ( item is BaseClothing )
				((BaseClothing)item).Identified = true;

			return item;
		}

		public Item Immovable( Item item )
		{
			item.Movable = false;
			return item;
		}

		public static Item IsDyable( Item item )
		{
			item.Dyable = true;
			return item;
		}

		public static Item ChangeLootType( Item item, LootType type )
		{
			item.LootType = type;
			return item;
		}

		public static Item NotCorpseCont( Item item )
		{
			item.SetSavedFlag( 0x01, true );
			return item;
		}

		public static Item Rehued( Item item, int hue )
		{
			item.Hue = hue;
			return item;
		}

		public static Item Renamed( Item item, string name )
		{
			item.Name = name;
			return item;
		}

		public static Item Layered( Item item, Layer layer )
		{
			item.Layer = layer;
			return item;
		}

		public static Item Resourced( BaseWeapon weapon, CraftResource resource )
		{
			weapon.Resource = resource;
			return weapon;
		}

		public static Item Resourced( BaseArmor armor, CraftResource resource )
		{
			armor.Resource = resource;
			return armor;
		}

		protected DateTime SummonEnd
		{
			get { return m_SummonEnd; }
			set { m_SummonEnd = value; }
		}

		public virtual Faction FactionAllegiance{ get{ return null; } }
		public virtual int FactionSilverWorth{ get{ return 10; } }

		#region Begging
		public virtual bool CanBeBegged( Mobile from )
		{
			return false;
		}

		public virtual void OnBegged( Mobile beggar )
		{
		}
		#endregion

		#region Accelerated Healing
		public virtual TimeSpan RegenRate{ get{ return Mobile.DefaultHitsRate; } }
		#endregion

		#region Bonding
		public const bool BondingEnabled = true;

		public virtual bool IsBondable{ get{ return ( BondingEnabled && !Summoned ); } }
		public virtual TimeSpan BondingDelay{ get{ return TimeSpan.FromDays( 7.0 ); } }
        public virtual TimeSpan BondingAbandonDelay { get { return PseudoSeerStone.BondingAbandonDelay; } }
		public virtual TimeSpan AbandonDelay{ get{ return TimeSpan.FromDays( 1.0 ); } }

		public override bool CanRegenHits{ get{ return !m_IsDeadPet && base.CanRegenHits; } }
		public override bool CanRegenStam{ get{ return !m_IsDeadPet && base.CanRegenStam; } }
		public override bool CanRegenMana{ get{ return !m_IsDeadPet && base.CanRegenMana; } }

		public override bool IsDeadBondedPet{ get{ return m_IsDeadPet; } }

		private bool m_IsBonded;
		private bool m_IsDeadPet;
		private DateTime m_BondingBegin;
		private DateTime m_OwnerAbandonTime;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile LastOwner
		{
			get
			{
				if ( m_Owners == null || m_Owners.Count == 0 )
					return null;

				return m_Owners[m_Owners.Count - 1];
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsBonded
		{
			get{ return m_IsBonded; }
			set{
                m_IsBonded = value;
                InvalidateProperties();
            }
		}

		public bool IsDeadPet
		{
			get{ return m_IsDeadPet; }
			set{ m_IsDeadPet = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime BondingBegin
		{
			get{ return m_BondingBegin; }
			set{ m_BondingBegin = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime OwnerAbandonTime
		{
			get{ return m_OwnerAbandonTime; }
			set{ m_OwnerAbandonTime = value; }
		}
		#endregion
		#region Delete Previously Tamed Timer
		private DeleteTimer m_DeleteTimer;

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan DeleteTimeLeft
		{
			get
			{
				if ( m_DeleteTimer != null && m_DeleteTimer.Running )
					return m_DeleteTimer.Next - DateTime.UtcNow;

				return TimeSpan.Zero;
			}
		}

		private class DeleteTimer : Timer
		{
			private Mobile m;

			public DeleteTimer( Mobile creature, TimeSpan delay ) : base( delay )
			{
				m = creature;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m.Delete();
			}
		}

		public void BeginDeleteTimer()
		{
			if ( !(this is BaseEscortable) && !Summoned && !Deleted && !IsStabled )
			{
				StopDeleteTimer();
				m_DeleteTimer = new DeleteTimer( this, TimeSpan.FromDays( 3.0 ) );
				m_DeleteTimer.Start();
			}
		}

		public void StopDeleteTimer()
		{
			if ( m_DeleteTimer != null )
			{
				m_DeleteTimer.Stop();
				m_DeleteTimer = null;
			}
		}
		#endregion

		public virtual double WeaponAbilityChance{ get{ return 0.4; } }

		public virtual WeaponAbility GetWeaponAbility()
		{
			return null;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsParagon
		{
			get{ return m_Paragon; }
			set
			{
				if ( m_Paragon == value )
					return;
				else if ( value )
					Paragon.Convert( this );
				else
					Paragon.UnConvert( this );

				m_Paragon = value;

                InvalidateProperties();
			}
		}

        //Easter 2014/Halloween2014
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsCorrupt
        {
            get { return m_Corrupt; }
            set
            {
                if (m_Corrupt == value)
                    return;
                if (value)
                    HalloweenCorruption.Corrupt(this);
                else
                    HalloweenCorruption.Uncorrupt(this);

                m_Corrupt = value;

                InvalidateProperties();
            }
        }

        public Invasion Invasion { get; set; }

        public ZombieInstanceSerial ZombieSerial { get; set; }

        public Portal Portal { get; set; }

        public bool PortalBoss { get; set; }

		public virtual FoodType FavoriteFood{ get{ return FoodType.None; } }
		public virtual PackInstinct PackInstinct{ get{ return PackInstinct.None; } }

		public List<Mobile> Owners { get { return m_Owners; } }

		public virtual bool AllowMaleTamer{ get{ return true; } }
		public virtual bool AllowFemaleTamer{ get{ return true; } }
		public virtual bool SubdueBeforeTame{ get{ return false; } }
		public virtual bool StatLossAfterTame{ get{ return SubdueBeforeTame; } }
        public virtual bool ReduceSpeedWithDamage { get { return ReduceSpeedWithDamageCustom; } }
		public virtual bool IsSubdued{ get{ return SubdueBeforeTame && ( Hits < ( HitsMax / 10 ) ); } }

		public virtual bool Commandable{ get{ return true; } }

		public virtual Poison HitPoison{ get{ return null; } }
		public virtual double HitPoisonChance{ get{ return 0.5; } }
		public virtual Poison PoisonImmune{ get{ return null; } }

		public virtual bool BardImmune{ get{ return false; } }
        public virtual bool UnprovokableTarget { get { return BardImmune || BardImmuneCustom || m_IsDeadPet; } }
        public virtual bool Unprovokable { get { return BardImmune || BardImmuneCustom || m_IsDeadPet; } }
        public virtual bool Uncalmable { get { return BardImmune || BardImmuneCustom || m_IsDeadPet; } }
        public virtual bool AreaPeaceImmune { get { return BardImmune || BardImmuneCustom || m_IsDeadPet; } }

		public virtual bool BleedImmune{ get{ return false; } }
		public virtual double BonusPetDamageScalar{ get{ return 1.0; } }

		public virtual bool DeathAdderCharmable{ get{ return false; } }

		//TODO: Find the pub 31 tweaks to the DispelDifficulty and apply them of course.
		public virtual double DispelDifficulty{ get{ return 0.0; } } // at this skill level we dispel 50% chance
		public virtual double DispelFocus{ get{ return 20.0; } } // at difficulty - focus we have 0%, at difficulty + focus we have 100%
		public virtual bool DisplayWeight{ get{ return Backpack is StrongBackpack; } }

		#region Breath ability, like dragon fire breath
		private DateTime m_NextBreathTime;

	    public DateTime NextBreathTime
	    {
            set { m_NextBreathTime = value; }
	        get { return m_NextBreathTime; }
	    }

		// Must be overridden in subclass to enable
		public virtual bool HasBreath{ get{ return false; } }

		// Base damage given is: CurrentHitPoints * BreathDamageScalar
		public virtual double BreathDamageScalar{ get{ return EraAOS ? 0.16 : 0.05; } }

		// Min/max seconds until next breath
		public virtual double BreathMinDelay{ get{ return 10.0; } }
		public virtual double BreathMaxDelay{ get{ return 15.0; } }

		// Creature stops moving for 1.0 seconds while breathing
		public virtual double BreathStallTime{ get{ return 1.0; } }

		// Effect is sent 1.3 seconds after BreathAngerSound and BreathAngerAnimation is played
		public virtual double BreathEffectDelay{ get{ return 1.3; } }

		// Damage is given 1.0 seconds after effect is sent
		public virtual double BreathDamageDelay{ get{ return 1.0; } }

		public virtual int BreathRange{ get{ return RangePerception; } }

		// Damage types
		public virtual int BreathChaosDamage { get { return 0; } }
		public virtual int BreathPhysicalDamage{ get{ return 0; } }
		public virtual int BreathFireDamage{ get{ return 100; } }
		public virtual int BreathColdDamage{ get{ return 0; } }
		public virtual int BreathPoisonDamage{ get{ return 0; } }
		public virtual int BreathEnergyDamage{ get{ return 0; } }

		// Is immune to breath damages
		public virtual bool BreathImmune{ get{ return false; } }

		// Effect details and sound
		public virtual int BreathEffectItemID{ get{ return 0x36D4; } }
		public virtual int BreathEffectSpeed{ get{ return 5; } }
		public virtual int BreathEffectDuration{ get{ return 0; } }
		public virtual bool BreathEffectExplodes{ get{ return false; } }
		public virtual bool BreathEffectFixedDir{ get{ return false; } }
		public virtual int BreathEffectHue{ get{ return 0; } }
		public virtual int BreathEffectRenderMode{ get{ return 0; } }

		public virtual int BreathEffectSound{ get{ return 0x227; } }

		// Anger sound/animations
		public virtual int BreathAngerSound{ get{ return GetAngerSound(); } }
		public virtual int BreathAngerAnimation{ get{ return 12; } }

		public virtual void BreathStart( Mobile target )
		{
			BreathStallMovement();
			BreathPlayAngerSound();
			BreathPlayAngerAnimation();

			this.Direction = this.GetDirectionTo( target );

			Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( BreathEffectDelay ), new TimerStateCallback<Mobile>( BreathEffect_Callback ), target );
		}

        public virtual void SetNextBreathTime()
        {
            if (m_BreathCustomDelay < 0)
            {
                m_NextBreathTime = DateTime.UtcNow + TimeSpan.FromSeconds(BreathMinDelay + (Utility.RandomDouble() * BreathMaxDelay));
            }
            else
            {
                m_NextBreathTime = DateTime.UtcNow + TimeSpan.FromSeconds(BreathCustomDelay);
            }
        }

		public virtual void BreathStallMovement()
		{
			if ( m_AI != null )
				m_AI.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds( BreathStallTime );
		}

		public virtual void BreathPlayAngerSound()
		{
			PlaySound( BreathAngerSound );
		}

		public virtual void BreathPlayAngerAnimation()
		{
			Animate( BreathAngerAnimation, 5, 1, true, false, 0 );
		}

		public virtual void BreathEffect_Callback( Mobile target )
		{
			if ( !target.Alive || !CanBeHarmful( target ) )
				return;

			BreathPlayEffectSound();
			BreathPlayEffect( target );

			Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( BreathDamageDelay ), new TimerStateCallback<Mobile>( BreathDamage_Callback ), target );
		}

		public virtual void BreathPlayEffectSound()
		{
			PlaySound( BreathEffectSound );
		}

		public virtual void BreathPlayEffect( Mobile target )
		{
			Effects.SendMovingEffect( this, target, BreathEffectItemID,
				BreathEffectSpeed, BreathEffectDuration, BreathEffectFixedDir,
				BreathEffectExplodes, BreathEffectHue, BreathEffectRenderMode );
		}

		public virtual void BreathDamage_Callback( Mobile target )
		{
			if ( target is BaseCreature && ((BaseCreature)target).BreathImmune )
				return;

			if ( CanBeHarmful( target ) )
			{
				DoHarmful( target );
				BreathDealDamage( target );
			}
		}

		public virtual void BreathDealDamage( Mobile target )
		{
            if (BreathDamageCustom > -1)
                target.Damage(BreathDamageCustom, this);
            else
            {
                int damage = 0;
                if (target is BaseCreature)
                {
                    damage = BreathComputeDamage();
                }
                else
                {
                    damage = BreathComputeDamagePlayer();
                }
				AlterAbilityDamageTo( target, ref damage );

				if ( target is BaseCreature )
					((BaseCreature)target).AlterAbilityDamageFrom( this, ref damage );
	
				target.Damage( damage, this );
			}
		}

		public virtual int BreathComputeDamage()
		{
			int damage = (int)(Hits * BreathDamageScalar);

			if ( IsParagon || IsCorrupt)
				damage = (int)(damage / Paragon.HitsBuff);

			if ( damage > 200 )
				damage = 200;

			return damage;
		}

        public virtual int BreathComputeDamagePlayer()
        {
            int damage = (int)(Hits * BreathDamageScalar);

            if (IsParagon || IsCorrupt)
                damage = (int)(damage / Paragon.HitsBuff);

            if (damage > 35 && Controlled)
                damage = 35;

            return damage;
        }
		#endregion

		#region Spill Acid
		public void SpillAcid( int Amount )
		{
			SpillAcid( null, Amount );
		}

		public void SpillAcid( Mobile target, int Amount )
		{
			if ( (target != null && target.Map == null) || this.Map == null )
				return;

			for ( int i = 0; i < Amount; ++i )
			{
				Point3D loc = this.Location;
				Map map = this.Map;
				Item acid = NewHarmfulItem();

				if ( target != null && target.Map != null && Amount == 1 )
				{
					loc = target.Location;
					map = target.Map;
				} else
				{
					bool validLocation = false;
					for ( int j = 0; !validLocation && j < 10; ++j )
					{
						loc = new Point3D(
							loc.X+(Utility.Random(0,3)-2),
							loc.Y+(Utility.Random(0,3)-2),
							loc.Z );
						loc.Z = map.GetAverageZ( loc.X, loc.Y );
						validLocation = map.CanFit( loc, 16, false, false ) ;
					}
				}
				acid.MoveToWorld( loc, map );
			}
		}

		/*
			Solen Style, override me for other mobiles/items:
			kappa+acidslime, grizzles+whatever, etc.
		*/
		public virtual Item NewHarmfulItem()
		{
			return new PoolOfAcid( TimeSpan.FromSeconds(10), 30, 30 );
		}
		#endregion

		#region Flee!!!
		public virtual bool CanFlee{ get{ return !m_Paragon && ZombieSerial == null && !m_Corrupt; } }

		private DateTime m_EndFlee;

		public DateTime EndFleeTime
		{
			get{ return m_EndFlee; }
			set{ m_EndFlee = value; }
		}

		public virtual void StopFlee()
		{
			m_EndFlee = DateTime.MinValue;
		}

		public virtual bool CheckFlee()
		{
			if ( m_EndFlee == DateTime.MinValue )
				return false;

			if ( DateTime.UtcNow >= m_EndFlee )
			{
				StopFlee();
				return false;
			}

			return true;
		}

		public virtual void BeginFlee( TimeSpan maxDuration )
		{
			m_EndFlee = DateTime.UtcNow + maxDuration;
		}
		#endregion

		public BaseAI AIObject{ get{ return m_AI; } }

		public const int MaxOwners = 5;

		public virtual OppositionGroup OppositionGroup
		{
			get{ return null; }
		}

		#region Friends
		public List<Mobile> Friends { get { return m_Friends; } }

		public virtual bool AllowNewPetFriend
		{
			get{ return ( m_Friends == null || m_Friends.Count < 5 ); }
		}

		public virtual bool IsPetFriend( Mobile m )
		{
			return ( m_Friends != null && m_Friends.Contains( m ) );
		}

		public virtual void AddPetFriend( Mobile m )
		{
			if ( m_Friends == null )
				m_Friends = new List<Mobile>();

			m_Friends.Add( m );
		}

		public virtual void RemovePetFriend( Mobile m )
		{
			if ( m_Friends != null )
				m_Friends.Remove( m );
		}

		public virtual bool IsFriend( Mobile m )
		{
			OppositionGroup g = this.OppositionGroup;

			if ( g != null && g.IsEnemy( this, m ) )
				return false;

			if ( !(m is BaseCreature) )
				return false;

			BaseCreature c = (BaseCreature)m;

			return ( m_iTeam == c.m_iTeam && ( (m_bSummoned || m_bControlled) == (c.m_bSummoned || c.m_bControlled) )/* && c.Combatant != this */);
		}
		#endregion

		#region Allegiance
		public virtual Ethics.Ethic EthicAllegiance { get { return null; } }

		public enum Allegiance
		{
			None,
			Ally,
			Enemy
		}

		public virtual Allegiance GetFactionAllegiance( Mobile mob )
		{
			if ( mob == null || !Faction.IsFactionFacet( mob.Map ) )
				return Allegiance.None;

			Faction allegiance = FactionAllegiance;

			if ( allegiance == null )
				return Allegiance.None;

			Faction fac = Faction.Find( mob, true );

			if ( fac == null )
				return Allegiance.None;

			return ( fac == allegiance ? Allegiance.Ally : Allegiance.Enemy );
		}

		public virtual Allegiance GetEthicAllegiance( Mobile mob )
		{
			if ( mob == null /*|| !Faction.IsFactionFacet( mob.Map )*/ || EthicAllegiance == null )
				return Allegiance.None;

			Ethics.Ethic ethic = Ethics.Ethic.Find( mob, true );

			if ( ethic == null )
				return Allegiance.None;

			return ( ethic == EthicAllegiance ? Allegiance.Ally : Allegiance.Enemy );
		}
		#endregion

		public virtual bool IsEnemy( Mobile m )
		{
			OppositionGroup g = this.OppositionGroup;

			if ( g != null && g.IsEnemy( this, m ) )
				return true;

			if ( m is BaseGuard )
				return false;

			if ( GetFactionAllegiance( m ) == Allegiance.Ally )
				return false;

			//Ethics.Ethic ourEthic = EthicAllegiance;
			//Ethics.Player pl = Ethics.Player.Find( m, true );

			//if ( pl != null && pl.IsShielded && ( ourEthic == null || ourEthic == pl.Ethic ) )
			//	return false;

			if ( !(m is BaseCreature) || m is Server.Engines.Quests.Haven.MilitiaFighter )
				return true;

			BaseCreature c = (BaseCreature)m;

			if ( ( FightMode == FightMode.Evil && m.Karma < 0 ) || ( c.FightMode == FightMode.Evil && Karma < 0 ) )
				return true;

			return ( m_iTeam != c.m_iTeam || ( (m_bSummoned || m_bControlled) != (c.m_bSummoned || c.m_bControlled) ) );
		}

		public override string ApplyNameSuffix( string suffix )
		{
			if ( IsParagon )
			{
				if ( suffix.Length == 0 )
					suffix = "paragon";
				else
					suffix = String.Concat( suffix, " paragon" );
			}

			return base.ApplyNameSuffix( suffix );
		}

		public override bool CheckShove( Mobile shoved )
		{
			if ( shoved == ControlMaster )
				return true;
			else
				return base.CheckShove( shoved );
		}
		
		public virtual bool CheckControlChance( Mobile m, bool affectLoyalty )
		{
			if ( GetControlChance( m ) > Utility.RandomDouble() )
			{
				if (affectLoyalty)
				{
					Loyalty += 1;
				}

				return true;
			}

			PlaySound( GetAngerSound() );

			if ( Body.IsAnimal )
				Animate( 10, 5, 1, true, false, 0 );
			else if ( Body.IsMonster )
				Animate( 18, 5, 1, true, false, 0 );

			if (affectLoyalty && MinTameSkill < 100)
			{
				Loyalty -= 1;
			}

			return false;
		}

		public virtual bool CanBeControlledBy( Mobile m )
		{
			return ( GetControlChance( m ) > 0.0 );
		}

		public double GetControlChance( Mobile m )
		{
			return GetControlChance( m, false );
		}

		public virtual double GetControlChance( Mobile m, bool useBaseSkill )
		{
			if ( m_dMinTameSkill <= 29.1 || m_bSummoned || m.AccessLevel >= AccessLevel.GameMaster )
				return 1.0;

			double dMinTameSkill = m_dMinTameSkill;

			if ( dMinTameSkill > -24.9 && AnimalTaming.CheckMastery( m, this ) )
				dMinTameSkill = -24.9;

			int taming = (int)((useBaseSkill ? m.Skills[SkillName.AnimalTaming].Base : m.Skills[SkillName.AnimalTaming].Value ) * 10);
			int lore = (int)((useBaseSkill ? m.Skills[SkillName.AnimalLore].Base : m.Skills[SkillName.AnimalLore].Value )* 10);
			int bonus = 0, chance = 700;

			if( EraML )
			{
				int SkillBonus = taming - (int)(dMinTameSkill * 10);
				int LoreBonus = lore - (int)(dMinTameSkill * 10);

				int SkillMod = 6, LoreMod = 6;

				if( SkillBonus < 0 )
					SkillMod = 28;
				if( LoreBonus < 0 )
					LoreMod = 14;

				SkillBonus *= SkillMod;
				LoreBonus *= LoreMod;

				bonus = (SkillBonus + LoreBonus ) / 2;
			}
			else
			{
				int difficulty = (int)(dMinTameSkill * 10);
				int weighted = ((taming * 4) + lore) / 5;
				bonus = weighted - difficulty;

				if ( bonus <= 0 )
					bonus *= 14;
				else
					bonus *= 6;
			}

			chance += bonus;

			if ( chance >= 0 && chance < 200 )
				chance = 200;
			else if ( chance > 990 )
				chance = 990;

		    if (dMinTameSkill > 100)
		    {
                chance -= (MaxLoyalty - m_Loyalty) * 5; 		        
		    }
		    else
		    {
                chance -= (MaxLoyalty - m_Loyalty) * 10; 
		    }

            if (m.FindItemOnLayer(Layer.OneHanded).TypeEquals(typeof(WandofLoyalty)))
            {
                var wand = m.FindItemOnLayer(Layer.OneHanded) as WandofLoyalty;
                if (wand != null && wand.Activated)
                {
                    chance += (int)Math.Round((chance * 0.25)/10);
                }
            }

			return ( (double)chance / 1000 );
		}

		private static Type[] m_AnimateDeadTypes = new Type[]
			{
				typeof( MoundOfMaggots ), typeof( HellSteed ), typeof( SkeletalMount ),
				typeof( WailingBanshee ), typeof( Wraith ), typeof( SkeletalDragon ),
				typeof( LichLord ), typeof( FleshGolem ), typeof( Lich ),
				typeof( SkeletalKnight ), typeof( BoneKnight ), typeof( Mummy ),
				typeof( SkeletalMage ), typeof( BoneMage ), typeof( PatchworkSkeleton )
			};

		public virtual bool IsAnimatedDead
		{
			get
			{
				if ( !Summoned )
					return false;

				Type type = this.GetType();

				bool contains = false;

				for ( int i = 0; !contains && i < m_AnimateDeadTypes.Length; ++i )
					contains = ( type == m_AnimateDeadTypes[i] );

				return contains;
			}
		}

		public override void Damage( int amount, Mobile target )
		{
			int oldHits = this.Hits;

			if ( /*Core.AOS &&*/ !this.Summoned && this.Controlled && 0.2 > Utility.RandomDouble() )
				amount = (int)(amount * BonusPetDamageScalar);

			base.Damage( amount, target );

			if ( SubdueBeforeTame && !Controlled )
			{
				if ( (oldHits > (this.HitsMax / 10)) && (this.Hits <= (this.HitsMax / 10)) )
					//PublicOverheadMessage( MessageType.Regular, 0x3B2, 1080057 );
					PublicOverheadMessage( MessageType.Regular, 0x3B2, false, "* The creature has been beaten into subjugation! *" );
			}
		}

		public virtual bool DeleteCorpseOnDeath
		{
			get
			{
				return !EraAOS && m_bSummoned;
			}
		}

		public override void SetLocation( Point3D newLocation, bool isTeleport )
		{
			base.SetLocation( newLocation, isTeleport );

			if ( isTeleport && m_AI != null )
				m_AI.OnTeleported();
		}

		public override void OnBeforeSpawn( Point3D location, Map m )
		{
            if (HalloweenEventController.Instance != null && HalloweenEventController.Halloween && HalloweenCorruption.CheckCorrupt(this, location, m))
                IsCorrupt = true;

			if ( !IsCorrupt && Paragon.CheckConvert( this, location, m ) )
				IsParagon = true;

			base.OnBeforeSpawn( location, m );
		}

		public override ApplyPoisonResult ApplyPoison( Mobile from, Poison poison )
		{
			if ( !Alive || IsDeadPet )
				return ApplyPoisonResult.Immune;

			ApplyPoisonResult result = base.ApplyPoison( from, poison );

			if ( from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer )
				(PoisonTimer as PoisonImpl.PoisonTimer).From = from;

			return result;
		}

		public override bool CheckPoisonImmunity( Mobile from, Poison poison )
		{
			if ( base.CheckPoisonImmunity( from, poison ) )
				return true;

            Poison p = this.PoisonCustomImmune;
            if (p == null) p = this.PoisonImmune;

			if ( IsParagon)
				p = PoisonImpl.IncreaseLevel( p );

			return ( p != null && p.Level >= poison.Level );
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Loyalty
		{
			get
			{
				return m_Loyalty;
			}
			set
			{
				//ADD LOYALTY AND FEED LOGGING
				m_Loyalty = Math.Max(0, Math.Min(MaxLoyalty, value));
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WayPoint CurrentWayPoint
		{
			get
			{
				return m_CurrentWayPoint;
			}
			set
			{
				m_CurrentWayPoint = value;
                if (value != null && m_AI != null && m_AI.m_Timer != null && m_AI.m_Timer.Running == false)
                    m_AI.m_Timer.Start();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public IPoint2D TargetLocation
		{
			get
			{
				return m_TargetLocation;
			}
			set
			{
				m_TargetLocation = value;
			}
		}

		public virtual Mobile ConstantFocus{ get{ return null; } }

		public virtual bool DisallowAllMoves
		{
			get
			{
				return false;
			}
		}

		public virtual bool InitialInnocent
		{
			get
			{
				return false;
			}
		}

		public virtual bool AlwaysMurderer
		{
			get
			{
				return m_AlwaysMurdererCustom;
			}
		}

		public virtual bool AlwaysAttackable
		{
			get
			{
				return false;
			}
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual string SpecialTitle { get; set;}

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int TitleHue { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int DamageMin{ get{ return m_DamageMin; } set{ m_DamageMin = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int DamageMax{ get{ return m_DamageMax; } set{ m_DamageMax = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override int HitsMax
		{
			get
			{
				if ( m_HitsMax > 0 ) {
					int value = m_HitsMax + GetStatOffset( StatType.Str );

					if( value < 1 )
						value = 1;
					else if( value > 65000 )
						value = 65000;

					return value;
				}

				return Str;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitsMaxSeed
		{
			get{ return m_HitsMax; }
			set{ m_HitsMax = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int StamMax
		{
			get
			{
				if ( m_StamMax > 0 ) {
					int value = m_StamMax + GetStatOffset( StatType.Dex );

					if( value < 1 )
						value = 1;
					else if( value > 65000 )
						value = 65000;

					return value;
				}

				return Dex;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StamMaxSeed
		{
			get{ return m_StamMax; }
			set{ m_StamMax = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int ManaMax
		{
			get
			{
				if ( m_ManaMax > 0 ) {
					int value = m_ManaMax + GetStatOffset( StatType.Int );

					if( value < 1 )
						value = 1;
					else if( value > 65000 )
						value = 65000;

					return value;
				}

				return Int;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ManaMaxSeed
		{
			get{ return m_ManaMax; }
			set{ m_ManaMax = value; }
		}

		public virtual bool CanOpenDoors
		{
			get
			{
				return !this.Body.IsAnimal && !this.Body.IsSea;
			}
		}

        public override bool CanPaperdollBeOpenedBy( Mobile from )
		{
			return base.CanPaperdollBeOpenedBy( from )
                || (this == from && this.NetState != null); // pseudoseer addition
		}

		public virtual bool CanMoveOverObstacles
		{
			get
			{
                return true;
                //return Core.AOS || this.Body.IsMonster;
			}
		}

		public virtual bool CanDestroyObstacles
		{
			get
			{
				// to enable breaking of furniture, 'return CanMoveOverObstacles;'
				return true;
			}
		}

		public void Unpacify()
		{
			BardEndTime = DateTime.UtcNow;
			BardPacified = false;
			BardMaster = null;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
            LastDamageAmount = amount;
            if ( BardPacified && ((HitsMax - Hits) * 0.001) > Utility.RandomDouble() )
				Unpacify();

			int disruptThreshold = 0; 
			//NPCs can use bandages too!

			if( amount > disruptThreshold )
			{
				BandageContext c = BandageContext.GetContext( this );

				if( c != null )
					c.Slip();
			}

			WeightOverloading.FatigueOnDamage( this, amount );

			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null && !willKill && Speaks)
				speechType.OnDamage( this, amount );

			if ( willKill && from is PlayerMobile )
				Timer.DelayCall( TimeSpan.FromSeconds( 10 ), ((PlayerMobile)from).RecoverAmmo );

            XmlAttach.CheckOnHit(this, from);
            //UberScriptTriggers.Trigger(this, from, TriggerName.onTakenHit, null, null, null, amount);

			// ALAN MOD: better reward system
			double points = amount;

            if (ChampSpawn != null || this is BaseChampion)
			    AwardScorePoints(from, ref points);

		    if (Invasion != null)
		    {
		        Invasion.AddScore(from, amount);
		    }

            if (Portal != null)
            {
                Portal.AddScore(from, amount);
            }

            if (!Controlled)
            {
                if (from is BaseMetaPet)
                {
                    var score = amount;
                    if (evodragons == null)
                    {
                        evodragons = new Dictionary<BaseMetaPet, int>();
                    }
                    if (!evodragons.ContainsKey(from as BaseMetaPet))
                    {
                        evodragons.Add(from as BaseMetaPet, score);
                    }
                    else
                    {
                        evodragons[from as BaseMetaPet] += score;
                    }
                }
            }

			base.OnDamage( amount, from, willKill );
		}

		#region PlayerScores
		/// <summary>
		///		Overrideable. 
		///		Enable/Disable PlayerScores tracking on a per-creature basis. 
		///		Default: true if ChampSpawn is not null, otherwise false. 
		/// </summary>
		[CommandProperty(AccessLevel.Counselor)]
		public virtual bool UseScores { get { return ChampSpawn != null; } }

		/// <summary>
		///		The root key to use to store player scores. 
		///		Use the champ spawn if there is one, so that scores get pooled together.
		///		If there is no champ spawn set, this instance of the creature will be 
		///		used as the primary key for score-keeping.
		///		Default: ChampSpawn if not null, otherwise use this creature.
		/// </summary>
		[CommandProperty(AccessLevel.Counselor)]
		public virtual IEntity ScoresKey { get { return ChampSpawn ?? (IEntity)this; } }

		public virtual Dictionary<Mobile, double> Scores
		{
			get
			{
				if (!UseScores)
				{
					PlayerScores.RemovePlayerScores(ScoresKey);
					return null;
				}

				return PlayerScores.GetPlayerScores(ScoresKey, !Deleted);
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public virtual double ScoresMin
		{
			get
			{
				if (!UseScores)
				{
					return 0.0;
				}

				var scores = Scores;

				return scores != null && scores.Count > 0 ? scores.Values.Min() : 0.0;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public virtual double ScoresMax
		{
			get
			{
				if (!UseScores)
				{
					return 0.0;
				}

				var scores = Scores;

				return scores != null && scores.Count > 0 ? scores.Values.Max() : 0.0;
			}
		}

		public virtual void AwardScorePoints(Mobile from, ref double points)
		{
			PlayerScores.AwardScorePoints(ScoresKey, from, this is BaseChampion, ref points);
		}
		#endregion

		public virtual void OnDamagedBySpell( Mobile attacker )
		{
		}

		public virtual void OnHarmfulSpell( Mobile from )
		{
		}

		public virtual void OnGaveSpellDamage( Mobile defender )
		{
		}

		#region Alter[...]Damage From/To

		public virtual void AlterDamageScalarFrom( Mobile caster, ref double scalar )
		{
		}

		public virtual void AlterDamageScalarTo( Mobile target, ref double scalar )
		{
		}

		public virtual void AlterSpellDamageFrom( Mobile from, ref int damage )
		{
            if (FactionObelisks.Obelisks != null && from is PlayerMobile)
            {
                var p = from as PlayerMobile;
                var acct = p.Account as Account;
                foreach (var obelisk in FactionObelisks.Obelisks)
                {
                    if (obelisk.ObeliskType == ObeliskType.Power && !String.IsNullOrEmpty(obelisk.OwningFaction) && acct != null)
                    {
                        if (obelisk.ObeliskUsers != null && obelisk.ObeliskUsers.ContainsKey(acct))
                        {
                            from.SendMessage(61, "Damage before: " + damage);
                            damage += (int)Math.Round(damage * 0.05);
                            from.SendMessage(61, "Damage after: " + damage);
                            break;
                        }
                    }
                }
            }
		}

        public virtual void AlterSpellDamageTo(Mobile to, ref int damage)
        {
            // this is where I check for slayers
            if (to.Player)
            {
                int slayerArmorPieces = 0;
                foreach (Item item in to.Items)
                {
                    if (item is BaseArmor)
                    {
                        BaseArmor armor = (BaseArmor)item;
                        if (armor.CheckSlayers(this, to) == CheckSlayerResult.Slayer)
                        {
                            slayerArmorPieces++;
                        }
                    }
                }
                if (slayerArmorPieces > 0)
                {
                    if (Utility.Random(6) < slayerArmorPieces)
                    {
                        damage /= 2;
                    }
                }
            }
        }

		public virtual void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
            if (FactionObelisks.Obelisks != null && from is PlayerMobile)
            {
                var p = from as PlayerMobile;
                var acct = p.Account as Account;
                foreach (var obelisk in FactionObelisks.Obelisks)
                {
                    if (obelisk.ObeliskType == ObeliskType.Power && !String.IsNullOrEmpty(obelisk.OwningFaction) && acct != null)
                    {
                        if (obelisk.ObeliskUsers != null && obelisk.ObeliskUsers.ContainsKey(acct))
                        {
                            damage += (int)Math.Round(damage * 0.05);
                            break;
                        }
                    }
                }
            }
		}

		public virtual void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
		}

		public virtual void AlterAbilityDamageFrom( Mobile from, ref int damage )
		{
		}

		public virtual void AlterAbilityDamageTo( Mobile to, ref int damage )
		{
		}
		#endregion

		public virtual void CheckReflect( Mobile caster, ref bool reflect )
		{
		}

		public virtual void OnCarve( Mobile from, Corpse corpse, Item with )
		{
			int feathers = Feathers;
			int wool = Wool;
			int meat = Meat;
			int hides = Hides;
			int scales = Scales;
            
            // Proxy corpses are created for any mobs that have items equipped when they die
            if (corpse.ProxyCorpse != null)
            {
                corpse.Carved = true;
                corpse = corpse.ProxyCorpse;
            }

			if ( (feathers == 0 && wool == 0 && meat == 0 && hides == 0 && scales == 0) || Summoned || IsBonded || corpse.Animated )
			{
				if ( corpse.Animated )
					corpse.SendLocalizedMessageTo( from, 500464 );	// Use this on corpses to carve away meat and hide
				else
					from.SendLocalizedMessage( 500485 ); // You see nothing useful to carve from the corpse.
			}
			else
			{
				/*
				if( Core.ML && from.Race == Race.Human )
				{
					hides = (int)Math.Ceiling( hides * 1.1 );	//10% Bonus Only applies to Hides, Ore & Logs
				}
				*/
/*
				if ( corpse.Map == Map.Felucca )
				{
					feathers *= 2;
					wool *= 2;
					hides *= 2;

					if (Core.ML)
					{
						meat *= 2;
						scales *= 2;
					}
				}
*/
				if ( corpse.BloodHue != -1 )
					new Blood( 0x122D, corpse.BloodHue ).MoveToWorld( corpse.Location, corpse.Map );

				if ( feathers != 0 )
				{
					corpse.AddCarvedItem( new Feather( feathers ), from );
					from.SendLocalizedMessage( 500479 ); // You pluck the bird. The feathers are now on the corpse.
				}

				if ( wool != 0 )
				{
					corpse.AddCarvedItem( new TaintedWool( wool ), from );
					from.SendLocalizedMessage( 500483 ); // You shear it, and the wool is now on the corpse.
				}

				if ( meat != 0 )
				{
					if ( MeatType == MeatType.Ribs )
						corpse.AddCarvedItem( new RawRibs( meat ), from );
					else if ( MeatType == MeatType.Bird )
						corpse.AddCarvedItem( new RawBird( meat ), from );
					else if ( MeatType == MeatType.LambLeg )
						corpse.AddCarvedItem( new RawLambLeg( meat ), from );

					from.SendLocalizedMessage( 500467 ); // You carve some meat, which remains on the corpse.
				}

				if ( hides != 0 )
				{
					Item holding = from.Weapon as Item;
					if (EraAOS && (holding is SkinningKnife /* TODO: || holding is ButcherWarCleaver || with is ButcherWarCleaver */ ))
					{
						Item leather = null;

						switch ( HideType )
						{
							case HideType.Regular: leather = new Hides( hides ); break;
							case HideType.Spined: leather = new SpinedHides( hides ); break;
							case HideType.Horned: leather = new HornedHides( hides ); break;
							case HideType.Barbed: leather = new BarbedHides( hides ); break;
						}

						if ( leather != null )
						{
							if ( !from.PlaceInBackpack( leather ) )
							{
								corpse.DropItem( leather );
								from.SendLocalizedMessage( 500471 ); // You skin it, and the hides are now in the corpse.
							}
							else
								from.SendLocalizedMessage( 1073555 ); // You skin it and place the cut-up hides in your backpack.
						}
					}
					else
					{
						switch ( HideType )
						{
							case HideType.Regular: corpse.DropItem( new Hides( hides ) ); break;
							case HideType.Spined: corpse.DropItem( new SpinedHides( hides ) ); break;
							case HideType.Horned: corpse.DropItem( new HornedHides( hides ) ); break;
							case HideType.Barbed: corpse.DropItem( new BarbedHides( hides ) ); break;
						}

						from.SendLocalizedMessage( 500471 ); // You skin it, and the hides are now in the corpse.
					}
				}

				if ( scales != 0 )
				{
					ScaleType sc = this.ScaleType;

					switch ( sc )
					{
						case ScaleType.Red:     corpse.AddCarvedItem( new RedScales( scales ), from ); break;
						case ScaleType.Yellow:  corpse.AddCarvedItem( new YellowScales( scales ), from ); break;
						case ScaleType.Black:   corpse.AddCarvedItem( new BlackScales( scales ), from ); break;
						case ScaleType.Green:   corpse.AddCarvedItem( new GreenScales( scales ), from ); break;
						case ScaleType.White:   corpse.AddCarvedItem( new WhiteScales( scales ), from ); break;
						case ScaleType.Blue:    corpse.AddCarvedItem( new BlueScales( scales ), from ); break;
						case ScaleType.Blood:	corpse.AddCarvedItem( new BloodScales( scales ), from ); break;
						case ScaleType.All:
						{
							corpse.AddCarvedItem( new RedScales( scales ), from );
							corpse.AddCarvedItem( new YellowScales( scales ), from );
							corpse.AddCarvedItem( new BlackScales( scales ), from );
							corpse.AddCarvedItem( new GreenScales( scales ), from );
							corpse.AddCarvedItem( new WhiteScales( scales ), from );
							corpse.AddCarvedItem( new BlueScales( scales ), from );
							break;
						}
						case ScaleType.Any:
						{
							switch ( Utility.Random( 5 ) )
							{
								case 0: corpse.AddCarvedItem( new RedScales( scales ), from ); break;
								case 1: corpse.AddCarvedItem( new YellowScales( scales ), from ); break;
								case 2: corpse.AddCarvedItem( new BlackScales( scales ), from ); break;
								case 3: corpse.AddCarvedItem( new GreenScales( scales ), from ); break;
								case 4: corpse.AddCarvedItem( new WhiteScales( scales ), from ); break;
								case 5: corpse.AddCarvedItem( new BlueScales( scales ), from ); break;
							}
							break;
						}
					}

					from.SendMessage( "You cut away some scales, but they remain on the corpse." );
				}

				corpse.Carved = true;


				if ( corpse.IsCriminalAction( from ) )
					from.CriminalAction( true );
			}
		}

		public const int DefaultRangePerception = 12;
		public const int OldRangePerception = 10;

		public BaseCreature(AIType ai,
			FightMode mode,
			int iRangePerception,
			int iRangeFight,
			double dActiveSpeed,
			double dPassiveSpeed)
		{
			if ( iRangePerception == OldRangePerception )
				iRangePerception = DefaultRangePerception;

			if ( iRangePerception < 0 )
				iRangePerception = DefaultRangePerception;

			m_Loyalty = MaxLoyalty; // Wonderfully Happy

			m_CurrentAI = ai;
			m_DefaultAI = ai;

			m_iRangePerception = iRangePerception;
			m_iRangeFight = iRangeFight;

			m_FightMode = mode;

			m_iTeam = 0;

			SpeedInfo.GetSpeeds( this, ref dActiveSpeed, ref dPassiveSpeed );

			m_dActiveSpeed = dActiveSpeed;
			m_dPassiveSpeed = dPassiveSpeed;
			m_dCurrentSpeed = dPassiveSpeed;

			m_bDebugAI = false;

			m_arSpellAttack = new List<Type>();
			m_arSpellDefense = new List<Type>();

			m_bControlled = false;
			m_ControlMaster = null;
			m_ControlTarget = null;
			m_ControlOrder = OrderType.None;

			m_bTamable = false;

			m_Owners = new List<Mobile>();

			m_NextReacquireTime = DateTime.UtcNow + ReacquireDelay;

			ChangeAIType(AI);

			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null )
				speechType.OnConstruct( this );

			SetTitle( String.Empty );

            // Alan mod--make sure every mob has a backpack (for pseudoseer system)
            Container backpack = Backpack;

            if (backpack == null)
            {
                backpack = new Backpack();

                backpack.Movable = false;

                AddItem(backpack);
            }
            // end Alan mod

			GenerateLoot( true );
		}

		public BaseCreature( Serial serial ) : base( serial )
		{
			m_arSpellAttack = new List<Type>();
			m_arSpellDefense = new List<Type>();

			m_bDebugAI = false;
		}

        #region AlanCustomMobProperties
        private bool m_BardImmuneCustom = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool BardImmuneCustom
        {
            get { return m_BardImmuneCustom; }
            set { m_BardImmuneCustom = value; }
        }

        private bool m_Pseu_EQPlayerAllowed = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Pseu_EQPlayerAllowed
        {
            get { return m_Pseu_EQPlayerAllowed; }
            set { m_Pseu_EQPlayerAllowed = value; }
        }

        private bool m_Pseu_SpellBookRequired = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Pseu_SpellBookRequired
        {
            get { return m_Pseu_SpellBookRequired; }
            set { m_Pseu_SpellBookRequired = value; }
        }

        private bool m_Pseu_KeepKillCredit = true; // if true, RegisterKill uses the controlled basecreature as the LastKiller, otherwise it uses the Last controlled pseudoseer playermobile as the LastKiller
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Pseu_KeepKillCredit
        {
            get { return m_Pseu_KeepKillCredit; }
            set { m_Pseu_KeepKillCredit = value; }
        }

        private bool m_Pseu_AllowFizzle = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Pseu_AllowFizzle
        {
            get { return m_Pseu_AllowFizzle; }
            set { m_Pseu_AllowFizzle = value; }
        }

        private bool m_Pseu_AllowInterrupts = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Pseu_AllowInterrupts
        {
            get { return m_Pseu_AllowInterrupts; }
            set { m_Pseu_AllowInterrupts = value; }
        }

        private TimeSpan m_Pseu_SpellDelay = TimeSpan.Zero;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual TimeSpan Pseu_SpellDelay
        {
            get { return m_Pseu_SpellDelay; }
            set { m_Pseu_SpellDelay = value; }
        }

        private bool m_Pseu_CanBeHealed = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Pseu_CanBeHealed
        {
            get { return m_Pseu_CanBeHealed; }
            set { m_Pseu_CanBeHealed = value; }
        }

        private bool m_Pseu_CanUseGates = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Pseu_CanUseGates
        {
            get { return m_Pseu_CanUseGates; }
            set { m_Pseu_CanUseGates = value; }
        }

        private bool m_Pseu_CanUseRecall = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Pseu_CanUseRecall
        {
            get { return m_Pseu_CanUseRecall; }
            set { m_Pseu_CanUseRecall = value; }
        }

        private bool m_Pseu_CanAttackInnocents = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Pseu_CanAttackInnocents
        {
            get { return m_Pseu_CanAttackInnocents; }
            set { m_Pseu_CanAttackInnocents = value; }
        }

        private bool m_Pseu_ConsumeReagents = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Pseu_ConsumeReagents
        {
            get { return m_Pseu_ConsumeReagents; }
            set { m_Pseu_ConsumeReagents = value; }
        }

        private bool m_PowerWords = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool PowerWords
        {
            get { return m_PowerWords; }
            set { m_PowerWords = value; }
        }

        private bool m_ClearHandsOnCast = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool ClearHandsOnCast
        {
            get { return m_ClearHandsOnCast; }
            set { m_ClearHandsOnCast = value; }
        }

        private bool m_WeaponDamage = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool WeaponDamage
        {
            get { return m_WeaponDamage; }
            set { m_WeaponDamage = value; }
        }

        private bool m_TakesNormalDamage = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool TakesNormalDamage
        {
            get { return m_TakesNormalDamage; }
            set { m_TakesNormalDamage = value; }
        }

        private bool m_ReduceSpeedWithDamageCustom = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool ReduceSpeedWithDamageCustom
        {
            get { return m_ReduceSpeedWithDamageCustom; }
            set { m_ReduceSpeedWithDamageCustom = value; }
        }

        private double m_SkillGainMultiplier = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double SkillGainMultiplier
        {
            get { return m_SkillGainMultiplier; }
            set { m_SkillGainMultiplier = value; }
        }

        private bool m_KillCriminals = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool KillCriminals { get { return m_KillCriminals; } set { m_KillCriminals = true; } }

        private bool m_KillMurderers = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool KillMurderers { get { return m_KillMurderers; } set { m_KillMurderers = true; } }

        private bool m_InnocentDefault = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool InnocentDefault
        {
            get { return m_InnocentDefault; }
            set { m_InnocentDefault = value; Delta(MobileDelta.Noto); InvalidateProperties(); }
        }

        private bool m_ForceWaypoint = false; // just for the current waypoint, not for all waypoints
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceWaypoint
        {
            get { return m_ForceWaypoint; }
            set { m_ForceWaypoint = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual string PseudoSeerPermissions
        {
            get
            {
                if (PseudoSeerStone.Instance == null || this.Deleted || this.NetState == null)
                {
                    return null;
                }

                return PseudoSeerStone.Instance.GetPermissionsFor(this.NetState.Account);
            }
            set
            {
                if (PseudoSeerStone.Instance == null || this.Deleted || this.NetState == null)
                {
                    return;
                }
                string oldPermissions = PseudoSeerStone.Instance.CurrentPermissionsClipboard;
                PseudoSeerStone.Instance.CurrentPermissionsClipboard = value;
                if (value == null)
                {
                    PseudoSeerStone.Instance.PseudoSeerRemove = this;
                }
                else
                {
                    PseudoSeerStone.Instance.PseudoSeerAdd = this;
                }
                PseudoSeerStone.Instance.CurrentPermissionsClipboard = oldPermissions;
            }
        }

        private Poison m_PoisonCustomImmune = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public Poison PoisonCustomImmune
        {
            get { return m_PoisonCustomImmune; }
            set { m_PoisonCustomImmune = value; }
        }
        private Poison m_PoisonCustomHit = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public Poison PoisonCustomHit
        {
            get { return m_PoisonCustomHit; }
            set { m_PoisonCustomHit = value; }
        }
        private double m_PoisonCustomChance = -1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double PoisonCustomChance
        {
            get { return m_PoisonCustomChance; }
            set { m_PoisonCustomChance = value; }
        }

        private bool m_AutoDispelCustom = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AutoDispelCustom
        {
            get { return m_AutoDispelCustom; }
            set { m_AutoDispelCustom = value; }
        }

        private double m_AutoDispelChanceCustom = -1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double AutoDispelChanceCustom
        {
            get { return m_AutoDispelChanceCustom; }
            set { m_AutoDispelChanceCustom = value; }
        }

        private bool m_AlwaysMurdererCustom = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AlwaysMurdererCustom
        {
            get { return m_AlwaysMurdererCustom; }
            set { m_AlwaysMurdererCustom = value; }
        }

        private bool m_IsScaryToPetsCustom = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsScaryToPetsCustom
        {
            get { return m_IsScaryToPetsCustom; }
            set { m_IsScaryToPetsCustom = value; }
        }

        private double m_BreathCustomDelay = -1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double BreathCustomDelay { get { return m_BreathCustomDelay; } set { m_BreathCustomDelay = value; } }
        private bool m_CanBreathCustom = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanBreathCustom
        {
            get { return m_CanBreathCustom; }
            set { m_CanBreathCustom = value; }
        }
        private int m_BreathDamageCustom = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BreathDamageCustom
        {
            get { return m_BreathDamageCustom; }
            set { m_BreathDamageCustom = value; }
        }
        protected bool m_Speaks = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Speaks
        {
            get { return m_Speaks; }
            set { m_Speaks = value; }
        }

        #endregion

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 28 ); // version

			writer.Write( (int)m_CurrentAI );
			writer.Write( (int)m_DefaultAI );

			writer.Write( (int)m_iRangePerception );
			writer.Write( (int)m_iRangeFight );

			writer.Write( (int)m_iTeam );

			writer.Write( (double)m_dActiveSpeed );
			writer.Write( (double)m_dPassiveSpeed );
			writer.Write( (double)m_dCurrentSpeed );

			writer.Write( (int) m_Home.X );
			writer.Write( (int) m_Home.Y );
			writer.Write( (int) m_Home.Z );

			// Version 1
			writer.Write( (int) m_iRangeHome );

			int i=0;

			writer.Write( (int) m_arSpellAttack.Count );
			for ( i=0; i< m_arSpellAttack.Count; i++ )
			{
				writer.Write( m_arSpellAttack[i].ToString() );
			}

			writer.Write( (int) m_arSpellDefense.Count );
			for ( i=0; i< m_arSpellDefense.Count; i++ )
			{
				writer.Write( m_arSpellDefense[i].ToString() );
			}

			// Version 2
			writer.Write( (int) m_FightMode );

			writer.Write( (bool) m_bControlled );
			writer.Write( (Mobile) m_ControlMaster );
			writer.Write( (Mobile) m_ControlTarget );
			writer.Write( (Point3D) m_ControlDest );
			writer.Write( (int) m_ControlOrder );
			writer.Write( (double) m_dMinTameSkill );
			// Removed in version 9
			//writer.Write( (double) m_dMaxTameSkill );
			writer.Write( (bool) m_bTamable );
			writer.Write( (bool) m_bSummoned );

			if ( m_bSummoned )
				writer.WriteDeltaTime( m_SummonEnd );

			writer.Write( (int) m_iControlSlots );

			// Version 3
			writer.Write( (int)m_Loyalty );

			// Version 4
			writer.Write( m_CurrentWayPoint );

			// Verison 5
			writer.Write( m_SummonMaster );

			// Version 6
			writer.Write( (int) m_HitsMax );
			writer.Write( (int) m_StamMax );
			writer.Write( (int) m_ManaMax );
			writer.Write( (int) m_DamageMin );
			writer.Write( (int) m_DamageMax );

			// Version 7
			writer.Write( (int) m_PhysicalResistance );
			writer.Write( (int) m_PhysicalDamage );

			writer.Write( (int) m_FireResistance );
			writer.Write( (int) m_FireDamage );

			writer.Write( (int) m_ColdResistance );
			writer.Write( (int) m_ColdDamage );

			writer.Write( (int) m_PoisonResistance );
			writer.Write( (int) m_PoisonDamage );

			writer.Write( (int) m_EnergyResistance );
			writer.Write( (int) m_EnergyDamage );

			// Version 8
			writer.Write( m_Owners, true );

			// Version 10
			writer.Write( (bool) m_IsDeadPet );
			writer.Write( (bool) m_IsBonded );
			writer.Write( (DateTime) m_BondingBegin );
			writer.Write( (DateTime) m_OwnerAbandonTime );

			// Version 11
			writer.Write( (bool) m_HasGeneratedLoot );

			// Version 12
			writer.Write( (bool) m_Paragon );

			// Version 13
			writer.Write( (bool) ( m_Friends != null && m_Friends.Count > 0 ) );

			if ( m_Friends != null && m_Friends.Count > 0 )
				writer.Write( m_Friends, true );

			// Version 14
			writer.Write( (bool)m_RemoveIfUntamed );
			writer.Write( (int)m_RemoveStep );

			// Version 17
			if ( IsStabled || ( Controlled && ControlMaster != null ) )
				writer.Write( TimeSpan.Zero );
			else
				writer.Write( DeleteTimeLeft );

			// Version 18
			writer.WriteDeltaTime( m_StabledDate );

			// Version 19
			writer.Write( (Map)m_HomeMap );

			// Version 20 (RunUO SVN 829, version 18)
			writer.Write( m_CorpseNameOverride );

            // Version 21 (Alan's enabling custom mob tweaks)
            // NOTE: replaced this with ExtSaveFlags!
            /*
            writer.Write((bool)m_KillCriminals);
            writer.Write((bool)m_KillMurderers);
            writer.Write((bool)m_InnocentDefault);
            writer.Write((bool)m_Pseu_KeepKillCredit);
            writer.Write((double)m_PoisonCustomChance);
            if (m_PoisonCustomImmune == null) writer.Write((int)-1); else writer.Write((int)m_PoisonCustomImmune.Level);
            if (m_PoisonCustomHit == null) writer.Write((int)-1); else writer.Write((int)m_PoisonCustomHit.Level);
            writer.Write((bool)m_CanBreathCustom);
            writer.Write((int)m_BreathDamageCustom);
            writer.Write((double)m_BreathCustomDelay);
            writer.Write((TimeSpan)m_Pseu_SpellDelay);
            writer.Write((bool)m_BardImmuneCustom);
            writer.Write((bool)m_Pseu_EQPlayerAllowed);
            writer.Write((bool)m_Pseu_AllowFizzle);
            writer.Write((bool)m_Pseu_AllowInterrupts);
            writer.Write((bool)m_Pseu_CanBeHealed);
            writer.Write((bool)m_WeaponDamage);
            writer.Write((bool)m_TakesNormalDamage);
            writer.Write((bool)m_PowerWords);
            writer.Write((bool)m_ClearHandsOnCast);
            writer.Write((bool)m_Pseu_ConsumeReagents);
            writer.Write((bool)m_Pseu_SpellBookRequired);
            writer.Write((bool)m_Speaks);
            // Version 22
            writer.Write((bool)m_FreelyLootable);
            writer.Write((bool)m_AutoDispelCustom);
            writer.Write((double)m_AutoDispelChanceCustom);
            writer.Write((bool)m_AlwaysMurdererCustom);
            writer.Write((bool)m_IsScaryToPetsCustom);
             */

            // version 23 (Alan's enabling custom mob tweaks)
            ExtSaveFlag flags = ExtSaveFlag.None;
            SetExtSaveFlag(ref flags, ExtSaveFlag.KillCriminals, m_KillCriminals);
            SetExtSaveFlag(ref flags, ExtSaveFlag.KillMurderers, m_KillMurderers);
            SetExtSaveFlag(ref flags, ExtSaveFlag.InnocentDefault, m_InnocentDefault);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Pseu_KeepKillCredit, m_Pseu_KeepKillCredit);
            SetExtSaveFlag(ref flags, ExtSaveFlag.CanBreathCustom, m_CanBreathCustom);
            SetExtSaveFlag(ref flags, ExtSaveFlag.BardImmuneCustom, m_BardImmuneCustom);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Pseu_EQPlayerAllowed, m_Pseu_EQPlayerAllowed);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Pseu_AllowFizzle, !m_Pseu_AllowFizzle);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Pseu_AllowInterrupts, m_Pseu_AllowInterrupts);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Pseu_CanBeHealed, !m_Pseu_CanBeHealed);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Pseu_CanUseGates, !m_Pseu_CanUseGates);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Pseu_CanUseRecall, !m_Pseu_CanUseRecall);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Pseu_CanAttackInnocents, !m_Pseu_CanAttackInnocents);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Account, Account != null);
            SetExtSaveFlag(ref flags, ExtSaveFlag.WeaponDamage, m_WeaponDamage);
            SetExtSaveFlag(ref flags, ExtSaveFlag.TakesNormalDamage, m_TakesNormalDamage);
            SetExtSaveFlag(ref flags, ExtSaveFlag.PowerWords, m_PowerWords);
            SetExtSaveFlag(ref flags, ExtSaveFlag.ClearHandsOnCast, m_ClearHandsOnCast);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Pseu_ConsumeReagents, m_Pseu_ConsumeReagents);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Pseu_SpellBookRequired, m_Pseu_SpellBookRequired);
            SetExtSaveFlag(ref flags, ExtSaveFlag.Speaks, !m_Speaks);
            SetExtSaveFlag(ref flags, ExtSaveFlag.FreelyLootable, m_FreelyLootable);
            SetExtSaveFlag(ref flags, ExtSaveFlag.AutoDispelCustom, m_AutoDispelCustom);
            SetExtSaveFlag(ref flags, ExtSaveFlag.AlwaysMurdererCustom, m_AlwaysMurdererCustom);
            SetExtSaveFlag(ref flags, ExtSaveFlag.IsScaryToPetsCustom, m_IsScaryToPetsCustom);
            SetExtSaveFlag(ref flags, ExtSaveFlag.ReduceSpeedWithDamage, !m_ReduceSpeedWithDamageCustom);
            SetExtSaveFlag(ref flags, ExtSaveFlag.SkillGainMultiplier, m_SkillGainMultiplier != 1.0);
            writer.Write((int)flags);

            writer.Write((double)m_PoisonCustomChance);
            if (m_PoisonCustomImmune == null) writer.Write((int)-1); else writer.Write((int)m_PoisonCustomImmune.Level);
            if (m_PoisonCustomHit == null) writer.Write((int)-1); else writer.Write((int)m_PoisonCustomHit.Level);
            writer.Write((int)m_BreathDamageCustom);
            writer.Write((double)m_BreathCustomDelay);
            writer.Write((TimeSpan)m_Pseu_SpellDelay);
            writer.Write((double)m_AutoDispelChanceCustom);

            // version 24 -- account for previously controlled mobs
            if (Account != null)
                writer.Write((string)Account.Username);
            if (m_SkillGainMultiplier != 1.0)
                writer.Write((double)m_SkillGainMultiplier);

            writer.Write(SpecialTitle);
            writer.Write(TitleHue);

            writer.Write((int)Alignment);

            writer.Write((bool)m_Corrupt);
		}

		private static double[] m_StandardActiveSpeeds = new double[]
			{
				0.175, 0.1, 0.15, 0.2, 0.25, 0.3, 0.4, 0.5, 0.6, 0.8
			};

		private static double[] m_StandardPassiveSpeeds = new double[]
			{
				0.350, 0.2, 0.4, 0.5, 0.6, 0.8, 1.0, 1.2, 1.6, 2.0
			};

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_CurrentAI = (AIType)reader.ReadInt();
			m_DefaultAI = (AIType)reader.ReadInt();

			m_iRangePerception = reader.ReadInt();
			m_iRangeFight = reader.ReadInt();

			m_iTeam = reader.ReadInt();

			m_dActiveSpeed = reader.ReadDouble();
			m_dPassiveSpeed = reader.ReadDouble();
			m_dCurrentSpeed = reader.ReadDouble();

			if ( m_iRangePerception == OldRangePerception )
				m_iRangePerception = DefaultRangePerception;

			m_Home.X = reader.ReadInt();
			m_Home.Y = reader.ReadInt();
			m_Home.Z = reader.ReadInt();
            

			if ( version >= 1 )
			{
				m_iRangeHome = reader.ReadInt();

				int i, iCount;

				iCount = reader.ReadInt();
				for ( i=0; i< iCount; i++ )
				{
					string str = reader.ReadString();
					Type type = Type.GetType( str );

					if ( type != null )
					{
						m_arSpellAttack.Add( type );
					}
				}

				iCount = reader.ReadInt();
				for ( i=0; i< iCount; i++ )
				{
					string str = reader.ReadString();
					Type type = Type.GetType( str );

					if ( type != null )
					{
						m_arSpellDefense.Add( type );
					}
				}
			}
			else
			{
				m_iRangeHome = 0;
			}

			if ( version >= 2 )
			{
				m_FightMode = ( FightMode )reader.ReadInt();

				m_bControlled = reader.ReadBool();
				m_ControlMaster = reader.ReadMobile();
				m_ControlTarget = reader.ReadMobile();
				m_ControlDest = reader.ReadPoint3D();
				m_ControlOrder = (OrderType) reader.ReadInt();

				m_dMinTameSkill = reader.ReadDouble();

				if ( version < 9 )
					reader.ReadDouble();

				m_bTamable = reader.ReadBool();
				m_bSummoned = reader.ReadBool();

				if ( m_bSummoned )
				{
					m_SummonEnd = reader.ReadDeltaTime();
					new UnsummonTimer( m_ControlMaster, this, m_SummonEnd - DateTime.UtcNow ).Start();
				}

				m_iControlSlots = reader.ReadInt();
			}
			else
			{
				m_FightMode = FightMode.Closest;

				m_bControlled = false;
				m_ControlMaster = null;
				m_ControlTarget = null;
				m_ControlOrder = OrderType.None;
			}

			if ( version >= 3 )
				m_Loyalty = reader.ReadInt();
			else
				m_Loyalty = MaxLoyalty; // Wonderfully Happy

			if ( version >= 4 )
				m_CurrentWayPoint = reader.ReadItem() as WayPoint;

			if ( version >= 5 )
				m_SummonMaster = reader.ReadMobile();

			if ( version >= 6 )
			{
				m_HitsMax = reader.ReadInt();
				m_StamMax = reader.ReadInt();
				m_ManaMax = reader.ReadInt();
				m_DamageMin = reader.ReadInt();
				m_DamageMax = reader.ReadInt();
			}

			if ( version >= 7 )
			{
				m_PhysicalResistance = reader.ReadInt();
				m_PhysicalDamage = reader.ReadInt();

				m_FireResistance = reader.ReadInt();
				m_FireDamage = reader.ReadInt();

				m_ColdResistance = reader.ReadInt();
				m_ColdDamage = reader.ReadInt();

				m_PoisonResistance = reader.ReadInt();
				m_PoisonDamage = reader.ReadInt();

				m_EnergyResistance = reader.ReadInt();
				m_EnergyDamage = reader.ReadInt();
			}

			if ( version >= 8 )
				m_Owners = reader.ReadStrongMobileList();
			else
				m_Owners = new List<Mobile>();


			if ( version >= 10 )
			{
				m_IsDeadPet = reader.ReadBool();
				m_IsBonded = reader.ReadBool();
				m_BondingBegin = reader.ReadDateTime();
				m_OwnerAbandonTime = reader.ReadDateTime();
			}

			if ( version >= 11 )
				m_HasGeneratedLoot = reader.ReadBool();
			else
				m_HasGeneratedLoot = true;

			if ( version >= 12 )
			{
				m_Paragon = reader.ReadBool();
				if ( m_Paragon )
					SolidHueOverride = Paragon.Hue;
			}
			else
				m_Paragon = false;

			if ( version >= 13 && reader.ReadBool() )
				m_Friends = reader.ReadStrongMobileList();
			else if ( version < 13 && m_ControlOrder >= OrderType.Unfriend )
				++m_ControlOrder;

			if ( version < 16 && Loyalty != MaxLoyalty )
				Loyalty *= 10;

			double activeSpeed = m_dActiveSpeed;
			double passiveSpeed = m_dPassiveSpeed;

			SpeedInfo.GetSpeeds( this, ref activeSpeed, ref passiveSpeed );

			bool isStandardActive = false;
			for ( int i = 0; !isStandardActive && i < m_StandardActiveSpeeds.Length; ++i )
				isStandardActive = ( m_dActiveSpeed == m_StandardActiveSpeeds[i] );

			bool isStandardPassive = false;
			for ( int i = 0; !isStandardPassive && i < m_StandardPassiveSpeeds.Length; ++i )
				isStandardPassive = ( m_dPassiveSpeed == m_StandardPassiveSpeeds[i] );

			if ( isStandardActive && m_dCurrentSpeed == m_dActiveSpeed )
				m_dCurrentSpeed = activeSpeed;
			else if ( isStandardPassive && m_dCurrentSpeed == m_dPassiveSpeed )
				m_dCurrentSpeed = passiveSpeed;

			if ( isStandardActive && !m_Paragon && !m_Corrupt)
				m_dActiveSpeed = activeSpeed;

            if (isStandardPassive && !m_Paragon && !m_Corrupt)
				m_dPassiveSpeed = passiveSpeed;

			if ( version >= 14 )
			{
				m_RemoveIfUntamed = reader.ReadBool();
				m_RemoveStep = reader.ReadInt();
			}

			TimeSpan deleteTime = TimeSpan.Zero;

			if ( version >= 17 )
				deleteTime = reader.ReadTimeSpan();

			if ( ( deleteTime > TimeSpan.Zero || LastOwner != null ) && !Controlled && !IsStabled )
			{
				if ( deleteTime == TimeSpan.Zero )
					deleteTime = TimeSpan.FromDays( 3.0 );

				m_DeleteTimer = new DeleteTimer( this, deleteTime );
				m_DeleteTimer.Start();
			}

			if ( version >= 18 )
				m_StabledDate = reader.ReadDeltaTime();
/*
			if( version <= 14 && m_Paragon && Hue == 0x31 )
			{
				Hue = Paragon.Hue; //Paragon hue fixed, should now be 0x501.
			}
*/
			if ( version >= 19 )
				m_HomeMap = reader.ReadMap();

			if ( version >= 20 )
				m_CorpseNameOverride = reader.ReadString();

            if (version == 21 || version == 22) // NO LONGER USED!
            {
                m_KillCriminals = reader.ReadBool();
                m_KillMurderers = reader.ReadBool();
                m_InnocentDefault = reader.ReadBool();
                m_Pseu_KeepKillCredit = reader.ReadBool();
                m_PoisonCustomChance = reader.ReadDouble();
                m_PoisonCustomImmune = Poison.GetPoison(reader.ReadInt());
                m_PoisonCustomHit = Poison.GetPoison(reader.ReadInt());
                m_CanBreathCustom = reader.ReadBool();
                m_BreathDamageCustom = reader.ReadInt();
                m_BreathCustomDelay = reader.ReadDouble();
                m_Pseu_SpellDelay = reader.ReadTimeSpan();
                m_BardImmuneCustom = reader.ReadBool();
                m_Pseu_EQPlayerAllowed = reader.ReadBool();
                m_Pseu_AllowFizzle = reader.ReadBool();
                m_Pseu_AllowInterrupts = reader.ReadBool();
                m_Pseu_CanBeHealed = reader.ReadBool();
                m_WeaponDamage = reader.ReadBool();
                m_TakesNormalDamage = reader.ReadBool();
                m_PowerWords = reader.ReadBool();
                m_ClearHandsOnCast = reader.ReadBool();
                m_Pseu_ConsumeReagents = reader.ReadBool();
                m_Pseu_SpellBookRequired = reader.ReadBool();
                m_Speaks = reader.ReadBool();
            }
            if (version == 22) // NO LONGER USED!
            {
                m_FreelyLootable = reader.ReadBool();
                m_AutoDispelCustom = reader.ReadBool();
                m_AutoDispelChanceCustom = reader.ReadDouble();
                m_AlwaysMurdererCustom = reader.ReadBool();
                m_IsScaryToPetsCustom = reader.ReadBool();
            }
            if (version >= 23)
            {
                ExtSaveFlag flags = (ExtSaveFlag)reader.ReadInt();

                if (GetExtSaveFlag(flags, ExtSaveFlag.KillCriminals))
                    m_KillCriminals = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.KillMurderers))
                    m_KillMurderers = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.InnocentDefault))
                    m_InnocentDefault = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Pseu_KeepKillCredit))
                    m_Pseu_KeepKillCredit = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.CanBreathCustom))
                    m_CanBreathCustom = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.BardImmuneCustom))
                    m_BardImmuneCustom = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Pseu_EQPlayerAllowed))
                    m_Pseu_EQPlayerAllowed = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Pseu_AllowFizzle))
                    m_Pseu_AllowFizzle = false;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Pseu_AllowInterrupts))
                    m_Pseu_AllowInterrupts = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Pseu_CanBeHealed))
                    m_Pseu_CanBeHealed = false;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Pseu_CanUseGates))
                    m_Pseu_CanUseGates = false;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Pseu_CanUseRecall))
                    m_Pseu_CanUseRecall = false;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Pseu_CanAttackInnocents))
                    m_Pseu_CanAttackInnocents = false;
                if (GetExtSaveFlag(flags, ExtSaveFlag.ReduceSpeedWithDamage))
                    m_ReduceSpeedWithDamageCustom = false;
                if (GetExtSaveFlag(flags, ExtSaveFlag.WeaponDamage))
                    m_WeaponDamage = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.TakesNormalDamage))
                    m_TakesNormalDamage = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.PowerWords))
                    m_PowerWords = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.ClearHandsOnCast))
                    m_ClearHandsOnCast = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Pseu_ConsumeReagents))
                    m_Pseu_ConsumeReagents = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Pseu_SpellBookRequired))
                    m_Pseu_SpellBookRequired = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.Speaks))
                     m_Speaks = false;
                if (GetExtSaveFlag(flags, ExtSaveFlag.FreelyLootable))
                    m_FreelyLootable = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.AutoDispelCustom))
                    m_AutoDispelCustom = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.AlwaysMurdererCustom))
                    m_AlwaysMurdererCustom = true;
                if (GetExtSaveFlag(flags, ExtSaveFlag.IsScaryToPetsCustom))
                    m_IsScaryToPetsCustom = true;

                m_PoisonCustomChance = reader.ReadDouble();
                m_PoisonCustomImmune = Poison.GetPoison(reader.ReadInt());
                m_PoisonCustomHit = Poison.GetPoison(reader.ReadInt());
                m_BreathDamageCustom = reader.ReadInt();
                m_BreathCustomDelay = reader.ReadDouble();
                m_Pseu_SpellDelay = reader.ReadTimeSpan();
                m_AutoDispelChanceCustom = reader.ReadDouble();

                if (version >= 24)
                {
                    if (GetExtSaveFlag(flags, ExtSaveFlag.Account))
                    {
                        string username = reader.ReadString();
                        AccountDeserializateInfos.Add(new AccountDeserializeInfo(this, username));
                    }
                }

                if (version >= 25)
                {
                    if (GetExtSaveFlag(flags, ExtSaveFlag.SkillGainMultiplier))
                        m_SkillGainMultiplier = reader.ReadDouble();
                }

                if (version >= 26)
                {
                    SpecialTitle = reader.ReadString();
                    TitleHue = reader.ReadInt();
                }

                if (version >= 27)
                {
                    Alignment = (Alignment)reader.ReadInt();
                }

                if (version >= 28)
                {
                    m_Corrupt = reader.ReadBool();
                    if (m_Corrupt)
                        SolidHueOverride = HalloweenCorruption.Hue;
                }
                else
                    m_Corrupt = false;
            }

			CheckStatTimers();

			ChangeAIType(m_CurrentAI);

			AddFollowers();
		}

		public virtual bool IsHumanInTown()
		{
			bool output = Body.IsHuman && Region.IsPartOf( typeof( Regions.GuardedRegion ) );
            if (output == true)
            {
                if (Region is CustomRegion)
                {
                    if (((CustomRegion)Region).Controller != null && ((CustomRegion)Region).Controller.IsGuarded)
                        return true;
                    else return false;
                }
            }
            return output;
		}

		public virtual bool CheckGold( Mobile from, Item dropped )
		{
			if ( dropped is Gold )
				return OnGoldGiven( from, (Gold)dropped );

			return false;
		}

		public virtual bool OnGoldGiven( Mobile from, Gold dropped )
		{
			if ( CheckTeachingMatch( from ) )
			{
				int goldtaken = Teach( m_Teaching, from, dropped.Amount / TeachScalar, true ) * TeachScalar;

				if ( goldtaken > 0 )
				{
					dropped.Consume( goldtaken );
					return dropped == null || dropped.Deleted || dropped.Amount <= 0;
				}
			}
			else if ( IsHumanInTown() )
			{
				Direction = GetDirectionTo( from );

				int oldSpeechHue = this.SpeechHue;

				this.SpeechHue = 0x23F;
				SayTo( from, "Thou art giving me gold?" );

				if ( dropped.Amount >= 400 )
					SayTo( from, "'Tis a noble gift." );
				else
					SayTo( from, "Money is always welcome." );

				from.Karma += Math.Max( dropped.Amount / 10, 100 );

				this.SpeechHue = 0x3B2;
				SayTo( from, 501548 ); // I thank thee.

				this.SpeechHue = oldSpeechHue;

				dropped.Delete();
				return true;
			}

			return false;
		}

		public override bool ShouldCheckStatTimers{ get{ return false; } }

		#region Food
		private static Type[] m_Eggs = new Type[]
			{
				typeof( FriedEggs ), typeof( Eggs ), /*typeof( EasterEggs ),
				typeof( BrightlyColoredEggs )*/
			};

		private static Type[] m_Fish = new Type[]
			{
				typeof( FishSteak ), typeof( RawFishSteak )
			};

		private static Type[] m_GrainsAndHay = new Type[]
			{
				typeof( BreadLoaf ), typeof( FrenchBread ), typeof( SheafOfHay )
			};

		private static Type[] m_Meat = new Type[]
			{
				/* Cooked */
				typeof( Bacon ), typeof( CookedBird ), typeof( Sausage ),
				typeof( Ham ), typeof( Ribs ), typeof( LambLeg ),
				typeof( ChickenLeg ), typeof( RoastPig ),

				/* Uncooked */
				typeof( RawBird ), typeof( RawRibs ), typeof( RawLambLeg ),
				typeof( RawChickenLeg ),

				/* Body Parts */
				typeof( Head ), typeof( LeftArm ), typeof( LeftLeg ),
				typeof( Torso ), typeof( RightArm ), typeof( RightLeg )
			};

		private static Type[] m_FruitsAndVeggies = new Type[]
			{
				typeof( HoneydewMelon ), typeof( YellowGourd ), typeof( GreenGourd ),
				typeof( Banana ), typeof( Bananas ), typeof( Lemon ), typeof( Lime ),
				typeof( Dates ), typeof( Grapes ), typeof( Peach ), typeof( Pear ),
				typeof( Apple ), typeof( Watermelon ), typeof( Squash ),
				typeof( Cantaloupe ), typeof( Carrot ), typeof( Cabbage ),
				typeof( Onion ), typeof( Lettuce ), typeof( Pumpkin )
			};

		private static Type[] m_Gold = new Type[]
			{
				// white wyrms eat gold..
				typeof( Gold )
			};

		public static Type[] FoodTypeFromPreference( FoodType type )
		{
			List<Type> types = new List<Type>();

			if ( ( type & FoodType.FruitsAndVeggies ) != 0 )
				types.AddRange( m_FruitsAndVeggies );

			if ( ( type & FoodType.Meat ) != 0 )
				types.AddRange( m_Meat );

			if ( ( type & FoodType.GrainsAndHay ) != 0 )
				types.AddRange( m_GrainsAndHay );

			if ( ( type & FoodType.Fish ) != 0 )
				types.AddRange( m_Fish );

			if ( ( type & FoodType.Eggs ) != 0 )
				types.AddRange( m_Eggs );

			return types.ToArray();
		}

		public virtual bool CheckFoodPreference( Item f )
		{
			if ( CheckFoodPreference( f, FoodType.Eggs, m_Eggs ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.Fish, m_Fish ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.GrainsAndHay, m_GrainsAndHay ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.Meat, m_Meat ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.FruitsAndVeggies, m_FruitsAndVeggies ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.Gold, m_Gold ) )
				return true;

			return false;
		}

		public virtual bool CheckFoodPreference( Item fed, FoodType type, Type[] types )
		{
			if ( (FavoriteFood & type) == 0 )
				return false;

			Type fedType = fed.GetType();
			bool contains = false;

			for ( int i = 0; !contains && i < types.Length; ++i )
				contains = ( fedType == types[i] );

			return contains;
		}

		public virtual bool CheckFeed( Mobile from, Item dropped )
		{
			if ( !IsDeadPet && Controlled && (ControlMaster == from || IsPetFriend( from )) && (dropped is Food || dropped is Gold || dropped is CookableFood || dropped is Head || dropped is LeftArm || dropped is LeftLeg || dropped is Torso || dropped is RightArm || dropped is RightLeg || dropped is SheafOfHay) )
			{
				Item f = dropped;

				if ( CheckFoodPreference( f ) )
				{
					int amount = f.Amount;

					if ( amount > 0 )
					{
						bool happier = false;

						int stamGain;

						if ( f is Gold )
							stamGain = amount - 50;
						else
							stamGain = (amount * 15) - 50;

						if ( stamGain > 0 )
							Stam += stamGain;

						/*if ( Core.SE )
						{
							if ( m_Loyalty < MaxLoyalty )
							{
								m_Loyalty = MaxLoyalty;
								happier = true;
							}
						}
						else*/
						{
							for ( int i = 0; i < amount; ++i )
							{
								if ( m_Loyalty < MaxLoyalty  && 0.5 >= Utility.RandomDouble() )
								{
									Loyalty += 10;
									happier = true;
								}
							}
						}

						if ( happier )	// looks like in OSI pets say they are happier even if they are at maximum loyalty
							SayTo( from, 502060 ); // Your pet looks happier.

						if ( Body.IsAnimal )
							Animate( 3, 5, 1, true, false, 0 );
						else if ( Body.IsMonster )
							Animate( 17, 5, 1, true, false, 0 );

						if ( IsBondable && !IsBonded )
						{
							Mobile master = m_ControlMaster;

							if ( master != null && master == from )	//So friends can't start the bonding process
							{
								if ( m_dMinTameSkill <= 29.1 || master.Skills[SkillName.AnimalTaming].Base >= m_dMinTameSkill || OverrideBondingReqs() || (EraML && master.Skills[SkillName.AnimalTaming].Value >= m_dMinTameSkill) )
								{
									if ( BondingBegin == DateTime.MinValue )
									{
										BondingBegin = DateTime.UtcNow;
                                        from.SendMessage(0x38, "You can tell that your pet has begun bonding with you. Feed them again after 7 days from now to complete the bonding process.");
									}
                                    else if ((BondingBegin + BondingDelay) <= DateTime.UtcNow)
                                    {
                                        IsBonded = true;
                                        BondingBegin = DateTime.MinValue;
                                        from.SendLocalizedMessage(1049666); // Your pet has bonded with you!
                                    }
                                    else // they are in the middle of the bonding process
                                    {
                                        TimeSpan timeLeft = (BondingBegin + BondingDelay) - DateTime.UtcNow;
                                        from.SendMessage(0x38, "Your pet will bond in " + UberScriptFunctions.Methods.TIMESPANSTRING(null, timeLeft));
                                    }
								}
								else/* if( Core.ML )*/
								{
									from.SendLocalizedMessage( 1075268 ); // Your pet cannot form a bond with you until your animal taming ability has risen.
								}
							}
						}

						dropped.Delete();
						return true;
					}
				}
			}

			return false;
		}

		#endregion

		public virtual bool OverrideBondingReqs()
		{
			return false;
		}

		public virtual bool CanAngerOnTame{ get{ return false; } }

		#region OnAction[...]
		public virtual void OnActionWander()
		{
		}

		public virtual void OnActionCombat()
		{
		}

		public virtual void OnActionGuard()
		{
		}

		public virtual void OnActionFlee()
		{
		}

		public virtual void OnActionInteract()
		{
		}

		public virtual void OnActionBackoff()
		{
		}
		#endregion

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
            // vendors have already handled the uberscript once
            if ((this is BaseVendor) == false)
            {
                // trigger returns true if returnoverride
                if (XmlScript.HasTrigger(this, TriggerName.onDragDrop) && UberScriptTriggers.Trigger(this, from, TriggerName.onDragDrop, dropped))
                    return true;
            }

            if ( CheckFeed( from, dropped ) )
				return true;
			else if ( CheckGold( from, dropped ) )
				return true;

			return base.OnDragDrop( from, dropped );
		}

        // called when dragging item that doesn't belong to anything else
        public override bool CheckLiftTrigger(Item item, ref LRReason reject)
        {
            if ((XmlScript.HasTrigger(this, TriggerName.onDragLift) && UberScriptTriggers.Trigger(this, this, TriggerName.onDragLift, item))
                ||
                (XmlScript.HasTrigger(item, TriggerName.onDragLift) && UberScriptTriggers.Trigger(item, this, TriggerName.onDragLift, item)))
            {
                reject = LRReason.Inspecific;
                return false;
            }
            return true;
        }
        // called when dragging item that belongs to somebody or some container
        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (XmlScript.HasTrigger(from, TriggerName.onDragLift) && UberScriptTriggers.Trigger(from, from, TriggerName.onDragLift, item))
            {
                reject = LRReason.Inspecific;
                return false;
            }
            if (XmlScript.HasTrigger(item, TriggerName.onDragLift) && UberScriptTriggers.Trigger(item, from, TriggerName.onDragLift, item))
            {
                reject = LRReason.Inspecific;
                return false;
            }
            
            return base.CheckLift(from, item, ref reject);
        }

		protected virtual BaseAI ForcedAI { get { return null; } }

		public void ChangeAIType( AIType NewAI )
		{
			if ( m_AI != null )
				m_AI.m_Timer.Stop();

			if( ForcedAI != null )
			{
				m_AI = ForcedAI;
				return;
			}

			m_AI = null;

			switch ( NewAI )
			{
				case AIType.AI_Melee:
					m_AI = new MeleeAI(this);
					break;
				case AIType.AI_Animal:
					m_AI = new AnimalAI(this);
					break;
				case AIType.AI_Berserk:
					m_AI = new BerserkAI(this);
					break;
				case AIType.AI_Archer:
					m_AI = new ArcherAI(this);
					break;
				case AIType.AI_Healer:
					m_AI = new HealerAI(this);
					break;
				case AIType.AI_Vendor:
					m_AI = new VendorAI(this);
					break;
				case AIType.AI_Mage:
					m_AI = new MageAI(this);
					break;
				case AIType.AI_Predator:
					//m_AI = new PredatorAI(this);
					m_AI = new MeleeAI(this);
					break;
				case AIType.AI_Thief:
					m_AI = new ThiefAI(this);
					break;
				case AIType.AI_RangedMelee:
					m_AI = new RangedMeleeAI(this);
					break;
				case AIType.AI_AmazonGuard:
					m_AI = new AmazonGuardAI(this);
					break;
                // custom by Alan
                case AIType.AI_Arcade:
                    m_AI = new ArcadeAI(this);
                    break;
			}
		}

		public virtual void ChangeAIToDefault()
		{
			ChangeAIType(m_DefaultAI);
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AIType AI
		{
			get
			{
				return m_CurrentAI;
			}
			set
			{
				m_CurrentAI = value;

				if (m_CurrentAI == AIType.AI_Use_Default)
				{
					m_CurrentAI = m_DefaultAI;
				}

				ChangeAIType(m_CurrentAI);
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Debug
		{
			get
			{
				return m_bDebugAI;
			}
			set
			{
				m_bDebugAI = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Team
		{
			get
			{
				return m_iTeam;
			}
			set
			{
				m_iTeam = value;

				OnTeamChange();
			}
		}

		public virtual void OnTeamChange()
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile FocusMob
		{
			get
			{
				return m_FocusMob;
			}
			set
			{
				m_FocusMob = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public FightMode FightMode
		{
			get
			{
				return m_FightMode;
			}
			set
			{
				m_FightMode = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RangePerception
		{
			get
			{
				return m_iRangePerception;
			}
			set
			{
				m_iRangePerception = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RangeFight
		{
			get
			{
				return m_iRangeFight;
			}
			set
			{
				m_iRangeFight = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RangeHome
		{
			get
			{
				return m_iRangeHome;
			}
			set
			{
				m_iRangeHome = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double ActiveSpeed
		{
			get
			{
				return m_dActiveSpeed;
			}
			set
			{
				m_dActiveSpeed = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double PassiveSpeed
		{
			get
			{
				return m_dPassiveSpeed;
			}
			set
			{
				m_dPassiveSpeed = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double CurrentSpeed
		{
			get
			{
				return m_dCurrentSpeed;
			}
			set
			{
				if ( m_dCurrentSpeed != value )
				{
					m_dCurrentSpeed = value;

					if (m_AI != null)
						m_AI.OnCurrentSpeedChanged();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Home
		{
			get
			{
				return m_Home;
			}
			set
			{
				m_Home = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Map HomeMap
		{
			get
			{
				return m_HomeMap;
			}
			set
			{
				m_HomeMap = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Controlled
		{
			get
			{
				return m_bControlled;
			}
			set
			{
				if ( m_bControlled == value )
					return;

				m_bControlled = value;
				Delta( MobileDelta.Noto );

				InvalidateProperties();
			}
		}

	    [CommandProperty(AccessLevel.GameMaster)]
	    public Alignment Alignment { get; set; }

	    public override void RevealingAction( bool disruptive )
		{
			Spells.Sixth.InvisibilitySpell.RemoveTimer( this );

			base.RevealingAction( disruptive );
		}

		public void RemoveFollowers()
		{
			if ( m_ControlMaster != null )
			{
				m_ControlMaster.Followers -= ControlSlots;
				if( m_ControlMaster is PlayerMobile )
				{
					((PlayerMobile)m_ControlMaster).AllFollowers.Remove( this );
					if( ((PlayerMobile)m_ControlMaster).AutoStabled.Contains( this ) )
						((PlayerMobile)m_ControlMaster).AutoStabled.Remove( this );
				}
			}
			else if ( m_SummonMaster != null )
			{
				m_SummonMaster.Followers -= ControlSlots;
				if( m_SummonMaster is PlayerMobile )
					((PlayerMobile)m_SummonMaster).AllFollowers.Remove( this );
			}

			if ( m_ControlMaster != null && m_ControlMaster.Followers < 0 )
				m_ControlMaster.Followers = 0;

			if ( m_SummonMaster != null && m_SummonMaster.Followers < 0 )
				m_SummonMaster.Followers = 0;
		}

		public void AddFollowers()
		{
			if ( m_ControlMaster != null )
			{
				m_ControlMaster.Followers += ControlSlots;
				if( m_ControlMaster is PlayerMobile )
				{
					((PlayerMobile)m_ControlMaster).AllFollowers.Add( this );
				}
			}
			else if ( m_SummonMaster != null )
			{
				m_SummonMaster.Followers += ControlSlots;
				if( m_SummonMaster is PlayerMobile )
				{
					((PlayerMobile)m_SummonMaster).AllFollowers.Add( this );
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile ControlMaster
		{
			get
			{
				return m_ControlMaster;
			}
			set
			{
				if ( m_ControlMaster == value  || value == this)
					return;

                /*if (value == null)
                {
                    // we know that control master was previously not null, but now will be (going untamed)
                    try
                    {
	                    LoggingCustom.Log(
		                    "LOG_PetUntame.txt",
							DateTime.Now + "\t" + this + "\t" + GetType() + "\tBonded=" + IsBonded + "\tOwner=" + m_ControlMaster,
							new System.Diagnostics.StackTrace().ToString());
                    }
                    catch { }
                }*/

				RemoveFollowers();
				m_ControlMaster = value;
				AddFollowers();

				if ( m_ControlMaster == null )
				{
					m_ControlOrder = OrderType.None;
					m_bControlled = false;
				}
				else
					StopDeleteTimer();

				Delta( MobileDelta.Noto );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile SummonMaster
		{
			get
			{
				return m_SummonMaster;
			}
			set
			{
				if ( m_SummonMaster == value || this == value )
					return;

				RemoveFollowers();
				m_SummonMaster = value;
				AddFollowers();

				Delta( MobileDelta.Noto );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile ControlTarget
		{
			get
			{
				return m_ControlTarget;
			}
			set
			{
				m_ControlTarget = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D ControlDest
		{
			get
			{
				return m_ControlDest;
			}
			set
			{
				m_ControlDest = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public OrderType ControlOrder
		{
			get
			{
				return m_ControlOrder;
			}
			set
			{
				m_ControlOrder = value;

				if ( m_AI != null )
					m_AI.OnCurrentOrderChanged();

				InvalidateProperties();

				if ( m_ControlMaster != null )
					m_ControlMaster.InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BardProvoked
		{
			get
			{
				return m_bBardProvoked;
			}
			set
			{
				m_bBardProvoked = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BardPacified
		{
			get
			{
				return m_bBardPacified;
			}
			set
			{
				m_bBardPacified = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile BardMaster
		{
			get
			{
				return m_bBardMaster;
			}
			set
			{
				m_bBardMaster = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile BardTarget
		{
			get
			{
				return m_bBardTarget;
			}
			set
			{
				m_bBardTarget = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime BardEndTime
		{
			get
			{
				return m_BardEndTime;
			}
			set
			{
				m_BardEndTime = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double MinTameSkill
		{
			get
			{
				return m_dMinTameSkill;
			}
			set
			{
				m_dMinTameSkill = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Tamable
		{
			get
			{
				return m_bTamable && !m_Paragon && !m_Corrupt;
			}
			set
			{
				m_bTamable = value;
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Summoned
		{
			get
			{
				return m_bSummoned;
			}
			set
			{
				if ( m_bSummoned == value )
					return;

				m_NextReacquireTime = DateTime.UtcNow;

				m_bSummoned = value;
				Delta( MobileDelta.Noto );

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public int ControlSlots
		{
			get
			{
				return m_iControlSlots;
			}
			set
			{
				m_iControlSlots = value;
			}
		}

		public virtual bool NoHouseRestrictions{ get{ return false; } }
		public virtual bool IsHouseSummonable{ get{ return false; } }

		#region Corpse Resources
		public virtual int Feathers{ get{ return 0; } }
		public virtual int Wool{ get{ return 0; } }

		public virtual MeatType MeatType{ get{ return MeatType.Ribs; } }
		public virtual int Meat{ get{ return 0; } }

		public virtual int Hides{ get{ return 0; } }
		public virtual HideType HideType{ get{ return HideType.Regular; } }

		public virtual int Scales{ get{ return 0; } }
		public virtual ScaleType ScaleType{ get{ return ScaleType.Red; } }
		#endregion

        public virtual bool AutoDispel { get { return false; } }
		public virtual double AutoDispelChance{ get { return 1.0; } }

		public virtual bool IsScaryToPets{ get{ return m_IsScaryToPetsCustom; } }
		public virtual bool IsScaredOfScaryThings{ get{ return true; } }

		public virtual bool CanRummageCorpses{ get{ return false; } }

		public virtual void OnGotMeleeAttack( Mobile attacker )
		{
            if ((AutoDispel || AutoDispelCustom) 
                && attacker is BaseCreature && ((BaseCreature)attacker).IsDispellable 
                && (AutoDispelChanceCustom < 0.0 ? AutoDispelChance : AutoDispelChanceCustom) > Utility.RandomDouble())
				    Dispel( attacker );
		}

		public virtual void Dispel( Mobile m )
		{
			Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
			Effects.PlaySound( m, m.Map, 0x201 );

			m.Delete();
		}

		public virtual bool DeleteOnRelease{ get{ return m_bSummoned; } }

		public virtual void OnGaveMeleeAttack( Mobile defender )
		{
            Poison p = PoisonCustomHit;
            if (p == null) p = HitPoison;
            double hitPoisonChance = PoisonCustomChance;
            if (hitPoisonChance == -1.0) { hitPoisonChance = HitPoisonChance; }

			if ( m_Paragon || m_Corrupt)
				p = PoisonImpl.IncreaseLevel( p );

            if (p != null && hitPoisonChance >= Utility.RandomDouble())
			{
				if ( Skills[SkillName.Poisoning].BaseFixedPoint >= 990 && (Skills[SkillName.Poisoning].BaseFixedPoint - 900) > Utility.Random(900) )
					p = PoisonImpl.IncreaseLevel( p );

				defender.ApplyPoison( this, p );

				if ( Controlled )
					CheckSkill( SkillName.Poisoning, 0, Skills[SkillName.Poisoning].Cap );
			}

			if ( AutoDispel && defender is BaseCreature && ((BaseCreature)defender).IsDispellable && AutoDispelChance > Utility.RandomDouble() )
				Dispel( defender );
		}

		public override void OnAfterDelete()
		{
			if ( m_AI != null )
			{
				if ( m_AI.m_Timer != null )
					m_AI.m_Timer.Stop();

				m_AI = null;
			}

			StopDeleteTimer();

            FocusMob = null;

			base.OnAfterDelete();
		}

		public void DebugSay( string text )
		{
			if ( m_bDebugAI )
				this.PublicOverheadMessage( MessageType.Regular, 41, false, text );
		}

		public void DebugSay( string format, params object[] args )
		{
			if ( m_bDebugAI )
				this.PublicOverheadMessage( MessageType.Regular, 41, false, String.Format( format, args ) );
		}

		/*
		 * This function can be overridden.. so a "Strongest" mobile, can have a different definition depending
		 * on who check for value
		 * -Could add a FightMode.Prefered
		 *
		 */
		public virtual double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			if ( ( bPlayerOnly && m.Player ) ||  !bPlayerOnly )
			{
				switch( acqType )
				{
					case FightMode.Strongest :
						return (m.Skills[SkillName.Tactics].Value + m.Str); //returns strongest mobile

					case FightMode.Weakest :
						return -m.Hits; // returns weakest mobile

					default :
						return -GetDistanceToSqrt( m ); // returns closest mobile
				}
			}
			else
			{
				return double.MinValue;
			}
		}

		// Turn, - for left, + for right
		// Basic for now, needs work
		public virtual void Turn(int iTurnSteps)
		{
			int v = (int)Direction;

			Direction = (Direction)((((v & 0x7) + iTurnSteps) & 0x7) | (v & 0x80));
		}

		public virtual void TurnInternal(int iTurnSteps)
		{
			int v = (int)Direction;

			SetDirection( (Direction)((((v & 0x7) + iTurnSteps) & 0x7) | (v & 0x80)) );
		}

		public bool IsHurt()
		{
			return ( Hits != HitsMax );
		}

		public double GetHomeDistance()
		{
			return GetDistanceToSqrt( m_Home );
		}

		public virtual int GetTeamSize(int iRange)
		{
			int iCount = 0;

			foreach ( Mobile m in this.GetMobilesInRange( iRange ) )
			{
				if (m is BaseCreature)
				{
					if ( ((BaseCreature)m).Team == Team )
					{
						if ( !m.Deleted )
						{
							if ( m != this )
							{
								if ( CanSee( m ) )
								{
									iCount++;
								}
							}
						}
					}
				}
			}

			return iCount;
		}

		private class TameEntry : ContextMenuEntry
		{
			private BaseCreature m_Mobile;

			public TameEntry( Mobile from, BaseCreature creature ) : base( 6130, 6 )
			{
				m_Mobile = creature;

				Enabled = Enabled && ( from.Female ? creature.AllowFemaleTamer : creature.AllowMaleTamer );
			}

			public override void OnClick()
			{
				if ( !Owner.From.CheckAlive() )
					return;

				Owner.From.TargetLocked = true;
				SkillHandlers.AnimalTaming.DisableMessage = true;

				if ( Owner.From.UseSkill( SkillName.AnimalTaming ) )
					Owner.From.Target.Invoke( Owner.From, m_Mobile );

				SkillHandlers.AnimalTaming.DisableMessage = false;
				Owner.From.TargetLocked = false;
			}
		}

		#region Teaching
		public virtual bool CanTeach{ get{ return false; } }

		public virtual bool CheckTeach( SkillName skill, Mobile from )
		{
			if ( !CanTeach )
				return false;

			if( skill == SkillName.Stealth && from.Skills[SkillName.Hiding].Base < Stealth.GetHidingRequirement(from) )
				return false;

			if ( skill == SkillName.RemoveTrap && (from.Skills[SkillName.Lockpicking].Base < 50.0 || from.Skills[SkillName.DetectHidden].Base < 50.0) )
				return false;

			if (!EraAOS && (skill == SkillName.Focus || skill == SkillName.Chivalry || skill == SkillName.Necromancy))
				return false;

			return true;
		}

		public enum TeachResult
		{
			Success,
			Failure,
			KnowsMoreThanMe,
			KnowsWhatIKnow,
			SkillNotRaisable,
			NotEnoughFreePoints
		}

		public virtual TeachResult CheckTeachSkills( SkillName skill, Mobile m, int maxPointsToLearn, ref int pointsToLearn, bool doTeach )
		{
			if ( !CheckTeach( skill, m ) || !m.CheckAlive() )
				return TeachResult.Failure;

			Skill ourSkill = Skills[skill];
			Skill theirSkill = m.Skills[skill];

			if ( ourSkill == null || theirSkill == null )
				return TeachResult.Failure;

			int baseToSet = ourSkill.BaseFixedPoint / 2;

			if ( baseToSet > 500 )
				baseToSet = 500;
			else if ( baseToSet < 200 )
				return TeachResult.Failure;

			if ( baseToSet > theirSkill.CapFixedPoint )
				baseToSet = theirSkill.CapFixedPoint;

			pointsToLearn = baseToSet - theirSkill.BaseFixedPoint;

			if ( maxPointsToLearn > 0 && pointsToLearn > maxPointsToLearn )
			{
				pointsToLearn = maxPointsToLearn;
				baseToSet = theirSkill.BaseFixedPoint + pointsToLearn;
			}

			if ( pointsToLearn < 0 )
				return TeachResult.KnowsMoreThanMe;

			if ( pointsToLearn == 0 )
				return TeachResult.KnowsWhatIKnow;

			if ( theirSkill.Lock != SkillLock.Up )
				return TeachResult.SkillNotRaisable;

			int freePoints = m.Skills.Cap - m.Skills.Total;
			int freeablePoints = 0;

			if ( freePoints < 0 )
				freePoints = 0;

			for ( int i = 0; (freePoints + freeablePoints) < pointsToLearn && i < m.Skills.Length; ++i )
			{
				Skill sk = m.Skills[i];

				if ( sk == theirSkill || sk.Lock != SkillLock.Down )
					continue;

				freeablePoints += sk.BaseFixedPoint;
			}

			if ( (freePoints + freeablePoints) == 0 )
				return TeachResult.NotEnoughFreePoints;

			if ( (freePoints + freeablePoints) < pointsToLearn )
			{
				pointsToLearn = freePoints + freeablePoints;
				baseToSet = theirSkill.BaseFixedPoint + pointsToLearn;
			}

			if ( doTeach )
			{
				int need = pointsToLearn - freePoints;

				for ( int i = 0; need > 0 && i < m.Skills.Length; ++i )
				{
					Skill sk = m.Skills[i];

					if ( sk == theirSkill || sk.Lock != SkillLock.Down )
						continue;

					if ( sk.BaseFixedPoint < need )
					{
						need -= sk.BaseFixedPoint;
						sk.BaseFixedPoint = 0;
					}
					else
					{
						sk.BaseFixedPoint -= need;
						need = 0;
					}
				}

				/* Sanity check */
				if ( baseToSet > theirSkill.CapFixedPoint || (m.Skills.Total - theirSkill.BaseFixedPoint + baseToSet) > m.Skills.Cap )
					return TeachResult.NotEnoughFreePoints;

				theirSkill.BaseFixedPoint = baseToSet;
			}

			return TeachResult.Success;
		}

		public virtual bool CheckTeachingMatch( Mobile m )
		{
			if ( m_Teaching == (SkillName)(-1) )
				return false;

			if ( m is PlayerMobile )
				return ( ((PlayerMobile)m).Learning == m_Teaching );

			return true;
		}

		private SkillName m_Teaching = (SkillName)(-1);

		private static readonly int TeachScalar = 1;

		public virtual int Teach( SkillName skill, Mobile m, int maxPointsToLearn, bool doTeach )
		{
			int pointsToLearn = 0;
			TeachResult res = CheckTeachSkills( skill, m, maxPointsToLearn, ref pointsToLearn, doTeach );

			switch ( res )
			{
				case TeachResult.KnowsMoreThanMe:
				{
					Say( 501508 ); // I cannot teach thee, for thou knowest more than I!
					break;
				}
				case TeachResult.KnowsWhatIKnow:
				{
					Say( 501509 ); // I cannot teach thee, for thou knowest all I can teach!
					break;
				}
				case TeachResult.NotEnoughFreePoints:
				case TeachResult.SkillNotRaisable:
				{
					// Make sure this skill is marked to raise. If you are near the skill cap (700 points) you may need to lose some points in another skill first.
					m.SendLocalizedMessage( 501510, "", 0x22 );
					break;
				}
				case TeachResult.Success:
				{
					if ( doTeach )
					{
						Say( 501539 ); // Let me show thee something of how this is done.
						m.SendLocalizedMessage( 501540 ); // Your skill level increases.

						m_Teaching = (SkillName)(-1);

						if ( m is PlayerMobile )
							((PlayerMobile)m).Learning = (SkillName)(-1);

						return pointsToLearn;
					}
					else
					{
						// I will teach thee all I know, if paid the amount in full.  The price is:
						Say( 1019077, AffixType.Append, String.Format( " {0}", pointsToLearn * TeachScalar ), "" );
						Say( 1043108 ); // For less I shall teach thee less.

						m_Teaching = skill;

						if ( m is PlayerMobile )
							((PlayerMobile)m).Learning = skill;

						return 0;
					}
				}
			}

			return 0;
		}
		#endregion

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );

            if (this.NetState != null) { return; }

            if (this.ControlMaster != null)
            {
                if (NotorietyHandlers.CheckAggressor(this.ControlMaster.Aggressors, aggressor))
                {
                    aggressor.Aggressors.Add(AggressorInfo.Create(this, aggressor, true));
                }
            }

			OrderType ct = m_ControlOrder;

			if ( m_AI != null )
			{
				if( !EraML || ( ct != OrderType.Follow && ct != OrderType.Stop && ct != OrderType.Stay ) )
				{
					m_AI.OnAggressiveAction( aggressor );
				}
				else
				{
					DebugSay( "I'm being attacked but my master told me not to fight." );
					Warmode = false;
					return;
				}
			}

			StopFlee();

			ForceReacquire( aggressor, false );
/*
			if ( !IsEnemy( aggressor ) )
			{
				Ethics.Player pl = Ethics.Player.Find( aggressor, true );

				if ( pl != null && pl.IsShielded )
					pl.FinishShield();
			}
			else
*/
			DebugSay( "I'm being attacked by my enemy {0}.", aggressor.Name );

			if ( aggressor.ChangingCombatant && (m_bControlled || m_bSummoned) && (ct == OrderType.Come || ( !EraML && ct == OrderType.Stay ) || ct == OrderType.Stop || ct == OrderType.None || ct == OrderType.Follow) )
			{
				ControlTarget = aggressor;
				ControlOrder = OrderType.Attack;
			}
			else if ( Combatant == null && !m_bBardPacified )
			{
				Warmode = true;
				Combatant = aggressor;
			}
		}

		public override bool OnMoveOver( Mobile m )
		{
            if (XmlScript.HasTrigger(m, TriggerName.onMoveOver) && UberScriptTriggers.Trigger(m, this, TriggerName.onMoveOver))
            {
                return false;
            }
            
            if (m.IgnoreMobiles || this.IgnoreMobiles)
                return true;

            if ( m is BaseCreature && !(((BaseCreature)m).Controlled || (!m.Deleted && m.NetState != null)) )
				return ( !Alive || !m.Alive || IsDeadBondedPet || m.IsDeadBondedPet ) || ( Hidden && AccessLevel > AccessLevel.Player );

			return base.OnMoveOver( m );
		}

		public virtual void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( IsDeadBondedPet && from.AccessLevel >= AccessLevel.GameMaster )
				list.Add( new GMRevivifyEntry( this ) );
		}

		public virtual bool CanDrop { get { return IsBonded; } }

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( m_AI != null && Commandable )
				m_AI.GetContextMenuEntries( from, list );

			if ( m_bTamable && !m_bControlled && from.Alive )
				list.Add( new TameEntry( from, this ) );

			AddCustomContextEntries( from, list );

			if ( CanTeach && from.Alive )
			{
				Skills ourSkills = this.Skills;
				Skills theirSkills = from.Skills;

				for ( int i = 0; i < ourSkills.Length && i < theirSkills.Length; ++i )
				{
					Skill skill = ourSkills[i];
					Skill theirSkill = theirSkills[i];

					if ( skill != null && theirSkill != null && skill.Base >= 60.0 && CheckTeach( skill.SkillName, from ) )
					{
						int toTeach = skill.BaseFixedPoint / 2;

						if ( toTeach > 500 )
							toTeach = 500;

						list.Add( new TeachEntry( (SkillName)i, this, from, ( toTeach > theirSkill.BaseFixedPoint ) ) );
					}
				}
			}
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null && (speechType.Flags & IHSFlags.OnSpeech) != 0 && from.InRange( this, 3 ) )
				return true;

            if (XmlAttach.GetScripts(this) != null) return true;

			return ( m_AI != null && m_AI.HandlesOnSpeech( from ) && from.InRange( this, m_iRangePerception ) );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
            if (XmlScript.HasTrigger(this, TriggerName.onSpeech) && UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
            {
                e.Handled = true;
                return;
            }

            InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null && speechType.OnSpeech( this, e.Mobile, e.Speech ) )
				e.Handled = true;
			else if ( !e.Handled && m_AI != null && e.Mobile.InRange( this, m_iRangePerception ) )
				m_AI.OnSpeech( e );
		}

		public override bool IsHarmfulCriminal( Mobile target )
		{
			if ( (Controlled && target == m_ControlMaster) || (Summoned && target == m_SummonMaster) )
				return false;

			if ( target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled )
				return false;

			if ( target is PlayerMobile && ((PlayerMobile)target).PermaFlags.Count > 0 )
				return false;

			return base.IsHarmfulCriminal( target );
		}

		public override void CriminalAction( bool message )
		{
			base.CriminalAction( message );

			if ( Controlled || Summoned )
			{
				if ( m_ControlMaster != null && m_ControlMaster.Player )
					m_ControlMaster.CriminalAction( false );
				else if ( m_SummonMaster != null && m_SummonMaster.Player )
					m_SummonMaster.CriminalAction( false );
			}
		}

		public override void DoHarmful( Mobile target, bool indirect )
		{
			base.DoHarmful( target, indirect );

			if ( target == this || target == m_ControlMaster || target == m_SummonMaster || (!Controlled && !Summoned) )
				return;

            if (target == null || Deleted)
                return;

			List<AggressorInfo> list = this.Aggressors;

		    if (list == null)
		        return;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo ai = list[i];

				if ( ai != null && ai.Attacker == target )
					return;
			}

			list = this.Aggressed;

            if (list == null)
                return;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo ai = list[i];

				if ( ai != null && ai.Defender == target )
				{
					if ( m_ControlMaster != null && m_ControlMaster.Player && m_ControlMaster.CanBeHarmful( target, false ) )
						m_ControlMaster.DoHarmful( target, true );
					else if ( m_SummonMaster != null && m_SummonMaster.Player && m_SummonMaster.CanBeHarmful( target, false ) )
						m_SummonMaster.DoHarmful( target, true );

					return;
				}
			}
		}

		private static Mobile m_NoDupeGuards;

		public void ReleaseGuardDupeLock()
		{
			m_NoDupeGuards = null;
		}

		public void ReleaseGuardLock()
		{
			EndAction( typeof( GuardedRegion ) );
		}

		private DateTime m_IdleReleaseTime;

		public virtual bool CheckIdle()
		{
			if ( Combatant != null )
				return false; // in combat.. not idling

			if ( m_IdleReleaseTime > DateTime.MinValue )
			{
				// idling...

				if ( DateTime.UtcNow >= m_IdleReleaseTime )
				{
					m_IdleReleaseTime = DateTime.MinValue;
					return false; // idle is over
				}

				return true; // still idling
			}

			if ( 95 > Utility.Random( 100 ) )
				return false; // not idling, but don't want to enter idle state

			m_IdleReleaseTime = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.RandomMinMax( 15, 25 ) );

			if ( Body.IsHuman )
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: Animate( 5, 5, 1, true,  true, 1 ); break;
					case 1: Animate( 6, 5, 1, true, false, 1 ); break;
				}
			}
			else if ( Body.IsAnimal )
			{
				switch ( Utility.Random( 3 ) )
				{
					case 0: Animate(  3, 3, 1, true, false, 1 ); break;
					case 1: Animate(  9, 5, 1, true, false, 1 ); break;
					case 2: Animate( 10, 5, 1, true, false, 1 ); break;
				}
			}
			else if ( Body.IsMonster )
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: Animate( 17, 5, 1, true, false, 1 ); break;
					case 1: Animate( 18, 5, 1, true, false, 1 ); break;
				}
			}

			PlaySound( GetIdleSound() );
			return true; // entered idle state
		}

		private void CheckAIActive()
		{
			Map map = Map;

			if ( PlayerRangeSensitive && m_AI != null && map != null && map.GetSector( Location ).Active )
				m_AI.Activate();
		}

		protected override void OnMapChange( Map oldMap )
		{
			CheckAIActive();

			base.OnMapChange( oldMap );
		}

		protected override void OnLocationChange( Point3D oldLocation )
		{
			CheckAIActive();

			base.OnLocationChange( oldLocation );
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			base.OnMovement( m, oldLocation );

			if ( (ReacquireOnMovement || m_Paragon || m_Corrupt) && m.Location != oldLocation )
				ForceReacquire( m, true );

			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null && Speaks)
				speechType.OnMovement( this, m, oldLocation );

			/* Begin notice sound */
			if ( (!m.Hidden || m.AccessLevel == AccessLevel.Player) && m.Player && m_FightMode != FightMode.Aggressor && m_FightMode != FightMode.None && Combatant == null && !Controlled && !Summoned )
			{
				// If this creature defends itself but doesn't actively attack (animal) or
				// doesn't fight at all (vendor) then no notice sounds are played..
				// So, players are only notified of aggressive monsters

				// Monsters that are currently fighting are ignored

				// Controlled or summoned creatures are ignored

				if ( InRange( m.Location, 18 ) && !InRange( oldLocation, 18 ) )
				{
					if ( Body.IsMonster )
						Animate( 11, 5, 1, true, false, 1 );

					PlaySound( GetAngerSound() );
				}
			}
			/* End notice sound */

			if ( m_NoDupeGuards == m )
				return;

			if ( !Body.IsHuman || Kills >= Mobile.MurderCount || AlwaysMurderer || AlwaysAttackable || m.Kills < Mobile.MurderCount || !m.InRange( Location, 12 ) || !m.Alive )
				return;

			GuardedRegion guardedRegion = (GuardedRegion) this.Region.GetRegion( typeof( GuardedRegion ) );

			if ( guardedRegion != null )
			{
				if ( !guardedRegion.IsDisabled() && guardedRegion.IsGuardCandidate( m ) && guardedRegion.ContainsCandidateKey( m ) && BeginAction( typeof( GuardedRegion ) ) )
				{
					Say( 1013037 + Utility.Random( 16 ) );
					guardedRegion.CallGuards( this.Location );

					Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( ReleaseGuardLock ) );

					m_NoDupeGuards = m;
					Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ReleaseGuardDupeLock ) );
				}
			}
		}


		public void AddSpellAttack( Type type )
		{
			m_arSpellAttack.Add ( type );
		}

		public void AddSpellDefense( Type type )
		{
			m_arSpellDefense.Add ( type );
		}

		public Spell GetAttackSpellRandom()
		{
			if ( m_arSpellAttack.Count > 0 )
			{
				Type type = m_arSpellAttack[Utility.Random(m_arSpellAttack.Count)];

				object[] args = {this, null};
				return Activator.CreateInstance( type, args ) as Spell;
			}
			else
			{
				return null;
			}
		}

		public Spell GetDefenseSpellRandom()
		{
			if ( m_arSpellDefense.Count > 0 )
			{
				Type type = m_arSpellDefense[Utility.Random(m_arSpellDefense.Count)];

				object[] args = {this, null};
				return Activator.CreateInstance( type, args ) as Spell;
			}
			else
			{
				return null;
			}
		}

		public Spell GetSpellSpecific( Type type )
		{
			int i;

			for( i=0; i< m_arSpellAttack.Count; i++ )
			{
				if( m_arSpellAttack[i] == type )
				{
					object[] args = { this, null };
					return Activator.CreateInstance( type, args ) as Spell;
				}
			}

			for ( i=0; i< m_arSpellDefense.Count; i++ )
			{
				if ( m_arSpellDefense[i] == type )
				{
					object[] args = {this, null};
					return Activator.CreateInstance( type, args ) as Spell;
				}
			}

			return null;
		}

		#region Set[...]

		public void SetDamage( int val )
		{
			m_DamageMin = val;
			m_DamageMax = val;
		}

		public void SetDamage( int min, int max )
		{
			m_DamageMin = min;
			m_DamageMax = max;
		}

		public void SetHits( int val )
		{
			if (val < 1000 && !EraAOS)
				val = (val * 100) / 60;

			m_HitsMax = val;
			Hits = HitsMax;
		}

		public void SetHits( int min, int max )
		{
			if ( min < 1000 /*&& !Core.AOS*/ )
			{
				min = (min * 100) / 60;
				max = (max * 100) / 60;
			}

			m_HitsMax = Utility.RandomMinMax( min, max );
			Hits = HitsMax;
		}

		public void SetStam( int val )
		{
			m_StamMax = val;
			Stam = StamMax;
		}

		public void SetStam( int min, int max )
		{
			m_StamMax = Utility.RandomMinMax( min, max );
			Stam = StamMax;
		}

		public void SetMana( int val )
		{
			m_ManaMax = val;
			Mana = ManaMax;
		}

		public void SetMana( int min, int max )
		{
			m_ManaMax = Utility.RandomMinMax( min, max );
			Mana = ManaMax;
		}

		public void SetStr( int val )
		{
			RawStr = val;
			Hits = HitsMax;
		}

		public void SetStr( int min, int max )
		{
			RawStr = Utility.RandomMinMax( min, max );
			Hits = HitsMax;
		}

		public void SetDex( int val )
		{
			RawDex = val;
			Stam = StamMax;
		}

		public void SetDex( int min, int max )
		{
			RawDex = Utility.RandomMinMax( min, max );
			Stam = StamMax;
		}

		public void SetInt( int val )
		{
			RawInt = val;
			Mana = ManaMax;
		}

		public void SetInt( int min, int max )
		{
			RawInt = Utility.RandomMinMax( min, max );
			Mana = ManaMax;
		}

		public void SetSkill( SkillName name, double val )
		{
			Skills[name].BaseFixedPoint = (int)(val * 10);

			if ( Skills[name].Base > Skills[name].Cap )
			{
				if ( EraSE )
					this.SkillsCap += ( Skills[name].BaseFixedPoint - Skills[name].CapFixedPoint );

				Skills[name].Cap = Skills[name].Base;
			}
		}

		public void SetSkill( SkillName name, double min, double max )
		{
			int minFixed = (int)(min * 10);
			int maxFixed = (int)(max * 10);

			Skills[name].BaseFixedPoint = Utility.RandomMinMax( minFixed, maxFixed );

			if ( Skills[name].Base > Skills[name].Cap )
			{
				if ( EraSE )
					this.SkillsCap += ( Skills[name].BaseFixedPoint - Skills[name].CapFixedPoint );

				Skills[name].Cap = Skills[name].Base;
			}
		}

		public void SetTitle( string title )
		{
			if ( CanHaveLlamaTitle && Utility.Random( 5000 ) == 0 )
				Title = "the mystic llamaherder";
			else
				Title = title;
		}

		public void SetFameLevel( int level )
		{
			switch ( level )
			{
				case 1: Fame = Utility.RandomMinMax(     0,  1249 ); break;
				case 2: Fame = Utility.RandomMinMax(  1250,  2499 ); break;
				case 3: Fame = Utility.RandomMinMax(  2500,  4999 ); break;
				case 4: Fame = Utility.RandomMinMax(  5000,  9999 ); break;
				case 5: Fame = Utility.RandomMinMax( 10000, 10000 ); break;
			}
		}

		public void SetKarmaLevel( int level )
		{
			switch ( level )
			{
				case 0: Karma = -Utility.RandomMinMax(     0,   624 ); break;
				case 1: Karma = -Utility.RandomMinMax(   625,  1249 ); break;
				case 2: Karma = -Utility.RandomMinMax(  1250,  2499 ); break;
				case 3: Karma = -Utility.RandomMinMax(  2500,  4999 ); break;
				case 4: Karma = -Utility.RandomMinMax(  5000,  9999 ); break;
				case 5: Karma = -Utility.RandomMinMax( 10000, 10000 ); break;
			}
		}

		#endregion

		public static void Cap( ref int val, int min, int max )
		{
			if ( val < min )
				val = min;
			else if ( val > max )
				val = max;
		}

		#region Pack & Loot

		public void PackPotion()
		{
			PackItem( Loot.RandomPotion() );
		}

		public void PackScroll( int minCircle, int maxCircle )
		{
			PackScroll( Utility.RandomMinMax( minCircle, maxCircle ) );
		}

		public void PackScroll( int circle )
		{
			int min = (circle - 1) * 8;

			PackItem( Loot.RandomScroll( min, min + 7, SpellbookType.Regular ) );
		}

		public void PackMagicItems( int minLevel, int maxLevel )
		{
			PackMagicItems( minLevel, maxLevel, 0.30, 0.15 );
		}

		public void PackMagicItems( int minLevel, int maxLevel, double armorChance, double weaponChance )
		{
			if ( !PackArmor( minLevel, maxLevel, armorChance ) )
				PackWeapon( minLevel, maxLevel, weaponChance );
		}

		public virtual void DropBackpack()
		{
			if ( Backpack != null )
			{
				if ( Backpack.Items.Count > 0 )
				{
					Backpack b = new CreatureBackpack( Name );

					List<Item> list = new List<Item>( Backpack.Items );
					foreach ( Item item in list )
						b.DropItem( item );

					BaseHouse house = BaseHouse.FindHouseAt( this );
					if ( house  != null )
						b.MoveToWorld( house.BanLocation, house.Map );
					else
						b.MoveToWorld( Location, Map );
				}
			}
		}

		protected bool m_Spawning;

		public virtual void GenerateLoot( bool spawning )
		{
			m_Spawning = spawning;

			LootCollection coll = LootSystem.GetCollection( this.GetType() );

			if ( coll == null )
			{
				coll = GenerateLootCollection();
				LootSystem.Register( this.GetType(), coll );
			}

            if (spawning && FavoriteFood != FoodType.None && 0.25 > Utility.RandomDouble() )
			{
				int total = Utility.Random( 1, Math.Min( Str / 10, 10 ) );
				for ( int i = 0;i < total; i++ )
					PackItem( Loot.Construct( FoodTypeFromPreference( FavoriteFood ) ) );
			}

			GenerateLoot();

			m_Spawning = false;
		}

		public virtual void GenerateLoot()
		{
		}

		public virtual LootCollection GenerateLootCollection()
		{
			return new LootCollection( "1d1+0" );
		}

		public virtual void AddLoot( LootPack pack, int amount )
		{
			if ( Summoned )
				return;

			Container backpack = Backpack;

			if ( backpack == null )
			{
				backpack = new Backpack();

				backpack.Movable = false;

				AddItem( backpack );
			}

			for ( int i = 0; i < amount; ++i )
				AddLoot( pack, backpack );
		}

		public virtual void AddLoot( LootPack pack )
		{
			AddLoot( pack, 1 );
		}

		public virtual void AddLoot( LootPack pack, Container cont )
		{
			if ( Summoned )
				return;

			pack.Generate( this, cont, m_Spawning );
		}

		public virtual void AddPackedLoot( LootPack pack, Type type )
		{
			AddPackedLoot( pack, type, 1 );
		}

        public virtual void ForceAddLoot(LootPack pack, int amount)
        {
            if (Summoned)
                return;

            Container backpack = Backpack;

            if (backpack == null)
            {
                backpack = new Backpack();

                backpack.Movable = false;

                AddItem(backpack);
            }

            for (int i = 0; i < amount; ++i)
                ForceAddLoot(pack, backpack);
        }

        public virtual void ForceAddLoot(LootPack pack, Container cont)
        {
            if (Summoned)
                return;

            pack.ForceGenerate(this, cont);
        }

		public virtual void AddPackedLoot( LootPack pack, Type type, int amount )
		{
			if ( type.IsSubclassOf( typeof( Container ) ) )
			{
				try
				{
					Container backpack = Backpack;

					if ( backpack == null )
					{
						backpack = new Backpack();
						backpack.Movable = false;
						AddItem( backpack );
					}

					Container cnt = (Container)Activator.CreateInstance( type );
					if ( cnt != null )
						for ( int i = 0; i < amount; ++i )
							AddLoot( pack, cnt );

					if ( cnt.Items.Count == 0 )
						cnt.Delete();
					else
						backpack.DropItem( cnt );
				}
				catch
				{
				}
			}
		}

		public bool PackArmor( int minLevel, int maxLevel )
		{
			return PackArmor( minLevel, maxLevel, 1.0 );
		}

		public bool PackArmor( int minLevel, int maxLevel, double chance )
		{
			if ( chance <= Utility.RandomDouble() )
				return false;

			Cap( ref minLevel, 0, 5 );
			Cap( ref maxLevel, 0, 5 );

			BaseArmor armor = Loot.RandomArmorOrShield();

			if ( armor == null )
				return false;

			armor.ProtectionLevel = (ArmorProtectionLevel)RandomMinMaxScaled( minLevel, maxLevel );
			armor.Durability = (ArmorDurabilityLevel)RandomMinMaxScaled( minLevel, maxLevel );

			PackItem( armor );

			return true;
		}

		public static int RandomMinMaxScaled( int min, int max )
		{
			if ( min == max )
				return min;

			if ( min > max )
			{
				int hold = min;
				min = max;
				max = hold;
			}

			/* Example:
			 *    min: 1
			 *    max: 5
			 *  count: 5
			 *
			 * total = (5*5) + (4*4) + (3*3) + (2*2) + (1*1) = 25 + 16 + 9 + 4 + 1 = 55
			 *
			 * chance for min+0 : 25/55 : 45.45%
			 * chance for min+1 : 16/55 : 29.09%
			 * chance for min+2 :  9/55 : 16.36%
			 * chance for min+3 :  4/55 :  7.27%
			 * chance for min+4 :  1/55 :  1.81%
			 */

			int count = max - min + 1;
			int total = 0, toAdd = count;

			for ( int i = 0; i < count; ++i, --toAdd )
				total += toAdd*toAdd;

			int rand = Utility.Random( total );
			toAdd = count;

			int val = min;

			for ( int i = 0; i < count; ++i, --toAdd, ++val )
			{
				rand -= toAdd*toAdd;

				if ( rand < 0 )
					break;
			}

			return val;
		}

		public bool PackSlayer()
		{
			return PackSlayer( 0.05 );
		}

		public bool PackSlayer( double chance )
		{
			if ( chance <= Utility.RandomDouble() )
				return false;

			if ( Utility.RandomBool() )
			{
				BaseInstrument instrument = Loot.RandomInstrument(EraSE);

				if ( instrument != null )
				{
					instrument.Slayer = SlayerGroup.GetLootSlayerType( GetType() );
					PackItem( instrument );
				}
			}
			else if (!EraAOS)
			{
				BaseWeapon weapon = Loot.RandomWeapon();

				if ( weapon != null )
				{
					weapon.Slayer = SlayerGroup.GetLootSlayerType( GetType() );
					PackItem( weapon );
				}
			}

			return true;
		}

		public bool PackWeapon( int minLevel, int maxLevel )
		{
			return PackWeapon( minLevel, maxLevel, 1.0 );
		}

		public bool PackWeapon( int minLevel, int maxLevel, double chance )
		{
			if ( chance <= Utility.RandomDouble() )
				return false;

			Cap( ref minLevel, 0, 5 );
			Cap( ref maxLevel, 0, 5 );

			BaseWeapon weapon = Loot.RandomWeapon();

			if ( weapon == null )
				return false;

			if ( 0.05 > Utility.RandomDouble() )
				weapon.Slayer = SlayerName.Silver;

            int damageLevel = RandomMinMaxScaled(minLevel, maxLevel);
            if (PseudoSeerStone.Instance != null && PseudoSeerStone.Instance._HighestDamageLevelSpawn < damageLevel)
            {
                if (damageLevel == 5 && PseudoSeerStone.ReplaceVanqWithSkillScrolls) { PackItem(PuzzleChest.CreateRandomSkillScroll()); }
                int platAmount = PseudoSeerStone.PlatinumPerMissedDamageLevel * (damageLevel - PseudoSeerStone.Instance._HighestDamageLevelSpawn);
                if (platAmount > 0) PackItem(new Platinum(platAmount));
                
                damageLevel = PseudoSeerStone.Instance._HighestDamageLevelSpawn;
            }
            weapon.DamageLevel = (WeaponDamageLevel)damageLevel;
			weapon.AccuracyLevel = (WeaponAccuracyLevel)RandomMinMaxScaled( minLevel, maxLevel );
			weapon.DurabilityLevel = (WeaponDurabilityLevel)RandomMinMaxScaled( minLevel, maxLevel );

			PackItem( weapon );

			return true;
		}

		public void PackGold( int amount )
		{
			if ( amount > 0 )
				PackItem( new Gold( amount ) );
		}

		public void PackGold( int min, int max )
		{
			PackGold( Utility.RandomMinMax( min, max ) );
		}

		public void PackStatue( int min, int max )
		{
			PackStatue( Utility.RandomMinMax( min, max ) );
		}

		public void PackStatue( int amount )
		{
			for ( int i = 0; i < amount; ++i )
				PackStatue();
		}

		public void PackStatue()
		{
			PackItem( Loot.RandomStatue() );
		}

		public void PackGem()
		{
			PackGem( 1 );
		}

		public void PackGem( int min, int max )
		{
			PackGem( Utility.RandomMinMax( min, max ) );
		}

		public void PackGem( int amount )
		{
			if ( amount <= 0 )
				return;

			Item gem = Loot.RandomGem();

			gem.Amount = amount;

			PackItem( gem );
		}

		public void PackNecroReg( int min, int max )
		{
			PackNecroReg( Utility.RandomMinMax( min, max ) );
		}

		public void PackNecroReg( int amount )
		{
			for ( int i = 0; i < amount; ++i )
				PackNecroReg();
		}

		public void PackNecroReg()
		{
			if (!EraAOS)
				return;

			PackItem( Loot.RandomNecromancyReagent() );
		}

		public void PackReg( int min, int max )
		{
			PackReg( Utility.RandomMinMax( min, max ) );
		}

		public void PackReg( int amount )
		{
			if ( amount <= 0 )
				return;

			Item reg = Loot.RandomReagent();

			reg.Amount = amount;

			PackItem( reg );
		}

		public Bag PackBagofRecallRegs( int amount )
		{
			Bag bag = new Bag();
			PackItem( bag );
			bag.DropItem( new BlackPearl   ( amount ) );
			bag.DropItem( new Bloodmoss    ( amount ) );
			bag.DropItem( new MandrakeRoot ( amount ) );
			return bag;
		}

		public Bag PackBagofRegs( int amount )
		{
			Bag bag = new Bag();
			PackItem( bag );
			bag.DropItem( new BlackPearl   ( amount ) );
			bag.DropItem( new Bloodmoss    ( amount ) );
			bag.DropItem( new Garlic       ( amount ) );
			bag.DropItem( new Ginseng      ( amount ) );
			bag.DropItem( new MandrakeRoot ( amount ) );
			bag.DropItem( new Nightshade   ( amount ) );
			bag.DropItem( new SulfurousAsh ( amount ) );
			bag.DropItem( new SpidersSilk  ( amount ) );
			return bag;
		}

		public void PackItem( Item item )
		{
			if ( Summoned || item == null )
			{
				if ( item != null )
					item.Delete();

				return;
			}

			Container pack = Backpack;

			if ( pack == null )
			{
				pack = new Backpack();

				pack.Movable = false;

				AddItem( pack );
			}

			if ( !item.Stackable || !pack.TryDropItem( this, item, false ) ) // try stack
				pack.DropItem( item ); // failed, drop it anyway
		}
		#endregion

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel >= AccessLevel.GameMaster && !Body.IsHuman )
			{
				Container pack = this.Backpack;

				if ( pack != null )
					pack.DisplayTo( from );
			}

			base.OnDoubleClick( from );
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );

			if ( Summoned && !IsAnimatedDead )
				list.Add( 1049646 ); // (summoned)
			else if ( Controlled && Commandable )
			{
				if ( IsBonded )	//Intentional difference (showing ONLY bonded when bonded instead of bonded & tame)
					list.Add( 1049608 ); // (bonded)
				else
					list.Add( 502006 ); // (tame)
			}
		}

		public override void OnSingleClick( Mobile from )
		{
		    if (SpecialTitle != null || SpecialTitle != string.Empty)
		    {
                PrivateOverheadMessage(MessageType.Regular, TitleHue, true, SpecialTitle, from.NetState);		        
		    }
			if ( Controlled && Commandable )
			{
				int number;

				if ( Summoned )
					number = 1049646; // (summoned)
				else if ( IsBonded )
					number = 1049608; // (bonded)
				else
					number = 502006; // (tame)

				PrivateOverheadMessage( MessageType.Regular, 0x3B2, number, from.NetState );
			}

			base.OnSingleClick( from );
		}

		public virtual double TreasureMapChance{ get{ return TreasureMap.LootChance; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TMapLevel { get { return TreasureMapLevel; } }

		public virtual int TreasureMapLevel{ get{ return -1; } }

		public virtual bool IgnoreYoungProtection { get { return false; } }

        public override void OnNetStateChanged()
        {
            // pseudoseers logging out--don't make them disappear!
            if (this.m_LogoutTimer != null)
            {
                this.m_LogoutTimer.Stop();
            }
            this.m_LogoutTimer = null;
        }

        public override void Delete()
        {
            if (this.NetState != null && !CreaturePossession.AttemptReturnToOriginalBody(this.NetState))
            {
                this.NetState.Dispose();
            }
            base.Delete();
        }

		public override bool OnBeforeDeath()
		{
			int treasureLevel = TreasureMapLevel;

            // you can live forever!! mwahaha
            if (XmlScript.HasTrigger(this, TriggerName.onBeforeDeath) && UberScriptTriggers.Trigger(this, this.LastKiller, TriggerName.onBeforeDeath))
            {
                return false;
            }

/*
			if ( treasureLevel == 1 && this.Map == Map.Trammel && TreasureMap.IsInHavenIsland( this ) )
			{
				Mobile killer = this.LastKiller;

				if ( killer is BaseCreature )
					killer = ((BaseCreature)killer).GetMaster();

				if ( killer is PlayerMobile && ((PlayerMobile)killer).Young )
					treasureLevel = 0;
			}
*/

            // ALAN MOD: for pseudoseer controlled mobs
            if (Party != null && Party is Party)
            {
                try
                {
                    ((Party)Party).Remove(this);
                }
                catch { }
            }
            // END ALAN MOD

            // if they are controlled by a pseudoseer, attempt to reconnect:

            if (this.NetState != null && !CreaturePossession.AttemptReturnToOriginalBody(this.NetState))
            {
                this.NetState.Dispose();
            }

			if ( !Summoned && !NoKillAwards && !IsBonded && treasureLevel >= 0 )
			{
				if ( m_Paragon || m_Corrupt)
                {
                    if (m_Paragon)
                    {
                        if (Fame < 10000)
                            ForceAddLoot(LootPack.Rich, 2);
                        else if (Fame < 20000)
                            ForceAddLoot(LootPack.FilthyRich, 2);
                        else
                            ForceAddLoot(LootPack.FilthyRich, 4);
                    }


                    if (m_Paragon && Paragon.ChestChance > Utility.RandomDouble())
                    {
                        var chest = new ParagonChest(Name, treasureLevel);
                        PackItem(chest);

                        if (Utility.RandomDouble() < PseudoSeerStone.MobStatueChance*2)
                        {
                            MonsterStatuette.TryAddStatue(chest, GetType(), 1157);
                        }
                    }

                    if (m_Corrupt && HalloweenEventController.Instance != null && HalloweenEventController.CostumeChance > Utility.RandomDouble())
                        PackItem(HalloweenMask.Randomize());

                    if (m_Corrupt && HalloweenEventController.Instance != null && HalloweenEventController.HalloweenSkinDyeChance > Utility.RandomDouble())
                        PackItem(Utility.RandomBool() ? new HalloweenPaint(1358) : new HalloweenPaint(1378));

                    if (m_Corrupt && Paragon.ChestChance > Utility.RandomDouble())
                    {
                        var chest = new ParagonChest(Name, treasureLevel);
                        chest.Hue = Hue;
                        PackItem(chest);
                        chest.Name = "Chest of Goodies";

                        if (Utility.RandomDouble() < 0.01)
                        {
                            MonsterStatuette.TryAddStatue(chest, GetType(), Hue);
                        }
                    }

                }
				else if ( (Map == Map.Felucca) && TreasureMap.LootChance >= Utility.RandomDouble() )
					PackItem( new TreasureMap( treasureLevel, Map ) );

			    var metapet = LastKiller as BaseMetaPet;
			    if (metapet != null)
			    {
                    metapet.DoAbility(MetaSkillType.GoldFind, this);
			    }
			}

			if ( !Summoned && !NoKillAwards && !m_HasGeneratedLoot )
			{
				m_HasGeneratedLoot = true;
				GenerateLoot( false );
			}

			if ( !NoKillAwards && ( Region != null && Region.IsPartOf( "Doom" ) ) )
			{
				int bones = Engines.Quests.Doom.TheSummoningQuest.GetDaemonBonesFor( this );

				if ( bones > 0 )
					PackItem( new DaemonBone( bones ) );
			}

            if (Invasion != null)
            {
                Invasion.HandleInvaderDeath(this);
            }

            if (Portal != null)
            {
                Portal.HandleMobDeath(this);
            }

			if ( IsAnimatedDead )
				Effects.SendLocationEffect( Location, Map, 0x3728, 13, 1, 0x461, 4 );

			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null && Speaks)
				speechType.OnDeath( this );

            
			return base.OnBeforeDeath();
		}

		private bool m_NoKillAwards;

		public bool NoKillAwards
		{
			get{ return m_NoKillAwards; }
			set{ m_NoKillAwards = value; }
		}

		public int ComputeBonusDamage( List<DamageEntry> list, Mobile m )
		{
			int bonus = 0;

			for ( int i = list.Count - 1; i >= 0; --i )
			{
				DamageEntry de = list[i];

				if ( de.Damager == m || !(de.Damager is BaseCreature) )
					continue;

				BaseCreature bc = (BaseCreature)de.Damager;
				Mobile master = null;

				master = bc.GetMaster();

				if ( master == m )
					bonus += de.DamageGiven;
			}

			return bonus;
		}

		public Mobile GetMaster()
		{
			if ( Controlled && ControlMaster != null )
				return ControlMaster;
			else if ( Summoned && SummonMaster != null )
				return SummonMaster;

			return null;
		}

		private class FKEntry
		{
			public Mobile m_Mobile;
			public int m_Damage;

			public FKEntry( Mobile m, int damage )
			{
				m_Mobile = m;
				m_Damage = damage;
			}
		}

		public static List<DamageStore> GetLootingRights( List<DamageEntry> damageEntries, int hitsMax )
		{
			return GetLootingRights( damageEntries, hitsMax, false );
		}

		public static List<DamageStore> GetLootingRights( List<DamageEntry> damageEntries, int hitsMax, bool partyAsIndividual )
		{
			List<DamageStore> rights = new List<DamageStore>();

			for ( int i = damageEntries.Count - 1; i >= 0; --i )
			{
				if ( i >= damageEntries.Count )
					continue;

				DamageEntry de = damageEntries[i];

				if ( de.HasExpired )
				{
					damageEntries.RemoveAt( i );
					continue;
				}

				int damage = de.DamageGiven;

				List<DamageEntry> respList = de.Responsible;

				if ( respList != null )
				{
					for ( int j = 0; j < respList.Count; ++j )
					{
						DamageEntry subEntry = respList[j];
						Mobile master = subEntry.Damager;

						if ( master == null || master.Deleted || !master.Player )
							continue;

						bool needNewSubEntry = true;

						for ( int k = 0; needNewSubEntry && k < rights.Count; ++k )
						{
							DamageStore ds = rights[k];

							if ( ds.m_Mobile == master )
							{
								ds.m_Damage += subEntry.DamageGiven;
								needNewSubEntry = false;
							}
						}

						if ( needNewSubEntry )
							rights.Add( new DamageStore( master, subEntry.DamageGiven ) );

						damage -= subEntry.DamageGiven;
					}
				}

				Mobile m = de.Damager;

				if ( m == null || m.Deleted || !m.Player )
					continue;

				if ( damage <= 0 )
					continue;

				bool needNewEntry = true;

				for ( int j = 0; needNewEntry && j < rights.Count; ++j )
				{
					DamageStore ds = rights[j];

					if ( ds.m_Mobile == m )
					{
						ds.m_Damage += damage;
						needNewEntry = false;
					}
				}

				if ( needNewEntry )
					rights.Add( new DamageStore( m, damage ) );
			}

			if ( rights.Count > 0 )
			{
				//rights[0].m_Damage = (int)(rights[0].m_Damage * 1.25);	//This would be the first valid person attacking it.  Gets a 25% bonus.  Per 1/19/07 Five on Friday

				if ( rights.Count > 1 )
					rights.Sort();			//Sort by damage

				int topDamage = rights[0].m_Damage;
				int minDamage;

				if ( hitsMax >= 3000 )
					minDamage = topDamage / 16;
				else if ( hitsMax >= 1000 )
					minDamage = topDamage / 8;
				else if ( hitsMax >= 200 )
					minDamage = topDamage / 4;
				else
					minDamage = topDamage / 2;

				for ( int i = 0; i < rights.Count; ++i )
				{
					DamageStore ds = rights[i];

					ds.m_HasRight = ( ds.m_Damage >= minDamage );
				}
			}

			return rights;
		}

		public virtual void OnKilledBy( Mobile mob )
		{
			if ( IsParagon )
			{
				if ( Paragon.CheckArtifactChance( mob, this ) )
					Paragon.GiveArtifactTo( mob );
			}
		}

		public override void OnDeath(Container c)
		{
			Corpse corpse = c as Corpse;

			if (corpse != null && corpse.ProxyCorpse != null)
				c = corpse.ProxyCorpse;

		    if (ZombieSerial != null)
		    {
		        var instance = ZombieEvent.GetInstance(ZombieSerial);
		        if (instance != null && LastKiller != null && LastKiller is ZombieAvatar)
		            instance.HandleMobDeath(this, LastKiller as ZombieAvatar);
		    }

			List<XmlTeam> teams = XmlAttach.GetTeams(this);

			if (teams != null)
			{
				try
				{
					foreach (XmlTeam team in teams)
					{
						// copy a new XmlTeam attachment onto the corpse, but no scripts
						XmlAttach.AttachTo(
							c,
							new XmlTeam
							{
								TeamVal = team.TeamVal,
								TeamGreen = team.TeamGreen
							});
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("Attach team to corpse error: " + e.Message);
					Console.WriteLine(e.StackTrace);
				}
			}

		    if (!Controlled && !Summoned && evodragons != null)
		    {
                foreach (var evolutionDragon in evodragons)
		        {
                    int score = evolutionDragon.Value;
                    
		            if (score > HitsMax)
		            {
		                score = HitsMax;
		            }
		            evolutionDragon.Key.Addpoints(this, score);
		        }
                evodragons.Clear();
		    }

		    MeerMage.StopEffect(this, false);

			if (IsBonded)
			{
				int sound = GetDeathSound();

				if (sound >= 0)
					Effects.PlaySound(this, Map, sound);

				Warmode = false;

				Poison = null;
				Combatant = null;

				Hits = 0;
				Stam = 0;
				Mana = 0;

				IsDeadPet = true;
				ControlTarget = ControlMaster;
				ControlOrder = OrderType.Follow;

				ProcessDeltaQueue();
				SendIncomingPacket();
				SendIncomingPacket();

				List<AggressorInfo> aggressors = Aggressors;

				foreach (AggressorInfo info in aggressors.Where(info => info.Attacker.Combatant == this))
				{
					info.Attacker.Combatant = null;
				}

				List<AggressorInfo> aggressed = Aggressed;

				foreach (AggressorInfo info in aggressed.Where(info => info.Defender.Combatant == this))
				{
					info.Defender.Combatant = null;
				}

				Mobile owner = ControlMaster;

				if (owner == null || owner.Deleted || owner.Map != Map || !owner.InRange(this, 12) || !CanSee(owner) ||
					!InLOS(owner))
				{
					if (OwnerAbandonTime == DateTime.MinValue)
						OwnerAbandonTime = DateTime.UtcNow;
				}
				else
				{
					OwnerAbandonTime = DateTime.MinValue;
				}

				CheckStatTimers();
			}
			else
			{
				if (!Summoned && !m_NoKillAwards)
				{
					int totalFame = Fame / 100;
					int totalKarma = -Karma / 100;

					if (Map == Map.Felucca)
					{
						totalFame += ((totalFame / 10) * 3);
						totalKarma += ((totalKarma / 10) * 3);
					}

					List<DamageStore> list = GetLootingRights(DamageEntries, HitsMax);
					List<Mobile> titles = new List<Mobile>();
					List<int> fame = new List<int>();
					List<int> karma = new List<int>();

					bool givenQuestKill = false;
					bool givenFactionKill = false;
					bool givenToTKill = false;

					foreach (DamageStore ds in list)
					{
						if (!ds.m_HasRight)
							continue;

						Party party = Engines.PartySystem.Party.Get(ds.m_Mobile);

						if (party != null)
						{
							int divedFame = totalFame / party.Members.Count;
							int divedKarma = totalKarma / party.Members.Count;

							foreach (PartyMemberInfo info in party.Members)
							{
								if (info == null || info.Mobile == null)
								{
									continue;
								}

								int index = titles.IndexOf(info.Mobile);

								if (index == -1)
								{
									titles.Add(info.Mobile);
									fame.Add(divedFame);
									karma.Add(divedKarma);
								}
								else
								{
									fame[index] += divedFame;
									karma[index] += divedKarma;
								}
							}
						}
						else
						{
							titles.Add(ds.m_Mobile);
							fame.Add(totalFame);
							karma.Add(totalKarma);
						}

						OnKilledBy(ds.m_Mobile);

						if (!givenFactionKill)
						{
							givenFactionKill = true;
							Faction.HandleDeath(this, ds.m_Mobile);
						}

						Region region = ds.m_Mobile.Region;

						if (!givenToTKill &&
							(Map == Map.Tokuno || region.IsPartOf("Yomotsu Mines") || region.IsPartOf("Fan Dancer's Dojo")))
						{
							givenToTKill = true;
							TreasuresOfTokuno.HandleKill(this, ds.m_Mobile);
						}

						if (givenQuestKill)
							continue;

						PlayerMobile pm = ds.m_Mobile as PlayerMobile;

						if (pm == null)
						{
							continue;
						}

						QuestSystem qs = pm.Quest;

						if (qs != null)
						{
							qs.OnKill(this, c);
							givenQuestKill = true;
						}
					}

					for (int i = 0; i < titles.Count; ++i)
					{
						Titles.AwardFame(titles[i], fame[i], true);
						Titles.AwardKarma(titles[i], karma[i], true);
					}
				}

				XmlQuest.RegisterKill(this, LastKiller);

                if (c is Corpse)
                {
                    Corpse corpseToAddItemsToOrTriggerWith = ((Corpse)c).ProxyCorpse != null ? ((Corpse)c).ProxyCorpse : (Corpse)c;

                    if (Utility.RandomDouble() < PseudoSeerStone.MobStatueChance)
                    {
                        MonsterStatuette.TryAddStatue(corpseToAddItemsToOrTriggerWith, GetType());
                    }

                    double gargoyleRuneDropChance = this.HitsMaxSeed / 100 * DynamicSettingsController.GargoyleRuneSpawnChancePer100HP;
                    if (Utility.RandomDouble() < gargoyleRuneDropChance && !(this is BloodoftheHydra))
                    {
                        corpseToAddItemsToOrTriggerWith.AddItem(new GargoyleRune());
                    }

                    // proxy corpse used for Mobs with human bodies
                    if (XmlScript.HasTrigger(this, TriggerName.onDeath))
                        UberScriptTriggers.Trigger(this, this.LastKiller, TriggerName.onDeath, corpseToAddItemsToOrTriggerWith);
                }

				//EventSink.InvokeCreatureDeath(new CreatureDeathEventArgs(this, LastKiller, c));

                Conquests.HandleCreatureDeath(new CreatureConquestContainer(this, LastKiller, c));

				base.OnDeath(c);

				if (corpse != null && DeleteCorpseOnDeath)
					corpse.Delete();
			}
		}

		/* To save on cpu usage, RunUO creatures only reacquire creatures under the following circumstances:
		 *  - 10 seconds have elapsed since the last time it tried
		 *  - The creature was attacked
		 *  - Some creatures, like dragons, will reacquire when they see someone move
		 *
		 * This functionality appears to be implemented on OSI as well
		 */

		private DateTime m_NextReacquireTime;

		public DateTime NextReacquireTime{ get{ return m_NextReacquireTime; } set{ m_NextReacquireTime = value; } }

		public virtual TimeSpan ReacquireDelay{ get{ return TimeSpan.FromSeconds( 10.0 ); } }
		public virtual bool ReacquireOnMovement{ get{ return false; } }

		public virtual void ForceReacquire( Mobile m, bool resetBarding )
		{
			if ( resetBarding && m != null && m == BardMaster )
			{
				if ( BardProvoked )
				{
					double skill = m.Skills[SkillName.Provocation].Value;
					double chance = Math.Max( (100.0 - skill) / 2.0, 0.05 );

					if ( chance > Utility.RandomDouble() )
					{
						Combatant = m;
						m_NextReacquireTime = DateTime.UtcNow + ReacquireDelay;
					}
				}
				else if ( BardPacified )
				{
					double skill = m.Skills[SkillName.Peacemaking].Value;
					double chance = Math.Max( (100.0 - skill) / 2.0, 0.05 );

					if ( chance > Utility.RandomDouble() )
					{
						BardEndTime = DateTime.UtcNow;
						BardPacified = false;

						if ( AIObject != null )
							AIObject.DoMove( Direction );

						m_NextReacquireTime = DateTime.UtcNow + ReacquireDelay;
					}
				}
			}
			else
				m_NextReacquireTime = DateTime.MinValue;
		}

		public override void OnDelete()
		{
			Mobile m = m_ControlMaster;

            if (this is BaseChampion)
                PlayerScores.RemovePlayerScores(ChampSpawn != null ? (IEntity)ChampSpawn : this);

			SetControlMaster( null );
			SummonMaster = null;

            if (Portal != null)
            {
                Portal.HandleMobDeath(this);
            }

			base.OnDelete();

			if ( m != null )
				m.InvalidateProperties();

            // ALAN MOD: for pseudoseer controlled mobs
            if (Party != null && Party is Party)
            {
                try
                {
                    ((Party)Party).Remove(this);
                }
                catch { }
            }
            if (XmlScript.HasTrigger(this, TriggerName.onDelete))
                UberScriptTriggers.Trigger(this, this, TriggerName.onDelete);
		}

		public override bool CanBeHarmful( Mobile target, bool message, bool ignoreOurBlessedness )
		{
			if ( target is BaseFactionGuard )
				return false;

			if ( (target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier )
			{
				if ( message )
				{
					if ( target.Title == null )
						SendMessage( "{0} the vendor cannot be harmed.", target.Name );
					else
						SendMessage( "{0} {1} cannot be harmed.", target.Name, target.Title );
				}

				return false;
			}

			return base.CanBeHarmful( target, message, ignoreOurBlessedness );
		}

		public override bool CanBeRenamedBy( Mobile from )
		{
			bool ret = base.CanBeRenamedBy( from );

			if ( Controlled && from == ControlMaster && !from.Region.IsPartOf( typeof( Jail ) ) )
				ret = true;

			return ret;
		}

		public bool SetControlMaster( Mobile m )
		{
			if ( m == null )
			{
				ControlMaster = null;
				Controlled = false;
				ControlTarget = null;
				ControlOrder = OrderType.None;
				Guild = null;

				Delta( MobileDelta.Noto );
			}
			else
			{
				ISpawner se = this.Spawner;
				if ( se != null && se.UnlinkOnTaming )
				{
					this.Spawner.Remove( this );
					this.Spawner = null;
				}

				if ( m.Followers + ControlSlots > m.FollowersMax )
				{
					m.SendLocalizedMessage( 1049607 ); // You have too many followers to control that creature.
					return false;
				}

				CurrentWayPoint = null;//so tamed animals don't try to go back

				ControlMaster = m;
				Controlled = true;
				ControlTarget = null;
				ControlOrder = OrderType.Come;
				Guild = null;

				Delta( MobileDelta.Noto );
			}

			InvalidateProperties();

			return true;
		}

		public override void OnRegionChange( Region Old, Region New )
		{
			base.OnRegionChange( Old, New );

			if ( this.Controlled )
			{
				ISpawner se = this.Spawner;

				if ( se != null && !se.UnlinkOnTaming && ( New == null || !New.AcceptsSpawnsFrom( se.Region ) ) )
				{
					this.Spawner.Remove( this );
					this.Spawner = null;
				}
			}
		}

		private static bool m_Summoning;

		public static bool Summoning
		{
			get{ return m_Summoning; }
			set{ m_Summoning = value; }
		}

		public static bool Summon( BaseCreature creature, Mobile caster, Point3D p, int sound, TimeSpan duration )
		{
			return Summon( creature, true, caster, p, sound, duration );
		}

		public static bool Summon( BaseCreature creature, bool controlled, Mobile caster, Point3D p, int sound, TimeSpan duration )
		{
			if ( caster.Followers + creature.ControlSlots > caster.FollowersMax )
			{
				caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				creature.Delete();
				return false;
			}

			m_Summoning = true;

			if ( controlled )
				creature.SetControlMaster( caster );

			creature.RangeHome = 10;
			creature.Summoned = true;

			creature.SummonMaster = caster;

			Container pack = creature.Backpack;

			if ( pack != null )
			{
				for ( int i = pack.Items.Count - 1; i >= 0; --i )
				{
					if ( i >= pack.Items.Count )
						continue;

					pack.Items[i].Delete();
				}
			}

			new UnsummonTimer( caster, creature, duration ).Start();
			creature.m_SummonEnd = DateTime.UtcNow + duration;

			creature.MoveToWorld( p, caster.Map );

			Effects.PlaySound( p, creature.Map, sound );

			m_Summoning = false;

			return true;
		}

		private static bool EnableRummaging = true;

		private const double ChanceToRummage = 0.5; // 50%

		private const double MinutesToNextRummageMin = 1.0;
		private const double MinutesToNextRummageMax = 4.0;

		private const double MinutesToNextChanceMin = 0.25;
		private const double MinutesToNextChanceMax = 0.75;

		private DateTime m_NextRummageTime;

        public virtual bool CanBreath { get { return CanBreathCustom || (HasBreath && !Summoned); } }
		public virtual bool IsDispellable { get { return Summoned && !IsAnimatedDead; } }

		#region Healing
		public virtual bool CanHeal { get { return false; } }
		public virtual bool CanHealOwner { get { return false; } }
		public virtual double HealScalar { get { return 1.0; } }

		public virtual int HealSound { get { return 0x57; } }
		public virtual int HealStartRange { get { return 2; } }
		public virtual int HealEndRange { get { return RangePerception; } }
		public virtual double HealTrigger { get { return 0.78; } }
		public virtual double HealDelay { get { return 6.5; } }
		public virtual double HealInterval { get { return 0.0; } }
		public virtual bool HealFully { get { return true; } }
		public virtual double HealOwnerTrigger { get { return 0.78; } }
		public virtual double HealOwnerDelay { get { return 6.5; } }
		public virtual double HealOwnerInterval { get { return 30.0; } }
		public virtual bool HealOwnerFully { get { return false; } }

		private DateTime m_NextHealTime = DateTime.UtcNow;
		private DateTime m_NextHealOwnerTime = DateTime.UtcNow;
		private Timer m_HealTimer = null;

		public bool IsHealing { get { return ( m_HealTimer != null ); } }

		public virtual void HealStart( Mobile patient )
		{
			bool onSelf = ( patient == this );

			//DoBeneficial( patient );

			RevealingAction();

			if ( !onSelf )
			{
				patient.RevealingAction();
				patient.SendLocalizedMessage( 1008078, false, Name ); //  : Attempting to heal you.
			}

			double seconds = ( onSelf ? HealDelay : HealOwnerDelay ) + ( patient.Alive ? 0.0 : 5.0 );

			m_HealTimer = Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( seconds ), new TimerStateCallback<Mobile>( Heal_Callback ), patient );
		}

		private void Heal_Callback( Mobile patient )
		{
			Heal( patient );
		}

		public virtual void Heal( Mobile patient )
		{
			if ( !Alive || this.Map == Map.Internal || !CanBeBeneficial( patient, true, true ) || patient.Map != this.Map || !InRange( patient, HealEndRange ) )
			{
				StopHeal();
				return;
			}

			bool onSelf = ( patient == this );

			if ( !patient.Alive )
			{
			}
			else if ( patient.Poisoned )
			{
				int poisonLevel = patient.Poison.Level;

				double healing = Skills.Healing.Value;
				double anatomy = Skills.Anatomy.Value;
				double chance = ( healing - 30.0 ) / 50.0 - poisonLevel * 0.1;

				if ( ( healing >= 60.0 && anatomy >= 60.0 ) && chance > Utility.RandomDouble() )
				{
					if ( patient.CurePoison( this ) )
					{
						patient.SendLocalizedMessage( 1010059 ); // You have been cured of all poisons.

						CheckSkill( SkillName.Healing, 0.0, 60.0 + poisonLevel * 10.0 ); // TODO: Verify formula
						CheckSkill( SkillName.Anatomy, 0.0, 100.0 );
					}
				}
			}
			else if ( BleedAttack.IsBleeding( patient ) )
			{
				patient.SendLocalizedMessage( 1060167 ); // The bleeding wounds have healed, you are no longer bleeding!
				BleedAttack.EndBleed( patient, false );
			}
			else
			{
				double healing = Skills.Healing.Value;
				double anatomy = Skills.Anatomy.Value;
				double chance = ( healing + 10.0 ) / 100.0;

				if ( chance > Utility.RandomDouble() )
				{
					double min, max;

					min = ( anatomy / 10.0 ) + ( healing / 6.0 ) + 4.0;
					max = ( anatomy / 8.0 ) + ( healing / 3.0 ) + 4.0;

					if ( onSelf )
						max += 10;

					double toHeal = min + ( Utility.RandomDouble() * ( max - min ) );

					toHeal *= HealScalar;

					patient.Heal( (int)toHeal );

					CheckSkill( SkillName.Healing, 0.0, 90.0 );
					CheckSkill( SkillName.Anatomy, 0.0, 100.0 );
				}
			}

			HealEffect( patient );

			StopHeal();

			if ( ( onSelf && HealFully && Hits >= HealTrigger * HitsMax && Hits < HitsMax ) || ( !onSelf && HealOwnerFully && patient.Hits >= HealOwnerTrigger * patient.HitsMax && patient.Hits < patient.HitsMax ) )
				HealStart( patient );
		}

		public virtual void StopHeal()
		{
			if ( m_HealTimer != null )
				m_HealTimer.Stop();

			m_HealTimer = null;
		}

		public virtual void HealEffect( Mobile patient )
		{
			patient.PlaySound( HealSound );
		}
		#endregion

		#region Damaging Aura
		private DateTime m_NextAura;

		public virtual bool HasAura { get { return false; } }
		public virtual double AuraMinInterval { get { return 5.0; } }
		public virtual double AuraMaxInterval { get { return 10.0; } }
		public virtual int AuraRange { get { return 2; } }

		public virtual int AuraMinDamage { get { return 5; } }
		public virtual int AuraMaxDamage { get { return 10; } }
		public virtual int AuraPhysicalDamage { get{ return 0; } }
		public virtual int AuraFireDamage { get{ return 100; } }
		public virtual int AuraColdDamage { get{ return 0; } }
		public virtual int AuraPoisonDamage { get{ return 0; } }
		public virtual int AuraEnergyDamage { get{ return 0; } }
		public virtual int AuraChaosDamage { get { return 0; } }

		public virtual void AuraDamage()
		{
			if ( !Alive || IsDeadBondedPet || Controlled )
				return;

			List<Mobile> list = new List<Mobile>();

			foreach ( Mobile m in GetMobilesInRange( AuraRange ) )
			{
				if (m == this || !CanBeHarmful(m, false) || m.AccessLevel > AccessLevel.Player || (EraAOS && !InLOS(m)))
					continue;

				if ( m is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)m;

					if ( bc.Controlled || bc.Summoned || bc.Team != Team )
						list.Add( m );
				}
				else if ( m.Player )
				{
					list.Add( m );
				}
			}

			Packet p = Packet.Acquire( new MessageLocalizedAffix( Serial.MinusOne, -1, MessageType.Label, 0x3B2, 3, 1072073, "", AffixType.Prepend | AffixType.System, Name, "" ) );

			foreach ( Mobile m in list )
			{
				DoHarmful( m );
				m.Hidden = false;
                m.Damage(Utility.RandomMinMax(AuraMinDamage, AuraMaxDamage), this);
				Combatant = m;
				m.Send( p );

				AuraEffect( m );
			}

			Packet.Release( p );
		}

		public virtual void AuraEffect( Mobile m )
		{
		}
		#endregion

		public virtual void OnThink()
		{
            if ( EnableRummaging && CanRummageCorpses && !Summoned && !Controlled && DateTime.UtcNow >= m_NextRummageTime )
			{
				double min, max;

				if ( ChanceToRummage > Utility.RandomDouble() && Rummage() )
				{
					min = MinutesToNextRummageMin;
					max = MinutesToNextRummageMax;
				}
				else
				{
					min = MinutesToNextChanceMin;
					max = MinutesToNextChanceMax;
				}

				double delay = min + (Utility.RandomDouble() * (max - min));
				m_NextRummageTime = DateTime.UtcNow + TimeSpan.FromMinutes( delay );
			}

			if ( CanBreath && DateTime.UtcNow >= m_NextBreathTime ) // tested: controlled dragons do breath fire, what about summoned skeletal dragons?
			{
				Mobile target = this.Combatant;

				if ( target != null && target.Alive && !target.IsDeadBondedPet && CanBeHarmful( target ) && target.Map == this.Map && !IsDeadBondedPet && target.InRange( this, BreathRange ) && CanSee( target ) && InLOS( target ) && !BardPacified )
				{
					if( ( DateTime.UtcNow - m_NextBreathTime ) < TimeSpan.FromSeconds( 30 ) )
						BreathStart( target );

                    SetNextBreathTime();
				}
			}

			if ( ( CanHeal || CanHealOwner ) && Alive && !IsHealing && !BardPacified )
			{
				Mobile owner = this.ControlMaster;

				if ( owner != null && CanHealOwner && DateTime.UtcNow >= m_NextHealOwnerTime && CanBeBeneficial( owner, true, true ) && owner.Map == this.Map && InRange( owner, HealStartRange ) && InLOS( owner ) && owner.Hits < HealOwnerTrigger * owner.HitsMax )
				{
					HealStart( owner );

					m_NextHealOwnerTime = DateTime.UtcNow + TimeSpan.FromSeconds( HealOwnerInterval );
				}
				else if ( CanHeal && DateTime.UtcNow >= m_NextHealTime && CanBeBeneficial( this ) && ( Hits < HealTrigger * HitsMax || Poisoned ) )
				{
					HealStart( this );

					m_NextHealTime = DateTime.UtcNow + TimeSpan.FromSeconds( HealInterval );
				}
			}

			if ( HasAura && DateTime.UtcNow >= m_NextAura )
			{
				AuraDamage();
				m_NextAura = DateTime.UtcNow + TimeSpan.FromSeconds( AuraMinInterval + ( Utility.RandomDouble() * ( AuraMaxInterval - AuraMinInterval ) ) );
			}
		}

		public virtual bool Rummage()
		{
			Corpse toRummage = null;

			foreach ( Item item in this.GetItemsInRange( 2 ) )
			{
				if ( item is Corpse && item.Items.Count > 0 )
				{
					toRummage = (Corpse)item;
					break;
				}
			}

			if ( toRummage == null )
				return false;

			Container pack = this.Backpack;

			if ( pack == null )
				return false;

			List<Item> items = toRummage.Items;

			bool rejected;
			LRReason reason;

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = items[Utility.Random( items.Count )];

				Lift( item, item.Amount, out rejected, out reason );

				if ( !rejected && Drop( this, new Point3D( -1, -1, 0 ) ) )
				{
					// *rummages through a corpse and takes an item*
					PublicOverheadMessage( MessageType.Emote, 0x3B2, 1008086 );
					//TODO: Instancing of Rummaged stuff.
					return true;
				}
			}

			return false;
		}

		public virtual void Pacify( Mobile master, DateTime endtime )
		{
			BardPacified = true;
			BardMaster = master;
			BardEndTime = endtime;
		}

		public override Mobile GetDamageMaster( Mobile damagee )
		{
			if ( m_bBardProvoked && damagee == m_bBardTarget )
				return m_bBardMaster;
			else if ( m_bControlled && m_ControlMaster != null )
				return m_ControlMaster;
			else if ( m_bSummoned && m_SummonMaster != null )
				return m_SummonMaster;

			return base.GetDamageMaster( damagee );
		}

		public void Provoke( Mobile master, Mobile target, bool bSuccess )
		{
			BardProvoked = true;

			this.PublicOverheadMessage( MessageType.Emote, EmoteHue, false, "*looks furious*" );

			if ( bSuccess )
			{
				PlaySound( GetIdleSound() );

				BardMaster = master;
				BardTarget = target;
				Combatant = target;
				BardEndTime = DateTime.UtcNow;

				if ( target is BaseCreature )
				{
					BaseCreature t = (BaseCreature)target;

					if ( t.Unprovokable || (t.IsParagon && BaseInstrument.GetBaseDifficulty( t ) >= 160.0))
						return;

					t.BardProvoked = true;

					t.BardMaster = master;
					t.BardTarget = this;
					t.Combatant = this;
					t.BardEndTime = DateTime.UtcNow;
				}
			}
			else
			{
				PlaySound( GetAngerSound() );

				BardMaster = master;
				BardTarget = target;
			}
		}

        protected override bool OnMove(Direction d)
        {
            if (XmlScript.HasTrigger(this, TriggerName.onMove) && UberScriptTriggers.Trigger(this, this, TriggerName.onMove))
            {
                return false;
            }
            if (!PseudoSeerStone.CreaturesStealthLikePlayers)
            {
                return base.OnMove(d);
            }

            if (Hidden)
            {
	            if (!Mounted && Skills.Stealth.Value >= 25.0)
	            {
		            bool running = (d & Direction.Running) != 0;

		            if (running)
		            {
			            if ((AllowedStealthSteps -= 2) <= 0)
				            RevealingAction();
		            }
		            else if (AllowedStealthSteps-- <= 0)
		            {
			            Server.SkillHandlers.Stealth.OnUse(this);
		            }
	            }
	            else
	            {
		            RevealingAction();
	            }
            }

            return true;
        }

		public bool FindMyName( string str, bool bWithAll )
		{
			int i, j;

			string name = this.Name;

			if( name == null || str.Length < name.Length )
				return false;

			string[] wordsString = str.Split(' ');
			string[] wordsName = name.Split(' ');

			for ( j=0 ; j < wordsName.Length; j++ )
			{
				string wordName = wordsName[j];

				bool bFound = false;
				for ( i=0 ; i < wordsString.Length; i++ )
				{
					string word = wordsString[i];

					if ( Insensitive.Equals( word, wordName ) )
						bFound = true;

					if ( bWithAll && Insensitive.Equals( word, "all" ) )
						return true;
				}

				if ( !bFound )
					return false;
			}

			return true;
		}

		public static void TeleportPets( Mobile master, Point3D loc, Map map )
		{
			TeleportPets( master, loc, map, false );
		}

		public static void TeleportPets( Mobile master, Point3D loc, Map map, bool onlyBonded )
		{
			List<Mobile> move = new List<Mobile>();

			foreach ( Mobile m in master.GetMobilesInRange( 3 ) )
			{
				if ( m is BaseCreature )
				{
					BaseCreature pet = (BaseCreature)m;

					if ( pet.Controlled && ( pet.ControlMaster == master || pet.IsPetFriend( m ) ) )
					{
						if ( !onlyBonded || pet.IsBonded )
						{
							if ( pet.ControlOrder == OrderType.Guard || pet.ControlOrder == OrderType.Follow || pet.ControlOrder == OrderType.Come )
								move.Add( pet );
						}
					}
				}
			}

			foreach ( Mobile m in move )
				m.MoveToWorld( loc, map );
		}

		public virtual void ResurrectPet()
		{
			if ( IsDeadPet )
			{
				OnBeforeResurrect();

				Poison = null;

				Warmode = false;

				Hits = 10;
				Stam = StamMax;
				Mana = 0;

				ProcessDeltaQueue();

				IsDeadPet = false;

				Effects.SendPacket( Location, Map, new BondedStatus( 0, this.Serial, 0 ) );

				this.SendIncomingPacket();
				this.SendIncomingPacket();

				OnAfterResurrect();

				Mobile owner = this.ControlMaster;

				if ( owner == null || owner.Deleted || owner.Map != this.Map || !owner.InRange( this, 12 ) || !this.CanSee( owner ) || !this.InLOS( owner ) )
				{
					if ( this.OwnerAbandonTime == DateTime.MinValue )
						this.OwnerAbandonTime = DateTime.UtcNow;
				}
				else
				{
					this.OwnerAbandonTime = DateTime.MinValue;
				}

				CheckStatTimers();
			}
		}

		public override bool CanBeDamaged()
		{
			if ( IsDeadPet )
				return false;

			return base.CanBeDamaged();
		}

		public virtual bool PlayerRangeSensitive{ get{ return (this.CurrentWayPoint == null); } }	//If they are following a waypoint, they'll continue to follow it even if players aren't around

		public override void OnSectorDeactivate()
		{
			if ( PlayerRangeSensitive && m_AI != null )
				m_AI.Deactivate();

			base.OnSectorDeactivate();
		}

		public override void OnSectorActivate()
		{
			if ( PlayerRangeSensitive && m_AI != null )
				m_AI.Activate();

			base.OnSectorActivate();
		}

		private bool m_RemoveIfUntamed;

		// used for deleting untamed creatures [in houses]
		private int m_RemoveStep;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RemoveIfUntamed{ get{ return m_RemoveIfUntamed; } set{ m_RemoveIfUntamed = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int RemoveStep { get { return m_RemoveStep; } set { m_RemoveStep = value; } }

        public override void OnConnected()
        {
            if (XmlScript.HasTrigger(this, TriggerName.onLogin))
                UberScriptTriggers.Trigger(this, this, TriggerName.onLogin);
            base.OnConnected();
        }

        public override void OnDisconnected()
        {
        
            if (CreaturePossession.HasAnyPossessPermissions(this.Account))
                LoggingCustom.LogPseudoseer(DateTime.Now + "\t" + this.Account + "\t" + this + "\tdisconnected\t" + "\tRegion: " + this.Region + "\tLocation: " + this.Location);
            
            
            if (XmlScript.HasTrigger(this, TriggerName.onDisconnected))
                UberScriptTriggers.Trigger(this, this, TriggerName.onDisconnected);
            base.OnDisconnected();
        }
	}

	public class LoyaltyTimer : Timer
	{
		private static TimeSpan InternalDelay = TimeSpan.FromMinutes( 5.0 );

		public static void Initialize()
		{
			new LoyaltyTimer().Start();
		}

		public LoyaltyTimer() : base( InternalDelay, InternalDelay )
		{
			m_NextHourlyCheck = DateTime.UtcNow + TimeSpan.FromHours( 1.0 );
			Priority = TimerPriority.FiveSeconds;
		}

		private DateTime m_NextHourlyCheck;

		protected override void OnTick()
		{
			if ( DateTime.UtcNow >= m_NextHourlyCheck )
			{
				m_NextHourlyCheck = DateTime.UtcNow + TimeSpan.FromHours( 1.0 );

				List<BaseCreature> toRelease = new List<BaseCreature>();

				// added array for wild creatures in house regions to be removed
				List<BaseCreature> toRemove = new List<BaseCreature>();

				foreach ( Mobile m in World.Mobiles.Values )
				{
					if ( m is BaseMount && ((BaseMount)m).Rider != null )
					{
						((BaseCreature)m).OwnerAbandonTime = DateTime.MinValue;
						continue;
					}

					if ( m is BaseCreature )
					{
						BaseCreature c = (BaseCreature)m;
                        if (c is SkeletalMount)
                        {
                            c.Loyalty = BaseCreature.MaxLoyalty;
                            continue; // skeletal mounts don't go untame
                        }

						if ( c.IsDeadPet )
						{
							Mobile owner = c.ControlMaster;

							if ( owner == null || owner.Deleted || owner.Map != c.Map || !owner.InRange( c, 12 ) || !c.CanSee( owner ) || !c.InLOS( owner ) )
							{
								if ( c.OwnerAbandonTime == DateTime.MinValue )
									c.OwnerAbandonTime = DateTime.UtcNow;
								else if ( (c.OwnerAbandonTime + c.BondingAbandonDelay) <= DateTime.UtcNow )
									toRemove.Add( c );
							}
							else
								c.OwnerAbandonTime = DateTime.MinValue;
						}
						else if ( c.Controlled && c.Commandable )
						{
							c.OwnerAbandonTime = DateTime.MinValue;

							if ( c.Map != Map.Internal && !c.IsStabled )
							{
								c.Loyalty -= (BaseCreature.MaxLoyalty / 10);

								if( c.Loyalty < (BaseCreature.MaxLoyalty / 10) )
								{
									c.Say( 1043270, c.Name ); // * ~1_NAME~ looks around desperately *
									c.PlaySound( c.GetIdleSound() );
								}

								if ( c.Loyalty <= 0)
									toRelease.Add( c );
							}
						}

						// added lines to check if a wild creature in a house region has to be removed or not
						if ( !c.Controlled && !c.IsStabled && ( (c.Region.IsPartOf( typeof(HouseRegion) ) && c.CanBeDamaged()) || (c.RemoveIfUntamed && c.Spawner == null) ) )
						{
							c.RemoveStep++;

							if ( c.RemoveStep >= 48 )
								toRemove.Add( c );
						}
						else
							c.RemoveStep = 0;
					}
				}

				foreach ( BaseCreature c in toRelease )
				{
                    try
                    {
                        string toWrite = DateTime.Now + "\t" + c + "\t" + this.GetType() + "\tBonded=" + c.IsBonded + "\tOwner=" + c.ControlMaster + "\tLoyalty:" + c.Loyalty + "\tIsStabled:" + c.IsStabled + "\tStabledDate:" + c.StabledDate;
                        toWrite += "\n" + (new System.Diagnostics.StackTrace()).ToString();
                        LoggingCustom.Log("LOG_PetLoyaltyRelease.txt", toWrite);
                    }
                    catch { }
                    c.Say( 1043255, c.Name ); // ~1_NAME~ appears to have decided that is better off without a master!
					c.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy
					c.IsBonded = false;
					c.BondingBegin = DateTime.MinValue;
					c.OwnerAbandonTime = DateTime.UtcNow;
					c.ControlTarget = null;
					//c.ControlOrder = OrderType.Release;
                    if (c is MetaDragon)
                    {
                        c.Say("The evolution dragon reverts back to its egg form.");
                        var egg = new EvolutionEgg(c.Hue);
                        egg.MoveToWorld(c.Location, c.Map);
                        c.Delete();
                    }
                    else
					    c.AIObject.DoOrderRelease(); // this will prevent no release of creatures left alone with AI disabled (and consequent bug of Followers)
				}

				// added code to handle removing of wild creatures in house regions
			    foreach (BaseCreature c in toRemove)
			    {
			        if (!c.IsStabled)
			        {
                        if (c is EvolutionDragon)
                        {
                            c.Say("The evolution dragon reverts back to its egg form.");
                            var egg = new BaseMetaPet();
                            egg.MoveToWorld(c.Location, c.Map);
                        }
			            c.Delete();
			        }
			    }

			}
		}
	}
}