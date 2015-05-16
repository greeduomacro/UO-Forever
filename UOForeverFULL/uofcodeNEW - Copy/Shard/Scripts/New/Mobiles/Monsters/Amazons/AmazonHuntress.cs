using System;
using Server;
using Server.Items;
using Server.Games;

namespace Server.Mobiles
{
	public class AmazonHuntress : BaseAmazon
	{
		[Constructable]
		public AmazonHuntress() : base( AIType.AI_Archer, FightMode.Closest, 5, 1, 0.45, 0.45 )
		{
			Title = "an Amazon Huntress";

			SetSkill( SkillName.Parry, 40.0, 60.0 );
			SetSkill( SkillName.Archery, 40.0, 60.0 );
			SetSkill( SkillName.Tactics, 50.0, 70.0 );
			SetSkill( SkillName.MagicResist, 100.0 );

			AddItem( MakeAmazonArmor( new FemaleLeatherChest() ) );
			AddItem( MakeAmazonArmor( new LeatherGloves() ) );
			AddItem( MakeAmazonArmor( new Boots() ) );

			if ( Utility.Random( 350 ) == 0 )
				AddItem( new JadeNecklace() );

			if ( 0.10 > Utility.RandomDouble() )
			{
				HeavyCrossbow weapon = new HeavyCrossbow();

				double random = Utility.RandomDouble();
				if ( 0.025 > random )
					weapon.DamageLevel = PseudoSeerStone.Instance != null ? (WeaponDamageLevel)PseudoSeerStone.Instance._HighestDamageLevelSpawn : WeaponDamageLevel.Vanq;
				else if ( 0.075 > random )
					weapon.DamageLevel = WeaponDamageLevel.Force;

				AddItem( MakeAmazonArmor( weapon ) );
			}
			else
				AddItem( new Crossbow() );

			
			
			
			
			

			Fame = 100;
			Karma = -3000;

			SetStr( 50, 65 );
			SetDex( 50, 55 );
			SetInt( 10, 13 );

			SetHits( 180, 200 );

			SetDamage( 6, 10 );

			
			

			VirtualArmor = 30;
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public AmazonHuntress( Serial serial ) : base( serial )
		{
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.LowScrolls, 2 );
			AddLoot( LootPack.Potions, 2 );
			AddLoot( LootPack.Gems, Utility.Random( 2, 3 ) );
			AddLoot( LootPack.Average, 2 );
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