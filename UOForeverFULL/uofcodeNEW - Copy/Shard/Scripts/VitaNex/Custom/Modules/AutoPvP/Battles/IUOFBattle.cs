using System.Collections.Generic;

using Server;
using Server.Mobiles;

namespace VitaNex.Modules.AutoPvP.Battles
{
	public interface IUOFBattle : IPvPBattle
	{
		//Dictionary<PlayerMobile, PvPProfileHistoryEntry> BattleStats { get; }
		Dictionary<PlayerMobile, PvPTeam> TeamStats { get; }

		PvPTeam[] Winners { get; }

		[CommandProperty(AutoPvP.Access)]
		bool UseTemplates { get; set; }

		[CommandProperty(AutoPvP.Access)]
		bool UseTemplateEquipment { get; set; }

		[CommandProperty(AutoPvP.Access)]
		bool NoConsume { get; set; }

        [CommandProperty(AutoPvP.Access)]
        bool PlayerStarted { get; set; }

        [CommandProperty(AutoPvP.Access)]
        bool GiveTrophies { get; set; }
	}
}