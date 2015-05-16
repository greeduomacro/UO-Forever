using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a tree fellow corpse" )]
	public class TreeFellow : BaseCreature
	{
		[Constructable]
		public TreeFellow () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = NameList.RandomName("ethereal warrior");
			Body = 301;
            BaseSoundID = 0x1ba;

			SetStr( 526, 615 );
			SetDex( 66, 85 );
			SetInt( 226, 350 );

			SetHits( 316, 369 );

			SetDamage( 17, 27 );





            Alignment = Alignment.Elemental;
			
			
			
			

			SetSkill( SkillName.EvalInt, 85.1, 100.0 );
			SetSkill( SkillName.Magery, 85.1, 100.0 );
			SetSkill( SkillName.Meditation, 10.4, 50.0 );
			SetSkill( SkillName.MagicResist, 80.1, 95.0 );
			SetSkill( SkillName.Tactics, 80.1, 100.0 );
			SetSkill( SkillName.Wrestling, 80.1, 100.0 );

			Fame = 12500;
			Karma = -12500;

			VirtualArmor = 60;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );

		}

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() < 0.2)
            {
                c.DropItem(new SeedofRenewal());
            }
            Engines.ZombieEvent.ZombieEvent.AddItem(c);
            base.OnDeath(c);
        }

		public override int TreasureMapLevel{ get{ return 5; } }

        public TreeFellow(Serial serial)
            : base(serial)
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