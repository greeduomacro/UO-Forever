#region References
using System;
using System.Linq;

using Server.Network;
using Server.Targeting;

using VitaNex.Targets;
#endregion

namespace Server.Items
{
	public interface IMill
	{
		int MaxCapacity { get; set; }
		int CurCapacity { get; set; }

		bool IsFilled { get; }
		bool IsFull { get; }
		bool IsWorking { get; }
	}

	public enum MillStage
	{
		Empty,
		Filled,
		Working
	}

	public sealed class MillTarget<TItemIn, TItemOut> : GenericSelectTarget<AddonComponent>
		where TItemIn : Item
		where TItemOut : Item
	{
		public TItemIn ToAdd { get; set; }

		public BaseMillAddon<TItemIn, TItemOut> Targeted { get; private set; }

		public MillTarget(
			TItemIn toAdd, Action<Mobile, BaseMillAddon<TItemIn, TItemOut>> success = null, Action<Mobile> fail = null)
			: base(null, fail, 4, false, TargetFlags.None)
		{
			ToAdd = toAdd;

			if (success != null)
			{
				SuccessHandler = (m, a) => success(m, Targeted);
			}
		}

		protected override void OnTarget(Mobile user, AddonComponent targeted)
		{
			if (user == null || user.Deleted || targeted == null || targeted.Deleted)
			{
				return;
			}

			Targeted = targeted.Addon as BaseMillAddon<TItemIn, TItemOut>;

			if (Targeted != null && !Targeted.Deleted && Targeted.TryAddItem(ToAdd, user, true))
			{
				base.OnTarget(user, targeted);
			}
		}
	}

	[TypeAlias("Server.Items.BaseFlourMillAddon")]
	public abstract class BaseMillAddon<TItemIn, TItemOut> : BaseAddon, IMill
		where TItemIn : Item
		where TItemOut : Item
	{
		private Timer _Timer;

		private int _MaxCapacity;
		private int _CurCapacity;

		public abstract int[][] StageTable { get; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int CurCapacity
		{
			get { return _CurCapacity; }
			set
			{
				_CurCapacity = Math.Max(0, Math.Min(MaxCapacity, value));
				UpdateStage();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxCapacity { get { return _MaxCapacity; } set { _MaxCapacity = Math.Max(1, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsFilled { get { return _CurCapacity > 0; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsFull { get { return _CurCapacity >= MaxCapacity; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsWorking { get { return _Timer != null; } }

		public BaseMillAddon(int hue)
		{
			_CurCapacity = 0;
			_MaxCapacity = 2;

			Hue = hue;
		}

		public BaseMillAddon(Serial serial)
			: base(serial)
		{ }

		private int[] FindItemTable(int itemID)
		{
			return StageTable.FirstOrDefault(itemTable => itemTable.Any(t => t == itemID));
		}

		public virtual bool TryAddItem(TItemIn item, Mobile user, bool message)
		{
			if (item == null || item.Deleted || !item.Movable || item.Amount <= 0 || !item.IsAccessibleTo(user))
			{
				return false;
			}

			if (IsFull)
			{
				if (user != null && message)
				{
					user.SendMessage("The mill is already full.");
				}

				return false;
			}

			int take = Math.Max(0, Math.Min(item.Amount, _MaxCapacity - _CurCapacity));

			item.Consume(take);
			_CurCapacity += take;

			if (user != null && message)
			{
				user.SendMessage("You add {0:#,0} {1} to the mill.", take, item.ResolveName(user));
			}

			return true;
		}

		public virtual TItemOut TryCreateProduct(Mobile user)
		{
			return typeof(TItemOut).CreateInstanceSafe<TItemOut>();
		}

		protected virtual void OnStartWork(Mobile user)
		{ }

		protected virtual void OnFinishWork(Mobile user)
		{ }

		private void StartWorking(Mobile user)
		{
			if (IsWorking)
			{
				return;
			}

			_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), FinishWorking, user);

			OnStartWork(user);

			UpdateStage();
		}

		private void FinishWorking(Mobile user)
		{
			if (_Timer != null)
			{
				_Timer.Stop();
				_Timer = null;
			}

			OnFinishWork(user);

			if (user != null && !user.Deleted && !Deleted && IsFull)
			{
				TItemOut product = TryCreateProduct(user);

				if (product != null && !product.Deleted)
				{
					if (user.PlaceInBackpack(product))
					{
						_CurCapacity = 0;
					}
					else
					{
						product.Delete();
						user.SendLocalizedMessage(500998); // There is not enough room in your backpack!  You stop grinding.
					}
				}
			}

			UpdateStage();
		}

		protected void UpdateStage()
		{
			if (IsWorking)
			{
				UpdateStage(MillStage.Working);
			}
			else if (IsFilled)
			{
				UpdateStage(MillStage.Filled);
			}
			else
			{
				UpdateStage(MillStage.Empty);
			}
		}

		protected void UpdateStage(MillStage stage)
		{
			foreach (AddonComponent component in Components)
			{
				if (component == null)
				{
					continue;
				}

				int[] itemTable = FindItemTable(component.ItemID);

				if (itemTable != null)
				{
					component.ItemID = itemTable[(int)stage];
				}
			}
		}

		public override void OnComponentUsed(AddonComponent c, Mobile user)
		{
			if (user == null || user.Deleted)
			{
				return;
			}

			if (!user.InRange(GetWorldLocation(), 4) || !user.InLOS(this))
			{
				user.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				return;
			}

			OnUse(user);
		}

		protected virtual void OnUse(Mobile user)
		{
			if (user == null || user.Deleted)
			{
				return;
			}

			if (!IsFull)
			{
				OnUseNotFull(user);
				return;
			}

			StartWorking(user);
		}

		protected virtual void OnUseNotFull(Mobile user)
		{
			if (user != null && !user.Deleted)
			{
				user.SendMessage("You need more resources to complete the milling process.");
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(2); // version

			writer.Write(_MaxCapacity);
			writer.Write(_CurCapacity);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 2:
					_MaxCapacity = reader.ReadInt();
					goto case 1;
				case 1:
					_CurCapacity = reader.ReadInt();
					break;
			}

			if (version < 2)
			{
				_MaxCapacity = 2;
			}

			UpdateStage();
		}
	}
}