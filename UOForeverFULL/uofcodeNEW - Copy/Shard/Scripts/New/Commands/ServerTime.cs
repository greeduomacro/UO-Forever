#region References
using System;
/*
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps.UI;
*/
#endregion

namespace Server.Commands
{
	public enum ServerTimeFormat
	{
		/// <summary>
		/// HH:MM 
		/// {00:00}
		/// </summary>
		ShortTimeNow,

		/// <summary>
		/// DD-MM-YYYY 
		/// {03-03-2014}
		/// </summary>
		ShortDateNow,

		/// <summary>
		/// HH:MM DD-MM-YYYY  
		/// {00:00 03-03-2014}
		/// </summary>
		ShortDateTimeNow,

		/// <summary>
		/// HH:MM:SS TZO
		/// {00:00:00 EST}
		/// </summary>
		LongTimeNow,

		/// <summary>
		/// Day DD Month YYYY 
		/// {Monday 03 March 2014}
		/// </summary>
		LongDateNow,

		/// <summary>
		/// HH:MM:SS TZO Day DD Month YYYY 
		/// {00:00:00 EST Monday 03 March 2014}
		/// </summary>
		LongDateTimeNow,

		/// <summary>
		/// HH:MM 
		/// {00:00}
		/// </summary>
		ShortTimeUtcNow,

		/// <summary>
		/// DD-MM-YYYY 
		/// {03-03-2014}
		/// </summary>
		ShortDateUtcNow,

		/// <summary>
		/// HH:MM DD-MM-YYYY  
		/// {00:00 03-03-2014}
		/// </summary>
		ShortDateTimeUtcNow,

		/// <summary>
		/// HH:MM:SS UTC 
		/// {00:00:00 UTC}
		/// </summary>
		LongTimeUtcNow,

		/// <summary>
		/// Day DD Month YYYY 
		/// {Monday 03 March 2014}
		/// </summary>
		LongDateUtcNow,

		/// <summary>
		/// HH:MM:SS UTC Day DD Month YYYY 
		/// {00:00:00 UTC Monday 03 March 2014}
		/// </summary>
		LongDateTimeUtcNow
	}

	public static class ServerTime
	{
		/// <summary>
		/// The default formatting used when handling ServerTime display.
		/// </summary>
		public static ServerTimeFormat DisplayFormat = ServerTimeFormat.LongDateTimeNow;

		/// <summary>
		/// Gets a System.DateTime object that is set to the current date and time on this computer, 
		/// expressed as the local time.
		/// </summary>
		public static DateTime Now { get { return DateTime.Now; } }

		/// <summary>
		/// Gets a System.DateTime object that is set to the current date and time on this computer, 
		/// expressed as the Coordinated Universal Time (UTC).
		/// </summary>
		public static DateTime UtcNow { get { return DateTime.UtcNow; } }

		public static string Global { get { return GetString(DisplayFormat); } }

		#region Now

		#region Short
		/// <summary>
		/// HH:MM 
		/// {00:00}
		/// </summary>
		public static string ShortTimeNow { get { return Now.ToSimpleString("t@h:m@"); } }

		/// <summary>
		/// DD-MM-YYYY 
		/// {03-03-2014}
		/// </summary>
		public static string ShortDateNow { get { return Now.ToSimpleString("d-m-y"); } }

		/// <summary>
		/// HH:MM DD-MM-YYYY  
		/// {00:00 03-03-2014}
		/// </summary>
		public static string ShortDateTimeNow { get { return Now.ToSimpleString("t@h:m@ d-m-y"); } }
		#endregion

		#region Long
		/// <summary>
		/// HH:MM:SS TZO
		/// {00:00:00 EST}
		/// </summary>
		public static string LongTimeNow { get { return Now.ToSimpleString("t@h:m:s@ Z"); } }

		/// <summary>
		/// Day DD Month YYYY 
		/// {Monday 03 March 2014}
		/// </summary>
		public static string LongDateNow { get { return Now.ToSimpleString("D d M y"); } }

		/// <summary>
		/// HH:MM:SS TZO Day DD Month YYYY 
		/// {00:00:00 EST Monday 03 March 2014}
		/// </summary>
		public static string LongDateTimeNow { get { return Now.ToSimpleString("t@h:m:s@ Z D d M y"); } }
		#endregion

		#endregion

		#region UtcNow

		#region Short
		/// <summary>
		/// HH:MM 
		/// {00:00}
		/// </summary>
		public static string ShortTimeUtcNow { get { return UtcNow.ToSimpleString("h:m"); } }

		/// <summary>
		/// DD-MM-YYYY 
		/// {03-03-2014}
		/// </summary>
		public static string ShortDateUtcNow { get { return UtcNow.ToSimpleString("d-m-y"); } }

		/// <summary>
		/// HH:MM DD-MM-YYYY  
		/// {00:00 03-03-2014}
		/// </summary>
		public static string ShortDateTimeUtcNow { get { return UtcNow.ToSimpleString("t@h:m@ d-m-y"); } }
		#endregion

		#region Long
		/// <summary>
		/// HH:MM:SS UTC 
		/// {00:00:00 UTC}
		/// </summary>
		public static string LongTimeUtcNow { get { return UtcNow.ToSimpleString("t@h:m:s@ Z"); } }

		/// <summary>
		/// Day DD Month YYYY 
		/// {Monday 03 March 2014}
		/// </summary>
		public static string LongDateUtcNow { get { return UtcNow.ToSimpleString("D d M y"); } }

		/// <summary>
		/// HH:MM:SS UTC Day DD Month YYYY 
		/// {00:00:00 UTC Monday 03 March 2014}
		/// </summary>
		public static string LongDateTimeUtcNow { get { return UtcNow.ToSimpleString("t@h:m:s@ Z D d M y"); } }
		#endregion

		#endregion

		public static void Configure()
		{
			CommandSystem.Register("Now", AccessLevel.Player, OnCommand);
			CommandSystem.Register("Time", AccessLevel.Player, OnCommand);
			CommandSystem.Register("ServerTime", AccessLevel.Player, OnCommand);
		}

		[Usage("Now | Time | ServerTime")]
		[Description("Displays the current server time.")]
		public static void OnCommand(CommandEventArgs e)
		{
			if (e.Mobile != null)
			{
				e.Mobile.SendMessage(Global);
			}
		}

		public static string GetString(this ServerTimeFormat format)
		{
			switch (format)
			{
				case ServerTimeFormat.ShortTimeNow:
					return ShortTimeNow;
				case ServerTimeFormat.ShortDateNow:
					return ShortDateNow;
				case ServerTimeFormat.ShortDateTimeNow:
					return ShortDateTimeNow;

				case ServerTimeFormat.LongTimeNow:
					return LongTimeNow;
				case ServerTimeFormat.LongDateNow:
					return LongDateNow;
				case ServerTimeFormat.LongDateTimeNow:
					return LongDateTimeNow;

				case ServerTimeFormat.ShortTimeUtcNow:
					return ShortTimeUtcNow;
				case ServerTimeFormat.ShortDateUtcNow:
					return ShortDateUtcNow;
				case ServerTimeFormat.ShortDateTimeUtcNow:
					return ShortDateTimeUtcNow;

				case ServerTimeFormat.LongTimeUtcNow:
					return LongTimeUtcNow;
				case ServerTimeFormat.LongDateUtcNow:
					return LongDateUtcNow;
				case ServerTimeFormat.LongDateTimeUtcNow:
					return LongDateTimeUtcNow;

				default:
					return LongDateTimeNow;
			}
		}
	}

	/*
	public class ServerTimeGump : DigitalNumericDisplayGump
	{
		public ServerTimeGump(PlayerMobile user, Gump parent = null)
			: base(user, parent, null, null, 30, 60, 10004, 30073)
		{
			CanClose = true;
			CanDispose = true;
			CanResize = true;
			CanMove = true;

			AutoRefresh = true;
			AutoRefreshRate = TimeSpan.FromSeconds(1.0);
		}

		protected override void Compile()
		{
			var n = DateTime.Now.ToSimpleString("t@hm@");

			Numerics = new int[n.Length];
			Numerics.SetAll(i => n[i]);

			base.Compile();
		}
	}
	*/
}