using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class DarkSkeletalKnight : BaseCreature
	{
		public override string DefaultName{ get{ return "a dark skeletal knight"; } }

		[Constructable]
		public DarkSkeletalKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 147;
			BaseSoundID = 451;
			Hue = 2403;

            Alignment = Alignment.Undead;

			SetStr( 296, 350 );
			SetDex( 96, 115 );
			SetInt( 86, 110 );

			SetHits( 268, 310 );

			SetDamage( 9, 20 );			

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 85.1, 100.0 );
			SetSkill( SkillName.Wrestling, 85.1, 95.0 );

			Fame = 3000;
			Karma = -3000;

			VirtualArmor = 45;

			switch ( Utility.Random( 6 ) )
			{
				case 0: PackItem( new PlateArms() ); break;
				case 1: PackItem( new PlateChest() ); break;
				case 2: PackItem( new PlateGloves() ); break;
				case 3: PackItem( new PlateGorget() ); break;
				case 4: PackItem( new PlateLegs() ); break;
				case 5: PackItem( new PlateHelm() ); break;
			}

			PackItem( new Scimitar() );
			PackItem( new WoodenShield() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager, 2 );
		}

		public override bool OnBeforeDeath()
		{
			if ( base.OnBeforeDeath() )
			{
				if ( !Summoned && !NoKillAwards && ( Region != null && Region.IsPartOf( "Skara Brae" ) ) )
				{
					PackItem( new BoneAsh( 2 + Utility.Random( 1 ) ) );

					if ( 0.05 > Utility.RandomDouble() )
						PackItem( new CandleSkull() );
				}

				return true;
			}

			return false;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public DarkSkeletalKnight( Serial serial ) : base( serial )
		{
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}