using System;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Factions;
using Server.Guilds;
using Server.Items;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

namespace Server.Items
{
	#region enumerations
	[Flags]
	public enum EGRFlags : ulong
	{
		No_Male					= 0x0000000000000001,
		No_Female				= 0x0000000000000002,
		No_NonDonators			= 0x0000000000000004,
		No_Donators				= 0x0000000000000008,
		No_Ghosts				= 0x0000000000000010,
		No_Living				= 0x0000000000000020,
		No_Naked				= 0x0000000000000040,//holding nothing
		No_Clothed				= 0x0000000000000080,
		No_Murderer				= 0x0000000000000100,
		No_BlueKarma			= 0x0000000000000200,
		No_GrayKarma			= 0x0000000000000400,
		No_Criminal				= 0x0000000000000800,
		No_Mounted				= 0x0000000000001000,
		No_OnFoot				= 0x0000000000002000,
		No_Factionless			= 0x0000000000004000,
		No_TrueBrits			= 0x0000000000008000,
		No_COM					= 0x0000000000010000,
		No_Minax				= 0x0000000000020000,
		No_Shadowlords			= 0x0000000000040000,
		No_Factions				= 0x0000000000078000,
		No_Guildless			= 0x0000000000080000,
		No_RegGuild				= 0x0000000000100000,
		No_OrderGuild			= 0x0000000000200000,
		No_ChaosGuild			= 0x0000000000400000,
		No_Guilds				= 0x0000000000700000,
		No_Veteran				= 0x0000000000800000,
		No_Young				= 0x0000000001000000,
		No_NPCGuildless			= 0x0000000002000000,
		No_MagesGuild			= 0x0000000004000000,
		No_WarriorsGuild		= 0x0000000008000000,
		No_ThievesGuild			= 0x0000000010000000,
		No_RangersGuild			= 0x0000000020000000,
		No_HealersGuild			= 0x0000000040000000,
		No_MinersGuild			= 0x0000000080000000,
		No_MerchantsGuild		= 0x0000000100000000,
		No_TinkersGuild			= 0x0000000200000000,
		No_TailorsGuild			= 0x0000000400000000,
		No_FishermensGuild		= 0x0000000800000000,
		No_BardsGuild			= 0x0000001000000000,
		No_BlacksmithsGuild		= 0x0000002000000000,
		No_NPCGuilds			= 0x0000003FFC000000
	}

	[Flags]
	public enum EGateOptFlags
	{
		Active					= 0x00000001,
		StrMax					= 0x00000002,
		IntMax					= 0x00000004,
		DexMax					= 0x00000008,
		RemoveGameRobe			= 0x00000010,
		GiveGameRobe			= 0x00000020,//otherwise, remove game robes
		StaffOnly				= 0x00000040,
		FreeForDonators			= 0x00000080,
		HideWhenFull			= 0x00000100,
		StaffOverride			= 0x00000200,
		ResGhosts				= 0x00000400,
		CannotFleeFromBattle	= 0x00000800,
		RemoveFSL				= 0x00001000,
		TransportPets			= 0x00002000,
		RemovePolymorph			= 0x00004000,
		RemoveIncognito			= 0x00008000,
		RemoveDisguise			= 0x00010000,
		IsTeleporter			= 0x00020000,
		CheckTollBackpack		= 0x00040000,
		CheckTollBankBox		= 0x00080000,
		Skill1Max				= 0x00100000,
		Skill2Max				= 0x00200000
	}

	public enum MinMaxOption{ Min, Max }
	public enum GenderOption{ Male, Female, Both }
	public enum VetOption{ Veteran, Young, Both }
	public enum NakedOption{ Clothed, Naked, Both }
	public enum MountOption{ Mounted, Unmounted, Both }
	public enum DonatorOption{ Donator, NonDonator, Both }
	public enum GhostOption{ Living, Ghost, Both }
	public enum TollOption{ Backpack, BankBox, Both }
	#endregion

	[PropertyObject]
	public class FactionOption
	{
		private ExtEventMoongate m_Gate;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Factionless{ get{ return !m_Gate.No_Factionless; } set{ m_Gate.No_Factionless = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool COM{ get{ return !m_Gate.No_COM; } set{ m_Gate.No_COM = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Minax{ get{ return !m_Gate.No_Minax; } set{ m_Gate.No_Minax = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool TrueBrits{ get{ return !m_Gate.No_TrueBrits; } set{ m_Gate.No_TrueBrits = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Shadowlords{ get{ return !m_Gate.No_Shadowlords; } set{ m_Gate.No_Shadowlords = !value; } }

		public FactionOption( ExtEventMoongate gate )
		{
			m_Gate = gate;
		}

		public override string ToString()
		{
			return "...";
		}
	}

	[PropertyObject]
	public class GuildOption
	{
		private ExtEventMoongate m_Gate;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Guildless{ get{ return !m_Gate.No_Guildless; } set{ m_Gate.No_Guildless = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Order{ get{ return !m_Gate.No_OrderGuild; } set{ m_Gate.No_OrderGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Chaos{ get{ return !m_Gate.No_ChaosGuild; } set{ m_Gate.No_ChaosGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Neutral{ get{ return !m_Gate.No_RegGuild; } set{ m_Gate.No_RegGuild = !value; } }

		public GuildOption( ExtEventMoongate gate )
		{
			m_Gate = gate;
		}

		public override string ToString()
		{
			return "...";
		}
	}

	[PropertyObject]
	public class NotoOption
	{
		private ExtEventMoongate m_Gate;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Murderer{ get{ return !m_Gate.No_Murderer; } set{ m_Gate.No_Murderer = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool GrayKarma{ get{ return !m_Gate.No_GrayKarma; } set{ m_Gate.No_GrayKarma = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BlueKarma{ get{ return !m_Gate.No_BlueKarma; } set{ m_Gate.No_BlueKarma = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Criminal{ get{ return !m_Gate.No_Criminal; } set{ m_Gate.No_Criminal = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int KarmaThreshold{ get{ return m_Gate.KarmaThreshold; } set{ m_Gate.KarmaThreshold = value; } }

		public NotoOption( ExtEventMoongate gate )
		{
			m_Gate = gate;
		}

		public override string ToString()
		{
			return "...";
		}
	}

	[PropertyObject]
	public class NPCGuildOption
	{
		private ExtEventMoongate m_Gate;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Guildless{ get{ return !m_Gate.No_NPCGuildless; } set{ m_Gate.No_NPCGuildless = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool MagesGuild{ get{ return !m_Gate.No_MagesGuild; } set{ m_Gate.No_MagesGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool WarriorsGuild{ get{ return !m_Gate.No_WarriorsGuild; } set{ m_Gate.No_WarriorsGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool ThievesGuild{ get{ return !m_Gate.No_ThievesGuild; } set{ m_Gate.No_ThievesGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RangersGuild{ get{ return !m_Gate.No_RangersGuild; } set{ m_Gate.No_RangersGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HealersGuild{ get{ return !m_Gate.No_HealersGuild; } set{ m_Gate.No_HealersGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool MinersGuild{ get{ return !m_Gate.No_MinersGuild; } set{ m_Gate.No_MinersGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool MerchantsGuild{ get{ return !m_Gate.No_MerchantsGuild; } set{ m_Gate.No_MerchantsGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool TinkersGuild{ get{ return !m_Gate.No_TinkersGuild; } set{ m_Gate.No_TinkersGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool TailorsGuild{ get{ return !m_Gate.No_TailorsGuild; } set{ m_Gate.No_TailorsGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool FishermensGuild{ get{ return !m_Gate.No_FishermensGuild; } set{ m_Gate.No_FishermensGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BardsGuild{ get{ return !m_Gate.No_BardsGuild; } set{ m_Gate.No_BardsGuild = !value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BlacksmithsGuild{ get{ return !m_Gate.No_BlacksmithsGuild; } set{ m_Gate.No_BlacksmithsGuild = !value; } }

		public NPCGuildOption( ExtEventMoongate gate )
		{
			m_Gate = gate;
		}

		public override string ToString()
		{
			return "...";
		}
	}

	public class ExtEventMoongate : Moongate
	{
		#region event gate restriction flags
		[CommandProperty( AccessLevel.GameMaster )]
		public FactionOption Factions{ get{ return new FactionOption( this ); } set{} }

		[CommandProperty( AccessLevel.GameMaster )]
		public GuildOption Guilds{ get{ return new GuildOption( this ); } set{} }

		[CommandProperty( AccessLevel.GameMaster )]
		public NotoOption Notoriety{ get{ return new NotoOption( this ); } set{} }

		[CommandProperty( AccessLevel.GameMaster )]
		public NPCGuildOption NPCGuilds{ get{ return new NPCGuildOption( this ); } set{} }

		[CommandProperty( AccessLevel.GameMaster )]
		public GenderOption Gender
		{
			get{ return GetRestFlag( EGRFlags.No_Female ) ? GenderOption.Male : GetRestFlag( EGRFlags.No_Male ) ? GenderOption.Female : GenderOption.Both; }
			set
			{
				switch( value )
				{
					case GenderOption.Male: SetRestFlag( EGRFlags.No_Female, true ); SetRestFlag( EGRFlags.No_Male, false ); break;
					case GenderOption.Female: SetRestFlag( EGRFlags.No_Female, false ); SetRestFlag( EGRFlags.No_Male, true ); break;
					case GenderOption.Both: SetRestFlag( EGRFlags.No_Female | EGRFlags.No_Male, false ); break;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public VetOption Veterans
		{
			get{ return GetRestFlag( EGRFlags.No_Young ) ? VetOption.Veteran : GetRestFlag( EGRFlags.No_Veteran ) ? VetOption.Young : VetOption.Both; }
			set
			{
				switch( value )
				{
					case VetOption.Veteran: SetRestFlag( EGRFlags.No_Young, true ); SetRestFlag( EGRFlags.No_Veteran, false ); break;
					case VetOption.Young: SetRestFlag( EGRFlags.No_Young, false ); SetRestFlag( EGRFlags.No_Veteran, true ); break;
					case VetOption.Both: SetRestFlag( EGRFlags.No_Young | EGRFlags.No_Veteran, false ); break;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public GhostOption Ghosts
		{
			get{ return GetRestFlag( EGRFlags.No_Ghosts ) ? GhostOption.Living : GetRestFlag( EGRFlags.No_Living ) ? GhostOption.Ghost : GhostOption.Both; }
			set
			{
				switch( value )
				{
					case GhostOption.Living: SetRestFlag( EGRFlags.No_Ghosts, true ); SetRestFlag( EGRFlags.No_Living, false ); break;
					case GhostOption.Ghost: SetRestFlag( EGRFlags.No_Ghosts, false ); SetRestFlag( EGRFlags.No_Living, true ); break;
					case GhostOption.Both: SetRestFlag( EGRFlags.No_Ghosts | EGRFlags.No_Living, false ); break;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public NakedOption Naked
		{
			get{ return GetRestFlag( EGRFlags.No_Naked ) ? NakedOption.Clothed : GetRestFlag( EGRFlags.No_Clothed ) ? NakedOption.Naked : NakedOption.Both; }
			set
			{
				switch( value )
				{
					case NakedOption.Clothed: SetRestFlag( EGRFlags.No_Naked, true ); SetRestFlag( EGRFlags.No_Clothed, false ); break;
					case NakedOption.Naked: SetRestFlag( EGRFlags.No_Naked, false ); SetRestFlag( EGRFlags.No_Clothed, true ); break;
					case NakedOption.Both: SetRestFlag( EGRFlags.No_Naked | EGRFlags.No_Clothed, false ); break;
				}
			}
		}

		public bool No_Murderer
		{
			get{ return GetRestFlag( EGRFlags.No_Murderer ); }
			set
			{
				SetRestFlag( EGRFlags.No_Murderer, value );
				if ( value && GetRestFlag( EGRFlags.No_BlueKarma | EGRFlags.No_GrayKarma | EGRFlags.No_Criminal ) )
					SetRestFlag( EGRFlags.No_BlueKarma, false );
			}
		}

		public bool No_BlueKarma
		{
			get{ return GetRestFlag( EGRFlags.No_BlueKarma ); }
			set
			{
				SetRestFlag( EGRFlags.No_BlueKarma, value );
				if ( value && GetRestFlag( EGRFlags.No_Murderer | EGRFlags.No_GrayKarma | EGRFlags.No_Criminal ) )
					SetRestFlag( EGRFlags.No_GrayKarma, false );
			}
		}

		public bool No_GrayKarma
		{
			get{ return GetRestFlag( EGRFlags.No_GrayKarma ); }
			set
			{
				SetRestFlag( EGRFlags.No_GrayKarma, value );
				if ( value && GetRestFlag( EGRFlags.No_Murderer | EGRFlags.No_BlueKarma | EGRFlags.No_Criminal ) )
					SetRestFlag( EGRFlags.No_BlueKarma, false );
			}
		}

		public bool No_Criminal
		{
			get{ return GetRestFlag( EGRFlags.No_Criminal ); }
			set
			{
				SetRestFlag( EGRFlags.No_Criminal, value );
				if ( value && GetRestFlag( EGRFlags.No_Murderer | EGRFlags.No_BlueKarma | EGRFlags.No_GrayKarma ) )
					SetRestFlag( EGRFlags.No_BlueKarma, false );
			}
		}

		public bool No_OrderGuild
		{
			get{ return GetRestFlag( EGRFlags.No_OrderGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_OrderGuild, value );
				SetRestFlag( EGRFlags.No_COM | EGRFlags.No_TrueBrits, value );
				if ( value )
				{
					if ( GetRestFlag( EGRFlags.No_Guilds | EGRFlags.No_Guildless ) )
						SetRestFlag( EGRFlags.No_Guildless, false );
				}
			}
		}

		public bool No_ChaosGuild
		{
			get{ return GetRestFlag( EGRFlags.No_ChaosGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_ChaosGuild, value );
				if ( value )
				{
					if ( GetRestFlag( EGRFlags.No_Guilds | EGRFlags.No_Guildless ) )
						SetRestFlag( EGRFlags.No_Guildless, false );
					SetRestFlag( EGRFlags.No_Minax | EGRFlags.No_Shadowlords, true );
				}
			}
		}

		public bool No_RegGuild
		{
			get{ return GetRestFlag( EGRFlags.No_RegGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_RegGuild, value );
				if ( value && GetRestFlag( EGRFlags.No_Guilds | EGRFlags.No_Guildless ) )
					SetRestFlag( EGRFlags.No_Guildless, false );
			}
		}

		public bool No_Guildless
		{
			get{ return GetRestFlag( EGRFlags.No_Guildless ); }
			set
			{
				SetRestFlag( EGRFlags.No_Guildless, value );
				if ( value && GetRestFlag( EGRFlags.No_Guilds ) )
					SetRestFlag( EGRFlags.No_Guilds, false );
			}
		}

		public bool No_COM
		{
			get{ return GetRestFlag( EGRFlags.No_COM ); }
			set
			{
				SetRestFlag( EGRFlags.No_COM, value );
				if ( value )
				{
					if ( GetRestFlag( EGRFlags.No_Factions | EGRFlags.No_Factionless ) )
						SetRestFlag( EGRFlags.No_Factionless, false );
				}
				else
					SetRestFlag( EGRFlags.No_OrderGuild, false );
			}
		}

		public bool No_Minax
		{
			get{ return GetRestFlag( EGRFlags.No_Minax ); }
			set
			{
				SetRestFlag( EGRFlags.No_Minax, value );
				if ( value )
				{
					if ( GetRestFlag( EGRFlags.No_Factions | EGRFlags.No_Factionless ) )
						SetRestFlag( EGRFlags.No_Factionless, false );
				}
				else
					SetRestFlag( EGRFlags.No_ChaosGuild, false );
			}
		}

		public bool No_Shadowlords
		{
			get{ return GetRestFlag( EGRFlags.No_Shadowlords ); }
			set
			{
				SetRestFlag( EGRFlags.No_Shadowlords, value );
				if ( value )
				{
					if ( GetRestFlag( EGRFlags.No_Factions | EGRFlags.No_Factionless ) )
						SetRestFlag( EGRFlags.No_Factionless, false );
				}
				else
					SetRestFlag( EGRFlags.No_ChaosGuild, false );
			}
		}

		public bool No_TrueBrits
		{
			get{ return GetRestFlag( EGRFlags.No_TrueBrits ); }
			set
			{
				SetRestFlag( EGRFlags.No_TrueBrits, value );
				if ( value )
				{
					if ( GetRestFlag( EGRFlags.No_Factions | EGRFlags.No_Factionless ) )
						SetRestFlag( EGRFlags.No_Factionless, false );
				}
				else
					SetRestFlag( EGRFlags.No_OrderGuild, false );
			}
		}

		public bool No_Factionless
		{
			get{ return GetRestFlag( EGRFlags.No_Factionless ); }
			set
			{
				SetRestFlag( EGRFlags.No_Factionless, value );
				if ( value && GetRestFlag( EGRFlags.No_Factions ) )
					SetRestFlag( EGRFlags.No_Factions, false );
			}
		}

		public bool No_MagesGuild
		{
			get{ return GetRestFlag( EGRFlags.No_MagesGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_MagesGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_WarriorsGuild
		{
			get{ return GetRestFlag( EGRFlags.No_WarriorsGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_WarriorsGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_ThievesGuild
		{
			get{ return GetRestFlag( EGRFlags.No_ThievesGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_ThievesGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_RangersGuild
		{
			get{ return GetRestFlag( EGRFlags.No_RangersGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_RangersGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_HealersGuild
		{
			get{ return GetRestFlag( EGRFlags.No_HealersGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_HealersGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_MinersGuild
		{
			get{ return GetRestFlag( EGRFlags.No_MinersGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_MinersGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_MerchantsGuild
		{
			get{ return GetRestFlag( EGRFlags.No_MerchantsGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_MerchantsGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_TinkersGuild
		{
			get{ return GetRestFlag( EGRFlags.No_TinkersGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_TinkersGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_TailorsGuild
		{
			get{ return GetRestFlag( EGRFlags.No_TailorsGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_TailorsGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_FishermensGuild
		{
			get{ return GetRestFlag( EGRFlags.No_FishermensGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_FishermensGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}


		public bool No_BardsGuild
		{
			get{ return GetRestFlag( EGRFlags.No_BardsGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_BardsGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_BlacksmithsGuild
		{
			get{ return GetRestFlag( EGRFlags.No_BlacksmithsGuild ); }
			set
			{
				SetRestFlag( EGRFlags.No_BlacksmithsGuild, value );
				if ( GetRestFlag( EGRFlags.No_NPCGuilds | EGRFlags.No_NPCGuildless ) )
					SetRestFlag( EGRFlags.No_NPCGuildless, false );
			}
		}

		public bool No_NPCGuildless
		{
			get{ return GetRestFlag( EGRFlags.No_NPCGuildless ); }
			set
			{
				SetRestFlag( EGRFlags.No_NPCGuildless, value );
				if ( value && GetRestFlag( EGRFlags.No_NPCGuilds ) )
					SetRestFlag( EGRFlags.No_NPCGuilds, false );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public MountOption Mounted
		{
			get{ return GetRestFlag( EGRFlags.No_OnFoot ) ? MountOption.Mounted : GetRestFlag( EGRFlags.No_Mounted ) ? MountOption.Unmounted : MountOption.Both; }
			set
			{
				switch( value )
				{
					case MountOption.Mounted: SetRestFlag( EGRFlags.No_OnFoot, true ); SetRestFlag( EGRFlags.No_Mounted, false ); break;
					case MountOption.Unmounted: SetRestFlag( EGRFlags.No_OnFoot, false ); SetRestFlag( EGRFlags.No_Mounted, true ); break;
					case MountOption.Both: SetRestFlag( EGRFlags.No_OnFoot | EGRFlags.No_Mounted, false ); break;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DonatorOption Donator
		{
			get{ return GetRestFlag( EGRFlags.No_NonDonators ) ? DonatorOption.Donator : GetRestFlag( EGRFlags.No_Donators ) ? DonatorOption.NonDonator : DonatorOption.Both; }
			set
			{
				switch( value )
				{
					case DonatorOption.Donator: SetRestFlag( EGRFlags.No_NonDonators, true ); SetRestFlag( EGRFlags.No_Donators, false ); break;
					case DonatorOption.NonDonator: SetRestFlag( EGRFlags.No_NonDonators, false ); SetRestFlag( EGRFlags.No_Donators, true ); break;
					case DonatorOption.Both: SetRestFlag( EGRFlags.No_NonDonators | EGRFlags.No_Donators, false ); break;
				}
			}
		}
		#endregion

		#region event gate option flags
		[CommandProperty( AccessLevel.GameMaster )]
		public bool RemoveGameRobe
		{
			get{ return GetOptFlag( EGateOptFlags.RemoveGameRobe ); }
			set{ SetOptFlag( EGateOptFlags.RemoveGameRobe, value ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public MinMaxOption StrReq
		{
			get{ return GetOptFlag( EGateOptFlags.StrMax ) ? MinMaxOption.Max : MinMaxOption.Min; }
			set{ SetOptFlag( EGateOptFlags.StrMax, value == MinMaxOption.Max ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public MinMaxOption IntReq
		{
			get{ return GetOptFlag( EGateOptFlags.IntMax ) ? MinMaxOption.Max : MinMaxOption.Min; }
			set{ SetOptFlag( EGateOptFlags.IntMax, value == MinMaxOption.Max ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public MinMaxOption DexReq
		{
			get{ return GetOptFlag( EGateOptFlags.DexMax ) ? MinMaxOption.Max : MinMaxOption.Min; }
			set{ SetOptFlag( EGateOptFlags.DexMax, value == MinMaxOption.Max ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public MinMaxOption Skill1Req
		{
			get{ return GetOptFlag( EGateOptFlags.Skill1Max ) ? MinMaxOption.Max : MinMaxOption.Min; }
			set{ SetOptFlag( EGateOptFlags.Skill1Max, value == MinMaxOption.Max ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public MinMaxOption Skill2Req
		{
			get{ return GetOptFlag( EGateOptFlags.Skill2Max ) ? MinMaxOption.Max : MinMaxOption.Min; }
			set{ SetOptFlag( EGateOptFlags.Skill2Max, value == MinMaxOption.Max ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool GiveGameRobe{ get{ return GetOptFlag( EGateOptFlags.GiveGameRobe ); } set{ SetOptFlag( EGateOptFlags.GiveGameRobe, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool StaffOnly{ get{ return GetOptFlag( EGateOptFlags.StaffOnly ); } set{ SetOptFlag( EGateOptFlags.StaffOnly, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool FreeForDonators{ get{ return GetOptFlag( EGateOptFlags.FreeForDonators ); } set{ SetOptFlag( EGateOptFlags.FreeForDonators, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HideWhenFull{ get{ return GetOptFlag( EGateOptFlags.HideWhenFull ); } set{ SetOptFlag( EGateOptFlags.HideWhenFull, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool StaffOverride{ get{ return GetOptFlag( EGateOptFlags.StaffOverride ); } set{ SetOptFlag( EGateOptFlags.StaffOverride, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool ResGhosts{ get{ return GetOptFlag( EGateOptFlags.ResGhosts ); } set{ SetOptFlag( EGateOptFlags.ResGhosts, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool CannotFleeFromBattle{ get{ return GetOptFlag( EGateOptFlags.CannotFleeFromBattle ); } set{ SetOptFlag( EGateOptFlags.CannotFleeFromBattle, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RemoveFSL{ get{ return GetOptFlag( EGateOptFlags.RemoveFSL ); } set{ SetOptFlag( EGateOptFlags.RemoveFSL, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool TransportPets{ get{ return GetOptFlag( EGateOptFlags.TransportPets ); } set{ SetOptFlag( EGateOptFlags.TransportPets, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RemovePolymorph{ get{ return GetOptFlag( EGateOptFlags.RemovePolymorph ); } set{ SetOptFlag( EGateOptFlags.RemovePolymorph, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RemoveIncognito{ get{ return GetOptFlag( EGateOptFlags.RemoveIncognito ); } set{ SetOptFlag( EGateOptFlags.RemoveIncognito, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RemoveDisguise{ get{ return GetOptFlag( EGateOptFlags.RemoveDisguise ); } set{ SetOptFlag( EGateOptFlags.RemoveDisguise, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsTeleporter{ get{ return GetOptFlag( EGateOptFlags.IsTeleporter ); } set{ SetOptFlag( EGateOptFlags.IsTeleporter, value ); Visible = !(value || (!Active && HideWhenFull)); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public TollOption TollLocation
		{
			get{ return GetOptFlag( EGateOptFlags.CheckTollBackpack | EGateOptFlags.CheckTollBankBox ) ? TollOption.Both : GetOptFlag( EGateOptFlags.CheckTollBackpack ) ? TollOption.Backpack : TollOption.BankBox; }
			set
			{
				switch( value )
				{
					case TollOption.Backpack: SetOptFlag( EGateOptFlags.CheckTollBackpack, true ); SetOptFlag( EGateOptFlags.CheckTollBankBox, false ); break;
					case TollOption.BankBox: SetOptFlag( EGateOptFlags.CheckTollBackpack, false ); SetOptFlag( EGateOptFlags.CheckTollBankBox, true ); break;
					case TollOption.Both: SetOptFlag( EGateOptFlags.CheckTollBackpack | EGateOptFlags.CheckTollBankBox, true ); break;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active{ get{ return !IsFull && GetOptFlag( EGateOptFlags.Active ); } set{ SetOptFlag( EGateOptFlags.Active, value ); Visible = value && Visible; } }
		#endregion

		#region variables

		public static readonly string[] NpcGuildNames = {"None", "Mages", "Warriors", "Thiefs", "Rangers", "Healers",
													  "Miners", "Merchants", "Tinkers", "Tailors", "Fishermens",
													  "Bards", "Blacksmiths"};

		private EGRFlags m_RestFlags;
		private EGateOptFlags m_OptFlags;

		private int m_Players;
		private int m_MaxPlayers;
		private Type m_TollItem;
		private int m_TollAmount;
		private SkillName m_SkillName1;
		private SkillName m_SkillName2;
		private double m_SkillValue1;
		private double m_SkillValue2;
		private int m_GMsNeeded;
		private int m_StrValue;
		private int m_DexValue;
		private int m_IntValue;
		private string m_Confirmation;
		private int m_GameRobeHue;
		private TimeSpan m_Delay;
		private int m_KarmaThreshold;
		#endregion

		#region properties
		public int KarmaThreshold{ get{ return m_KarmaThreshold; } set{ m_KarmaThreshold = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int StrValue{ get{ return m_StrValue; } set{ m_StrValue = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DexValue{ get{ return m_DexValue; } set{ m_DexValue = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int IntValue{ get{ return m_IntValue; } set{ m_IntValue = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Players{ get{ return m_Players; } set{ m_Players = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxPlayers{ get{ return m_MaxPlayers; } set{ m_MaxPlayers = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Type TollItem{ get{ return m_TollItem; } set{ m_TollItem = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int TollAmount{ get{ return m_TollAmount; } set{ m_TollAmount = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public SkillName SkillName1{ get{ return m_SkillName1; } set{ m_SkillName1 = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public SkillName SkillName2{ get{ return m_SkillName2; } set{ m_SkillName2 = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public double SkillValue1{ get{ return m_SkillValue1; } set{ m_SkillValue1 = value < 0.0 ? 0.0 : value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public double SkillValue2{ get{ return m_SkillValue2; } set{ m_SkillValue2 = value < 0.0 ? 0.0 : value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int GMSkillsNeeded{ get{ return m_GMsNeeded; } set{ m_GMsNeeded = Math.Max( value, 0 ); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string Confirmation{ get{ return m_Confirmation; } set{ m_Confirmation = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int GameRobeHue{ get{ return m_GameRobeHue; } set{ m_GameRobeHue = Math.Max( value, 0 ); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan Delay{ get{ return m_Delay; } set{ m_Delay = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsFull{ get{ return !IsTeleporter && ( m_Players >= m_MaxPlayers && m_MaxPlayers > 0 ); } }
		#endregion

		#region flag functions
		public bool GetRestFlag( EGRFlags flag )
		{
			return ( m_RestFlags & flag ) == flag;
		}

		public void SetRestFlag( EGRFlags flag, bool value )
		{
			if ( value )
				m_RestFlags |= flag;
			else
				m_RestFlags &= ~flag;
		}

		public bool GetOptFlag( EGateOptFlags flag )
		{
			return ( m_OptFlags & flag ) == flag;
		}

		public void SetOptFlag( EGateOptFlags flag, bool value )
		{
			if ( value )
				m_OptFlags |= flag;
			else
				m_OptFlags &= ~flag;
		}
		#endregion

		#region constructors
		[Constructable]
		public ExtEventMoongate() : this( Point3D.Zero, Map.Internal )
		{
		}

		[Constructable]
		public ExtEventMoongate( Point3D target, Map targetMap )
		{
			Name = "Event Moongate";
			Hue = 315;
			Target = target;
			TargetMap = targetMap;
			Light = LightType.Circle300;
			Movable = Dispellable = false;

			m_SkillName1 = m_SkillName2 = SkillName.Alchemy;
			m_Confirmation = String.Empty;

			m_Delay = TimeSpan.FromSeconds( 1.0 );

			m_RestFlags = (EGRFlags)0x0; //Restrict nothing
			m_OptFlags = (EGateOptFlags)0xBF711;
			/*
				Active | RemoveGameRobe | HideWhenFull | StaffOverride
				CannotFleeFromBattle | RemoveFSL | TransportPets
				RemovePolymorph | RemoveIncognito | RemoveDisguise | CheckTollBackpack
				CheckTollBankBox
			 */
		}

		public ExtEventMoongate( Serial serial ) : base( serial )
		{
		}
		#endregion

		#region function overrides
		public override void OnDoubleClick( Mobile from )
		{
			if ( Active && ( Visible || from.AccessLevel >= AccessLevel.GameMaster || IsTeleporter ) )
				base.OnDoubleClick( from );
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( Active && ( Visible || m.AccessLevel >= AccessLevel.GameMaster || IsTeleporter ) )
				CheckGate( m, 1 );
			return true;
		}

		public override void CheckGate( Mobile m, int range )
		{
			if ( IsFull && HideWhenFull )
				Visible = false;

			new DelayTimer( m, this, range ).Start();
		}

		public override bool ValidateUse( Mobile m, bool message )
		{
			if ( !base.ValidateUse( m, message ) || !ValidPlayer( m ) )
				return false;

			bool player = m.AccessLevel == AccessLevel.Player;

			if ( player && !Active )
				return false;

			if ( !player && StaffOverride )
				return true;

			PlayerMobile pm = (PlayerMobile)m;
			if ( RemoveGameRobe || GiveGameRobe )
				RemoveRobe( pm ); //Remove the robe before we check nakedness
			//bool donator = pm.HasDonated;
			bool naked = !m.HasItemsNoBankBox();

			if ( player && StaffOnly )
				m.SendMessage("Players are not allowed to enter this gate.");
			else if ( m.HasTrade )
				m.SendMessage("You cannot enter this gate with a trade pending." );
			else if ( CannotFleeFromBattle && ( SpellHelper.CheckCombat( m ) || CheckCombat( m ) ) )
				m.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
			else if ( GetRestFlag( EGRFlags.No_Ghosts ) && !m.Alive )
				m.SendMessage( "This moongate does not allow the dead to pass." );
			else if ( GetRestFlag( EGRFlags.No_Living ) && m.Alive )
				m.SendMessage( "This moongate does not allow the living to pass." );
			else if ( GetRestFlag( EGRFlags.No_Mounted ) && m.Mounted )
				m.SendMessage( "You must be unmounted to enter this gate.." );
			else if ( GetRestFlag( EGRFlags.No_OnFoot ) && !m.Mounted )
				m.SendMessage( "You must be mounted to enter this gate." );
			else	if ( m.Spell != null )
				m.SendLocalizedMessage( 1049616 );
			else if ( Server.Factions.Sigil.ExistsOn( m ) )
				m.SendLocalizedMessage( 1061632 );
			//else if ( ( GetRestFlag( EGRFlags.No_Donators ) && donator ) || ( GetRestFlag( EGRFlags.No_NonDonators ) && !donator ) )
			//	m.SendMessage( "This moongate does not allow your donation status to pass." );
			else if ( ( No_Murderer && m.Kills > 5 ) || ( No_BlueKarma && m.Karma > m_KarmaThreshold ) || ( No_GrayKarma && m.Karma <= m_KarmaThreshold ) || ( No_Criminal && m.Criminal ) )
				m.SendMessage( "This moongate does not allows your notoriety to pass." );
			else if ( ( GetRestFlag( EGRFlags.No_Female ) && m.Female ) || ( GetRestFlag( EGRFlags.No_Male ) && !m.Female ) )
				m.SendMessage( "This moongate does not allow your gender to pass." );
			else if ( ( GetRestFlag( EGRFlags.No_Young ) && pm.Young ) || ( GetRestFlag( EGRFlags.No_Veteran ) && !(pm.Young || pm.Companion) ) )
				m.SendMessage("This moongate does not allow your age status to pass.");
			else if ( InvalidNPCGuild( pm ) )
			{ //message in function
			}
			else if ( InvalidGuild( m ) )
			{ //message in function
			}
			else if ( InvalidFaction( m ) )
			{ //message in function
			}
			else if ( InvalidSkill( m ) )
			{ //message in function
			}
			else if ( MissingGMSkills( m ) )
				m.SendMessage( "You must have a minimum of {0} GM skills to enter this gate.", m_GMsNeeded );
			else if ( InvalidStats( m ) )
			{ //message in function
			}
			else if ( ( GetRestFlag( EGRFlags.No_Naked ) && naked ) || ( GetRestFlag( EGRFlags.No_Clothed ) && !naked ) )
				m.SendMessage( "You must be {0} to enter this gate.", naked ? "clothed" : "naked" );
			else
				return true;

			return false;
		}

		public override void BeginConfirmation( Mobile m )
		{
			if ( IsTeleporter || String.IsNullOrEmpty( m_Confirmation ) )
				EndConfirmation( m );
			else
			{
				m.CloseGump( typeof(EGateConfirmGump) );
				m.SendGump( new EGateConfirmGump( m, this ) );
			}
		}

		//Assume all checks are already done, and m is a playermobile (crash otherwise)
		public override void UseGate( Mobile m )
		{
			PlayerMobile pm = m as PlayerMobile;

			Map map = TargetMap;

			if ( map == null || map == Map.Internal )
				map = m.Map;

			Point3D p = Target;

			if ( p == Point3D.Zero )
				p = Location;

			if ( m.AccessLevel == AccessLevel.Player || !StaffOverride )
			{
				if ( /*( FreeForDonators && pm.HasDonated ) ||*/ m_TollItem == null || m_TollAmount <= 0 || ( GetOptFlag( EGateOptFlags.CheckTollBackpack ) && m.Backpack.ConsumeTotal( m_TollItem, m_TollAmount, true ) ) || ( GetOptFlag( EGateOptFlags.CheckTollBankBox ) && m.BankBox.ConsumeTotal( m_TollItem, m_TollAmount, true ) ) )
				{
					if ( GiveGameRobe )
						m.EquipItem( new GameRobe( m_GameRobeHue ) );

					if ( RemovePolymorph )
					{
						PolymorphSpell.StopTimer( m );
                        if (!m.CanBeginAction(typeof(PolymorphSpell)))
                        {
                            m.BodyMod = 0;
                            m.HueMod = -1;
                            m.EndAction(typeof(PolymorphSpell));

                            BaseArmor.ValidateMobile(m);
                            BaseClothing.ValidateMobile(m);
                        }
					}

					if ( RemoveIncognito )
					{
						IncognitoSpell.StopTimer( m );
                        if (!m.CanBeginAction(typeof(IncognitoSpell)))
                        {
                            if (pm != null)
                                pm.SetHairMods(-1, -1);

                            m.BodyMod = 0;
                            m.HueMod = -1;
                            m.NameMod = null;
                            m.EndAction(typeof(IncognitoSpell));

                            BaseArmor.ValidateMobile(m);
                            BaseClothing.ValidateMobile(m);
                        }
					}

                    if (RemoveDisguise)
                    {
                        m.NameMod = null;
                        if (pm != null)
                        {
                            pm.SetHairMods(-1, -1);
                        }
                        DisguiseTimers.RemoveTimer(m);
                    }

					if ( !pm.Alive && ResGhosts )
						pm.Resurrect();

					if ( RemoveFSL && pm != null )
						Faction.ClearSkillLoss( pm );
				}
				else
				{
					m.SendMessage( "You must have the required toll in your bank to enter this gate." );
					return;
				}
			}

			if ( TransportPets )
				BaseCreature.TeleportPets( pm, p, map );

			pm.MoveToWorld( p, map );

			pm.PlaySound( 0x1FE );

			OnGateUsed( m );
		}

		public override void OnGateUsed( Mobile m )
		{
			if ( !IsTeleporter && m.AccessLevel == AccessLevel.Player )
				m_Players++;

			if ( IsFull && HideWhenFull )
				Visible = false;
		}
		#endregion

		#region new functions
		private static TimeSpan CombatHeatDelay = TimeSpan.FromSeconds( 30.0 );
		public static bool CheckCombat( Mobile m )
		{
			for ( int i = 0; i < m.Aggressors.Count; ++i )
			{
				AggressorInfo info = (AggressorInfo)m.Aggressors[i];

				if ( info.Attacker.Player && ( DateTime.UtcNow - info.LastCombatTime ) < CombatHeatDelay )
					return true;
			}

			return false;
		}
/*
		public bool NakedPlayer( Mobile m )
		{
			if ( m.Holding != null )
				return false;

			for ( int i = 1; i < 29; i++ ) //there are more layers, check those?
			{
				if ( i == 9 || i == 15 || i == 24 || i == 11 || i == 16 || i == 25 )
					continue;

				Item item = m.FindItemOnLayer( (Layer)i );

				//Deathrobe check?
				if ( item == null || ( i == 22 && !item.Deleted && item.ItemID == 0x204E ) )
					continue;

				Container cnt = item as Container;
				if ( cnt == null || cnt.Items.Count > 0 )
					return false;
			}
			return true;
		}
*/
		public bool ValidPlayer( Mobile m )
		{
			return m is PlayerMobile && !m.Deleted && m.Account != null;
		}

		public bool InvalidNPCGuild( PlayerMobile pm )
		{
			if ( pm != null )
			{
				int guild = (int)pm.NpcGuild;
				if ( guild >= 0 && guild < NpcGuildNames.Length )
				{
					if ( GetRestFlag( (EGRFlags)(1ul<<(25+guild)) ) )
					{
						string must = "must not";
						string an = "an";
						if ( guild > 0 )
						{
							must = "must";
							an = NpcGuildNames[guild];
						}

						pm.SendMessage( "You {0} be a member of {1} NPC guild to enter this gate.", must, an );
					}
					else
						return false;
				}
				return true;
			}

			return false;
		}

		public bool InvalidGuild( Mobile m )
		{
			Guild g = m.Guild as Guild;

			if ( g == null )
			{
				if ( No_Guildless )
					m.SendMessage( "You must be in a guild to enter this gate." );
				else
					return false;
			}
			else
			{
				if ( GetRestFlag( EGRFlags.No_Guilds ) )
					m.SendMessage( "This moongate does not allow guild members to pass." );
				else if ( ( No_ChaosGuild && g.Type == GuildType.Chaos ) || ( No_OrderGuild && g.Type == GuildType.Order ) || ( No_RegGuild && g.Type == GuildType.Regular ) )
					m.SendMessage( "This moongate does not allow your guild type to pass." );
				else
					return false;
			}

			return true;
		}

		public bool InvalidFaction( Mobile m )
		{
			PlayerState ps = PlayerState.Find( m );
			if ( ps == null )
			{
				if ( No_Factionless )
					m.SendMessage( "You must be in a faction to enter this gate." );
				else
					return false;
			}
			else
			{
				Faction fact = ps.Faction;

				if ( GetRestFlag( EGRFlags.No_Factions ) )
					m.SendMessage( "This moongate does not allow faction members to pass." );
				else if ( ( No_COM && fact is CouncilOfMages ) || ( No_Minax && fact is Minax ) || ( No_Shadowlords && fact is Shadowlords ) || ( No_TrueBrits && fact is TrueBritannians ) )
					m.SendMessage( "This moongate does not allow your faction to pass." );
				else
					return false;
			}

			return true;
		}

		public bool InvalidSkill( Mobile m )
		{
			return CheckSkill( m, m_SkillName1, m_SkillValue1, EGateOptFlags.Skill1Max ) || CheckSkill( m, m_SkillName2, m_SkillValue2, EGateOptFlags.Skill2Max );
		}

		//Base or Value?
		public bool CheckSkill( Mobile m, SkillName name, double value, EGateOptFlags flag )
		{
			double skill = m.Skills[name].Base;
			bool checkmax = GetOptFlag( flag );
			bool ret = checkmax ? ( skill > value ) : ( skill < value );

			if ( ret )
				m.SendMessage( "You must have {0} {1} {2} to enter this gate.", checkmax ? "no more than" : "at least" , value, m.Skills[name].Name );
			return ret;
		}


		public bool MissingGMSkills( Mobile m )
		{
			int length = m.Skills.Length;
			for ( int i = 0, c = 0; i < length; i++ )
			{
				if ( c == m_GMsNeeded )
					return false;

				Skill skill = m.Skills[i];
				if ( skill.Base == skill.Cap )
					c++;
			}

			return true;
		}

		public bool InvalidStats( Mobile m )
		{
			return CheckStat( m, m.RawStr, m_StrValue, EGateOptFlags.StrMax, "strength" ) || CheckStat( m, m.RawInt, m_IntValue, EGateOptFlags.IntMax, "intelligence" ) || CheckStat( m, m.RawDex, m_DexValue, EGateOptFlags.DexMax, "dexterity" );
		}

		public bool CheckStat( Mobile m, int value, int statval, EGateOptFlags flag, string stat )
		{
			bool checkmax = GetOptFlag( flag );
			bool ret = checkmax ? ( value >= statval ) : ( value <= statval );

			if ( ret )
				m.SendMessage( "You must have a {0} of {1} {2} to enter this gate.", checkmax ? "maximum" : "minimum" , statval, stat );
			return ret;
		}

		public void RemoveRobe( PlayerMobile pm )
		{
			if ( pm != null )
			{
				GameRobe gr = pm.FindItemOnLayer( Layer.OuterTorso ) as GameRobe;
				if ( gr != null )
					gr.Delete();
				if ( pm.Backpack != null )
				{
					Item[] items = pm.Backpack.FindItemsByType( typeof(GameRobe) );
					for ( int i = items.Length-1; i > 0; i-- )
						items[i].Delete();
				}
			}
		}
		#endregion

		#region serialization
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 7 ); // version

			PSerial.WriteEncoded( writer, (long)m_RestFlags );
			writer.WriteEncodedInt( (int)m_OptFlags );
			writer.Write( m_MaxPlayers );
			Reflector.Serialize( writer, m_TollItem );
			writer.Write( m_TollAmount );
			writer.Write( (int)m_SkillName1 );
			writer.Write( (int)m_SkillName2 );
			writer.Write( m_SkillValue1 );
			writer.Write( m_SkillValue2 );
			writer.Write( m_GMsNeeded );
			writer.Write( m_StrValue );
			writer.Write( m_IntValue );
			writer.Write( m_DexValue );
			writer.Write( m_Confirmation );
			writer.Write( m_GameRobeHue );
			writer.Write( m_Delay );
			writer.Write( m_KarmaThreshold );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch( version )
			{
				case 7:
				{
					m_RestFlags = (EGRFlags)PSerial.ReadEncodedLong( reader );
					m_OptFlags = (EGateOptFlags)reader.ReadEncodedInt();
					m_MaxPlayers = reader.ReadInt();
					m_TollItem = Reflector.Deserialize( reader );
					m_TollAmount = reader.ReadInt();
					m_SkillName1 = (SkillName)reader.ReadInt();
					m_SkillName2 = (SkillName)reader.ReadInt();
					m_SkillValue1 = reader.ReadDouble();
					m_SkillValue2 = reader.ReadDouble();
					m_GMsNeeded = reader.ReadInt();
					m_StrValue = reader.ReadInt();
					m_IntValue = reader.ReadInt();
					m_DexValue = reader.ReadInt();
					m_Confirmation = reader.ReadString();
					m_GameRobeHue = reader.ReadInt();
					m_Delay = reader.ReadTimeSpan();
					m_KarmaThreshold = reader.ReadInt();
					break;
				}
				case 6:
				{
					switch ( reader.ReadInt() )
					{
						case 0: TollLocation = TollOption.Backpack; break;
						case 1: TollLocation = TollOption.BankBox; break;
						case 2: TollLocation = TollOption.Both; break;
					}

					goto case 5;
				}
				case 5:
				{
					CannotFleeFromBattle = reader.ReadBool();
					goto case 4;
				}
				case 4:
				{
					m_GameRobeHue = reader.ReadInt();
					GiveGameRobe = !reader.ReadBool();
					goto case 3;
				}
				case 3:
				{
					StaffOverride = !reader.ReadBool();
					m_Confirmation = reader.ReadString();
					StaffOnly = reader.ReadBool();

					switch( reader.ReadInt() ) //Sex-Select
					{
						case 1: Gender = GenderOption.Male; break;
						case 2: Gender = GenderOption.Female; break;
					}

					int npcguild = reader.ReadInt();
					if ( npcguild > 0 )
					{
						SetRestFlag( EGRFlags.No_NPCGuilds, true );
						SetRestFlag( (EGRFlags)(1ul<<(25+npcguild)), false );
					}

					switch( reader.ReadInt() ) //Faction-Select
					{
						case 1: SetRestFlag( EGRFlags.No_Factions, true ); break;
						case 2: SetRestFlag( EGRFlags.No_Factionless, true ); break;
						case 3: SetRestFlag( EGRFlags.No_Minax | EGRFlags.No_Shadowlords | EGRFlags.No_TrueBrits, true ); break;
						case 4: SetRestFlag( EGRFlags.No_COM | EGRFlags.No_Shadowlords | EGRFlags.No_TrueBrits, true ); break;
						case 5: SetRestFlag( EGRFlags.No_COM | EGRFlags.No_Minax | EGRFlags.No_TrueBrits, true ); break;
						case 6: SetRestFlag( EGRFlags.No_COM | EGRFlags.No_Minax | EGRFlags.No_Shadowlords, true ); break;
					}

					switch( reader.ReadInt() ) //ChaosOrder-Select
					{
						case 1: SetRestFlag( EGRFlags.No_ChaosGuild | EGRFlags.No_OrderGuild, true ); break;
						case 2: SetRestFlag( EGRFlags.No_RegGuild | EGRFlags.No_Guildless, true ); break;
						case 3: SetRestFlag( EGRFlags.No_OrderGuild | EGRFlags.No_RegGuild | EGRFlags.No_Guildless, true ); break;
						case 4: SetRestFlag( EGRFlags.No_ChaosGuild | EGRFlags.No_RegGuild | EGRFlags.No_Guildless, true ); break;
					}
					goto case 2;
				}
				case 2:
				{
					switch( reader.ReadInt() ) //Young-Select
					{
						case 1: Veterans = VetOption.Veteran; break;
						case 2: Veterans = VetOption.Young; break;
					}

					switch( reader.ReadInt() ) //Factioners-Select
					{
						case 1: SetRestFlag( EGRFlags.No_Factions, true ); break;
						case 2: No_Factionless = true; break;
					}

					goto case 1;
				}
				case 1:
				{
					int stat = reader.ReadInt();

					int amount = (int)reader.ReadDouble();

					switch( stat )
					{
						case 1: m_StrValue = amount; break;
						case 2: m_IntValue = amount; break;
						case 3: m_DexValue = amount; break;
					}

					reader.ReadString(); //obselete
					switch( reader.ReadInt() )
					{
						case 1: SetRestFlag( EGRFlags.No_Ghosts, true ); break;
						case 2: SetRestFlag( EGRFlags.No_Living, true ); break;
					}

					ResGhosts = reader.ReadBool();
					RemoveFSL = reader.ReadBool();
					SetOptFlag( EGateOptFlags.StrMax | EGateOptFlags.IntMax | EGateOptFlags.DexMax, reader.ReadInt() == 0 );
					reader.ReadInt(); //obselete
					m_MaxPlayers = reader.ReadInt();
					HideWhenFull = reader.ReadBool();
					switch( reader.ReadInt() )
					{
						case 1: Mounted = MountOption.Unmounted; break;
						case 2: Mounted = MountOption.Mounted; break;
					}
					TransportPets = reader.ReadBool();
					SetRestFlag( EGRFlags.No_Clothed, reader.ReadBool() );
					SetRestFlag( EGRFlags.No_NonDonators, reader.ReadBool() );
					switch( reader.ReadInt() )
					{
						case 1: No_Murderer = true; break;
						case 2: No_BlueKarma = No_GrayKarma = No_Criminal = true; break;
					}
					m_TollItem = ScriptCompiler.FindTypeByFullName( reader.ReadString() );
					m_TollAmount = reader.ReadInt();
					FreeForDonators = reader.ReadBool();
					m_SkillName1 = (SkillName)reader.ReadInt();
					m_SkillValue1 = reader.ReadDouble();
					reader.ReadString(); //obselete
					m_GMsNeeded = reader.ReadInt();
					break;
				}
			}
		}
		#endregion

		private class DelayTimer : Timer
		{
			private Mobile m_From;
			private ExtEventMoongate m_Gate;
			private int m_Range;

			public DelayTimer( Mobile from, ExtEventMoongate gate, int range ) : base( gate.Delay )
			{
				m_From = from;
				m_Gate = gate;
				m_Range = range;
			}

			protected override void OnTick()
			{
				m_Gate.DelayCallback( m_From, m_Range );
			}
		}
	}

	public class GameRobe : Robe
	{
		public GameRobe( int hue ) : base( hue )
		{
			Name = "Game Robe";
			Weight = 0;
			Movable = false;
			LootType = LootType.Blessed;
		}

		public GameRobe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}

namespace Server.Gumps
{
	public class EGateConfirmGump : Gump
	{
		private Mobile m_From;
		private ExtEventMoongate m_Gate;

		public EGateConfirmGump( Mobile from, ExtEventMoongate gate ) : base( 20, 30 )
		{
			m_From = from;
			m_Gate = gate;

			AddPage(0);

			AddBackground(0, 0, 275, 240, 5054);
			AddBackground(10, 10, 255, 220, 3000);

			AddLabel(30, 20, 909, "You entered a magical moongate");

			AddHtml(20, 60, 250, 100, String.Format( "<BASEFONT COLOR=#2F4F4F>{0}</BASEFONT>", m_Gate.Confirmation ), false, false);

			AddLabel(20, 165, 909, "Are you willing to proceed?");

			AddLabel(85, 202, 909, "Yes");
			AddButton(50, 200, 4005, 4007, 1, GumpButtonType.Reply, 0);

			AddLabel(205, 202, 909, "No");
			AddButton(170, 200, 4005, 4007, 0, GumpButtonType.Reply, 0);

		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( m_Gate != null && info.ButtonID > 0 )
				m_Gate.EndConfirmation( m_From );
		}
	}
}