#region References
using System;
using System.Drawing;

using Server;

using VitaNex.Crypto;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public abstract class Entry : PropertyObject, IEquatable<Entry>
	{
		[CommandProperty(UOFCentral.Access, true)]
		public Page ParentPage { get; private set; }

		[CommandProperty(UOFCentral.Access, true)]
		public CryptoHashCode UID { get; private set; }

		[CommandProperty(UOFCentral.Access)]
		public string Label { get; set; }

		[CommandProperty(UOFCentral.Access)]
		public KnownColor LabelColor { get; set; }

		[CommandProperty(UOFCentral.Access)]
		public int ArtID { get; set; }

		[CommandProperty(UOFCentral.Access)]
		public Point2D ArtOffset { get; set; }

		[Hue, CommandProperty(UOFCentral.Access)]
		public int ArtHue { get; set; }

		public Entry(Page parent)
		{
			ParentPage = parent;

			UID = new CryptoHashCode(
				CryptoHashType.MD5, GetType().Name + "+" + TimeStamp.UtcNow.Stamp + "+" + Utility.RandomDouble());

			EnsureDefaults();
		}

		public Entry(Page parent, GenericReader reader)
			: base(reader)
		{
			ParentPage = parent;
		}

		public virtual void EnsureDefaults()
		{
			Label = "New Entry";
			LabelColor = KnownColor.White;

			ArtID = 0;
			ArtOffset = Point2D.Zero;
			ArtHue = 0;
		}

		public override void Clear()
		{
			EnsureDefaults();
		}

		public override void Reset()
		{
			EnsureDefaults();
		}

		public virtual bool Valid(UOFCentralGump g)
		{
			return !String.IsNullOrWhiteSpace(Label);
		}

		public virtual void Select(UOFCentralGump g)
		{
			g.Refresh(true);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			UID.Serialize(writer);

			switch (version)
			{
				case 1:
					writer.Write(ArtOffset);
					goto case 0;
				case 0:
					{
						writer.Write(Label);
						writer.WriteFlag(LabelColor);
						writer.Write(ArtID);
						writer.Write(ArtHue);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			UID = new CryptoHashCode(reader);

			switch (version)
			{
				case 1:
					ArtOffset = reader.ReadPoint2D();
					goto case 0;
				case 0:
					{
						Label = reader.ReadString();
						LabelColor = reader.ReadFlag<KnownColor>();
						ArtID = reader.ReadInt();
						ArtHue = reader.ReadInt();
					}
					break;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Entry && Equals((Entry)obj);
		}

		public bool Equals(Entry other)
		{
			return !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || UID.Equals(other.UID));
		}

		public override int GetHashCode()
		{
			return UID.GetHashCode();
		}

		public override string ToString()
		{
			return Label;
		}

		public static bool operator ==(Entry left, Entry right)
		{
			return ReferenceEquals(null, left) ? ReferenceEquals(null, right) : left.Equals(right);
		}

		public static bool operator !=(Entry left, Entry right)
		{
			return ReferenceEquals(null, left) ? !ReferenceEquals(null, right) : !left.Equals(right);
		}
	}
}