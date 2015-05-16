using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class AmazonPriestess : BaseAmazon
	{
		[Constructable]
		public AmazonPriestess() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.45, 0.45 )
		{
			Title = "an Amazon Priestess";

			SetSkill( SkillName.Parry, 50.0 );
			SetSkill( SkillName.Swords, 75.0 );
			SetSkill( SkillName.Macing, 75.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );

			SetSkill( SkillName.Magery, 73.0, 90.0 );
			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.Meditation, 100.0 );

			AddItem( MakeAmazonArmor( new Robe() ) );
			AddItem( MakeAmazonArmor( new Sandals() ) );

			if ( 0.01 > Utility.RandomDouble() )
				AddItem( new JadeNecklace() );

			
			
			
			
			

			Fame = 700;
			Karma = -2500;

			SetStr( 85, 110 );
			SetDex( 50, 55 );
			SetInt( 90, 125 );

			SetHits( 125, 183 );

			SetDamage( 6, 12 );

			
			

			VirtualArmor = 35;
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor, 2 );
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.Potions, 2 );
			AddLoot( LootPack.Gems, 5 );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public AmazonPriestess( Serial serial ) : base( serial )
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