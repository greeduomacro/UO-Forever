using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a balron corpse" )]
	public class DoomGuardianDaemon : BaseCreature
	{
		[Constructable]
		public DoomGuardianDaemon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "the Guardian of Doom";
			Body = 40;
			BaseSoundID = 357;
			Hue = 2403;

            Alignment = Alignment.Demon;

			SetStr( 1186, 1385 );
			SetDex( 277, 355 );
			SetInt( 251, 350 );

			SetHits( 792, 911 );

			SetDamage( 25, 31 );

			SetSkill( SkillName.Anatomy, 55.1, 70.0 );
			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 95.5, 100.0 );
			SetSkill( SkillName.Meditation, 55.1, 70.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 100.1, 115.0 );
			SetSkill( SkillName.Wrestling, 100.1, 115.0 );

			Fame = 20000;
			Karma = -20000;

			VirtualArmor = 95;

			PackItem( new Longsword() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.MedScrolls, 2 );
		}



		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Meat{ get{ return 3; } }

		public DoomGuardianDaemon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}