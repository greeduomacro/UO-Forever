using System;
using System.Xml;
using Server;
using Server.Mobiles;

namespace Server.Regions
{
	public class YoungDungeonRegion : DungeonRegion
	{
		public override bool YoungProtected { get { return true; } }

		public YoungDungeonRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}

		public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
		{
			if ( m is PlayerMobile && ((PlayerMobile)m).Young )
				personal = LightCycle.DayLevel;

			global = LightCycle.DungeonLevel;
		}

		public override bool CanUseStuckMenu( Mobile m )
		{
			if ( !(m is PlayerMobile && ((PlayerMobile)m).Young) && this.Map == Map.Felucca )
				return false;

			return base.CanUseStuckMenu( m );
		}
	}
}