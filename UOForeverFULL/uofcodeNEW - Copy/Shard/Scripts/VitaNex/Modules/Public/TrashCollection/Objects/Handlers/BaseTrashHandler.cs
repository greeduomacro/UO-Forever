#region Header
//   Vorspire    _,-'/-'/  BaseTrashHandler.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;

using VitaNex.Crypto;
using VitaNex.Items;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public abstract class BaseTrashHandler
	{
		public static Type[] DefaultAcceptList = new[] {typeof(Item)};

		public static Type[] DefaultIgnoredList = new[]
		{typeof(Gold), typeof(Bandage), typeof(Bottle), typeof(BaseReagent), typeof(IVendorToken), typeof(BaseTrashContainer)};

		private int _BonusTokens;
		private int _BonusTokensChance;

		private bool _Enabled = true;

		private TrashPriority _Priority;

		public BaseTrashHandler()
			: this(true)
		{ }

		public BaseTrashHandler(
			bool enabled,
			TrashPriority priority = TrashPriority.Normal,
			IEnumerable<Type> accepts = null,
			IEnumerable<Type> ignores = null)
		{
			UID = CryptoGenerator.GenString(CryptoHashType.MD5, GetType().FullName);
			Enabled = enabled;
			Priority = priority;
			Accepted = new List<Type>(accepts ?? DefaultAcceptList);
			Ignored = new List<Type>(ignores ?? DefaultIgnoredList);
		}

		public BaseTrashHandler(GenericReader reader)
		{
			Deserialize(reader);
		}

		[CommandProperty(TrashCollection.Access)]
		public int BonusTokensChance { get { return _BonusTokensChance; } set { _BonusTokensChance = Math.Max(0, Math.Min(100, value)); } }

		public string UID { get; private set; }

		[CommandProperty(TrashCollection.Access)]
		public bool Enabled
		{
			get
			{
				if (!TrashCollection.CMOptions.ModuleEnabled)
				{
					return false;
				}

				return _Enabled;
			}
			set { _Enabled = value; }
		}

		[CommandProperty(TrashCollection.Access)]
		public TrashPriority Priority
		{
			get { return _Priority; }
			set
			{
				if (_Priority != value)
				{
					_Priority = value;
					TrashCollection.InvalidateHandlers();
				}
			}
		}

		[CommandProperty(TrashCollection.Access)]
		public int BonusTokens { get { return _BonusTokens; } set { _BonusTokens = Math.Max(0, value); } }

		public List<Type> Accepted { get; protected set; }
		public List<Type> Ignored { get; protected set; }

		public bool Trash(Mobile from, Item trashed, bool message = true)
		{
			int tokens = 0;
			return Trash(from, trashed, ref tokens, message);
		}

		public bool Trash(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);

			if (!Enabled || from == null || trashed == null || trashed.Deleted)
			{
				return false;
			}

			if (!CanTrash(from, trashed))
			{
				OnTrashRejected(from, trashed, message);
				return false;
			}

			bool multiple = false;

			if (trashed is Container)
			{
				bool trashThis = true;
				Container c = (Container)trashed;
				var items = c.FindItemsByType<Item>(false);

				if (items.Count > 0)
				{
					multiple = true;
				}

				items.ForEach(
					ci =>
					{
					    if (ci is Container)
					    {
                            from.SendMessage(0x55, "You cannot trash containers within containers.  Please trash them individually.");
                            trashThis = false;
					    }
						if (!Trash(from, ci, false))
						{
							trashThis = false;
						}
					});

				if (!trashThis)
				{
					OnTrashRejected(from, trashed, message);
					return false;
				}
			}

			GetTrashTokens(from, trashed, ref tokens);
			OnTrashed(from, trashed, ref tokens, message);

			ItemTrashedEventArgs e = new ItemTrashedEventArgs(this, from, trashed, tokens, message);
			VitaNexCore.TryCatch(e.Invoke, TrashCollection.CMOptions.ToConsole);

			if (tokens != e.Tokens)
			{
				tokens = Math.Max(0, e.Tokens);
			}

			message = e.Message;

			if (!e.HandledTokens)
			{
				TrashCollection.EnsureProfile(from).TransferTokens(from, trashed, tokens, message);
			}

			if (from.Backpack != null && TrashCollection.CMOptions.UseTrashedProps)
			{
				from.Backpack.FindItemsByType(true, (Item i) => (i is ITrashTokenProperties)).ForEach(i => i.InvalidateProperties());
			}

			if (multiple && message)
			{
				from.SendMessage(0x55, "You trashed multiple items, check your profile history for more information.");
			}

			trashed.Delete();
			return true;
		}

		public virtual bool IsAccepted(Mobile from, Type trash)
		{
			return Accepted.Any(trash.IsEqualOrChildOf);
		}

		public virtual bool IsIgnored(Mobile from, Type trash)
		{
			return Ignored.Any(trash.IsEqualOrChildOf);
		}

		public virtual bool CanTrash(Mobile from, Item trash, bool message = true)
		{
			if (!Enabled || trash == null || trash.Deleted || !trash.Movable || !trash.IsAccessibleTo(from))
			{
				return false;
			}

			Type iType = trash.GetType();

			if (!IsAccepted(from, iType) || IsIgnored(from, iType))
			{
				return false;
			}

			if (trash is Container)
			{
				Container c = (Container)trash;
				bool trashThis = c.FindItemsByType<Item>().All(ci => CanTrash(from, ci));

				if (!trashThis)
				{
					return false;
				}
			}

			return true;
		}

		public virtual void Serialize(GenericWriter writer)
		{
			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.Write(Enabled);
						writer.Write(UID);
						writer.WriteFlag(_Priority);
						writer.Write(_BonusTokens);
						writer.Write(_BonusTokensChance);
						writer.WriteList(Accepted, t => writer.WriteType(t));
						writer.WriteList(Ignored, t => writer.WriteType(t));
					}
					break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
						Enabled = reader.ReadBool();
						UID = reader.ReadString();
						_Priority = reader.ReadFlag<TrashPriority>();
						_BonusTokens = reader.ReadInt();
						_BonusTokensChance = reader.ReadInt();
						Accepted = reader.ReadList(reader.ReadType);
						Ignored = reader.ReadList(reader.ReadType);
					}
					break;
			}
		}

		protected virtual void OnTrashed(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);
		}

		protected virtual void OnTrashRejected(Mobile from, Item trashed, bool message = true)
		{ }

		protected virtual void GetTrashTokens(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);

			if (!Enabled || from == null || trashed == null || trashed.Deleted)
			{
				return;
			}

			int amount = trashed.GetAttributeCount();

			if (trashed.Stackable)
			{
				amount *= trashed.Amount;
			}

			if (TrashCollection.CMOptions.GiveBonusTokens && BonusTokens > 0 && Utility.RandomMinMax(0, 100) <= BonusTokensChance)
			{
				amount += BonusTokens;
			}

			if (amount > 0)
			{
				tokens += amount;
			}
		}
	}
}