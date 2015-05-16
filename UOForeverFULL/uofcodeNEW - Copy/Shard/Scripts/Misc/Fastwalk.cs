#region Header
//   Vorspire    _,-'/-'/  Fastwalk.cs
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
using System.Linq;

using Server.Mobiles;
using Server.Network;

using VitaNex;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Misc
{
	[CoreModule("Speedhack Detection", "1.1.1", true, TaskPriority.Highest)]
	public static class SpeedhackDetection
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static SpeedhackDetectionOptions CMOptions { get; private set; }

		static SpeedhackDetection()
		{
			CMOptions = new SpeedhackDetectionOptions();
		}

		private static void CMConfig()
		{
			EventSink.FastWalk += OnDetected;
		}

		private static void CMEnabled()
		{
			EventSink.FastWalk += OnDetected;
		}

		private static void CMDisabled()
		{
			EventSink.FastWalk -= OnDetected;
		}

		public static void OnDetected(FastWalkEventArgs e)
		{
			if (!(e.NetState.Mobile is PlayerMobile))
			{
				return;
			}

			PlayerMobile pm = (PlayerMobile)e.NetState.Mobile;

			Console.WriteLine("Client: {0}: Speed exploit detected: {1}", e.NetState, pm);

			if (CMOptions.DetectAction == SpeedhackAction.None)
			{
				return;
			}

			if (CMOptions.DetectAction.HasFlag(SpeedhackAction.Block))
			{
				e.Blocked = true;
			}

			if (CMOptions.DetectAction.HasFlag(SpeedhackAction.Warn))
			{
				NoticeDialogGump g =
					SuperGump.EnumerateInstances<NoticeDialogGump>(pm, true)
							 .FirstOrDefault(d => !d.IsDisposed && d.Title == "Speed Exploit Detection") ?? new NoticeDialogGump(pm)
							 {
								 CanClose = false,
								 CanDispose = false,
								 Width = 420,
								 Height = 420,
								 Modal = true,
								 BlockMovement = true,
								 Icon = 7000,
								 Title = "Speed Exploit Detection",
								 Html =
									 "You seem to be moving faster than the universe allows, that isn't a good thing!" +
									 "\nIf you defy the laws of physics, bad things can happen." +
									 "\nYou don't want to end up in a black hole, unable to return to the universe, do you?" +
									 "\nI didn't think so." + "\nPlay fair and disable any artificial speed exploits that you may be using.",
								 AcceptHandler = b =>
								 {
									 if (CMOptions.DetectAction.HasFlag(SpeedhackAction.Kick))
									 {
										 e.NetState.Dispose(true);
									 }
								 }
							 };

				g.Refresh(true);
			}
			else if (CMOptions.DetectAction.HasFlag(SpeedhackAction.Kick))
			{
				e.NetState.Dispose(true);
			}
		}
	}

	[Flags]
	public enum SpeedhackAction
	{
		None = 0x0,
		Block = 0x1,
		Warn = 0x2,
		Kick = 0x4,

		Silent = Block | Kick,
		Passive = Block | Warn,
		Aggressive = Block | Warn | Kick
	}

	public sealed class SpeedhackDetectionOptions : CoreModuleOptions
	{
		private SpeedhackAction _DetectAction;

		[CommandProperty(SpeedhackDetection.Access)]
		public TimeSpan Threshold { get { return PlayerMobile.FastwalkThreshold; } set { PlayerMobile.FastwalkThreshold = value; } }

		[CommandProperty(SpeedhackDetection.Access, true)]
		public SpeedhackAction DetectAction
		{
			get { return _DetectAction; }
			set
			{
				_DetectAction = value;
				InvalidateDetectImpl();
			}
		}

		[CommandProperty(SpeedhackDetection.Access)]
		public bool DetectBlock { get { return GetActionFlag(SpeedhackAction.Block); } set { SetActionFlag(SpeedhackAction.Block, value); } }

		[CommandProperty(SpeedhackDetection.Access)]
		public bool DetectWarn { get { return GetActionFlag(SpeedhackAction.Warn); } set { SetActionFlag(SpeedhackAction.Warn, value); } }

		[CommandProperty(SpeedhackDetection.Access)]
		public bool DetectKick { get { return GetActionFlag(SpeedhackAction.Kick); } set { SetActionFlag(SpeedhackAction.Kick, value); } }

		public SpeedhackDetectionOptions()
			: base(typeof(SpeedhackDetection))
		{
			Threshold = TimeSpan.FromMilliseconds(600);

			DetectAction = SpeedhackAction.Passive;
		}

		public SpeedhackDetectionOptions(GenericReader reader)
			: base(reader)
		{ }

		private void InvalidateDetectImpl()
		{
			PlayerMobile.FastwalkPrevention = ModuleEnabled && _DetectAction != SpeedhackAction.None;

			if (PlayerMobile.FastwalkPrevention)
			{
				PacketHandlers.RegisterThrottler(0x02, PlayerMobile.MovementThrottle_Callback);
			}
			else
			{
				PacketHandlers.RegisterThrottler(0x02, null);
			}
		}

		private bool GetActionFlag(SpeedhackAction action)
		{
			return DetectAction.HasFlag(action);
		}

		private void SetActionFlag(SpeedhackAction action, bool value)
		{
			if (value)
			{
				DetectAction |= action;
			}
			else
			{
				DetectAction &= ~action;
			}
		}

		public override void Clear()
		{
			base.Clear();

			Threshold = TimeSpan.FromMilliseconds(600);
			DetectAction = SpeedhackAction.None;
		}

		public override void Reset()
		{
			base.Reset();

			Threshold = TimeSpan.FromMilliseconds(600);
			DetectAction = SpeedhackAction.Passive;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.Write(Threshold);
						writer.WriteFlag(DetectAction);
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
						Threshold = reader.ReadTimeSpan();
						DetectAction = reader.ReadFlag<SpeedhackAction>();
					}
					break;
			}
		}
	}
}