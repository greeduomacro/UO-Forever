using Server;
using Server.Engines.Conquests;
using Server.Mobiles;

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class UOF_PvPTeam : PvPTeam
	{
		public UOF_PvPTeam(PvPBattle battle, string name = "Incognito", int minCapacity = 1, int maxCapacity = 1, int color = 12)
			: base(battle, name, minCapacity, maxCapacity, color)
		{ }

		public UOF_PvPTeam(PvPBattle battle, GenericReader reader)
			: base(battle, reader)
		{ }

		public override void OnMemberDeath(PlayerMobile pm)
		{
			base.OnMemberDeath(pm);

			var pk = pm.GetLastKiller<PlayerMobile>(true);

			if (pk != null)
			{
				Conquests.CheckProgress<BattleKillConquest>(pk, pm);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}