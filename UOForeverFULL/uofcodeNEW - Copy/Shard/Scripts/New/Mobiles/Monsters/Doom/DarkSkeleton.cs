using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class DarkSkeleton : BaseCreature
	{
		public override string DefaultName{ get{ return "a dark skeleton"; } }

		[Constructable]
		public DarkSkeleton() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = Utility.RandomList( 50, 56 );
			BaseSoundID = 0x48D;
			Hue = 2403;

            Alignment = Alignment.Undead;

			SetStr( 106, 130 );
			SetDex( 56, 75 );
			SetInt( 76, 100 );

			SetHits( 84, 98 );

			SetDamage( 5, 10 );

			SetSkill( SkillName.MagicResist, 45.1, 60.0 );
			SetSkill( SkillName.Tactics, 45.1, 60.0 );
			SetSkill( SkillName.Wrestling, 45.1, 55.0 );

			Fame = 450;
			Karma = -450;

			VirtualArmor = 18;

			switch ( Utility.Random( 5 ))
			{
				case 0: PackItem( new BoneArms() ); break;
				case 1: PackItem( new BoneChest() ); break;
				case 2: PackItem( new BoneGloves() ); break;
				case 3: PackItem( new BoneLegs() ); break;
				case 4: PackItem( new BoneHelm() ); break;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override bool OnBeforeDeath()
		{
			if ( base.OnBeforeDeath() )
			{
				if ( !Summoned && !NoKillAwards && ( Region != null && Region.IsPartOf( "Skara Brae" ) ) )
				{
					PackItem( new BoneAsh() );

					if ( 0.01 > Utility.RandomDouble() )
						PackItem( new BoneContainer() );
				}

				return true;
			}

			return false;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public DarkSkeleton( Serial serial ) : base( serial )
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