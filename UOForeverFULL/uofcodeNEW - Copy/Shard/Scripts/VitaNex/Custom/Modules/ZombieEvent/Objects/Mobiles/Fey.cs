using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Mobiles
{
	[CorpseName( "an fey warrior corpse" )]
	public class FeyWarrior : BaseCreature
	{
		public override bool InitialInnocent{ get{ return true; } }

		[Constructable]
		public FeyWarrior() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "ethereal warrior" );
			Body = 123;

			SetStr( 586, 785 );
			SetDex( 177, 255 );
			SetInt( 351, 450 );

			SetHits( 352, 471 );

			SetDamage( 13, 19 );

			SetSkill( SkillName.Anatomy, 50.1, 75.0 );
			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 99.1, 100.0 );
			SetSkill( SkillName.Meditation, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 90.1, 100.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 97.6, 100.0 );

			Fame = 7000;
			Karma = 7000;

			VirtualArmor = 120;
		}

		public override int TreasureMapLevel{ get{ return 5; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 1 );
			AddLoot( LootPack.Gems );
		}

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() < 0.2)
            {
                c.DropItem(new FeyWings());
            }
            Engines.ZombieEvent.ZombieEvent.AddItem(c);
            base.OnDeath(c);
        }


		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override int Feathers{ get{ return 30; } }

		public override int GetAngerSound()
		{
			return 0x2F8;
		}

		public override int GetIdleSound()
		{
			return 0x2F8;
		}

		public override int GetAttackSound()
		{
			return Utility.Random( 0x2F5, 2 );
		}

		public override int GetHurtSound()
		{
			return 0x2F9;
		}

		public override int GetDeathSound()
		{
			return 0x2F7;
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			int rand = Utility.Random( 10, 10 );

			defender.Stam -= rand;
			defender.Mana -= rand;
			defender.Damage( rand, this );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			int rand = Utility.Random( 10, 10 );

			attacker.Stam -= rand;
			attacker.Mana -= rand;
			attacker.Damage( rand, this );
		}

        public FeyWarrior(Serial serial)
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