using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Factions;
using Server.Ethics;

namespace Server.Mobiles
{
	[CorpseName( "an ogre corpse" )]
	public class OgreChamp : BaseCreature
	{
		public override string DefaultName{ get{ return "Thagokk"; } }

		[Constructable]
		public OgreChamp () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 267;
			BaseSoundID = 427;
            Title = "the Ogre Champion";
			Hue = 0;


			SetStr( 867, 1045 );
			SetDex( 166,175 );
			SetInt( 146, 170 );

			SetHits(3476,4552 );

			SetDamage( 13, 19 );

			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 155.1, 170.0 );
			SetSkill( SkillName.Tactics, 120.1, 140.0 );
			SetSkill( SkillName.Wrestling, 120.1, 130.0 );
			SetSkill( SkillName.EvalInt, 120.1, 130.0 );
			SetSkill( SkillName.Magery, 120.1, 130.0 );
			SetSkill( SkillName.Meditation, 120.1, 130.0 );
            SetSkill(SkillName.DetectHidden, 120.1, 130.0);

			Fame = 25000;
			Karma = -25000;

			VirtualArmor = 150;

			PackItem( new Club() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 9 );
            if (0.02 >= Utility.RandomDouble())
            { SkillScroll scroll = new SkillScroll(); scroll.Randomize(); PackItem(scroll); }
			AddPackedLoot( LootPack.RichProvisions, typeof( Pouch ) );
			AddPackedLoot( LootPack.MeagerProvisions, typeof( Bag ) );
			if ( 0.5 > Utility.RandomDouble() )
				AddPackedLoot( LootPack.AverageProvisions, typeof( Backpack ) );
		}
			public override void GenerateLoot( bool spawning )
		{
			base.GenerateLoot( spawning );

			if ( !spawning )
			{
				double rand = Utility.RandomDouble();
				if ( 0.025 > rand )
					PackItem( new OrcishKinMask(193) );
				else if ( 0.125 > rand )
					PackItem( new OgreIdol() );
			}
		}

		        public override bool CanRummageCorpses{ get{ return true; } }
		        public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		        public override int TreasureMapLevel{ get{ return 3; } }
        		public override int Meat{ get{ return 2; } }
		        public override int BreathColdDamage{ get{ return 9; } }
		        public override bool IsScaryToPets{ get{ return true; } }
				public override bool AutoDispel{ get{ return true; } }
                public override bool BardImmune{ get{ return true; } }
                public override bool Unprovokable{ get{ return true; } }
                public override Poison HitPoison{ get{ return Poison. Greater ; } }
                public override bool AlwaysMurderer{ get{ return true; } }
				public override bool IsScaredOfScaryThings{ get{ return false; } }

             
		public OgreChamp( Serial serial ) : base( serial )
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