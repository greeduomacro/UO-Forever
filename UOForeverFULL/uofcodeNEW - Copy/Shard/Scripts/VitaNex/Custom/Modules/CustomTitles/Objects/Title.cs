#region References
using System;
#endregion

namespace Server.Engines.CustomTitles
{
	public sealed class Title : TitleObject
	{
		[CommandProperty(CustomTitles.Access, true)]
		public string MaleTitle { get; private set; }

		[CommandProperty(CustomTitles.Access, true)]
		public string FemaleTitle { get; private set; }

		[CommandProperty(CustomTitles.Access)]
		public TitleDisplay Display { get; set; }

		public Title(
			string maleTitle,
			string femalTitle,
			TitleRarity rarity = TitleRarity.Common,
			TitleDisplay display = TitleDisplay.BeforeName)
			: base(rarity)
		{
			MaleTitle = maleTitle;
			FemaleTitle = femalTitle;

			Display = display;
		}

		public Title(GenericReader reader)
			: base(reader)
		{ }

		public override void Reset()
		{
			base.Reset();

			MaleTitle = String.Empty;
			FemaleTitle = String.Empty;
		}

		public override void Clear()
		{
			base.Clear();

			MaleTitle = String.Empty;
			FemaleTitle = String.Empty;
		}

		public bool Match(string title)
		{
			if (String.IsNullOrWhiteSpace(title))
			{
				return false;
			}

			title = title.Trim();

			int i = title.IndexOf('~');

			if (i >= 0)
			{
				return Insensitive.Equals(MaleTitle, title.Substring(0, i).Trim()) ||
					   Insensitive.Equals(FemaleTitle, title.Substring(i + 1, title.Length - (i + 1)).Trim());
			}

			return Insensitive.Equals(MaleTitle, title) || Insensitive.Equals(FemaleTitle, title) ||
				   Insensitive.Equals(title, ToString());
		}

		public override int CompareTo(TitleObject other)
		{
			int result = base.CompareTo(other);

			if (this.CompareNull(other as Title, ref result))
			{
				return result;
			}

			return Insensitive.Compare(ToString(), other.ToString());
		}

		public string ToString(bool female)
		{
			return !female ? MaleTitle : FemaleTitle;
		}

		public override string ToString()
		{
			return String.Format("{0} ~ {1}", MaleTitle ?? String.Empty, FemaleTitle ?? String.Empty);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					writer.WriteFlag(Display);
					goto case 0;
				case 0:
					{
						writer.Write(MaleTitle);
						writer.Write(FemaleTitle);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 1:
					Display = reader.ReadFlag<TitleDisplay>();
					goto case 0;
				case 0:
					{
						MaleTitle = reader.ReadString();
						FemaleTitle = reader.ReadString();
					}
					break;
			}

			if (version < 1)
			{
				Display = TitleDisplay.BeforeName;
			}
		}
	}
}