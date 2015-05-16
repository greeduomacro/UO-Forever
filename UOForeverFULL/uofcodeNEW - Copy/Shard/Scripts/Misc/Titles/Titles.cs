#region References
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;
#endregion

namespace Server.Scripts.Engines.ValorSystem
{
	[TypeAlias("Server.Scripts.Misc.Titles.Titles")]
	public class ValorTitleItem : Item
	{
		public static void Initialize()
		{
			CommandSystem.Register("FillValorTitles", AccessLevel.GameMaster, OnFillValorTitlesCommand);
			CommandSystem.Register("ClearValorTitles", AccessLevel.GameMaster, OnClearValorTitlesCommand);
		}

		[Usage("FillValorTitles")]
		[Description("Gives a PlayerMobile every available valor title.")]
		private static void OnFillValorTitlesCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;
			m.SendMessage("Target a player to give them every available valor title.");

			m.BeginTarget(
				-1,
				false,
				TargetFlags.None,
				(from, targeted) =>
				{
					if (targeted is PlayerMobile && ValorRewardController.Instance != null)
					{
						ValorRewardController.MTitle.ForEach(x => ((PlayerMobile)targeted).AddValorTitle(x.Title));

						m.SendMessage("You have given them all of the available valor titles");
					}
					else
					{
						m.SendMessage("That is not a player!");
					}
				});
		}

		[Usage("ClearValorTitles")]
		[Description("Makes a player forget all of their valor titles.")]
		private static void OnClearValorTitlesCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;
			m.SendMessage("Target a player to have them forget all of the valor titles they've learned.");

			m.BeginTarget(
				-1,
				false,
				TargetFlags.None,
				(from, targeted) =>
				{
					if (targeted is PlayerMobile)
					{
						//((PlayerMobile)targeted).ResetTitles();

						m.SendMessage("They forget all their valor titles.");
					}
					else
					{
						m.SendMessage("That is not a player!");
					}
				});
		}

		public ValorTitleItem(Serial serial)
			: base(serial)
		{ }

		private string _mTitle;
		public string Title { get { return _mTitle; } }

		private string _fTitle;
		public string fTitle { get { return _fTitle; } }

		private string _mDescription;
		public string Description { get { return _mDescription; } }

		private int _mValorCost;

		public int ValorCost { get { return _mValorCost; } }

		public ValorTitleItem(string title, string ftitle, int valorcost, string desc)
		{
			_mTitle = title;
			_fTitle = ftitle;
			_mValorCost = valorcost;
			_mDescription = desc;
		}

		public ValorTitleItem(string title, int valorcost)
		{
			_mTitle = title;
			_fTitle = null;
			_mValorCost = valorcost;
			_mDescription = null;
		}

		public ValorTitleItem(string title)
		{
			_mTitle = title;
			_fTitle = null;
			_mValorCost = 0;
			_mDescription = null;
		}

		public ValorTitleItem(string title, string ftitle)
		{
			_mTitle = title;
			_fTitle = ftitle;
			_mValorCost = 0;
			_mDescription = null;
		}

		private TextDefinition _mTd;

		public TextDefinition TextDefinition
		{
			get
			{
				string rtitle;
				if (_fTitle != null && ValorRewardController.Instance.GetTitles.Exists(x => x.fTitle == _fTitle))
				{
					rtitle = _mTitle + " | " + _fTitle;
				}
				else
				{
					rtitle = _mTitle;
				}
				return _mTd ?? (_mTd = new TextDefinition(rtitle));
			}
		}

		public void Edit(string title, string ftitle, int cost, string desc)
		{
			_mTitle = title;
			_fTitle = ftitle;
			_mValorCost = cost;
			_mDescription = desc;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1);
			writer.Write(_fTitle);
			writer.Write(_mTitle);
			writer.Write(_mValorCost);
			writer.Write(_mDescription);
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
						_mValorCost = reader.ReadInt();
						_mDescription = reader.ReadString();
						break;
					}
			}
		}
	}
}