using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class DemonHunter : BaseHunter
	{
		[Constructable]
		public DemonHunter() : base( AIType.AI_Archer )
		{
			Title = "the demon hunter";
			int hue = 1882 + Utility.Random( 5 );
			AddItem( new ThighBoots( hue ) );
			AddItem( new SkullCap( hue ) );
			//AddItem( new ShortPants() );
			AddItem( Identify( Rehued( new StuddedChest(), hue ) ) );
			AddItem( Identify( Rehued( new StuddedGloves(), hue ) ) );

			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( new Bow() ); PackItem( new Arrow( 50 ) ); break;
				case 1: AddItem( new Crossbow() ); PackItem( new Bolt( 50 ) ); break;
				case 2: AddItem( new HeavyCrossbow() ); PackItem( new Bolt( 50 ) ); break;
				//case 3: AddItem( new CompositeBow() ); PackItem( new Arrow( 50 ) ); break;
				//case 4: AddItem( new DoubleBladedStaff() ); break;
			}

			SetStr( 285, 325 );
			SetDex( 135, 145 );
			SetInt( 61, 75 );

			SetDamage( 15, 20 );

			SetSkill( SkillName.MagicResist, 45.0, 67.5 );
			SetSkill( SkillName.Archery, 75.0, 97.5 );
			SetSkill( SkillName.Tactics, 75.0, 97.5 );
			SetSkill( SkillName.Wrestling, 55.0, 87.5 );
			SetSkill( SkillName.Anatomy, 100.0 );

			Karma = -5500;
			Fame = 3500;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
		}

		public override bool OnBeforeDeath()
		{
			if ( 0.005 >= Utility.RandomDouble() )
			{
				if ( Utility.RandomBool() )
					PackItem( new DemonSkull() );
				else
					PackItem( new DemonCowSkull() );
			}
			return base.OnBeforeDeath();
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public DemonHunter( Serial serial ) : base( serial )
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