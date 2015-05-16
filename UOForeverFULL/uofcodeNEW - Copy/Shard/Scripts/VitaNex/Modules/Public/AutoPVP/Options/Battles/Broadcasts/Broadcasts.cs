#region Header
//   Vorspire    _,-'/-'/  Broadcasts.cs
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
	public class PvPBattleBroadcasts : PropertyObject
	{
		public PvPBattleBroadcasts()
		{
			Local = new PvPBattleLocalBroadcasts();
			World = new PvPBattleWorldBroadcasts();
		}

		public PvPBattleBroadcasts(GenericReader reader)
			: base(reader)
		{ }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleLocalBroadcasts Local { get; protected set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleWorldBroadcasts World { get; protected set; }

		public override string ToString()
		{
			return "Battle Broadcasts";
		}

		public override void Clear()
		{
			Local.Clear();
			World.Clear();
		}

		public override void Reset()
		{
			Local.Reset();
			World.Reset();
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
								Local,
								t =>
								{
									if (t != null)
									{
										Local.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								World,
								t =>
								{
									if (t != null)
									{
										World.Serialize(writer);
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
							() => { Local = reader.ReadTypeCreate<PvPBattleLocalBroadcasts>(reader) ?? new PvPBattleLocalBroadcasts(); });

						reader.ReadBlock(
							() => { World = reader.ReadTypeCreate<PvPBattleWorldBroadcasts>(reader) ?? new PvPBattleWorldBroadcasts(); });
					}
					break;
			}
		}
	}
}