using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class BoneHunter : BaseHunter
	{
		[Constructable]
		public BoneHunter() : base()
		{
			Title = "the bone hunter";
			int hue = 2103 + Utility.Random( 3 );
			AddItem( new ThighBoots( hue ) );
			AddItem( new SkullCap( hue ) );
			//AddItem( new ShortPants() );
			AddItem( Identify( Rehued( new RingmailChest(), hue ) ) );
			hue = 2110 + Utility.Random( 3 );
			AddItem( Identify( Rehued( new BoneArms(), hue ) ) );
			AddItem( Identify( Rehued( new BoneGloves(), hue ) ) );

			hue = ( 0.05 > Utility.RandomDouble() ) ? ( 2107 + Utility.Random( 2 ) ) : 0;

			AddItem( Identify( Immovable( Rehued( new DiamondMace(), hue ) ) ) );

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

		public BoneHunter( Serial serial ) : base( serial )
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