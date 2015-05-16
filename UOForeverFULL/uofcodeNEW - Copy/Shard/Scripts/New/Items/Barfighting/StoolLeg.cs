using System;
using Server.Network;

namespace Server.Items
{
	public class StoolLeg : Club
	{
		public override int NewMinDamage{ get{ return 1; } }
		public override int NewMaxDamage{ get{ return 4; } }

		private int m_HitsLeft;

		[Constructable]
		public StoolLeg() : base()
		{
			Name = "a bit of wood";
			m_HitsLeft = Utility.RandomMinMax( 5, 8 );
		}

		public StoolLeg( Serial serial ) : base( serial )
		{
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damagebonus )
		{
			base.OnHit( attacker, defender, damagebonus );

			m_HitsLeft -= 1;

			if ( m_HitsLeft == 0 )
			{
				this.Delete();
				attacker.SendMessage( "The shards of wood break on the last hit and is no more!" );
			}
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