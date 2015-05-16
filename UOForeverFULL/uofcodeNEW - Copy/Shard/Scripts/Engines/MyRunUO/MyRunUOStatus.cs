#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Commands;
using Server.Network;

using VitaNex;
#endregion

namespace Server.Engines.MyRunUO
{
	public class MyRunUOStatus
	{
		public static PollTimer UpdateTimer { get; private set; }

		public static void Initialize()
		{
			UpdateTimer = PollTimer.CreateInstance(Config.Options.StatusUpdateInterval, Begin, () => Config.Options.Enabled);

			CommandSystem.Register("UpdateWebStatus", AccessLevel.Administrator, UpdateWebStatus_OnCommand);
		}

		[Usage("UpdateWebStatus")]
		[Description("Starts the process of updating the MyRunUO online status database.")]
		public static void UpdateWebStatus_OnCommand(CommandEventArgs e)
		{
			if (!Config.Options.Enabled)
			{
				return;
			}

			if (m_Command == null || m_Command.HasCompleted)
			{
				Begin();
				e.Mobile.SendMessage("Web status update process has been started.");
			}
			else
			{
				e.Mobile.SendMessage("Web status database is already being updated.");
			}
		}

		private static DatabaseCommandQueue m_Command;

		public static void Begin()
		{
			if (m_Command != null && !m_Command.HasCompleted)
			{
				return;
			}

			Console.WriteLine("MyRunUO: Updating status database");

			try
			{
				m_Command = new DatabaseCommandQueue(
					"MyRunUO: Status database updated in {0:F1} seconds", "MyRunUO Status Database Thread");

				m_Command.Enqueue("DELETE FROM myrunuo_status");

				List<NetState> online = NetState.Instances;

				foreach (Mobile mob in online.Select(ns => ns.Mobile).Where(mob => mob != null))
				{
					m_Command.Enqueue(String.Format("INSERT INTO myrunuo_status (char_id) VALUES ({0})", mob.Serial.Value));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("MyRunUO: Error updating status database");
				Console.WriteLine(e);
			}

			if (m_Command != null)
			{
				m_Command.Enqueue(null);
			}
		}
	}
}