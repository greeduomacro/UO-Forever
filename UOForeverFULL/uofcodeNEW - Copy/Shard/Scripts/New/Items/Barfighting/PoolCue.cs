using System;
using Server.Network;

namespace Server.Items
{
	public class PoolCue : QuarterStaff
	{
		public override int NewMinDamage{ get{ return 15; } }
		public override int NewMaxDamage{ get{ return 20; } }

		[Constructable]
		public PoolCue() : base()
		{
			Name = "a pool cue";
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damagebonus )
		{
			base.OnHit( attacker, defender, damagebonus );

			this.Delete();
			attacker.SendMessage( "The pool cue breaks as it smashes over their head!" );
			defender.SendMessage( "Your head breaks their pool cue!" );

			StoolLeg sl = new StoolLeg();
			sl.MoveToWorld( defender.Location, defender.Map );

			attacker.AddToBackpack( new StoolLeg() );
		}

		public override bool OnEquip( Mobile from )
		{
			SkillName sk;

			double swrd = from.Skills[SkillName.Swords].Value;
			double fenc = from.Skills[SkillName.Fencing].Value;
			double mcng = from.Skills[SkillName.Macing].Value;
			double wres = from.Skills[SkillName.Wrestling].Value;
			double arch = from.Skills[SkillName.Archery].Value;
			double val;

			sk = SkillName.Swords;
			val = swrd;

			if ( fenc > val ){ sk = SkillName.Fencing; val = fenc; }
			if ( mcng > val ){ sk = SkillName.Macing; val = mcng; }
			if ( wres > val ){ sk = SkillName.Wrestling; val = wres; }
			if ( arch > val ){ sk = SkillName.Archery; val = arch; }

			this.Skill = sk;

			return true;
		}

		public PoolCue( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}