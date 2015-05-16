using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Ethics
{
	public class EthicDefinition
	{
		private bool m_ArticleAn;

		private int m_PrimaryHue;

        private int m_TitleHue;

		private TextDefinition m_Title;
		private TextDefinition m_NPCAdjunct;

		private TextDefinition m_JoinPhrase;

		private Power[] m_Powers;

		private RankDefinition[] m_Ranks;

		public bool ArticleAn { get { return m_ArticleAn; } }

		public int PrimaryHue { get { return m_PrimaryHue; } }

        public int TitleHue { get { return m_TitleHue; } }

		public TextDefinition Title { get { return m_Title; } }
		public TextDefinition NPCAdjunct { get { return m_NPCAdjunct; } }

		public TextDefinition JoinPhrase { get { return m_JoinPhrase; } }

		public Power[] Powers { get { return m_Powers; } }

		public RankDefinition[] Ranks{ get{ return m_Ranks; } }

		public EthicDefinition( int primaryHue, int titleHue, TextDefinition title, bool articlean, TextDefinition npcadjunct, TextDefinition joinPhrase, Power[] powers, RankDefinition[] ranks )
		{
			m_PrimaryHue = primaryHue;
		    m_TitleHue = titleHue;

			m_Title = title;
			m_NPCAdjunct = npcadjunct;

			m_JoinPhrase = joinPhrase;

			m_Powers = powers;

			m_Ranks = ranks;

			m_ArticleAn = articlean;
		}
	}
}