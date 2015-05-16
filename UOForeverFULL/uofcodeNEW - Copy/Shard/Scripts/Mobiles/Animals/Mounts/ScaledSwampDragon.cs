using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a swamp dragon corpse" )]
	public class ScaledSwampDragon : BaseMount
	{
		public override string DefaultName{ get{ return "a swamp dragon"; } }

        public override int InternalItemItemID { get { return 0x3EBE; } }

		[Constructable]
		public ScaledSwampDragon() : base( 0x31F, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			SetStr( 201, 300 );
			SetDex( 66, 85 );
			SetInt( 61, 100 );

			SetHits( 121, 180 );

			SetDamage( 3, 4 );

			
			

			
			
			
			
			

			SetSkill( SkillName.Anatomy, 45.1, 55.0 );
			SetSkill( SkillName.MagicResist, 45.1, 55.0 );
			SetSkill( SkillName.Tactics, 45.1, 55.0 );
			SetSkill( SkillName.Wrestling, 45.1, 55.0 );

			Fame = 2000;
			Karma = -2000;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 93.9;
		}

		public override bool OverrideBondingReqs()
		{
			return true;
		}

		public override double GetControlChance( Mobile m, bool useBaseSkill )
		{
			return 1.0;
		}

		public override bool AutoDispel{ get{ return !Controlled; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomGreenHue(); } }

		public ScaledSwampDragon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}