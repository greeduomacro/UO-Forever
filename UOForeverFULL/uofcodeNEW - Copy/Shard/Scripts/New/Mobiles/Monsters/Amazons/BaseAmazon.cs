using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class BaseAmazon : BaseCreature
	{
		public BaseAmazon(
			AIType ai,
			FightMode mode,
			int iRangePerception,
			int iRangeFight,
			double dActiveSpeed,
			double dPassiveSpeed )
		: base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
		{
			Name = NameList.RandomName( "female" );
			Hue = 33220;
			Body = 0x191;
			Female = true;

			HairItemID = 0x203C;
			HairHue = 2127;

			AddItem( new BodySash( 58 ) );
		}

		public override bool ClickTitle{ get{ return false; } } // Do not display 'the Amazon' when single-clicking
		public override bool AlwaysMurderer{ get{ return true; } }

		public override int GetAttackSound()
		{
			return Utility.Random( 804, 6 );
		}

		public override int GetAngerSound()
		{
			return 825;
		}

		public override int GetDeathSound()
		{
			return Utility.Random( Utility.RandomBool() ? 788 : 336, 3);
		}

		public override int GetHurtSound()
		{
			return Utility.Random( 804, 3 );
		}

		//781, 782, 785, 817, 820, 821, 822
		public override int GetIdleSound()
		{
			switch( Utility.Random( 18 ) )
			{
				case 0: return 821;
				case 1: return 822;
				case 2: return 781;
				case 3: return 785;
				case 4: return 820;
				case 5: return 817;
				case 6: return 782;
			}

			return -1;
		}

		public void AddRingOfPower()
		{
			BaseJewel jewel = new GoldRing();
			//jewel.Resource = CraftResource.Amazon;
			jewel.Hue = 2126;
			jewel.Name = "a ring of power";
			//jewel.Attributes.BonusInt = Utility.Random( 1, 6 );
			AddItem( jewel );
		}

		public static Item MakeAmazonArmor( Item item )
		{
			item.Hue = 2126;
			if ( Utility.Random( 300 ) != 0 )
				item.SetSavedFlag( 0x1, true );

			if ( item is BaseArmor )
				((BaseArmor)item).Identified = true;
			else if ( item is BaseWeapon )
				((BaseWeapon)item).Identified = true;

			return item;
		}

		public BaseAmazon( Serial serial ) : base( serial )
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