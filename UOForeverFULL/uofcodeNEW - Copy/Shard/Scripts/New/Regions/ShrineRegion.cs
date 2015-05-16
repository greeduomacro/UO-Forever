using System;
using System.Xml;
using Server;

namespace Server.Regions
{
	public class ShrineRegion : BaseRegion
	{
		public ShrineRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}
	}
}