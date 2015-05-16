using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class OrcHunter : BaseHunter
	{
		[Constructable]
		public OrcHunter() : base()
		{
			Title = "the orc hunter";
			int hue = 2413 + Utility.Random( 5 );
			AddItem( new ThighBoots( hue ) );
			AddItem( new SkullCap( hue ) );
			//AddItem( new ShortPants() );
			AddItem( Identify( Rehued( new LeatherChest(), hue ) ) );
			AddItem( Identify( Rehued( new LeatherGloves(), hue ) ) );

			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( new Club() ); break;
				case 1: AddItem( new WarMace() ); break;
				case 2: AddItem( new Maul() ); break;
				case 3: AddItem( new Mace() ); break;
			}

			SetStr( 200, 350 );
			SetDex( 81, 95 );
			SetInt( 61, 75 );

			SetDamage( 12, 20 );

            Alignment = Alignment.Orc;

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

		public OrcHunter( Serial serial ) : base( serial )
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