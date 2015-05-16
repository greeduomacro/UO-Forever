using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a white horse corpse" )]
	public class WhiteHorse : BaseMount
	{
		public override string DefaultName{ get{ return "a rare white horse"; } }

        public override int InternalItemItemID { get { return 0x3E9E; } }

		[Constructable]
		public WhiteHorse() : base( 0xBE, AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			BaseSoundID = 0xA8;
			Hue = 1150;

			SetStr( 376, 400 );
			SetDex( 91, 120 );
			SetInt( 291, 300 );

			SetHits( 226, 240 );

			SetDamage( 11, 30 );

			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 85.0, 100.0 );
			SetSkill( SkillName.Magery, 85.0, 100.0 );
			SetSkill( SkillName.MagicResist, 85.0, 100.0 );
			SetSkill( SkillName.Tactics, 85.0, 100.0 );
			SetSkill( SkillName.Wrestling, 85.0, 100.0 );
			SetSkill( SkillName.Anatomy, 85.0, 100.0 );

			Fame = 20000;
			Karma = -20000;

			VirtualArmor = 45;
			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 100.0;
		}

		public override void GenerateLoot()
		{
			PackItem( new Sapphire( Utility.RandomMinMax( 16, 16 ) ) );
		}

		public override bool HasBreath{ get{ return true; } } // cold breath enabled
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 100; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Daemon | PackInstinct.Equine; } }

		public WhiteHorse( Serial serial ) : base( serial )
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