using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Items;
using System.IO;
using Server.Engines.XmlSpawner2;

namespace Server.Games
{
	public class CompanionListGump : Gump
	{
		public static void Initialize()
		{
            CommandSystem.Register("Young", AccessLevel.Player, new CommandEventHandler(YoungPlayersList_OnCommand));
		}

        [Usage("Young")]
        [Description("Lists all current YoungPlayers.")]
        private static void YoungPlayersList_OnCommand(CommandEventArgs e)
		{
            if (e.Mobile.Criminal)
            {
                e.Mobile.SendMessage("You can't use this command while you are a criminal!");
                return;
            }
            
            foreach (AggressorInfo aggressor in e.Mobile.Aggressors)
            {
                if (aggressor.Attacker is PlayerMobile)
                {
                    e.Mobile.SendMessage("You can't use that command while you are in combat!");
                    return;
                }
            }
            foreach (AggressorInfo aggressed in e.Mobile.Aggressed)
            {
                if (aggressed.Defender is PlayerMobile)
                {
                    e.Mobile.SendMessage("You can't use that command while you are in combat!");
                    return;
                }
            }
            //if (e.Mobile.Aggressors.Count > 0 || e.Mobile.Aggressed.Count > 0)
            
            if (e.Mobile is PlayerMobile && ((PlayerMobile)e.Mobile).Companion)
            {
                try
                {
                    if (!PlayerMobile.OnlineCompanions.Contains((PlayerMobile)e.Mobile))
                    {
                        PlayerMobile.OnlineCompanions.Add((PlayerMobile)e.Mobile);
                        UberScriptFunctions.Methods.LOCALMSG(null, e.Mobile, "You have signed on as a companion and will now receive young player notifications (log-out will sign you off).", 38);
                        LoggingCustom.Log(Path.Combine(new string[] { CompanionListGump.LogFileLocation, e.Mobile.RawName + ".txt" }), DateTime.Now + "\t" + "Signed on as Companion");
                    }
                }
                catch { }

                e.Mobile.SendGump(new CompanionListGump(e.Mobile));
            }
            else
            {
                e.Mobile.SendMessage("You must be a companion to have access to that command!");
            }
		}

		public static bool OldStyle = PropsConfig.OldStyle;

		public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
		public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;

		public static readonly int TextHue = PropsConfig.TextHue;
		public static readonly int TextOffsetX = PropsConfig.TextOffsetX;

		public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
		public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
		public static readonly int  EntryGumpID = PropsConfig.EntryGumpID;
		public static readonly int   BackGumpID = PropsConfig.BackGumpID;
		public static readonly int    SetGumpID = PropsConfig.SetGumpID;

		public static readonly int SetWidth = PropsConfig.SetWidth;
		public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
		public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
		public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;

		public static readonly int PrevWidth = PropsConfig.PrevWidth;
		public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
		public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
		public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;

		public static readonly int NextWidth = PropsConfig.NextWidth;
		public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
		public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
		public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;

		public static readonly int OffsetSize = PropsConfig.OffsetSize;

		public static readonly int EntryHeight = PropsConfig.EntryHeight;
		public static readonly int BorderSize = PropsConfig.BorderSize;

		private static bool PrevLabel = false, NextLabel = false;

		private static readonly int PrevLabelOffsetX = PrevWidth + 1;
		private static readonly int PrevLabelOffsetY = 0;

		private static readonly int NextLabelOffsetX = -29;
		private static readonly int NextLabelOffsetY = 0;

		private static readonly int EntryWidth = 760;
		private static readonly int EntryCount = 15;

		private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
		private static readonly int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 1));

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
		private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

		private Mobile m_Owner;
		private List<Mobile> m_Mobiles;
		private int m_Page;

        private class InternalComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new InternalComparer();

            public InternalComparer()
            {
            }

            public int Compare(Mobile x, Mobile y)
            {
                if (x == null || y == null)
                    throw new ArgumentException();

                if (x.AccessLevel > y.AccessLevel)
                    return -1;
                else if (x.AccessLevel < y.AccessLevel)
                    return 1;
                else
                    return Insensitive.Compare(x.Name, y.Name);
            }
        }

		public CompanionListGump( Mobile owner ) : this( owner, BuildList( owner ), 0 )
		{
		}

        public CompanionListGump(Mobile owner, List<Mobile> list, int page)
            : base(GumpOffsetX, GumpOffsetY)
		{
            owner.CloseGump(typeof(CompanionListGump));

			m_Owner = owner;
			m_Mobiles = list;

			Initialize( page );
		}

		public static List<Mobile> BuildList( Mobile owner )
		{
            List<Mobile> list = YoungMobiles();
            list.Sort( InternalComparer.Instance );

			return list;
		}

        public static List<Mobile> YoungMobiles()
        {
            List<Mobile> list = new List<Mobile>();
            List<NetState> states = NetState.Instances;
            for (int i = 0; i < states.Count; ++i)
            {
                Mobile m = states[i].Mobile;
                if (m != null && m.IsYoung())
                {
                    list.Add(m);
                }
            }
            return list;
        }

		public void Initialize( int page )
		{
			m_Page = page;

			int count = m_Mobiles.Count - (page * EntryCount);

			if ( count < 0 )
				count = 0;
			else if ( count > EntryCount )
				count = EntryCount;

			int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

			AddPage( 0 );

			AddBackground( 0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID );
			AddImageTiled( BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID );

			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize;

			int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

			if ( !OldStyle )
				AddImageTiled( x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, EntryGumpID );

			AddLabel( x + TextOffsetX, y, TextHue, String.Format( "Page {0} of {1} ({2})", page+1, (m_Mobiles.Count + EntryCount - 1) / EntryCount, m_Mobiles.Count ) );
            AddButton(x + TextOffsetX + 120, y, 0xa94, 0xa95, 1000, GumpButtonType.Reply, 0);
            AddLabel(x + TextOffsetX + 140, y, 100, "Sign off");

			x += emptyWidth + OffsetSize;

			if ( OldStyle )
				AddImageTiled( x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID );
			else
				AddImageTiled( x, y, PrevWidth, EntryHeight, HeaderGumpID );

			if ( page > 0 )
			{
				AddButton( x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0 );

				if ( PrevLabel )
					AddLabel( x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous" );
			}

			x += PrevWidth + OffsetSize;

			if ( !OldStyle )
				AddImageTiled( x, y, NextWidth, EntryHeight, HeaderGumpID );

			if ( (page + 1) * EntryCount < m_Mobiles.Count )
			{
				AddButton( x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1 );

				if ( NextLabel )
					AddLabel( x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next" );
			}

			for ( int i = 0, index = page * EntryCount; i < EntryCount && index < m_Mobiles.Count; ++i, ++index )
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				Mobile m = m_Mobiles[index];

				AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
                if (m is PlayerMobile)
                {
                    PlayerMobile pm = ((PlayerMobile)m);
                    TimeSpan idleTime = DateTime.UtcNow - pm.LastMoveTime;
                    if (pm.Alive)
                    {
                        if (pm.LastHelped == DateTime.MinValue)
                        {
                            AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, GetHueFor(m), m.Deleted ? "(deleted)" : m.Name + " - Not been helped yet!" + " - Region: " + m.Region + " - Last Move: " + UberScriptFunctions.Methods.TIMESPANSTRING(null, idleTime));
                        }
                        else
                        {
                            AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, GetHueFor(m), m.Deleted ? "(deleted)" : m.Name + " - Last helped at: " + ((PlayerMobile)m).LastHelped + " - Region: " + m.Region + " - Last Move: " + UberScriptFunctions.Methods.TIMESPANSTRING(null, idleTime));
                        }
                    }
                    else
                    {
                        AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, GetHueFor(m), m.Deleted ? "(deleted)" : m.Name + " - DEAD!" + " - Region: " + m.Region + " - Last Move: " + UberScriptFunctions.Methods.TIMESPANSTRING(null, idleTime));
                    }
                }
                else
                    AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, GetHueFor(m), m.Deleted ? "(deleted)" : m.Name);

				x += EntryWidth + OffsetSize;

				if ( SetGumpID != 0 )
					AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

				if ( m.NetState != null && !m.Deleted )
					AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, i + 3, GumpButtonType.Reply, 0 );
			}
		}

		private static int GetHueFor( Mobile m )
		{
			switch ( m.AccessLevel )
			{
				case AccessLevel.Owner:
				case AccessLevel.Developer:
				case AccessLevel.Administrator: return 0x516;
				case AccessLevel.Lead: return 0x151;
				case AccessLevel.Seer: return 0x144;
				case AccessLevel.EventMaster: return 0x48C;
				case AccessLevel.GameMaster: return 0x21;
				case AccessLevel.Counselor: return 0x2;
				case AccessLevel.Player: default:
				{
					if ( m.Kills >= Mobile.MurderCount )
						return 0x21;
					else if ( m.Criminal )
						return 0x3B1;

					return 0x58;
				}
			}
		}

        public static string LogFileLocation = "Logs" + Path.DirectorySeparatorChar + "Companions";

        public static List<BaseCreature> GetNearbyPets(Mobile master)
        {
            List<BaseCreature> pets = new List<BaseCreature>();
            foreach (Mobile m in master.GetMobilesInRange(3))
            {
                if (m is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)m;

                    if (pet.Controlled && (pet.ControlMaster == master || pet.IsPetFriend(m)))
                    {
                        pets.Add(pet);
                    }
                }
            }
            return pets;
        }

        public List<BaseCreature> pets = new List<BaseCreature>();

        public void SendCompanionTo(Mobile companion, Mobile youngPlayer)
        {
            if (youngPlayer.Map == Map.Internal) return;
            if (youngPlayer.Alive)
            {
                companion.Frozen = true;
            }
            else // dead young player
            {
                youngPlayer.LocalOverheadMessage(MessageType.Regular, 0x38, false, "Since you are a young player and died, a companion has volunteered to assist you!");
            }

            pets = GetNearbyPets(companion);

            companion.Hidden = true;
            companion.MoveToWorld(youngPlayer.Location, youngPlayer.Map);

            foreach (BaseCreature pet in pets)
            {
                if (pet.ControlOrder == OrderType.Guard || pet.ControlOrder == OrderType.Follow || pet.ControlOrder == OrderType.Come)
                {
                    pet.Hidden = true;
                    pet.Frozen = true;
                    pet.MoveToWorld(youngPlayer.Location, youngPlayer.Map);
                }
            }

            HideEffects(companion);
            if (youngPlayer is PlayerMobile)
            {
                ((PlayerMobile)youngPlayer).LastHelped = DateTime.UtcNow;
            }
            LoggingCustom.Log(Path.Combine(new string[] { LogFileLocation, companion.Name + ".txt" }), DateTime.Now + "\t" + companion.Name + "\thelping\t" + youngPlayer + "\t" + youngPlayer.Account + "\t" + youngPlayer.Location + "\t" + youngPlayer.Region);
        }

        public void HideEffects(Mobile from)
        {
            Entity entity = new Entity(from.Serial, from.Location, from.Map);

            Effects.SendLocationParticles(entity, 0x1AF3, 8, 26, 0, 0, 0, 0);
            Effects.PlaySound(entity.Location, entity.Map, 496);
            Timer.DelayCall<AnimInfo>(TimeSpan.FromSeconds(1.25), new TimerStateCallback<AnimInfo>(Anim_Continue), new AnimInfo(from, entity));
        }

        private class AnimInfo
        {
            public Mobile From;
            public IEntity Entity;

            public AnimInfo(Mobile from, IEntity entity)
            {
                From = from;
                Entity = entity;
            }
        }

        private void Anim_Continue(AnimInfo info)
        {
            Mobile from = info.From;
            IEntity entity = info.Entity;

            Moongate gate = new Moongate(false);
            gate.Hue = 0;
            gate.MoveToWorld(entity.Location, entity.Map);
            gate.TargetMap = Map.Internal;

            Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(1.0), new TimerStateCallback<Mobile>(ChangeHide), from);
            Timer.DelayCall<Moongate>(TimeSpan.FromSeconds(3.0), new TimerStateCallback<Moongate>(KillGate), gate);
        }

        private void ChangeHide(Mobile from)
        {
            foreach (BaseCreature pet in pets)
            {
                pet.Hidden = false;
                pet.Frozen = false;
            }
            from.Hidden = false;
            from.Frozen = false;
            from.PlaySound(0x1FE);
        }

        private void KillGate(Moongate gate)
        {
            if (gate != null && !gate.Deleted)
            {
                Effects.SendLocationParticles(EffectItem.Create(gate.Location, gate.Map, EffectItem.DefaultDuration), 0x376A, 9, 20, 5042);
                Effects.PlaySound(gate.Location, gate.Map, 0x201);
                gate.Delete();
            }
        }

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
            
            if (from.Criminal)
            {
                from.SendMessage("You can't use this command while you are a criminal!");
                return;
            }

            foreach (AggressorInfo aggressor in from.Aggressors)
            {
                if (aggressor.Attacker is PlayerMobile)
                {
                    from.SendMessage("You can't use that command while you are in combat!");
                    return;
                }
            }
            foreach (AggressorInfo aggressed in from.Aggressed)
            {
                if (aggressed.Defender is PlayerMobile)
                {
                    from.SendMessage("You can't use that command while you are in combat!");
                    return;
                }
            }
            //if (from.Aggressors.Count > 0 || from.Aggressed.Count > 0)
            //{
            //    from.SendMessage("You can't use that command while you are in combat!");
            //    return;
            //}

			switch ( info.ButtonID )
			{
				case 0: // Closed
				{
					return;
				}
				case 1: // Previous
				{
					if ( m_Page > 0 )
                        from.SendGump(new CompanionListGump(from, m_Mobiles, m_Page - 1));

					break;
				}
				case 2: // Next
				{
					if ( (m_Page + 1) * EntryCount < m_Mobiles.Count )
                        from.SendGump(new CompanionListGump(from, m_Mobiles, m_Page + 1));

					break;
				}
                case 1000: // sign out
                {
                    if (from is PlayerMobile && ((PlayerMobile)from).Companion)
                    {
                        PlayerMobile pm = (PlayerMobile)from;
                        if (PlayerMobile.OnlineCompanions.Contains(pm))
                        {
                            PlayerMobile.OnlineCompanions.Remove(pm);
                            UberScriptFunctions.Methods.LOCALMSG(null, from, "You have signed off as a companion. You will no longer receive young player alerts.", 38);
                            LoggingCustom.Log(Path.Combine(new string[] { CompanionListGump.LogFileLocation, from.RawName + ".txt" }), DateTime.Now + "\t" + "Signed off as Companion");
                            PlayerMobile.OnlineCompanions.Remove(pm);
                        }
                    }
                    break;
                }
				default:
				{
					int index = (m_Page * EntryCount) + (info.ButtonID - 3);

					if ( index >= 0 && index < m_Mobiles.Count )
					{
						Mobile m = m_Mobiles[index];

						if ( m.Deleted )
						{
							from.SendMessage( "That player has deleted their character." );
                            from.SendGump(new CompanionListGump(from, m_Mobiles, m_Page));
						}
						else if ( m.NetState == null )
						{
							from.SendMessage( "That player is no longer online." );
                            from.SendGump(new CompanionListGump(from, m_Mobiles, m_Page));
						}
						else if ( m == from || !m.Hidden || from.AccessLevel >= m.AccessLevel || (m is PlayerMobile && ((PlayerMobile)m).VisibilityList.Contains( from ))) 
						{
                            if (m is PlayerMobile)
                            {
                                PlayerMobile pm = (PlayerMobile)m;
                                if (pm.Alive && DateTime.UtcNow < pm.LastHelped + TimeSpan.FromMinutes(PseudoSeerStone.CompanionMinutesBetweenHelp))
                                {
                                    from.SendMessage("That player has been helped within the last minute, so you cannot jump to them at this time!");
                                }
                                else
                                {
                                    if (CreaturePossession.IsInHouseOrBoat(pm.Location, pm.Map))
                                        from.SendMessage("That player is in a house or boat, so you cannot teleport to them.");
                                    else
                                        SendCompanionTo(from, m);
                                }
                            }
                            else
                                SendCompanionTo(from, m);
						}
						else
						{
							from.SendMessage( "You cannot see them." );
                            from.SendGump(new CompanionListGump(from, m_Mobiles, m_Page));
						}
					}

					break;
				}
			}
		}
	}
}