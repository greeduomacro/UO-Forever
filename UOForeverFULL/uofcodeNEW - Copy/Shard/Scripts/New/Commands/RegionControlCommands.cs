using System;
using Server.Regions;
using Server.Items;

namespace Server.Commands
{
    public class FindRegionControl
    {
        public static void Initialize()
        {
            CommandSystem.Register("GoToRC", AccessLevel.GameMaster, new CommandEventHandler(FindRegionControl_OnCommand));
            CommandSystem.Register("GetRC", AccessLevel.Lead, new CommandEventHandler(PullRegionControl_OnCommand));
        }

        [Usage("GoToRC")]
        [Description("Goes to the region controller for the current region.")]
        public static void FindRegionControl_OnCommand(CommandEventArgs e)
        {
			CustomRegion reg = e.Mobile.Region as CustomRegion;

            if ( reg != null && reg.Controller != null && !reg.Controller.Deleted )
                e.Mobile.MoveToWorld( reg.Controller.Location, reg.Controller.Map );
            else
                e.Mobile.SendMessage("There is no controller for this region.");
        }

        [Usage("GetRC")]
        [Description("Gets the region controller for the current region.")]
        public static void PullRegionControl_OnCommand(CommandEventArgs e)
        {
			CustomRegion reg = e.Mobile.Region as CustomRegion;

            if ( reg != null && reg.Controller != null && !reg.Controller.Deleted )
			{
				reg.Controller.MoveToWorld( e.Mobile.Location, e.Mobile.Map );
                e.Mobile.SendMessage("The region controller has been pulled to your location.");
            }
            else
                e.Mobile.SendMessage("There is no controller for this region.");
        }
    }
}