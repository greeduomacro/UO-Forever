/*
using System;

namespace Server
{
	public class MemoryHelperThinggie : Timer
	{
		private static TimeSpan Frequency = TimeSpan.FromMinutes( 30.0 );

		public static void Initialize()
		{
			new MemoryHelperThinggie().Start();
		}

		private MemoryHelperThinggie() : base( TimeSpan.Zero, Frequency )
		{
			Priority = TimerPriority.OneMinute;
		}

		[System.Runtime.InteropServices.DllImport( "Kernel32" )]
		private static extern uint SetProcessWorkingSetSize( IntPtr hProc, int minSize, int maxSize );

		protected override void OnTick()
		{
			SetProcessWorkingSetSize( System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1 );
		}
	}
}
*/