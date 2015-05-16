#region References
using System;
using System.IO;
using System.Net;

using Server.Accounting;
using Server.Items;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Misc
{
	public class CharacterCreation
	{
		public static void Initialize()
		{
			// Register our event handler
			EventSink.CharacterCreated += EventSink_CharacterCreated;
		}

		private static void AddBackpack(Mobile m, Expansion e)
		{
			Container pack = m.Backpack;

			if (pack == null)
			{
				pack = new Backpack
				{
					Movable = false
				};

				m.AddItem(pack);
			}

			PackItem(MakeNewbie(new RedBook("a book", m.Name, 20, true), e), e);
			PackItem(new Gold(500), e); // Starting gold can be customized here
			PackItem(MakeNewbie(new Dagger(), e), e);
			PackItem(MakeNewbie(new Candle(), e), e);
			PackItem(new Bandage(100), e);
			//PackItem(new ForeverWelcomeBag(), e);
		}

		private static Item MakeNewbie(Item item, Expansion e)
		{
			if (e < Expansion.AOS)
			{
				item.LootType = LootType.Newbied;
			}

			return item;
		}

		private static void AddShirt(Mobile m, int shirtHue, Expansion e)
		{
			int hue = Utility.ClipDyedHue(shirtHue & 0x3FFF);

			if (m.Race == Race.Elf)
			{
				EquipItem(new ElvenShirt(hue), true, e);
			}
			else
			{
				switch (Utility.Random(3))
				{
					case 0:
						EquipItem(new Shirt(hue), true, e);
						break;
					case 1:
						EquipItem(new FancyShirt(hue), true, e);
						break;
					case 2:
						EquipItem(new Doublet(hue), true, e);
						break;
				}
			}
		}

		private static void AddPants(Mobile m, int pantsHue, Expansion e)
		{
			int hue = Utility.ClipDyedHue(pantsHue & 0x3FFF);

			if (m.Race == Race.Elf)
			{
				EquipItem(new ElvenPants(hue), true, e);
			}
			else
			{
				if (m.Female)
				{
					switch (Utility.Random(2))
					{
						case 0:
							EquipItem(new Skirt(hue), true, e);
							break;
						case 1:
							EquipItem(new Kilt(hue), true, e);
							break;
					}
				}
				else
				{
					switch (Utility.Random(2))
					{
						case 0:
							EquipItem(new LongPants(hue), true, e);
							break;
						case 1:
							EquipItem(new ShortPants(hue), true, e);
							break;
					}
				}
			}
		}

		private static void AddShoes(Mobile m, Expansion e)
		{
			if (m.Race == Race.Elf)
			{
				EquipItem(new ElvenBoots(), true, e);
			}
			else
			{
				EquipItem(new Shoes(Utility.RandomYellowHue()), true, e);
			}
		}

		private static Mobile CreateMobile(Account a)
		{
			if (a.Count >= a.Limit)
			{
				return null;
			}

			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] == null)
				{
					return (a[i] = new PlayerMobile());
				}
			}

			return null;
		}

		public static void LogIPAccess(IPAddress ipAddress, Account acct, Mobile mob)
		{
			string directory = "Logs/Login";

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			try
			{
				using (var op = new StreamWriter(Path.Combine(directory, String.Format("{0}.log", acct.Username.ToLower())), true))
				{
					op.WriteLine("{0} - {1} - {2} - created character: {3}", DateTime.UtcNow, acct.Username, ipAddress, mob);
				}
			}
			catch
			{
				Console.WriteLine(
					"Failed to write to file: {0}", Path.Combine("Logs/Login", String.Format("{0}.log", acct.Username.ToLower())));
			}
		}

		private static void EventSink_CharacterCreated(CharacterCreatedEventArgs args)
		{
			NetState state = args.State;
			var acct = args.Account as Account;

			Mobile newChar = CreateMobile(acct);

			if (newChar == null)
			{
				Console.WriteLine("Login: {0}: Character creation failed, account full", args.State);
				return;
			}

			args.Mobile = newChar;
			m_Mobile = newChar;

			newChar.Player = true;
			newChar.AccessLevel = args.Account.AccessLevel;

			bool young = false;

			if (newChar is PlayerMobile)
			{
				var pm = (PlayerMobile)newChar;

				if (pm.AccessLevel == AccessLevel.Player && ((Account)pm.Account).Young)
				{
					young = pm.Young = true;
				}
			}

			//CityInfo city = GetStartLocation(args, young);
			var city = new CityInfo("Britain", "Forever", 1495, 1628, 10, Map.Felucca);

			if (!VerifyProfession(args.Profession, city.Map.Expansion))
			{
				args.Profession = 0;
			}

			if (newChar is PlayerMobile)
			{
				((PlayerMobile)newChar).Profession = args.Profession;
			}

			newChar.Female = args.Female;
			//newChar.Body = newChar.Female ? 0x191 : 0x190;

			newChar.Race = city.Map.Expansion >= args.Race.RequiredExpansion ? args.Race : Race.DefaultRace;

			//newChar.Hue = Utility.ClipSkinHue( args.Hue & 0x3FFF ) | 0x8000;
			newChar.Hue = newChar.Race.ClipSkinHue(args.Hue & 0x3FFF) | 0x8000;

			newChar.Hunger = 20;
			newChar.Thirst = 20;

			//NameResultMessage result = SetName(newChar, args.Name);
			SetName(newChar, args.Name);
			AddBackpack(newChar, city.Map.Expansion);

			SetStats(newChar, state, args.Str, args.Dex, args.Int);
			SetSkills(newChar, city.Map.Expansion, args.Skills, args.Profession);

			Race race = newChar.Race;

			if (race.ValidateHair(newChar, args.HairID))
			{
				newChar.HairItemID = args.HairID;
				newChar.HairHue = race.ClipHairHue(args.HairHue & 0x3FFF);
			}

			if (race.ValidateFacialHair(newChar, args.BeardID))
			{
				newChar.FacialHairItemID = args.BeardID;
				newChar.FacialHairHue = race.ClipHairHue(args.BeardHue & 0x3FFF);
			}

			if (args.Profession <= 3)
			{
				AddShirt(newChar, args.ShirtHue, city.Map.Expansion);
				AddPants(newChar, args.PantsHue, city.Map.Expansion);
				AddShoes(newChar, city.Map.Expansion);
			}

			BankBox bank = newChar.BankBox;

			bool needstartpack = acct != null && String.IsNullOrEmpty(acct.GetTag("startpack"));

			if (needstartpack /*|| TestCenter.Enabled*/)
			{
				Container startpack = new StarterPack();
				bank.DropItem(startpack);
				/*
				Item housedeed = new SmallBrickHouseDeed();
				housedeed.Name = "a beta tester's small brick house deed";
				startpack.DropItem(housedeed);
				housedeed.X = 23;
				housedeed.Y = 53;

				Item startercheck = new BankCheck(10000);
				startpack.DropItem(startercheck);
				startercheck.X = 52;
				startercheck.Y = 36;
				*/
				acct.SetTag("startpack", "true");
			}

			if (young)
			{
				bank.DropItem(
					new NewPlayerTicket
					{
						Owner = newChar
					});
			}

			newChar.MoveToWorld(city.Location, city.Map);

			LogIPAccess(args.State.Address, acct, newChar);

			Console.WriteLine("Login: {0}: New character being created (account={1})", args.State, args.Account.Username);
			Console.WriteLine(" - Character: {0} (serial={1})", newChar.Name, newChar.Serial);
			Console.WriteLine(" - Started: {0} {1} in {2}", city.City, city.Location, city.Map);

			new WelcomeTimer(newChar).Start();

			/*if (result != NameResultMessage.Allowed)
			{
				newChar.SendGump(new NameChangeGump(newChar, result, args.Name));
			}*/
		}

		public static bool VerifyProfession(int profession, Expansion e)
		{
			if (profession < 0)
			{
				return false;
			}

			if (profession < 4)
			{
				return true;
			}

			if (e >= Expansion.AOS && profession < 6)
			{
				return true;
			}

			if (e >= Expansion.SE && profession < 8)
			{
				return true;
			}

			return false;
		}

		private class BadStartMessage : Timer
		{
			private readonly Mobile _Mobile;
			private readonly int _Message;

			public BadStartMessage(Mobile m, int message)
				: base(TimeSpan.FromSeconds(3.5))
			{
				_Mobile = m;
				_Message = message;
				Start();
			}

			protected override void OnTick()
			{
				_Mobile.SendLocalizedMessage(_Message);
			}
		}

		private static readonly CityInfo m_NewHavenInfo = new CityInfo(
			"New Haven", "The Bountiful Harvest Inn", 3503, 2574, 14, Map.Trammel);

		private static CityInfo GetStartLocation(CharacterCreatedEventArgs args, bool isYoung)
		{
			if (args.City.Map.Expansion >= Expansion.ML)
			{
				if ((args.City.Map.Expansion < Expansion.SA || isYoung) && args.State != null && args.State.NewHaven)
				{
					return m_NewHavenInfo;
				}

				return args.City;
			}

			//bool useHaven = isYoung;
			//bool useHaven = false;

			ClientFlags flags = args.State == null ? ClientFlags.None : args.State.Flags;
			Mobile m = args.Mobile;

			switch (args.Profession)
			{
				case 4: //Necro
					{
						if ((flags & ClientFlags.Malas) != 0)
						{
							return new CityInfo("Umbra", "Mardoth's Tower", 2114, 1301, -50, Map.Malas);
						}

						//useHaven = true;

						new BadStartMessage(m, 1062205);
						/*
						 * Unfortunately you are playing on a *NON-Age-Of-Shadows* game
						 * installation and cannot be transported to Malas.
						 * You will not be able to take your new player quest in Malas
						 * without an AOS client.  You are now being taken to the city of
						 * Haven on the Trammel facet.
						 */
					}
					break;
				case 5: //Paladin
					break;
				case 6: //Samurai
					{
						if ((flags & ClientFlags.Tokuno) != 0)
						{
							return new CityInfo("Samurai DE", "Haoti's Grounds", 368, 780, -1, Map.Malas);
						}

						//useHaven = true;

						new BadStartMessage(m, 1063487);
						/*
						 * Unfortunately you are playing on a *NON-Samurai-Empire* game
						 * installation and cannot be transported to Tokuno.
						 * You will not be able to take your new player quest in Tokuno
						 * without an SE client. You are now being taken to the city of
						 * Haven on the Trammel facet.
						 */
					}
					break;
				case 7: //Ninja
					{
						if ((flags & ClientFlags.Tokuno) != 0)
						{
							return new CityInfo("Ninja DE", "Enimo's Residence", 414, 823, -1, Map.Malas);
						}

						//useHaven = true;

						new BadStartMessage(m, 1063487);
						/*
						 * Unfortunately you are playing on a *NON-Samurai-Empire* game
						 * installation and cannot be transported to Tokuno.
						 * You will not be able to take your new player quest in Tokuno
						 * without an SE client. You are now being taken to the city of
						 * Haven on the Trammel facet.
						 */
					}
					break;
			}

			/*if (useHaven)
			{
				return m_NewHavenInfo;
			}*/

			return args.City;
		}

		private static void FixStats(ref int str, ref int dex, ref int intel, int max)
		{
			int vMax = max - 30;

			int vStr = str - 10;
			int vDex = dex - 10;
			int vInt = intel - 10;

			if (vStr < 0)
			{
				vStr = 0;
			}

			if (vDex < 0)
			{
				vDex = 0;
			}

			if (vInt < 0)
			{
				vInt = 0;
			}

			int total = vStr + vDex + vInt;

			if (total == 0 || total == vMax)
			{
				return;
			}

			double scalar = vMax / (double)total;

			vStr = (int)(vStr * scalar);
			vDex = (int)(vDex * scalar);
			vInt = (int)(vInt * scalar);

			FixStat(ref vStr, (vStr + vDex + vInt) - vMax, vMax);
			FixStat(ref vDex, (vStr + vDex + vInt) - vMax, vMax);
			FixStat(ref vInt, (vStr + vDex + vInt) - vMax, vMax);

			str = vStr + 10;
			dex = vDex + 10;
			intel = vInt + 10;
		}

		private static void FixStat(ref int stat, int diff, int max)
		{
			stat += diff;

			if (stat < 0)
			{
				stat = 0;
			}
			else if (stat > max)
			{
				stat = max;
			}
		}

		public static void SetStats(Mobile m, NetState state, int str, int dex, int intel)
		{
			int max = state.NewCharacterCreation ? 90 : 80;

			FixStats(ref str, ref dex, ref intel, max);

			if (str < 10 || str > 60 || dex < 10 || dex > 60 || intel < 10 || intel > 60 || (str + dex + intel) != max)
			{
				str = 10;
				dex = 10;
				intel = 10;
			}

			m.InitStats(str, dex, intel);
		}

		private static void SetName(Mobile m, string name)
		{
			name = name.Trim();

			if (m.Account.AccessLevel == AccessLevel.Player)
			{
				NameResultMessage result = NameVerification.ValidatePlayerName(
					name, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote);

				m.Name = result != NameResultMessage.Allowed ? String.Format("Citizen {0}", Utility.Random(10000, 99999)) : name;
			}
			else
			{
				m.Name = name;
			}

			//return result;
		}

		private static bool ValidSkills(Expansion e, SkillNameValue[] skills)
		{
			int total = 0;

			for (int i = 0; i < skills.Length; ++i)
			{
				if (skills[i].Value < 0 || skills[i].Value > 50)
				{
					return false;
				}

				if (e >= Expansion.SA || (e >= Expansion.ML && skills[i].Name >= SkillName.Mysticism) ||
					(e >= Expansion.SE && skills[i].Name >= SkillName.Spellweaving) || skills[i].Name >= SkillName.Bushido)
				{
					return false;
				}

				total += skills[i].Value;

				for (int j = i + 1; j < skills.Length; ++j)
				{
					if (skills[j].Value > 0 && skills[j].Name == skills[i].Name)
					{
						return false;
					}
				}
			}

			return (total == 100 || total == 120);
		}

		private static Mobile m_Mobile;

		public static void SetSkills(Mobile m, Expansion e, SkillNameValue[] skills, int prof)
		{
			switch (prof)
			{
				case 1: // Warrior
					{
						skills = new[]
						{
							new SkillNameValue(SkillName.Anatomy, 30), new SkillNameValue(SkillName.Healing, 45),
							new SkillNameValue(SkillName.Swords, 35), new SkillNameValue(SkillName.Tactics, 50)
						};

						break;
					}
				case 2: // Magician
					{
						skills = new[]
						{
							new SkillNameValue(SkillName.EvalInt, 30), new SkillNameValue(SkillName.Wrestling, 30),
							new SkillNameValue(SkillName.Magery, 50), new SkillNameValue(SkillName.Meditation, 50)
						};

						break;
					}
				case 3: // Blacksmith
					{
						skills = new[]
						{
							new SkillNameValue(SkillName.Mining, 30), new SkillNameValue(SkillName.ArmsLore, 30),
							new SkillNameValue(SkillName.Blacksmith, 50), new SkillNameValue(SkillName.Tinkering, 50)
						};

						break;
					}
				case 4: // Necromancer
					{
						skills = new[]
						{
							new SkillNameValue(SkillName.Necromancy, 50), new SkillNameValue(SkillName.Focus, 30),
							new SkillNameValue(SkillName.SpiritSpeak, 30), new SkillNameValue(SkillName.Swords, 30),
							new SkillNameValue(SkillName.Tactics, 20)
						};

						break;
					}
				case 5: // Paladin
					{
						skills = new[]
						{
							new SkillNameValue(SkillName.Chivalry, 51), new SkillNameValue(SkillName.Swords, 49),
							new SkillNameValue(SkillName.Focus, 30), new SkillNameValue(SkillName.Tactics, 30)
						};

						break;
					}
				case 6: //Samurai
					{
						skills = new[]
						{
							new SkillNameValue(SkillName.Bushido, 50), new SkillNameValue(SkillName.Swords, 50),
							new SkillNameValue(SkillName.Anatomy, 30), new SkillNameValue(SkillName.Healing, 30)
						};
						break;
					}
				case 7: //Ninja
					{
						skills = new[]
						{
							new SkillNameValue(SkillName.Ninjitsu, 50), new SkillNameValue(SkillName.Hiding, 50),
							new SkillNameValue(SkillName.Fencing, 30), new SkillNameValue(SkillName.Stealth, 30)
						};
						break;
					}
				default:
					{
						if (!ValidSkills(e, skills))
						{
							return;
						}

						break;
					}
			}

			bool addSkillItems = true;
			bool elf = (m.Race == Race.Elf);

			switch (prof)
			{
				case 1: // Warrior
					{
						if (elf)
						{
							EquipItem(new LeafChest(), e);
						}
						else
						{
							EquipItem(new LeatherChest(), e);
						}
						break;
					}
				case 4: // Necromancer
					{
						Container regs = new BagOfNecroReagents(50);
						/*
						if (e < Expansion.AOS)
						{
							foreach (Item item in regs.Items)
							{
								item.LootType = LootType.Newbied;
							}
						}
						*/
						PackItem(regs, e);

						regs.LootType = LootType.Regular;

						EquipItem(new BoneHelm(), e);

						if (elf)
						{
							EquipItem(new ElvenMachete(), e);
							EquipItem(NecroHue(new LeafChest()), e);
							EquipItem(NecroHue(new LeafArms()), e);
							EquipItem(NecroHue(new LeafGloves()), e);
							EquipItem(NecroHue(new LeafGorget()), e);
							EquipItem(NecroHue(new LeafGorget()), e);
							EquipItem(NecroHue(new ElvenPants()), e); //TODO: Verify the pants
							EquipItem(new ElvenBoots(), e);
						}
						else
						{
							EquipItem(new BoneHarvester(), e);
							EquipItem(NecroHue(new LeatherChest()), e);
							EquipItem(NecroHue(new LeatherArms()), e);
							EquipItem(NecroHue(new LeatherGloves()), e);
							EquipItem(NecroHue(new LeatherGorget()), e);
							EquipItem(NecroHue(new LeatherLegs()), e);
							EquipItem(NecroHue(new Skirt()), e);
							EquipItem(new Sandals(0x8FD), e);
						}

						addSkillItems = false;

						break;
					}
				case 5: // Paladin
					{
						if (elf)
						{
							EquipItem(new ElvenMachete(), e);
							EquipItem(new WingedHelm(), e);
							EquipItem(new LeafGorget(), e);
							EquipItem(new LeafArms(), e);
							EquipItem(new LeafChest(), e);
							EquipItem(new LeafLegs(), e);
							EquipItem(new ElvenBoots(), e); //Verify hue
						}
						else
						{
							EquipItem(new Broadsword(), e);
							EquipItem(new Helmet(), e);
							EquipItem(new PlateGorget(), e);
							EquipItem(new RingmailArms(), e);
							EquipItem(new RingmailChest(), e);
							EquipItem(new RingmailLegs(), e);
							EquipItem(new ThighBoots(0x748), e);
							EquipItem(new Cloak(0xCF), e);
							EquipItem(new BodySash(0xCF), e);
						}

						addSkillItems = false;

						break;
					}

				case 6: // Samurai
					{
						addSkillItems = false;
						EquipItem(new HakamaShita(0x2C3), e);
						EquipItem(new Hakama(0x2C3), e);
						EquipItem(new SamuraiTabi(0x2C3), e);
						EquipItem(new TattsukeHakama(0x22D), e);
						EquipItem(new Bokuto(), e);

						if (elf)
						{
							EquipItem(new RavenHelm(), e);
						}
						else
						{
							EquipItem(new LeatherJingasa(), e);
						}

						PackItem(new Scissors(), e);
						PackItem(new Bandage(50), e);

						break;
					}
				case 7: // Ninja
					{
						addSkillItems = false;
						EquipItem(new Kasa(), e);

						var hues = new[] {0x1A8, 0xEC, 0x99, 0x90, 0xB5, 0x336, 0x89};
						//TODO: Verify that's ALL the hues for that above.

						EquipItem(new TattsukeHakama(hues[Utility.Random(hues.Length)]), e);

						EquipItem(new HakamaShita(0x2C3), e);
						EquipItem(new NinjaTabi(0x2C3), e);

						if (elf)
						{
							EquipItem(new AssassinSpike(), e);
						}
						else
						{
							EquipItem(new Tekagi(), e);
						}

						PackItem(new SmokeBomb(), e);

						break;
					}
			}

			foreach (SkillNameValue snv in skills)
			{
				if (snv.Value <= 0 || (snv.Name == SkillName.Stealth && prof != 7) || snv.Name == SkillName.RemoveTrap ||
					snv.Name == SkillName.Spellweaving)
				{
					continue;
				}

				Skill skill = m.Skills[snv.Name];

				if (skill == null)
				{
					continue;
				}

				skill.BaseFixedPoint = snv.Value * 10;

				if (addSkillItems)
				{
					AddSkillItems(snv.Name, m, e);
				}
			}

			// all players start with a spellbook with level 1-4 spells
			PackItem(
				new Spellbook(0xFFFFFFFF)
				{
					LootType = LootType.Blessed
				},
				e);
		}

		private static void EquipItem(Item item, Expansion e)
		{
			EquipItem(item, false, e);
		}

		private static void EquipItem(Item item, bool mustEquip, Expansion e)
		{
			if (e < Expansion.AOS && !(item is Spellbook))
			{
				item.LootType = LootType.Newbied;
			}

			if (m_Mobile.EquipItem(item))
			{
				return;
			}

			Container pack = m_Mobile.Backpack;

			if (!mustEquip && pack != null)
			{
				pack.DropItem(item);
			}
			else
			{
				item.Delete();
			}
		}

		private static void PackItem(Item item, Expansion e)
		{
			if (e < Expansion.AOS && !(item is Spellbook))
			{
				item.LootType = LootType.Newbied;
			}

			Container pack = m_Mobile.Backpack;

			if (pack != null)
			{
				pack.DropItem(item);
			}
			else
			{
				item.Delete();
			}
		}

		private static void PackInstrument(Expansion e)
		{
			switch (Utility.Random(6))
			{
				case 0:
					PackItem(new Drums(), e);
					break;
				case 1:
					PackItem(new Harp(), e);
					break;
				case 2:
					PackItem(new LapHarp(), e);
					break;
				case 3:
					PackItem(new Lute(), e);
					break;
				case 4:
					PackItem(new Tambourine(), e);
					break;
				case 5:
					PackItem(new TambourineTassel(), e);
					break;
			}
		}

		private static void PackScroll(int circle, Expansion e)
		{
			switch (Utility.Random(8) + (circle + 1))
			{
				case 0:
					PackItem(new ClumsyScroll(), e);
					break;
				case 1:
					PackItem(new CreateFoodScroll(), e);
					break;
				case 2:
					PackItem(new FeeblemindScroll(), e);
					break;
				case 3:
					PackItem(new HealScroll(), e);
					break;
				case 4:
					PackItem(new MagicArrowScroll(), e);
					break;
				case 5:
					PackItem(new NightSightScroll(), e);
					break;
				case 6:
					PackItem(new ReactiveArmorScroll(), e);
					break;
				case 7:
					PackItem(new WeakenScroll(), e);
					break;
				case 8:
					PackItem(new AgilityScroll(), e);
					break;
				case 9:
					PackItem(new CunningScroll(), e);
					break;
				case 10:
					PackItem(new CureScroll(), e);
					break;
				case 11:
					PackItem(new HarmScroll(), e);
					break;
				case 12:
					PackItem(new MagicTrapScroll(), e);
					break;
				case 13:
					PackItem(new MagicUnTrapScroll(), e);
					break;
				case 14:
					PackItem(new ProtectionScroll(), e);
					break;
				case 15:
					PackItem(new StrengthScroll(), e);
					break;
				case 16:
					PackItem(new BlessScroll(), e);
					break;
				case 17:
					PackItem(new FireballScroll(), e);
					break;
				case 18:
					PackItem(new MagicLockScroll(), e);
					break;
				case 19:
					PackItem(new PoisonScroll(), e);
					break;
				case 20:
					PackItem(new TelekinesisScroll(), e);
					break;
				case 21:
					PackItem(new TeleportScroll(), e);
					break;
				case 22:
					PackItem(new UnlockScroll(), e);
					break;
				case 23:
					PackItem(new WallOfStoneScroll(), e);
					break;
				case 32:
					PackItem(new BladeSpiritsScroll(), e);
					break;
					//Fifth
				case 33:
					PackItem(new DispelFieldScroll(), e);
					break;
				case 34:
					PackItem(new IncognitoScroll(), e);
					break;
				case 35:
					PackItem(new MagicReflectScroll(), e);
					break;
				case 36:
					PackItem(new MindBlastScroll(), e);
					break;
				case 37:
					PackItem(new ParalyzeScroll(), e);
					break;
				case 38:
					PackItem(new PoisonFieldScroll(), e);
					break;
				case 39:
					PackItem(new SummonCreatureScroll(), e);
					break;
					//Sixth
				case 40:
					PackItem(new DispelScroll(), e);
					break;
				case 41:
					PackItem(new EnergyBoltScroll(), e);
					break;
				case 42:
					PackItem(new ExplosionScroll(), e);
					break;
				case 43:
					PackItem(new InvisibilityScroll(), e);
					break;
				case 44:
					PackItem(new MarkScroll(), e);
					break;
				case 45:
					PackItem(new MassCurseScroll(), e);
					break;
				case 46:
					PackItem(new ParalyzeFieldScroll(), e);
					break;
				case 47:
					PackItem(new RevealScroll(), e);
					break;
			}
		}

		private static Item NecroHue(Item item)
		{
			item.Hue = 0x2C3;

			return item;
		}

		private static void AddSkillItems(SkillName skill, Mobile m, Expansion e)
		{
			bool elf = (m.Race == Race.Elf);

			switch (skill)
			{
				case SkillName.Alchemy:
					{
						PackItem(new Bottle(4), e);
						PackItem(new MortarPestle(), e);

						int hue = Utility.RandomPinkHue();

						if (elf)
						{
							if (m.Female)
							{
								EquipItem(new FemaleElvenRobe(hue), e);
							}
							else
							{
								EquipItem(new MaleElvenRobe(hue), e);
							}
						}
						else
						{
							EquipItem(new Robe(Utility.RandomPinkHue()), e);
						}
						break;
					}
				case SkillName.Anatomy:
					{
						PackItem(new Bandage(3), e);

						int hue = Utility.RandomYellowHue();

						if (elf)
						{
							if (m.Female)
							{
								EquipItem(new FemaleElvenRobe(hue), e);
							}
							else
							{
								EquipItem(new MaleElvenRobe(hue), e);
							}
						}
						else
						{
							EquipItem(new Robe(Utility.RandomPinkHue()), e);
						}
						break;
					}
				case SkillName.AnimalLore:
					{
						int hue = Utility.RandomBlueHue();

						if (elf)
						{
							EquipItem(new WildStaff(), e);

							if (m.Female)
							{
								EquipItem(new FemaleElvenRobe(hue), e);
							}
							else
							{
								EquipItem(new MaleElvenRobe(hue), e);
							}
						}
						else
						{
							EquipItem(new ShepherdsCrook(), e);
							EquipItem(new Robe(hue), e);
						}
						break;
					}
				case SkillName.Archery:
					{
						PackItem(new Arrow(50), e);

						if (elf)
						{
							EquipItem(new ElvenCompositeLongbow(), e);
						}
						else
						{
							EquipItem(new Bow(), e);
						}

						break;
					}
				case SkillName.ArmsLore:
					{
						if (elf)
						{
							switch (Utility.Random(3))
							{
								case 0:
									EquipItem(new Leafblade(), e);
									break;
								case 1:
									EquipItem(new RuneBlade(), e);
									break;
								case 2:
									EquipItem(new DiamondMace(), e);
									break;
							}
						}
						else
						{
							switch (Utility.Random(3))
							{
								case 0:
									EquipItem(new Kryss(), e);
									break;
								case 1:
									EquipItem(new Katana(), e);
									break;
								case 2:
									EquipItem(new Club(), e);
									break;
							}
						}

						break;
					}
				case SkillName.Begging:
					{
						if (elf)
						{
							EquipItem(new WildStaff(), e);
						}
						else
						{
							EquipItem(new GnarledStaff(), e);
						}
						break;
					}
				case SkillName.Blacksmith:
					{
						PackItem(new Tongs(), e);
						PackItem(new Pickaxe(), e);
						PackItem(new Pickaxe(), e);
						PackItem(new IronIngot(50), e);
						EquipItem(new HalfApron(Utility.RandomYellowHue()), e);
						break;
					}
				case SkillName.Fletching:
					{
						PackItem(new Board(14), e);
						PackItem(new Feather(5), e);
						PackItem(new Shaft(5), e);
						break;
					}
				case SkillName.Camping:
					{
						PackItem(new Bedroll(), e);
						PackItem(new Kindling(5), e);
						break;
					}
				case SkillName.Carpentry:
					{
						PackItem(new Board(10), e);
						PackItem(new Saw(), e);
						EquipItem(new HalfApron(Utility.RandomYellowHue()), e);
						break;
					}
				case SkillName.Cartography:
					{
						PackItem(new BlankMap(), e);
						PackItem(new BlankMap(), e);
						PackItem(new BlankMap(), e);
						PackItem(new BlankMap(), e);
						PackItem(new Sextant(), e);
						break;
					}
				case SkillName.Cooking:
					{
						PackItem(new Kindling(2), e);
						PackItem(new RawLambLeg(), e);
						PackItem(new RawChickenLeg(), e);
						PackItem(new RawFishSteak(), e);
						PackItem(new SackFlour(), e);
						PackItem(new Pitcher(BeverageType.Water), e);
						break;
					}
				case SkillName.DetectHidden:
					{
						EquipItem(new Cloak(997), e);
						break;
					}
				case SkillName.Discordance:
					{
						PackInstrument(e);
						break;
					}
				case SkillName.Fencing:
					{
						if (elf)
						{
							EquipItem(new Leafblade(), e);
						}
						else
						{
							EquipItem(new Kryss(), e);
						}

						break;
					}
				case SkillName.Fishing:
					{
						EquipItem(new FishingPole(), e);

						int hue = Utility.RandomYellowHue();

						if (elf)
						{
							Item i = new Circlet();
							i.Hue = hue;
							EquipItem(i, e);
						}
						else
						{
							EquipItem(new FloppyHat(Utility.RandomYellowHue()), e);
						}

						break;
					}
				case SkillName.Healing:
					{
						PackItem(new Bandage(50), e);
						PackItem(new Scissors(), e);
						break;
					}
				case SkillName.Herding:
					{
						if (elf)
						{
							EquipItem(new WildStaff(), e);
						}
						else
						{
							EquipItem(new ShepherdsCrook(), e);
						}

						break;
					}
				case SkillName.Hiding:
					{
						EquipItem(new Cloak(997), e);
						break;
					}
				case SkillName.Inscribe:
					{
						PackItem(new BlankScroll(2), e);
						PackItem(new BlueBook(), e);
						break;
					}
				case SkillName.ItemID:
					{
						if (elf)
						{
							EquipItem(new WildStaff(), e);
						}
						else
						{
							EquipItem(new GnarledStaff(), e);
						}
						break;
					}
				case SkillName.Lockpicking:
					{
						PackItem(new Lockpick(20), e);
						break;
					}
				case SkillName.Lumberjacking:
					{
						EquipItem(new Hatchet(), e);
						break;
					}
				case SkillName.Macing:
					{
						if (elf)
						{
							EquipItem(new DiamondMace(), e);
						}
						else
						{
							EquipItem(new Club(), e);
						}

						break;
					}
				case SkillName.Magery:
					{
						var regs = new BagOfReagents(50);
						/*
						if (e < Expansion.AOS)
						{
							foreach (Item item in regs.Items)
							{
								item.LootType = LootType.Newbied;
							}
						}
						*/
						PackItem(regs, e);

						regs.LootType = LootType.Regular;

						PackScroll(0, e);
						PackScroll(1, e);
						PackScroll(2, e);

						// All characters get a spellbook
						/*
						Spellbook book = new Spellbook((ulong)0xFFFFFFFF);
						book.LootType = LootType.Newbied;
						EquipItem(book, e);
						*/

						if (elf)
						{
							EquipItem(new Circlet(), e);

							if (m.Female)
							{
								EquipItem(new FemaleElvenRobe(Utility.RandomBlueHue()), e);
							}
							else
							{
								EquipItem(new MaleElvenRobe(Utility.RandomBlueHue()), e);
							}
						}
						else
						{
							EquipItem(new WizardsHat(), e);
							EquipItem(new Robe(Utility.RandomBlueHue()), e);
						}

						break;
					}
				case SkillName.Mining:
					{
						PackItem(new Pickaxe(), e);
						break;
					}
				case SkillName.Musicianship:
					{
						PackInstrument(e);
						break;
					}
				case SkillName.Peacemaking:
					{
						PackInstrument(e);
						break;
					}
				case SkillName.Poisoning:
					{
						PackItem(new LesserPoisonPotion(), e);
						PackItem(new LesserPoisonPotion(), e);
						break;
					}
				case SkillName.Provocation:
					{
						PackInstrument(e);
						break;
					}
				case SkillName.Snooping:
					{
						PackItem(new Lockpick(20), e);
						break;
					}
				case SkillName.SpiritSpeak:
					{
						EquipItem(new Cloak(997), e);
						break;
					}
				case SkillName.Stealing:
					{
						PackItem(new Lockpick(20), e);
						break;
					}
				case SkillName.Swords:
					{
						if (elf)
						{
							EquipItem(new RuneBlade(), e);
						}
						else
						{
							EquipItem(new Katana(), e);
						}

						break;
					}
				case SkillName.Tactics:
					{
						if (elf)
						{
							EquipItem(new RuneBlade(), e);
						}
						else
						{
							EquipItem(new Katana(), e);
						}

						break;
					}
				case SkillName.Tailoring:
					{
						//PackItem( new BoltOfCloth() );
						PackItem(new SewingKit(), e);
						break;
					}
				case SkillName.Tracking:
					{
						if (m_Mobile != null)
						{
							Item shoes = m_Mobile.FindItemOnLayer(Layer.Shoes);

							if (shoes != null)
							{
								shoes.Delete();
							}
						}

						int hue = Utility.RandomYellowHue();

						if (elf)
						{
							EquipItem(new ElvenBoots(hue), e);
						}
						else
						{
							EquipItem(new Boots(hue), e);
						}

						EquipItem(new SkinningKnife(), e);
						break;
					}
				case SkillName.Veterinary:
					{
						PackItem(new Bandage(5), e);
						PackItem(new Scissors(), e);
						break;
					}
				case SkillName.Wrestling:
					{
						if (elf)
						{
							EquipItem(new LeafGloves(), e);
						}
						else
						{
							EquipItem(new LeatherGloves(), e);
						}

						break;
					}
			}
		}
	}
}