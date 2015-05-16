#region References
using Server.Mobiles;
#endregion

namespace Server.Scripts.Engines.ValorSystem
{
	[TypeAlias("Server.Scripts.Misc.Titles.TitleScrollOld")]
	public class TitleScrollOld : Item
	{
		public override int LabelNumber { get { return 1074560; } } // recipe scroll

		private string _mTitle;
		private string _fTitle;

		public TitleScrollOld(ValorTitleItem r)
			: this(r.Title)
		{ }

		[Constructable]
		public TitleScrollOld(string title, string ftitle)
			: base(0x2831)
		{
			Name = "a title scroll";
			_mTitle = title;
			_fTitle = ftitle;
			Weight = 2;
			Hue = 1195;
		}

		[Constructable]
		public TitleScrollOld(string title)
			: base(0x2831)
		{
			Name = "a title scroll";
			_mTitle = title;
			_fTitle = null;
			Weight = 2;
			Hue = 1195;
		}

		public TitleScrollOld(Serial serial)
			: base(serial)
		{ }

		public ValorTitleItem ValorTitleItem
		{
			get
			{
				if (ValorRewardController.Instance != null)
				{
					ValorTitleItem valorTitleItem = ValorRewardController.Instance.GetTitles.Find(x => x.Title == _mTitle);
					return valorTitleItem;
				}
				return null;
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			ValorTitleItem t = ValorTitleItem;

			if (ValorRewardController.Instance == null)
			{
				return;
			}
			if (t != null && !ValorRewardController.Instance.GetTitles.Exists(x => x.fTitle == _fTitle) &&
				ValorRewardController.Instance.GetTitles.Exists(x => x.Title == _mTitle))
			{
				LabelTo(from, 1049644, _mTitle); // [~1_stuff~]
			}
			else if (t != null && ValorRewardController.Instance.GetTitles.Exists(x => x.fTitle == _fTitle) &&
					 ValorRewardController.Instance.GetTitles.Exists(x => x.Title == _mTitle))
			{
				LabelTo(from, 1049644, _mTitle + " | " + _fTitle); // [~1_stuff~]
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			var pm = from as PlayerMobile;
			if (ValorRewardController.Instance == null)
			{
				return;
			}
			bool mtitle = ValorRewardController.Instance.GetTitles.Exists(x => x.Title == _mTitle);
			bool ftitle = ValorRewardController.Instance.GetTitles.Exists(x => x.fTitle == _fTitle);
			if (ValorRewardController.Instance != null && pm != null && mtitle)
			{
				if (!pm.Female && !pm.HasValorTitle(_mTitle) && !pm.HasValorTitle(_fTitle) || pm.Female && !ftitle && !pm.HasValorTitle(_mTitle))
				{
					pm.AddValorTitle(_mTitle);
					pm.SendMessage(44, "You have obtained the title: " + _mTitle + ".");
					Delete();
				}
				else if (pm.Female && ftitle && !pm.HasValorTitle(_mTitle) && !pm.HasValorTitle(_fTitle))
				{
					pm.AddValorTitle(_fTitle);
					pm.SendMessage(44, "You have obtained the title: " + _fTitle + ".");
					Delete();
				}
				else if (!pm.Female && pm.HasValorTitle(_mTitle) || pm.Female && pm.HasValorTitle(_mTitle))
				{
					pm.SendMessage(44, "You already have the title: " + _mTitle + ".");
				}
				else if (pm.Female && ftitle && pm.HasValorTitle(_fTitle) || !pm.Female && ftitle && pm.HasValorTitle(_fTitle))
				{
					pm.SendMessage(44, "You already have the title: " + _fTitle + ".");
				}
			}
			else
			{
				from.SendMessage(44, "This title no longer exists.");
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write(_fTitle);
			writer.Write(_mTitle);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					{
						_fTitle = reader.ReadString();

						goto case 0;
					}
				case 0:
					{
						_mTitle = reader.ReadString();

						break;
					}
			}
		}
	}
}