#region References
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.Conquests;
using Server.Engines.Plants;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;
using Server.Engines.Quests.Matriarch;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server.Items
{
	public enum BeverageType
	{
		Ale,
		Cider,
		Liquor,
		Milk,
		Wine,
		Water,
		Mead
	}

	public interface IHasQuantity
	{
		int Quantity { get; set; }
	}

	public interface IWaterSource : IHasQuantity
	{ }

	public class BeverageBottle : BaseBeverage
	{
		public override bool Fillable { get { return false; } }

		public override int MaxQuantity { get { return 5; } }

		public override string DefaultName { get { return IsEmpty ? "a bottle" : String.Format("a bottle of {0}", GetContentName()); } }

		public override int ComputeItemID()
		{
			if (!IsEmpty)
			{
				switch (Content)
				{
					case BeverageType.Ale:
						return 0x99F;
					case BeverageType.Cider:
						return 0x99F;
					case BeverageType.Wine:
						return 0x9C7;
					case BeverageType.Mead:
						return 0x99F;
					case BeverageType.Liquor:
						return 0x99B;
					case BeverageType.Milk:
						return 0x99B;
					case BeverageType.Water:
						return 0x99B;
				}
			}

			return 0;
		}

		[Constructable]
		public BeverageBottle(BeverageType type)
			: base(type)
		{
			Weight = 1.0;
		}

		public BeverageBottle(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class Jug : BaseBeverage
	{
		public override int MaxQuantity { get { return 20; } }

		public override string DefaultName { get { return IsEmpty ? "a jug" : String.Format("a jug of {0}", GetContentName()); } }

		public override int ComputeItemID()
		{
			return !IsEmpty ? 0x9C8 : 0;
		}

		[Constructable]
		public Jug(BeverageType type)
			: base(type)
		{
			Weight = 1.0;
		}

		public Jug(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class CeramicMug : BaseBeverage
	{
		public override int MaxQuantity { get { return 1; } }

		public override string DefaultName { get { return IsEmpty ? "a ceramic mug" : String.Format("a ceramic mug of {0}", GetContentName()); } }

		public override int ComputeItemID()
		{
			return ItemID >= 0x995 && ItemID <= 0x999 ? ItemID : (ItemID == 0x9CA ? ItemID : 0x995);
		}

		[Constructable]
		public CeramicMug()
		{
			Weight = 1.0;
		}

		[Constructable]
		public CeramicMug(BeverageType type)
			: base(type)
		{
			Weight = 1.0;
		}

		public CeramicMug(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class PewterMug : BaseBeverage
	{
		public override int MaxQuantity { get { return 1; } }

		public override string DefaultName { get { return IsEmpty ? "a pewter mug" : String.Format("a pewter mug of {0}", GetContentName()); } }

		public override int ComputeItemID()
		{
			return ItemID >= 0xFFF && ItemID <= 0x1002 ? ItemID : 0xFFF;
		}

		[Constructable]
		public PewterMug()
		{
			Weight = 1.0;
		}

		[Constructable]
		public PewterMug(BeverageType type)
			: base(type)
		{
			Weight = 1.0;
		}

		public PewterMug(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class Goblet : BaseBeverage
	{
		public override int MaxQuantity { get { return 2; } }

		public override string DefaultName { get { return IsEmpty ? "a goblet" : String.Format("a goblet of {0}", GetContentName()); } }

		public override int ComputeItemID()
		{
			return ItemID == 0x99A || ItemID == 0x9B3 || ItemID == 0x9BF || ItemID == 0x9CB ? ItemID : 0x99A;
		}

		[Constructable]
		public Goblet()
		{
			Weight = 1.0;
		}

		[Constructable]
		public Goblet(BeverageType type)
			: base(type)
		{
			Weight = 1.0;
		}

		public Goblet(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class GlassMug : BaseBeverage
	{
		public override int MaxQuantity { get { return 5; } }

		public override string DefaultName { get { return IsEmpty ? "a glass mug" : String.Format("a glass mug of {0}", GetContentName()); } }

		public override int ComputeItemID()
		{
			if (IsEmpty)
			{
				return (ItemID >= 0x1F81 && ItemID <= 0x1F84 ? ItemID : 0x1F81);
			}

			switch (Content)
			{
				case BeverageType.Ale:
					return ItemID == 0x9EF ? 0x9EF : 0x9EE;
				case BeverageType.Cider:
					return ItemID >= 0x1F7D && ItemID <= 0x1F80 ? ItemID : 0x1F7D;
				case BeverageType.Wine:
					return ItemID >= 0x1F8D && ItemID <= 0x1F90 ? ItemID : 0x1F8D;
				case BeverageType.Mead:
					return ItemID == 0x9EF ? 0x9EF : 0x9EE;
				case BeverageType.Liquor:
					return ItemID >= 0x1F85 && ItemID <= 0x1F88 ? ItemID : 0x1F85;
				case BeverageType.Milk:
					return ItemID >= 0x1F89 && ItemID <= 0x1F8C ? ItemID : 0x1F89;
				case BeverageType.Water:
					return ItemID >= 0x1F91 && ItemID <= 0x1F94 ? ItemID : 0x1F91;
			}

			return 0;
		}

		[Constructable]
		public GlassMug()
		{
			Weight = 1.0;
		}

		[Constructable]
		public GlassMug(BeverageType type)
			: base(type)
		{
			Weight = 1.0;
		}

		public GlassMug(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class Pitcher : BaseBeverage
	{
		public override int MaxQuantity { get { return 8; } }

		public override string DefaultName { get { return IsEmpty ? "a pitcher" : String.Format("a pitcher of {0}", GetContentName()); } }

		public override int ComputeItemID()
		{
			if (IsEmpty)
			{
				return ItemID == 0x9A7 || ItemID == 0xFF7 ? ItemID : 0xFF6;
			}

			switch (Content)
			{
				case BeverageType.Ale:
					return ItemID == 0x1F96 ? ItemID : 0x1F95;
				case BeverageType.Cider:
					return ItemID == 0x1F98 ? ItemID : 0x1F97;
				case BeverageType.Wine:
					return ItemID == 0x1F9C ? ItemID : 0x1F9B;
				case BeverageType.Mead:
					return ItemID == 0x1F96 ? ItemID : 0x1F95;
				case BeverageType.Liquor:
					return ItemID == 0x1F9A ? ItemID : 0x1F99;
				case BeverageType.Milk:
					return ItemID == 0x9AD ? ItemID : 0x9F0;
				case BeverageType.Water:
					return ItemID == 0xFF8 || ItemID == 0xFF9 || ItemID == 0x1F9E ? ItemID : 0x1F9D;
			}

			return 0;
		}

		[Constructable]
		public Pitcher()
		{
			Weight = 2.0;
		}

		[Constructable]
		public Pitcher(BeverageType type)
			: base(type)
		{
			Weight = 2.0;
		}

		public Pitcher(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public abstract class BaseBeverage : Item, IHasQuantity
	{
		public virtual bool ShowQuantity { get { return MaxQuantity > 1; } }
		public virtual bool Fillable { get { return true; } }
		public virtual bool Pourable { get { return true; } }
		public virtual bool CanChangeContent { get { return true; } }

		public abstract int MaxQuantity { get; }

		private BeverageType _Content;
		private int _Quantity;

		[CommandProperty(AccessLevel.GameMaster)]
		public double Volume { get { return ComputeVolume(_Quantity); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ContainsAlchohol { get { return (!IsEmpty && _Content != BeverageType.Milk && _Content != BeverageType.Water); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsEmpty { get { return _Quantity <= 0; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsFull { get { return _Quantity >= MaxQuantity; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Poison Poison { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Poisoner { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public BeverageType Content
		{
			get { return _Content; }
			set
			{
				if (_Content != value && CanChangeContent)
				{
					_Content = value;

					if (!IsEmpty)
					{
						ContentName = null;
					}
				}

				InvalidateProperties();

				int itemID = ComputeItemID();

				if (itemID > 0)
				{
					ItemID = itemID;
				}
				else
				{
					Delete();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string ContentName { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Quantity
		{
			get { return _Quantity; }
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				else if (value > MaxQuantity)
				{
					value = MaxQuantity;
				}

				_Quantity = value;

				InvalidateProperties();

				int itemID = ComputeItemID();

				if (itemID > 0)
				{
					ItemID = itemID;
				}
				else
				{
					Delete();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Kick { get; set; }

		public override string DefaultName
		{
			get
			{
				return IsEmpty
						   ? String.Format("a {0}", GetType().Name.SpaceWords().ToLower())
						   : String.Format("a {0} of {1}", GetType().Name.SpaceWords().ToLower(), GetContentName());
			}
		}

		public virtual string GetContentName()
		{
			return !String.IsNullOrWhiteSpace(ContentName) ? ContentName : Content.ToString();
		}

		public abstract int ComputeItemID();

		public virtual int GetQuantityDescription()
		{
			int perc = (_Quantity * 100) / MaxQuantity;

			if (perc <= 0)
			{
				return 1042975; // It's empty.
			}

			if (perc <= 33)
			{
				return 1042974; // It's nearly empty.
			}

			if (perc <= 66)
			{
				return 1042973; // It's half full.
			}

			return 1042972; // It's full.
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (ShowQuantity)
			{
				list.Add(GetQuantityDescription());
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (ShowQuantity)
			{
				LabelTo(from, GetQuantityDescription());
			}
		}

		public virtual bool ValidateUse(Mobile from, bool message)
		{
			if (Deleted)
			{
				return false;
			}

			if (!Movable && !Fillable)
			{
				BaseHouse house = BaseHouse.FindHouseAt(this);

				if (house == null || !house.IsLockedDown(this))
				{
					if (message)
					{
						from.SendLocalizedMessage(502946, "", 0x59); // That belongs to someone else.
					}

					return false;
				}
			}

			if (from.Map != Map || !from.InRange(GetWorldLocation(), 2) || !from.InLOS(this))
			{
				if (message)
				{
					from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				}

				return false;
			}

			return true;
		}

		public virtual void Fill_OnTarget(Mobile from, object targ)
		{
			if (!IsEmpty || !Fillable || !ValidateUse(from, false))
			{
				return;
			}

			if (targ is BaseBeverage)
			{
				var bev = (BaseBeverage)targ;

				if (bev.IsEmpty || !bev.ValidateUse(from, true))
				{
					return;
				}

				if ((Content != bev.Content || ContentName != bev.ContentName) && !CanChangeContent)
				{
					from.SendMessage("You can't use that to fill this beverage.");
					return;
				}

				Poison = bev.Poison;
				Poisoner = bev.Poisoner;
				Kick = bev.Kick;

				if (bev.Quantity > MaxQuantity)
				{
					Quantity = MaxQuantity;
					bev.Quantity -= MaxQuantity;
				}
				else
				{
					Quantity += bev.Quantity;
					bev.Quantity = 0;
				}

				Content = bev.Content;
				ContentName = bev.ContentName;
			}
			else if (targ is BaseWaterContainer)
			{
				var bwc = targ as BaseWaterContainer;

				if ((Content != BeverageType.Water || !String.IsNullOrWhiteSpace(ContentName)) && !CanChangeContent)
				{
					from.SendMessage("You can't use that to fill this beverage.");
					return;
				}

				if (Quantity == 0 || (Content == BeverageType.Water && !IsFull))
				{
					int iNeed = Math.Min((MaxQuantity - Quantity), bwc.Quantity);

					if (iNeed > 0 && !bwc.IsEmpty && !IsFull)
					{
						bwc.Quantity -= iNeed;
						Quantity += iNeed;
						Kick = 0;

						Content = BeverageType.Water;
						ContentName = null;

						from.PlaySound(0x4E);
					}
				}
			}
			else if (targ is Item)
			{
				var item = (Item)targ;
				var src = (item as IWaterSource);

				if (src == null && item is AddonComponent)
				{
					src = (((AddonComponent)item).Addon as IWaterSource);
				}

				if (src == null || src.Quantity <= 0)
				{
					return;
				}

				if (from.Map != item.Map || !from.InRange(item.GetWorldLocation(), 2) || !from.InLOS(item))
				{
					from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
					return;
				}

				if ((Content != BeverageType.Water || !String.IsNullOrWhiteSpace(ContentName)) && !CanChangeContent)
				{
					from.SendMessage("You can't use that to fill this beverage.");
					return;
				}

				Poison = null;
				Poisoner = null;
				Kick = 0;

				if (src.Quantity > MaxQuantity)
				{
					Quantity = MaxQuantity;
					src.Quantity -= MaxQuantity;
				}
				else
				{
					Quantity += src.Quantity;
					src.Quantity = 0;
				}

				Content = BeverageType.Water;
				ContentName = null;

				from.SendLocalizedMessage(1010089); // You fill the container with water.
			}
			else if (targ is Cow)
			{
				if ((Content != BeverageType.Milk || !String.IsNullOrWhiteSpace(ContentName)) && !CanChangeContent)
				{
					from.SendMessage("You can't use that to fill this beverage.");
					return;
				}

				var cow = (Cow)targ;

				if (cow.TryMilk(from))
				{
					Quantity = MaxQuantity;
					Kick = 0;
					Content = BeverageType.Milk;
					ContentName = null;

					from.SendLocalizedMessage(1080197); // You fill the container with milk.
				}
			}
			else if (targ is StaticTarget)
			{
				var src = (StaticTarget)targ;

				if (src.ItemID >= 2881 && src.ItemID <= 2884 || src.ItemID == 3707 || src.ItemID == 5453 ||
					src.ItemID >= 13549 && src.ItemID <= 13608)
				{
					if (!from.InRange(src.Location, 2) || !from.InLOS(src))
					{
						from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
						return;
					}

					if ((Content != BeverageType.Water || !String.IsNullOrWhiteSpace(ContentName)) && !CanChangeContent)
					{
						from.SendMessage("You can't use that to fill this beverage.");
						return;
					}

					Poison = null;
					Poisoner = null;
					Kick = 0;
					Quantity = MaxQuantity;
					ContentName = null;
					Content = BeverageType.Water;

					from.SendLocalizedMessage(1010089); // You fill the container with water.
				}
			}
			else if (targ is LandTarget)
			{
				int tileID = ((LandTarget)targ).TileID;

				var player = from as PlayerMobile;

				if (player != null)
				{
					QuestSystem qs = player.Quest;

					if (qs is WitchApprenticeQuest)
					{
						var obj = qs.FindObjective(typeof(FindIngredientObjective)) as FindIngredientObjective;

						if (obj != null && !obj.Completed && obj.Ingredient == Ingredient.SwampWater)
						{
							bool contains = false;

							for (int i = 0; !contains && i < _SwampTiles.Length; i += 2)
							{
								contains = (tileID >= _SwampTiles[i] && tileID <= _SwampTiles[i + 1]);
							}

							if (contains)
							{
								Delete();

								// You dip the container into the disgusting swamp water, collecting enough for the Hag's vile stew.
								player.SendLocalizedMessage(1055035);
								obj.Complete();
							}
						}
					}
				}
			}
		}

		private static readonly int[] _SwampTiles = new[]
		{0x9C4, 0x9EB, 0x3D65, 0x3D65, 0x3DC0, 0x3DD9, 0x3DDB, 0x3DDC, 0x3DDE, 0x3EF0, 0x3FF6, 0x3FF6, 0x3FFC, 0x3FFE};

		/*
		private static PollTimer _DrinkersTimer;
		private static bool _DrinkersPolling;

		private static readonly Dictionary<Mobile, Tuple<Type, DateTime, Func<Mobile, Type, DateTime, bool>>> _Drinkers =
			new Dictionary<Mobile, Tuple<Type, DateTime, Func<Mobile, Type, DateTime, bool>>>();

		public static Dictionary<Mobile, Tuple<Type, DateTime, Func<Mobile, Type, DateTime, bool>>> Drinkers { get { return _Drinkers; } }

		public static void RegisterDrinker(Mobile m, Type drink, Func<Mobile, Type, DateTime, bool> callback)
		{
			if (!Drinkers.ContainsKey(m))
			{
				Drinkers.Add(m, null);
			}

			Drinkers[m] = Tuple.Create(drink, DateTime.UtcNow, callback);
		}

		static BaseBeverage()
		{
			_DrinkersTimer = PollTimer.CreateInstance(
				TimeSpan.FromSeconds(0.1),
				() =>
				{
					_DrinkersPolling = true;

					_Drinkers.ForEach(
						(drinker, info) =>
						{
							if (drinker.Deleted || !drinker.Alive || info == null || info.Item1 == null || info.Item3 == null ||
								!info.Item3(drinker, info.Item1, info.Item2))
							{
								_Drinkers.Remove(drinker);
							}
						});

					_DrinkersPolling = false;
				},
				() => !_DrinkersPolling && _Drinkers.Count > 0);
		}
		*/

		#region Effects of achohol
		private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

		public static void Initialize()
		{
			EventSink.Login += e => CheckHeaveTimer(e.Mobile);
		}

		public static void CheckHeaveTimer(Mobile from)
		{
			if (from.BAC > 0 && from.Map != Map.Internal && !from.Deleted)
			{
				Timer t;
				m_Table.TryGetValue(from, out t);

				if (t == null)
				{
					if (from.BAC > 100)
					{
						from.BAC = 100;
					}

					t = new HeaveTimer(from);
					t.Start();

					m_Table[from] = t;
				}
			}
			else
			{
				Timer t;
				m_Table.TryGetValue(from, out t);

				if (t != null)
				{
					t.Stop();
					m_Table.Remove(from);

					from.SendLocalizedMessage(500850); // You feel sober.
				}
			}
		}

		private class HeaveTimer : Timer
		{
			private readonly Mobile m_Drunk;

			public HeaveTimer(Mobile drunk)
				: base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
			{
				m_Drunk = drunk;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if (m_Drunk.Deleted || m_Drunk.Map == Map.Internal)
				{
					Stop();
					m_Table.Remove(m_Drunk);
				}
				else if (m_Drunk.Alive)
				{
					if (m_Drunk.BAC > 100)
					{
						m_Drunk.BAC = 100;
					}

					// chance to get sober
					if (10 > Utility.Random(100))
					{
						--m_Drunk.BAC;
					}

					// lose some stats
					m_Drunk.Stam -= 1;
					m_Drunk.Mana -= 1;

					if (Utility.Random(1, 4) == 1)
					{
						if (!m_Drunk.Mounted)
						{
							// turn in a random direction
							m_Drunk.Direction = (Direction)Utility.Random(8);

							// heave
							m_Drunk.Animate(32, 5, 1, true, false, 0);
						}

						// *hic*
						m_Drunk.PublicOverheadMessage(MessageType.Regular, 0x3B2, 500849);
					}

					if (m_Drunk.BAC <= 0)
					{
						Stop();
						m_Table.Remove(m_Drunk);

						m_Drunk.SendLocalizedMessage(500850); // You feel sober.
					}
				}
			}
		}
		#endregion

		public virtual void Pour_OnTarget(Mobile from, object targ)
		{
			if (IsEmpty || !Pourable || !ValidateUse(from, false))
			{
				return;
			}

			if (from == targ)
			{
				if (OnDrink(from))
				{
					Quantity = 0;
				}

				return;
			}

			if (targ is BaseBeverage)
			{
				var bev = (BaseBeverage)targ;

				if (!bev.ValidateUse(from, true))
				{
					return;
				}

				if (bev.IsFull)
				{
					from.SendLocalizedMessage(500848); // Couldn't pour it there.  It was already full.
					return;
				}

				if (!bev.IsEmpty || ((bev.Content != Content || bev.ContentName != ContentName) && !bev.CanChangeContent))
				{
					from.SendLocalizedMessage(500846); // Can't pour it there.
					return;
				}

				bev.Poison = Poison;
				bev.Poisoner = Poisoner;
				bev.Kick = Kick;

				if (Quantity > bev.MaxQuantity)
				{
					bev.Quantity = bev.MaxQuantity;
					Quantity -= bev.MaxQuantity;
				}
				else
				{
					bev.Quantity += Quantity;
					Quantity = 0;
				}

				bev.Content = Content;
				bev.ContentName = ContentName;

				from.PlaySound(0x4E);
				return;
			}

			if (targ is BaseWaterContainer)
			{
				var bwc = targ as BaseWaterContainer;

				if (Content != BeverageType.Water)
				{
					from.SendLocalizedMessage(500842); // Can't pour that in there.
					return;
				}

				if (bwc.Items.Count != 0)
				{
					from.SendLocalizedMessage(500841); // That has something in it.
					return;
				}

				int itNeeds = Math.Min((bwc.MaxQuantity - bwc.Quantity), Quantity);

				if (itNeeds > 0)
				{
					bwc.Quantity += itNeeds;
					Quantity -= itNeeds;

					from.PlaySound(0x4E);
				}

				return;
			}

			if (targ is PlantItem)
			{
				((PlantItem)targ).Pour(from, this);
				return;
			}

			if (targ is AddonComponent &&
				(((AddonComponent)targ).Addon is WaterVatEast || ((AddonComponent)targ).Addon is WaterVatSouth) &&
				Content == BeverageType.Water)
			{
				var player = from as PlayerMobile;

				if (player == null)
				{
					return;
				}

				var qs = player.Quest as SolenMatriarchQuest;

				if (qs == null)
				{
					return;
				}

				QuestObjective obj = qs.FindObjective(typeof(GatherWaterObjective));

				if (obj != null && !obj.Completed)
				{
					BaseAddon vat = ((AddonComponent)targ).Addon;

					if (vat.X > 5784 && vat.X < 5814 && vat.Y > 1903 && vat.Y < 1934 &&
						((qs.RedSolen && vat.Map == Map.Trammel) || (!qs.RedSolen && vat.Map == Map.Felucca)))
					{
						if (obj.CurProgress + Quantity > obj.MaxProgress)
						{
							int delta = obj.MaxProgress - obj.CurProgress;

							Quantity -= delta;
							obj.CurProgress = obj.MaxProgress;
						}
						else
						{
							obj.CurProgress += Quantity;
							Quantity = 0;
						}
					}
				}

				return;
			}

			from.SendLocalizedMessage(500846); // Can't pour it there.
		}

		public virtual bool OnDrink(Mobile from)
		{
			if (from == null || from.Deleted)
			{
				return false;
			}

			int oldThirst = from.Thirst;

			from.PlaySound(Utility.RandomList(0x30, 0x2D6));

			if (from.Stam < from.StamMax)
			{
				from.Stam += Utility.Random(2, 4); //restore some stamina
			}

			if (ContainsAlchohol)
			{
				from.Thirst = Math.Max(0, Math.Min(20, from.Thirst + _Quantity));

				from.SendMessage("You feel more thirsty from drinking the alcoholic beverage.");

				CheckBAC(from, _Quantity);
				CheckHeaveTimer(from);
			}
			else
			{
				from.Thirst = Math.Max(0, Math.Min(20, from.Thirst - _Quantity));

				if (from.Thirst > 15)
				{
					from.SendMessage("You drink the beverage, but are still extremely thirsty.");
				}
				else if (from.Thirst > 10)
				{
					from.SendMessage("You drink the beverage and begin to feel more satiated.");
				}
				else if (from.Thirst > 5)
				{
					from.SendMessage("After drinking the beverage, you feel much less thirsty.");
				}
				else if (from.Thirst > 0)
				{
					from.SendMessage("You feel quite full after drinking the beverage.");
				}
				else
				{
					from.SendMessage("You manage to drink the beverage, but are stuffed!");
				}
			}

			//EventSink.InvokeOnConsume(new OnConsumeEventArgs(from, this, from.Thirst - oldThirst));

            if (from is PlayerMobile)
            {
                Conquests.CheckProgress<ItemConquest>((PlayerMobile)from, this);

                //CheckProgress<ConsumeItemConquest>((PlayerMobile)e.Consumer, e);
            }

			ApplyPoison(from);
			return true;
		}

		public virtual void CheckBAC(Mobile from, int qty)
		{
			if (!ContainsAlchohol)
			{
				return;
			}

			int bac = 0;
			int kick = 0;
			ComputeBAC(ref bac, ref kick, qty);

			if (kick > 0)
			{
				bac += kick;

				from.SendMessage("The alcohol has a kick, making it more potent.");

				Timer.DelayCall(TimeSpan.FromSeconds(1.5), from.PlaySound, 1055);
			}

			bac = Math.Max(0, Math.Min(100, bac));

			from.BAC = Math.Max(from.BAC, Math.Min(100, from.BAC + bac));

			if (from.BAC <= 60)
			{
				return;
			}

			from.Emote("{0} looks completely smashed from the {1}", from.Name, this.ResolveName(from));

			if (ApplyPoison(from, true) == ApplyPoisonResult.Poisoned)
			{
				from.SendMessage("Your blood alcohol content is too high and you poison yourself.");
			}
		}

		public virtual void ComputeBAC(ref int bac, ref int kick, int qty)
		{
			switch (Content)
			{
				case BeverageType.Ale:
					bac += 1;
					break;
				case BeverageType.Cider:
					bac += 2;
					break;
				case BeverageType.Wine:
					bac += 3;
					break;
				case BeverageType.Mead:
					bac += 4;
					break;
				case BeverageType.Liquor:
					bac += 5;
					break;
			}

			bac *= qty;

			kick += Kick * qty;
		}

		public virtual int ComputeBAC(int qty)
		{
			int bac = 0;
			int kick = 0;

			ComputeBAC(ref bac, ref kick, qty);

			return Math.Max(0, Math.Min(100, bac + kick));
		}

		public virtual double ComputeVolume(int qty)
		{
			return !ContainsAlchohol ? 0 : ComputeBAC(qty) / 100.0;
		}

		public virtual ApplyPoisonResult ApplyPoison(Mobile from, bool bac = false)
		{
			if (from == null)
			{
				return ApplyPoisonResult.Immune;
			}

			if (!bac)
			{
				return Poison == null ? ApplyPoisonResult.Immune : from.ApplyPoison(Poisoner, Poison);
			}

			Poison p = Poison.Deadly;

			if (from.BAC > 90)
			{
				p = Poison.Fatal;
			}
			else if (from.BAC > 80)
			{
				p = Poison.Lethal;
			}
			else if (from.BAC > 70)
			{
				p = Poison.Deadly;
			}

			return from.ApplyPoison(from, p);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!this.CheckDoubleClick(from, true, false, 2) || !ValidateUse(from, true))
			{
				return;
			}

			if (IsEmpty)
			{
				if (!Fillable)
				{
					return;
				}

				from.BeginTarget(-1, true, TargetFlags.None, Fill_OnTarget);
				SendLocalizedMessageTo(from, 500837); // Fill from what?
				return;
			}

			if (!Pourable)
			{
				return;
			}

			from.BeginTarget(-1, true, TargetFlags.None, Pour_OnTarget);
			from.SendLocalizedMessage(1010086); // What do you want to use this on?
		}

		public static bool ConsumeTotal(Container pack, BeverageType content, int quantity)
		{
			return ConsumeTotal(pack, typeof(BaseBeverage), content, quantity);
		}

		public static bool ConsumeTotal(Container pack, Type itemType, BeverageType content, int quantity)
		{
			Item[] items = pack.FindItemsByType(itemType);

			// First pass, compute total
			int total = items.OfType<BaseBeverage>()
							 .Where(bev => bev.Content == content && !bev.IsEmpty)
							 .Sum(bev => bev.Quantity);

			if (total >= quantity)
			{
				// We've enough, so consume it
				int need = quantity;

				foreach (BaseBeverage bev in items.OfType<BaseBeverage>())
				{
					if (bev.Content != content || bev.IsEmpty)
					{
						continue;
					}

					int theirQuantity = bev.Quantity;

					if (theirQuantity < need)
					{
						bev.Quantity = 0;
						need -= theirQuantity;
					}
					else
					{
						bev.Quantity -= need;
						return true;
					}
				}
			}

			return false;
		}

		public BaseBeverage()
		{
			ItemID = ComputeItemID();
		}

		public BaseBeverage(BeverageType type)
		{
			_Content = type;
			_Quantity = MaxQuantity;
			ItemID = ComputeItemID();
		}

		public BaseBeverage(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(4); // version

			writer.Write(ContentName);

			writer.Write(Kick);

			writer.Write(Poisoner);

			Poison.Serialize(Poison, writer);
			writer.Write((int)_Content);
			writer.Write(_Quantity);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 4:
					ContentName = reader.ReadString();
					goto case 3;
				case 3:
				case 2:
					Kick = reader.ReadInt();
					goto case 1;
				case 1:
					Poisoner = reader.ReadMobile();
					goto case 0;
				case 0:
					{
						Poison = Poison.Deserialize(reader);
						_Content = (BeverageType)reader.ReadInt();
						_Quantity = reader.ReadInt();
					}
					break;
			}
		}
	}
}