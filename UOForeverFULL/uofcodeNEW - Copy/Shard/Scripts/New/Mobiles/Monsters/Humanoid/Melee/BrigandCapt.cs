using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class BrigandCapt : BaseCreature
	{
		[Constructable]
		public BrigandCapt() : base( AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.1, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the brigand captain";
			Hue = Utility.RandomSkinHue();

			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				AddItem( new Skirt( Utility.RandomNeutralHue() ) );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
				AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
			}

			SetStr( 96, 125 );
			SetDex( 81, 95 );
			SetInt( 61, 75 );
			SetHits( 180, 250 );
			SetDamage( 10, 23 );

			SetSkill( SkillName.Fencing, 105.0, 115.5 );
			SetSkill( SkillName.Macing, 105.0, 115.5 );
			SetSkill( SkillName.MagicResist, 107.0, 110.5 );
			SetSkill( SkillName.Swords, 105.0, 115.5 );
			SetSkill( SkillName.Tactics, 105.0, 110.5 );
			SetSkill( SkillName.Wrestling, 50.0, 77.5 );

			Fame = 1000;
			Karma = -1000;

			AddItem( new Boots( Utility.RandomNeutralHue() ) );
			AddItem( new FancyShirt());
			AddItem( new Bandana());

			switch ( Utility.Random( 7 ))
			{
				case 0: AddItem( new Longsword() ); break;
				case 1: AddItem( new Cutlass() ); break;
				case 2: AddItem( new Broadsword() ); break;
				case 3: AddItem( new Axe() ); break;
				case 4: AddItem( new Club() ); break;
				case 5: AddItem( new Dagger() ); break;
				case 6: AddItem( new Spear() ); break;
			}

			if ( Utility.Random( 10 ) == 0 )
				PackItem( new EvilCutlass() );

			HairItemID = Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A );
			HairHue = Utility.RandomNondyedHue();

			PackGold( 205, 705 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public BrigandCapt( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}