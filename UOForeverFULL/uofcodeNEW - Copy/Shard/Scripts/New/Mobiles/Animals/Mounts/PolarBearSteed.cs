using System;

namespace Server.Mobiles
{
	[CorpseName( "a polar bear corpse" )]
	public class RidablePolarBear : BaseMount
	{
		public override string DefaultName{ get{ return "a ridable polar bear"; } }

        public override int InternalItemItemID { get { return 0x3EC5; } }

		[Constructable]
		public RidablePolarBear() : base( 0xD5, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Body = 213;
			BaseSoundID = 0xA3;

			SetStr( 116, 140 );
			SetDex( 96, 120 );
			SetInt( 26, 50 );

			SetHits( 70, 84 );
			SetMana( 0 );

			SetDamage( 7, 12 );

			

			
			
			
			

			SetSkill( SkillName.MagicResist, 55.1, 65.0 );
			SetSkill( SkillName.Tactics, 65.1, 95.0 );
			SetSkill( SkillName.Wrestling, 50.1, 75.0 );

			Fame = 1500;
			Karma = 0;

			VirtualArmor = 18;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 75.1;
		}

		public override int Meat{ get{ return 2; } }
		public override int Hides{ get{ return 16; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.FruitsAndVeggies | FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }

		public RidablePolarBear( Serial serial ) : base( serial )
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