using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a spider corpse" )]
	public class CamelSpiderHatchling : BaseCreature
	{
		public override string DefaultName{ get{ return "a camel spider hatchling"; } }

		[Constructable]
		public CamelSpiderHatchling() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 717;
			Hue = 546;
			BaseSoundID = 397;

			SetStr( 73, 115 );
			SetDex( 76, 95 );
			SetInt( 16, 30 );

			SetHits( 50, 63 );
			SetDamage( 5, 10 );

			SetSkill( SkillName.Poisoning, 80.1, 100.0 );
			SetSkill( SkillName.MagicResist, 30.1, 35.0 );
			SetSkill( SkillName.Tactics, 60.3, 75.0 );
			SetSkill( SkillName.Wrestling, 50.3, 65.0 );

			Fame = 2000;
			Karma = -2000;

			VirtualArmor = 28;

			Timer.DelayCall( TimeSpan.FromSeconds( 15.0 + Utility.Random( 16 ) ), new TimerCallback( Evolve ) );
		}

		private void Evolve()
		{
			if ( !Deleted && Alive )
			{
				CamelSpider spid = new CamelSpider( false );
				spid.MoveToWorld( Location, Map );
				Delete();
			}
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override Poison HitPoison{ get{ return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly); } }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
        }

		public CamelSpiderHatchling( Serial serial ) : base( serial )
		{
			Timer.DelayCall( TimeSpan.FromSeconds( 15.0 + Utility.Random( 16 ) ), new TimerCallback( Evolve ) );
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
