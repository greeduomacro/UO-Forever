using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a gaman corpse" )]
	public class Gaman : BaseCreature
	{
		public override string DefaultName{ get{ return "a gaman"; } }

		[Constructable]
		public Gaman() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Body = 248;

			SetStr( 146, 175 );
			SetDex( 111, 150 );
			SetInt( 46, 60 );

			SetHits( 131, 160 );
			SetMana( 0 );

			SetDamage( 6, 11 );

			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 37.6, 42.5 );
			SetSkill( SkillName.Tactics, 70.6, 83.0 );
			SetSkill( SkillName.Wrestling, 50.1, 57.5 );

			Fame = 2000;
			Karma = -2000;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 68.7;
		}

		public override int GetAngerSound()
		{
			return 0x4F8;
		}

		public override int GetIdleSound()
		{
			return 0x4F7;
		}

		public override int GetAttackSound()
		{
			return 0x4F6;
		}

		public override int GetHurtSound()
		{
			return 0x4F9;
		}

		public override int GetDeathSound()
		{
			return 0x4F5;
		}

		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 15; } }
		public override FoodType FavoriteFood{ get{ return FoodType.GrainsAndHay; } }

		public Gaman( Serial serial ) : base( serial )
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