#region References
using System;
using System.Drawing;

using VitaNex;
#endregion

namespace Server.Engines.CustomTitles
{
	public abstract class TitleObject : PropertyObject, IEquatable<TitleObject>, IComparable<TitleObject>
	{
		[CommandProperty(CustomTitles.Access, true)]
		public TitleObjectSerial UID { get; private set; }

		[CommandProperty(CustomTitles.Access, true)]
		public TitleRarity Rarity { get; private set; }

		public TitleObject()
			: this(TitleRarity.Common)
		{ }

		public TitleObject(TitleRarity rarity)
		{
			UID = new TitleObjectSerial();

			Rarity = rarity;
		}

		public TitleObject(GenericReader reader)
			: base(reader)
		{ }

		public override void Reset()
		{
			Rarity = TitleRarity.Common;
		}

		public override void Clear()
		{
			Rarity = TitleRarity.Common;
		}

		public virtual int GetRarityHue()
		{
			return Rarity.AsHue();
		}

		public virtual Color GetRarityColor()
		{
			return Rarity.AsColor();
		}

		public virtual int CompareTo(TitleObject other)
		{
			int result = 0;

			if (this.CompareNull(other, ref result))
			{
				return result;
			}

			if (Rarity > other.Rarity)
			{
				--result;
			}
			else if (Rarity < other.Rarity)
			{
				++result;
			}

			return result;
		}

		public override bool Equals(object obj)
		{
			return obj is TitleObject ? Equals((TitleObject)obj) : base.Equals(obj);
		}

		public virtual bool Equals(TitleObject other)
		{
			return !ReferenceEquals(other, null) && UID.Equals(other.UID);
		}

		public override sealed int GetHashCode()
		{
			return UID.GetHashCode();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			UID.Serialize(writer);

			switch (version)
			{
				case 0:
					writer.WriteFlag(Rarity);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			UID = new TitleObjectSerial(reader);

			switch (version)
			{
				case 0:
					Rarity = reader.ReadFlag<TitleRarity>();
					break;
			}
		}
	}
}