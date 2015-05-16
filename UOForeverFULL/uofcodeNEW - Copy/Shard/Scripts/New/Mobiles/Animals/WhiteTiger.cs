using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a white tiger corpse" )]
	public class WhiteTiger : BaseCreature
	{
		[Constructable]
		public WhiteTiger() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a white tiger";
			Body = 0xD6;
			BaseSoundID = 0x462;
			Hue = 1150;

			SetStr( 416, 505 );
			SetDex( 146, 165 );
			SetInt( 566, 655 );

			SetHits( 2 * 250, 2 * 303 );

			SetDamage( 11, 13 );

			
			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 150.5, 200.0 );
			SetSkill( SkillName.Tactics, 50.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 80.0 );

			VirtualArmor = 50;
		}

		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 7; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Feline; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public WhiteTiger( Serial serial ) : base( serial )
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