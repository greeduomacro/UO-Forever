using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Conquests
{
	public sealed class ConquestCategory : IEquatable<ConquestCategory>, IEquatable<string>
	{
		public const string Default = "Misc";
		
		public ConquestCategory Parent { get; private set; }

		public string Name { get; private set; }
		public string FullName { get; private set; }

		public bool HasParent { get { return Parent != null && !Parent.IsEmpty; } }

		public bool IsRoot { get { return !HasParent; } }
		public bool IsEmpty { get { return String.IsNullOrWhiteSpace(FullName); } }
		public bool IsDefault { get { return Insensitive.Equals(FullName, Default); } }

		public ConquestCategory()
			: this(Default)
		{ }

		public ConquestCategory(string root)
		{
			FullName = root ?? String.Empty;

			var parents = FullName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			
			if (parents.Length == 0)
			{
				Name = FullName;
				return;
			}

			Name = parents.Last();

			if (parents.Length > 1)
			{
				Parent = String.Join("/", parents.Take(parents.Length - 1));
			}
		}
		
		public bool IsChildOf(ConquestCategory parent)
		{
			if (parent == null)
			{
				return false;
			}

			ConquestCategory p = Parent;

			while (p != null)
			{
				if (p == parent)
				{
					return true;
				}

				p = p.Parent;
			}

			return false;
		}

		public IEnumerable<ConquestCategory> GetParents()
		{
			ConquestCategory c = this;

			while (c.HasParent)
			{
				c = c.Parent;

				yield return c;
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = FullName.Length;
				hash = (hash * 397) ^ FullName.ToLower().GetHashCode();
				return hash;
			}
		}

		public override bool Equals(object obj)
		{
			return (obj is string && Equals((string)obj)) || (obj is ConquestCategory && Equals((ConquestCategory)obj));
		}

		public bool Equals(ConquestCategory other)
		{
			return !ReferenceEquals(other, null) && Equals(other.FullName);
		}

		public bool Equals(string other)
		{
			return Insensitive.Equals(FullName, other);
		}

		public static bool operator ==(ConquestCategory l, ConquestCategory r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(ConquestCategory l, ConquestCategory r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}

		public static implicit operator ConquestCategory(string root)
		{
			return new ConquestCategory(root);
		}

		public static implicit operator string(ConquestCategory cat)
		{
			return cat.FullName;
		}
	}
}