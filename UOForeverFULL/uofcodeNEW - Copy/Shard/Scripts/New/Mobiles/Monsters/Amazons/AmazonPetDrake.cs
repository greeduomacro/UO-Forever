using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a drake corpse" )]
	public class AmazonPetDrake : BaseCreature
	{
		public override string DefaultName{ get{ return "Amazon Queen's Pet Drake"; } }

		[Constructable]
		public AmazonPetDrake() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.4 )
		{
			Body = 61;
			Hue = Utility.Random( 60, 1 );
			BaseSoundID = 362;
			//Thorns = 4;
			SetStr( 401, 430 );
			SetDex( 133, 152 );
			SetInt( 101, 140 );

			SetHits( 350, 450 );

			SetDamage( 13, 18 );

			
			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 100.0, 100.0 );
			SetSkill( SkillName.Wrestling, 65.1, 80.0 );

			Fame = 0;
			Karma = -5500;

			VirtualArmor = 55;

			//Tamable = true;
			//ControlSlots = 2;
			//MinTameSkill = 89.3;

			PackGem();
			PackGem();

			PackScroll( 1, 6 );
			PackScroll( 1, 6 );

			PackReg( 3 );
			PackGold( 180, 220 );
			PackMagicItems( 1, 5 );
		}

	//	public override bool BardImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 15; } }
		public override int Hides{ get{ return 25; } }
		public override HideType HideType{ get{ return HideType.Spined; } }
		public override int Scales{ get{ return 5; } }
		public override ScaleType ScaleType{ get{ return ( ScaleType.Green ); } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public AmazonPetDrake( Serial serial ) : base( serial )
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