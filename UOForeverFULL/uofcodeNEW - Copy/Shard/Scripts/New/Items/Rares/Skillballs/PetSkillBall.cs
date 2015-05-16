using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Targeting;

namespace Server.Items
{
	public class PetSkillBall : SkillBall
	{
		//public override string DefaultName{ get{ return "a pet skillball"; } }

		[Constructable]
		public PetSkillBall( int bonus, bool newbied, int days ) : base( bonus, newbied, days )
		{
		}

		[Constructable]
		public PetSkillBall( int bonus, bool newbied ) : this( bonus, false, -1 )
		{
		}

		[Constructable]
		public PetSkillBall( int bonus ) : this( bonus, false )
		{
		}

		[Constructable]
		public PetSkillBall() : this( 25 )
		{
		}

		public PetSkillBall( Serial serial ) : base( serial )
		{
		}

		public override void UpdateHue()
		{
			Hue = 1278;
		}

		public override void UpdateName()
		{
			if ( Expires )
				Name = "an unstable pet skill ball";
			else
				Name = "a pet skill ball";
		}

		public override string GetLabelName()
		{
			if ( Expires )
				return "a +{0} unstable pet skill ball";
			return "a +{0} pet skill ball";
		}

		public override SkillName[] GetAllowedSkills()
		{
			return m_PetSkills;
		}

		private static readonly SkillName[] m_PetSkills = new SkillName[]
			{
				SkillName.Wrestling,
				SkillName.Tactics,
				SkillName.MagicResist,
				SkillName.Anatomy,
				SkillName.Poisoning,
				SkillName.Magery,
				SkillName.EvalInt,
				SkillName.Meditation
			};

		public override void SendGump( Mobile from )
		{
			from.SendMessage( "Target your pet to use the skill ball." );
			from.Target = new PetSkillBallTarget( from, this );
		}

		public override void SendConfirmMessage( Mobile from, Mobile target, Skill skill )
		{
			from.SendMessage( "Your pet, {0}'s skill in {1} has been raised by {2}", target.Name, skill.Name, SkillBonus );
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

		public class PetSkillBallTarget : Target
		{
			private Mobile m_From;
			private PetSkillBall m_Ball;

			public PetSkillBallTarget( Mobile from, PetSkillBall ball ) : base( 12, false, TargetFlags.None )
			{
				m_From = from;
				m_Ball = ball;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				BaseCreature creature = targeted as BaseCreature;
				if ( creature == null || !( creature.Controlled && creature.ControlMaster == from ) )
					from.SendMessage( "You cannot use this skill ball on that." );
				else if ( from.HasGump( typeof(SkillBall) ) )
					from.SendMessage( "You are already using a skill ball." );
				else if ( m_Ball.Expires && DateTime.UtcNow >= m_Ball.ExpireDate )
					m_Ball.SendLocalizedMessageTo( from, 1042544 ); // This item is out of charges.
				else
					from.SendGump( new SkillBallGump( from, creature, m_Ball ) );
			}
		}
	}
}