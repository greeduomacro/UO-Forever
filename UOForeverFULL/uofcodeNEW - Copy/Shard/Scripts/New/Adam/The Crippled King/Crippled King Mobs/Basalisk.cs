using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a basalisk corpse" )]
	public class Basalisk : BaseCreature
	{
        public override string DefaultName { get { return "a basalisk"; } }

		[Constructable]
		public Basalisk() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{

		    Body = 0x31A;
		    Hue = 2407;

            BaseSoundID = 0x16A;

            SetStr(767, 945);
            SetDex(100, 120);
            SetInt(46, 70);

            SetHits(476, 552);

            SetDamage(20, 25);

            SpecialTitle = "Stone Construct";
            TitleHue = 891;

            SetSkill(SkillName.MagicResist, 130.1, 165.0);
            SetSkill(SkillName.Tactics, 100.0, 100.0);
            SetSkill(SkillName.Anatomy, 100.0, 100.0);
            SetSkill(SkillName.Wrestling, 130.1, 140.0);

            Fame = 4500;
            Karma = -4500;

            VirtualArmor = 50;
		}

		public override int GetIdleSound()
		{
			return 0x2CE;
		}

		public override int GetDeathSound()
		{
			return 0x2CC;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override int GetAttackSound()
		{
			return 0x2C8;
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool AutoDispel{ get{ return !Controlled; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override int Meat{ get{ return 19; } }
		public override int Hides{ get{ return 20; } }
		public override int Scales{ get{ return 5; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Green; } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomGreenHue(); } }
        public override Poison HitPoison { get { return (0.6 >= Utility.RandomDouble() ? Poison.Deadly : Poison.Lethal); } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.Gems, 1);
            AddLoot(LootPack.Potions);
            AddPackedLoot(LootPack.AverageProvisions, typeof(Bag));
            AddPackedLoot(LootPack.AverageProvisions, typeof(Pouch));
        }

        public override bool OnBeforeDeath()
        {
            if (0.1 >= Utility.RandomDouble())
                PackItem(new StoneClaw());
            return base.OnBeforeDeath();
        }

		public Basalisk( Serial serial ) : base( serial )
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