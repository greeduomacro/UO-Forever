using System;
using Server.Items;

namespace Server.ContextMenus
{
   public class ReDeedEntry : ContextMenuEntry
   {
      private CannonFire m_fcannon;
      private Mobile m_from;

      public ReDeedEntry( Mobile from, CannonFire fcannon ) : base( 89, 3 )
      {
         m_fcannon = fcannon;
         m_from = from;
      }

      public override void OnClick()
      {
         if ( !Owner.From.CheckAlive() )
            return;

            m_from.AddToBackpack ( new fireableCannonDeed(m_fcannon.Dir) );
            m_fcannon.Delete();

      }
   }
}
