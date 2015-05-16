using System;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "a spider corpse" )]
	public class BananaSpider : BaseCreature
	{
		public override string DefaultName{ get{ return "a banana spider"; } }

		[Constructable]
		public BananaSpider() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 11;
			Hue = 55;
			BaseSoundID = 0x388;

			SetStr( 76, 100 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 300, 350 );
			SetDamage( 12, 19 );

			SetSkill( SkillName.Poisoning, 60.1, 80.0 );
			SetSkill( SkillName.MagicResist, 25.1, 40.0 );
			SetSkill( SkillName.Tactics, 35.1, 50.0 );
			SetSkill( SkillName.Wrestling, 50.1, 65.0 );

			Fame = 600;
			Karma = -600;

			VirtualArmor = 16;
			PackItem( new SpidersSilk( 5 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.Gems );
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath( c );

			double rand = Utility.RandomDouble();

			if ( 0.005 > rand ) //Medium Web
				c.AddItem( new MediumWebEast() );
			else if ( 0.01 > rand ) //Medium Web
				c.AddItem( new MediumWebSouth() );
		}

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return Poison.Deadly; } }

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
                Mobile target = targets[Utility.Random( targets.Count )];
                TimeSpan delay = TimeSpan.FromSeconds( GetDistanceToSqrt( target ) / 15.0 );
                Effects.SendMovingEffect( this, target, 0x10D2, 20, 1, false, false );
                Timer.DelayCall<Mobile>( delay, new TimerStateCallback<Mobile>( Entangle ), target );
            }

            m_NextWeb = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.Random( 5, 11 ) );
        }

        public void Entangle( Mobile m )
        {
            Point3D p = Location;

            if ( SpellHelper.FindValidSpawnLocation( Map, ref p, true ) )
            {
                TimeSpan delay = TimeSpan.FromSeconds( Utility.Random( 3, 4 ) );
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

		public BananaSpider( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
