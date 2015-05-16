#region References
using System;

using Server;

using VitaNex.IO;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	[CoreModule("UOFCentral", "1.0.0", true, TaskPriority.High)]
	public static partial class UOFCentral
	{
		static UOFCentral()
		{
			EntryTypes = typeof(Entry).GetConstructableChildren();

			CMOptions = new UOFCentralOptions();

			StateFile = IOUtility.EnsureFile(VitaNexCore.SavesDirectory + "/UOFCentral/State.bin");

			State = new State();
		}

		private static void CMConfig()
		{
			EventSink.Login += OnLogin;
		}

		private static void CMEnabled()
		{
			EventSink.Login += OnLogin;

			if (CMOptions.LoginPopup)
			{
				OpenAll();
			}
		}

		private static void CMDisabled()
		{
			EventSink.Login -= OnLogin;
			CloseAll();
		}

		private static void CMSave()
		{
			VitaNexCore.TryCatch(() => StateFile.Serialize(State.Serialize), CMOptions.ToConsole);
		}

		private static void CMLoad()
		{
			VitaNexCore.TryCatch(() => StateFile.Deserialize(State.Deserialize), CMOptions.ToConsole);
		}
	}
}