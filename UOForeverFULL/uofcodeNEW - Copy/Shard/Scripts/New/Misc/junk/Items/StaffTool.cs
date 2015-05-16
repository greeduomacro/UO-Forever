#region Header
///**********************************************
/// Created on: Friday June 4th, 2004 05:05(am)
/// Edited on:  Thursday May 26th, 2005 23:59(pm)
/// Renamed to: StaffTool (Too Avoid Possible
/// Conflicts with other peoples systems)
/// Created By: Greystar
/// Created reason: Because I'm Lazy
/// Edited Again On: June 27th 2010
/// Edited Again By: Lord Dio
/// Edited For: RunUO 2.0 Final Compatibility
///**********************************************
/// Original Concept Idea:
/// LevelBoots Copyright (c) 2003 morkil
/// Additonal Abilities taken from StaffOrb
/// By David O'Hara Date: 08-13-2004
/// Some things taken from the TitleStaffRobe
/// Submitted by Rylock and last updated on
/// 06-18-2004 at 10:07 PM
/// Now Includes Page Checker from Dreamrunner
/// Submitted on Thursday May 26th, 2005 16:05(pm)
/// Free for use, but do not remove this text.
///**********************************************
///
///* * * Please Leave the Above lines alone * * *
/// StaffTool
/// Free for use.
#endregion //Header
#region Description
/// Makes it virtually possible to change your access level to a player for easy testing of your shard.
/// Can Also Resurrect you if you die and are not in Staff Mode.  This item is made
/// blessed so that if you die it stays on you. the verbal commands are tied to the
/// owner field and if a player does get one of these the only thing they will be
/// able to do with this is res themselves, which would still unbalance the game.  So
/// just make sure that never happens.  With this stone a player cannot trigger this
/// if they snoop you, they have to own the stone.  The only people who can change
/// the ownership of the stone or the level stored in the stone is the Administrators.
/// A home location can be set/used thru the context menu and Verbal.
/// Will auto resurrect it's owner on death (Off by default, settable in Props).
/// Now includes a gump accessible from ContextMenu
#endregion //Description
using System;
using Server;
using Server.Gumps;
using Server.Engines.Help;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using Server.ContextMenus;
using Server.Accounting;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    public class StaffTool : Item
    {
        private AccessLevel m_StaffLevel;
        private Mobile m_Owner;
        public Point3D m_HomeLocation;
        public Map m_HomeMap;
        private string m_Title = null;
        private bool m_AutoRes = false;
        private int m_GumpChoice = -1;
        private bool m_deleting = false;
        private DateTime m_LastClicked;
        private bool m_HideEffects;

        [Constructable]
        public StaffTool()
            : base(0x0E73)
        {
            Weight = 0.0;
            Name = "Unassigned Staff Stone";
            LootType = LootType.Blessed;
            m_StaffLevel = AccessLevel.Player;
            deleting = false;
        }

        public StaffTool(Serial serial) : base(serial) { }

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && Insensitive.Equals(e.Speech, "i claim this staff stone") && m_Owner == null)
            {
                m_Owner = e.Mobile;
                if (m_Owner.AccessLevel == AccessLevel.Player)
                {
                    if (!HideEffects)
                        this.Say(String.Format("You are not authorized to have a Staff Stone!"));
                    else
                        e.Mobile.SendMessage("You are not authorized to have a Staff Stone!");
                    deleting = true;
                    DoDelete(this);
                    return;
                }
                else
                {
                    if (!HideEffects)
                        this.Say(String.Format("How May I serve you master!"));
                    else
                        e.Mobile.SendMessage("How May I serve you master!");
                    Name = m_Owner.Name + "'s Staff Stone";
                    this.StaffLevel = m_Owner.AccessLevel;
                    LootType = LootType.Blessed;
                    this.HomeLocation = m_Owner.Location;
                    this.HomeMap = m_Owner.Map;
                    if (m_Owner.Title != null)
                        this.Title = m_Owner.Title;
                }
                e.Handled = true;
            }

            if (!e.Handled && Insensitive.Equals(e.Speech, "release stone") && m_Owner == e.Mobile)
            {
                if (!HideEffects)
                    this.Say(String.Format("You released me!"));
                else
                    e.Mobile.SendMessage("You released me!");
                Movable = true;
                e.Handled = true;
            }

            if (!e.Handled && Insensitive.Equals(e.Speech, "reveal") && m_Owner == e.Mobile)
            {
                if (!HideEffects)
                    this.Say(String.Format("You are no longer hidden."));
                else
                    e.Mobile.SendMessage("You are no longer hidden.");
                e.Mobile.Hidden = false;
                if (!HideEffects)
                {
                    Effects.SendLocationEffect(new Point3D(e.Mobile.X + 1, e.Mobile.Y, e.Mobile.Z + 4), e.Mobile.Map, 0x3728, 13);
                    Effects.SendLocationEffect(new Point3D(e.Mobile.X + 1, e.Mobile.Y, e.Mobile.Z), e.Mobile.Map, 0x3728, 13);
                    Effects.SendLocationEffect(new Point3D(e.Mobile.X + 1, e.Mobile.Y, e.Mobile.Z - 4), e.Mobile.Map, 0x3728, 13);
                    Effects.SendLocationEffect(new Point3D(e.Mobile.X, e.Mobile.Y + 1, e.Mobile.Z + 4), e.Mobile.Map, 0x3728, 13);
                    Effects.SendLocationEffect(new Point3D(e.Mobile.X, e.Mobile.Y + 1, e.Mobile.Z), e.Mobile.Map, 0x3728, 13);
                    Effects.SendLocationEffect(new Point3D(e.Mobile.X, e.Mobile.Y + 1, e.Mobile.Z - 4), e.Mobile.Map, 0x3728, 13);

                    Effects.SendLocationEffect(new Point3D(e.Mobile.X + 1, e.Mobile.Y + 1, e.Mobile.Z + 11), e.Mobile.Map, 0x3728, 13);
                    Effects.SendLocationEffect(new Point3D(e.Mobile.X + 1, e.Mobile.Y + 1, e.Mobile.Z + 7), e.Mobile.Map, 0x3728, 13);
                    Effects.SendLocationEffect(new Point3D(e.Mobile.X + 1, e.Mobile.Y + 1, e.Mobile.Z + 3), e.Mobile.Map, 0x3728, 13);
                    Effects.SendLocationEffect(new Point3D(e.Mobile.X + 1, e.Mobile.Y + 1, e.Mobile.Z - 1), e.Mobile.Map, 0x3728, 13);

                    e.Mobile.PlaySound(0x228);
                }
                e.Handled = true;
            }

            if (!e.Handled && Insensitive.Equals(e.Speech, "secure stone") && m_Owner == e.Mobile)
            {
                if (!HideEffects)
                    this.Say(String.Format("You secured me!"));
                else
                    e.Mobile.SendMessage("You secured me!");
                Movable = false;
                e.Handled = true;
            }

            if (!e.Handled && Insensitive.Equals(e.Speech, "switch player") && m_Owner == e.Mobile)
            {
                if (!HideEffects)
                    this.Say(String.Format("You change your level to a common Player."));
                else
                    e.Mobile.SendMessage("You change your level to a common Player.");
                m_StaffLevel = e.Mobile.AccessLevel;
                e.Mobile.AccessLevel = AccessLevel.Player;
                if (!HideEffects)
                    e.Mobile.BoltEffect(0);
                e.Handled = true;
            }

            if (!e.Handled && Insensitive.Equals(e.Speech, "switch normal") && m_Owner == e.Mobile)
            {
                if (!HideEffects)
                    this.Say(String.Format("You change your level back."));
                else
                    e.Mobile.SendMessage("You change your level back.");
                e.Mobile.AccessLevel = m_StaffLevel;
                if (!HideEffects)
                    e.Mobile.BoltEffect(0);
                e.Handled = true;
            }

            // With the autoressurect this may not be needed anymore but I left it in anyway
            if (!e.Handled && Insensitive.Equals(e.Speech, "resurrect me") && m_Owner == e.Mobile)
            {
                e.Mobile.PlaySound(0x214);
                e.Mobile.FixedEffect(0x376A, 10, 16);

                e.Mobile.Resurrect();
                e.Mobile.Hidden = true;
                e.Handled = true;
            }

            if (!e.Handled && Insensitive.Equals(e.Speech, "res me") && m_Owner == e.Mobile)
            {
                e.Mobile.PlaySound(0x214);
                e.Mobile.FixedEffect(0x376A, 10, 16);

                e.Mobile.Resurrect();
                e.Mobile.Hidden = true;
                e.Handled = true;
            }

            if (!e.Handled && Insensitive.Equals(e.Speech, "set title") && m_Owner == e.Mobile)
            {
                this.Title = e.Mobile.Title;
                e.Mobile.SendMessage("Your current title has been saved on the stone.");
                e.Handled = true;
            }

            if (!e.Handled && Insensitive.Equals(e.Speech, "clear title") && m_Owner == e.Mobile)
            {
                this.Title = null;
                e.Mobile.SendMessage("Your current title has been cleared from the stone.");
                e.Handled = true;
            }

            if (!e.Handled && Insensitive.Equals(e.Speech, "title") && m_Owner == e.Mobile)
            {
                switch (this.StaffLevel)
                {
                    case AccessLevel.Owner:
                        {
                            if (e.Mobile.Title != this.Title)
                                e.Mobile.Title = this.Title;
                            else
                                e.Mobile.Title = "[Owner]";
                            break;
						}
					case AccessLevel.Developer:
						{
							if (e.Mobile.Title != this.Title)
								e.Mobile.Title = this.Title;
							else
								e.Mobile.Title = "[Developer]";
							break;
						}
					case AccessLevel.Administrator:
						{
							if (e.Mobile.Title != this.Title)
								e.Mobile.Title = this.Title;
							else
								e.Mobile.Title = "[Administrator]";
							break;
						}
					case AccessLevel.Lead:
						{
							if (e.Mobile.Title != this.Title)
								e.Mobile.Title = this.Title;
							else
								e.Mobile.Title = "[Lead]";
							break;
						}
					case AccessLevel.GameMaster:
						{
							if (e.Mobile.Title != this.Title)
								e.Mobile.Title = this.Title;
							else
								e.Mobile.Title = "[Game Master]";
							break;
						}
					case AccessLevel.EventMaster:
						{
							if (e.Mobile.Title != this.Title)
								e.Mobile.Title = this.Title;
							else
								e.Mobile.Title = "[Event Master]";
							break;
						}
                    case AccessLevel.Seer:
                        {
                            if (e.Mobile.Title != this.Title)
                                e.Mobile.Title = this.Title;
                            else
                                e.Mobile.Title = "[Seer]";
                            break;
						}
                    case AccessLevel.Counselor:
                        {
                            if (e.Mobile.Title != this.Title)
                                e.Mobile.Title = this.Title;
                            else
                                e.Mobile.Title = "[Counselor]";
                            break;
                        }
                    case AccessLevel.Player:
                        {
                            if (e.Mobile.Title != this.Title)
                                e.Mobile.Title = this.Title;
                            else
                                e.Mobile.Title = null;
                            break;
                        }
                    default:
                        {
                            if (e.Mobile.Title != this.Title)
                                e.Mobile.Title = this.Title;
                            else
                                e.Mobile.Title = null;
                            break;
                        }
                }
                e.Mobile.SendMessage("Your title has been adjusted.");
                if (!HideEffects)
                    e.Mobile.BoltEffect(0);
                e.Handled = true;
            }

            base.OnSpeech(e);
        }

        public void Say(int number)
        {
            PublicOverheadMessage(MessageType.Whisper, 0x3B2, number);
        }

        public void Say(string args)
        {
            PublicOverheadMessage(MessageType.Whisper, 0x3B2, false, args);
        }

        public override bool OnDragLift(Mobile from)
        {
            if (m_Owner != from && m_Owner != null)
            {
                from.SendMessage("You cannot pick that up.");
                if (!HideEffects)
                    from.BoltEffect(0);
                return false;
            }
            return base.OnDragLift(from);
        }

        public DateTime LastClicked { get { return m_LastClicked; } set { m_LastClicked = value; } }
        [CommandProperty(AccessLevel.Administrator)]
        public AccessLevel StaffLevel { get { return m_StaffLevel; } set { m_StaffLevel = value; } }
        public int GumpChoice { get { return m_GumpChoice; } set { m_GumpChoice = value; } }
        public bool deleting { get { return m_deleting; } set { m_deleting = value; } }
        [CommandProperty(AccessLevel.Administrator)]
        public string Title { get { return m_Title; } set { m_Title = value; } }
        [CommandProperty(AccessLevel.Administrator)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HideEffects { get { return m_HideEffects; } set { m_HideEffects = value; } }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (from != m_Owner && m_Owner != null)
            {
                from.SendMessage("You do not own this stone!");
                return;
            }

            if (m_Owner == null)
            {
                if (from.AccessLevel == AccessLevel.Player)
                {
                    if (HideEffects)
                        from.SendMessage("You are not authorized to have me! Goodbye!");
                    else
                        this.Say(String.Format("You are not authorized to have me! Goodbye!"));
                    deleting = true;
                    DoDelete(this);
                    return;
                }
                else
                {
                    if (HideEffects)
                        from.SendMessage("How May I serve you master!");
                    else
                        this.Say(String.Format("How May I serve you master!"));
                    m_Owner = from;
                    Name = m_Owner.Name + "'s Staff Stone";
                    this.StaffLevel = m_Owner.AccessLevel;
                    Movable = false;
                    this.HomeLocation = from.Location;
                    this.HomeMap = from.Map;
                    if (from.Title != null)
                        this.Title = from.Title;
                }
                return;
            }

            if (from is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)from;
                if (player != null)
                {
                    if (this.Title == null)
                        this.Title = from.Title;

                    if (DateTime.UtcNow < LastClicked + TimeSpan.FromSeconds(1.0))
                    {
                        if (!HideEffects)
                            this.Say(String.Format("Try clicking again a little later."));
                        else
                            from.SendMessage("Try clicking again a little later.");
                        return;
                    }
                    else
                    {
                        LastClicked = DateTime.UtcNow;
                        switch (player.AccessLevel)
                        {
                            case AccessLevel.Owner:
                                m_StaffLevel = from.AccessLevel;
                                player.AccessLevel = AccessLevel.Player;
                                break;
                            case AccessLevel.Administrator:
                                m_StaffLevel = from.AccessLevel;
                                player.AccessLevel = AccessLevel.Player;
                                break;
                            case AccessLevel.GameMaster:
                                m_StaffLevel = from.AccessLevel;
                                player.AccessLevel = AccessLevel.Player;
                                break;
                            case AccessLevel.Seer:
                                m_StaffLevel = from.AccessLevel;
                                player.AccessLevel = AccessLevel.Player;
                                break;
                            case AccessLevel.Counselor:
                                m_StaffLevel = from.AccessLevel;
                                player.AccessLevel = AccessLevel.Player;
                                break;
                            case AccessLevel.Player:
                                player.AccessLevel = m_StaffLevel;
                                break;
                            default:
                                deleting = true;
                                DoDelete(this);
                                break;
                        }
                    }
                    if (!HideEffects)
                        from.BoltEffect(0);
                }
            }
        }

        public Point3D HomeLocation { get { return m_HomeLocation; } set { m_HomeLocation = value; } }
        public Map HomeMap { get { return m_HomeMap; } set { m_HomeMap = value; } }
        public bool AutoRes { get { return m_AutoRes; } set { m_AutoRes = value; } }

        private class GoHomeEntry : ContextMenuEntry
        {
            private StaffTool m_Item;
            private Mobile m_Mobile;

            public GoHomeEntry(Mobile from, Item item)
                : base(5134) // uses "Goto Loc" entry
            {
                m_Item = (StaffTool)item;
                m_Mobile = from;
            }

            public override void OnClick()
            {
                // go to home location
                m_Mobile.Location = m_Item.HomeLocation;
                if (m_Item.HomeMap != null)
                    m_Mobile.Map = m_Item.HomeMap;
                else
                    m_Mobile.Map = m_Mobile.Map;
                if (!m_Item.HideEffects)
                {
                    Effects.SendLocationParticles(EffectItem.Create(m_Mobile.Location, m_Mobile.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                    Effects.SendLocationParticles(EffectItem.Create(m_Mobile.Location, m_Mobile.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
                    m_Mobile.PlaySound(0x1FE);
                }
            }
        }

        private class SetHomeEntry : ContextMenuEntry
        {
            private StaffTool m_Item;
            private Mobile m_Mobile;

            public SetHomeEntry(Mobile from, Item item)
                : base(2055) // uses "Mark" entry
            {
                m_Item = (StaffTool)item;
                m_Mobile = from;
            }

            public override void OnClick()
            {
                // set home location
                m_Item.HomeLocation = m_Mobile.Location;
                m_Item.HomeMap = m_Mobile.Map;
                m_Mobile.SendMessage("The home location on your stone has been set to your current position.");
            }
        }

        private class SetTitleEntry : ContextMenuEntry
        {
            private StaffTool m_Item;
            private Mobile m_Mobile;

            public SetTitleEntry(Mobile from, Item item)
                : base(0165) // uses "Title" entry
            {
                m_Item = (StaffTool)item;
                m_Mobile = from;
            }

            public override void OnClick()
            {
                // staff title on
                switch (m_Item.StaffLevel)
                {
                    case AccessLevel.Owner:
                        {
                            if (m_Mobile.Title != m_Item.Title)
                                m_Mobile.Title = m_Item.Title;
                            else
                                m_Mobile.Title = "[Owner]";
                            break;
                        }
                    case AccessLevel.Administrator:
                        {
                            if (m_Mobile.Title != m_Item.Title)
                                m_Mobile.Title = m_Item.Title;
                            else
                                m_Mobile.Title = "[Administrator]";
                            break;
                        }
                    case AccessLevel.Seer:
                        {
                            if (m_Mobile.Title != m_Item.Title)
                                m_Mobile.Title = m_Item.Title;
                            else
                                m_Mobile.Title = "[Seer]";
                            break;
                        }
                    case AccessLevel.GameMaster:
                        {
                            if (m_Mobile.Title != m_Item.Title)
                                m_Mobile.Title = m_Item.Title;
                            else
                                m_Mobile.Title = "[GameMaster]";
                            m_Mobile.Title = m_Item.Title;
                            break;
                        }
                    case AccessLevel.Counselor:
                        {
                            if (m_Mobile.Title != m_Item.Title)
                                m_Mobile.Title = m_Item.Title;
                            else
                                m_Mobile.Title = "[Counselor]";
                            m_Mobile.Title = m_Item.Title;
                            break;
                        }
                    case AccessLevel.Player:
                        {
                            if (m_Mobile.Title != m_Item.Title)
                                m_Mobile.Title = m_Item.Title;
                            else
                                m_Mobile.Title = null;
                            break;
                        }
                    default:
                        {
                            if (m_Mobile.Title != m_Item.Title)
                                m_Mobile.Title = m_Item.Title;
                            else
                                m_Mobile.Title = null;
                            break;
                        }
                }
                if (!m_Item.HideEffects)
                    m_Mobile.BoltEffect(0);
                m_Mobile.SendMessage("Your title has been adjusted.");
            }
        }

        private class MenuEntry : ContextMenuEntry
        {
            private StaffTool m_Item;
            private Mobile m_Mobile;

            public MenuEntry(Mobile from, Item item)
                : base(2132) // uses "Configure" entry
            {
                m_Item = item as StaffTool;
                m_Mobile = from;
            }

            public override void OnClick()
            {
                // send gump
                m_Mobile.CloseGump(typeof(StaffToolGump));
                m_Mobile.SendGump(new StaffToolGump(m_Mobile));
            }
        }

        private class PageQueueEntry : ContextMenuEntry
        {
            private Mobile m_Mobile;

            public PageQueueEntry(Mobile from, Item item)
                : base(1002) // uses "Messages" entry
            {
                m_Mobile = from;
            }

            public override void OnClick()
            {
                // send gump
                m_Mobile.CloseGump(typeof(PageQueueGump));
                m_Mobile.SendGump(new PageQueueGump());
            }
        }

        private class ToggleVisEntry : ContextMenuEntry
        {
            private Mobile m_Mobile;
            private StaffTool m_Item;

            public ToggleVisEntry(Mobile from, Item item)
                : base(0510) // uses "Toggle Vis" entry
            {
                m_Item = item as StaffTool;
                m_Mobile = from;
            }

            public override void OnClick()
            {
                if (m_Mobile.Hidden)
                {
                    m_Mobile.Hidden = false;
                    if (!m_Item.HideEffects)
                    {
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y, m_Mobile.Z + 4), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y, m_Mobile.Z), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y, m_Mobile.Z - 4), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X, m_Mobile.Y + 1, m_Mobile.Z + 4), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X, m_Mobile.Y + 1, m_Mobile.Z), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X, m_Mobile.Y + 1, m_Mobile.Z - 4), m_Mobile.Map, 0x3728, 13);

                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y + 1, m_Mobile.Z + 11), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y + 1, m_Mobile.Z + 7), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y + 1, m_Mobile.Z + 3), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y + 1, m_Mobile.Z - 1), m_Mobile.Map, 0x3728, 13);

                        m_Mobile.PlaySound(0x228);
                    }
                    m_Mobile.SendMessage("You have been revealed!");
                }
                else
                {
                    m_Mobile.Hidden = true;
                    if (!m_Item.HideEffects)
                    {
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y, m_Mobile.Z + 4), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y, m_Mobile.Z), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y, m_Mobile.Z - 4), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X, m_Mobile.Y + 1, m_Mobile.Z + 4), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X, m_Mobile.Y + 1, m_Mobile.Z), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X, m_Mobile.Y + 1, m_Mobile.Z - 4), m_Mobile.Map, 0x3728, 13);

                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y + 1, m_Mobile.Z + 11), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y + 1, m_Mobile.Z + 7), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y + 1, m_Mobile.Z + 3), m_Mobile.Map, 0x3728, 13);
                        Effects.SendLocationEffect(new Point3D(m_Mobile.X + 1, m_Mobile.Y + 1, m_Mobile.Z - 1), m_Mobile.Map, 0x3728, 13);

                        m_Mobile.PlaySound(0x228);
                    }

                    m_Mobile.SendMessage("You have been hidden!");
                }
            }
        }

        public static void GetContextMenuEntries(Mobile from, Item item, List<ContextMenuEntry> list)
        {
            list.Add(new GoHomeEntry(from, item));
            list.Add(new SetHomeEntry(from, item));
            list.Add(new SetTitleEntry(from, item));
            list.Add(new MenuEntry(from, item));
            list.Add(new PageQueueEntry(from, item));
            list.Add(new ToggleVisEntry(from, item));
        }

		 public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            if (m_Owner == null)
            {
                return;
            }
            else
            {
                if (m_Owner != from)
                {
                    from.SendMessage("This is not yours to use.");
                    return;
                }
                else
                {
                    base.GetContextMenuEntries(from, list);
                    StaffTool.GetContextMenuEntries(from, this, list);
                }
            }
        }

        public override DeathMoveResult OnInventoryDeath(Mobile parent)
        {
            if (m_Owner == null)
                return base.OnInventoryDeath(parent);

            if (m_AutoRes && parent == m_Owner)
            {
                new AutoResTimer(parent).Start();
            }
            else if (parent != m_Owner)
            {
                m_Owner.AddToBackpack(this);
                deleting = true;
                DoDelete(this);
            }
            return base.OnInventoryDeath(parent);
        }

        public void DoDelete(StaffTool me)
        {
            if (deleting)
            {
                me.Say(String.Format("Oh what a cruel world we live in! They are purging me from the system!"));
                new DeleteTimer(me).Start();
            }
        }

        private class DeleteTimer : Timer
        {
            public StaffTool me;
            public DeleteTimer(StaffTool stone)
                : base(TimeSpan.FromSeconds(5.0))
            {
                me = stone;
            }

            protected override void OnTick()
            {
                me.Delete();
            }
        }

        private class AutoResTimer : Timer
        {
            private Mobile m_Mobile;
            public AutoResTimer(Mobile mob)
                : base(TimeSpan.FromSeconds(5.0))
            {
                m_Mobile = mob;
            }

            protected override void OnTick()
            {
                m_Mobile.PlaySound(0x214);
                m_Mobile.FixedEffect(0x376A, 10, 16);
                m_Mobile.Resurrect();
                m_Mobile.Hidden = true;
                m_Mobile.SendMessage("As a staff member, you should be more careful in the future.");
                Stop();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)4);

            writer.Write(m_HideEffects);
            writer.WriteDeltaTime((DateTime)m_LastClicked);
            writer.Write(m_deleting);
            writer.Write(m_Title);
            writer.Write(m_AutoRes);
            writer.Write(m_HomeLocation);
            writer.Write(m_HomeMap);
            writer.Write((int)m_StaffLevel);
            writer.Write((Mobile)m_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 4:
                    m_HideEffects = reader.ReadBool();
                    goto case 3;
                case 3:
                    m_LastClicked = reader.ReadDeltaTime();
                    goto case 2;
                case 2:
                    m_deleting = reader.ReadBool();
                    goto case 1;
                case 1:
                    m_Title = reader.ReadString();
                    goto case 0;
                case 0:
                    m_AutoRes = reader.ReadBool();
                    m_HomeLocation = reader.ReadPoint3D();
                    m_HomeMap = reader.ReadMap();
                    m_StaffLevel = (AccessLevel)reader.ReadInt();
                    m_Owner = reader.ReadMobile();
                    break;
            }
        }
    }

    public class LevelTarget : Target
    {
        private object m_Object;
        private Item m_Item;
        private int m_SLevel;

        public int SLevel { get { return m_SLevel; } set { m_SLevel = value; } }

        public LevelTarget()
            : base(-1, false, TargetFlags.None)
        {
            //m_Object = o;
        }

        protected override void OnTarget(Mobile from, object o)
        {
            IPoint3D p = o as IPoint3D;
            m_Object = o;

            if (p != null)
            {
                if (m_Object is Mobile)
                {
                    Mobile m = (Mobile)m_Object;
                    Account acc = (Account)m.Account;
                    StaffTool stone = null;

                    StaffTool nstn = new StaffTool();

                    Container pack = from.Backpack;

                    if (pack != null)
                        m_Item = pack.FindItemByType(typeof(StaffTool), true);

                    if (m_Item != null)
                        stone = m_Item as StaffTool;

                    StaffTool thing = m.Backpack.FindItemByType(typeof(StaffTool), true) as StaffTool;
                    if (thing != null)
                    {
                        thing.deleting = true;
                        thing.DoDelete(thing);
                    }

                    if (stone != null)
                    {
                        m_SLevel = stone.GumpChoice;
                    }

                    if (!m.Deleted && m.Player)
                    {
                        if (SLevel == 0)
                        {
                            if (from.AccessLevel == AccessLevel.Player)
                            {
                                from.SendMessage("You can Not Change someones accesslevel who is equal to yours!");
                                return;
                            }
                            else if (from.AccessLevel < AccessLevel.Player)
                            {
                                from.SendMessage("You can Not Change someones accesslevel who is greater than yours!");
                                return;
                            }
                            else
                            {
                                from.SendMessage("You Changed someones accesslevel to a Player!");
                                acc.AccessLevel = m.AccessLevel = AccessLevel.Player;
                            }
                        }
                        if (SLevel == 1)
                        {
                            if (from.AccessLevel == AccessLevel.Counselor)
                            {
                                from.SendMessage("You can Not Change someones accesslevel who is equal to yours!");
                                return;
                            }
                            else if (from.AccessLevel < AccessLevel.Counselor)
                            {
                                from.SendMessage("You can Not Change someones accesslevel who is greater than yours!");
                                return;
                            }
                            else
                            {
                                from.SendMessage("You Changed someones accesslevel to a Counselor!");
                                acc.AccessLevel = m.AccessLevel = AccessLevel.Counselor;
                                nstn.Owner = m;
                                nstn.Name = m.Name + "'s Staff Stone";
                                nstn.StaffLevel = m.AccessLevel;
                                nstn.LootType = LootType.Blessed;
                                nstn.HomeLocation = m.Location;
                                nstn.HomeMap = m.Map;
                                if (m.Title != null)
                                    nstn.Title = m.Title;
                                m.AddToBackpack(nstn);
                                //m.BoltEffect( 0 );
                            }
                        }
                        if (SLevel == 2)
                        {
                            if (from.AccessLevel == AccessLevel.Seer)
                            {
                                from.SendMessage("You can Not Change someones accesslevel who is equal to yours!");
                                return;
                            }
                            else if (from.AccessLevel < AccessLevel.Seer)
                            {
                                from.SendMessage("You can Not Change someones accesslevel who is greater than yours!");
                                return;
                            }
                            else
                            {
                                from.SendMessage("You Changed someones accesslevel to a Seer!");
                                acc.AccessLevel = m.AccessLevel = AccessLevel.Seer;
                                nstn.Owner = m;
                                nstn.Name = m.Name + "'s Staff Stone";
                                nstn.StaffLevel = m.AccessLevel;
                                nstn.LootType = LootType.Blessed;
                                nstn.HomeLocation = m.Location;
                                nstn.HomeMap = m.Map;
                                if (m.Title != null)
                                    nstn.Title = m.Title;
                                m.AddToBackpack(nstn);
                                //m.BoltEffect( 0 );
                            }
						}
						if (SLevel == 3)
						{
							if (from.AccessLevel == AccessLevel.EventMaster)
							{
								from.SendMessage("You can Not Change someones accesslevel who is equal to yours!");
								return;
							}
							else if (from.AccessLevel < AccessLevel.EventMaster)
							{
								from.SendMessage("You can Not Change someones accesslevel who is greater than yours!");
								return;
							}
							else
							{
								from.SendMessage("You Changed someones accesslevel to a EventMaster!");
								acc.AccessLevel = m.AccessLevel = AccessLevel.EventMaster;
								nstn.Owner = m;
								nstn.Name = m.Name + "'s Staff Stone";
								nstn.StaffLevel = m.AccessLevel;
								nstn.LootType = LootType.Blessed;
								nstn.HomeLocation = m.Location;
								nstn.HomeMap = m.Map;
								if (m.Title != null)
									nstn.Title = m.Title;
								m.AddToBackpack(nstn);
								//m.BoltEffect( 0 );
							}
						}
						if (SLevel == 4)
						{
							if (from.AccessLevel == AccessLevel.GameMaster)
							{
								from.SendMessage("You can Not Change someones accesslevel who is equal to yours!");
								return;
							}
							else if (from.AccessLevel < AccessLevel.GameMaster)
							{
								from.SendMessage("You can Not Change someones accesslevel who is greater than yours!");
								return;
							}
							else
							{
								from.SendMessage("You Changed someones accesslevel to a GameMaster!");
								acc.AccessLevel = m.AccessLevel = AccessLevel.GameMaster;
								nstn.Owner = m;
								nstn.Name = m.Name + "'s Staff Stone";
								nstn.StaffLevel = m.AccessLevel;
								nstn.LootType = LootType.Blessed;
								nstn.HomeLocation = m.Location;
								nstn.HomeMap = m.Map;
								if (m.Title != null)
									nstn.Title = m.Title;
								m.AddToBackpack(nstn);
								//m.BoltEffect( 0 );
							}
						}
						if (SLevel == 5)
						{
							if (from.AccessLevel == AccessLevel.Lead)
							{
								from.SendMessage("You can Not Change someones accesslevel who is equal to yours!");
								return;
							}
							else if (from.AccessLevel < AccessLevel.Lead)
							{
								from.SendMessage("You can Not Change someones accesslevel who is greater than yours!");
								return;
							}
							else
							{
								from.SendMessage("You Changed someones accesslevel to a Lead!");
								acc.AccessLevel = m.AccessLevel = AccessLevel.Lead;
								nstn.Owner = m;
								nstn.Name = m.Name + "'s Staff Stone";
								nstn.StaffLevel = m.AccessLevel;
								nstn.LootType = LootType.Blessed;
								nstn.HomeLocation = m.Location;
								nstn.HomeMap = m.Map;
								if (m.Title != null)
									nstn.Title = m.Title;
								m.AddToBackpack(nstn);
								//m.BoltEffect( 0 );
							}
						}
						if (SLevel == 6)
						{
							if (from.AccessLevel < AccessLevel.Administrator)
							{
								from.SendMessage("You can Not Change someones accesslevel who is greater than yours!");
								return;
							}
							else
							{
								from.SendMessage("You Changed someones accesslevel to an Administrator!");
								acc.AccessLevel = m.AccessLevel = AccessLevel.Administrator;
								nstn.Owner = m;
								nstn.Name = m.Name + "'s Staff Stone";
								nstn.StaffLevel = m.AccessLevel;
								nstn.LootType = LootType.Blessed;
								nstn.HomeLocation = m.Location;
								nstn.HomeMap = m.Map;
								if (m.Title != null)
									nstn.Title = m.Title;
								m.AddToBackpack(nstn);
								//m.BoltEffect( 0 );
							}
						}
						if (SLevel == 7)
						{
							if (from.AccessLevel < AccessLevel.Developer)
							{
								from.SendMessage("You can Not Change someones accesslevel who is greater than yours!");
								return;
							}
							else
							{
								from.SendMessage("You Changed someones accesslevel to a Developer!");
								acc.AccessLevel = m.AccessLevel = AccessLevel.Developer;
								nstn.Owner = m;
								nstn.Name = m.Name + "'s Staff Stone";
								nstn.StaffLevel = m.AccessLevel;
								nstn.LootType = LootType.Blessed;
								nstn.HomeLocation = m.Location;
								nstn.HomeMap = m.Map;
								if (m.Title != null)
									nstn.Title = m.Title;
								m.AddToBackpack(nstn);
								//m.BoltEffect( 0 );
							}
						}
                        if (SLevel == 8)
                        {
                            if (from.AccessLevel < AccessLevel.Owner)
                            {
                                from.SendMessage("You can Not Change someones accesslevel who is greater than yours!");
                                return;
                            }
                            else
                            {
                                from.SendMessage("You Changed someones accesslevel to an Owner!");
                                acc.AccessLevel = m.AccessLevel = AccessLevel.Owner;
                                nstn.Owner = m;
                                nstn.Name = m.Name + "'s Staff Stone";
                                nstn.StaffLevel = m.AccessLevel;
                                nstn.LootType = LootType.Blessed;
                                nstn.HomeLocation = m.Location;
                                nstn.HomeMap = m.Map;
                                if (m.Title != null)
                                    nstn.Title = m.Title;
                                m.AddToBackpack(nstn);
                                //m.BoltEffect( 0 );
                            }
                        }
                    }
                    else if (!m.Player)
                    {
                        from.SendMessage("This can only be used on Players!");
                        return;
                    }
                }
            }
        }
    }

    public class StaffToolGump : Gump
    {
        private Mobile m_from;
        private Item m_Item;
        public Mobile from { get { return m_from; } set { m_from = value; } }

        public StaffToolGump(Mobile owner)
            : base(0, 0)
        {
            m_from = owner;

            Container pack = m_from.Backpack;

            if (pack != null)
                m_Item = pack.FindItemByType(typeof(StaffTool), true);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(1);
            AddBackground(0, 0, 210, 375, 9200);
            AddLabel(75, 15, 55, @"General");
            AddButton(175, 225, 2224, 2224, 1, GumpButtonType.Page, 2);

            AddLabel(45, 45, 55, @"Toggle AutoRes");
            AddButton(15, 45, 1210, 1209, 7, GumpButtonType.Reply, 0);
            AddLabel(45, 75, 55, @"Toggle AccessLevel");
            AddButton(15, 75, 1210, 1209, 8, GumpButtonType.Reply, 0);
            AddLabel(46, 105, 55, @"Recall To Home");
            AddButton(15, 105, 1210, 1209, 9, GumpButtonType.Reply, 0);
            AddLabel(45, 135, 55, @"Toggle Title");
            AddButton(15, 135, 1210, 1209, 10, GumpButtonType.Reply, 0);
            AddLabel(45, 165, 55, @"Toggle Hidden");
            AddButton(15, 165, 1210, 1209, 11, GumpButtonType.Reply, 0);
            AddLabel(45, 195, 55, @"Check GM Queue");
            AddButton(15, 195, 1210, 1209, 21, GumpButtonType.Reply, 0);

            AddPage(2);
			AddBackground(0, 0, 210, 375, 9200);
            AddLabel(50, 15, 55, @"Set Staff Level");
            AddButton(15, 225, 2223, 2223, 3, GumpButtonType.Page, 1);
            AddButton(175, 225, 2224, 2224, 2, GumpButtonType.Page, 3);

            AddLabel(45, 45, 55, @"Target AccessLevel");
            AddButton(15, 45, 1210, 1209, 12, GumpButtonType.Reply, 0);
            AddGroup(0);
            AddRadio(45, 70, 208, 209, false, 13);
            AddRadio(45, 100, 208, 209, false, 14);
            AddRadio(45, 130, 208, 209, false, 15);
			AddRadio(45, 160, 208, 209, false, 16);
			AddRadio(45, 190, 208, 209, false, 17);
			AddRadio(45, 220, 208, 209, false, 18);
			AddRadio(45, 250, 208, 209, false, 19);
			AddRadio(45, 280, 208, 209, false, 20);
			AddRadio(45, 310, 208, 209, false, 21);
            AddLabel(80, 70, 62, @"Player");
            AddLabel(80, 100, 62, @"Counselor");
			AddLabel(80, 130, 62, @"Seer");
			AddLabel(80, 160, 62, @"Event Master");
			AddLabel(80, 190, 62, @"Game Master");
			AddLabel(80, 220, 62, @"Lead");
			AddLabel(80, 250, 62, @"Administrator");
			AddLabel(80, 280, 62, @"Developer");
            AddLabel(80, 310, 62, @"Owner");

            AddPage(3);
			AddBackground(0, 0, 210, 375, 9200);
            AddLabel(75, 15, 55, @"Config");
            AddButton(15, 225, 2223, 2223, 4, GumpButtonType.Page, 2);

            AddButton(15, 75, 1210, 1209, 18, GumpButtonType.Reply, 0);
            AddLabel(45, 75, 55, @"Set Home");
            AddButton(15, 105, 1210, 1209, 19, GumpButtonType.Reply, 0);
            AddLabel(45, 105, 55, @"Set Title");
            AddButton(15, 45, 1210, 1209, 20, GumpButtonType.Reply, 0);
            AddLabel(45, 45, 55, @"Clear Title");

        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            StaffTool stone = m_Item as StaffTool;
            if (stone != null)
            {
                if (info.IsSwitched(13))
                {
                    stone.GumpChoice = 0;
                }
                else if (info.IsSwitched(14))
                {
                    stone.GumpChoice = 1;
                }
                else if (info.IsSwitched(15))
                {
                    stone.GumpChoice = 2;
                }
                else if (info.IsSwitched(16))
                {
                    stone.GumpChoice = 3;
				}
				else if (info.IsSwitched(17))
				{
					stone.GumpChoice = 4;
				}
				else if (info.IsSwitched(18))
				{
					stone.GumpChoice = 5;
				}
				else if (info.IsSwitched(19))
				{
					stone.GumpChoice = 6;
				}
				else if (info.IsSwitched(20))
				{
					stone.GumpChoice = 7;
				}
				else if (info.IsSwitched(21))
				{
					stone.GumpChoice = 8;
				}
                else
                    stone.GumpChoice = -1;


                switch (info.ButtonID)
                {/*
				case 1:
				{
					break;
				}
				case 2:
				{
					break;
				}
				case 3:
				{
					break;
				}
				case 4:
				{
					break;
				}
				case 5:
				{
					break;
				}
				case 6:
				{
					break;
				}*/
                    case 7:
                        {
                            if (stone.AutoRes)
                            {
                                stone.AutoRes = false;
                                from.SendMessage("AutoRes feature of the stone has been disbled.");
                            }
                            else
                            {
                                stone.AutoRes = true;
                                from.SendMessage("AutoRes feature of the stone has been enabled.");
                            }
                            from.CloseGump(typeof(StaffToolGump));
                            from.SendGump(new StaffToolGump(from));
                            break;
                        }
                    case 8:
                        {
                            if (from is PlayerMobile)
                            {
                                PlayerMobile player = (PlayerMobile)from;
                                if (player != null)
                                {
                                    if (stone.Title == null)
                                        stone.Title = from.Title;

                                    switch (player.AccessLevel)
                                    {
                                        case AccessLevel.Owner:
                                            stone.StaffLevel = from.AccessLevel;
                                            player.AccessLevel = AccessLevel.Player;
											break;
										case AccessLevel.Developer:
											stone.StaffLevel = from.AccessLevel;
											player.AccessLevel = AccessLevel.Player;
											break;
										case AccessLevel.Administrator:
											stone.StaffLevel = from.AccessLevel;
											player.AccessLevel = AccessLevel.Player;
											break;
										case AccessLevel.Lead:
											stone.StaffLevel = from.AccessLevel;
											player.AccessLevel = AccessLevel.Player;
											break;
										case AccessLevel.GameMaster:
											stone.StaffLevel = from.AccessLevel;
											player.AccessLevel = AccessLevel.Player;
											break;
										case AccessLevel.EventMaster:
											stone.StaffLevel = from.AccessLevel;
											player.AccessLevel = AccessLevel.Player;
											break;
                                        case AccessLevel.Seer:
                                            stone.StaffLevel = from.AccessLevel;
                                            player.AccessLevel = AccessLevel.Player;
                                            break;
                                        case AccessLevel.Counselor:
                                            stone.StaffLevel = from.AccessLevel;
                                            player.AccessLevel = AccessLevel.Player;
                                            break;
                                        case AccessLevel.Player:
                                            player.AccessLevel = stone.StaffLevel;
                                            break;
                                        default:
                                            stone.deleting = true;
                                            stone.DoDelete(stone);
                                            break;
                                    }
                                    if (!stone.HideEffects)
                                        player.BoltEffect(0);
                                }
                            }
                            from.CloseGump(typeof(StaffToolGump));
                            from.SendGump(new StaffToolGump(from));
                            break;
                        }
                    case 9:
                        {
                            from.Location = stone.HomeLocation;
                            if (stone.HomeMap != null)
                                from.Map = stone.HomeMap;
                            else
                                from.Map = from.Map;
                            from.CloseGump(typeof(StaffToolGump));
                            from.SendGump(new StaffToolGump(from));
                            break;
                        }
                    case 10:
                        {
                            switch (stone.StaffLevel)
                            {
                               case AccessLevel.Owner:
                                    {
                                        if (from.Title != stone.Title)
                                            from.Title = stone.Title;
                                        else
                                            from.Title = " [Owner] ";
                                        break;
									}
							   case AccessLevel.Developer:
									{
										if (from.Title != stone.Title)
											from.Title = stone.Title;
										else
											from.Title = " [Developer] ";
										break;
									}
							   case AccessLevel.Administrator:
									{
										if (from.Title != stone.Title)
											from.Title = stone.Title;
										else
											from.Title = " [Administrator] ";
										break;
									}
							   case AccessLevel.Lead:
									{
										if (from.Title != stone.Title)
											from.Title = stone.Title;
										else
											from.Title = " [Lead] ";
										break;
									}
							   case AccessLevel.GameMaster:
									{
										if (from.Title != stone.Title)
											from.Title = stone.Title;
										else
											from.Title = " [Game Master] ";
										break;
									}
                                case AccessLevel.Seer:
                                    {
                                        if (from.Title != stone.Title)
                                            from.Title = stone.Title;
                                        else
                                            from.Title = " [Seer] ";
                                        break;
									}
								case AccessLevel.EventMaster:
									{
										if (from.Title != stone.Title)
											from.Title = stone.Title;
										else
											from.Title = " [Event Master] ";
										break;
									}
                                case AccessLevel.Counselor:
                                    {
                                        if (from.Title != stone.Title)
                                            from.Title = stone.Title;
                                        else
                                            from.Title = " [Counselor] ";
                                        break;
                                    }
                                case AccessLevel.Player:
                                    {
                                        if (from.Title != stone.Title)
                                            from.Title = stone.Title;
                                        else
                                            from.Title = null;
                                        break;
                                    }
                                default:
                                    {
                                        if (from.Title != stone.Title)
                                            from.Title = stone.Title;
                                        else
                                            from.Title = null;
                                        break;
                                    }
                            }
                            from.SendMessage("Your title has been changed.");
                            if (!stone.HideEffects)
                                from.BoltEffect(0);
                            from.CloseGump(typeof(StaffToolGump));
                            from.SendGump(new StaffToolGump(from));
                            break;
                        }
                    case 11:
                        {
                            if (from.Hidden)
                            {
                                from.Hidden = false;
                                if (!stone.HideEffects)
                                {
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y, from.Z + 4), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y, from.Z), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y, from.Z - 4), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X, from.Y + 1, from.Z + 4), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X, from.Y + 1, from.Z), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X, from.Y + 1, from.Z - 4), from.Map, 0x3728, 13);

                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y + 1, from.Z + 11), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y + 1, from.Z + 7), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y + 1, from.Z + 3), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y + 1, from.Z - 1), from.Map, 0x3728, 13);

                                    from.PlaySound(0x228);
                                }
                                from.SendMessage("You have been revealed!");
                            }
                            else
                            {
                                from.Hidden = true;
                                if (!stone.HideEffects)
                                {
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y, from.Z + 4), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y, from.Z), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y, from.Z - 4), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X, from.Y + 1, from.Z + 4), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X, from.Y + 1, from.Z), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X, from.Y + 1, from.Z - 4), from.Map, 0x3728, 13);

                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y + 1, from.Z + 11), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y + 1, from.Z + 7), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y + 1, from.Z + 3), from.Map, 0x3728, 13);
                                    Effects.SendLocationEffect(new Point3D(from.X + 1, from.Y + 1, from.Z - 1), from.Map, 0x3728, 13);

                                    from.PlaySound(0x228);
                                }
                                from.SendMessage("You have been hidden!");
                            }
                            from.CloseGump(typeof(StaffToolGump));
                            from.SendGump(new StaffToolGump(from));
                            break;
                        }
                    case 12:
                        {
                            from.Target = new LevelTarget();
                            from.CloseGump(typeof(StaffToolGump));
                            from.SendGump(new StaffToolGump(from));
                            break;
                        }/*
					case 13:
					{
						stone.GumpChoice = 0;
						break;
					}
					case 14:
					{
						stone.GumpChoice = 1;
						break;
					}
					case 15:
					{
						stone.GumpChoice = 2;
						break;
					}
					case 16:
					{
						stone.GumpChoice = 3;
						break;
					}
					case 17:
					{
						stone.GumpChoice = 4;
						break;
					}*/
                    case 18:
                        {
                            stone.HomeLocation = from.Location;
                            stone.HomeMap = from.Map;
                            from.SendMessage("The home location on your stone has been set to your current position.");
                            from.CloseGump(typeof(StaffToolGump));
                            from.SendGump(new StaffToolGump(from));
                            break;
                        }
                    case 19:
                        {
                            stone.Title = from.Title;
                            from.SendMessage("Your title has been saved on the stone.");
                            from.CloseGump(typeof(StaffToolGump));
                            from.SendGump(new StaffToolGump(from));
                            break;
                        }
                    case 20:
                        {
                            stone.Title = null;
                            from.SendMessage("Your title has been cleared from the stone.");
                            from.CloseGump(typeof(StaffToolGump));
                            from.SendGump(new StaffToolGump(from));
                            break;
                        }
                    case 21:
                        {
                            from.CloseGump(typeof(StaffToolGump));
                            from.SendGump(new StaffToolGump(from));
                            from.CloseGump(typeof(PageQueueGump));
                            from.SendGump(new PageQueueGump());
                            break;
                        }
                }
            }
        }
    }
}