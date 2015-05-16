using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a lion corpse" )]
	public class Lion : BaseCreature
	{
		[Constructable]
		public Lion () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a lion";
			Body = 0xD6;
			BaseSoundID = 0x462;
			Hue = 248;

			SetStr( 401, 430 );
			SetDex( 133, 152 );
			SetInt( 101, 140 );

			SetHits( 3 * 241, 3 * 258 );

			SetDamage( 11, 17 );

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 65.1, 90.0 );
			SetSkill( SkillName.Wrestling, 65.1, 80.0 );

			//Fame = 5500;
			//Karma = -5500;

			VirtualArmor = 46;
		}

		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 7; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Feline; } }

		public Lion( Serial serial ) : base( serial )
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