using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a scorpion corpse" )]
	public class DesertScorpion : BaseCreature
	{
		public override string DefaultName{ get{ return "a desert scorpion"; } }

		[Constructable]
		public DesertScorpion() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 48;
			BaseSoundID = 397;

			SetStr( 123, 165 );
			SetDex( 96, 100 );
			SetInt( 56, 70 );

			SetHits( 135, 168 );
			SetMana( 0 );

			SetDamage( 7, 12 );

			
			

			
			
			
			
			

			SetSkill( SkillName.Poisoning, 100.0, 110.0 );
			SetSkill( SkillName.MagicResist, 50.1, 60.0 );
			SetSkill( SkillName.Tactics, 85.3, 100.0 );
			SetSkill( SkillName.Wrestling, 85.3, 100.0 );

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 33;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 64.1;

			PackItem( new PoisonPotion() );
			PackItem( new LesserPoisonPotion() );
			PackItem( new Nightshade( Utility.Random( 2, 3 ) ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public override int Meat{ get{ return 2; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return (0.95 >= Utility.RandomDouble() ? Poison.Deadly : Poison.Lethal); } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomGreyHue(); } }

		public DesertScorpion( Serial serial ) : base( serial )
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