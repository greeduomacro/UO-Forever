#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Engines.Conquests;
using Server.Engines.XmlSpawner2;
using Server.Ethics;
using Server.Factions;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using Server.Targeting;
#endregion

namespace Server.SkillHandlers
{
	public class Stealing
	{
		public static void Initialize()
		{
			SkillInfo.Table[33].Callback = OnUse;
		}

		public static readonly bool ClassicMode = true;
		public static readonly bool SuspendOnMurder = true;

		public static bool IsInGuild(Mobile m)
		{
			return (m is PlayerMobile && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild);
		}

		public static bool IsInnocentTo(Mobile from, Mobile to)
		{
			return (Notoriety.Compute(from, to) == Notoriety.Innocent);
		}

		public sealed class StealingTarget : Target
		{
			private readonly Mobile m_Thief;

			public StealingTarget(Mobile thief)
				: base(1, false, TargetFlags.None)
			{
				m_Thief = thief;

				AllowNonlocal = true;
			}

			private Item TryStealItem(Item toSteal, ref bool caught)
			{
				//Close bank box on steal attempt -Adam
				BankBox box = m_Thief.FindBankNoCreate();

				if (box != null && box.Opened)
				{
					box.Close();
					m_Thief.Send(new MobileUpdate(m_Thief));
				}

				Item stolen = null;

				object root = toSteal.RootParent;

				var contParent = toSteal.Parent as Container;

				StealableArtifactsSpawner.StealableInstance si = null;
				if (toSteal.Parent == null || !toSteal.Movable)
				{
					si = StealableArtifactsSpawner.GetStealableInstance(toSteal);
				}

				BaseAddon addon = null;

				if (toSteal is AddonComponent)
				{
					addon = ((AddonComponent)toSteal).Addon;
				}

				bool stealflag = (addon != null && addon.GetSavedFlag(ItemFlags.StealableFlag)) ||
								 toSteal.GetSavedFlag(ItemFlags.StealableFlag);

				if (!IsEmptyHanded(m_Thief))
				{
					m_Thief.SendLocalizedMessage(1005584); // Both hands must be free to steal.
				}
				else if (root is Mobile && ((Mobile)root).Player && IsInnocentTo(m_Thief, (Mobile)root) && !IsInGuild(m_Thief))
				{
					m_Thief.SendLocalizedMessage(1005596); // You must be in the thieves guild to steal from other players.
				}
				else if (SuspendOnMurder && root is Mobile && ((Mobile)root).Player && IsInGuild(m_Thief) && m_Thief.Kills > 0)
				{
					m_Thief.SendLocalizedMessage(502706); // You are currently suspended from the thieves guild.
				}
				else if (root is BaseVendor && ((BaseVendor)root).IsInvulnerable)
				{
					m_Thief.SendLocalizedMessage(1005598); // You can't steal from shopkeepers.
				}
				else if (root is PlayerVendor)
				{
					m_Thief.SendLocalizedMessage(502709); // You can't steal from vendors.
				}
				else if (!m_Thief.CanSee(toSteal))
				{
					m_Thief.SendLocalizedMessage(500237); // Target can not be seen.
				}
				else if (m_Thief.Backpack == null || !m_Thief.Backpack.CheckHold(m_Thief, toSteal, false, true))
				{
					m_Thief.SendLocalizedMessage(1048147); // Your backpack can't hold anything else.
				}
					#region Sigils
				else if (toSteal is Sigil)
				{
					PlayerState pl = PlayerState.Find(m_Thief);
					Faction faction = (pl == null ? null : pl.Faction);

					var sig = (Sigil)toSteal;

					if (!m_Thief.InRange(toSteal.GetWorldLocation(), 1))
					{
						m_Thief.SendLocalizedMessage(502703); // You must be standing next to an item to steal it.
					}
					else if (root != null) // not on the ground
					{
						m_Thief.SendLocalizedMessage(502710); // You can't steal that!
					}
					else if (faction != null)
					{
						if (!m_Thief.CanBeginAction(typeof(IncognitoSpell)))
						{
							m_Thief.SendLocalizedMessage(1010581); //	You cannot steal the sigil when you are incognito
						}
						else if (DisguiseTimers.IsDisguised(m_Thief))
						{
							m_Thief.SendLocalizedMessage(1010583); //	You cannot steal the sigil while disguised
						}
						else if (!m_Thief.CanBeginAction(typeof(PolymorphSpell)))
						{
							m_Thief.SendLocalizedMessage(1010582); //	You cannot steal the sigil while polymorphed
						}
						else if (TransformationSpellHelper.UnderTransformation(m_Thief))
						{
							m_Thief.SendLocalizedMessage(1061622); // You cannot steal the sigil while in that form.
						}
						else if (m_Thief is PlayerMobile && ((PlayerMobile)m_Thief).SavagePaintExpiration > TimeSpan.Zero)
						{
							m_Thief.SendLocalizedMessage(1114352); // You cannot steal the sigil while disguised in savage paint.
						}
						else if (pl.IsLeaving)
						{
							m_Thief.SendLocalizedMessage(1005589); // You are currently quitting a faction and cannot steal the town sigil
						}
						else if (sig.IsBeingCorrupted && sig.LastMonolith.Faction == faction)
						{
							m_Thief.SendLocalizedMessage(1005590); //	You cannot steal your own sigil
						}
						else if (sig.IsPurifying)
						{
							m_Thief.SendLocalizedMessage(1005592); // You cannot steal this sigil until it has been purified
						}
						else if (m_Thief.CheckTargetSkill(SkillName.Stealing, toSteal, 0.0, 0.0))
						{
							if (Sigil.ExistsOn(m_Thief))
							{
								m_Thief.SendLocalizedMessage(1010258);
								//	The sigil has gone back to its home location because you already have a sigil.
							}
							else if (m_Thief.Backpack == null || !m_Thief.Backpack.CheckHold(m_Thief, sig, false, true))
							{
								m_Thief.SendLocalizedMessage(1010259); //	The sigil has gone home because your backpack is full
							}
							else
							{
								if (sig.IsBeingCorrupted)
								{
									sig.GraceStart = DateTime.UtcNow; // begin grace period
								}

								m_Thief.SendLocalizedMessage(1010586); // YOU STOLE THE SIGIL!!!   (woah, calm down now)

								if (sig.LastMonolith != null && sig.LastMonolith.Sigil != null)
								{
									sig.LastMonolith.Sigil = null;
									sig.LastStolen = DateTime.UtcNow;
								}

								switch (PvPController._SigilAnnounceStolen)
								{
									case PvPController.SigilStolenAnnouncing.All:
										{
											foreach (Faction factionToBCast in Faction.Factions)
											{
												List<PlayerState> members = factionToBCast.Members;

												if (sig.Corrupted != null)
												{
													if (sig.Corrupted == factionToBCast)
													{
														foreach (PlayerState member in members)
														{
															member.Mobile.SendMessage(
																factionToBCast.Definition.HueBroadcast,
																"The {0} have stolen the {1} sigil from your faction stronghold!",
																faction.Definition.FriendlyName,
																sig.Town.Definition.FriendlyName);
														}
													}
													else
													{
														foreach (PlayerState member in members)
														{
															member.Mobile.SendMessage(
																factionToBCast.Definition.HueBroadcast,
																"The {0} have stolen the {1} sigil from the {2} stronghold!",
																faction.Definition.FriendlyName,
																sig.Town.Definition.FriendlyName,
																sig.Corrupted.Definition.FriendlyName);
														}
													}
												}
												else
												{
													foreach (PlayerState member in members)
													{
														member.Mobile.SendMessage(
															factionToBCast.Definition.HueBroadcast,
															"The {0} have stolen the {1} sigil!",
															faction.Definition.FriendlyName,
															sig.Town.Definition.FriendlyName);
													}
												}
											}
										}
										break;
									case PvPController.SigilStolenAnnouncing.Owner:
										{
											if (sig.Corrupted != null)
											{
												List<PlayerState> members = sig.Corrupted.Members;

												foreach (PlayerState member in members)
												{
													member.Mobile.SendMessage(
														sig.Corrupted.Definition.HueBroadcast,
														"The {0} have stolen the {1} sigil from your faction stronghold!",
														faction.Definition.FriendlyName,
														sig.Town.Definition.FriendlyName);
												}
											}
										}
										break;
								}

								return sig;
							}
						}
						else
						{
							m_Thief.SendLocalizedMessage(1005594); //	You do not have enough skill to steal the sigil
						}
					}
					else
					{
						m_Thief.SendLocalizedMessage(1005588); //	You must join a faction to do that
					}
				}
					#endregion

				else if (!stealflag && si == null && (toSteal.Parent == null || !toSteal.Movable))
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}
				else if ((toSteal.LootType == LootType.Newbied || toSteal.CheckBlessed(root)) &&
						 !(toSteal.RootParent is FillableContainer) && !stealflag)
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}
				else if (toSteal is Spellbook)
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}
				else if (toSteal.Nontransferable)
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}
				else if (m_Thief.EraAOS && si == null && toSteal is Container)
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}
				else if (toSteal is IEthicsItem && ((IEthicsItem)toSteal).EthicsItemState != null &&
						 !((IEthicsItem)toSteal).EthicsItemState.HasExpired)
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}
				else if (!m_Thief.InRange(toSteal.GetWorldLocation(), 1))
				{
					m_Thief.SendLocalizedMessage(502703); // You must be standing next to an item to steal it.
				}
					// Alan: commented this out b/c there shouldn't be a required skill level to steal stealflag stuff
					//else if ( ( si != null && m_Thief.Skills[SkillName.Stealing].Value < 100.0 ) || ( stealflag ) && m_Thief.Skills[SkillName.Stealing].Value < 100.0 ) ) //&& m_Thief.Skills[SkillName.Stealing].Value < 90.0 ) )
					//	m_Thief.SendLocalizedMessage( 1060025, "", 0x66D ); // You're not skilled enough to attempt the theft of this item.
				else if (toSteal.Parent is Mobile)
				{
					m_Thief.SendLocalizedMessage(1005585); // You cannot steal items which are equipped.
				}
				else if (toSteal.GetSavedFlag(0x01)) //Not lootable item
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}
				else if (root == m_Thief || (root is BaseCreature && ((BaseCreature)root).GetMaster() == m_Thief))
				{
					m_Thief.SendLocalizedMessage(502704); // You catch yourself red-handed.
				}
				else if (root is Mobile && ((Mobile)root).AccessLevel > AccessLevel.Player)
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}
				else if (root is Mobile && !m_Thief.CanBeHarmful((Mobile)root))
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}
				else if (root is Corpse)
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}
				else if (m_Thief.Spell != null)
				{
					m_Thief.SendMessage("You are too busy concentrating on your spell to steal that item.");
				}
				else
				{
					double w = toSteal.Weight + toSteal.TotalWeight;

					if (addon != null)
					{
						w = addon.Weight = addon.TotalWeight;
					}

					if (w > 10 && !stealflag)
					{
						m_Thief.SendMessage("That is too heavy to steal.");
					}
					else
					{
						m_Thief.BeginAction(typeof(Hiding));

						Timer.DelayCall(TimeSpan.FromSeconds(SpecialMovesController._StealingRehideDelay), ReleaseHideLock, m_Thief);

						if (toSteal.Stackable && toSteal.Amount > 1)
						{
							var maxAmount = (int)((m_Thief.Skills[SkillName.Stealing].Value / 10.0) / toSteal.Weight);
							var minAmount = (int)((m_Thief.Skills[SkillName.Stealing].Value / 25.0) / toSteal.Weight); //added a min amount

							if (minAmount < 1)
							{
								minAmount = 1;
							}

							if (maxAmount < 1)
							{
								maxAmount = 1;
							}
							else if (maxAmount > toSteal.Amount)
							{
								maxAmount = toSteal.Amount;
							}

							int amount = Utility.RandomMinMax(minAmount, maxAmount); //(change from 1, maxamount)

							if (amount >= toSteal.Amount)
							{
								var pileWeight = (int)Math.Ceiling(toSteal.Weight * toSteal.Amount);
								pileWeight *= 10;

								if (m_Thief.CheckTargetSkill(SkillName.Stealing, toSteal, pileWeight - 22.5, pileWeight + 27.5))
								{
									stolen = toSteal;
								}
							}
							else
							{
								var pileWeight = (int)Math.Ceiling(toSteal.Weight * amount);
								pileWeight *= 10;

								if (m_Thief.CheckTargetSkill(SkillName.Stealing, toSteal, pileWeight - 22.5, pileWeight + 27.5))
								{
									stolen = Mobile.LiftItemDupe(toSteal, toSteal.Amount - amount) ?? toSteal;
								}
							}
						}
						else
						{
							var iw = (int)Math.Ceiling(w);
							iw *= 10;

							if (stealflag || m_Thief.CheckTargetSkill(SkillName.Stealing, toSteal, iw - 22.5, iw + 27.5))
							{
								stolen = toSteal;
							}
						}

						if (stolen is PowerScroll)
						{
							var scroll = (PowerScroll)stolen;
							bool success = Utility.RandomBool();

							if (success || Utility.RandomBool())
							{
								BaseMount.Dismount(m_Thief);
								BaseMount.SetMountPrevention(
									m_Thief, BlockMountType.DismountRecovery, TimeSpan.FromSeconds(success ? 60.0 : 5.0));
							}
						}

						if (stolen != null)
						{
                            m_Thief.SendLocalizedMessage(502724); // You successfully steal the item.

							if (stolen is Head2)
							{
								((Head2)stolen).Owner = m_Thief as PlayerMobile;
							}

							if (si != null)
							{
								toSteal.Movable = true;
								si.Item = null;
							}

							if (stealflag)
							{
								//toSteal.SetSavedFlag( 0x04, false );
								// ARTEGORDONMOD
								// set the taken flag to trigger release from any controlling spawner
								ItemFlags.SetTaken(stolen, true);
								// clear the stealable flag so that the item can only be stolen once if it is later locked down.
								ItemFlags.SetStealable(stolen, false);

								if (addon != null) //deed it up!
								{
									Item deed = addon.Deed;

									addon.Delete();
									stolen = deed;
								}
								else // release it if it was locked down
								{
									toSteal.Movable = true;
								}

								if (toSteal.Spawner != null) //its not spawned anymore, its STOLEN!
								{
									toSteal.Spawner.Remove(toSteal);
									toSteal.Spawner = null;
								}
							}

							Conquests.CheckProgress<StealingConquest>(m_Thief as PlayerMobile, toSteal);
						}
						else
						{
							m_Thief.SendLocalizedMessage(502723); // You fail to steal the item.
						}

						caught = !stealflag && (m_Thief.Skills[SkillName.Stealing].Value < Utility.Random(150));
					}
				}

				return stolen;
			}

			public static void ReleaseHideLock(Mobile from)
			{
				from.EndAction(typeof(Hiding));
			}

			protected override void OnTarget(Mobile from, object target)
			{
				from.RevealingAction();

                if (m_Thief.IsYoung())
                {
                    m_Thief.SendMessage(54, "You may not steal from players or NPCs as a young player.");
                    return;
                }

				Item stolen = null;
				object root = null;
				bool caught = false;

				var entity = target as IEntity;
				if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) &&
					UberScriptTriggers.Trigger(
						entity,
						from,
						TriggerName.onTargeted,
						null,
						null,
						null,
						0,
						null,
						SkillName.Stealing,
						from.Skills[SkillName.Stealing].Value))
				{
					return;
				}

				if (target is Item)
				{
					root = ((Item)target).RootParent;
					stolen = TryStealItem((Item)target, ref caught);
				}
				else if (target is Mobile)
				{
					Container pack = ((Mobile)target).Backpack;

					if (pack != null && pack.Items.Count > 0)
					{
						int randomIndex = Utility.Random(pack.Items.Count);

						root = target;
						stolen = TryStealItem(pack.Items[randomIndex], ref caught);
					}
				}
				else
				{
					m_Thief.SendLocalizedMessage(502710); // You can't steal that!
				}

				if (stolen != null)
				{
					from.AddToBackpack(stolen);

					StolenItem.Add(stolen, m_Thief, root as Mobile);
				}

				if (caught)
				{
					if (root == null)
					{
						m_Thief.CriminalAction(false);
					}
					else if (root is Corpse && ((Corpse)root).IsCriminalAction(m_Thief))
					{
						m_Thief.CriminalAction(false);
					}
					else if (root is Mobile)
					{
						var mobRoot = (Mobile)root;

						if (!IsInGuild(mobRoot) && IsInnocentTo(m_Thief, mobRoot))
						{
							m_Thief.CriminalAction(false);
						}

						string message = String.Format("You notice {0} trying to steal from {1}.", m_Thief.Name, mobRoot.Name);

						foreach (NetState ns in m_Thief.GetClientsInRange(8))
						{
							if (ns.Mobile != m_Thief)
							{
								ns.Mobile.SendMessage(message);
							}
						}
					}
				}
				else if (root is Corpse && ((Corpse)root).IsCriminalAction(m_Thief))
				{
					m_Thief.CriminalAction(false);
				}

				if (root is Mobile && root != m_Thief)
				{
					Item item = m_Thief.FindItemOnLayer(Layer.Helm);

					if (item is OrcishKinMask)
					{
						if (root is Orc || root is OrcBomber || root is OrcBrute || root is OrcishMage || root is OrcishLord ||
							root is OrcishMineOverseer || root is OrcLeader || root is OrcMineBomber || root is OrcMiner)
						{
							item.Delete();
							m_Thief.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
							m_Thief.PlaySound(0x307);
						}
					}

					if (((Mobile)root).Player && m_Thief is PlayerMobile && IsInnocentTo(m_Thief, (Mobile)root) &&
						!IsInGuild((Mobile)root))
					{
						var pm = (PlayerMobile)m_Thief;

						pm.PermaFlags.Add((Mobile)root);
						pm.Delta(MobileDelta.Noto);
					}
				}
			}
		}

		public static bool IsEmptyHanded(Mobile from)
		{
			if (from.FindItemOnLayer(Layer.OneHanded) != null)
			{
				return false;
			}

			if (from.FindItemOnLayer(Layer.TwoHanded) != null)
			{
				return false;
			}

			return true;
		}

		public static TimeSpan OnUse(Mobile m)
		{
			if (!IsEmptyHanded(m))
			{
				m.SendLocalizedMessage(1005584); // Both hands must be free to steal.
			}
			else
			{
				m.Target = new StealingTarget(m);
				//				m.RevealingAction();// added back in/removed again

				m.SendLocalizedMessage(502698); // Which item do you want to steal?
			}

			return TimeSpan.FromSeconds(10.0);
		}
	}

	public class StolenItem
	{
		public static readonly TimeSpan StealTime = TimeSpan.FromMinutes(2.0);

		private readonly Item m_Stolen;
		private readonly Mobile m_Thief;
		private readonly Mobile m_Victim;
		private DateTime m_Expires;

		public Item Stolen { get { return m_Stolen; } }
		public Mobile Thief { get { return m_Thief; } }
		public Mobile Victim { get { return m_Victim; } }
		public DateTime Expires { get { return m_Expires; } }

		public bool IsExpired { get { return (DateTime.UtcNow >= m_Expires); } }

		public StolenItem(Item stolen, Mobile thief, Mobile victim)
		{
			m_Stolen = stolen;
			m_Thief = thief;
			m_Victim = victim;

			m_Expires = DateTime.UtcNow + StealTime;
		}

		private static readonly Queue<StolenItem> m_Queue = new Queue<StolenItem>();

		public static void Add(Item item, Mobile thief, Mobile victim)
		{
			Clean();

			m_Queue.Enqueue(new StolenItem(item, thief, victim));
		}

		public static bool IsStolen(Item item)
		{
			Mobile victim = null;

			return IsStolen(item, ref victim);
		}

		public static bool IsStolen(Item item, ref Mobile victim)
		{
			Clean();

			foreach (StolenItem si in m_Queue.Where(si => si.m_Stolen == item && !si.IsExpired))
			{
				victim = si.m_Victim;
				return true;
			}

			return false;
		}

		public static void ReturnOnDeath(Mobile killed, Container corpse)
		{
			Clean();

			foreach (StolenItem si in
				m_Queue.Where(si => si.m_Stolen.RootParent == corpse && si.m_Victim != null && !si.IsExpired))
			{
				// the item that was stolen is returned to you. : the item that was stolen from you falls to the ground.
				si.m_Victim.SendLocalizedMessage(si.m_Victim.AddToBackpack(si.m_Stolen) ? 1010464 : 1010463);

				si.m_Expires = DateTime.UtcNow; // such a hack
			}
		}

		public static void Clean()
		{
			while (m_Queue.Count > 0)
			{
				StolenItem si = m_Queue.Peek();

				if (!si.IsExpired)
				{
					break;
				}

				m_Queue.Dequeue();
			}
		}
	}
}