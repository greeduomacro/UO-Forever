using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Engines.CustomTitles;
using Server.Items;
using Server.Mobiles;
using Server.Ethics.Evil;
using Server.Ethics.Hero;

namespace Server.Ethics
{
	[PropertyObject]
	public class Player
	{
		public static Player Find( Mobile mob )
		{
			return Find( mob, false );
		}

		public static Player Find( Mobile mob, bool inherit )
		{
			PlayerMobile pm = mob as PlayerMobile;

			if ( pm == null )
			{
				if ( inherit && mob is BaseCreature )
				{
					BaseCreature bc = mob as BaseCreature;

					if ( bc != null && bc.Controlled )
						pm = bc.ControlMaster as PlayerMobile;
					else if ( bc != null && bc.Summoned )
						pm = bc.SummonMaster as PlayerMobile;
				}

				if ( pm == null )
					return null;
			}

			Player pl = pm.EthicPlayer;

			return pl;
		}

		public static readonly int MaxPower = 100;

		private Ethic m_Ethic;
		private Mobile m_Mobile;

		private int m_Power; //Lifeforce
		private int m_Sphere; //Sphere Points
		private int m_Rank; //Sphere Rank
		private int m_History; //Sphere Point History

		private Mobile m_Steed;
		private Mobile m_Familiar;

		public Ethic Ethic { get { return m_Ethic; } }
		public Mobile Mobile { get { return m_Mobile; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Lead )]
		public int Power { get { return m_Power; } set { m_Power = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Owner )]
		public int Sphere { get { return m_Sphere; } set { m_Sphere = value;  AdjustRank(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Rank { get { return m_Rank; } } //Rank is adjusted automatically by Sphere points.

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Owner )]
		public int History { get { return m_History; } set { m_History = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Lead )]
		public Mobile Steed { get { return m_Steed; } set { m_Steed = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Lead )]
		public Mobile Familiar { get { return m_Familiar; } set { m_Familiar = value; } }

		public Player( Ethic ethic, Mobile mobile )
		{
			m_Ethic = ethic;
			m_Mobile = mobile;

            m_Sphere = 5;
			m_Power = 10;
            Grant();
		}

		public void CheckAttach()
		{
		    Attach();
		}

		public void Attach()
		{
		    if (m_Mobile is PlayerMobile)
		    {
                (m_Mobile as PlayerMobile).EthicPlayer = this;
		    }

		    if (!m_Ethic.Players.Contains(this))
		    {
		        m_Ethic.Players.Add(this);
		    }
		}

	    public void Grant()
	    {
            if (m_Mobile is PlayerMobile)
            {
                RankDefinition[] defs = m_Ethic.Definition.Ranks;
                TitleProfile p = CustomTitles.EnsureProfile(m_Mobile as PlayerMobile);
                Title title = null;
                TitleHue hue = null;
                if (CustomTitles.TryGetTitle(defs[0].Title, out title) && !p.Contains((title)))
                {
                    p.Add(title);
                }
                if (CustomTitles.TryGetHue(m_Ethic.Definition.TitleHue, out hue) && !p.Contains(hue))
                {
                    p.Add(hue);
                }
                m_Mobile.SendMessage(54, "You have decided to follow the path of {0}!", m_Ethic.Definition.Title);
                m_Mobile.SendMessage(54, "You have been granted the title {0} and the title hue {1}.", defs[0].Title, m_Ethic.Definition.TitleHue);
                m_Mobile.SendMessage(54, "You have been granted an ethic spellbook.");
                if (m_Mobile.Backpack != null)
                {
                    Item spellbook;
                    if (m_Ethic == Ethic.Evil)
                    {
                        spellbook = new Evilspellbook();
                        m_Mobile.AddToBackpack(spellbook);
                    }
                    else if (m_Ethic == Ethic.Hero)
                    {
                        spellbook = new Herospellbook();
                        m_Mobile.AddToBackpack(spellbook);
                    }
                }
            }	        
	    }

		public void Detach()
		{
		    if (m_Mobile is PlayerMobile)
		    {
                RankDefinition[] defs = m_Ethic.Definition.Ranks;
		        (m_Mobile as PlayerMobile).EthicPlayer = null;
                TitleProfile p = CustomTitles.EnsureProfile(m_Mobile as PlayerMobile);
                TitleHue hue = null;
                if (CustomTitles.TryGetHue(m_Ethic.Definition.TitleHue, out hue) && p.Contains(hue))
                {
                    if (p.SelectedHue != null && p.SelectedHue.Hue == hue.Hue)
                    {
                        p.SelectedHue = null;
                    }
                    p.Remove(hue);
                }

		        foreach (var rank in defs)
		        {
		            Title title;
		            if (CustomTitles.TryGetTitle(rank.Title, out title) && p.Contains(title))
                    {
                        if (p.SelectedTitle != null && p.SelectedTitle.MaleTitle == title.MaleTitle)
                        {
                            p.SelectedTitle = null;
                        }
                        p.Remove(title);
                    }
		        }

		        m_Ethic.Players.Remove(this);

                m_Mobile.SendMessage(54, "You have departed from the path of {0}!", m_Ethic.Definition.Title);
                m_Mobile.SendMessage(54, "For your abandonment of this ethic, you have been stripped of all ethic titles and the title hue {0}.", m_Ethic.Definition.TitleHue);
		    }
		}

		public void AdjustRank()
		{
			int rank = m_Rank;

			RankDefinition[] defs = m_Ethic.Definition.Ranks;

			for ( int i = 0; i < defs.Length; i++ )
			{
				if ( i+1 == defs.Length || ( m_Sphere >= defs[i].Points && m_Sphere < defs[i+1].Points ) )
				{
					m_Rank = i;
					break;
				}
			}

			if ( m_Rank < rank )
			{
                m_Mobile.SendMessage(54, "You have lost a rank in your ethic.  You are now rank {0}.", m_Rank);
                m_Mobile.SendMessage(54, "For your failure, you are no longer considered a {0} in the {1} ethic.", defs[rank].Title, m_Ethic.Definition.Title);

                TitleProfile p = CustomTitles.EnsureProfile(m_Mobile as PlayerMobile);
                Title title = null;
                if (CustomTitles.TryGetTitle(defs[rank].Title, out title) && p.Contains(title))
                {
                    if (p.SelectedTitle != null && p.SelectedTitle.MaleTitle == title.MaleTitle)
                    {
                        p.SelectedTitle = null;
                    }
                    p.Remove(title);
                }
			}
			else if ( m_Rank > rank )
			{
				m_Mobile.SendMessage( 54, "You are now rank {0} in your ethic.", m_Rank);
                m_Mobile.SendMessage(54, "You have been granted the title {0} for your hard work and dedication towards following the path of {1}.", defs[m_Rank].Title, m_Ethic.Definition.Title);

                TitleProfile p = CustomTitles.EnsureProfile(m_Mobile as PlayerMobile);
			    Title title = null;
                if (CustomTitles.TryGetTitle(defs[m_Rank].Title, out title) && !p.Contains(title))
			    {
			        p.Add(title);
			    }
			}
		}

		public string Title()
		{
			//Theoretically the rank should NEVER go beyond the length of the array.
            if (m_Ethic is EvilEthic)
                return "[Evil]";
            else
                return "[Hero]";
		}

		public override string ToString()
		{
			return "...";
		}

		public Player( Ethic ethic, GenericReader reader )
		{
			m_Ethic = ethic;

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 2:
				{
					m_Sphere = reader.ReadInt(); //We want the rank adjusted from here.

					goto case 1;
				}
				case 1:
				{
					m_Mobile = reader.ReadMobile();

					m_Power = reader.ReadEncodedInt();

					if ( version < 2 )
					{
						/*m_History =*/ reader.ReadEncodedInt();
						m_History = 0;
					}

					m_Steed = reader.ReadMobile();
					m_Familiar = reader.ReadMobile();

					break;
				}
				case 0:
				{
					m_Mobile = reader.ReadMobile();

					m_Power = reader.ReadEncodedInt();

					if ( version < 2 )
					{
						/*m_History =*/ reader.ReadEncodedInt();
						m_History = 0;
					}

					m_Steed = reader.ReadMobile();
					m_Familiar = reader.ReadMobile();

					break;
				}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( 2 ); // version

			writer.Write( m_Sphere );

			writer.Write( m_Mobile );

			writer.WriteEncodedInt( m_Power );

			writer.Write( m_Steed );
			writer.Write( m_Familiar );

		}
	}
}