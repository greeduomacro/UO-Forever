using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "an undead rat corpse" )]
	public class UndeadRat : BaseCreature
	{
		public override string DefaultName{ get{ return "an undead rat"; } }

		[Constructable]
		public UndeadRat() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 0xD7;
			BaseSoundID = 0x188;
			Hue = 930;

            Alignment = Alignment.Undead;

			SetStr( 176, 200 );
			SetDex( 176, 195 );
			SetInt( 136, 160 );

			SetHits( 146, 160 );
			SetMana( 110 );

			SetDamage( 5, 13 );

			

			
			

			SetSkill( SkillName.Poisoning, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 125.1, 140.0 );
			SetSkill( SkillName.Tactics, 135.1, 150.0 );
			SetSkill( SkillName.Wrestling, 150.1, 165.0 );

			Fame = 600;
			Karma = -600;

			VirtualArmor = 40;

			Tamable = false;

			if ( Utility.Random( 30 ) == 0 )
				PackItem( new MouldyCheese() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override Poison HitPoison{ get{ return Poison.Regular; } }

		public UndeadRat( Serial serial ) : base( serial )
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