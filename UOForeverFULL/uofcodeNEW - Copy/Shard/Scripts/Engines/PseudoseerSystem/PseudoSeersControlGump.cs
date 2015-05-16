using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;
using Server.Accounting;
using Server.Network;
using Server.Gumps;
using Server.Targeting;
using Server.Commands.Generic;

namespace Server.Games 
{
	public class PseudoSeersControlGump : Gump
	{
		public static void Initialize()
		{
			CommandSystem.Register( "pseudoseers", CreaturePossession.FullAccessStaffLevel, new CommandEventHandler( PseudoseersList_OnCommand ) );
		}

        [Usage("Pseudoseers")]
		[Description( "Lists all current pseudoseers." )]
        private static void PseudoseersList_OnCommand(CommandEventArgs e)
		{
            if (PseudoSeerStone.Instance == null)
            {
                e.Mobile.SendMessage("No Pseudoseer stone exists.  [add pseudoseerstone to create one.");
                return;
            }
            e.Mobile.SendGump( new PseudoSeersControlGump( e.Mobile ) );
		}

        public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
        public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;

		public static readonly int TextHue = PropsConfig.TextHue;
		public static readonly int TextOffsetX = PropsConfig.TextOffsetX;

		public static readonly int NextWidth = PropsConfig.NextWidth;
		public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
		public static readonly int ArrowButtonID1 = PropsConfig.NextButtonID1;
        public static readonly int ArrowButtonID2 = PropsConfig.NextButtonID2;

        private const int LabelColor32 = 0x000044;
        private const int WIDTH = 400;
        private const int HEIGHT = 450;
        readonly public static int LEFT_EDGE = 10;
        readonly public static int TOP_EDGE = 10;
        readonly public static int ARROW_WIDTH = 20;
        readonly public static int LABEL_WIDTH = 20;

		private Mobile m_Owner;

        public PseudoSeersControlGump(Mobile owner)
            : base(GumpOffsetX, GumpOffsetY)
		{
            owner.CloseGump(typeof( PseudoSeersControlGump ));
            if (PseudoSeerStone.Instance == null)
            {
                return;
            }
			m_Owner = owner;
            
			InitializeGump();
		}

        

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }
        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public enum BUTTON_IDS : int
        {
            Closed = 0,
            AddPseudoSeer = 1,
            RemovePseudoSeer = 2,
            BringAllPseudoSeersHere = 3,
            PermissionClipboardAdd = 4,
            PermissionClipboardClear = 5,
            PermissionClipboardApplyToAll = 6,
            BringPseudoSeerStoneHere = 7,
            RemoveAllPseudoSeers = 8,
            PseudoSeersList = 9
        }

        public void InitializeGump()
        {

            int penX = LEFT_EDGE;
            int penY = TOP_EDGE;
            int lineHeight = 20;

            AddBackground(0, 0, WIDTH, HEIGHT, 5054);
            AddAlphaRegion(LEFT_EDGE, TOP_EDGE, WIDTH - 2 * LEFT_EDGE, HEIGHT - 2 * TOP_EDGE);

            AddHtml(penX, penY, WIDTH - 2 * LEFT_EDGE, lineHeight, Color(Center("<u>Pseudoseer Control Panel</u>"), LabelColor32), false, false);
            penY += lineHeight;

            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.AddPseudoSeer, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "Add Pseudoseer / Update Pseudoseer Permissions");
            penX = LEFT_EDGE;
            penY += lineHeight;

            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.RemovePseudoSeer, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "Remove Pseudoseer");
            penX = LEFT_EDGE + WIDTH / 2;
            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.RemoveAllPseudoSeers, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "Remove ALL Pseudoseers");
            penY += lineHeight;

            penX = LEFT_EDGE;
            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.BringAllPseudoSeersHere, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "Bring All Pseudoseers Here");
            penX = LEFT_EDGE;
            penY += lineHeight * 2;

            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.BringPseudoSeerStoneHere, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "Bring Stone Here");
            penX = LEFT_EDGE;
            penY += lineHeight * 2;

            // Permissions stuff

            AddHtml(penX, penY + (int)(0.66*lineHeight), WIDTH / 2, lineHeight, Color(Center("<u>Permissions Clipboard</u>"), LabelColor32), false, false);
            penX += WIDTH / 2;
            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.PermissionClipboardApplyToAll, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "Apply to All Pseudoseers");
            penY += lineHeight;
            
            penX = LEFT_EDGE + WIDTH / 2;
            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.PermissionClipboardClear, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "Clear");
            penY += lineHeight;
            penX = LEFT_EDGE;

            /*
            string[] names = Enum.GetNames(typeof(CreaturePossession.PossessPermissions));
            int[] values = (int[])Enum.GetValues(typeof(CreaturePossession.PossessPermissions));
            for (int i = 1; i < names.Length; i++)
            { // skip None

                if ((i + 0) % 3 == 0) // 3 checkboxes per line
                {
                    penY += lineHeight;
                    penX = LEFT_EDGE;
                }
                bool isChecked = (((int)PseudoSeerStone.PermissionsClipboard & values[i]) != 0) && (values[i] != -1); // All is never checked
                AddCheck(penX, penY, 0xD2, 0xD3, isChecked, values[i]);
                penX += 20;
                AddLabel(penX, penY, TextHue, names[i]);
                penX += 115;
            }*/

            AddLabel(penX, penY, LabelColor32, PseudoSeerStone.Instance.CurrentPermissionsClipboard);
            penY += lineHeight;
            AddLabel(penX, penY, TextHue, "NOTE: To change the clipboard, use '[pseudoseeradd' command..." );
            penY += lineHeight;
            AddLabel(penX, penY, TextHue, "    e.g. [pseudoseeradd daemon orc ancientlich");

            penY += lineHeight * 2;

            penX = LEFT_EDGE;
            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.PseudoSeersList, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "List All Pseudoseers");
            penY += lineHeight;
            penX = LEFT_EDGE;
            
            
        }

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
            /*for (int i = 0; i < info.Switches.Length; i++) {
                newClipboard |= info.Switches[i];
            }
            PseudoSeerStone.PermissionsClipboard = (CreaturePossession.PossessPermissions)newClipboard;
             */
            if (PseudoSeerStone.Instance == null) {
                from.SendMessage("No Pseudoseer stone exists.  [add pseudoseerstone to create one.");
                return;
            }
			switch ( (BUTTON_IDS)info.ButtonID )
			{

                case BUTTON_IDS.Closed:
                    return;
                case BUTTON_IDS.AddPseudoSeer:
                    from.Target = new PseudoSeerTarget(from, BUTTON_IDS.AddPseudoSeer);
                    from.SendMessage("Target Player to add (with current clipboard permissions) or target Pseudoseer to update their permissions.");
                    break;
                case BUTTON_IDS.RemovePseudoSeer:
                    from.Target = new PseudoSeerTarget(from, BUTTON_IDS.RemovePseudoSeer);
                    from.SendMessage("Target Pseudoseer to remove.");
                    break;
                case BUTTON_IDS.RemoveAllPseudoSeers:
                    PseudoSeerStone.Instance.ClearPseudoSeers = true;
                    from.SendGump(new PseudoSeersControlGump(from));
                    break;
                case BUTTON_IDS.BringAllPseudoSeersHere:
                    from.SendGump(new PseudoSeersControlGump(from));
                    List<Mobile> pseudoseerControlledMobiles = PseudoseerControlledMobiles();
                    from.SendMessage("Bringing all Pseudoseers to your position...");
                    foreach (Mobile m in pseudoseerControlledMobiles)
                    {
                        from.SendMessage("Account: " + (m.NetState == null ? "" : m.NetState.Account.ToString())+ "  MobileType: " + m.GetType().Name);
                        if (!BaseCommand.IsAccessible(from, m))
                        {
                            from.SendMessage("Warning: Pseudoseer account " + m.Account + " is not accessible.");
                            continue;
                        }
                        if (!m.Deleted)
                        {
                            m.MoveToWorld(new Point3D(from.Location), from.Map);
                        }
                    }
                    break;
                case BUTTON_IDS.BringPseudoSeerStoneHere:
                    from.SendGump(new PseudoSeersControlGump(from)); 
                    if (!BaseCommand.IsAccessible(from, PseudoSeerStone.Instance))
                    {
                        from.SendMessage("Warning: Pseudoseer stone instance is not accessible.");
                        break;
                    }
                    if (!PseudoSeerStone.Instance.Deleted)
                    {
                        PseudoSeerStone.Instance.MoveToWorld(new Point3D(from.Location), from.Map);
                    }
                    break;
                case BUTTON_IDS.PermissionClipboardClear:
                    PseudoSeerStone.PermissionsClipboard = "";
                    from.SendGump(new PseudoSeersControlGump(from)); 
                    break;
                case BUTTON_IDS.PermissionClipboardApplyToAll:
                    // can't edit the dictionary as you iterate over it, so keep track of which ones need updated
                    List<IAccount> pseudoseersToUpdate = new List<IAccount>(PseudoSeerStone.Instance.PseudoSeers.Keys);
                    if (pseudoseersToUpdate.Count == 0)
                    {
                        from.SendMessage("There are currently no pseudoseers to update.");
                        from.SendGump(new PseudoSeersControlGump(from));
                    }
                    else
                    {
                        from.SendMessage("Updating Pseudoseer Permissions for the following accounts:");
                        foreach (IAccount account in pseudoseersToUpdate)
                        {
                            PseudoSeerStone.Instance.PseudoSeers[account] = PseudoSeerStone.PermissionsClipboard;
                            from.SendMessage("-->Account = " + account);
                        }
                        from.SendMessage("Clipboard permissions successfully applied to all current Pseudoseers.");
                        from.SendGump(new PseudoSeersControlGump(from));
                    }
                    break;
                case BUTTON_IDS.PseudoSeersList:
                    from.SendGump(new PseudoSeersListGump(from));
                    break;
			}
		}

        public static List<Mobile> PseudoseerControlledMobiles()
        {
            List<Mobile> list = new List<Mobile>();
            List<NetState> states = NetState.Instances;
            if (PseudoSeerStone.Instance == null)
            {
                return list;
            }
            for (int i = 0; i < states.Count; ++i)
            {
                Mobile m = states[i].Mobile;
                if (m != null && PseudoSeerStone.Instance.PseudoSeers.ContainsKey(states[i].Account))
                {
                    list.Add(m);
                }
            }
            return list;
        }

        protected class PseudoSeerTarget : Target
        {
            private object m_Object;
            private BUTTON_IDS type;

            public PseudoSeerTarget(object o, BUTTON_IDS type)
                : base(-1, false, TargetFlags.None)
            {
                m_Object = o;
                this.type = type;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                from.SendGump(new PseudoSeersControlGump(from));
                if (!(o is PlayerMobile))
                {
                    from.SendMessage("Not a valid target!  You can only target PlayerMobiles.");
                    // TODO handle the case where the target is a pseudoseer but is in monster form
                    return;
                }

                PlayerMobile p = (PlayerMobile)o;

                if (PseudoSeerStone.Instance == null) {
                    from.SendMessage("No Pseudoseer stone exists.  [add pseudoseerstone to create one.");
                    return;
                }
                switch ( type ) {
                    case BUTTON_IDS.AddPseudoSeer:
                        PseudoSeerStone.Instance.PseudoSeerAdd = p;

                        break;
                    case BUTTON_IDS.RemovePseudoSeer:
                        PseudoSeerStone.Instance.PseudoSeerRemove = p;
                        break;
                }
            }            
        }
	}


    
}