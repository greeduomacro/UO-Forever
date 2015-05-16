using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a headless corpse" )]
	public class BrawnyHeadlessOne : BaseCreature
	{
		public override string DefaultName{ get{ return "a brawny headless one"; } }

		[Constructable]
		public BrawnyHeadlessOne() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 31;
			Hue = Utility.Random( 1837, 8 );
			BaseSoundID = 0x39D;

			SetStr( 135, 150 );
			SetDex( 36, 55 );
			SetInt( 16, 30 );

			SetHits( 135, 150 );

			SetDamage( 5, 10 );

			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 30.1, 35.0 );
			SetSkill( SkillName.Tactics, 40.1, 55.0 );
			SetSkill( SkillName.Wrestling, 40.1, 45.0 );

			Fame = 550;
			Karma = -550;

			VirtualArmor = 25;

			switch ( Utility.Random( 10 ))
			{
				case 0: PackItem( new LeftArm() ); break;
				case 1: PackItem( new RightArm() ); break;
				case 2: PackItem( new Torso() ); break;
				case 3: PackItem( new Bone() ); break;
				case 4:	case 5: PackItem( new RibCage() ); break;
				case 6:	case 7:	case 8:	case 9: PackItem( new BonePile() ); break;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor, Utility.Random( 1, 1 ) );
			AddLoot( LootPack.Meager );

            if (0.02 >= Utility.RandomDouble())
            { SkillScroll scroll = new SkillScroll(); scroll.Randomize(); PackItem(scroll); }
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 1; } }

		public BrawnyHeadlessOne( Serial serial ) : base( serial )
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