#region Header
//   Vorspire    _,-'/-'/  Options.cs
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
	public class PvPBattleOptions : PropertyObject
	{
		public PvPBattleOptions()
		{
			Broadcasts = new PvPBattleBroadcasts();
			Locations = new PvPBattleLocations();
			Restrictions = new PvPBattleRestrictions();
			Rewards = new PvPRewards();
			Rules = new PvPBattleRules();
			Sounds = new PvPBattleSounds();
			SuddenDeath = new PvPBattleSuddenDeath();
			Timing = new PvPBattleTiming();
			Weather = new PvPBattleWeather();
		}

		public PvPBattleOptions(GenericReader reader)
			: base(reader)
		{ }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleBroadcasts Broadcasts { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleLocations Locations { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleRestrictions Restrictions { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPRewards Rewards { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleRules Rules { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleSounds Sounds { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleSuddenDeath SuddenDeath { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleTiming Timing { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleWeather Weather { get; set; }

		public override void Clear()
		{
			Broadcasts.Clear();
			Locations.Clear();
			Restrictions.Clear();
			Rewards.Clear();
			Rules.Clear();
			Sounds.Clear();
			SuddenDeath.Clear();
			Timing.Clear();
			Weather.Clear();
		}

		public override void Reset()
		{
			Broadcasts.Reset();
			Locations.Reset();
			Restrictions.Reset();
			Rewards.Reset();
			Rules.Reset();
			Sounds.Reset();
			SuddenDeath.Reset();
			Timing.Reset();
			Weather.Reset();
		}

		public override string ToString()
		{
			return "Advanced Options";
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
								Broadcasts,
								t =>
								{
									if (t != null)
									{
										Broadcasts.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								Locations,
								t =>
								{
									if (t != null)
									{
										Locations.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								Restrictions,
								t =>
								{
									if (t != null)
									{
										Restrictions.Serialize(writer);
									}
								}));

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

						writer.WriteBlock(
							() => writer.WriteType(
								Rules,
								t =>
								{
									if (t != null)
									{
										Rules.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								Sounds,
								t =>
								{
									if (t != null)
									{
										Sounds.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								SuddenDeath,
								t =>
								{
									if (t != null)
									{
										SuddenDeath.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								Timing,
								t =>
								{
									if (t != null)
									{
										Timing.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								Weather,
								t =>
								{
									if (t != null)
									{
										Weather.Serialize(writer);
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
						reader.ReadBlock(
							() => { Broadcasts = reader.ReadTypeCreate<PvPBattleBroadcasts>(reader) ?? new PvPBattleBroadcasts(reader); });

						reader.ReadBlock(
							() => { Locations = reader.ReadTypeCreate<PvPBattleLocations>(reader) ?? new PvPBattleLocations(reader); });

						reader.ReadBlock(
							() => { Restrictions = reader.ReadTypeCreate<PvPBattleRestrictions>(reader) ?? new PvPBattleRestrictions(reader); });

						reader.ReadBlock(() => { Rewards = reader.ReadTypeCreate<PvPRewards>(reader) ?? new PvPRewards(reader); });

						reader.ReadBlock(() => { Rules = reader.ReadTypeCreate<PvPBattleRules>(reader) ?? new PvPBattleRules(reader); });

						reader.ReadBlock(() => { Sounds = reader.ReadTypeCreate<PvPBattleSounds>(reader) ?? new PvPBattleSounds(reader); });

						reader.ReadBlock(
							() => { SuddenDeath = reader.ReadTypeCreate<PvPBattleSuddenDeath>(reader) ?? new PvPBattleSuddenDeath(reader); });

						reader.ReadBlock(() => { Timing = reader.ReadTypeCreate<PvPBattleTiming>(reader) ?? new PvPBattleTiming(reader); });

						reader.ReadBlock(() => { Weather = reader.ReadTypeCreate<PvPBattleWeather>(reader) ?? new PvPBattleWeather(reader); });
					}
					break;
			}
		}
	}
}