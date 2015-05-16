#region References
using System;
using System.Globalization;
#endregion

namespace Server.Engines.CustomTitles
{
	public sealed class TitleHue : TitleObject
	{
		[CommandProperty(CustomTitles.Access, true)]
		public int Hue { get; private set; }

		public TitleHue()
			: this(0)
		{ }

		public TitleHue(int hue, TitleRarity rarity = TitleRarity.Common)
			: base(rarity)
		{
			Hue = hue;
		}

		public TitleHue(GenericReader reader)
			: base(reader)
		{ }

		public override void Reset()
		{
			base.Reset();

			Hue = 0;
		}

		public override void Clear()
		{
			base.Clear();

			Hue = 0;
		}

		public bool Match(int hue)
		{
			return Hue == hue;
		}

		public override int CompareTo(TitleObject other)
		{
			int result = base.CompareTo(other);

			if (this.CompareNull(other as TitleHue, ref result))
			{
				return result;
			}

			var hue = (TitleHue)other;

			if (Hue > hue.Hue)
			{
				--result;
			}
			else if (Hue > hue.Hue)
			{
				++result;
			}

			return result;
		}

		public string ToString(string format)
		{
			return Hue.ToString(format);
		}

		public override string ToString()
		{
			return Hue.ToString(CultureInfo.InvariantCulture);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(Hue);
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
					Hue = reader.ReadInt();
					break;
			}
		}
	}
}