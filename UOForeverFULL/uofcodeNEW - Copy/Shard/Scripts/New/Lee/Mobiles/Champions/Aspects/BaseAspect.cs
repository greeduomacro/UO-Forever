#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.ContextMenus;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Network;

using VitaNex;
#endregion

namespace Server.Mobiles
{
	[Flags]
	public enum Aspects : ulong
	{
		None = 0x0,
		Time = 0x1,
		Light = 0x2,
		Dark = 0x4,
		Faith = 0x8,
		Despair = 0x10,
		Illusion = 0x20,
		Life = 0x40,
		Death = 0x80,
		Elements = 0x100,

		All = Time | Light | Dark | Faith | Despair | Illusion | Life | Death | Elements
	}

	[CorpseName("remains of an Aspect")]
	public abstract class BaseAspect : BaseChampion
	{
		private static readonly Aspects[] _InitialAspects = ((Aspects)0).GetValues<Aspects>();
		private static readonly SkillName[] _InitialSkills = ((SkillName)0).GetValues<SkillName>();

		public static Type[] AspectTypes { get; private set; }

		public static List<BaseAspect> Instances { get; private set; }

		static BaseAspect()
		{
			AspectTypes = typeof(BaseAspect).GetConstructableChildren();

			Instances = new List<BaseAspect>();
		}

		public static BaseAspect CreateRandomAspect()
		{
			return CreateAspect(AspectTypes.GetRandom());
		}

		public static BaseAspect CreateAspect(Type t)
		{
			return t != null && t.IsChildOf<BaseAspect>() ? t.CreateInstanceSafe<BaseAspect>() : null;
		}

		public static TAspect CreateAspect<TAspect>()
		{
			return typeof(TAspect).CreateInstanceSafe<TAspect>();
		}

		private static bool ConstructItem(out Item item, params Type[] types)
		{
			return (item = (types == null || types.Length == 0)
							   ? null //
							   : types.GetRandom().CreateInstanceSafe<Item>()) != null;
		}

		private DateTime _NextAbility = DateTime.UtcNow;

		public override ChampionSkullType SkullType { get { return ChampionSkullType.Aspect; } }

		public override string DefaultName { get { return NameList.RandomName("daemon"); } }
		public override FoodType FavoriteFood { get { return FoodType.All; } }
		public override bool CanRummageCorpses { get { return false; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool CanDestroyObstacles { get { return true; } }
		public override bool CanFlee { get { return false; } }
		public override bool AutoDispel { get { return true; } }
		public override bool Unprovokable { get { return true; } }
		public override bool Uncalmable { get { return true; } }
		public override bool ShowFameTitle { get { return false; } }

		public virtual SkillName[] InitialSkills { get { return _InitialSkills; } }

		public virtual bool HealFromPoison { get { return Enraged; } }

		public abstract Aspects Aspects { get; }

		public virtual double EnrageThreshold { get { return 0.10; } }

		private readonly StatBuffInfo _EnrageStatBuff = new StatBuffInfo(StatType.All, "Enraged", 500, TimeSpan.Zero);

		public StatType EnrageBuffType { get { return _EnrageStatBuff.Type; } set { _EnrageStatBuff.Type = value; } }
		public string EnrageBuffName { get { return _EnrageStatBuff.Name; } set { _EnrageStatBuff.Name = value; } }
		public int EnrageBuffOffset { get { return _EnrageStatBuff.Offset; } set { _EnrageStatBuff.Offset = value; } }
		public TimeSpan EnrageBuffDuration { get { return _EnrageStatBuff.Duration; } set { _EnrageStatBuff.Duration = value; } }

		public bool Enraged { get { return Hits / (double)HitsMax <= EnrageThreshold; } }

		public BaseAspect(
			AIType aiType, FightMode mode, int perception, int rangeFight, double activeSpeed, double passiveSpeed)
			: base(aiType, mode, perception, rangeFight, activeSpeed, passiveSpeed)
		{
			Instances.Add(this);

			Female = Utility.RandomBool();

			Title = InitTitle();

			Body = InitBody();

			Fame = 50000;
			Karma = -50000;

			SpeechHue = YellHue = 85;

			VirtualArmor = 50;
			
			SetStr(500, 1000);
			SetDex(500, 1000);
			SetInt(500, 1000);

			SetHits(10000, 50000);
			SetStam(1000, 5000);
			SetMana(1000, 5000);

			SetDamage(10, 25);

			foreach (SkillName skill in InitialSkills)
			{
				SetSkill(skill, 90.0, 120.0);
			}

			Container pack = Backpack;

			if (pack != null)
			{
				pack.Delete();
			}

			EquipItem(new BottomlessBackpack());

			PackItems();
			EquipItems();
		}

		public BaseAspect(Serial serial)
			: base(serial)
		{
			Instances.Add(this);
		}

		protected abstract int InitBody();

		protected virtual string InitTitle()
		{
			string title = "the Aspect";

			if (Aspects == Aspects.None)
			{
				return title;
			}

			if (Aspects == Aspects.All)
			{
				title += " of Infinity";
				return title;
			}

			Aspects[] flags =
				_InitialAspects.Not(a => a == Aspects.None || a == Aspects.All).Where(a => Aspects.HasFlag(a)).ToArray();

			if (flags.Length > 0)
			{
				title += " of ";

				for (int i = 0; i < flags.Length; i++)
				{
					title += flags[i];

					if (i + 1 < flags.Length)
					{
						title += i + 2 < flags.Length ? ", " : " and ";
					}
				}
			}

			return title;
		}

		protected virtual void PackItems()
		{ }

		protected virtual void EquipItems()
		{ }

		public override void Damage(int amount, Mobile m, bool informMount)
		{
			// Poison will cause all damage to heal instead.
			if (HealFromPoison && Poison != null)
			{
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

		public override void OnPoisoned(Mobile m, Poison poison, Poison oldPoison)
		{
			if (HealFromPoison)
			{
				NonlocalOverheadMessage(MessageType.Regular, 0x21, false, "*The poison seems to have the opposite effect*");
				return;
			}

			base.OnPoisoned(m, poison, oldPoison);
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			list.RemoveAll(e => e is PaperdollEntry);
		}

		public override void OnThink()
		{
			base.OnThink();

			if (!this.InCombat())
			{
				return;
			}

			if (EnrageThreshold > 0 && Enraged)
			{
				var old = GetStatMod(EnrageBuffName);

				if (old != null && old.Type != EnrageBuffType && old.Offset != EnrageBuffOffset && RemoveStatMod(EnrageBuffName))
				{
					old = null;
				}

				if (old == null && EnrageBuffOffset != 0 && !String.IsNullOrWhiteSpace(EnrageBuffName))
				{
					AddStatMod(_EnrageStatBuff);
				}
			}

			DateTime now = DateTime.UtcNow;

			if (Aspects == Aspects.None || now <= _NextAbility)
			{
				_NextAbility = now.AddSeconds(1.0);
				return;
			}

			AspectAbility ability = GetRandomAbility(true);

			if (ability == null || !ability.TryInvoke(this))
			{
				_NextAbility = now.AddSeconds(1.0);
				return;
			}

			TimeSpan cooldown = GetAbilityCooldown(ability);

			_NextAbility = cooldown > TimeSpan.Zero ? now.Add(cooldown) : now;
		}

		protected virtual AspectAbility GetRandomAbility(bool checkLock)
		{
			var abilities = GetAbilities(checkLock);

			return abilities.GetRandom();
		}

		protected virtual TimeSpan GetAbilityCooldown(AspectAbility ability)
		{
			return ability != null ? ability.Cooldown : TimeSpan.Zero;
		}

		public virtual AspectAbility[] GetAbilities(bool checkLock)
		{
			return AspectAbility.GetAbilities(this, checkLock);
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.SuperBoss, 4);
		}

		public override void Delete()
		{
			Instances.Remove(this);

			base.Delete();
		}

		public override void OnDelete()
		{
			Instances.Remove(this);

			base.OnDelete();
		}

		public override void OnAfterDelete()
		{
			Instances.Remove(this);

			base.OnAfterDelete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
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
					break;
			}
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

			public override void OnSnoop(Mobile from)
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
	}
}