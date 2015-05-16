using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a spider corpse" )]
	public class WolfSpider : BaseCreature
	{
		public override string DefaultName{ get{ return "a wolf spider"; } }

		[Constructable]
		public WolfSpider() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 737;
			BaseSoundID = 0x388;

			SetStr( 76, 100 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 60 );
			SetMana( 0 );

			SetDamage( 5, 13 );

			Fame = 600;
			Karma = -600;

			VirtualArmor = 16;

			PackItem( new SpidersSilk( 5 ) );
		}

		private DateTime m_NextAbility;

		public override void OnThink()
		{
			if ( DateTime.UtcNow >= m_NextAbility )
			{
				int count = 0;
				int maxhits = HitsMaxSeed;

				foreach( Mobile mob in GetMobilesInRange( 10 ) )
					if ( mob is WolfSpider )
						count++;

				int newhits = 60 + ( count * 10 );

				if ( maxhits != newhits )
				{
					int diff = newhits - maxhits;

					HitsMaxSeed = newhits;

					if ( Hits + diff <= 0 )
						Kill();
					else
						Hits += diff;
				}

				m_NextAbility = DateTime.UtcNow + TimeSpan.FromSeconds( 5.0 );
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.Gems );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			double rand = Utility.RandomDouble();

			if ( 0.002 > rand ) //Small Web
				c.AddItem( new SmallWebEast() );
			else if ( 0.004 > rand ) //Small Web
				c.AddItem( new SmallWebSouth() );
		}

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override Poison HitPoison{ get{ return Poison.Regular; } }

		public WolfSpider( Serial serial ) : base( serial )
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
