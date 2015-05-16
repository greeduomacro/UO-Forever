using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class TrollHunter : BaseHunter
	{
		[Constructable]
		public TrollHunter() : base()
		{
			Title = "the troll hunter";
			int hue = 2407 + Utility.Random( 5 );
			AddItem( new ThighBoots( hue ) );
			AddItem( new SkullCap( hue ) );
			//AddItem( new ShortPants() );
			AddItem( Identify( Rehued( new LeatherChest(), hue ) ) );
			AddItem( Identify( Rehued( new LeatherGloves(), hue ) ) );

			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( new Kryss() ); break;
				case 1: AddItem( new ExecutionersAxe() ); break;
				case 2: AddItem( new DoubleAxe() ); break;
				case 3: AddItem( new ShortSpear() ); break;
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
			SetSkill( SkillName.Wrestling, 100.0 );
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
				PackItem( new TrollHead() );
			return base.OnBeforeDeath();
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public TrollHunter( Serial serial ) : base( serial )
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