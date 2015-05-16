#region References
using System;
using System.Collections.Generic;

using Server.Engines.CannedEvil;
using Server.Engines.ConPVP;
using Server.Engines.PartySystem;
using Server.Engines.XmlSpawner2;
using Server.Factions;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;
using Server.Spells.Eighth;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using Server.Targeting;
#endregion

namespace Server
{
	public class DefensiveSpell
	{
		public static void Nullify(Mobile from)
		{
			if (!from.CanBeginAction(typeof(DefensiveSpell)))
			{
				new InternalTimer(from).Start();
			}
		}

		private class InternalTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public InternalTimer(Mobile m)
				: base(TimeSpan.FromMinutes(1.0))
			{
				m_Mobile = m;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Mobile.EndAction(typeof(DefensiveSpell));
			}
		}
	}
}

namespace Server.Spells
{
	[Flags]
	public enum TravelCheckType
	{
		None = 0x00,
		RecallFrom = 0x01,
		RecallTo = 0x02,
		GateFrom = 0x04,
		GateTo = 0x08,
		Mark = 0x10,
		TeleportFrom = 0x20,
		TeleportTo = 0x40
	}

	public class SpellHelper
	{
		private static readonly TimeSpan OldDamageDelay = TimeSpan.FromSeconds(0.5);

		public static TimeSpan GetDamageDelayForSpell(Spell sp)
		{
			if (sp == null)
			{
				return TimeSpan.Zero;
			}

			if (!sp.DelayedDamage)
			{
				return TimeSpan.Zero;
			}

			return OldDamageDelay;
		}

		public static bool CheckMulti(Point3D p, Map map)
		{
			return CheckMulti(p, map, true, 0);
		}

		public static bool CheckMulti(Point3D p, Map map, bool houses)
		{
			return CheckMulti(p, map, houses, 0);
		}

		public static bool CheckMulti(Point3D p, Map map, bool houses, int housingrange)
		{
			if (map == null || map == Map.Internal)
			{
				return false;
			}

			Sector sector = map.GetSector(p.X, p.Y);

			for (int i = 0; i < sector.Multis.Count; ++i)
			{
				BaseMulti multi = sector.Multis[i];

				if (multi is BaseHouse)
				{
					if (houses)
					{
						var bh = (BaseHouse)multi;

						if (bh.IsInside(p, 16) || (housingrange > 0 && bh.InRange(p, housingrange)))
						{
							return true;
						}
					}
				}
				else if (multi.Contains(p))
				{
					return true;
				}
			}

			return false;
		}

		public static void Turn(Mobile from, object to)
		{
			var target = to as IPoint3D;

			if (target is Item)
			{
				var item = (Item)target;

				if (item.RootParent != from)
				{
					from.Direction = from.GetDirectionTo(item.GetWorldLocation());
				}
			}
			else if (target != null && from != target)
			{
				from.Direction = from.GetDirectionTo(target);
			}
		}

		private static readonly TimeSpan CombatHeatDelay = TimeSpan.FromSeconds(30.0);
		private static bool RestrictTravelCombat = true;

		public static bool CheckCombat(Mobile m)
		{
			if (RestrictTravelCombat)
			{
				Map map = m.Map;
				Faction srcFaction = Faction.Find(m, true, true);

				for (int i = 0; i < m.Aggressed.Count; ++i)
				{
					AggressorInfo info = m.Aggressed[i];

					if (info.Defender.Player && (DateTime.UtcNow - info.LastCombatTime) < CombatHeatDelay)
					{
						return true;
					}
				}

				if (srcFaction != null) //Causes reds to be unable to recall when attacked by anyone
				{
					for (int i = 0; i < m.Aggressors.Count; ++i)
					{
						AggressorInfo info = m.Aggressors[i];
						Faction trgFaction = Faction.Find(info.Attacker, true, true);

						if (trgFaction != null && srcFaction != trgFaction && Faction.IsFactionFacet(map) && info.Attacker.Player &&
							(DateTime.UtcNow - info.LastCombatTime) < CombatHeatDelay)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public static bool AdjustField(ref Point3D p, Map map, int height, bool mobsBlock)
		{
			if (map != null)
			{
				for (int offset = 0; offset < 10; ++offset)
				{
					var loc = new Point3D(p.X, p.Y, p.Z - offset);

					if (map.CanFit(loc, height, true, mobsBlock))
					{
						p = loc;
						return true;
					}
				}
			}

			return false;
		}

		public static bool CanRevealCaster(Mobile m)
		{
			if (m is BaseCreature)
			{
				var c = (BaseCreature)m;

				if (!c.Controlled && !c.Summoned)
				{
					return true;
				}
			}

			return false;
		}

		public static void GetSurfaceTop(ref IPoint3D p)
		{
			if (p is Item)
			{
				p = ((Item)p).GetSurfaceTop();
			}
			else if (p is StaticTarget)
			{
				var t = (StaticTarget)p;
				int z = t.Z;

				if ((t.Flags & TileFlag.Surface) == 0)
				{
					z -= TileData.ItemTable[t.ItemID & TileData.MaxItemValue].CalcHeight;
				}

				p = new Point3D(t.X, t.Y, z);
			}
		}

		public static bool AddStatOffset(Mobile m, StatType type, int offset, TimeSpan duration)
		{
			if (offset > 0)
			{
				return AddStatBonus(m, m, type, offset, duration);
			}
			else if (offset < 0)
			{
				return AddStatCurse(m, m, type, -offset, duration);
			}

			return true;
		}

		public static bool AddStatBonus(Mobile caster, Mobile target, StatType type, bool skillcheck)
		{
			return AddStatBonus(
				caster, target, type, GetOffset(caster, target, type, false, skillcheck), GetDuration(caster, target));
		}

		public static bool AddStatBonus(Mobile caster, Mobile target, StatType type, int bonus, TimeSpan duration)
		{
			int offset = bonus;
			string name = String.Format("[Magic] {0} Offset", type);

			StatMod mod = target.GetStatMod(name);

			if (mod != null && mod.Offset < 0)
			{
				target.AddStatMod(new StatMod(type, name, mod.Offset + offset, duration));
				return true;
			}
			else if (mod == null || mod.Offset < offset)
			{
				target.AddStatMod(new StatMod(type, name, offset, duration));
				return true;
			}

			return false;
		}

		public static bool AddStatCurse(Mobile caster, Mobile target, StatType type, bool skillcheck)
		{
			return AddStatCurse(
				caster, target, type, GetOffset(caster, target, type, true, skillcheck), GetDuration(caster, target));
		}

		public static bool AddStatCurse(Mobile caster, Mobile target, StatType type, int curse, TimeSpan duration)
		{
			int offset = -curse;
			string name = String.Format("[Magic] {0} Offset", type);

			StatMod mod = target.GetStatMod(name);

			if (mod != null && mod.Offset > 0)
			{
				target.AddStatMod(new StatMod(type, name, mod.Offset + offset, duration));
				return true;
			}
			else if (mod == null || mod.Offset > offset)
			{
				target.AddStatMod(new StatMod(type, name, offset, duration));
				return true;
			}

			return false;
		}

		public static TimeSpan GetDuration(Mobile caster, Mobile target)
		{
			if (caster == null)
			{
				return TimeSpan.Zero;
			}

			if (caster.EraAOS)
			{
				return TimeSpan.FromSeconds(((6 * caster.Skills.EvalInt.Fixed) / 50) + 1);
			}

			if (caster.EraUOR)
			{
				double duration = caster.Skills[SkillName.Magery].Value;

				if (target.Player)
				{
					duration = Math.Min(duration, 100.0);

					if (caster.Skills.Magery.Value > 100.0) // adjusted for powerscrolls
					{
						duration *= 1.0 + ((caster.Skills.Magery.Value / 120.0) * 0.1);
					}
				}

				return TimeSpan.FromSeconds(duration * 1.2);
			}

			if (caster.EraT2A)
			{
				// HACK: Convert to T2A mechanics.
				double duration = caster.Skills[SkillName.Magery].Value;

				if (target.Player)
				{
					duration = Math.Min(duration, 100.0);

					if (caster.Skills.Magery.Value > 100.0) // adjusted for powerscrolls
					{
						duration *= 1.0 + ((caster.Skills.Magery.Value / 120.0) * 0.1);
					}
				}

				return TimeSpan.FromSeconds(duration * 1.2);
			}

			return TimeSpan.FromSeconds(1.2);
		}

		/*
		private static bool m_DisableSkillCheck;

		public static bool DisableSkillCheck
		{
			get { return m_DisableSkillCheck; }
			set { m_DisableSkillCheck = value; }
		}
*/

		public static double GetOffsetScalar(Mobile caster, Mobile target, bool curse)
		{
			double percent;

			//Adjusted for powerscrolls

			double evalint = caster.Skills.EvalInt.Fixed;
			double resist = target.Skills.MagicResist.Fixed;

			if (target.Player && caster.Player)
			{
				evalint = Math.Min(evalint, 1000);

				if (caster.Skills.EvalInt.Fixed > 1000)
				{
					evalint *= 1.0 + ((caster.Skills.EvalInt.Value / 120.0) * 0.1);
				}

				resist = Math.Min(resist, 1000);

				if (target.Skills.MagicResist.Fixed > 1000)
				{
					resist *= 1.0 + ((target.Skills.MagicResist.Value / 120.0) * 0.1);
				}
			}

			if (curse)
			{
				percent = 8 + (evalint / 100) - (resist / 100);
			}
			else
			{
				percent = 1 + (evalint / 100);
			}

			percent *= 0.01;

			if (percent < 0)
			{
				percent = 0;
			}

			return percent;
		}

		public static int GetOffset(Mobile caster, Mobile target, StatType type, bool curse, bool skillcheck)
		{
			if (caster == null)
			{
				return 0;
			}

			if (caster.EraAOS)
			{
				if (skillcheck)
				{
					caster.CheckSkill(SkillName.EvalInt, 0.0, 120.0);

					if (curse)
					{
						target.CheckSkill(SkillName.MagicResist, 0.0, 120.0);
					}
				}

				double percent = GetOffsetScalar(caster, target, curse);

				switch (type)
				{
					case StatType.Str:
						return (int)(target.RawStr * percent);
					case StatType.Dex:
						return (int)(target.RawDex * percent);
					case StatType.Int:
						return (int)(target.RawInt * percent);
				}
			}
			else if (caster.EraUOR)
			{
				return 1 + (int)(caster.Skills[SkillName.Magery].Value * 0.1);
			}
			else if (caster.EraT2A)
			{
				// HACK: Convert to T2A mechanics
				return 1 + (int)(caster.Skills[SkillName.Magery].Value * 0.1);
			}

			return 0;
		}

		public static Guild GetGuildFor(Mobile m)
		{
			var g = m.Guild as Guild;

			if (g == null && m is BaseCreature)
			{
				var c = (BaseCreature)m;
				m = c.ControlMaster;

				if (m != null)
				{
					g = m.Guild as Guild;
				}

				if (g == null)
				{
					m = c.SummonMaster;

					if (m != null)
					{
						g = m.Guild as Guild;
					}
				}
			}

			return g;
		}

		public static bool ValidIndirectTarget(Mobile from, Mobile to)
		{
			if (from == to)
			{
				return true;
			}

			if (to.Hidden && to.AccessLevel > from.AccessLevel)
			{
				return false;
			}

			if (to.IsYoung())
			{
				return false;
			}

			#region Dueling
			var pmFrom = from as PlayerMobile;
			var pmTarg = to as PlayerMobile;

			if (pmFrom == null && from is BaseCreature)
			{
				var bcFrom = (BaseCreature)from;

				if (bcFrom.Summoned)
				{
					pmFrom = bcFrom.SummonMaster as PlayerMobile;
				}
			}

			if (pmTarg == null && to is BaseCreature)
			{
				var bcTarg = (BaseCreature)to;

				if (bcTarg.Summoned)
				{
					pmTarg = bcTarg.SummonMaster as PlayerMobile;
				}
			}

			if (pmFrom != null && pmTarg != null)
			{
				if (pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.Started &&
					pmFrom.DuelPlayer != null && pmTarg.DuelPlayer != null)
				{
					return (pmFrom.DuelPlayer.Participant != pmTarg.DuelPlayer.Participant);
				}
			}
			#endregion

			Guild fromGuild = GetGuildFor(from);
			Guild toGuild = GetGuildFor(to);

			if (fromGuild != null && toGuild != null && (fromGuild == toGuild || fromGuild.IsAlly(toGuild)))
			{
				return false;
			}

			Party p = Party.Get(from);

			if (p != null && p.Contains(to))
			{
				return false;
			}

			Map map = from.Map;
			Faction srcFaction = Faction.Find(from, true, true);
			Faction trgFaction = Faction.Find(to, true, true);

			if (srcFaction != null && trgFaction != null && srcFaction != trgFaction && Faction.IsFactionFacet(map))
			{
				return true;
			}

			var tbc = to as BaseCreature;
			var fbc = from as BaseCreature;

			if (tbc != null)
			{
				if (tbc.Controlled || tbc.Summoned)
				{
					if (tbc.ControlMaster == from || tbc.SummonMaster == from)
					{
						return false;
					}

					if (p != null && (p.Contains(tbc.ControlMaster) || p.Contains(tbc.SummonMaster)))
					{
						return false;
					}
				}
			}

			if (fbc != null)
			{
				if (fbc.Controlled || fbc.Summoned)
				{
					if (fbc.ControlMaster == to || fbc.SummonMaster == to)
					{
						return false;
					}

					p = Party.Get(to);

					if (p != null && (p.Contains(fbc.ControlMaster) || p.Contains(fbc.SummonMaster)))
					{
						return false;
					}
				}
				if (fbc.Pseu_EQPlayerAllowed) // anybody is fair game
				{
					return true;
				}
			}

			if (tbc != null && fbc != null && !tbc.Controlled && !fbc.Controlled && !tbc.BardProvoked && !fbc.BardProvoked &&
				tbc.Team == fbc.Team)
			{
				return false;
			}

			if (tbc != null && !tbc.Controlled && tbc.InitialInnocent)
			{
				return true;
			}

			int noto = Notoriety.Compute(from, to);
			//			if (noto != Notoriety.Innocent || from.Kills >= Mobile.MurderCount)
			//				return true;
			return (noto != Notoriety.Innocent || from.Kills >= Mobile.MurderCount);
		}

		private static readonly int[] m_Offsets = new[] {-1, -1, -1, 0, -1, 1, 0, -1, 0, 1, 1, -1, 1, 0, 1, 1};

		public static void Summon(
			BaseCreature creature, Mobile caster, int sound, TimeSpan duration, bool scaleDuration, bool scaleStats)
		{
			Map map = caster.Map;

			if (map == null)
			{
				return;
			}

			double scale = 1.0 + ((caster.Skills[SkillName.Magery].Value - 100.0) / 200.0);

			if (scaleDuration)
			{
				duration = TimeSpan.FromSeconds(duration.TotalSeconds * scale);
			}

			if (scaleStats)
			{
				creature.RawStr = (int)(creature.RawStr * scale);
				creature.Hits = creature.HitsMax;

				creature.RawDex = (int)(creature.RawDex * scale);
				creature.Stam = creature.StamMax;

				creature.RawInt = (int)(creature.RawInt * scale);
				creature.Mana = creature.ManaMax;
			}

			var p = new Point3D(caster);

			if (FindValidSpawnLocation(map, ref p, true))
			{
				BaseCreature.Summon(creature, caster, p, sound, duration);
				return;
			}

			/*
			int offset = Utility.Random( 8 ) * 2;

			for( int i = 0; i < m_Offsets.Length; i += 2 )
			{
				int x = caster.X + m_Offsets[(offset + i) % m_Offsets.Length];
				int y = caster.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

				if( map.CanSpawnMobile( x, y, caster.Z ) )
				{
					BaseCreature.Summon( creature, caster, new Point3D( x, y, caster.Z ), sound, duration );
					return;
				}
				else
				{
					int z = map.GetAverageZ( x, y );

					if( map.CanSpawnMobile( x, y, z ) )
					{
						BaseCreature.Summon( creature, caster, new Point3D( x, y, z ), sound, duration );
						return;
					}
				}
			}
			 * */

			creature.Delete();
			caster.SendLocalizedMessage(501942); // That location is blocked.
		}

		public static bool FindValidSpawnLocation(Map map, ref Point3D p, bool surroundingsOnly)
		{
			if (map == null) //sanity
			{
				return false;
			}

			if (!surroundingsOnly)
			{
				if (map.CanSpawnMobile(p)) //p's fine.
				{
					p = new Point3D(p);
					return true;
				}

				int z = map.GetAverageZ(p.X, p.Y);

				if (map.CanSpawnMobile(p.X, p.Y, z))
				{
					p = new Point3D(p.X, p.Y, z);
					return true;
				}
			}

			int offset = Utility.Random(8) * 2;

			for (int i = 0; i < m_Offsets.Length; i += 2)
			{
				int x = p.X + m_Offsets[(offset + i) % m_Offsets.Length];
				int y = p.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

				if (map.CanSpawnMobile(x, y, p.Z))
				{
					p = new Point3D(x, y, p.Z);
					return true;
				}
				else
				{
					int z = map.GetAverageZ(x, y);

					if (map.CanSpawnMobile(x, y, z))
					{
						p = new Point3D(x, y, z);
						return true;
					}
				}
			}

			return false;
		}

		private static readonly TravelRestrictionValidator[] m_Validators = new[]
		{
			new TravelRestrictionValidator(
				IsFeluccaT2A,
				TravelCheckType.RecallFrom | TravelCheckType.RecallTo | TravelCheckType.GateFrom | TravelCheckType.GateTo |
				TravelCheckType.Mark | TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(IsIlshenar, TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo | TravelCheckType.RecallFrom),
			new TravelRestrictionValidator(
				IsTrammelWind,
				TravelCheckType.RecallFrom | TravelCheckType.RecallTo | TravelCheckType.GateFrom | TravelCheckType.GateTo |
				TravelCheckType.Mark | TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(
				IsFeluccaWind,
				TravelCheckType.RecallFrom | TravelCheckType.RecallTo | TravelCheckType.GateFrom | TravelCheckType.GateTo |
				TravelCheckType.Mark | TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(
				IsFeluccaDungeon,
				TravelCheckType.RecallFrom | TravelCheckType.RecallTo | TravelCheckType.GateFrom | TravelCheckType.GateTo |
				TravelCheckType.Mark | TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(IsTrammelSolenHive, TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(IsFeluccaSolenHive, TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(IsCrystalCave, TravelCheckType.None),
			new TravelRestrictionValidator(IsDoomGauntlet, TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(IsDoomFerry, TravelCheckType.TeleportFrom),
			new TravelRestrictionValidator(IsSafeZone, TravelCheckType.TeleportFrom | TravelCheckType.RecallFrom),
			new TravelRestrictionValidator(IsFactionStronghold, TravelCheckType.None),
			new TravelRestrictionValidator(IsChampionSpawn, TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(IsTokunoDungeon, TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(IsLampRoom, TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(IsGuardianRoom, TravelCheckType.TeleportFrom | TravelCheckType.TeleportTo),
			new TravelRestrictionValidator(IsHeartwood, TravelCheckType.None),
			new TravelRestrictionValidator(IsMLDungeon, TravelCheckType.TeleportFrom)
		};

		public static bool CheckTravel(Mobile caster, TravelCheckType type)
		{
			//if ( CheckTravel( caster, caster.Map, caster.Location, type ) )
			//	return true;

			//SendInvalidMessage( caster, type );
			//return false;

			return CheckTravel(caster, caster.Map, caster.Location, type);
		}

		public static void SendInvalidMessage(Mobile caster, TravelCheckType type)
		{
			if (type == TravelCheckType.RecallTo || type == TravelCheckType.GateTo)
			{
				caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
			}
			else if (type == TravelCheckType.TeleportTo)
			{
				caster.SendLocalizedMessage(501035); // You cannot teleport from here to the destination.
			}
			else
			{
				caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
			}
		}

		public static bool CheckTravel(Map map, Point3D loc, TravelCheckType type)
		{
			return CheckTravel(null, map, loc, type);
		}

		public static bool CheckTravel(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			if (IsInvalid(map, loc)) // null, internal, out of bounds
			{
				if (caster != null)
				{
					SendInvalidMessage(caster, type);
				}

				return false;
			}

            if (caster is BaseCreature && (type == TravelCheckType.TeleportTo || type == TravelCheckType.TeleportFrom))
            {
                var bc = (BaseCreature)caster;

                if (!bc.Controlled && !bc.Summoned)
                {
                    return true;
                }
            }

			if (caster != null && caster.AccessLevel == AccessLevel.Player)
			{
				if (caster.Map != map && caster.Region != null && !(caster.Region is CustomRegion))
				{
					caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
					return false;
				}

				// Alan MOD only recall, gate, mark in felucca allowed
				if (map != Map.Felucca && caster.Region != null && !(caster.Region is CustomRegion))
				{
					return false;
				}
				// end Alan MOD

				if (caster.Region.IsPartOf(typeof(Jail)))
				{
					caster.SendLocalizedMessage(1114345); // You'll need a better jailbreak plan than that!
					return false;
				}
				CustomRegion customRegion;
				Region ga = Region.Find(loc, map);
				if (ga != null)
				{
					if (ga.IsPartOf(typeof(GreenAcres)))
					{
						caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
						return false;
					}
					customRegion = ga as CustomRegion;
					if (type == TravelCheckType.RecallTo &&
						(customRegion != null && customRegion.Controller != null &&
						 (!customRegion.Controller.AllowRecallIn || !customRegion.Controller.CanEnter)) ||
						type == TravelCheckType.GateTo &&
						(customRegion != null && customRegion.Controller != null &&
						 (!customRegion.Controller.AllowGateIn || !customRegion.Controller.CanEnter)))
					{
						caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
						return false;
					}
				}
			}

			// Always allow monsters to teleport
			bool isValid = true;

			for (int i = 0; isValid && i < m_Validators.Length; ++i)
			{
				isValid = m_Validators[i].CanTravel(type) || !m_Validators[i].Validator(caster, map, loc, type);
			}

			if (!isValid && caster != null)
			{
				SendInvalidMessage(caster, type);
			}

			return isValid;
		}

		public static bool IsWindLoc(Point3D loc)
		{
			int x = loc.X, y = loc.Y;

			return (x >= 5120 && y >= 0 && x < 5376 && y < 256);
		}

		public static bool IsWind(Point3D loc, Map map)
		{
			return (map == Map.Felucca || map == Map.Trammel) && IsWindLoc(loc);
		}

		public static bool IsFeluccaWind(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			return (map == Map.Felucca && IsWindLoc(loc) && caster.Skills[SkillName.Magery].Base >= 70.0);
		}

		public static bool IsTrammelWind(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			return (map == Map.Trammel && IsWindLoc(loc) && caster.Skills[SkillName.Magery].Base >= 70.0);
		}

		public static bool IsIlshenar(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			return (map == Map.Ilshenar);
		}

		public static bool IsSolenHiveLoc(Point3D loc)
		{
			int x = loc.X, y = loc.Y;

			return (x >= 5640 && y >= 1776 && x < 5935 && y < 2039);
		}

		public static bool IsTrammelSolenHive(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			return (map == Map.Trammel && IsSolenHiveLoc(loc));
		}

		public static bool IsFeluccaSolenHive(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			return (map == Map.Felucca && IsSolenHiveLoc(loc));
		}

		public static bool IsFeluccaT2A(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			return IsFeluccaT2A(map, loc);
		}

		public static bool IsFeluccaT2A(Map map, Point3D loc)
		{
			int x = loc.X, y = loc.Y;

			return (map == Map.Felucca && x >= 5120 && y >= 2304 && x < 6144 && y < 4096);
		}

		public static bool IsAnyT2A(Map map, Point3D loc)
		{
			int x = loc.X, y = loc.Y;

			return ((map == Map.Trammel || map == Map.Felucca) && x >= 5120 && y >= 2304 && x < 6144 && y < 4096);
		}

		public static bool IsFeluccaDungeon(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			Region region = Region.Find(loc, map);
			return (region.IsPartOf(typeof(DungeonRegion)) && region.Map == Map.Felucca);
		}

		public static bool IsCrystalCave(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			if (map != Map.Malas || loc.Z >= -80)
			{
				return false;
			}

			int x = loc.X, y = loc.Y;

			return (x >= 1182 && y >= 437 && x < 1211 && y < 470) || (x >= 1156 && y >= 470 && x < 1211 && y < 503) ||
				   (x >= 1176 && y >= 503 && x < 1208 && y < 509) || (x >= 1188 && y >= 509 && x < 1201 && y < 513);
		}

		public static bool IsSafeZone(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			#region Duels
			if (Region.Find(loc, map).IsPartOf(typeof(SafeZone)))
			{
				if (type == TravelCheckType.TeleportTo || type == TravelCheckType.TeleportFrom)
				{
					var pm = caster as PlayerMobile;

					if (pm != null && pm.DuelPlayer != null && !pm.DuelPlayer.Eliminated)
					{
						return true;
					}
				}

				return true;
			}
			#endregion

			return false;
		}

		public static bool IsFactionStronghold(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			// Teleporting is allowed, but only for faction members
			if (caster != null && !caster.EraAOS && (type == TravelCheckType.TeleportTo || type == TravelCheckType.TeleportFrom))
			{
				if (Faction.Find(caster, true, true) != null)
				{
					return false;
				}
			}

			return (Region.Find(loc, map).IsPartOf(typeof(StrongholdRegion)));
		}

		public static bool IsChampionSpawn(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			return (Region.Find(loc, map).IsPartOf(typeof(ChampionSpawnRegion)));
		}

		public static bool IsDoomFerry(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			if (map != Map.Malas)
			{
				return false;
			}

			int x = loc.X, y = loc.Y;

			if (x >= 426 && y >= 314 && x <= 430 && y <= 331)
			{
				return true;
			}

			if (x >= 406 && y >= 247 && x <= 410 && y <= 264)
			{
				return true;
			}

			return false;
		}

		public static bool IsTokunoDungeon(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			//The tokuno dungeons are really inside malas
			if (map != Map.Malas)
			{
				return false;
			}

			int x = loc.X, y = loc.Y, z = loc.Z;

			bool r1 = (x >= 0 && y >= 0 && x <= 128 && y <= 128);
			bool r2 = (x >= 45 && y >= 320 && x < 195 && y < 710);

			return (r1 || r2);
		}

		public static bool IsDoomGauntlet(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			return IsDoomGauntlet(map, loc);
		}

		public static bool IsDoomGauntlet(Map map, Point3D loc)
		{
			if (map != Map.Malas)
			{
				return false;
			}

			int x = loc.X - 256, y = loc.Y - 304;

			return (x >= 0 && y >= 0 && x < 256 && y < 256);
		}

		public static bool IsLampRoom(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			if (map != Map.Malas)
			{
				return false;
			}

			int x = loc.X, y = loc.Y;

			return (x >= 465 && y >= 92 && x < 474 && y < 102);
		}

		public static bool IsGuardianRoom(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			if (map != Map.Malas)
			{
				return false;
			}

			int x = loc.X, y = loc.Y;

			return (x >= 356 && y >= 5 && x < 375 && y < 25);
		}

		public static bool IsHeartwood(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			int x = loc.X, y = loc.Y;

			return (map == Map.Trammel || map == Map.Felucca) && (x >= 6911 && y >= 254 && x < 7167 && y < 511);
		}

		public static bool IsMLDungeon(Mobile caster, Map map, Point3D loc, TravelCheckType type)
		{
			return MondainsLegacy.IsMLRegion(Region.Find(loc, map));
		}

		public static bool IsInvalid(Map map, Point3D loc)
		{
			if (map == null || map == Map.Internal)
			{
				return true;
			}

			int x = loc.X, y = loc.Y;

			return (x < 0 || y < 0 || x >= map.Width || y >= map.Height);
		}

		//towns
		public static bool IsTown(IPoint3D loc, Mobile caster)
		{
			if (loc is Item)
			{
				loc = ((Item)loc).GetWorldLocation();
			}

			return IsTown(new Point3D(loc), caster);
		}

		public static bool IsTown(Point3D loc, Mobile caster)
		{
			Map map = caster.Map;

			if (map == null)
			{
				return false;
			}

			#region Dueling
			var sz = (SafeZone)Region.Find(loc, map).GetRegion(typeof(SafeZone));

			if (sz != null)
			{
				var pm = (PlayerMobile)caster;

				if (pm == null || pm.DuelContext == null || !pm.DuelContext.Started || pm.DuelPlayer == null ||
					pm.DuelPlayer.Eliminated)
				{
					return true;
				}
			}
			#endregion

			var reg = (GuardedRegion)Region.Find(loc, map).GetRegion(typeof(GuardedRegion));

			return (reg != null && !reg.IsDisabled());
		}

		public static bool CheckTown(IPoint3D loc, Mobile caster)
		{
			if (loc is Item)
			{
				loc = ((Item)loc).GetWorldLocation();
			}

			return CheckTown(new Point3D(loc), caster);
		}

		public static bool CheckTown(Point3D loc, Mobile caster)
		{
			if (IsTown(loc, caster))
			{
				caster.SendLocalizedMessage(500946); // You cannot cast this in town!
				return false;
			}

			return true;
		}

		//magic reflection
		public static void CheckReflect(int circle, Mobile caster, ref Mobile target)
		{
			CheckReflect(circle, ref caster, ref target);
		}

		public static void CheckReflect(int circle, ref Mobile caster, ref Mobile target)
		{
			bool reflect = false;

			if (target.MagicDamageAbsorb > 0)
			{
				++circle;

				target.MagicDamageAbsorb -= circle;

				reflect = (target.MagicDamageAbsorb >= 0);

				if (target.MagicDamageAbsorb <= 0)
				{
					target.MagicDamageAbsorb = 0;
					DefensiveSpell.Nullify(target);
				}
			}

			if (target is BaseCreature)
			{
				((BaseCreature)target).CheckReflect(caster, ref reflect);
			}

			if (reflect)
			{
				target.FixedEffect(0x37B9, 10, 5);

				//if ( target.Spell != null && target.Spell.IsCasting )
				//	((Spell)target.Spell).Disturb( DisturbType.Hurt, false, false );

                caster.DoHarmful(target);
				Mobile temp = caster;
				caster = target;
				target = temp;
			}
		}

		public static void Damage(Spell spell, Mobile target, double damage)
		{
			TimeSpan ts = GetDamageDelayForSpell(spell);
			Damage(spell, ts, target, damage);
		}

		public static void Damage(TimeSpan delay, Mobile target, double damage)
		{
			Damage(null, delay, target, null, damage);
		}

		public static void Damage(TimeSpan delay, Mobile target, Mobile from, double damage)
		{
			Damage(null, delay, target, from, damage);
		}

		public static void Damage(Spell spell, TimeSpan delay, Mobile target, double damage)
		{
			Damage(spell, delay, target, (spell == null ? null : spell.Caster), damage);
		}

		public static void Damage(Spell spell, TimeSpan delay, Mobile target, Mobile from, double damage)
		{
			var iDamage = (int)damage;

			if (delay == TimeSpan.Zero)
			{
				// return if return override has been encountered
				// NOTE this would skip an XmlOnHit attachment's action
				if ((XmlScript.HasTrigger(from, TriggerName.onGivenHit) &&
					 UberScriptTriggers.Trigger(from, target, TriggerName.onGivenHit, null, null, spell, iDamage)) ||
					(XmlScript.HasTrigger(target, TriggerName.onTakenHit) &&
					 UberScriptTriggers.Trigger(target, from, TriggerName.onTakenHit, null, null, spell, iDamage)))
				{
					return;
				}

				if (from != null)
				{
					from.DoHarmful(target);
					if (from is BaseCreature)
					{
						((BaseCreature)from).AlterSpellDamageTo(target, ref iDamage);
					}
				}

				if (target is BaseCreature)
				{
					((BaseCreature)target).AlterSpellDamageFrom(from, ref iDamage);
				}

				target.Damage(iDamage, from);

				if (from is BaseCreature && target != null && delay == TimeSpan.Zero)
				{
					((BaseCreature)from).OnGaveSpellDamage(target);
				}

				if (target is BaseCreature && from != null && delay == TimeSpan.Zero)
				{
					var c = (BaseCreature)target;

					c.OnHarmfulSpell(from);
					c.OnDamagedBySpell(from);
				}
			}
			else
			{
				if (target.Player)
				{
					from.DoHarmful(target);
				}
				new SpellDamageTimer(spell, target, from, iDamage, delay).Start();
			}
		}

		public static void Heal(int amount, Mobile target, Mobile from)
		{
			Heal(amount, target, from, true);
		}

		public static void Heal(int amount, Mobile target, Mobile from, bool message)
		{
			//TODO: All Healing *spells* go through ArcaneEmpowerment
			target.Heal(amount, from, message);
		}

		private class SpellDamageTimer : Timer
		{
			private readonly Mobile m_Target;
			private readonly Mobile m_From;
			private int m_Damage;
			private readonly Spell m_Spell;

			public SpellDamageTimer(Spell s, Mobile target, Mobile from, int damage, TimeSpan delay)
				: base(delay)
			{
				m_Target = target;
				m_From = from;
				m_Damage = damage;
				m_Spell = s;

				if (m_Spell != null && m_Spell.DelayedDamage && !m_Spell.DelayedDamageStacking)
				{
					m_Spell.StartDelayedDamageContext(target, this);
				}

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				// return if return override has been encountered
				// NOTE this would skip an XmlOnHit attachment's action
				if ((XmlScript.HasTrigger(m_From, TriggerName.onGivenHit) &&
					 UberScriptTriggers.Trigger(m_From, m_Target, TriggerName.onGivenHit, null, null, m_Spell, m_Damage)) ||
					(XmlScript.HasTrigger(m_Target, TriggerName.onTakenHit) &&
					 UberScriptTriggers.Trigger(m_Target, m_From, TriggerName.onTakenHit, null, null, m_Spell, m_Damage)))
				{
					return;
				}

				if (m_From != null)
				{
					if (!m_Target.Player)
					{
						m_From.DoHarmful(m_Target);
					}

					if (m_From is BaseCreature)
					{
						((BaseCreature)m_From).AlterSpellDamageTo(m_Target, ref m_Damage);
					}
				}

				if (m_Target is BaseCreature)
				{
					((BaseCreature)m_Target).AlterSpellDamageFrom(m_From, ref m_Damage);
				}

				if (m_Spell is EarthquakeSpell)
				{
					m_Damage = m_Target.Hits / 2;
					if (m_Damage > 75)
					{
						m_Damage = 75;
					}
				}

				m_Target.Damage(m_Damage, m_From);

				if (m_Spell != null)
				{
					m_Spell.RemoveDelayedDamageContext(m_Target);
				}

				if (m_From is BaseCreature && m_Target != null)
				{
					((BaseCreature)m_From).OnGaveSpellDamage(m_Target);
				}

				if (m_Target is BaseCreature && m_From != null)
				{
					var c = (BaseCreature)m_Target;

					c.OnHarmfulSpell(m_From);
					c.OnDamagedBySpell(m_From);
				}

				if (m_Spell != null)
				{
					m_Spell.RemoveDelayedDamageContext(m_Target);
				}
			}
		}
	}

	public class TransformationSpellHelper
	{
		#region Context Stuff
		private static readonly Dictionary<Mobile, TransformContext> m_Table = new Dictionary<Mobile, TransformContext>();

		public static void AddContext(Mobile m, TransformContext context)
		{
			m_Table[m] = context;
		}

		public static void RemoveContext(Mobile m)
		{
			RemoveContext(m, true);
		}

		public static void RemoveContext(Mobile m, bool resetGraphics)
		{
			TransformContext context = GetContext(m);

			if (context != null)
			{
				RemoveContext(m, context, resetGraphics);
			}
		}

        public static void RemoveContextNoHue(Mobile m, bool resetGraphics, bool resetHue)
        {
            TransformContext context = GetContext(m);

            if (context != null)
            {
                RemoveContextNoHue(m, context, resetGraphics, resetHue);
            }
        }

		public static void RemoveContext(Mobile m, TransformContext context, bool resetGraphics)
		{
			if (m_Table.ContainsKey(m))
			{
				m_Table.Remove(m);

				if (resetGraphics)
				{
					m.HueMod = -1;
					m.BodyMod = 0;
				}

				if (context.Timer != null)
				{
					context.Timer.Stop();
				}
				if (context.Spell != null)
				{
					context.Spell.RemoveEffect(m);
				}
			}
		}

        public static void RemoveContextNoHue(Mobile m, TransformContext context, bool resetGraphics, bool resetHue)
        {
            if (m_Table.ContainsKey(m))
            {
                m_Table.Remove(m);

                if (resetGraphics)
                {
                    if (resetHue)
                        m.HueMod = -1;
                    m.BodyMod = 0;
                }

                if (context.Timer != null)
                {
                    context.Timer.Stop();
                }
                if (context.Spell != null)
                {
                    context.Spell.RemoveEffect(m);
                }
            }
        }

		public static TransformContext GetContext(Mobile m)
		{
			TransformContext context = null;

			m_Table.TryGetValue(m, out context);

			return context;
		}

		public static bool UnderTransformation(Mobile m)
		{
			return (GetContext(m) != null);
		}

		public static bool UnderTransformation(Mobile m, Type type)
		{
			TransformContext context = GetContext(m);

			return (context != null && context.Type == type);
		}
		#endregion

		public static bool CheckCast(Mobile caster, Spell spell)
		{
			if (Sigil.ExistsOn(caster))
			{
				caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
				return false;
			}
			else if (!caster.CanBeginAction(typeof(PolymorphSpell)))
			{
				caster.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
				return false;
			}

			return true;
		}

		public static bool OnCast(Mobile caster, Spell spell)
		{
			var transformSpell = spell as ITransformationSpell;

			if (transformSpell == null)
			{
				return false;
			}

			if (Sigil.ExistsOn(caster))
			{
				caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
			}
			else if (!caster.CanBeginAction(typeof(PolymorphSpell)))
			{
				caster.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
			}
			else if (DisguiseTimers.IsDisguised(caster))
			{
				caster.SendLocalizedMessage(1061631); // You can't do that while disguised.
				return false;
			}
			else if (!caster.CanBeginAction(typeof(IncognitoSpell)) || (caster.IsBodyMod && GetContext(caster) == null))
			{
				spell.DoFizzle();
			}
			else if (spell.CheckSequence())
			{
				TransformContext context = GetContext(caster);
				Type ourType = spell.GetType();

				bool wasTransformed = (context != null);
				bool ourTransform = (wasTransformed && context.Type == ourType);

				if (wasTransformed)
				{
					RemoveContext(caster, context, ourTransform);

					if (ourTransform)
					{
						caster.PlaySound(0xFA);
						caster.FixedParticles(0x3728, 1, 13, 5042, EffectLayer.Waist);
					}
				}

				if (!ourTransform)
				{
					if (!((Body)transformSpell.Body).IsHuman)
					{
						IMount mt = caster.Mount;

						if (mt != null)
						{
							mt.Rider = null;
						}
					}

					caster.BodyMod = transformSpell.Body;
					caster.HueMod = transformSpell.Hue;

					transformSpell.DoEffect(caster);

					Timer timer = new TransformTimer(caster, transformSpell);
					timer.Start();

					AddContext(caster, new TransformContext(timer, ourType, transformSpell));
					return true;
				}
			}

			return false;
		}
	}

	public interface ITransformationSpell
	{
		int Body { get; }
		int Hue { get; }

		int PhysResistOffset { get; }
		int FireResistOffset { get; }
		int ColdResistOffset { get; }
		int PoisResistOffset { get; }
		int NrgyResistOffset { get; }

		double TickRate { get; }
		void OnTick(Mobile m);

		void DoEffect(Mobile m);
		void RemoveEffect(Mobile m);
	}

	public class TransformContext
	{
		private readonly Timer m_Timer;
		private readonly Type m_Type;
		private readonly ITransformationSpell m_Spell;

		public Timer Timer { get { return m_Timer; } }
		public Type Type { get { return m_Type; } }
		public ITransformationSpell Spell { get { return m_Spell; } }

		public TransformContext(Timer timer, Type type, ITransformationSpell spell)
		{
			m_Timer = timer;
			m_Type = type;
			m_Spell = spell;
		}
	}

	public class TransformTimer : Timer
	{
		private readonly Mobile m_Mobile;
		private readonly ITransformationSpell m_Spell;

		public TransformTimer(Mobile from, ITransformationSpell spell)
			: base(TimeSpan.FromSeconds(spell.TickRate), TimeSpan.FromSeconds(spell.TickRate))
		{
			m_Mobile = from;
			m_Spell = spell;

			Priority = TimerPriority.TwoFiftyMS;
		}

		protected override void OnTick()
		{
			if (m_Mobile.Deleted || !m_Mobile.Alive || m_Mobile.Body != m_Spell.Body || m_Mobile.Hue != m_Spell.Hue)
			{
				TransformationSpellHelper.RemoveContext(m_Mobile, true);
				Stop();
			}
			else
			{
				m_Spell.OnTick(m_Mobile);
			}
		}
	}

	public delegate bool TravelValidator(Mobile caster, Map map, Point3D loc, TravelCheckType type);

	public class TravelRestrictionValidator
	{
		private readonly TravelCheckType m_CheckTypes;
		private readonly TravelValidator m_Validator;

		public TravelCheckType CheckTypes { get { return m_CheckTypes; } }
		public TravelValidator Validator { get { return m_Validator; } }

		public TravelRestrictionValidator(TravelValidator validator, TravelCheckType checktypes)
		{
			m_Validator = validator;
			m_CheckTypes = checktypes;
		}

		public bool CanTravel(TravelCheckType type)
		{
			return (m_CheckTypes & type) != 0;
		}
	}
}