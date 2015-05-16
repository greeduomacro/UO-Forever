using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a polar bear's corpse" )]
	[TypeAlias( "Server.Mobiles.EnragedPolarbear" )]
	public class EnragedPolarBear : BaseCreature
	{
		public override string DefaultName{ get{ return "an enraged polar bear"; } }

		[Constructable]
		public EnragedPolarBear() : base( AIType.AI_Animal, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 213;
			BaseSoundID = 0xA3;

			SetStr( 725, 750 );
			SetDex( 425, 450 );
			SetInt( 26, 50 );

			SetHits( 950, 1000 );
			SetMana( 0 );

			SetDamage( 40, 50 );

			Hue = 1034;

			

			
			
			
			

			SetSkill( SkillName.MagicResist, 45.1, 60.0 );
			SetSkill( SkillName.Tactics, 60.1, 90.0 );
			SetSkill( SkillName.Wrestling, 45.1, 70.0 );

			Fame = 1500;
			Karma = 0;

			VirtualArmor = 50;

			Tamable = false;
			ControlSlots = 1;
			MinTameSkill = 35.1;
		}

		public override int Meat{ get{ return 2; } }
		public override int Hides{ get{ return 16; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.FruitsAndVeggies | FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }

		public EnragedPolarBear( Serial serial ) : base( serial )
		{
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Average, 2 );
		}

		public override void GenerateLoot( bool spawning )
		{
			base.GenerateLoot( spawning );

			if ( !spawning && Utility.Random( 250 ) == 0 )
				PackItem( new PileOfSnow() );
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