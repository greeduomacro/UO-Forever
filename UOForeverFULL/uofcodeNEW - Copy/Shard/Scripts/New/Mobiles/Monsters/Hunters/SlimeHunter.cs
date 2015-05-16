using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class SlimeHunter : BaseHunter
	{
		[Constructable]
		public SlimeHunter() : base()
		{
			Title = "the slime hunter";
			int hue = 0.10 > Utility.RandomDouble() ? ( 2201 + ( 6 * Utility.Random( 4 ) ) + Utility.Random( 4 ) ) : Utility.Random( 2017, 2 );
			AddItem( new ThighBoots( hue ) );
			AddItem( new SkullCap( hue ) );
			//AddItem( new ShortPants() );
			AddItem( Identify( Rehued( new StuddedGorget(), hue ) ) );
			AddItem( Identify( Rehued( new StuddedChest(), hue ) ) );
			AddItem( Identify( Rehued( new RingmailGloves(), hue ) ) );

			switch ( Utility.Random( 6 ) )
			{
				case 0: AddItem( new Katana() ); break;
				case 1: AddItem( new HammerPick() ); break;
				case 2: AddItem( new GnarledStaff() ); break;
				case 3: AddItem( new Cutlass() ); break;
				case 4: AddItem( new Axe() ); break;
				case 5: AddItem( new WarFork() ); break;
			}

			SetStr( 200, 350 );
			SetDex( 81, 95 );
			SetInt( 61, 75 );

			SetDamage( 12, 20 );

			SetSkill( SkillName.Fencing, 75.0, 97.5 );
			SetSkill( SkillName.Macing, 100.0 );
			SetSkill( SkillName.MagicResist, 45.0, 67.5 );
			SetSkill( SkillName.Swords, 75.0, 97.5 );
			SetSkill( SkillName.Tactics, 75.0, 97.5 );
			SetSkill( SkillName.Wrestling, 100.0 );
			SetSkill( SkillName.Anatomy, 100.0 );

			Karma = -4500;
			Fame = 2000;
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public SlimeHunter( Serial serial ) : base( serial )
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