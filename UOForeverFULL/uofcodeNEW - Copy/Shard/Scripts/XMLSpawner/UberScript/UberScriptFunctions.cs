#region References
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Server.Accounting;
using Server.Commands;
using Server.Engines.CannedEvil;
using Server.Engines.Harvest;
using Server.Engines.PartySystem;
using Server.Games;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Second;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;

using VitaNex.FX;
using VitaNex.Items;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class UberScriptFunction
	{
		public MethodInfo Method { get; private set; }
		public Type[] ArgTypes { get; private set; }

		public int NumArgs { get { return ArgTypes.Length; } }

		public UberScriptFunction(MethodInfo method)
		{
			Method = method;
			ArgTypes = Method.GetParameters().Select(p => p.ParameterType).ToArray();
		}
	}

	public class UberScriptFunctions
	{
		public static Dictionary<string, Item> RecentlyLookedUpItems = new Dictionary<string, Item>();
		public static Dictionary<string, Mobile> RecentlyLookedUpMobiles = new Dictionary<string, Mobile>();

		public static void ClearRecentLookups()
		{
			RecentlyLookedUpItems.Clear();
			RecentlyLookedUpMobiles.Clear();
		}

		public static Item FindInRecentItemSearchList(string name)
		{
			if (RecentlyLookedUpItems.ContainsKey(name))
			{
				Item item = RecentlyLookedUpItems[name];

				if (item.Deleted)
				{
					RecentlyLookedUpItems.Remove(name);
					return null;
				}

				return item;
			}

			return null;
		}

		public static Mobile FindInRecentMobileSearchList(string name)
		{
			if (RecentlyLookedUpMobiles.ContainsKey(name))
			{
				Mobile mob = RecentlyLookedUpMobiles[name];

				if (mob.Deleted)
				{
					RecentlyLookedUpMobiles.Remove(name);
					return null;
				}

				return mob;
			}

			return null;
		}

		public static Dictionary<string, UberScriptFunction[]> Functions = new Dictionary<string, UberScriptFunction[]>();

		public static bool HasFunction(string function)
		{
			if (Functions.Count == 0)
			{
				InitializeFunctions();
			}

			return Functions.ContainsKey(function);
		}

		public static bool IsFunctionString(string input)
		{
			if (Functions.Count == 0)
			{
				InitializeFunctions();
			}

			if (String.IsNullOrWhiteSpace(input))
			{
				return false;
			}

			if (HasFunction(input))
			{
				return true;
			}

			int parenthPos = input.IndexOf('(');

			return parenthPos > 0 && HasFunction(input.Substring(0, parenthPos).Trim());
		}

		private static Type[] GetTypes(params object[] args)
		{
			return args == null ? new Type[0] : args.Select(o => o == null ? null : o.GetType()).ToArray();
		}

		private static bool MatchTypes(Type[] l, Type[] r)
		{
			return l != null && r != null && l.Length == r.Length && l.Where(
				(tl, i) =>
				{
					var tr = r[i];

					if (tl == null)
					{
						return tr == null;
					}

					if (tl.IsEqual<object>())
					{
						return true;
					}

					if (tl.IsEnum || tl.IsValueType)
					{
						return tr != null && tr.IsEqual(tl);
					}

					if (tl.IsInterface)
					{
						return tr == null || tr.HasInterface(tl);
					}

					return tr == null || tr.IsEqualOrChildOf(tl);
				}).Count() == l.Length;
		}

		/// <summary>
		///     Using a sequential trial and error test approach,
		///     this method will attempt to call the appropriate
		///     UberScript function with the given matching parameters.
		///     An UberScriptException is thrown when any of the tests fail.
		/// </summary>
		public static Object Invoke(string function, object[] parameters)
		{
			if (Functions.Count == 0)
			{
				InitializeFunctions();
			}

			if (String.IsNullOrWhiteSpace(function))
			{
				throw new UberScriptException("Invoke: function can not be null or white-space!");
			}

			if (parameters == null || parameters.Length == 0)
			{
				throw new UberScriptException(String.Format("Invoke: at least one parameter is expected, none supplied."));
			}

			if (!(parameters[0] is TriggerObject))
			{
				throw new UberScriptException(
					String.Format("Invoke: the first parameter expects 'TriggerObject', '{0}' supplied.", parameters[0].GetType()));
			}

			function = function.Trim().ToUpper();

			if (!Functions.ContainsKey(function) || Functions[function].Length == 0)
			{
				throw new UberScriptException(String.Format("Invoke: function '{0}' does not exist!", function));
			}

			var p = GetTypes(parameters);
			var f = Functions[function].FirstOrDefault(m => m.NumArgs == p.Length && MatchTypes(m.ArgTypes, p));

			if (f == null)
			{
				StringBuilder overloads = new StringBuilder();

				overloads.AppendLine("Available functions and overloads:");

				foreach (var ol in Functions[function])
				{
					overloads.AppendLine(" {0}({1})", function, String.Join(", ", ol.ArgTypes.Skip(1)));
				}

				throw new UberScriptException(
					String.Format("Invoke: function '{0}({1})' does not exist!", function, String.Join(", ", p.Skip(1))),
					new UberScriptException(overloads.ToString()));
			}

			if (f.Method == null)
			{
				throw new UberScriptException(
					String.Format("Invoke: function '{0}({1})' is not implemented!", function, String.Join(", ", p.Skip(1))));
			}

			try
			{
				return f.Method.Invoke(null, parameters);
			}
			catch (Exception x)
			{
				throw new UberScriptException(
					String.Format("Invoke: an exception was thrown in function '{0}({1})'", function, String.Join(", ", p.Skip(1))), x);
			}
		}

		public static void Initialize()
		{
			if (Functions.Count == 0)
			{
				InitializeFunctions();
			}
		}

		#region Initialize Functions
		private static void InitializeFunctions()
		{
			// public static methods only
			var methods = typeof(Methods).GetMethods(BindingFlags.Static | BindingFlags.Public);

			var funcs = new ConcurrentDictionary<string, ConcurrentBag<UberScriptFunction>>();

			Parallel.ForEach(
				methods,
				m =>
				{
					var p = m.GetParameters();

					// Always expect first parameter to be a trigger object.
					// Skip methods where names don't match rules: uppercase, optional digit, optional _
					if (p.Length == 0 || !p[0].ParameterType.IsEqual<TriggerObject>() ||
						!m.Name.All(n => Char.IsUpper(n) || Char.IsDigit(n) || n == '_'))
					{
						return;
					}

					var usf = new UberScriptFunction(m);

					funcs.AddOrUpdate(
						m.Name,
						s => new ConcurrentBag<UberScriptFunction>
						{
							usf
						},
						(s, l) =>
						{
							l.Add(usf);
							return l;
						});
				});

			Functions.Clear();

			int count = 0, overloaded = 0;

			funcs.ForEach(
				(k, l) =>
				{
					var v = l.ToArray();

					if (v.Length == 0)
					{
						return;
					}

					++count;
					overloaded += v.Length - 1;

					//Console.WriteLine("[UberScript]: {0}({1})", k, v.Length);

					Functions.Add(k, v);
				});

			Console.WriteLine("[UberScript]: Initialized {0} functions and {1} overloads!", count, overloaded);
		}
		#endregion

		#region Functions
		/// <summary>
		///     UberScript function definitions.
		///     When adding new definitions, three rules must be met:
		///     1. The function must be public and static.
		///     2. The first parameter must be a TriggerObject reference.
		///     3. The name must be in all UPPER CASE, use of _ and 0 to 9 is also allowed.
		/// </summary>
		public static class Methods
		{
			#region Special Effects
			public static SpecialFX SPECIALFX_ENUM(TriggerObject trigObject, string name)
			{
				SpecialFX fx;

				if (!Enum.TryParse(name, out fx))
				{
					fx = SpecialFX.Random;
				}

				return fx;
			}

			public static void SPECIALFX_SEND(TriggerObject trigObject, BaseExplodeEffect effect)
			{
				if (effect != null)
				{
					effect.Send();
				}
			}

			public static BaseSpecialEffect SPECIALFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map)
			{
				return SPECIALFX_ENUM(trigObject, fx).CreateInstance(start, map);
			}

			public static BaseSpecialEffect SPECIALFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map, int range)
			{
				return SPECIALFX_ENUM(trigObject, fx).CreateInstance(start, map, range);
			}

			public static BaseSpecialEffect SPECIALFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map, int range, int repeat)
			{
				return SPECIALFX_ENUM(trigObject, fx).CreateInstance(start, map, range, repeat);
			}

			public static BaseSpecialEffect SPECIALFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map, int range, int repeat, TimeSpan interval)
			{
				return SPECIALFX_ENUM(trigObject, fx).CreateInstance(start, map, range, repeat, interval);
			}
			#endregion

			#region Explode Effects
			public static ExplodeFX EXPLODEFX_ENUM(TriggerObject trigObject, string name)
			{
				ExplodeFX fx;

				if (!Enum.TryParse(name, out fx))
				{
					fx = ExplodeFX.Random;
				}

				return fx;
			}

			public static void EXPLODEFX_SEND(TriggerObject trigObject, BaseExplodeEffect effect)
			{
				if (effect != null)
				{
					effect.Send();
				}
			}

			public static BaseExplodeEffect EXPLODEFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map)
			{
				return EXPLODEFX_ENUM(trigObject, fx).CreateInstance(start, map);
			}

			public static BaseExplodeEffect EXPLODEFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map, int range)
			{
				return EXPLODEFX_ENUM(trigObject, fx).CreateInstance(start, map, range);
			}

			public static BaseExplodeEffect EXPLODEFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map, int range, int repeat)
			{
				return EXPLODEFX_ENUM(trigObject, fx).CreateInstance(start, map, range, repeat);
			}

			public static BaseExplodeEffect EXPLODEFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map, int range, int repeat, TimeSpan interval)
			{
				return EXPLODEFX_ENUM(trigObject, fx).CreateInstance(start, map, range, repeat, interval);
			}
			#endregion

			#region Wave Effects
			public static WaveFX WAVEFX_ENUM(TriggerObject trigObject, string name)
			{
				WaveFX fx;

				if (!Enum.TryParse(name, out fx))
				{
					fx = WaveFX.Random;
				}

				return fx;
			}

			public static void WAVEFX_SEND(TriggerObject trigObject, BaseExplodeEffect effect)
			{
				if (effect != null)
				{
					effect.Send();
				}
			}

			public static BaseWaveEffect WAVEFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map, Direction d)
			{
				return WAVEFX_ENUM(trigObject, fx).CreateInstance(start, map, d);
			}

			public static BaseWaveEffect WAVEFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map, Direction d, int range)
			{
				return WAVEFX_ENUM(trigObject, fx).CreateInstance(start, map, d, range);
			}

			public static BaseWaveEffect WAVEFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map, Direction d, int range, int repeat)
			{
				return WAVEFX_ENUM(trigObject, fx).CreateInstance(start, map, d, range, repeat);
			}

			public static BaseWaveEffect WAVEFX_CREATE(
				TriggerObject trigObject, string fx, IPoint3D start, Map map, Direction d, int range, int repeat, TimeSpan interval)
			{
				return WAVEFX_ENUM(trigObject, fx).CreateInstance(start, map, d, range, repeat, interval);
			}
			#endregion

			public static void UNDRESS(TriggerObject trigObject, Mobile m)
			{
				if (m == null || m.Deleted || m.Backpack == null)
				{
					return;
				}

				foreach (Item item in
					m.Items.Not(
						item =>
						item == null || item.Deleted || !item.Movable || item == m.Backpack || item == m.BankBox || item == m.Mount ||
						item.Layer == Layer.Bank || item.Layer == Layer.Backpack || item.Layer == Layer.Hair ||
						item.Layer == Layer.Face || item.Layer == Layer.FacialHair || item.Layer == Layer.Mount).ToArray())
				{
					m.Backpack.DropItem(item);
				}
			}

			public static void DECAPITATE(TriggerObject trigObject, Mobile from, Mobile target)
			{
				SeveredHead.Decapitate(from, target, f => CREATEHEAD(trigObject, f, target, false));
			}

			public static Item CREATEHEAD(TriggerObject trigObject, Mobile from, Mobile target, bool murderSystem)
			{
				if (from == null || target == null || !target.Body.IsHuman || target.Blessed)
				{
					return null;
				}

				if (target is PlayerMobile && murderSystem)
				{
					return new Head2((PlayerMobile)target, target.Region);
				}

				return new SeveredHead(target);
			}

			public static void DEVOURCORPSES(TriggerObject trigObject, Mobile devourer, int range, bool emote)
			{
				if (devourer == null || devourer.Deleted || devourer.Backpack == null || devourer.Map == null ||
					devourer.Map == Map.Internal)
				{
					return;
				}

				foreach (var corpse in
					devourer.GetEntitiesInRange<Corpse>(devourer.Map, range)
						.Where(
							c => c != null && !c.Deleted && !c.IsDecoContainer && !c.DoesNotDecay && !c.IsBones && c.Owner is PlayerMobile))
				{
					if (emote)
					{
						devourer.Emote("*You see {0} completely devour a corpse and its contents*", devourer.RawName);
					}

					foreach (var item in
						corpse.Items.Where(item => item != null && !item.Deleted && item.Movable && item.Visible).ToArray())
					{
						devourer.Backpack.DropItem(item);
					}

					corpse.TurnToBones();
				}
			}

			public static void ADDQUESTARROW(TriggerObject trigObject, Mobile from, IEntity to)
			{
				ADDQUESTARROW(trigObject, from, to, -1, false);
			}

			public static void ADDQUESTARROW(TriggerObject trigObject, Mobile from, IEntity to, int range)
			{
				ADDQUESTARROW(trigObject, from, to, range, false);
			}

			public static void ADDQUESTARROW(TriggerObject trigObject, Mobile from, IEntity to, int range, bool closable)
			{
				from.QuestArrow = new UberScriptArrow(from, to, range, closable);
			}

			public static void REMOVEQUESTARROW(TriggerObject trigObject, Mobile mob)
			{
				if (mob.QuestArrow != null)
				{
					mob.QuestArrow.Stop();
				}
			}

			public static void EJECTREGIONPLAYERS(TriggerObject trigObject, Region region)
			{
				var mobs = region.GetMobiles();
				ArrayList murdererRectangles = new ArrayList(MurdererEjectionRectangles);
				ArrayList normalRectangles = new ArrayList(EjectionRectangles);
				foreach (Mobile m in mobs)
				{
					if (m.Player)
					{
						if (m.Kills < 5)
						{
							if (!MOVETOSPAWNLOCATION(trigObject, m, Map.Felucca, normalRectangles))
							{
								m.MoveToWorld(new Point3D(2667, 2108, 0), Map.Felucca); // bucs, shouldn't ever get here really
							}
						}
						else
						{
							if (!MOVETOSPAWNLOCATION(trigObject, m, Map.Felucca, murdererRectangles))
							{
								m.MoveToWorld(new Point3D(2667, 2108, 0), Map.Felucca); // bucs, shouldn't ever get here really
							}
						}
					}
					else if (m is BaseCreature)
					{
						BaseCreature bc = (BaseCreature)m;

						if (bc.ControlMaster != null && bc.ControlMaster.Player && bc.ControlMaster.Map == Map.Felucca)
						{
							bc.MoveToWorld(bc.ControlMaster.Location, bc.ControlMaster.Map);
						}
					}
				}

				// now boot out any logged out mobs in the region
				foreach (Mobile m in
					World.Mobiles.Values.Where(
						m =>
						!m.Deleted && m.Player && m.Map == Map.Internal && m.LogoutMap == region.Map && region.Contains(m.LogoutLocation))
					)
				{
					m.LogoutMap = Map.Felucca;

					// brit bank : buc's den
					m.LogoutLocation = m.Kills < 5 ? new Point3D(1438, 1690, 0) : new Point3D(2722, 2184, 0);
				}
			}

			public static void SETSTEALABLE(TriggerObject trigObject, Item item, bool value)
			{
				if (item != null)
				{
					ItemFlags.SetStealable(item, value);
				}
			}

			public static void USE(TriggerObject trigObject, Mobile from, IEntity entity)
			{
				if (from == null || entity == null)
				{
					return;
				}

				// this just calls the uberscript use trigger
				if (XmlScript.HasTrigger(from, TriggerName.onUse) &&
					UberScriptTriggers.Trigger(from, from, TriggerName.onUse, entity as Item) ||
					(XmlScript.HasTrigger(entity, TriggerName.onUse) && UberScriptTriggers.Trigger(entity, from, TriggerName.onUse)))
				{ }
			}

			public static Rectangle2D[] EjectionRectangles = new[]
			{
				new Rectangle2D(new Point2D(2877, 647), new Point2D(2931, 697)), // britain
				new Rectangle2D(new Point2D(2233, 1193), new Point2D(2277, 1235)), // cove
				new Rectangle2D(new Point2D(4433, 1105), new Point2D(4453, 1141)), // moonglow
				new Rectangle2D(new Point2D(542, 2110), new Point2D(603, 2156)) // skara brae
			};

			public static Rectangle2D[] MurdererEjectionRectangles = new[]
			{new Rectangle2D(new Point2D(2702, 2123), new Point2D(2737, 2191))};

			public static void EJECTPLAYERSTOFELUCCA(TriggerObject trigObject)
			{
				ArrayList murdererRectangles = new ArrayList(MurdererEjectionRectangles);
				ArrayList normalRectangles = new ArrayList(EjectionRectangles);

                List<BaseCreature> petsToMove = new List<BaseCreature>();

				foreach (Mobile m in World.Mobiles.Values.Where(m => !m.Deleted))
				{
					if (m.Map == Map.Internal)
					{
						// boot PlayerMobiles logged out in that map (don't need to check for BaseCreatures
						if (m.Player && m.LogoutMap != Map.Felucca)
						{
							m.LogoutMap = Map.Felucca;

							// brit bank : buc's den
							m.LogoutLocation = m.Kills < 5 ? new Point3D(1438, 1690, 0) : new Point3D(2722, 2184, 0);
						}

						continue;
					}

					if (m.Map == Map.Felucca || m.AccessLevel >= AccessLevel.GameMaster || BaseMount.IsInExemptArea(m, m.Map))
					{
						continue;
					}

					if (m.Player)
					{
						if (m.Kills < 5)
						{
							if (!MOVETOSPAWNLOCATION(trigObject, m, Map.Felucca, normalRectangles))
							{
								m.MoveToWorld(new Point3D(2667, 2108, 0), Map.Felucca); // bucs, shouldn't ever get here really
							}
						}
						else
						{
							if (!MOVETOSPAWNLOCATION(trigObject, m, Map.Felucca, murdererRectangles))
							{
								m.MoveToWorld(new Point3D(2667, 2108, 0), Map.Felucca); // bucs, shouldn't ever get here really
							}
						}
					}
                    else if (m is BaseCreature) // send pets too
                    {
                        BaseCreature bc = (BaseCreature)m;

                        if (bc.ControlMaster != null && bc.ControlMaster.Player)
                        {
                            petsToMove.Add(bc);
                        }
                    }
				}

                foreach (BaseCreature pet in petsToMove)
                {
                    if (pet.ControlMaster.Map == Map.Felucca)
                    {
                        pet.MoveToWorld(pet.ControlMaster.Location, pet.ControlMaster.Map);
                    }
                    else if (pet.ControlMaster.Map == Map.Internal)
                    {
                        pet.Stable();
                    }
                }
			}

			public static void EJECTPLAYERSFROMMAP(TriggerObject trigObject, Map map)
			{
				if (map == null || map == Map.Felucca || map == Map.Internal)
				{
					return; // can't eject from felucca or internal
				}

				ArrayList murdererRectangles = new ArrayList(MurdererEjectionRectangles);
				ArrayList normalRectangles = new ArrayList(EjectionRectangles);

                List<BaseCreature> petsToMove = new List<BaseCreature>();

				foreach (Mobile m in World.Mobiles.Values)
				{
					if (m == null)
					{
						continue;
					}

					// boot PlayerMobiles logged out in that map (don't need to check for BaseCreatures
					if (m.Map == Map.Internal)
					{
						if (m.Player && m.LogoutMap == map)
						{
							m.LogoutMap = Map.Felucca;

							// brit bank : buc's den
							m.LogoutLocation = m.Kills < 5 ? new Point3D(1438, 1690, 0) : new Point3D(2722, 2184, 0);
						}

						continue;
					}

					if (m.Map != map || BaseMount.IsInExemptArea(m, m.Map))
					{
						continue;
					}

					if (m.Player)
					{
						if (m.AccessLevel < AccessLevel.GameMaster)
						{
							if (m.Kills < 5)
							{
								if (!MOVETOSPAWNLOCATION(trigObject, m, Map.Felucca, normalRectangles))
								{
									m.MoveToWorld(new Point3D(2667, 2108, 0), Map.Felucca); // bucs, shouldn't ever get here really
								}
							}
							else
							{
								if (!MOVETOSPAWNLOCATION(trigObject, m, Map.Felucca, murdererRectangles))
								{
									m.MoveToWorld(new Point3D(2667, 2108, 0), Map.Felucca); // bucs, shouldn't ever get here really
								}
							}
						}
					}
					else if (m is BaseCreature) // send pets too
					{
						BaseCreature bc = (BaseCreature)m;

						if (bc.ControlMaster != null && bc.ControlMaster.Player)
						{
                            petsToMove.Add(bc);
						}
					}
				}

                foreach (BaseCreature pet in petsToMove)
                {
                    if (pet.ControlMaster.Map == Map.Felucca)
                    {
                        pet.MoveToWorld(pet.ControlMaster.Location, pet.ControlMaster.Map);
                    }
                    else if (pet.ControlMaster.Map == Map.Internal)
                    {
                        pet.Stable();
                    }
                }
			}

			public static void SENDTARGET(TriggerObject trigObject, Mobile mob, bool allowGround, bool checkLOS)
			{
				if (mob != null)
				{
					mob.Target = new UberScriptTarget(trigObject.Script, trigObject.This, allowGround, checkLOS);
				}
			}

			public static bool INBOAT(TriggerObject trigObject, BaseBoat boat, IEntity testEntity)
			{
				return boat != null && boat.GetMovingEntities().Any(entity => entity == testEntity);
			}

			public static void DRYDOCKBOAT(TriggerObject trigObject, BaseBoat boat, Mobile mobToGetBoat)
			{
				if (boat != null && mobToGetBoat != null)
				{
					boat.ForceDryDock(mobToGetBoat);
				}
			}

			public static void SENDMSGBOATSINKERS(TriggerObject trigObject, BaseBoat boat, string message)
			{
				if (boat == null || message == null)
				{
					return;
				}

				foreach (BoatDamageEntry entry in boat.DamageEntries)
				{
					SENDMSG(trigObject, entry.Mob, message, 0x38);
				}
			}

			public static void BOATREMOVECANNONS(TriggerObject trigObject, BaseBoat boat)
			{
				if (boat == null)
				{
					return;
				}

				foreach (SmallShipCannon cannon in boat.BoatComponents.OfType<SmallShipCannon>().ToArray())
				{
					boat.BoatComponents.Remove(cannon);
					cannon.Delete();
				}
			}

			public static int BOATRANSOMCOST(TriggerObject trigObject, BaseBoat boat)
			{
				if (boat == null)
				{
					return 0;
				}

				int cost = 100000; // just in case it doesn't fall under these

				if (boat is SmallBoat || boat is SmallDragonBoat)
				{
					cost = BoatSystemController._SmallBoatCost;
				}
				else if (boat is MediumBoat || boat is MediumDragonBoat)
				{
					cost = BoatSystemController._MediumBoatCost;
				}
				else if (boat is LargeBoat || boat is LargeDragonBoat)
				{
					cost = BoatSystemController._LargeBoatCost;
				}
				else if (boat is GargoyleBoat)
				{
					cost = BoatSystemController._GargoyleBoatCost;
				}
				else if (boat is TokunoBoat)
				{
					cost = BoatSystemController._TokunoBoatCost;
				}
				else if (boat is OrcBoat)
				{
					cost = BoatSystemController._OrcBoatCost;
				}

				return (int)Math.Round(cost * BoatSystemController._BoatRansomCostFraction);
			}

			public static bool BOATRANSOMPAYOUT(TriggerObject trigObject, BaseBoat boat)
			{
				if (boat == null || boat.Owner == null)
				{
					return false;
				}

				int payout = BOATRANSOMCOST(trigObject, boat);

				if (TAKEGOLDFROM(trigObject, boat.Owner, payout) == false) // they couldn't afford it
				{
					return false;
				}

				boat.Owner.SendMessage("You have paid out " + payout + " gold in ransom for you boat.");

				string toLog = DateTime.UtcNow + " " + boat.Owner + " paying a ransom of " + payout + "\n";

				payout = (int)Math.Round(payout * (1.0 - BoatSystemController._BoatRansomGoldSinkFraction));
				toLog += payout + " gold will be distributed to those who sunk the boat.\n";

				// sum up the total damage in order to calculate how much each person gets
				int totalDamage = boat.DamageEntries.Sum(entry => entry.Damage);

				foreach (BoatDamageEntry entry in boat.DamageEntries)
				{
					double fraction = ((double)entry.Damage) / totalDamage;
					int awardAmount = (int)(payout * fraction);
					Gold award = new Gold(awardAmount);

					if (entry.Mob.BankBox != null)
					{
						entry.Mob.BankBox.DropItem(award);
					}

					if (boat.Owner != null)
					{
						entry.Mob.SendMessage(
							"You have been awarded " + awardAmount + " gold from " + boat.Owner.RawName + "'s boat ransom!");
					}
					else
					{
						entry.Mob.SendMessage("You have been awarded " + awardAmount + " gold from a boat ransom!");
					}

					toLog += entry.Mob + " was awarded " + awardAmount + " for dealing " + entry.Damage + "\n";
				}

				LoggingCustom.Log("BoatRansom.txt", toLog);
				return true;
			}

			public static void ADDBOATDAMAGEENTRY(TriggerObject trigObject, BaseBoat boat, Mobile mob, int damage)
			{
				if (boat != null && mob != null)
				{
					// default expiration of 15 minutes
					boat.AddDamageEntry(mob, damage, DateTime.UtcNow + TimeSpan.FromMinutes(30.0));
				}
			}

			public static void ADDBOATDAMAGEENTRY(
				TriggerObject trigObject, BaseBoat boat, Mobile mob, int damage, DateTime expiration)
			{
				if (boat != null && mob != null)
				{
					boat.AddDamageEntry(mob, damage, expiration);
				}
			}

			public static Point3D GETVALIDSPAWNLOCATIONANDMAP(TriggerObject trigObject, Map map, ArrayList rectangles)
			{
				if (map == null || map == Map.Internal || rectangles == null || rectangles.Count == 0)
				{
					return Point3D.Zero;
				}

				//Rectangle2D randomRectangle = (Rectangle2D)rectangles[Utility.Random(rectangles.Count)];
				//return GETVALIDSPAWNLOCATIONANDMAP(trigObject, randomRectangle.Start.X, randomRectangle.Start.Y, randomRectangle.End.X, randomRectangle.End.Y, 0, true);
				ArrayList weightedProbabilities = new ArrayList(rectangles.Count);

				foreach (Rectangle2D r in rectangles)
				{
					weightedProbabilities.Add(r.Width * r.Height);
				}

				return GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles, weightedProbabilities, 0, true);
			}

			public static Point3D GETVALIDSPAWNLOCATIONANDMAP(
				TriggerObject trigObject, Map map, ArrayList rectangles, ArrayList weightedProbabilities)
			{
				return map != null && map != Map.Internal && rectangles != null && rectangles.Count != 0 &&
					   weightedProbabilities != null && weightedProbabilities.Count != 0
						   ? GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles, weightedProbabilities, 0, true)
						   : Point3D.Zero;
			}

			public static Point3D GETVALIDSPAWNLOCATIONANDMAP(
				TriggerObject trigObject,
				Map map,
				ArrayList rectangles,
				ArrayList weightedProbabilities,
				ArrayList zLevels,
				bool requiresSurface)
			{
				if (map == null || map == Map.Internal || rectangles == null || rectangles.Count == 0 ||
					weightedProbabilities == null || weightedProbabilities.Count == 0 || zLevels == null || zLevels.Count == 0)
				{
					return Point3D.Zero;
				}

				// expectes rectangles and weighted probabilities to be the same length
				if (rectangles.Count != weightedProbabilities.Count || rectangles.Count != zLevels.Count)
				{
					throw new UberScriptException(
						"GETVALIDSPAWNLOCATIONANDMAP expects rectangles and weightedProbabilities to have the same length!");
				}

				int totalWeights = weightedProbabilities.Cast<int>().Sum();
				int roll = Utility.Random(totalWeights);
				int current = 0;

				for (int i = 0; i < rectangles.Count; i++)
				{
					current += (int)weightedProbabilities[i];

					if (roll >= current)
					{
						continue;
					}

					if (rectangles[i] is Rectangle2D)
					{
						Rectangle2D randomRectangle = (Rectangle2D)rectangles[i];

						return GETVALIDSPAWNLOCATIONANDMAP(
							trigObject,
							map,
							randomRectangle.Start.X,
							randomRectangle.Start.Y,
							randomRectangle.End.X,
							randomRectangle.End.Y,
							(int)zLevels[i],
							requiresSurface);
					}

					throw new UberScriptException(
						"GETVALIDSPAWNLOCATIONANDMAP expected Rectangle2D objects in rectangles ArrayList but found " + rectangles[i]);
				}

				throw new UberScriptException("GETVALIDSPAWNLOCATIONANDMAPS with rectangles failed to return a valid rectangle!");
			}

			public static Point3D GETVALIDSPAWNLOCATIONANDMAP(
				TriggerObject trigObject,
				Map map,
				ArrayList rectangles,
				ArrayList weightedProbabilities,
				int zLevel,
				bool requiresSurface)
			{
				if (map == null || map == Map.Internal || rectangles == null || rectangles.Count == 0 ||
					weightedProbabilities == null || weightedProbabilities.Count == 0)
				{
					return Point3D.Zero;
				}

				// expectes rectangles and weighted probabilities to be the same length
				if (rectangles.Count != weightedProbabilities.Count)
				{
					throw new UberScriptException(
						"GETVALIDSPAWNLOCATIONANDMAP expects rectangles and weightedProbabilities to have the same length!");
				}

				int totalWeights = weightedProbabilities.Cast<int>().Sum();
				int roll = Utility.Random(totalWeights);
				int current = 0;

				for (int i = 0; i < rectangles.Count; i++)
				{
					current += (int)weightedProbabilities[i];

					if (roll >= current)
					{
						continue;
					}

					if (rectangles[i] is Rectangle2D)
					{
						Rectangle2D randomRectangle = (Rectangle2D)rectangles[i];

						return GETVALIDSPAWNLOCATIONANDMAP(
							trigObject,
							map,
							randomRectangle.Start.X,
							randomRectangle.Start.Y,
							randomRectangle.End.X,
							randomRectangle.End.Y,
							zLevel,
							requiresSurface);
					}

					throw new UberScriptException(
						"GETVALIDSPAWNLOCATIONANDMAP expected Rectangle2D objects in rectangles ArrayList but found " + rectangles[i]);
				}

				throw new UberScriptException("GETVALIDSPAWNLOCATIONANDMAPS with rectangles failed to return a valid rectangle!");
			}

			public static Point3D GETVALIDSPAWNLOCATIONANDMAP(TriggerObject trigObject, Map map, IPoint3D target, int range)
			{
				return map != null && map != Map.Internal && target != null
						   ? GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, target, range, false)
						   : Point3D.Zero;
			}

			public static Point3D GETVALIDSPAWNLOCATIONANDMAP(
				TriggerObject trigObject, Map map, IPoint3D target, int range, bool requiresurface)
			{
				return map != null && map != Map.Internal && target != null
						   ? GETVALIDSPAWNLOCATIONANDMAP(
							   trigObject,
							   map,
							   target.X - range,
							   target.Y - range,
							   target.X + range,
							   target.Y + range,
							   target.Z,
							   requiresurface)
						   : Point3D.Zero;
			}

			public static Point3D GETVALIDSPAWNLOCATIONANDMAP(
				TriggerObject trigObject, Map map, int startX, int startY, int endX, int endY, int z, bool requiresurface)
			{
				if (map == null || map == Map.Internal)
				{
					return Point3D.Zero;
				}

				// --- from XmlSpawner2.cs GetSpawnPosition function ---
				// try to find a valid spawn location using the z coord of the spawner
				// relax the normal surface requirement for mobiles if the flag is set

				// try 10 times; this is a potential performance bottleneck
				for (int i = 0; i < 10; i++)
				{
					int x = Utility.RandomMinMax(startX, endX);
					int y = Utility.RandomMinMax(startY, endY);

					bool fit = requiresurface
								   ? CANFITMOB(trigObject, x, y, z, 16, false, true, true, null, map)
								   : CANFIT(trigObject, new Point2D(x, y));

					// if that fails then try to find a valid z coord
					if (fit)
					{
						return new Point3D(x, y, z);
					}

					z = map.GetAverageZ(x, y);

					fit = requiresurface
							  ? CANFITMOB(trigObject, x, y, z, 16, false, true, true, null, map)
							  : map.CanFit(x, y, z, 16, true, false, false);

					if (fit)
					{
						return new Point3D(x, y, z);
					}

					// check for a possible static surface that works
					var staticTiles = map.Tiles.GetStaticTiles(x, y, true);

					foreach (StaticTile tile in staticTiles)
					{
						ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

						//int calcTop = (tile.Z + id.CalcHeight);

						if ((id.Flags & TileFlag.Surface) == 0)
						{
							continue;
						}

						int top = tile.Z + id.Height;

						fit = requiresurface
								  ? CANFITMOB(trigObject, x, y, top, 16, false, true, true, null, map)
								  : map.CanFit(x, y, top, 16, true, false, false);

						if (fit)
						{
							return new Point3D(x, y, top);
						}
					}
				}

				// unable to find a valid spot in 10 tries
				return Point3D.Zero;
			}

			public static void OVERHEADBCAST(TriggerObject trigObject, object message)
			{
				if (message != null)
				{
					OVERHEADBCAST(trigObject, message, 0x482);
				}
			}

			public static void OVERHEADBCAST(TriggerObject trigObject, object message, int hue)
			{
				foreach (NetState state in NetState.Instances.Where(state => state.Mobile != null))
				{
					state.Mobile.LocalOverheadMessage(MessageType.Regular, hue, false, message == null ? "null" : message.ToString());
				}
			}

			public static void CONSUME(TriggerObject trigObject, Item item)
			{
				if (item != null)
				{
					item.Consume();
				}
			}

			public static void CONSUME(TriggerObject trigObject, Item item, int amount)
			{
				if (item != null && amount > 0)
				{
					item.Consume(amount);
				}
			}

			public static bool REGIONCONTAINS(TriggerObject trigObject, Region region, IPoint3D location)
			{
				return region != null && location != null && region.Contains(new Point3D(location));
			}

			public static Region GETREGIONBYNAME(TriggerObject trigObject, string name)
			{
				return name != null ? Region.Regions.FirstOrDefault(region => region.Name == name) : null;
			}

			public static Region GETREGION(TriggerObject trigObject, IPoint3D loc, object type_OR_typeString)
			{
				return loc != null && type_OR_typeString != null ? GETREGION(trigObject, loc, type_OR_typeString, false) : null;
			}

			public static Region GETREGION(TriggerObject trigObject, IPoint3D loc, object type_OR_typeString, bool strictTyping)
			{
				if (loc == null || type_OR_typeString == null)
				{
					return null;
				}

				Type regionType = null;

				if (type_OR_typeString is string)
				{
					regionType = ScriptCompiler.FindTypeByName((string)type_OR_typeString);
				}
				else if (type_OR_typeString is Type)
				{
					regionType = (Type)type_OR_typeString;
				}

				if (regionType == null)
				{
					return null;
				}

				Region r = GETREGION(trigObject, loc);

				do
				{
					if (!strictTyping)
					{
						if (regionType.IsInstanceOfType(r))
						{
							return r;
						}
					}
					else
					{
						if (r.GetType() == regionType)
						{
							return r;
						}
					}

					r = r.Parent;
				}
				while (r != null);

				return null;
			}

			public static Region GETREGION(TriggerObject trigObject, IPoint3D loc)
			{
				if (loc == null)
				{
					return null;
				}

				if (loc is Mobile)
				{
					return ((Mobile)loc).Region;
				}

				Map map = loc is IEntity ? ((IEntity)loc).Map : Map.Felucca;

				return Region.Find(new Point3D(loc), map);
			}

			public static string STRINGREPLACE(TriggerObject trigObject, string original, string toReplace, string replacement)
			{
				if (original == null)
				{
					original = String.Empty;
				}

				if (toReplace == null)
				{
					toReplace = String.Empty;
				}

				if (replacement == null)
				{
					replacement = String.Empty;
				}

				return original.Replace(toReplace, replacement);
			}

			public static void AGGRESSIVEACTION(TriggerObject trigObject, Mobile from, Mobile to)
			{
				if (from != null && to != null)
				{
					AGGRESSIVEACTION(trigObject, from, to, from.IsHarmfulCriminal(to));
				}
			}

			public static void AGGRESSIVEACTION(TriggerObject trigObject, Mobile from, Mobile to, bool isCriminal)
			{
				if (from != null && to != null)
				{
					to.AggressiveAction(from, isCriminal);
				}
			}

			public static bool ISHARMFULCRIMINAL(TriggerObject trigObject, Mobile from, Mobile to)
			{
				return from != null && to != null && from.IsHarmfulCriminal(to);
			}

			public static void CRIMINALACTION(TriggerObject trigObject, Mobile mob)
			{
				if (mob != null)
				{
					mob.CriminalAction(true);
				}
			}

			public static ArrayList BOATMOBILES(TriggerObject trigObject, BaseBoat boat)
			{
				return boat != null ? BOATMOBILES(trigObject, boat, false) : new ArrayList();
			}

			public static ArrayList BOATMOBILES(TriggerObject trigObject, BaseBoat boat, bool deadOnly)
			{
				return boat != null
						   ? new ArrayList(boat.GetMovingEntities().OfType<Mobile>().Where(m => !deadOnly || !m.Alive).ToArray())
						   : new ArrayList();
			}

			public static void STRANDED(TriggerObject trigObject, Mobile mob)
			{
				if (mob != null)
				{
					Strandedness.MoveStrandedMobile(mob);
				}
			}

			public static void HEFFECT(TriggerObject trigObject, int hue, int effect, int duration, IPoint3D p)
			{
				HEFFECT(trigObject, hue, effect, duration, p.X, p.Y, p.Z, 0);
			}

			public static void HEFFECT(TriggerObject trigObject, int hue, int effect, int duration, IPoint3D p, int range)
			{
				HEFFECT(trigObject, hue, effect, duration, p.X, p.Y, p.Z, range);
			}

			public static void HEFFECT(TriggerObject trigObject, int hue, int effect, int duration, int x, int y, int z)
			{
				HEFFECT(trigObject, hue, effect, duration, x, y, z, 0);
			}

			// TODO: Overload with a Map argument
			public static void HEFFECT(
				TriggerObject trigObject, int hue, int effect, int duration, int x, int y, int z, int range)
			{
				// some interesting effects are explosion(14013,15), sparkle(14155,15), explosion2(14000,13)
				Map map = Map.Felucca;

				if (trigObject.This != null)
				{
					if (trigObject.This is Mobile)
					{
						map = ((Mobile)trigObject.This).Map;
					}
					else if (trigObject.This is Item)
					{
						map = ((Item)trigObject.This).Map;
					}
				}

				if (effect <= 0)
				{
					return;
				}

				for (int xOffset = -range; xOffset <= range; xOffset++)
				{
					for (int yOffset = -range; yOffset <= range; yOffset++)
					{
						if (Math.Sqrt(xOffset * xOffset + yOffset * yOffset) <= range)
						{
							Effects.SendLocationEffect(new Point3D(x + xOffset, y + yOffset, z), map, effect, duration, hue, 0);
						}
					}
				}
			}

			public static void HMEFFECT(
				TriggerObject trigObject, int hue, int effect, int speed, int x, int y, int z, IPoint3D end)
			{
				if (end != null)
				{
					HMEFFECT(trigObject, hue, effect, speed, x, y, z, end.X, end.Y, end.Z, false, false);
				}
			}

			public static void HMEFFECT(
				TriggerObject trigObject, int hue, int effect, int speed, int x, int y, int z, IPoint3D end, bool fixedDirection)
			{
				if (end != null)
				{
					HMEFFECT(trigObject, hue, effect, speed, x, y, z, end.X, end.Y, end.Z, fixedDirection, false);
				}
			}

			public static void HMEFFECT(
				TriggerObject trigObject,
				int hue,
				int effect,
				int speed,
				int x,
				int y,
				int z,
				IPoint3D end,
				bool fixedDirection,
				bool explosion)
			{
				if (end != null)
				{
					HMEFFECT(trigObject, hue, effect, speed, x, y, z, end.X, end.Y, end.Z, fixedDirection, explosion);
				}
			}

			public static void HMEFFECT(
				TriggerObject trigObject, int hue, int effect, int speed, IPoint3D start, int x2, int y2, int z2)
			{
				if (start != null)
				{
					HMEFFECT(trigObject, hue, effect, speed, start.X, start.Y, start.Z, x2, y2, z2, false, false);
				}
			}

			public static void HMEFFECT(
				TriggerObject trigObject,
				int hue,
				int effect,
				int speed,
				IPoint3D start,
				int x2,
				int y2,
				int z2,
				bool fixedDirection)
			{
				if (start != null)
				{
					HMEFFECT(trigObject, hue, effect, speed, start.X, start.Y, start.Z, x2, y2, z2, fixedDirection, false);
				}
			}

			public static void HMEFFECT(
				TriggerObject trigObject,
				int hue,
				int effect,
				int speed,
				IPoint3D start,
				int x2,
				int y2,
				int z2,
				bool fixedDirection,
				bool explosion)
			{
				if (start != null)
				{
					HMEFFECT(trigObject, hue, effect, speed, start.X, start.Y, start.Z, x2, y2, z2, fixedDirection, explosion);
				}
			}

			public static void HMEFFECT(TriggerObject trigObject, int hue, int effect, int speed, IPoint3D start, IPoint3D end)
			{
				if (start != null && end != null)
				{
					HMEFFECT(trigObject, hue, effect, speed, start.X, start.Y, start.Z, end.X, end.Y, end.Z, false, false);
				}
			}

			public static void HMEFFECT(
				TriggerObject trigObject, int hue, int effect, int speed, IPoint3D start, IPoint3D end, bool fixedDirection)
			{
				if (start != null && end != null)
				{
					HMEFFECT(trigObject, hue, effect, speed, start.X, start.Y, start.Z, end.X, end.Y, end.Z, fixedDirection, false);
				}
			}

			public static void HMEFFECT(
				TriggerObject trigObject,
				int hue,
				int effect,
				int speed,
				IPoint3D start,
				IPoint3D end,
				bool fixedDirection,
				bool explosion)
			{
				if (start != null && end != null)
				{
					HMEFFECT(trigObject, hue, effect, speed, start.X, start.Y, start.Z, end.X, end.Y, end.Z, fixedDirection, explosion);
				}
			}

			public static void HMEFFECT(
				TriggerObject trigObject, int hue, int effect, int speed, int x, int y, int z, int x2, int y2, int z2)
			{
				HMEFFECT(trigObject, hue, effect, speed, x, y, z, x2, y2, z2, false, false);
			}

			public static void HMEFFECT(
				TriggerObject trigObject,
				int hue,
				int effect,
				int speed,
				int x,
				int y,
				int z,
				int x2,
				int y2,
				int z2,
				bool fixedDirection)
			{
				HMEFFECT(trigObject, hue, effect, speed, x, y, z, x2, y2, z2, fixedDirection, false);
			}

			// TODO: Overload with a Map argument 
			public static void HMEFFECT(
				TriggerObject trigObject,
				int hue,
				int effect,
				int speed,
				int x,
				int y,
				int z,
				int x2,
				int y2,
				int z2,
				bool fixedDirection,
				bool explosion)
			{
				//syntax is HMEFFECT,itemid,speed,x,y,z,x2,y2,z2
				const int duration = 10;
				Point3D eloc1 = new Point3D(x, y, z + 10);
				// offset by 10 because it always looks so bad going out of anythign--it drags along the floor
				Point3D eloc2 = new Point3D(x2, y2, z2 + 10);
				Map map = Map.Felucca;

				if (trigObject.This != null)
				{
					if (trigObject.This is Mobile)
					{
						map = ((Mobile)trigObject.This).Map;
					}

					if (trigObject.This is Item)
					{
						map = ((Item)trigObject.This).Map;
					}
				}

				if (effect >= 0 && map != Map.Internal)
				{
					// TODO: might want to implement those last booleans!
					Effects.SendPacket(
						eloc1,
						map,
						new HuedEffect(
							EffectType.Moving, -1, -1, effect, eloc1, eloc2, speed, duration, fixedDirection, explosion, hue, 0));
				}
			}

			public static void ADDTOLISTAT(TriggerObject trigObject, ArrayList list, object obj, int index)
			{
				if (list != null)
				{
					list.Insert(index, obj);
				}
			}

			public static void SENDITEMPACKET(TriggerObject trigObject, Item item, Mobile mob)
			{
				if (item != null && mob != null && mob.NetState != null)
				{
					item.SendInfoTo(mob.NetState);
				}
			}

			public static void OPENCONTAINERTO(TriggerObject trigObject, Container container, Mobile mob)
			{
				if (container != null && mob != null && mob.NetState != null)
				{
					container.DisplayTo(mob);
				}
			}

			public static bool ISPARENT(TriggerObject trigObject, object toTest, Item item)
			{
				if (toTest == null || item == null)
				{
					return false;
				}

				object parent = item.Parent;

				while (parent != null)
				{
					if (parent == toTest)
					{
						return true;
					}

					if (parent is Item)
					{
						parent = ((Item)parent).Parent;
					}
					else
					{
						break;
					}
				}

				return false;
			}

			public static string TRIM(TriggerObject trigObject, string input)
			{
				return input != null ? input.Trim() : String.Empty;
			}

			public static string EMPTYSTRING(TriggerObject trigObject)
			{
				return String.Empty;
			}

			public static bool INHOUSEORBOAT(TriggerObject trigObject, IPoint3D location, Map map)
			{
				return location != null && map != null && map != Map.Internal &&
					   CreaturePossession.IsInHouseOrBoat(new Point3D(location), map);
			}

			public static bool INHOUSEORBOAT(TriggerObject trigObject, IEntity entity)
			{
				return entity != null && CreaturePossession.IsInHouseOrBoat(entity.Location, entity.Map);
			}

			public static bool INLOS(TriggerObject trigObject, IEntity entity1, IEntity entity2)
			{
				return entity1 != null && entity2 != null && entity1.Map == entity2.Map && entity1.Map.LineOfSight(entity1, entity2);
			}

			public static int GETSPAWNRECTANGLEINDEX(TriggerObject trigObject, IPoint3D location, ArrayList rectangles)
			{
				if (location == null || rectangles == null || rectangles.Count == 0)
				{
					return -1;
				}

				for (int i = 0; i < rectangles.Count; i++)
				{
					if (((Rectangle2D)rectangles[i]).Contains(location))
					{
						return i;
					}
				}

				return -1;
			}

			public static bool MOVETOSPAWNLOCATION(TriggerObject trigObject, IEntity mob, Map map, ArrayList rectangles)
			{
				if (mob == null || map == null || map == Map.Internal || rectangles == null || rectangles.Count == 0)
				{
					return false;
				}

				// try up to 4 times
				Point3D spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles);

				if (spawnLoc == Point3D.Zero)
				{
					spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles);

					if (spawnLoc == Point3D.Zero)
					{
						LOG(trigObject, "MOVETOSPAWNLOCATION", trigObject.Script.ScriptFile + " failing to find spawn loc");

						spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles);

						if (spawnLoc == Point3D.Zero)
						{
							spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles);

							if (spawnLoc == Point3D.Zero)
							{
								return false;
							}
						}
					}
				}

				mob.MoveToWorld(spawnLoc, map);

				if (mob is BaseCreature)
				{
					((BaseCreature)mob).Home = spawnLoc;
				}

				return true;
			}

			public static bool MOVETOSPAWNLOCATION(
				TriggerObject trigObject, IEntity mob, Map map, ArrayList rectangles, ArrayList weightedProbabilities)
			{
				if (mob == null || map == null || map == Map.Internal || rectangles == null || rectangles.Count == 0 ||
					weightedProbabilities == null || weightedProbabilities.Count == 0)
				{
					return false;
				}

				// try up to 4 times
				Point3D spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles, weightedProbabilities);

				if (spawnLoc == Point3D.Zero)
				{
					spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles, weightedProbabilities);

					if (spawnLoc == Point3D.Zero)
					{
						LOG(trigObject, "MOVETOSPAWNLOCATION", trigObject.Script.ScriptFile + " failing to find spawn loc");

						spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles, weightedProbabilities);

						if (spawnLoc == Point3D.Zero)
						{
							spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles, weightedProbabilities);

							if (spawnLoc == Point3D.Zero)
							{
								return false;
							}
						}
					}
				}

				mob.MoveToWorld(spawnLoc, map);

				if (mob is BaseCreature)
				{
					((BaseCreature)mob).Home = spawnLoc;
				}

				return true;
			}

			public static bool MOVETOSPAWNLOCATION(
				TriggerObject trigObject,
				IEntity mob,
				Map map,
				ArrayList rectangles,
				ArrayList weightedProbabilities,
				ArrayList zLevels,
				bool requiresSurface)
			{
				if (mob == null || map == null || map == Map.Internal || rectangles == null || rectangles.Count == 0 ||
					weightedProbabilities == null || weightedProbabilities.Count == 0 || zLevels == null || zLevels.Count == 0)
				{
					return false;
				}

				// try up to 4 times
				Point3D spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(
					trigObject, map, rectangles, weightedProbabilities, zLevels, requiresSurface);

				if (spawnLoc == Point3D.Zero)
				{
					spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(
						trigObject, map, rectangles, weightedProbabilities, zLevels, requiresSurface);

					if (spawnLoc == Point3D.Zero)
					{
						LOG(trigObject, "MOVETOSPAWNLOCATION", trigObject.Script.ScriptFile + " failing to find spawn loc");

						spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(
							trigObject, map, rectangles, weightedProbabilities, zLevels, requiresSurface);

						if (spawnLoc == Point3D.Zero)
						{
							spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(
								trigObject, map, rectangles, weightedProbabilities, zLevels, requiresSurface);

							if (spawnLoc == Point3D.Zero)
							{
								return false;
							}
						}
					}
				}

				mob.MoveToWorld(spawnLoc, map);

				if (mob is BaseCreature)
				{
					((BaseCreature)mob).Home = spawnLoc;
				}

				return true;
			}

			public static bool MOVETOSPAWNLOCATION(
				TriggerObject trigObject,
				IEntity mob,
				Map map,
				ArrayList rectangles,
				ArrayList weightedProbabilities,
				int zLevel,
				bool requiresSurface)
			{
				if (mob == null || map == null || map == Map.Internal || rectangles == null || rectangles.Count == 0 ||
					weightedProbabilities == null || weightedProbabilities.Count == 0)
				{
					return false;
				}

				// try up to 4 times
				Point3D spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(
					trigObject, map, rectangles, weightedProbabilities, zLevel, requiresSurface);

				if (spawnLoc == Point3D.Zero)
				{
					spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(trigObject, map, rectangles, weightedProbabilities, zLevel, requiresSurface);

					if (spawnLoc == Point3D.Zero)
					{
						LOG(trigObject, "MOVETOSPAWNLOCATION", trigObject.Script.ScriptFile + " failing to find spawn loc");

						spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(
							trigObject, map, rectangles, weightedProbabilities, zLevel, requiresSurface);

						if (spawnLoc == Point3D.Zero)
						{
							spawnLoc = GETVALIDSPAWNLOCATIONANDMAP(
								trigObject, map, rectangles, weightedProbabilities, zLevel, requiresSurface);

							if (spawnLoc == Point3D.Zero)
							{
								return false;
							}
						}
					}
				}

				mob.MoveToWorld(spawnLoc, map);

				if (mob is BaseCreature)
				{
					((BaseCreature)mob).Home = spawnLoc;
				}

				return true;
			}

			public static void SETHITSSTAMMANA(TriggerObject trigObject, BaseCreature toSet, int hits, int stam, int mana)
			{
				if (toSet == null)
				{
					return;
				}

				toSet.HitsMaxSeed = hits;
				toSet.Hits = hits;
				toSet.StamMaxSeed = stam;
				toSet.Stam = stam;
				toSet.ManaMaxSeed = mana;
				toSet.Mana = mana;
			}

			public static void MOBCLICKMSG(TriggerObject trigObject, Mobile from, Mobile clickedMob, string message)
			{
				if (from != null && clickedMob != null && message != null && from.NetState != null)
				{
					clickedMob.PrivateOverheadMessage(
						MessageType.Label,
						clickedMob.NameHue != -1 ? clickedMob.NameHue : Notoriety.GetHue(Notoriety.Compute(from, clickedMob)),
						Mobile.AsciiClickMessage,
						message,
						from.NetState);
				}
			}

			public static void ITEMCLICKMSG(TriggerObject trigObject, Mobile from, Item item, string message)
			{
				if (from != null && item != null && message != null && from.NetState != null)
				{
					from.Send(new UnicodeMessage(item.Serial, item.ItemID, MessageType.Label, 0x3B2, 3, from.Language, "", message));
				}
			}

			public static MoonPhase GETMOONPHASE(TriggerObject trigObject, IPoint2D location, Map map)
			{
				return location == null || map == null || map == Map.Internal ? 0 : Clock.GetMoonPhase(map, location.X, location.Y);
			}

			public static int GETUOHOURS(TriggerObject trigObject, IPoint2D location, Map map)
			{
				if (location == null || map == null || map == Map.Internal)
				{
					return 0;
				}

				int hours, minutes, totalminutes;

				Clock.GetTime(map, location.X, location.Y, out hours, out minutes, out totalminutes);

				return hours;
			}

			public static int GETUOMINUTES(TriggerObject trigObject, IPoint2D location, Map map)
			{
				if (location == null || map == null || map == Map.Internal)
				{
					return 0;
				}

				int hours, minutes, totalminutes;

				Clock.GetTime(map, location.X, location.Y, out hours, out minutes, out totalminutes);

				return minutes;
			}

			public static void RANDOMFACIALHAIR(TriggerObject trigObject, Mobile mob)
			{
				if (mob != null)
				{
					Utility.AssignRandomFacialHair(mob);
				}
			}

			public static void RANDOMFACIALHAIR(TriggerObject trigObject, Mobile mob, bool randomHue)
			{
				if (mob != null)
				{
					Utility.AssignRandomFacialHair(mob, randomHue);
				}
			}

			public static void RANDOMHAIR(TriggerObject trigObject, Mobile mob)
			{
				if (mob != null)
				{
					Utility.AssignRandomHair(mob);
				}
			}

			public static void RANDOMHAIR(TriggerObject trigObject, Mobile mob, bool randomHue)
			{
				if (mob != null)
				{
					Utility.AssignRandomHair(mob, randomHue);
				}
			}

			public static int RANDOMNEUTRALHUE(TriggerObject trigObject)
			{
				return Utility.RandomNeutralHue();
			}

			public static ArrayList ONLINEYOUNG(TriggerObject trigObject)
			{
				return
					new ArrayList(
						NetState.Instances.Where(ns => ns.Mobile is PlayerMobile && ((PlayerMobile)ns.Mobile).Young)
								.Select(ns => ns.Mobile)
								.ToArray());
			}

			public static ArrayList ONLINECOMPANIONS(TriggerObject trigObject)
			{
				return
					new ArrayList(
						NetState.Instances.Where(ns => ns.Mobile is PlayerMobile && ((PlayerMobile)ns.Mobile).Companion)
								.Select(ns => ns.Mobile)
								.ToArray());
			}

			public static ArrayList ALLCOMPANIONS(TriggerObject trigObject)
			{
				return new ArrayList(PlayerMobile.AllCompanions);
			}

			public static void DISCONNECT(TriggerObject trigObject, Mobile mob)
			{
				if (mob != null && mob.NetState != null)
				{
					mob.NetState.Dispose();
				}
			}

			public static void RANDOMOUTFIT(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return;
				}

				switch (Utility.Random(3))
				{
					case 0:
						mob.AddItem(new FancyShirt(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
						break;
					case 1:
						mob.AddItem(new Doublet(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
						break;
					case 2:
						mob.AddItem(new Shirt(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
						break;
				}

				switch (Utility.Random(4))
				{
					case 0:
						mob.AddItem(new Shoes(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
						break;
					case 1:
						mob.AddItem(new Boots(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
						break;
					case 2:
						mob.AddItem(new Sandals(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
						break;
					case 3:
						mob.AddItem(new ThighBoots(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
						break;
				}

				if (mob.Female)
				{
					switch (Utility.Random(6))
					{
						case 0:
							mob.AddItem(new ShortPants(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
							break;
						case 1:
						case 2:
							mob.AddItem(new Kilt(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
							break;
						case 3:
						case 4:
						case 5:
							mob.AddItem(new Skirt(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
							break;
					}
				}
				else
				{
					switch (Utility.Random(2))
					{
						case 0:
							mob.AddItem(new LongPants(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
							break;
						case 1:
							mob.AddItem(new ShortPants(0.1 > Utility.RandomDouble() ? 0 : Utility.RandomNeutralHue()));
							break;
					}
				}
			}

			public static bool ISHEATSOURCE(TriggerObject trigObject, object obj)
			{
				return obj != null && CookableFood.IsHeatSource(obj);
			}

			public static int RANDOMSKINHUE(TriggerObject trigObject)
			{
				return Utility.RandomSkinHue();
			}

			public static int RANDOMHAIRHUE(TriggerObject trigObject)
			{
				return Utility.RandomHairHue();
			}

			public static Container CREATECORPSE(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return null;
				}

				var content = new List<Item>();
				var equip = new List<Item>();
				var moveToPack = new List<Item>();
				var itemsCopy = new List<Item>(mob.Items);

				Container pack = mob.Backpack;

				foreach (Item item in itemsCopy)
				{
					if (item == pack)
					{
						continue;
					}

					DeathMoveResult res = mob.GetParentMoveResultFor(item);

					switch (res)
					{
						case DeathMoveResult.MoveToCorpse:
							{
								content.Add(item);
								equip.Add(item);
								break;
							}
						case DeathMoveResult.MoveToBackpack:
							{
								moveToPack.Add(item);
								break;
							}
					}
				}

				if (pack != null)
				{
					var packCopy = new List<Item>();

					if (pack.Items != null && pack.Items.Count > 0)
					{
						packCopy.AddRange(pack.Items);

						foreach (Item item in packCopy)
						{
							DeathMoveResult res = mob.GetInventoryMoveResultFor(item);

							if (res == DeathMoveResult.MoveToCorpse)
							{
								content.Add(item);

								//RunUO SVN 610 - Core change instead.
								var subItems = new List<Item>();
								var lookup = item.LookupItems();

								if (lookup != null && lookup.Count > 0)
								{
									subItems.AddRange(lookup);
								}

								moveToPack.AddRange(
									subItems.Where(subItem => !subItem.Deleted && (subItem.LootType == LootType.Blessed || subItem.Insured)));
							}
							else
							{
								moveToPack.Add(item);
							}
						}
					}

					foreach (Item item in moveToPack.Where(item => !mob.RetainPackLocsOnDeath || item.Parent != pack))
					{
						pack.DropItem(item);
					}
				}

				HairInfo hair = null;

				if (mob.HairItemID != 0)
				{
					hair = new HairInfo(mob.HairItemID, mob.HairHue);
				}

				FacialHairInfo facialhair = null;

				if (mob.FacialHairItemID != 0)
				{
					facialhair = new FacialHairInfo(mob.FacialHairItemID, mob.FacialHairHue);
				}

				return Mobile.CreateCorpseHandler != null ? Mobile.CreateCorpseHandler(mob, hair, facialhair, content, equip) : null;
			}

			// THIS IS WHERE I LEFT OFF IN THE DOCUMENTATION!!!!!!!!!!!!!!!!!!!
			public static void SETSTATS(
				TriggerObject trigObject, BaseCreature creature, int strength, int dexterity, int intelligence)
			{
				if (creature == null)
				{
					return;
				}

				creature.RawStr = strength;
				creature.HitsMaxSeed = strength;
				creature.Hits = strength;
				creature.RawDex = dexterity;
				creature.StamMaxSeed = dexterity;
				creature.Stam = dexterity;
				creature.RawInt = intelligence;
				creature.ManaMaxSeed = intelligence;
				creature.Mana = intelligence;
			}

			public static bool BEGINACTION(TriggerObject trigObject, Mobile mob, object obj)
			{
				return mob != null && obj != null && mob.BeginAction(obj);
			}

			public static bool BEGINACTION(TriggerObject trigObject, Mobile mob, object obj, TimeSpan duration)
			{
				if (mob == null || obj == null || duration < TimeSpan.Zero || !mob.BeginAction(obj))
				{
					return false;
				}

				Timer.DelayCall(duration, ReleaseActionLock, Tuple.Create(mob, obj));
				return true;
			}

			public static void ReleaseActionLock(Tuple<Mobile, object> state)
			{
				if (state != null && state.Item1 != null && state.Item2 != null)
				{
					state.Item1.EndAction(state.Item2);
				}
			}

			public static void UpdateNearbyClients(Mobile m, Direction d)
			{
				if (m == null)
				{
					return;
				}

				var eable = m.Map.GetClientsInRange(m.Location);

				foreach (NetState state in eable.OfType<NetState>().Where(ns => ns != m.NetState))
				{
					Mobile beholder = state.Mobile;

					if (state.StygianAbyss)
					{
						int noto = Notoriety.Compute(beholder, m);

						Packet p = Packet.Acquire(new MobileDirectionToOthersOnly(m, noto, d));

						state.Send(p);
					}
					else
					{
						int noto = Notoriety.Compute(beholder, m);

						Packet p = Packet.Acquire(new MobileDirectionToOthersOnly(m, noto, d));

						state.Send(p);
					}
				}

				eable.Free();
			}

			public static bool CANBEGINACTION(TriggerObject trigObject, Mobile mob, object obj)
			{
				return mob.CanBeginAction(obj);
			}

			public static void SWINGDIRECTION(TriggerObject trigObject, Mobile mob, Direction direction)
			{
				if (mob != null && mob.Weapon != null)
				{
					SWINGDIRECTION(trigObject, mob, direction, mob.Weapon.MaxRange);
				}
			}

			public static void SWINGDIRECTION(TriggerObject trigObject, Mobile mob, Direction direction, int range)
			{
				if (mob == null || mob.Weapon == null)
				{
					return;
				}

				IWeapon weapon = mob.Weapon;

				//mob.Direction = direction;  // don't do this b/c it kind of causes rubber band to the client
				UpdateNearbyClients(mob, direction);

				Point2D directionVector = Point2D.Zero;

				switch (direction & Direction.Mask)
				{
					case Direction.North:
						directionVector = new Point2D(0, -1);
						break;
					case Direction.Right:
						directionVector = new Point2D(1, -1);
						break;
					case Direction.East:
						directionVector = new Point2D(1, 0);
						break;
					case Direction.Down:
						directionVector = new Point2D(1, 1);
						break;
					case Direction.South:
						directionVector = new Point2D(0, 1);
						break;
					case Direction.Left:
						directionVector = new Point2D(-1, 1);
						break;
					case Direction.West:
						directionVector = new Point2D(-1, 0);
						break;
					case Direction.Up:
						directionVector = new Point2D(-1, -1);
						break;
				}

				var possibleTargets = new List<Mobile>();
				Point3D currentLoc = mob.Location;

				if (range <= 1 || directionVector == Point2D.Zero)
				{
					//IPooledEnumerable mobsOnHitSpot = mob.Map.GetMobilesInRange(new Point3D(currentLoc.X + directionVector.X, currentLoc.Y + directionVector.Y, mob.Location.Z));

					currentLoc.X += directionVector.X;
					currentLoc.Y += directionVector.Y;

					Sector newSector = mob.Map.GetSector(currentLoc);

					possibleTargets.AddRange(
						newSector.Mobiles.Where(m => m.X == currentLoc.X && m.Y == currentLoc.Y && m != mob && mob.CanBeHarmful(m)));
				}
				else
				{
					for (int i = 0; i < range; i++)
					{
						currentLoc.X += directionVector.X;
						currentLoc.Y += directionVector.Y;

						Sector newSector = mob.Map.GetSector(currentLoc);

						possibleTargets.AddRange(
							newSector.Mobiles.Where(
								m => m.X == currentLoc.X && m.Y == currentLoc.Y && m != mob && mob.CanBeHarmful(m) && mob.InLOS(m)));

						if (possibleTargets.Count > 0)
						{
							break; // we found our mark
						}
					}
				}

				if (possibleTargets.Count > 0)
				{
					// TODO: maybe I should add a check for friends? (less likely to hit a friend?)
					Mobile target = possibleTargets[Utility.Random(possibleTargets.Count)];

					if (weapon is BaseRanged)
					{
						BaseRanged ranged = weapon as BaseRanged;
						bool canSwing = ranged.CanSwing(mob, target);

						if (mob is PlayerMobile)
						{
							PlayerMobile pm = (PlayerMobile)mob;

							if (pm.DuelContext != null && !pm.DuelContext.CheckItemEquip(mob, ranged))
							{
								canSwing = false;
							}
						}

						if (canSwing && mob.HarmfulCheck(target))
						{
							mob.DisruptiveAction();
							mob.Send(new Swing(0, mob, target));

							if (ranged.OnFired(mob, target))
							{
								if (ranged.CheckHit(mob, target))
								{
									ranged.OnHit(mob, target);
								}
								else
								{
									ranged.OnMiss(mob, target);
								}
							}
						}

						mob.RevealingAction();

						//GetDelay(mob);
					}
					else
					{
						weapon.OnSwing(mob, target);
					}
				}
				else
				{
					if (weapon is BaseRanged)
					{
						if (((BaseRanged)weapon).OnFired(mob, null))
						{
							MEFFECT(trigObject, ((BaseRanged)weapon).EffectID, 18, mob, currentLoc);
							SOUND(null, mob, Utility.RandomMinMax(0x538, 0x53a));
							SWINGANIMATION(trigObject, mob);
						}
					}
					else
					{
						SOUND(null, mob, Utility.RandomMinMax(0x538, 0x53a));
						SWINGANIMATION(trigObject, mob);
					}
				}
			}

			public static bool MOVE(TriggerObject trigObject, Mobile mob, Direction direction)
			{
				return mob != null && mob.Move(direction);
			}

			public static Direction RANDOMDIRECTION(TriggerObject trigObject)
			{
				return (Direction)Utility.Random(8);
			}

			public static Direction DIRECTION(TriggerObject trigObject, string directionString)
			{
				Direction d;

				if (!Enum.TryParse(directionString, out d))
				{
					d = Direction.Mask;
				}

				return d;
			}

			public static void LINESCRIPT(
				TriggerObject trigObject, IPoint3D from, IPoint3D to, string scriptFile, TimeSpan delay)
			{
				if (from == null || to == null || scriptFile == null)
				{
					return;
				}

				Map map;

				if (from is IEntity)
				{
					map = ((IEntity)from).Map;
				}
				else if (to is IEntity)
				{
					map = ((IEntity)to).Map;
				}
				else
				{
					map = Map.Felucca;
				}

				LINESCRIPT(trigObject, from, to, scriptFile, delay, map);
			}

			public static void LINESCRIPT(
				TriggerObject trigObject, IPoint3D from, IPoint3D to, string scriptFile, TimeSpan delay, Map map)
			{
				if (from == null || to == null || scriptFile == null || map == null || map == Map.Internal)
				{
					return;
				}

				UberScriptItem scriptSpot = new UberScriptItem();
				XmlScript newScript = new XmlScript(scriptFile)
				{
					Name = "line"
				};

				scriptSpot.MoveToWorld(new Point3D(from), map);

				XmlAttach.AttachTo(scriptSpot, newScript);

				if (from.X == to.X && from.Y == to.Y)
				{
					newScript.Execute(trigObject, false);
					scriptSpot.Delete();
					return;
				}

				LineScriptTimer timer = new LineScriptTimer(from, to, map, scriptSpot, trigObject, delay);
				timer.Start();

				AllLineScriptTimers.Add(timer);
			}

			private static readonly List<LineScriptTimer> AllLineScriptTimers = new List<LineScriptTimer>();

			public static void StopAllLineScriptTimers()
			{
				foreach (LineScriptTimer timer in AllLineScriptTimers.Where(timer => timer.Running))
				{
					timer.Stop();
				}

				AllLineScriptTimers.Clear();
			}

			private class LineScriptTimer : Timer
			{
				private Map Map;
				private readonly IPoint3D From;
				private readonly IPoint3D To;
				private readonly Point3D DirectionVector;
				private int Count;
				private double CurrentX;
				private double CurrentY;
				private double CurrentZ;
				private readonly double NormalizedDirectionVectorX;
				private readonly double NormalizedDirectionVectorY;
				private readonly double NormalizedDirectionVectorZ;
				private readonly UberScriptItem ScriptSpot;
				private readonly List<IPoint2D> Visited = new List<IPoint2D>();
				private readonly TriggerObject TrigObj;
				private readonly TimeSpan DelayPerTick;
				private DateTime LastExecuted;

				public List<Mobile> FrozenMobs = new List<Mobile>();

				public LineScriptTimer(
					IPoint3D from, IPoint3D to, Map map, UberScriptItem scriptSpot, TriggerObject trigObject, TimeSpan delay)
					: base(TimeSpan.Zero, TimeSpan.Zero)
				{
					if (from.X == to.X && from.Y == to.Y)
					{
						throw new UberScriptException("LineScriptTimer cannot have same from and to point!");
					}

					Priority = TimerPriority.TenMS;

					From = from;
					To = to;
					Map = map;
					ScriptSpot = scriptSpot;
					DirectionVector = new Point3D(to.X - from.X, to.Y - from.Y, to.Z - from.Z);

					double magnitude =
						Math.Sqrt(
							DirectionVector.X * DirectionVector.X + DirectionVector.Y * DirectionVector.Y +
							DirectionVector.Z * DirectionVector.Z);

					NormalizedDirectionVectorX = DirectionVector.X / magnitude;
					NormalizedDirectionVectorY = DirectionVector.Y / magnitude;
					NormalizedDirectionVectorZ = DirectionVector.Z / magnitude;

					CurrentX = from.X;
					CurrentY = from.Y;
					CurrentZ = from.Z;

					TrigObj = trigObject;
					DelayPerTick = delay;
					LastExecuted = DateTime.UtcNow;
				}

				protected override void OnTick()
				{
					if (DateTime.UtcNow < LastExecuted + DelayPerTick)
					{
						return;
					}

					LastExecuted = DateTime.UtcNow;

					try
					{
						bool foundUnvisitedSquare;

						do
						{
							CurrentX += NormalizedDirectionVectorX;
							CurrentY += NormalizedDirectionVectorY;
							CurrentZ += NormalizedDirectionVectorZ;

							foundUnvisitedSquare = true;

							for (int i = Visited.Count - 1; i > -1; i--) // most likely to hit the last visited first
							{
								IPoint2D p = Visited[i];

								if (Convert.ToInt32(Math.Round(CurrentX)) != p.X || Convert.ToInt32(Math.Round(CurrentY)) != p.Y)
								{
									continue;
								}

								foundUnvisitedSquare = false;
								break;
							}
						}
						while (foundUnvisitedSquare == false);

						Point3D target = new Point3D(
							Convert.ToInt32(Math.Round(CurrentX)),
							Convert.ToInt32(Math.Round(CurrentY)),
							Convert.ToInt32(Math.Round(CurrentZ)));

						Visited.Add(target);
						ScriptSpot.MoveToWorld(target, ScriptSpot.Map);

						var scripts = XmlAttach.GetScripts(ScriptSpot);

						foreach (XmlScript script in scripts)
						{
							script.Execute(TrigObj, false);
						}

						Count++;

						if (Count > 1000)
						{
							throw new Exception("LINEFFECT had more than 1000 iterations!... not allowed!");
						}

						//Console.WriteLine(target.X + " vs " + To.X + "    " + target.Y + " vs " + To.Y);

						if (Math.Abs(target.X - From.X) + Math.Abs(target.Y - From.Y) < Math.Abs(To.X - From.X) + Math.Abs(To.Y - From.Y))
						{
							return;
						}

						Stop();
						AllLineScriptTimers.Remove(this);
					}
					catch (Exception e)
					{
						Stop();
						AllLineScriptTimers.Remove(this);
						Console.WriteLine("LINESCRIPT timer error! target script = " + ScriptSpot + ": " + e.Message);
						Console.WriteLine(e.StackTrace);
					}
				}
			}

			public static void SWING(TriggerObject trigObject, Mobile from, Mobile to)
			{
				if (to == null || to.Deleted || from.Deleted || to.Map != from.Map || !to.Alive || !from.Alive || !from.CanSee(to) ||
					to.IsDeadBondedPet || from.IsDeadBondedPet)
				{
					return;
				}

				IWeapon weapon = from.Weapon;

				if (!from.InRange(to, weapon.MaxRange))
				{
					return;
				}

				if (!from.InLOS(to))
				{
					return;
				}

				weapon.OnBeforeSwing(@from, to); //OnBeforeSwing for checking in regards to being hidden and whatnot
				from.RevealingAction();
				weapon.OnSwing(from, to);
				//from.m_NextCombatTime = DateTime.UtcNow + weapon.OnSwing(from, combatant);
			}

			public static bool HARMFULCHECK(TriggerObject trigObject, Mobile from, Mobile to)
			{
				return from != null && to != null && from.HarmfulCheck(to);
			}

			public static bool CHECKUSERANGE(TriggerObject trigObject, Mobile mob, Item item)
			{
				return mob != null && item != null && CHECKUSERANGE(trigObject, mob, item, 2);
			}

			public static bool CHECKUSERANGE(TriggerObject trigObject, Mobile mob, Item item, int distance)
			{
				if (mob == null || item == null)
				{
					return false;
				}

				if (item.RootParentEntity == mob)
				{
					return true;
				}

				if (distance < 0)
				{
					return false;
				}

				// they must be rootparententity in this case
				if (mob.Map != item.Map)
				{
					return false;
				}

				if (item.RootParentEntity == null)
				{
					if (Math.Max(Math.Abs(mob.X - item.X), Math.Abs(mob.Y - item.Y)) <= distance && item.IsAccessibleTo(mob) &&
						mob.CanSee(item))
					{
						return true;
					}
				}
				else
				{
					IEntity parentEntity = item.RootParentEntity;

					if (Math.Max(Math.Abs(mob.X - parentEntity.X), Math.Abs(mob.Y - parentEntity.Y)) <= distance &&
						item.IsAccessibleTo(mob) && mob.CanSee(item))
					{
						return true;
					}
				}

				return false;
			}

			public static Rectangle2D RECTANGLE2D(TriggerObject trigObject, IPoint2D northwestCorner, IPoint2D southwestCorner)
			{
				return northwestCorner != null && southwestCorner != null
						   ? new Rectangle2D(northwestCorner, southwestCorner)
						   : new Rectangle2D();
			}

			public static ArrayList SPLIT(TriggerObject trigObject, string input)
			{
				if (input == null)
				{
					return new ArrayList();
				}

				var splitString = input.Split();
				ArrayList output = new ArrayList(splitString.Length);

				output.AddRange(splitString);

				return output;
			}

			public static ArrayList SPLIT(TriggerObject trigObject, string input, string delimiter)
			{
				if (input == null || delimiter == null)
				{
					return new ArrayList();
				}

				var splitString = input.Split(delimiter[0]);
				ArrayList output = new ArrayList(splitString.Length);

				output.AddRange(splitString);

				return output;
			}

			public static int GETGROUPTOTALSCORE(TriggerObject trigObject, XmlGroup grp, string xmlValueName)
			{
				if (grp == null || xmlValueName == null)
				{
					return 0;
				}

				Type xmlvalueType = typeof(XmlValue);

				return
					grp.Members.Cast<Mobile>()
					   .Select(mob => GETATTACHMENT(null, mob, xmlvalueType, xmlValueName) as XmlValue)
					   .Where(value => value != null)
					   .Sum(value => value.Value);
			}

			/// <summary>
			///     Straight up award given to qualifying members of a group
			/// </summary>
			public static void AWARDGROUP(TriggerObject trigObject, XmlGroup grp, Item reward)
			{
				if (grp == null || reward == null)
				{
					AWARDGROUP(trigObject, grp, reward, 10000000); // give to all the team members
				}
			}

			public static void AWARDGROUP(TriggerObject trigObject, XmlGroup grp, Item reward, int indexArg)
			{
				if (grp == null || reward == null)
				{
					AWARDGROUP(trigObject, grp, reward, indexArg, "TopScorers"); // give to all the team members
				}
			}

			public static void AWARDGROUP(TriggerObject trigObject, XmlGroup grp, Item reward, int indexArg, string selector)
			{
				if (grp == null || reward == null)
				{
					return;
				}

				AwardGroupSelector awardGroupSelector;

				if (!Enum.TryParse(selector, out awardGroupSelector))
				{
					reward.Delete();
					throw new UberScriptException("Award selector invalid!");
				}

				DateTime now = DateTime.Now;

				switch (awardGroupSelector)
				{
					case AwardGroupSelector.Index:
						{
							if (grp.Members.Count > indexArg)
							{
								Mobile member = (Mobile)grp.Members[indexArg];

								LOCALMSG(
									null,
									member,
									"You received an award for your achievements in the " + grp.EventName +
									" event. It has been placed in your bankbox.");
								Item toAdd = DUPE(null, reward);

								if (toAdd != null)
								{
									member.BankBox.AddItem(toAdd);
									LoggingCustom.Log("UAwards.txt", now + "\t" + member + "\t" + toAdd);
								}
							}
						}
						break;
					case AwardGroupSelector.TopScorers:
						{
							for (int i = 0; i < grp.Members.Count; i++)
							{
								if (i >= indexArg)
								{
									break;
								}

								Mobile member = (Mobile)grp.Members[i];
								Item toAdd = DUPE(null, reward);

								if (toAdd == null)
								{
									continue;
								}

								member.BankBox.AddItem(toAdd);

								LoggingCustom.Log("UAwards.txt", now + "\t" + member + "\t" + toAdd);
								LOCALMSG(
									null,
									member,
									"You received an award for your achievements in the " + grp.EventName +
									" event. It has been placed in your bankbox.");
							}
						}
						break;
					case AwardGroupSelector.BottomScorers:
						{
							for (int i = grp.Members.Count - 1; i >= 0; i--)
							{
								if (i < grp.Members.Count - indexArg)
								{
									break;
								}

								Mobile member = (Mobile)grp.Members[i];
								Item toAdd = DUPE(null, reward);

								if (toAdd == null)
								{
									continue;
								}

								member.BankBox.AddItem(toAdd);

								LoggingCustom.Log("UAwards.txt", now + "\t" + member + "\t" + toAdd);
								LOCALMSG(
									null,
									member,
									"You received an award for your achievements in the " + grp.EventName +
									" event. It has been placed in your bankbox.");
							}
						}
						break;
				}

				// delete the original
				reward.Delete();
			}

			private enum AwardGroupSelector
			{
				TopScorers, // indexArg gives the number of top scorers to receive the award
				Index, // e.g. for 2nd place or 3rd place specifically
				BottomScorers // indexArg gives the number of bottom scorers to receive the award
			}

			public static void SORTGROUPMEMBERS(TriggerObject trigObject, XmlGroup grp, string xmlValueName)
			{
				if (grp == null)
				{
					return;
				}

				ArrayList output = new ArrayList(grp.Members.Count);
				ArrayList scores = new ArrayList(grp.Members.Count); // parallel scores list--get the scores first
				Type xmlvalueType = typeof(XmlValue);

				foreach (XmlValue value in
					grp.Members.Cast<Mobile>().Select(member => GETATTACHMENT(null, member, xmlvalueType, xmlValueName) as XmlValue))
				{
					// @null: pretty sure they will be the worst score at -2 billion
					scores.Add(value == null ? Int32.MinValue : value.Value);
				}

				var tempScores = new List<int>(grp.Members.Count);

				for (int i = 0; i < grp.Members.Count; i++)
				{
					bool added = false;
					// scores is in the same ordder as group.Members
					Mobile thisMember = (Mobile)grp.Members[i];
					int thisMemberScore = (int)scores[i];

					for (int j = 0; j < output.Count; j++)
					{
						if (thisMemberScore <= tempScores[j])
						{
							continue;
						}

						output.Insert(j, thisMember);
						tempScores.Insert(j, thisMemberScore);
						added = true;
						break;
					}

					if (added)
					{
						continue;
					}

					// add to end of list
					output.Add(thisMember);
					tempScores.Add(thisMemberScore);
				}

				grp.Members = output;
			}

			public static Mobile GETGROUPMOB(TriggerObject trigObject, Mobile mob, XmlGroup grp)
			{
				if (mob == null || grp == null)
				{
					return null;
				}

				Account account = GETACCOUNT(trigObject, mob);

				return account == null ? null : grp.Members.Cast<Mobile>().FirstOrDefault(member => member.Account == account);
			}

			public static void EVENTMESSAGE(TriggerObject trigObject, IEntity owner, string eventName, string message)
			{
				if (owner != null && eventName != null && !String.IsNullOrWhiteSpace(message))
				{
					EVENTMESSAGE(trigObject, owner, eventName, message, 0x3b2);
				}
			}

			public static void EVENTMESSAGE(TriggerObject trigObject, IEntity owner, string eventName, string message, int hue)
			{
				if (owner == null || eventName == null || String.IsNullOrWhiteSpace(message))
				{
					return;
				}

				ArrayList xmlgroups = GETATTACHMENTS(trigObject, owner, typeof(XmlGroup));

				foreach (XmlGroup grp in xmlgroups.Cast<XmlGroup>().Where(grp => grp.EventInProgress && grp.EventName == eventName))
				{
					GROUPMESSAGE(trigObject, grp, message, hue);
				}
			}

			public static void GROUPMESSAGE(TriggerObject trigObject, XmlGroup grp, string message)
			{
				if (grp != null && !String.IsNullOrWhiteSpace(message))
				{
					GROUPMESSAGE(trigObject, grp, message, 0x3b2);
				}
			}

			public static void GROUPMESSAGE(TriggerObject trigObject, XmlGroup grp, string message, int hue)
			{
				if (grp == null || String.IsNullOrWhiteSpace(message))
				{
					return;
				}

				var accountList =
					grp.Members.Cast<Mobile>().Where(mob => mob.Account is Account).Select(mob => (Account)mob.Account).ToList();

				foreach (NetState state in
					NetState.Instances.Where(ns => ns.Mobile != null && ns.Account is Account && accountList.Contains(ns.Account)))
				{
					state.Mobile.SendMessage(hue, message);
				}
			}

			public static ArrayList ONLINETEAMMOBS(TriggerObject trigObject, XmlTeam team)
			{
				if (team == null)
				{
					return new ArrayList();
				}

				var fromTeams = new List<XmlTeam>
				{
					team
				};

				ArrayList output = new ArrayList(10);

				foreach (NetState nextstate in NetState.Instances.Where(ns => ns.Mobile != null))
				{
					if (nextstate.Mobile.CustomTeam)
					{
						var toTeams = XmlAttach.GetTeams(nextstate.Mobile);

						if (XmlTeam.SameTeam(fromTeams, toTeams))
						{
							output.Add(nextstate.Mobile);
						}
					}
				}

				return output;
			}

			public static void TEAMMESSAGE(TriggerObject trigObject, Mobile mob, string msg)
			{
				if (mob == null || String.IsNullOrWhiteSpace(msg))
				{
					return;
				}

				var fromTeams = XmlAttach.GetTeams(mob);

				if (fromTeams == null || fromTeams.Count == 0)
				{
					return;
				}

				foreach (NetState nextstate in NetState.Instances.Where(ns => ns.Mobile != null))
				{
					if (nextstate.Mobile.AccessLevel >= AccessLevel.GameMaster)
					{
						// just get the first team
						nextstate.Mobile.SendMessage(101, "[" + fromTeams.First().TeamVal + "] " + msg);
					}
					else
					{
						if (nextstate.Mobile.CustomTeam)
						{
							var toTeams = XmlAttach.GetTeams(nextstate.Mobile);
							if (XmlTeam.SameTeam(fromTeams, toTeams))
							{
								nextstate.Mobile.SendMessage(101, msg);
							}
						}
					}
				}
			}

			public static void TEAMMESSAGE(TriggerObject trigObject, XmlTeam team, string msg)
			{
				if (team == null || String.IsNullOrWhiteSpace(msg))
				{
					return;
				}

				var fromTeams = new List<XmlTeam>
				{
					team
				};

				foreach (NetState nextstate in NetState.Instances.Where(ns => ns.Mobile != null))
				{
					if (nextstate.Mobile.AccessLevel >= AccessLevel.GameMaster)
					{
						// just get the first team
						nextstate.Mobile.SendMessage(101, "[" + team.TeamVal + "] " + msg);
					}
					else
					{
						if (nextstate.Mobile.CustomTeam)
						{
							var toTeams = XmlAttach.GetTeams(nextstate.Mobile);

							if (XmlTeam.SameTeam(fromTeams, toTeams))
							{
								nextstate.Mobile.SendMessage(101, msg);
							}
						}
					}
				}
			}

			public static void REVEALHIDDEN(TriggerObject trigObject, IPoint3D target, int range)
			{
				if (target == null)
				{
					return;
				}

				if (target is IEntity)
				{
					REVEALHIDDEN(trigObject, target, range, ((IEntity)target).Map);
				}
				else
				{
					REVEALHIDDEN(trigObject, target, range, Map.Felucca);
				}
			}

			public static void REVEALHIDDEN(TriggerObject trigObject, IPoint3D target, int range, Map map)
			{
				var inRange = map.GetMobilesInRange(new Point3D(target), range);

				foreach (Mobile trg in inRange.OfType<Mobile>().Where(trg => trg.Hidden && trg.AccessLevel == AccessLevel.Player))
				{
					trg.RevealingAction();
					trg.SendLocalizedMessage(500814); // You have been revealed
				}

				inRange.Free();
			}

			public static bool DETECTHIDDEN(TriggerObject trigObject, Mobile src)
			{
				return DETECTHIDDEN(trigObject, src, src);
			}

			public static bool DETECTHIDDEN(TriggerObject trigObject, Mobile src, IPoint3D targ)
			{
				if (src == null)
				{
					return false;
				}

				bool foundAnyone = false;
				Point3D p;

				if (targ is Mobile)
				{
					p = ((Mobile)targ).Location;
				}
				else if (targ is Item)
				{
					p = ((Item)targ).Location;
				}
				else if (targ != null)
				{
					p = new Point3D(targ);
				}
				else
				{
					p = src.Location;
				}

				double srcSkill = src.Skills[SkillName.DetectHidden].Value;
				int range = (int)(srcSkill / 10.0);

				if (!src.CheckSkill(SkillName.DetectHidden, 0.0, 100.0))
				{
					range /= 2;
				}

				if (range > 0)
				{
					var inRange = src.Map.GetMobilesInRange(p, range);

					foreach (Mobile trg in inRange.OfType<Mobile>().Where(trg => trg.Hidden && src != trg))
					{
						double ss = srcSkill + Utility.Random(21) - 10;
						double ts = trg.Skills[SkillName.Hiding].Value + Utility.Random(21) - 10;

						if (src.AccessLevel < trg.AccessLevel || ss < ts || (trg is ShadowKnight && (trg.X != p.X || trg.Y != p.Y)))
						{
							continue;
						}

						trg.RevealingAction();
						trg.SendLocalizedMessage(500814); // You have been revealed!
						foundAnyone = true;
					}

					inRange.Free();
				}

				return foundAnyone;
			}

			public static Mobile GETCHAMPREWARDMOB(TriggerObject trigObject, ChampionSpawn champSpawn)
			{
				return champSpawn == null ? null : GETCHAMPREWARDMOB(trigObject, champSpawn, false);
			}

			public static Mobile GETCHAMPREWARDMOB(TriggerObject trigObject, ChampionSpawn champSpawn, bool removeFromList)
			{
				if (champSpawn == null)
				{
					return null;
				}

				var scores = champSpawn.Scores;

				if (scores == null)
				{
					return null;
				}

				Mobile value = null;

				// first find all eligible receivers
				var eligibleMobs = new List<Mobile>();
				var eligibleMobScores = new List<double>();
				double totalScores = 0.0;
				
				foreach (var pair in scores)
				{
					if (pair.Key == null)
					{
						continue;
					}

					Mobile mob = pair.Key;

					bool eligible = champSpawn.IsEligible(mob);

					if (!eligible)
					{
						continue;
					}

					eligibleMobs.Add(mob);
					eligibleMobScores.Add(pair.Value);

					totalScores += pair.Value;
				}

				double currentTestValue = 0.0;
				double roll = Utility.RandomDouble() * totalScores;

				for (int i = 0; i < eligibleMobScores.Count; i++)
				{
					currentTestValue += eligibleMobScores[i];

					if (!(roll <= currentTestValue))
					{
						continue;
					}

					value = eligibleMobs[i];
					break;
				}

				if (value != null && removeFromList)
				{
					scores.Remove(value);
				}

				return value;
			}

			public static void CREATEPARTY(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return;
				}

				if (mob.Party is Party)
				{
					((Party)mob.Party).Remove(mob);
				}

				mob.Party = new Party(mob);
			}

			public static void ADDTOPARTY(TriggerObject trigObject, Party party, Mobile mob)
			{
				if (party == null || mob == null)
				{
					return;
				}

				if (mob.Party is Party)
				{
					((Party)mob.Party).Remove(mob);
				}

				mob.Party = party;
				party.Add(mob);
			}

			public static void RESETMOB(TriggerObject trigObject, Mobile mob)
			{
				CLEARSTATS(trigObject, mob);
				CLEARSKILLS(trigObject, mob);
				CLEARMOBITEMS(trigObject, mob);
			}

			public static void CLEARSTATS(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return;
				}

				mob.RawStr = 25;
				mob.RawInt = 25;
				mob.RawDex = 25;

				if (!(mob is BaseCreature))
				{
					return;
				}

				BaseCreature bc = (BaseCreature)mob;

				bc.HitsMaxSeed = 60;
				bc.Hits = 60;
				bc.ManaMaxSeed = 25;
				bc.ManaMaxSeed = 25;
				bc.StamMaxSeed = 25;
				bc.Stam = 25;
			}

			public static void CLEARSKILLS(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return;
				}

				//var info = SkillInfo.Table;

				for (int i = 0; i < mob.Skills.Length; i++)
				{
					mob.Skills[i].Base = 0.0;
				}
			}

			public static void CLEARCONTAINER(TriggerObject trigObject, Container container)
			{
				if (container == null)
				{
					return;
				}

				foreach (Item item in container.Items.ToArray())
				{
					item.Delete();
				}
			}

			public static void CLEARMOBITEMS(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return;
				}

				CLEARCONTAINER(trigObject, mob.Backpack);

				foreach (Item item in mob.Items.Select(
					i => new
					{
						item = i,
						layer = (byte)i.Layer
					})
										 .Where(kv => kv.layer > 0 && kv.layer <= 0x18 && kv.layer != 0x0F && kv.layer != 0x10 && kv.layer != 0x15)
										 .Select(kv => kv.item)
										 .ToArray())
				{
					item.Delete();
				}
			}

			public static void DEATHSOUND(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return;
				}

				int sound = mob.GetDeathSound();

				if (sound >= 0)
				{
					Effects.PlaySound(mob, mob.Map, sound);
				}
			}

			public static void DEATHANIMATION(TriggerObject trigObject, Mobile mob)
			{
				if (mob != null)
				{
					DEATHANIMATION(trigObject, mob, null);
				}
			}

			public static void DEATHANIMATION(TriggerObject trigObject, Mobile mob, Corpse c)
			{
				if (mob == null || mob.Map == null)
				{
					return;
				}

				Packet animPacket = null; //new DeathAnimation( this, c );
				Packet remPacket = null; //this.RemovePacket;

				var eable = mob.Map.GetClientsInRange(mob.Location);

				foreach (NetState state in eable.OfType<NetState>().Where(state => state != mob.NetState))
				{
					if (animPacket == null)
					{
						animPacket = Packet.Acquire(new DeathAnimation(mob, c));
					}

					state.Send(animPacket);

					if (state.Mobile.CanSee(mob))
					{
						continue;
					}

					if (remPacket == null)
					{
						remPacket = mob.RemovePacket;
					}

					state.Send(remPacket);
				}

				Packet.Release(animPacket);

				eable.Free();
			}

			public static bool TRYCREATEGROUPPARTY(TriggerObject trigObject, XmlGroup grp)
			{
				return grp != null && TRYCREATEGROUPPARTY(trigObject, grp, null, true);
			}

			public static bool TRYCREATEGROUPPARTY(TriggerObject trigObject, XmlGroup grp, Type mobType)
			{
				return grp != null && TRYCREATEGROUPPARTY(trigObject, grp, mobType, true);
			}

			public static bool TRYCREATEGROUPPARTY(TriggerObject trigObject, XmlGroup grp, Type mobType, bool participantsOnly)
			{
				if (grp == null)
				{
					return false;
				}

				var mobsToParty = new List<Mobile>();
				var accountsToAdd = new List<Account>();

				if (participantsOnly)
				{
					if (grp.Participants.Count < 2)
					{
						return false;
					}

					accountsToAdd.AddRange(
						grp.Participants.Cast<Mobile>().Where(mob => mob.Account is Account).Select(mob => (Account)mob.Account));
				}
				else
				{
					if (grp.Members.Count < 2)
					{
						return false;
					}

					accountsToAdd.AddRange(
						grp.Members.Cast<Mobile>().Where(mob => mob.Account is Account).Select(mob => (Account)mob.Account));
				}

				foreach (NetState state in NetState.Instances.Where(ns => ns.Account is Account))
				{
					Account account = (Account)state.Account;

					if (!accountsToAdd.Contains(account))
					{
						continue;
					}

					if (mobType != null)
					{
						if (IS(null, state.Mobile, mobType))
						{
							mobsToParty.Add(state.Mobile);
						}
					}
					else
					{
						mobsToParty.Add(state.Mobile);
					}
				}

				if (mobsToParty.Count < 2)
				{
					return false;
				}

				// we need to bring them all into one party, so grab the first party available
				Party existingParty = null;
				var mobsToAddToExistingParty = new List<Mobile>();

				foreach (Mobile mobToParty in mobsToParty)
				{
					if (mobToParty.Party is Party)
					{
						if (existingParty == null)
						{
							existingParty = (Party)mobToParty.Party;
							continue;
						}

						if (mobToParty.Party == existingParty)
						{
							continue; // already in the party
						}

						try // get them out of the party they were in
						{
							((Party)mobToParty.Party).Remove(mobToParty);
							mobsToAddToExistingParty.Add(mobToParty);
						}
						catch
						{ }
					}
					else
					{
						// not in a party yet
						mobsToAddToExistingParty.Add(mobToParty);
					}
				}

				if (existingParty == null)
				{
					// create a new party and add them in
					Party newParty = new Party(mobsToParty[0]);

					mobsToParty[0].Party = newParty;

					for (int i = 1; i < mobsToParty.Count; i++)
					{
						newParty.Add(mobsToParty[i]);
					}
				}
				else
				{
					// add the mobs that weren't in the party into the existing one
					foreach (Mobile mob in mobsToAddToExistingParty)
					{
						existingParty.Add(mob);
					}
				}

				return true;
			}

			/*public static ArrayList ALLBOATS()
			{
				return new ArrayList(World.Items.Values.OfType<BaseBoat>().ToArray());
			}*/

			public static ArrayList SORTGROUPS(TriggerObject trigObject, IEntity obj)
			{
				return obj == null ? new ArrayList() : SORTGROUPS(trigObject, obj, null);
			}

			public static ArrayList SORTGROUPS(TriggerObject trigObject, IEntity obj, string eventName)
			{
				if (obj == null)
				{
					return new ArrayList();
				}

				ArrayList groups = GETATTACHMENTS(trigObject, obj, typeof(XmlGroup));

				if (groups == null || groups.Count == 0)
				{
					return null;
				}

				if (eventName == null)
				{
					groups.Sort(GroupsComparer.Instance);
					return groups;
				}

				ArrayList output = new ArrayList(groups.Count);

				foreach (XmlGroup grp in groups.Cast<XmlGroup>().Where(grp => grp.EventName == eventName))
				{
					output.Add(grp);
				}

				output.Sort(GroupsComparer.Instance);

				return output;
			}

			private class GroupsComparer : IComparer //<XmlGroup>
			{
				public static readonly IComparer Instance = new GroupsComparer();

				public int Compare(object x, object y)
				{
					if (x == y || (x == null && y == null))
					{
						return 0;
					}

					if (x == null)
					{
						return 1;
					}

					if (y == null)
					{
						return -1;
					}

					XmlGroup a = x as XmlGroup;
					XmlGroup b = y as XmlGroup;

					if (a == null || b == null)
					{
						throw new ArgumentException();
					}

					if (a.EventType != b.EventType)
					{
						throw new UberScriptException(
							"SORTGROUPS error: two groups did not have the same EventScoring value!: " + a.EventType + " vs. " + b.EventType);
					}

					if (a.EventType == XmlGroup.EventScoring.HighestScore)
					{
						return b.Score - a.Score;
					}

					if (a.EventType == XmlGroup.EventScoring.LowestScore)
					{
						return a.Score - b.Score;
					}

					if (a.EventType == XmlGroup.EventScoring.LongestTime)
					{
						if (a.Time == TimeSpan.MinValue || a.Time < TimeSpan.Zero) // always a loser
						{
							return 1; // b is "higher"
						}

						if (b.Time == TimeSpan.MinValue || b.Time < TimeSpan.Zero)
						{
							return -1; // a is always "higher"
						}

						int aTime = (int)Math.Round(a.Time.TotalSeconds * 100.0);
						int bTime = (int)Math.Round(b.Time.TotalSeconds * 100.0);

						return bTime - aTime;
					}
					else // (a.EventType == XmlGroup.EventScoring.ShortestTime)
					{
						if (a.Time == TimeSpan.MinValue || a.Time < TimeSpan.Zero) // always a loser
						{
							return 1; // b is "higher"
						}

						if (b.Time == TimeSpan.MinValue || b.Time < TimeSpan.Zero)
						{
							return -1; // a is always "higher"
						}

						if (a.Time == TimeSpan.MaxValue) // always a loser
						{
							return 1; // b is "higher"
						}

						if (b.Time == TimeSpan.MaxValue)
						{
							return -1; // a is always "higher"
						}

						int aTime = (int)Math.Round(a.Time.TotalSeconds * 100.0);
						int bTime = (int)Math.Round(b.Time.TotalSeconds * 100.0);

						return aTime - bTime;
					}
				}
			}

			public static TimeSpan TIMESPANMIN(TriggerObject trigObject)
			{
				return TimeSpan.MinValue;
			}

			public static TimeSpan TIMESPANMAX(TriggerObject trigObject)
			{
				return TimeSpan.MaxValue;
			}

			public static DateTime DATETIMEMIN(TriggerObject trigObject)
			{
				return DateTime.MinValue;
			}

			public static DateTime DATETIMEMAX(TriggerObject trigObject)
			{
				return DateTime.MaxValue;
			}

			public static bool RETURNTOPLAYERMOBILE(TriggerObject trigObject, Mobile mob)
			{
				return mob != null && mob.NetState != null && CreaturePossession.AttemptReturnToOriginalBody(mob.NetState);
			}

			public static void INDIVIDUALFLASHEFFECT(TriggerObject trigObject, Mobile mob)
			{
				if (mob != null)
				{
					INDIVIDUALFLASHEFFECT(trigObject, mob, 2);
				}
			}

			public static void INDIVIDUALFLASHEFFECT(TriggerObject trigObject, Mobile mob, int type)
			{
				if (mob != null)
				{
					Effects.SendIndividualFlashEffect(mob, (FlashType)type);
				}
			}

			public static void FLASHEFFECT(TriggerObject trigObject, IEntity entity)
			{
				if (entity != null)
				{
					FLASHEFFECT(trigObject, entity, 2);
				}
			}

			public static void FLASHEFFECT(TriggerObject trigObject, IEntity entity, int type)
			{
				if (entity != null)
				{
					Effects.SendFlashEffect(entity, (FlashType)type);
				}
			}

			public static void BOLTEFFECT(TriggerObject trigObject, IEntity entity)
			{
				if (entity != null)
				{
					BOLTEFFECT(trigObject, entity, 0);
				}
			}

			public static void BOLTEFFECT(TriggerObject trigObject, IEntity entity, int hue)
			{
				if (entity != null)
				{
					Effects.SendBoltEffect(entity, true, hue);
				}
			}

			public static bool DISARM(TriggerObject trigObject, Mobile m)
			{
				if (m == null || m.Backpack == null)
				{
					return false;
				}

				Item disarm = m.FindItemOnLayer(Layer.OneHanded);

				if (disarm != null && disarm.Movable)
				{
					m.Backpack.AddItem(disarm);
				}

				disarm = m.FindItemOnLayer(Layer.TwoHanded);

				if (disarm != null && disarm.Movable)
				{
					m.Backpack.AddItem(disarm);
				}

				return true;
			}

			public static bool POLYMORPH(TriggerObject trigObject, Mobile mob, int bodyValue)
			{
				return POLYMORPH(trigObject, mob, bodyValue, TimeSpan.FromMinutes(3.0));
			}

			public static bool POLYMORPH(TriggerObject trigObject, Mobile mob, int bodyValue, TimeSpan duration)
			{
				if (mob == null)
				{
					throw new UberScriptException("POLYMORPH mob cannot be <= 0");
				}

				if (bodyValue <= 0)
				{
					throw new UberScriptException("POLYMORPH bodyvalue cannot be <= 0");
				}

				if (duration <= TimeSpan.Zero)
				{
					throw new UberScriptException("POLYMORPH duration cannot be <= 0");
				}

				if (!mob.CanBeginAction(typeof(PolymorphSpell)) || !mob.CanBeginAction(typeof(IncognitoSpell)) || mob.IsBodyMod)
				{
					return false;
				}

				IMount mount = mob.Mount;

				if (mount != null)
				{
					mount.Rider = null;
				}

				if (mob.Mounted)
				{
					return false;
				}

				if (mob.BeginAction(typeof(PolymorphSpell)))
				{
					mob.BodyMod = bodyValue;
					mob.HueMod = 0;

					if (duration != TimeSpan.MaxValue)
					{
						new ExpirePolymorphTimer(mob, duration).Start();
					}
				}

				return true;
			}

			private class ExpirePolymorphTimer : Timer
			{
				private readonly Mobile m_Owner;

				public ExpirePolymorphTimer(Mobile owner, TimeSpan duration)
					: base(duration)
				{
					m_Owner = owner;

					Priority = TimerPriority.OneSecond;
				}

				protected override void OnTick()
				{
					if (m_Owner.CanBeginAction(typeof(PolymorphSpell)))
					{
						return;
					}

					m_Owner.BodyMod = 0;
					m_Owner.HueMod = -1;
					m_Owner.EndAction(typeof(PolymorphSpell));
				}
			}

			public static bool ADDRUNEBOOKENTRY(
				TriggerObject trigObject, Runebook book, string description, Map map, Point3D location)
			{
				if (book == null || map == null || map == Map.Internal || location == Point3D.Zero || book.Entries.Count >= 16)
				{
					return false;
				}

				if (description == null)
				{
					description = String.Empty;
				}

				book.Entries.Add(new RunebookEntry(location, map, description, null, 0));
				return true;
			}

			public static bool REMOVERUNEBOOKENTRY(TriggerObject trigObject, Runebook book, RunebookEntry entry)
			{
				return book != null && entry != null && book.Entries.Remove(entry);
			}

			public static ArrayList GETRUNEBOOKENTRIES(TriggerObject trigObject, Runebook book)
			{
				return book != null ? new ArrayList(book.Entries) : new ArrayList();
			}

			public static ArrayList ALLITEMSOFTYPE(TriggerObject trigObject, Type type)
			{
				if (type == null || !type.IsSubclassOf(typeof(Item)))
				{
					return new ArrayList();
				}

				return new ArrayList(World.Items.Values.Where(i => i.GetType() == type).ToArray());
			}

			public static bool ISHOUSECOOWNER(TriggerObject trigObject, Mobile mob, BaseHouse house)
			{
				return mob != null && house != null && house.IsCoOwner(mob);
			}

			public static bool ISHOUSEFRIEND(TriggerObject trigObject, Mobile mob, BaseHouse house)
			{
				return mob != null && house != null && house.IsFriend(mob);
			}

			public static Item FINDITEMONLAYER(TriggerObject trigObject, Mobile mob, string layer)
			{
				if (mob == null || layer == null)
				{
					return null;
				}

				Layer l;

				if (!Enum.TryParse(layer, out l))
				{
					l = Layer.Invalid;
				}

				return mob.FindItemOnLayer(l);
			}

			// copied from AdminGump.cs
			public static ArrayList GETALLSHAREDACCOUNTS(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return new ArrayList();
				}

				Hashtable table = new Hashtable();
				ArrayList list;

				foreach (Account acct in Accounts.GetAccounts())
				{
					var theirAddresses = acct.LoginIPs;

					foreach (IPAddress ip in theirAddresses)
					{
						list = (ArrayList)table[ip];

						if (list == null)
						{
							table[ip] = list = new ArrayList();
						}

						list.Add(acct);
					}
				}

				list = new ArrayList(table);

				for (int i = 0; i < list.Count; ++i)
				{
					DictionaryEntry de = (DictionaryEntry)list[i];
					ArrayList accts = (ArrayList)de.Value;

					if (accts.Count == 1)
					{
						list.RemoveAt(i--);
					}
					else
					{
						accts.Sort(AccountComparer.Instance);
					}
				}

				list.Sort(SharedAccountComparer.Instance);

				return list;
			}

			// copied from AdminGump.cs
			private class SharedAccountComparer : IComparer //<DictionaryEntry>
			{
				public static readonly IComparer Instance = new SharedAccountComparer();

				public int Compare(object x, object y)
				{
					if (x == y || x == null && y == null)
					{
						return 0;
					}

					if (x == null)
					{
						return 1;
					}

					if (y == null)
					{
						return -1;
					}

					if (!(x is DictionaryEntry) || !(y is DictionaryEntry))
					{
						throw new ArgumentException();
					}

					DictionaryEntry a = (DictionaryEntry)x;
					DictionaryEntry b = (DictionaryEntry)y;

					ArrayList aList = (ArrayList)a.Value;
					ArrayList bList = (ArrayList)b.Value;

					return bList.Count - aList.Count;
				}
			}

			// copied from AdminGump.cs
			private class AccountComparer : IComparer //<Account>
			{
				public static readonly IComparer Instance = new AccountComparer();

				public int Compare(object x, object y)
				{
					if (x == y || x == null && y == null)
					{
						return 0;
					}

					if (x == null)
					{
						return 1;
					}

					if (y == null)
					{
						return -1;
					}

					Account a = x as Account;
					Account b = y as Account;

					if (a == null || b == null)
					{
						throw new ArgumentException();
					}

					AccessLevel aLevel, bLevel;
					bool aOnline, bOnline;

					AdminGump.GetAccountInfo(a, out aLevel, out aOnline);
					AdminGump.GetAccountInfo(b, out bLevel, out bOnline);

					if (aOnline && !bOnline)
					{
						return -1;
					}

					if (!aOnline && bOnline)
					{
						return 1;
					}

					if (aLevel > bLevel)
					{
						return -1;
					}

					if (aLevel < bLevel)
					{
						return 1;
					}

					return Insensitive.Compare(a.Username, b.Username);
				}
			}

			private static ArrayList GETALLSHAREDACCOUNTS(IPAddress ipAddress)
			{
				if (ipAddress == null)
				{
					return new ArrayList();
				}

				ArrayList list = new ArrayList(100);

				foreach (Account acct in Accounts.GetAccounts())
				{
					var theirAddresses = acct.LoginIPs;
					bool contains = false;

					for (int i = 0; !contains && i < theirAddresses.Count; ++i)
					{
						contains = ipAddress.Equals(theirAddresses[i]);
					}

					if (contains)
					{
						list.Add(acct);
					}
				}

				list.Sort(AccountComparer.Instance);
				return list;
			}

			public static ArrayList GETIPASSOCIATEDHOUSES(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return new ArrayList();
				}

				ArrayList output = new ArrayList(10);
				ArrayList allHouses = BaseHouse.AllHouses;
				ArrayList accounts = GETALLSHAREDACCOUNTS(trigObject, mob);

				foreach (Account acct in accounts)
				{
					for (int i = 0; i < acct.Length; ++i)
					{
						Mobile accountMob = acct[i];

						if (accountMob == null)
						{
							continue;
						}

						output.AddRange(BaseHouse.GetHouses(accountMob));

						foreach (BaseHouse house in allHouses)
						{
							if (house.CoOwners.Contains(accountMob) && !output.Contains(house))
							{
								output.Add(house);
							}
							else if (house.Friends.Contains(accountMob) && !output.Contains(house))
							{
								output.Add(house);
							}
						}
					}
				}

				return output;
			}

			public static ArrayList GETASSOCIATEDHOUSES(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return new ArrayList();
				}

				ArrayList allHouses = BaseHouse.AllHouses;
				ArrayList output = new ArrayList(4);
				Account acct = mob.Account as Account;

				if (acct == null)
				{
					output.AddRange(BaseHouse.GetHouses(mob)); // add owned houses... might not have any

					foreach (BaseHouse house in allHouses)
					{
						if (house.CoOwners.Contains(mob) && !output.Contains(house))
						{
							output.Add(house);
						}
						else if (house.Friends.Contains(mob) && !output.Contains(house))
						{
							output.Add(house);
						}
					}
				}
				else
				{
					for (int i = 0; i < acct.Length; ++i)
					{
						Mobile accountMob = acct[i];

						if (accountMob == null)
						{
							continue;
						}

						output.AddRange(BaseHouse.GetHouses(accountMob));

						foreach (BaseHouse house in allHouses)
						{
							if (house.CoOwners.Contains(accountMob) && !output.Contains(house))
							{
								output.Add(house);
							}
							else if (house.Friends.Contains(accountMob) && !output.Contains(house))
							{
								output.Add(house);
							}
						}
					}
				}

				return output;
			}

			public static ArrayList ALLHOUSES(TriggerObject trigObject)
			{
				return BaseHouse.AllHouses;
			}

			public static ArrayList GETHOUSES(TriggerObject trigObject, Mobile owner)
			{
				if (owner == null)
				{
					return new ArrayList();
				}

				ArrayList output = new ArrayList();
				Account acct = owner.Account as Account;

				if (acct == null)
				{
					output.AddRange(BaseHouse.GetHouses(owner));
				}
				else
				{
					for (int i = 0; i < acct.Length; ++i)
					{
						Mobile mob = acct[i];

						if (mob != null)
						{
							output.AddRange(BaseHouse.GetHouses(mob));
						}
					}
				}

				return output;
			}

			public static ArrayList GETIPHOUSES(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return new ArrayList();
				}

				ArrayList output = new ArrayList(3);
				ArrayList accounts = GETALLSHAREDACCOUNTS(trigObject, mob);

				foreach (Account acct in accounts)
				{
					for (int i = 0; i < acct.Length; ++i)
					{
						Mobile accountMob = acct[i];

						if (accountMob != null)
						{
							output.AddRange(BaseHouse.GetHouses(mob));
						}
					}
				}

				return output;
			}

			public static bool GROUPMOBSINRANGE(TriggerObject trigObject, Mobile mob, string name, IPoint2D location, int range)
			{
				if (mob == null || name == null || location == null)
				{
					return false;
				}

				XmlGroup grp = GETGROUP(trigObject, mob, name);

				if (grp == null)
				{
					return false; // expect there to be a group
				}

				return
					grp.Members.Cast<Mobile>()
					   .Where(groupMob => groupMob.NetState != null)
					   .All(groupMob => groupMob.InRange(location, range));
			}

			public static bool PARTYMOBSINRANGE(TriggerObject trigObject, Mobile mob, IPoint2D location, int range)
			{
				if (mob == null || location == null)
				{
					return false;
				}

				ArrayList partyMobs = GETPARTYMOBS(trigObject, mob);

				if (partyMobs == null)
				{
					return true; // only 1 mob here
				}

				return partyMobs.Cast<Mobile>().All(partyMob => partyMob.InRange(location, range));
			}

			public static ArrayList GETPARTYMOBS(TriggerObject trigObject, Mobile mob)
			{
				return mob == null || mob.Party == null || !(mob.Party is Party) ? null : new ArrayList(((Party)mob.Party).Members);
			}

			public static Account GETACCOUNT(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null || mob.Account == null)
				{
					return null;
				}

				if (mob.Account == null)
				{
					mob = GETPOSSESSOR(trigObject, mob);

					if (mob == null || mob.Account == null)
					{
						return null;
					}
				}

				return mob.Account as Account;
			}

			public static XmlGroup GETGROUP(TriggerObject trigObject, Mobile mob, string name)
			{
				if (mob == null || name == null)
				{
					return null;
				}

				Account account = GETACCOUNT(trigObject, mob);

				if (account == null)
				{
					return null;
				}

				string tagValue = account.GetTag(name);

				if (tagValue == null)
				{
					return null;
				}

				Serial serial = int.Parse(tagValue);
				IEntity owningEntity = World.FindEntity(serial);

				if (owningEntity == null) // no longer a valid group account tag, so remove it
				{
					account.RemoveTag(name);
					return null;
				}

				var groups = XmlGroup.GetGroups(owningEntity);

				if (groups == null)
				{
					// if we got here, then the group must have been manually deleted
					account.RemoveTag(name);
					return null;
				}

				foreach (XmlGroup grp in groups.Where(grp => grp.Members.Cast<Mobile>().Any(member => member.Account == account)))
				{
					return grp;
				}

				// if we got here, then the group must have been manually deleted
				account.RemoveTag(name);

				return null;
			}

			/// <summary>
			///     Return all XmlGroups associated with this mob
			/// </summary>
			/// <param name="trigObject"></param>
			/// <param name="mob"></param>
			/// <returns></returns>
			public static ArrayList GETGROUPS(TriggerObject trigObject, Mobile mob)
			{
				Account account = GETACCOUNT(trigObject, mob);

				if (account == null)
				{
					return null;
				}

				ArrayList output = new ArrayList();
				IEntity owningEntity;

				foreach (string tagValue in account.Tags.Where(t => t.Value != null).Select(tag => tag.Value))
				{
					try
					{
						Serial serial = int.Parse(tagValue);

						owningEntity = World.FindEntity(serial);

						if (owningEntity == null)
						{
							continue; // don't remove it here since it might remove other integer tags, and this checks all tags
						}

						var groups = XmlGroup.GetGroups(owningEntity);

						foreach (XmlGroup grp in groups.Where(grp => grp.Members.Contains(mob)))
						{
							output.Add(grp);
						}
					}
					catch
					{ }
				}

				if (output.Count == 0)
				{
					return null;
				}

				return output;
			}

			public static bool ADDTOGROUP(TriggerObject trigObject, Mobile mob, string eventName, XmlGroup grp)
			{
				if (mob == null || eventName == null || grp == null)
				{
					return false;
				}

				Account account = GETACCOUNT(trigObject, mob);

				if (account == null)
				{
					return false;
				}

				if (account.GetTag(eventName) != null)
				{
					return false; // already has that tag, can't add another one
				}

				if (grp.Members.Count >= grp.MaxMembers)
				{
					return false; // too many members
				}

				account.AddTag(eventName, (grp.AttachedTo).Serial.Value + "");

				// track the group tracking entity in the account tab
				grp.Members.Add(mob);

				return true;
			}

			public static bool ADDPARTYMEMBERSTOGROUP(TriggerObject trigObject, Mobile captain, string eventName)
			{
				return captain != null && eventName != null && ADDPARTYMEMBERSTOGROUP(trigObject, captain, eventName, true);
			}

			public static bool ADDPARTYMEMBERSTOGROUP(TriggerObject trigObject, Mobile captain, string eventName, bool invite)
			{
				if (captain == null || eventName == null)
				{
					return false;
				}

				ArrayList partyMobs = GETPARTYMOBS(trigObject, captain);

				if (partyMobs == null)
				{
					return false;
				}

				XmlGroup grp = GETGROUP(trigObject, captain, eventName);

				if (grp == null)
				{
					return false;
				}

				foreach (Mobile mob in partyMobs.Cast<Mobile>().Where(mob => !grp.Members.Contains(mob)))
				{
					/*
					if (ADDTOGROUP(trigObject, mob, name, group) == false)
					{
						captain.SendMessage(38, mob.RawName + " was not added to the group! They are probably in another group for the " + name + " event!");
						mob.SendMessage(38, "You were not added to the " + name + " event group, most likely because your account is already in a group for this event.");
					}
					else
					{
						captain.SendMessage(0x38, mob.RawName + " was added to the group for the " + name + " event.");
					}
					*/

					if (invite)
					{
						if (mob.CanBeginAction(typeof(XmlGroup)))
						{
							if (GETGROUP(trigObject, mob, eventName) == null) // not in a group already
							{
								mob.SendGump(
									new WarningGump(
										1060637,
										30720,
										captain.RawName + " is inviting you to join their group for the " + eventName +
										" event. Press OK to join or Cancel to reject the offer.\n BE AWARE that groups are associated BY ACCOUNT (not character) and that SOME groups are long-term commitments once the event has started (e.g. permanent or it costs something to leave the group), so make SURE you want to join that person's group before accepting. See the specific Event Master for the details regarding this particular event and the rules regarding leaving groups and group duration!",
										0xFFC000,
										320,
										240,
										JoinGroupCallback,
										new object[] {captain, grp, eventName}));
								mob.BeginAction(typeof(XmlGroup));
								Timer.DelayCall(TimeSpan.FromMinutes(2), ReleasePartyInviteLock, mob);
								captain.SendMessage(
									0x38, "You send an invite to " + mob.RawName + " to join your group for the " + eventName + ".");
							}
							else
							{
								captain.SendMessage(
									38,
									mob.RawName +
									" is already in a group (possibly on a different character) for that event and therefore cannot be invited!");
							}
						}
						else
						{
							captain.SendMessage(
								38,
								mob.RawName +
								" already has a pending invitation and cannot be invited until they choose to accept or reject it.");
						}
					}
					else // don't send invite, just try to add to the group
					{
						if (GETGROUP(trigObject, mob, eventName) == null) // not in a group already
						{
							if (ADDTOGROUP(null, mob, eventName, grp))
							{
								captain.SendMessage(0x38, "{0} has joined your group for {1}.", mob.RawName, eventName);
								mob.SendMessage(0x38, "You have joined " + captain.RawName + "'s group for " + eventName + ".");
							}
							else
							{
								if (grp.Members.Count >= grp.MaxMembers)
								{
									captain.SendMessage(
										0x38,
										"{0} cannot join your group because you already have the maximum {1} members in your group for {2}.",
										mob.RawName,
										grp.MaxMembers,
										eventName);
									mob.SendMessage(
										38,
										"You cannot join " + captain.RawName +
										"'s group because it already has the maximum number of team members allowed for this event.");
								}
								else
								{
									captain.SendMessage(
										0x38, "{0} cannot join your group because they are already in a group for {1}.", mob.RawName, eventName);
									mob.SendMessage(
										38,
										"You cannot join " + captain.RawName +
										"'s group because you are already in another group for this event (possibly on another character).");
								}
							}
						}
						else
						{
							captain.SendMessage(
								38,
								mob.RawName +
								" is already in a group (possibly on a different character) for that event and therefore cannot be added to your group!");
						}
					}
				}

				return true;
			}

			public static void ReleasePartyInviteLock(Mobile from)
			{
				if (from != null)
				{
					from.EndAction(typeof(XmlGroup));
				}
			}

			// THIS CALLBACK IS USED FOR ADDPARTYMEMBERSTOGROUP calls
			public static void JoinGroupCallback(Mobile from, bool okay, object state)
			{
				if (from == null || from.Deleted || state == null || !(state is object[]))
				{
					return;
				}

				try
				{
					var states = (object[])state;
					Mobile captain = (Mobile)states[0];
					XmlGroup group = (XmlGroup)states[1];
					string eventName = (string)states[2];

					if (okay)
					{
						if (ADDTOGROUP(null, from, eventName, group))
						{
							captain.SendMessage(0x38, "{0} has joined your group for {1}.", from.RawName, eventName);
							from.SendMessage(0x38, "You have joined " + captain.RawName + "'s group for " + eventName + ".");
						}
						else
						{
							if (group.Members.Count == group.MaxMembers)
							{
								captain.SendMessage(
									0x38,
									"{0} cannot join your group because you already have the maximum {1} members in your group for {2}.",
									from.RawName,
									group.MaxMembers,
									eventName);
								from.SendMessage(
									38,
									"You cannot join " + captain.RawName +
									"'s group because it already has the maximum number of team members allowed for this event.");
							}
							else
							{
								captain.SendMessage(
									0x38, "{0} cannot join your group because they are already in a group for {1}.", from.RawName, eventName);
								from.SendMessage(
									38,
									"You cannot join " + captain.RawName +
									"'s group because you are already in another group for this event (possibly on another character).");
							}
						}
					}
					else
					{
						captain.SendMessage(38, "{0} has rejected your offer to join your group for {1}.", from.RawName, eventName);
						from.SendMessage(0x38, "You have rejected " + captain.RawName + "'s invitation.");
					}
				}
				catch
				{ }

				from.EndAction(typeof(XmlGroup));
			}

			public static XmlGroup CREATEGROUP(
				TriggerObject trigObject, Mobile captain, string eventName, IEntity groupTrackingObject)
			{
				if (captain == null)
				{
					throw new UberScriptException("CREATEGROUP: Cannot create group because captain was null!");
				}

				if (groupTrackingObject == null)
				{
					throw new UberScriptException("CREATEGROUP: Cannot create group because groupTrackingObject was null!");
				}

				if (eventName == null)
				{
					throw new UberScriptException("CREATEGROUP: Cannot create group because eventName was null!");
				}

				Account account = GETACCOUNT(trigObject, captain);

				if (account == null)
				{
					return null;
				}

				string tagValue = account.GetTag(eventName);

				if (tagValue != null)
				{
					// they are already in a group of that type
					return null;
				}

				XmlGroup newGroup = new XmlGroup(captain.RawName, eventName)
				{
					Captain = captain
				}; // RawNames are unique across the server

				XmlAttach.AttachTo(groupTrackingObject, newGroup);
				ADDTOGROUP(trigObject, captain, eventName, newGroup);

				return newGroup;
			}

			public static bool REMOVEFROMGROUP(TriggerObject trigObject, Mobile mob, string name)
			{
				return mob != null && name != null && REMOVEFROMGROUP(trigObject, mob, name, false);
			}

			public static bool REMOVEFROMGROUP(TriggerObject trigObject, Mobile mob, string name, bool forceRemove)
			{
				if (mob == null || name == null)
				{
					return false;
				}

				Account account = GETACCOUNT(trigObject, mob);

				if (account == null || account.GetTag(name) == null)
				{
					return false;
				}

				XmlGroup group = GETGROUP(trigObject, mob, name);

				if (group == null)
				{
					return false;
				}

				if (group.Locked)
				{
					// only remove if it's force remove
					if (forceRemove)
					{
						account.RemoveTag(name);

						if (mob == group.Captain)
						{
							DISBANDGROUP(trigObject, group);
						}
						else
						{
							group.Members.Remove(mob);
						}
					}
					else
					{
						return false;
					}
				}
				else
				{
					account.RemoveTag(name);

					if (mob == group.Captain)
					{
						DISBANDGROUP(trigObject, group);
					}
					else
					{
						group.Members.Remove(mob);
					}
				}

				return true;
			}

			public static bool DISBANDGROUP(TriggerObject trigObject, XmlGroup group)
			{
				return group != null && DISBANDGROUP(trigObject, group, false);
			}

			public static bool DISBANDGROUP(TriggerObject trigObject, XmlGroup group, bool forceDisband)
			{
				if (group == null || (group.Locked && !forceDisband))
				{
					return false;
				}

				// the only way to find the correct account tag to remove is going a round-about way
				foreach (Mobile mob in group.Members)
				{
					Account account = GETACCOUNT(trigObject, mob);

					if (account == null || account.GetTag(group.EventName) == null)
					{
						continue;
					}

					account.RemoveTag(group.EventName);
				}

				group.Delete();
				return true;
			}

			public static void FACTIONBCAST(TriggerObject trigObject, object message)
			{
				FACTIONBCAST(trigObject, message, 0x482);
			}

			public static void FACTIONBCAST(TriggerObject trigObject, object message, int hue)
			{
				FACTIONBCAST(trigObject, message, hue, null);
			}

			public static void FACTIONBCAST(TriggerObject trigObject, object message, int hue, string factionName)
			{
				if (message == null)
				{
					message = "null";
				}

				if (factionName == null)
				{
					foreach (PlayerMobile pm in
						NetState.Instances.Where(ns => ns.Mobile is PlayerMobile)
								.Select(ns => (PlayerMobile)ns.Mobile)
								.Where(pm => pm.FactionName != null))
					{
						pm.SendMessage(hue, message.ToString());
					}
				}
				else
				{
					foreach (PlayerMobile pm in
						NetState.Instances.Where(ns => ns.Mobile is PlayerMobile)
								.Select(ns => (PlayerMobile)ns.Mobile)
								.Where(pm => pm.FactionName == factionName))
					{
						pm.SendMessage(hue, message.ToString());
					}
				}
			}

			public static XmlScript ADDSCRIPT(TriggerObject trigObject, IEntity entity, string fileName)
			{
				return entity == null || fileName == null ? null : ADDSCRIPT(trigObject, entity, fileName, null);
			}

			public static XmlScript ADDSCRIPT(TriggerObject trigObject, IEntity entity, string fileName, string name)
			{
				if (entity == null || fileName == null) // it's ok for name to be null
				{
					return null;
				}

				XmlScript script = new XmlScript(fileName)
				{
					Name = name
				};

				XmlAttach.AttachTo(entity, script);

				return script;
			}

			public static void ADDREGIONCONTROLAREA(
				TriggerObject trigObject, RegionControl control, IPoint2D start, IPoint2D end)
			{
				if (control == null || start == null || end == null)
				{
					return;
				}
				Rectangle2D newrect = new Rectangle2D(start, end);
				Rectangle3D rect3D = Region.ConvertTo3D(newrect);

				control.DoChooseArea(null, control.Map, rect3D.Start, rect3D.End, control);
			}

			public static ArrayList GETREGIONMOBS(TriggerObject trigObject, Region region)
			{
				return region != null ? new ArrayList(region.GetMobiles()) : new ArrayList();
			}

			public static ArrayList ORDERMOBS(TriggerObject trigObject)
			{
				return
					new ArrayList(
						World.Mobiles.Values.OfType<PlayerMobile>().Where(pm => pm.GuildLoyalty == GuildType.Order).ToArray());
			}

			public static ArrayList CHAOSMOBS(TriggerObject trigObject)
			{
				return
					new ArrayList(
						World.Mobiles.Values.OfType<PlayerMobile>().Where(pm => pm.GuildLoyalty == GuildType.Chaos).ToArray());
			}

			public static ArrayList CHAOSORDERMOBS(TriggerObject trigObject)
			{
				return
					new ArrayList(
						World.Mobiles.Values.OfType<PlayerMobile>().Where(pm => pm.GuildLoyalty != GuildType.Regular).ToArray());
			}

			public static ArrayList FACTIONMOBS(TriggerObject trigObject)
			{
				return
					new ArrayList(
						World.Mobiles.Values.OfType<PlayerMobile>()
							 .Where(pm => pm.FactionPlayerState != null && pm.FactionPlayerState.Faction != null)
							 .ToArray());
			}

			public static void OPENBROWSER(TriggerObject trigObject, Mobile mob, string url)
			{
				if (mob != null && url != null)
				{
					mob.LaunchBrowser(url);
				}
			}

			public static ArrayList PLAYERMOBS(TriggerObject trigObject)
			{
				return new ArrayList(World.Mobiles.Values.OfType<PlayerMobile>().ToArray());
			}

			public static string TIMESPANSTRING(TriggerObject trigObject, TimeSpan span)
			{
				if (span == TimeSpan.MinValue)
				{
					return "Never";
				}

				if (span == TimeSpan.MaxValue)
				{
					return "Forever";
				}

				StringBuilder sb = new StringBuilder();
				bool hasDays = false;
				bool hasHours = false;
				bool hasMinutes = false;

				if (span.Days > 0)
				{
					sb.Append(span.Days > 1 ? span.Days + " days" : "1 day");
					hasDays = true;
				}

				if (span.Hours > 0)
				{
					if (hasDays)
					{
						sb.Append(", ");
					}

					sb.Append(span.Hours > 1 ? span.Hours + " hours" : "1 hour");
					hasHours = true;
				}

				if (span.Minutes > 0)
				{
					if (hasDays || hasHours)
					{
						sb.Append(", ");
					}

					sb.Append(span.Minutes > 1 ? span.Minutes + " minutes" : "1 minute");
					hasMinutes = true;
				}

				if (span.Seconds > 0)
				{
					if (hasDays || hasHours || hasMinutes)
					{
						sb.Append(", ");
					}

					sb.Append(span.Seconds > 1 ? span.Seconds + " seconds" : "1 second");
				}

				return sb.ToString();
			}

			public static int GETRANGETOMULTI(TriggerObject trigObject, IPoint2D start, IPoint2D vector, int range)
			{
				if (start == null || vector == null)
				{
					return 0;
				}

				Point2D testLocation = new Point2D(start.X, start.Y);
				Map map = (start is IEntity ? ((IEntity)start).Map : Map.Felucca);

				for (int i = 0; i < range; i++)
				{
					testLocation.X += vector.X;
					testLocation.Y += vector.Y;

					BaseMulti target = BaseMulti.FindMultiAt(testLocation, map);

					if (target == null)
					{
						continue;
					}

					return i + 1;
				}

				return -1;
			}

			public static int GETRANGETOMULTI(
				TriggerObject trigObject, IPoint2D start, IPoint2D vector, int range, object type_or_typeString)
			{
				if (start == null || vector == null || type_or_typeString == null)
				{
					return 0;
				}

				Type type;

				if (type_or_typeString is Type)
				{
					type = (Type)type_or_typeString;
				}
				else
				{
					type = ScriptCompiler.FindTypeByName((string)type_or_typeString);
				}

				if (type == null)
				{
					return -1;
				}

				Point2D testLocation = new Point2D(start.X, start.Y);
				Map map = (start is IEntity ? ((IEntity)start).Map : Map.Felucca);

				for (int i = 0; i < range; i++)
				{
					testLocation.X += vector.X;
					testLocation.Y += vector.Y;

					BaseMulti target = BaseMulti.FindMultiAt(testLocation, map);

					if (target == null || !type.IsInstanceOfType(target))
					{
						continue;
					}

					return i + 1;
				}

				return -1;
			}

			public static int GETCANNONTARGETRANGE(TriggerObject trigObject, ShipCannon cannon, int range)
			{
				if (cannon == null || cannon.Deleted)
				{
					return 0;
				}

				Point2D testLocation = new Point2D(cannon.Location.X, cannon.Location.Y);
				Point2D dir = cannon.CannonDirectionVector;

				for (int i = 0; i < range; i++)
				{
					testLocation.X += dir.X;
					testLocation.Y += dir.Y;

					BaseMulti boatTarget = BaseMulti.FindMultiAt(testLocation, cannon.Map);

					if (boatTarget == null || boatTarget == cannon.Boat || !(boatTarget is BaseBoat) || ((BaseBoat)boatTarget).Sunk)
					{
						continue;
					}

					return i + 1;
				}

				return -1;
			}

			public static BaseMulti GETCANNONTARGET(TriggerObject trigObject, ShipCannon cannon, int range)
			{
				if (cannon == null || cannon.Deleted)
				{
					return null;
				}

				Point2D testLocation = new Point2D(cannon.Location.X, cannon.Location.Y);
				Point2D dir = cannon.CannonDirectionVector;

				for (int i = 0; i < range; i++)
				{
					testLocation.X += dir.X;
					testLocation.Y += dir.Y;

					BaseMulti boatTarget = BaseMulti.FindMultiAt(testLocation, cannon.Map);

					if (boatTarget == null || boatTarget == cannon.Boat || !(boatTarget is BaseBoat) || ((BaseBoat)boatTarget).Sunk)
					{
						continue;
					}

					return boatTarget;
				}

				return null;
			}

			public static bool RAREFILEEXISTS(TriggerObject trigObject, string filename)
			{
				return filename != null && RareSystem.ParsedFiles.ContainsKey(filename);
			}

			public static Item RARE(TriggerObject trigObject, string filename)
			{
				return filename == null ? null : RARE(trigObject, filename, null);
			}

			public static Item RARE(TriggerObject trigObject, string filename, string name)
			{
				RaresFile raresFile = RareSystem.GetRaresFile(filename);

				if (raresFile == null)
				{
					throw new UberScriptException("RARE file " + filename + " did not exist!");
				}

				if (name == null)
				{
					return raresFile.GetRandomRareEntry();
				}

				Item item = raresFile.GetRareByName(name);

				if (item == null)
				{
					throw new UberScriptException("RARE file " + filename + " did not have entry named \"" + name + "\"");
				}

				return item;
			}

			public static void CREATEMAP(TriggerObject trigObject, string mapToDupeName, string newMapName)
			{
				Map existing = null;

				try
				{
					existing = Map.Parse(newMapName);
				}
				catch
				{ }

				if (existing != null)
				{
					throw new UberScriptException("A map named " + newMapName + " already exists! You must use a unique name!");
				}

				Map map = Map.Parse(mapToDupeName);

				if (map != null)
				{
					new BaseInstanceMap(map, newMapName, MapRules.FeluccaRules);
				}
			}

			public static void DELETEMAP(TriggerObject trigObject, string toDelete)
			{
				BaseInstanceMap basemap = Map.Parse(toDelete) as BaseInstanceMap;

				if (basemap != null)
				{
					/*
					List<Item> items = new List<Item>();
					List<Mobile> mobiles = new List<Mobile>();

					foreach (Item item in World.Items.Values)
						if (item.Map == basemap && item.Parent == null)
							items.Add(item);

					for (int i = items.Count - 1; i >= 0; i--)
						items[i].Delete();

					foreach (Mobile m in World.Mobiles.Values)
						if (!m.Player && m.Map == basemap)
							mobiles.Add(m);

					for (int i = mobiles.Count - 1; i >= 0; i--)
						mobiles[i].Delete();
					*/
					basemap.Delete();
				}
			}

			public static string SEXTANTCOORDS(TriggerObject trigObject, IPoint3D loc, Map map)
			{
				int xLong = 0, yLat = 0;
				int xMins = 0, yMins = 0;
				bool xEast = false, ySouth = false;

				if (Sextant.Format(new Point3D(loc), map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
				{
					string location = String.Format(
						"{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
					return location;
				}

				return "Unknown";
			}

			/// <summary>
			///     Getter function (can use vars like THIS().xmlints.test can be accessed with XMLINTS(THIS(), xmlstring.varName) with this function, which you can't do with with THIS().xmlints._____ alone
			/// </summary>
			/// <param name="trigObject"></param>
			/// <param name="o"></param>
			/// <param name="varName"></param>
			/// <returns></returns>
			public static object XMLINTS(TriggerObject trigObject, IEntity o, string varName)
			{
				if (!(o is Mobile || o is Item))
				{
					throw new UberScriptException("Can't set xmlints on anything but Mobile or Item");
				}

				// check for existing xmlValue attachment or create a new one
				Type ptype;
				var arglist = varName.Split('.');
				XmlValue xmlValue = XmlAttach.GetValueAttachment(o, arglist[0]);

				if (xmlValue == null)
				{
					return null; //throw new UberScriptException("Could not find XmlValue named " + name + " on " + o);
				}

				if (arglist.Length > 1) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
				{
					return PropertyGetters.GetObject(trigObject, xmlValue, arglist[1], out ptype);
				}

				return xmlValue.Value;
			}

			/// <summary>
			///     Setter function (can use vars like THIS().xmlints.test can be accessed with XMLINTS(THIS(), xmlstring.varName) with this function, which you can't do with with THIS().xmlints._____ alone
			/// </summary>
			/// <param name="trigObject"></param>
			/// <param name="o"></param>
			/// <param name="name"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public static void XMLINTS(TriggerObject trigObject, IEntity o, string name, object value)
			{
				if (!(o is Mobile || o is Item))
				{
					throw new UberScriptException("Can't set xmlints on anything but Mobile or Item");
				}

				// check for existing xmlValue attachment or create a new one
				var arglist = name.Split('.');
				XmlValue xmlValue = XmlAttach.GetValueAttachment(o, arglist[0]);

				if (xmlValue == null)
				{
					// arglist should only have [xmlints, name], nothing more
					if (arglist.Length > 1)
					{
						throw new UberScriptException("Attempted to set property on a not-yet-existant attachment!:" + name);
					}

					if (value == null)
					{
						return; // don't add anything
					}

					xmlValue = new XmlValue(arglist[0], ((int)value));

					XmlAttach.AttachTo(null, o, xmlValue);
				}
				else if (arglist.Length == 1)
				{
					if (value == null)
					{
						xmlValue.DoDelete = true;
					}
					else
					{
						xmlValue.Value = (int)value;
					}
				}
				else if (arglist.Length > 1)
				{
					// could be setting a property on an existing XmlAttachment!
					// e.g. xmlints.test.expiration = 0:0:1
					PropertySetters.SetPropertyValue(trigObject, xmlValue, arglist[1], value);
				}
			}

			public static object XMLSTRINGS(TriggerObject trigObject, IEntity o, string varName)
			{
				if (!(o is Mobile || o is Item))
				{
					throw new UberScriptException("Can't set xmlstrings on anything but Mobile or Item");
				}

				Type ptype;
				var arglist = varName.Split('.');
				XmlLocalVariable xmlLocalVariable = XmlAttach.GetStringAttachment(o, arglist[0]);

				if (xmlLocalVariable == null)
				{
					return null; // throw new UberScriptException("Could not find XmlLocalVariable named " + name + " on " + o);
				}

				if (arglist.Length > 1) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
				{
					return PropertyGetters.GetObject(trigObject, xmlLocalVariable, arglist[1], out ptype);
				}

				return xmlLocalVariable.Data;
			}

			/// <summary>
			///     Setter function (can use vars like THIS().xmlstrings.test can be accessed with XMLSTRINGS(THIS(), xmlstring.varName) with this function, which you can't do with with THIS().xmlstrings._____ alone
			/// </summary>
			/// <param name="trigObject"></param>
			/// <param name="o"></param>
			/// <param name="name"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public static void XMLSTRINGS(TriggerObject trigObject, IEntity o, string name, object value)
			{
				if (!(o is Mobile || o is Item))
				{
					throw new UberScriptException("Can't set xmlstrings on anything but Mobile or Item");
				}

				var arglist = name.Split('.');
				XmlLocalVariable xmlLocalVariable = XmlAttach.GetStringAttachment(o, arglist[0]);

				if (xmlLocalVariable == null)
				{
					// arglist should only have [xmlints, name], nothing more
					if (arglist.Length > 1)
					{
						throw new UberScriptException("Attempted to set property on a not-yet-existant attachment!:" + name);
					}

					if (value == null)
					{
						return; // don't add anything
					}

					xmlLocalVariable = new XmlLocalVariable(arglist[0], value.ToString());

					XmlAttach.AttachTo(null, o, xmlLocalVariable);
				}
				else if (arglist.Length == 1)
				{
					if (value == null)
					{
						xmlLocalVariable.DoDelete = true;
					}
					else
					{
						xmlLocalVariable.Data = value.ToString();
					}
				}
				else if (arglist.Length > 1)
				{
					// could be setting a property on an existing XmlAttachment!
					// e.g. xmlints.test.expiration = 0:0:1
					PropertySetters.SetPropertyValue(trigObject, xmlLocalVariable, arglist[1], value);
				}
			}

			public static object XMLDOUBLES(TriggerObject trigObject, IEntity o, string varName)
			{
				if (!(o is Mobile || o is Item))
				{
					throw new UberScriptException("Can't set xmldoubles on anything but Mobile or Item");
				}

				Type ptype;
				var arglist = varName.Split('.');
				XmlDouble xmlDouble = XmlAttach.GetDoubleAttachment(o, arglist[0]);

				if (xmlDouble == null)
				{
					return null; //  throw new UberScriptException("Could not find XmlDouble named " + name + " on " + o);
				}

				if (arglist.Length > 1) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
				{
					return PropertyGetters.GetObject(trigObject, xmlDouble, arglist[1], out ptype);
				}

				return xmlDouble.Value;
			}

			/// <summary>
			///     Setter function (can use vars like THIS().xmldoubles.test can be accessed with XMLDOUBLES(THIS(), xmldoubles.varName) with this function, which you can't do with with THIS().xmldoubles._____ alone
			/// </summary>
			/// <param name="trigObject"></param>
			/// <param name="o"></param>
			/// <param name="name"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public static void XMLDOUBLES(TriggerObject trigObject, IEntity o, string name, object value)
			{
				if (!(o is Mobile || o is Item))
				{
					throw new UberScriptException("Can't set xmlstrings on anything but Mobile or Item");
				}

				var arglist = name.Split('.');
				XmlDouble xmlDouble = XmlAttach.GetDoubleAttachment(o, arglist[0]);

				if (xmlDouble == null)
				{
					// arglist should only have [xmlints, name], nothing more
					if (arglist.Length > 1)
					{
						throw new UberScriptException("Attempted to set property on a not-yet-existant attachment!:" + name);
					}

					if (value == null)
					{
						return; // don't add anything
					}

					xmlDouble = new XmlDouble(arglist[0], (double)value);

					XmlAttach.AttachTo(null, o, xmlDouble);
				}
				else if (arglist.Length == 1)
				{
					if (value == null)
					{
						xmlDouble.DoDelete = true;
					}
					else
					{
						xmlDouble.Value = (double)value;
					}
				}
				else if (arglist.Length > 1)
				{
					// could be setting a property on an existing XmlAttachment!
					// e.g. xmlints.test.expiration = 0:0:1
					PropertySetters.SetPropertyValue(trigObject, xmlDouble, arglist[1], value);
				}
			}

			public static object XMLOBJS(TriggerObject trigObject, IEntity o, string varName)
			{
				if (!(o is Mobile || o is Item))
				{
					throw new UberScriptException("Can't set xmlobjs on anything but Mobile or Item");
				}

				Type ptype;
				var arglist = varName.Split('.');
				XmlObject xmlObject = XmlAttach.GetObjectAttachment(o, arglist[0]);

				if (xmlObject == null)
				{
					return null; // throw new UberScriptException("Could not find XmlObject named " + name + " on " + o);
				}

				//if (arglist.Length > 1) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
				//    return GetObject(trigObject, xmlObject, arglist[2], out ptype);

				//=====
				if (arglist.Length > 1)
				{
					// XmlObject only contains a few properties that
					// can be accessed through statements like THIS().xmlobjs.test._____
					// since there is a potential conflict between the developer wanting access
					// to the properties on the object contained in the XmlObject.Value (most likely)
					// or the properties on the XmlObject itself (far less likely)
					string testPropName = arglist[1].ToLower();

					// to access properties on the xmlobject itself (e.g. expiration), one must do this:
					//  THIS().xmlobjs.test.xmlobject.expiration
					if (testPropName == "xmlobject")
					{
						if (arglist.Length < 3)
						{
							return xmlObject;
						}

						string propLookup = arglist[2]; // add this first so later additions all prepended with '.'

						for (int i = 3; i < arglist.Length; i++)
						{
							propLookup += "." + arglist[i];
						}

						return PropertyGetters.GetObject(trigObject, xmlObject, propLookup, out ptype);
					}
					else
					{
						if (xmlObject.Value == null)
						{
							return null;
						}

						string propLookup = arglist[1]; // add this first so later additions all prepended with '.'

						for (int i = 2; i < arglist.Length; i++)
						{
							propLookup += "." + arglist[i];
						}

						return PropertyGetters.GetObject(trigObject, xmlObject.Value, propLookup, out ptype);
					}
				}
				//====

				return xmlObject.Value;
			}

			/// <summary>
			///     Setter function (can use vars like THIS().xmlobjs.test can be accessed with XMLOBJS(THIS(), xmlobjs.varName) with this function, which you can't do with with THIS().xmlobjs._____ alone
			/// </summary>
			/// <param name="trigObject"></param>
			/// <param name="o"></param>
			/// <param name="name"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public static void XMLOBJS(TriggerObject trigObject, IEntity o, string name, object value)
			{
				if (!(o is Mobile || o is Item))
				{
					throw new UberScriptException("Can't set xmlobjs on anything but Mobile or Item");
				}

				var arglist = name.Split('.');
				XmlObject xmlObject = XmlAttach.GetObjectAttachment(o, arglist[0]);

				if (xmlObject == null)
				{
					// arglist should only have [xmlints, name], nothing more
					if (arglist.Length > 1)
					{
						throw new UberScriptException("Attempted to set property on a not-yet-existant attachment!:" + name);
					}

					xmlObject = new XmlObject(arglist[0], value);

					XmlAttach.AttachTo(null, o, xmlObject);
				}
				else if (arglist.Length == 1)
				{
					xmlObject.Value = value;
				}
				else if (arglist.Length > 1)
				{
					// XmlObject only contains a few properties that
					// can be accessed through statements like THIS().xmlobjs.test._____
					// since there is a potential conflict between the developer wanting access
					// to the properties on the object contained in the XmlObject.Value (most likely)
					// or the properties on the XmlObject itself (far less likely)
					string testPropName = arglist[1].ToLower();

					// to access properties on the xmlobject itself (e.g. expiration), one must do this:
					//  THIS().xmlobjs.test.xmlobject.expiration
					if (testPropName == "xmlobject")
					{
						if (arglist.Length < 3)
						{
							throw new UberScriptException("Can't set an xmlobject directly, use ATTACH function!");
						}

						string propLookup = arglist[2]; // add this first so later additions all prepended with '.'

						for (int i = 3; i < arglist.Length; i++)
						{
							propLookup += "." + arglist[i];
						}

						PropertySetters.SetPropertyValue(trigObject, xmlObject, propLookup, value);
					}
					else
					{
						string propLookup = arglist[1]; // add this first so later additions all prepended with '.'

						for (int i = 2; i < arglist.Length; i++)
						{
							propLookup += "." + arglist[i];
						}

						PropertySetters.SetPropertyValue(trigObject, xmlObject.Value, propLookup, value);
					}
				}
			}

			public static object OBJS(TriggerObject trigObject, string varName)
			{
				if (varName == null)
				{
					return null;
				}

				object output;

				trigObject.objs.TryGetValue(varName, out output);

				return output;
			}

			public static void OBJS(TriggerObject trigObject, string varName, object toStore)
			{
				if (varName != null)
				{
					trigObject.objs[varName] = toStore;
				}
			}

			public static object INTS(TriggerObject trigObject, string varName)
			{
				return varName == null ? null : trigObject.ints.ContainsKey(varName) ? (object)trigObject.ints[varName] : null;
			}

			public static void INTS(TriggerObject trigObject, string varName, object toStore)
			{
				if (varName == null)
				{
					return;
				}

				if (toStore is int)
				{
					trigObject.ints[varName] = (int)toStore;
				}
				else
				{
					throw new UberScriptException(
						"INTS error: " + toStore + " was not a double! Cannot store it as an int with varName " + varName + "!");
				}
			}

			public static object STRINGS(TriggerObject trigObject, string varName)
			{
				return varName == null ? null : (trigObject.strings.ContainsKey(varName) ? trigObject.strings[varName] : null);
			}

			public static void STRINGS(TriggerObject trigObject, string varName, object toStore)
			{
				if (varName == null)
				{
					return;
				}

				if (toStore == null)
				{
					trigObject.strings[varName] = null;
				}
				else
				{
					trigObject.strings[varName] = toStore.ToString();
				}
			}

			public static object DOUBLES(TriggerObject trigObject, string varName)
			{
				return varName == null
						   ? null
						   : (trigObject.doubles.ContainsKey(varName) ? (object)trigObject.doubles[varName] : null);
			}

			public static void DOUBLES(TriggerObject trigObject, string varName, object toStore)
			{
				if (varName != null && toStore is double)
				{
					trigObject.doubles[varName] = (double)toStore;
				}
				else
				{
					throw new UberScriptException(
						"DOUBLES error: " + toStore + " was not a double! Cannot store it as a double with varName " + varName + "!");
				}
			}

			public static void SENDGUMP(TriggerObject trigObject, Mobile to, string gumpFileName)
			{
				if (to != null && gumpFileName != null)
				{
					SENDGUMP(trigObject, to, gumpFileName, true);
				}
			}

			public static void SENDGUMP(TriggerObject trigObject, Mobile to, string gumpFileName, bool closeOpenedUberGumps)
			{
				if (to != null && gumpFileName != null)
				{
					ParsedGumps.SendGump(trigObject, to, gumpFileName, closeOpenedUberGumps);
				}
			}

			public static void CLOSEGUMP(TriggerObject trigObject, Mobile to)
			{
				if (to != null)
				{
					to.CloseGump(typeof(UberScriptGump));
				}
			}

			public static bool CANBEHARMFUL(TriggerObject trigObject, Mobile from, Mobile to)
			{
				return from != null && from.CanBeHarmful(to);
			}

			public static bool CANBEBENEFICIAL(TriggerObject trigObject, Mobile from, Mobile to)
			{
				return from != null && from.CanBeBeneficial(to);
			}

			public static bool ISDIRECTDAMAGESPELL(TriggerObject trigObject, Spell spell)
			{
				if (spell == null)
				{
					return false;
				}

				return spell is MagicArrowSpell || spell is HarmSpell || spell is FireballSpell || spell is LightningSpell ||
					   spell is MindBlastSpell || spell is EnergyBoltSpell || spell is ExplosionSpell || spell is FlameStrikeSpell ||
					   spell is MeteorSwarmSpell || spell is ChainLightningSpell;
			}

			public static bool ISOFFENSIVESPELL(TriggerObject trigObject, Spell spell)
			{
				if (spell == null)
				{
					return false;
				}

				return spell is MagicArrowSpell || spell is ClumsySpell || spell is FeeblemindSpell || spell is WeakenSpell ||
					   spell is HarmSpell || spell is FireballSpell || spell is PoisonSpell || spell is LightningSpell ||
					   spell is ManaDrainSpell || spell is MindBlastSpell || spell is ParalyzeSpell || spell is EnergyBoltSpell ||
					   spell is ExplosionSpell || spell is MassCurseSpell || spell is FlameStrikeSpell || spell is MeteorSwarmSpell ||
					   spell is ChainLightningSpell;
			}

			public static ArrayList TILE(TriggerObject trigObject, IPoint3D caller, string spawnString, int range)
			{
				if (spawnString == null)
				{
					return new ArrayList();
				}

				ArrayList output = new ArrayList();
				Item thisItem = caller as Item;

				if (thisItem != null)
				{
					if (thisItem.RootParentEntity != null)
					{
						caller = thisItem.RootParentEntity;
					}
				}

				for (int x = caller.X - range; x <= caller.X + range; x++)
				{
					for (int y = caller.Y - range; y <= caller.Y + range; y++)
					{
						int z = GETAVERAGEZ(trigObject, new Point2D(x, y));
						bool fit = CANFITMOB(trigObject, x, y, z, 16, false, true, true, null);

						if (fit)
						{
							output.Add(SpawnHandlers.Spawn(spawnString, new Point3D(new Point2D(x, y), z)));
						}
					}
				}

				return output;
			}

			public static void DELETEACCOUNTTAGS(TriggerObject trigObject, string tagName)
			{
				if (String.IsNullOrWhiteSpace(tagName))
				{
					return;
				}

				foreach (Account account in Accounts.GetAccounts().OfType<Account>())
				{
					account.RemoveTag(tagName);
				}
			}

			public static string RANDOMNAME(TriggerObject trigObject, string nameType)
			{
				if (String.IsNullOrWhiteSpace(nameType))
				{
					return String.Empty;
				}

				string output = NameList.RandomName(nameType);

				if (String.IsNullOrWhiteSpace(output))
				{
					throw new UberScriptException(
						"RANDOMNAME nameType " + nameType + " not found. examples are 'male', 'female', 'orc', 'balron'");
				}

				return output;
			}

			public static bool ISINCOGNITO(TriggerObject trigObject, Mobile mob)
			{
				return mob != null && !mob.CanBeginAction(typeof(IncognitoSpell)) || DisguiseTimers.IsDisguised(mob);
			}

			public static bool ISPOLYMORPHED(TriggerObject trigObject, Mobile mob)
			{
				return mob != null && !mob.CanBeginAction(typeof(PolymorphSpell));
			}

			public static Map MAP(TriggerObject trigObject, string mapName)
			{
				if (!String.IsNullOrWhiteSpace(mapName))
				{
					foreach (Map map in Map.AllMaps.Where(map => Insensitive.Equals(map.Name, mapName)))
					{
						return map;
					}
				}

				throw new UberScriptException("Map " + mapName + " not recognized!");
			}

			public static Point3D GETRANDOMDUNGEONLOCATION(TriggerObject trigObject)
			{
				Rectangle2D rect = RandomLocations.GetRandomRectangle();

				return GETVALIDSPAWNLOCATION(trigObject, rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height, 0, true);
			}

			public static void SAYTO(TriggerObject trigObject, Mobile from, Mobile to, object said)
			{
				if (from == null || to == null)
				{
					return;
				}

				if (said == null)
				{
					said = "null";
				}

				from.SayTo(to, said.ToString());
			}

			public static void ANIMATE(
				TriggerObject trigObject,
				Mobile mob,
				int action,
				int frameCount,
				int repeatCount,
				bool forward,
				bool repeat,
				int delay)
			{
				if (mob != null && !mob.Deleted)
				{
					mob.Animate(action, frameCount, repeatCount, forward, repeat, delay);
				}
			}

			public static PlayerMobile GETPOSSESSOR(TriggerObject trigObject, Mobile mob)
			{
				return mob != null && mob.NetState != null ? CreaturePossession.GetAssociatedPlayerMobile(mob) : null;
			}

			/// <summary>
			///     This always returns a DOUBLE if a number can be found (you may need to cast it as something else if needed)
			/// </summary>
			/// <param name="trigObject"></param>
			/// <param name="input"></param>
			/// <returns></returns>
			public static object STRINGTONUMBER(TriggerObject trigObject, string input)
			{
				var args = input.Split();

				foreach (string t in args)
				{
					double d;

					if (Double.TryParse(t, out d))
					{
						return d;
					}
				}

				return null;
			}

			public static bool CANSEE(TriggerObject trigObject, Mobile from, object to)
			{
				return from != null && from.CanSee(to);
			}

			public static bool CHECKBSPELLSEQUENCE(TriggerObject trigObject, Spell spell, Mobile target)
			{
				return spell != null && spell.CheckBSequence(target);
			}

			public static bool CHECKBSPELLSEQUENCE(TriggerObject trigObject, Spell spell, Mobile target, bool allowDead)
			{
				return spell != null && spell.CheckBSequence(target, allowDead);
			}

			public static bool CHECKSPELLSEQUENCE(TriggerObject trigObject, Spell spell)
			{
				return spell != null && spell.CheckSequence();
			}

			public static ArrayList SORTINTSLIST(TriggerObject trigObject, ArrayList list, bool descendingOrder)
			{
				ArrayList output = new ArrayList(20);

				if (descendingOrder)
				{
					foreach (XmlValue xmlValue in list)
					{
						bool added = false;

						for (int i = 0; i < output.Count; i++)
						{
							if (xmlValue.Value <= ((XmlValue)output[i]).Value)
							{
								continue;
							}

							output.Insert(i, xmlValue);
							added = true;
							break;
						}

						if (added)
						{
							continue;
						}

						output.Add(xmlValue);
					}
				}
				else // sort in ascending order
				{
					foreach (XmlValue xmlValue in list)
					{
						bool added = false;

						for (int i = 0; i < output.Count; i++)
						{
							if (xmlValue.Value >= ((XmlValue)output[i]).Value)
							{
								continue;
							}

							output.Insert(i, xmlValue);
							added = true;
							break;
						}

						if (added)
						{
							continue;
						}

						output.Add(xmlValue);
					}
				}

				return output;
			}

			public static ArrayList SORTDOUBLESLIST(TriggerObject trigObject, ArrayList list, bool descendingOrder)
			{
				ArrayList output = new ArrayList(list.Count);

				if (descendingOrder)
				{
					foreach (XmlDouble xmlDouble in list)
					{
						bool added = false;

						for (int i = 0; i < output.Count; i++)
						{
							if (xmlDouble.Value <= ((XmlDouble)output[i]).Value)
							{
								continue;
							}

							output.Insert(i, xmlDouble);
							added = true;
							break;
						}

						if (added)
						{
							continue;
						}

						output.Add(xmlDouble);
					}
				}
				else // sort in ascending order
				{
					foreach (XmlDouble xmlDouble in list)
					{
						bool added = false;

						for (int i = 0; i < output.Count; i++)
						{
							if (xmlDouble.Value >= ((XmlDouble)output[i]).Value)
							{
								continue;
							}

							output.Insert(i, xmlDouble);
							added = true;
							break;
						}

						if (added)
						{
							continue;
						}

						output.Add(xmlDouble);
					}
				}

				return output;
			}

			public static int GETATTACHMENTDOUBLECOUNT(TriggerObject trigObject, string name)
			{
				return name == null ? 0 : GETATTACHMENTDOUBLECOUNT(trigObject, name, false);
			}

			public static int GETATTACHMENTDOUBLECOUNT(TriggerObject trigObject, string name, bool playermobileOnly)
			{
				return name == null ? 0 : GLOBALGETATTACHMENTS(trigObject, "xmldouble", name, playermobileOnly).Count;
			}

			public static double GETATTACHMENTDOUBLEMAX(TriggerObject trigObject, string name)
			{
				return name == null ? 0.0 : GETATTACHMENTDOUBLEMAX(trigObject, name, false);
			}

			public static double GETATTACHMENTDOUBLEMAX(TriggerObject trigObject, string name, bool playermobileOnly)
			{
				if (name == null)
				{
					return 0.0;
				}

				ArrayList xmlDoubles = GLOBALGETATTACHMENTS(trigObject, "xmldouble", name, playermobileOnly);

				return xmlDoubles.Cast<XmlDouble>().Max(xmlDouble => xmlDouble.Value);
			}

			public static double GETATTACHMENTDOUBLESUM(TriggerObject trigObject, string name)
			{
				return name == null ? 0.0 : GETATTACHMENTDOUBLESUM(trigObject, name, false);
			}

			public static double GETATTACHMENTDOUBLESUM(TriggerObject trigObject, string name, bool playermobileOnly)
			{
				if (name == null)
				{
					return 0.0;
				}

				ArrayList xmlDoubles = GLOBALGETATTACHMENTS(trigObject, "xmldouble", name, playermobileOnly);

				return xmlDoubles.Cast<XmlDouble>().Sum(xmlDouble => xmlDouble.Value);
			}

			public static double GETATTACHMENTINTCOUNT(TriggerObject trigObject, string name)
			{
				return name == null ? 0 : GETATTACHMENTINTCOUNT(trigObject, name, false);
			}

			public static double GETATTACHMENTINTCOUNT(TriggerObject trigObject, string name, bool playermobileOnly)
			{
				return name == null ? 0 : GLOBALGETATTACHMENTS(trigObject, "xmlvalue", name, playermobileOnly).Count;
			}

			public static int GETATTACHMENTINTMAX(TriggerObject trigObject, string name)
			{
				return name == null ? 0 : GETATTACHMENTINTMAX(trigObject, name, false);
			}

			public static int GETATTACHMENTINTMAX(TriggerObject trigObject, string name, bool playermobileOnly)
			{
				if (name == null)
				{
					return 0;
				}

				ArrayList xmlValues = GLOBALGETATTACHMENTS(trigObject, "xmlvalue", name, playermobileOnly);

				return xmlValues.Cast<XmlValue>().Max(xmlValue => xmlValue.Value);
			}

			public static int GETATTACHMENTINTSUM(TriggerObject trigObject, string name)
			{
				return GETATTACHMENTINTSUM(trigObject, name, false);
			}

			public static int GETATTACHMENTINTSUM(TriggerObject trigObject, string name, bool playermobileOnly)
			{
				if (name == null)
				{
					return 0;
				}

				ArrayList xmlValues = GLOBALGETATTACHMENTS(trigObject, "xmlvalue", name, playermobileOnly);

				return xmlValues.Cast<XmlValue>().Sum(xmlValue => xmlValue.Value);
			}

			public static void GLOBALDELATTACHMENTS(TriggerObject trigObject, string attachmenttype, string name)
			{
				if (attachmenttype != null && name != null)
				{
					GLOBALDELATTACHMENTS(trigObject, attachmenttype, name, false);
				}
			}

			public static void GLOBALDELATTACHMENTS(
				TriggerObject trigObject, string attachmenttype, string name, bool playermobileOnly)
			{
				if (attachmenttype == null || name == null)
				{
					return;
				}

				ArrayList toDelete = GLOBALGETATTACHMENTS(trigObject, attachmenttype, name, playermobileOnly);

				foreach (XmlAttachment attachment in toDelete)
				{
					attachment.Delete();
				}
			}

			public static ArrayList GLOBALGETATTACHMENTS(TriggerObject trigObject, string attachmenttype, string name)
			{
				return attachmenttype != null && name != null
						   ? GLOBALGETATTACHMENTS(trigObject, attachmenttype, name, false)
						   : new ArrayList();
			}

			public static ArrayList GLOBALGETATTACHMENTS(
				TriggerObject trigObject, string attachmenttype, string name, bool playermobileOnly)
			{
				if (attachmenttype == null || name == null)
				{
					return new ArrayList();
				}

				ArrayList output = new ArrayList();
				//Type type = SpawnerType.GetType(name);
				// use strings--a bit better performance
				Type type = ScriptCompiler.FindTypeByName(attachmenttype);

				if (type == null)
				{
					return output;
				}

				// MIGHT BE MORE OPTIMAL TO USE XmlAttach.AllAttachments?
				foreach (XmlAttachment a in
					XmlAttach.AllAttachments.Select(pair => pair.Value).Where(a => a.GetType() == type && !a.Deleted && a.Name == name)
					)
				{
					if (playermobileOnly)
					{
						if (a.AttachedTo is PlayerMobile)
						{
							output.Add(a);
						}
					}
					else
					{
						output.Add(a);
					}
				}

				/*
				foreach ( Mobile mob in World.Mobiles.Values )
				{                
					if (playermobileOnly && !(mob is PlayerMobile))
					{
						continue;
					}
					ArrayList alist = XmlAttach.FindAttachments(mob);
					if (alist != null)
					{
						foreach (XmlAttachment a in alist)
						{
							if (a.GetType() == type && !a.Deleted && a.Name == name)
							{
								output.Add(a);
							}
						}
					}
				}
				 */
				return output;
			}

			public static int GETAMOUNTINCONTAINER(TriggerObject trigObject, Container container, object typeObjectOrString)
			{
				if (container == null || container.Deleted)
				{
					return 0;
				}

				Type type = TYPE(trigObject, typeObjectOrString);

				if (type == null)
				{
					throw new UberScriptException(
						"GETAMOUNTINCONTAINER typeString " + typeObjectOrString + " did not match any available types!");
				}

				return container.FindItemsByType(type).Sum(t => t.Amount);
			}

			public static void DELETEFROMCONTAINER(TriggerObject trigObject, Container container, object typeObjectOrString)
			{
				DELETEFROMCONTAINER(trigObject, container, typeObjectOrString, 1);
			}

			public static void DELETEFROMCONTAINER(
				TriggerObject trigObject, Container container, object typeObjectOrString, int amount)
			{
				Type type = TYPE(trigObject, typeObjectOrString);

				if (type == null)
				{
					throw new UberScriptException(
						"DELETEFROMCONTAINER typeObjectOrString " + typeObjectOrString + " did not match any available types!");
				}

				var list = container.FindItemsByType(type);

				for (int i = 0; amount > 0 && i < list.Length; i++)
				{
					int consume = Math.Min(list[i].Amount, amount);

					list[i].Consume(consume);
					amount -= consume;
				}
			}

			public static bool TAKEGOLDFROM(TriggerObject trigObject, Mobile mob, int amount)
			{
				return mob != null && !mob.Deleted &&
					   Banker.WithdrawPackAndBank(mob, typeof(Gold), amount);
			}

			public static int GOLDINBANK(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null || mob.Deleted || mob.BankBox == null)
				{
					return 0;
				}

				int gold = GOLDINCONTAINER(trigObject, mob.BankBox);

				if (mob.BankBox.Expansion != Expansion.T2A)
				{
					gold += (int)mob.BankBox.Credit;
				}

				return gold;
			}

			public static int GOLDINCONTAINER(TriggerObject trigObject, Container c)
			{
				if (c == null || c.Deleted)
				{
					return 0;
				}

				int currency = 0;

				foreach (Item item in c.Items)
				{
					if (item is Container)
					{
						currency += GOLDINCONTAINER(trigObject, (Container)item);
					}
					else if (item is Gold)
					{
						currency += item.Amount;
					}
					else if (item is BankCheck && ((BankCheck)item).TypeOfCurrency.IsEqualOrChildOf<Gold>())
					{
						currency += ((BankCheck)item).Worth;
					}
				}

				return currency;
			}

			public static bool TAKESILVERFROM(TriggerObject trigObject, Mobile mob, int amount)
			{
				return mob != null && !mob.Deleted &&
					   Banker.WithdrawPackAndBank(mob, typeof(Silver), amount);
			}

			public static int SILVERINBANK(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null || mob.Deleted || mob.BankBox == null)
				{
					return 0;
				}

				int silver = SILVERINCONTAINER(trigObject, mob.BankBox);

				if (mob.BankBox.Expansion == Expansion.T2A)
				{
					silver += (int)mob.BankBox.Credit;
				}

				return silver;
			}

			public static int SILVERINCONTAINER(TriggerObject trigObject, Container c)
			{
				if (c == null || c.Deleted)
				{
					return 0;
				}

				int currency = 0;

				foreach (Item item in c.Items)
				{
					if (item is Container)
					{
						currency += SILVERINCONTAINER(trigObject, (Container)item);
					}
					else if (item is Silver)
					{
						currency += item.Amount;
					}
					else if (item is BankCheck && ((BankCheck)item).TypeOfCurrency.IsEqualOrChildOf<Silver>())
					{
						currency += ((BankCheck)item).Worth;
					}
				}

				return currency;
			}

			public static bool LISTCONTAINS(TriggerObject trigObject, ArrayList list, object obj)
			{
				if (list == null || list.Count == 0 || obj == null)
				{
					return false;
				}

				foreach (object element in list)
				{
					if (element == obj)
					{
						return true;
					}

					if (element != null && element.Equals(obj))
					{
						return true;
					}
				}

				return false;
			}

			public static void CLEARLIST(TriggerObject trigObject, ArrayList list)
			{
				if (list != null)
				{
					list.Clear();
				}
			}

			public static DateTime NOW(TriggerObject trigObject)
			{
				return DateTime.Now;
			}

			public static DateTime UTCNOW(TriggerObject trigObject)
			{
				return DateTime.UtcNow;
			}

			public static int GETYEAR(TriggerObject trigObject, DateTime dateTime)
			{
				return dateTime.Year;
			}

			public static int GETMONTH(TriggerObject trigObject, DateTime dateTime)
			{
				return dateTime.Month;
			}

			public static int GETDAY(TriggerObject trigObject, DateTime dateTime)
			{
				return dateTime.Day;
			}

			public static string GETDAYOFWEEK(TriggerObject trigObject, DateTime dateTime)
			{
				return dateTime.DayOfWeek.ToString();
			}

			public static int GETHOUR(TriggerObject trigObject, DateTime dateTime)
			{
				return dateTime.Hour;
			}

			public static int GETMINUTE(TriggerObject trigObject, DateTime dateTime)
			{
				return dateTime.Minute;
			}

			public static int GETSECOND(TriggerObject trigObject, DateTime dateTime)
			{
				return dateTime.Second;
			}

			public static int GETMILLISECOND(TriggerObject trigObject, DateTime dateTime)
			{
				return dateTime.Millisecond;
			}

			public static TimeSpan TIMESPANMILLISECONDS(TriggerObject trigObject, double milliseconds)
			{
				return TimeSpan.FromMilliseconds(milliseconds);
			}

			public static TimeSpan TIMESPANSECONDS(TriggerObject trigObject, double seconds)
			{
				return TimeSpan.FromSeconds(seconds);
			}

			public static TimeSpan TIMESPANMINUTES(TriggerObject trigObject, double minutes)
			{
				return TimeSpan.FromMinutes(minutes);
			}

			public static TimeSpan TIMESPANHOURS(TriggerObject trigObject, double hours)
			{
				return TimeSpan.FromHours(hours);
			}

			public static TimeSpan TIMESPANDAYS(TriggerObject trigObject, double days)
			{
				return TimeSpan.FromDays(days);
			}

			public static bool SAID(TriggerObject trigObject, string test)
			{
				return trigObject.Speech != null && !String.IsNullOrWhiteSpace(test) &&
					   Insensitive.Contains(trigObject.Speech, test);
			}

			public static bool SAID(TriggerObject trigObject, ArrayList testWords)
			{
				return testWords != null && testWords.Count > 0 && testWords.OfType<string>().Any(test => SAID(trigObject, test));
			}

			public static bool INRECTANGLE(TriggerObject trigObject, IPoint2D testObject, Rectangle2D rectangle)
			{
				return testObject != null && rectangle.Contains(testObject);
			}

			public static bool INRECTANGLE(TriggerObject trigObject, IPoint2D testObject, IPoint2D topLeft, IPoint2D bottomRight)
			{
				return testObject != null && topLeft != null && bottomRight != null && testObject.X >= topLeft.X &&
					   testObject.X <= bottomRight.X && testObject.Y >= topLeft.Y && testObject.Y <= bottomRight.Y;
			}

			public static bool INRECTANGLE(
				TriggerObject trigObject, IPoint2D testObject, IPoint2D topLeft, int width, int height)
			{
				return testObject != null && topLeft != null && testObject.X >= topLeft.X && testObject.X <= topLeft.X + width &&
					   testObject.Y >= topLeft.Y && testObject.Y <= topLeft.Y + height;
			}

			public static void COMMAND(TriggerObject trigObject, Mobile gameMaster, string command)
			{
				if (gameMaster == null || gameMaster.AccessLevel < AccessLevel.GameMaster)
				{
					throw new Exception("COMMAND ERROR: You must provide a Mobile with AccessLevel >= GameMaster");
				}

				CommandSystem.Handle(gameMaster, String.Format("{0}{1}", CommandSystem.Prefix, command));
			}

			public static void LOG(TriggerObject trigObject, string fileName, string toLog)
			{
				if (String.IsNullOrWhiteSpace(fileName))
				{
					throw new UberScriptException("LOG error: no filename given!");
				}

				if (toLog == null)
				{
					toLog = "null";
				}

				if (fileName.EndsWith(".txt"))
				{
					LoggingCustom.LogUberScript(fileName, toLog);
				}
				else
				{
					LoggingCustom.LogUberScript(fileName + ".txt", toLog);
				}
			}

			public static void SETSKILL(TriggerObject trigObject, Mobile mob, string skillString, double value)
			{
				if (mob == null)
				{
					throw new UberScriptException("SETSKILL skill mob cannot be null!");
				}

				SkillName sk;

				if (!Enum.TryParse(skillString, true, out sk))
				{
					throw new UberScriptException("SETSKILL skill " + skillString + " is not a valid skill!");
				}

				mob.Skills[sk].Base = value;
			}

			public static double GETSKILLBASE(TriggerObject trigObject, Mobile mob, string skillString)
			{
				if (mob == null)
				{
					throw new UberScriptException("GETSKILLBASE skill mob cannot be null!");
				}

				SkillName sk;

                if (!Enum.TryParse(skillString, true, out sk))
				{
					throw new UberScriptException("GETSKILLBASE skill " + skillString + " is not a valid skill!");
				}

				return mob.Skills[sk].Base;
			}

			public static double GETSKILL(TriggerObject trigObject, Mobile mob, string skillString)
			{
				if (mob == null)
				{
					throw new UberScriptException("GETSKILL skill mob cannot be null!");
				}

				SkillName sk;

                if (!Enum.TryParse(skillString, true, out sk))
				{
					throw new UberScriptException("GETSKILL skill " + skillString + " is not a valid skill!");
				}

				return mob.Skills[sk].Value;
			}

			public static void APPLYSKILLS(TriggerObject trigObject, Mobile mob, ArrayList skills)
			{
				if (mob == null || skills == null || skills.Count == 0)
				{
					return;
				}

				for (int i = 0; i < mob.Skills.Length; i++)
				{
					mob.Skills[i].Base = (double)skills[i];
				}
			}

			public static ArrayList COPYSKILLS(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null)
				{
					return new ArrayList();
				}

				var info = SkillInfo.Table;

				ArrayList skills = new ArrayList(info.Length);

				for (int i = 0; i < mob.Skills.Length; i++)
				{
					skills.Add(mob.Skills[i].Base);
				}

				return skills;
			}

			public static ArrayList ONLINEMOBS(TriggerObject trigObject)
			{
				ArrayList output = new ArrayList(NetState.Instances.Count);

				foreach (NetState instance in NetState.Instances.Where(instance => instance.Mobile != null))
				{
					output.Add(instance.Mobile);
				}

				return output;
			}

			public static object SPAWN(TriggerObject trigObject, IPoint3D loc, string spawnString, Map map)
			{
				if (loc == null || spawnString == null)
				{
					return null;
				}

				IEntity spawned = SpawnHandlers.Spawn(spawnString, loc) as IEntity;

				if (spawned == null)
				{
					throw new UberScriptException("SPAWN^1 spawned IEntity is null!");
				}

				spawned.MoveToWorld(new Point3D(loc.X, loc.Y, loc.Z), map);

				if (spawned is BaseCreature)
				{
					BaseCreature c = (BaseCreature)spawned;
					c.Home = c.Location;
					c.HomeMap = c.Map;
				}

				return spawned;
			}

			public static object SPAWN(TriggerObject trigObject, IPoint3D loc, string spawnString)
			{
				if (loc == null || spawnString == null)
				{
					return null;
				}

				IEntity spawned = SpawnHandlers.Spawn(spawnString, loc) as IEntity;
				Map spawnMap = Map.Felucca;

				if (loc is IEntity)
				{
					spawnMap = ((IEntity)loc).Map;
				}

				if (spawned == null)
				{
					throw new UberScriptException("SPAWN^2 spawned IEntity is null!");
				}

				spawned.MoveToWorld(new Point3D(loc.X, loc.Y, loc.Z), spawnMap);

				if (spawned is BaseCreature)
				{
					BaseCreature c = (BaseCreature)spawned;

					c.Home = c.Location;
					c.HomeMap = c.Map;
				}

				return spawned;
			}

			public static object SPAWN(TriggerObject trigObject, IPoint3D caller, string spawnString, int range)
			{
				if (spawnString == null || caller == null)
				{
					return null;
				}

				Map spawnMap = Map.Felucca;

				if (caller is IEntity)
				{
					spawnMap = ((IEntity)caller).Map;
				}

				return SPAWN(trigObject, caller, spawnString, range, spawnMap);
			}

			public static object SPAWN(TriggerObject trigObject, IPoint3D caller, string spawnString, int range, Map map)
			{
				if (spawnString == null || caller == null)
				{
					return null;
				}

				Item thisItem = caller as Item;

				if (thisItem != null && thisItem.RootParentEntity != null)
				{
					caller = thisItem.RootParentEntity;
				}

				IPoint3D point = GETVALIDSPAWNLOCATION(trigObject, caller, range);

				if (point.X == 0 && point.Y == 0 && point.Z == 0)
				{
					point = caller;
				}

				IEntity spawned = SpawnHandlers.Spawn(spawnString, point) as IEntity;

				if (spawned == null)
				{
					throw new UberScriptException("SPAWN^3 spawned IEntity is null!");
				}

				spawned.MoveToWorld(new Point3D(point.X, point.Y, point.Z), map);

				if (spawned is BaseCreature)
				{
					BaseCreature c = (BaseCreature)spawned;
					c.Home = c.Location;
					c.HomeMap = c.Map;
				}

				return spawned;
			}

			public static object SPAWN(TriggerObject trigObject, string spawnString)
			{
				return spawnString != null ? SpawnHandlers.Spawn(spawnString, trigObject.This) : null;
			}

			public static object SPAWN(TriggerObject trigObject, string spawnString, int range)
			{
				return spawnString != null ? SPAWN(trigObject, trigObject.This as IPoint3D, spawnString, range) : null;
			}

			public static bool PUTNEARBY(TriggerObject trigObject, IPoint3D target, IEntity toPut, int range)
			{
				return target != null && toPut != null && PUTNEARBY(trigObject, target, toPut, range, false, 1, false);
			}

			public static bool PUTNEARBY(TriggerObject trigObject, IPoint3D target, IEntity toPut, int range, bool dupe)
			{
				return target != null && toPut != null && PUTNEARBY(trigObject, target, toPut, range, dupe, 1, false);
			}

			public static bool PUTNEARBY(
				TriggerObject trigObject, IPoint3D target, IEntity toPut, int range, bool dupe, int amount)
			{
				return target != null && toPut != null && PUTNEARBY(trigObject, target, toPut, range, dupe, amount, false);
			}

			public static bool PUTNEARBY(
				TriggerObject trigObject, IPoint3D target, IEntity toPut, int range, bool dupe, int amount, bool dupeContents)
			{
				if (target == null || toPut == null)
				{
					return false;
				}

				Point3D point = GETVALIDSPAWNLOCATION(trigObject, target, range);

				if (point == Point3D.Zero)
				{
					return false;
				}

				Map map = (target is IEntity ? ((IEntity)target).Map : Map.Felucca);

				if (dupe)
				{
					if (toPut is BaseCreature)
					{
						for (int i = 0; i < amount; i++)
						{
							BaseCreature copy = (BaseCreature)DUPEMOB(trigObject, (BaseCreature)toPut, 1, dupeContents);

							if (copy == null)
							{
								continue;
							}

							point = GETVALIDSPAWNLOCATION(trigObject, target, range, true);

							if (point == Point3D.Zero)
							{
								point = new Point3D(target);
							}

							copy.MoveToWorld(point, map);
						}
					}
					else if (toPut is Item)
					{
						for (int i = 0; i < amount; i++)
						{
							Item copy = DUPE(trigObject, (Item)toPut, false, 1, dupeContents);

							if (copy == null)
							{
								continue;
							}

							point = GETVALIDSPAWNLOCATION(trigObject, target, range, true);

							if (point == Point3D.Zero)
							{
								point = new Point3D(target);
							}

							copy.MoveToWorld(point, map);
						}
					}
					else
					{
						throw new UberScriptException("PUTONNEARBY dupe can only be performed on Items or BaseCreature objects!");
					}
				}
				else
				{
					toPut.MoveToWorld(point, map);
				}

				return true;
			}

			public static Point3D GETVALIDSPAWNLOCATION(TriggerObject trigObject, ArrayList rectangles)
			{
				if (rectangles == null || rectangles.Count == 0)
				{
					throw new UberScriptException("GETVALIDSPAWNLOCATION expects rectangles to be equal or greater than 1 in length!");
				}

				//Rectangle2D randomRectangle = (Rectangle2D)rectangles[Utility.Random(rectangles.Count)];
				//return GETVALIDSPAWNLOCATION(trigObject, randomRectangle.Start.X, randomRectangle.Start.Y, randomRectangle.End.X, randomRectangle.End.Y, 0, true);
				ArrayList weightedProbabilities = new ArrayList(rectangles.Count);

				foreach (Rectangle2D r in rectangles.Cast<Rectangle2D>())
				{
					weightedProbabilities.Add(r.Width * r.Height);
				}

				return GETVALIDSPAWNLOCATION(trigObject, rectangles, weightedProbabilities, 0, true);
			}

			public static Point3D GETVALIDSPAWNLOCATION(
				TriggerObject trigObject, ArrayList rectangles, ArrayList weightedProbabilities)
			{
				return GETVALIDSPAWNLOCATION(trigObject, rectangles, weightedProbabilities, 0, true);
			}

			public static Point3D GETVALIDSPAWNLOCATION(
				TriggerObject trigObject,
				ArrayList rectangles,
				ArrayList weightedProbabilities,
				ArrayList zLevels,
				bool requiresSurface)
			{
				// expectes rectangles and weighted probabilities to be the same length
				if (rectangles == null || weightedProbabilities == null || rectangles.Count != weightedProbabilities.Count ||
					rectangles.Count != zLevels.Count)
				{
					throw new UberScriptException(
						"GETVALIDSPAWNLOCATION expects rectangles and weightedProbabilities to have the same length!");
				}

				int totalWeights = weightedProbabilities.Cast<int>().Sum();

				int roll = Utility.Random(totalWeights);
				int current = 0;

				for (int i = 0; i < rectangles.Count; i++)
				{
					current += (int)weightedProbabilities[i];

					if (roll >= current)
					{
						continue;
					}

					if (!(rectangles[i] is Rectangle2D))
					{
						throw new UberScriptException(
							"GETVALIDSPAWNLOCATION expected Rectangle2D objects in rectangles ArrayList but found " + rectangles[i]);
					}

					Rectangle2D randomRectangle = (Rectangle2D)rectangles[i];

					return GETVALIDSPAWNLOCATION(
						trigObject,
						randomRectangle.Start.X,
						randomRectangle.Start.Y,
						randomRectangle.End.X,
						randomRectangle.End.Y,
						(int)zLevels[i],
						requiresSurface);
				}

				throw new UberScriptException("GETVALIDSPAWNLOCATIONS with rectangles failed to return a valid rectangle!");
			}

			public static Point3D GETVALIDSPAWNLOCATION(
				TriggerObject trigObject, ArrayList rectangles, ArrayList weightedProbabilities, int zLevel, bool requiresSurface)
			{
				// expectes rectangles and weighted probabilities to be the same length
				if (rectangles.Count != weightedProbabilities.Count)
				{
					throw new UberScriptException(
						"GETVALIDSPAWNLOCATION expects rectangles and weightedProbabilities to have the same length!");
				}

				int totalWeights = weightedProbabilities.Cast<int>().Sum();
				int roll = Utility.Random(totalWeights);
				int current = 0;

				for (int i = 0; i < rectangles.Count; i++)
				{
					current += (int)weightedProbabilities[i];

					if (roll >= current)
					{
						continue;
					}

					if (!(rectangles[i] is Rectangle2D))
					{
						throw new UberScriptException(
							"GETVALIDSPAWNLOCATION expected Rectangle2D objects in rectangles ArrayList but found " + rectangles[i]);
					}

					Rectangle2D randomRectangle = (Rectangle2D)rectangles[i];

					return GETVALIDSPAWNLOCATION(
						trigObject,
						randomRectangle.Start.X,
						randomRectangle.Start.Y,
						randomRectangle.End.X,
						randomRectangle.End.Y,
						zLevel,
						requiresSurface);
				}

				throw new UberScriptException("GETVALIDSPAWNLOCATIONS with rectangles failed to return a valid rectangle!");
			}

			public static Point3D GETVALIDSPAWNLOCATION(TriggerObject trigObject, IPoint3D target, int range)
			{
				return GETVALIDSPAWNLOCATION(trigObject, target, range, false);
			}

			public static Point3D GETVALIDSPAWNLOCATION(
				TriggerObject trigObject, IPoint3D target, int range, bool requiresurface)
			{
				return GETVALIDSPAWNLOCATION(
					trigObject, target.X - range, target.Y - range, target.X + range, target.Y + range, target.Z, requiresurface);
			}

			public static Point3D GETVALIDSPAWNLOCATION(
				TriggerObject trigObject, int startX, int startY, int endX, int endY, int z, bool requiresurface)
			{
				// --- from XmlSpawner2.cs GetSpawnPosition function ---
				// try to find a valid spawn location using the z coord of the spawner
				// relax the normal surface requirement for mobiles if the flag is set
				Map map = Map.Felucca; //(target is IEntity) ? ((IEntity)target).Map : Map.Felucca;

				// try 10 times; this is a potential performance bottleneck
				for (int i = 0; i < 10; i++)
				{
					int x = Utility.RandomMinMax(startX, endX);
					int y = Utility.RandomMinMax(startY, endY);

					bool fit = requiresurface
								   ? CANFITMOB(trigObject, x, y, z, 16, false, true, true, null)
								   : CANFIT(trigObject, new Point2D(x, y));

					// if that fails then try to find a valid z coord
					if (fit)
					{
						return new Point3D(x, y, z);
					}

					z = map.GetAverageZ(x, y);

					fit = requiresurface
							  ? CANFITMOB(trigObject, x, y, z, 16, false, true, true, null)
							  : map.CanFit(x, y, z, 16, true, false, false);

					if (fit)
					{
						return new Point3D(x, y, z);
					}

					// check for a possible static surface that works
					var staticTiles = map.Tiles.GetStaticTiles(x, y, true);

					foreach (StaticTile tile in staticTiles)
					{
						ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

						//int calcTop = (tile.Z + id.CalcHeight);

						if ((id.Flags & TileFlag.Surface) == 0)
						{
							continue;
						}

						int top = tile.Z + id.Height;

						fit = requiresurface
								  ? CANFITMOB(trigObject, x, y, top, 16, false, true, true, null)
								  : map.CanFit(x, y, top, 16, true, false, false);

						if (fit)
						{
							return new Point3D(x, y, top);
						}
					}
				}

				// unable to find a valid spot in 10 tries
				return Point3D.Zero;
			}

			// from XmlSpawner2.cs CanFit: if a non-null mob argument is passed, then check the canswim and cantwalk props to determine valid placement
			public static bool CANFITMOB(
				TriggerObject trigObj,
				int x,
				int y,
				int z,
				int height,
				bool checkBlocksFit,
				bool checkMobiles,
				bool requireSurface,
				Mobile mob)
			{
				return CANFITMOB(trigObj, x, y, z, height, checkBlocksFit, checkMobiles, requireSurface, mob, Map.Felucca);
			}

			public static bool CANFITMOB(
				TriggerObject trigObj,
				int x,
				int y,
				int z,
				int height,
				bool checkBlocksFit,
				bool checkMobiles,
				bool requireSurface,
				Mobile mob,
				Map map)
			{
				if (map == null || map == Map.Internal)
				{
					return false;
				}

				if (x < 0 || y < 0 || x >= map.Width || y >= map.Height)
				{
					return false;
				}

				bool hasSurface = false;
				bool checkmob = false;
				bool canswim = false;
				bool cantwalk = false;

				if (mob != null)
				{
					checkmob = true;
					canswim = mob.CanSwim;
					cantwalk = mob.CantWalk;
				}

				LandTile lt = map.Tiles.GetLandTile(x, y);
				int lowZ = 0, avgZ = 0, topZ = 0;

				bool surface;
				bool wet;

				map.GetAverageZ(x, y, ref lowZ, ref avgZ, ref topZ);

				TileFlag landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;
				bool impassable = (landFlags & TileFlag.Impassable) != 0;

				if (checkmob)
				{
					wet = (landFlags & TileFlag.Wet) != 0;

					// dont allow wateronly creatures on land
					if (cantwalk && !wet)
					{
						impassable = true;
					}

					// allow water creatures on water
					if (canswim && wet)
					{
						impassable = false;
					}
				}

				if (impassable && avgZ > z && (z + height) > lowZ)
				{
					return false;
				}

				if (!impassable && z == avgZ && !lt.Ignored)
				{
					hasSurface = true;
				}

				var staticTiles = map.Tiles.GetStaticTiles(x, y, true);

				foreach (StaticTile t in staticTiles)
				{
					ItemData id = TileData.ItemTable[t.ID & TileData.MaxItemValue];
					surface = id.Surface;
					impassable = id.Impassable;

					if (checkmob)
					{
						wet = (id.Flags & TileFlag.Wet) != 0;

						// dont allow wateronly creatures on land
						if (cantwalk && !wet)
						{
							impassable = true;
						}

						// allow water creatures on water
						if (canswim && wet)
						{
							surface = true;
							impassable = false;
						}
					}

					if ((surface || impassable) && (t.Z + id.CalcHeight) > z && (z + height) > t.Z)
					{
						return false;
					}

					if (surface && !impassable && z == (t.Z + id.CalcHeight))
					{
						hasSurface = true;
					}
				}

				Sector sector = map.GetSector(x, y);
				var items = sector.Items;
				var mobs = sector.Mobiles;

				foreach (Item item in items)
				{
					if (item.ItemID >= 0x4000 || !item.AtWorldPoint(x, y))
					{
						continue;
					}

					ItemData id = item.ItemData;
					surface = id.Surface;
					impassable = id.Impassable;

					if (checkmob)
					{
						wet = (id.Flags & TileFlag.Wet) != 0;

						// dont allow wateronly creatures on land
						if (cantwalk && !wet)
						{
							impassable = true;
						}

						// allow water creatures on water
						if (canswim && wet)
						{
							surface = true;
							impassable = false;
						}
					}

					if ((surface || impassable || (checkBlocksFit && item.BlocksFit)) && (item.Z + id.CalcHeight) > z &&
						(z + height) > item.Z)
					{
						return false;
					}

					if (surface && !impassable && !item.Movable && z == (item.Z + id.CalcHeight))
					{
						hasSurface = true;
					}
				}

				if (checkMobiles)
				{
					if (
						mobs.Where(m => m.Location.X == x && m.Location.Y == y && (m.AccessLevel == AccessLevel.Player || !m.Hidden))
							.Any(m => (m.Z + 16) > z && (z + height) > m.Z))
					{
						return false;
					}
				}

				return !requireSurface || hasSurface;
			}

			// NOTE: if amount is set to something > 1, then this function will only return the last copy
			public static Item DUPE(TriggerObject trigObject, Item copy)
			{
				return DUPE(trigObject, copy, false, 1, false);
			}

			public static Item DUPE(TriggerObject trigObject, Item copy, bool inBag, int amount)
			{
				return DUPE(trigObject, copy, inBag, amount, false);
			}

			public static Item DUPE(TriggerObject trigObject, Item copy, bool inBag, int amount, bool dupeContents)
			{
				Container pack = null;

				if (inBag)
				{
					if (copy.Parent is Container)
					{
						pack = (Container)copy.Parent;
					}
					else if (copy.Parent is Mobile)
					{
						pack = ((Mobile)copy.Parent).Backpack;
					}
				}

				Type t = copy.GetType();
				Item output = null;

				//ConstructorInfo[] info = t.GetConstructors();

				ConstructorInfo c = t.GetConstructor(Type.EmptyTypes);

				bool done = false;

				if (c != null)
				{
					for (int i = 0; i < amount; i++)
					{
						object o = c.Invoke(null);

						if (!(o is Item))
						{
							continue;
						}

						Item newItem = (Item)o;
						Dupe.CopyProperties(newItem, copy); //copy.Dupe( item, copy.Amount );
						copy.OnAfterDuped(newItem);
						newItem.Parent = null;

						var scripts = XmlAttach.GetScripts(copy);

						if (scripts != null)
						{
							foreach (XmlScript script in scripts)
							{
								XmlScript copiedScript = new XmlScript(script.ScriptFile)
								{
									Name = script.Name,
									Enabled = script.Enabled
								};

								XmlAttach.AttachTo(newItem, copiedScript);
							}
						}

						if (pack != null)
						{
							pack.DropItem(newItem);
						}
						else
						{
							if (copy.RootParentEntity == null)
							{
								newItem.MoveToWorld(copy.Location, copy.Map);
							}
							else
							{
								newItem.MoveToWorld(copy.RootParentEntity.Location, copy.RootParentEntity.Map);
							}
						}

						if (newItem is Container && dupeContents)
						{
							Container newContainer = (Container)newItem;

							foreach (Item inItem in copy.Items)
							{
								newContainer.AddItem(DUPE(trigObject, inItem, false, 1, true));
							}
						}

						newItem.InvalidateProperties();
						output = newItem;
					}

					done = true;
				}

				if (!done)
				{
					throw new UberScriptException("DUPE command: tried to dupe object without 0 arg constructor!: " + copy);
				}

				return output;
			}

			public static Mobile DUPEMOB(TriggerObject trigObject, BaseCreature copy)
			{
				return DUPEMOB(trigObject, copy, 1, false);
			}

			public static Mobile DUPEMOB(TriggerObject trigObject, BaseCreature copy, int amount)
			{
				return DUPEMOB(trigObject, copy, amount, false);
			}

			public static Mobile DUPEMOB(TriggerObject trigObject, BaseCreature copy, int amount, bool copyItems)
			{
				Type t = copy.GetType();
				BaseCreature output = null;

				//ConstructorInfo[] info = t.GetConstructors();

				ConstructorInfo c = t.GetConstructor(Type.EmptyTypes);

				bool done = false;
				if (c != null)
				{
					for (int i = 0; i < amount; i++)
					{
						object o = c.Invoke(null);

						if (!(o is BaseCreature))
						{
							continue;
						}

						BaseCreature newMob = (BaseCreature)o;
						Dupe.CopyMobProperties(newMob, copy);

						// internalize it while everything is updating 
						newMob.Map = Map.Internal;

						if (copyItems)
						{
							// have to remove all the spawned layers (except backpack) on the mob, so that
							// it doesn't double up when they are duped over

							var toDelete = new List<Item>();

							foreach (Item item in newMob.Items)
							{
								if (item.Layer == Layer.Backpack)
								{
									while (item.Items.Count != 0)
									{
										item.Items[0].Delete();
									}
									continue;
								}

								toDelete.Add(item);
							}

							foreach (Item item in toDelete)
							{
								item.Delete();
							}

							// does not copy bank contents
							foreach (Item inItem in copy.Items)
							{
								if (inItem.Layer == Layer.Bank)
								{
									continue;
								}

								if (inItem.Layer == Layer.Backpack)
								{
									foreach (Item packItem in inItem.Items)
									{
										newMob.Backpack.AddItem(DUPE(trigObject, packItem, false, 1, true));
									}
								}
								else
								{
									newMob.AddItem(DUPE(trigObject, inItem, false, 1));
								}
							}
						}

						newMob.MoveToWorld(copy.Location, copy.Map);

						newMob.InvalidateProperties();
						output = newMob;
					}

					done = true;
				}

				if (!done)
				{
					throw new UberScriptException("DUPEMOB command: tried to dupe object without 0 arg constructor!: " + copy);
				}

				return output;
			}

			public static XmlTeam SETTEAM(TriggerObject trigObject, Mobile mob, string teamString)
			{
				return SETTEAM(trigObject, mob, teamString, true, false, false);
			}

			public static XmlTeam SETTEAM(TriggerObject trigObject, Mobile mob, string teamString, bool allyGreen)
			{
				return SETTEAM(trigObject, mob, teamString, allyGreen, false, false);
			}

			public static XmlTeam SETTEAM(TriggerObject trigObject, Mobile mob, string teamString, bool allyGreen, bool teamHarm)
			{
				return SETTEAM(trigObject, mob, teamString, allyGreen, teamHarm, false);
			}

			public static XmlTeam SETTEAM(
				TriggerObject trigObject, Mobile mob, string teamString, bool allyGreen, bool teamHarm, bool enemyHeal)
			{
				XmlTeam team = new XmlTeam
				{
					TeamGreen = allyGreen,
					TeamHarmAllowed = teamHarm,
					HealEnemyAllowed = enemyHeal,
					TeamVal = (XmlTeam.Team)Enum.Parse(typeof(XmlTeam.Team), teamString, true)
				};

				XmlAttach.AttachTo(mob, team);
				return team;
			}

			public static bool ISPARTOFREGION(
				TriggerObject trigObject, Region regionToTest, object region_OR_RegionType_Or_TypeString)
			{
				// this goes through the regionToTest and all its the parent regions to see if any of them are of type regionType
				Region possibleParent = region_OR_RegionType_Or_TypeString as Region;

				if (possibleParent != null)
				{
					return regionToTest.IsPartOf(possibleParent);
				}

				Type type;

				if (region_OR_RegionType_Or_TypeString is string)
				{
					type = ScriptCompiler.FindTypeByName((string)region_OR_RegionType_Or_TypeString);
				}
				else if (region_OR_RegionType_Or_TypeString is Type)
				{
					type = (Type)region_OR_RegionType_Or_TypeString;
				}
				else
				{
					type = region_OR_RegionType_Or_TypeString.GetType();
				}

				return regionToTest.IsPartOf(type);
			}

			public static bool ISDEFAULTREGION(TriggerObject trigObject, Region region)
			{
				return region.Map.DefaultRegion == region;
			}

			public static void LINEEFFECT(TriggerObject trigObject, IPoint3D from, IPoint3D to, string scriptFile)
			{
				LINEEFFECT(trigObject, 0, from, to, scriptFile);
			}

			public static void LINEEFFECT(TriggerObject trigObject, int radius, IPoint3D from, IPoint3D to, string scriptFile)
			{
				if (radius > 3)
				{
					throw new UberScriptException("LINEFFECT maximum radius is 3! You used a radius of " + radius);
				}

				if (from.X == to.X && from.Y == to.Y)
				{
					RUNSCRIPT(trigObject, to, scriptFile);
					return;
				}

				Point3D directionVector = new Point3D(to.X - from.X, to.Y - from.Y, to.Z - from.Z); // offset by 8 (looks better)
				double magnitude =
					Math.Sqrt(
						directionVector.X * directionVector.X + directionVector.Y * directionVector.Y +
						directionVector.Z * directionVector.Z);
				double normalizedDirectionVectorX = directionVector.X / magnitude;
				double normalizedDirectionVectorY = directionVector.Y / magnitude;
				double normalizedDirectionVectorZ = directionVector.Z / magnitude;
				double currentX = from.X;
				double currentY = from.Y;
				double currentZ = from.Z;
				var visited = new List<IPoint2D>
				{
					from
				};
				int count = 0;
				Point3D target;

				do
				{
					bool foundUnvisitedSquare;

					do
					{
						currentX += normalizedDirectionVectorX;
						currentY += normalizedDirectionVectorY;
						currentZ += normalizedDirectionVectorZ;
						foundUnvisitedSquare = true;

						for (int i = visited.Count - 1; i > -1; i--) // most likely to hit the last visited first
						{
							IPoint2D p = visited[i];

							if (Convert.ToInt32(Math.Round(currentX)) != p.X || Convert.ToInt32(Math.Round(currentY)) != p.Y)
							{
								continue;
							}

							foundUnvisitedSquare = false;
							break;
						}
					}
					while (foundUnvisitedSquare == false);

					target = new Point3D(
						Convert.ToInt32(Math.Round(currentX)),
						Convert.ToInt32(Math.Round(currentY)),
						Convert.ToInt32(Math.Round(currentZ)));

					if (radius == 0)
					{
						visited.Add(target);
						RUNSCRIPT(trigObject, target, scriptFile);
					}
					else
					{
						bool alreadyVisited;

						for (int x = target.X - radius; x <= target.X + radius; x++)
						{
							for (int y = target.Y - radius; y <= target.Y + radius; y++)
							{
								alreadyVisited = visited.Any(p => x == p.X && y == p.Y);

								if (alreadyVisited)
								{
									continue;
								}

								visited.Add(new Point2D(x, y));
								RUNSCRIPT(trigObject, new Point3D(x, y, target.Z), scriptFile);
							}
						}
					}

					count++;

					if (count > 1000)
					{
						throw new Exception("LINEFFECT had more than 1000 iterations!... not allowed!");
					}

					//Console.WriteLine(target.X + " vs " + to.X + "    " + target.Y + " vs " + to.Y);
				}
				while (Math.Abs(target.X - from.X) + Math.Abs(target.Y - from.Y) < Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y));
			}

			public static void LINEEFFECT(TriggerObject trigObject, IPoint3D from, IPoint3D to, string scriptFile, int delay)
			{
				LINEEFFECT(trigObject, 0, from, to, scriptFile, delay);
			}

			public static void LINEEFFECT(
				TriggerObject trigObject, int radius, IPoint3D from, IPoint3D to, string scriptFile, int delay)
			{
				if (radius > 3)
				{
					throw new UberScriptException("LINEFFECT maximum radius is 3! You used a radius of " + radius);
				}

				if (from.X == to.X && from.Y == to.Y)
				{
					RUNSCRIPT(trigObject, to, scriptFile);
					return;
				}

				Map map;

				if (from is IEntity)
				{
					map = ((IEntity)from).Map;
				}
				else if (to is IEntity)
				{
					map = ((IEntity)to).Map;
				}
				else
				{
					map = Map.Felucca;
				}

				LineEffectTimer timer = new LineEffectTimer(
					radius, from, to, map, scriptFile, trigObject, TimeSpan.FromMilliseconds(delay));

				timer.Start();
				AllLineEffectTimers.Add(timer);
			}

			private static readonly List<LineEffectTimer> AllLineEffectTimers = new List<LineEffectTimer>();

			public static void StopAllLineEffectTimers()
			{
				foreach (LineEffectTimer timer in AllLineEffectTimers.Where(timer => timer.Running))
				{
					timer.Stop();
				}

				AllLineEffectTimers.Clear();
			}

			private class LineEffectTimer : Timer
			{
				private Map Map;
				private readonly IPoint3D From;
				private readonly IPoint3D To;
				private readonly Point3D DirectionVector;
				private int Count;
				private double CurrentX;
				private double CurrentY;
				private double CurrentZ;
				private readonly double NormalizedDirectionVectorX;
				private readonly double NormalizedDirectionVectorY;
				private readonly double NormalizedDirectionVectorZ;
				private readonly string ScriptFile;
				private readonly List<IPoint2D> Visited = new List<IPoint2D>();
				private readonly TriggerObject TrigObj;
				private readonly TimeSpan DelayPerTick;
				private DateTime LastExecuted;
				private readonly int Radius;

				public List<Mobile> FrozenMobs = new List<Mobile>();

				public LineEffectTimer(
					int radius, IPoint3D from, IPoint3D to, Map map, string scriptFile, TriggerObject trigObject, TimeSpan delay)
					: base(TimeSpan.Zero, TimeSpan.Zero)
				{
					if (from.X == to.X && from.Y == to.Y)
					{
						throw new UberScriptException("LineEffectTimer cannot have same from and to point!");
					}

					Priority = TimerPriority.TenMS;
					Radius = radius;
					From = from;
					To = to;
					Map = map;
					ScriptFile = scriptFile;
					DirectionVector = new Point3D(to.X - from.X, to.Y - from.Y, to.Z - from.Z);

					double magnitude =
						Math.Sqrt(
							DirectionVector.X * DirectionVector.X + DirectionVector.Y * DirectionVector.Y +
							DirectionVector.Z * DirectionVector.Z);

					NormalizedDirectionVectorX = DirectionVector.X / magnitude;
					NormalizedDirectionVectorY = DirectionVector.Y / magnitude;
					NormalizedDirectionVectorZ = DirectionVector.Z / magnitude;
					CurrentX = from.X;
					CurrentY = from.Y;
					CurrentZ = from.Z;
					TrigObj = trigObject;
					DelayPerTick = delay;
					LastExecuted = DateTime.UtcNow;
				}

				protected override void OnTick()
				{
					if (DateTime.UtcNow < (LastExecuted + DelayPerTick))
					{
						return;
					}

					LastExecuted = DateTime.UtcNow;

					try
					{
						bool foundUnvisitedSquare = true;

						do
						{
							CurrentX += NormalizedDirectionVectorX;
							CurrentY += NormalizedDirectionVectorY;
							CurrentZ += NormalizedDirectionVectorZ;
							foundUnvisitedSquare =
								Visited.All(p => Convert.ToInt32(Math.Round(CurrentX)) != p.X || Convert.ToInt32(Math.Round(CurrentY)) != p.Y);
						}
						while (foundUnvisitedSquare == false);

						Point3D target = new Point3D(
							Convert.ToInt32(Math.Round(CurrentX)),
							Convert.ToInt32(Math.Round(CurrentY)),
							Convert.ToInt32(Math.Round(CurrentZ)));

						if (Radius == 0)
						{
							Visited.Add(target);
							RUNSCRIPT(TrigObj, target, ScriptFile);
						}
						else
						{
							bool alreadyVisited;

							for (int x = target.X - Radius; x <= target.X + Radius; x++)
							{
								for (int y = target.Y - Radius; y <= target.Y + Radius; y++)
								{
									alreadyVisited = Visited.Any(p => x == p.X && y == p.Y);

									if (alreadyVisited)
									{
										continue;
									}

									Visited.Add(new Point2D(x, y));
									RUNSCRIPT(TrigObj, new Point3D(x, y, target.Z), ScriptFile);
								}
							}
						}

						Count++;

						if (Count > 1000)
						{
							throw new Exception("LINEFFECT had more than 1000 iterations!... not allowed!");
						}

						if (Math.Abs(target.X - From.X) + Math.Abs(target.Y - From.Y) < Math.Abs(To.X - From.X) + Math.Abs(To.Y - From.Y))
						{
							return;
						}

						Stop();
						AllLineEffectTimers.Remove(this);
						//Console.WriteLine(target.X + " vs " + To.X + "    " + target.Y + " vs " + To.Y);
					}
					catch (Exception e)
					{
						Stop();
						AllLineEffectTimers.Remove(this);
						Console.WriteLine("LINEEFFECT timer error! target script = " + ScriptFile + ": " + e.Message);
						Console.WriteLine(e.StackTrace);
					}
				}
			}

			public static void POSSESS(TriggerObject trigObject, Mobile from, BaseCreature target)
			{
				if (!(from is BaseCreature || from is PlayerMobile))
				{
					throw new UberScriptException("POSSESS only accepts 'from' BaseCreature or PlayerMobile!");
				}

				if (from.NetState == null)
				{
					throw new UberScriptException("POSSESS Attempted to force possession from a mob without Netstate!:" + from);
				}

				if (target.NetState != null)
				{
					throw new UberScriptException("POSSESS attempted to force possession into an already possessed mob!:" + target);
				}

				CreaturePossession.ForcePossessCreature(null, from, target);
			}

			public static void MOVETOWORLD(TriggerObject trigObject, IEntity entity, IPoint3D loc)
			{
				Map map = Map.Felucca;

				if (loc is IEntity)
				{
					map = ((IEntity)loc).Map;
				}

				entity.MoveToWorld(new Point3D(loc.X, loc.Y, loc.Z), map);
			}

			public static void MOVETOWORLD(TriggerObject trigObject, IEntity entity, IPoint3D loc, object map)
			{
				Map existingMap = map as Map;

				if (existingMap == null && map is string)
				{
					try
					{
						existingMap = Map.Parse((string)map);
					}
					catch
					{ }
				}

				if (existingMap == null)
				{
					throw new UberScriptException("A map named " + map + " does not exist!");
				}

				entity.MoveToWorld(new Point3D(loc.X, loc.Y, loc.Z), existingMap);
			}

			public static ArrayList GETITEMSINCONTAINER(TriggerObject trigObject, Mobile mob)
			{
				return new ArrayList(mob.Items);
			}

			public static ArrayList GETITEMSINCONTAINER(TriggerObject trigObject, Container container)
			{
				return new ArrayList(container.Items);
			}

			// copied from Mobile.cs
			public static Direction GETDIRECTIONTO(TriggerObject trigObject, IPoint2D from, IPoint2D to)
			{
				int dx = from.X - to.X;
				int dy = from.Y - to.Y;

				int rx = (dx - dy) * 44;
				int ry = (dx + dy) * 44;

				int ax = Math.Abs(rx);
				int ay = Math.Abs(ry);

				Direction ret;

				if (((ay >> 1) - ax) >= 0)
				{
					ret = (ry > 0) ? Direction.Up : Direction.Down;
				}
				else if (((ax >> 1) - ay) >= 0)
				{
					ret = (rx > 0) ? Direction.Left : Direction.Right;
				}
				else if (rx >= 0 && ry >= 0)
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

			public static void FREEZE(TriggerObject trigObject, IPoint2D p, int range)
			{
				FREEZE(trigObject, p, range, false);
			}

			public static void FREEZE(TriggerObject trigObject, IPoint2D p, int range, bool iceHue)
			{
				Map map = (p is IEntity) ? ((IEntity)p).Map : Map.Felucca;
				var effected = map.GetMobilesInRange(new Point3D(p.X, p.Y, 0), range);

				foreach (Mobile mob in effected)
				{
					mob.Frozen = true;

					if (iceHue)
					{
						mob.HueMod = 1152;
					}
				}

				effected.Free();
			}

			public static void UNFREEZE(TriggerObject trigObject, Mobile mob)
			{
				mob.Frozen = false;

				if (mob.HueMod == 1152)
				{
					mob.HueMod = -1;
				}
			}

			public static void UNFREEZE(TriggerObject trigObject, IPoint2D p, int range)
			{
				Map map = (p is IEntity) ? ((IEntity)p).Map : Map.Felucca;
				var effected = map.GetMobilesInRange(new Point3D(p.X, p.Y, 0), range);

				foreach (Mobile mob in effected)
				{
					mob.Frozen = false;

					if (mob.HueMod == 1152)
					{
						mob.HueMod = -1;
					}
				}

				effected.Free();
			}

			public static ArrayList Concuss(IPoint2D p, Map map, int distance, int range)
			{
				var effected = map.GetMobilesInRange(new Point3D(p.X, p.Y, 0), range);

				var effectedMobOutput = effected.OfType<Mobile>().Where(mob => !mob.Blessed && !(mob is Reaper)).ToList();

				effected.Free();

				foreach (Mobile mob in effectedMobOutput)
				{
					Direction direction = GETDIRECTIONTO(null, p, mob);

					int startX = mob.X;
					int startY = mob.Y;

					switch (direction)
					{
						case Direction.North:
							startY -= distance;
							break;
						case Direction.Right:
							startY -= distance;
							startX += distance;
							break;
						case Direction.East:
							startX += distance;
							break;
						case Direction.Down:
							startY += distance;
							startX += distance;
							break;
						case Direction.South:
							startY += distance;
							break;
						case Direction.Left:
							startY += distance;
							startX -= distance;
							break;
						case Direction.West:
							startX -= distance;
							break;
						case Direction.Up:
							startY -= distance;
							startX -= distance;
							break;
					}

					int newZ;
					Point3D newPosition = new Point3D(startX, startY, mob.Z);

					if (!mob.CheckMovement(direction, out newZ))
					{
						continue;
					}

					newPosition.Z = newZ;
					mob.MoveToWorld(newPosition, mob.Map);
				}

				return new ArrayList(effectedMobOutput);
			}

			public static void CONCUSSION(TriggerObject trigObj, IPoint2D p, int distance, int range)
			{
				Map map = (p is IEntity) ? ((IEntity)p).Map : Map.Felucca;

				for (int i = 0; i < distance; i++)
				{
					Concuss(p, map, 1, range);
				}
			}

			public static void CONCUSSION(TriggerObject trigObj, IPoint2D p, int distance, int range, string speed)
			{
				Map map = (p is IEntity) ? ((IEntity)p).Map : Map.Felucca;

				speed = speed.ToLower();

				switch (speed)
				{
					case "tenms":
						new ConcussionTimer(TimerPriority.TenMS, range, p, map).Start();
						break;
					case "twentyfivems":
						new ConcussionTimer(TimerPriority.TwentyFiveMS, range, p, map).Start();
						break;
					case "fiftyms":
						new ConcussionTimer(TimerPriority.FiftyMS, range, p, map).Start();
						break;
					case "twofiftyms":
						new ConcussionTimer(TimerPriority.TwoFiftyMS, range, p, map).Start();
						break;
					case "onesecond":
						new ConcussionTimer(TimerPriority.OneSecond, range, p, map).Start();
						break;
					case "fiveseconds":
						new ConcussionTimer(TimerPriority.FiveSeconds, range, p, map).Start();
						break;
					case "oneminute":
						new ConcussionTimer(TimerPriority.OneMinute, range, p, map).Start();
						break;
					default:
						throw new UberScriptException(
							"CONCUSSION function had invalid time string--expected TenMS, TwentyFiveMS, FiftyMS, TwoFiftyMS, OneSecond, FiveSeconds, or OneMinute");
				}
			}

			private class ConcussionTimer : Timer
			{
				private readonly int Range;
				private int CurrentDistance;
				private readonly Map Map;
				private readonly IPoint2D Point;
				public readonly List<Mobile> FrozenMobs = new List<Mobile>();

				public ConcussionTimer(TimerPriority priority, int range, IPoint2D point2D, Map map)
					: base(TimeSpan.Zero, TimeSpan.Zero)
				{
					Priority = priority;
					Point = point2D;
					Map = map;
					Range = range;
				}

				protected override void OnTick()
				{
					if (CurrentDistance > Range)
					{
						Stop();
						foreach (Mobile mob in FrozenMobs)
						{
							mob.Frozen = false;
						}
						return;
					}
					ArrayList effected = Concuss(Point, Map, 1, Range); // push them back 1 tile at a time
					foreach (Mobile mob in effected)
					{
						if (!FrozenMobs.Contains(mob))
						{
							FrozenMobs.Add(mob);
							mob.Frozen = true;
						}
					}
					CurrentDistance++;
				}
			}

			public static bool CANFIT(TriggerObject trigObj, IPoint2D p)
			{
				Map map = (p is IEntity) ? ((IEntity)p).Map : Map.Felucca;
				int z = map.GetAverageZ(p.X, p.Y);

				return p is Mobile
						   ? CANFITMOB(trigObj, p.X, p.Y, z, 16, true, false, true, (Mobile)p)
						   : map.CanFit(p.X, p.Y, z, 16, true, false, false);
			}

			public static bool CANFIT(TriggerObject trigObj, IPoint3D p)
			{
				Map map = (p is IEntity) ? ((IEntity)p).Map : Map.Felucca;

				return p is Mobile
						   ? CANFITMOB(trigObj, p.X, p.Y, p.Z, 16, true, false, true, (Mobile)p)
						   : map.CanFit(p.X, p.Y, p.Z, 16, true, false, false);
			}

			public static bool CANFIT(TriggerObject trigObj, IPoint3D p, Mobile mob)
			{
				return CANFITMOB(trigObj, p.X, p.Y, p.Z, 16, true, false, true, mob);
			}

			public static int GETAVERAGEZ(TriggerObject trigObj, IPoint2D p)
			{
				Map map = (p is IEntity) ? ((IEntity)p).Map : Map.Felucca;

				return map.GetAverageZ(p.X, p.Y);
			}

			// the syntax is REWARD,xmlvalueName,highest|linear|exponential,platinumAmount[,bank][,eventname][,rare[,probability]]
			public static void PLATREWARD(
				TriggerObject trigObj, string xmlvalueName, string scaletype, string eventname, int platinumAmount)
			{
				PLATREWARD(trigObj, xmlvalueName, scaletype, eventname, platinumAmount, true, false, 0.0);
			}

			public static void PLATREWARD(
				TriggerObject trigObj, string xmlvalueName, string scaletype, string eventname, int platinumAmount, bool putInBank)
			{
				PLATREWARD(trigObj, xmlvalueName, scaletype, eventname, platinumAmount, putInBank, false, 0.0);
			}

			public static void PLATREWARD(
				TriggerObject trigObj,
				string xmlvalueName,
				string scaletype,
				string eventname,
				int platinumAmount,
				bool putInBank,
				bool rare,
				double probability)
			{
				// the syntax is REWARD,xmlvalueName,highest|linear|exponential,platinumAmount[,bank][,eventname][,rare[,probability]]

				// disburse the platinum reward and/or rare to the one with the highest score
				var sortedValues = XmlAttachmentProcessing.Search(xmlvalueName);

				try
				{
					if (sortedValues.Count > 0 && sortedValues[0].Value != 0)
					{
						switch (scaletype)
						{
							case "highest":
								{
									Mobile owner = sortedValues[0].Owner as Mobile;

									if (owner != null && !owner.Deleted)
									{
										AwardPlatinum(putInBank, owner, platinumAmount, eventname);
										LogReward(sortedValues[0], platinumAmount, eventname);
									}
								}
								break;
							case "linear":
								{
									Mobile owner;
									double maxValue = sortedValues[0].Value;

									if (maxValue > 0)
									{
										double slope = platinumAmount / maxValue;

										foreach (XmlValue xmlValue in sortedValues)
										{
											owner = xmlValue.Owner as Mobile;

											if (owner == null || owner.Deleted)
											{
												continue;
											}

											int awardedAmount = (int)Math.Round(xmlValue.Value * slope);

											AwardPlatinum(putInBank, owner, awardedAmount, eventname);
											LogReward(xmlValue, awardedAmount, eventname);
										}
									}
								}
								break;
							case "exponential":
								{
									double maxValue = sortedValues[0].Value;

									if (maxValue > 0)
									{
										double slope = 1.0 / maxValue;

										const double exponentialParameter = 2.0; // arbitrarily set to increase the slope at lower levels

										foreach (XmlValue xmlValue in sortedValues)
										{
											Mobile owner = xmlValue.Owner as Mobile;

											if (owner == null || owner.Deleted)
											{
												continue;
											}

											double linearScaledValue = xmlValue.Value * slope; // 0-1
											int awardedAmount =
												(int)Math.Round((1.0 - Math.Exp(-linearScaledValue * exponentialParameter)) * platinumAmount);

											AwardPlatinum(putInBank, owner, awardedAmount, eventname);
											LogReward(xmlValue, awardedAmount, eventname);
										}
									}
								}
								break;
							case "exponential2":
								{
									double maxValue = sortedValues[0].Value;

									if (maxValue > 0)
									{
										double slope = 1.0 / maxValue;

										const double exponentialParameter = 6.0; // arbitrarily set to increase the slope at lower levels

										foreach (XmlValue xmlValue in sortedValues)
										{
											Mobile owner = xmlValue.Owner as Mobile;

											if (owner == null || owner.Deleted)
											{
												continue;
											}

											double linearScaledValue = xmlValue.Value * slope; // 0-1
											int awardedAmount =
												(int)Math.Round((1.0 - Math.Exp(-linearScaledValue * exponentialParameter)) * platinumAmount);

											AwardPlatinum(putInBank, owner, awardedAmount, eventname);
											LogReward(xmlValue, awardedAmount, eventname);
										}
									}
								}
								break;
							case "uniform":
								foreach (XmlValue xmlValue in sortedValues)
								{
									Mobile owner = xmlValue.Owner as Mobile;

									if (xmlValue.Value <= 0 || owner == null || owner.Deleted)
									{
										continue;
									}

									AwardPlatinum(putInBank, owner, platinumAmount, eventname);
									LogReward(xmlValue, platinumAmount, eventname);
								}
								break;
							default:
								throw new UberScriptException("bad scaletype arg in REWARD");
						}

						// rare rewards are always given only to the player with the highest score
						if (rare)
						{
							Mobile owner = sortedValues[0].Owner as Mobile;

							if (owner != null && !owner.Deleted)
							{
								if (Utility.RandomDouble() < probability)
								{
									owner.SendMessage(38, "You were #1 in the {0} and got a special reward!", eventname ?? "event");
									owner.AddToBackpack(new IOU());
									LogReward(sortedValues[0], 9999, eventname); // temporary approach
								}
								else
								{
									owner.SendMessage(
										38,
										"Although you were #1 in the {0}, you didn't get a special reward this time. So sorry!",
										eventname ?? "event");
								}
							}
						}
					}
				}
				catch
				{ }

				foreach (XmlValue xmlvalue in sortedValues)
				{
					try
					{
						xmlvalue.Delete();
					}
					catch
					{ }
				}
			}

			private static void AwardPlatinum(bool putInBank, Mobile mob, int platinumAmount, string eventname)
			{
				if (platinumAmount == 0)
				{
					mob.SendMessage(
						38, "Sorry, but you did not acquire enough points to gain any platinum for the {0}.", eventname ?? "event");
					return;
				}

				bool atFeet = false;

				if (putInBank)
				{
					mob.BankBox.AddItem(new Platinum(platinumAmount));
				}
				else
				{
					atFeet = !mob.AddToBackpack(new Platinum(platinumAmount));
				}

				mob.SendMessage(
					38,
					"You have been awarded {0} platinum coin{1} for your participation in the {2}! {3} been placed {4}.",
					platinumAmount,
					platinumAmount != 1 ? "s" : String.Empty,
					eventname ?? "event",
					platinumAmount != 1 ? "It has" : "They have",
					putInBank ? "in your bank" : !atFeet ? "in your pack" : "at your feet");
			}

			private static void LogReward(XmlValue value, int platinumAmount, string eventname)
			{
				if (value == null || !(value.Owner is Mobile))
				{
					return;
				}

				Mobile owner = (Mobile)value.Owner;

				if (String.IsNullOrWhiteSpace(eventname))
				{
					eventname = "event";
				}

				using (StreamWriter writer = new StreamWriter("Rewards.txt", true))
				{
					writer.WriteLine(owner + "\t" + eventname + "\t" + value.Value + "\t" + platinumAmount + "\t" + value.Name);
				}
			}

			public static void REFRESHRESOURCES(TriggerObject trigObject, int x, int y)
			{
				REFRESHRESOURCES(trigObject, x, y, Map.Felucca);
			}

			public static void REFRESHRESOURCES(TriggerObject trigObject, int x, int y, Map map)
			{
				if (map == null || map == Map.Internal)
				{
					return;
				}

				HarvestBank bank = Lumberjacking.System.Definition.GetBank(map, x, y);
				bank.NextRespawn = DateTime.MinValue;
				bank = Mining.System.OreAndStone.GetBank(map, x, y);
				bank.NextRespawn = DateTime.MinValue;
				bank = Fishing.System.Fish.GetBank(map, x, y);
				bank.NextRespawn = DateTime.MinValue;
			}

			public static void REFRESHRESOURCES(TriggerObject trigObject, int x, int y, Map map, string type)
			{
				if (map == null || map == Map.Internal || String.IsNullOrWhiteSpace(type))
				{
					return;
				}

				HarvestBank bank = null;

				switch (type.ToLower())
				{
					case "lumberjacking":
						bank = Lumberjacking.System.Definition.GetBank(map, x, y);
						break;
					case "mining":
						bank = Mining.System.OreAndStone.GetBank(map, x, y);
						break;
					case "fishing":
						bank = Fishing.System.Fish.GetBank(map, x, y);
						break;
				}

				if (bank != null)
				{
					bank.NextRespawn = DateTime.MinValue;
				}
			}

			public static Item GETITEMFROM(TriggerObject trigObject, Mobile mob, Item item)
			{
				return mob != null ? GETITEMFROMLIST(trigObject, mob.Items, item) : null;
			}

			public static Item GETITEMFROM(TriggerObject trigObject, Mobile mob, Type type)
			{
				return mob != null ? GETITEMFROMLIST(trigObject, mob.Items, type, null) : null;
			}

			public static Item GETITEMFROM(TriggerObject trigObject, Mobile mob, Type type, string name)
			{
				return mob != null ? GETITEMFROMLIST(trigObject, mob.Items, type, name) : null;
			}

			public static Item GETITEMFROM(TriggerObject trigObject, Container container, Type type)
			{
				return container != null ? GETITEMFROMLIST(trigObject, container.Items, type, null) : null;
			}

			public static Item GETITEMFROM(TriggerObject trigObject, Container container, Type type, string name)
			{
				return container != null ? GETITEMFROMLIST(trigObject, container.Items, type, name) : null;
			}

			public static Item GETITEMFROM(TriggerObject trigObject, Container container, Item searchForItem)
			{
				return container != null ? GETITEMFROMLIST(trigObject, container.Items, searchForItem) : null;
			}

			public static Item GETITEMFROMLIST(TriggerObject trigObject, List<Item> items, Type type)
			{
				return GETITEMFROMLIST(trigObject, items, type, null);
			}

			public static Item GETITEMFROMLIST(TriggerObject trigObject, List<Item> items, Type type, string name)
			{
				if ((type == null && name == null) || items == null || items.Count == 0)
				{
					return null;
				}

				Item output;

				if (type == null)
				{
					foreach (Item item in items)
					{
						if (item.Name == name)
						{
							return item;
						}

						if (!(item is Container))
						{
							continue;
						}

						output = GETITEMFROMLIST(trigObject, item.Items, null, name);

						if (output != null)
						{
							return output;
						}
					}
				}
				else
				{
					foreach (Item item in items)
					{
						if (item.GetType() == type)
						{
							if (name != null)
							{
								if (name == item.Name)
								{
									return item;
								}
							}
							else
							{
								return item;
							}
						}

						if (!(item is Container))
						{
							continue;
						}

						output = GETITEMFROMLIST(trigObject, item.Items, type, name);

						if (output != null)
						{
							return output;
						}
					}
				}

				return null;
			}

			public static Item GETITEMFROMLIST(TriggerObject trigObject, List<Item> items, Item searchForItem)
			{
				if (items == null || items.Count == 0 || searchForItem == null)
				{
					return null;
				}

				Item output;

				foreach (Item item in items)
				{
					if (item == searchForItem)
					{
						return item;
					}

					if (!(item is Container))
					{
						continue;
					}

					output = GETITEMFROMLIST(trigObject, item.Items, searchForItem);

					if (output != null)
					{
						return output;
					}
				}

				return null;
			}

			public static bool EQUIP(TriggerObject trigObject, Mobile mob, Item item)
			{
				if (mob == null || item == null || item.Deleted || item.Layer == Layer.Invalid)
				{
					return false;
				}

				Item currentlyEquipped = mob.FindItemOnLayer(item.Layer);

				if (currentlyEquipped != null && !currentlyEquipped.Deleted)
				{
					mob.AddToBackpack(currentlyEquipped);
				}

				return mob.EquipItem(item);
			}

			public static Item UNEQUIP(TriggerObject trigObject, Mobile mob, Item item)
			{
				if (mob == null || item == null || item.Deleted || !mob.Items.Contains(item))
				{
					return null;
				}

				mob.AddToBackpack(item);
				return item;
			}

			public static Item UNEQUIP(TriggerObject trigObject, Mobile mob, string layer)
			{
				if (mob == null || layer == null)
				{
					return null;
				}

				Layer l;

				if (!Enum.TryParse(layer, true, out l))
				{
					l = Layer.Invalid;
				}

				Item onLayer = mob.FindItemOnLayer(l);

				if (onLayer != null)
				{
					mob.AddToBackpack(onLayer);
				}

				return onLayer;
			}

			public static void DROPALLCARRIED(TriggerObject trigObject, Mobile mob)
			{
				if (mob != null)
				{
					DROPALLCARRIED(trigObject, mob.Backpack);
				}
			}

			public static void DROPALLCARRIED(TriggerObject trigObject, Container container)
			{
				if (container == null || container.Deleted)
				{
					return;
				}

				IEntity worldEntity = container.RootParentEntity ?? container;

				while (container.Items.Count > 0)
				{
					Item item = container.Items[0];

					container.RemoveItem(item);

					item.MoveToWorld(worldEntity.Location, worldEntity.Map == Map.Internal ? Map.Felucca : worldEntity.Map);
				}
			}

			public static Item DROPHOLDING(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null || mob.Holding == null)
				{
					return null;
				}

				Item output = mob.Holding;
				mob.DropHolding();

				return output;
			}

			public static Item GETHOLDING(TriggerObject trigObject, Mobile mob)
			{
				return mob != null ? mob.Holding : null;
			}

			public static void AUTOSTABLE(TriggerObject trigObject, Mobile mob, BaseCreature pet)
			{
				if (mob is PlayerMobile)
				{
					((PlayerMobile)mob).StablePet(pet, true, true);
				}
			}

			public static bool NEXTSTAGE(TriggerObject trigObject)
			{
				return NEXTSTAGE(trigObject, trigObject.Script, false);
			}

			public static bool NEXTSTAGE(TriggerObject trigObject, XmlScript script)
			{
				return NEXTSTAGE(trigObject, script, false);
			}

			public static bool NEXTSTAGE(TriggerObject trigObject, XmlScript script, bool wrap)
			{
				if (script == null)
				{
					return false;
				}

				script.ProceedToNextStage = true;

				if (script.Execute(trigObject, false))
				{
					// end of sequence has been reached
					if (wrap)
					{
						script.Stage = -1;
						script.ProceedToNextStage = true;

						return !script.Execute(trigObject, false); // returns false if it hits the end again
					}

					return false;
				}

				return true;
			}

			public static UberScriptSpot RUNSCRIPT(TriggerObject trigObject, IPoint3D p, string filename)
			{
				UberScriptSpot spot = new UberScriptSpot
				{
					Location = new Point3D(p.X, p.Y, p.Z),
					Map = (p is IEntity ? ((IEntity)p).Map : Map.Felucca)
				};

				XmlScript script = new XmlScript(filename);

				if (!ParsedScripts.ScriptFileMap.ContainsKey(filename.ToLower()))
				{
					// it failed to parse the script, so delete the plate, return null
					spot.Delete();
					return null;
				}

				XmlAttach.AttachTo(spot, script);

				TriggerObject newTrigObject = new TriggerObject
				{
					This = spot,
					TrigMob = trigObject.TrigMob,
					TrigItem = trigObject.TrigItem,
					Spell = trigObject.Spell,
					Damage = trigObject.Damage,
					Speech = trigObject.Speech,
					Targeted = trigObject.Targeted
				};

				//newTrigObject.TriggerName = trigObject.TriggerName;
				script.Execute(newTrigObject, false);

				return spot;
			}

			public static void UNSUBSCRIBETIMER(TriggerObject trigObject, string time)
			{
				UNSUBSCRIBETIMER(trigObject, trigObject.Script, time);
			}

			public static void UNSUBSCRIBETIMER(TriggerObject trigObject, XmlScript script, string time)
			{
				if (script == null)
				{
					return;
				}

				if (time == null)
				{
					time = String.Empty;
				}

				switch (time.ToLower())
				{
					case "everytick":
						script.TimerUnsubscribe = XmlScript.TimerSubscriptionFlag.EveryTick;
						break;
					case "tenms":
						script.TimerUnsubscribe = XmlScript.TimerSubscriptionFlag.TenMS;
						break;
					case "twentyfivems":
						script.TimerUnsubscribe = XmlScript.TimerSubscriptionFlag.TwentyFiveMS;
						break;
					case "fiftyms":
						script.TimerUnsubscribe = XmlScript.TimerSubscriptionFlag.FiftyMS;
						break;
					case "twofiftyms":
						script.TimerUnsubscribe = XmlScript.TimerSubscriptionFlag.TwoFiftyMS;
						break;
					case "onesecond":
						script.TimerUnsubscribe = XmlScript.TimerSubscriptionFlag.OneSecond;
						break;
					case "fiveseconds":
						script.TimerUnsubscribe = XmlScript.TimerSubscriptionFlag.FiveSeconds;
						break;
					case "oneminute":
						script.TimerUnsubscribe = XmlScript.TimerSubscriptionFlag.OneMinute;
						break;
					default:
						throw new UberScriptException(
							"SUBSCRIBETIMER function had invalid time string--expected EveryTick, TenMS, TwentyFiveMS, FiftyMS, TwoFiftyMS, OneSecond, FiveSeconds, or OneMinute");
				}
			}

			public static void SUBSCRIBETIMER(TriggerObject trigObject, string time)
			{
				SUBSCRIBETIMER(trigObject, trigObject.Script, time);
			}

			public static void SUBSCRIBETIMER(TriggerObject trigObject, XmlScript script, string time)
			{
				if (script == null)
				{
					return;
				}

				if (time == null)
				{
					time = String.Empty;
				}

				switch (time.ToLower())
				{
					case "everytick":
						script.TimerSubscribe = XmlScript.TimerSubscriptionFlag.EveryTick;
						break;
					case "tenms":
						script.TimerSubscribe = XmlScript.TimerSubscriptionFlag.TenMS;
						break;
					case "twentyfivems":
						script.TimerSubscribe = XmlScript.TimerSubscriptionFlag.TwentyFiveMS;
						break;
					case "fiftyms":
						script.TimerSubscribe = XmlScript.TimerSubscriptionFlag.FiftyMS;
						break;
					case "twofiftyms":
						script.TimerSubscribe = XmlScript.TimerSubscriptionFlag.TwoFiftyMS;
						break;
					case "onesecond":
						script.TimerSubscribe = XmlScript.TimerSubscriptionFlag.OneSecond;
						break;
					case "fiveseconds":
						script.TimerSubscribe = XmlScript.TimerSubscriptionFlag.FiveSeconds;
						break;
					case "oneminute":
						script.TimerSubscribe = XmlScript.TimerSubscriptionFlag.OneMinute;
						break;
					default:
						throw new UberScriptException(
							"SUBSCRIBETIMER function had invalid time string--expected EveryTick, TenMS, TwentyFiveMS, FiftyMS, TwoFiftyMS, OneSecond, FiveSeconds, or OneMinute");
				}
			}

			public static void SENDTARGET(TriggerObject trigObject, Mobile mob)
			{
				if (mob != null)
				{
					mob.Target = new UberScriptTarget(trigObject.Script, trigObject.This, true);
				}
			}

			public static void SENDTARGET(TriggerObject trigObject, Mobile mob, bool allowGround)
			{
				if (mob != null)
				{
					mob.Target = new UberScriptTarget(trigObject.Script, trigObject.This, allowGround);
				}
			}

			public static Mobile GETMOB(TriggerObject trigObject, int serial)
			{
				return GETMOB(trigObject, serial, null);
			}

			public static Mobile GETMOB(TriggerObject trigObject, int serial, Type type)
			{
				Mobile testmob;

				if (serial >= 0 && World.Mobiles.ContainsKey(serial))
				{
					testmob = World.Mobiles[serial];
				}
				else
				{
					return null;
				}

				if (testmob.Deleted)
				{
					return null;
				}

				Type mobType = testmob.GetType();

				return type == null || mobType == type || mobType.IsSubclassOf(type) ? testmob : null;
			}

			public static object GETMOB(TriggerObject trigObject, string name)
			{
				return GETMOB(trigObject, name, null);
			}

			public static object GETMOB(TriggerObject trigObject, string name, Type type)
			{
				int count = 0;
				Mobile foundMobile = FindInRecentMobileSearchList(name);

				if (foundMobile != null)
				{
					Type mobType = foundMobile.GetType();

					if (foundMobile.Deleted)
					{
						RecentlyLookedUpMobiles.Remove(name);
					}
					else if (type == null || (type == mobType || mobType.IsSubclassOf(type)))
					{
						return foundMobile;
					}
				}

				// search through all mobiles in the world and find the first one with a matching name
				foreach (Mobile mob in World.Mobiles.Values.Where(m => m != null && !m.Deleted && m.RawName == name))
				{
					Type mobtype = mob.GetType();

					if (type != null && mobtype != type && !mobtype.IsSubclassOf(type))
					{
						continue;
					}

					foundMobile = mob;
					count++;

					// added the break in to return the first match instead of forcing uniqueness (overrides the count test)
					break;
				}

				// if a unique item is found then success
				if (count == 1)
				{
					// add this to the recent search list
					RecentlyLookedUpMobiles.Add(name, foundMobile);

					return foundMobile;
				}

				return null;
			}

			public static Item GETITEM(TriggerObject trigObject, int serial)
			{
				return GETITEM(trigObject, serial, null);
			}

			public static Item GETITEM(TriggerObject trigObject, int serial, Type type)
			{
				Item testitem;

				if (serial >= 0 && World.Items.ContainsKey(serial))
				{
					testitem = World.Items[serial];
				}
				else
				{
					return null;
				}

				if (testitem.Deleted)
				{
					return null;
				}

				Type itemType = testitem.GetType();

				return type == null || itemType == type || itemType.IsSubclassOf(type) ? testitem : null;
			}

			public static Item GETITEM(TriggerObject trigObject, string name)
			{
				return GETITEM(trigObject, name, null);
			}

			public static Item GETITEM(TriggerObject trigObject, string name, Type type)
			{
				int count = 0;
				Item foundItem = FindInRecentItemSearchList(name);

				if (foundItem != null)
				{
					Type itemType = foundItem.GetType();

					if (foundItem.Deleted)
					{
						RecentlyLookedUpItems.Remove(name);
					}
					else if (type == null || (itemType == type || itemType.IsSubclassOf(type)))
					{
						return foundItem;
					}
				}

				// search through all items in the world and find the first one with a matching name
				foreach (Item item in World.Items.Values.Where(i => i != null && !i.Deleted && i.Name == name))
				{
					Type itemType = item.GetType();

					if (type != null && itemType != type && !itemType.IsSubclassOf(type))
					{
						continue;
					}

					foundItem = item;
					count++;

					// added the break in to return the first match instead of forcing uniqueness (overrides the count test)
					break;
				}

				// if a unique item is found then success
				if (count == 1)
				{
					// add this to the recent search list
					RecentlyLookedUpItems.Add(name, foundItem);

					return (foundItem);
				}

				return null;
			}

			public static void SWINGANIMATION(TriggerObject trigObject, Mobile mob)
			{
				if (mob == null || mob.Weapon == null || !(mob.Weapon is BaseWeapon))
				{
					return;
				}

				int action;
				WeaponAnimation animation = ((BaseWeapon)mob.Weapon).Animation;

				switch (mob.Body.Type)
				{
					case BodyType.Sea:
					case BodyType.Animal:
						action = Utility.Random(5, 2);
						break;
					case BodyType.Monster:
						{
							switch (animation)
							{
								default:
									action = Utility.Random(4, 3);
									break;
							}

							break;
						}
					case BodyType.Human:
						{
							if (!mob.Mounted)
							{
								action = (int)animation;
							}
							else
							{
								switch (animation)
								{
									default:
										/* default case makes all these cases redundant
										case WeaponAnimation.Wrestle:
										case WeaponAnimation.Bash1H:
										case WeaponAnimation.Pierce1H:
										case WeaponAnimation.Slash1H:
										*/
										action = 26;
										break;
									case WeaponAnimation.Bash2H:
									case WeaponAnimation.Pierce2H:
									case WeaponAnimation.Slash2H:
										action = 29;
										break;
									case WeaponAnimation.ShootBow:
										action = 27;
										break;
									case WeaponAnimation.ShootXBow:
										action = 28;
										break;
								}
							}

							break;
						}
					default:
						return;
				}

				mob.Animate(action, 7, 1, true, false, 0);
			}

			public static void ADDTOLIST(TriggerObject trigObject, ArrayList list, object toAdd)
			{
				if (list != null)
				{
					list.Add(toAdd);
				}
			}

			public static ArrayList NEWLIST(TriggerObject trigObject)
			{
				return new ArrayList();
			}

			public static void REMOVEFROMLISTAT(TriggerObject trigObject, ArrayList list, int index)
			{
				list.RemoveAt(index);
			}

			public static void REMOVEFROMLIST(TriggerObject trigObject, ArrayList list, object toRemove)
			{
				list.Remove(toRemove);
			}

			// integer distance (smallest square radius that contains both points)
			public static int DISTANCE(TriggerObject trigObject, IPoint2D p1, IPoint2D p2)
			{
				return Math.Max(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
			}

			public static double DISTANCESQRT(TriggerObject trigObject, IPoint2D p1, IPoint2D p2)
			{
				if (p1 == null || p2 == null)
				{
					return 0;
				}

				double xdiff = p1.X - p2.X;
				double ydiff = p1.Y - p2.Y;
				return Math.Sqrt(xdiff * xdiff + ydiff * ydiff);
			}

			public static void BCAST(TriggerObject trigObject, object message)
			{
				BCAST(trigObject, message, 0x482);
			}

			public static void BCAST(TriggerObject trigObject, object message, int hue)
			{
				BCAST(trigObject, message, hue, null);
			}

			public static void BCAST(TriggerObject trigObject, object message, int hue, string accesslevel)
			{
				AccessLevel access = AccessLevel.Player;

				if (accesslevel != null)
				{
					accesslevel = accesslevel.ToLower();
				}

				switch (accesslevel)
				{
					case "gamemaster":
						access = AccessLevel.GameMaster;
						break;
					case "seer":
						access = AccessLevel.Seer;
						break;
					case "lead":
						access = AccessLevel.Lead;
						break;
					case "developer":
						access = AccessLevel.Developer;
						break;
				}

				CommandHandlers.BroadcastMessage(access, hue, message != null ? message.ToString() : "null");
			}

			public static int CEILING(TriggerObject trigObject, object input)
			{
				return input == null ? 0 : Convert.ToInt32(Math.Ceiling(Convert.ToDouble(input)));
			}

			public static int FLOOR(TriggerObject trigObject, object input)
			{
				return input == null ? 0 : Convert.ToInt32(Math.Floor(Convert.ToDouble(input)));
			}

			public static int ROUND(TriggerObject trigObject, object input)
			{
				return input == null ? 0 : Convert.ToInt32(Math.Round(Convert.ToDouble(input)));
			}

			public static UInt64 UINT64(TriggerObject trigObject, object input)
			{
				return input == null ? 0 : Convert.ToUInt64(input);
			}

			public static double DOUBLE(TriggerObject trigObject, object input)
			{
				return input == null ? 0 : Convert.ToDouble(input);
			}

			public static float FLOAT(TriggerObject trigObject, object input)
			{
				return input == null ? 0 : Convert.ToSingle(input);
			}

			public static int INT(TriggerObject trigObject, object input)
			{
				// need special case for serial
				return input == null ? 0 : input is Serial ? ((Serial)input).Value : Convert.ToInt32(input);
			}

			public static string STRING(TriggerObject trigObject, object input)
			{
				return input == null ? "null" : input.ToString();
			}

			public static bool ADDTOPACK(TriggerObject trigObject, Mobile mob, Item item)
			{
				return ADDTOPACK(trigObject, mob, item, 1.0);
			}

			public static bool ADDTOPACK(TriggerObject trigObject, Mobile mob, Item item, double probability)
			{
				return mob != null && item != null && Utility.RandomDouble() <= probability && mob.AddToBackpack(item);
			}

			public static bool ADDTOCONTAINER(TriggerObject trigObject, Container container, Item item)
			{
				return container != null && item != null && ADDTOCONTAINER(trigObject, container, item, 1.0);
			}

			public static bool ADDTOCONTAINER(TriggerObject trigObject, Container container, Item item, double probability)
			{
				if (container == null || item == null)
				{
					return false;
				}

				if (Utility.RandomDouble() <= probability)
				{
					container.DropItem(item);
					return true;
				}

				return false;
			}

			public static bool STRINGCONTAINSIGNORECASE(TriggerObject trigObject, string input, string searchterm)
			{
				return input != null && searchterm != null &&
					   STRINGCONTAINSSTRICT(trigObject, input.ToLower(), searchterm.ToLower());
			}

			public static bool STRINGCONTAINS(TriggerObject trigObject, string input, string searchterm)
			{
				return input != null && searchterm != null &&
					   STRINGCONTAINSSTRICT(trigObject, input.ToLower(), searchterm.ToLower());
			}

			public static bool STRINGCONTAINSSTRICT(TriggerObject trigObject, string input, string searchterm)
			{
				return input != null && searchterm != null && input.Contains(searchterm);
			}

			public static bool INRANGE(TriggerObject trigObject, IPoint2D p1, IPoint2D p2, int range)
			{
				return p1 != null && p2 != null && p1.X >= p2.X - range && p1.X <= p2.X + range && p1.Y >= p2.Y - range &&
					   p1.Y <= p2.Y + range;
			}

			public static object THIS(TriggerObject trigObject)
			{
				return trigObject.This;
			}

			public static object TRIGMOB(TriggerObject trigObject)
			{
				return trigObject.TrigMob;
			}

			public static object TRIGITEM(TriggerObject trigObject)
			{
				return trigObject.TrigItem;
			}

			public static object TARGETED(TriggerObject trigObject)
			{
				return trigObject.Targeted;
			}

			public static object DAMAGE(TriggerObject trigObject)
			{
				return trigObject.Damage;
			}

			public static object SPEECH(TriggerObject trigObject)
			{
				return trigObject.Speech;
			}

			public static object SPAWNED(TriggerObject trigObject)
			{
				return trigObject.Spawn;
			}

			public static object SCRIPT(TriggerObject trigObject)
			{
				return trigObject.Script;
			}

			public static object SPELL(TriggerObject trigObject)
			{
				return trigObject.Spell;
			}

			public static object SKILLNAME(TriggerObject trigObject)
			{
				return trigObject.SkillName;
			}

			public static object SKILLVALUE(TriggerObject trigObject)
			{
				return trigObject.SkillValue;
			}

			public static object GUMPID(TriggerObject trigObject)
			{
				return trigObject.GumpID;
			}

			public static void SAY(TriggerObject trigObject, object obj, object text)
			{
				SAY(trigObject, obj, text, 1.0d);
			}

			public static void SAY(TriggerObject trigObject, object obj, object text, double probability)
			{
				if (Utility.RandomDouble() > probability)
				{
					return;
				}

				if (text == null)
				{
					text = "null";
				}

				if (obj is Mobile)
				{
					((Mobile)obj).Say(text.ToString());
				}
				else if (obj is Item)
				{
					((Item)obj).PublicOverheadMessage(MessageType.Regular, 0x3b2, false, text.ToString());
				}
				else
				{
					throw new UberScriptException("SAY obj argument was not Mobile or Item!");
				}
			}

			public static void SENDMSG(TriggerObject trigObject, object obj, object text)
			{
				SENDMSG(trigObject, obj, text, 0x3b2, 1.0);
			}

			public static void SENDMSG(TriggerObject trigObject, object obj, object text, int hue)
			{
				SENDMSG(trigObject, obj, text, hue, 1.0);
			}

			public static void SENDMSG(TriggerObject trigObject, object obj, object text, int hue, double probability)
			{
				if (Utility.RandomDouble() > probability)
				{
					return;
				}

				if (obj is Mobile)
				{
					((Mobile)obj).SendMessage(hue, text.ToString());
				}
				else
				{
					throw new UberScriptException("SENDMSG obj argument was not Mobile!");
				}
			}

			public static void LOCALMSG(TriggerObject trigObject, object obj, object text)
			{
				LOCALMSG(trigObject, obj, text, 0x3b2, 1.0);
			}

			public static void LOCALMSG(TriggerObject trigObject, object obj, object text, int hue)
			{
				LOCALMSG(trigObject, obj, text, hue, 1.0);
			}

			public static void LOCALMSG(TriggerObject trigObject, object obj, object text, int hue, double probability)
			{
				if (Utility.RandomDouble() > probability)
				{
					return;
				}

				if (text == null)
				{
					text = "null";
				}

				if (obj is Mobile)
				{
					((Mobile)obj).LocalOverheadMessage(MessageType.Regular, hue, false, text.ToString());
				}
				else
				{
					throw new UberScriptException("LOCALMSG obj argument was not Mobile!");
				}
			}

			public static void MSG(TriggerObject trigObject, object obj, object text)
			{
				MSG(trigObject, obj, text, 0x3b2, 1.0);
			}

			public static void MSG(TriggerObject trigObject, object obj, object text, int hue)
			{
				MSG(trigObject, obj, text, hue, 1.0);
			}

			public static void MSG(TriggerObject trigObject, object obj, object text, int hue, double probability)
			{
				if (Utility.RandomDouble() > probability)
				{
					return;
				}

				if (text == null)
				{
					text = "null";
				}

				if (obj is Mobile)
				{
					((Mobile)obj).PublicOverheadMessage(MessageType.Regular, hue, false, text.ToString());
				}
				else if (obj is Item)
				{
					((Item)obj).PublicOverheadMessage(MessageType.Regular, hue, false, text.ToString());
				}
				else
				{
					throw new UberScriptException("MSG obj argument was not Mobile or Item!");
				}
			}

			public static Point2D POINT2D(TriggerObject trigObject, int x, int y)
			{
				return new Point2D(x, y);
			}

			public static Point3D POINT3D(TriggerObject trigObject, int x, int y, int z)
			{
				return new Point3D(x, y, z);
			}

			public static Type TYPE(TriggerObject trigObject, object objToCheck)
			{
				if (objToCheck == null)
				{
					return null;
				}

				if (objToCheck is string)
				{
					// check whether it is the name of a type
					Type test = ScriptCompiler.FindTypeByName((string)objToCheck);

					if (test != null)
					{
						return test;
					}
				}

				return objToCheck.GetType();
			}

			public static bool IS(TriggerObject trigObject, object objToCheck, object typeToCheckAgainst)
			{
				if (objToCheck == null)
				{
					return typeToCheckAgainst == null || typeToCheckAgainst.ToString() == "null";
				}

				Type type = typeToCheckAgainst as Type;

				if (typeToCheckAgainst is string)
				{
					// check whether it is the name of a type
					type = ScriptCompiler.FindTypeByName((string)typeToCheckAgainst);
				}

				if (type == null)
				{
					type = typeToCheckAgainst.GetType();
				}

				return type.IsInstanceOfType(objToCheck);
			}

			public static void KILL(TriggerObject trigObject, Mobile toKill)
			{
				if (toKill != null)
				{
					toKill.Kill();
				}
			}

			public static void DELETE(TriggerObject trigObject, object objToDelete)
			{
				if (objToDelete is BaseCreature)
				{
					((BaseCreature)objToDelete).Delete();
				}
				else if (objToDelete is Item)
				{
					((Item)objToDelete).Delete();
				}
				else if (objToDelete is XmlAttachment)
				{
					((XmlAttachment)objToDelete).Delete();
				}
				else
				{
					throw new UberScriptException(objToDelete + " was not BaseCreature, Item, or XmlAttachment!  Cannot delete!");
				}
			}

			public static bool RANDOMBOOL(TriggerObject trigObj)
			{
				return Utility.RandomBool();
			}

			public static int RANDOMMINMAX(TriggerObject trigObj, int min, int max)
			{
				return Utility.RandomMinMax(min, max);
			}

			public static int RANDOM(TriggerObject trigObj, int max)
			{
				return Utility.Random(max);
			}

			public static double RANDOMDOUBLE(TriggerObject trigObject)
			{
				return Utility.RandomDouble();
			}

			public static object RANDOMFROMLIST(TriggerObject trigObject, ArrayList list)
			{
				return list != null && list.Count != 0 ? list[Utility.Random(list.Count)] : null;
			}

			public static string GETACCOUNTTAG(TriggerObject trigObject, Mobile mob, string tagname)
			{
				if (mob != null && !mob.Deleted)
				{
					if (mob is BaseCreature && mob.NetState != null)
					{
						mob = GETPOSSESSOR(trigObject, mob);

						if (mob == null)
						{
							throw new UberScriptException(
								"GETACCOUNTTAG error: tried to get account tag for a possessed mob but could not find the possessing playermobile");
						}
					}

					Account acct = mob.Account as Account;

					if (acct != null)
					{
						return acct.GetTag(tagname);
					}
				}

				throw new UberScriptException("Invalid GETACCOUNTTAG call!");
			}

			public static void SETACCOUNTTAG(TriggerObject trigObject, Mobile mob, string tagname, string value)
			{
				if (mob != null && !mob.Deleted)
				{
					if (mob.Account == null)
					{
						if (mob is BaseCreature && mob.NetState != null)
						{
							mob = GETPOSSESSOR(trigObject, mob);

							if (mob == null)
							{
								throw new UberScriptException(
									"SETACCOUNTTAG error: tried to set account tag for a possessed mob but could not find the possessing playermobile");
							}
						}
						else
						{
							throw new UberScriptException("SETACCOUNTTAG error: tried to set account tag for a mob without an account!");
						}
					}

					Account acct = mob.Account as Account;

					if (acct != null)
					{
						if (value == null)
						{
							acct.RemoveTag(tagname);
						}
						else
						{
							acct.SetTag(tagname, value);
						}

						return;
					}
				}

				throw new UberScriptException("Invalid SETACCOUNTTAG call!");
			}

			public static void SOUND(TriggerObject trigObject, IPoint3D entity, int sound)
			{
				if (entity != null && sound >= 0)
				{
					Effects.PlaySound(entity, (entity is IEntity ? ((IEntity)entity).Map : Map.Felucca), sound);
				}
			}

			public static void LOCALSOUND(TriggerObject trigObject, Mobile mob, int soundID)
			{
				if (mob != null && soundID >= 0)
				{
					mob.PlaySound(soundID);
				}
			}

			public static void EFFECT(TriggerObject trigObject, int effect, int duration, IPoint3D p)
			{
				EFFECT(trigObject, effect, duration, p.X, p.Y, p.Z, 0);
			}

			public static void EFFECT(TriggerObject trigObject, int effect, int duration, IPoint3D p, int range)
			{
				EFFECT(trigObject, effect, duration, p.X, p.Y, p.Z, range);
			}

			public static void EFFECT(TriggerObject trigObject, int effect, int duration, int x, int y, int z)
			{
				EFFECT(trigObject, effect, duration, x, y, z, 0);
			}

			public static void EFFECT(TriggerObject trigObject, int effect, int duration, int x, int y, int z, int range)
			{
				// syntax is EFFECT(itemid,duration,x,y,z,range)
				// try to get the effect argument
				// some interesting effects are explosion(14013,15), sparkle(14155,15), explosion2(14000,13)
				Map map = Map.Felucca;

				if (trigObject.This != null)
				{
					if (trigObject.This is Mobile)
					{
						map = ((Mobile)trigObject.This).Map;
					}
					else if (trigObject.This is Item)
					{
						map = ((Item)trigObject.This).Map;
					}
				}

				EFFECT(trigObject, map, effect, duration, x, y, z, range);
			}

			public static void EFFECT(
				TriggerObject trigObject, Map map, int effect, int duration, int x, int y, int z, int range)
			{
				if (effect < 0 || map == null || map == Map.Internal)
				{
					return;
				}

				for (int xOffset = -range; xOffset <= range; xOffset++)
				{
					for (int yOffset = -range; yOffset <= range; yOffset++)
					{
						if (Math.Sqrt(xOffset * xOffset + yOffset * yOffset) <= range)
						{
							Effects.SendLocationEffect(new Point3D(x + xOffset, y + yOffset, z), map, effect, duration);
						}
					}
				}
			}

			public static ArrayList GETNEARBYITEMS(TriggerObject trigObject, IPoint3D from, int range)
			{
				Map map = from is IEntity ? ((IEntity)from).Map : Map.Felucca;

				return GETNEARBYITEMS(trigObject, from, range, map);
			}

			public static ArrayList GETNEARBYITEMS(TriggerObject trigObject, IPoint3D from, int range, Map map)
			{
				if (from == null || map == null)
				{
					return new ArrayList();
				}

				var enumerator = map.GetItemsInRange(new Point3D(from), range);

				ArrayList output = new ArrayList(50);

				foreach (Item nearby in enumerator)
				{
					output.Add(nearby);
				}

				enumerator.Free();
				return output;
			}

			public static ArrayList GETITEMSINBOUNDS(TriggerObject trigObject, IPoint2D start, IPoint2D end, Map map)
			{
				if (start == null || end == null || map == null || map == Map.Internal)
				{
					return new ArrayList();
				}

				var enumerator = map.GetItemsInBounds(new Rectangle2D(start, end));

				ArrayList output = new ArrayList(50);

				foreach (Item nearby in enumerator)
				{
					output.Add(nearby);
				}

				enumerator.Free();
				return output;
			}

			public static ArrayList GETNEARBYMOBS(TriggerObject trigObject, IPoint3D from, int range)
			{
				Map map = from is IEntity ? ((IEntity)from).Map : Map.Felucca;

				return GETNEARBYMOBS(trigObject, from, range, map);
			}

			public static ArrayList GETNEARBYMOBS(TriggerObject trigObject, IPoint3D from, int range, Map map)
			{
				if (from == null || map == null || map == Map.Internal)
				{
					return new ArrayList();
				}

				var enumerator = map.GetMobilesInRange(new Point3D(from), range);

				ArrayList output = new ArrayList(50);

				foreach (Mobile nearby in enumerator)
				{
					output.Add(nearby);
				}

				enumerator.Free();
				return output;
			}

			public static ArrayList GETMOBSINBOUNDS(TriggerObject trigObject, IPoint2D start, IPoint2D end, Map map)
			{
				if (start == null || end == null || map == null || map == Map.Internal)
				{
					return new ArrayList();
				}

				var enumerator = map.GetMobilesInBounds(new Rectangle2D(start, end));

				ArrayList output = new ArrayList(50);

				foreach (Mobile nearby in enumerator)
				{
					output.Add(nearby);
				}

				enumerator.Free();
				return output;
			}

			public static ArrayList GETNEARBYOBJS(TriggerObject trigObject, IPoint3D from, int range)
			{
				Map map = from is IEntity ? ((IEntity)from).Map : Map.Felucca;

				return GETNEARBYOBJS(trigObject, from, range, map);
			}

			public static ArrayList GETNEARBYOBJS(TriggerObject trigObject, IPoint3D from, int range, Map map)
			{
				var enumerator = map.GetObjectsInRange(new Point3D(from), range);

				ArrayList output = new ArrayList(50);

				foreach (IEntity nearby in enumerator)
				{
					output.Add(nearby);
				}

				enumerator.Free();
				return output;
			}

			public static ArrayList GETOBJSINBOUNDS(TriggerObject trigObject, IPoint2D start, IPoint2D end, Map map)
			{
				var enumerator = map.GetObjectsInBounds(new Rectangle2D(start, end));

				ArrayList output = new ArrayList(50);

				foreach (IEntity nearby in enumerator)
				{
					output.Add(nearby);
				}

				enumerator.Free();
				return output;
			}

			public static void REMOVESUPPLIES(TriggerObject trigObject)
			{
				foreach (Item item in World.Items.Values.Where(item => item != null && item.Name == "eventsupply").ToArray())
				{
					item.Delete();
				}
			}

			public static void MEFFECT(TriggerObject trigObject, int effect, int speed, int x, int y, int z, IPoint3D end)
			{
				if (effect > 0 && end != null)
				{
					MEFFECT(trigObject, effect, speed, x, y, z, end.X, end.Y, end.Z, false, false);
				}
			}

			public static void MEFFECT(
				TriggerObject trigObject, int effect, int speed, int x, int y, int z, IPoint3D end, bool fixedDirection)
			{
				if (effect > 0 && end != null)
				{
					MEFFECT(trigObject, effect, speed, x, y, z, end.X, end.Y, end.Z, fixedDirection, false);
				}
			}

			public static void MEFFECT(
				TriggerObject trigObject,
				int effect,
				int speed,
				int x,
				int y,
				int z,
				IPoint3D end,
				bool fixedDirection,
				bool explosion)
			{
				if (effect > 0 && end != null)
				{
					MEFFECT(trigObject, effect, speed, x, y, z, end.X, end.Y, end.Z, fixedDirection, explosion);
				}
			}

			public static void MEFFECT(TriggerObject trigObject, int effect, int speed, IPoint3D start, int x2, int y2, int z2)
			{
				if (effect > 0 && start != null)
				{
					MEFFECT(trigObject, effect, speed, start.X, start.Y, start.Z, x2, y2, z2, false, false);
				}
			}

			public static void MEFFECT(
				TriggerObject trigObject, int effect, int speed, IPoint3D start, int x2, int y2, int z2, bool fixedDirection)
			{
				if (effect > 0 && start != null)
				{
					MEFFECT(trigObject, effect, speed, start.X, start.Y, start.Z, x2, y2, z2, fixedDirection, false);
				}
			}

			public static void MEFFECT(
				TriggerObject trigObject,
				int effect,
				int speed,
				IPoint3D start,
				int x2,
				int y2,
				int z2,
				bool fixedDirection,
				bool explosion)
			{
				if (effect > 0 && start != null)
				{
					MEFFECT(trigObject, effect, speed, start.X, start.Y, start.Z, x2, y2, z2, fixedDirection, explosion);
				}
			}

			public static void MEFFECT(TriggerObject trigObject, int effect, int speed, IPoint3D start, IPoint3D end)
			{
				if (effect > 0 && start != null && end != null)
				{
					MEFFECT(trigObject, effect, speed, start.X, start.Y, start.Z, end.X, end.Y, end.Z, false, false);
				}
			}

			public static void MEFFECT(
				TriggerObject trigObject, int effect, int speed, IPoint3D start, IPoint3D end, bool fixedDirection)
			{
				if (effect > 0 && start != null && end != null)
				{
					MEFFECT(trigObject, effect, speed, start.X, start.Y, start.Z, end.X, end.Y, end.Z, fixedDirection, false);
				}
			}

			public static void MEFFECT(
				TriggerObject trigObject, int effect, int speed, IPoint3D start, IPoint3D end, bool fixedDirection, bool explosion)
			{
				if (effect > 0 && start != null && end != null)
				{
					MEFFECT(trigObject, effect, speed, start.X, start.Y, start.Z, end.X, end.Y, end.Z, fixedDirection, explosion);
				}
			}

			public static void MEFFECT(
				TriggerObject trigObject, int effect, int speed, int x, int y, int z, int x2, int y2, int z2)
			{
				if (effect > 0)
				{
					MEFFECT(trigObject, effect, speed, x, y, z, x2, y2, z2, false, false);
				}
			}

			public static void MEFFECT(
				TriggerObject trigObject, int effect, int speed, int x, int y, int z, int x2, int y2, int z2, bool fixedDirection)
			{
				if (effect > 0)
				{
					MEFFECT(trigObject, effect, speed, x, y, z, x2, y2, z2, fixedDirection, false);
				}
			}

			public static void MEFFECT(
				TriggerObject trigObject,
				int effect,
				int speed,
				int x,
				int y,
				int z,
				int x2,
				int y2,
				int z2,
				bool fixedDirection,
				bool explosion)
			{
				if (effect <= 0)
				{
					return;
				}

				//syntax is MEFFECT,itemid,speed,x,y,z,x2,y2,z2
				int duration = 10;
				Point3D eloc1 = new Point3D(x, y, z + 10);

				// offset by 10 because it always looks so bad going out of anythign--it drags along the floor
				Point3D eloc2 = new Point3D(x2, y2, z2 + 10);

				Map emap = Map.Felucca;

				if (trigObject.This != null)
				{
					if (trigObject.This is Mobile)
					{
						emap = ((Mobile)trigObject.This).Map;
					}

					if (trigObject.This is Item)
					{
						emap = ((Item)trigObject.This).Map;
					}
				}

				if (effect >= 0 && emap != Map.Internal)
				{
					// might want to implement those last booleans!
					Effects.SendPacket(
						eloc1,
						emap,
						new HuedEffect(EffectType.Moving, -1, -1, effect, eloc1, eloc2, speed, duration, fixedDirection, explosion, 0, 0));
				}
			}

			public static void CURE(TriggerObject trigObj, IPoint2D target)
			{
				if (target != null)
				{
					CURE(trigObj, target, null, -1, false, false);
				}
			}

			public static void CURE(TriggerObject trigObj, IPoint2D target, Mobile from)
			{
				if (target != null)
				{
					CURE(trigObj, target, from, -1, false, false);
				}
			}

			public static void CURE(TriggerObject trigObj, IPoint2D target, int range)
			{
				if (target != null)
				{
					CURE(trigObj, target, null, range, true, false);
				}
			}

			public static void CURE(TriggerObject trigObj, IPoint2D target, Mobile from, int range)
			{
				if (target != null)
				{
					CURE(trigObj, target, from, range, true, false);
				}
			}

			public static void CURE(
				TriggerObject trigObj, IPoint2D target, Mobile from, int range, bool playeronly, bool mobsonly)
			{
				if (target == null)
				{
					return;
				}

				// apply the damage to all mobs within range if range is > 0
				Map map = (target is IEntity) ? ((IEntity)target).Map : Map.Felucca;
				Mobile targetMob = target as Mobile;

				// if it's a death trigger, then don't do damage to trigObj.This (if you do, it results in an infinite loop!)
				Mobile dyingMob = (trigObj.TrigName == TriggerName.onDeath ? trigObj.This as Mobile : null);

				if (range >= 0)
				{
					var rangelist = map.GetMobilesInRange(new Point3D(target, 0), range);
					// don't do damage in a list like this--if mobs are killed it screws up the enumerator!
					// ... copy the list first
					var affected = new List<Mobile>();

					affected.AddRange(
						rangelist.OfType<Mobile>()
								 .Where(p => p.Alive && (p is PlayerMobile && !mobsonly) || (p is BaseCreature && !playeronly)));

					rangelist.Free();

					foreach (Mobile mob in affected.Where(mob => mob != dyingMob))
					{
						if (from != null)
						{
							if (from.CanBeBeneficial(mob))
							{
								from.DoBeneficial(mob);
								mob.CurePoison(from);
							}
						}
						else
						{
							mob.CurePoison(mob);
						}
					}
				} // range of -1 means hit only the target
				else if ((targetMob is PlayerMobile && !mobsonly) || (targetMob is BaseCreature && !playeronly))
				{
					if (targetMob.Alive && targetMob != dyingMob)
					{
						if (from != null)
						{
							if (from.CanBeBeneficial(targetMob))
							{
								from.DoBeneficial(targetMob);
								targetMob.CurePoison(from);
							}
						}
						else
						{
							targetMob.CurePoison(targetMob);
						}
					}
				}
			}

			public static void POISON(TriggerObject trigObj, IPoint2D target, int level)
			{
				POISON(trigObj, target, null, level, -1, false, false);
			}

			public static void POISON(TriggerObject trigObj, IPoint2D target, Mobile from, int level)
			{
				POISON(trigObj, target, from, level, -1, false, false);
			}

			public static void POISON(TriggerObject trigObj, IPoint2D target, int level, int range)
			{
				POISON(trigObj, target, null, level, range, true, false);
			}

			public static void POISON(TriggerObject trigObj, IPoint2D target, Mobile from, int level, int range)
			{
				POISON(trigObj, target, from, level, range, true, false);
			}

			public static void POISON(
				TriggerObject trigObj, IPoint2D target, Mobile from, int level, int range, bool playeronly, bool mobsonly)
			{
				if (target == null)
				{
					return;
				}

				// apply the damage to all mobs within range if range is > 0
				Map map = (target is IEntity) ? ((IEntity)target).Map : Map.Felucca;
				Mobile targetMob = target as Mobile;

				// if it's a death trigger, then don't do damage to trigObj.This (if you do, it results in an infinite loop!)
				Mobile dyingMob = (trigObj.TrigName == TriggerName.onDeath ? trigObj.This as Mobile : null);

				if (range >= 0)
				{
					var rangelist = map.GetMobilesInRange(new Point3D(target, 0), range);
					// don't do damage in a list like this--if mobs are killed it screws up the enumerator!
					// ... copy the list first
					var affected =
						rangelist.OfType<Mobile>()
								 .Where(p => p.Alive && (p is PlayerMobile && !mobsonly) || (p is BaseCreature && !playeronly))
								 .ToList();

					rangelist.Free();

					foreach (Mobile mob in affected.Where(mob => mob != dyingMob))
					{
						if (from != null)
						{
							if (from.CanBeHarmful(mob))
							{
								from.DoHarmful(mob);
								mob.ApplyPoison(from, Poison.GetPoison(level));
							}
						}
						else
						{
							mob.ApplyPoison(mob, Poison.GetPoison(level));
						}
					}
				} // range of -1 means hit only the target
				else if ((targetMob is PlayerMobile && !mobsonly) || (targetMob is BaseCreature && !playeronly))
				{
					if (targetMob.Alive && targetMob != dyingMob)
					{
						if (from != null)
						{
							if (from.CanBeHarmful(targetMob))
							{
								from.DoHarmful(targetMob);
								targetMob.ApplyPoison(from, Poison.GetPoison(level));
							}
						}
						else
						{
							targetMob.ApplyPoison(targetMob, Poison.GetPoison(level));
						}
					}
				}
			}

			public static void DODAMAGE(TriggerObject trigObj, IPoint2D target, int amount)
			{
				if (target != null)
				{
					DODAMAGE(trigObj, target, null, amount, -1, false, false);
				}
			}

			public static void DODAMAGE(TriggerObject trigObj, IPoint2D target, Mobile from, int amount)
			{
				if (target != null)
				{
					DODAMAGE(trigObj, target, from, amount, -1, false, false);
				}
			}

			public static void DODAMAGE(TriggerObject trigObj, IPoint2D target, int amount, int range)
			{
				if (target != null)
				{
					DODAMAGE(trigObj, target, null, amount, range, true, false);
				}
			}

			public static void DODAMAGE(TriggerObject trigObj, IPoint2D target, Mobile from, int amount, int range)
			{
				if (target != null)
				{
					DODAMAGE(trigObj, target, from, amount, range, true, false);
				}
			}

			public static void DODAMAGE(
				TriggerObject trigObj, IPoint2D target, Mobile from, int amount, int range, bool playeronly, bool mobsonly)
			{
				if (target != null)
				{
					DODAMAGE(trigObj, target, from, amount, range, playeronly, mobsonly, -1);
				}
			}

			public static void DODAMAGE(
				TriggerObject trigObj,
				IPoint2D target,
				Mobile from,
				int amount,
				int range,
				bool playeronly,
				bool mobsonly,
				int zRange)
			{
				if (target == null)
				{
					return;
				}

				// apply the damage to all mobs within range if range is > 0
				Map map = (target is IEntity) ? ((IEntity)target).Map : Map.Felucca;
				Mobile targetMob = target as Mobile;

				// if it's a death trigger, then don't do damage to trigObj.This (if you do, it results in an infinite loop!)
				Mobile dyingMob = (trigObj.TrigName == TriggerName.onDeath ? trigObj.This as Mobile : null);

				if (range >= 0)
				{
					Point3D target3D = (target is IPoint3D) ? new Point3D((IPoint3D)target) : new Point3D(target, 0);

					var rangelist = map.GetMobilesInRange(target3D, range);
					// don't do damage in a list like this--if mobs are killed it screws up the enumerator!
					// ... copy the list first
					var affected = new List<Mobile>();
					bool inZRange;

					foreach (Mobile p in rangelist)
					{
						if (zRange == -1)
						{
							inZRange = true;
						}
						else
						{
							inZRange = Math.Abs(target3D.Z - p.Z) <= zRange;
						}

						if (!inZRange)
						{
							continue;
						}

						if (p.Alive && (p is PlayerMobile && !mobsonly) || (p is BaseCreature && !playeronly))
						{
							affected.Add(p);
						}
					}

					rangelist.Free();

					foreach (Mobile mob in affected.Where(mob => mob != dyingMob))
					{
						if (from != null)
						{
							if (from.CanBeHarmful(mob))
							{
								from.DoHarmful(mob);
								mob.Damage(amount, from);
							}
						}
						else
						{
							mob.Damage(amount, mob);
						}
					}
				} // range of -1 means hit only the target
				else if ((targetMob is PlayerMobile && !mobsonly) || (targetMob is BaseCreature && !playeronly))
				{
					if (targetMob.Alive && targetMob != dyingMob)
					{
						if (from != null)
						{
							if (from.CanBeHarmful(targetMob))
							{
								from.DoHarmful(targetMob);
								targetMob.Damage(amount, from);
							}
						}
						else
						{
							targetMob.Damage(amount, targetMob);
						}
					}
				}
			}

			public static void RESURRECT(TriggerObject trigObj, object target)
			{
				if (target is Mobile)
				{
					((Mobile)target).Resurrect();
				}
			}

			public static void BSOUND(TriggerObject trigObj, int soundid)
			{
				if (soundid >= 0)
				{
					// broadcast a sound to all players
					BaseXmlSpawner.BroadcastSound(AccessLevel.Player, soundid);
				}
			}

			public static ArrayList GETATTACHMENTS(TriggerObject trigObj, IEntity obj, object attachmenttype)
			{
				Type type = attachmenttype as Type;

				if (type == null)
				{
					type = ScriptCompiler.FindTypeByName(attachmenttype.ToString());

					if (type == null || !type.IsSubclassOf(typeof(XmlAttachment)))
					{
						return null;
					}
				}

				return XmlAttach.FindAttachments(obj, type);
			}

			public static XmlAttachment GETATTACHMENT(TriggerObject trigObj, IEntity obj, object attachmenttype)
			{
				return GETATTACHMENT(trigObj, obj, attachmenttype, null);
			}

			public static XmlAttachment GETATTACHMENT(TriggerObject trigObj, IEntity obj, object attachmenttype, string name)
			{
				//Type type = SpawnerType.GetType(name);
				Type type = attachmenttype as Type;

				if (type == null)
				{
					type = ScriptCompiler.FindTypeByName(attachmenttype.ToString());

					if (type == null || !(typeof(XmlAttachment).IsAssignableFrom(type)))
					{
						return null;
					}
				}

				ArrayList attachments = name == null
											? XmlAttach.FindAttachments(obj, type)
											: XmlAttach.FindAttachments(obj, type, name);

				return attachments == null || attachments.Count == 0 ? null : attachments[0] as XmlAttachment;
			}

			public static XmlAttachment NEWATTACHMENT(TriggerObject trigObj, string attachmenttype)
			{
				return NEWATTACHMENT(trigObj, attachmenttype, null);
			}

			public static XmlAttachment NEWATTACHMENT(TriggerObject trigObj, string attachmenttype, string name)
			{
				attachmenttype = attachmenttype.ToLower().Trim();

				XmlAttachment attachment = null;

				switch (attachmenttype)
				{
					case "xmlvalue":
						attachment = new XmlValue("", 0);
						break;
					case "xmllocalvariable":
						attachment = new XmlLocalVariable("");
						break;
					case "xmlscript":
						attachment = new XmlScript();
						break;
					case "xmlteam":
						attachment = new XmlTeam();
						break;
					case "xmldouble":
						attachment = new XmlDouble("", 0.0);
						break;
					case "xmlgroup":
						attachment = new XmlGroup();
						break;
					case "xmlslayer":
						attachment = new XmlSlayer("orcslaying", name);
						break;
					case "xmldate":
						attachment = new XmlDate("");
						break;
					case "xmlcorpseaction":
						attachment = new XmlCorpseAction();
						break;
					case "xmldeathaction":
						attachment = new XmlDeathAction();
						break;
					case "xmluse":
						attachment = new XmlUse();
						break;
					case "xmlonhit":
						attachment = new XmlOnHit();
						break;
					case "xmladdfame":
						attachment = new XmlAddFame(0);
						break;
					case "xmladdkarma":
						attachment = new XmlAddKarma(0);
						break;
					case "xmldex":
						attachment = new XmlDex();
						break;
					case "xmldialog":
						attachment = new XmlDialog();
						break;
					case "xmlenemymastery":
						attachment = new XmlEnemyMastery("");
						break;
					case "xmlfire":
						attachment = new XmlFire(1);
						break;
					case "xmlfreeze":
						attachment = new XmlFreeze();
						break;
					case "xmlhue":
						attachment = new XmlHue(0);
						break;
					case "xmllifedrain":
						attachment = new XmlLifeDrain(1);
						break;
					case "xmllightning":
						attachment = new XmlLightning(1);
						break;
					case "xmlmagicword":
						attachment = new XmlMagicWord();
						break;
					case "xmlmanadrain":
						attachment = new XmlManaDrain(1);
						break;
					case "xmlmessage":
						attachment = new XmlMessage("");
						break;
					case "xmlsaveitem":
						attachment = new XmlSaveItem();
						break;
					case "xmlskill":
						attachment = new XmlSkill("", "wrestling");
						break;
					case "xmlsound":
						attachment = new XmlSound();
						break;
					case "xmlstamdrain":
						attachment = new XmlStamDrain(1);
						break;
					case "xmlstr":
						attachment = new XmlStr();
						break;
					case "xmlint":
						attachment = new XmlInt();
						break;
				}

				if (attachment == null)
				{
					throw new UberScriptException("NEWATTACHMENT error: " + attachmenttype + " is not an available xmlattachment!");
				}

				if (attachment.Name == "" && name == null) // those attachments that require a name
				{
					return attachment;
				}

				attachment.Name = name;

				return attachment;
			}

			public static void ATTACH(TriggerObject trigObj, IEntity target, XmlAttachment xmlattachment)
			{
				Mobile targetMobile = target as Mobile;
				Item targetItem = target as Item;

				if (targetMobile == null && targetItem == null)
				{
					throw new UberScriptException(
						"ATTACH function requires either Mobile or Item target, but had a " + target + " target");
				}

				if (xmlattachment == null)
				{
					throw new UberScriptException("ATTACH function requires non-null xmlattachment!");
				}

				XmlAttach.AttachTo(target, xmlattachment);
			}

			public static void SPECIALFX(TriggerObject trigObj, IEntity m, string effect)
			{
				if (m != null)
				{
					SPECIALFX(trigObj, m, effect, 0);
				}
			}

			public static void SPECIALFX(TriggerObject trigObj, IEntity m, string effect, int effHue)
			{
				if (m == null)
				{
					return;
				}

				if (effHue > 0)
				{
					effHue--; //Adjust the friggin hue to match true effect color
				}

				switch (effect)
				{
						//[s7]
					case "gate":
						{
							Effects.SendLocationParticles(
								EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x1FCB, 10, 14, effHue, 0, 0x1FCB, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x20E);
							Timer.DelayCall(TimeSpan.FromSeconds(0.65), new TimerStateCallback(InternalShowGate), new object[] {m, effHue});
							Timer.DelayCall(TimeSpan.FromSeconds(1.5), new TimerStateCallback(InternalHideGate), new object[] {m, effHue});
						}
						break;
						//[/s7]
					case "flamestrike1":
						{
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
						}
						break;
					case "flamestrike2":
						{
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 6), m.Map, 0x3709, 15, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 6), m.Map, 0x3709, 15, effHue, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
						}
						break;
					case "snow":
						{
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x376A, 15, effHue, 0); //0x47D );
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 492);
						}
						break;
					case "flamestrikelightningbolt":
						{
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
							Effects.SendBoltEffect(m, true, 0);
						}
						break;
					case "sparkle1":
						{
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x375A, 15, effHue, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x213);
						}
						break;
					case "sparkle3":
						{
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x373A, 15, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x373A, 15, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z - 1), m.Map, 0x373A, 15, effHue, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x213);
						}
						break;
					case "explosion":
						{
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x36BD, 15, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x36BD, 15, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x307);
						}
						break;
					case "explosionlightningbolt":
						{
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x36BD, 15, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x36BD, 15, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
							Effects.SendBoltEffect(m, true, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x307);
						}
						break;
					case "defaultrunuo":
						{
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 4), m.Map, 0x3728, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x3728, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z - 4), m.Map, 0x3728, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 4), m.Map, 0x3728, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x3728, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z - 4), m.Map, 0x3728, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 11), m.Map, 0x3728, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 7), m.Map, 0x3728, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 3), m.Map, 0x3728, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3728, 13, effHue, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x228);
						}
						break;
					case "glow":
						{
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z), m.Map, 0x37C4, 15, effHue, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x1E2);
						}
						break;
					case "poisonfield":
						{
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z), m.Map, 0x3915, 15, effHue, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x231);
						}
						break;
					case "fireball":
						{
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 6), m.Map, 0x36D4, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x36D4, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 6), m.Map, 0x36D4, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 8), m.Map, 0x36D4, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x36D4, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 6), m.Map, 0x36D4, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 11), m.Map, 0x36D4, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 8), m.Map, 0x36D4, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 10), m.Map, 0x36D4, 13, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x15E);
						}
						break;
					case "firestorm1": //Added By Nitewender (further modifed by me to carry color effect to timer
						{
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 520);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 525);
							Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X - 1, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x3709, 17, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y - 1, m.Z), m.Map, 0x3709, 17, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);

							new GMHidingStone.FireStormTimer(DateTime.UtcNow, m, effHue, 0, 1).Start();
						}
						break;
					case "firestorm2": //CEO Using above idea, this one does the firestorm outside->in
						{
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 520);
							Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 525);
							Effects.SendLocationEffect(new Point3D(m.X + 5, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X - 5, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y + 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X, m.Y - 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X + 5, m.Y - 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
							Effects.SendLocationEffect(new Point3D(m.X - 5, m.Y + 5, m.Z), m.Map, 0x3709, 17, effHue, 0);

							new GMHidingStone.FireStormTimer(DateTime.UtcNow, m, effHue, 5, -1).Start();
						}
						break;
				}
			}

			private static void InternalHideGate(object arg)
			{
				if (arg == null || !(arg is object[]))
				{
					return;
				}

				var args = (object[])arg;

				if (args.Length < 2)
				{
					return;
				}

				IEntity m = args[0] as IEntity;

				if (m == null)
				{
					return;
				}

				int hue = (int)args[1];

				if (m is Mobile)
				{
					Mobile mob = (Mobile)m;

					mob.Hidden = !mob.Hidden;
					mob.Frozen = false;
				}

				Effects.SendLocationParticles(
					EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x376A, 9, 20, hue, 0, 5042, 0);
				Effects.PlaySound(m.Location, m.Map, 0x201);
			}

			private static void InternalShowGate(object arg)
			{
				if (arg == null || !(arg is object[]))
				{
					return;
				}

				var args = (object[])arg;

				if (args.Length < 2)
				{
					return;
				}

				IEntity m = args[0] as IEntity;

				if (m == null)
				{
					return;
				}

				int hue = (int)args[1];

				if (m is Mobile)
				{
					Effects.SendLocationParticles(
						EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 8148, 9, 20, hue, 0, 8149, 0);
				}
			}
		}
		#endregion
	}
}