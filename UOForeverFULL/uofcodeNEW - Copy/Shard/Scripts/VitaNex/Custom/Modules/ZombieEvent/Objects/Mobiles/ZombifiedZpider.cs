using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a decaying spider corpse" )]
	public class ZombieSpider : BaseCreature
	{
		public override string DefaultName{ get{ return "a zombified spider"; } }

		[Constructable]
		public ZombieSpider () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 11;
			BaseSoundID = 1170;

			SetStr( 196, 220 );
			SetDex( 126, 145 );
			SetInt( 286, 310 );

			SetHits( 118, 132 );

		    Hue = 61;

			SetDamage( 5, 17 );

			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 65.1, 80.0 );
			SetSkill( SkillName.Magery, 65.1, 80.0 );
			SetSkill( SkillName.Meditation, 65.1, 80.0 );
			SetSkill( SkillName.MagicResist, 45.1, 60.0 );
			SetSkill( SkillName.Tactics, 55.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 75.0 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 36;

			PackItem( new SpidersSilk( 8 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.OldAverage );
		}

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() < 0.2)
            {
                c.DropItem(new SpiderCarapace());
            }
            Engines.ZombieEvent.ZombieEvent.AddItem(c);
            base.OnDeath(c);
        }

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 3; } }

        public ZombieSpider(Serial serial)
            : base(serial)
		{
		}

        public override bool CheckFlee()
        {
            return false;
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

			if ( BaseSoundID == 263 )
				BaseSoundID = 1170;
		}
	}
}