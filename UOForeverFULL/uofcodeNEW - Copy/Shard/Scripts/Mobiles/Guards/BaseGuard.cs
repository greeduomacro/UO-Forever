using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Factions;
using Server.SkillHandlers;
//using Server.BountySystem;  // removed bounty system
using Server.Commands;
using Server.Engines.XmlSpawner2;

namespace Server.Mobiles
{
	public abstract class BaseGuard : Mobile, IBegged
	{
		private TimeSpan m_SpeechDelay = TimeSpan.FromSeconds( 300.0 ); // time between speech
		public DateTime m_NextSpeechTime;
		public static void Spawn( Mobile caller, Mobile target )
		{
			Spawn( caller, target, 1, false );
		}

		public static void Spawn( Mobile caller, Mobile target, int amount, bool onlyAdditional )
		{
			if ( target == null || target.Deleted )
				return;

			foreach ( Mobile m in target.GetMobilesInRange( 15 ) )
			{
				if ( m is BaseGuard )
				{
					BaseGuard g = (BaseGuard)m;

					if ( g.Focus == null ) // idling
					{
						g.Focus = target;

						--amount;
					}
					else if ( g.Focus == target && !onlyAdditional )
					{
						--amount;
					}
				}
			}

			while ( amount-- > 0 )
				caller.Region.MakeGuard( target );
		}

		public BaseGuard( Mobile target, Faction faction )
		{
			InitBody( faction );

			if ( target != null )
			{
				if ( faction == null )
				{
					Town town = Town.FromRegion( Region.Find( target.Location, target.Map ) );
					if ( town != null )
						faction = town.Owner;
				}

				InitOutfit( faction );

				Location = target.Location;
				Map = target.Map;

				Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
			}
			else
				InitOutfit( faction );
		}

		public BaseGuard( Serial serial ) : base( serial )
		{
		}

		public abstract void InitBody( Faction faction );
		public abstract void InitOutfit( Faction faction );

		#region Begging
		public virtual bool CanBeBegged( Mobile from )
		{
			return false;
		}

		public virtual void OnBegged( Mobile beggar )
		{
		}
		#endregion

		public Item InitItem( Item item )
		{
			return InitItem( item, 0 );
		}

		public Item InitItem( Item item, int hue )
		{
			if ( item is BaseArmor )
				((BaseArmor)item).Identified = true;
			else if ( item is BaseWeapon )
				((BaseWeapon)item).Identified = true;
			else if ( item is BaseClothing )
				((BaseClothing)item).Identified = true;

			item.Movable = false;
			item.Hue = hue;

			return item;
		}

		public override bool OnBeforeDeath()
		{
			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );

			PlaySound( 0x1FE );

			Delete();

			return false;
		}
		
		public override bool OnDragDrop( Mobile from, Item dropped )
		{
            // trigger returns true if returnoverride
            if (XmlScript.HasTrigger(this, TriggerName.onDragDrop) && UberScriptTriggers.Trigger(this, from, TriggerName.onDragDrop, dropped))
                return true;
            bool bReturn = false;
			/*if( dropped is Head )  //Removed for bounty system removal
			{
				int result = 0;
				int goldGiven = 0;
				bReturn = BountyKeeper.CollectBounty( (Head)dropped, from, this, ref goldGiven, ref result );
				switch(result)
				{
					case -2:
						Say("You disgusting miscreant!  Why are you giving me an innocent person's head?");
						break;
					case -3:
						Say("I suspect treachery....");
						Say("I'll take that head, you just run along now.");
						break;
					case 1: //good, gold given
						Say( string.Format("My thanks for slaying this vile person.  Here's the reward of {0} gold!", goldGiven) );
						break;
					default:
						if( bReturn )
						{
							Say("I'll take that.");
						}
						else
						{
							Say("I don't want that.");
						}
						break;
				}
			}*/
			return bReturn;
		}
		
		public override void OnMovement( Mobile m, Point3D oldLocation  )
		{
		
		
			if ( m.Player && m.AccessLevel == AccessLevel.Player && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild && ((Mobile)m).Hidden == false && m.InRange( this, 3 ) && DateTime.UtcNow >= m_NextSpeechTime )
					{
						if (Utility.RandomDouble() < 0.03)
							{
								m_NextSpeechTime = DateTime.UtcNow + m_SpeechDelay;
								
								switch( Utility.Random( 5 ) )
								{
									case 0:
										this.Say( "Beware a thief is in our midst." ); break;
									case 1:
										this.Say( "Beware, {0}. For I know your true intentions.", m.Name ); break;
									case 2:
										this.Say( "Back away dirty thief, my possesions are no concern of yours." ); break;
									case 3:
										this.Say( "Citizens! {0} seeks to relieve you of your belongings.", m.Name ); break;
									case 4:
										this.Say( "Take heed scum, any thieving in these parts and thou shall feel my steel." ); break;
								}
							}		
						}
					
			base.OnMovement( m, oldLocation );
		}

		public abstract Mobile Focus{ get; set; }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}