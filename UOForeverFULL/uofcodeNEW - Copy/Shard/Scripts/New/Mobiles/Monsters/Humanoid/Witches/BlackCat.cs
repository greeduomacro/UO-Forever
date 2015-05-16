using System;

namespace Server.Mobiles
{
	[CorpseName( "a cat corpse" )]
	public class BlackCat : BaseCreature
	{
		public override string DefaultName{ get{ return "a black cat"; } }

		[Constructable]
		public BlackCat() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 3, 0.2, 0.4 )
		{
			Body = 0xC9;
			Hue = 1;
			BaseSoundID = 0x69;

			SetStr( 20 );
			SetDex( 40 );
			SetInt( 20 );

			SetHits( 30 );
			SetMana( 15 );

			SetDamage( 5 );

			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 5.0 );
			SetSkill( SkillName.Tactics, 4.0 );
			SetSkill( SkillName.Wrestling, 5.0 );

			Fame = 50;
			Karma = -150;

			VirtualArmor = 15;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 25.9;
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Feline; } }

		public BlackCat( Serial serial ) : base( serial )
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
		}
	}
}