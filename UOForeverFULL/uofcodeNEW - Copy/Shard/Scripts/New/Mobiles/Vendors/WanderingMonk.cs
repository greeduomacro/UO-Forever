using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class WanderingMonk : BaseHealer
	{
		public override bool CanTeach{ get{ return true; } }

		public override bool CheckTeach( SkillName skill, Mobile from )
		{
			if ( !base.CheckTeach( skill, from ) )
				return false;

			return ( skill == SkillName.EvalInt )
				|| ( skill == SkillName.MagicResist )
				|| ( skill == SkillName.Tactics )
				|| ( skill == SkillName.Macing )
				|| ( skill == SkillName.Wrestling );
		}

		[Constructable]
		public WanderingMonk()
		{
			Title = "the monk";

			AddItem( new GnarledStaff() );

			SetSkill( SkillName.Macing, 100.0 );
		}

		public override bool ClickTitle{ get{ return false; } } // Do not display title in OnSingleClick

		public override bool CheckResurrect( Mobile m )
		{
			return false;
		}

		public WanderingMonk( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}