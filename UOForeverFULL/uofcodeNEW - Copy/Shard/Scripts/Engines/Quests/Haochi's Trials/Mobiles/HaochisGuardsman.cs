using System;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Items;

namespace Server.Engines.Quests.Samurai
{
	public class HaochisGuardsman : BaseQuester
	{
		[Constructable]
		public HaochisGuardsman() : base( "the Guardsman of Daimyo Haochi" )
		{
		}

		public override void InitBody()
		{
			InitStats( 100, 100, 25 );

			Hue = Race.Human.RandomSkinHue();

			Female = false;
			Body = 0x190;
			Name = NameList.RandomName( "male" );
		}

		public override void InitOutfit()
		{
			Utility.AssignRandomHair( this );

			AddItem( Immovable(new LeatherDo()) );
			AddItem( Immovable(new LeatherHiroSode()) );
			AddItem( Immovable(new SamuraiTabi( Utility.RandomNondyedHue() )) );

			switch ( Utility.Random( 3 ) )
			{
				case 0: AddItem( Immovable(new StuddedHaidate()) ); break;
				case 1: AddItem( Immovable(new PlateSuneate()) ); break;
				default: AddItem( Immovable(new LeatherSuneate()) ); break;
			}

			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( Immovable(new DecorativePlateKabuto()) ); break;
				case 1: AddItem( Immovable(new ChainHatsuburi()) ); break;
				case 2: AddItem( Immovable(new LightPlateJingasa()) ); break;
				default: AddItem( Immovable(new LeatherJingasa()) ); break;
			}

			Item weapon;
			switch ( Utility.Random( 3 ) )
			{
				case 0: weapon = new NoDachi(); break;
				case 1: weapon = new Lajatang(); break;
				default: weapon = new Wakizashi(); break;
			}
			weapon.Movable = false;
			AddItem( weapon );
		}

		public override int TalkNumber{ get	{ return -1; } }

		public override void OnTalk( PlayerMobile player, bool contextMenu )
		{
		}

		public HaochisGuardsman( Serial serial ) : base( serial )
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