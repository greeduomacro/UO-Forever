#region Header
//   Vorspire    _,-'/-'/  AdvancedOpts.cs
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
	public class AutoPvPAdvancedOptions : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPCommandOptions Commands { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPProfileOptions Profiles { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPSeasonOptions Seasons { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPMiscOptions Misc { get; set; }

		public AutoPvPAdvancedOptions()
		{
			Commands = new AutoPvPCommandOptions();
			Profiles = new AutoPvPProfileOptions();
			Seasons = new AutoPvPSeasonOptions();
			Misc = new AutoPvPMiscOptions();
		}

		public AutoPvPAdvancedOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			Commands.Clear();
			Profiles.Clear();
			Seasons.Clear();
			Misc.Clear();
		}

		public override void Reset()
		{
			Commands.Reset();
			Profiles.Reset();
			Seasons.Reset();
			Misc.Reset();
		}

		public override string ToString()
		{
			return "Advanced Options";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					{
						writer.WriteBlock(
							() => writer.WriteType(
								Misc,
								t =>
								{
									if (t != null)
									{
										Misc.Serialize(writer);
									}
								}));
					}
					goto case 0;
				case 0:
					{
						writer.WriteBlock(
							() => writer.WriteType(
								Commands,
								t =>
								{
									if (t != null)
									{
										Commands.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								Profiles,
								t =>
								{
									if (t != null)
									{
										Profiles.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								Seasons,
								t =>
								{
									if (t != null)
									{
										Seasons.Serialize(writer);
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
						reader.ReadBlock(() => { Misc = reader.ReadTypeCreate<AutoPvPMiscOptions>(reader) ?? new AutoPvPMiscOptions(reader); });
					}
					goto case 0;
				case 0:
					{
						if (version == 0)
						{
							Misc = new AutoPvPMiscOptions();
						}

						reader.ReadBlock(
							() => { Commands = reader.ReadTypeCreate<AutoPvPCommandOptions>(reader) ?? new AutoPvPCommandOptions(reader); });

						reader.ReadBlock(
							() => { Profiles = reader.ReadTypeCreate<AutoPvPProfileOptions>(reader) ?? new AutoPvPProfileOptions(reader); });

						reader.ReadBlock(
							() => { Seasons = reader.ReadTypeCreate<AutoPvPSeasonOptions>(reader) ?? new AutoPvPSeasonOptions(reader); });
					}
					break;
			}
		}
	}
}