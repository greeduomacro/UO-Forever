using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly corpse" )]
	public class Wraith : BaseCreature
	{
		public override string DefaultName{ get{ return "a wraith"; } }

		[Constructable]
		public Wraith() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 26;
			Hue = 0x4001;
			BaseSoundID = 0x482;

			SetStr( 140, 160 );
			SetDex( 23, 76 );
			SetInt( 36, 60 );

			SetHits( 46, 60 );

			SetDamage( 8, 13 );


            Alignment = Alignment.Undead;

			
			
			

			SetSkill( SkillName.EvalInt, 70.1, 75.0 );
			SetSkill( SkillName.Magery, 70.1, 75.0 );
			SetSkill( SkillName.MagicResist, 70.1, 75.0 );
			SetSkill( SkillName.Tactics, 65.1, 89.0 );
			SetSkill( SkillName.Wrestling, 65.1, 89.0 );

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 28;

			PackReg( 10 );
			PackItem( Loot.RandomWeapon() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager, 2 );
		}

		public override bool OnBeforeDeath()
		{
			if ( base.OnBeforeDeath() )
			{
				if ( !Summoned && !NoKillAwards && ( Region != null && Region.IsPartOf( "Skara Brae" ) ) )
					PackItem( new EtherealResidue( 1 + Utility.Random( 1 ) ) );
				
				return true;
			}

			return false;
		}

		public override bool BleedImmune{ get{ return true; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public Wraith( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}