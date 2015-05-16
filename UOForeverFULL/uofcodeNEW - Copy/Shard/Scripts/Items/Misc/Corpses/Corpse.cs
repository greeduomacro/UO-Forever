#region References
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Accounting;
using Server.ContextMenus;
using Server.Engines.CannedEvil;
using Server.Engines.Conquests;
using Server.Engines.PartySystem;
using Server.Engines.Quests;
using Server.Engines.Quests.Doom;
using Server.Engines.Quests.Haven;
using Server.Engines.XmlSpawner2;
using Server.Factions;
using Server.Guilds;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Scripts.New.Adam;

using VitaNex;
#endregion

namespace Server.Items
{
	public interface IDevourer
	{
		bool Devour(Corpse corpse);
	}

	[Flags]
	public enum CorpseFlag
	{
		None = 0x00000000,

		/// <summary>
		///     Has this corpse been carved?
		/// </summary>
		Carved = 0x00000001,

		/// <summary>
		///     If true, this corpse will not turn into bones
		/// </summary>
		NoBones = 0x00000002,

		/// <summary>
		///     If true, the corpse has turned into bones
		/// </summary>
		IsBones = 0x00000004,

		/// <summary>
		///     Has this corpse yet been visited by a taxidermist?
		/// </summary>
		VisitedByTaxidermist = 0x00000008,

		/// <summary>
		///     Has this corpse yet been used to channel spiritual energy? (AOS Spirit Speak)
		/// </summary>
		Channeled = 0x00000010,

		/// <summary>
		///     Was the owner criminal when he died?
		/// </summary>
		Criminal = 0x00000020,

		/// <summary>
		///     Has this corpse been animated?
		/// </summary>
		Animated = 0x00000040,

		/// <summary>
		///     Has this corpse been self looted?
		/// </summary>
		SelfLooted = 0x00000080,
	}

	public class Corpse : Container, ICarvable
	{
		[CommandProperty(AccessLevel.Counselor)]
		public override bool ExpansionChangeAllowed { get { return true; } }

		private Mobile m_Owner; // Whos corpse is this?
		private Mobile m_Killer; // Who killed the owner?
		private CorpseFlag m_Flags; // @see CorpseFlag

		private List<Mobile> m_Looters; // Who's looted this corpse?

		private List<Item> m_EquipItems;
		// List of items equipped when the owner died. Ingame, these items display /on/ the corpse, not just inside

		private List<Item> m_RestoreEquip; // List of items equipped when the owner died. Includes insured and blessed items.

		private List<Mobile> m_Aggressors;
		// Anyone from this list will be able to loot this corpse; we attacked them, or they attacked us when we were freely attackable

		private string m_CorpseName;
		// Value of the CorpseNameAttribute attached to the owner when he died -or- null if the owner had no CorpseNameAttribute; use "the remains of ~name~"

		private IDevourer m_Devourer; // The creature that devoured this corpse

		private int m_BloodHue; // Hue of the blood of the slayed creature.

		// For notoriety:
		private AccessLevel m_AccessLevel; // Which AccessLevel the owner had when he died
		private readonly Guild m_Guild; // Which Guild the owner was in when he died
		private int m_Kills; // How many kills the owner had when he died

		private DateTime m_TimeOfDeath; // What time was this corpse created?

		// For Forensics Evaluation

		public static readonly TimeSpan MonsterLootRightSacrifice = TimeSpan.FromMinutes(0.20);

		public static readonly TimeSpan InstancedCorpseTime = TimeSpan.FromMinutes(3.0);

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool InstancedCorpse { get { return EraSE && DateTime.UtcNow < m_TimeOfDeath + InstancedCorpseTime; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string Forensicist { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public Corpse ProxyCorpse { get; set; }

		private Dictionary<Item, InstancedItemInfo> m_InstancedItems;

		private class InstancedItemInfo
		{
			private readonly Mobile m_Mobile;
			private readonly Item m_Item;

			public bool Perpetual { get; set; }

			public InstancedItemInfo(Item i, Mobile m)
			{
				m_Item = i;
				m_Mobile = m;
			}

			public bool IsOwner(Mobile m)
			{
				if (m_Item.LootType == LootType.Cursed) //Cursed Items are part of everyone's instanced corpse... (?)
				{
					return true;
				}

				if (m == null)
				{
					return false; //sanity
				}

				if (m_Mobile == m)
				{
					return true;
				}

				Party myParty = Party.Get(m_Mobile);

				return (myParty != null && myParty == Party.Get(m));
			}
		}

		public override bool IsChildVisibleTo(Mobile m, Item child)
		{
			if (!m.Player || m.AccessLevel > AccessLevel.Player) //Staff and creatures not subject to instancing.
			{
				return true;
			}

			if (m_InstancedItems != null)
			{
				InstancedItemInfo info;

				if (m_InstancedItems.TryGetValue(child, out info) && (InstancedCorpse || info.Perpetual))
				{
					return info.IsOwner(m); //IsOwner checks Party stuff.
				}
			}

			return true;
		}

		private void AssignInstancedLoot()
		{
			if (m_Aggressors.Count == 0 || Items.Count == 0)
			{
				return;
			}

			if (m_InstancedItems == null)
			{
				m_InstancedItems = new Dictionary<Item, InstancedItemInfo>();
			}

			var m_Stackables = new List<Item>();
			var m_Unstackables = new List<Item>();

			//Don't have cursed items take up someone's item spot.. (?)
			foreach (Item item in Items.Where(item => item.LootType != LootType.Cursed))
			{
				if (item.Stackable)
				{
					m_Stackables.Add(item);
				}
				else
				{
					m_Unstackables.Add(item);
				}
			}

			var attackers = new List<Mobile>(m_Aggressors);

			for (int i = 1; i < attackers.Count - 1; i++) //randomize
			{
				int rand = Utility.Random(i + 1);

				Mobile temp = attackers[rand];
				attackers[rand] = attackers[i];
				attackers[i] = temp;
			}

			//stackables first, for the remaining stackables, have those be randomly added after

			foreach (Item item in m_Stackables)
			{
				if (item.Amount >= attackers.Count)
				{
					int amountPerAttacker = (item.Amount / attackers.Count);
					int remainder = (item.Amount % attackers.Count);

					for (int j = 0; j < ((remainder == 0) ? attackers.Count - 1 : attackers.Count); j++)
					{
						Item splitItem = Mobile.LiftItemDupe(item, item.Amount - amountPerAttacker);
						//LiftItemDupe automagically adds it as a child item to the corpse

						m_InstancedItems.Add(splitItem, new InstancedItemInfo(splitItem, attackers[j]));

						//What happens to the remaining portion?  TEMP FOR NOW UNTIL OSI VERIFICATION:  Treat as Single Item.
					}

					if (remainder == 0)
					{
						m_InstancedItems.Add(item, new InstancedItemInfo(item, attackers[attackers.Count - 1]));
						//Add in the original item (which has an equal amount as the others) to the instance for the last attacker, cause it wasn't added above.
					}
					else
					{
						m_Unstackables.Add(item);
					}
				}
				else
				{
					//What happens in this case?  TEMP FOR NOW UNTIL OSI VERIFICATION:  Treat as Single Item.
					m_Unstackables.Add(item);
				}
			}

			for (int i = 0; i < m_Unstackables.Count; i++)
			{
				Mobile m = attackers[i % attackers.Count];
				Item item = m_Unstackables[i];

				m_InstancedItems.Add(item, new InstancedItemInfo(item, m));
			}
		}

		public void AddCarvedItem(Item carved, Mobile carver)
		{
			DropItem(carved);

			if (InstancedCorpse)
			{
				if (m_InstancedItems == null)
				{
					m_InstancedItems = new Dictionary<Item, InstancedItemInfo>();
				}

				m_InstancedItems.Add(carved, new InstancedItemInfo(carved, carver));
			}
		}

		public override bool IsDecoContainer { get { return false; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime TimeOfDeath { get { return m_TimeOfDeath; } set { m_TimeOfDeath = value; } }

		public override bool DisplayWeight { get { return false; } }

		public HairInfo Hair { get; set; }
		public FacialHairInfo FacialHair { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsBones { get { return GetFlag(CorpseFlag.IsBones); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Devoured { get { return (m_Devourer != null); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Carved { get { return GetFlag(CorpseFlag.Carved); } set { SetFlag(CorpseFlag.Carved, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool VisitedByTaxidermist { get { return GetFlag(CorpseFlag.VisitedByTaxidermist); } set { SetFlag(CorpseFlag.VisitedByTaxidermist, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Channeled { get { return GetFlag(CorpseFlag.Channeled); } set { SetFlag(CorpseFlag.Channeled, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Animated { get { return GetFlag(CorpseFlag.Animated); } set { SetFlag(CorpseFlag.Animated, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool SelfLooted { get { return GetFlag(CorpseFlag.SelfLooted); } set { SetFlag(CorpseFlag.SelfLooted, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int BloodHue { get { return m_BloodHue; } set { m_BloodHue = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public AccessLevel AccessLevel { get { return m_AccessLevel; } }

		public List<Mobile> Aggressors { get { return m_Aggressors; } }

		public List<Mobile> Looters { get { return m_Looters; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Killer { get { return m_Killer; } }

		public List<Item> EquipItems { get { return m_EquipItems; } }

		public List<Item> RestoreEquip { get { return m_RestoreEquip; } set { m_RestoreEquip = value; } }

		public Guild Guild { get { return m_Guild; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Kills { get { return m_Kills; } set { m_Kills = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Criminal { get { return GetFlag(CorpseFlag.Criminal); } set { SetFlag(CorpseFlag.Criminal, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Owner { get { return m_Owner; } }

		public void TurnToBones()
		{
			if (Deleted)
			{
				return;
			}

			ProcessDelta();
			SendRemovePacket();

			if (Amount != 508)
			{
				ItemID = Utility.Random(0xECA, 9); // bone graphic
				Hue = 0;
			}

			ProcessDelta();

			SetFlag(CorpseFlag.NoBones, true);
			SetFlag(CorpseFlag.IsBones, true);
			SetFlag(CorpseFlag.Criminal, true);
			BeginDecay(m_BoneDecayTime);
		}

		private static readonly TimeSpan m_DefaultDecayTime = TimeSpan.FromMinutes(10.0);
		private static readonly TimeSpan m_BoneDecayTime = TimeSpan.FromMinutes(5.0);

		private DateTime m_DecayTime;

		public void BeginDecay(TimeSpan delay)
		{
			m_DecayTime = delay >= TimeSpan.Zero ? DateTime.UtcNow + delay : DateTime.UtcNow;
		}

		private static readonly List<Corpse> _Instances;
		private static readonly PollTimer[] _DecayTimers;
		private static bool _Processing;

		static Corpse()
		{
			_Instances = new List<Corpse>();
			_DecayTimers = new PollTimer[10];
		}

		public static void Initialize()
		{
			Mobile.CreateCorpseHandler += Mobile_CreateCorpseHandler;

			if (_DecayTimers.All(t => t == null))
			{
				_DecayTimers.SetAll(i => PollTimer.FromSeconds(15.0, () => ProcessDecay(i), () => _Instances.Count > i * 1000));
			}
			else
			{
				_DecayTimers.For(i => _DecayTimers[i].Running = true);
			}
		}

		public static void ProcessDecay(int i)
		{
			if (_Processing)
			{
				return;
			}

			_Processing = true;

			VitaNexCore.TryCatch(
				() =>
				{
					DateTime now = DateTime.UtcNow;

					int skip = i * 1000;
					int take = i == _DecayTimers.Length - 1 ? _Instances.Count - skip : (1 + i) * 1000;

					foreach (Corpse corpse in
						_Instances.AsParallel()
								  .Skip(skip)
								  .Take(take)
								  .Where(c => !c.Deleted && !c.DoesNotDecay && c.m_DecayTime <= now)
								  .ToArray())
					{
						if (!corpse.GetFlag(CorpseFlag.NoBones))
						{
							corpse.TurnToBones();
						}
						else
						{
							corpse.Delete();
						}
					}

					_Instances.RemoveAll(c => c == null || c.Deleted);
				},
				x => x.ToConsole(true));

			_Processing = false;
		}

		public static string GetCorpseName(Mobile m)
		{
			if (m is BaseCreature)
			{
				var bc = (BaseCreature)m;

				if (bc.CorpseNameOverride != null)
				{
					return bc.CorpseNameOverride;
				}
			}

			Type t = m.GetType();

			object[] attrs = t.GetCustomAttributes(typeof(CorpseNameAttribute), true);

			if (attrs.Length > 0)
			{
				var attr = attrs[0] as CorpseNameAttribute;

				if (attr != null)
				{
					return attr.Name;
				}
			}

			return null;
		}

		public static Container Mobile_CreateCorpseHandler(
			Mobile owner, HairInfo hair, FacialHairInfo facialhair, List<Item> initialContent, List<Item> equipItems)
		{
			var bc = owner as BaseCreature;

			bool champspawn = false;

			if (bc != null)
			{
				var region = Region.Find(owner.Location, owner.Map) as ChampionSpawnRegion;

				if (region != null)
				{
					ChampionSpawn spawn = region.Spawn;

					if (spawn != null && spawn.IsChampionSpawn(bc))
					{
						champspawn = true;
					}
				}
			}

			Corpse c;
			Corpse p = null;

			if (owner is MilitiaFighter)
			{
				c = new MilitiaFighterCorpse(owner, hair, facialhair, equipItems);
			}
			else
			{
				c = new Corpse(owner, hair, facialhair, equipItems, champspawn);
			}

			owner.Corpse = c;

			var proxyEquip = new List<Item>();

			for (int i = initialContent.Count - 1; i >= 0; i--)
			{
				Item item = initialContent[i];

				if (owner.Player)
				{
					c.DropItem(owner, item);
				}
				else
				{
					if (equipItems.Contains(item))
					{
						proxyEquip.Add(DupeProxyEquipped(item));
					}

					if (item.GetSavedFlag(0x02))
					{
						item.LootType = LootType.Blessed;
						item.SetSavedFlag(0x02, false);
					}

					if (!item.GetSavedFlag(0x01))
					{
						c.DropItem(owner, item);
					}
				}
			}

			if (proxyEquip.Count > 0)
			{
				c.Stackable = true;
				c.Amount = 508; //Original corpse is invisible
				c.Stackable = false;
				c.Hair = null;
				c.FacialHair = null;

				if (owner is MilitiaFighter)
				{
					p = new MilitiaFighterCorpse(owner, hair, facialhair, proxyEquip);
				}
				else
				{
					p = new Corpse(owner, hair, facialhair, proxyEquip);
				}

				foreach (Item t in proxyEquip)
				{
					p.DropItem(owner, t);
				}

				p.ProxyCorpse = c;
				owner.Corpse = p;
			}

			if (c.EraAOS)
			{
				var pm = owner as PlayerMobile;

				if (pm != null)
				{
					c.RestoreEquip = pm.EquipSnapshot;
				}
			}

			Point3D loc = owner.Location;
			Map map = owner.Map;

			if (map == null || map == Map.Internal)
			{
				loc = owner.LogoutLocation;
				map = owner.LogoutMap;
			}

			c.MoveToWorld(loc, map);

			if (p != null)
			{
				p.MoveToWorld(loc, map);
			}

			return p ?? c;
		}

		private static Item DupeProxyEquipped(Item orig)
		{
			var item = new Item(orig.ItemID)
			{
				Hue = orig.Hue,
				Layer = orig.Layer
			};

			item.SetSavedFlag(0x1, true); //Not lootable!

			return item;
		}

		private void DropItem(Mobile owner, Item item)
		{
			if (EraAOS && owner.Player && item.Parent == owner.Backpack)
			{
				AddItem(item);
			}
			else
			{
				DropItem(item);
			}

			if (EraAOS && owner.Player)
			{
				SetRestoreInfo(item, item.Location);
			}
		}

		public override void OnDelete()
		{
			if (ProxyCorpse != null && !ProxyCorpse.Deleted)
			{
				ProxyCorpse.Delete();
			}
		}

		public override bool IsPublicContainer { get { return true; } }

		public Corpse(Mobile owner, List<Item> equipItems)
			: this(owner, null, null, equipItems)
		{ }

		public Corpse(Mobile owner, HairInfo hair, FacialHairInfo facialhair, List<Item> equipItems)
			: this(owner, hair, facialhair, equipItems, false)
		{ }

		public Corpse(Mobile owner, HairInfo hair, FacialHairInfo facialhair, List<Item> equipItems, bool champspawn)
			: base(0x2006)
		{
			if (owner != null && owner.CustomTeam)
			{
				List<XmlTeam> ownerTeams = XmlAttach.GetTeams(owner);

				foreach (XmlTeam copy in ownerTeams.Select(
					team => new XmlTeam
					{
						TeamGreen = team.TeamGreen,
						TeamHarmAllowed = team.TeamHarmAllowed
					}))
				{
					copy.TeamVal = copy.TeamVal;

					XmlAttach.AttachTo(this, copy);
				}
			}

			// To supress console warnings, stackable must be true
			Stackable = true;
			Amount = owner == null ? 400 : (int)owner.Body; // protocol defines that for itemid 0x2006, amount=body
			Stackable = false;

			Movable = false;
			Hue = owner == null ? 0 : owner.Hue;
			Direction = owner == null ? Direction.North : owner.Direction;
			Name = owner == null ? "unidentifiable remains" : owner.Name;

			var bc = owner as BaseCreature;

			m_Owner = owner;

			m_CorpseName = GetCorpseName(owner);

			m_TimeOfDeath = DateTime.UtcNow;

			m_AccessLevel = owner == null ? AccessLevel.Player : owner.AccessLevel;
			m_Guild = owner == null ? null : owner.Guild as Guild;
			m_Kills = bc != null && bc.AlwaysMurderer ? 100 : owner == null ? 0 : owner.Kills;
			SetFlag(CorpseFlag.Criminal, owner != null && owner.Criminal);

			Hair = hair;
			FacialHair = facialhair;

			// This corpse does not turn to bones if: the owner is not a player
			SetFlag(CorpseFlag.NoBones, owner == null || !owner.Player);
			m_BloodHue = owner == null ? 0x22 : owner.BloodHue;

			m_Looters = new List<Mobile>();
			m_EquipItems = equipItems;

			m_Aggressors = new List<Mobile>(owner == null ? 0 : owner.Aggressors.Count + owner.Aggressed.Count);

			bool addToAggressors = bc == null;
			TimeSpan lastTime = TimeSpan.MaxValue;

			if (owner != null)
			{
				foreach (AggressorInfo info in owner.Aggressors)
				{
					if ((DateTime.UtcNow - info.LastCombatTime) < lastTime)
					{
						m_Killer = info.Attacker;
						lastTime = (DateTime.UtcNow - info.LastCombatTime);
					}

					if (addToAggressors && !info.CriminalAggression)
					{
						m_Aggressors.Add(info.Attacker);
					}
				}

				foreach (AggressorInfo info in owner.Aggressed)
				{
					if ((DateTime.UtcNow - info.LastCombatTime) < lastTime)
					{
						m_Killer = info.Defender;
						lastTime = (DateTime.UtcNow - info.LastCombatTime);
					}

					if (addToAggressors)
					{
						m_Aggressors.Add(info.Defender);
					}
				}
			}

			if (!addToAggressors)
			{
				Mobile master = bc.GetMaster();

				if (master != null)
				{
					m_Aggressors.Add(master);
				}

				List<DamageStore> rights = BaseCreature.GetLootingRights(bc.DamageEntries, bc.HitsMax);

				foreach (DamageStore ds in rights.Where(ds => ds.m_HasRight))
				{
					m_Aggressors.Add(ds.m_Mobile);
				}
			}

			TimeSpan decayTime = m_DefaultDecayTime;

			if (champspawn)
			{
				decayTime = TimeSpan.FromSeconds(120.0);
			}

			BeginDecay(decayTime);

			DevourCorpse();

			_Instances.Add(this);
		}

		public Corpse(Serial serial)
			: base(serial)
		{ }

		public override void OnAfterDelete()
		{
			if (!_Processing)
			{
				_Instances.Remove(this);
			}

			base.OnAfterDelete();
		}

		protected bool GetFlag(CorpseFlag flag)
		{
			return ((m_Flags & flag) != 0);
		}

		protected void SetFlag(CorpseFlag flag, bool on)
		{
			m_Flags = (on ? m_Flags | flag : m_Flags & ~flag);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(15); // version

			writer.Write(ProxyCorpse);

			if (m_RestoreEquip == null)
			{
				writer.Write(false);
			}
			else
			{
				writer.Write(true);
				writer.Write(m_RestoreEquip);
			}

			writer.Write(m_BloodHue);

			writer.Write((int)m_Flags);

			writer.WriteDeltaTime(m_TimeOfDeath);

			List<KeyValuePair<Item, Point3D>> list = m_RestoreTable == null
														 ? new List<KeyValuePair<Item, Point3D>>()
														 : new List<KeyValuePair<Item, Point3D>>(m_RestoreTable);
			int count = list.Count;

			writer.Write(count);

			for (int i = 0; i < count; ++i)
			{
				KeyValuePair<Item, Point3D> kvp = list[i];
				Item item = kvp.Key;
				Point3D loc = kvp.Value;

				writer.Write(item);

				if (item.Location == loc)
				{
					writer.Write(false);
				}
				else
				{
					writer.Write(true);
					writer.Write(loc);
				}
			}

			//Removed in v15
			/*writer.Write( m_DecayTimer != null );

			if ( m_DecayTimer != null )*/
			writer.WriteDeltaTime(m_DecayTime);

			writer.Write(m_Looters);
			writer.Write(m_Killer);

			writer.Write(m_Aggressors);

			writer.Write(m_Owner);

			writer.Write(m_CorpseName);

			writer.Write((int)m_AccessLevel);
			writer.Write(m_Guild);
			writer.Write(m_Kills);

			writer.Write(m_EquipItems);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 15: //Changed decay timer serialization.
				case 14: //Proxy corpse for looting when corpses have non-lootables
					ProxyCorpse = reader.ReadItem() as Corpse;
					goto case 13;
				case 13:
					{
						if (reader.ReadBool())
						{
							m_RestoreEquip = reader.ReadStrongItemList();
						}
					}
					goto case 12;
				case 12:
					m_BloodHue = reader.ReadInt();
					goto case 11;
				case 11:
					{
						// Version 11, we move all bools to a CorpseFlag
						m_Flags = (CorpseFlag)reader.ReadInt();

						m_TimeOfDeath = reader.ReadDeltaTime();

						int count = reader.ReadInt();

						for (int i = 0; i < count; ++i)
						{
							Item item = reader.ReadItem();

							if (reader.ReadBool())
							{
								SetRestoreInfo(item, reader.ReadPoint3D());
							}
							else if (item != null)
							{
								SetRestoreInfo(item, item.Location);
							}
						}

						if (version > 14 || reader.ReadBool())
						{
							BeginDecay(reader.ReadDeltaTime() - DateTime.UtcNow);
						}

						m_Looters = reader.ReadStrongMobileList();
						m_Killer = reader.ReadMobile();

						m_Aggressors = reader.ReadStrongMobileList();
						m_Owner = reader.ReadMobile();

						m_CorpseName = reader.ReadString();

						m_AccessLevel = (AccessLevel)reader.ReadInt();
						reader.ReadInt(); // guild reserve
						m_Kills = reader.ReadInt();

						m_EquipItems = reader.ReadStrongItemList();
					}
					break;
					//MAJOR REVISION BREAK
				case 10:
					m_TimeOfDeath = reader.ReadDeltaTime();
					goto case 9;
				case 9:
					{
						int count = reader.ReadInt();

						for (int i = 0; i < count; ++i)
						{
							Item item = reader.ReadItem();

							if (reader.ReadBool())
							{
								SetRestoreInfo(item, reader.ReadPoint3D());
							}
							else if (item != null)
							{
								SetRestoreInfo(item, item.Location);
							}
						}
					}
					goto case 8;
				case 8:
					SetFlag(CorpseFlag.VisitedByTaxidermist, reader.ReadBool());
					goto case 7;
				case 7:
					{
						if (version > 14 || reader.ReadBool())
						{
							BeginDecay(reader.ReadDeltaTime() - DateTime.UtcNow);
						}
					}
					goto case 6;
				case 6:
					{
						m_Looters = reader.ReadStrongMobileList();
						m_Killer = reader.ReadMobile();
					}
					goto case 5;
				case 5:
					SetFlag(CorpseFlag.Carved, reader.ReadBool());
					goto case 4;
				case 4:
					m_Aggressors = reader.ReadStrongMobileList();
					goto case 3;
				case 3:
					m_Owner = reader.ReadMobile();
					goto case 2;
				case 2:
					SetFlag(CorpseFlag.NoBones, reader.ReadBool());
					goto case 1;
				case 1:
					m_CorpseName = reader.ReadString();
					goto case 0;
				case 0:
					{
						if (version < 10)
						{
							m_TimeOfDeath = DateTime.UtcNow;
						}

						if (version < 7)
						{
							BeginDecay(m_DefaultDecayTime);
						}

						if (version < 6)
						{
							m_Looters = new List<Mobile>();
						}

						if (version < 4)
						{
							m_Aggressors = new List<Mobile>();
						}

						m_AccessLevel = (AccessLevel)reader.ReadInt();
						reader.ReadInt(); // guild reserve
						m_Kills = reader.ReadInt();
						SetFlag(CorpseFlag.Criminal, reader.ReadBool());

						m_EquipItems = reader.ReadStrongItemList();
					}
					break;
			}

			_Instances.Add(this);
		}

		public bool DevourCorpse()
		{
			if (Devoured || Deleted || m_Killer == null || m_Killer.Deleted || !m_Killer.Alive || !(m_Killer is IDevourer) ||
				m_Owner == null || m_Owner.Deleted)
			{
				return false;
			}

			m_Devourer = (IDevourer)m_Killer; // Set the devourer the killer

			return m_Devourer.Devour(this); // Devour the corpse if it hasn't
		}

		public override void SendContentTo(NetState state)
		{
			if (state == null)
			{
				return;
			}

			if (state.ContainerGridLines)
			{
				state.Send(new CorpseContent6017(state.Mobile, this));
			}
			else
			{
				state.Send(new CorpseContent(state.Mobile, this));
			}

			if (ItemID == 0x2006)
			{
				state.Send(new CorpseEquip(state.Mobile, this));
			}
		}

		public override void SendInfoTo(NetState state, bool sendOplPacket)
		{
			base.SendInfoTo(state, sendOplPacket);

			if (state == null || ItemID != 0x2006)
			{
				return;
			}

			if (state.ContainerGridLines)
			{
				state.Send(new CorpseEquipContent6017(state.Mobile, this));
			}
			else
			{
				state.Send(new CorpseEquipContent(state.Mobile, this));
			}

			state.Send(new CorpseEquip(state.Mobile, this));
		}

		public bool IsCriminalAction(Mobile from)
		{
			if (from == m_Owner || from.AccessLevel >= AccessLevel.GameMaster)
			{
				return false;
			}

			Party p = Party.Get(m_Owner);

			if (p != null && p.Contains(from))
			{
				PartyMemberInfo pmi = p[m_Owner];

				if (pmi != null && pmi.CanLoot)
				{
					return false;
				}
			}



			return NotorietyHandlers.CorpseNotoriety(from, this) == Notoriety.Innocent;
		}

		public override bool CheckItemUse(Mobile from, Item item)
		{
			if (!base.CheckItemUse(from, item))
			{
				return false;
			}

			if (item != this)
			{
				return CanLoot(from, item);
			}

			return true;
		}

		public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
		{
			if (item.GetSavedFlag(0x01)) //Not lootable
			{
				reject = LRReason.CannotLift;
				from.PublicOverheadMessage(MessageType.Regular, 33, false, "[Looting] Attempted to loot sticky item!");
				return false;
			}

			if (!base.CheckLift(from, item, ref reject))
			{
				return false;
			}

			return CanLoot(from, item);
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (ProxyCorpse != null)
			{
				return ProxyCorpse.OnDragDrop(from, dropped);
			}

			return base.OnDragDrop(from, dropped);
		}

		public override void OnItemUsed(Mobile from, Item item)
		{
			base.OnItemUsed(from, item);

			if ((EraSE && item is Food) || from != m_Owner)
			{
				from.RevealingAction();
			}

			if (item != this && IsCriminalAction(from))
			{
				from.CriminalAction(true);
			}

			if (!m_Looters.Contains(from))
			{
				m_Looters.Add(from);
			}

			if (m_InstancedItems != null && m_InstancedItems.ContainsKey(item))
			{
				m_InstancedItems.Remove(item);
			}
		}

		public override void OnItemLifted(Mobile from, Item item)
		{
			base.OnItemLifted(from, item);

			if (item != this && from != m_Owner)
			{
				from.RevealingAction();
			}

			if (item != this && IsCriminalAction(from))
			{
				from.CriminalAction(true);
			}

			if (!m_Looters.Contains(from))
			{
				m_Looters.Add(from);
			}

			if (m_InstancedItems != null && m_InstancedItems.ContainsKey(item))
			{
				m_InstancedItems.Remove(item);
			}
		}

		private class OpenCorpseEntry : ContextMenuEntry
		{
			public OpenCorpseEntry()
				: base(6215, 2)
			{ }

			public override void OnClick()
			{
				var corpse = Owner.Target as Corpse;

				if (corpse != null && Owner.From.CheckAlive())
				{
					corpse.Open(Owner.From, false);
				}
			}
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (EraAOS && m_Owner == from && from.Alive)
			{
				list.Add(new OpenCorpseEntry());
			}
		}

		private Dictionary<Item, Point3D> m_RestoreTable;

		public bool GetRestoreInfo(Item item, ref Point3D loc)
		{
			if (m_RestoreTable == null || item == null)
			{
				return false;
			}

			return m_RestoreTable.TryGetValue(item, out loc);
		}

		public void SetRestoreInfo(Item item, Point3D loc)
		{
			if (item == null)
			{
				return;
			}

			if (m_RestoreTable == null)
			{
				m_RestoreTable = new Dictionary<Item, Point3D>();
			}

			m_RestoreTable[item] = loc;
		}

		public void ClearRestoreInfo(Item item)
		{
			if (m_RestoreTable == null || item == null)
			{
				return;
			}

			m_RestoreTable.Remove(item);

			if (m_RestoreTable.Count == 0)
			{
				m_RestoreTable = null;
			}
		}

		public bool CanLoot(Mobile from, Item item)
		{
			Map map = Map;

			return !IsCriminalAction(from) || map == null || (map.Rules & MapRules.HarmfulRestrictions) == 0;
		}

		public bool CheckLoot(Mobile from, Item item)
		{
			if (!CanLoot(from, item))
			{
				if (m_Owner == null || !m_Owner.Player)
				{
					from.SendLocalizedMessage(1005035); // You did not earn the right to loot this creature!
				}
				else
				{
					from.SendLocalizedMessage(1010049); // You may not loot this corpse.
				}

				return false;
			}

			if (DoesNotDecay)
			{
				from.SendMessage("A mystical force prevents you from looting the corpse!");
				return false;
			}

		    if (from is PlayerMobile && from.IsYoung() && IsCriminalAction(from))
		    {
		        from.SendMessage(54, "You may not loot innocent corpses as a young player!");
		        return false;
		    }

            if (from is PlayerMobile && from.IsYoung() && Aggressors != null && !Aggressors.Contains(from))
            {
                from.SendMessage(54, "You may not loot creatures that you did not kill as a young player!");
                return false;
            }

			if (IsCriminalAction(from))
			{
				if (m_Owner == null || !m_Owner.Player)
				{
					from.SendLocalizedMessage(1005036); // Looting this monster corpse will be a criminal act!
				}
				else
				{
					from.SendLocalizedMessage(1005038); // Looting this corpse will be a criminal act!
				}
			}

			return true;
		}

		public virtual void Open(Mobile from, bool checkSelfLoot)
		{
			if (ProxyCorpse != null)
			{
				ProxyCorpse.Open(from, checkSelfLoot);
			}
			else
			{
				if (from.AccessLevel > AccessLevel.Player || from.InRange(GetWorldLocation(), 2))
				{
					#region Self Looting
					if (checkSelfLoot && from == m_Owner && !GetFlag(CorpseFlag.SelfLooted) && Items.Count != 0)
					{
						var robe = from.FindItemOnLayer(Layer.OuterTorso) as DeathRobe;

						if (robe != null)
						{
							Map map = from.Map;

							if (map != null && map != Map.Internal)
							{
								robe.MoveToWorld(from.Location, map);
								robe.BeginDecay();
							}
						}

						Container pack = from.Backpack;

						if (m_RestoreEquip != null && pack != null)
						{
							Item[] packItems = pack.Items.ToArray(); // Only items in the top-level pack are re-equipped

							foreach (Item packItem in packItems.Where(packItem => m_RestoreEquip.Contains(packItem) && packItem.Movable))
							{
								from.EquipItem(packItem);
							}
						}

						var items = new List<Item>(Items);

						bool didntFit = false;

						for (int i = 0; !didntFit && i < items.Count; ++i)
						{
							Item item = items[i];
							Point3D loc = item.Location;

							if (item.Layer == Layer.Hair || item.Layer == Layer.FacialHair || !item.Movable || !GetRestoreInfo(item, ref loc))
							{
								continue;
							}

							if (pack != null && pack.CheckHold(from, item, false, true))
							{
								item.Location = loc;
								pack.AddItem(item);

								if (m_RestoreEquip != null && m_RestoreEquip.Contains(item))
								{
									from.EquipItem(item);
								}
							}
							else
							{
								didntFit = true;
							}
						}

						from.PlaySound(0x3E3);

						if (Items.Count != 0)
						{
							from.SendLocalizedMessage(1062472); // You gather some of your belongings. The rest remain on the corpse.
						}
						else
						{
							SetFlag(CorpseFlag.Carved, true);

							if (ItemID == 0x2006)
							{
								ProcessDelta();
								SendRemovePacket();
								ItemID = Utility.Random(0xECA, 9); // bone graphic
								Hue = 0;
								ProcessDelta();
							}

							from.SendLocalizedMessage(1062471); // You quickly gather all of your belongings.
						}

						SetFlag(CorpseFlag.SelfLooted, true);
					}
					#endregion

					if (!CheckLoot(from, null))
					{
						return;
					}

					#region Quests
					var player = from as PlayerMobile;

					if (player != null)
					{
						QuestSystem qs = player.Quest;

						if (qs is UzeraanTurmoilQuest)
						{
							var obj = qs.FindObjective(typeof(GetDaemonBoneObjective)) as GetDaemonBoneObjective;

							if (obj != null && obj.CorpseWithBone == this &&
								(!obj.Completed || UzeraanTurmoilQuest.HasLostDaemonBone(player)))
							{
								Item bone = new QuestDaemonBone();

								if (player.PlaceInBackpack(bone))
								{
									obj.CorpseWithBone = null;
									player.SendLocalizedMessage(1049341, "", 0x22);
									// You rummage through the bones and find a Daemon Bone!  You quickly place the item in your pack.

									if (!obj.Completed)
									{
										obj.Complete();
									}
								}
								else
								{
									bone.Delete();
									player.SendLocalizedMessage(1049342, "", 0x22);
									// Rummaging through the bones you find a Daemon Bone, but can't pick it up because your pack is too full.
									// Come back when you have more room in your pack.
								}

								return;
							}
						}
						else if (qs is TheSummoningQuest)
						{
							var obj = qs.FindObjective(typeof(VanquishDaemonObjective)) as VanquishDaemonObjective;

							if (obj != null && obj.Completed && obj.CorpseWithSkull == this)
							{
								var sk = new GoldenSkull();

								if (player.PlaceInBackpack(sk))
								{
									obj.CorpseWithSkull = null;

									// For your valor in combating the devourer, you have been awarded a golden skull.
									player.SendLocalizedMessage(1050022);
									qs.Complete();
								}
								else
								{
									sk.Delete();

									// You find a golden skull, but your backpack is too full to carry it.
									player.SendLocalizedMessage(1050023);
								}
							}
						}
					}
					#endregion

					base.OnDoubleClick(from);

					if (!EraSE && from != m_Owner)
					{
						from.RevealingAction();
					}
				}
				else
				{
					from.SendLocalizedMessage(500446); // That is too far away.
				}
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			Open(from, EraAOS);
		}

		public override bool CheckContentDisplay(Mobile from)
		{
			return false;
		}

		public override bool DisplaysContent { get { return false; } }

		public override void AddNameProperty(ObjectPropertyList list)
		{
			if (ItemID == 0x2006) // Corpse form
			{
				if (m_CorpseName != null)
				{
					list.Add(m_CorpseName);
				}
				else
				{
					list.Add(1046414, Name); // the remains of ~1_NAME~
				}
			}
			else // Bone form
			{
				list.Add(1046414, Name); // the remains of ~1_NAME~
			}
		}

		public override void OnAosSingleClick(Mobile from)
		{
			int hue = Notoriety.GetHue(NotorietyHandlers.CorpseNotoriety(from, this));
			ObjectPropertyList opl = PropertyList;

			if (opl.Header > 0)
			{
				from.Send(new MessageLocalized(Serial, ItemID, MessageType.Label, hue, 3, opl.Header, Name, opl.HeaderArgs));
			}
		}

		public override int GetPacketFlags()
		{
			if (Amount == 508)
			{
				return base.GetPacketFlags() | 0x80;
			}

			return base.GetPacketFlags();
		}

		public override void OnSingleClick(Mobile from)
		{
			LabelToExpansion(from);

			int hue = Notoriety.GetHue(NotorietyHandlers.CorpseNotoriety(from, this));

			if (ItemID == 0x2006) // Corpse form
			{
				if (m_CorpseName != null)
				{
					from.Send(new AsciiMessage(Serial, ItemID, MessageType.Label, hue, 3, "", m_CorpseName));
				}
				else
				{
					from.Send(new MessageLocalized(Serial, ItemID, MessageType.Label, hue, 3, 1046414, "", Name));
				}
			}
			else // Bone form
			{
				from.Send(new MessageLocalized(Serial, ItemID, MessageType.Label, hue, 3, 1046414, "", Name));
			}
		}

		public virtual void Carve(Mobile from, Item item)
		{
			if (from == null)
			{
				return;
			}

			if (DoesNotDecay)
			{
				from.SendMessage("A mystical force prevents you from carving the corpse!");
				return;
			}

			var region1 = from.Region as CustomRegion;
			var captureZone = from.Region as CaptureZoneRegion;

			if ((region1 != null && !region1.AllowCutCorpse()) || (captureZone != null && !captureZone.Czone.NoCorpseCarving))
			{
				from.SendMessage("A mystical force prevents you from carving the corpse!");
				return;
			}

			if (IsCriminalAction(from) && Map != null && (Map.Rules & MapRules.HarmfulRestrictions) != 0)
			{
				if (m_Owner == null || !m_Owner.Player)
				{
					from.SendLocalizedMessage(1005035); // You did not earn the right to loot this creature!
				}
				else
				{
					from.SendLocalizedMessage(1010049); // You may not loot this corpse.
				}

				return;
			}

			if (region1 != null && region1.PlayingGame(from))
			{
				from.SendMessage(32, "You cannot carve this corpse while an event is taking place!");
				return;
			}

		    Faction faction = Faction.Find(m_Owner);

            if (faction != null && Region.Find(Location, Map) is TownRegion)
			{
				from.SendMessage(32, "You cannot carve faction member corpses in town!");
				return;
			}

            var p = m_Owner as PlayerMobile;
            if (FactionObelisks.Obelisks != null && p != null && p.FactionPlayerState != null && !(Region.Find(Location, Map) is DungeonRegion))
            {
                var acct = p.Account as Account;
                foreach (var obelisk in FactionObelisks.Obelisks)
                {
                    if (obelisk.ObeliskType == ObeliskType.Bloodshed && !String.IsNullOrEmpty(obelisk.OwningFaction) && acct != null)
                    {
                        if (obelisk.ObeliskUsers != null && obelisk.ObeliskUsers.ContainsKey(acct))
                        {
                            from.SendMessage(61, "This player's body is protected by the point of bloodshed.");
                            return;
                        }
                    }
                }
            }

			Mobile dead = m_Owner;

			if (GetFlag(CorpseFlag.Carved) || dead == null)
			{
				from.SendLocalizedMessage(500485); // You see nothing useful to carve from the corpse.
			}
			else if (((Body)Amount).IsHuman && ItemID == 0x2006)
			{
				if (m_BloodHue > -1)
				{
					new Blood(0x122D, m_BloodHue).MoveToWorld(Location, Map);
				}

				if (dead is PlayerMobile)
				{
					new Head2((PlayerMobile)dead, dead.Region).MoveToWorld(Location, Map);
					new Torso().MoveToWorld(Location, Map);
					new LeftLeg().MoveToWorld(Location, Map);
					new LeftArm().MoveToWorld(Location, Map);
					new RightLeg().MoveToWorld(Location, Map);
					new RightArm().MoveToWorld(Location, Map);
                    new RawHumanFlesh(2, dead).MoveToWorld(Location, Map);
                    new RawHumanBrain(1, dead).MoveToWorld(Location, Map);
				}
				else
				{
					new Torso().MoveToWorld(Location, Map);
					new LeftLeg().MoveToWorld(Location, Map);
					new LeftArm().MoveToWorld(Location, Map);
					new RightLeg().MoveToWorld(Location, Map);
					new RightArm().MoveToWorld(Location, Map);
					new Head2(dead.RawName).MoveToWorld(Location, Map);
                    new RawHumanFlesh(2, dead).MoveToWorld(Location, Map);
                    new RawHumanBrain(1, dead).MoveToWorld(Location, Map);
				}

				SetFlag(CorpseFlag.Carved, true);

				ProcessDelta();
				SendRemovePacket();
				ItemID = Utility.Random(0xECA, 9); // bone graphic
				Hue = 0;
				ProcessDelta();

				if (IsCriminalAction(from))
				{
					from.CriminalAction(true);
				}
			}
			else if (dead is BaseCreature)
			{
				Conquests.CheckProgress<SkinningConquest>(from as PlayerMobile, this);

				((BaseCreature)dead).OnCarve(from, this, item);
			}
			else
			{
				from.SendLocalizedMessage(500485); // You see nothing useful to carve from the corpse.
			}
		}
	}
}