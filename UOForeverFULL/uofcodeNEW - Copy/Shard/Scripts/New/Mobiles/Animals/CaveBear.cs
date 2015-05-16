using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a cave bear corpse" )]
	public class CaveBear : BaseCreature
	{
		public override string DefaultName{ get{ return "a cave bear"; } }

		[Constructable]
		public CaveBear() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.15, 0.2 )
		{
			Body = 212;
			BaseSoundID = 0xA3;
			Hue = Utility.RandomList( 1879, 1880, 1881, 1888, 1889, 1890 );

			SetStr( 226, 354 );
			SetDex( 405, 415 );
			SetInt( 10, 20 );

			SetHits( 426, 554 );

			SetDamage( 10, 15 );

			SetSkill( SkillName.MagicResist, 115.1, 125.0 );
			SetSkill( SkillName.Tactics, 90.1, 125.0 );
			SetSkill( SkillName.Wrestling, 96.1, 98.0 );

			Fame = 3000;
			Karma = 0;

			VirtualArmor = 50;

			PackGold( 125, 150 );
		}

		public override int Meat{ get{ return 14; } }
		public override int Hides{ get{ return 10; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }

		public CaveBear( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); //version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}