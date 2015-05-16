using System;
using System.IO;
using System.Xml;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Regions
{
	public class CraftShopRegion : GuardedRegion
	{
		private CraftSkillType m_Skills;
		public CraftSkillType Skills{ get{ return m_Skills; } set{ m_Skills = value; } }

		public override bool IsDisabled()
		{
			if ( Parent is GuardedRegion )
				return ((GuardedRegion)Parent).IsDisabled();

			return base.IsDisabled();
		}

		public CraftShopRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
			int skill = 0;
			ReadInt32( xml, "skill", ref skill, true );
			m_Skills = (CraftSkillType)skill;
		}

		public bool HasSkill( CraftSkillType skill )
		{
			return ( ( m_Skills & skill ) != 0 );
		}

		public override void OnExit(Mobile m)
		{
			if ( TestCenter.Enabled )
				m.SendMessage("You have left {0}", this.Name);
			base.OnExit(m);

		}

		public override void OnEnter(Mobile m)
		{
			if ( TestCenter.Enabled )
				m.SendMessage("You have entered {0}", this.Name);
			base.OnEnter(m);
		}
	}
}