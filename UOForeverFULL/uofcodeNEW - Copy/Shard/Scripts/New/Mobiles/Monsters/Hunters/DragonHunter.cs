using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class DragonHunter : BaseHunter
	{
		[Constructable]
		public DragonHunter() : base()
		{
			Title = "the dragon hunter";
			int hue = 1828 + Utility.Random( 5 );
			AddItem( new ThighBoots( hue ) );
			AddItem( new SkullCap( hue ) );
			//AddItem( new ShortPants( hue ) );
			AddItem( Identify( Rehued( new RingmailChest(), hue ) ) );
			AddItem( Identify( Rehued( new RingmailGloves(), hue ) ) );

			switch ( Utility.Random( 6 ) )
			{
				case 0: AddItem( new Longsword() ); break;
				case 1: AddItem( new Broadsword() ); break;
				case 2: AddItem( new Kryss() ); break;
				case 3: AddItem( new Spear() ); break;
				case 4: AddItem( new WarAxe() ); break;
				case 5: AddItem( new ShortSpear() ); break;
			}

			SetStr( 250, 300 );
			SetDex( 81, 95 );
			SetInt( 61, 75 );

			SetDamage( 15, 20 );

			SetSkill( SkillName.Fencing, 75.0, 97.5 );
			SetSkill( SkillName.Macing, 75.0, 97.5 );
			SetSkill( SkillName.MagicResist, 45.0, 67.5 );
			SetSkill( SkillName.Swords, 75.0, 97.5 );
			SetSkill( SkillName.Tactics, 75.0, 97.5 );
			SetSkill( SkillName.Wrestling, 55.0, 87.5 );
			SetSkill( SkillName.Anatomy, 100.0 );

			Karma = -4500;
			Fame = 2000;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
		}

		public override bool OnBeforeDeath()
		{
			if ( 0.005 >= Utility.RandomDouble() )
				PackItem( new DragonHead( Utility.RandomBool() ? 1830 : 0 ) );
			return base.OnBeforeDeath();
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public DragonHunter( Serial serial ) : base( serial )
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