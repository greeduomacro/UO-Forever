using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a headless corpse" )]
	public class LesserHeadlessOne : BaseCreature
	{
		public override string DefaultName{ get{ return "a lesser headless one"; } }

		[Constructable]
		public LesserHeadlessOne() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 31;
			Hue = Utility.Random( 1891, 8 );
			BaseSoundID = 0x39D;

			SetStr( 26, 50 );
			SetDex( 36, 55 );
			SetInt( 16, 30 );

			SetHits( 15, 35 );

			SetDamage( 5, 10 );

			

			

			SetSkill( SkillName.MagicResist, 15.1, 25.0 );
			SetSkill( SkillName.Tactics, 25.1, 30.0 );
			SetSkill( SkillName.Wrestling, 25.1, 30.0 );

			Fame = 350;
			Karma = -350;

			VirtualArmor = 18;

			switch ( Utility.Random( 10 ) )
			{
				case 0: PackItem( new LeftArm() ); break;
				case 1: PackItem( new RightArm() ); break;
				case 2: PackItem( new Torso() ); break;
				case 3: PackItem( new Bone() ); break;
				case 4:	case 5: PackItem( new RibCage() ); break;
				case 6:	case 7:	case 8:	case 9: PackItem( new BonePile() ); break;
			}

			PackItem( new Gold( Utility.Dice( 1, 10, 10 ) ) );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 1; } }

		public LesserHeadlessOne( Serial serial ) : base( serial )
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