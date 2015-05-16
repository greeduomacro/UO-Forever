#region Header
//   Vorspire    _,-'/-'/  SeasonOpts.cs
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

using VitaNex.Schedules;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class AutoPvPSeasonOptions : PropertyObject
	{
		public AutoPvPSeasonOptions()
		{
			CurrentSeason = 1;
			TopListCount = 3;
			RunnersUpCount = 7;
			Rewards = new PvPRewards();
		}

		public AutoPvPSeasonOptions(GenericReader reader)
			: base(reader)
		{ }

		[CommandProperty(AutoPvP.Access)]
		public ScheduleInfo ScheduleInfo { get { return AutoPvP.SeasonSchedule.Info; } set { AutoPvP.SeasonSchedule.Info = value; } }

		[CommandProperty(AutoPvP.Access)]
		public virtual int CurrentSeason { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int TopListCount { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int RunnersUpCount { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPRewards Rewards { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int SkipTicks { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int SkippedTicks { get; set; }

		public override void Clear()
		{
			ScheduleInfo.Clear();
			TopListCount = 0;
			RunnersUpCount = 0;
			SkipTicks = 0;
			SkippedTicks = 0;

			Rewards.Clear();
		}

		public override void Reset()
		{
			ScheduleInfo.Clear();
			TopListCount = 3;
			RunnersUpCount = 7;
			SkipTicks = 0;
			SkippedTicks = 0;

			Rewards.Reset();
		}

		public override string ToString()
		{
			return "Season Options";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					{
						writer.Write(SkipTicks);
						writer.Write(SkippedTicks);
					}
					goto case 0;
				case 0:
					{
						writer.WriteBlock(
							() =>
							{
								writer.Write(CurrentSeason);
								writer.Write(TopListCount);
								writer.Write(RunnersUpCount);

								writer.WriteType(
									ScheduleInfo,
									t =>
									{
										if (t != null)
										{
											ScheduleInfo.Serialize(writer);
										}
									});

								writer.Write(AutoPvP.SeasonSchedule.Enabled);
							});

						writer.WriteBlock(
							() => writer.WriteType(
								Rewards,
								t =>
								{
									if (t != null)
									{
										Rewards.Serialize(writer);
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
				case 1:
					{
						SkipTicks = reader.ReadInt();
						SkippedTicks = reader.ReadInt();
					}
					goto case 0;
				case 0:
					{
						reader.ReadBlock(
							() =>
							{
								CurrentSeason = reader.ReadInt();
								TopListCount = reader.ReadInt();
								RunnersUpCount = reader.ReadInt();

								ScheduleInfo = reader.ReadTypeCreate<ScheduleInfo>(reader) ?? new ScheduleInfo(reader);
								AutoPvP.SeasonSchedule.Enabled = reader.ReadBool();
							});

						reader.ReadBlock(() => { Rewards = reader.ReadTypeCreate<PvPRewards>(reader) ?? new PvPRewards(reader); });
					}
					break;
			}
		}
	}
}