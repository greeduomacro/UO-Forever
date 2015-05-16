using System;
using System.Xml;
using Server;

namespace Server.Regions
{
	public interface IUnjust
	{
		Point3D Home{ get; set; }
		Map HomeMap{ get; set; }
	}

	public class JusticeShrineRegion : ShrineRegion
	{
		public JusticeShrineRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}

		public override bool OnMoveInto( Mobile m, Direction d, Point3D newLocation, Point3D oldLocation )
		{
			if ( m is IUnjust && !Contains( oldLocation ) )
			{
				IUnjust uj = m as IUnjust;

				if ( uj.HomeMap == null || uj.HomeMap == Map.Internal )
					uj.HomeMap = Map.Felucca;

				m.MoveToWorld( uj.Home, uj.HomeMap );
				return false;
			}

			return true;
		}
	}
}