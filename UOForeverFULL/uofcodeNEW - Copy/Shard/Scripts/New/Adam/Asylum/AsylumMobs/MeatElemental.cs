using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a meat construct corpse" )]
	public class MeatElemental : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.BleedAttack;
		}

		public override string DefaultName{ get{ return "a meaty construct"; } }

		[Constructable]
		public MeatElemental() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 304;
			BaseSoundID = 684;

            SpecialTitle = "[Asylum Guardian]";
            TitleHue = 1161;

			SetStr( 176, 200 );
			SetDex( 51, 75 );
			SetInt( 46, 70 );

			SetHits( 106, 120 );

			SetDamage( 18, 22 );			

			SetSkill( SkillName.MagicResist, 50.1, 75.0 );
			SetSkill( SkillName.Tactics, 55.1, 80.0 );
			SetSkill( SkillName.Wrestling, 60.1, 70.0 );

			Fame = 1000;
			Karma = -1800;

			VirtualArmor = 34;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );       
		}

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() <= 0.01)
            {
                c.DropItem(new Ham{Hue = 1372, Name = "rotting meat"});
            }
            base.OnDeath(c);
        }

		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }

        public MeatElemental(Serial serial)
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