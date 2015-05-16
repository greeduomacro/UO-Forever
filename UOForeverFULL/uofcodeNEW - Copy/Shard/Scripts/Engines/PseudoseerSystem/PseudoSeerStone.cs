#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
#endregion

namespace Server.Games
{
	public sealed class PseudoSeerStone : Item
	{
		public static void Initialize()
		{
			if (Instance == null)
			{
				return;
			}

			for (int i = 0; i < PseudoseerUserNamesToReinstate.Count; i++)
			{
				try
				{
					IAccount account = Accounts.GetAccount(PseudoseerUserNamesToReinstate[i]);

					if (account != null)
					{
						Instance.PseudoSeers.Add(account, PseudoseerPermissionsToReinstate[i]);
					}
				}
				catch
				{
					Console.WriteLine("Account unable to be reinstated as pseudoseer: " + PseudoseerUserNamesToReinstate[i]);
				}
			}

			//Mobile.FwdAccessOverride = PseudoSeerStone.Instance.FastWalkDisableMonitoring;
		}

		private static PseudoSeerStone _Instance;
		public static PseudoSeerStone Instance { get { return _Instance; } }

		public static List<string> PseudoseerUserNamesToReinstate = new List<string>();
		public static List<string> PseudoseerPermissionsToReinstate = new List<string>();

		private readonly Dictionary<IAccount, string> _PseudoSeers = new Dictionary<IAccount, string>();
		public Dictionary<IAccount, string> PseudoSeers { get { return _PseudoSeers; } }

		public static int GetDamageLevel(int treasureLevel)
		{
			double magicChance = Utility.RandomDouble();

			if (magicChance < TreasureChestDamageModifierChance)
			{
				double levelChance = Utility.RandomDouble() + (4 - treasureLevel) * TreasureChestVanqChance;

				if (levelChance < TreasureChestVanqChance)
				{
					return 5;
				}

				if (levelChance < TreasureChestPowerChance)
				{
					return 4;
				}

				if (levelChance < TreasureChestForceChance)
				{
					return 3;
				}

				if (levelChance < TreasureChestMightChance)
				{
					return 2;
				}

				return 1;
			}

			return 0;
		}

		public static int GetDurabilityLevel(int treasureLevel)
		{
			double magicChance = Utility.RandomDouble();

			if (magicChance < TreasureChestDurabilityModifierChance)
			{
				double levelChance = Utility.RandomDouble() + (4 - treasureLevel) * TreasureChestIndestructibleChance;

				if (levelChance < TreasureChestIndestructibleChance)
				{
					return 5;
				}

				if (levelChance < TreasureChestFortifiedChance)
				{
					return 4;
				}

				if (levelChance < TreasureChestMassiveChance)
				{
					return 3;
				}

				if (levelChance < TreasureChestSubstantialChance)
				{
					return 2;
				}

				return 1;
			}

			return 0;
		}

		public static int GetAccuracyLevel(int treasureLevel)
		{
			double magicChance = Utility.RandomDouble();

			if (magicChance < TreasureChestAccuracyModifierChance)
			{
				double levelChance = Utility.RandomDouble() + (4 - treasureLevel) * TreasureChestSupremelyChance;

				if (levelChance < TreasureChestSupremelyChance)
				{
					return 5;
				}

				if (levelChance < TreasureChestExceedinglyChance)
				{
					return 4;
				}

				if (levelChance < TreasureChestEminentlyChance)
				{
					return 3;
				}

				if (levelChance < TreasureChestSurpassinglyChance)
				{
					return 2;
				}

				return 1;
			}

			return 0;
		}

		public static bool CreaturesStealthLikePlayers = false;
		public static bool AllowThirdDawnClient = true;
		public static TimeSpan BondingAbandonDelay = TimeSpan.FromHours(24.0);
		public static double HoursBetweenBODs = 6.0;
		public static int MaxBODGoldRewardAllowed = 5000;
		public static double TreasureMapMobSpawnChance = 0.14;
		public static double TreasureChestDamageModifierChance = 0.65;
		public static double TreasureChestVanqChance = 0.07;
		public static double TreasureChestPowerChance = 0.22;
		public static double TreasureChestForceChance = 0.45;
		public static double TreasureChestMightChance = 0.70;
		public static double TreasureChestDurabilityModifierChance = 0.65;
		public static double TreasureChestIndestructibleChance = 0.08;
		public static double TreasureChestFortifiedChance = 0.22;
		public static double TreasureChestMassiveChance = 0.45;
		public static double TreasureChestSubstantialChance = 0.70;
		public static double TreasureChestAccuracyModifierChance = 0.65;
		public static double TreasureChestSupremelyChance = 0.08;
		public static double TreasureChestExceedinglyChance = 0.22;
		public static double TreasureChestEminentlyChance = 0.45;
		public static double TreasureChestSurpassinglyChance = 0.70;
		public static bool AllowRedsInTown = false;
		public static double CompanionMinutesBetweenHelp = 1.0;
		public static double JustMurderedMinutesTracked = 5.0;
		public static double SpeedHackSeriousThreshold = 0.75;
		public static int ParagonChestPlatinumPerLevel = 75;
		public static double MobStatueChance = 0.001;
		public static double ParagonMinChance = 0.05;
		public static double ParagonMaxChance = 0.10;
		public static double ParagonChestChance = 0.10;
		public static double ParagonRevertInHours = 3;
		public static bool AllowCriminalUseGate = false;
		public static bool AllowAllPossessedMobsSpeedBoost = false;
		public static bool AllowPseudoseerMobsSpeedBoost = true;
		public static int PlatinumPerMissedDamageLevel = 5;
		public static bool ReplaceVanqWithSkillScrolls = false;
		public static string OnLoginUberScript = null;
		public static string OnPlayerDeathUberScript = null;
		public static int HighestDamageLevelSpawn = 5; // 5 = Vanq

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool _CreaturesStealthLikePlayers { get { return CreaturesStealthLikePlayers; } set { CreaturesStealthLikePlayers = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool _AllowThirdDawnClient { get { return AllowThirdDawnClient; } set { AllowThirdDawnClient = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public TimeSpan _BondingAbandonDelay { get { return BondingAbandonDelay; } set { BondingAbandonDelay = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _HoursBetweenBODs { get { return HoursBetweenBODs; } set { HoursBetweenBODs = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public int _MaxBODGoldRewardAllowed { get { return MaxBODGoldRewardAllowed; } set { MaxBODGoldRewardAllowed = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureMapMobSpawnChance { get { return TreasureMapMobSpawnChance; } set { TreasureMapMobSpawnChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestDamageModifierChance { get { return TreasureChestDamageModifierChance; } set { TreasureChestDamageModifierChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestVanqChance { get { return TreasureChestVanqChance; } set { TreasureChestVanqChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestPowerChance { get { return TreasureChestPowerChance; } set { TreasureChestPowerChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestForceChance { get { return TreasureChestForceChance; } set { TreasureChestForceChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestMightChance { get { return TreasureChestMightChance; } set { TreasureChestMightChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestDurabilityModifierChance { get { return TreasureChestDurabilityModifierChance; } set { TreasureChestDurabilityModifierChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestIndestructibleChance { get { return TreasureChestIndestructibleChance; } set { TreasureChestIndestructibleChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestFortifiedChance { get { return TreasureChestFortifiedChance; } set { TreasureChestFortifiedChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestMassiveChance { get { return TreasureChestMassiveChance; } set { TreasureChestMassiveChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestSubstantialChance { get { return TreasureChestSubstantialChance; } set { TreasureChestSubstantialChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestAccuracyModifierChance { get { return TreasureChestAccuracyModifierChance; } set { TreasureChestAccuracyModifierChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestSupremelyChance { get { return TreasureChestSupremelyChance; } set { TreasureChestSupremelyChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestExceedinglyChance { get { return TreasureChestExceedinglyChance; } set { TreasureChestExceedinglyChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestEminentlyChance { get { return TreasureChestEminentlyChance; } set { TreasureChestEminentlyChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _TreasureChestSurpassinglyChance { get { return TreasureChestSurpassinglyChance; } set { TreasureChestSurpassinglyChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool _AllowRedsInTown { get { return AllowRedsInTown; } set { AllowRedsInTown = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _CompanionMinutesBetweenHelp { get { return CompanionMinutesBetweenHelp; } set { CompanionMinutesBetweenHelp = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _JustMurderedMinutesTracked { get { return JustMurderedMinutesTracked; } set { JustMurderedMinutesTracked = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _SpeedHackSeriousThreshold { get { return SpeedHackSeriousThreshold; } set { SpeedHackSeriousThreshold = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public int _ParagonChestPlatinumPerLevel { get { return ParagonChestPlatinumPerLevel; } set { ParagonChestPlatinumPerLevel = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _MobStatueChance { get { return MobStatueChance; } set { MobStatueChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _ParagonMinChance { get { return ParagonMinChance; } set { ParagonMinChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _ParagonMaxChance { get { return ParagonMaxChance; } set { ParagonMaxChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _ParagonChestChance { get { return ParagonChestChance; } set { ParagonChestChance = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double _ParagonRevertInHours { get { return ParagonRevertInHours; } set { ParagonRevertInHours = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool _AllowCriminalUseGate { get { return AllowCriminalUseGate; } set { AllowCriminalUseGate = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool _AllowAllPossessedMobsSpeedBoost { get { return AllowAllPossessedMobsSpeedBoost; } set { AllowAllPossessedMobsSpeedBoost = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool _AllowPseudoseerMobsSpeedBoost { get { return AllowPseudoseerMobsSpeedBoost; } set { AllowPseudoseerMobsSpeedBoost = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public int _PlatinumPerMissedDamageLevel { get { return PlatinumPerMissedDamageLevel; } set { PlatinumPerMissedDamageLevel = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool _ReplaceVanqWithSkillScrolls { get { return ReplaceVanqWithSkillScrolls; } set { ReplaceVanqWithSkillScrolls = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool _FastWalkPrevention { get { return PlayerMobile.FastwalkPrevention; } set { PlayerMobile.FastwalkPrevention = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public TimeSpan _FastWalkThreshold { get { return PlayerMobile.FastwalkThreshold; } set { PlayerMobile.FastwalkThreshold = value; } }

		[CommandProperty(AccessLevel.Administrator)]
		public string _OnLoginUberScript { get { return OnLoginUberScript; } set { OnLoginUberScript = value; } }

		[CommandProperty(AccessLevel.Administrator)]
		public string _OnPlayerDeathUberScript { get { return OnPlayerDeathUberScript; } set { OnPlayerDeathUberScript = value; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public int _HighestDamageLevelSpawn { get { return HighestDamageLevelSpawn; } set { HighestDamageLevelSpawn = Math.Max(0, Math.Min(5, value)); } }

		//End Globals ^

		private DateTime _StartTime;

		public Timer PseudoSeerTimerInstance { get; private set; }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public TimeSpan SeerDuration { get; set; }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool MessageStaff { get; set; }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public double CreatureLootDropMultiplier { get; set; }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public int TournamentAward { get; set; }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public int CTFAward { get; set; }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public TimeSpan TimeLeft { get { return _TimerRunning ? (SeerDuration - (DateTime.UtcNow - _StartTime)) : TimeSpan.MaxValue; } }

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool MovePSeerToLastPossessed { get; set; }

		private bool _TimerRunning;

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool TimerRunning
		{
			get { return _TimerRunning; }
			set
			{
				if (value == _TimerRunning)
				{
					return;
				}

				_TimerRunning = value;

				if (!_TimerRunning)
				{
					if (PseudoSeerTimerInstance != null)
					{
						PseudoSeerTimerInstance.Stop();
					}
				}
				else
				{
					_StartTime = DateTime.UtcNow;
					PseudoSeerTimerInstance = new PseudoSeerTimer();
					PseudoSeerTimerInstance.Start();
				}
			}
		}

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public bool ClearPseudoSeers
		{
			get { return false; }
			set
			{
				if (value == false)
				{
					return;
				}

				CleanUp();
			}
		}

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public Mobile PseudoSeerAdd
		{
			get { return null; }
			set
			{
				if (value == null || value.Deleted || value.NetState == null || value.NetState.Account == null)
				{
					return;
				}

				PseudoSeers[value.NetState.Account] = CurrentPermissionsClipboard;

				if (!String.IsNullOrEmpty(CurrentPermissionsClipboard))
				{
					value.SendGump(new PossessGump(value));
				}
			}
		}

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public Mobile PseudoSeerRemove
		{
			get { return null; }
			set
			{
				if (value == null || value.Deleted || value.NetState == null || value.NetState.Account == null ||
					!PseudoSeers.ContainsKey(value.NetState.Account))
				{
					return;
				}

				PseudoSeers.Remove(value.NetState.Account);
				value.CloseGump(typeof(PossessGump));
			}
		}

		public static string PermissionsClipboard = "";

		[CommandProperty(CreaturePossession.FullAccessStaffLevel)]
		public string CurrentPermissionsClipboard { get { return PermissionsClipboard; } set { PermissionsClipboard = value; } }

		[Constructable]
		public PseudoSeerStone()
			: base(0xEDC)
		{
			MovePSeerToLastPossessed = true;
			Name = "Pseudoseer Stone";
			Movable = false;
			SeerDuration = TimeSpan.MaxValue;
			MessageStaff = true;

			if (_Instance != null)
			{
				// there can only be one PseudoSeerStone game stone in the world
				_Instance.Delete();

				CommandHandlers.BroadcastMessage(
					CreaturePossession.FullAccessStaffLevel,
					0x489,
					"Old PseudoSeerStone gamestone has been deleted as new one was added.");
			}

			_Instance = this;
		}

		public PseudoSeerStone(Serial serial)
			: base(serial)
		{
			MovePSeerToLastPossessed = true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(21); //version

			// version 21
			writer.Write(_FastWalkPrevention);

			// version 20
			writer.Write(OnPlayerDeathUberScript);
			writer.Write(_CreaturesStealthLikePlayers);

			// version 19
			writer.Write(TreasureMapMobSpawnChance);

			// version 18
			writer.Write(OnLoginUberScript);

			// version 17
			writer.Write(AllowThirdDawnClient);

			// version 16
			writer.Write(MaxBODGoldRewardAllowed);
			writer.Write(HoursBetweenBODs);
			writer.Write(BondingAbandonDelay);

			// version 15
			writer.Write(TreasureChestDamageModifierChance);
			writer.Write(TreasureChestVanqChance);
			writer.Write(TreasureChestPowerChance);
			writer.Write(TreasureChestForceChance);
			writer.Write(TreasureChestMightChance);

			writer.Write(TreasureChestDurabilityModifierChance);
			writer.Write(TreasureChestIndestructibleChance);
			writer.Write(TreasureChestFortifiedChance);
			writer.Write(TreasureChestMassiveChance);
			writer.Write(TreasureChestSubstantialChance);

			writer.Write(TreasureChestAccuracyModifierChance);
			writer.Write(TreasureChestSupremelyChance);
			writer.Write(TreasureChestExceedinglyChance);
			writer.Write(TreasureChestEminentlyChance);
			writer.Write(TreasureChestSurpassinglyChance);

			// version 14
			writer.Write(AllowRedsInTown);
			writer.Write(CompanionMinutesBetweenHelp);
			writer.Write(ParagonRevertInHours);
			writer.Write(AllowCriminalUseGate);

			// version 13
			writer.Write(AllowAllPossessedMobsSpeedBoost);
			writer.Write(AllowPseudoseerMobsSpeedBoost);

			// version 12
			writer.Write(JustMurderedMinutesTracked);

			// version 11
			writer.Write(SpeedHackSeriousThreshold);

			// version 10
			writer.Write(ParagonChestPlatinumPerLevel);
			writer.Write(MobStatueChance);

			// version 9
			writer.Write(ParagonChestChance);
			writer.Write(PlatinumPerMissedDamageLevel);
			writer.Write(ReplaceVanqWithSkillScrolls);

			// version 8
			writer.Write(ParagonMaxChance);
			writer.Write(ParagonMinChance);

			// version 7
			writer.Write(HighestDamageLevelSpawn);

			// version 6
			writer.Write((byte)0); //m_FastWalkDisableMonitoring

			// version 5
			writer.Write(false); //MessagePump.RubberBandSpeedhackers
			writer.Write(_FastWalkThreshold);

			// verision 4
			writer.Write(CreatureLootDropMultiplier);

			// version 3
			writer.Write(MovePSeerToLastPossessed);

			// version 2
			var mobilesToWrite = new List<Mobile>();

			foreach (IAccount key in PseudoSeers.Keys)
			{
				Mobile lastChar = key.GetPseudoSeerLastCharacter();

				if (lastChar != null)
				{
					mobilesToWrite.Add(lastChar);
				}
				else
				{
					if (key is Account)
					{
						Account account = key as Account;

						// attempt to add first mobile on the account as pseudoseer--will use it's account in deserialize
						if (account.Mobiles != null && account.Mobiles.Length > 0 && account.Mobiles[0] != null &&
							account == account.Mobiles[0].Account)
						{
							mobilesToWrite.Add(account.Mobiles[0]);
						}
					}
				}
			}

			writer.Write(mobilesToWrite.Count);

			foreach (Mobile mob in mobilesToWrite)
			{
				writer.Write(mob.Account.Username);
				writer.Write(PseudoSeers[mob.Account]);
			}

			//version 1
			writer.Write(CurrentPermissionsClipboard);
			writer.Write(SeerDuration);
			writer.Write(MessageStaff);
			// NOTE: The pseudoseer list & possessed monsters list are not serialized because
			// if server goes down, pseudoseers lose connection to possessed monsters (and cannot log back into them)
			// and the pseudoseer list is emptied, meaning they can no longer use the [possess command
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 21:
					_FastWalkPrevention = reader.ReadBool();
					goto case 20;
				case 20:
					{
						OnPlayerDeathUberScript = reader.ReadString();
						CreaturesStealthLikePlayers = reader.ReadBool();
					}
					goto case 19;
				case 19:
					TreasureMapMobSpawnChance = reader.ReadDouble();
					goto case 18;
				case 18:
					OnLoginUberScript = reader.ReadString();
					goto case 17;
				case 17:
					AllowThirdDawnClient = reader.ReadBool();
					goto case 16;
				case 16:
					{
						MaxBODGoldRewardAllowed = reader.ReadInt();
						HoursBetweenBODs = reader.ReadDouble();
						BondingAbandonDelay = reader.ReadTimeSpan();
					}
					goto case 15;
				case 15:
					{
						TreasureChestDamageModifierChance = reader.ReadDouble();
						TreasureChestVanqChance = reader.ReadDouble();
						TreasureChestPowerChance = reader.ReadDouble();
						TreasureChestForceChance = reader.ReadDouble();
						TreasureChestMightChance = reader.ReadDouble();

						TreasureChestDurabilityModifierChance = reader.ReadDouble();
						TreasureChestIndestructibleChance = reader.ReadDouble();
						TreasureChestFortifiedChance = reader.ReadDouble();
						TreasureChestMassiveChance = reader.ReadDouble();
						TreasureChestSubstantialChance = reader.ReadDouble();

						TreasureChestAccuracyModifierChance = reader.ReadDouble();
						TreasureChestSupremelyChance = reader.ReadDouble();
						TreasureChestExceedinglyChance = reader.ReadDouble();
						TreasureChestEminentlyChance = reader.ReadDouble();
						TreasureChestSurpassinglyChance = reader.ReadDouble();
					}
					goto case 14;
				case 14:
					{
						AllowRedsInTown = reader.ReadBool();
						CompanionMinutesBetweenHelp = reader.ReadDouble();
						ParagonRevertInHours = reader.ReadDouble();
						AllowCriminalUseGate = reader.ReadBool();
					}
					goto case 13;
				case 13:
					{
						AllowAllPossessedMobsSpeedBoost = reader.ReadBool();
						AllowPseudoseerMobsSpeedBoost = reader.ReadBool();
					}
					goto case 12;
				case 12:
					JustMurderedMinutesTracked = reader.ReadDouble();
					goto case 11;
				case 11:
					SpeedHackSeriousThreshold = reader.ReadDouble();
					goto case 10;
				case 10:
					{
						ParagonChestPlatinumPerLevel = reader.ReadInt();
						MobStatueChance = reader.ReadDouble();
					}
					goto case 9;
				case 9:
					{
						ParagonChestChance = reader.ReadDouble();
						PlatinumPerMissedDamageLevel = reader.ReadInt();
						ReplaceVanqWithSkillScrolls = reader.ReadBool();
					}
					goto case 8;
				case 8:
					{
						ParagonMaxChance = reader.ReadDouble();
						ParagonMinChance = reader.ReadDouble();
					}
					goto case 7;
				case 7:
					HighestDamageLevelSpawn = reader.ReadInt();
					goto case 6;
				case 6:
					reader.ReadByte();
					goto case 5;
				case 5:
					{
						reader.ReadBool();
						_FastWalkThreshold = reader.ReadTimeSpan();
					}
					goto case 4;
				case 4:
					CreatureLootDropMultiplier = reader.ReadDouble();
					goto case 3;
				case 3:
					MovePSeerToLastPossessed = reader.ReadBool();
					goto case 2;
				case 2:
					{
						int numPseudoseers = reader.ReadInt();

						for (int i = 0; i < numPseudoseers; i++)
						{
							PseudoseerUserNamesToReinstate.Add(reader.ReadString());
							PseudoseerPermissionsToReinstate.Add(reader.ReadString());
						}
					}
					goto case 1;
				case 1:
					{
						CurrentPermissionsClipboard = reader.ReadString();
						SeerDuration = reader.ReadTimeSpan();
						MessageStaff = reader.ReadBool();
					}
					break;
			}

			_Instance = this;
		}

		public string GetPermissionsFor(IAccount account)
		{
			return account != null && PseudoSeers.ContainsKey(account) ? PseudoSeers[account] : null;
		}

		public override void OnAfterDelete()
		{
			CleanUp();
			_Instance = null;

			base.OnAfterDelete();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (CreaturePossession.IsAuthorizedStaff(from))
			{
				from.SendGump(new PseudoSeersControlGump(from));
				/*
                Gump gump = new Gump(0, 0);
                gump.AddPage( 0 );
				gump.AddBackground(0, 0, 420, 420, 5054);
                gump.AddHtml(10, 10, 400, 400, "<p>To use this stone, you can set the following attributes:</p>"
                                + "**<u>_ClearPseudoSeers</u>: If set to true, current pseudoseers are removed and possesed monsters are kicked. This attribute remains false, and can be set to true anytime.\n"
                                + "**<u>_TimerRunning</u>: Start / stop the timer for clearing the pseudoseer list (see SeerDuration).\n"
                                + "**<u>MessageStaff</u>: Messages between pseudoseers forwarded to staff (not implemented yet)\n"
                                + "**<u>PermissionAdd</u>: Add a monster group to CurrentPermissionsClipboard (selecting None = remove all groups).  This attribute remains \"None\", as making a selection only adds to the bit flags in CurrentPermissionsClipboard.  NOTE: All is at the end of the list\n"
                                + "**<u>PermissionRemove</u>: Opposite of PermissionAdd\n"
                                + "**<u>CurrentPermissionsClipboard</u>: bit flags for monster groups that pseudoseers can be possessed.  Permissions are assigned when a pseudoseer is added using PseudoSeerAdd.\n"
                                + "**<u>PseudoSeerAdd</u>: Add pseudoseer to the list (or update Permissions for an existing one)\n"
                                + "**<u>PseudoSeerRemove</u>: Remove a pseudoseer (does NOT kick monsters possessed by that pseudoseer)\n"
                                + "**<u>SeerDuration</u>: If _TimerRunning is true, after this amount of time, _ClearPseudoSeers is called as above\n"
                                + "**<u>TimeLeft</u>: Self-explanatory", true, true);
                from.SendGump(gump);
                */
			}
			else
			{
				from.SendMessage("Sorry, but you don't have permission access this.");
			}

			base.OnDoubleClick(from);
		}

		private void CleanUp()
		{
			if (PseudoSeerTimerInstance != null)
			{
				PseudoSeerTimerInstance.Stop();
			}

			// need to close all the PossessionGumps for existing PseudoSeers
			foreach (Mobile pseudoSeerLastCharacter in
				PseudoSeers.Keys.Select(account => account.GetPseudoSeerLastCharacter())
						   .Where(pseudoSeerLastCharacter => pseudoSeerLastCharacter != null && pseudoSeerLastCharacter.NetState != null))
			{
				pseudoSeerLastCharacter.CloseGump(typeof(PossessGump));
			}

			PseudoSeers.Clear();
			CreaturePossession.BootAllPossessions();
		}

		public void PseudoSeerMessage(string message, params object[] args)
		{
			message = String.Format(message, args);

			foreach (PlayerMobile member in PseudoSeers.Keys)
			{
				member.SendMessage(0x489, message);
			}

			if (MessageStaff)
			{
				CommandHandlers.BroadcastMessage(CreaturePossession.FullAccessStaffLevel, 0x489, message);
			}
		}

		private class PseudoSeerTimer : Timer
		{
			public PseudoSeerTimer()
				: base(TimeSpan.FromMinutes(1.0))
			{
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				if (Instance == null)
				{
					Stop();
					return;
				}

				Instance.CleanUp();
				Instance.TimerRunning = false;
				Stop();
			}
		}
	}
}