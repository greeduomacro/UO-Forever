using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Commands
{
	public class AccessLevelMod
	{
		private AccessLevel m_Level;
		public AccessLevel Level{ get{ return m_Level; } set{ m_Level = value; } }

		public AccessLevelMod( AccessLevel level )
		{
			m_Level = level;
		}
	}

	public class AccessLevelToggler
	{
		public static Dictionary<PlayerMobile, AccessLevelMod> m_Mobiles = new Dictionary<PlayerMobile, AccessLevelMod>();

		public static AccessLevelMod GetMod( PlayerMobile from )
		{
			AccessLevelMod mod = null;
			m_Mobiles.TryGetValue( from, out mod );

			return mod;
		}

		public static AccessLevel GetRawAccessLevel( PlayerMobile from )
		{
			AccessLevelMod mod = null;
			m_Mobiles.TryGetValue( from, out mod );

			if ( mod != null )
				return mod.Level;
			else
				return from.AccessLevel;
		}

		public static void Initialize()
		{
			CommandSystem.Register( "Staff", AccessLevel.Player, new CommandEventHandler( Staff_OnCommand ) );
		}

		[Usage( "Staff [<AccessLevel>]" )]
		[Description( "Switches a staff member between their own access levels" )]
		public static void Staff_OnCommand( CommandEventArgs e )
		{
            if (!(e.Mobile is PlayerMobile)) return;
            PlayerMobile from = (PlayerMobile)e.Mobile;

			AccessLevelMod mod = GetMod( from );

			if ( mod != null )
			{
				if ( e.Length == 0 )
				{
					from.AccessLevel = mod.Level;
					m_Mobiles.Remove( from );
					from.SetCustomFlag( CustomPlayerFlag.StaffLevel, false );
				}
				else
				{
					AccessLevel level;
					if ( !ArgumentToAccessLevel( e.Arguments[0], out level ) )
						from.SendMessage( "Invalid AccessLevel: " + e.Arguments[0] );
					else
					{
						if ( mod.Level < level )
							from.SendMessage( "Invalid AccessLevel: " + e.Arguments[0] );
						else
						{
							if ( mod.Level == level )
							{
								m_Mobiles.Remove( from );
								from.SetCustomFlag( CustomPlayerFlag.StaffLevel, false );
							}
							from.AccessLevel = level;
						}
					}
				}
			}
			else if ( from.AccessLevel == AccessLevel.Player )
				from.Say( e.ArgString );
			else
			{
				if ( e.Length == 0 )
				{
					m_Mobiles.Add( from, new AccessLevelMod( from.AccessLevel ) );
					from.SetCustomFlag( CustomPlayerFlag.StaffLevel, true );
					from.AccessLevel = AccessLevel.Player;
				}
				else
				{
					AccessLevel level;
					if ( !ArgumentToAccessLevel( e.Arguments[0], out level ) )
						from.SendMessage( "Invalid AccessLevel: " + e.Arguments[0] );
					else
					{
						if ( from.AccessLevel <= level )
							from.SendMessage( "Invalid AccessLevel: " + e.Arguments[0] );
						else
						{
							m_Mobiles.Add( from, new AccessLevelMod( from.AccessLevel ) );
							from.SetCustomFlag( CustomPlayerFlag.StaffLevel, true );
							from.AccessLevel = level;
						}
					}
				}
			}
		}

		public static bool ArgumentToAccessLevel( string argument, out AccessLevel accesslevel )
		{
			switch ( argument.ToLower() )
			{
				case "player": case "0":
					accesslevel = AccessLevel.Player;
					return true;
//				case "trial": case "1":
//					accesslevel = AccessLevel.Trial;
//					return true;
//				case "counselor": case "2":
//					accesslevel = AccessLevel.Counselor;
//					return true;
//				case "senior counselor": case "3":
//					accesslevel = AccessLevel.SeniorCounselor;
//					return true;
				case "seer": case "3":
					accesslevel = AccessLevel.Seer;
					return true;
				case "event master": case "em": case "5":
					accesslevel = AccessLevel.EventMaster;
					return true;
				case "game master": case "gm": case "4":
					accesslevel = AccessLevel.GameMaster;
					return true;
				case "lead": case "lead gm": case "lead game master":
				case "lead seer": case "lead em": case "lead event master": case "6":
					accesslevel = AccessLevel.Lead;
					return true;
				case "administrator": case "admin": case "7":
					accesslevel = AccessLevel.Administrator;
					return true;
				case "developer": case "dev": case "8":
					accesslevel = AccessLevel.Developer;
					return true;
				case "owner": case "founder": case "9":
					accesslevel = AccessLevel.Owner;
					return true;
				default:
					accesslevel = AccessLevel.Player;
					return false;
			}
		}
	}
}