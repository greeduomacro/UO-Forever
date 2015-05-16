using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a turkey corpse" )]
	public class ThanksgivingTurkey : BaseCreature
	{
		[Constructable]
		public ThanksgivingTurkey () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a thanksgiving Turkey";
			Body = 208;
			BaseSoundID = 110;
			Hue = 446;
			SetStr( 1096, 1185 );
			SetDex( 86, 175 );
			SetInt( 686, 775 );

			SetHits( 700, 811 );

			SetDamage( 31, 37 );
			ThanksgivingTurkeyDropPack();
			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 80.1, 100.0 );
			SetSkill( SkillName.Magery, 80.1, 100.0 );
			SetSkill( SkillName.Meditation, 52.5, 75.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 97.6, 100.0 );

			Fame = 27500;
			Karma = -27500;
			PackItem( new Thanksgiving2012() );
			VirtualArmor = 70;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 5 );
			AddLoot( LootPack.Gems, 5 );
		}
		
		public virtual void ThanksgivingTurkeyDropPack()
		{
			PackItem( new BreadLoaf(4) );
			PackItem( new CheeseWheel(3) );
			PackItem( new FrenchBread(4) );
			PackItem( new CookedBird(50) );
			PackItem( new Cookies(10) );
			PackItem( new Muffins(6) );
			PackItem( new FruitPie() );
			PackItem( new MeatPie() );
			PackItem( new PumpkinPie() );
			PackItem( new ApplePie() );
			PackItem( new PeachCobbler() );
			PackItem( new YellowGourd(3) );
			PackItem( new GreenGourd(5) );
			PackItem( new EarOfCorn(20) );
			PackItem( new Turnip(6) );
			PackItem( new FruitBasket() );
			PackItem( new Dates(10) );
			PackItem( new Grapes(6) );
			PackItem( new Peach(7) );
			PackItem( new Pear(5) );
			PackItem( new Apple(8) );
			PackItem( new Squash(2) );
			PackItem( new Carrot(5) );
			PackItem( new Cabbage(6) );
			PackItem( new Onion(3) );
			PackItem( new Lettuce(6) );
			PackItem( new Pumpkin() );
			PackItem( new SmallPumpkin() );
		}

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Hides{ get{ return 40; } }
		public override int Meat{ get{ return 19; } }
		public override int Scales{ get{ return 12; } }
		public override ScaleType ScaleType{ get{ return (ScaleType)Utility.Random( 4 ); } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override Poison HitPoison{ get{ return Utility.RandomBool() ? Poison.Lesser : Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 5; } }

		public ThanksgivingTurkey( Serial serial ) : base( serial )
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