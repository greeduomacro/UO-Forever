#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Server.Commands;
using Server.Factions;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
#endregion

namespace Server.Misc
{
	public class AutoRestart : Timer
	{
		public static bool Enabled = false; // is the script enabled?

		private static readonly TimeSpan DefaultRestartTime = TimeSpan.FromHours(2.0); // time of day at which to restart

		private static TimeSpan RestartDelay = TimeSpan.FromMinutes(30.0);
		// how long the server should remain active before restart (period of 'server wars')

		public static int[] WarningIntervals = new[]
		{172800, 86400, 43200, 21600, 10800, 3600, 900, 600, 300, 180, 60, 10, 5, 4, 3, 2, 1};

		//Warning intervals in seconds BEFORE any given restart time

		private static int m_WarningCount;
		private static bool m_Restarting;
		private static bool m_ServerWars;
		private static DateTime m_RestartTime;

		public static bool Restarting { get { return m_Restarting; } }

		public static bool ServerWars { get { return m_ServerWars; } }

		public static DateTime RestartTime { get { return m_RestartTime; } set { m_RestartTime = value; } }

		public static int WarningCount { get { return m_WarningCount; } set { m_WarningCount = value; } }

		public static TimeSpan TimeTillRestart { get { return m_RestartTime + RestartDelay - DateTime.UtcNow; } }

		public static void Initialize()
		{
			new AutoRestart().Start();
			EventSink.Login += OnLogin;

			CommandSystem.Register("Restart", AccessLevel.Administrator, Restart_OnCommand);
			CommandSystem.Register("RestartWithWar", AccessLevel.Administrator, RestartWithWar_OnCommand);
		}

		private static void OnLogin(LoginEventArgs e)
		{
			Mobile from = e.Mobile;
			if (m_ServerWars)
			{
				from.SendAsciiMessage(38, 0, "---- SERVER WARS ----");
				from.SendAsciiMessage(38, 0, String.Format("RESTARTING in {0}", FormatTimeSpan(TimeTillRestart)));
			}
		}

		public static void RestartWithWar_OnCommand(CommandEventArgs e)
		{
			if (m_Restarting || m_ServerWars)
			{
				e.Mobile.SendMessage("The server is already restarting.");
			}
			else
			{
				e.Mobile.SendMessage("You have initiated a server shutdown.");
				Enabled = true;
				m_RestartTime = DateTime.UtcNow;
				m_WarningCount = WarningIntervals.Length;
				RestartDelay = TimeSpan.FromMinutes(30.0);
			}
		}

		public static void Restart_OnCommand(CommandEventArgs e)
		{
			if (m_Restarting || m_ServerWars)
			{
				e.Mobile.SendMessage("The server is already restarting.");
			}
			else
			{
				e.Mobile.SendMessage("You have initiated a server shutdown.");
				Enabled = true;
				m_RestartTime = DateTime.UtcNow;
				m_WarningCount = WarningIntervals.Length - 1;
				RestartDelay = TimeSpan.Zero;
			}
		}

		private static void Warning_Callback()
		{
			if (RestartDelay > TimeSpan.Zero)
			{
				World.Broadcast(
					38,
					0,
					true,
					String.Format(
						"--- SERVER RESTARTING IN {0} ---", FormatTimeSpan(TimeSpan.FromSeconds(WarningIntervals[m_WarningCount]))));
			}
			else
			{
				World.Broadcast(38, 0, true, "--- SERVER RESTARTING ---");
			}

			m_WarningCount++;
		}

		private static void Restart_Callback()
		{
			if (File.Exists("PublishPusher.exe"))
			{
				Process.Start("PublishPusher.exe");
				Core.Kill();
			}
			else
			{
				Core.Kill(true);
			}
		}

		public static string FormatTimeSpan(TimeSpan span)
		{
			return String.Format(
				"{0}{1}{2}{3}",
				span.Days > 0 ? String.Format("{0} days ", span.Days) : "",
				span.Hours > 0 ? String.Format("{0:0#} hours ", span.Hours) : "",
				span.Minutes > 0 ? String.Format("{0:0#} minutes ", span.Minutes) : "",
				span.Seconds > 0 ? String.Format("{0:0#} seconds", span.Seconds) : "");
		}

		public AutoRestart()
			: base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
		{
			Priority = TimerPriority.FiveSeconds;

			m_RestartTime = DateTime.UtcNow.Date + DefaultRestartTime;

			if (m_RestartTime < DateTime.UtcNow)
			{
				m_RestartTime += TimeSpan.FromDays(1.0);
			}
		}

		protected override void OnTick()
		{
			if (m_Restarting || !Enabled)
			{
				return;
			}

			//if ( DateTime.UtcNow < m_RestartTime )
			//	return;

			if (m_WarningCount < WarningIntervals.Length &&
				DateTime.UtcNow >= (m_RestartTime - TimeSpan.FromSeconds(WarningIntervals[m_WarningCount])))
			{
				Warning_Callback();
			}
			else if (DateTime.UtcNow >= m_RestartTime)
			{
				AutoSave.SavesEnabled = false;

				FinalRestart();
			}
		}

		public void FinalRestart()
		{
			AutoSave.Save(); //ONE LAST SAVE! :P

			m_Restarting = true;

			DelayCall(RestartDelay, Restart_Callback);

			if (RestartDelay > TimeSpan.Zero)
			{
				World.Broadcast(38, 0, true, "---- SERVER WARS ----");
				World.Broadcast(38, 0, true, String.Format("RESTARTING IN {0}", FormatTimeSpan(RestartDelay)));
				PrepareServerWar();
			}
		}

		public static void PrepareServerWar()
		{
			m_ServerWars = true;
			FactionReset();

			Map[] maps = Map.Maps;

			foreach (GuardedRegion region in maps.Where(m => m != null).SelectMany(m => m.Regions.Values.OfType<GuardedRegion>())
				)
			{
				region.Disabled = true;
			}

			foreach (
				PlayerMobile mob in World.Mobiles.Values.OfType<PlayerMobile>().Where(mob => mob.AccessLevel == AccessLevel.Player))
			{
				mob.BankBox.Credit = 10000000; //10 mill in the bank!

				Bag bag = new BagOfReagents(10000);
				bag.Name = "Bag of Reagents (10K)";
				bag.Hue = RandomHue();

				mob.BankBox.DropItem(bag);
				ResurrectGump.ClearSkillLoss(mob);

				bag = new Bag
				{
					Hue = RandomHue(),
					Name = "Bag of PVP Supplies"
				};

				mob.BankBox.DropItem(bag);

				if (mob.Skills[SkillName.Magery].Value > 0.0)
				{
					mob.Backpack.DropItem(
						BaseCreature.Rehued(BaseCreature.ChangeLootType(new BagOfReagents(150), LootType.Blessed), RandomHue()));

					Spellbook book = Spellbook.FindRegular(mob);

					if (book != null)
					{
						book.Content = ulong.MaxValue;
						book.LootType = LootType.Blessed;
						book.Hue = Utility.Random(1000);
					}
					else
					{
						book = new Spellbook
						{
							Content = ulong.MaxValue,
							LootType = LootType.Blessed,
							Hue = Utility.Random(1000)
						};

						mob.Backpack.DropItem(book);
					}
				}

				//if ( mob.Skills[SkillName.Healing].Value > 0.0 )
				//{
				mob.BankBox.DropItem(BaseCreature.ChangeLootType(new Bandage(150), LootType.Blessed));
				//}

				//if ( mob.Skills[SkillName.Fencing].Value > 0.0 )
				//{
				bag.DropItem(SetWeapon(new ShortSpear()));
				bag.DropItem(SetWeapon(new Kryss()));
				bag.DropItem(SetWeapon(new Spear()));
				//}

				//if ( mob.Skills[SkillName.Parry].Value > 0.0 )
				//{
				bag.DropItem(
					BaseCreature.ChangeLootType(
						BaseCreature.Resourced(new MetalKiteShield(), CraftResource.Valorite), LootType.Blessed));
				//}

				//if ( mob.Skills[SkillName.Swords].Value > 0.0 )
				//{
				if (mob.Skills[SkillName.Lumberjacking].Value > 0.0)
				{
					bag.DropItem(SetWeapon(new Hatchet()));
					bag.DropItem(SetWeapon(new LargeBattleAxe()));
				}

				bag.DropItem(SetWeapon(new Halberd()));
				bag.DropItem(SetWeapon(new Katana()));
				//}

				//if ( mob.Skills[SkillName.Macing].Value > 0.0 )
				//{
				bag.DropItem(SetWeapon(new WarAxe()));
				bag.DropItem(SetWeapon(new WarHammer()));
				//}

				//if ( mob.Skills[SkillName.Archery].Value > 0.0 )
				//{
				bag.DropItem(SetWeapon(new Bow()));
				bag.DropItem(SetWeapon(new Crossbow()));
				bag.DropItem(SetWeapon(new HeavyCrossbow()));

				var quiver = new ElvenQuiver
				{
					Hue = RandomHue()
				};
				quiver.DropItem(new Arrow(300));

				bag.DropItem(new Bolt(300));
				mob.BankBox.DropItem(BaseCreature.ChangeLootType(quiver, LootType.Blessed));
				//}

				//if ( mob.Skills[SkillName.Poisoning].Value > 0.0 )
				//{
				for (int i = 0; i < 5; i++)
				{
					bag.DropItem(BaseCreature.ChangeLootType(new DeadlyPoisonPotion(), LootType.Blessed));
				}
				//}

				mob.Kills = mob.ShortTermMurders = 0;

				var horse = new EtherealHorse
				{
					IsDonationItem = true,
					Hue = RandomHue()
				};

				mob.Backpack.DropItem(horse);
				mob.Backpack.DropItem(BaseCreature.ChangeLootType(new StatsBall(), LootType.Blessed));

				bag = new Bag
				{
					Hue = 1437,
					Name = "Bag of Barbed Leather Armor"
				};

				bag.DropItem(
					BaseCreature.Rehued(
						BaseCreature.ChangeLootType(
							BaseCreature.Resourced(new LeatherChest(), CraftResource.BarbedLeather), LootType.Blessed),
						RandomHue()));
				bag.DropItem(
					BaseCreature.Rehued(
						BaseCreature.ChangeLootType(
							BaseCreature.Resourced(new LeatherLegs(), CraftResource.BarbedLeather), LootType.Blessed),
						RandomHue()));
				bag.DropItem(
					BaseCreature.Rehued(
						BaseCreature.ChangeLootType(
							BaseCreature.Resourced(new LeatherGorget(), CraftResource.BarbedLeather), LootType.Blessed),
						RandomHue()));
				bag.DropItem(
					BaseCreature.Rehued(
						BaseCreature.ChangeLootType(
							BaseCreature.Resourced(new LeatherGloves(), CraftResource.BarbedLeather), LootType.Blessed),
						RandomHue()));
				bag.DropItem(
					BaseCreature.Rehued(
						BaseCreature.ChangeLootType(
							BaseCreature.Resourced(new LeatherArms(), CraftResource.BarbedLeather), LootType.Blessed),
						RandomHue()));
				bag.DropItem(
					BaseCreature.Rehued(
						BaseCreature.ChangeLootType(
							BaseCreature.Resourced(new LeatherCap(), CraftResource.BarbedLeather), LootType.Blessed),
						RandomHue()));

				mob.Backpack.DropItem(bag);

				Skills skills = mob.Skills;

				foreach (Skill s in skills.Where(t => t.SkillName != SkillName.ItemID))
				{
					s.BaseFixedPoint = 1000;
				}
			}
		}

		public static int RandomHue()
		{
			return Utility.RandomList(
				Utility.Random(1, 1000),
				Utility.RandomNondyedHue(),
				Utility.RandomGreyHue(),
				Utility.RandomBlackHue(),
				Utility.RandomPinkHue(),
				Utility.RandomBlueHue(),
				Utility.RandomGreenHue(),
				Utility.RandomRedHue(),
				Utility.RandomOrangeHue(),
				Utility.RandomYellowHue(),
				Utility.RandomNeutralHue(),
				Utility.RandomSnakeHue(),
				Utility.RandomBirdHue(),
				Utility.RandomSlimeHue(),
				Utility.RandomAnimalHue(),
				Utility.RandomMetalHue(),
				Utility.RandomDyedHue(),
				Utility.RandomHairHue());
		}

		public static BaseWeapon SetWeapon(BaseWeapon weapon)
		{
			weapon.LootType = LootType.Blessed;
			weapon.Hue = RandomHue();
			weapon.DamageLevel = WeaponDamageLevel.Vanq;
			weapon.AccuracyLevel = WeaponAccuracyLevel.Supremely;
			weapon.Identified = true;
			return weapon;
		}

		private static void FactionReset()
		{
			List<Faction> factions = Faction.Factions;

			foreach (Faction f in factions)
			{
				var memberlist = new List<PlayerState>(f.Members);

				foreach (PlayerState s in memberlist)
				{
					f.RemoveMember(s.Mobile);
				}

				var itemlist = new List<FactionItem>(f.State.FactionItems);

				foreach (FactionItem fi in itemlist)
				{
					if (fi.Expiration == DateTime.MinValue)
					{
						fi.Item.Delete();
					}
					else
					{
						fi.Detach();
					}
				}

				var traplist = new List<BaseFactionTrap>(f.Traps);

				foreach (BaseFactionTrap t in traplist)
				{
					t.Delete();
				}
			}
		}
	}
}