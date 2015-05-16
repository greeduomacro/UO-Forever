using System;
using Server;

namespace Server.Ethics
{
	public class RankDefinition
	{
		private int m_Points;

		private TextDefinition m_Title;

		public int Points{ get{ return m_Points; } }

		public TextDefinition Title{ get { return m_Title; } }

		public RankDefinition( int points, TextDefinition title )
		{
			m_Points = points;
			m_Title = title;
		}
	}
}