using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class ForestElfRanger : BaseElf
	{
		public override string DefaultName{ get{ return "a forest elf"; } }

		[Constructable]
		public ForestElfRanger() : base( AIType.AI_Archer, FightMode.Closest, 5, 1, 0.45, 0.45 )
		{
			SetSkill( SkillName.Parry, 40.0, 80.0 );
			SetSkill( SkillName.Archery, 80.0, 100.0 );
			SetSkill( SkillName.Anatomy, 95.0, 100.0 );
			SetSkill( SkillName.Tactics, 90.0, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );

			
			
			
			
			

			Fame = 350;
			Karma = -5000;

			SetStr( 126, 189 );
			SetDex( 100 );
			SetInt( 10, 13 );

			SetHits( 375, 425 );

			SetDamage( 12, 35 );

			
			

			int hue = Utility.Random( 2207, 12 );

			AddItem( Identify( Rehued( new LeatherChest(), hue ) ) );
			AddItem( Identify( Rehued( new Sandals(), hue ) ) );
			AddItem( Identify( Rehued( new LeatherGloves(), hue ) ) );

			ElvenQuiver quiver = new ElvenQuiver();
			quiver.AddItem( new Arrow( 100 ) );
			quiver.Hue = hue;
			quiver.LootType = LootType.Blessed;
			AddItem( quiver );

			AddItem( ChangeLootType( Rehued( new ElvenShortBow(), hue ), 0.01 > Utility.RandomDouble() ? LootType.Regular : LootType.Blessed ) );

			VirtualArmor = 20;
		}

		public ForestElfRanger( Serial serial ) : base( serial )
		{
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Potions, 2 );
			AddLoot( LootPack.Gems, Utility.Random( 2, 2 ) );
			AddLoot( LootPack.Average, 2 );
		}

		public override void GenerateLoot( bool spawning )
		{
			base.GenerateLoot( spawning );

			if ( !spawning )
			{
				if ( Utility.Random( 1000 ) == 0 )
				{
					switch ( Utility.Random( 5 ) )
					{
						case 0: PackItem( new RangerArms() ); break;
						case 1: PackItem( new RangerChest() ); break;
						case 2: PackItem( new RangerLegs() ); break;
						case 3: PackItem( new RangerGorget() ); break;
						case 4: PackItem( new RangerGloves() ); break;
					}
				}
				else if ( Utility.Random( 2000 ) == 0 )
					PackItem( new RecipeScroll( EraML ? 502 : 501 ) ); //Elven Quiver recipe

				if ( Utility.RandomBool() )
					PackItem( new Arrow( 25 ) );

				if ( Utility.Random( 3 ) == 0 )
					AddPackedLoot( LootPack.AverageProvisions, typeof( Pouch ) );
				else
					AddPackedLoot( LootPack.MeagerProvisions, typeof( Bag ) );
			}
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