using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class AmazonGuard : BaseAmazon
	{
		[Constructable]
		public AmazonGuard() : base( AIType.AI_AmazonGuard, FightMode.Closest, 5, 1, 0.4, 0.4 )
		{
			Title = "an Amazon Guard";

			SetSkill( SkillName.Parry, 100.0 );
			SetSkill( SkillName.Swords, 80.0, 100.0 );
			SetSkill( SkillName.Macing, 80.0, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );

			BaseArmor armor = (BaseArmor)MakeAmazonArmor( new FemalePlateChest() );

			double random = Utility.RandomDouble();

			if ( 0.10 > random )
				armor.ProtectionLevel = ArmorProtectionLevel.Defense;
			else if ( 0.125 > random )
				armor.ProtectionLevel = ArmorProtectionLevel.Hardening;

			AddItem( armor );

			if ( Utility.Random( 100 ) < 33 )
				AddItem( MakeAmazonArmor( new PlateHelm() ) );

			AddItem( MakeAmazonArmor( new PlateGloves() ) );
			AddItem( MakeAmazonArmor( new PlateLegs() ) );
			AddItem( MakeAmazonArmor( new PlateArms() ) );
			AddItem( MakeAmazonArmor( new PlateGorget() ) );

			if ( 0.06 > Utility.RandomDouble() )
				AddItem( new ExecutionersAxe() );
			else
				AddItem( new QuarterStaff() );

			
			
			
			
			

			Fame = 1000;
			Karma = -5000;

			SetStr( 100, 150 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 360, 380 );

			SetDamage( 7, 17 );

			
			
			VirtualArmor = 50;

		}

	//	public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public AmazonGuard( Serial serial ) : base( serial )
		{
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.LowScrolls, 2 );
			AddLoot( LootPack.Gems, Utility.Random( 3, 3 ) );
			AddLoot( LootPack.Average, 2 );
			if ( 0.50 > Utility.RandomDouble() )
				AddLoot( LootPack.Rich );
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