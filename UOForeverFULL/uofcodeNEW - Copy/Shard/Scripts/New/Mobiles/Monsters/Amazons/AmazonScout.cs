using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class AmazonScout : BaseAmazon
	{
		[Constructable]
		public AmazonScout() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.5, 0.5 )
		{
			Title = "an Amazon Scout";
			SetSkill( SkillName.Parry, 35.0, 40.0 );
			SetSkill( SkillName.Swords, 35.0, 40.0 );
			SetSkill( SkillName.Tactics, 50.0, 70.0 );
			SetSkill( SkillName.MagicResist, 85.0 );

			AddItem( MakeAmazonArmor( new LeatherBustierArms() ) );
			AddItem( MakeAmazonArmor( new LeatherSkirt() ) );
			AddItem( MakeAmazonArmor( new Sandals() ) );
			AddItem( new QuarterStaff() );

			
			
			
			
			

			Fame = 0;
			Karma = -2000;

			SetStr( 40, 45 );
			SetDex( 50, 55 );
			SetInt( 10, 13 );

			SetHits( 65, 80 );

			SetDamage( 3, 7 );

			
			

			VirtualArmor = 15;
		}

		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );

			AddLoot( LootPack.Potions );
			AddLoot( LootPack.Gems );
			AddLoot( LootPack.LowScrolls );

			if ( 0.20 > Utility.RandomDouble() )
				AddLoot( LootPack.Average, 2 );
		}

		public AmazonScout( Serial serial ) : base( serial )
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