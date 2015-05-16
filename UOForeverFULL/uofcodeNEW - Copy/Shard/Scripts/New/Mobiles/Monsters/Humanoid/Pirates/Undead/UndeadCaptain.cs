using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "an undead pirate captain corpse" )]
    public class UndeadCaptain : BaseCreature
    {
		public override string DefaultName{ get{ return "an undead pirate captain"; } }

	   [Constructable]
	   public UndeadCaptain () : base( AIType.AI_Melee, FightMode.Strongest, 10, 1, 0.2, 0.4 )
	   {
		  Body = 0x190;

		  SetStr( 1498, 1825 );
		  SetDex( 155, 197 );
		  SetInt( 801, 899 );
		  SetHits( 1900 );
		  SetMana( 950 );

		  SetDamage( 3, 5 );

          Alignment = Alignment.Undead;

		  SetSkill( SkillName.EvalInt, 99.1, 125.0 );
		  SetSkill( SkillName.Magery, 99.0, 135.0 );
		  SetSkill( SkillName.Meditation, 100.0 );
		  SetSkill( SkillName.MagicResist, 125.1, 150.0 );
		  SetSkill( SkillName.Tactics, 100.6, 110.0 );
			SetSkill( SkillName.Anatomy, 100.6, 110.0 );
			SetSkill( SkillName.Healing, 100.6, 110.0 );
			SetSkill( SkillName.Swords, 100.6, 120.0 );
		  SetSkill( SkillName.Wrestling, 250.6, 260.0 );

		  Fame = 10500;
		  Karma = -10500;

		  VirtualArmor = 40;

		  switch ( Utility.Random( 7 ) )
		  {
			 case 0: PackItem( new GreaterCurePotion() ); break;
			 case 1: PackItem( new GreaterPoisonPotion() ); break;
			 case 2: PackItem( new GreaterHealPotion() ); break;
			 case 3: PackItem( new GreaterStrengthPotion() ); break;
			 case 4: PackItem( new GreaterAgilityPotion() ); break;
		  }

			AddLoot( LootPack.FilthyRich, 4 );
			AddLoot( LootPack.FilthyRich, 4 );
			AddLoot( LootPack.Gems, 5 );

		  AddItem( new TricorneHat() );
		  AddItem( new StuddedGloves() );
			AddItem( new Cutlass() );

		  AddItem(new LongPants(Utility.RandomNeutralHue()));

			HairItemID = Utility.RandomList(0x203B, 0x2049, 0x2048, 0x204A);
			HairHue = Utility.RandomNondyedHue();

		  switch ( Utility.Random( 2 ) )
		  {
			 case 0: PackWeapon( 0, 5 ); break;
			 case 1: PackArmor( 0, 5 ); break;
		  }

		  switch ( Utility.Random( 2 ) )
		  {
			 case 0: PackWeapon( 0, 5 ); break;
			 case 1: PackArmor( 0, 5 ); break;
		  }

		  switch ( Utility.Random( 2 ) )
		  {
			 case 0: PackWeapon( 1, 5 ); break;
			 case 1: PackArmor( 1, 5 ); break;
		  }

		  switch ( Utility.Random( 3 ) )
		  {
			 case 0: PackWeapon( 1, 5 ); break;
			 case 1: PackArmor( 1, 5 ); break;
		  }

			if ( Utility.Random( 100 ) == 0 )
				PackItem( new PirateShipModel() );

		  if ( 0.01 > Utility.RandomDouble() )
			 PackItem( new IDWand() );
	  }

		public override bool AutoDispel{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
	   public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
	   public override int TreasureMapLevel{ get{ return 5; } }

	   public override void AlterMeleeDamageTo( Mobile to, ref int damage )
	   {
		  if ( to is BaseCreature )
			 damage *= 15;
	   }

	   public UndeadCaptain( Serial serial ) : base( serial )
	   {
	   }

	   public override void Serialize( GenericWriter writer )
	   {
		  base.Serialize( writer );
		  writer.WriteEncodedInt( (int)0 );
	   }

	   public override void Deserialize( GenericReader reader )
	   {
		  base.Deserialize( reader );
		  int version = reader.ReadEncodedInt();
	   }
    }
}