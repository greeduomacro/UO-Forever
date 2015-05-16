using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a devourer of souls corpse" )]
	public class Devourer : BaseCreature
	{
		public override string DefaultName{ get{ return "a devourer of souls"; } }

		[Constructable]
		public Devourer() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 303;
			BaseSoundID = 357;

			SetStr( 801, 950 );
			SetDex( 126, 175 );
			SetInt( 201, 250 );

			SetHits( 650 );

			SetDamage( 22, 26 );





            Alignment = Alignment.Demon;
			
			
			
			

			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 90.1, 100.0 );
			SetSkill( SkillName.Meditation, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 90.1, 105.0 );
			SetSkill( SkillName.Tactics, 75.1, 85.0 );
			SetSkill( SkillName.Wrestling, 80.1, 100.0 );

			Fame = 9500;
			Karma = -9500;

			VirtualArmor = 44;

			PackNecroReg( 24, 45 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int Meat{ get{ return 3; } }
		//public override int DefaultBloodHue{ get{ return -2; } }
		//public override int BloodHueTemplate{ get{ return Utility.RandomGreenHue(); } }

		public Devourer( Serial serial ) : base( serial )
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