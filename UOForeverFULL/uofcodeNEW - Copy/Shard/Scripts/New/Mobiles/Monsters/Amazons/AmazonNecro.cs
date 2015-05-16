using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class AmazonNecro : BaseAmazon
	{
		[Constructable]
		public AmazonNecro() : base( AIType.AI_Mage, FightMode.Closest, 4, 1, 0.45, 0.45 )
		{
			Name = NameList.RandomName( "female" );
			Title = "a Cursed Amazon";
			Hue = 34199;
			HairHue = 1428;

			SetSkill( SkillName.SpiritSpeak, 90.0, 99.0 );
			SetSkill( SkillName.Necromancy, 35.0, 75.0 );
			SetSkill( SkillName.Meditation, 80.0, 100.0 );
			SetSkill( SkillName.Tactics, 35.0, 60.0 );
			SetSkill( SkillName.MagicResist, 100.0 );

			AddItem( MakeAmazonArmor( new StuddedBustierArms() ) );
			AddItem( new Kilt( 2126 ) );
			AddItem( MakeAmazonArmor( new BoneHelm() ) );
			AddItem( MakeAmazonArmor( new BoneArms() ) );
			AddItem( MakeAmazonArmor( new BoneGloves() ) );
			//AddItem( NotCorpseCont( MakeAmazonArmor( new NecromancerSpellbook() ) ) );

			if ( Utility.Random( 350 ) == 0 )
				AddRingOfPower();

			
			
			
			
			

			Fame = 5000;
			Karma = -8000;

			SetStr( 100, 120 );
			SetDex( 100 );
			SetInt( 250, 300 );

			SetHits( 250 );

			SetDamage( 10, 15 );

			
			

			VirtualArmor = 100;
		}

		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Potions, 4 );
			AddLoot( LootPack.Gems, 3 );
			AddLoot( LootPack.HighScrolls );
			AddLoot( LootPack.MedScrolls, 2 );
			AddLoot( LootPack.FilthyRich );
		}

		public AmazonNecro( Serial serial ) : base( serial )
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