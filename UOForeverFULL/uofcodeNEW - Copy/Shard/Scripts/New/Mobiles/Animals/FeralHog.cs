using System;

namespace Server.Mobiles
{
	[CorpseName( "a pig corpse" )]
	public class FeralHog : BaseCreature
	{
		[Constructable]
		public FeralHog() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a feral hog";
			Body = 0xCB;
			BaseSoundID = 0xC4;
			Hue = 1822;

			SetStr( 23 );
			SetDex( 30 );
			SetInt( 7 );

			SetHits( 20 );
			SetMana( 0 );

			SetDamage( 3, 6 );

			

			

			SetSkill( SkillName.MagicResist, 8.0 );
			SetSkill( SkillName.Tactics, 10.0 );
			SetSkill( SkillName.Wrestling, 8.0 );

			Fame = -100;
			Karma = -100;

			VirtualArmor = 15;
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVeggies | FoodType.GrainsAndHay; } }

		public FeralHog( Serial serial ) : base( serial )
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