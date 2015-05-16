using System;
using Server;
using Server.Targeting;

namespace Server.Engines.Jail
{
	/// <summary>
	/// Generic target for use in the jailing system
	/// </summary>
	public class JailTarget : Target
	{
		/// <summary>
		/// The function that should be called if the targeting action is succesful
		/// </summary>
		JailTargetCallback m_Callback;

		/// <summary>
		/// Creates a new jail target
		/// </summary>
		/// <param name="callback">The function that will be called if the target action is succesful</param>
		public JailTarget( JailTargetCallback callback ) : base( -1, false, TargetFlags.None )
		{
			m_Callback = callback;
		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			Server.Mobiles.PlayerMobile pm = targeted as Server.Mobiles.PlayerMobile;

			if ( pm != null )
			{
				if ( pm.AccessLevel == AccessLevel.Player )
				{
					try
					{
						m_Callback.DynamicInvoke( new object[] { from, pm } );
					}
					catch {}
				}
				else
					from.SendMessage( "The jail system isn't supposed to be used on staff" );
			}
			else
				from.SendMessage( "The jail system only works on players" );
		}
	}
}