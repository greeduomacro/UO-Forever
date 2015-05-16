using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a fire elemental corpse" )]
	public class SummonedFireElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override string DefaultName{ get{ return "a fire elemental"; } }

		[Constructable]
		public SummonedFireElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 15;
			BaseSoundID = 838;

			SetStr( 200 );
			SetDex( 200 );
			SetInt( 100 );

			SetDamage( 9, 14 );

			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 90.0 );
			SetSkill( SkillName.Magery, 90.0 );
			SetSkill( SkillName.MagicResist, 85.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 92.0 );

			VirtualArmor = 40;
			ControlSlots = 4;

			AddItem( new LightSource() );
		}

		public override int DefaultBloodHue{ get{ return -1; } }

		public SummonedFireElemental( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}