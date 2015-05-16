#region Header
//   Vorspire    _,-'/-'/  GeneralTrashHandler.cs
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

using Server;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public class GeneralTrashHandler : BaseTrashHandler
	{
		public GeneralTrashHandler()
			: this(true)
		{ }

		public GeneralTrashHandler(
			bool enabled,
			TrashPriority priority = TrashPriority.Lowest,
			IEnumerable<Type> accepts = null,
			IEnumerable<Type> ignores = null)
			: base(enabled, priority, accepts, ignores)
		{ }

		public GeneralTrashHandler(GenericReader reader)
			: base(reader)
		{ }

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