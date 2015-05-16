using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an imp corpse" )]
	public class ArcaneFiend : BaseCreature
	{
		public override string DefaultName{ get{ return "an imp"; } }
		public override double DispelDifficulty { get { return 70.0; } }
		public override double DispelFocus { get { return 20.0; } }

		public override PackInstinct PackInstinct { get { return PackInstinct.Daemon; } }
		public override bool BleedImmune { get { return true; } }	//TODO: Verify on OSI.  Guide says this.

		[Constructable]
		public ArcaneFiend() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 74;
			BaseSoundID = 422;

			SetStr( 55 );
			SetDex( 40 );
			SetInt( 60 );

			SetDamage( 10, 14 );

			
			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 20.1, 30.0 );
			SetSkill( SkillName.Magery, 60.1, 70.0 );
			SetSkill( SkillName.MagicResist, 30.1, 50.0 );
			SetSkill( SkillName.Tactics, 42.1, 50.0 );
			SetSkill( SkillName.Wrestling, 40.1, 44.0 );

			Fame = 0;
			Karma = 0;

			ControlSlots = 1;
		}

		public ArcaneFiend( Serial serial ) : base( serial )
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