using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName("a spider corpse")]
	public class CamelSpider : BaseCreature
	{
		public override string DefaultName{ get{ return "a camel spider"; } }
		private bool m_CanHatch;

		[Constructable]
		public CamelSpider() : this( true )
		{
		}

		[Constructable]
		public CamelSpider( bool canhatch ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			m_CanHatch = canhatch;

			if ( !canhatch )
				m_NextAbility = DateTime.UtcNow + TimeSpan.FromMinutes( 3.0 );

			Body = 48;
			Hue = 546;
			BaseSoundID = 397;

			SetStr( 73, 115 );
			SetDex( 100, 120 );
			SetInt( 16, 30 );

			SetHits( 400, 500 );

			SetDamage( 15, 20 );

			SetSkill( SkillName.Poisoning, 80.1, 100.0 );
			SetSkill( SkillName.MagicResist, 30.1, 35.0 );
			SetSkill( SkillName.Tactics, 60.3, 75.0 );
			SetSkill( SkillName.Wrestling, 60.3, 75.0 );

			Fame = 2000;
			Karma = -2000;

			VirtualArmor = 28;
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			if ( m_CanHatch && 0.01 > Utility.RandomDouble() ) //Egg Case
				c.AddItem( new EggCase() );
		}

		private DateTime m_NextAbility;

		public override void OnThink()
		{
			if ( m_CanHatch )
			{
				if ( DateTime.UtcNow >= m_NextAbility && Combatant != null && Combatant.Alive && Combatant.InRange( this, RangePerception ) )
				{
					Timer.DelayCall( TimeSpan.FromSeconds( 30.0 ), new TimerCallback( Hatch ) );
					m_NextAbility = DateTime.UtcNow + TimeSpan.FromSeconds( 35 + Utility.Random( 5 ) );
				}
			}
			else if ( DateTime.UtcNow < m_NextAbility && Combatant != null && Combatant.Alive && Combatant.InRange( this, RangePerception ) )
				m_NextAbility = DateTime.UtcNow + TimeSpan.FromMinutes( 3.0 );
			else
				Delete();
		}

		public void Hatch()
		{
			if ( !Deleted && Map != null )
			{
				CamelSpiderEgg spawned = new CamelSpiderEgg();

				spawned.MoveToWorld( Location, Map );
			}
		}

		public override int Meat { get { return 1; } }
		public override FoodType FavoriteFood { get { return FoodType.Meat; } }
		public override PackInstinct PackInstinct { get { return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override Poison HitPoison { get { return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly); } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.Gems );
		}

		public CamelSpider( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( m_CanHatch );

			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			m_CanHatch = reader.ReadBool();

			if ( !m_CanHatch )
				m_NextAbility = DateTime.UtcNow + TimeSpan.FromMinutes( 3.0 );

			int version = reader.ReadEncodedInt();
		}
	}
}
