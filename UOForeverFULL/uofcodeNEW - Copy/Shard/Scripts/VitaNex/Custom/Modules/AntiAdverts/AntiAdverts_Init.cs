#region References
using System.Collections.Generic;

using Server.Mobiles;

using VitaNex;
using VitaNex.SuperGumps;
#endregion

namespace Server.Misc
{
	[CoreModule("Anti-Adverts", "1.0.1", false)]
	public static partial class AntiAdverts
	{
		static AntiAdverts()
		{
			Reports = new List<AntiAdvertsReport>();
		}

		private static void CMConfig()
		{
			EventSink.Speech += OnSpeech;
		}

		private static void CMEnabled()
		{
			EventSink.Speech += OnSpeech;
		}

		private static void CMDisabled()
		{
			EventSink.Speech -= OnSpeech;
		}

		private static void CMInvoke()
		{
			CommandUtility.Register(
				"AntiAds",
				Access,
				e =>
				{
					if (e.Mobile is PlayerMobile)
					{
						SuperGump.Send(new AntiAvertsReportsGump((PlayerMobile)e.Mobile));
					}
				});
		}

		private static void CMSave()
		{
			VitaNexCore.TryCatch(() => ReportsFile.Serialize(SerializeReports), CMOptions.ToConsole);
		}

		private static void CMLoad()
		{
			VitaNexCore.TryCatch(() => ReportsFile.Deserialize(DeserializeReports), CMOptions.ToConsole);
		}

		private static void CMDisposed()
		{ }

		private static void SerializeReports(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.WriteBlockList(Reports, r => r.Serialize(writer));
		}

		private static void DeserializeReports(GenericReader reader)
		{
			reader.GetVersion();

			reader.ReadBlockList(() => new AntiAdvertsReport(reader), Reports);
		}
	}
}