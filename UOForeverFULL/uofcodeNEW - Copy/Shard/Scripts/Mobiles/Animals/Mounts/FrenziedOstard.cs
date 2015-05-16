using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "an ostard corpse" )]
	public class FrenziedOstard : BaseMount
	{
		public override string DefaultName{ get{ return "a frenzied ostard"; } }

        public override int InternalItemItemID { get { return 0x3EA4; } }

		[Constructable]
		public FrenziedOstard() : base( 0xDA, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Hue = Utility.RandomHairHue() | 0x8000;

			BaseSoundID = 0x275;

			SetStr( 94, 170 );
			SetDex( 96, 115 );
			SetInt( 6, 10 );

			SetHits( 71, 110 );
			SetMana( 0 );

			SetDamage( 11, 17 );

			

			
			
			
			

			SetSkill( SkillName.MagicResist, 75.1, 80.0 );
			SetSkill( SkillName.Tactics, 79.3, 94.0 );
			SetSkill( SkillName.Wrestling, 79.3, 94.0 );

			Fame = 1500;
			Karma = -1500;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 77.1;
		}

		public override int Meat{ get{ return 3; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish | FoodType.Eggs | FoodType.FruitsAndVeggies; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Ostard; } }

		public FrenziedOstard( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}