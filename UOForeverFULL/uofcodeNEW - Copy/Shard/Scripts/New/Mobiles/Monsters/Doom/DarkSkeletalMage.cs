using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class DarkSkeletalMage : BaseCreature
	{
		public override string DefaultName{ get{ return "a dark skeletal mage"; } }

		[Constructable]
		public DarkSkeletalMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 148;
			BaseSoundID = 451;
			Hue = 2403;

            Alignment = Alignment.Undead;

			SetStr( 76, 100 );
			SetDex( 56, 75 );
			SetInt( 206, 240 );

			SetHits( 106, 120 );

			SetDamage( 5, 9 );

			SetSkill( SkillName.EvalInt, 80.1, 90.0 );
			SetSkill( SkillName.Magery, 80.1, 90.0 );
			SetSkill( SkillName.MagicResist, 75.1, 90.0 );
			SetSkill( SkillName.Tactics, 75.1, 90.0 );
			SetSkill( SkillName.Wrestling, 75.1, 95.0 );

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 50;
			PackReg( 4 );
			PackNecroReg( 5, 12 );
			PackItem( new Bone() );
			PackItem( new Bone() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.LowScrolls );
			AddLoot( LootPack.Potions );
		}

		public override bool OnBeforeDeath()
		{
			if ( base.OnBeforeDeath() )
			{
				if ( !Summoned && !NoKillAwards && ( Region != null && Region.IsPartOf( "Skara Brae" ) ) )
				{
					PackItem( new BoneAsh( 2 + Utility.Random( 1 ) ) );

					if ( 0.03 > Utility.RandomDouble() )
						PackItem( new BoneContainer() );
					else if ( 0.03 > Utility.RandomDouble() )
						PackItem( new CandleSkull() );
				}

				return true;
			}

			return false;
		}

		public override bool BleedImmune{ get{ return true; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public DarkSkeletalMage( Serial serial ) : base( serial )
		{
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