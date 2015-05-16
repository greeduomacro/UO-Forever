using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a baby turkey corpse" )]
	public class BabyThanksgivingTurkey : BaseCreature
	{
		[Constructable]
		public BabyThanksgivingTurkey() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a baby turkey";
			Body = 208;
			Hue = 442;
			BaseSoundID = 110;
			ThanksgivingTurkeyDropPack();
			SetStr( 22, 34 );
			SetDex( 16, 25 );
			SetInt( 6, 10 );

			SetHits( 30, 38 );
			SetMana( 0 );

			SetDamage( 3, 6 );

			

			
			

			SetSkill( SkillName.Poisoning, 50.1, 70.0 );
			SetSkill( SkillName.MagicResist, 15.1, 20.0 );
			SetSkill( SkillName.Tactics, 19.3, 34.0 );
			SetSkill( SkillName.Wrestling, 19.3, 34.0 );

			Fame = 300;
			Karma = -300;

			VirtualArmor = 16;

			Tamable = false;
			ControlSlots = 1;
			MinTameSkill = 59.1;
		}

		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }
		public override Poison HitPoison{ get{ return Poison.Lesser; } }

		public override bool DeathAdderCharmable{ get{ return true; } }

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Eggs; } }
		public virtual void ThanksgivingTurkeyDropPack()
		{
			PackItem( new BreadLoaf(2) );
			PackItem( new CheeseWheel(1) );
			PackItem( new FrenchBread(2) );
			PackItem( new CookedBird(14) );
			PackItem( new Cookies(6) );
			PackItem( new Muffins(2) );
			PackItem( new PumpkinPie() );
			PackItem( new PeachCobbler() );
			PackItem( new YellowGourd(1) );
			PackItem( new GreenGourd(2) );
			PackItem( new EarOfCorn(10) );
			PackItem( new Turnip(2) );
			PackItem( new FruitBasket() );
			PackItem( new Dates(6) );
			PackItem( new Grapes(2) );
			PackItem( new Peach(3) );
			PackItem( new Pear(2) );
			PackItem( new Apple(4) );
			PackItem( new Carrot(3) );
			PackItem( new Cabbage(2) );
			PackItem( new Onion(1) );
			PackItem( new Lettuce(2) );
		}
		public BabyThanksgivingTurkey(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}