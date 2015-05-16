#region References
using VitaNex;
#endregion

namespace Server.Mobiles
{
	[CoreService("Champion Globals", "1.0.0", TaskPriority.Highest)]
	public static partial class ChampionGlobals
	{
		static ChampionGlobals()
		{
			CSOptions = new ChampionGlobalsOptions();
		}
	}
}