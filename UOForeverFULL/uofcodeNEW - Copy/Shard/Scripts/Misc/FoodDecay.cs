using System;
using Server.Network;
using Server;
using Server.Mobiles;
using Server.Commands;
using Server.Engines.XmlSpawner2;
using Server.Engines.Harvest;

namespace Server.Misc
{
	public class FoodDecayTimer : Timer
	{
		public static void Initialize()
		{
			new FoodDecayTimer().Start();
		}

		public FoodDecayTimer() : base( TimeSpan.FromMinutes( 30.0 ), TimeSpan.FromMinutes( 30.0 ) )
		{
			Priority = TimerPriority.OneMinute;
		}

        // Alan mod: I figured I'd make things a bit more efficient by hooking into a single timer here
        // with several occasional things.  Kinda hacky... so sue me.
        private static int thirtyMinuteCount = 0;
		protected override void OnTick()
		{
			FoodDecay();
            Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Paragon.RevertParagons));
            //Timer.DelayCall(TimeSpan.FromSeconds(10.0), new TimerCallback(LoggingCustom.WriteLogsToFiles));
            thirtyMinuteCount++;
            if (thirtyMinuteCount > 96) // every 48 hours
            {
                Timer.DelayCall(TimeSpan.FromSeconds(20.0), new TimerCallback(HarvestSystem.LogHarvesters));
                thirtyMinuteCount = 0;
            }
		}

		public static void FoodDecay()
		{
			foreach ( NetState state in NetState.Instances )
			{
				HungerDecay( state.Mobile );
				ThirstDecay( state.Mobile );
                // Why not decay murderer stat loss here?
                BaseShieldGuard.StatLossDecay( state.Mobile );
			}
		}

		public static void HungerDecay( Mobile m )
		{
			if ( m != null && m.Hunger >= 1 )
				m.Hunger--;
		}

		public static void ThirstDecay( Mobile m )
		{
			if ( m != null && m.Thirst >= 1 )
				m.Thirst--;
		}
	}
}