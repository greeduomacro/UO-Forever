#region References
using System;
#endregion

namespace Server.Mobiles
{
	public static partial class ChampionGlobals
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static ChampionGlobalsOptions CSOptions { get; private set; }

		public static bool IsEligible(BaseChampion c, Mobile mob)
		{
			if (mob == null)
			{
				return false;
			}

			if (CSOptions.PowerScrollRequireAlive && !mob.Alive)
			{
				return false;
			}

			if (CSOptions.PowerScrollMinimumDistance > -1)
			{
				if (Math.Abs(mob.X - c.X) > CSOptions.PowerScrollMinimumDistance ||
					Math.Abs(mob.Y - c.Y) > CSOptions.PowerScrollMinimumDistance)
				{
					return false;
				}
			}

			return true;
		}
	}
}