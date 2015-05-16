#region Header
//   Vorspire    _,-'/-'/  SystemOpts.cs
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
	[PropertyObject]
	public class AutoPvPOptions : CoreModuleOptions
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPAdvancedOptions Advanced { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPStatistics Statistics { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPExecuteCommands ExecuteCommands { get; set; }

		public AutoPvPOptions()
			: base(typeof(AutoPvP))
		{
			Advanced = new AutoPvPAdvancedOptions();
			Statistics = new AutoPvPStatistics();
			ExecuteCommands = new AutoPvPExecuteCommands();
		}

		public AutoPvPOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			Advanced.Clear();
			Statistics.Clear();
			ExecuteCommands.Clear();
		}

		public override void Reset()
		{
			Advanced.Reset();
			Statistics.Reset();
			ExecuteCommands.Reset();
		}

		public override string ToString()
		{
			return "System Options";
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
								Advanced,
								t =>
								{
									if (t != null)
									{
										Advanced.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								Statistics,
								t =>
								{
									if (t != null)
									{
										Statistics.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								ExecuteCommands,
								t =>
								{
									if (t != null)
									{
										ExecuteCommands.Serialize(writer);
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
							() => { Advanced = reader.ReadTypeCreate<AutoPvPAdvancedOptions>(reader) ?? new AutoPvPAdvancedOptions(reader); });

						reader.ReadBlock(
							() => { Statistics = reader.ReadTypeCreate<AutoPvPStatistics>(reader) ?? new AutoPvPStatistics(reader); });

						reader.ReadBlock(
							() =>
							{ ExecuteCommands = reader.ReadTypeCreate<AutoPvPExecuteCommands>(reader) ?? new AutoPvPExecuteCommands(reader); });
					}
					break;
			}
		}
	}
}