using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class AmazonPuriest : BaseAmazon
	{
		[Constructable]
		public AmazonPuriest() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.5, 0.5 )
		{
			Title = "an Amazon Puriest";
			SetSkill( SkillName.Parry, 85.0, 100.0 );
			SetSkill( SkillName.Swords, 85.0, 100.0 );
			SetSkill( SkillName.Tactics, 85.0, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );

			AddItem( MakeAmazonArmor( new StuddedBustierArms() ) );
			AddItem( MakeAmazonArmor( new LeatherSkirt() ) );
			AddItem( MakeAmazonArmor( new Sandals() ) );
			AddItem( new GnarledStaff() );

			if ( 0.01 > Utility.RandomDouble() )
				AddItem( new JadeNecklace() );

			
			
			
			
			

			Fame = 0;
			Karma = -2000;

			SetStr( 85, 110 );
			SetDex( 50, 55 );
			SetInt( 10, 13 );

			SetHits( 96, 125 );

			SetDamage( 4, 8 );

			
			

			VirtualArmor = 25;
		}

		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
			AddLoot( LootPack.Potions, 2 );
			AddLoot( LootPack.Gems, 3 );
			AddLoot( LootPack.LowScrolls, 2 );
			if ( 0.30 > Utility.RandomDouble() )
				AddLoot( LootPack.Average, 2 );
		}

		public AmazonPuriest( Serial serial ) : base( serial )
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