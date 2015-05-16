using System;
using Server;
using Server.Items;
using Server.Engines.Craft;

namespace Server.Mobiles
{
	[CorpseName( "a stone dragon corpse" )]
	public class StoneDragon : BaseCreature
	{
		public override string DefaultName{ get{ return "a stone wyrm"; } }

		[Constructable]
		public StoneDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.1, 0.15 )
		{
			Body = 46;
			BaseSoundID = 362;
			Hue = 2407;

			SetStr( 1296, 1385 );
			SetDex( 186, 275 );
			SetInt( 786, 875 );

			SetHits( 858, 911 );

			SetDamage( 26, 37 );


            SpecialTitle = "Stone Construct";
            TitleHue = 891;

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 100.1, 120.0 );
			SetSkill( SkillName.Magery, 100.1, 120.0 );
			SetSkill( SkillName.Meditation, 69.5, 86.0 );
			SetSkill( SkillName.MagicResist, 115.5, 165.0 );
			SetSkill( SkillName.Tactics, 110.6, 125.0 );
			SetSkill( SkillName.Wrestling, 110.6, 125.0 );

			Fame = 25000;
			Karma = -25000;

			VirtualArmor = 85;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Gems, 5 );
			AddLoot( LootPack.HighScrolls );
			AddLoot( LootPack.MedScrolls, 3 );
		}

		public override void GenerateLoot( bool spawning )
		{
			base.GenerateLoot( spawning );

			if ( !spawning )
				PackBagofRegs( (0.25 > Utility.RandomDouble()) ? 75 : Utility.RandomMinMax( 35, 50 ) );
		}

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

	//	public override bool Unprovokable{ get{ return true; } }
		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Hides{ get{ return 35; } }
		public override int Meat{ get{ return 15; } }
		public override int Scales{ get{ return 18; } }
		public override ScaleType ScaleType{ get{ return (ScaleType)Utility.Random( 4 ); } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int DefaultBloodHue{ get{ return -1; } }

        public StoneDragon(Serial serial)
            : base(serial)
		{
		}

        public override bool OnBeforeDeath()
        {
            switch (Utility.Random(500))
            {
                case 0: PackItem(new LeatherDyeTub());
                    break;
                case 1: PackItem(new DragonHead()); break;


            }

            if (0.1 >= Utility.RandomDouble())
                PackItem(new StoneScale());
            if (!Controlled)
            {
                if (0.05 > Utility.RandomDouble())
                {
                    PackItem(new DragonBoneShards());
                }
                if (0.001 > Utility.RandomDouble())
                {
                    PackItem(new DragonHeart());
                }
            }
            return base.OnBeforeDeath();
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

