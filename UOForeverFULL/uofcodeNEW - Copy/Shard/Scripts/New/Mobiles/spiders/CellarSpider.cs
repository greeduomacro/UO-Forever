using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a spider corpse" )]
	[TypeAlias( "Server.Mobiles.DaddyLongLegs" )]
	public class CellarSpider : BaseCreature
	{
		public override string DefaultName{ get{ return "a cellar spider"; } }

		[Constructable]
		public CellarSpider() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.1 )
		{
			Body = 20;
			Hue = 2054;
			BaseSoundID = 0x388;

			SetStr( 76, 100 );
			SetDex( 209, 225 );
			SetInt( 36, 60 );

			SetHits( 100, 130 );
			SetMana( 0 );

			SetDamage( 5, 13 );

			

			
			

			SetSkill( SkillName.Poisoning, 60.1, 80.0 );
			SetSkill( SkillName.MagicResist, 25.1, 40.0 );
			SetSkill( SkillName.Tactics, 35.1, 50.0 );
			SetSkill( SkillName.Wrestling, 50.1, 65.0 );

			Fame = 600;
			Karma = -600;

			VirtualArmor = 16;

			Tamable = false;
			ControlSlots = 1;
			MinTameSkill = 59.1;

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

			if ( 0.008 > rand ) //Small Web
				c.AddItem( new SmallWebEast() );
			else if ( 0.008 > rand ) //Small Web
				c.AddItem( new SmallWebSouth() );
		}

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override Poison HitPoison{ get{ return Poison.Regular; } }

		public CellarSpider( Serial serial ) : base( serial )
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
