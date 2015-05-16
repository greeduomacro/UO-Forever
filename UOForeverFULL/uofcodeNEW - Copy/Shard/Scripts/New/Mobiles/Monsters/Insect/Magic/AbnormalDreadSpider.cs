using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an abnormal dread spider corpse" )]
	public class AbnormalDreadSpider : BaseCreature
	{
		public override string DefaultName{ get{ return "an abnormal dread spider"; } }

		[Constructable]
		public AbnormalDreadSpider () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 11;
			BaseSoundID = 1170;
			Hue = 33929;

			SetStr( 900, 910 );
			SetDex( 200, 350 );
			SetInt( 500, 500 );

			SetHits( 900, 901 );

			SetDamage( 30, 35 );

			
			

			
			
			
			
			

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
			if ( 0.25 > Utility.RandomDouble() )
				AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.Rich, 2 );
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 3; } }

		public AbnormalDreadSpider( Serial serial ) : base( serial )
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