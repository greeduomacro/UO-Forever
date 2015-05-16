#region References
using System;

using Server.Commands;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
#endregion

namespace Server.Items
{
	public sealed class DynamicSettingsController : Item
	{
		#region Globals/Defaults
		public static bool MountFeluccaOnly = true;

		public static double SkillGainMultiplier = 1.0;
        public static double GoldMulti = 1.0;
		public static double PetDamageToPlayersMultiplier = 1.0;
		public static double PetNoGuardAfterDeathChance = 1.0;
        public static double GargoyleRuneSpawnChancePer100HP = 0.001;
		#endregion

		#region Instancing
		public static DynamicSettingsController Instance { get; private set; }

		public static void Configure()
		{
			CommandSystem.Register("DynSettings", AccessLevel.Administrator, OnDynSettingsCommand);
		}

		private static void OnDynSettingsCommand(CommandEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted)
			{
				return;
			}

			if (Instance == null)
			{
				Instance = new DynamicSettingsController {
					Location = e.Mobile.Location,
					Map = e.Mobile.Map
				};
			}

			e.Mobile.SendGump(new PropertiesGump(e.Mobile, Instance));
		}

		public static void UpdateInstancePosition(DynamicSettingsController attemptedConstruct)
		{
			if (attemptedConstruct == null)
			{
				return;
			}

			if (Instance == null) // should never happen, but if it does, make this the instance
			{
				Instance = attemptedConstruct;
			}
			else if (attemptedConstruct.Location != Point3D.Zero) // move the instance to it's location and delete it
			{
				Instance.Location = attemptedConstruct.Location;
				attemptedConstruct.Destroy();
			}
		}
		#endregion

		[CommandProperty(AccessLevel.Administrator)]
		public bool _IPLimitEnabled { get { return IPLimiter.Enabled; } set { IPLimiter.Enabled = value; } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool _IPLimitSocketBlock { get { return IPLimiter.SocketBlock; } set { IPLimiter.SocketBlock = value; } }

		[CommandProperty(AccessLevel.Administrator)]
		public int _IPLimitMax { get { return IPLimiter.MaxAddresses; } set { IPLimiter.MaxAddresses = value; } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool _FastwalkPrevention { get { return PlayerMobile.FastwalkPrevention; } set { PlayerMobile.FastwalkPrevention = value; } }

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan _FastwalkThreshold { get { return PlayerMobile.FastwalkThreshold; } set { PlayerMobile.FastwalkThreshold = value; } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool _FrozenPacketFlagsEnabled { get { return Mobile.FrozenPacketFlagsEnabled; } set { Mobile.FrozenPacketFlagsEnabled = value; } }
		
		[CommandProperty(AccessLevel.Administrator)]
		public bool _MountFeluccaOnly { get { return MountFeluccaOnly; } set { MountFeluccaOnly = value; } }

        [CommandProperty(AccessLevel.Administrator)]
        public double _SkillGainMultiplier { get { return SkillGainMultiplier; } set { SkillGainMultiplier = value; } }

        [CommandProperty(AccessLevel.Administrator)]
        public double _GoldMulti { get { return GoldMulti; } set { GoldMulti = value; } }
		
		[CommandProperty(AccessLevel.Administrator)]
		public double _PetDamageToPlayersMultiplier { get { return PetDamageToPlayersMultiplier; } set { PetDamageToPlayersMultiplier = value; } }
		
		[CommandProperty(AccessLevel.Administrator)]
		public double _PetNoGuardAfterDeathChance { get { return PetNoGuardAfterDeathChance; } set { PetNoGuardAfterDeathChance = value; } }

        [CommandProperty(AccessLevel.Administrator)]
        public double _GargoyleRuneSpawnChancePer100HP { get { return GargoyleRuneSpawnChancePer100HP; } set { GargoyleRuneSpawnChancePer100HP = value; } }

		[Constructable]
		public DynamicSettingsController()
			: base(0xEDC)
		{
			Name = "Dynamic Settings Controller";

			Movable = false;
			Visible = false;

			if (Instance != null && Instance != this)
			{
				// there can only be one DynamicSettingsController game stone in the world
				Instance.Location = Location;

				CommandHandlers.BroadcastMessage(
					AccessLevel.Administrator,
					1161,
					"Existing DynamicSettingsController has been moved to this location.");

				Timer.DelayCall(TimeSpan.FromSeconds(1), UpdateInstancePosition, this);
			}
			else
			{
				Instance = this;
			}
		}

		public DynamicSettingsController(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile from)
		{
			if (from.AccessLevel >= AccessLevel.Administrator)
			{
				from.SendGump(new PropertiesGump(from, this));
			}
			else
			{
				from.SendMessage("Sorry, but you don't have permission to access this.");
			}

			base.OnDoubleClick(from);
		}

		private void Destroy()
		{
			base.Delete();
		}

		public override void Delete()
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(7); //version

            //7
            writer.Write(_GoldMulti);

            //6
            writer.Write(_GargoyleRuneSpawnChancePer100HP); 

			//5
			writer.Write(_IPLimitEnabled);
			writer.Write(_IPLimitSocketBlock);
			writer.Write(_IPLimitMax);

			//4
			writer.Write(_FastwalkThreshold);

			//3
			writer.Write(_FastwalkPrevention);

			//2
			writer.Write(_FrozenPacketFlagsEnabled);

			//1
			writer.Write(_MountFeluccaOnly);

			//0
			writer.Write(_SkillGainMultiplier);
			writer.Write(_PetDamageToPlayersMultiplier);
			writer.Write(_PetNoGuardAfterDeathChance);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
                case 7:
                    {
                        _GoldMulti = reader.ReadDouble();
                    }
                    goto case 6;
                case 6:
                    {
                        _GargoyleRuneSpawnChancePer100HP = reader.ReadDouble();
                    }
                    goto case 5;
                case 5:
					{
						_IPLimitEnabled = reader.ReadBool();
						_IPLimitSocketBlock = reader.ReadBool();
						_IPLimitMax = reader.ReadInt();
					}
					goto case 4;
				case 4:
					_FastwalkThreshold = reader.ReadTimeSpan();
					goto case 3;
				case 3:
					_FastwalkPrevention = reader.ReadBool();
					goto case 2;
				case 2:
					_FrozenPacketFlagsEnabled = reader.ReadBool();
					goto case 1;
				case 1:
					_MountFeluccaOnly = reader.ReadBool();
					goto case 0;
				case 0:
					{
						_SkillGainMultiplier = reader.ReadDouble();
						_PetDamageToPlayersMultiplier = reader.ReadDouble();
						_PetNoGuardAfterDeathChance = reader.ReadDouble();
					}
					break;
			}

			Instance = this;
		}
	}
}