using System;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "a spider corpse" )]
	public class BigBertha : BaseCreature
	{
		public override string DefaultName{ get{ return "Monkey"; } }

		[Constructable]
		public BigBertha() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.2 )
		{
			Title = "Queen";
			Body = 735;
			Hue = 0;
			BaseSoundID = 0x388;

			SetStr( 1250, 1400 );
			SetDex( 155, 200 );
			SetInt( 250 );

			SetHits( 2500, 3000 );

			SetDamage( 20, 30 );

			SetSkill( SkillName.EvalInt, 120.0 );
			SetSkill( SkillName.Magery, 120.0 );
			SetSkill( SkillName.Meditation, 120.0 );
			SetSkill( SkillName.MagicResist, 250.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0 );
			SetSkill( SkillName.Healing, 120.0 );
			SetSkill( SkillName.Swords, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0 );
			SetSkill( SkillName.Poisoning, 120.0 );

			Fame = 600;
			Karma = -600;

			VirtualArmor = 60;
		}

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( from != null && from.Map != null )
			{
				int amt = 0;

				Mobile target = this;

				if ( willKill )
					amt = 3 + ((Utility.Random( 6 ) % 5) >> 2); // 20% = 1, 20% = 2, 60% = 0

				if ( Hits < 750 )
				{
					double rand = Utility.RandomDouble();

					if ( 0.10 > rand )
						target = from;

					if ( 0.20 > rand )
						amt++;
				}

				if ( amt > 0 )
				{
					SpillAcid( target, amt );

					if ( willKill )
						from.SendMessage( "Your body explodes into a pile of venom!" );
					else
						from.SendMessage( "The creature spits venom at you!" );
				}
			}

			base.OnDamage( amount, from, willKill );
		}

		public void SpawnGiantSpider( Mobile m )
		{
			Map map = this.Map;

			if ( map != null )
			{
				GiantSpider spawned = new GiantSpider();

				spawned.Team = this.Team;

				bool validLocation = false;
				Point3D loc = this.Location;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = X + Utility.Random( 3 ) - 1;
					int y = Y + Utility.Random( 3 ) - 1;
					int z = map.GetAverageZ( x, y );

					if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
						loc = new Point3D( x, y, Z );
					else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				spawned.MoveToWorld( loc, map );
				spawned.Combatant = m;
			}
		}

		public void EatGiantSpider()
		{
			List<Mobile> toEat = new List<Mobile>();
  
			foreach ( Mobile m in this.GetMobilesInRange( 2 ) )
				if ( m is GiantSpider )
					toEat.Add( m );

			if ( toEat.Count > 0 )
			{
				PlaySound( Utility.Random( 0x3B, 2 ) ); // Eat sound

				foreach ( Mobile m in toEat )
				{
					Hits += ( m.Hits / 2 );
					m.Delete();
				}
			}
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( 0.25 >= Utility.RandomDouble() )
			{
				if ( this.Hits > (this.HitsMax / 4) )
					SpawnGiantSpider( attacker );
				else
					EatGiantSpider();
			}
		}

		public override void OnThink()
		{
			base.OnThink();

			if ( Combatant != null && DateTime.UtcNow > m_NextWeb )
				DoWebAttack();
		}

		private DateTime m_NextWeb;

		public void DoWebAttack()
		{
			List<Mobile> targets = new List<Mobile>();

			foreach ( Mobile m in GetMobilesInRange( RangePerception ) )
				if ( CanBeHarmful( m ) && m.Player && !InRange( m, 1 ) && !m.Paralyzed )
					targets.Add( m );

			if ( targets.Count > 0 )
			{
				Mobile target = targets[Utility.Random(targets.Count)];
				TimeSpan delay = TimeSpan.FromSeconds( GetDistanceToSqrt( target ) / 15.0 );
				Effects.SendMovingEffect( this, target, 0x10D2, 20, 1, false, false );
				Timer.DelayCall<Mobile>( delay, new TimerStateCallback<Mobile>( Entangle ), target );
			}

			m_NextWeb = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.RandomMinMax( 15, 30 ) );
		}

		public void Entangle( Mobile m )
		{
			Point3D p = Location;

			if ( SpellHelper.FindValidSpawnLocation( Map, ref p, true ) )
			{
				TimeSpan delay = TimeSpan.FromSeconds( Utility.RandomMinMax( 5, 10 ) );
				m.MoveToWorld( p, Map );
				m.Freeze( delay );
				m.SendLocalizedMessage( 1042555 ); // You become entangled in the spider web.

				SpidersWeb web = new SpidersWeb( m, delay );
				p.Z += 2;
				web.MoveToWorld( p, Map );

				Combatant = m;
			}
		}

		private class SpidersWeb : Static, ICarvable
		{
			private Timer m_DeleteTimer;
			private Mobile m_Frozen;

			public SpidersWeb( Mobile m, TimeSpan delay ) : base( 0x10D2 )
			{
				m_Frozen = m;
				m_DeleteTimer = Timer.DelayCall( delay, new TimerCallback( Delete ) );
			}

			public void Carve( Mobile from, Item item )
			{
				from.SendMessage( "You destroy the web." );

				if ( m_DeleteTimer != null )
					m_DeleteTimer.Stop();

				if ( m_Frozen != null )
					m_Frozen.Frozen = false;

				Delete();
			}

			public SpidersWeb( Serial serial ) : base( serial )
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

				Delete();
			}
		}

		public override void GenerateLoot( bool spawning )
		{
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.Rich, 2 );
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.UltraRich );

			AddLoot( LootPack.Potions, 4 );
			AddLoot( LootPack.Gems, 4 );
			AddLoot( LootPack.HighScrolls, 4 );
			AddLoot( LootPack.MedScrolls, 3 );

			if ( !spawning )
			{
				if ( 0.05 > Utility.RandomDouble() )
					PackBagofRegs( Utility.RandomMinMax( 25, 45 ) );

				if ( 0.25 > Utility.RandomDouble() )
					PackBagofRegs( Utility.RandomMinMax( 10, 20 ) );
			}
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath( c );	

			double rand = Utility.RandomDouble();

			if ( 0.0025 > rand ) //Giant Web
				c.AddItem( new GiantWebEastDeed() );
			else if ( 0.0050 > rand ) //Giant Web
				c.AddItem( new GiantWebSouthDeed() );
			else if ( 0.0060 > rand ) //Cacoon
				c.AddItem( new Cocoon() );
			else if ( 0.0065 > rand ) //Large Web
				c.AddItem( new LargeWebEast() );
			else if ( 0.0070 > rand ) //Large Web
				c.AddItem( new LargeWebSouth() );
			else if ( 0.0080 > rand ) //Egg Case
				c.AddItem( new EggCase() );
			else if ( 0.0085 > rand ) //Medium Web
				c.AddItem( new MediumWebEast() );
			else if ( 0.0090 > rand ) //Medium Web
				c.AddItem( new MediumWebSouth() );
			else if ( 0.0095 > rand ) //Small Web
				c.AddItem( new SmallWebEast() );
			else if ( 0.01 > rand ) //Small Web
				c.AddItem( new SmallWebSouth() );
		}
	
		public BigBertha( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
