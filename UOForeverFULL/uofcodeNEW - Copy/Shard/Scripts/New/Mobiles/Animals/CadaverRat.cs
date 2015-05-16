using System;
using Server;

namespace Server.Mobiles
{
	[CorpseName( "a rat corpse" )]
	[TypeAlias( "Server.Mobiles.Cadaverrat" )]
	public class CadaverRat : BaseCreature
	{
		public override string DefaultName{ get{ return "a cadaver rat"; } }

		[Constructable]
		public CadaverRat() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.18, 0.35 )
		{
			Body = 238;
			BaseSoundID = 0x188;
			Hue = 1072;

			SetStr( 45, 84 );
			SetDex( 47, 80 );
			SetInt( 16, 30 );

			SetHits( 39, 49 );
			SetMana( 0 );

			SetDamage( 5, 9 );

			
			

			
			
			
			

			SetSkill( SkillName.MagicResist, 30.1, 40.0 );
			SetSkill( SkillName.Tactics, 39.3, 45.0 );
			SetSkill( SkillName.Wrestling, 39.3, 45.0 );

			Fame = 500;
			Karma = -500;

			VirtualArmor = 21;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor, 2 );
		}

		public override Poison HitPoison{ get{ return 0.25 > Utility.RandomDouble() ? Poison.Lethal : Poison.Regular; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override double HitPoisonChance{	get{ return 0.25; } }

		public CadaverRat( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}