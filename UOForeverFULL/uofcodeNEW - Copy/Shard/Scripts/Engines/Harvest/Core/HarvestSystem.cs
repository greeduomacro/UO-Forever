#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Server.Commands;
using Server.Engines.Conquests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server.Engines.Harvest
{
	public class HarvestScanGump : Gump
	{
		public class HarvestScanGumpEntry
		{
			public Mobile Mob;
			public int Amount;

			public HarvestScanGumpEntry(Mobile mob, int amount)
			{
				Mob = mob;
				Amount = amount;
			}
		}

		public enum HarvestScanType
		{
			Ore,
			Wood,
			Wool,
			Lockpicking
		}

		private readonly Mobile caller;
		private int currentIndex;

		public static void Initialize()
		{
			CommandSystem.Register("ResourceScan", AccessLevel.Seer, Scan_OnCommand);
		}

		[Usage("Scan")]
		[Description("Makes a call to your custom gump.")]
		public static void Scan_OnCommand(CommandEventArgs e)
		{
			Mobile caller = e.Mobile;

			HarvestScanType type;

			if (e.Arguments.Length == 0)
			{
				caller.SendMessage("Usage: [ResourceScan ore|wood|wool|lockpick");
				return;
			}
			
			string arg = e.Arguments[0].ToLower();
			
			switch (arg)
			{
				case "ore":
					type = HarvestScanType.Ore;
					break;
				case "wood":
					type = HarvestScanType.Wood;
					break;
				case "wool":
					type = HarvestScanType.Wool;
					break;
				case "lockpick":
					type = HarvestScanType.Lockpicking;
					break;
				default:
					caller.SendMessage("Usage: [ResourceScan ore|wood|wool");
					return;
			}

			if (caller.HasGump(typeof(HarvestScanGump)))
			{
				caller.CloseGump(typeof(HarvestScanGump));
			}

			caller.SendGump(new HarvestScanGump(caller, 0, type));
		}

		private readonly HarvestScanType ResourceType = HarvestScanType.Ore;

		public HarvestScanGump(Mobile from, int index, HarvestScanType type)
			: base(0, 0)
		{
			caller = from;
			currentIndex = index;
			ResourceType = type;
			DoGump(currentIndex);
		}

		public void DoGump(int curIndex)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			HarvestScanGumpEntry watchedEntry = GetEntry(curIndex);

			if (watchedEntry == null)
			{
				caller.SendMessage("No players found gathering that resource!");
				return;
			}
			
			caller.Location = watchedEntry.Mob.Location;

			AddPage(0);
			AddBackground(293, 206, 195, 118, 9270);
			AddButton(425, 260, 4502, 4502, (int)Buttons.Next, GumpButtonType.Reply, 0);
			AddImage(363, 277, 2444, 983);
			AddButton(308, 261, 4506, 4506, (int)Buttons.Previous, GumpButtonType.Reply, 0);
			AddLabel(315, 223, 2300, @"Currently watching:");
			AddLabel(319, 242, 2300, watchedEntry.Mob.Name + " " + watchedEntry.Amount);
			AddButton(359, 274, 2444, 248, (int)Buttons.TeleportTo, GumpButtonType.Reply, 0);
			AddLabel(374, 275, 2300, @"Go to");
		}

		public List<HarvestScanGumpEntry> BuildList(Mobile owner)
		{
			var harvesters = new List<Mobile>();
			var amounts = new List<int>();
			
			switch (ResourceType)
			{
				case HarvestScanType.Ore:
					HarvestSystem.GetSortedLists(HarvestSystem.PlayerOreHarvested, harvesters, amounts);
					break;
				case HarvestScanType.Wood:
					HarvestSystem.GetSortedLists(HarvestSystem.PlayerLogsHarvested, harvesters, amounts);
					break;
				case HarvestScanType.Wool:
					HarvestSystem.GetSortedLists(HarvestSystem.PlayerWoolHarvested, harvesters, amounts);
					break;
				case HarvestScanType.Lockpicking:
					HarvestSystem.GetSortedLists(HarvestSystem.PlayerLockpicking, harvesters, amounts);
					break;
			}

			return harvesters.Select((t, i) => new HarvestScanGumpEntry(t, amounts[i])).ToList();
		}

		private HarvestScanGumpEntry GetEntry(int index)
		{
			List<HarvestScanGumpEntry> entries = BuildList(caller);

			if (entries.Count <= 0)
			{
				return null;
			}

			if (index >= entries.Count)
			{
				currentIndex = 0;
				index = 0;
			}

			if (index < 0)
			{
				currentIndex = entries.Count - 1;
				index = entries.Count - 1;
			}

			return entries[index];
		}

		public enum Buttons
		{
			Next = 1,
			Previous = 2,
			TeleportTo = 3,
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			//Mobile from = sender.Mobile;

			switch (info.ButtonID)
			{
				case (int)Buttons.Next:
					{
						if (caller.HasGump(typeof(HarvestScanGump)))
						{
							caller.CloseGump(typeof(HarvestScanGump));
						}
						caller.SendGump(new HarvestScanGump(caller, currentIndex + 1, ResourceType));

						break;
					}
				case (int)Buttons.Previous:
					{
						if (caller.HasGump(typeof(HarvestScanGump)))
						{
							caller.CloseGump(typeof(HarvestScanGump));
						}
						caller.SendGump(new HarvestScanGump(caller, currentIndex - 1, ResourceType));

						break;
					}
				case (int)Buttons.TeleportTo:
					{
						if (caller.HasGump(typeof(HarvestScanGump)))
						{
							caller.CloseGump(typeof(HarvestScanGump));
						}
						caller.SendGump(new HarvestScanGump(caller, currentIndex, ResourceType));
						break;
					}
			}
		}
	}

	public abstract class HarvestSystem
	{
		public static void Initialize()
		{
			CommandSystem.Register("ResourceLog", AccessLevel.GameMaster, ResourceLog_Command);
			CommandSystem.Register("ResourceLogSave", AccessLevel.GameMaster, ResourceLogSave_Command);
		}

		public static void ResourceLogSave_Command(CommandEventArgs e)
		{
			LogHarvesters();
		}

		public static DateTime LastReset = DateTime.UtcNow;

		public static void ResourceLog_Command(CommandEventArgs e)
		{
			try
			{
				Mobile from = e.Mobile;

				var harvesters = new List<Mobile>();
				var amounts = new List<int>();

				// sort all the logged players in ascending order
				from.SendMessage("Top 5 Wool Harvesters since " + LastReset);
				GetSortedLists(PlayerWoolHarvested, harvesters, amounts);
				int numEntries = harvesters.Count > 5 ? 5 : harvesters.Count;
				for (int i = 0; i < numEntries; i++)
				{
					from.SendMessage(harvesters[i].Name + "   " + harvesters[i].Account + "   " + amounts[i]);
				}
				//============ same thing for ore
				from.SendMessage("Top 10 Ore Harvesters since " + LastReset);
				GetSortedLists(PlayerOreHarvested, harvesters, amounts);
				numEntries = harvesters.Count > 10 ? 10 : harvesters.Count;
				for (int i = 0; i < numEntries; i++)
				{
					from.SendMessage(harvesters[i].Name + "   " + harvesters[i].Account + "   " + amounts[i]);
				}
				//============ same thing for logs
				from.SendMessage("Top 10 Log Harvesters since " + LastReset);
				GetSortedLists(PlayerLogsHarvested, harvesters, amounts);
				numEntries = harvesters.Count > 10 ? 10 : harvesters.Count;
				for (int i = 0; i < numEntries; i++)
				{
					from.SendMessage(harvesters[i].Name + "   " + harvesters[i].Account + "   " + amounts[i]);
				}
				//============ same thing for lockpicking
				from.SendMessage("Top 5 Lockpickers since " + LastReset);
				GetSortedLists(PlayerLockpicking, harvesters, amounts);
				numEntries = harvesters.Count > 10 ? 10 : harvesters.Count;
				for (int i = 0; i < numEntries; i++)
				{
					from.SendMessage(harvesters[i].Name + "   " + harvesters[i].Account + "   " + amounts[i]);
				}
			}
			catch (Exception except)
			{
				Console.WriteLine(except.Message);
				Console.WriteLine(except.StackTrace);
			}
		}

		public static void LogHarvesters()
		{
			try
			{
				var sb = new StringBuilder();

				string now = DateTime.Now.ToString(CultureInfo.InvariantCulture);
				var harvesters = new List<Mobile>();
				var amounts = new List<int>();
				// check for ore gathering
				GetSortedLists(PlayerOreHarvested, harvesters, amounts);
				int count = harvesters.Count;
				for (int i = 0; i < count; i++)
				{
					sb.Append(now + "\t" + harvesters[i].Name + "\t" + harvesters[i].Account + "\t" + amounts[i] + "\tOre\n");
				}
				//============ same thing for logs
				GetSortedLists(PlayerLogsHarvested, harvesters, amounts);
				count = harvesters.Count;
				for (int i = 0; i < count; i++)
				{
					sb.Append(now + "\t" + harvesters[i].Name + "\t" + harvesters[i].Account + "\t" + amounts[i] + "\tLogs\n");
				}
				//============ same thing for wool
				GetSortedLists(PlayerWoolHarvested, harvesters, amounts);
				count = harvesters.Count;
				for (int i = 0; i < count; i++)
				{
					sb.Append(now + "\t" + harvesters[i].Name + "\t" + harvesters[i].Account + "\t" + amounts[i] + "\tWool\n");
				}
				//============ same thing for lockpickers
				GetSortedLists(PlayerLockpicking, harvesters, amounts);
				count = harvesters.Count;
				for (int i = 0; i < count; i++)
				{
					sb.Append(now + "\t" + harvesters[i].Name + "\t" + harvesters[i].Account + "\t" + amounts[i] + "\tLockpicking\n");
				}
				LoggingCustom.LogHarvester(sb.ToString());
				LastReset = DateTime.UtcNow;
				PlayerLogsHarvested.Clear();
				PlayerOreHarvested.Clear();
			}
			catch (Exception except)
			{
				Console.WriteLine(except.Message);
				Console.WriteLine(except.StackTrace);
			}
		}

		public static void GetSortedLists(Dictionary<Mobile, int> dict, List<Mobile> harvesters, List<int> amounts)
		{
			// fill harvesters in this function
			harvesters.Clear();
			amounts.Clear();
			foreach (KeyValuePair<Mobile, int> entry in dict)
			{
				if (harvesters.Count == 0)
				{
					harvesters.Add(entry.Key);
					amounts.Add(entry.Value);
					continue;
				}
				int count = harvesters.Count;
				bool foundSpot = false;
				for (int i = 0; i < count; i++)
				{
					if (entry.Value > amounts[i])
					{
						harvesters.Insert(i, entry.Key);
						amounts.Insert(i, entry.Value);
						foundSpot = true;
						break;
					}
				}
				if (!foundSpot)
				{
					harvesters.Add(entry.Key);
					amounts.Add(entry.Value);
				}
			}
		}

		private readonly List<HarvestDefinition> m_Definitions;

		public List<HarvestDefinition> Definitions { get { return m_Definitions; } }

		public HarvestSystem()
		{
			m_Definitions = new List<HarvestDefinition>();
		}

		public virtual bool CheckTool(Mobile from, Item tool)
		{
			bool wornOut = (tool == null || tool.Deleted || (tool is IUsesRemaining && ((IUsesRemaining)tool).UsesRemaining <= 0));

			if (wornOut)
			{
				from.SendLocalizedMessage(1044038); // You have worn out your tool!
			}

			return !wornOut;
		}

		public virtual bool CheckHarvest(Mobile from, Item tool)
		{
			return CheckTool(from, tool);
		}

		public virtual bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{
			return CheckTool(from, tool);
		}

		public virtual bool CheckRange(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
		{
			bool inRange = (from.Map == map && from.InRange(loc, def.MaxRange));

			if (!inRange)
			{
				def.SendMessageTo(from, timed ? def.TimedOutOfRangeMessage : def.OutOfRangeMessage);
			}

			return inRange;
		}

		public virtual bool CheckResources(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
		{
			HarvestBank bank = def.GetBank(map, loc.X, loc.Y);
			bool available = (bank != null && bank.Current >= def.ConsumedPerHarvest);

			if (!available)
			{
				def.SendMessageTo(from, timed ? def.DoubleHarvestMessage : def.NoResourcesMessage);
			}

			return available;
		}

		public virtual void OnBadHarvestTarget(Mobile from, Item tool, object toHarvest)
		{ }

		public virtual object GetLock(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{
			/* Here we prevent multiple harvesting.
			 *
			 * Some options:
			 *  - 'return tool;' : This will allow the player to harvest more than once concurrently, but only if they use multiple tools. This seems to be as OSI.
			 *  - 'return GetType();' : This will disallow multiple harvesting of the same type. That is, we couldn't mine more than once concurrently, but we could be both mining and lumberjacking.
			 *  - 'return typeof( HarvestSystem );' : This will completely restrict concurrent harvesting.
			 */

			return tool;
		}

		public virtual void OnConcurrentHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{ }

		public virtual void OnHarvestStarted(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{ }

		public static Dictionary<Mobile, int> PlayerOreHarvested = new Dictionary<Mobile, int>();
		public static Dictionary<Mobile, int> PlayerLogsHarvested = new Dictionary<Mobile, int>();
		public static Dictionary<Mobile, int> PlayerWoolHarvested = new Dictionary<Mobile, int>();
		public static Dictionary<Mobile, int> PlayerLockpicking = new Dictionary<Mobile, int>();

		public static void LogHarvest(Mobile from, Type type)
		{
			if (from == null)
			{
				return;
			}
			if (typeof(BaseOre).IsAssignableFrom(type))
			{
				if (PlayerOreHarvested.ContainsKey(from))
				{
					PlayerOreHarvested[from] += 1;
				}
				else
				{
					PlayerOreHarvested.Add(from, 1);
				}
			}
			else if (typeof(BaseLog).IsAssignableFrom(type))
			{
				if (PlayerLogsHarvested.ContainsKey(from))
				{
					PlayerLogsHarvested[from] += 1;
				}
				else
				{
					PlayerLogsHarvested.Add(from, 1);
				}
			}
			else if (typeof(Wool).IsAssignableFrom(type))
			{
				if (PlayerWoolHarvested.ContainsKey(from))
				{
					PlayerWoolHarvested[from] += 1;
				}
				else
				{
					PlayerWoolHarvested.Add(from, 1);
				}
			}
			else if (typeof(LockableContainer).IsAssignableFrom(type))
			{
				if (type == typeof(TreasureChestLevel1) || type == typeof(TreasureChestLevel2) ||
					type == typeof(TreasureChestLevel3) || type == typeof(TreasureChestLevel4) || type == typeof(TreasureChestLevel5) ||
					type == typeof(TreasureChestLevel6))
				{
					if (PlayerLockpicking.ContainsKey(from))
					{
						PlayerLockpicking[from] += 1;
					}
					else
					{
						PlayerLockpicking.Add(from, 1);
					}
				}
			}
		}

		public virtual bool BeginHarvesting(Mobile from, Item tool)
		{
			if (!CheckHarvest(from, tool))
			{
				return false;
			}

			from.Target = new HarvestTarget(tool, this);
			return true;
		}

		public virtual void FinishHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked)
		{
			from.EndAction(locked);

			if (!CheckHarvest(from, tool))
			{
				return;
			}

			int tileID;
			Map map;
			Point3D loc;

			if (!GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
			{
				OnBadHarvestTarget(from, tool, toHarvest);
				return;
			}
			
			if (!def.Validate(tileID))
			{
				OnBadHarvestTarget(from, tool, toHarvest);
				return;
			}

			if (!CheckRange(from, tool, def, map, loc, true))
			{
				return;
			}
			
			if (!CheckResources(from, tool, def, map, loc, true))
			{
				return;
			}
			
			if (!CheckHarvest(from, tool, def, toHarvest))
			{
				return;
			}

			if (SpecialHarvest(from, tool, def, map, loc))
			{
				return;
			}

			HarvestBank bank = def.GetBank(map, loc.X, loc.Y);

			if (bank == null)
			{
				return;
			}

			HarvestVein vein = bank.Vein;

			if (vein != null)
			{
				vein = MutateVein(from, tool, def, bank, toHarvest, vein);
			}

			if (vein == null)
			{
				return;
			}

			HarvestResource primary = vein.PrimaryResource;
			HarvestResource fallback = vein.FallbackResource;
			HarvestResource resource = MutateResource(from, tool, def, map, loc, vein, primary, fallback);

			double skillBase = from.Skills[def.Skill].Base;
			//double skillValue = from.Skills[def.Skill].Value;

			Type type = null;

			if (from.Expansion >= resource.ReqExpansion && skillBase >= resource.ReqSkill &&
				from.CheckSkill(def.Skill, resource.MinSkill, resource.MaxSkill))
			{
				type = GetResourceType(from, tool, def, map, loc, resource);

				if (type != null)
				{
					type = MutateType(type, from, tool, def, map, loc, resource);
				}

				if (type != null)
				{
					Item item = Construct(type, from);

					if (item == null)
					{
						type = null;
					}
					else
					{
						//The whole harvest system is kludgy and I'm sure this is just adding to it.
						if (item.Stackable)
						{
							int amount = def.ConsumedPerHarvest;
							int feluccaAmount = def.ConsumedPerFeluccaHarvest;

							var racialAmount = (int)Math.Ceiling(amount * 1.1);
							var feluccaRacialAmount = (int)Math.Ceiling(feluccaAmount * 1.1);

							bool eligableForRacialBonus = (def.RaceBonus && from.Race == Race.Human);
							bool inFelucca = (map == Map.Felucca);

							if (eligableForRacialBonus && inFelucca && bank.Current >= feluccaRacialAmount && 0.1 > Utility.RandomDouble())
							{
								item.Amount = feluccaRacialAmount;
							}
							else if (inFelucca && bank.Current >= feluccaAmount)
							{
								item.Amount = feluccaAmount;
							}
							else if (eligableForRacialBonus && bank.Current >= racialAmount && 0.1 > Utility.RandomDouble())
							{
								item.Amount = racialAmount;
							}
							else
							{
								item.Amount = amount;
							}
						}

						bank.Consume(item.Amount, from);

						if (Give(from, item, def.PlaceAtFeetIfFull))
						{
							SendSuccessTo(from, item, resource);

							Conquests.Conquests.CheckProgress<HarvestConquest>(
								from as PlayerMobile, item, CraftResources.GetFromType(item.GetType()), this);
						}
						else
						{
							SendPackFullTo(from, item, def, resource);
							item.Delete();
						}

						LogHarvest(from, type);

						BonusHarvestResource bonus = def.GetBonusResource(from.Expansion);

						if (bonus != null && bonus.Type != null && skillBase >= bonus.ReqSkill)
						{
							Item bonusItem = Construct(bonus.Type, from);

							if (Give(from, bonusItem, true)) //Bonuses always allow placing at feet, even if pack is full regrdless of def
							{
								bonus.SendSuccessTo(from);

								Conquests.Conquests.CheckProgress<HarvestConquest>(
									from as PlayerMobile, bonusItem, CraftResources.GetFromType(bonus.Type), this);
							}
							else
							{
								item.Delete();
							}
						}

						if (tool is IUsesRemaining)
						{
							var toolWithUses = (IUsesRemaining)tool;

							toolWithUses.ShowUsesRemaining = true;

							if (toolWithUses.UsesRemaining > 0)
							{
								--toolWithUses.UsesRemaining;
							}

							if (toolWithUses.UsesRemaining < 1)
							{
								tool.Delete();
								def.SendMessageTo(from, def.ToolBrokeMessage);
							}
						}
					}
				}
			}

			if (type == null)
			{
				def.SendMessageTo(from, def.FailMessage);
			}

			OnHarvestFinished(from, tool, def, vein, bank, resource, toHarvest);
		}

		public virtual void OnHarvestFinished(
			Mobile from,
			Item tool,
			HarvestDefinition def,
			HarvestVein vein,
			HarvestBank bank,
			HarvestResource resource,
			object harvested)
		{ }

		public virtual bool SpecialHarvest(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
		{
			return false;
		}

		public virtual Item Construct(Type type, Mobile from)
		{
			try
			{
				return Activator.CreateInstance(type) as Item;
			}
			catch
			{
				return null;
			}
		}

		public virtual HarvestVein MutateVein(
			Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein)
		{
			return vein;
		}

		public virtual void SendSuccessTo(Mobile from, Item item, HarvestResource resource)
		{
			resource.SendSuccessTo(from);
		}

		public virtual void SendPackFullTo(Mobile from, Item item, HarvestDefinition def, HarvestResource resource)
		{
			def.SendMessageTo(from, def.PackFullMessage);
		}

		public virtual bool Give(Mobile m, Item item, bool placeAtFeet)
		{
			if (m.PlaceInBackpack(item))
			{
				return true;
			}

			if (!placeAtFeet)
			{
				return false;
			}

			Map map = m.Map;

			if (map == null)
			{
				return false;
			}

			var list = m.GetItemsInRange(0);
			var atFeet = list.OfType<Item>().ToList();

			list.Free();

			if (atFeet.Any(check => check.StackWith(m, item, false)))
			{
				return true;
			}

			item.MoveToWorld(m.Location, map);
			return true;
		}

		public virtual Type MutateType(
			Type type, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
		{
			return from.Region.GetResource(type);
		}

		public virtual Type GetResourceType(
			Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
		{
			if (resource.Types.Length > 0)
			{
				return resource.Types[Utility.Random(resource.Types.Length)];
			}

			return null;
		}

		public virtual HarvestResource MutateResource(
			Mobile from,
			Item tool,
			HarvestDefinition def,
			Map map,
			Point3D loc,
			HarvestVein vein,
			HarvestResource primary,
			HarvestResource fallback)
		{
			bool racialBonus = (def.RaceBonus && from.Race == Race.Elf);

			if (vein.ChanceToFallback > (Utility.RandomDouble() + (racialBonus ? 0.20 : 0)))
			{
				return fallback;
			}

			double skillValue = from.Skills[def.Skill].Value;

			if (fallback != null && (from.Expansion < primary.ReqExpansion || skillValue < primary.ReqSkill || skillValue < primary.MinSkill))
			{
				return fallback;
			}

			return primary;
		}

		public virtual bool OnHarvesting(
			Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked, bool last)
		{
			if (!CheckHarvest(from, tool))
			{
				from.EndAction(locked);
				return false;
			}

			int tileID;
			Map map;
			Point3D loc;

			if (!GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
			{
				from.EndAction(locked);
				OnBadHarvestTarget(from, tool, toHarvest);
				return false;
			}
			
			if (!def.Validate(tileID))
			{
				from.EndAction(locked);
				OnBadHarvestTarget(from, tool, toHarvest);
				return false;
			}
			
			if (!CheckRange(from, tool, def, map, loc, true))
			{
				from.EndAction(locked);
				return false;
			}
			
			if (!CheckResources(from, tool, def, map, loc, true))
			{
				from.EndAction(locked);
				return false;
			}
			
			if (!CheckHarvest(from, tool, def, toHarvest))
			{
				from.EndAction(locked);
				return false;
			}

			DoHarvestingEffect(from, tool, def, map, loc);

			new HarvestSoundTimer(from, tool, this, def, toHarvest, locked, last).Start();

			return !last;
		}

		public virtual void DoHarvestingSound(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{
			if (def.EffectSounds.Length > 0)
			{
				from.PlaySound(Utility.RandomList(def.EffectSounds));
			}
		}

		public virtual void DoHarvestingEffect(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
		{
			from.Direction = from.GetDirectionTo(loc);

			if (!from.Mounted)
			{
				from.Animate(Utility.RandomList(def.EffectActions), 5, 1, true, false, 0);
			}
		}

		public virtual HarvestDefinition GetDefinition(int tileID)
		{
			HarvestDefinition def = null;

			for (int i = 0; def == null && i < m_Definitions.Count; ++i)
			{
				HarvestDefinition check = m_Definitions[i];

				if (check.Validate(tileID))
				{
					def = check;
				}
			}

			return def;
		}

		public virtual void StartHarvesting(Mobile from, Item tool, object toHarvest)
		{
			if (!CheckHarvest(from, tool))
			{
				return;
			}

			int tileID;
			Map map;
			Point3D loc;

			if (!GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
			{
				OnBadHarvestTarget(from, tool, toHarvest);
				return;
			}

			HarvestDefinition def = GetDefinition(tileID);

			if (def == null)
			{
				OnBadHarvestTarget(from, tool, toHarvest);
				return;
			}

			if (!CheckRange(from, tool, def, map, loc, false))
			{
				return;
			}
			
			if (!CheckResources(from, tool, def, map, loc, false))
			{
				return;
			}
			
			if (!CheckHarvest(from, tool, def, toHarvest))
			{
				return;
			}

			object toLock = GetLock(from, tool, def, toHarvest);

			if (!from.BeginAction(toLock))
			{
				OnConcurrentHarvest(from, tool, def, toHarvest);
				return;
			}

			new HarvestTimer(from, tool, this, def, toHarvest, toLock).Start();
			OnHarvestStarted(from, tool, def, toHarvest);
		}

		public virtual bool GetHarvestDetails(
			Mobile from, Item tool, object toHarvest, out int tileID, out Map map, out Point3D loc)
		{
			if (toHarvest is Static && !((Static)toHarvest).Movable)
			{
				var obj = (Static)toHarvest;

				tileID = (obj.ItemID & 0x3FFF) | 0x4000;
				map = obj.Map;
				loc = obj.GetWorldLocation();
			}
			else if (toHarvest is StaticTarget)
			{
				var obj = (StaticTarget)toHarvest;

				tileID = (obj.ItemID & 0x3FFF) | 0x4000;
				map = from.Map;
				loc = obj.Location;
			}
			else if (toHarvest is LandTarget)
			{
				var obj = (LandTarget)toHarvest;

				tileID = obj.TileID;
				map = from.Map;
				loc = obj.Location;
			}
			else
			{
				tileID = 0;
				map = null;
				loc = Point3D.Zero;
				return false;
			}

			return (map != null && map != Map.Internal);
		}
	}
}

namespace Server
{
	public interface IChopable
	{
		void OnChop(Mobile from);
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class FurnitureAttribute : Attribute
	{
		public static bool Check(Item item)
		{
			return (item != null && item.GetType().IsDefined(typeof(FurnitureAttribute), false));
		}
	}
}