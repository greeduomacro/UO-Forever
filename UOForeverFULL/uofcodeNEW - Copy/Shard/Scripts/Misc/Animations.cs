using System;
using Server;
using Server.Mobiles;

namespace Server.Misc
{
	public class Animations
	{
		public static void Initialize()
		{
			EventSink.AnimateRequest += new AnimateRequestEventHandler( EventSink_AnimateRequest );
		}

		private static void EventSink_AnimateRequest( AnimateRequestEventArgs e )
		{
			Mobile from = e.Mobile;

			int action;

			switch ( e.Action )
			{
				case "bow": action = 32; break;
				case "salute": action = 33; break;
				default: return;
			}

            if (from.Alive && !from.Mounted)
            {
                if (from.Body.IsHuman)
                    from.Animate(action, 5, 1, true, false, 0);
                else
                {
                    if (from.Body.IsAnimal)
			        {
                        if (action == 32) { from.Animate(3, 3, 1, true, false, 1); }
                        else if (action == 33) { from.Animate( 10, 5, 1, true, false, 1 ); }
			        }
                    else if ( from.Body.IsMonster)
			        {
                        if (action == 32) { from.Animate(11, 5, 1, true, false, 1); }
                        else if (action == 33) { from.Animate(18, 5, 1, true, false, 1); }
			        }
                }
            }
		}
	}
}