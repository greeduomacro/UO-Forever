#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server.Accounting;
using Server.Commands;
using Server.ContextMenus;
using Server.Engines.BulkOrders;
using Server.Engines.CannedEvil;
using Server.Engines.CentralGump;
using Server.Engines.ConPVP;
using Server.Engines.Conquests;
using Server.Engines.Craft;
using Server.Engines.CustomTitles;
using Server.Engines.Help;
using Server.Engines.MyRunUO;
using Server.Engines.PartySystem;
using Server.Engines.Quests;
using Server.Engines.XmlSpawner2;
using Server.Engines.ZombieEvent;
using Server.Arcade;
using Server.Ethics;
using Server.Factions;
using Server.Games;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Movement;
using Server.Multis;
using Server.Network;
using Server.Poker;
using Server.Regions;
using Server.Scripts.New.Adam;
using Server.SkillHandlers;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Targeting;
using Server.Twitch;

using VitaNex.Modules.AutoPvP;
using VitaNex.Modules.AutoPvP.Battles;
using VitaNex.Modules.UOFLegends;
using RankDefinition = Server.Guilds.RankDefinition;
#endregion

namespace Server.Mobiles
{
	#region Enums
	public enum StaffRank
	{
		None,
		TrialCounselor,
		Counselor,
		LeadCounselor,
		TrialSeer,
		Seer,
		LeadSeer,
		TrialEM,
		EM,
		LeadEM,
		TrialGM,
		GM,
		LeadGM,
		TrialAdmin,
		Admin,
		LeadAdmin,
		TrialDev,
		Dev,
		LeadDev,
		Owner,
		QM,
		Emissary
	}

	[Flags]
	public enum CustomPlayerFlag
	{
		None = 0x00000000,
		StaffLevel = 0x00000001,
		VisibilityList = 0x00000002,
		StaffRank = 0x00000004,
		DisplayStaffRank = 0x00000008
	}

	[Flags]
	public enum PlayerFlag // First 16 bits are reserved for default-distro use, start custom flags at 0x00010000
	{
		None = 0x00000000,
		Glassblowing = 0x00000001,
		Masonry = 0x00000002,
		SandMining = 0x00000004,
		StoneMining = 0x00000008,
		ToggleMiningStone = 0x00000010,
		KarmaLocked = 0x00000020,
		AutoRenewInsurance = 0x00000040,
		UseOwnFilter = 0x00000080,
		PublicMyRunUO = 0x00000100,
		PagingSquelched = 0x00000200,
		Young = 0x00000400,
		AcceptGuildInvites = 0x00000800,
		DisplayChampionTitle = 0x00001000,
		HasStatReward = 0x00002000,
		RefuseTrades = 0x00004000,

		#region Scroll of Alacrity
		AcceleratedSkill = 0x00080000,
		#endregion
	}

	public enum NpcGuild
	{
		None,
		MagesGuild,
		WarriorsGuild,
		ThievesGuild,
		RangersGuild,
		HealersGuild,
		MinersGuild,
		MerchantsGuild,
		TinkersGuild,
		TailorsGuild,
		FishermensGuild,
		BardsGuild,
		BlacksmithsGuild
	}

	public enum SolenFriendship
	{
		None,
		Red,
		Black
	}
	#endregion

	public partial class PlayerMobile : Mobile
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public override Container Backpack
		{
			get
			{
				Container pack = base.Backpack;

				if (pack == null)
				{
					AddItem(
						pack = new Backpack(Expansion)
						{
							Movable = false
						});
					pack.SendInfoTo(NetState);
				}

				return pack;
			}
			set { base.Backpack = value; }
		}

		public override void OnHoldingChanged(Item old)
		{
			base.OnHoldingChanged(old);

			Conquests.CheckProgress<ObtainConquest>(this, Holding);
		}

		#region Mount Blocking
		public void SetMountBlock(BlockMountType type, TimeSpan duration, bool dismount)
		{
			if (dismount)
			{
				BaseMount.Dismount(this);
				BaseMount.SetMountPrevention(this, type, duration);
			}
			else
			{
				BaseMount.SetMountPrevention(this, type, duration);
			}
		}
		#endregion

		protected override bool OnBeforeSendGump(Gump g)
		{
			if (g == null || (ActionCams.IsCamera(this) && !ActionCams.GumpWhitelist.Any(t => g.TypeEquals(t))))
			{
				return false;
			}

			return base.OnBeforeSendGump(g);
		}

		private class CountAndTimeStamp
		{
			private int _Count;

			public int Count
			{
				get { return _Count; }
				set
				{
					_Count = value;
					TimeStamp = DateTime.UtcNow;
				}
			}

			public DateTime TimeStamp { get; private set; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual string PseudoSeerPermissions
		{
			get
			{
				return PseudoSeerStone.Instance != null && !Deleted && NetState != null
						   ? PseudoSeerStone.Instance.GetPermissionsFor(NetState.Account)
						   : null;
			}
			set
			{
				if (PseudoSeerStone.Instance == null || Deleted || NetState == null)
				{
					return;
				}

				string oldPermissions = PseudoSeerStone.Instance.CurrentPermissionsClipboard;

				PseudoSeerStone.Instance.CurrentPermissionsClipboard = value;

				if (value == null)
				{
					PseudoSeerStone.Instance.PseudoSeerRemove = this;
				}
				else
				{
					PseudoSeerStone.Instance.PseudoSeerAdd = this;
				}

				PseudoSeerStone.Instance.CurrentPermissionsClipboard = oldPermissions;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public List<Mobile> PartyMembers
		{
			get
			{
				if (Party == null)
				{
					return null;
				}

				var party = Party as Party;

				if (party == null)
				{
					return null;
				}

				return party.Members.Where(info => info.Mobile != null).Select(info => info.Mobile).ToList();
			}
		}

		#region Valor Titles
		private List<string> _ValorTitles;

		public int ValorTitleCount { get { return _ValorTitles == null ? 0 : _ValorTitles.Count; } }

		public virtual bool HasValorTitle(string title)
		{
			return _ValorTitles != null && _ValorTitles.Exists(e => e == title);
		}

		public virtual void AddValorTitle(string title)
		{
			if (_ValorTitles == null)
			{
				_ValorTitles = new List<string>();
			}

			_ValorTitles.Add(title);
		}

		public virtual List<string> GetValorTitles()
		{
			return _ValorTitles;
		}
		#endregion

		// new event stuff
		public bool EventMsgFlag { get; set; }
		public int EventPoints { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string FactionName
		{
			get
			{
				return FactionPlayerState != null && FactionPlayerState.Faction != null &&
					   FactionPlayerState.Faction.Definition != null && FactionPlayerState.Faction.Definition.Name != null
						   ? FactionPlayerState.Faction.Definition.Name.String
						   : null;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public GuildType GuildLoyalty { get { return Guild != null ? Guild.Type : GuildType.Regular; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string GuildName { get { return Guild != null ? Guild.Name : null; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string GuildAbbreviation { get { return Guild != null ? Guild.Abbreviation : null; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string GuildRankName { get { return Guild != null && GuildRank != null && GuildRank.Name != null ? GuildRank.Name.String : null; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MurderBounty { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RingofForgiveness { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool GameParticipant { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool NewbieQuestCompleted { get; set; }

		#region Companions
		public static List<PlayerMobile> OnlineCompanions = new List<PlayerMobile>();
		public static List<PlayerMobile> AllCompanions = new List<PlayerMobile>();

		private bool m_Companion;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Companion
		{
			get { return m_Companion; }
			set
			{
				if (m_Companion == value)
				{
					return;
				}

				m_Companion = value;

				if (m_Companion)
				{
					if (!AllCompanions.Contains(this))
					{
						AllCompanions.Add(this);
					}

					if (Title != "the Companion" && Title != "the Elder")
					{
						Title = "the Companion";
					}
				}
				else
				{
					AllCompanions.Remove(this);
					OnlineCompanions.Remove(this);

					if (Title == "the Companion")
					{
						Title = null;
					}
				}
			}
		}
		#endregion

		public CaptureZone CaptureZone { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int BankCredit
		{
			get { return BankBox != null ? (int)BankBox.Credit : 0; }
			set
			{
				if (BankBox != null)
				{
					BankBox.Credit = value > 0 ? (ulong)value : 0;
				}
			}
		}

		public DateTime LastHelped { get; set; }

		public override void OnConnected()
		{
		    var profile = ZombieEvent.EnsureProfile(this);
		    profile.Active = false;
			if (XmlScript.HasTrigger(this, TriggerName.onLogin))
			{
				UberScriptTriggers.Trigger(this, this, TriggerName.onLogin);
			}

			base.OnConnected();
		}

		public override void OnDisconnected()
		{
			if (Companion)
			{
				if (OnlineCompanions.Contains(this))
				{
					LoggingCustom.Log(
						Path.Combine(new[] {CompanionListGump.LogFileLocation, RawName + ".txt"}),
						DateTime.Now + "\t" + "Signed off as Companion");
					OnlineCompanions.Remove(this);
				}
			}

			if (XmlScript.HasTrigger(this, TriggerName.onDisconnected))
			{
				UberScriptTriggers.Trigger(this, this, TriggerName.onDisconnected);
			}

			base.OnDisconnected();
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int KMUsed { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool InStat
		{
			get { return StatEnd > DateTime.UtcNow; }
			set
			{
				if (value || !InStat)
				{
					return;
				}

				StatEnd = DateTime.MinValue;
				BaseShieldGuard.ClearSkillLoss(this);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime StatEnd { get; set; }

		// number of items that could not be automaitically reinsured because gold in bank was not enough
		private int m_NonAutoreinsuredItems;

		#region Getters & Setters
		public override bool RetainPackLocsOnDeath { get { return true; } }

		public PokerGame PokerGame { get; set; }

        public DateTime PokerJoinTimer { get; set; }

		public List<Mobile> RecentlyReported { get; private set; }
		public List<Mobile> AutoStabled { get; private set; }
		public List<Mobile> AllFollowers { get; private set; }

		public bool NinjaWepCooldown { get; set; }

		private RankDefinition m_GuildRank;

		public RankDefinition GuildRank { get { return AccessLevel >= AccessLevel.GameMaster ? RankDefinition.Leader : m_GuildRank; } set { m_GuildRank = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime AsylumDoorNextUse { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool FactionDeath { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int FactionPoints
		{
			get
			{
				PlayerState pl = PlayerState.Find(this);

				if (pl != null)
				{
					return pl.KillPoints;
				}

				return 0;
			}
			set
			{
				PlayerState pl = PlayerState.Find(this);

				if (pl != null)
				{
					pl.KillPoints = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ValorPoints { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime Lastkilled { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MurderersKilled { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ValorRank { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string ValorTitle { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ValorQuests { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextLeaveCommand { get; set; }

		private int _ValorRating;

		[CommandProperty(AccessLevel.GameMaster)]
		public int ValorRating
		{
			get { return _ValorRating; }
			set
			{
				if (value >= 0 && value <= 6)
				{
					_ValorRating = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int GuildMessageHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int AllianceMessageHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Profession { get; set; }

		public int StepsTaken { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public NpcGuild NpcGuild { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime NpcGuildJoinTime { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime NextBODTurnInTime { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastOnline { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastMoved { get { return LastMoveTime; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NpcGuildGameTime { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ToTItemsTurnedIn { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ToTTotalMonsterFame { get; set; }
		#endregion

		#region PlayerFlags
		public PlayerFlag Flags { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool PagingSquelched { get { return GetFlag(PlayerFlag.PagingSquelched); } set { SetFlag(PlayerFlag.PagingSquelched, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Glassblowing { get { return GetFlag(PlayerFlag.Glassblowing); } set { SetFlag(PlayerFlag.Glassblowing, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Masonry { get { return GetFlag(PlayerFlag.Masonry); } set { SetFlag(PlayerFlag.Masonry, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool SandMining { get { return GetFlag(PlayerFlag.SandMining); } set { SetFlag(PlayerFlag.SandMining, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool StoneMining { get { return GetFlag(PlayerFlag.StoneMining); } set { SetFlag(PlayerFlag.StoneMining, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ToggleMiningStone { get { return GetFlag(PlayerFlag.ToggleMiningStone); } set { SetFlag(PlayerFlag.ToggleMiningStone, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool KarmaLocked { get { return GetFlag(PlayerFlag.KarmaLocked); } set { SetFlag(PlayerFlag.KarmaLocked, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AutoRenewInsurance { get { return GetFlag(PlayerFlag.AutoRenewInsurance); } set { SetFlag(PlayerFlag.AutoRenewInsurance, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool UseOwnFilter { get { return GetFlag(PlayerFlag.UseOwnFilter); } set { SetFlag(PlayerFlag.UseOwnFilter, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool PublicMyRunUO
		{
			get { return GetFlag(PlayerFlag.PublicMyRunUO); }
			set
			{
				SetFlag(PlayerFlag.PublicMyRunUO, value);
                EventSink.InvokeMobileInvalidate(new MobileInvalidateEventArgs(this));
			}
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PublicLegends
        {
            get
            {

                return CentralGump.EnsureProfile(this).PublicLegends;
            }
            set
            {
                CentralGump.EnsureProfile(this).PublicLegends = value;
                EventSink.InvokeMobileInvalidate(new MobileInvalidateEventArgs(this));
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string CustomTitle
        {
            get
            {
                var profile = CustomTitles.EnsureProfile(this);
                if (profile.SelectedTitle != null)
                    return Female ? profile.SelectedTitle.MaleTitle : profile.SelectedTitle.FemaleTitle;
                return String.Empty;
            }
        }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AcceptGuildInvites { get { return GetFlag(PlayerFlag.AcceptGuildInvites); } set { SetFlag(PlayerFlag.AcceptGuildInvites, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasStatReward { get { return GetFlag(PlayerFlag.HasStatReward); } set { SetFlag(PlayerFlag.HasStatReward, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool RefuseTrades { get { return GetFlag(PlayerFlag.RefuseTrades); } set { SetFlag(PlayerFlag.RefuseTrades, value); } }
		#endregion

		#region Auto Arrow Recovery
		private readonly Dictionary<Type, int> m_RecoverableAmmo = new Dictionary<Type, int>();

		public Dictionary<Type, int> RecoverableAmmo { get { return m_RecoverableAmmo; } }

		public void RecoverAmmo()
		{
			if (!Alive /* || !EraSE*/)
			{
				return;
			}

			foreach (KeyValuePair<Type, int> kvp in m_RecoverableAmmo)
			{
				if (kvp.Value <= 0)
				{
					continue;
				}

				var ammo = kvp.Key.CreateInstanceSafe<Item>();

				if (ammo == null)
				{
					continue;
				}

				//string name = ammo.Name;

				ammo.Amount = kvp.Value;

				/*if (name == null)
				{
					if (ammo is Arrow)
					{
						name = "arrow";
					}
					else if (ammo is Bolt)
					{
						name = "bolt";
					}
				}

				if (name != null && ammo.Amount > 1)
				{
					name = String.Format("{0}s", name);
				}

				if (name == null)
				{
					name = String.Format("#{0}", ammo.LabelNumber);
				}*/

				var quiver = FindItemOnLayer(Layer.Cloak) as BaseQuiver;

				if (quiver == null || !quiver.TryDropItem(this, ammo, false))
				{
					PlaceInBackpack(ammo);
				}

				//SendLocalizedMessage( 1073504, String.Format( "{0}\t{1}", ammo.Amount, name ) ); // You recover ~1_NUM~ ~2_AMMO~.
			}

			m_RecoverableAmmo.Clear();
		}
		#endregion

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime AnkhNextUse { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DisguiseTimeLeft { get { return DisguiseTimers.TimeRemaining(this); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime PeacedUntil { get; set; }

		#region Scroll of Alacrity
		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime AcceleratedEnd { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public SkillName AcceleratedSkill { get; set; }
		#endregion

		public static Direction GetDirection4(Point3D from, Point3D to)
		{
			int dx = from.X - to.X;
			int dy = from.Y - to.Y;

			int rx = dx - dy;
			int ry = dx + dy;

			Direction ret;

			if (rx >= 0 && ry >= 0)
			{
				ret = Direction.West;
			}
			else if (rx >= 0 && ry < 0)
			{
				ret = Direction.South;
			}
			else if (rx < 0 && ry < 0)
			{
				ret = Direction.East;
			}
			else
			{
				ret = Direction.North;
			}

			return ret;
		}

		// called when dragging item that doesn't belong to anything else
		public override bool CheckLiftTrigger(Item item, ref LRReason reject)
		{
			if ((XmlScript.HasTrigger(this, TriggerName.onDragLift) &&
				 UberScriptTriggers.Trigger(this, this, TriggerName.onDragLift, item)) ||
				(XmlScript.HasTrigger(item, TriggerName.onDragLift) &&
				 UberScriptTriggers.Trigger(item, this, TriggerName.onDragLift)))
			{
				reject = LRReason.Inspecific;
				return false;
			}

			return true;
		}

		// called when dragging item that belongs to somebody or some container
		public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
		{
			if (XmlScript.HasTrigger(from, TriggerName.onDragLift) &&
				UberScriptTriggers.Trigger(from, from, TriggerName.onDragLift, item))
			{
				reject = LRReason.Inspecific;
				return false;
			}

			if (XmlScript.HasTrigger(item, TriggerName.onDragLift) &&
				UberScriptTriggers.Trigger(item, from, TriggerName.onDragLift, item))
			{
				reject = LRReason.Inspecific;
				return false;
			}

			if (item.RootParent is Mobile)
			{
				var parent = item.RootParent as Mobile;

				if (parent.AccessLevel == AccessLevel.Player && from.AccessLevel > AccessLevel.Player)
				{
					CommandLogging.WriteLine(
						from,
						"{0} {1} lifted {3:X} ({4}) from {2}",
						from.AccessLevel,
						CommandLogging.Format(from),
						CommandLogging.Format(parent),
						item.Serial.Value,
						item.GetType().Name);
				}
			}

			return base.CheckLift(from, item, ref reject);
		}

		public override bool OnDroppedItemToWorld(Item item, Point3D location)
		{
			if (!base.OnDroppedItemToWorld(item, location))
			{
				return false;
			}

			IPooledEnumerable mobiles = Map.GetMobilesInRange(location, 0);

			if (mobiles.OfType<Mobile>().Any(m => m.Z >= location.Z && m.Z < location.Z + 16))
			{
				mobiles.Free();
				return false;
			}

			mobiles.Free();

			if (XmlScript.HasTrigger(item, TriggerName.onDropToWorld) &&
				UberScriptTriggers.Trigger(item, this, TriggerName.onDropToWorld, item))
			{
				return false;
			}

			BounceInfo bi = item.GetBounce();

			if (bi == null)
			{
				return true;
			}

			Type type = item.GetType();

			if (!type.IsDefined(typeof(FurnitureAttribute), true) && !type.IsDefined(typeof(DynamicFlipingAttribute), true))
			{
				return true;
			}

			object[] objs = type.GetCustomAttributes(typeof(FlipableAttribute), true);

			if (objs.Length == 0)
			{
				return true;
			}

			var fp = objs[0] as FlipableAttribute;

			if (fp == null)
			{
				return true;
			}

			int[] itemIDs = fp.ItemIDs;

			Point3D oldWorldLoc = bi.m_WorldLoc;
			Point3D newWorldLoc = location;

			if (oldWorldLoc.X == newWorldLoc.X && oldWorldLoc.Y == newWorldLoc.Y)
			{
				return true;
			}

			Direction dir = GetDirection4(oldWorldLoc, newWorldLoc);

			switch (itemIDs.Length)
			{
				case 2:
					{
						switch (dir)
						{
							case Direction.North:
							case Direction.South:
								item.ItemID = itemIDs[0];
								break;
							case Direction.East:
							case Direction.West:
								item.ItemID = itemIDs[1];
								break;
						}
					}
					break;
				case 4:
					{
						switch (dir)
						{
							case Direction.South:
								item.ItemID = itemIDs[0];
								break;
							case Direction.East:
								item.ItemID = itemIDs[1];
								break;
							case Direction.North:
								item.ItemID = itemIDs[2];
								break;
							case Direction.West:
								item.ItemID = itemIDs[3];
								break;
						}
					}
					break;
			}

			return true;
		}

		public bool GetFlag(PlayerFlag flag)
		{
			return Flags.HasFlag(flag);
		}

		public void SetFlag(PlayerFlag flag, bool value)
		{
			if (value)
			{
				Flags |= flag;
			}
			else
			{
				Flags &= ~flag;
			}
		}

		public DesignContext DesignContext { get; set; }

		#region DFI-Beg Custom
		private static void OnCustomLogin(LoginEventArgs e)
		{
			var m = e.Mobile as PlayerMobile;

			if (m == null)
			{
				return;
			}

			List<Mobile> vislist = m.VisibilityList;

			foreach (Mobile targ in vislist.Where(targ => Utility.InUpdateRange(targ, m)))
			{
				if (targ.CanSee(m))
				{
					NetState ns = targ.NetState;
					if (ns != null)
					{
						ns.Send(MobileIncoming.Create(ns, targ, m));

						if (ObjectPropertyList.Enabled && targ.EraAOS)
						{
							targ.Send(m.OPLPacket);

							foreach (Item item in m.Items)
							{
								targ.Send(item.OPLPacket);
							}
						}
					}
				}
				else
				{
					targ.Send(m.RemovePacket);
				}
			}
		}

		public override int GetHurtSound()
		{
			if (!Body.IsHuman)
			{
				return base.GetDeathSound();
			}

			if (Body.IsFemale)
			{
				return Utility.Random(331, 5); // 331 - 334
			}

			return Utility.Random(340, 6); // 340 - 345
		}

		public override int GetDeathSound()
		{
			if (!Body.IsHuman)
			{
				return base.GetDeathSound();
			}

			if (Body.IsFemale)
			{
				return Utility.Random(336, 4); // 336 - 339
			}

			return Utility.Random(346, 4); // 346 - 349
		}

		public CustomPlayerFlag CustomFlags { get; set; }

		public bool GetCustomFlag(CustomPlayerFlag flag)
		{
			return CustomFlags.HasFlag(flag);
		}

		public void SetCustomFlag(CustomPlayerFlag flag, bool value)
		{
			if (value)
			{
				CustomFlags |= flag;
			}
			else
			{
				CustomFlags &= ~flag;
			}
		}
		#endregion

		public static void Initialize()
		{
			PacketHandlers.RegisterThrottler(0x02, MovementThrottle_Callback);

			EventSink.Login += OnLogin;
			EventSink.Logout += OnLogout;
			EventSink.Connected += EventSink_Connected;
			EventSink.Disconnected += EventSink_Disconnected;

			EventSink.Login += OnCustomLogin;

			Timer.DelayCall(TimeSpan.Zero, CheckPets);
			Timer.DelayCall(TimeSpan.Zero, CheckStatLossDecay);
		}

		private static void CheckPets()
		{
			foreach (PlayerMobile pm in
				World.Mobiles.Values.OfType<PlayerMobile>()
					 .Where(
						 pm =>
						 pm.EraSE && ((!pm.Mounted || pm.Mount is EtherealMount) && (pm.AllFollowers.Count > pm.AutoStabled.Count)) ||
						 (pm.Mounted && (pm.AllFollowers.Count > (pm.AutoStabled.Count + 1)))))
			{
				pm.AutoStablePets(); /* autostable checks summons, et al: no need here */
			}
		}

		private static void CheckStatLossDecay()
		{
			foreach (PlayerMobile pm in World.Mobiles.Values.OfType<PlayerMobile>())
			{
				BaseShieldGuard.StatLossDecay(pm);
			}
		}

		protected override void OnRaceChange(Race oldRace)
		{
			ValidateEquipment();
		}

		public override int MaxWeight
		{
			get
			{
				var max = (int)(3.5 * Str);

				if (EraML && Race == Race.Human)
				{
					max += 100;
				}
				else
				{
					max += 40;
				}

				return max;
			}
		}

		private int m_LastGlobalLight = -1;
		private int m_LastPersonalLight = -1;

		public override void OnNetStateChanged()
		{
			m_LastGlobalLight = -1;
			m_LastPersonalLight = -1;
		}

		public override void ComputeBaseLightLevels(out int global, out int personal)
		{
			global = LightCycle.ComputeLevelFor(this);

			if (AccessLevel > AccessLevel.Player)
			{
				personal = 50;
			}
			else if (LightLevel < 21 && (AosAttributes.GetValue(this, AosAttribute.NightSight) > 0 || (EraML && Race == Race.Elf)))
			{
				personal = 21;
			}
			else
			{
				personal = LightLevel;
			}
		}

		public override void CheckLightLevels(bool forceResend)
		{
			NetState ns = NetState;

			if (ns == null)
			{
				return;
			}

			int global, personal;

			ComputeLightLevels(out global, out personal);

			if (!forceResend)
			{
				forceResend = global != m_LastGlobalLight || personal != m_LastPersonalLight;
			}

			if (!forceResend)
			{
				return;
			}

			m_LastGlobalLight = global;
			m_LastPersonalLight = personal;

			ns.Send(GlobalLightLevel.Instantiate(global));
			ns.Send(new PersonalLightLevel(this, personal));
		}

		private static void OnLogin(LoginEventArgs e)
		{
			Mobile from = e.Mobile;

			// if you uncomment this line, all UOSteam users will be booted after logging in.
			Timer.DelayCall(TimeSpan.FromSeconds(9.5), Assistants.Negotiator.NegotiateFeatures, from);

			CheckAtrophies(from);

			// fix a bug with parties where you can't invite anybody
			// b/c client still thinks you are in party
			if (from.Party == null)
			{
				from.Send(new PartyEmptyList(from));
			}

			if (from is PlayerMobile)
			{
				var pm = (PlayerMobile)from;

			    if (CentralGump.EnsureProfile(pm).LoginGump)
			    {
			        if (pm.IsYoung())
			        {
                        new CentralGumpUIList(e.Mobile as PlayerMobile, CentralGump.EnsureProfile(e.Mobile as PlayerMobile), 115, 0).Send(); 
			        }
			        else
			        {
                        new CentralGumpUI(e.Mobile as PlayerMobile, CentralGump.EnsureProfile(e.Mobile as PlayerMobile), CentralGumpType.News).Send();
			        }

			    }

                EventSink.InvokeMobileInvalidate(new MobileInvalidateEventArgs(pm));

			    if (pm.IsYoung())
				{
					foreach (PlayerMobile companion in OnlineCompanions)
					{
						companion.LocalOverheadMessage(MessageType.Regular, 0x38, false, "New player " + pm.Name + " just logged in!");
					}

					pm.SendGump(new YoungPlayerGump());
				}

				// changed to sign on when they use the young command
				/*if (pm.Companion && !OnlineCompanions.Contains(pm))
				{
				    OnlineCompanions.Add(pm);
				}*/
			}

			if (AccountHandler.LockdownLevel > AccessLevel.Player)
			{
				string notice;

				var acct = from.Account as Account;

				if (acct == null || !acct.HasAccess(from.NetState))
				{
					notice = from.AccessLevel == AccessLevel.Player
								 ? "The server is currently under lockdown. No players are allowed to log in at this time."
								 : "The server is currently under lockdown. You do not have sufficient access level to connect.";

					if (from.NetState != null)
					{
						Timer.DelayCall(TimeSpan.FromSeconds(1.0), Disconnect, from.NetState);
					}
				}
				else if (from.AccessLevel >= AccessLevel.Administrator)
				{
					notice =
						"The server is currently under lockdown. As you are an administrator, you may change this from the [Admin gump.";
				}
				else
				{
					notice = "The server is currently under lockdown. You have sufficient access level to connect.";
				}

				from.SendGump(new NoticeGump(1060637, 30720, notice, 0xFFC000, 300, 140, null, null));
				return;
			}

			if (from is PlayerMobile)
			{
				from.PreferredLanguage = ((Account)from.Account).Language;
				((PlayerMobile)from).ClaimAutoStabledPets();
			}

			if (PseudoSeerStone.OnLoginUberScript != null)
			{
				XmlAttach.AttachTo(
					from,
					new XmlScript(PseudoSeerStone.OnLoginUberScript)
					{
						Name = "LOGIN_SCRIPT",
						Expiration = TimeSpan.FromSeconds(10.0)
					});
			}

		    var region = Region.Find(from.LogoutLocation, from.LogoutMap) as CustomRegion;

		    if (region != null && region.IsT2A() && from is PlayerMobile)
		    {
		        from.IsT2A = true;
                PvPTemplates.PvPTemplates.FetchProfile(from as PlayerMobile).ApplyDelta(false);
		    }

		}

		private bool m_NoDeltaRecursion;

		public void ValidateEquipment()
		{
			if (m_NoDeltaRecursion || Map == null || Map == Map.Internal)
			{
				return;
			}

			if (Items == null)
			{
				return;
			}

			m_NoDeltaRecursion = true;
			Timer.DelayCall(TimeSpan.Zero, ValidateEquipment_Sandbox);
		}

		private void ValidateEquipment_Sandbox()
		{
			try
			{
				if (Map == null || Map == Map.Internal)
				{
					return;
				}

				List<Item> items = Items;

				if (items == null)
				{
					return;
				}

				bool moved = false;

				int str = Str;
				int dex = Dex;
				int intel = Int;

				#region Factions
				int factionItemCount = 0;
				#endregion

				Mobile from = this;

				if (AccessLevel >= AccessLevel.GameMaster)
				{
					return;
				}

				for (int i = items.Count - 1; i >= 0; --i)
				{
					if (i >= items.Count)
					{
						continue;
					}

					Item item = items[i];

					if (item is BaseWeapon)
					{
						var weapon = (BaseWeapon)item;

						bool drop = false;

						if (dex < weapon.DexRequirement)
						{
							drop = true;
						}
						else if (str < weapon.StrRequirement)
						{
							drop = true;
						}
						else if (intel < weapon.IntRequirement)
						{
							drop = true;
						}
						else if (weapon.RequiredRace != null && weapon.RequiredRace != Race)
						{
							drop = true;
						}

						if (drop)
						{
							string name = weapon.ResolveName(from);

							from.SendLocalizedMessage(1062001, name); // You can no longer wield your ~1_WEAPON~
							from.AddToBackpack(weapon);
							moved = true;
						}
					}
					else if (item is BaseArmor)
					{
						var armor = (BaseArmor)item;

						bool drop = false;

						if (!armor.AllowMaleWearer && !@from.Female)
						{
							drop = true;
						}
						else if (!armor.AllowFemaleWearer && @from.Female)
						{
							drop = true;
						}
						else if (armor.RequiredRace != null && armor.RequiredRace != Race)
						{
							drop = true;
						}
						else
						{
							int strBonus = armor.StrBonus, strReq = armor.StrRequirement;
							int dexBonus = armor.DexBonus, dexReq = armor.DexRequirement;
							int intBonus = armor.IntBonus, intReq = armor.IntRequirement;

							if (dex < dexReq || (dex + dexBonus) < 1)
							{
								drop = true;
							}
							else if (str < strReq || (str + strBonus) < 1)
							{
								drop = true;
							}
							else if (intel < intReq || (intel + intBonus) < 1)
							{
								drop = true;
							}
						}

						if (drop)
						{
							string name = armor.ResolveName(from);

							if (armor is BaseShield)
							{
								from.SendLocalizedMessage(1062003, name); // You can no longer equip your ~1_SHIELD~
							}
							else
							{
								from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~
							}

							from.AddToBackpack(armor);
							moved = true;
						}
					}
					else if (item is BaseClothing)
					{
						var clothing = (BaseClothing)item;

						bool drop = false;

						if (!clothing.AllowMaleWearer && !from.Female)
						{
							drop = true;
						}
						else if (!clothing.AllowFemaleWearer && from.Female)
						{
							drop = true;
						}
						else if (clothing.RequiredRace != null && clothing.RequiredRace != Race)
						{
							drop = true;
						}
						else
						{
							int strBonus = clothing.ComputeStatBonus(StatType.Str);
							int strReq = clothing.ComputeStatReq(StatType.Str);

							if (str < strReq || (str + strBonus) < 1)
							{
								drop = true;
							}
						}

						if (drop)
						{
							string name = clothing.ResolveName(from);

							from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~

							from.AddToBackpack(clothing);
							moved = true;
						}
					}

					FactionItem factionItem = FactionItem.Find(item);

					if (factionItem == null)
					{
						continue;
					}

					bool dropF = false;

					Faction ourFaction = Faction.Find(this);

					if (ourFaction == null || ourFaction != factionItem.Faction)
					{
						dropF = true;
					}
					else if (++factionItemCount > FactionItem.GetMaxWearables(this))
					{
						dropF = true;
					}

					if (dropF)
					{
						from.AddToBackpack(item);
						moved = true;
					}
				}

				if (moved)
				{
					from.SendLocalizedMessage(500647); // Some equipment has been moved to your backpack.
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				m_NoDeltaRecursion = false;
			}
		}

		public override void Delta(MobileDelta flag)
		{
			base.Delta(flag);

			if ((flag & MobileDelta.Stat) != 0)
			{
				ValidateEquipment();
			}

			if ((flag & (MobileDelta.Name | MobileDelta.Hue)) != 0)
			{
                EventSink.InvokeMobileInvalidate(new MobileInvalidateEventArgs(this));
			}
		}

		private static void Disconnect(NetState ns)
		{
			ns.Dispose();
		}

		private static void OnLogout(LogoutEventArgs e)
		{
			if (TestCenter.Enabled && e.Mobile is PlayerMobile)
			{
				((PlayerMobile)e.Mobile).AutoStablePets();
			}

			if (e.Mobile != null && e.Mobile.Account is Account)
			{
				((Account)e.Mobile.Account).LogIPAccess(null, true);
			}

            #region SmoothMove
            if (((PlayerMobile)e.Mobile).Transport != null)
                ((PlayerMobile)e.Mobile).Transport.LeaveCommand((PlayerMobile)e.Mobile);
            #endregion
		}

		private static void EventSink_Connected(ConnectedEventArgs e)
		{
			var pm = e.Mobile as PlayerMobile;

			if (pm != null)
			{
				pm.SessionStart = DateTime.UtcNow;

				if (pm.Quest != null)
				{
					pm.Quest.StartTimer();
				}

				pm.BedrollLogout = false;
				pm.LastOnline = DateTime.UtcNow;
			}

			DisguiseTimers.StartTimer(e.Mobile);

			//Timer.DelayCall<Mobile>( TimeSpan.Zero, ClearSpecialMovesCallback, e.Mobile );
		}

		private static void EventSink_Disconnected(DisconnectedEventArgs e)
		{
			Mobile from = e.Mobile;
			DesignContext context = DesignContext.Find(from);

			if (context != null)
			{
				/* Client disconnected
				 *  - Remove design context
				 *  - Eject all from house
				 *  - Restore relocated entities
				 */

				// Remove design context
				DesignContext.Remove(from);

				// Eject all from house
				from.RevealingAction();

				foreach (Item item in context.Foundation.GetItems())
				{
					item.Location = context.Foundation.BanLocation;
				}

				foreach (Mobile mobile in context.Foundation.GetMobiles())
				{
					mobile.Location = context.Foundation.BanLocation;
				}

				// Restore relocated entities
				context.Foundation.RestoreRelocatedEntities();
			}

			var pm = e.Mobile as PlayerMobile;

			if (pm != null)
			{
				pm.m_GameTime += (DateTime.UtcNow - pm.SessionStart);

				if (pm.Quest != null)
				{
					pm.Quest.StopTimer();
				}

				pm.SpeechLog = null;
				pm.LastOnline = DateTime.UtcNow;
			}

			DisguiseTimers.StopTimer(from);
		}

		public override void RevealingAction(bool disruptive)
		{
			if (DesignContext != null)
			{
				return;
			}

			InvisibilitySpell.RemoveTimer(this);

			base.RevealingAction(disruptive);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override bool Hidden
		{
			get { return base.Hidden; }
			set
			{
				base.Hidden = value;

				//Always remove, default to the hiding icon EXCEPT in the invis spell where it's explicitly set
				RemoveBuff(BuffIcon.Invisibility);

				if (!Hidden)
				{
					RemoveBuff(BuffIcon.HidingAndOrStealth);
				}
				else // if( !InvisibilitySpell.HasTimer( this ) )
				{
					//Hidden/Stealthing & You Are Hidden
					BuffInfo.AddBuff(this, new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));
				}
			}
		}

		public override void OnSubItemAdded(Item item)
		{
			if (AccessLevel >= AccessLevel.GameMaster || !item.IsChildOf(Backpack))
			{
				return;
			}

			int maxWeight = WeightOverloading.GetMaxWeight(this);
			int curWeight = BodyWeight + TotalWeight;

			if (curWeight > maxWeight)
			{
				SendLocalizedMessage(1019035, true, String.Format(" : {0} / {1}", curWeight, maxWeight));
			}
		}

		public override bool CanBeHarmful(Mobile target, bool message, bool ignoreOurBlessedness)
		{
			if (DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).DesignContext != null))
			{
				return false;
			}

			if (NameMod != null)
			{
                var battle = AutoPvP.FindBattle(this) as UOF_PvPBattle;

				if (Notoriety.Compute(this, target) == Notoriety.Ally && (battle == null || !battle.IncognitoMode))
				{
					// this is during a check in which a player is trying to perform a negative act upon another player.
					// ... if they are incognitoed in any way and they are being harmful to a guild or alliance member
					// then they become un-incognito
					NameMod = null;

					// remove disguises (if any)
					SetHairMods(-1, -1);

					if (!CanBeginAction(typeof(IncognitoSpell)))
					{
						BodyMod = 0;
						HueMod = -1;
						NameMod = null;
						EndAction(typeof(IncognitoSpell));

						BaseArmor.ValidateMobile(this);
						BaseClothing.ValidateMobile(this);
					}

					DisguiseTimers.RemoveTimer(this);

					SendMessage(38, "Your ally sees through your disguise!");
					target.SendMessage(38, "You see through " + RawName + "'s disguise!");
				}
			}

			if ((target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier)
			{
				if (message)
				{
					if (target.Title == null)
					{
						SendMessage("{0} the vendor cannot be harmed.", target.Name);
					}
					else
					{
						SendMessage("{0} {1} cannot be harmed.", target.Name, target.Title);
					}
				}

				return false;
			}

			return base.CanBeHarmful(target, message, ignoreOurBlessedness);
		}

		public override bool CanBeBeneficial(Mobile target, bool message, bool allowDead)
		{
			if (DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).DesignContext != null))
			{
				return false;
			}

			return base.CanBeBeneficial(target, message, allowDead);
		}

		public override bool CheckContextMenuDisplay(IEntity target)
		{
			return (DesignContext == null);
		}

		public override void OnItemAdded(Item item)
		{
			base.OnItemAdded(item);

			if (item is BaseArmor || item is BaseWeapon)
			{
				Hits = Hits;
				Stam = Stam;
				Mana = Mana;
			}

			if (NetState != null)
			{
				CheckLightLevels(false);
			}

            EventSink.InvokeMobileInvalidate(new MobileInvalidateEventArgs(this));
		}

		public override void OnItemRemoved(Item item)
		{
			base.OnItemRemoved(item);

			if (item is BaseArmor || item is BaseWeapon)
			{
				Hits = Hits;
				Stam = Stam;
				Mana = Mana;
			}

			if (NetState != null)
			{
				CheckLightLevels(false);
			}

            EventSink.InvokeMobileInvalidate(new MobileInvalidateEventArgs(this));
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override double ArmorRating
		{
			get
			{
				//BaseArmor ar;
				double rating = 0.0;

				AddArmorRating(ref rating, NeckArmor);
				AddArmorRating(ref rating, HandArmor);
				AddArmorRating(ref rating, HeadArmor);
				AddArmorRating(ref rating, ArmsArmor);
				AddArmorRating(ref rating, LegsArmor);
				AddArmorRating(ref rating, ChestArmor);
				AddArmorRating(ref rating, ShieldArmor);

				return VirtualArmor + VirtualArmorMod + rating;
			}
		}

		private static void AddArmorRating(ref double rating, Item armor)
		{
			var ar = armor as BaseArmor;

			if (ar != null)
			{
			    if (armor is BaseShield)
			    {
                    rating += ar.ArmorRating(null);
			    }
			    else
			    {
                    rating += ar.ArmorRatingScaled(null);			        
			    }

			}
		}

		#region [Stats]Max
		[CommandProperty(AccessLevel.GameMaster)]
		public override int HitsMax
		{
			get
			{
				int strOffs = GetStatOffset(StatType.Str);

                int diff = 0;

				if (AccessLevel == AccessLevel.Player)
				{
                    var template = PvPTemplates.PvPTemplates.FetchProfile(this);
                    if (template.Active && template.Selected != null && template.Selected.Stats != null && template.Selected.Stats.ContainsKey(StatType.Str))
                    {
                        diff = template.Selected.Stats[StatType.Str] - RawStr;
                    }

					return Math.Min((RawStr / 2) + (50 - diff/2) + strOffs, 120);
				}

				return (RawStr / 2) + 50 + strOffs;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int StamMax { get { return base.StamMax + AosAttributes.GetValue(this, AosAttribute.BonusStam); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public override int ManaMax { get { return base.ManaMax + AosAttributes.GetValue(this, AosAttribute.BonusMana) + (EraML && Race == Race.Elf ? 20 : 0); } }
		#endregion

		#region Stat Getters/Setters
		[CommandProperty(AccessLevel.GameMaster)]
		public override int Str
		{
			get
			{
				if (EraML && AccessLevel == AccessLevel.Player)
				{
					return Math.Min(base.Str, 150);
				}

				return base.Str;
			}
			set { base.Str = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int Int
		{
			get
			{
				if (EraML && AccessLevel == AccessLevel.Player)
				{
					return Math.Min(base.Int, 150);
				}

				return base.Int;
			}
			set { base.Int = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int Dex
		{
			get
			{
				if (EraML && AccessLevel == AccessLevel.Player)
				{
					return Math.Min(base.Dex, 150);
				}

				return base.Dex;
			}
			set { base.Dex = value; }
		}
		#endregion

		public override bool Move(Direction d)
		{
			NetState ns = NetState;

			if (ns != null && HasGump(typeof(ResurrectGump)))
			{
				if (Alive)
				{
					CloseGump(typeof(ResurrectGump));
				}
				else
				{
					SendLocalizedMessage(500111); // You are frozen and cannot move.
					return false;
				}
			}

			TimeSpan speed = ComputeMovementSpeed(d);

			if (!Alive)
			{
				MovementImpl.IgnoreMovableImpassables = true;
			}

			bool res = base.Move(d);

			MovementImpl.IgnoreMovableImpassables = false;

			if (res)
			{
				m_NextMovementTime += speed;
			}

			return res;
		}

		public override bool CheckMovement(Direction d, out int newZ)
		{
			DesignContext context = DesignContext;

			if (context == null)
			{
				return base.CheckMovement(d, out newZ);
			}

			HouseFoundation foundation = context.Foundation;

			newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

			int newX = X, newY = Y;

			Movement.Movement.Offset(d, ref newX, ref newY);

			int startX = foundation.X + foundation.Components.Min.X + 1;
			int startY = foundation.Y + foundation.Components.Min.Y + 1;
			int endX = startX + foundation.Components.Width - 1;
			int endY = startY + foundation.Components.Height - 2;

			return (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map);
		}

		public override bool AllowItemUse(Item item)
		{
			#region Dueling
			if (DuelContext != null && !DuelContext.AllowItemUse(this, item))
			{
				return false;
			}
			#endregion

			return DesignContext.Check(this);
		}

		public SkillName[] AnimalFormRestrictedSkills { get { return m_AnimalFormRestrictedSkills; } }

		private readonly SkillName[] m_AnimalFormRestrictedSkills = new[]
		{
			SkillName.ArmsLore, SkillName.Begging, SkillName.Discordance, SkillName.Forensics, SkillName.Inscribe,
			SkillName.ItemID, SkillName.Meditation, SkillName.Peacemaking, SkillName.Provocation, SkillName.RemoveTrap,
			SkillName.SpiritSpeak, SkillName.Stealing, SkillName.TasteID
		};

		public override bool AllowSkillUse(SkillName skill)
		{
			#region Dueling
			if (DuelContext != null && !DuelContext.AllowSkillUse(this, skill))
			{
				return false;
			}
			#endregion

            Skill tskill = Skills[skill];
            Conquests.CheckProgress<SkillConquest>(this, tskill);

			return DesignContext.Check(this);
		}

		private bool m_LastProtectedMessage;
		private int m_NextProtectionCheck = 10;

		public virtual void RecheckTownProtection()
		{
			m_NextProtectionCheck = 10;

			var reg = (GuardedRegion)Region.GetRegion(typeof(GuardedRegion));
			bool isProtected = (reg != null && !reg.IsDisabled());

			if (isProtected == m_LastProtectedMessage)
			{
				return;
			}

			// You are now under the protection of the town guards. : You have left the protection of the town guards.
			SendLocalizedMessage(isProtected ? 500112 : 500113);

			m_LastProtectedMessage = isProtected;
		}

		public override void MoveToWorld(Point3D loc, Map map)
		{
			base.MoveToWorld(loc, map);

			RecheckTownProtection();
		}

		public override void SetLocation(Point3D loc, bool isTeleport)
		{
			if (!isTeleport && AccessLevel == AccessLevel.Player)
			{
				// moving, not teleporting
				int zDrop = (Location.Z - loc.Z);

				if (zDrop > 20) // we fell more than one story
				{
					Hits -= ((zDrop / 20) * 10) - 5; // deal some damage; does not kill, disrupt, etc
				}
			}

			base.SetLocation(loc, isTeleport);

			if (isTeleport || --m_NextProtectionCheck == 0)
			{
				RecheckTownProtection();
			}
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from.AccessLevel >= AccessLevel.GameMaster && from.AccessLevel >= AccessLevel && !CheckAlive())
			{
				list.Add(new GMRevivifyEntry(this));
			}

			if (from == this)
			{
				if (Quest != null)
				{
					Quest.GetContextMenuEntries(list);
				}

				if (Alive && InsuranceEnabled && EraAOS)
				{
					list.Add(new CallbackEntry(6201, ToggleItemInsurance));

					list.Add(
						AutoRenewInsurance
							? new CallbackEntry(6202, CancelRenewInventoryInsurance)
							: new CallbackEntry(6200, AutoRenewInventoryInsurance));
				}

				BaseHouse house = BaseHouse.FindHouseAt(this);

				if (house != null)
				{
					if (Alive && house.InternalizedVendors.Count > 0 && house.IsOwner(this))
					{
						list.Add(new CallbackEntry(6204, GetVendor));
					}

					if ( /*house.IsAosRules && */house.Owner != null && house.Owner.AccessLevel <= from.AccessLevel &&
												 !Region.IsPartOf(typeof(SafeZone)))
					{
						list.Add(new CallbackEntry(6207, LeaveHouse));
					}
				}

				if (Alive)
				{
					list.Add(new CallbackEntry(6210, ToggleChampionTitleDisplay));
				}

				if (EraHS)
				{
					NetState ns = from.NetState;

					if (ns != null && ns.ExtendedStatus)
					{
						// Allow Trades / Refuse Trades
						list.Add(new CallbackEntry(RefuseTrades ? 1154112 : 1154113, ToggleTrades));
					}
				}
                if (from is PlayerMobile && Conquests.CMOptions.ModuleEnabled)
                {
                    list.Add(new ConquestContextEntry(from, this));
                }
			}
			else
			{
				if (Alive /*&& EraAOS*/)
				{
					var theirParty = from.Party as Party;
					var ourParty = Party as Party;

					if (theirParty == null && ourParty == null)
					{
						list.Add(new AddToPartyEntry(from, this));
					}
					else if (theirParty != null && theirParty.Leader == from)
					{
						if (ourParty == null)
						{
							list.Add(new AddToPartyEntry(from, this));
						}
						else if (ourParty == theirParty)
						{
							list.Add(new RemoveFromPartyEntry(from, this));
						}
					}
				}

                if (from is PlayerMobile && Conquests.CMOptions.ModuleEnabled)
                {
                    list.Add(new ConquestContextEntry(from, this));
                }

				BaseHouse curhouse = BaseHouse.FindHouseAt(this);

				if (Alive && curhouse != null && /*curhouse.IsAosRules &&*/ curhouse.IsFriend(from))
				{
					list.Add(new EjectPlayerEntry(from, this));
				}
			}
		}

		#region Insurance
		private void ToggleItemInsurance()
		{
			if (!CheckAlive())
			{
				return;
			}

			BeginTarget(-1, false, TargetFlags.None, ToggleItemInsurance_Callback);
			SendLocalizedMessage(1060868); // Target the item you wish to toggle insurance status on <ESC> to cancel
		}

		private static bool CanInsure(Item item)
		{
			if ((item is Container && !(item is BaseQuiver)) || item is BagOfSending || item is KeyRing)
			{
				return false;
			}

			if ((item is Spellbook && item.LootType == LootType.Blessed) || item is Runebook || item is PotionKeg ||
				item is Sigil)
			{
				return false;
			}

			if (item.Stackable)
			{
				return false;
			}

			if (item.LootType == LootType.Cursed)
			{
				return false;
			}

			if (item.ItemID == 0x204E) // death shroud
			{
				return false;
			}

			return true;
		}

		private void ToggleItemInsurance_Callback(Mobile from, object obj)
		{
			if (!CheckAlive())
			{
				return;
			}

			var item = obj as Item;

			if (item == null || !item.IsChildOf(this))
			{
				BeginTarget(-1, false, TargetFlags.None, ToggleItemInsurance_Callback);

				// You can only insure items that you have equipped or that are in your backpack
				SendLocalizedMessage(1060871, "", 0x23);
			}
			else if (item.Insured)
			{
				item.Insured = false;

				SendLocalizedMessage(1060874, "", 0x35); // You cancel the insurance on the item

				BeginTarget(-1, false, TargetFlags.None, ToggleItemInsurance_Callback);

				// Target the item you wish to toggle insurance status on <ESC> to cancel
				SendLocalizedMessage(1060868, "", 0x23);
			}
			else if (!CanInsure(item))
			{
				BeginTarget(-1, false, TargetFlags.None, ToggleItemInsurance_Callback);
				SendLocalizedMessage(1060869, "", 0x23); // You cannot insure that
			}
			else if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied || item.BlessedFor == from)
			{
				BeginTarget(-1, false, TargetFlags.None, ToggleItemInsurance_Callback);
				SendLocalizedMessage(1060870, "", 0x23); // That item is blessed and does not need to be insured
				SendLocalizedMessage(1060869, "", 0x23); // You cannot insure that
			}
			else
			{
				if (!item.PaidInsurance)
				{
					Type cType = Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

					if (Banker.Withdraw(from, cType, 600))
					{
						SendMessage("{0:#,0} {1} has been withdrawn from your bank box.", 600, cType.Name);
						item.PaidInsurance = true;
					}
					else
					{
						SendLocalizedMessage(1061079, "", 0x23); // You lack the funds to purchase the insurance
						return;
					}
				}

				item.Insured = true;

				SendLocalizedMessage(1060873, "", 0x23); // You have insured the item

				BeginTarget(-1, false, TargetFlags.None, ToggleItemInsurance_Callback);

				// Target the item you wish to toggle insurance status on <ESC> to cancel
				SendLocalizedMessage(1060868, "", 0x23);
			}
		}

		private void AutoRenewInventoryInsurance()
		{
			if (!CheckAlive())
			{
				return;
			}

			// You have selected to automatically reinsure all insured items upon death
			SendLocalizedMessage(1060881, "", 0x23);
			AutoRenewInsurance = true;
		}

		private void CancelRenewInventoryInsurance()
		{
			if (!CheckAlive())
			{
				return;
			}

			if (EraSE)
			{
				if (!HasGump(typeof(CancelRenewInventoryInsuranceGump)))
				{
					SendGump(new CancelRenewInventoryInsuranceGump(this));
				}
			}
			else
			{
				// You have cancelled automatically reinsuring all insured items upon death
				SendLocalizedMessage(1061075, "", 0x23);
				AutoRenewInsurance = false;
			}
		}

		private class CancelRenewInventoryInsuranceGump : Gump
		{
			private readonly PlayerMobile m_Player;

			public CancelRenewInventoryInsuranceGump(PlayerMobile player)
				: base(250, 200)
			{
				m_Player = player;

				AddBackground(0, 0, 240, 142, 0x13BE);
				AddImageTiled(6, 6, 228, 100, 0xA40);
				AddImageTiled(6, 116, 228, 20, 0xA40);
				AddAlphaRegion(6, 6, 228, 142);

				// You are about to disable inventory insurance auto-renewal.
				AddHtmlLocalized(8, 8, 228, 100, 1071021, 0x7FFF, false, false);

				AddButton(6, 116, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
				AddHtmlLocalized(40, 118, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL

				AddButton(114, 116, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
				AddHtmlLocalized(148, 118, 450, 20, 1071022, 0x7FFF, false, false); // DISABLE IT!
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (!m_Player.CheckAlive())
				{
					return;
				}

				if (info.ButtonID == 1)
				{
					// You have cancelled automatically reinsuring all insured items upon death
					m_Player.SendLocalizedMessage(1061075, "", 0x23);
					m_Player.AutoRenewInsurance = false;
				}
				else
				{
					m_Player.SendLocalizedMessage(1042021); // Cancelled.
				}
			}
		}
		#endregion

		private void ToggleTrades()
		{
			RefuseTrades = !RefuseTrades;
		}

		private void GetVendor()
		{
			BaseHouse house = BaseHouse.FindHouseAt(this);

			if (!CheckAlive() || house == null || !house.IsOwner(this) || house.InternalizedVendors.Count <= 0)
			{
				return;
			}

			CloseGump(typeof(ReclaimVendorGump));
			SendGump(new ReclaimVendorGump(house));
		}

	    private void LeaveHouse()
	    {
	        BaseHouse house = BaseHouse.FindHouseAt(this);



	        if (house != null && DateTime.UtcNow > NextLeaveCommand)
	        {
	            NextLeaveCommand = DateTime.UtcNow + TimeSpan.FromSeconds(30);
	            Location = house.BanLocation;
	        }
	        else
	        {
                var nextuse = NextLeaveCommand - DateTime.UtcNow;
                SendMessage("You cannot use this command again for another " + nextuse.Seconds + " seconds.");
	        }
	    }

		private delegate void ContextCallback();

		private class CallbackEntry : ContextMenuEntry
		{
			private readonly ContextCallback m_Callback;

			public CallbackEntry(int number, ContextCallback callback)
				: this(number, -1, callback)
			{ }

			public CallbackEntry(int number, int range, ContextCallback callback)
				: base(number, range)
			{
				m_Callback = callback;
			}

			public override void OnClick()
			{
				if (m_Callback != null)
				{
					m_Callback();
				}
			}
		}

		public override void DisruptiveAction()
		{
			if (Meditating)
			{
				RemoveBuff(BuffIcon.ActiveMeditation);
			}

			base.DisruptiveAction();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (this == from && !Warmode)
			{
				IMount mount = Mount;

				if (mount != null && !DesignContext.Check(this))
				{
					return;
				}
			}

			base.OnDoubleClick(from);
		}

		public override void DisplayPaperdollTo(Mobile to)
		{
			if (DesignContext.Check(this))
			{
				base.DisplayPaperdollTo(to);
			}
		}

		private static bool m_NoRecursion;

		public override bool CheckEquip(Item item)
		{
			if (!base.CheckEquip(item))
			{
				return false;
			}

			#region Dueling
			if (DuelContext != null && !DuelContext.AllowItemEquip(this, item))
			{
				return false;
			}
			#endregion

			#region Factions
			FactionItem factionItem = FactionItem.Find(item);

			if (factionItem != null)
			{
				Faction faction = Faction.Find(this);

				if (faction == null)
				{
					SendLocalizedMessage(1010371); // You cannot equip a faction item!
					return false;
				}

				if (faction != factionItem.Faction)
				{
					SendLocalizedMessage(1010372); // You cannot equip an opposing faction's item!
					return false;
				}

				int maxWearables = FactionItem.GetMaxWearables(this);

				if (Items.Any(equipped => item != equipped && FactionItem.Find(equipped) != null && --maxWearables == 0))
				{
					SendLocalizedMessage(1010373); // You do not have enough rank to equip more faction items!
					return false;
				}
			}
			#endregion

			if (AccessLevel < AccessLevel.GameMaster && item.Layer != Layer.Mount && HasTrade)
			{
				BounceInfo bounce = item.GetBounce();

				if (bounce != null)
				{
					if (bounce.m_Parent is Item)
					{
						var parent = (Item)bounce.m_Parent;

						if (parent == Backpack || parent.IsChildOf(Backpack))
						{
							return true;
						}
					}
					else if (bounce.m_Parent == this)
					{
						return true;
					}
				}

				// You can only equip what you are already carrying while you have a trade pending.
				SendLocalizedMessage(1004042);
				return false;
			}

			return true;
		}

		public override bool CheckTrade(
			Mobile to, Item item, SecureTradeContainer cont, bool message, bool checkItems, int plusItems, int plusWeight)
		{
			int msgNum = 0;

			if (cont == null)
			{
			    var topm = to as PlayerMobile;
				if (to.Holding != null)
				{
					msgNum = 1062727; // You cannot trade with someone who is dragging something.
				}
				else if (HasTrade)
				{
					msgNum = 1062781; // You are already trading with someone else!
				}
				else if (to.HasTrade)
				{
					msgNum = 1062779; // That person is already involved in a trade
				}
				else if (to is PlayerMobile && ((PlayerMobile)to).RefuseTrades)
				{
					msgNum = 1154111; // ~1_NAME~ is refusing all trades
				}
                else if (topm != null && topm.PokerGame != null)
                {
                    SendMessage(61, "You cannot trade with another player while they are playing poker.");
                    return false;
                }
			}

			if (msgNum == 0)
			{
				if (cont != null)
				{
					plusItems += cont.TotalItems;
					plusWeight += cont.TotalWeight;
				}

				if (Backpack == null || !Backpack.CheckHold(this, item, false, checkItems, plusItems, plusWeight))
				{
					msgNum = 1004040; // You would not be able to hold this if the trade failed.
				}
				else if (to.Backpack == null || !to.Backpack.CheckHold(to, item, false, checkItems, plusItems, plusWeight))
				{
					msgNum = 1004039; // The recipient of this trade would not be able to carry this.
				}
				else
				{
					msgNum = CheckContentForTrade(item);
				}
			}

			if (msgNum != 0)
			{
				if (message)
				{
					SendLocalizedMessage(msgNum);
				}

				return false;
			}

			item.SendPropertiesTo(to);
			return true;
		}

		private static int CheckContentForTrade(Item item)
		{
			if (item is TrapableContainer && ((TrapableContainer)item).TrapType != TrapType.None)
			{
				return 1004044; // You may not trade trapped items.
			}

			if (StolenItem.IsStolen(item))
			{
				return 1004043; // You may not trade recently stolen items.
			}

			if (item is Container)
			{
				return item.Items.Select(CheckContentForTrade).FirstOrDefault(msg => msg != 0);
			}

			return 0;
		}

		public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
		{
			if (!base.CheckNonlocalDrop(from, item, target))
			{
				return false;
			}

			if (from.AccessLevel >= AccessLevel.GameMaster)
			{
				return true;
			}

			Container pack = Backpack;

			if (from == this && HasTrade && (target == pack || target.IsChildOf(pack)))
			{
				BounceInfo bounce = item.GetBounce();

				if (bounce != null && bounce.m_Parent is Item)
				{
					var parent = (Item)bounce.m_Parent;

					if (parent == pack || parent.IsChildOf(pack))
					{
						return true;
					}
				}

				SendLocalizedMessage(1004041); // You can't do that while you have a trade pending.
				return false;
			}

			return true;
		}

		protected override void OnLocationChange(Point3D oldLocation)
		{
			CheckLightLevels(false);

			#region Dueling
			if (DuelContext != null)
			{
				DuelContext.OnLocationChanged(this);
			}
			#endregion

			DesignContext context = DesignContext;

			if (context == null || m_NoRecursion)
			{
				ActionCams.OnPlayerMove(this, oldLocation);
				return;
			}

			m_NoRecursion = true;

			HouseFoundation foundation = context.Foundation;

			int newX = X, newY = Y;
			int newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

			int startX = foundation.X + foundation.Components.Min.X + 1;
			int startY = foundation.Y + foundation.Components.Min.Y + 1;
			int endX = startX + foundation.Components.Width - 1;
			int endY = startY + foundation.Components.Height - 2;

			if (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map)
			{
				if (Z != newZ)
				{
					Location = new Point3D(X, Y, newZ);
				}

				m_NoRecursion = false;
				return;
			}

			Map = foundation.Map;
			Location = new Point3D(foundation.X, foundation.Y, newZ);

			m_NoRecursion = false;

			ActionCams.OnPlayerMove(this, oldLocation);
		}

		public override bool OnMoveOver(Mobile m)
		{
			if (XmlScript.HasTrigger(m, TriggerName.onMoveOver) && UberScriptTriggers.Trigger(m, this, TriggerName.onMoveOver))
			{
				return false;
			}

			if (m.IgnoreMobiles || IgnoreMobiles)
			{
				return true;
			}

			var region1 = m.Region as CustomRegion;

			if (m is BaseCreature && !(((BaseCreature)m).Controlled || (!m.Deleted && m.NetState != null))) // pseudoseer
			{
				return (!Alive || !m.Alive || /*IsDeadBondedPet ||*/ m.IsDeadBondedPet) ||
					   (Hidden && AccessLevel > AccessLevel.Player) || region1 != null && region1.PlayingGame(m) ||
					   region1 != null && region1.MoveThrough();
			}

			#region Dueling
			if (Region.IsPartOf(typeof(SafeZone)) && m is PlayerMobile)
			{
				var pm = (PlayerMobile)m;

				if (pm.DuelContext == null || pm.DuelPlayer == null || !pm.DuelContext.Started || pm.DuelContext.Finished ||
					pm.DuelPlayer.Eliminated)
				{
					return true;
				}
			}
			#endregion

			return base.OnMoveOver(m);
		}

		public override bool CheckShove(Mobile shoved)
		{
			if (IgnoreMobiles /*|| TransformationSpellHelper.UnderTransformation(shoved, typeof(WraithFormSpell))*/)
			{
				return true;
			}

			var region1 = shoved.Region as CustomRegion;

			if (region1 != null && region1.PlayingGame(shoved) || region1 != null && region1.MoveThrough())
			{
				return true;
			}

			PvPBattle battle = AutoPvP.FindBattle(this);

			if (battle != null && battle.CanMoveThrough(this, shoved))
			{
				return true;
			}

			if (shoved is BaseCreature && ((BaseCreature)shoved).ControlMaster == this)
			{
				if (shoved.Alive)
				{
					SendLocalizedMessage(1019040);
				}

				return true;
			}

			return base.CheckShove(shoved);
		}

		protected override void OnMapChange(Map oldMap)
		{
			base.OnMapChange(oldMap);

			if ((!Faction.IsFactionFacet(Map) && Faction.IsFactionFacet(oldMap)) ||
				(Faction.IsFactionFacet(Map) && !Faction.IsFactionFacet(oldMap)))
			{
				InvalidateProperties();
			}

			#region Dueling
			if (DuelContext != null)
			{
				DuelContext.OnMapChanged(this);
			}
			#endregion

			DesignContext context = DesignContext;

			if (context == null || m_NoRecursion)
			{
				return;
			}

			m_NoRecursion = true;

			HouseFoundation foundation = context.Foundation;

			Map = foundation.Map;
			m_NoRecursion = false;
		}

		public override void OnBeneficialAction(Mobile target, bool isCriminal)
		{
			// THIS IS MESSY, but basically it gives us the option to determine:
			// if you are red and a criminal and another red or member of your faction heals you
			// does it make them Criminal?  Does are you Guard whackable?

			// Also, if you are a red and not a criminal, and you are healed by another red or member
			// of your faction, does it make them criminal / guardwhackable

			if (target.Kills >= 5)
			{
				if (target.Criminal)
				{
					if (!MurderSystemController._RedHealRedIsCriminal && !MurderSystemController._RedHealRedCrimIsCriminal &&
						Kills >= 5)
					{
						isCriminal = false;
					}

					if (!MurderSystemController._FactionHealRedCrimIsCriminal && !MurderSystemController._FactionHealRedIsCriminal &&
						target is PlayerMobile)
					{
						var pmtarget = (PlayerMobile)target;
						PlayerState targetState = PlayerState.Find(pmtarget);
						PlayerState fromState = PlayerState.Find(this);

						if (targetState != null && fromState != null && targetState.Faction == fromState.Faction)
						{
							isCriminal = false;
						}
					}

					if (isCriminal) // still a criminal action by this point, check if guardwhackable
					{
						if (!MurderSystemController._RedHealRedCrimIsGuardWhackable && Kills >= 5)
						{
							isCriminal = false;
							Criminal = true; // still make them criminal, just not guard whackable
						}

						if (!MurderSystemController._FactionHealRedCrimIsGuardWhackable && target is PlayerMobile)
						{
							var pmtarget = (PlayerMobile)target;
							PlayerState targetState = PlayerState.Find(pmtarget);
							PlayerState fromState = PlayerState.Find(this);

							if (targetState != null && fromState != null && targetState.Faction == fromState.Faction)
							{
								isCriminal = false;
								Criminal = true; // still make them criminal, just not guard whackable
							}
						}
					}
				}
				else if (isCriminal) // target is not criminal but is red
				{
					if (!MurderSystemController._RedHealRedIsCriminal && Kills >= 5)
					{
						isCriminal = false;
					}
					else if (!MurderSystemController._FactionHealRedIsCriminal && target is PlayerMobile)
					{
						var pmtarget = (PlayerMobile)target;
						PlayerState targetState = PlayerState.Find(pmtarget);
						PlayerState fromState = PlayerState.Find(this);

						if (targetState != null && fromState != null && targetState.Faction == fromState.Faction)
						{
							isCriminal = false;
						}
					}
					if (isCriminal) // still criminal by this point -- would be guardwhacked unless...
					{
						if (!MurderSystemController._RedHealRedIsGuardWhackable && Kills >= 5)
						{
							isCriminal = false;
							Criminal = true; // still make them criminal, just not guard whackable
						}

						if (!MurderSystemController._FactionHealRedIsGuardWhackable && target is PlayerMobile)
						{
							var pmtarget = (PlayerMobile)target;
							PlayerState targetState = PlayerState.Find(pmtarget);
							PlayerState fromState = PlayerState.Find(this);

							if (targetState != null && fromState != null && targetState.Faction == fromState.Faction)
							{
								isCriminal = false;
								Criminal = true; // still make them criminal, just not guard whackable
							}
						}
					}
				}
			}

			base.OnBeneficialAction(target, isCriminal);
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
		    const int disruptThreshold = 0;

			if (amount > disruptThreshold)
			{
				BandageContext c = BandageContext.GetContext(this);

				if (c != null)
				{
					c.Slip();
				}
			}

			WeightOverloading.FatigueOnDamage(this, amount);

			if (willKill && from is PlayerMobile)
			{
				Timer.DelayCall(TimeSpan.FromSeconds(10), ((PlayerMobile)from).RecoverAmmo);
			}

			XmlAttach.CheckOnHit(this, from);

			var captureZone = Region as CaptureZoneRegion;

			if (CaptureZone != null && from != null && captureZone != null && CaptureZone.Active)
			{
				Mobile creditMob = null;
				double points = amount;

				if (from is BaseCreature)
				{
					var bc = (BaseCreature)from;

					if (bc.ControlMaster is PlayerMobile)
					{
						creditMob = bc.ControlMaster;
					}
					else if (bc.SummonMaster is PlayerMobile)
					{
						creditMob = bc.SummonMaster;
					}
					else if (bc.BardMaster is PlayerMobile)
					{
						creditMob = bc.BardMaster;
					}
				}
				else if (from is PlayerMobile)
				{
					creditMob = from;
				}

				// add to the player score 
				if (creditMob != null)
				{
					if (!CaptureZone.PlayerScores.ContainsKey(creditMob))
					{
						CaptureZone.PlayerScores.Add(creditMob, points);
					}
					else
					{
						double current = CaptureZone.PlayerScores[creditMob];

						current += points;

						if (current > CaptureZone.MaxKillPoints)
						{
							current = CaptureZone.MaxKillPoints;
						}

						CaptureZone.PlayerScores[creditMob] = current;
					}
				}
			}

			base.OnDamage(amount, from, willKill);
		}

		public override void Resurrect()
		{
			bool wasAlive = Alive;

			base.Resurrect();

			if (Alive && !wasAlive)
			{
				Item deathRobe = new DeathRobe();

				if (!EquipItem(deathRobe))
				{
					deathRobe.Delete();
				}
			}

			FactionDeath = false;
		}

		public override double RacialSkillBonus { get { return EraML && Race == Race.Human ? 20.0 : 0; } }

		public override void OnWarmodeChanged()
		{
			if (!Warmode)
			{
				Timer.DelayCall(TimeSpan.FromSeconds(10), RecoverAmmo);
			}
		}

		private Mobile m_InsuranceAward;
		private int m_InsuranceCost;
		private int m_InsuranceBonus;

		public List<Item> EquipSnapshot { get; private set; }

		public override bool OnBeforeDeath()
		{
			if (LastKiller != null && CreaturePossession.HasAnyPossessPermissions(LastKiller))
			{
				LoggingCustom.LogPseudoseer(
					DateTime.Now + "\t" + LastKiller.Account + "\t" + LastKiller.Name + "\tkilled player\t" + Account + "\t" + this +
					"\tRegion: " + Region + "\tLocation: " + Location);
			}

			EquipSnapshot = new List<Item>(Items);

			m_NonAutoreinsuredItems = 0;
			m_InsuranceCost = 0;
			m_InsuranceAward = FindMostRecentDamager(false);

			if (m_InsuranceAward is BaseCreature)
			{
				Mobile master = ((BaseCreature)m_InsuranceAward).GetMaster();

				if (master != null)
				{
					m_InsuranceAward = master;
				}
			}

			if (m_InsuranceAward != null && (!m_InsuranceAward.Player || m_InsuranceAward == this))
			{
				m_InsuranceAward = null;
			}

			if (m_InsuranceAward is PlayerMobile)
			{
				((PlayerMobile)m_InsuranceAward).m_InsuranceBonus = 0;
			}

			RecoverAmmo();

            #region SmoothMove#
            if (Transport != null)
                Transport.LeaveCommand(this);
            #endregion

			if (PseudoSeerStone.OnPlayerDeathUberScript != null)
			{
				XmlAttach.AttachTo(
					this,
					new XmlScript(PseudoSeerStone.OnPlayerDeathUberScript)
					{
						Name = "PLAYERDEATH_SCRIPT",
						Expiration = TimeSpan.FromSeconds(10.0)
					});
			}

			XmlQuest.RegisterKill(this, LastKiller);

			if (XmlScript.HasTrigger(this, TriggerName.onBeforeDeath))
			{
				UberScriptTriggers.Trigger(this, LastKiller, TriggerName.onBeforeDeath);
			}

			return base.OnBeforeDeath();
		}

		private bool CheckInsuranceOnDeath(Item item)
		{
			if (InsuranceEnabled && item.EraAOS && item.Insured)
			{
				#region Dueling
				if (m_DuelPlayer != null && DuelContext != null && DuelContext.Registered && DuelContext.Started &&
					!m_DuelPlayer.Eliminated)
				{
					return true;
				}
				#endregion

				Type cType = Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

				if (AutoRenewInsurance)
				{
					int cost = (m_InsuranceAward == null ? 600 : 300);

					if (Banker.Withdraw(this, cType, cost))
					{
						m_InsuranceCost += cost;
						item.PaidInsurance = true;

						SendMessage("{0:#,0} {1} has been withdrawn from your bank box.", 600, cType.Name);
					}
					else
					{
						// You lack the funds to purchase the insurance
						SendLocalizedMessage(1061079, "", 0x23);

						item.PaidInsurance = false;
						item.Insured = false;
						m_NonAutoreinsuredItems++;
					}
				}
				else
				{
					item.PaidInsurance = false;
					item.Insured = false;
				}

				if (m_InsuranceAward != null && Banker.Deposit(m_InsuranceAward, cType, 300) && m_InsuranceAward is PlayerMobile)
				{
					((PlayerMobile)m_InsuranceAward).m_InsuranceBonus += 300;
				}

				return true;
			}

			return false;
		}

		public override DeathMoveResult GetParentMoveResultFor(Item item)
		{
			if (CheckInsuranceOnDeath(item))
			{
				return DeathMoveResult.MoveToBackpack;
			}

			var custreg = Region as CustomRegion;

			if (custreg != null && custreg.Controller.NoPlayerItemDrop)
			{
				return DeathMoveResult.MoveToBackpack;
			}

			DeathMoveResult res = base.GetParentMoveResultFor(item);

			if (res == DeathMoveResult.MoveToCorpse && item.Movable && Young)
			{
				res = DeathMoveResult.MoveToBackpack;
			}

			return res;
		}

		public override DeathMoveResult GetInventoryMoveResultFor(Item item)
		{
			if (CheckInsuranceOnDeath(item))
			{
				return DeathMoveResult.MoveToBackpack;
			}

			var custreg = Region as CustomRegion;

			if (custreg != null && custreg.Controller.NoPlayerItemDrop)
			{
				return DeathMoveResult.MoveToBackpack;
			}

			DeathMoveResult res = base.GetInventoryMoveResultFor(item);

			if (res == DeathMoveResult.MoveToCorpse && item.Movable && Young)
			{
				res = DeathMoveResult.MoveToBackpack;
			}

			return res;
		}

        public override void CriminalAction(bool message)
        {
            base.CriminalAction(message);


            // Added this to guard whack pets and summons when people go criminal
            if (this.AllFollowers != null)
            {
                foreach (Mobile m in this.AllFollowers.ToArray())
                {
                    if (m.Deleted || !m.Alive || m.GuardImmune)
                        continue;

                    m.Criminal = true;
                    this.Region.OnCriminalAction(m, false);
                }
            }           
        }

		public override void OnDeath(Container c)
		{
			if (m_NonAutoreinsuredItems > 0)
			{
				SendLocalizedMessage(1061115); // You do not have the gold to automatically reinsure all your items.
			}

			if (c is Corpse && ((Corpse)c).ProxyCorpse != null)
			{
				// proxy corpse used for Mobs with human bodies
				if (XmlScript.HasTrigger(this, TriggerName.onDeath))
				{
					UberScriptTriggers.Trigger(this, LastKiller, TriggerName.onDeath, ((Corpse)c).ProxyCorpse);
				}
			}
			else if (XmlScript.HasTrigger(this, TriggerName.onDeath))
			{
				UberScriptTriggers.Trigger(this, LastKiller, TriggerName.onDeath, c);
			}

			base.OnDeath(c);

            Conquests.HandlePlayerDeath(new PlayerConquestContainer(this, LastKiller, c));

			foreach (BaseCreature bc in
				AllFollowers.OfType<BaseCreature>()
							.Where(
								bc =>
								bc.ControlOrder == OrderType.Guard &&
								Utility.RandomDouble() <= DynamicSettingsController.PetNoGuardAfterDeathChance))
			{
				bc.ControlTarget = this;
				bc.ControlOrder = OrderType.Follow;
			}

			EquipSnapshot = null;

			HueMod = -1;
			NameMod = null;
			SavagePaintExpiration = TimeSpan.Zero;
		    BattlePaintExpirationRed = TimeSpan.Zero;
            BattlePaintExpirationShadow = TimeSpan.Zero;
            HalloweenPaintExpirationOrange = TimeSpan.Zero;
            HalloweenPaintExpirationPurple = TimeSpan.Zero;
            ZombiePaintExperationBooger = TimeSpan.Zero;
            ZombiePaintExperationVesper = TimeSpan.Zero;

			SetHairMods(-1, -1);

			PolymorphSpell.StopTimer(this);
			IncognitoSpell.StopTimer(this);
			DisguiseTimers.RemoveTimer(this);

			EndAction(typeof(PolymorphSpell));
			EndAction(typeof(IncognitoSpell));

			MeerMage.StopEffect(this, false);

			//SkillHandlers.StolenItem.ReturnOnDeath(this, c);

			List<XmlTeam> teams = XmlAttach.GetTeams(this);

			if (teams != null)
			{
				try
				{
					foreach (XmlTeam team in teams)
					{
						// copy a new XmlTeam attachment onto the corpse, but no scripts
						XmlAttach.AttachTo(
							c,
							new XmlTeam
							{
								TeamVal = team.TeamVal,
								TeamGreen = team.TeamGreen
							});
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("Attach team to corpse error: " + e.Message);
					Console.WriteLine(e.StackTrace);
				}
			}

			if (m_PermaFlags.Count > 0)
			{
				m_PermaFlags.Clear();

				if (c is Corpse)
				{
					((Corpse)c).Criminal = true;
				}

				if (Stealing.ClassicMode)
				{
					Criminal = true;
				}
			}

			Mobile killer = FindMostRecentDamager(true);

			var captureZone = Region as CaptureZoneRegion;

			if (killer is BaseCreature)
			{
				var bc = (BaseCreature)killer;

				Mobile master = bc.GetMaster();

				if (master != null)
				{
					killer = master;

					var mobile = killer as PlayerMobile;

					if (mobile != null && Kills >= 5 && captureZone == null)
					{
						mobile.MurderersKilled++;

						if (DateTime.UtcNow > Lastkilled + TimeSpan.FromMinutes(10.0) && mobile.Address != Address)
						{
							Lastkilled = DateTime.UtcNow;
							mobile.ValorPoints++;
							mobile.SendMessage(44, "You have gained a valor point for killing: " + RawName + ".");
						}
					}
				}
			}

			var playerMobile = killer as PlayerMobile;

			if (playerMobile != null && captureZone == null && Kills >= 5)
			{
				playerMobile.MurderersKilled++;

				if (DateTime.UtcNow > Lastkilled + TimeSpan.FromMinutes(10.0) && playerMobile.Address != Address)
				{
					playerMobile.ValorPoints++;
					Lastkilled = DateTime.UtcNow;
					playerMobile.SendMessage(44, "You have gained a valor point for killing: " + RawName + ".");
				}
			}

			if (Young && DuelContext == null)
			{
				if (YoungDeathTeleport())
				{
					Timer.DelayCall(TimeSpan.FromSeconds(2.5), SendYoungDeathNotice);
				}

				if (LastKiller is PlayerMobile)
				{
					foreach (PlayerMobile pm in OnlineCompanions)
					{
						if (Criminal)
						{
							pm.LocalOverheadMessage(
								MessageType.Regular,
								0x38,
								false,
								"Young player " + RawName + " was criminal and was killed by " + LastKiller.Name + "!");
						}
						else
						{
							pm.LocalOverheadMessage(
								MessageType.Regular, 0x38, false, "Young player " + RawName + " was just murdered by " + LastKiller.Name + "!");
						}
					}
				}
				else
				{
					foreach (PlayerMobile pm in OnlineCompanions)
					{
						pm.LocalOverheadMessage(MessageType.Regular, 0x38, false, "Young player " + RawName + " has died.");
					}
				}
			}

			if (DuelContext == null || !DuelContext.Registered || !DuelContext.Started || m_DuelPlayer == null ||
				m_DuelPlayer.Eliminated)
			{
				Faction.HandleDeath(this, killer);
			}

			Guilds.Guild.HandleDeath(this, killer);

			#region Dueling
			if (DuelContext != null)
			{
				DuelContext.OnDeath(this, c);
			}
			#endregion

			if (m_BuffTable != null)
			{
				m_BuffTable.Values.Where(buff => !buff.RetainThroughDeath).ForEach(RemoveBuff);
			}

			/*
			CustomRegion custreg = Region as CustomRegion;
			
			if ( custreg != null )
			{
				custreg.OnPlayerDeath( this );
			}
			*/
		}

		private List<Mobile> m_PermaFlags;
		private readonly List<Mobile> m_VisList;
		private readonly Hashtable m_AntiMacroTable;
		private TimeSpan m_GameTime;
		private DateTime m_ShortTermElapse;
		private DateTime m_NextSmithBulkOrder;
		private DateTime m_NextTailorBulkOrder;
		private DateTime m_SavagePaintExpiration;
        private DateTime m_BattlePaintExpirationShadow;
        private DateTime m_BattlePaintExpirationRed;
        private DateTime m_HalloweenPaintExpirationOrange;
        private DateTime m_HalloweenPaintExpirationPurple;
        private DateTime m_ZombiePaintExperationBooger;
        private DateTime m_ZombiePaintExperationVesper;
		private StaffRank m_StaffRank;
		private string m_StaffTitle;

		private static readonly string[] m_StaffTitles = new[]
		{
			//
			String.Empty, //
			"Trial Counselor", "Counselor", "Lead Counselor", //
			"Trial Seer", "Seer", "Lead Seer", //
			"Trial EM", "EM", "Lead EM", //
			"Trial GM", "GM", "Lead GM", //
			"Trial Admin", "Admin", "Lead Admin", //
			"Trial Dev", "Dev", "Lead Dev", //
			"Owner", "QM", "Emissary" //
		};

		public static string GetStaffTitle(StaffRank rank)
		{
			return m_StaffTitles[(int)rank];
		}

		public string GetStaffTitle()
		{
			if (m_StaffRank != StaffRank.None)
			{
				return m_StaffTitles[(int)m_StaffRank];
			}

			if (AccessLevel > AccessLevel.Player)
			{
				switch (AccessLevel)
				{
					case AccessLevel.Counselor:
						return m_StaffTitles[2];
					case AccessLevel.Seer:
						return m_StaffTitles[5];
					case AccessLevel.EventMaster:
						return m_StaffTitles[8];
					case AccessLevel.GameMaster:
						return m_StaffTitles[11];
					case AccessLevel.Lead:
						return "Lead";
					case AccessLevel.Administrator:
						return m_StaffTitles[14];
					case AccessLevel.Developer:
						return m_StaffTitles[17];
					case AccessLevel.Owner:
						return m_StaffTitles[19];
				}
			}

			return String.Empty;
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public SkillName Learning { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan SavagePaintExpiration
		{
			get
			{
				TimeSpan ts = m_SavagePaintExpiration - DateTime.UtcNow;

				if (ts < TimeSpan.Zero)
				{
					ts = TimeSpan.Zero;
				}

				return ts;
			}
			set { m_SavagePaintExpiration = DateTime.UtcNow + value; }
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan BattlePaintExpirationShadow
        {
            get
            {
                TimeSpan ts = m_BattlePaintExpirationShadow - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                {
                    ts = TimeSpan.Zero;
                }

                return ts;
            }
            set { m_BattlePaintExpirationShadow = DateTime.UtcNow + value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan BattlePaintExpirationRed
        {
            get
            {
                TimeSpan ts = m_BattlePaintExpirationRed - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                {
                    ts = TimeSpan.Zero;
                }

                return ts;
            }
            set { m_BattlePaintExpirationRed = DateTime.UtcNow + value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan HalloweenPaintExpirationOrange
        {
            get
            {
                TimeSpan ts = m_HalloweenPaintExpirationOrange - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                {
                    ts = TimeSpan.Zero;
                }

                return ts;
            }
            set { m_BattlePaintExpirationShadow = DateTime.UtcNow + value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan HalloweenPaintExpirationPurple
        {
            get
            {
                TimeSpan ts = m_HalloweenPaintExpirationPurple - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                {
                    ts = TimeSpan.Zero;
                }

                return ts;
            }
            set { m_BattlePaintExpirationRed = DateTime.UtcNow + value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ZombiePaintExperationBooger
        {
            get
            {
                TimeSpan ts = m_ZombiePaintExperationBooger - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                {
                    ts = TimeSpan.Zero;
                }

                return ts;
            }
            set { m_ZombiePaintExperationBooger = DateTime.UtcNow + value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ZombiePaintExperationVesper
        {
            get
            {
                TimeSpan ts = m_ZombiePaintExperationVesper - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                {
                    ts = TimeSpan.Zero;
                }

                return ts;
            }
            set { m_ZombiePaintExperationVesper = DateTime.UtcNow + value; }
        }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NextSmithBulkOrder
		{
			get
			{
				TimeSpan ts = m_NextSmithBulkOrder - DateTime.UtcNow;

				if (ts < TimeSpan.Zero)
				{
					ts = TimeSpan.Zero;
				}

				return ts;
			}
			set
			{
				try
				{
					m_NextSmithBulkOrder = DateTime.UtcNow + value;
				}
				catch
				{ }
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NextTailorBulkOrder
		{
			get
			{
				TimeSpan ts = m_NextTailorBulkOrder - DateTime.UtcNow;

				if (ts < TimeSpan.Zero)
				{
					ts = TimeSpan.Zero;
				}

				return ts;
			}
			set
			{
				try
				{
					m_NextTailorBulkOrder = DateTime.UtcNow + value;
				}
				catch
				{ }
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastEscortTime { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastPetBallTime { get; set; }

		[CommandProperty(AccessLevel.GameMaster, AccessLevel.Owner)]
		public StaffRank StaffRank
		{
			get { return m_StaffRank; }
			set
			{
				m_StaffRank = value;
				SetCustomFlag(CustomPlayerFlag.StaffRank, value > StaffRank.None);
			}
		}

		[CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
		public string StaffTitle
		{
			get { return String.IsNullOrEmpty(m_StaffTitle) ? GetStaffTitle() : m_StaffTitle; }
			set
			{
				m_StaffTitle = value;
				SetCustomFlag(CustomPlayerFlag.DisplayStaffRank, !String.IsNullOrEmpty(value));
			}
		}

		public static readonly TimeSpan ShortTermDuration = TimeSpan.FromHours(8.0);
		public static readonly TimeSpan LongTermDuration = TimeSpan.FromHours(24.0);

		public PlayerMobile()
		{
			AllFollowers = new List<Mobile>();
			AutoStabled = new List<Mobile>();
			RecentlyReported = new List<Mobile>();
			SkillGainMods = new List<SkillGainMod>();
			m_VisList = new List<Mobile>();
			m_PermaFlags = new List<Mobile>();
			m_AntiMacroTable = new Hashtable();

			BOBFilter = new BOBFilter();

			ChampionTitles = new ChampionTitleInfo();

			Learning = (SkillName)(-1);
			LastHelped = DateTime.MinValue;
			StatEnd = DateTime.MinValue;

			EventMsgFlag = true;

			m_GameTime = TimeSpan.Zero;
			m_ShortTermElapse = DateTime.UtcNow + ShortTermDuration;
			LongTermLastDecayGameTime = TimeSpan.Zero;

			m_GuildRank = RankDefinition.Lowest;

            //InvalidateLegends();
		}

		public PlayerMobile(Serial s)
			: base(s)
		{
			AllFollowers = new List<Mobile>();
			AutoStabled = new List<Mobile>();
			RecentlyReported = new List<Mobile>();
			SkillGainMods = new List<SkillGainMod>();
			m_VisList = new List<Mobile>();
			m_PermaFlags = new List<Mobile>();
			m_AntiMacroTable = new Hashtable();

			BOBFilter = new BOBFilter();

			ChampionTitles = new ChampionTitleInfo();

			Learning = (SkillName)(-1);
			LastHelped = DateTime.MinValue;
			StatEnd = DateTime.MinValue;

			EventMsgFlag = true;

            //InvalidateLegends();
		}

		public override bool MutateSpeech(List<Mobile> hears, ref string text, ref object context)
		{
			if (Alive)
			{
				return false;
			}

			if (EraML && Skills[SkillName.SpiritSpeak].Value >= 100.0)
			{
				return false;
			}

			if (EraAOS && hears.Any(m => m != this && m.Skills[SkillName.SpiritSpeak].Value >= 100.0))
			{
				return false;
			}

			return base.MutateSpeech(hears, ref text, ref context);
		}

		public override void DoSpeech(string text, int[] keywords, MessageType type, int hue)
		{
			if (Guilds.Guild.NewGuildSystem && (type == MessageType.Guild || type == MessageType.Alliance))
			{
				var g = Guild as Guild;

				if (g == null)
				{
					SendLocalizedMessage(1063142); // You are not in a guild!
					return;
				}

				if (type == MessageType.Alliance)
				{
					if (g.Alliance != null && g.Alliance.IsMember(g))
					{
						//g.Alliance.AllianceTextMessage( hue, "[Alliance][{0}]: {1}", this.Name, text );
						g.Alliance.AllianceChat(this, text);
						SendToStaffMessage(this, "[Alliance]: {0}", text);

						AllianceMessageHue = hue;
					}
					else
					{
						SendLocalizedMessage(1071020); // You are not in an alliance!
					}

					return;
				}

				/*if (type != MessageType.Guild)
				{
					return;
				}*/

				GuildMessageHue = hue;

				g.GuildChat(this, text);
				SendToStaffMessage(this, "[Guild]: {0}", text);
				return;
			}

            SpeechConquestContainer regArgs = new SpeechConquestContainer(this, text, type, hue, keywords);

            Conquests.HandleSpeech(regArgs);

		    if (IsArcade)
		    {
		        var wep = Weapon as BaseWeapon;
                if (wep != null && DateTime.UtcNow > NextArcadeSwingTime)
		        {
		            switch (text)
		            {
		                case "!9":
		                {
                            Arcade.Arcade.SwingDirection(this, Direction.North);
                            NextArcadeSwingTime = DateTime.UtcNow +
		                                      TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
		                    return;
		                }
		                case "!6":
		                {
                            Arcade.Arcade.SwingDirection(this, Direction.Right);
                            NextArcadeSwingTime = DateTime.UtcNow +
		                                      TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
		                    return;
		                }
		                case "!3":
		                {
                            Arcade.Arcade.SwingDirection(this, Direction.East);
                            NextArcadeSwingTime = DateTime.UtcNow +
		                                      TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
		                    return;
		                }
		                case "!2":
		                {
                            Arcade.Arcade.SwingDirection(this, Direction.Down);
                            NextArcadeSwingTime = DateTime.UtcNow +
		                                      TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
		                    return;
		                }
		                case "!1":
		                {
                            Arcade.Arcade.SwingDirection(this, Direction.South);
                            NextArcadeSwingTime = DateTime.UtcNow + TimeSpan.FromSeconds(0.6 * wep.DamageMax / 24);
		                    return;
		                }
		                case "!4":
		                {
		                    Arcade.Arcade.SwingDirection(this, Direction.Left);
                            NextArcadeSwingTime = DateTime.UtcNow +
		                                      TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
		                    return;
		                }
		                case "!7":
		                {
                            Arcade.Arcade.SwingDirection(this, Direction.West);
                            NextArcadeSwingTime = DateTime.UtcNow +
		                                      TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
		                    return;
		                }
		                case "!8":
		                {
                            Arcade.Arcade.SwingDirection(this, Direction.Up);
                            NextArcadeSwingTime = DateTime.UtcNow +
		                                      TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
		                    return;
		                }
		            }
		        }
		        else if ((text == "!2" || text == "!3" || text == "!4" || text == "!1" || text == "!6" || text == "!7" ||
                          text == "!8" || text == "!9") && DateTime.UtcNow <= NextArcadeSwingTime)
		        {
		            SendMessage(38, "You fumble your weapon attempting to swing it too quickly.");
                    NextArcadeSwingTime = DateTime.UtcNow +
		                              TimeSpan.FromSeconds(0.8 * Math.Pow(wep.DamageMax / 48.0, 0.5));
		            return;
		        }
		    }

		    base.DoSpeech(text, keywords, type, hue);
		}

		private static void SendToStaffMessage(Mobile m, string text, params object[] args)
		{
		    if (args != null && args.Length > 0)
			{
				text = String.Format(text, args);
			}

			Packet p = null;

			IPooledEnumerable list = m.GetClientsInRange(8);

			foreach (NetState ns in
				list.OfType<NetState>()
					.Not(
						ns =>
                        ns.Mobile == null || ns.Mobile.AccessLevel < AccessLevel.GameMaster || ns.Mobile.AccessLevel <= m.AccessLevel || ActionCams.IsCamera(ns.Mobile as PlayerMobile)))
			{
				if (p == null)
				{
					p =
						Packet.Acquire(
							new UnicodeMessage(m.Serial, m.Body, MessageType.Regular, m.SpeechHue, 3, m.Language, m.Name, text));
				}

				ns.Send(p);
			}

			list.Free();

			Packet.Release(p);
		}

		public override void Damage(int amount, Mobile from)
		{
			double bonus = 1.0;

			if (from is BaseCreature)
			{
				var bc = (BaseCreature)from;

				if (bc.Controlled && bc.ControlMaster is PlayerMobile)
				{
					// will actually decrease the damage usually
					bonus *= DynamicSettingsController.PetDamageToPlayersMultiplier;
				}
			}

			/* Per EA's UO Herald Pub48 (ML):
			 * ((resist spellsx10)/20 + 10=percentage of damage resisted)
			 */
			base.Damage((int)(amount * bonus), from);
		}

		#region Poison
		public override ApplyPoisonResult ApplyPoison(Mobile from, Poison poison)
		{
			if (!Alive)
			{
				return ApplyPoisonResult.Immune;
			}

			ApplyPoisonResult result = base.ApplyPoison(from, poison);

			if (from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer)
			{
				((PoisonImpl.PoisonTimer)PoisonTimer).From = from;
			}

			return result;
		}

		public override bool CheckPoisonImmunity(Mobile from, Poison poison)
		{
			if (Young && (DuelContext == null || !DuelContext.Started || DuelContext.Finished || AutoPvP.IsParticipant(this)))
			{
				return true;
			}

			return base.CheckPoisonImmunity(from, poison);
		}

		public override void OnPoisonImmunity(Mobile from, Poison poison)
		{
            if (Young && (DuelContext == null || !DuelContext.Started || DuelContext.Finished || AutoPvP.IsParticipant(this)))
			{
				// You would have been poisoned, were you not new to the land of Britannia. Be careful in the future.
				SendLocalizedMessage(502808);
			}
			else
			{
				base.OnPoisonImmunity(from, poison);
			}
		}
		#endregion

		public List<Mobile> VisibilityList { get { return m_VisList; } }

		public List<Mobile> PermaFlags { get { return m_PermaFlags; } }

		public override void DoHarmful(Mobile target, bool indirect) //Is this all types of aggression?
		{
			if (target is PlayerMobile)
			{
				// MODIFICATIONS FOR Capture the Flag / Double Dom games

				Player sourceEPL = EthicPlayer;
				Player targetEPL = ((PlayerMobile)target).EthicPlayer;

				var fromGuild = Guild as Guild;
				var targetGuild = target.Guild as Guild;

				if (target.Kills < MurderCount && !target.Criminal)
				{
					if (fromGuild == null || targetGuild == null ||
						(targetGuild != fromGuild && !fromGuild.IsAlly(targetGuild) && !fromGuild.IsEnemy(targetGuild)))
					{
						if (!Faction.IsFactionFacet(Map) || Faction.Find(target, true) == null || Faction.Find(this, true) == null)
						{
							//No factions here, or one or other is not in factions
							if (sourceEPL == null && targetEPL != null)
							{
								//Doing harmful to an evil/hero, while outside of the system
								targetEPL.Ethic.AddAggressor(this);
							}
						}
					}
				}
			}

			base.DoHarmful(target, indirect);
		}

		public override bool IsHarmfulCriminal(Mobile target)
		{
			if (Stealing.ClassicMode && target is PlayerMobile && ((PlayerMobile)target).m_PermaFlags.Count > 0)
			{
				int noto = Notoriety.Compute(this, target);

				if (noto == Notoriety.Innocent)
				{
					target.Delta(MobileDelta.Noto);
				}

				return false;
			}

			if (target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled)
			{
				return false;
			}

			if (EraML && target is BaseCreature && ((BaseCreature)target).Controlled &&
				this == ((BaseCreature)target).ControlMaster)
			{
				return false;
			}

			return base.IsHarmfulCriminal(target);
		}

		public bool AntiMacroCheck(Skill skill, object obj)
		{
			if (obj == null || m_AntiMacroTable == null || AccessLevel != AccessLevel.Player)
			{
				return true;
			}

			var tbl = (Hashtable)m_AntiMacroTable[skill];

			if (tbl == null)
			{
				m_AntiMacroTable[skill] = tbl = new Hashtable();
			}

			var count = (CountAndTimeStamp)tbl[obj];

			if (count != null)
			{
				if (count.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.UtcNow)
				{
					count.Count = 1;
					return true;
				}

				++count.Count;

				return count.Count <= SkillCheck.Allowance;
			}

			tbl[obj] = count = new CountAndTimeStamp();
			count.Count = 1;

			return true;
		}

		private void RevertHair()
		{
			SetHairMods(-1, -1);
		}

		public BOBFilter BOBFilter { get; private set; }

		public static void CheckAtrophies(Mobile m)
		{
			if (m is PlayerMobile)
			{
				ChampionTitleInfo.CheckAtrophy((PlayerMobile)m);
			}
		}

		public void CheckKillDecay()
		{
			if (LongTermLastDecayGameTime == TimeSpan.Zero)
			{
				LongTermLastDecayGameTime = GameTime;
			}

			if (m_ShortTermElapse < DateTime.UtcNow)
			{
				m_ShortTermElapse += ShortTermDuration;

				if (ShortTermMurders > 0)
				{
					--ShortTermMurders;
				}
			}

			if (LongTermDuration >= GameTime - LongTermLastDecayGameTime)
			{
				return;
			}

			LongTermLastDecayGameTime = GameTime;

			if (Kills > 0)
			{
				--Kills;
			}
		}

		public void ResetKillTime()
		{
			m_ShortTermElapse = DateTime.UtcNow + ShortTermDuration;
		}

		[CommandProperty(AccessLevel.GameMaster, true)]
		public DateTime SessionStart { get; private set; }

		[CommandProperty(AccessLevel.GameMaster, true)]
		public TimeSpan LongTermLastDecayGameTime { get; private set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan GameTime { get { return NetState != null ? m_GameTime + (DateTime.UtcNow - SessionStart) : m_GameTime; } }

		public override bool CanSee(Mobile m)
		{
			if (m is CharacterStatue)
			{
				((CharacterStatue)m).OnRequestedAnimation(this);
			}

			if (m is PlayerMobile && ((PlayerMobile)m).m_VisList.Contains(this))
			{
				return true;
			}

		    if (m.Alive && Party != null && PartyMembers != null && PartyMembers.Contains(m))
		        return true;

			if (DuelContext != null && m_DuelPlayer != null && !DuelContext.Finished && DuelContext.m_Tournament != null &&
				!m_DuelPlayer.Eliminated)
			{
				Mobile owner = m;

				if (owner is BaseCreature)
				{
					var bc = (BaseCreature)owner;

					if (bc.Controlled || bc.Summoned)
					{
						Mobile master = bc.GetMaster();

						if (master != null)
						{
							owner = master;
						}
					}
				}

				if (m.AccessLevel == AccessLevel.Player && owner is PlayerMobile && ((PlayerMobile)owner).DuelContext != DuelContext)
				{
					return false;
				}
			}

			return base.CanSee(m);
		}

		public override bool CanSee(Item item)
		{
			if (DesignContext != null && DesignContext.Foundation.IsHiddenToCustomizer(item))
			{
				return false;
			}

			return base.CanSee(item);
		}

		public override void OnAfterResurrect()
		{
			// resync client, so GhostVisible == false things are sent
			if (!Deleted && NetState != null)
			{
				PacketHandlers.Resynchronize(NetState, null);
			}

			base.OnAfterResurrect();
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			Faction faction = Faction.Find(this);

			if (faction != null)
			{
				faction.RemoveMember(this);
			}

			BaseHouse.HandleDeletion(this);
			DisguiseTimers.RemoveTimer(this);
		}

		public override bool NewGuildDisplay { get { return Guilds.Guild.NewGuildSystem; } }

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (!Faction.IsFactionFacet(Map))
			{
				return;
			}

			PlayerState pl = PlayerState.Find(this);

			if (pl == null)
			{
				return;
			}

			Faction faction = pl.Faction;

			if (faction.Commander == this)
			{
				// Commanding Lord of the ~1_FACTION_NAME~
				list.Add(1042733, faction.Definition.PropName);
			}
			else if (pl.Sheriff != null)
			{
				// The Sheriff of  ~1_CITY~, ~2_FACTION_NAME~
				list.Add(1042734, "{0}\t{1}", pl.Sheriff.Definition.FriendlyName, faction.Definition.PropName);
			}
			else if (pl.Finance != null)
			{
				// The Finance Minister of ~1_CITY~, ~2_FACTION_NAME~
				list.Add(1042735, "{0}\t{1}", pl.Finance.Definition.FriendlyName, faction.Definition.PropName);
			}
			else if (pl.MerchantTitle != MerchantTitle.None)
			{
				// ~1_val~, ~2_val~
				list.Add(1060776, "{0}\t{1}", MerchantTitles.GetInfo(pl.MerchantTitle).Title, faction.Definition.PropName);
			}
			else
			{
				list.Add(1060776, "{0}\t{1}", pl.Rank.Title, faction.Definition.PropName); // ~1_val~, ~2_val~
			}

			if (EraML)
			{
				for (int i = AllFollowers.Count - 1; i >= 0; i--)
				{
					var c = AllFollowers[i] as BaseCreature;

					if (c == null || c.ControlOrder != OrderType.Guard)
					{
						continue;
					}

					list.Add(501129); // guarded
					break;
				}
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			if (AccessLevel > AccessLevel.Player)
			{
				int hue = NameHue != -1 ? NameHue : 11;
				string name = Name ?? String.Empty;
				string title = StaffTitle;

				if (!String.IsNullOrEmpty(title))
				{
					name = String.Format("{1} {0}", name, title);
				}

				PrivateOverheadMessage(MessageType.Label, hue, AsciiClickMessage, name, from.NetState);

				return;
			}

            var battle = AutoPvP.FindBattle(this) as UOF_PvPBattle;

			if (Faction.IsFactionFacet(Map))
			{
				PlayerState pl = PlayerState.Find(this);

				if (pl != null && (battle == null || !battle.IncognitoMode))
				{
					string text;

					Faction faction = pl.Faction;

					if (faction.Commander == this)
					{
						text = String.Format("(Commanding {0} of the {1})", Female ? "Lady" : "Lord", faction.Definition.FriendlyName);
					}
					else if (pl.Sheriff != null)
					{
						text = String.Format(
							"(The Sheriff of {0}, {1})", pl.Sheriff.Definition.FriendlyName, faction.Definition.FriendlyName);
					}
					else if (pl.Finance != null)
					{
						text = String.Format(
							"(The Finance Minister of {0}, {1})", pl.Finance.Definition.FriendlyName, faction.Definition.FriendlyName);
					}
					else
					{
						text = String.Format(
							"({0}, {1})",
							pl.MerchantTitle != MerchantTitle.None
								? MerchantTitles.GetInfo(pl.MerchantTitle).Title.String
								: pl.Rank.Title.String,
							faction.Definition.FriendlyName);
					}

				    int hue = 0;
				    if (faction is TrueBritannians)
				    {
				        hue = faction.Definition.HuePrimary;
				    }
                    else if (faction is Minax)
                    {
                        hue = 2117;
                    }
                    else if (faction is Shadowlords)
                    {
                        hue = faction.Definition.HueSecondary;
                    }
                    else if (faction is CouncilOfMages)
                    {
                        hue = faction.Definition.HueSecondary;
                    }

				    PrivateOverheadMessage(MessageType.Label, hue, true, text, from.NetState);
				}
			}

			if (GuildClickMessage && (battle == null || !battle.IncognitoMode))
			{
				BaseGuild guild = Guild;

				if (guild != null && (DisplayGuildTitle || (Player && guild.Type != GuildType.Regular)))
				{
					string title = GuildTitle;
					string type;

					title = title == null ? "" : title.Trim();

					if (guild.Type >= 0 && (int)guild.Type < GuildTypes.Length)
					{
						type = GuildTypes[(int)guild.Type];
					}
					else
					{
						type = "";
					}

					string text = String.Format(title.Length <= 0 ? "[{1}]{2}" : "[{0}, {1}]{2}", title, guild.Abbreviation, type);

					PrivateOverheadMessage(MessageType.Regular, 0, true, text, from.NetState);
				}
			}

			if (AccessLevel == AccessLevel.Player)
			{
				int hue = NameHue != -1 ? NameHue : Notoriety.GetHue(Notoriety.Compute(from, this));
				string name = Name ?? String.Empty;
				string prefix = "";

                if ((Player || Body.IsHuman) && Fame >= 10000 && ValorTitle == null && CentralGump.EnsureProfile(this).FameTitle)
				{
					prefix = Female ? "Lady" : "Lord";
				}
				else if ((Player || Body.IsHuman) && ValorTitle != null)
				{
					prefix = ValorTitle;
				}

				string suffix = "";

				if (ClickTitle && !String.IsNullOrEmpty(Title))
				{
					suffix = Title;
				}

				suffix = ApplyNameSuffix(suffix);

				string val;

				if (prefix.Length > 0 && suffix.Length > 0)
				{
					val = String.Concat(prefix, " ", name, " ", suffix);
				}
				else if (prefix.Length > 0)
				{
					val = String.Concat(prefix, " ", name);
				}
				else if (suffix.Length > 0)
				{
					val = String.Concat(name, " ", suffix);
				}
				else
				{
					val = name;
				}

				PrivateOverheadMessage(MessageType.Label, hue, AsciiClickMessage, val, from.NetState);
			}

			base.OnSingleClick(from);
		}

		protected override bool OnMove(Direction d)
		{
			if (XmlScript.HasTrigger(this, TriggerName.onMove) && UberScriptTriggers.Trigger(this, this, TriggerName.onMove))
			{
				return false;
			}

			if (PokerGame != null)
			{
			    CloseGump(typeof(PokerLeaveGump));
				SendGump(new PokerLeaveGump(PokerGame));
				return false;
			}

			if (AccessLevel >= AccessLevel.Counselor)
			{
				return true;
			}

			/*if (!EraSE)
			{
				return base.OnMove(d);
			}*/

			if (Hidden && DesignContext.Find(this) == null) //Hidden & NOT customizing a house
			{
				if (!Mounted && Skills.Stealth.Value >= 25.0)
				{
					bool running = (d & Direction.Running) != 0;

					if (running)
					{
						if ((AllowedStealthSteps -= 2) <= 0)
						{
							RevealingAction();
						}
					}
					else if (AllowedStealthSteps-- <= 0)
					{
						Stealth.OnUse(this);
					}
				}
				else
				{
					RevealingAction();
				}
			}
            MovementConquestContainer e = MovementConquestContainer.Create(this, d);

            Conquests.HandleMovement(new MovementConquestContainer(this, d));

			return true;
		}

		public bool BedrollLogout { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public override bool Paralyzed
		{
			get { return base.Paralyzed; }
			set
			{
				base.Paralyzed = value;

				if (value)
				{
					AddBuff(new BuffInfo(BuffIcon.Paralyze, 1075827)); //You are frozen and can not move
				}
				else
				{
					RemoveBuff(BuffIcon.Paralyze);
				}
			}
		}

		#region Ethics
		[CommandProperty(AccessLevel.Counselor, AccessLevel.Lead)]
		public Player EthicPlayer { get; set; }
		#endregion

		#region Factions
		public PlayerState FactionPlayerState { get; set; }
		#endregion

		#region Dueling
		public DuelContext DuelContext { get; private set; }

		private DuelPlayer m_DuelPlayer;

		public DuelPlayer DuelPlayer
		{
			get { return m_DuelPlayer; }
			set
			{
				bool wasInTourny = (DuelContext != null && !DuelContext.Finished && DuelContext.m_Tournament != null);

				m_DuelPlayer = value;

				DuelContext = m_DuelPlayer == null ? null : m_DuelPlayer.Participant.Context;

				bool isInTourny = (DuelContext != null && !DuelContext.Finished && DuelContext.m_Tournament != null);

				if (wasInTourny != isInTourny)
				{
					SendEverything();
				}
			}
		}
		#endregion

		#region Quests
		public QuestSystem Quest { get; set; }

		public List<QuestRestartInfo> DoneQuests { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public SolenFriendship SolenFriendship { get; set; }
		#endregion

		public override void OnKillsChange(int oldValue)
		{
			if (Young && Kills > oldValue)
			{
				var acc = Account as Account;

				if (acc != null)
				{
					acc.RemoveYoungStatus(0);
				}
			}

		    EventSink.InvokeMobileInvalidate( new MobileInvalidateEventArgs( this) );
		}

		public override void OnGenderChanged(bool oldFemale)
		{
		    EventSink.InvokeMobileInvalidate( new MobileInvalidateEventArgs( this) );
		}

		public override void OnGuildChange(BaseGuild oldGuild)
		{
		    EventSink.InvokeMobileInvalidate( new MobileInvalidateEventArgs( this) );
		}

		public override void OnGuildTitleChange(string oldTitle)
		{
		    EventSink.InvokeMobileInvalidate( new MobileInvalidateEventArgs( this) );
		}

		public override void OnKarmaChange(int oldValue)
		{
   		    EventSink.InvokeMobileInvalidate( new MobileInvalidateEventArgs( this) );
		}

		public override void OnFameChange(int oldValue)
		{
		    EventSink.InvokeMobileInvalidate( new MobileInvalidateEventArgs( this) );
		}

		public override void OnSkillChange(SkillName skill, double oldBase)
		{
			if (Young && SkillsTotal >= 6000)
			{
				var acc = Account as Account;

				if (acc != null)
				{
					// You have successfully obtained a respectable skill level, 
					// and have outgrown your status as a young player!
					acc.RemoveYoungStatus(1019036);
				}
			}

		    EventSink.InvokeMobileInvalidate( new MobileInvalidateEventArgs( this) );
		}

		public override void OnAccessLevelChanged(AccessLevel oldLevel)
		{
			IgnoreMobiles = AccessLevel > AccessLevel.Player;

		    EventSink.InvokeMobileInvalidate( new MobileInvalidateEventArgs( this) );
		}

		public override void OnRawStatChange(StatType stat, int oldValue)
		{
		    EventSink.InvokeMobileInvalidate( new MobileInvalidateEventArgs( this) );
		}

		public override void OnDelete()
		{
		    EventSink.InvokeMobileInvalidate( new MobileInvalidateEventArgs( this) );
		}

		#region Fastwalk Prevention
		public static bool FastwalkPrevention = true;
		public static TimeSpan FastwalkThreshold = TimeSpan.FromSeconds(0.40);

		private DateTime m_NextMovementTime;
		private bool m_HasMoved;

		public override TimeSpan ComputeMovementSpeed(Direction dir, bool checkTurning)
		{
			if (checkTurning && (dir & Direction.Mask) != (Direction & Direction.Mask))
			{
				return RunMount; // We are NOT actually moving (just a direction change)
			}

			/*TransformContext context = TransformationSpellHelper.GetContext(this);

			if (context != null && context.Type == typeof(ReaperFormSpell))
			{
				return Mobile.WalkFoot;
			}*/

			bool running = (dir & Direction.Running) != 0;
			bool onHorse = Mounted || Flying;

			//AnimalFormContext animalContext = AnimalForm.GetContext(this);

			if (onHorse /* || (animalContext != null && animalContext.SpeedBoost)*/)
			{
				return running ? RunMount : WalkMount;
			}

			return running ? RunFoot : WalkFoot;
		}

		public static bool MovementThrottle_Callback(NetState ns)
		{
			if (!FastwalkPrevention)
			{
				return true;
			}

			var pm = ns.Mobile as PlayerMobile;

			if (pm == null || !pm.UsesFastwalkPrevention)
			{
				return true;
			}

			if (!pm.m_HasMoved)
			{
				// has not yet moved
				pm.LastMoveTime = DateTime.UtcNow;
				pm.m_HasMoved = true;
				return true;
			}



			TimeSpan ts = pm.m_NextMovementTime - DateTime.UtcNow;

			if (ts < TimeSpan.Zero)
			{
				// been a while since we've last moved
				pm.m_NextMovementTime = DateTime.UtcNow;
				return true;
			}

		    if (ts < FastwalkThreshold)
		    {
		        return true;
		    }

		    //pm.m_NextMovementTime = DateTime.UtcNow;
			return false;

			/*var e = new FastWalkEventArgs(ns);

			EventSink.InvokeFastWalk(e);

			return !e.Blocked;*/
		}
		#endregion

		#region Hair and beard mods
		private int m_HairModID = -1, m_HairModHue;
		private int m_BeardModID = -1, m_BeardModHue;

		public void SetHairMods(int hairID, int beardID)
		{
			if (hairID == -1)
			{
				InternalRestoreHair(true, ref m_HairModID, ref m_HairModHue);
			}
			else if (hairID != -2)
			{
				InternalChangeHair(true, hairID, ref m_HairModID, ref m_HairModHue);
			}

			if (beardID == -1)
			{
				InternalRestoreHair(false, ref m_BeardModID, ref m_BeardModHue);
			}
			else if (beardID != -2)
			{
				InternalChangeHair(false, beardID, ref m_BeardModID, ref m_BeardModHue);
			}
		}

		private void CreateHair(bool hair, int id, int hue)
		{
			if (hair)
			{
				//TODO Verification?
				HairItemID = id;
				HairHue = hue;
			}
			else
			{
				FacialHairItemID = id;
				FacialHairHue = hue;
			}
		}

		private void InternalRestoreHair(bool hair, ref int id, ref int hue)
		{
			if (id == -1)
			{
				return;
			}

			if (hair)
			{
				HairItemID = 0;
			}
			else
			{
				FacialHairItemID = 0;
			}

			//if( id != 0 )
			CreateHair(hair, id, hue);

			id = -1;
			hue = 0;
		}

		private void InternalChangeHair(bool hair, int id, ref int storeID, ref int storeHue)
		{
			if (storeID == -1)
			{
				storeID = hair ? HairItemID : FacialHairItemID;
				storeHue = hair ? HairHue : FacialHairHue;
			}

			CreateHair(hair, id, 0);
		}
		#endregion

		#region Young system
		public override bool IsYoung()
		{
			return Young;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Young
		{
			get { return GetFlag(PlayerFlag.Young); }
			set
			{
				SetFlag(PlayerFlag.Young, value);
				InvalidateProperties();
			}
		}

		public override string ApplyNameSuffix(string suffix)
		{
			if (Young)
			{
				suffix = suffix.Length == 0 ? "(Young)" : String.Concat(suffix, " (Young)");
			}
			else
			{
				#region Ethics
				if (EthicPlayer != null && string.IsNullOrEmpty(Title))
				{
					suffix = suffix.Length == 0 ? EthicPlayer.Title() : String.Concat(suffix, " ", EthicPlayer.Title());
				}
				#endregion
			}

			if (EraML && Faction.IsFactionFacet(Map))
			{
				Faction faction = Faction.Find(this);

				if (faction != null)
				{
					string adjunct = String.Format("[{0}]", faction.Definition.Abbreviation);

					suffix = suffix.Length == 0 ? adjunct : String.Concat(suffix, " ", adjunct);
				}
			}

			return base.ApplyNameSuffix(suffix);
		}

		public override TimeSpan GetLogoutDelay()
		{
			if (Young || BedrollLogout || TestCenter.Enabled)
			{
				return TimeSpan.Zero;
			}

			return base.GetLogoutDelay();
		}

		private DateTime m_LastYoungMessage = DateTime.MinValue;

		public bool CheckYoungProtection(Mobile from)
		{
		    return false;

			/*if (!Young)
			{
				return false;
			}

			if (Region is BaseRegion && !((BaseRegion)Region).YoungProtected)
			{
				return false;
			}

			if (from is BaseCreature && ((BaseCreature)from).IgnoreYoungProtection)
			{
				return false;
			}

			if (Quest != null && Quest.IgnoreYoungProtection(from))
			{
				return false;
			}

			if (DateTime.UtcNow - m_LastYoungMessage > TimeSpan.FromMinutes(1.0))
			{
				m_LastYoungMessage = DateTime.UtcNow;

				// A monster looks at you menacingly but does not attack. 
				// You would be under attack now if not for your status as a new citizen of Britannia.
				SendLocalizedMessage(1019067);
			}

			return true;*/
		}

		private DateTime m_LastYoungHeal = DateTime.MinValue;

		public bool CheckYoungHealTime()
		{
			if (DateTime.UtcNow - m_LastYoungHeal > TimeSpan.FromMinutes(5.0))
			{
				m_LastYoungHeal = DateTime.UtcNow;
				return true;
			}

			return false;
		}

		private static Point3D[] m_TrammelDeathDestinations = new[]
		{
			new Point3D(1481, 1612, 20), new Point3D(2708, 2153, 0), //
			new Point3D(2249, 1230, 0), new Point3D(5197, 3994, 37), //
			new Point3D(1412, 3793, 0), new Point3D(3688, 2232, 20), //
			new Point3D(2578, 604, 0), new Point3D(4397, 1089, 0), //
			new Point3D(5741, 3218, -2), new Point3D(2996, 3441, 15), //
			new Point3D(624, 2225, 0), new Point3D(1916, 2814, 0), //
			new Point3D(2929, 854, 0), new Point3D(545, 967, 0), //
			new Point3D(3665, 2587, 0)
		};

		private static Point3D[] m_IlshenarDeathDestinations = new[]
		{
			new Point3D(1216, 468, -13), new Point3D(723, 1367, -60), //
			new Point3D(745, 725, -28), new Point3D(281, 1017, 0), //
			new Point3D(986, 1011, -32), new Point3D(1175, 1287, -30), //
			new Point3D(1533, 1341, -3), new Point3D(529, 217, -44), //
			new Point3D(1722, 219, 96)
		};

		private static Point3D[] m_MalasDeathDestinations = new[]
		{
			//
			new Point3D(2079, 1376, -70), new Point3D(944, 519, -71)
		};

		private static Point3D[] m_TokunoDeathDestinations = new[]
		{
			//
			new Point3D(1166, 801, 27), new Point3D(782, 1228, 25), new Point3D(268, 624, 15)
		};

		public bool YoungDeathTeleport()
		{
			return false;
			/*
			if (Region.IsPartOf(typeof(Jail)) || Region.IsPartOf("Samurai start location") ||
				Region.IsPartOf("Ninja start location") || Region.IsPartOf("Ninja cave"))
			{
				return false;
			}

			Point3D loc;
			Map map;

			var dungeon = (DungeonRegion)Region.GetRegion(typeof(DungeonRegion));
			if (dungeon != null && dungeon.EntranceLocation != Point3D.Zero)
			{
				loc = dungeon.EntranceLocation;
				map = dungeon.EntranceMap;
			}
			else
			{
				loc = Location;
				map = Map;
			}
			
			Point3D[] list;

			if ( map == Map.Trammel )
				list = m_TrammelDeathDestinations;
			else if ( map == Map.Ilshenar )
				list = m_IlshenarDeathDestinations;
			else if ( map == Map.Malas )
				list = m_MalasDeathDestinations;
			else if ( map == Map.Tokuno )
				list = m_TokunoDeathDestinations;
			else
				return false;

			Point3D dest = Point3D.Zero;
			int sqDistance = int.MaxValue;

			for ( int i = 0; i < list.Length; i++ )
			{
				Point3D curDest = list[i];

				int width = loc.X - curDest.X;
				int height = loc.Y - curDest.Y;
				int curSqDistance = width * width + height * height;

				if ( curSqDistance < sqDistance )
				{
					dest = curDest;
					sqDistance = curSqDistance;
				}
			}

			this.MoveToWorld( dest, map );
			return true;
			*/
		}

		private void SendYoungDeathNotice()
		{
			SendGump(new YoungDeathNotice());
		}
		#endregion

		#region Speech log
		public SpeechLog SpeechLog { get; private set; }

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (SpeechLog.Enabled && NetState != null)
			{
				if (SpeechLog == null)
				{
					SpeechLog = new SpeechLog();
				}

				SpeechLog.Add(e.Mobile, e.Speech);
			}

			// if it's a counselor or companion, log to file
			if (e.Mobile == null || e.Mobile == this)
			{
				return;
			}

		    if (AccessLevel == AccessLevel.Counselor && PageQueue.Contains(e.Mobile))
			{
				LoggingCustom.LogCounselor(DateTime.Now + "\t" + e.Mobile.RawName + "(nearby " + RawName + ")\t" + e.Speech);
			}

			if (!Companion || !(e.Mobile is PlayerMobile) || !((PlayerMobile)e.Mobile).Young)
			{
				return;
			}

			LoggingCustom.Log(
				Path.Combine(new[] {CompanionListGump.LogFileLocation, RawName + ".txt"}),
				DateTime.Now + "\t" + e.Mobile.RawName + "(nearby " + RawName + ")\t" + e.Speech);
			LoggingCustom.LogCompanion(DateTime.Now + "\t" + e.Mobile.RawName + "(nearby " + RawName + ")\t" + e.Speech);
		}
		#endregion

		#region Champion Titles
		[CommandProperty(AccessLevel.GameMaster)]
		public bool DisplayChampionTitle
		{
			//
			get { return GetFlag(PlayerFlag.DisplayChampionTitle); }
			set { SetFlag(PlayerFlag.DisplayChampionTitle, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public ChampionTitleInfo ChampionTitles { get; private set; }

		public void ToggleChampionTitleDisplay()
		{
			if (!CheckAlive())
			{
                SendMessage(54, "You can only do this whilst alive!");
				return;
			}

			// You have chosen to hide your monster kill title. : You have chosen to display your monster kill title.
			SendLocalizedMessage(DisplayChampionTitle ? 1062419 : 1062418, "", 0x23);

			DisplayChampionTitle = !DisplayChampionTitle;
		}

		[PropertyObject]
		public class ChampionTitleInfo
		{
			public static TimeSpan LossDelay = TimeSpan.FromDays(1.0);
			public const int LossAmount = 90;

			private class TitleInfo
			{
				public int Value { get; set; }
				public DateTime LastDecay { get; set; }

				public TitleInfo()
				{ }

				public TitleInfo(GenericReader reader)
				{
					int version = reader.ReadEncodedInt();

					switch (version)
					{
						case 0:
							{
								Value = reader.ReadEncodedInt();
								LastDecay = reader.ReadDateTime();
							}
							break;
					}
				}

				public static void Serialize(GenericWriter writer, TitleInfo info)
				{
					writer.WriteEncodedInt(0); // version

					writer.WriteEncodedInt(info.Value);
					writer.Write(info.LastDecay);
				}
			}

			private TitleInfo[] m_Values;

			public int GetValue(ChampionSpawnType type)
			{
				return GetValue((int)type);
			}

			public void SetValue(ChampionSpawnType type, int value)
			{
				SetValue((int)type, value);
			}

			public void Award(ChampionSpawnType type, int value)
			{
				Award((int)type, value);
			}

			public int GetValue(int index)
			{
				if (m_Values == null || index < 0 || index >= m_Values.Length)
				{
					return 0;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				return m_Values[index].Value;
			}

			public DateTime GetLastDecay(int index)
			{
				if (m_Values == null || index < 0 || index >= m_Values.Length)
				{
					return DateTime.MinValue;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				return m_Values[index].LastDecay;
			}

			public void SetValue(int index, int value)
			{
				if (m_Values == null)
				{
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				if (value < 0)
				{
					value = 0;
				}

				if (index < 0 || index >= m_Values.Length)
				{
					return;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				m_Values[index].Value = value;
			}

			public void Award(int index, int value)
			{
				if (m_Values == null)
				{
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				if (index < 0 || index >= m_Values.Length || value <= 0)
				{
					return;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				m_Values[index].Value += value;
			}

			public void Atrophy(int index, int value)
			{
				if (m_Values == null)
				{
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				if (index < 0 || index >= m_Values.Length || value <= 0)
				{
					return;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				int before = m_Values[index].Value;

				if ((m_Values[index].Value - value) < 0)
				{
					m_Values[index].Value = 0;
				}
				else
				{
					m_Values[index].Value -= value;
				}

				if (before != m_Values[index].Value)
				{
					m_Values[index].LastDecay = DateTime.UtcNow;
				}
			}

			public override string ToString()
			{
				return "...";
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int Pestilence
			{
				//
				get { return GetValue(ChampionSpawnType.Pestilence); }
				set { SetValue(ChampionSpawnType.Pestilence, value); }
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int Abyss
			{
				//
				get { return GetValue(ChampionSpawnType.Abyss); }
				set { SetValue(ChampionSpawnType.Abyss, value); }
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int Arachnid
			{
				//
				get { return GetValue(ChampionSpawnType.Arachnid); }
				set { SetValue(ChampionSpawnType.Arachnid, value); }
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int ColdBlood
			{
				//
				get { return GetValue(ChampionSpawnType.ColdBlood); }
				set { SetValue(ChampionSpawnType.ColdBlood, value); }
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int ForestLord
			{
				//
				get { return GetValue(ChampionSpawnType.ForestLord); }
				set { SetValue(ChampionSpawnType.ForestLord, value); }
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int SleepingDragon
			{
				//
				get { return GetValue(ChampionSpawnType.SleepingDragon); }
				set { SetValue(ChampionSpawnType.SleepingDragon, value); }
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int UnholyTerror
			{
				//
				get { return GetValue(ChampionSpawnType.UnholyTerror); }
				set { SetValue(ChampionSpawnType.UnholyTerror, value); }
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int VerminHorde
			{
				//
				get { return GetValue(ChampionSpawnType.VerminHorde); }
				set { SetValue(ChampionSpawnType.VerminHorde, value); }
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int Necro
			{
				//
				get { return GetValue(ChampionSpawnType.Necro); }
				set { SetValue(ChampionSpawnType.Necro, value); }
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int Harrower { get; set; }

			public ChampionTitleInfo()
			{ }

			public ChampionTitleInfo(GenericReader reader)
			{
				int version = reader.ReadEncodedInt();

				switch (version)
				{
					case 0:
						{
							Harrower = reader.ReadEncodedInt();

							int length = reader.ReadEncodedInt();

							m_Values = new TitleInfo[length];

							for (int i = 0; i < length; i++)
							{
								m_Values[i] = new TitleInfo(reader);
							}

							if (m_Values.Length != ChampionSpawnInfo.Table.Length)
							{
								TitleInfo[] oldValues = m_Values;

								m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

								for (int i = 0; i < m_Values.Length && i < oldValues.Length; i++)
								{
									m_Values[i] = oldValues[i];
								}
							}
						}
						break;
				}
			}

			public static void Serialize(GenericWriter writer, ChampionTitleInfo titles)
			{
				writer.WriteEncodedInt(0); // version

				writer.WriteEncodedInt(titles.Harrower);

				int length = titles.m_Values.Length;

				writer.WriteEncodedInt(length);

				for (int i = 0; i < length; i++)
				{
					if (titles.m_Values[i] == null)
					{
						titles.m_Values[i] = new TitleInfo();
					}

					TitleInfo.Serialize(writer, titles.m_Values[i]);
				}
			}

			public static void CheckAtrophy(PlayerMobile pm)
			{
				ChampionTitleInfo t = pm.ChampionTitles;

				if (t == null)
				{
					return;
				}

				if (t.m_Values == null)
				{
					t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				for (int i = 0; i < t.m_Values.Length; i++)
				{
					if (t.GetLastDecay(i) + LossDelay < DateTime.UtcNow)
					{
						t.Atrophy(i, LossAmount);
					}
				}
			}

			/// <summary>
			///     Called when killing a harrower. Will give a minimum of 1 point.
			/// </summary>
			public static void AwardHarrowerTitle(PlayerMobile pm)
			{
				ChampionTitleInfo t = pm.ChampionTitles;

				if (t == null)
				{
					return;
				}

				if (t.m_Values == null)
				{
					t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				int count = 1 + t.m_Values.Count(t1 => t1.Value > 900);

				t.Harrower = Math.Max(count, t.Harrower); //Harrower titles never decay.
			}
		}
		#endregion

		#region Recipes
		private Dictionary<int, bool> m_AcquiredRecipes;

		public virtual bool HasRecipe(Recipe r)
		{
			if (r == null)
			{
				return false;
			}

			return HasRecipe(r.ID);
		}

		public virtual bool HasRecipe(int recipeID)
		{
			if (m_AcquiredRecipes != null && m_AcquiredRecipes.ContainsKey(recipeID))
			{
				return m_AcquiredRecipes[recipeID];
			}

			return false;
		}

		public virtual void AcquireRecipe(Recipe r)
		{
			if (r != null)
			{
				AcquireRecipe(r.ID);
			}
		}

		public virtual void AcquireRecipe(int recipeID)
		{
			if (m_AcquiredRecipes == null)
			{
				m_AcquiredRecipes = new Dictionary<int, bool>();
			}

			m_AcquiredRecipes[recipeID] = true;
		}

		public virtual void ResetRecipes()
		{
			m_AcquiredRecipes = null;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int KnownRecipes { get { return m_AcquiredRecipes != null ? m_AcquiredRecipes.Count : 0; } }
		#endregion

		#region Buff Icons
		private Dictionary<BuffIcon, BuffInfo> m_BuffTable;

		public void ResendBuffs()
		{
			if (!BuffInfo.Enabled || !EraML || m_BuffTable == null)
			{
				return;
			}

			NetState state = NetState;

			if (state == null || !state.BuffIcon)
			{
				return;
			}

			foreach (BuffInfo info in m_BuffTable.Values)
			{
				state.Send(new AddBuffPacket(this, info));
			}
		}

		public void AddBuff(BuffInfo b)
		{
			if (!BuffInfo.Enabled || !EraML || b == null)
			{
				return;
			}

			RemoveBuff(b); //Check & subsequently remove the old one.

			if (m_BuffTable == null)
			{
				m_BuffTable = new Dictionary<BuffIcon, BuffInfo>();
			}

			m_BuffTable.Add(b.ID, b);

			NetState state = NetState;

			if (state != null && state.BuffIcon)
			{
				state.Send(new AddBuffPacket(this, b));
			}
		}

		public void RemoveBuff(BuffInfo b)
		{
			if (b == null)
			{
				return;
			}

			RemoveBuff(b.ID);
		}

		public void RemoveBuff(BuffIcon b)
		{
			if (m_BuffTable == null || !m_BuffTable.ContainsKey(b))
			{
				return;
			}

			BuffInfo info = m_BuffTable[b];

			if (info.Timer != null && info.Timer.Running)
			{
				info.Timer.Stop();
			}

			m_BuffTable.Remove(b);

			NetState state = NetState;

			if (state != null && state.BuffIcon)
			{
				state.Send(new RemoveBuffPacket(this, b));
			}

			if (m_BuffTable.Count <= 0)
			{
				m_BuffTable = null;
			}
		}
		#endregion

		#region SkillGain Mod (not ML)
		public List<SkillGainMod> SkillGainMods { get; private set; }

		public bool RemoveSkillGainMod(string name)
		{
			for (int i = 0; i < SkillGainMods.Count; ++i)
			{
				SkillGainMod check = SkillGainMods[i];

				if (check.Name != name)
				{
					continue;
				}

				SendSkillGainNormalMessage(check.Skill);
				SkillGainMods.RemoveAt(i);
				return true;
			}

			return false;
		}

		public SkillGainMod GetSkillGainMod(string name)
		{
			for (int i = 0; i < SkillGainMods.Count; ++i)
			{
				SkillGainMod check = SkillGainMods[i];

				if (check.HasElapsed())
				{
					SendSkillGainNormalMessage(check.Skill);
					SkillGainMods.RemoveAt(i--);
				}
				else if (check.Name == name)
				{
					return check;
				}
			}

			return null;
		}

		public double GetSkillGainModBonus(SkillName skill)
		{
			double bonus = 0.0;

			for (int i = 0; i < SkillGainMods.Count; ++i)
			{
				SkillGainMod check = SkillGainMods[i];

				if (check.HasElapsed())
				{
					SendSkillGainNormalMessage(check.Skill);
					SkillGainMods.RemoveAt(i--);
				}
				else if (check.Skill == skill || check.Skill == (SkillName)(-1))
				{
					bonus += check.Bonus;
				}
			}

			return bonus;
		}

		public void AddSkillGainMod(string name, SkillName skill, double bonus, TimeSpan duration)
		{
			for (int i = 0; i < SkillGainMods.Count; ++i)
			{
				SkillGainMod check = SkillGainMods[i];

				if (check.Name == name)
				{
					SkillGainMods.RemoveAt(i);
					break;
				}

				if (check.HasElapsed())
				{
					SendSkillGainNormalMessage(check.Skill);
					SkillGainMods.RemoveAt(i--);
				}
			}

			SendSkillGainAcceleratedMessage(skill, duration);
			SkillGainMods.Add(new SkillGainMod(this, name, skill, bonus, duration));
		}

		public void ValidateSkillGainMods()
		{
			for (int i = 0; i < SkillGainMods.Count; ++i)
			{
				SkillGainMod check = SkillGainMods[i];

				if (!check.HasElapsed())
				{
					continue;
				}

				SendSkillGainNormalMessage(check.Skill);
				SkillGainMods.RemoveAt(i--);
			}
		}

		private void SendSkillGainAcceleratedMessage(SkillName skill, TimeSpan duration)
		{
			SendMessage(
				"A magical force has accelerated your skill gain{0}{1}.",
				skill == (SkillName)(-1) ? String.Empty : " for " + Skills[skill].Name.ToLower(),
				duration > TimeSpan.Zero ? " temporarily" : String.Empty);
		}

		private void SendSkillGainNormalMessage(SkillName skill)
		{
			SendMessage(
				"Your skill gain rate{0} has return to normal.",
				skill == (SkillName)(-1) ? String.Empty : " for " + Skills[skill].Name.ToLower());
		}

		//This is called so often, and is linked to using skill gain that we can use this to defrag our list
		public override void ValidateSkillMods()
		{
			base.ValidateSkillMods();

			ValidateSkillGainMods();
		}
		#endregion

		public void AutoStablePets()
		{
			if ( /*!EraSE || */AllFollowers.Count <= 0)
			{
				return;
			}

			for (int i = AllFollowers.Count - 1; i >= 0; --i)
			{
				var pet = AllFollowers[i] as BaseCreature;

				if (pet == null || pet.ControlMaster == null)
				{
					continue;
				}

				if (pet.Summoned)
				{
					if (pet.Map != Map)
					{
						pet.PlaySound(pet.GetAngerSound());
						Timer.DelayCall(TimeSpan.Zero, pet.Delete);
					}

					continue;
				}

				if (pet is IMount && ((IMount)pet).Rider != null)
				{
					continue;
				}

				if ((pet is PackLlama || pet is PackHorse || pet is Beetle || pet is HordeMinionFamiliar) &&
					(pet.Backpack != null && pet.Backpack.Items.Count > 0))
				{
					continue;
				}

				if (pet is BaseEscortable)
				{
					continue;
				}

				if (!pet.Summoned && !pet.IsBonded)
				{
					continue;
				}

				StablePet(pet, true, true);
			}
		}

		public void StablePet(BaseCreature pet)
		{
			StablePet(pet, false);
		}

		public void StablePet(BaseCreature pet, bool maxloyal)
		{
			StablePet(pet, maxloyal, false);
		}

		public void StablePet(BaseCreature pet, bool maxloyal, bool autostable)
		{
			if (pet is IMount)
			{
				((IMount)pet).Rider = null;
			}

			pet.ControlTarget = null;
			pet.ControlOrder = OrderType.Stay;
			pet.Internalize();

			pet.SetControlMaster(null);
			pet.SummonMaster = null;

			pet.IsStabled = true;
			pet.StabledDate = DateTime.UtcNow;

			Stabled.Add(pet);

			if (maxloyal)
			{
				pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy
			}

			if (autostable)
			{
				AutoStabled.Add(pet);
			}
		}

		public void ClaimAutoStabledPets()
		{
			if ( /*!EraSE || */AutoStabled.Count <= 0)
			{
				return;
			}

			if (!Alive)
			{
				// Your pet was unable to join you while you are a ghost.  Please re-login once you have ressurected to claim your pets.
				SendLocalizedMessage(1076251);
				return;
			}

			for (int i = AutoStabled.Count - 1; i >= 0; --i)
			{
				var pet = AutoStabled[i] as BaseCreature;

				if (pet == null)
				{
					continue;
				}

				if (pet.Deleted)
				{
					pet.IsStabled = false;

					if (Stabled.Contains(pet))
					{
						Stabled.Remove(pet);
					}
				}
				else if ((Followers + pet.ControlSlots) <= FollowersMax)
				{
					pet.SetControlMaster(this);

					if (pet.Summoned)
					{
						pet.SummonMaster = this;
					}

					pet.ControlTarget = this;
					pet.ControlOrder = OrderType.Follow;

					pet.MoveToWorld(Location, Map);

					pet.IsStabled = false;

					pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy

					if (!Mounted && pet is IMount)
					{
						((IMount)pet).Rider = this; // I think this might fix the mount issue
					}

					if (Stabled.Contains(pet))
					{
						Stabled.Remove(pet);
					}
				}
				else
				{
					// ~1_NAME~ remained in the stables because you have too many followers.
					SendMessage(1049612, pet.Name);
				}
			}

			AutoStabled.Clear();
		}

		public void BoatDeedWarning()
		{
			if (!Deleted && NetState != null)
			{
				SendGump(
					new WarningGump(
						1060637,
						30720,
						"Though boat deeds and dry-docked boats are blessed, boats can be sunk with cannons! See the uoforum.com for complete boat combat details so you understand the risks, and be careful!",
						0xFFC000,
						320,
						240,
						null,
						null));
			}
		}

		public override bool CanUseStuckMenu()
		{
			if (!Alive && BaseBoat.FindBoatAt(Location, Map) != null)
			{
				return true; // you can always use help stuck as a ghost on boat
			}

			return base.CanUseStuckMenu();
		}

		public override void OnHeal(ref int amount, Mobile from)
		{
			base.OnHeal(ref amount, from);

			PvPBattle battle = AutoPvP.FindBattle(this);

			if (battle != null)
			{
				amount = Math.Min(HitsMax - Hits, amount);

				battle.OnHeal(from, this, ref amount);
			}
		}

		public override void OnHitsChange(int oldValue)
		{
			base.OnHitsChange(oldValue);

			ActionCams.OnPlayerHitsChange(this, oldValue);
		}

		public override void Serialize(GenericWriter writer)
		{
			//cleanup our anti-macro table
			foreach (Hashtable t in m_AntiMacroTable.Values)
			{
				t.Values.OfType<CountAndTimeStamp>()
				 .Where(time => time.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.UtcNow)
				 .ForEach(t.Remove);
			}

			CheckKillDecay();
			CheckAtrophies(this);

			base.Serialize(writer);

			writer.Write(51); // version
			
			//46 (This should ALWAYS come just after the version!)
			SerializeSnapshot(writer);

            //51
            writer.Write(HalloweenPaintExpirationOrange);
            writer.Write(HalloweenPaintExpirationPurple);

            //50
            writer.Write(ZombiePaintExperationBooger);
            writer.Write(ZombiePaintExperationVesper);

            //49
            writer.Write(BattlePaintExpirationShadow);
            writer.Write(BattlePaintExpirationRed);
            //48
            writer.Write(NewbieQuestCompleted);

			//47
			writer.Write(ActionCams.IsCamera(this));

			//46 - Snapshots

			//45
			writer.Write(EventMsgFlag);
			writer.Write(EventPoints);

			//44
			if (_ValorTitles == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(_ValorTitles.Count);

				foreach (string t in _ValorTitles)
				{
					writer.Write(t);
				}
			}

			//version 43
			writer.Write(Lastkilled);

			//version 42
			writer.Write(_ValorRating);

			//version 41
			writer.Write(ValorQuests);

			//version 40
			writer.Write(ValorTitle);

			//version 39
			writer.Write(ValorRank);

			//version 38
			writer.Write(MurderersKilled);

			//version 37
			writer.Write(ValorPoints);

			// version 36
			writer.Write(LongTermLastDecayGameTime);

			// version 35
			writer.Write(m_Companion);

			// version 34
			writer.Write(GameParticipant);
			writer.Write(StatEnd);
			writer.Write(0); // TODO: Remove
			writer.Write(KMUsed);
			writer.Write(MurderBounty);

			int skillgaincount = SkillGainMods.Count(t => t.Duration > TimeSpan.Zero && !t.HasElapsed());

			writer.WriteEncodedInt(skillgaincount);

			foreach (SkillGainMod sm in SkillGainMods.Where(sm => sm.Duration > TimeSpan.Zero && !sm.HasElapsed()))
			{
				sm.Serialize(writer);
			}

			writer.Write(PeacedUntil);
			writer.Write(AnkhNextUse);
			writer.Write(AutoStabled, true);

			SetCustomFlag(CustomPlayerFlag.VisibilityList, VisibilityList.Count > 0);

			writer.WriteEncodedInt((int)CustomFlags);

			if (GetCustomFlag(CustomPlayerFlag.VisibilityList))
			{
				writer.Write(VisibilityList.Count);

				foreach (Mobile m in VisibilityList)
				{
					writer.Write(m);
				}
			}

			if (GetCustomFlag(CustomPlayerFlag.StaffLevel))
			{
				writer.Write((int)(AccessLevelToggler.m_Mobiles[this].Level));
			}

			if (GetCustomFlag(CustomPlayerFlag.StaffRank))
			{
				writer.Write((int)m_StaffRank);
			}

			if (GetCustomFlag(CustomPlayerFlag.DisplayStaffRank))
			{
				writer.Write(m_StaffTitle);
			}

			if (m_AcquiredRecipes == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(m_AcquiredRecipes.Count);

				foreach (KeyValuePair<int, bool> kvp in m_AcquiredRecipes)
				{
					writer.Write(kvp.Key);
					writer.Write(kvp.Value);
				}
			}

			writer.WriteDeltaTime(DateTime.UtcNow); // TODO: Remove

			ChampionTitleInfo.Serialize(writer, ChampionTitles);

			writer.Write(DateTime.UtcNow); // TODO: Remove

			writer.WriteEncodedInt(ToTItemsTurnedIn);
			writer.Write(ToTTotalMonsterFame); //This ain't going to be a small #.

			writer.WriteEncodedInt(AllianceMessageHue);
			writer.WriteEncodedInt(GuildMessageHue);

			writer.WriteEncodedInt(m_GuildRank.Rank);
			writer.Write(LastOnline);

			writer.WriteEncodedInt((int)SolenFriendship);

			QuestSerializer.Serialize(Quest, writer);

			if (DoneQuests == null)
			{
				writer.WriteEncodedInt(0);
			}
			else
			{
				for (int i = DoneQuests.Count - 1; i >= 0; i--)
				{
					if (QuestSystem.QuestTypesPendingRemoval.Any(t => DoneQuests[i].QuestType == t))
					{
						DoneQuests.RemoveAt(i);
					}
				}

				writer.WriteEncodedInt(DoneQuests.Count);

				foreach (QuestRestartInfo restartInfo in DoneQuests)
				{
					QuestSerializer.Write(restartInfo.QuestType, QuestSystem.QuestTypes, writer);

					writer.Write(restartInfo.RestartTime);
				}
			}

			writer.WriteEncodedInt(Profession);

			writer.WriteDeltaTime(DateTime.UtcNow); // TODO: Remove
			writer.WriteEncodedInt(0); // TODO: Remove

			BOBFilter.Serialize(writer);

			bool useMods = (m_HairModID != -1 || m_BeardModID != -1);

			writer.Write(useMods);

			if (useMods)
			{
				writer.Write(m_HairModID);
				writer.Write(m_HairModHue);
				writer.Write(m_BeardModID);
				writer.Write(m_BeardModHue);
			}

			writer.Write(SavagePaintExpiration);

			writer.Write((int)NpcGuild);
			writer.Write(NpcGuildJoinTime);
			writer.Write(NpcGuildGameTime);

			writer.Write(m_PermaFlags, true);

			writer.Write(NextTailorBulkOrder);
			writer.Write(NextSmithBulkOrder);

			writer.Write((int)Flags);

			writer.Write(m_ShortTermElapse);
			writer.Write(GameTime);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			//46+ (This should ALWAYS come just after the version!)
			if (version >= 46)
			{
				DeserializeSnapshot(reader);
			}

			if (LostStabledPetRecorder.Enabled)
			{
				LostStabledPetRecorder.PlayerMobiles.Add(this);
			}

			switch (version)
			{
                case 51:
                    {
                        HalloweenPaintExpirationOrange = reader.ReadTimeSpan();

                        if (HalloweenPaintExpirationOrange > TimeSpan.Zero)
                        {
                            BodyMod = (Female ? 184 : 183);
                            HueMod = 1358;
                        }

                        HalloweenPaintExpirationPurple = reader.ReadTimeSpan();

                        if (HalloweenPaintExpirationPurple > TimeSpan.Zero)
                        {
                            BodyMod = (Female ? 184 : 183);
                            HueMod = 1378;
                        }
                    }
                    goto case 50;
                case 50:
                    {
                        ZombiePaintExperationBooger = reader.ReadTimeSpan();

                        if (ZombiePaintExperationBooger > TimeSpan.Zero)
                        {
                            BodyMod = (Female ? 184 : 183);
                            HueMod = 61;
                        }

                        ZombiePaintExperationVesper = reader.ReadTimeSpan();

                        if (ZombiePaintExperationVesper > TimeSpan.Zero)
                        {
                            BodyMod = (Female ? 184 : 183);
                            HueMod = 1159;
                        }
                    }
                    goto case 49;
                case 49:
                    {
                        BattlePaintExpirationShadow = reader.ReadTimeSpan();

                        if (BattlePaintExpirationShadow > TimeSpan.Zero)
                        {
                            BodyMod = (Female ? 184 : 183);
                            HueMod = 1175;
                        }

                        BattlePaintExpirationRed = reader.ReadTimeSpan();

                        if (BattlePaintExpirationRed > TimeSpan.Zero)
                        {
                            BodyMod = (Female ? 184 : 183);
                            HueMod = 1157;
                        }
                    }
                    goto case 48;
                case 48:
			    {
			        NewbieQuestCompleted = reader.ReadBool();
			    }
                    goto case 47;
				case 47:
					{
						if (reader.ReadBool())
						{
							BodyValue = Race.Body(this);
						}
					}
					goto case 46;
				case 46:
				case 45:
					{
						EventMsgFlag = reader.ReadBool();
						EventPoints = reader.ReadInt();
					}
					goto case 44;
				case 44:
					{
						int titleCount = reader.ReadInt();

						if (titleCount > 0)
						{
							_ValorTitles = new List<string>();

							for (int i = 0; i < titleCount; i++)
							{
								string title = reader.ReadString();
								_ValorTitles.Add(title);
							}
						}
					}
					goto case 43;
				case 43:
					{
						Lastkilled = reader.ReadDateTime();
						goto case 42;
					}
				case 42:
					_ValorRating = reader.ReadInt();
					goto case 41;
				case 41:
					ValorQuests = reader.ReadInt();
					goto case 40;
				case 40:
					ValorTitle = reader.ReadString();
					goto case 39;
				case 39:
					ValorRank = reader.ReadInt();
					goto case 38;
				case 38:
					MurderersKilled = reader.ReadInt();
					goto case 37;
				case 37:
					ValorPoints = reader.ReadInt();
					goto case 36;
				case 36:
					LongTermLastDecayGameTime = reader.ReadTimeSpan();
					goto case 35;
				case 35:
					Companion = reader.ReadBool();
					goto case 34;
				case 34:
					{
						// don't check stat loss decay here, just on login
						GameParticipant = reader.ReadBool();
					}
					goto case 33;
				case 33:
					{
						// don't check stat loss decay here, just on login
						StatEnd = reader.ReadDateTime();
					}
					goto case 32;
				case 32:
					reader.ReadInt(); //m_InStat = reader.ReadInt(); 
					// TODO: Remove
					goto case 31;
				case 31:
					KMUsed = reader.ReadInt();
					goto case 30;
				case 30:
					MurderBounty = reader.ReadInt();
					goto case 29;
				case 29:
					{
						int skillgaincount = reader.ReadEncodedInt();

						for (int i = 0; i < skillgaincount; i++)
						{
							SkillGainMods.Add(new SkillGainMod(this, reader));
						}

						goto case 28;
					}
				case 28:
					PeacedUntil = reader.ReadDateTime();
					goto case 27;
				case 27:
					AnkhNextUse = reader.ReadDateTime();
					goto case 26;
				case 26:
					{
						AutoStabled = reader.ReadStrongMobileList();

						CustomFlags = (CustomPlayerFlag)reader.ReadEncodedInt();

						if (GetCustomFlag(CustomPlayerFlag.VisibilityList))
						{
							int length = reader.ReadInt();

							for (int i = 0; i < length; i++)
							{
								VisibilityList.Add(reader.ReadMobile());
							}
						}

						if (GetCustomFlag(CustomPlayerFlag.StaffLevel))
						{
							AccessLevelToggler.m_Mobiles.Add(this, new AccessLevelMod((AccessLevel)reader.ReadInt()));
						}

						if (GetCustomFlag(CustomPlayerFlag.StaffRank))
						{
							m_StaffRank = (StaffRank)reader.ReadInt();
						}

						if (GetCustomFlag(CustomPlayerFlag.DisplayStaffRank))
						{
							m_StaffTitle = reader.ReadString();
						}
					}
					goto case 25;
				case 25:
					{
						int recipeCount = reader.ReadInt();

						if (recipeCount > 0)
						{
							m_AcquiredRecipes = new Dictionary<int, bool>();

							for (int i = 0; i < recipeCount; i++)
							{
								int r = reader.ReadInt();

								//Don't add in recipies which we haven't gotten or have been removed
								if (reader.ReadBool())
								{
									m_AcquiredRecipes.Add(r, true);
								}
							}
						}
					}
					goto case 24;
				case 24:
					reader.ReadDeltaTime(); //m_LastHonorLoss = reader.ReadDeltaTime();
					// TODO: Remove
					goto case 23;
				case 23:
					ChampionTitles = new ChampionTitleInfo(reader);
					goto case 22;
				case 22:
					reader.ReadDateTime(); //m_LastValorLoss = reader.ReadDateTime();
					// TODO: Remove
					goto case 21;
				case 21:
					{
						ToTItemsTurnedIn = reader.ReadEncodedInt();
						ToTTotalMonsterFame = reader.ReadInt();
					}
					goto case 20;
				case 20:
					{
						AllianceMessageHue = reader.ReadEncodedInt();
						GuildMessageHue = reader.ReadEncodedInt();
					}
					goto case 19;
				case 19:
					{
						int rank = reader.ReadEncodedInt();
						int maxRank = RankDefinition.Ranks.Length - 1;

						if (rank > maxRank)
						{
							rank = maxRank;
						}

						m_GuildRank = RankDefinition.Ranks[rank];
						LastOnline = reader.ReadDateTime();
					}
					goto case 18;
				case 18:
					SolenFriendship = (SolenFriendship)reader.ReadEncodedInt();
					goto case 17;
				case 17: // changed how DoneQuests is serialized
				case 16:
					{
						Quest = QuestSerializer.DeserializeQuest(reader);

						if (Quest != null)
						{
							Quest.From = this;
						}

						int count = reader.ReadEncodedInt();

						DoneQuests = new List<QuestRestartInfo>(count);

						if (count > 0)
						{
							for (int i = 0; i < count; ++i)
							{
								Type questType = QuestSerializer.ReadType(QuestSystem.QuestTypes, reader);
								DateTime restartTime = version < 17 ? DateTime.MaxValue : reader.ReadDateTime();

								DoneQuests.Add(new QuestRestartInfo(questType, restartTime));
							}
						}

						Profession = reader.ReadEncodedInt();
					}
					goto case 15;
				case 15:
					reader.ReadDeltaTime(); //m_LastCompassionLoss = reader.ReadDeltaTime();
					// TODO: Remove
					goto case 14;
				case 14:
					{
						// TODO: Remove
						if (reader.ReadEncodedInt() > 0)
						{
							reader.ReadDeltaTime(); //m_NextCompassionDay = reader.ReadDeltaTime();
						}
					}
					goto case 13;
				case 13: // removed PaidInsurance list
				case 12:
					BOBFilter = new BOBFilter(reader);
					goto case 11;
				case 11:
					{
						if (version < 13)
						{
							List<Item> paid = reader.ReadStrongItemList();

							foreach (Item i in paid)
							{
								i.PaidInsurance = true;
							}
						}
					}
					goto case 10;
				case 10:
					{
						if (reader.ReadBool())
						{
							m_HairModID = reader.ReadInt();
							m_HairModHue = reader.ReadInt();
							m_BeardModID = reader.ReadInt();
							m_BeardModHue = reader.ReadInt();
						}
					}
					goto case 9;
				case 9:
					{
						SavagePaintExpiration = reader.ReadTimeSpan();

						if (SavagePaintExpiration > TimeSpan.Zero)
						{
							BodyMod = (Female ? 184 : 183);
							HueMod = 0;
						}
					}
					goto case 8;
				case 8:
					{
						NpcGuild = (NpcGuild)reader.ReadInt();
						NpcGuildJoinTime = reader.ReadDateTime();
						NpcGuildGameTime = reader.ReadTimeSpan();
					}
					goto case 7;
				case 7:
					m_PermaFlags = reader.ReadStrongMobileList();
					goto case 6;
				case 6:
					NextTailorBulkOrder = reader.ReadTimeSpan();
					goto case 5;
				case 5:
					NextSmithBulkOrder = reader.ReadTimeSpan();
					goto case 4;
				case 4:
				case 3:
				case 2:
					Flags = (PlayerFlag)reader.ReadInt();
					goto case 1;
				case 1:
					{
						m_ShortTermElapse = reader.ReadDateTime();
						m_GameTime = reader.ReadTimeSpan();
					}
					goto case 0;
				case 0:
					break;
			}

			if (RecentlyReported == null)
			{
				RecentlyReported = new List<Mobile>();
			}

			// Professions weren't verified on 1.0 RC0
			if (!CharacterCreation.VerifyProfession(Profession, Expansion))
			{
				Profession = 0;
			}

			if (m_PermaFlags == null)
			{
				m_PermaFlags = new List<Mobile>();
			}

			if (BOBFilter == null)
			{
				BOBFilter = new BOBFilter();
			}

			if (m_GuildRank == null)
			{
				//Default to member if going from older version to new version (only time it should be null)
				m_GuildRank = RankDefinition.Member;
			}

			if (LastOnline == DateTime.MinValue && Account != null)
			{
				LastOnline = ((Account)Account).LastLogin;
			}

			if (ChampionTitles == null)
			{
				ChampionTitles = new ChampionTitleInfo();
			}

			if (AccessLevel > AccessLevel.Player)
			{
				IgnoreMobiles = true;
			}

			List<Mobile> list = Stabled;

			foreach (BaseCreature bc in list.OfType<BaseCreature>())
			{
				bc.IsStabled = true; //Charge date set in BaseCreature
			}

			CheckAtrophies(this);

			if (Hidden) //Hiding is the only buff where it has an effect that's serialized.
			{
				AddBuff(new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));
			}
		}
	}
}