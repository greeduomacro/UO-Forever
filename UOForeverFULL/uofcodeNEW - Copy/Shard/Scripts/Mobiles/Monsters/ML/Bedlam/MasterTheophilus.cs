using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a Master Theophilus corpse" )]
	public class MasterTheophilus : EvilMageLord
	{
		[Constructable]
		public MasterTheophilus()
		{
			IsParagon = true;

			Name = "Master Theophilus";
			Title = "the necromancer";
			Hue = 0;

			SetStr( 137, 187 );
			SetDex( 253, 301 );
			SetInt( 393, 444 );

			SetHits( 663, 876 );

			SetDamage( 15, 20 );

			

			
			
			
			
			

			SetSkill( SkillName.Wrestling, 69.9, 105.3 );
			SetSkill( SkillName.Tactics, 113.0, 117.9 );
			SetSkill( SkillName.MagicResist, 127.0, 132.8 );
			SetSkill( SkillName.Magery, 138.1, 143.7 );
			SetSkill( SkillName.EvalInt, 125.6, 133.8 );
			SetSkill( SkillName.Necromancy, 125.6, 133.8 );
			SetSkill( SkillName.SpiritSpeak, 125.6, 133.8 );
			SetSkill( SkillName.Meditation, 128.8, 132.9 );

			Fame = 18000;
			Karma = -18000;

			AddItem( Immovable(new Shoes( 0x537 )) );
			AddItem( Immovable(new Robe( 0x452 )) );

			for ( int i = 0; i < 2; ++i )
			{
				PackScroll( 4, 7 );
			}

			PackReg( 7 );
			PackReg( 7 );
			PackReg( 8 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 3 );
		}

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ParalyzingBlow;
		}

		public override int TreasureMapLevel{ get{ return 5; } }

		public MasterTheophilus( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
