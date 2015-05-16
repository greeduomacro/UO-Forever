using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a lich corpse" )]
	public class Lich : BaseCreature
	{
		public override string DefaultName{ get{ return "a lich"; } }

		[Constructable]
		public Lich() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 24;
			BaseSoundID = 0x3E9;

			SetStr( 171, 200 );
			SetDex( 126, 145 );
			SetInt( 276, 305 );

			SetHits( 103, 120 );

			SetDamage( 10, 16 );





            Alignment = Alignment.Undead;
			
			
			
			

			//SetSkill( SkillName.Necromancy, 89, 99.1 );
			SetSkill( SkillName.SpiritSpeak, 90.0, 99.0 );

			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.Magery, 70.1, 80.0 );
			SetSkill( SkillName.Meditation, 85.1, 95.0 );
			SetSkill( SkillName.MagicResist, 80.1, 100.0 );
			SetSkill( SkillName.Tactics, 70.1, 90.0 );

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 50;
			PackItem( new GnarledStaff() );
			PackItem(new Gold ( 290, 350 ));
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Gems, 2 );
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.Poor );
			AddLoot( LootPack.MedScrolls, 2 );

			if ( 0.01 > Utility.RandomDouble() )// 2 percent - multipy number x 100 to get percent
                { SkillScroll scroll = new SkillScroll(); scroll.Randomize(); PackItem(scroll); }
		}

		public override bool OnBeforeDeath()
		{
			if ( base.OnBeforeDeath() )
			{
				if ( !Summoned && !NoKillAwards && ( Region != null && Region.IsPartOf( "Skara Brae" ) ) )
				{
					PackItem( new LichDust( 1 + Utility.Random( 3 ) ) );

					if ( 0.01 > Utility.RandomDouble() )
					{
						Item deco = null;

						switch ( Utility.Random( 5 ) )
						{
							case 0: deco = new DecoGinsengRoot(); break;
							case 1: deco = new DecoGinsengRoot2(); break;
							case 2: deco = new DecoMandrakeRoot(); break;
							case 3: deco = new DecoMandrakeRoot2(); break;
                            case 4: PackItem(new BlackDyeTub()); break;
						}

						PackItem( deco );
					}
				}
				
				return true;
			}

			return false;
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public Lich( Serial serial ) : base( serial )
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