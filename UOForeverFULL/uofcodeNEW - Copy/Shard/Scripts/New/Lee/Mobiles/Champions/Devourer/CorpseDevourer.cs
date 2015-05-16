#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Server.ContextMenus;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.CustomTitles;
using Server.Items;
using Server.Network;
using Server.Spells;

using VitaNex;
using VitaNex.FX;
using VitaNex.Network;
using VitaNex.SuperGumps;
#endregion

namespace Server.Mobiles
{
	[CorpseName("remains of a corpse devourer")]
	public class CorpseDevourer : BaseChampion
	{
		private static readonly int[] _BodyHues = new[]
		{
			//
			0x86DB, 0x86DC, 0x86DD, 0x86DE, 0x86DF, //
			0x86E0, 0x86E1, 0x86E2, 0x86E3, 0x86E4, //
			0x86E5, 0x86E6 //
		};

		private static readonly int[] _HairHues = new[]
		{
			//
			0x709, 0x70B, 0x70D, 0x70F, 0x711, 0x763, //
			0x765, 0x768, 0x76B, 0x6F3, 0x6F1, 0x6EF, //
			0x6E4, 0x6E2, 0x6E0, 0x709, 0x70B, 0x70D //
		};

		private static readonly AIType[] _PossibleAI = new[]
		{
			//
			AIType.AI_Melee, AIType.AI_Mage, AIType.AI_Archer //
		};

		private static readonly Layer[] _InvalidLayers = new[]
		{
			//
			Layer.Hair, Layer.FacialHair, Layer.Face, // 
			Layer.ShopBuy, Layer.ShopResale, Layer.ShopSell, //
			Layer.Mount, Layer.Invalid, //
			Layer.Bank, Layer.Backpack // 
		};

		private static readonly Layer[] _EquipLayers = ((Layer)0).GetValues<Layer>().Not(_InvalidLayers.Contains).ToArray();

		private static readonly SkillName[] _Skills = ((SkillName)0).GetValues<SkillName>();

		private static bool ConstructItem(out Item item, params Type[] types)
		{
			return (item = (types == null || types.Length == 0) ? null : types.GetRandom().CreateInstanceSafe<Item>()) != null;
		}

		private DateTime _NextDevour = DateTime.UtcNow;
		private DateTime _NextAISwitch = DateTime.UtcNow;
		private DateTime _NextSpecial = DateTime.UtcNow;

		public virtual TimeSpan DevourInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(5, 15)); } }
		public virtual TimeSpan AISwitchInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 120)); } }

		public virtual bool DecrepifyEnabled { get { return Alive; } }
		public virtual TimeSpan DecrepifyInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(15, 45)); } }
		public virtual TimeSpan DecrepifyDuration { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10)); } }
		public virtual int DecrepifyRange { get { return Utility.RandomMinMax(8, 16); } }
		public virtual int DecrepifySpeed { get { return Utility.RandomMinMax(5, 10); } }

		public virtual bool ThrowBomb { get { return Alive; } }
		public virtual TimeSpan ThrowBombInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 45)); } }

		public virtual bool Inquisition { get { return Alive; } }
		public virtual TimeSpan InquisitionInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60)); } }
		public virtual TimeSpan InquisitionDuration { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(3, 5)); } }

		public override string DefaultName { get { return "a corpse devourer"; } }
		public override FoodType FavoriteFood { get { return FoodType.Gold; } }
		public override bool CanRummageCorpses { get { return true; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool CanDestroyObstacles { get { return true; } }
		public override bool CanFlee { get { return false; } }
		public override bool AutoDispel { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override bool ShowFameTitle { get { return false; } }

		public override ChampionSkullType SkullType { get { return ChampionSkullType.Death; } }

		[Constructable]
		public CorpseDevourer()
			: base(_PossibleAI.GetRandom(), FightMode.Closest, 16, 1, 0.1, 0.2)
		{
			Name = NameList.RandomName("daemon");
			Title = "the Devourer";

			Female = Utility.RandomBool();

			Body = Female ? 667 : 666;

			Fame = 11250;
			Karma = -11250;

			SpeechHue = YellHue = 34;

			VirtualArmor = 50;

			BaseSoundID = 372;

			InitStats(666, 666, 666);
			SetHits(5000, 15000);
			SetDamage(35, 45);

			foreach (SkillName skill in _Skills)
			{
				SetSkill(skill, 80.0, 120.0);
			}

			RandomBodyHue();
			RandomHair();
			RandomFacialHair();
			RandomHairHue();

			if (Backpack != null)
			{
				Backpack.Delete();
			}

			EquipItem(new BottomlessBackpack());

			EquipItem(
				new Robe
				{
					Name = "Robe of Inquisition",
					Hue = 16385,
					Movable = false,
					LootType = LootType.Blessed
				});

			PackItems();
		}

		public CorpseDevourer(Serial serial)
			: base(serial)
		{ }

		protected virtual Item[] PackItems()
		{
			Func<Item, string> resolveName = i =>
			{
				string name = i.ResolveName().ToUpperWords();

				if (Insensitive.StartsWith(name, "a "))
				{
					name = name.Substring(2);
				}
				else if (Insensitive.StartsWith(name, "an "))
				{
					name = name.Substring(3);
				}

				return "Hunting " + name;
			};

			var list = new List<Item>();
			Item item;

			if (ConstructItem(
				out item,
				typeof(Cleaver),
				typeof(Dagger),
				typeof(Longsword),
				typeof(Broadsword),
				typeof(Kryss),
				typeof(Halberd),
				typeof(Scythe)))
			{
				item.Name = resolveName(item);
				item.LootType = LootType.Blessed;

				PackItem(item);

				list.Add(item);
			}

			if (ConstructItem(
				out item,
				typeof(Buckler),
				typeof(WoodenKiteShield),
				typeof(MetalKiteShield),
				typeof(HeaterShield),
				typeof(MetalShield),
				typeof(BronzeShield)))
			{
				item.Name = resolveName(item);
				item.LootType = LootType.Blessed;

				PackItem(item);

				list.Add(item);
			}

			if (ConstructItem(out item, typeof(Bow), typeof(Crossbow)))
			{
				item.Name = resolveName(item);
				item.LootType = LootType.Blessed;

				PackItem(item);

				list.Add(item);
			}

			return list.ToArray();
		}

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;

            for (int i = 0; i < eligibleMobScores.Count; i++)
            {
                currentTestValue += eligibleMobScores[i];

                if (roll > currentTestValue)
                {
                    continue;
                }

                DevourerSoul heart = new DevourerSoul();
                if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                {
                    eligibleMobs[i].Backpack.DropItem(heart);
                    eligibleMobs[i].SendMessage(54, "You have captured the soul of the Devourer!");
                    return;
                }
            }
        }

		public override void Damage(int amount, Mobile m, bool informMount)
		{
			// Poison will cause all damage to heal the devourer instead.
			if (Poison != null)
			{
				// Uncomment to allow damage while poison to be a beneficial action.
				// This will cause the damager to go grey/criminal.
				/*
				if (m != null)
				{
					m.DoBeneficial(this);
				}
				*/

				Hits += amount;

				if (Utility.RandomDouble() < 0.10)
				{
					NonlocalOverheadMessage(
						MessageType.Regular,
						0x21,
						false,
						String.Format(
							"*{0} {1}*", Name, Utility.RandomList("looks healthy", "looks stronger", "is absorbing damage", "is healing")));
				}
			}
			else
			{
				base.Damage(amount, m, informMount);
			}
		}

		public override void OnPoisoned(Mobile from, Poison poison, Poison oldPoison)
		{
			NonlocalOverheadMessage(MessageType.Regular, 0x21, false, "*The poison seems to have the opposite effect*");
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			list.RemoveAll(e => e is PaperdollEntry);
		}

		public override void DisplayPaperdollTo(Mobile to)
		{
			base.DisplayPaperdollTo(to);

			if (Inquisition && to.AccessLevel < AccessLevel.Counselor && to is PlayerMobile)
			{
				PerformInquisition((PlayerMobile)to, InquisitionDuration);
			}
		}

		public override void OnThink()
		{
			base.OnThink();

			DateTime now = DateTime.UtcNow;

			if (now > _NextAISwitch)
			{
				_NextAISwitch = now + AISwitchInterval;

				SwitchAI();
			}

			if (!this.InCombat() || now <= _NextSpecial)
			{
				return;
			}

			TimeSpan delay;
			Action ability = GetSpecialAbility(out delay);

			if (ability == null)
			{
				return;
			}

			_NextSpecial = now + delay;

			VitaNexCore.TryCatch(ability, x => x.ToConsole(true));
		}

		public virtual Action GetSpecialAbility(out TimeSpan delay)
		{
			switch (Utility.Random(3))
			{
				case 0:
					delay = ThrowBombInterval;
					return ThrowBombs;
				case 1:
					delay = InquisitionInterval;
					return PerformInquisition;
				case 2:
					delay = DecrepifyInterval;
					return Decrepify;
				default:
					delay = TimeSpan.Zero;
					return null;
			}
		}

		#region AI Switching
		protected virtual void SwitchAI()
		{
			if (Deleted || Map == null || Map == Map.Internal || Location == Point3D.Zero)
			{
				return;
			}
			
			AI = _PossibleAI.Not(ai => ai == AI).GetRandom(AIType.AI_Mage);

			if (AI == AIType.AI_Archer || AI == AIType.AI_Mage)
			{
				RangeFight = 12;
			}
			else
			{
				RangeFight = 1;
			}

			SwitchEquipment();
		}
		#endregion

		#region Equipment Switching
		protected virtual void Undress(params Layer[] layers)
		{
			Items.Not(i => i == null || i.Deleted || !i.Movable || i == Backpack || i == FindBankNoCreate() || i == Mount)
				 .Not(i => _InvalidLayers.Contains(i.Layer))
				 .Where(item => layers == null || layers.Length == 0 || layers.Contains(item.Layer))
				 .ForEach(Backpack.DropItem);
		}

		protected virtual void SwitchEquipment()
		{
			if (Deleted || Map == null || Map == Map.Internal || Location == Point3D.Zero)
			{
				return;
			}

			Undress(Layer.OneHanded, Layer.TwoHanded);

			Item weapon;
			Item shield = null;

			switch (AI)
			{
				case AIType.AI_Archer:
					{
						weapon = Backpack.FindItemsByType<BaseRanged>(true, w => w != null && !w.Deleted && w.CanEquip(this)).GetRandom();

						if (weapon != null)
						{
							var ranged = (BaseRanged)weapon;

							if (ranged.AmmoType != null)
							{
								Item ammo = Backpack.FindItemByType(ranged.AmmoType, true);

								if (ammo == null || ammo.Deleted || ammo.Amount <= 0)
								{
									ammo = ranged.AmmoType.CreateInstanceSafe<Item>();

									if (ammo != null)
									{
										ammo.Amount = 100;
										ammo.LootType = LootType.Blessed;

										Backpack.DropItem(ammo);
									}
								}
							}
						}
					}
					break;
				case AIType.AI_Mage:
					{
						weapon =
							Backpack.FindItemsByType<Spellbook>(true, w => w != null && !w.Deleted && w.CanEquip(this)).GetRandom() ??
							new Spellbook(Int64.MaxValue)
							{
								LootType = LootType.Blessed
							};

						var regs = Backpack.FindItemByType<BagOfReagents>(true);

						if (regs == null || regs.Deleted || regs.Amount <= 0 || regs.Items.Count <= 0)
						{
							regs = new BagOfReagents(100)
							{
								LootType = LootType.Blessed
							};

							Backpack.DropItem(regs);
						}
					}
					break;
				case AIType.AI_Melee:
					{
						weapon =
							Backpack.FindItemsByType<BaseMeleeWeapon>(true, w => w != null && !w.Deleted && w.CanEquip(this)).GetRandom();

						if (weapon != null && weapon.Layer != Layer.TwoHanded)
						{
							shield = Backpack.FindItemsByType<BaseShield>(true, s => s != null && !s.Deleted && s.CanEquip(this)).GetRandom();
						}
					}
					break;
				default:
					weapon = Backpack.FindItemsByType<BaseWeapon>(true, w => w != null && !w.Deleted && w.CanEquip(this)).GetRandom();
					break;
			}

			if (weapon != null)
			{
				EquipItem(weapon);
			}

			if (shield != null)
			{
				EquipItem(shield);
			}

			Undress(_EquipLayers);

			var equip = new Item[_EquipLayers.Length];

			equip.SetAll(
				i =>
				FindItemOnLayer(_EquipLayers[i]) ??
				Backpack.FindItemsByType<Item>(true, item => item != null && !item.Deleted && item.CanEquip(this)).GetRandom());

			foreach (Item item in equip.Where(item => item != null && !item.IsEquipped()))
			{
				EquipItem(item);
			}
		}
		#endregion

		#region Decrepify
		protected virtual void Decrepify()
		{
			if (!DecrepifyEnabled || Deleted || Map == null || Map == Map.Internal || Location == Point3D.Zero)
			{
				return;
			}

			Yell(Utility.RandomBool() ? "YOUR SOUL WITHERS!" : "YOUR SOUL IN CHAINS!");

			new PoisonExplodeEffect(
				Location,
				Map,
				DecrepifyRange,
				0,
				TimeSpan.FromMilliseconds(Math.Max(0, 1000 - ((DecrepifySpeed - 1) * 100))),
				e =>
				{
					foreach (Mobile m in AcquireTargets(e.Source.Location, 0).Take(5))
					{
						Decrepify(m);
					}
				}).Send();
		}

		protected virtual void Decrepify(Mobile m)
		{
			if (m == null || m.Deleted || m.Map != Map)
			{
				return;
			}

			m.TryParalyze(DecrepifyDuration, DecrepifyFade);

			if (!m.Paralyzed)
			{
				return;
			}

			m.SendMessage("Decrepification paralyzes your soul!");

			new MovingEffectQueue(deferred: false)
			{
				new MovingEffectInfo(m.Clone3D(-1, -1, 50), m, m.Map, 8700, 0, 1, EffectRender.Darken),
				new MovingEffectInfo(m.Clone3D(-1, +1, 50), m, m.Map, 8700, 0, 1, EffectRender.Darken),
				new MovingEffectInfo(m.Clone3D(+1, -1, 50), m, m.Map, 8700, 0, 1, EffectRender.Darken),
				new MovingEffectInfo(m.Clone3D(+1, +1, 50), m, m.Map, 8700, 0, 1, EffectRender.Darken)
			}.Process();
		}

		protected virtual void DecrepifyFade(Mobile m)
		{
			if (m == null || m.Deleted || m.Map != Map)
			{
				return;
			}

			if (m.Paralyzed)
			{
				m.Paralyzed = false;
			}

			m.SendMessage("Decrepification has faded, your soul is unchained!");
		}
		#endregion

		#region ThrowBombs
		protected virtual void ThrowBombs()
		{
			if (!ThrowBomb || Deleted || Map == null || Map == Map.Internal || Location == Point3D.Zero)
			{
				return;
			}

			bool shout = false;

			foreach (Mobile m in AcquireTargets(Location, RangePerception).Take(5))
			{
				int damage = (int)Math.Floor(m.Hits * 0.20);
				
				new MovingEffectInfo(this, m, Map, 8700).MovingImpact(
					e =>
					{
						new FirePentagramEffect(m, Map).Send();
						m.Damage(damage, this, true);
					});

				shout = true;
			}

			if (shout)
			{
				Yell(Utility.RandomBool() ? "CLEANSE BY FIRE!" : "CONFLAGRATE!");
			}
		}
		#endregion

		#region Inquisition
		private Timer _ChannelingTimer;

		protected virtual void OnChannelingTick(int index, double[] durations)
		{
			if (durations == null || !durations.InBounds(index))
			{
				return;
			}

			/*double p = Hits / (double)HitsMax;

			if (p <= 0.50)
			{
				return;
			}*/

			double cur = durations[index];

			// Give the boss a weakness! 
			// 10 damage and 20 mana loss for every 1 second the target is under inquisition.
			Hits -= (int)Math.Floor(10.0 * cur);
			Mana -= (int)Math.Floor(20.0 * cur);
		}

		protected virtual void PerformInquisition()
		{
			if (!Inquisition || Deleted || Map == null || Map == Map.Internal || Location == Point3D.Zero)
			{
				return;
			}

			int range = (int)Math.Ceiling(RangePerception * (Hits / (double)HitsMax));

			var targets = AcquireTargets(Location, range).Take(10).ToArray();

			if (targets.Length == 0)
			{
				return;
			}

			List<double> durations = new List<double>(targets.Length);

			targets.For(
				(i, m) =>
				{
					var duration = InquisitionDuration;

					if (m is PlayerMobile)
					{
						if (PerformInquisition((PlayerMobile)m, duration))
						{
							durations.Add(duration.TotalSeconds);
						}
					}
					else
					{
						m.TryParalyze(duration, mc => durations.Add(duration.TotalSeconds));
					}
				});

			if (durations.Count == 0)
			{
				return;
			}

			if (_ChannelingTimer != null)
			{
				_ChannelingTimer.Stop();
				_ChannelingTimer = null;
			}

			double min = durations.Min();
			double avg = durations.Average();

			int tick = 0;

			_ChannelingTimer = Timer.DelayCall(
				TimeSpan.FromSeconds(min),
				TimeSpan.FromSeconds(avg),
				durations.Count,
				() => OnChannelingTick(tick++, durations.ToArray()));
		}

		protected virtual bool PerformInquisition(PlayerMobile m, TimeSpan duration)
		{
			if (!Inquisition || m == null)
			{
				return false;
			}

			SuperGump ig = CreateInquisitionGump(m, duration);

			if (ig != null)
			{
				return ig.Send() == ig;
			}

			return false;
		}

		protected virtual SuperGump CreateInquisitionGump(PlayerMobile m, TimeSpan duration)
		{
			return Inquisition ? new InquisitionGump(m, duration) : null;
		}
		#endregion

		public virtual IEnumerable<Mobile> AcquireTargets(Point3D p, int range)
		{
			return
				p.GetMobilesInRange(Map, range)
				 .Where(
					 m =>
					 m != null && !m.Deleted && m != this && m.AccessLevel <= AccessLevel && m.Alive && CanBeHarmful(m, false, true) &&
					 SpellHelper.ValidIndirectTarget(this, m) && (m.Party == null || m.Party != Party) &&
					 (m.Player || Combatant == m || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)));
		}

		protected virtual void RandomBodyHue()
		{
			Hue = Utility.RandomList(_BodyHues);
		}

		protected virtual void RandomHair()
		{
			if (Utility.Random(9) == 0)
			{
				return;
			}

			if (Female)
			{
				HairItemID = Utility.RandomList(0x4261, 0x4262, 0x4273, 0x4274, 0x4275, 0x42B0, 0x42B1, 0x42AA, 0x42AB);
			}
			else
			{
				HairItemID = 0x4258 + Utility.Random(8);
			}
		}

		protected virtual void RandomFacialHair()
		{
			if (!Female)
			{
				FacialHairItemID = Utility.RandomList(0, 0x42AD, 0x42AE, 0x42AF, 0x42B0);
			}
		}

		protected virtual void RandomHairHue()
		{
			HairHue = FacialHairHue = Utility.RandomList(_HairHues);
		}

		public override int GetAngerSound()
		{
			return 373;
		}

		public override int GetAttackSound()
		{
			return Female ? 1524 : 1528;
		}

		public override int GetDeathSound()
		{
			return Female ? 1525 : 1529;
		}

		public override int GetHurtSound()
		{
			return Female ? 1527 : 1531;
		}

		public override int GetIdleSound()
		{
			return Utility.RandomList(372, 373);
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.SuperBoss, 2);
		}

		public override void OnCombatantChange()
		{
			base.OnCombatantChange();

			Flying = Hits / (double)HitsMax > 0.10 && this.InCombat();
		}

		protected override bool OnMove(Direction d)
		{
			bool allow = base.OnMove(d);

			DateTime now = DateTime.UtcNow;

			if (now < _NextDevour)
			{
				return allow;
			}

			if (allow)
			{
				foreach (Corpse corpse in
					this.GetEntitiesInRange<Corpse>(Map, 2).Take(10)
						.Where(
							c => c != null && !c.Deleted && !c.IsDecoContainer && !c.DoesNotDecay && !c.IsBones && c.Owner is PlayerMobile))
				{
					Emote("*You see {0} completely devour a corpse and its contents*", RawName);

					foreach (Item item in
						corpse.Items.Where(item => item != null && !item.Deleted && item.Movable && item.Visible).ToArray())
					{
						Backpack.DropItem(item);
					}

					corpse.TurnToBones();
				}

				SwitchEquipment();

				_NextDevour = now + DevourInterval;
			}

			return allow;
		}

        public override void OnDeath(Container c)
        {
            if (0.025 > Utility.RandomDouble())
            {
                c.DropItem(new TitleScroll("The Nibbler"));
            }

            base.OnDeath(c);
        }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}

		private sealed class BottomlessBackpack : StrongBackpack
		{
			public override int MaxWeight { get { return Int16.MaxValue; } }

			public BottomlessBackpack()
			{
				MaxItems = Int16.MaxValue;
				Movable = false;
				Hue = 16385;
				Weight = 0.0;
			}

			public BottomlessBackpack(Serial serial)
				: base(serial)
			{ }

			public override void Serialize(GenericWriter writer)
			{
				base.Serialize(writer);

				writer.SetVersion(0);
			}

			public override void Deserialize(GenericReader reader)
			{
				base.Deserialize(reader);

				reader.GetVersion();
			}
		}

		protected sealed class InquisitionPath : IEnumerable<Point3D>
		{
			public Point3D[] Path { get; set; }
			public int Index { get; set; }

			public int Length { get { return Path.Length; } }

			public Point3D Current { get { return this[Index]; } }

			public Point3D this[int index] { get { return Path.InBounds(index) ? Path[index] : Point3D.Zero; } }

			public object Value { get; set; }

			public InquisitionPath(object value, params Point3D[] path)
			{
				Value = value;
				Path = path ?? new Point3D[0];
				Index = 0;
			}

			public IEnumerator<Point3D> GetEnumerator()
			{
				return Path.AsEnumerable().GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		protected sealed class InquisitionGump : SuperGumpList<InquisitionPath>
		{
			private static readonly Rectangle3D _Bounds = new Rectangle3D(0, 0, 0, 1920, 1080, 25);

			private static readonly Color[] _HtmlColors = new[]
			{Color.LightGray, Color.Red, Color.DarkRed, Color.OrangeRed, Color.DarkOrange};

			private static readonly string[] _Phrases = new[]
			{
				"hell is empty and all the devils are here",
				"we enjoy the night, the darkness, where we can do things that aren't " +
				"acceptable in the light.\nnight is when we slake our thirst",
				"its symptoms are madness, caused by the music in his head, sung by an endless choir, " +
				"called 'the voice of the dead'",
				"once the game is over, the king and the pawn go back in the same box",
				"he who dies with the most toys is, nonetheless, still dead",
				"it is possible to provide security against other ills, but as far as death is concerned, " +
				"we men live in a city without walls",
				"death most resembles a prophet who is without honor in his own land or a poet " +
				"who is a stranger among his people",
				"death is the only inescapable, unavoidable, sure thing.\nwe are sentenced to die the day we're born",
				"a man who won't die for something is not fit to live", "one death is a tragedy.\na million deaths is a statistic",
				"you can discover what your enemy fears most by observing the means he uses to frighten you",
				"courage is not the absence of fear, but rather the judgement that something else is more important than fear",
				"the only thing we have to fear is fear itself", "the basis of optimism is sheer terror"
			};

			private static readonly int[] _Images = new[]
			{
				//
				69, 70, 101, 102, 2251, 2259, 2274, 2278, 2279, //
				2280, 2300, 7011, 7019, 7034, 7038, 7039, 7040, //
				7060, 9804, 30501, 30505 //
			};

			private static readonly int[] _ItemIDs = new[]
			{
				//
				3786, 3787, 3789, 3790, 3791, 3792, 3793, 3794, //
				3799, 3800, 4650, 4651, 4652, 4653, 4654, 4655, //
				6657, 6658, 6659, 6660, 6661, 6662, 6663, 6664, //
				6665, 6666, 6667, 6668, 6669, 6670 //
			};

			private DateTime? _Sent;
			private Body _BodyMod;
			private int _PhraseCount;

			public TimeSpan Duration { get; set; }

			public InquisitionGump(PlayerMobile user, TimeSpan duration)
				: base(user, null, 0, 0, null)
			{
				Duration = duration;

				CanClose = false;
				CanDispose = false;
				CanMove = false;
				CanResize = false;

				Modal = true;
				ModalSafety = false;

				Sorted = true;
				ForceRecompile = true;

				AutoRefreshRate = TimeSpan.FromMilliseconds(100.0);
				AutoRefresh = true;
			}

			protected override void CompileList(List<InquisitionPath> list)
			{
				if (_Sent == null)
				{
					list.Clear();
					list.AddRange(GenerateSprites());
				}
				else
				{
					list.RemoveAll(e => e == null || e.Length == 0 || e.Index >= e.Length);
					list.ForEach(e => ++e.Index);
				}

				EntriesPerPage = list.Count;
			}

			protected override void CompileLayout(SuperGumpLayout layout)
			{
				base.CompileLayout(layout);

				Dictionary<int, InquisitionPath> range = GetListRange();

				if (range != null && range.Count > 0)
				{
					range.Values.For((index, entry) => CompileEntryLayout(layout, index, entry));
				}
			}

			public void CompileEntryLayout(SuperGumpLayout layout, int index, InquisitionPath entry)
			{
				layout.Add(
					"sprite/" + index,
					xpath =>
					{
						Point3D cur = entry.Current;

						if (entry.Value is string)
						{
							AddHtml(cur.X, cur.Y, 200, 200, (string)entry.Value, false, false);
						}
						else if (entry.Value is Pair<bool, int>)
						{
							var pair = (Pair<bool, int>)entry.Value;

							if (pair.Left)
							{
								AddItem(cur.X, cur.Y, pair.Right, 0);
							}
							else
							{
								AddImage(cur.X, cur.Y, pair.Right, 0);
							}
						}
					});
			}

			private IEnumerable<InquisitionPath> GenerateSprites()
			{
				double rate = AutoRefreshRate.TotalMilliseconds;
				double duration = Duration.TotalMilliseconds;

				var frames = (int)(Math.Max(rate, duration) / rate);

				//var center = new Point2D(_Bounds.Start.X + (_Bounds.Width / 2), _Bounds.Start.Y + (_Bounds.Height / 2));

				for (int index = 0; index < _Bounds.Depth; index++)
				{
					object sprite = GenerateSprite();

					bool html = sprite is string;

					int o = html ? 200 : 100;

					var start = new Point2D(
						Utility.RandomMinMax(_Bounds.Start.X, _Bounds.Start.X + (_Bounds.Width - o)),
						Utility.RandomMinMax(_Bounds.Start.Y, _Bounds.Start.Y + (_Bounds.Height - o)));

					var end = new Point2D(
						Utility.RandomMinMax(_Bounds.Start.X, _Bounds.Start.X + (_Bounds.Width - o)),
						Utility.RandomMinMax(_Bounds.Start.Y, _Bounds.Start.Y + (_Bounds.Height - o)));

					var path = new Point3D[frames];

					path.SetAll(frame => start.Lerp2D(end, frame / (double)frames).ToPoint3D(index));

					yield return new InquisitionPath(sprite, path);
				}
			}

			public object GenerateSprite()
			{
				if (Utility.RandomBool() && _PhraseCount < 5)
				{
					++_PhraseCount;
					return RandomFormatting(_Phrases.GetRandom());
				}

				bool item = Utility.RandomBool();

				return Pair.Create(item, (item ? _ItemIDs : _Images).GetRandom());
			}

			public string RandomFormatting(string html)
			{
				char[] chars = html.ToCharArray();

				chars.SetAll(
					(j, c) =>
					Char.IsLetter(c) && Utility.RandomDouble() <= 0.33 ? (Char.IsUpper(c) ? Char.ToLower(c) : Char.ToUpper(c)) : c);

				html = String.Join(String.Empty, chars);

				switch (Utility.Random(3))
				{
					case 0:
						html = html.WrapUOHtmlTag("small");
						break;
					case 1:
						html = html.WrapUOHtmlTag("big");
						break;
				}

				return html.WrapUOHtmlColor(Color.DarkGray.Interpolate(_HtmlColors.GetRandom(), Utility.RandomDouble()), false);
			}

			protected override void OnSend()
			{
				base.OnSend();

				if (IsDisposed || _Sent != null)
				{
					return;
				}

				_Sent = DateTime.UtcNow;

				_BodyMod = User.BodyMod;

				User.SendSound(1383); // icu

				User.BodyMod = User.Race.GhostBody(User);
				ScreenFX.FadeOut.Send(User);

				User.TryParalyze(Duration, m => m.SendMessage("The inquisition is over."));
			}

			protected override void OnClosed(bool all)
			{
				if (_Sent != null)
				{
					_Sent = null;

					User.BodyMod = _BodyMod;
					ScreenFX.FadeIn.Send(User);
				}

				base.OnClosed(all);
			}

			protected override bool CanAutoRefresh()
			{
				return _Sent != null && base.CanAutoRefresh();
			}

			protected override void OnAutoRefresh()
			{
				base.OnAutoRefresh();

				if (_Sent != null && DateTime.UtcNow > _Sent + Duration)
				{
					Close(true);
				}
			}

			public override int SortCompare(InquisitionPath a, InquisitionPath b)
			{
				int result = 0;

				if (a.CompareNull(b, ref result))
				{
					return result;
				}

				int zL = a.Current.Z;
				int zR = b.Current.Z;

				if (zL > zR)
				{
					return 1;
				}

				if (zL < zR)
				{
					return -1;
				}

				return 0;
			}
		}
	}
}