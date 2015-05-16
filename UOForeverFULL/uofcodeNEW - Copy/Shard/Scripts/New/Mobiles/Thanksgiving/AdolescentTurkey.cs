using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a adolescent turkey corpse" )]
	public class AdolescentThanksgivingTurkey : BaseCreature
	{
		[Constructable]
		public AdolescentThanksgivingTurkey () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a adolescent turkey";
			Body = 208;
			BaseSoundID = 110;
			Hue = 444;
			SetStr( 401, 430 );
			SetDex( 133, 152 );
			SetInt( 101, 140 );
			ThanksgivingTurkeyDropPack();
			SetHits( 291, 345 );

			SetDamage( 13, 19 );

			
			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 65.1, 90.0 );
			SetSkill( SkillName.Wrestling, 65.1, 80.0 );

			Fame = 7500;
			Karma = -7500;

			VirtualArmor = 46;

			Tamable = false;
			ControlSlots = 2;
			MinTameSkill = 84.3;

			PackReg( 4 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
			AddLoot( LootPack.MedScrolls, 2 );
		}
		public virtual void ThanksgivingTurkeyDropPack()
		{
			PackItem( new BreadLoaf(3) );
			PackItem( new CheeseWheel(2) );
			PackItem( new FrenchBread(3) );
			PackItem( new CookedBird(35) );
			PackItem( new Cookies(8) );
			PackItem( new Muffins(4) );
			PackItem( new MeatPie() );
			PackItem( new PumpkinPie() );
			PackItem( new PeachCobbler() );
			PackItem( new YellowGourd(2) );
			PackItem( new GreenGourd(3) );
			PackItem( new EarOfCorn(15) );
			PackItem( new Turnip(4) );
			PackItem( new FruitBasket() );
			PackItem( new Dates(8) );
			PackItem( new Grapes(4) );
			PackItem( new Peach(5) );
			PackItem( new Pear(3) );
			PackItem( new Apple(6) );
			PackItem( new Squash(1) );
			PackItem( new Carrot(4) );
			PackItem( new Cabbage(3) );
			PackItem( new Onion(2) );
			PackItem( new Lettuce(4) );
			PackItem( new SmallPumpkin() );
		}
		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int TreasureMapLevel{ get{ return 2; } }
		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override int Scales{ get{ return 2; } }
		public override ScaleType ScaleType{ get{ return ( Body == 60 ? ScaleType.Yellow : ScaleType.Red ); } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public AdolescentThanksgivingTurkey( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}