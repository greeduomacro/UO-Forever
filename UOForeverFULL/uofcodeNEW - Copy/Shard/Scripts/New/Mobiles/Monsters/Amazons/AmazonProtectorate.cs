using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class AmazonProtectorate : BaseAmazon
	{
		[Constructable]
		public AmazonProtectorate() : base( AIType.AI_AmazonGuard, FightMode.Closest, 7, 1, 0.35, 0.4 )
		{
			Title = "an Amazon Protector";

			SetSkill( SkillName.Parry, 120.0 );
			SetSkill( SkillName.Swords, 120.0 );
			SetSkill( SkillName.Macing, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.MagicResist, 120.0 );

			BaseArmor armor = (BaseArmor)MakeAmazonArmor( new FemalePlateChest() );

			double random = Utility.RandomDouble();
			if ( 0.10 > random )
				armor.ProtectionLevel = ArmorProtectionLevel.Guarding;
			else if ( 0.125 > random )
				armor.ProtectionLevel = ArmorProtectionLevel.Invulnerability;

			AddItem( armor );
			AddItem( MakeAmazonArmor( new PlateHelm() ) );
			AddItem( MakeAmazonArmor( new PlateGloves() ) );
			AddItem( MakeAmazonArmor( new PlateLegs() ) );
			AddItem( MakeAmazonArmor( new PlateArms() ) );
			AddItem( MakeAmazonArmor( new PlateGorget() ) );
			AddItem( new ExecutionersAxe() );

			if ( 0.10 > Utility.RandomDouble() )
				AddRingOfPower();

			
			
			
			
			

			Fame = 2000;
			Karma = -5000;

			SetStr( 100, 150 );
			SetDex( 100, 125 );
			SetInt( 36, 60 );

			SetHits( 425, 475 );

			SetDamage( 12, 21 );

			
			

			VirtualArmor = 80;
		}

	//	public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Potions, 2 );
			AddLoot( LootPack.Gems, 3 );
			AddLoot( LootPack.MedScrolls, 3 );

			AddLoot( LootPack.FilthyRich );
			if ( 0.25 > Utility.RandomDouble() )
				AddLoot( LootPack.UltraRich );
		}

		public AmazonProtectorate( Serial serial ) : base( serial )
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