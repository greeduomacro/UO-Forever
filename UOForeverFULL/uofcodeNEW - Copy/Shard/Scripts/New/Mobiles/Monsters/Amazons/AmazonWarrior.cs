using System;
using Server;
using Server.Items;
using Server.Games;

namespace Server.Mobiles
{
	public class AmazonWarrioress : BaseAmazon
	{
		[Constructable]
		public AmazonWarrioress() : base( AIType.AI_Melee, FightMode.Closest, 5, 1, 0.45, 0.45 )
		{
			Title = "an Amazon Warrioress";

			SetSkill( SkillName.Parry, 40.0, 60.0 );
			SetSkill( SkillName.Swords, 40.0, 60.0 );
			SetSkill( SkillName.Tactics, 50.0, 70.0 );
			SetSkill( SkillName.MagicResist, 100.0 );

			AddItem( MakeAmazonArmor( new FemalePlateChest() ) );
			AddItem( MakeAmazonArmor( new LeatherShorts() ) );
			AddItem( MakeAmazonArmor( new LeatherGloves() ) );
			AddItem( MakeAmazonArmor( new Boots() ) );

			double random = Utility.RandomDouble();

			if ( 0.33 > random )
			{
				Spear weapon = new Spear();

				double randomdamage = Utility.RandomDouble();
				if ( 0.05 > randomdamage )
                    weapon.DamageLevel = PseudoSeerStone.Instance != null ? (WeaponDamageLevel)PseudoSeerStone.Instance._HighestDamageLevelSpawn : WeaponDamageLevel.Vanq;
				else if ( 0.10 > randomdamage )
					weapon.DamageLevel = WeaponDamageLevel.Force;

				//if ( 0.10 > Utility.RandomDouble() )
				//	weapon.Attributes.SpellChanneling = 1;

				AddItem( MakeAmazonArmor( weapon ) );
				SetDamage( 10, 15 );
			}
			else if ( 0.10 > random )
			{
				DoubleBladedStaff weapon = new DoubleBladedStaff();

				double randomdamage = Utility.RandomDouble();
				if ( 0.05 > randomdamage )
                    weapon.DamageLevel = PseudoSeerStone.Instance != null ? (WeaponDamageLevel)PseudoSeerStone.Instance._HighestDamageLevelSpawn : WeaponDamageLevel.Vanq;
				else if ( 0.10 > randomdamage )
					weapon.DamageLevel = WeaponDamageLevel.Force;

				AddItem( NotCorpseCont( MakeAmazonArmor( weapon ) ) );
				SetDamage( 12, 18 );
			}
			else
			{
				AddItem( new BlackStaff() );
				SetDamage( 8, 10 );
			}

			Fame = 0;
			Karma = -3000;

			SetStr( 50, 65 );
			SetDex( 50, 55 );
			SetInt( 10, 13 );

			SetHits( 60, 80 );

			VirtualArmor = 30;
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public AmazonWarrioress( Serial serial ) : base( serial )
		{
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );

			AddLoot( LootPack.Potions, 2 );
			AddLoot( LootPack.Gems, 3 );
			AddLoot( LootPack.LowScrolls, 2 );
			AddLoot( LootPack.MedScrolls );

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