using System;

using Server.Factions;
using Server.Misc;
using Server.Targeting;
using Server.Network;
using Server.Regions;
using Server.Items;
using Server.Mobiles;
using Server.Engines.CannedEvil;
using Server.Engines.XmlSpawner2;
using VitaNex.Modules.AutoPvP;
using VitaNex.Modules.AutoPvP.Battles;

namespace Server.Spells.Third
{
	public class TeleportSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Teleport", "Rel Por",
				215,
				9031,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle { get { return SpellCircle.Third; } }

		public TeleportSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if (Sigil.ExistsOn(Caster))
				Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
			else if (WeightOverloading.IsOverloaded(Caster))
				Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
			else
				return SpellHelper.CheckTravel(Caster, TravelCheckType.TeleportFrom);

			return false;
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			IPoint3D orig = p;
			Map map = Caster.Map;

			SpellHelper.GetSurfaceTop( ref p );

		    var player = Caster as PlayerMobile;

			Point3D from = Caster.Location;
			Point3D to = new Point3D( p );

			ChampionSpawnRegion champregion = Region.Find( new Point3D( p ), map ).GetRegion( typeof( ChampionSpawn ) ) as ChampionSpawnRegion;

            CustomRegion customRegion = Region.Find(new Point3D(p), map) as CustomRegion;
            CustomRegion customRegionFrom = Region.Find(new Point3D(Caster), map) as CustomRegion;
            if (customRegion != null && customRegion.Controller != null && (!customRegion.Controller.CanEnter || customRegion.Controller.IsRestrictedSpell(this)))
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            else if (customRegionFrom != null && customRegionFrom.Controller != null && customRegionFrom.Controller.IsRestrictedSpell(this))
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            else if ( Factions.Sigil.ExistsOn( Caster ) )
				Caster.SendLocalizedMessage( 1061632 ); // You can't do that while carrying the sigil.
			else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
            else if (player != null && player.PokerGame != null)
            {
                player.SendMessage(61, "You cannot cast this spell whilst playing poker.");
            }
			else if ( !SpellHelper.CheckTravel( Caster, TravelCheckType.TeleportFrom ) )
			{
			}
			else if ( !SpellHelper.CheckTravel( Caster, map, to, TravelCheckType.TeleportTo ) )
			{
			}
			else if ( map == null || !map.CanSpawnMobile( p.X, p.Y, p.Z ) )
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			else if ( SpellHelper.CheckMulti( to, map ) )
				Caster.SendLocalizedMessage( 502831 ); // Cannot teleport to that spot.
			else if ( Region.Find( to, map ).GetRegion( typeof( HouseRegion ) ) != null )
				Caster.SendLocalizedMessage( 502829 ); // Cannot teleport to that spot.
            else if (customRegion != null && customRegion.m_Controller.IsSafeZone && Caster.InCombat(TimeSpan.FromSeconds(60.0)) && Caster.AccessLevel == AccessLevel.Player)
            {
                Caster.SendMessage(54, "You cannot enter a safe zone after recently having been in combat!");
            }
			else if ( (!Caster.Alive || (Caster is PlayerMobile && ((PlayerMobile)Caster).Young)) && champregion != null && !champregion.CanSpawn() )
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			else if ( CheckSequence() )
			{
				SpellHelper.Turn( Caster, orig );

				Mobile m = Caster;

				m.Location = to;
				m.ProcessDelta();

				if ( m.Player )
				{
					Effects.SendLocationParticles( EffectItem.Create( from, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
					Effects.SendLocationParticles( EffectItem.Create(   to, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
				}
				else
					m.FixedParticles( 0x376A, 9, 32, 0x13AF, EffectLayer.Waist );

				m.PlaySound( 0x1FE );

				IPooledEnumerable eable = m.GetItemsInRange( 0 );

				foreach ( Item item in eable )
					if ( item is Server.Spells.Sixth.ParalyzeFieldSpell.InternalItem || item is Server.Spells.Fifth.PoisonFieldSpell.InternalItem || item is Server.Spells.Fourth.FireFieldSpell.FireFieldItem )
						item.OnMoveOver( m );

				eable.Free();
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private TeleportSpell m_Owner;

			public InternalTarget(TeleportSpell owner)
				: base(owner.Caster.EraML ? 11 : 12, true, TargetFlags.None)
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
                IEntity entity = o as IEntity; if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) && UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, m_Owner))
                {
                    return;
                }
                IPoint3D p = o as IPoint3D;

				if ( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}