using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class BaseHunter : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }

		public BaseHunter() : this( AIType.AI_Melee )
		{
		}

		public BaseHunter( AIType ai ) : base( ai, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();

			if ( Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 65;

			Utility.AssignRandomHair( this, HairHue );
			Utility.AssignRandomFacialHair( this, HairHue );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public BaseHunter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}