#region Header
//   Vorspire    _,-'/-'/  Rewards.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using Server;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPRewards : PropertyObject
	{
		public PvPRewards()
		{
			Loser = new PvPReward();
			Winner = new PvPReward();
		}

		public PvPRewards(GenericReader reader)
			: base(reader)
		{ }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPReward Winner { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPReward Loser { get; set; }

		public override string ToString()
		{
			return "Rewards";
		}

		public override void Clear()
		{
			Winner.Clear();
			Loser.Clear();
		}

		public override void Reset()
		{
			Winner.Reset();
			Loser.Reset();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.WriteBlock(
							() => writer.WriteType(
								Loser,
								t =>
								{
									if (t != null)
									{
										Loser.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								Winner,
								t =>
								{
									if (t != null)
									{
										Winner.Serialize(writer);
									}
								}));
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
						reader.ReadBlock(() => { Loser = reader.ReadTypeCreate<PvPReward>(reader) ?? new PvPReward(reader); });

						reader.ReadBlock(() => { Winner = reader.ReadTypeCreate<PvPReward>(reader) ?? new PvPReward(reader); });
					}
					break;
			}
		}
	}
}