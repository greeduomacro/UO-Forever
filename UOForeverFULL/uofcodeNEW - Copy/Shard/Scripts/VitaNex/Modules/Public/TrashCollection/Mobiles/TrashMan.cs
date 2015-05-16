#region Header
//   Vorspire    _,-'/-'/  TrashMan.cs
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
using Server.Mobiles;

using VitaNex.Mobiles;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public class TrashMan : BaseTokenVendor, ITrashTokenProperties
	{
		private readonly List<SBInfo> _SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return _SBInfos; } }

		public override VendorShoeType ShoeType { get { return Utility.RandomBool() ? VendorShoeType.ThighBoots : VendorShoeType.Boots; } }

		[Constructable]
		public TrashMan()
			: base("the trash collector", "Trash Tokens", typeof(TrashToken))
		{ }

		public TrashMan(Serial serial)
			: base(serial)
		{ }

		public override void InitSBInfo()
		{
			_SBInfos.Add(new SBTrashMan());
		}

		public bool Trash(Mobile from, Item trashed, bool message = true)
		{
			int tokens = 0;
			return Trash(from, trashed, ref tokens, message);
		}

		public bool Trash(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);

			if (from == null || trashed == null || trashed.Deleted)
			{
				return false;
			}

			if (!from.InRange(Location, 5))
			{
				if (message)
				{
					from.SendMessage("You must be within 5 paces to use that trash collector.");
				}

				OnTrashRejected(from, trashed, false);
				return false;
			}

			if (!CanTrash(from, trashed, message))
			{
				OnTrashRejected(from, trashed, message);
				return false;
			}

			foreach (var h in TrashCollection.Handlers.Values)
			{
				if (h != null && h.Trash(from, trashed, ref tokens, message))
				{
					OnTrashed(from, trashed, ref tokens, message);
					return true;
				}
			}

			OnTrashRejected(from, trashed, message);
			return false;
		}

		protected virtual void OnTrashed(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);

			if (from == null || trashed == null || trashed.Deleted)
			{
				return;
			}

			if (message && Utility.RandomDouble() < 0.20)
			{
				Say(TrashCan.SuccessEmotes.GetRandom());
			}
		}

		protected virtual void OnTrashRejected(Mobile from, Item trashed, bool message = true)
		{
			if (from == null || trashed == null || trashed.Deleted)
			{
				return;
			}

			if (message && !TrashCollection.CMOptions.ModuleEnabled)
			{
				Say("Sorry, I'm on my lunch break.");
			}
			else if (message && Utility.RandomDouble() < 0.20)
			{
				Say(TrashCan.FailEmotes.GetRandom());
			}
		}

		public virtual bool CanTrash(Mobile from, Item trash, bool message = true)
		{
			if (trash == null || trash.Deleted || !trash.Movable || !trash.IsAccessibleTo(from) ||
				!TrashCollection.CMOptions.ModuleEnabled)
			{
				return false;
			}

			return TrashCollection.Handlers.Values.Any(h => h != null && h.CanTrash(@from, trash, message));
		}

		public override bool OnDragDrop(Mobile from, Item trashed)
		{
			if (from == null || trashed == null || trashed.Deleted)
			{
				return false;
			}

			if (Trash(from, trashed))
			{
				return true;
			}

			return base.OnDragDrop(from, trashed);
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
	}
}