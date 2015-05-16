using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a crystal elemental corpse" )]
	public class CrystalElemental : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.BleedAttack;
		}

		public override string DefaultName{ get{ return "a crystal elemental"; } }

		[Constructable]
		public CrystalElemental() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 300;
			BaseSoundID = 278;

			SetStr( 136, 160 );
			SetDex( 51, 65 );
			SetInt( 86, 110 );

			SetHits( 150 );

			SetDamage( 10, 15 );




            Alignment = Alignment.Elemental;
			
			
			
			

			SetSkill( SkillName.EvalInt, 70.1, 75.0 );
			SetSkill( SkillName.Magery, 70.1, 75.0 );
			SetSkill( SkillName.Meditation, 65.1, 75.0 );
			SetSkill( SkillName.MagicResist, 80.1, 90.0 );
			SetSkill( SkillName.Tactics, 75.1, 85.0 );
			SetSkill( SkillName.Wrestling, 65.1, 75.0 );

			Fame = 6500;
			Karma = -6500;

			VirtualArmor = 54;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 1 );
			AddLoot( LootPack.Average );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public CrystalElemental( Serial serial ) : base( serial )
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