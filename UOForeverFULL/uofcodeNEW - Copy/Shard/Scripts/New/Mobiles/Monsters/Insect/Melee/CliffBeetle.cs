using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a cliff beetle corpse" )]
	public class CliffBeetle : BaseCreature
	{
		public override string DefaultName{ get{ return "a cliff beetle"; } }

		[Constructable]
		public CliffBeetle() : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.15, 0.2 )
		{
			Body = 791;
			Hue = Utility.Random( 1102, 8 );

			SetStr( 500 );
			SetDex( 50 );
			SetInt( 50 );

			SetHits( 500, 750 );

			SetDamage( 17, 20 );

			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 97.0, 99.9 );
			SetSkill( SkillName.Wrestling, 97.0, 99.9 );

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 30;

			PackGold( 650, 850 );
			PackWeapon( 0, 5 );
			PackGem();
			PackGem();
			PackGem();
			PackGem();

			switch ( Utility.Random( 10 ) )
			{
				case 0: PackWeapon( 0, 5 ); break;
				case 1: PackArmor( 0, 5 ); break;
			}

			switch ( Utility.Random( 15 ) )
			{
				case 0: PackWeapon( 1, 5 ); break;
				case 1: PackArmor( 1, 5 ); break;
			}
		}

		public override int GetAngerSound()
		{
			return 0x21D;
		}

		public override int GetIdleSound()
		{
			return 0x21D;
		}

		public override int GetAttackSound()
		{
			return 0x162;
		}

		public override int GetHurtSound()
		{
			return 0x163;
		}

		public override int GetDeathSound()
		{
			return 0x21D;
		}

		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override int Meat{ get{ return 4; } }
		public override int Scales{ get{ return 8; } }
		public override ScaleType ScaleType{ get{ return ( Body == 12 ? ScaleType.Black : ScaleType.Black ); } }

		public CliffBeetle( Serial serial ) : base( serial )
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