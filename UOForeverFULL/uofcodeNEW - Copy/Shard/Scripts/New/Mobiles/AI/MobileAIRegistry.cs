using System;
using Server;

namespace Server.Mobiles
{
	public class MobileAIRegistry
	{
		private static readonly int m_AICount = 11;
		private static string[] m_AINames = new string[m_AICount];
		private static BaseAI[] m_AIValues = new BaseAI[m_AICount];

		public static void Register( BaseAI ai )
		{
			m_AINames[ai.Index] = ai.Name;
			m_AIValues[ai.Index] = ai;
		}

		public static string[] GetAINames()
		{
			return m_AINames;
		}

		public static BaseAI[] GetAIValues()
		{
			return m_AIValues;
		}

		public static BaseAI Parse( string value )
		{
			for ( int i = 0;i < m_AINames.Length; i++ )
			{
				if ( Insensitive.Equals( m_AINames[i], value ) )
					return m_AIValues[i];
			}

			throw new ArgumentException( String.Format( "Invalid mobile ai name: {0}", value ) );
		}
	}
}