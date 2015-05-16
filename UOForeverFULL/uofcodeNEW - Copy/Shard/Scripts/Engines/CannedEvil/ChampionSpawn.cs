#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Commands;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Targeting;

using VitaNex;
using VitaNex.Targets;
#endregion

namespace Server.Engines.CannedEvil
{
	public class ChampionCommands
	{
		public static void Initialize()
		{
			CommandSystem.Register("ClearChampByTarget", AccessLevel.EventMaster, KillByTarget_OnCommand);
			CommandSystem.Register("ClearChampByRegion", AccessLevel.EventMaster, KillByRegion_OnCommand);
		}

		[Usage("ClearChampByTarget")]
		[Description("Kills all minions of a champion spawn.")]
		private static void KillByTarget_OnCommand(CommandEventArgs e)
		{
			if (e == null || e.Mobile == null)
			{
				return;
			}

			e.Mobile.Target = new KillTarget();
			e.Mobile.SendMessage("Which champion spawn would you like to clear?");
		}

		[Usage("ClearChampByRegion")]
		[Description("Kills all minions of a champion spawn.")]
		private static void KillByRegion_OnCommand(CommandEventArgs e)
		{
			if (e == null || e.Mobile == null)
			{
				return;
			}

			var region = e.Mobile.GetRegion<ChampionSpawnRegion>();

			if (region != null && region.Spawn != null)
			{
				region.Spawn.DeleteCreatures();

				if (region.Spawn.Champion != null)
				{
					region.Spawn.Champion.Delete();
				}
			}
			else
			{
				e.Mobile.SendMessage("You are not in a champion spawn region.");
			}
		}

		private sealed class KillTarget : ItemSelectTarget<Item>
		{
			public KillTarget()
				: base(null, null, -1, false, TargetFlags.None)
			{ }

			protected override void OnTarget(Mobile m, Item targ)
			{
				if (m == null || targ == null || targ.Deleted || m.AccessLevel < AccessLevel.EventMaster)
				{
					return;
				}

				ChampionSpawn spawn = null;

				if (targ is ChampionSpawn)
				{
					spawn = (ChampionSpawn)targ;
				}
				else if (targ is IdolOfTheChampion)
				{
					spawn = ((IdolOfTheChampion)targ).Spawn;
				}
				else if (targ is ChampionAltar)
				{
					spawn = ((ChampionAltar)targ).Spawn;
				}
				else if (targ is ChampionPlatform)
				{
					spawn = ((ChampionPlatform)targ).Spawn;
				}

				if (spawn == null)
				{
					return;
				}

				spawn.DeleteCreatures();

				if (spawn.Champion != null)
				{
					spawn.Champion.Delete();
				}
			}
		}
	}

	public class ChampionSpawn : Item
	{
		public const int Level1 = 4;
		public const int Level2 = 8;
		public const int Level3 = 12;

		#region Scrolls
		private static void GiveScrollTo(Mobile killer, Item scroll)
		{
			if (killer == null || scroll == null)
			{
				return;
			}

			if (!killer.Alive && killer.Corpse != null && !killer.Corpse.Deleted)
			{
				killer.Corpse.DropItem(scroll);
				return;
			}

			killer.AddToBackpack(scroll);
		}

		public static void GiveScrollOfTranscendenceFelTo(Mobile killer, ScrollofTranscendence scroll)
		{
			if (killer == null || scroll == null)
			{
				return;
			}

			// You have received a Scroll of Transcendence!
			killer.SendLocalizedMessage(1094936);

			GiveScrollTo(killer, scroll);
		}

		public static void GivePowerScrollFelTo(Mobile killer, PowerScroll scroll)
		{
			if (killer == null || scroll == null)
			{
				return;
			}

			// You have received a scroll of power!
			killer.SendLocalizedMessage(1049524);

			GiveScrollTo(killer, scroll);
		}
		#endregion

		private bool _Active;

		private int _Kills;
		private int _MaxLevel;
		private int _Level;

		private List<Mobile> _Creatures;
		private List<Item> _RedSkulls;
		private List<Item> _WhiteSkulls;

		private Timer _Timer;
		private IdolOfTheChampion _Idol;
		private Rectangle2D _SpawnArea;
		private ChampionSpawnRegion _Region;
		private ChampionSpawnType _SpawnType;
		private ChampionPlatform _Platform;
		private ChampionAltar _Altar;

		public override int LabelNumber { get { return 1041030; } } // Evil in a Can:  Don't delete me!

		public override TimeSpan DecayTime { get { return TimeSpan.FromSeconds(180.0); } }

		public virtual bool ProximitySpawn { get { return true; } }
		public virtual bool CanAdvanceByValor { get { return false; } }
		public virtual bool CanActivateByValor { get { return false; } }
		public virtual bool AlwaysActive { get { return false; } }
		public virtual bool HasStarRoomGate { get { return true; } }

		public virtual int MaxSpawn { get { return 250 - (GetSubLevel() * 40); } }

		public virtual string BroadcastMessage
		{
			get
			{
				return "The Champion has sensed your presence!  Beware its wrath!  " +
					   "You figure you have about half a minute to prepare or to flee!";
			}
		}

		#region PlayerScores
		public virtual Dictionary<Mobile, double> Scores { get { return PlayerScores.GetPlayerScores(this, !Deleted); } }

		[CommandProperty(AccessLevel.Counselor)]
		public virtual double ScoresMin
		{
			get
			{
				var scores = Scores;

				return scores != null && scores.Count > 0 ? scores.Values.Min() : 0.0;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public virtual double ScoresMax
		{
			get
			{
				var scores = Scores;

				return scores != null && scores.Count > 0 ? scores.Values.Max() : 0.0;
			}
		}
		#endregion

		public Dictionary<Mobile, int> DamageEntries { get; private set; }

		public Timer RestartTimer { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public string BossScript { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public string MobScript { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public bool ConfinedRoaming { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public bool HasBeenAdvanced { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public Point3D EjectLocation { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public Map EjectMap { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public bool RandomizeType { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public int Kills
		{
			get { return _Kills; }
			set
			{
				_Kills = value;

				double n = _Kills / (double)MaxKills;
				var p = (int)(n * 100);

				if (p < 90)
				{
					SetWhiteSkullCount(p / 20);
				}

				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.EventMaster)]
		public Rectangle2D SpawnArea
		{
			get { return _SpawnArea; }
			set
			{
				_SpawnArea = value;
				InvalidateProperties();
				UpdateRegion();
			}
		}

		[CommandProperty(AccessLevel.EventMaster)]
		public TimeSpan RestartDelay { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public DateTime RestartTime { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public TimeSpan ExpireDelay { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public DateTime ExpireTime { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public ChampionSpawnType SpawnType
		{
			get { return _SpawnType; }
			set
			{
				_SpawnType = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.EventMaster)]
		public bool Active
		{
			get { return _Active; }
			set
			{
				if (value)
				{
					Start();
				}
				else
				{
					Stop();
				}

				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.EventMaster)]
		public bool ReadyToActivate { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public bool ActivatedByValor { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public bool ActivatedByProximity { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public DateTime NextProximityTime { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public Mobile Champion { get; set; }

		[CommandProperty(AccessLevel.EventMaster)]
		public int Level
		{
			get { return _Level; }
			set
			{
				for (int i = _RedSkulls.Count - 1; i >= value; --i)
				{
					_RedSkulls[i].Delete();
					_RedSkulls.RemoveAt(i);
				}

				for (int i = _RedSkulls.Count; i < Math.Min(value, 16); ++i)
				{
					var skull = new Item(0x1854)
					{
						Hue = 0x26,
						Movable = false,
						Light = LightType.Circle150
					};

					skull.MoveToWorld(GetRedSkullLocation(i), Map);

					_RedSkulls.Add(skull);
				}

				_Level = value;

				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.EventMaster, AccessLevel.EventMaster)]
		public int MaxLevel { get { return _MaxLevel; } set { _MaxLevel = Math.Max(Math.Min(value, 18), 0); } }

		[CommandProperty(AccessLevel.EventMaster)]
		public virtual int MaxKills
		{
			get
			{
				if (Level >= 16)
				{
					return 16;
				}

				if (Level >= 12)
				{
					return 32;
				}

				if (Level >= 8)
				{
					return 64;
				}

				if (Level >= 4)
				{
					return 128;
				}

				return 256;
			}
		}

		[Constructable]
		public ChampionSpawn()
			: base(0xBD2)
		{
			Movable = false;
			Visible = false;

			_Creatures = new List<Mobile>();
			_RedSkulls = new List<Item>();
			_WhiteSkulls = new List<Item>();

			_Platform = new ChampionPlatform(this);
			_Altar = new ChampionAltar(this);
			_Idol = new IdolOfTheChampion(this);

			ExpireDelay = TimeSpan.FromMinutes(30.0);
			RestartDelay = TimeSpan.FromMinutes(30.0);
			DamageEntries = new Dictionary<Mobile, int>();

			Timer.DelayCall(TimeSpan.Zero, SetInitialSpawnArea);
		}

		public ChampionSpawn(Serial serial)
			: base(serial)
		{ }

		public bool IsEligible(Mobile mob)
		{
			return Champion is BaseChampion && ((BaseChampion)Champion).IsEligible(mob);
		}

		public bool IsChampionSpawn(Mobile m)
		{
			return _Creatures.Contains(m);
		}

		public void SetInitialSpawnArea()
		{
			SpawnArea = new Rectangle2D(new Point2D(X - 24, Y - 24), new Point2D(X + 24, Y + 24));
		}

		public virtual ChampionSpawnRegion GetRegion()
		{
			return new ChampionSpawnRegion(this);
		}

		public void UpdateRegion()
		{
			if (_Region != null)
			{
				_Region.Unregister();
			}

			if (Deleted || Map == Map.Internal)
			{
				return;
			}

			_Region = GetRegion();
			_Region.Register();
		}

		public void SetWhiteSkullCount(int val)
		{
			for (int i = _WhiteSkulls.Count - 1; i >= val; --i)
			{
				_WhiteSkulls[i].Delete();
				_WhiteSkulls.RemoveAt(i);
			}

			for (int i = _WhiteSkulls.Count; i < val; ++i)
			{
				var skull = new Item(0x1854)
				{
					Movable = false,
					Light = LightType.Circle150
				};

				skull.MoveToWorld(GetWhiteSkullLocation(i), Map);

				_WhiteSkulls.Add(skull);

				Effects.PlaySound(skull.Location, skull.Map, 0x29);
				Effects.SendLocationEffect(new Point3D(skull.X + 1, skull.Y + 1, skull.Z), skull.Map, 0x3728, 10);
			}
		}

		public void Start()
		{
			PlayerScores.ClearPlayerScores(this);

			if (_Active || Deleted)
			{
				return;
			}

			if (RandomizeType)
			{
				switch (Utility.Random(5))
				{
					case 0:
						SpawnType = ChampionSpawnType.VerminHorde;
						break;
					case 1:
						SpawnType = ChampionSpawnType.UnholyTerror;
						break;
					case 2:
						SpawnType = ChampionSpawnType.ColdBlood;
						break;
					case 3:
						SpawnType = ChampionSpawnType.Abyss;
						break;
					case 4:
						SpawnType = ChampionSpawnType.Arachnid;
						break;
				}
			}

			_Active = true;
			ReadyToActivate = false;
			HasBeenAdvanced = false;
			_MaxLevel = 16 + Utility.Random(3);

			if (_Timer != null)
			{
				_Timer.Stop();
			}

			_Timer = new SliceTimer(this);
			_Timer.Start();

			if (RestartTimer != null)
			{
				RestartTimer.Stop();
			}

			RestartTimer = null;

			if (_Altar != null)
			{
				_Altar.Hue = Champion != null ? 0x26 : 0;
			}

			if (_Platform != null)
			{
				_Platform.Hue = 0x452;
			}

			ExpireTime = DateTime.UtcNow + ExpireDelay;
		}

		public void Stop()
		{
			PlayerScores.ClearPlayerScores(this);

			if (!_Active || Deleted)
			{
				return;
			}

			_Active = false;
			ActivatedByValor = false;
			HasBeenAdvanced = false;
			_MaxLevel = 0;

			if (_Timer != null)
			{
				_Timer.Stop();
			}

			_Timer = null;

			if (RestartTimer != null)
			{
				RestartTimer.Stop();
			}

			RestartTimer = null;

			if (_Altar != null)
			{
				_Altar.Hue = 0;
			}

			if (_Platform != null)
			{
				_Platform.Hue = 0x497;
			}

			if (AlwaysActive)
			{
				BeginRestart(RestartDelay);
			}
			else if (ActivatedByProximity)
			{
				ActivatedByProximity = false;
				NextProximityTime = DateTime.UtcNow + TimeSpan.FromHours(4.0);
			}

			Timer.DelayCall(TimeSpan.FromMinutes(10.0), ExpireCreatures);
		}

		public void BeginRestart(TimeSpan ts)
		{
			if (RestartTimer != null)
			{
				RestartTimer.Stop();
			}

			RestartTime = DateTime.UtcNow + ts;

			RestartTimer = new RestartTimer(this, ts);
			RestartTimer.Start();
		}

		public void EndRestart()
		{
			HasBeenAdvanced = false;
			ReadyToActivate = true;
			Start();
		}

		public void OnSlice()
		{
			if (!_Active)
			{
				PlayerScores.ClearPlayerScores(this);
				return;
			}

			if (Champion != null)
			{
				if (!Champion.Deleted)
				{
					return;
				}

				RegisterDamageTo(Champion);

				DamageEntries.Clear();

				if (_Platform != null)
				{
					_Platform.Hue = 0x497;
				}

				if (_Altar != null)
				{
					_Altar.Hue = 0;

					if (HasStarRoomGate && (!EraML || Map == Map.Felucca))
					{
						new StarRoomGate(true).MoveToWorld(_Altar.Location, _Altar.Map);

						Effects.PlaySound(_Altar.Location, _Altar.Map, 0x20E);
					}
				}

				Champion = null;
				Stop();
			}
			else
			{
				int kills = _Kills;

				for (int i = 0; i < _Creatures.Count; ++i)
				{
					Mobile m = _Creatures[i];

					if (!m.Deleted)
					{
						continue;
					}

					if (m.Corpse != null && !m.Corpse.Deleted)
					{
						((Corpse)m.Corpse).BeginDecay(TimeSpan.FromMinutes(1));
					}

					_Creatures.RemoveAt(i);
					--i;
					++_Kills;

					Mobile killer = m.FindMostRecentDamager(false);

					RegisterDamageTo(m);

					if (killer is BaseCreature)
					{
						killer = ((BaseCreature)killer).GetMaster();
					}

					if (!(killer is PlayerMobile))
					{
						continue;
					}

					int mobSubLevel = GetSubLevelFor(m) + 1;

					if (mobSubLevel < 0)
					{
						continue;
					}

					PlayerMobile.ChampionTitleInfo info = ((PlayerMobile)killer).ChampionTitles;

					if (info != null)
					{
						info.Award(_SpawnType, mobSubLevel);
					}
				}

				// Only really needed once.
				if (_Kills > kills)
				{
					InvalidateProperties();
				}

				double n = _Kills / (double)MaxKills;
				var p = (int)(n * 100);

				if (p >= 99)
				{
					AdvanceLevel();
				}
				else if (p > 0)
				{
					SetWhiteSkullCount(p / 20);
				}

				if (DateTime.UtcNow >= ExpireTime)
				{
					Expire();
				}

				Respawn();
			}
		}

		public void AdvanceLevel()
		{
			ExpireTime = DateTime.UtcNow + ExpireDelay;

			if (Level < _MaxLevel)
			{
				_Kills = 0;
				++Level;

				InvalidateProperties();
				SetWhiteSkullCount(0);

				if (_Altar == null)
				{
					return;
				}

				Effects.PlaySound(_Altar.Location, _Altar.Map, 0x29);
				Effects.SendLocationEffect(new Point3D(_Altar.X + 1, _Altar.Y + 1, _Altar.Z), _Altar.Map, 0x3728, 10);
			}
			else
			{
				SpawnChampion();
			}
		}

		public void SpawnChampion()
		{
			if (_Altar != null)
			{
				_Altar.Hue = 0x26;
			}

			if (_Platform != null)
			{
				_Platform.Hue = 0x452;
			}

			_Kills = 0;
			Level = 0;

			InvalidateProperties();
			SetWhiteSkullCount(0);

			Champion = VitaNexCore.TryCatchGet(
				() => ChampionSpawnInfo.GetInfo(_SpawnType).Champion.CreateInstance<Mobile>(),
				x =>
				{
					Console.WriteLine("Exception creating champion '{0}'", _SpawnType);
					x.ToConsole();
				});

			if (Champion == null)
			{
				return;
			}

			if (ChampionGlobals.CSOptions.InformChampSpawnRegionMobs)
			{
				foreach (Mobile mob in _Region.GetMobiles().Where(mob => mob is PlayerMobile && mob.Alive && !mob.Hidden))
				{
					mob.LocalOverheadMessage(MessageType.Regular, 38, false, "The champion has come!");
				}
			}

			Champion.MoveToWorld(new Point3D(X, Y, Z - 15), Map);

			if (!(Champion is BaseCreature))
			{
				throw new Exception("Champion Spawn is not inherited from BaseCreature");
			}

			var bc = (BaseCreature)Champion;

			bc.ChampSpawn = this;

			if (ConfinedRoaming)
			{
				bc.Home = Location;
				bc.HomeMap = Map;
				bc.RangeHome = Math.Min(_SpawnArea.Width / 2, _SpawnArea.Height / 2);
			}
			else
			{
				bc.Home = Champion.Location;
				bc.HomeMap = bc.Map;

				var xWall1 = new Point2D(_SpawnArea.X, bc.Y);
				var xWall2 = new Point2D(_SpawnArea.X + _SpawnArea.Width, bc.Y);
				var yWall1 = new Point2D(bc.X, _SpawnArea.Y);
				var yWall2 = new Point2D(bc.X, _SpawnArea.Y + _SpawnArea.Height);

				double minXDist = Math.Min(bc.GetDistanceToSqrt(xWall1), bc.GetDistanceToSqrt(xWall2));
				double minYDist = Math.Min(bc.GetDistanceToSqrt(yWall1), bc.GetDistanceToSqrt(yWall2));

				bc.RangeHome = (int)Math.Min(minXDist, minYDist);
			}

			if (!String.IsNullOrWhiteSpace(BossScript))
			{
				XmlAttach.AttachTo(
					bc,
					new XmlScript(BossScript)
					{
						Name = "champboss"
					});
			}
		}

		public void Respawn()
		{
			if (!_Active || Deleted || Champion != null)
			{
				return;
			}

			while (_Creatures.Count < MaxSpawn)
			{
				Mobile m = Spawn();

				if (m == null)
				{
					return;
				}

				Point3D loc = GetSpawnLocation();

				// Allow creatures to turn into Paragons at Ilshenar champions.
				m.OnBeforeSpawn(loc, Map);

				_Creatures.Add(m);

				m.MoveToWorld(loc, Map);

				if (!(m is BaseCreature))
				{
					continue;
				}

				var bc = (BaseCreature)m;

				bc.Tamable = false;
				bc.ChampSpawn = this;

				if (ConfinedRoaming)
				{
					bc.Home = Location;
					bc.HomeMap = Map;
					bc.RangeHome = Math.Min(_SpawnArea.Width / 2, _SpawnArea.Height / 2);
				}
				else
				{
					bc.Home = bc.Location;
					bc.HomeMap = bc.Map;

					var xWall1 = new Point2D(_SpawnArea.X, bc.Y);
					var xWall2 = new Point2D(_SpawnArea.X + _SpawnArea.Width, bc.Y);
					var yWall1 = new Point2D(bc.X, _SpawnArea.Y);
					var yWall2 = new Point2D(bc.X, _SpawnArea.Y + _SpawnArea.Height);

					double minXDist = Math.Min(bc.GetDistanceToSqrt(xWall1), bc.GetDistanceToSqrt(xWall2));
					double minYDist = Math.Min(bc.GetDistanceToSqrt(yWall1), bc.GetDistanceToSqrt(yWall2));

					bc.RangeHome = (int)Math.Min(minXDist, minYDist);
				}

				if (!String.IsNullOrWhiteSpace(MobScript))
				{
					XmlAttach.AttachTo(
						bc,
						new XmlScript(MobScript)
						{
							Name = "champmob"
						});
				}
			}
		}

		public Point3D GetSpawnLocation()
		{
			Map map = Map;

			if (map == null)
			{
				return Location;
			}

			// Try 20 times to find a spawnable location.
			for (int i = 0; i < 20; i++)
			{
				int x = Utility.Random(_SpawnArea.X, _SpawnArea.Width);
				int y = Utility.Random(_SpawnArea.Y, _SpawnArea.Height);

				int z = map.GetAverageZ(x, y);

				if (map.CanSpawnMobile(x, y, z))
				{
					return new Point3D(x, y, z);
				}
			}

			return Location;
		}

		public int GetSubLevel()
		{
			int level = Level;

			if (level <= Level1)
			{
				return 0;
			}

			if (level <= Level2)
			{
				return 1;
			}

			return level <= Level3 ? 2 : 3;
		}

		public int GetSubLevelFor(Mobile m)
		{
			ChampionSpawnInfo info = ChampionSpawnInfo.GetInfo(_SpawnType);

			if (info == null || info.SpawnTypes == null || info.SpawnTypes.Length == 0)
			{
				return -1;
			}

			Type t = m.GetType();

			for (int i = 0; i < info.SpawnTypes.Length; i++)
			{
				if (info.SpawnTypes[i].Contains(t))
				{
					return i;
				}
			}

			return -1;
		}

		public Mobile Spawn(params Type[] types)
		{
			if (types == null || types.Length == 0)
			{
				ChampionSpawnInfo info = ChampionSpawnInfo.GetInfo(_SpawnType);

				if (info == null || info.SpawnTypes == null)
				{
					return null;
				}

				int v = GetSubLevel();

				if (info.SpawnTypes.InBounds(v))
				{
					types = info.SpawnTypes[v];
				}
			}

			return types != null ? types.GetRandom().CreateInstanceSafe<Mobile>() : null;
		}

		public void Expire()
		{
			_Kills = 0;

			if (_WhiteSkulls.Count == 0)
			{
				// They didn't even get 20%, go back a level
				if (Level > 0)
				{
					--Level;
				}

				if (!AlwaysActive && Level == 0)
				{
					Stop();
				}

				InvalidateProperties();
			}
			else
			{
				SetWhiteSkullCount(0);
			}

			ExpireTime = DateTime.UtcNow + ExpireDelay;
		}

		public Point3D GetRedSkullLocation(int index)
		{
			int x, y;

			if (index < 5)
			{
				x = index - 2;
				y = -2;
			}
			else if (index < 9)
			{
				x = 2;
				y = index - 6;
			}
			else if (index < 13)
			{
				x = 10 - index;
				y = 2;
			}
			else
			{
				x = -2;
				y = 14 - index;
			}

			return new Point3D(X + x, Y + y, Z - 15);
		}

		public Point3D GetWhiteSkullLocation(int index)
		{
			int x, y;

			switch (index)
			{
				default:
					x = -1;
					y = -1;
					break;
				case 1:
					x = 1;
					y = -1;
					break;
				case 2:
					x = 1;
					y = 1;
					break;
				case 3:
					x = -1;
					y = 1;
					break;
			}

			return new Point3D(X + x, Y + y, Z - 15);
		}

		public override void AddNameProperty(ObjectPropertyList list)
		{
			list.Add("champion spawn");
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (_Active)
			{
				list.Add(1060742); // active
				list.Add(1060658, "Type\t{0}", _SpawnType); // ~1_val~: ~2_val~
				list.Add(1060659, "Level\t{0}", Level); // ~1_val~: ~2_val~
				list.Add(1060660, "Kills\t{0} of {1} ({2:F1}%)", _Kills, MaxKills, 100.0 * ((double)_Kills / MaxKills));
				// ~1_val~: ~2_val~
				//list.Add( 1060661, "Spawn Range\t{0}", m_SpawnRange ); // ~1_val~: ~2_val~
			}
			else
			{
				list.Add(1060743); // inactive
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			if (_Active)
			{
				LabelTo(from, "{0} (Active; Level: {1}; Kills: {2}/{3})", _SpawnType, Level, _Kills, MaxKills);
			}
			else
			{
				LabelTo(from, "{0} (Inactive)", _SpawnType);
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			from.SendGump(new PropertiesGump(from, this));
		}

		public override void OnLocationChange(Point3D oldLoc)
		{
			if (Deleted)
			{
				return;
			}

			if (_Platform != null)
			{
				_Platform.Location = new Point3D(X, Y, Z - 20);
			}

			if (_Altar != null)
			{
				_Altar.Location = new Point3D(X, Y, Z - 15);
			}

			if (_Idol != null)
			{
				_Idol.Location = new Point3D(X, Y, Z - 15);
			}

			if (_RedSkulls != null)
			{
				for (int i = 0; i < Math.Min(_RedSkulls.Count, 16); ++i)
				{
					_RedSkulls[i].Location = GetRedSkullLocation(i);
				}
			}

			if (_WhiteSkulls != null)
			{
				for (int i = 0; i < _WhiteSkulls.Count; ++i)
				{
					_WhiteSkulls[i].Location = GetWhiteSkullLocation(i);
				}
			}

			_SpawnArea.X += Location.X - oldLoc.X;
			_SpawnArea.Y += Location.Y - oldLoc.Y;

			UpdateRegion();
		}

		public override void OnMapChange()
		{
			if (Deleted)
			{
				return;
			}

			if (_Platform != null)
			{
				_Platform.Map = Map;
			}

			if (_Altar != null)
			{
				_Altar.Map = Map;
			}

			if (_Idol != null)
			{
				_Idol.Map = Map;
			}

			if (_RedSkulls != null)
			{
				foreach (Item t in _RedSkulls)
				{
					t.Map = Map;
				}
			}

			if (_WhiteSkulls != null)
			{
				foreach (Item t in _WhiteSkulls)
				{
					t.Map = Map;
				}
			}

			UpdateRegion();
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (_Platform != null)
			{
				_Platform.Delete();
			}

			if (_Altar != null)
			{
				_Altar.Delete();
			}

			if (_Idol != null)
			{
				_Idol.Delete();
			}

			if (_RedSkulls != null)
			{
				foreach (Item t in _RedSkulls)
				{
					t.Delete();
				}

				_RedSkulls.Clear();
			}

			if (_WhiteSkulls != null)
			{
				foreach (Item t in _WhiteSkulls)
				{
					t.Delete();
				}

				_WhiteSkulls.Clear();
			}

			DeleteCreatures();

			if (Champion != null && !Champion.Player)
			{
				Champion.Delete();
			}

			Stop();

			UpdateRegion();
		}

		public void ExpireCreatures()
		{
			if (!_Active && !ReadyToActivate && !AlwaysActive)
			{
				DeleteCreatures();
			}
		}

		public void DeleteCreatures()
		{
			if (_Creatures == null)
			{
				return;
			}

			foreach (Mobile mob in _Creatures.Where(mob => !mob.Player))
			{
				mob.Delete();
			}

			_Creatures.Clear();
		}

		public virtual void RegisterDamageTo(Mobile m)
		{
			if (m == null)
			{
				return;
			}

			foreach (DamageEntry de in m.DamageEntries)
			{
				if (de.HasExpired)
				{
					continue;
				}

				Mobile damager = de.Damager;

				Mobile master = damager.GetDamageMaster(m);

				if (master != null)
				{
					damager = master;
				}

				RegisterDamage(damager, de.DamageGiven);
			}
		}

		public void RegisterDamage(Mobile from, int amount)
		{
			if (from == null || !from.Player)
			{
				return;
			}

			if (DamageEntries.ContainsKey(from))
			{
				DamageEntries[from] += amount;
			}
			else
			{
				DamageEntries.Add(from, amount);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version;

			writer.Write(version = 14); // version

			if (version < 14)
			{
				#region Removed in version 14
				// version 13 
				writer.Write(ChampionGlobals.CSOptions.InformChampSpawnRegionMobs);

				// version 12
				writer.Write(PlayerScores.CSOptions.MeleeVsChampMod);
				writer.Write(PlayerScores.CSOptions.ArcherVsChampMod);
				writer.Write(PlayerScores.CSOptions.ArcherMod);

				// version 11
				writer.Write(PlayerScores.CSOptions.SummonMod);
				writer.Write(PlayerScores.CSOptions.ChampionMod);

				// version 10
				var scores = Scores;

				writer.Write(scores != null ? scores.Count : 0);

				if (scores != null)
				{
					foreach (var kvp in scores)
					{
						writer.Write(kvp.Key);
						writer.Write(kvp.Value);
					}
				}

				writer.Write(PlayerScores.CSOptions.BardMod);
				writer.Write(PlayerScores.CSOptions.TamerMod);
				writer.Write(PlayerScores.CSOptions.MaxPoints);
				writer.Write(ChampionGlobals.CSOptions.PowerScrollsToGive);
				writer.Write(ChampionGlobals.CSOptions.PowerScrollMinimumDistance);
				writer.Write(ChampionGlobals.CSOptions.PowerScrollRequireAlive);
				#endregion
			}

			writer.Write(MobScript);
			writer.Write(BossScript);

			// version 9
			writer.Write(_Level);

			writer.Write(ActivatedByProximity);
			writer.WriteDeltaTime(NextProximityTime);

			writer.Write(_MaxLevel); //This can change, based on how you use the champion spawn

			writer.Write(ActivatedByValor);

			writer.Write(DamageEntries.Count);

			foreach (KeyValuePair<Mobile, int> kvp in DamageEntries)
			{
				writer.Write(kvp.Key);
				writer.Write(kvp.Value);
			}

			writer.Write(ConfinedRoaming);
			writer.WriteItem(_Idol);
			writer.Write(HasBeenAdvanced);
			writer.Write(_SpawnArea);

			writer.Write(RandomizeType);

			// writer.Write( _SpawnRange );
			writer.Write(_Kills);

			writer.Write(_Active);
			writer.Write((int)_SpawnType);
			writer.Write(_Creatures, true);
			writer.Write(_RedSkulls, true);
			writer.Write(_WhiteSkulls, true);
			writer.WriteItem(_Platform);
			writer.WriteItem(_Altar);
			writer.Write(ExpireDelay);
			writer.WriteDeltaTime(ExpireTime);
			writer.Write(Champion);
			writer.Write(RestartDelay);

			writer.Write(RestartTimer != null);

			if (RestartTimer != null)
			{
				writer.WriteDeltaTime(RestartTime);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			DamageEntries = new Dictionary<Mobile, int>();

			int version = reader.ReadInt();

			switch (version)
			{
				case 14:
				case 13:
					{
						if (version < 14)
						{
							ChampionGlobals.CSOptions.InformChampSpawnRegionMobs = reader.ReadBool();
						}
					}
					goto case 12;
				case 12:
					{
						if (version < 14)
						{
							PlayerScores.CSOptions.MeleeVsChampMod = reader.ReadDouble();
							PlayerScores.CSOptions.ArcherVsChampMod = reader.ReadDouble();
							PlayerScores.CSOptions.ArcherMod = reader.ReadDouble();
						}
					}
					goto case 11;
				case 11:
					{
						if (version < 14)
						{
							PlayerScores.CSOptions.SummonMod = reader.ReadDouble();
							PlayerScores.CSOptions.ChampionMod = reader.ReadDouble();
						}
					}
					goto case 10;
				case 10:
					{
						if (version < 14)
						{
							Dictionary<Mobile, double> scores = Scores ?? new Dictionary<Mobile, double>();

							int entries = reader.ReadInt();

							for (int i = 0; i < entries; ++i)
							{
								Mobile m = reader.ReadMobile();
								double damage = reader.ReadDouble();

								if (m != null)
								{
									scores.Add(m, damage);
								}
							}

							PlayerScores.CSOptions.BardMod = reader.ReadDouble();
							PlayerScores.CSOptions.TamerMod = reader.ReadDouble();
							PlayerScores.CSOptions.MaxPoints = reader.ReadDouble();
							ChampionGlobals.CSOptions.PowerScrollsToGive = reader.ReadInt();
							ChampionGlobals.CSOptions.PowerScrollMinimumDistance = reader.ReadInt();
							ChampionGlobals.CSOptions.PowerScrollRequireAlive = reader.ReadBool();
						}

						MobScript = reader.ReadString();
						BossScript = reader.ReadString();
					}
					goto case 9;
				case 9:
					_Level = reader.ReadInt();
					goto case 8;
				case 8:
					{
						ActivatedByProximity = reader.ReadBool();
						NextProximityTime = reader.ReadDeltaTime();
					}
					goto case 7;
				case 7:
					_MaxLevel = reader.ReadInt();
					goto case 6;
				case 6:
					{
						if (version < 7)
						{
							_MaxLevel = 16 + Utility.Random(3); //full levels
						}

						ActivatedByValor = reader.ReadBool();
					}
					goto case 5;
				case 5:
					{
						int entries = reader.ReadInt();

						for (int i = 0; i < entries; ++i)
						{
							Mobile m = reader.ReadMobile();
							int damage = reader.ReadInt();

							if (m != null)
							{
								DamageEntries.Add(m, damage);
							}
						}
					}
					goto case 4;
				case 4:
					{
						ConfinedRoaming = reader.ReadBool();
						_Idol = reader.ReadItem<IdolOfTheChampion>();
						HasBeenAdvanced = reader.ReadBool();
					}
					goto case 3;
				case 3:
					_SpawnArea = reader.ReadRect2D();
					goto case 2;
				case 2:
					RandomizeType = reader.ReadBool();
					goto case 1;
				case 1:
					{
						if (version < 3)
						{
							int oldRange = reader.ReadInt();

							_SpawnArea = new Rectangle2D(new Point2D(X - oldRange, Y - oldRange), new Point2D(X + oldRange, Y + oldRange));
						}

						_Kills = reader.ReadInt();
					}
					goto case 0;
				case 0:
					{
						if (version < 1)
						{
							//Default was 24
							_SpawnArea = new Rectangle2D(new Point2D(X - 24, Y - 24), new Point2D(X + 24, Y + 24));
						}

						bool active = reader.ReadBool();

						_SpawnType = (ChampionSpawnType)reader.ReadInt();
						_Creatures = reader.ReadStrongMobileList();
						_RedSkulls = reader.ReadStrongItemList();
						_WhiteSkulls = reader.ReadStrongItemList();
						_Platform = reader.ReadItem<ChampionPlatform>();
						_Altar = reader.ReadItem<ChampionAltar>();
						ExpireDelay = reader.ReadTimeSpan();
						ExpireTime = reader.ReadDeltaTime();
						Champion = reader.ReadMobile();
						RestartDelay = reader.ReadTimeSpan();

						if (reader.ReadBool())
						{
							RestartTime = reader.ReadDeltaTime();
							BeginRestart(RestartTime - DateTime.UtcNow);
						}

						if (version < 4)
						{
							_Idol = new IdolOfTheChampion(this);
							_Idol.MoveToWorld(new Point3D(X, Y, Z - 15), Map);
						}

						if (_Platform == null || _Altar == null || _Idol == null)
						{
							Delete();
						}
						else if (active)
						{
							Start();
						}
						else if (AlwaysActive)
						{
							ReadyToActivate = true;
						}

						foreach (BaseCreature t in _Creatures.OfType<BaseCreature>())
						{
							t.ChampSpawn = this;
						}
					}
					break;
			}

			Timer.DelayCall(TimeSpan.Zero, UpdateRegion);
		}
	}

	public class ChampionSpawnRegion : BaseRegion
	{
		public ChampionSpawn Spawn { get; private set; }

		public ChampionSpawnRegion(ChampionSpawn spawn)
			: base(null, spawn.Map, Find(spawn.Location, spawn.Map), spawn.SpawnArea)
		{
			Spawn = spawn;
		}

		public override bool AllowHousing(Mobile from, Point3D p)
		{
			return false;
		}

		public bool CanSpawn()
		{
			return Spawn.EjectLocation != new Point3D(0, 0, 0) && Spawn.EjectMap != null;
		}

		public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
		{
			base.AlterLightLevel(m, ref global, ref personal);

			//This is a guesstimate. 
			global = Math.Max(global, 1 + Spawn.Level);
		}

		public override void OnEnter(Mobile m)
		{
			if (!m.Player || m.AccessLevel != AccessLevel.Player || Spawn.Active)
			{
				return;
			}

			if (Spawn.ReadyToActivate)
			{
				Spawn.Start();
			}
			else if (Spawn.ProximitySpawn && !Spawn.ActivatedByProximity && DateTime.UtcNow >= Spawn.NextProximityTime)
			{
				PlayerMobile[] players =
					Spawn.GetPlayersInRange(Spawn.Map, 25).Where(mob => mob.AccessLevel == AccessLevel.Player).ToArray();

				if (players.Length >= 3)
				{
					foreach (PlayerMobile player in players)
					{
						player.SendMessage(0x20, Spawn.BroadcastMessage);
					}

					Spawn.ActivatedByProximity = true;
					Spawn.BeginRestart(TimeSpan.FromSeconds(30.0));
				}
			}
		}

		public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
		{
			if (!base.OnMoveInto(m, d, newLocation, oldLocation))
			{
				return false;
			}

			if (!m.Player)
			{
				return true;
			}

			if (((PlayerMobile)m).Young)
			{
				m.SendMessage("You decide against going here because of the danger.");
				return false;
			}

			return true;
		}

		public override bool OnBeforeDeath(Mobile m)
		{
			if (Parent != null && !Parent.OnBeforeDeath(m))
			{
				return false;
			}

			if (!m.Player)
			{
				return true;
			}

			m.SendMessage("A magical force encompasses you, attempting to force you out of the area.");

			new EjectTimer(m, this).Start();

			return true;
		}

		private sealed class EjectTimer : Timer
		{
			private readonly Mobile _From;
			private readonly ChampionSpawnRegion _Region;

			public EjectTimer(Mobile from, ChampionSpawnRegion region)
				: base(TimeSpan.FromMinutes(1.0))
			{
				_From = from;
				_Region = region;
			}

			protected override void OnTick()
			{
				//See if they are dead, or logged out!
				if (_Region.Spawn == null || !_Region.CanSpawn() || _From.Alive)
				{
					return;
				}

				if (_From.NetState != null)
				{
					if (!_From.Region.IsPartOf(_Region))
					{
						return;
					}

					_From.MoveToWorld(_Region.Spawn.EjectLocation, _Region.Spawn.EjectMap);
					_From.SendMessage("A magical force forces you out of the area.");
				}
				else if (_From.NetState == null && Find(_From.LogoutLocation, _From.LogoutMap).IsPartOf(_Region))
				{
					_From.LogoutLocation = _Region.Spawn.EjectLocation;
					_From.LogoutMap = _Region.Spawn.EjectMap;
				}
			}
		}
	}

	public class IdolOfTheChampion : Item
	{
		public ChampionSpawn Spawn { get; private set; }

		public override string DefaultName { get { return "Idol of the Champion"; } }

		public IdolOfTheChampion(ChampionSpawn spawn)
			: base(0x1F18)
		{
			Spawn = spawn;
			Movable = false;
		}

		public IdolOfTheChampion(Serial serial)
			: base(serial)
		{ }

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (Spawn == null)
			{
				return;
			}

			Spawn.Delete();
			Spawn = null;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(Spawn);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					Spawn = reader.ReadItem<ChampionSpawn>();
					break;
			}

			if (Spawn == null)
			{
				Delete();
			}
		}
	}
}