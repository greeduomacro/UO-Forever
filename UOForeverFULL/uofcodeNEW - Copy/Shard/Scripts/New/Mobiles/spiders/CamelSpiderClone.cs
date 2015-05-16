using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a spider corpse" )]
	public class CamelSpiderClone : BaseCreature
	{



        [Constructable]
        public CamelSpiderClone()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a camel spider";
            Body = 48;
            Hue = 546;
            BaseSoundID = 397;

            SetStr(73, 115);
            SetDex(100, 120);
            SetInt(16, 30);

            SetHits(400, 500);
            SetMana(0);

            SetDamage(15, 20);

            
            

            
            
            
            
            

            SetSkill(SkillName.Poisoning, 80.1, 100.0);
            SetSkill(SkillName.MagicResist, 30.1, 35.0);
            SetSkill(SkillName.Tactics, 60.3, 75.0);
            SetSkill(SkillName.Wrestling, 60.3, 75.0);

            Fame = 2000;
            Karma = -2000;

            VirtualArmor = 28;

            Tamable = false;
            ControlSlots = 1;
            MinTameSkill = 47.1;
        }

			



		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly); } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Gems);
        }

		public CamelSpiderClone( Serial serial ) : base( serial )
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
