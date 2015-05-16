using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Guilds;
using Server.Multis;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Factions;
using Server.Spells;
using Server.Ethics;
using Server.Regions;
using Server.Engines.XmlSpawner2;
using Server.Scripts.New.Adam;
using VitaNex.Modules.AutoPvP;

namespace Server.Misc
{
	public class NotorietyHandlers
	{
		public static void Initialize()
		{
			Notoriety.Hues[Notoriety.Innocent]		= 0x59;
			Notoriety.Hues[Notoriety.Ally]			= 0x3F;
			Notoriety.Hues[Notoriety.CanBeAttacked]	= 0x3B2;
			Notoriety.Hues[Notoriety.Criminal]		= 0x3B2;
			Notoriety.Hues[Notoriety.Enemy]			= 0x90;
			Notoriety.Hues[Notoriety.Murderer]		= 0x22;
			Notoriety.Hues[Notoriety.Invulnerable]	= 0x35;

			Notoriety.Handler = new NotorietyHandler( MobileNotoriety );

			Mobile.AllowBeneficialHandler = new AllowBeneficialHandler( Mobile_AllowBeneficial );
			Mobile.AllowHarmfulHandler = new AllowHarmfulHandler( Mobile_AllowHarmful );
		}

		private enum GuildStatus { None, Peaceful, Waring }

		private static GuildStatus GetGuildStatus( Mobile m )
		{
			if( m.Guild == null )
				return GuildStatus.None;
			else if( ((Guild)m.Guild).Enemies.Count == 0 && m.Guild.Type == GuildType.Regular )
				return GuildStatus.Peaceful;

			return GuildStatus.Waring;
		}

		private static bool CheckBeneficialStatus( GuildStatus from, GuildStatus target )
		{
			if( from == GuildStatus.Waring || target == GuildStatus.Waring )
				return false;

			return true;
		}

		/*private static bool CheckHarmfulStatus( GuildStatus from, GuildStatus target )
		{
			if ( from == GuildStatus.Waring && target == GuildStatus.Waring )
				return true;

			return false;
		}*/

		public static bool Mobile_AllowBeneficial( Mobile from, Mobile target )
		{
			if( from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player )
				return true;

			#region Dueling
			PlayerMobile pmFrom = from as PlayerMobile;
			PlayerMobile pmTarg = target as PlayerMobile;

			if( pmFrom == null && from is BaseCreature )
			{
				BaseCreature bcFrom = (BaseCreature)from;

				if( bcFrom.Summoned )
					pmFrom = bcFrom.SummonMaster as PlayerMobile;
			}

			if( pmTarg == null && target is BaseCreature )
			{
				BaseCreature bcTarg = (BaseCreature)target;

				if( bcTarg.Summoned )
					pmTarg = bcTarg.SummonMaster as PlayerMobile;
			}

			if( pmFrom!= null && pmTarg != null )
			{
				if( pmFrom.DuelContext != pmTarg.DuelContext && ((pmFrom.DuelContext != null && pmFrom.DuelContext.Started) || (pmTarg.DuelContext != null && pmTarg.DuelContext.Started)) )
					return false;

				if( pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && ((pmFrom.DuelContext.StartedReadyCountdown && !pmFrom.DuelContext.Started) || pmFrom.DuelContext.Tied || pmFrom.DuelPlayer.Eliminated || pmTarg.DuelPlayer.Eliminated) )
					return false;

				if( pmFrom.DuelPlayer != null && !pmFrom.DuelPlayer.Eliminated && pmFrom.DuelContext != null && pmFrom.DuelContext.IsSuddenDeath )
					return false;

				if( pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.m_Tournament != null && pmFrom.DuelContext.m_Tournament.IsNotoRestricted && pmFrom.DuelPlayer != null && pmTarg.DuelPlayer != null && pmFrom.DuelPlayer.Participant != pmTarg.DuelPlayer.Participant )
					return false;

				if( pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.Started )
					return true;
			}

			if( (pmFrom != null && pmFrom.DuelContext != null && pmFrom.DuelContext.Started) || (pmTarg != null && pmTarg.DuelContext != null && pmTarg.DuelContext.Started) )
				return false;

			Engines.ConPVP.SafeZone sz = from.Region.GetRegion( typeof( Engines.ConPVP.SafeZone ) ) as Engines.ConPVP.SafeZone;

			if( sz != null /*&& sz.IsDisabled()*/ )
				return false;

			sz = target.Region.GetRegion( typeof( Engines.ConPVP.SafeZone ) ) as Engines.ConPVP.SafeZone;

			if( sz != null /*&& sz.IsDisabled()*/ )
				return false;
			#endregion

			Map map = from.Map;

            // Alan mod: custom teams
            if (from.CustomTeam && target.CustomTeam)
            {
                List<XmlTeam> targetTeams = XmlAttach.GetTeams(target);
                List<XmlTeam> fromTeams = XmlAttach.GetTeams(from);

                if (!XmlTeam.SameTeam(fromTeams, targetTeams))
                {
                    return XmlTeam.AllowHealEnemy(fromTeams, targetTeams);
                }
                // they are on the same team, allow beneficial
                return true;
            }
            // end Alan mod

			#region Factions
			Faction targetFaction = Faction.Find( target, true );

			if( (!target.EraML || Faction.IsFactionFacet( map )) && targetFaction != null )
			{
                if (Faction.Find(from, true) != targetFaction && pmFrom != null && pmTarg != null && !AutoPvP.IsParticipant(pmTarg))
					return false;
			}
			#endregion

			if( map != null && (map.Rules & MapRules.BeneficialRestrictions) == 0 )
				return true; // In felucca, anything goes

			if( !from.Player )
				return true; // NPCs have no restrictions

			if( target is BaseCreature && !((BaseCreature)target).Controlled )
				return false; // Players cannot heal uncontrolled mobiles

			if( from is PlayerMobile && ((PlayerMobile)from).Young && (!(target is PlayerMobile) || !((PlayerMobile)target).Young) )
				return false; // Young players cannot perform beneficial actions towards older players

			Guild fromGuild = from.Guild as Guild;
			Guild targetGuild = target.Guild as Guild;

			if( fromGuild != null && targetGuild != null && (targetGuild == fromGuild || fromGuild.IsAlly( targetGuild )) )
				return true; // Guild members can be beneficial

			return CheckBeneficialStatus( GetGuildStatus( from ), GetGuildStatus( target ) );
		}

		public static bool Mobile_AllowHarmful( Mobile from, Mobile target )
		{
            if (from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player)
                return true;

            #region Dueling
            PlayerMobile pmFrom = from as PlayerMobile;
            PlayerMobile pmTarg = target as PlayerMobile;
            BaseCreature bcFrom = from as BaseCreature;

            if (pmFrom == null && bcFrom != null)
            {
                if (bcFrom.Summoned)
                    pmFrom = bcFrom.SummonMaster as PlayerMobile;
            }

            if (pmFrom != null && pmFrom.Kills >= 5 && !MurderSystemController._RedAllowHarmfulToBluesInTown)
            {
                if (target != null && target.Region != null)
                {
                    GuardedRegion region = (GuardedRegion)target.Region.GetRegion(typeof(GuardedRegion));
                    if (region != null && !region.Disabled)
                    {
                        if (Notoriety.Compute(from, target) == Notoriety.Innocent)
                        {
                            return false;
                        }
                    }
                }
            }

            if (pmTarg == null && target is BaseCreature)
            {
                BaseCreature bcTarg = (BaseCreature)target;

                if (bcTarg.Summoned)
                    pmTarg = bcTarg.SummonMaster as PlayerMobile;
            }

            if (pmFrom != null && pmTarg != null)
            {
                if (pmFrom.DuelContext != pmTarg.DuelContext && ((pmFrom.DuelContext != null && pmFrom.DuelContext.Started) || (pmTarg.DuelContext != null && pmTarg.DuelContext.Started)))
                    return false;

                if (pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && ((pmFrom.DuelContext.StartedReadyCountdown && !pmFrom.DuelContext.Started) || pmFrom.DuelContext.Tied || pmFrom.DuelPlayer.Eliminated || pmTarg.DuelPlayer.Eliminated))
                    return false;

                if (pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.m_Tournament != null && pmFrom.DuelContext.m_Tournament.IsNotoRestricted && pmFrom.DuelPlayer != null && pmTarg.DuelPlayer != null && pmFrom.DuelPlayer.Participant == pmTarg.DuelPlayer.Participant)
                    return false;

                if (pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.Started)
                    return true;
            }

            if ((pmFrom != null && pmFrom.DuelContext != null && pmFrom.DuelContext.Started) || (pmTarg != null && pmTarg.DuelContext != null && pmTarg.DuelContext.Started))
                return false;

            Engines.ConPVP.SafeZone sz = from.Region.GetRegion(typeof(Engines.ConPVP.SafeZone)) as Engines.ConPVP.SafeZone;

            if (sz != null /*&& sz.IsDisabled()*/ )
                return false;

            sz = target.Region.GetRegion(typeof(Engines.ConPVP.SafeZone)) as Engines.ConPVP.SafeZone;

            if (sz != null /*&& sz.IsDisabled()*/ )
                return false;
            #endregion

            // Alan mod: custom teams
            if (from.CustomTeam && target.CustomTeam)
            {
                List<XmlTeam> targetTeams = XmlAttach.GetTeams(target);
                List<XmlTeam> fromTeams = XmlAttach.GetTeams(from);

                if (XmlTeam.SameTeam(fromTeams, targetTeams))
                {
                    return XmlTeam.AllowTeamHarmful(fromTeams, targetTeams);
                }
                // they are on the enemy team, allow harmful
                return true;
            }
            // end Alan mod

            if (bcFrom != null)
            {
                if (!bcFrom.Pseu_CanAttackInnocents && Notoriety.Compute(from, target) == Notoriety.Innocent)
                {
                    return false;
                }
            }
            // end Alan mod

            // ALAN MOD: protect young players from other players in regions where they are given protection
            if (MurderSystemController._YoungProtectionRegionsEnabled)
            {
                if (target is PlayerMobile && ((PlayerMobile)target).CheckYoungProtection(from) && !CheckAggressor(from.Aggressors, target) && !CheckAggressed(from.Aggressed, target))
                {
                    return false;
                }
            }
            // END ALAN MOD

            var g = (Guild)BaseGuild.FindByAbbrev("New");//don't allow people in the new guild to attack eachother
            if (g != null && g.IsMember(from) && g.IsMember(target))
            {
                from.CriminalAction(true);
                return true;
            }

            Map map = from.Map;

            if (map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0)
                return true; // In felucca, anything goes


            // check uncontrolled NPC mobs--there's a chance they won't attack if young protection is on
            if (!from.Player && !(bcFrom != null && bcFrom.GetMaster() != null && bcFrom.GetMaster().AccessLevel == AccessLevel.Player))
            {
                if (target is PlayerMobile && ((PlayerMobile)target).CheckYoungProtection(from) && !CheckAggressor(from.Aggressors, target) && !CheckAggressed(from.Aggressed, target))
                    return false;

                return true; // Uncontrolled NPCs are only restricted by the young system
            }

            Guild fromGuild = GetGuildFor(from.Guild as Guild, from);
            Guild targetGuild = GetGuildFor(target.Guild as Guild, target);

            if (fromGuild != null && targetGuild != null && (fromGuild == targetGuild || fromGuild.IsAlly(targetGuild) || fromGuild.IsEnemy(targetGuild)))
                return true; // Guild allies or enemies can be harmful

            if (target is BaseCreature && (((BaseCreature)target).Controlled || (((BaseCreature)target).Summoned && from != ((BaseCreature)target).SummonMaster)))
                return false; // Cannot harm other controlled mobiles

            if (target.Player)
                return false; // Cannot harm other players

            if (!(target is BaseCreature && ((BaseCreature)target).InitialInnocent))
            {
                if (Notoriety.Compute(from, target) == Notoriety.Innocent)
                    return false; // Cannot harm innocent mobiles
            }

            return true;
		}

		public static Guild GetGuildFor( Guild def, Mobile m )
		{
			Guild g = def;

			BaseCreature c = m as BaseCreature;

			if( c != null && c.Controlled && c.ControlMaster != null )
			{
				if (c.DisplayGuildTitle)
				{
					c.DisplayGuildTitle = false;
				}

				if (c.Map != Map.Internal && (c.EraAOS || Guild.NewGuildSystem || c.ControlOrder == OrderType.Attack || c.ControlOrder == OrderType.Guard))
					g = (Guild)(c.Guild = c.ControlMaster.Guild);
				else if( c.Map == Map.Internal || c.ControlMaster.Guild == null )
					g = (Guild)(c.Guild = null);
			}

			return g;
		}

		public static int CorpseNotoriety( Mobile source, Corpse target )
		{
			if( AutoRestart.ServerWars || target.AccessLevel > AccessLevel.Player)
				return Notoriety.CanBeAttacked;

            // Alan mod: custom teams
            if (source.CustomTeam)
            {
                // corpse teams are copied over from the mob
                List<XmlTeam> targetTeams = XmlAttach.GetTeams(target);
                if (targetTeams != null)
                {
                    List<XmlTeam> fromTeams = XmlAttach.GetTeams(source);

                    if (XmlTeam.SameTeam(fromTeams, targetTeams))
                    {
                        if (XmlTeam.ShowGreen(fromTeams, targetTeams))
                            return Notoriety.Ally;
                        // otherwise just use default notoriety
                    }
                    // they are on the enemy team, allow harmful
                    else
                    {
                        return Notoriety.Enemy;
                    }
                }
            }
            // end Alan mod

			Body body = (Body)target.Amount;

			BaseCreature cretOwner = target.Owner as BaseCreature;

			if( cretOwner != null )
			{
                if (cretOwner.FreelyLootable)
                    return Notoriety.CanBeAttacked;

				Guild sourceGuild = GetGuildFor( source.Guild as Guild, source );
				Guild targetGuild = GetGuildFor( target.Guild as Guild, target.Owner );

				if( sourceGuild != null && targetGuild != null )
				{
					if( sourceGuild == targetGuild || sourceGuild.IsAlly( targetGuild ) )
						return Notoriety.Ally;
					else if( sourceGuild.IsEnemy( targetGuild ) )
						return Notoriety.Enemy;
				}

				Faction srcFaction = Faction.Find( source, true, true );
				Faction trgFaction = Faction.Find( target.Owner, true, true );

				if( srcFaction != null && trgFaction != null && srcFaction != trgFaction && Faction.IsFactionFacet( source.Map ) )
					return Notoriety.Enemy;

				if( CheckHouseFlag( source, target.Owner, target.Location, target.Map ) )
					return Notoriety.CanBeAttacked;

				int actual = Notoriety.CanBeAttacked;

				if( target.Kills >= 5 || (body.IsMonster && IsSummoned( target.Owner as BaseCreature )) || (target.Owner is BaseCreature && (((BaseCreature)target.Owner).AlwaysMurderer || ((BaseCreature)target.Owner).IsAnimatedDead)) )
					actual = Notoriety.Murderer;

				if( DateTime.UtcNow >= (target.TimeOfDeath + Corpse.MonsterLootRightSacrifice) )
					return actual;

				Party sourceParty = Party.Get( source );

				List<Mobile> list = target.Aggressors;

				for( int i = 0; i < list.Count; ++i )
				{
					if( list[i] == source || (sourceParty != null && Party.Get( list[i] ) == sourceParty) )
						return actual;
				}

				return Notoriety.Innocent;
			}
			else
			{
				if( target.Kills >= 5 || (body.IsMonster && IsSummoned( target.Owner as BaseCreature )) || (target.Owner is BaseCreature && (((BaseCreature)target.Owner).AlwaysMurderer || ((BaseCreature)target.Owner).IsAnimatedDead)) )
					return Notoriety.Murderer;

				if (target.Criminal && target.Map != null && ((target.Map.Rules & MapRules.HarmfulRestrictions) == 0))
					return Notoriety.Criminal;

				Guild sourceGuild = GetGuildFor( source.Guild as Guild, source );
				Guild targetGuild = GetGuildFor( target.Guild as Guild, target.Owner );

				if( sourceGuild != null && targetGuild != null )
				{
					if( sourceGuild == targetGuild || sourceGuild.IsAlly( targetGuild ) )
						return Notoriety.Ally;
					else if( sourceGuild.IsEnemy( targetGuild ) )
						return Notoriety.Enemy;
				}

				Faction srcFaction = Faction.Find( source, true, true );
				Faction trgFaction = Faction.Find( target.Owner, true, true );

				if( srcFaction != null && trgFaction != null && srcFaction != trgFaction && Faction.IsFactionFacet( source.Map ) )
				{
					List<Mobile> secondList = target.Aggressors;

					for( int i = 0; i < secondList.Count; ++i )
					{
						if( secondList[i] == source || secondList[i] is BaseFactionGuard )
							return Notoriety.Enemy;
					}
				}

				if( target.Owner != null && target.Owner is BaseCreature && ((BaseCreature)target.Owner).AlwaysAttackable )
					return Notoriety.CanBeAttacked;

				if( CheckHouseFlag( source, target.Owner, target.Location, target.Map ) )
					return Notoriety.CanBeAttacked;

				if( !(target.Owner is PlayerMobile) && !IsPet( target.Owner as BaseCreature ) )
					return Notoriety.CanBeAttacked;
                                
                CustomRegion region1 = source.Region as CustomRegion;
                CaptureZoneRegion regionCz = source.Region as CaptureZoneRegion;
            
                if (region1 != null && region1.AlwaysGrey() || regionCz != null && regionCz.Czone.AlwaysGrey)
                    return Notoriety.CanBeAttacked;

				List<Mobile> list = target.Aggressors;

				for( int i = 0; i < list.Count; ++i )
				{
					if( list[i] == source )
						return Notoriety.CanBeAttacked;
				}

				return Notoriety.Innocent;
			}
		}

		public static int MobileNotoriety( Mobile source, Mobile target )
		{
            if ( (target.Blessed || (target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier) )
				return Notoriety.Invulnerable;

			if ( AutoRestart.ServerWars || target.AccessLevel > AccessLevel.Player )
				return Notoriety.CanBeAttacked;

			#region Dueling
			if( source is PlayerMobile && target is PlayerMobile )
			{
				PlayerMobile pmFrom = (PlayerMobile)source;
				PlayerMobile pmTarg = (PlayerMobile)target;

				if( pmFrom.DuelContext != null && pmFrom.DuelContext.StartedBeginCountdown && !pmFrom.DuelContext.Finished && pmFrom.DuelContext == pmTarg.DuelContext )
					return pmFrom.DuelContext.IsAlly( pmFrom, pmTarg ) ? Notoriety.Ally : Notoriety.Enemy;
			}
			#endregion

            // Alan mod: custom teams
            if (source.CustomTeam && target.CustomTeam)
            {
                List<XmlTeam> targetTeams = XmlAttach.GetTeams(target);
                List<XmlTeam> fromTeams = XmlAttach.GetTeams(source);

                if (XmlTeam.SameTeam(fromTeams, targetTeams))
                {
                    if (XmlTeam.ShowGreen(fromTeams, targetTeams))
                        return Notoriety.Ally;
                    // if not show green, use default notoriety
                }
                // they are on the enemy team, allow harmful
                else
                {
                    return Notoriety.Enemy;
                }
            }
            // end Alan mod

			BaseCreature tbc = target as BaseCreature;
			BaseCreature sbc = source as BaseCreature;
			PlayerMobile tpm = target as PlayerMobile;
			PlayerMobile spm = source as PlayerMobile;

			if ( source.Player && !target.Player && spm != null && tbc != null )
			{
				Mobile master = tbc.GetMaster();

				if ( !tbc.AlwaysMurderer && tbc.Controlled && master != null )
				{
					if ( master.AccessLevel > AccessLevel.Player || ( source == master && CheckAggressor( tbc.Aggressors, source ) ) || ( CheckAggressor( source.Aggressors, tbc ) ) )
						return Notoriety.CanBeAttacked;
					else
						return MobileNotoriety( source, master );
				}
			}

			if ( target.Kills >= 5 || ( target.Body.IsMonster && IsSummoned( tbc ) && !( target is BaseFamiliar ) && !( target is ArcaneFey ) && !( target is Golem ) ) || ( tbc != null && ( tbc.AlwaysMurderer || tbc.IsAnimatedDead ) ) )
				return Notoriety.Murderer;
				
			if ( target.Criminal )
				return Notoriety.Criminal;

            CustomRegion region1 = target.Region as CustomRegion;
            CaptureZoneRegion regionCz = target.Region as CaptureZoneRegion;

            if (region1 != null && region1.AlwaysGrey() || regionCz != null && regionCz.Czone.AlwaysGrey)
                return Notoriety.CanBeAttacked;

			Guild sourceGuild = GetGuildFor( source.Guild as Guild, source );
			Guild targetGuild = GetGuildFor( target.Guild as Guild, target );

			if( sourceGuild != null && targetGuild != null )
			{
				if( sourceGuild == targetGuild || sourceGuild.IsAlly( targetGuild ) )
					return Notoriety.Ally;
				else if( sourceGuild.IsEnemy( targetGuild ) )
					return Notoriety.Enemy;
			}



			#region Ethics Checks of target or master of target
			Ethics.Ethic sourceEthic = Ethics.Ethic.Find( source, true, true );
			Ethics.Ethic targetEthic = Ethics.Ethic.Find( target, true, true );

			if ( sourceEthic != null && targetEthic != null && targetEthic != sourceEthic )
				return Notoriety.Enemy;
			#endregion

			Faction srcFaction = Faction.Find( source, true, true );
			Faction trgFaction = Faction.Find( target, true, true );

			if( srcFaction != null && trgFaction != null && srcFaction != trgFaction && source.Map == Faction.Facet )
				return Notoriety.Enemy;

			if ( tpm != null && SkillHandlers.Stealing.ClassicMode && tpm.PermaFlags.Contains( source ) )
				return Notoriety.CanBeAttacked;

			if( tbc != null && tbc.AlwaysAttackable )
				return Notoriety.CanBeAttacked;

			if( CheckHouseFlag( source, target, target.Location, target.Map ) )
				return Notoriety.CanBeAttacked;

            if (!(tbc != null && (tbc.InitialInnocent || tbc.InnocentDefault)))   //If Target is NOT A baseCreature, OR it's a BC and the BC is initial innocent...
			{
				if( !target.Body.IsHuman && !target.Body.IsGhost && !IsPet( tbc ) && tpm == null && !target.Player || !target.EraML && !target.CanBeginAction( typeof( Server.Spells.Seventh.PolymorphSpell ) ) && !target.Player)
					return Notoriety.CanBeAttacked;
			}

			if ( CheckAggressor( source.Aggressors, target ) )
				return Notoriety.CanBeAttacked;

			if ( CheckAggressed( source.Aggressed, target ) )
				return Notoriety.CanBeAttacked;

			if ( tbc != null )
			{
				if ( tbc.Controlled && tbc.ControlOrder == OrderType.Guard && tbc.ControlTarget == source )
					return Notoriety.CanBeAttacked;
			}

			if( sbc != null )
			{
				Mobile smaster = sbc.GetMaster();

				if ( smaster != null )
					if( CheckAggressor( smaster.Aggressors, target ) || MobileNotoriety( smaster, target ) == Notoriety.CanBeAttacked || tbc != null )
						return Notoriety.CanBeAttacked;
			}

			return Notoriety.Innocent;
		}

		public static bool CheckHouseFlag( Mobile from, Mobile m, Point3D p, Map map )
		{
			BaseHouse house = BaseHouse.FindHouseAt( p, map, 16 );

			if( house == null || house.Public || !house.IsFriend( from ) )
				return false;

			if( m != null && house.IsFriend( m ) )
				return false;

			BaseCreature c = m as BaseCreature;

			if( c != null && !c.Deleted && c.Controlled && c.ControlMaster != null )
				return !house.IsFriend( c.ControlMaster );

			return true;
		}

		public static bool IsPet( BaseCreature c )
		{
			return (c != null && c.Controlled);
		}

		public static bool IsSummoned( BaseCreature c )
		{
			return (c != null && /*c.Controlled &&*/ c.Summoned);
		}

		public static bool CheckAggressor( List<AggressorInfo> list, Mobile target )
		{
			for( int i = 0; i < list.Count; ++i )
				if( list[i].Attacker == target )
					return true;

			return false;
		}

		public static bool CheckAggressed( List<AggressorInfo> list, Mobile target )
		{
			for( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo info = list[i];

				if( !info.CriminalAggression && info.Defender == target )
					return true;
			}

			return false;
		}
	}
}