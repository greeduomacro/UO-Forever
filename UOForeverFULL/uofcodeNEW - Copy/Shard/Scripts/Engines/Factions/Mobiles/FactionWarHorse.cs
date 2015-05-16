using System;
using Server;
using Server.ContextMenus;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Factions
{
	[CorpseName( "a war horse corpse" )]
	public class FactionWarHorse : BaseMount
	{
		private Faction m_Faction;

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public Faction Faction
		{
			get{ return m_Faction; }
			set
			{
				m_Faction = value;

				Body = ( m_Faction == null ? 0xE2 : m_Faction.Definition.WarHorseBody );
				ItemID = ( m_Faction == null ? 0x3EA0 : m_Faction.Definition.WarHorseItem );
			}
		}

		public const int SilverPrice = 1000;
		public const int GoldPrice = 6000;

		public override string DefaultName{ get{ return "a war horse"; } }

        public override int InternalItemItemID { get { return (m_Faction == null ? 0x3EA0 : m_Faction.Definition.WarHorseItem); } }

		[Constructable]
		public FactionWarHorse() : this( null )
		{
		}

		public FactionWarHorse( Faction faction ) : base( 0xE2, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			BaseSoundID = 0xA8;

			SetStr( 400, 550 );
			SetDex( 180 );
			SetInt( 51, 55 );

			SetHits( 240 );
			SetMana( 0 );

			SetDamage( 2, 4 );

			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 25.1, 30.0 );
			SetSkill( SkillName.Tactics, 29.3, 44.0 );
			SetSkill( SkillName.Wrestling, 29.3, 44.0 );

			Fame = 300;
			Karma = 300;

			Tamable = true;
			ControlSlots = 1;

			Faction = faction;
		}

		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVeggies | FoodType.GrainsAndHay; } }

		public FactionWarHorse( Serial serial ) : base( serial )
		{
		}
/*
		private DateTime m_TimeOfDeath;

		public override bool OnBeforeDeath()
		{
			m_TimeOfDeath = DateTime.UtcNow;
			return base.OnBeforeDeath();
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( this.IsDeadPet && this.Controlled && this.ControlMaster == from && from.Alive && ( m_TimeOfDeath + TimeSpan.FromSeconds(10) < DateTime.UtcNow ))
				list.Add( new ResurrectEntry( this ) );

			base.GetContextMenuEntries( from, list );
		}
*/
		public override void OnDoubleClick( Mobile from )
		{
			PlayerState pl = PlayerState.Find( from );

			if ( pl == null )
				from.SendLocalizedMessage( 1010366 ); // You cannot mount a faction war horse!
			else if ( pl.Faction != this.Faction )
				from.SendLocalizedMessage( 1010367 ); // You cannot ride an opposing faction's war horse!
			else if ( pl.Rank.Rank < 2 )
				from.SendLocalizedMessage( 1010368 ); // You must achieve a faction rank of at least two before riding a war horse!
			else
				base.OnDoubleClick( from );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			Faction.WriteReference( writer, m_Faction );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					Faction = Faction.ReadReference( reader );
					break;
				}
			}
		}
	}
}

namespace Server.ContextMenus
{
	public class ResurrectEntry : ContextMenuEntry
	{
		private BaseCreature m_Creature;

		public ResurrectEntry( BaseCreature cr ) : base( 489, 1 )
		{
			m_Creature = cr;
		}

		public override void OnClick()
		{
			m_Creature.PlaySound( 0x214 );
			m_Creature.FixedEffect( 0x376A, 10, 16 );
			m_Creature.ResurrectPet();
		}
	}
}