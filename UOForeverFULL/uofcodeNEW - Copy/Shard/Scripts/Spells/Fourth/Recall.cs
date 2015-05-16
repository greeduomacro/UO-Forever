using System;
using Server.Factions;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Regions;
using Server.Engines.CannedEvil;
using Server.Engines.XmlSpawner2;

namespace Server.Spells.Fourth
{
	public class RecallSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Recall", "Kal Ort Por",
				239,
				9031,
				Reagent.BlackPearl,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		private RunebookEntry m_Entry;
		private Runebook m_Book;

		public RecallSpell( Mobile caster, Item scroll ) : this( caster, scroll, null, null )
		{
		}

		public RecallSpell( Mobile caster, Item scroll, RunebookEntry entry, Runebook book ) : base( caster, scroll, m_Info )
		{
			m_Entry = entry;
			m_Book = book;
		}

		public override void OnCast()
		{
			if ( m_Entry == null )
				Caster.Target = new InternalTarget( this );
			else
				Effect( m_Entry.Location, m_Entry.Map, true );
		}

		public override bool CheckCast()
		{
		    var pm = Caster as PlayerMobile;
			if ( Factions.Sigil.ExistsOn( Caster ) )
				Caster.SendLocalizedMessage( 1061632 ); // You can't do that while carrying the sigil
			else if ( Caster.Criminal )
				Caster.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
			else if ( SpellHelper.CheckCombat( Caster ) )
				Caster.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
			else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
            else if ( Caster is BaseCreature && !((BaseCreature)Caster).Pseu_CanUseRecall)
                Caster.SendMessage("You are not allowed to do that.");
            else if (pm != null && pm.PokerGame != null)
            {
                pm.SendMessage(61, "You cannot recall while playing poker.");
            }
			else
				return SpellHelper.CheckTravel( Caster, TravelCheckType.RecallFrom );

			return false;
		}

		public void Effect( Point3D loc, Map map, bool checkMulti )
		{
            ChampionSpawnRegion champregion = Region.Find( loc, map ).GetRegion( typeof( ChampionSpawn ) ) as ChampionSpawnRegion;
            CustomRegion customRegion = Region.Find(loc, map) as CustomRegion;

            if (Factions.Sigil.ExistsOn(Caster))
                Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            //else if ( map == null || (!Core.AOS && Caster.Map != map) )
            //	Caster.SendLocalizedMessage( 1005569 ); // You can not recall to another facet.
            else if (!SpellHelper.CheckTravel(Caster, TravelCheckType.RecallFrom))
            {
            }
            else if (!SpellHelper.CheckTravel(Caster, map, loc, TravelCheckType.RecallTo))
            {
            }
            else if (SpellHelper.IsWind(loc, map) && Caster.Skills[SkillName.Magery].Base < 70.0)
                Caster.SendLocalizedMessage(503382); // You are not worthy of entrance to the city of Wind!
            else if (Caster.Criminal)
                Caster.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
            else if (SpellHelper.CheckCombat(Caster))
                Caster.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
            else if ((Region.Find(loc, map).IsPartOf(typeof(GuardedRegion))) && Caster.InCombat(TimeSpan.FromSeconds(60.0)) && Caster.Kills >= 5 && Faction.Find(Caster) != null && Caster.AccessLevel == AccessLevel.Player)
            {
                Caster.SendMessage(54, "You cannot enter town as a murderer and having recently been in combat!");
            }
            else if (Server.Misc.WeightOverloading.IsOverloaded(Caster))
                Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
            else if (!map.CanSpawnMobile(loc.X, loc.Y, loc.Z, false))
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            else if ((checkMulti && SpellHelper.CheckMulti(loc, map)))
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            else if ((!Caster.Alive || (Caster is PlayerMobile && ((PlayerMobile)Caster).Young)) && champregion != null && !champregion.CanSpawn())
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            else if (m_Book != null && m_Book.CurCharges <= 0)
                Caster.SendLocalizedMessage(502412); // There are no charges left on that item.
            else if (customRegion != null && customRegion.Controller != null && customRegion.Controller.NoMounts && Caster.Mounted)
                Caster.SendMessage("You cannot travel there while mounted!");
            else if (customRegion != null && customRegion.Controller != null && customRegion.Controller.NoPets && Caster.Followers > 0)
                Caster.SendMessage("Sorry, but no pets are allowed in that region at this time (including your mount).");
            else if (CheckSequence())
            {
                BaseCreature.TeleportPets(Caster, loc, map, true);

                if (m_Book != null)
                    --m_Book.CurCharges;

                Caster.PlaySound(0x1FC);
                Caster.MoveToWorld(loc, map);
                Caster.PlaySound(0x1FC);
            }

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private RecallSpell m_Owner;

			public InternalTarget(RecallSpell owner)
				: base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.None)
			{
				m_Owner = owner;

				owner.Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 501029 ); // Select Marked item.
			}

			protected override void OnTarget( Mobile from, object o )
			{
                IEntity entity = o as IEntity; if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) && UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, m_Owner))
                {
                    return;
                }
                
                if ( o is RecallRune )
				{
					RecallRune rune = (RecallRune)o;

					if ( rune.Marked )
						m_Owner.Effect( rune.Target, rune.TargetMap, true );
					else
						from.SendLocalizedMessage( 501805 ); // That rune is not yet marked.
				}
				else if ( o is Runebook )
				{
					RunebookEntry e = ((Runebook)o).Default;

					if ( e != null )
						m_Owner.Effect( e.Location, e.Map, true );
					else
						from.SendLocalizedMessage( 502354 ); // Target is not marked.
				}
				else if ( o is Key && ((Key)o).KeyValue != 0 && ((Key)o).Link is BaseBoat )
				{
					BaseBoat boat = ((Key)o).Link as BaseBoat;

					if ( !boat.Deleted && boat.CheckKey( ((Key)o).KeyValue ) )
						m_Owner.Effect( boat.GetMarkedLocation(), boat.Map, false );
					else
						from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // I can not recall from that object.
				}
				else
				{
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // I can not recall from that object.
				}
			}

			protected override void OnNonlocalTarget( Mobile from, object o )
			{
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}