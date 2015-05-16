#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Gumps;
using Server.Items;
using Server.Mobiles;

using VitaNex;
using VitaNex.IO;
using VitaNex.Modules.AutoPvP;
using VitaNex.Notify;
#endregion

namespace Server.Twitch
{
	public static partial class ActionCams
	{
		static ActionCams()
		{
			_PersistenceFile = IOUtility.EnsureFile(VitaNexCore.SavesDirectory + "/ActionCams/Cameras.bin");

			#region Non-Persisting Variables
			CurrentlyViewing = new Dictionary<PlayerMobile, PlayerMobile>();

			DeathCams = new Dictionary<PlayerMobile, DateTime>();
			DeathCamsEvents = new Dictionary<PlayerMobile, DateTime>();
			#endregion

			PlayerMurderers = new Dictionary<PlayerMobile, int>();
			MonsterMurderers = new Dictionary<BaseCreature, int>();

			GumpWhitelist = new List<Type> {
				typeof(ActionCamUI),
				typeof(BattleResultsGump),
				typeof(NotifyGump),
				typeof(PropertiesGump),
				typeof(WhoGump),
				typeof(ScoutGump)
			};

			RegionBlackList = new List<string>
			{
				"Jail",
				"Trinsic-Delucia Passage"
			};
		}

		public static void Configure()
		{
			EventSink.WorldLoad += Load;
			EventSink.WorldSave += e => Save();

			EventSink.PlayerDeath += OnPlayerDeath;

			// OnNotify event invoked in NotifyGump.OnBeforeSend :)
			NotifyGump.OnNotify += OnNotify;

			CommandUtility.Register("DCjoin", AccessLevel.Seer, e => JoinDeathCam(e.Mobile as PlayerMobile));
			CommandUtility.Register("DCquit", AccessLevel.Seer, e => QuitDeathCam(e.Mobile as PlayerMobile));

			CommandUtility.Register("DCbattle", AccessLevel.Seer, e => JoinDeathCamEvent(e.Mobile as PlayerMobile));

            CommandUtility.Register("DCClearStats", AccessLevel.Seer, e => DeathCamClearStats(e.Mobile as PlayerMobile));
		}

		public static void Save()
		{
			Defragment();

			_PersistenceFile.Serialize(SerializeCams);
		}

		public static void Load()
		{
			_PersistenceFile.Deserialize(DeserializeCams);

			Defragment();
		}

		private static void SerializeCams(GenericWriter writer)
		{
			int version = writer.SetVersion(3);

			switch (version)
			{
				case 3: // Skip to case 1
					goto case 1;
                case 2:
			        {
                        writer.WriteBlockDictionary(
                            CurrentlyViewing,
							(cam, viewed) =>
                            {
								writer.Write(cam);
                                writer.Write(viewed);
                            });			        
			        }
			        goto case 1;
				case 1:
					{
						writer.WriteBlockDictionary(
							PlayerMurderers,
							(player, count) =>
							{
								writer.Write(player);
								writer.Write(count);
							});

						writer.WriteBlockDictionary(
							MonsterMurderers,
							(mob, count) =>
							{
								writer.Write(mob);
								writer.Write(count);
							});

						// Version 0 -> 1 : int -> ulong
						writer.Write(CurrentDeathCount);
						writer.Write(CurrentPlayerMurders);
						writer.Write(CurrentMonsterMurders);
					}
					goto case 0;
				case 0:
					{
						if (version < 1)
						{
							writer.Write(Convert.ToInt32(CurrentDeathCount));
							writer.Write(Convert.ToInt32(CurrentPlayerMurders));
							writer.Write(Convert.ToInt32(CurrentMonsterMurders));
						}

						writer.Write(TopPlayerMurderer);
						writer.Write(TopMonsterMurderer);

						if (version < 1)
						{
							writer.WriteBlockDictionary(
								DeathCams,
								(player, date) =>
								{
									writer.Write(player);
									writer.Write(date);
								});

							writer.WriteBlockDictionary(
								DeathCamsEvents,
								(player, date) =>
								{
									writer.Write(player);
									writer.Write(date);
								});
						}
					}
					break;
			}
		}

		private static void DeserializeCams(GenericReader reader)
		{
			int version = reader.GetVersion();

			switch (version)
			{
				case 3: // Skip to case 1
					goto case 1;
                case 2:
			        {
                        reader.ReadBlockDictionary(
                            () =>
                            {
                                var cam = reader.ReadMobile<PlayerMobile>();
                                var viewed = reader.ReadMobile<PlayerMobile>();

								return new KeyValuePair<PlayerMobile, PlayerMobile>(cam, viewed);
                            },
                            CurrentlyViewing);			        
			        }
			        goto case 1;
				case 1:
					{
						reader.ReadBlockDictionary(
							() =>
							{
								var player = reader.ReadMobile<PlayerMobile>();
								int count = reader.ReadInt();

								return new KeyValuePair<PlayerMobile, int>(player, count);
							},
							PlayerMurderers);

						reader.ReadBlockDictionary(
							() =>
							{
								BaseCreature mob = reader.ReadMobile<BaseCreature>();
								int count = reader.ReadInt();

								return new KeyValuePair<BaseCreature, int>(mob, count);
							},
							MonsterMurderers);

						CurrentDeathCount = reader.ReadULong();
						CurrentPlayerMurders = reader.ReadULong();
						CurrentMonsterMurders = reader.ReadULong();
					}
					goto case 0;
				case 0:
					{
						if (version < 1)
						{
							CurrentDeathCount = Convert.ToUInt64(reader.ReadInt());
							CurrentPlayerMurders = Convert.ToUInt64(reader.ReadInt());
							CurrentMonsterMurders = Convert.ToUInt64(reader.ReadInt());
						}

						TopPlayerMurderer = reader.ReadMobile<PlayerMobile>();
						TopMonsterMurderer = reader.ReadMobile<BaseCreature>();

						if (version < 1)
						{
							reader.ReadBlockDictionary(
								() =>
								{
									var player = reader.ReadMobile<PlayerMobile>();
									DateTime date = reader.ReadDateTime();

									return new KeyValuePair<PlayerMobile, DateTime>(player, date);
								},
								DeathCams);

							reader.ReadBlockDictionary(
								() =>
								{
									var player = reader.ReadMobile<PlayerMobile>();
									DateTime date = reader.ReadDateTime();

									return new KeyValuePair<PlayerMobile, DateTime>(player, date);
								},
								DeathCamsEvents);
						}
					}
					break;
			}
		}
	}
}