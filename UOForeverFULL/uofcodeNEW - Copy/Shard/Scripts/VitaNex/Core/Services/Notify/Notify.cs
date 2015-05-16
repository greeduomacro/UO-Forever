using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Server;
using Server.Commands;
using Server.Engines.CentralGump;
using Server.Mobiles;
using Server.Network;

using VitaNex.IO;

namespace VitaNex.Notify
{
	public static partial class Notify
	{
		public static CoreServiceOptions CSOptions { get; private set; }

		public static Type[] GumpTypes { get; private set; }
		public static Type[] WorldGumpSubTypes { get; private set; }

		public static int WorldGumpSubIndex { get; private set; }

		public static BinaryDataStore<Type, NotifySettings> Settings { get; private set; }

		[Usage("Notify <text | html | bbc>")]
		[Description("Send a global notification gump to all online clients, " + //
					 "containing a message parsed from HTML, BBS or plain text.")]
		private static void HandleNotify(CommandEventArgs e)
		{
			if (ValidateCommand(e))
			{
				Broadcast(e.Mobile, e.ArgString, false, true);
			}
		}

		[Usage("NotifyAC <text | html | bbc>")]
		[Description("Send a global notification gump to all online clients, " + //
					 "containing a message parsed from HTML, BBS or plain text, " + //
					 "which auto-closes after 10 seconds.")]
		private static void HandleNotifyAC(CommandEventArgs e)
		{
			if (ValidateCommand(e))
			{
				Broadcast(e.Mobile, e.ArgString, true, true);
			}
		}

		[Usage("NotifyNA <text | html | bbc>")]
		[Description("Send a global notification gump to all online clients, " + //
					 "containing a message parsed from HTML, BBS or plain text, " + //
					 "which has no animation delay.")]
		private static void HandleNotifyNA(CommandEventArgs e)
		{
			if (ValidateCommand(e))
			{
				Broadcast(e.Mobile, e.ArgString, false, false);
			}
		}

		[Usage("NotifyACNA <text | html | bbc>")]
		[Description("Send a global notification gump to all online clients, " + //
					 "containing a message parsed from HTML, BBS or plain text, " + //
					 "which auto-closes after 10 seconds and has no animation delay.")]
		private static void HandleNotifyACNA(CommandEventArgs e)
		{
			if (ValidateCommand(e))
			{
				Broadcast(e.Mobile, e.ArgString, true, false);
			}
		}

		private static bool ValidateCommand(CommandEventArgs e)
		{
			if (e == null || e.Mobile == null)
			{
				return false;
			}

			if (String.IsNullOrWhiteSpace(e.ArgString) ||
			   String.IsNullOrWhiteSpace(Regex.Replace(e.ArgString, @"<[^>]*>", String.Empty)))
			{
				e.Mobile.SendMessage(0x22, "Html/BBC message must be at least 1 character and not all white-space after parsing.");
				e.Mobile.SendMessage(0x22, "Usage: {0}{1} <text | html | bbc>", CommandSystem.Prefix, e.Command);
				return false;
			}

			return true;
		}

		public static NotifySettings EnsureSettings<TGump>() where TGump : NotifyGump
		{
			return EnsureSettings(typeof(TGump));
		}

		public static NotifySettings EnsureSettings(Type t)
		{
			if (t == null || !t.IsEqualOrChildOf<NotifyGump>())
			{
				return null;
			}

			NotifySettings settings;
			bool init = false;

			if (!Settings.TryGetValue(t, out settings))
			{
				Settings.Add(t, settings = new NotifySettings(t));
				init = true;
			}
			else if (settings == null)
			{
				Settings[t] = settings = new NotifySettings(t);
				init = true;
			}

			if (init)
			{
				var m = t.GetMethod("InitSettings", BindingFlags.Static | BindingFlags.NonPublic);

				if (m != null)
				{
					m.Invoke(null, new object[] { settings });
				}
			}

			return settings;
		}

		public static bool IsIgnored<TGump>(PlayerMobile pm) where TGump : NotifyGump
		{
			return IsIgnored(typeof(TGump), pm);
		}

		public static bool IsIgnored(Type t, PlayerMobile pm)
		{
			NotifySettings settings;

			return Settings.TryGetValue(t, out settings) && settings != null && settings.IsIgnored(pm);
		}

		public static bool IsAnimated<TGump>(PlayerMobile pm) where TGump : NotifyGump
		{
			return IsAnimated(typeof(TGump), pm);
		}

		public static bool IsAnimated(Type t, PlayerMobile pm)
		{
			NotifySettings settings;

			return !Settings.TryGetValue(t, out settings) || settings == null || settings.IsAnimated(pm);
		}

		public static void Broadcast(Mobile m, string message)
		{
			Broadcast(m, message, true);
		}

		public static void Broadcast(Mobile m, string message, bool autoClose)
		{
			Broadcast(m, message, autoClose, true);
		}

		public static void Broadcast(Mobile m, string message, bool autoClose, bool animate)
		{
			if (m != null && !m.Deleted && m is PlayerMobile)
			{
				Broadcast(String.Format("{0}:\n{1}", m.RawName, message), autoClose, animate);
			}
		}

		public static void Broadcast(string message)
		{
			Broadcast(message, false);
		}

		public static void Broadcast(string message, bool autoClose)
		{
			Broadcast(message, autoClose, false);
		}

		public static void Broadcast(string message, bool autoClose, bool animate)
		{
			VitaNexCore.TryCatch(
				() =>
				{
					var type = WorldGumpSubTypes[WorldGumpSubIndex++];

					if (type == null)
					{
						return;
					}

					message = String.Format("[{0}]:\n{1}", DateTime.Now.ToSimpleString("t@h:m@"), message);

					NetState.Instances.AsParallel()
                            .Where(ns => ns != null && ns.Mobile != null && !ns.Mobile.Deleted && ns.Mobile is PlayerMobile && !CentralGump.EnsureProfile(ns.Mobile as PlayerMobile).IgnoreNotifies)
							.Select(ns => type.CreateInstanceSafe<WorldNotifyGump>((PlayerMobile)ns.Mobile, message, autoClose))
							.Where(g => g != null)
							.ForEach(
								g =>
								{
									if (!animate)
									{
										g.AnimDuration = TimeSpan.Zero;
									}

									g.Send();
								});
				});

			if (WorldGumpSubIndex >= WorldGumpSubTypes.Length)
			{
				WorldGumpSubIndex = 0;
			}
		}

		public static void Broadcast<TGump>(
			string html,
			bool autoClose = true,
			double delay = 1.0,
			double pause = 3.0,
			Color? color = null,
			Action<NotifyGump> beforeSend = null,
			Action<NotifyGump> afterSend = null) where TGump : NotifyGump
		{
			VitaNexCore.TryCatch(
				() =>
				NetState.Instances.AsParallel()
						.Where(ns => ns != null && ns.Mobile != null && !ns.Mobile.Deleted && ns.Mobile is PlayerMobile)
						.Select(ns => (PlayerMobile)ns.Mobile)
						.ForEach(pm => Send<TGump>(pm, html, autoClose, delay, pause, color, beforeSend, afterSend)));
		}

		public static void Send(
			PlayerMobile pm,
			string html,
			bool autoClose = true,
			double delay = 1.0,
			double pause = 3.0,
			Color? color = null,
			Action<NotifyGump> beforeSend = null,
			Action<NotifyGump> afterSend = null)
		{
			Send<NotifyGump>(pm, html, autoClose, delay, pause, color, beforeSend, afterSend);
		}

		public static void Send<TGump>(
			PlayerMobile pm,
			string html,
			bool autoClose = true,
			double delay = 1.0,
			double pause = 3.0,
			Color? color = null,
			Action<TGump> beforeSend = null,
			Action<TGump> afterSend = null) where TGump : NotifyGump
		{
			if (!pm.IsOnline())
			{
				return;
			}

			if (IsIgnored<TGump>(pm))
			{
				return;
			}

			if (!IsAnimated<TGump>(pm))
			{
				delay = 0.0;
			}

			var ng = typeof(TGump).CreateInstanceSafe<TGump>(pm, html);

            if (ng != null && !CentralGump.EnsureProfile(pm).IgnoreNotifies)
			{
				ng.AutoClose = autoClose;
				ng.AnimDuration = TimeSpan.FromSeconds(Math.Max(0, delay));
				ng.PauseDuration = TimeSpan.FromSeconds(Math.Max(0, pause));
				ng.HtmlColor = color ?? Color.White;

				if (beforeSend != null)
				{
					beforeSend(ng);
				}

				ng.Send();

				if (afterSend != null)
				{
					afterSend(ng);
				}

				return;
			}

			html.Split(new[] {"\n", "<br>", "<BR>"}, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => Regex.Replace(s, @"<[^>]*>", String.Empty))
				.ForEach(pm.SendMessage);
		}
	}
}