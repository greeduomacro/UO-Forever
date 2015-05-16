#region Header
//   Vorspire    _,-'/-'/  Battle_Serialize.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;

using Server;
using Server.Items;

using VitaNex.Crypto;
using VitaNex.Schedules;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public sealed class PvPSerial : CryptoHashCode
	{
		public static CryptoHashType Algorithm = CryptoHashType.MD5;

		[CommandProperty(AutoPvP.Access)]
		public override string Seed { get { return base.Seed; } }

		[CommandProperty(AutoPvP.Access)]
		public override string Value { get { return base.Value.Replace("-", String.Empty); } }

		public PvPSerial()
			: this(TimeStamp.UtcNow.ToString() + '+' + Utility.RandomDouble())
		{ }

		public PvPSerial(string seed)
			: base(Algorithm, seed)
		{ }

		public PvPSerial(GenericReader reader)
			: base(reader)
		{ }
	}

	public abstract partial class PvPBattle
	{
		[CommandProperty(AutoPvP.Access, true)]
		public PvPSerial Serial { get; private set; }

		protected bool Deserialized { get; private set; }
		protected bool Deserializing { get; private set; }

		private PvPBattle(bool deserializing)
		{
			Deserialized = deserializing;

			EnsureConstructDefaults();
		}

		public PvPBattle(GenericReader reader)
			: this(true)
		{
			Deserializing = true;

			Deserialize(reader);

			Deserializing = false;
		}

		public virtual void Serialize(GenericWriter writer)
		{
			int version = writer.SetVersion(7);

			if (version > 5)
			{
				writer.WriteBlock(
					() =>
					{
						if (version > 6)
						{
							Serial.Serialize(writer);
						}
						else
						{
							writer.WriteType(
								Serial,
								t =>
								{
									if (t != null)
									{
										Serial.Serialize(writer);
									}
								});
						}
					});
			}

			switch (version)
			{
				case 7:
				case 6:
				case 5:
					writer.Write(Hidden);
					goto case 4;
				case 4:
					writer.Write(FloorItemDelete);
					goto case 3;
				case 3:
				case 2:
					writer.Write(Gate);
					goto case 1;
				case 1:
					{
						writer.Write(Category);
						writer.Write(Ranked);
						writer.Write(InviteWhileRunning);
					}
					goto case 0;
				case 0:
					{
						if (version < 6)
						{
							writer.WriteBlock(
								() => writer.WriteType(
									Serial,
									t =>
									{
										if (t != null)
										{
											Serial.Serialize(writer);
										}
									}));
						}

						writer.Write(DebugMode);
						writer.WriteFlag(State);
						writer.Write(Name);
						writer.Write(Description);
						writer.Write(AutoAssign);
						writer.Write(UseTeamColors);
						writer.Write(IgnoreCapacity);
						writer.Write(SubCommandPrefix);
						writer.Write(QueueAllowed);
						writer.Write(SpectateAllowed);
						writer.Write(KillPoints);
						writer.Write(PointsBase);
						writer.Write(PointsRankFactor);
						writer.Write(IdleKick);
						writer.Write(IdleThreshold);
						writer.WriteFlag(LastState);
						writer.Write(LastStateChange);
						writer.Write(LightLevel);
						writer.Write(LogoutDelay);
						writer.WriteItemList(Doors, true);

						writer.WriteBlock(
							() => writer.WriteType(
								Options,
								t =>
								{
									if (t != null)
									{
										Options.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								Schedule,
								t =>
								{
									if (t != null)
									{
										Schedule.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								BattleRegion,
								t =>
								{
									if (t != null)
									{
										BattleRegion.Serialize(writer);
									}
								}));

						writer.WriteBlock(
							() => writer.WriteType(
								SpectateRegion,
								t =>
								{
									if (t != null)
									{
										SpectateRegion.Serialize(writer);
									}
								}));

						writer.WriteBlockList(
							Teams,
							team => writer.WriteType(
								team,
								t =>
								{
									if (t != null)
									{
										team.Serialize(writer);
									}
								}));
					}
					break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			int version = reader.GetVersion();

			if (version > 5)
			{
				reader.ReadBlock(() =>
				{
					if (version > 6)
					{
						Serial = new PvPSerial(reader);
					}
					else
					{
						Serial = reader.ReadTypeCreate<PvPSerial>(reader) ?? new PvPSerial(reader);
					}
				});
			}

			switch (version)
			{
				case 7:
				case 6:
				case 5:
					Hidden = reader.ReadBool();
					goto case 4;
				case 4:
					FloorItemDelete = reader.ReadBool();
					goto case 3;
				case 3:
				case 2:
					{
						Gate = reader.ReadItem<PvPSpectatorGate>();

						if (Gate != null)
						{
							Gate.Battle = this;
						}
					}
					goto case 1;
				case 1:
					{
						Category = reader.ReadString();
						Ranked = reader.ReadBool();
						InviteWhileRunning = reader.ReadBool();
					}
					goto case 0;
				case 0:
					{
						if (version < 6)
						{
							reader.ReadBlock(() => Serial = reader.ReadTypeCreate<PvPSerial>(reader) ?? new PvPSerial());
						}

						DebugMode = reader.ReadBool();
						State = reader.ReadFlag<PvPBattleState>();
						Name = reader.ReadString();
						Description = reader.ReadString();
						AutoAssign = reader.ReadBool();
						UseTeamColors = reader.ReadBool();
						IgnoreCapacity = reader.ReadBool();
						SubCommandPrefix = reader.ReadChar();
						QueueAllowed = reader.ReadBool();
						SpectateAllowed = reader.ReadBool();
						KillPoints = version < 3 ? (reader.ReadBool() ? 1 : 0) : reader.ReadInt();
						PointsBase = reader.ReadInt();
						PointsRankFactor = reader.ReadDouble();
						IdleKick = reader.ReadBool();
						IdleThreshold = reader.ReadTimeSpan();
						LastState = reader.ReadFlag<PvPBattleState>();
						LastStateChange = reader.ReadDateTime();
						LightLevel = reader.ReadInt();
						LogoutDelay = reader.ReadTimeSpan();

						Doors.AddRange(reader.ReadStrongItemList<BaseDoor>());

						reader.ReadBlock(() => Options = reader.ReadTypeCreate<PvPBattleOptions>(reader) ?? new PvPBattleOptions());

						if (Schedule != null && Schedule.Running)
						{
							Schedule.Stop();
						}

						reader.ReadBlock(
							() => Schedule = reader.ReadTypeCreate<Schedule>(reader) ?? new Schedule("Battle " + Serial.Value, false));

						reader.ReadBlock(
							() => BattleRegion = reader.ReadTypeCreate<PvPBattleRegion>(this, reader) ?? new PvPBattleRegion(this));

						reader.ReadBlock(
							() => SpectateRegion = reader.ReadTypeCreate<PvPSpectateRegion>(this, reader) ?? new PvPSpectateRegion(this));

						reader.ReadBlockList(() => reader.ReadTypeCreate<PvPTeam>(this, reader) ?? new PvPTeam(this), Teams);
					}
					break;
			}
		}
	}
}