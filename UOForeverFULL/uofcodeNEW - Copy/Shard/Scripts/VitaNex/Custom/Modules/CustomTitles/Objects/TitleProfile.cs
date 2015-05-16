#region References
using System.Collections.Generic;
using System.Linq;

using Server.Mobiles;

using VitaNex;
#endregion

namespace Server.Engines.CustomTitles
{
	public sealed class TitleProfile : PropertyObject
	{
		public List<Title> Titles { get; private set; }
		public List<TitleHue> Hues { get; private set; }

		[CommandProperty(CustomTitles.Access, true)]
		public PlayerMobile Owner { get; private set; }

		[CommandProperty(CustomTitles.Access, true)]
		public Title SelectedTitle { get; set; }

		[CommandProperty(CustomTitles.Access, true)]
		public TitleHue SelectedHue { get; set; }

		public TitleProfile(PlayerMobile pm)
		{
			Titles = new List<Title>();
			Hues = new List<TitleHue>();

			Owner = pm;
		}

		public TitleProfile(GenericReader reader)
			: base(reader)
		{ }

		public override void Reset()
		{
			Titles.Clear();
			Hues.Clear();
		}

		public override void Clear()
		{
			Titles.Clear();
			Hues.Clear();
		}

		public Title[] GetTitles(params TitleRarity[] rarities)
		{
			if (rarities != null && rarities.Length > 0)
			{
				return Titles.Where(t => rarities.Contains(t.Rarity)).ToArray();
			}

			return Titles.ToArray();
		}

		public void Add(Title title)
		{
			if (title != null)
			{
				Titles.Add(title);
			}
		}

		public bool Remove(Title title)
		{
			return title != null && Titles.Remove(title);
		}

		public bool Contains(Title title)
		{
			return title != null && Titles.Contains(title);
		}

		public TitleHue[] GetHues(params TitleRarity[] rarities)
		{
			if (rarities != null && rarities.Length > 0)
			{
				return Hues.Where(h => rarities.Contains(h.Rarity)).ToArray();
			}

			return Hues.ToArray();
		}

		public void Add(TitleHue hue)
		{
			if (hue != null)
			{
				Hues.Add(hue);
			}
		}

		public bool Remove(TitleHue hue)
		{
			return hue != null && Hues.Remove(hue);
		}

		public bool Contains(TitleHue hue)
		{
			return hue != null && Hues.Contains(hue);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.WriteMobile(Owner);

						writer.WriteBlockList(Titles, CustomTitles.WriteTitle);
						writer.WriteBlockList(Hues, CustomTitles.WriteTitleHue);

						CustomTitles.WriteTitle(writer, SelectedTitle);
						CustomTitles.WriteTitleHue(writer, SelectedHue);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						Owner = reader.ReadMobile<PlayerMobile>();

						Titles = reader.ReadBlockList(CustomTitles.ReadTitle);
						Hues = reader.ReadBlockList(CustomTitles.ReadTitleHue);

						SelectedTitle = CustomTitles.ReadTitle(reader);
						SelectedHue = CustomTitles.ReadTitleHue(reader);
					}
					break;
			}
		}
	}
}