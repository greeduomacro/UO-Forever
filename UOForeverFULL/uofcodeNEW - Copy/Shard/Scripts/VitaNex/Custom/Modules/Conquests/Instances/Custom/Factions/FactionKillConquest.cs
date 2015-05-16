using System;
using Server.Ethics;
using Server.Factions;
using Server.Mobiles;
using VitaNex;
using VitaNex.Modules.AutoPvP;

namespace Server.Engines.Conquests
{
    public class FactionKillConquest : PlayerKillConquest
    {
        [CommandProperty(Conquests.Access)]
        public bool IsEthic { get; set; }

		public FactionKillConquest()
		{ }

        public FactionKillConquest(GenericReader reader)
			: base(reader)
		{ }

        protected override int GetProgress(ConquestState state, PlayerConquestContainer args)
		{
            if (state.User == null)
                return 0;

            if (args == null || args.Mobile == null || args.Killer is PlayerMobile && args.Killer.Account != state.User.Account || !(args.Mobile is PlayerMobile) || !(args.Killer is PlayerMobile))
			{
				return 0;
			}

		    var killer = args.Killer as PlayerMobile;
		    var victim = args.Killer as PlayerMobile;

            Faction killerFaction = Faction.Find(killer, true);
            Faction victimFaction = Faction.Find(victim, true);

            if (killerFaction == null || victimFaction == null || killerFaction == victimFaction || AutoPvP.IsParticipant(victim))
            {
                return 0;
            }

            Player ethickiller = Player.Find(killer);
            Player ethicvictim = Player.Find(victim);

		    if (IsEthic && (ethickiller == null || ethicvictim == null || ethickiller.Ethic == ethicvictim.Ethic))
		    {
		        return 0;
		    }

		    return base.GetProgress(state, args);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

            switch (version)
            {
                case 1:
                    {
                        writer.Write(IsEthic);
                    }
                    goto case 0;
                case 0:
                    {}
                    break;
            }
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

            switch (version)
            {
                case 1:
                {
                    IsEthic = reader.ReadBool();
                }
                goto case 0;
                case 0:
                { }
                break;
            }
		}
	}
}