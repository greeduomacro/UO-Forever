using System;
using System.Collections;
using System.Collections.Generic;

using Server;
using Server.Engines;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;

namespace Server.Multis
{

    public enum SecuritySettingsGumpPage
    {
		Default,
		Public,
		Party,
		Guild,
		AccessListDefault,
		AccessListPlayer,
		AccessListPage
    }

    public class SecuritySettingsGump : Gump
    {	
	
        private BaseShip m_Ship;
		private SecuritySettingsGumpPage m_Page;
		private Dictionary<int, PlayerMobile> m_PlayersAboard;

        private const int LabelColor = 0x7FFF;
        private const int SelectedColor = 0x421F;
        private const int DisabledColor = 0x4210;
        private const int WarningColor = 0x7E10;

        private const int LabelHue = 0x481;
        private const int HighlightedLabelHue = 0x64;
		
		private int m_CurrentAccessListPage;
		
		private PlayerMobile m_SelectedPlayer;

        private string GetOwnerName()
        {
            Mobile m = this.m_Ship.Owner;

            if (m == null || m.Deleted)
                return "(unowned)";

            string name;

            if ((name = m.Name) == null || (name = name.Trim()).Length <= 0)
                name = "(no name)";

            return name;
        }

        public int CurrentAccessListPage{ get{ return m_CurrentAccessListPage; } }
		
		public Dictionary<int, PlayerMobile> PlayersAboard{ get{ return m_PlayersAboard; } }
		
        public int GetButtonID(int type, int index)
        {
            return 1 + (index * 15) + type;
        }	

        public void AddButtonLabeled(int x, int y, int buttonID, int number)
        {
            this.AddButtonLabeled(x, y, buttonID, number, true);
        }

        public void AddButtonLabeled(int x, int y, int buttonID, int number, bool enabled)
        {
            if (enabled)
                this.AddButton(x, y, 4005, 4007, buttonID, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(x + 35, y, 240, 20, number, enabled ? LabelColor : DisabledColor, false, false);
        }		

		public SecuritySettingsGump(SecuritySettingsGumpPage page, Mobile from, BaseShip ship, Dictionary<int, PlayerMobile> playersAboard, int currentAccessListPage, PlayerMobile selectedPlayer) 
			: base(50, 40)		
		{
            m_Ship = ship;
			m_Page = page;
			m_PlayersAboard = playersAboard;
			m_CurrentAccessListPage = currentAccessListPage;
			m_SelectedPlayer = selectedPlayer;

            from.CloseGump(typeof(SecuritySettingsGump));

			bool isOwner = false;
			if (from == ship.Owner)
				isOwner = true;
			
			if (!isOwner)
			{
				if (m_Ship.CanModifySecurity == 0)
					return;
				else if (m_Ship.CanModifySecurity == 1)
					if (from.Party != m_Ship.Owner.Party)
						return;
					else if (((Party)(m_Ship.Owner.Party)).Leader != m_Ship.Owner)
							return;
				else if (m_Ship.CanModifySecurity == 2)
					if (from.Party != m_Ship.Owner.Party)
						return;
			}
			
			this.AddPage(0);	
				
			this.AddImageTiled(0, 0, 20, 18, 0xA3C);
			this.AddImageTiled(20, 0, 270, 18, 0xA3D);
			this.AddImageTiled(286, 0, 20, 18, 0xA3E);
			this.AddImageTiled(0, 18, 22, 210, 0xA3F);
			this.AddImageTiled(20, 18, 266, 206, 0xA40);
			this.AddImageTiled(286, 18, 20, 210, 0xA41);
			this.AddImageTiled(0, 224, 22, 210, 0xA3F);
			this.AddImageTiled(20, 224, 266, 206, 0xA40);
			this.AddImageTiled(286, 224, 20, 210, 0xA41);
			this.AddImageTiled(0, 430, 20, 18, 0xA42);
			this.AddImageTiled(20, 430, 270, 18, 0xA43);
			this.AddImageTiled(286, 430, 20, 18, 0xA44);
			
			this.AddLabel(85, 20, 53, "Passenger and Crew Manifest");
			this.AddLabel(10, 50, LabelHue, String.Format("Ship: {0}", (m_Ship.ShipName != null)?m_Ship.ShipName:"unnamed ship"));
			this.AddLabel(10, 70, LabelHue, String.Format("Owner: {0}", m_Ship.Owner.Name));

			if ((page == SecuritySettingsGumpPage.Default) || (page == SecuritySettingsGumpPage.Public) || (page == SecuritySettingsGumpPage.Party) || (page == SecuritySettingsGumpPage.Guild))
			{
			
			
				this.AddLabel(10, 100, LabelHue, String.Format("Party membership modifies access to this ship: "));

				
				switch (m_Ship.CanModifySecurity)
				{
					case (0):
						{
							this.AddButton(60, 120, 0xFA6, 0xFA6, this.GetButtonID(0, 0), GumpButtonType.Reply, 0); 
							this.AddHtmlLocalized(95, 120, 240, 20, 1149778, LabelColor, false, false);// Never
							this.AddButton(60, 140, 0xFA5, 0xFA6, this.GetButtonID(0, 1), GumpButtonType.Reply, 0); 
							this.AddHtmlLocalized(95, 140, 240, 20, 1149744, LabelColor, false, false);// When I am a Party Leader
							this.AddButton(60, 160, 0xFA5, 0xFA6, this.GetButtonID(0, 2), GumpButtonType.Reply, 0); 
							this.AddHtmlLocalized(95, 160, 240, 20, 1149745, LabelColor, false, false);// When I am a Party Member
								
							break;					
						}
						
					case (1):
						{
							this.AddButton(60, 120, 0xFA5, 0xFA6, this.GetButtonID(0, 0), GumpButtonType.Reply, 0); 
							this.AddHtmlLocalized(95, 120, 240, 20, 1149778, LabelColor, false, false);// Never
							this.AddButton(60, 140, 0xFA6, 0xFA6, this.GetButtonID(0, 1), GumpButtonType.Reply, 0); 
							this.AddHtmlLocalized(95, 140, 240, 20, 1149744, LabelColor, false, false);// When I am a Party Leader
							this.AddButton(60, 160, 0xFA5, 0xFA6, this.GetButtonID(0, 2), GumpButtonType.Reply, 0); 
							this.AddHtmlLocalized(95, 160, 240, 20, 1149745, LabelColor, false, false);// When I am a Party Member
								
							break;
						}
						
					case (2):
						{
							this.AddButton(60, 120, 0xFA5, 0xFA6, this.GetButtonID(0, 0), GumpButtonType.Reply, 0); 
							this.AddHtmlLocalized(95, 120, 240, 20, 1149778, LabelColor, false, false);// Never
							this.AddButton(60, 140, 0xFA5, 0xFA6, this.GetButtonID(0, 1), GumpButtonType.Reply, 0); 
							this.AddHtmlLocalized(95, 140, 240, 20, 1149744, LabelColor, false, false);// When I am a Party Leader
							this.AddButton(60, 160, 0xFA6, 0xFA6, this.GetButtonID(0, 2), GumpButtonType.Reply, 0); 
							this.AddHtmlLocalized(95, 160, 240, 20, 1149745, LabelColor, false, false);// When I am a Party Member

							break;
						}
				}	

				this.AddLabel(10, 180, LabelHue, String.Format("Public Access: "));			
				
				if (page == SecuritySettingsGumpPage.Public)
					this.AddButton(120, 180, 0xFA6, 0xFA6, this.GetButtonID(1, 0), GumpButtonType.Reply, 0);
				else
					this.AddButton(120, 180, 0xFA5, 0xFA6, this.GetButtonID(1, 0), GumpButtonType.Reply, 0);
				
				switch (m_Ship.Public)
				{
					case (0):
						{
							this.AddLabel(155, 180, 906, String.Format("N/A"));
								
							break;					
						}
						
					case (1):
						{
							this.AddLabel(155, 180, 98, String.Format("PASSENGER"));
								
							break;
						}
						
					case (2):
						{
							this.AddLabel(155, 180, 68, String.Format("CREW"));

							break;
						}
						
					case (3):
						{
							this.AddLabel(155, 180, 53, String.Format("OFFICER"));

							break;
						}
		
					case (4):
						{
							this.AddLabel(155, 180, 906, String.Format("CAPTAIN"));

							break;
						}
						
					case (5):
						{
							this.AddLabel(155, 180, 38, String.Format("DENY ACCESS"));

							break;
						}
				}
				
				this.AddLabel(10, 200, LabelHue, String.Format("Party Access: "));		

				if (page == SecuritySettingsGumpPage.Party)
					this.AddButton(120, 200, 0xFA6, 0xFA6, this.GetButtonID(1, 1), GumpButtonType.Reply, 0);
				else
					this.AddButton(120, 200, 0xFA5, 0xFA6, this.GetButtonID(1, 1), GumpButtonType.Reply, 0);			
				
				switch (m_Ship.Party)
				{
					case (0):
						{
							this.AddLabel(155, 200, 906, String.Format("N/A"));
								
							break;					
						}
						
					case (1):
						{
							this.AddLabel(155, 200, 98, String.Format("PASSENGER"));
								
							break;
						}
						
					case (2):
						{
							this.AddLabel(155, 200, 68, String.Format("CREW"));

							break;
						}
						
					case (3):
						{
							this.AddLabel(155, 200, 53, String.Format("OFFICER"));

							break;
						}
		
					case (4):
						{
							this.AddLabel(155, 200, 906, String.Format("CAPTAIN"));

							break;
						}
						
					case (5):
						{
							this.AddLabel(155, 200, 38, String.Format("DENY ACCESS"));

							break;
						}
				}
							
				this.AddLabel(10, 220, LabelHue, String.Format("Guild Access: "));

				if (page == SecuritySettingsGumpPage.Guild)
					this.AddButton(120, 220, 0xFA6, 0xFA6, this.GetButtonID(1, 2), GumpButtonType.Reply, 0); 
				else
					this.AddButton(120, 220, 0xFA5, 0xFA6, this.GetButtonID(1, 2), GumpButtonType.Reply, 0); 
					
				switch (m_Ship.Guild)
				{
					case (0):
						{
							this.AddLabel(155, 220, 906, String.Format("N/A"));
								
							break;					
						}
						
					case (1):
						{
							this.AddLabel(155, 220, 98, String.Format("PASSENGER"));
								
							break;
						}
						
					case (2):
						{
							this.AddLabel(155, 220, 68, String.Format("CREW"));

							break;
						}
						
					case (3):
						{
							this.AddLabel(155, 220, 53, String.Format("OFFICER"));

							break;
						}
		
					case (4):
						{
							this.AddLabel(155, 220, 906, String.Format("CAPTAIN"));

							break;
						}
						
					case (5):
						{
							this.AddLabel(155, 220, 38, String.Format("DENY ACCESS"));

							break;
						}
				}			
				
				switch ( page )
				{	
					case SecuritySettingsGumpPage.Public:
						{	
							switch (m_Ship.Public)
							{
								case (0):
									{
										this.AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										this.AddButton(20, 280, 0xFA6, 0xFA6, this.GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 280, LabelHue, "N/A");
										this.AddButton(20, 300, 0xFA5, 0xFA6, this.GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 300, LabelHue, "PASSENGER");
										this.AddButton(20, 320, 0xFA5, 0xFA6, this.GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 320, LabelHue, "CREW");
										this.AddButton(20, 340, 0xFA5, 0xFA6, this.GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 340, LabelHue, "OFFICER");
										this.AddButton(20, 360, 0xFA5, 0xFA6, this.GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 360, LabelHue, "DENY ACCESS");
											
										break;					
									}
									
								case (1):
									{
										this.AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										this.AddButton(20, 280, 0xFA5, 0xFA6, this.GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 280, LabelHue, "N/A");
										this.AddButton(20, 300, 0xFA6, 0xFA6, this.GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 300, 98, "PASSENGER");
										this.AddButton(20, 320, 0xFA5, 0xFA6, this.GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 320, LabelHue, "CREW");
										this.AddButton(20, 340, 0xFA5, 0xFA6, this.GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 340, LabelHue, "OFFICER");
										this.AddButton(20, 360, 0xFA5, 0xFA6, this.GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 360, LabelHue, "DENY ACCESS");
											
										break;
									}
									
								case (2):
									{
										this.AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										this.AddButton(20, 280, 0xFA5, 0xFA6, this.GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 280, LabelHue, "N/A");
										this.AddButton(20, 300, 0xFA5, 0xFA6, this.GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 300, LabelHue, "PASSENGER");
										this.AddButton(20, 320, 0xFA6, 0xFA6, this.GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 320, 68, "CREW");
										this.AddButton(20, 340, 0xFA5, 0xFA6, this.GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 340, LabelHue, "OFFICER");
										this.AddButton(20, 360, 0xFA5, 0xFA6, this.GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 360, LabelHue, "DENY ACCESS");

										break;
									}
									
								case (3):
									{
										this.AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										this.AddButton(20, 280, 0xFA5, 0xFA6, this.GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 280, LabelHue, "N/A");
										this.AddButton(20, 300, 0xFA5, 0xFA6, this.GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 300, LabelHue, "PASSENGER");
										this.AddButton(20, 320, 0xFA5, 0xFA6, this.GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 320, LabelHue, "CREW");
										this.AddButton(20, 340, 0xFA6, 0xFA6, this.GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 340, 53, "OFFICER");
										this.AddButton(20, 360, 0xFA5, 0xFA6, this.GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 360, LabelHue, "DENY ACCESS");

										break;
									}
					
								case (4):
									{
										this.AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										this.AddButton(20, 280, 0xFA5, 0xFA6, this.GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 280, LabelHue, "N/A");
										this.AddButton(20, 300, 0xFA5, 0xFA6, this.GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 300, LabelHue, "PASSENGER");
										this.AddButton(20, 320, 0xFA5, 0xFA6, this.GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 320, LabelHue, "CREW");
										this.AddButton(20, 340, 0xFA5, 0xFA6, this.GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 340, LabelHue, "OFFICER");
										this.AddButton(20, 360, 0xFA5, 0xFA6, this.GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 360, LabelHue, "DENY ACCESS");

										break;
									}
									
								case (5):
									{
										this.AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										this.AddButton(20, 280, 0xFA5, 0xFA6, this.GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 280, LabelHue, "N/A");
										this.AddButton(20, 300, 0xFA5, 0xFA6, this.GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 300, LabelHue, "PASSENGER");
										this.AddButton(20, 320, 0xFA5, 0xFA6, this.GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 320, LabelHue, "CREW");
										this.AddButton(20, 340, 0xFA5, 0xFA6, this.GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 340, LabelHue, "OFFICER");
										this.AddButton(20, 360, 0xFA6, 0xFA6, this.GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(55, 360, 38, "DENY ACCESS");

										break;
									}
							}																																
							break;
						}									
						
					case SecuritySettingsGumpPage.Party:
						{

							switch (m_Ship.Party)
							{
								case (0):
									{
										this.AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										this.AddButton(80, 280, 0xFA6, 0xFA6, this.GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA5, 0xFA6, this.GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, LabelHue, "PASSENGER");
										this.AddButton(80, 320, 0xFA5, 0xFA6, this.GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, LabelHue, "CREW");
										this.AddButton(80, 340, 0xFA5, 0xFA6, this.GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, LabelHue, "OFFICER");
										this.AddButton(80, 360, 0xFA5, 0xFA6, this.GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, LabelHue, "DENY ACCESS");

											
										break;					
									}
									
								case (1):
									{
										this.AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										this.AddButton(80, 280, 0xFA5, 0xFA6, this.GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA6, 0xFA6, this.GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, 98, "PASSENGER");
										this.AddButton(80, 320, 0xFA5, 0xFA6, this.GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, LabelHue, "CREW");
										this.AddButton(80, 340, 0xFA5, 0xFA6, this.GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, LabelHue, "OFFICER");
										this.AddButton(80, 360, 0xFA5, 0xFA6, this.GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, LabelHue, "DENY ACCESS");

											
										break;
									}
									
								case (2):
									{
										this.AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										this.AddButton(80, 280, 0xFA5, 0xFA6, this.GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA5, 0xFA6, this.GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, LabelHue, "PASSENGER");
										this.AddButton(80, 320, 0xFA6, 0xFA6, this.GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, 68, "CREW");
										this.AddButton(80, 340, 0xFA5, 0xFA6, this.GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, LabelHue, "OFFICER");
										this.AddButton(80, 360, 0xFA5, 0xFA6, this.GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, LabelHue, "DENY ACCESS");


										break;
									}
									
								case (3):
									{
										this.AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										this.AddButton(80, 280, 0xFA5, 0xFA6, this.GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA5, 0xFA6, this.GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, LabelHue, "PASSENGER");
										this.AddButton(80, 320, 0xFA5, 0xFA6, this.GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, LabelHue, "CREW");
										this.AddButton(80, 340, 0xFA6, 0xFA6, this.GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, 53, "OFFICER");
										this.AddButton(80, 360, 0xFA5, 0xFA6, this.GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, LabelHue, "DENY ACCESS");


										break;
									}
					
								case (4):
									{
										this.AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										this.AddButton(80, 280, 0xFA5, 0xFA6, this.GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA5, 0xFA6, this.GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, LabelHue, "PASSENGER");
										this.AddButton(80, 320, 0xFA5, 0xFA6, this.GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, LabelHue, "CREW");
										this.AddButton(80, 340, 0xFA5, 0xFA6, this.GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, LabelHue, "OFFICER");
										this.AddButton(80, 360, 0xFA5, 0xFA6, this.GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, LabelHue, "DENY ACCESS");


										break;
									}
									
								case (5):
									{
										this.AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										this.AddButton(80, 280, 0xFA5, 0xFA6, this.GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA5, 0xFA6, this.GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, LabelHue, "PASSENGER");
										this.AddButton(80, 320, 0xFA5, 0xFA6, this.GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, LabelHue, "CREW");
										this.AddButton(80, 340, 0xFA5, 0xFA6, this.GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, LabelHue, "OFFICER");
										this.AddButton(80, 360, 0xFA6, 0xFA6, this.GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, 38, "DENY ACCESS");

										break;
									}
							}										
							break;
						}
						
					case SecuritySettingsGumpPage.Guild:
						{
							switch (m_Ship.Guild)
							{
								case (0):
									{
										this.AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										this.AddButton(140, 280, 0xFA6, 0xFA6, this.GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 280, LabelHue, "N/A");
										this.AddButton(140, 300, 0xFA5, 0xFA6, this.GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 300, LabelHue, "PASSENGER");
										this.AddButton(140, 320, 0xFA5, 0xFA6, this.GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 320, LabelHue, "CREW");
										this.AddButton(140, 340, 0xFA5, 0xFA6, this.GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 340, LabelHue, "OFFICER");
										this.AddButton(140, 360, 0xFA5, 0xFA6, this.GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 360, LabelHue, "DENY ACCESS");
											
										break;					
									}
									
								case (1):
									{
										this.AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										this.AddButton(140, 280, 0xFA5, 0xFA6, this.GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 280, LabelHue, "N/A");
										this.AddButton(140, 300, 0xFA6, 0xFA6, this.GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 300, 98, "PASSENGER");
										this.AddButton(140, 320, 0xFA5, 0xFA6, this.GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 320, LabelHue, "CREW");
										this.AddButton(140, 340, 0xFA5, 0xFA6, this.GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 340, LabelHue, "OFFICER");
										this.AddButton(140, 360, 0xFA5, 0xFA6, this.GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 360, LabelHue, "DENY ACCESS");

										break;
									}
									
								case (2):
									{
										this.AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										this.AddButton(140, 280, 0xFA5, 0xFA6, this.GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 280, LabelHue, "N/A");
										this.AddButton(140, 300, 0xFA5, 0xFA6, this.GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 300, LabelHue, "PASSENGER");
										this.AddButton(140, 320, 0xFA6, 0xFA6, this.GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 320, 68, "CREW");
										this.AddButton(140, 340, 0xFA5, 0xFA6, this.GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 340, LabelHue, "OFFICER");
										this.AddButton(140, 360, 0xFA5, 0xFA6, this.GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 360, LabelHue, "DENY ACCESS");

										break;
									}
									
								case (3):
									{
										this.AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										this.AddButton(140, 280, 0xFA5, 0xFA6, this.GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 280, LabelHue, "N/A");
										this.AddButton(140, 300, 0xFA5, 0xFA6, this.GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 300, LabelHue, "PASSENGER");
										this.AddButton(140, 320, 0xFA5, 0xFA6, this.GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 320, LabelHue, "CREW");
										this.AddButton(140, 340, 0xFA6, 0xFA6, this.GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 340, 53, "OFFICER");
										this.AddButton(140, 360, 0xFA5, 0xFA6, this.GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 360, LabelHue, "DENY ACCESS");

										break;
									}
					
								case (4):
									{
										this.AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										this.AddButton(140, 280, 0xFA5, 0xFA6, this.GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 280, LabelHue, "N/A");
										this.AddButton(140, 300, 0xFA5, 0xFA6, this.GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 300, LabelHue, "PASSENGER");
										this.AddButton(140, 320, 0xFA5, 0xFA6, this.GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 320, LabelHue, "CREW");
										this.AddButton(140, 340, 0xFA5, 0xFA6, this.GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 340, LabelHue, "OFFICER");
										this.AddButton(140, 360, 0xFA5, 0xFA6, this.GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 360, LabelHue, "DENY ACCESS");

										break;
									}
									
								case (5):
									{
										this.AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										this.AddButton(140, 280, 0xFA5, 0xFA6, this.GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 280, LabelHue, "N/A");
										this.AddButton(140, 300, 0xFA5, 0xFA6, this.GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 300, LabelHue, "PASSENGER");
										this.AddButton(140, 320, 0xFA5, 0xFA6, this.GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 320, LabelHue, "CREW");
										this.AddButton(140, 340, 0xFA5, 0xFA6, this.GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 340, LabelHue, "OFFICER");
										this.AddButton(140, 360, 0xFA6, 0xFA6, this.GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(175, 360, 38, "DENY ACCESS");

										break;
									}
							}		

						

							
							break;
						}					
				}
				
				this.AddButtonLabeled(160, 410, this.GetButtonID(6, 1), 1149734); // Access List
				
			}
			else
			{
				switch( page )
				{
					case (SecuritySettingsGumpPage.AccessListDefault):
						{
						
							if (playersAboard == null)
								break;
								
							int PlayersCounter = playersAboard.Count;				

							int currentPage = 1;
							int currentLine = 0;
							int currentButton = 0;
							
							this.AddPage(1);
							
							foreach(KeyValuePair<int, PlayerMobile> entry in playersAboard)
							{
																
								this.AddButton(10, 100 + currentLine * 20, 0xFA5, 0xFA6, this.GetButtonID(5, currentButton++), GumpButtonType.Reply, 0);
								this.AddLabel(45, 100 + currentLine * 20, LabelHue, (entry.Value).Name);
								
								byte listedPlayerAccess = 0;
								if (m_Ship.PlayerAccess != null)
									foreach(KeyValuePair<PlayerMobile, byte> entry2 in m_Ship.PlayerAccess)							
										if (entry.Value == entry2.Key)
											listedPlayerAccess = entry2.Value;								

								switch (listedPlayerAccess)
								{
									case (0):
										{
											this.AddLabel(120, 100 + currentLine * 20, LabelHue, "N/A");
												
											break;					
										}
										
									case (1):
										{
											this.AddLabel(120, 100 + currentLine * 20, 98, "PASSENGER");
												
											break;					
										}
									
									case (2):
										{
											this.AddLabel(120, 100 + currentLine * 20, 68, "CREW");
												
											break;					
										}
										
									case (3):
										{
											this.AddLabel(120, 100 + currentLine * 20, 53, "OFFICER");
												
											break;					
										}
										
									case (4):
										{
											this.AddLabel(120, 100 + currentLine * 20, 43, "CAPTAIN");
												
											break;					
										}
										
									case (5):
										{
											this.AddLabel(120, 100 + currentLine * 20, 38, "DENY ACCESS");
												
											break;					
										}
								}
								
								++currentLine;
								
								if (currentLine == 10)
								{
									currentLine = 0;
									currentButton = 0;
									this.AddPage(currentPage++);
									
									m_CurrentAccessListPage = currentPage;																		
								}
								
								if (currentPage > m_CurrentAccessListPage)
									break;
							}
							
							this.AddButton(10, 410, 0xFA5, 0xFA6, this.GetButtonID(6, 0), GumpButtonType.Reply, 0);
							this.AddLabel(45, 410, LabelHue, "MAIN MENU");	
							
							if (m_CurrentAccessListPage > 1)							
								this.AddButton(160, 410, 0xFAE, 0xFAF,this.GetButtonID(6, 2), GumpButtonType.Reply, 0);
							
							
							if (currentLine == 0) 
								this.AddButton(200, 410, 0xFA5, 0xFA6,this.GetButtonID(6, 3), GumpButtonType.Reply, 0);
								
							
						}	

						break;					
						
					case (SecuritySettingsGumpPage.AccessListPlayer):
						{
						
							if (selectedPlayer == null)
								break;

							this.AddLabel(10, 120, LabelHue, selectedPlayer.Name);
								
							byte selectedPlayerAccess = 0;
							if (m_Ship.PlayerAccess != null)
								foreach(KeyValuePair<PlayerMobile, byte> entry in m_Ship.PlayerAccess)							
									if (selectedPlayer == entry.Key)
										selectedPlayerAccess = entry.Value;								

							switch (selectedPlayerAccess)
							{
								case (0):
									{
										this.AddLabel(80, 120, LabelHue, "N/A");
										this.AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										this.AddButton(80, 280, 0xFA6, 0xFA6, this.GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA5, 0xFA6, this.GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, LabelHue, "PASSENGER");
										this.AddButton(80, 320, 0xFA5, 0xFA6, this.GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, LabelHue, "CREW");
										this.AddButton(80, 340, 0xFA5, 0xFA6, this.GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, LabelHue, "OFFICER");
										this.AddButton(80, 360, 0xFA5, 0xFA6, this.GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, LabelHue, "CAPTAIN");
										this.AddButton(80, 380, 0xFA5, 0xFA6, this.GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 380, LabelHue, "DENY ACCESS");

											
										break;					
									}
									
								case (1):
									{
										this.AddLabel(80, 120, 98, "PASSENGER");
										this.AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										this.AddButton(80, 280, 0xFA5, 0xFA6, this.GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA6, 0xFA6, this.GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, 98, "PASSENGER");
										this.AddButton(80, 320, 0xFA5, 0xFA6, this.GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, LabelHue, "CREW");
										this.AddButton(80, 340, 0xFA5, 0xFA6, this.GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, LabelHue, "OFFICER");
										this.AddButton(80, 360, 0xFA5, 0xFA6, this.GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, LabelHue, "CAPTAIN");
										this.AddButton(80, 380, 0xFA5, 0xFA6, this.GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 380, LabelHue, "DENY ACCESS");

											
										break;
									}
									
								case (2):
									{
										this.AddLabel(80, 120, 68, "CREW");
										this.AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										this.AddButton(80, 280, 0xFA5, 0xFA6, this.GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA5, 0xFA6, this.GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, LabelHue, "PASSENGER");
										this.AddButton(80, 320, 0xFA6, 0xFA6, this.GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, 68, "CREW");
										this.AddButton(80, 340, 0xFA5, 0xFA6, this.GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, LabelHue, "OFFICER");
										this.AddButton(80, 360, 0xFA5, 0xFA6, this.GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, LabelHue, "CAPTAIN");
										this.AddButton(80, 380, 0xFA5, 0xFA6, this.GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 380, LabelHue, "DENY ACCESS");


										break;
									}
									
								case (3):
									{
										this.AddLabel(80, 120, 53, "OFFICER");
										this.AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										this.AddButton(80, 280, 0xFA5, 0xFA6, this.GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA5, 0xFA6, this.GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, LabelHue, "PASSENGER");
										this.AddButton(80, 320, 0xFA5, 0xFA6, this.GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, LabelHue, "CREW");
										this.AddButton(80, 340, 0xFA6, 0xFA6, this.GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, 53, "OFFICER");
										this.AddButton(80, 360, 0xFA5, 0xFA6, this.GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, LabelHue, "CAPTAIN");
										this.AddButton(80, 380, 0xFA5, 0xFA6, this.GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 380, LabelHue, "DENY ACCESS");


										break;
									}
					
								case (4):
									{
										this.AddLabel(80, 120, 43, "CAPTAIN");
										this.AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										this.AddButton(80, 280, 0xFA5, 0xFA6, this.GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA5, 0xFA6, this.GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, LabelHue, "PASSENGER");
										this.AddButton(80, 320, 0xFA5, 0xFA6, this.GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, LabelHue, "CREW");
										this.AddButton(80, 340, 0xFA5, 0xFA6, this.GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, LabelHue, "OFFICER");
										this.AddButton(80, 360, 0xFA5, 0xFA6, this.GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, 43, "CAPTAIN");
										this.AddButton(80, 380, 0xFA5, 0xFA6, this.GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 380, LabelHue, "DENY ACCESS");


										break;
									}
									
								case (5):
									{
										this.AddLabel(80, 120, 38, "DENY ACCESS");
										this.AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										this.AddButton(80, 280, 0xFA5, 0xFA6, this.GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 280, LabelHue, "N/A");
										this.AddButton(80, 300, 0xFA5, 0xFA6, this.GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 300, LabelHue, "PASSENGER");
										this.AddButton(80, 320, 0xFA5, 0xFA6, this.GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 320, LabelHue, "CREW");
										this.AddButton(80, 340, 0xFA5, 0xFA6, this.GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 340, LabelHue, "OFFICER");
										this.AddButton(80, 360, 0xFA6, 0xFA6, this.GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 360, LabelHue, "CAPTAIN");
										this.AddButton(80, 380, 0xFA5, 0xFA6, this.GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										this.AddLabel(115, 380, 38, "DENY ACCESS");

										break;
									}
							}											
						}
						
						break;						
				}
			}														
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Ship.Deleted)
                return;

            Mobile from = sender.Mobile;

			bool isOwner = false;
			if (from == m_Ship.Owner)
				isOwner = true;

            if (!from.CheckAlive())
                return;

            int val = info.ButtonID - 1;

            if (val < 0)
                return;

            int type = val % 15;
            int index = val / 15;

            switch ( type )
            {
                case 0:
                    {
                        switch ( index )
                        {
                            case 0: // Never
                                {       
									m_Ship.CanModifySecurity = 0;
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));

                                    break;
                                }
                            case 1: // Leader
                                {
									m_Ship.CanModifySecurity = 1;
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));

                                    break;
                                }
                            case 2: // Member
                                {
									m_Ship.CanModifySecurity = 2;
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));
									
                                    break;
                                }
						}
                        
                        break;
                    }
					
                case 1:
                    {
                        switch ( index )
                        {
                            case 0: // Public
                                {      
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Public, from, this.m_Ship, PlayersAboard, 1, null));

                                    break;
                                }
                            case 1: // Party
                                {      
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Party, from, this.m_Ship, PlayersAboard, 1, null));

                                    break;
                                }
                            case 2: // Guild
                                {      
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Guild, from, this.m_Ship, PlayersAboard, 1, null));

                                    break;
                                }                                                       
						}
                        
                        break;
                    }	

				case 2:
					{
                        switch ( index )
                        {
							case 0: // N/A
								{
									m_Ship.Public = 0;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 1: // Passenger
								{
									m_Ship.Public = 1;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 2: // Crew
								{
									m_Ship.Public = 2;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 3: // Officer
								{
									m_Ship.Public = 3;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}

							case 4: // Deny Access
								{
									m_Ship.Public = 5;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}	
									
						}
						
						break;
						
					}
					
					case 3:
					{
                        switch ( index )
                        {
							case 0: // N/A
								{
									m_Ship.Party = 0;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 1: // Passenger
								{
									m_Ship.Party = 1;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 2: // Crew
								{
									m_Ship.Party = 2;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 3: // Officer
								{
									m_Ship.Party = 3;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}

							case 4: // Deny Access
								{
									m_Ship.Party = 5;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}									
						}
						
						break;
						
					}
					
					case 4:
					{
                        switch ( index )
                        {
							case 0: // N/A
								{
									m_Ship.Guild = 0;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 1: // Passenger
								{
									m_Ship.Guild = 1;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 2: // Crew
								{
									m_Ship.Guild = 2;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 3: // Officer
								{
									m_Ship.Guild = 3;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}

							case 4: // Deny Access
								{
									m_Ship.Guild = 5;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}									
						}
						
						break;						
					}
					
                case 5:
                    {   	
						int selectedPlayerKey = (m_CurrentAccessListPage -1) * 10 + index;
						PlayerMobile playerToFind = null;
						foreach(KeyValuePair<int, PlayerMobile> entry in m_PlayersAboard)						
							if (entry.Key == selectedPlayerKey)
								m_SelectedPlayer = entry.Value;
						
						
						from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListPlayer, from, this.m_Ship, PlayersAboard, 1, m_SelectedPlayer));					
                        
                        break;
                    }					
					
					
                case 6:
                    {
                        switch ( index )
                        {
                            case 0: // Main Menu
                                {       									
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, this.m_Ship, PlayersAboard, 1, null));

                                    break;
                                }
                            case 1: // Access List
                                {									
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, this.m_Ship, PlayersAboard, 1, null));

                                    break;
                                }
                            case 2: // Previous Page
                                {       									
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListPage, from, this.m_Ship, PlayersAboard, --m_CurrentAccessListPage, null));

                                    break;
                                }
                            case 3: // Next Page
                                {									
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListPage, from, this.m_Ship, PlayersAboard, ++m_CurrentAccessListPage, null));

                                    break;
                                }
						}
                        
                        break;
                    }	

					case 7:
					{
                        switch ( index )
                        {
							case 0: // N/A
								{
									if (m_SelectedPlayer == null)
										break;
										
									if (m_Ship.PlayerAccess != null)
										if (m_Ship.PlayerAccess.ContainsKey(m_SelectedPlayer))
											m_Ship.PlayerAccess[m_SelectedPlayer] = 0;
										else
											m_Ship.PlayerAccess.Add(m_SelectedPlayer, 0);
											
									if (m_Ship.PlayerAccess == null)
										m_Ship.PlayerAccess.Add(m_SelectedPlayer, 0);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 1: // Passenger
								{
									if (m_SelectedPlayer == null)
										break;
										
									if (m_Ship.PlayerAccess != null)
										if (m_Ship.PlayerAccess.ContainsKey(m_SelectedPlayer))
											m_Ship.PlayerAccess[m_SelectedPlayer] = 1;
										else
											m_Ship.PlayerAccess.Add(m_SelectedPlayer, 1);
											
									if (m_Ship.PlayerAccess == null)
										m_Ship.PlayerAccess.Add(m_SelectedPlayer, 1);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 2: // Crew
								{
									if (m_SelectedPlayer == null)
										break;
										
									if (m_Ship.PlayerAccess != null)
										if (m_Ship.PlayerAccess.ContainsKey(m_SelectedPlayer))
											m_Ship.PlayerAccess[m_SelectedPlayer] = 2;
										else
											m_Ship.PlayerAccess.Add(m_SelectedPlayer, 2);
											
									if (m_Ship.PlayerAccess == null)
										m_Ship.PlayerAccess.Add(m_SelectedPlayer, 2);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}
								
							case 3: // Officer
								{
									if (m_SelectedPlayer == null)
										break;
									
									if (m_Ship.PlayerAccess != null)
										if (m_Ship.PlayerAccess.ContainsKey(m_SelectedPlayer))
											m_Ship.PlayerAccess[m_SelectedPlayer] = 3;
										else
											m_Ship.PlayerAccess.Add(m_SelectedPlayer, 3);
											
									if (m_Ship.PlayerAccess == null)
										m_Ship.PlayerAccess.Add(m_SelectedPlayer, 3);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}

							case 4: // Captain
								{
									if (m_SelectedPlayer == null)
										break;
										
									if (m_Ship.PlayerAccess != null)
										if (m_Ship.PlayerAccess.ContainsKey(m_SelectedPlayer))
											m_Ship.PlayerAccess[m_SelectedPlayer] = 4;
										else
											m_Ship.PlayerAccess.Add(m_SelectedPlayer, 4);
											
									if (m_Ship.PlayerAccess == null)
										m_Ship.PlayerAccess.Add(m_SelectedPlayer, 4);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}	

							case 5: // DENY ACCESS
								{
									if (m_SelectedPlayer == null)
										break;
										
									if (m_Ship.PlayerAccess != null)
										if (m_Ship.PlayerAccess.ContainsKey(m_SelectedPlayer))
											m_Ship.PlayerAccess[m_SelectedPlayer] = 5;
										else
											m_Ship.PlayerAccess.Add(m_SelectedPlayer, 5);
											
									if (m_Ship.PlayerAccess == null)
										m_Ship.PlayerAccess.Add(m_SelectedPlayer, 5);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, this.m_Ship, PlayersAboard, 1, null));		

									break;
								}	
								
						}
						
						break;						
					}					
            }			      					
        }
    }
}