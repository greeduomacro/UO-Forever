#region References
using System;
using System.Collections.Generic;
using System.Linq;

using VitaNex;
using VitaNex.Crypto;
#endregion

namespace Server.PvPTemplates
{
	[PropertyObject]
	public sealed class TemplateSerial : CryptoHashCode
	{
		public static CryptoHashType Algorithm = CryptoHashType.MD5;

		[CommandProperty(PvPTemplates.Access)]
		public override string Seed
		{
			get { return base.Seed; }
		}

		[CommandProperty(PvPTemplates.Access)]
		public override string Value
		{
			get { return base.Value.Replace("-", String.Empty); }
		}

		public TemplateSerial()
			: this(String.Empty)
		{ }

		public TemplateSerial(string name)
			: base(Algorithm, (name ?? String.Empty) + TimeStamp.UtcNow)
		{ }

		public TemplateSerial(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return Value;
		}
	}

	public sealed class Template : PropertyObject, IEquatable<Template>
	{
		[CommandProperty(PvPTemplates.Access, true)]
		public TemplateSerial UID { get; private set; }

		[CommandProperty(PvPTemplates.Access)]
		public string Name { get; set; }

		[CommandProperty(PvPTemplates.Access)]
		public string Notes { get; set; }

		public Dictionary<SkillName, double> Skills { get; set; }
		public Dictionary<StatType, int> Stats { get; set; }

		public Template(
			string name,
			string notes = null,
			IDictionary<SkillName, double> skills = null,
			IDictionary<StatType, int> stats = null)
		{
			UID = new TemplateSerial(name);

			Name = name ?? "Template";
			Notes = notes ?? String.Empty;
			Skills = skills != null ? new Dictionary<SkillName, double>(skills) : new Dictionary<SkillName, double>();
			Stats = stats != null ? new Dictionary<StatType, int>(stats) : new Dictionary<StatType, int>();
		}

		public Template(GenericReader reader)
			: base(reader)
		{ }

		public override void Reset()
		{
			Name = "Template";
			Notes = String.Empty;
			Skills.Clear();
			Stats.Clear();
		}

		public override void Clear()
		{
			Name = "Template";
			Notes = String.Empty;
			Skills.Clear();
			Stats.Clear();
		}

		public IEnumerable<SkillName> GetActiveSkills()
		{
			return Skills.Where(kv => kv.Value != 0.0).Select(kv => kv.Key);
		}
		
		public IEnumerable<SkillName> GetInactiveSkills()
		{
			return Skills.Where(kv => kv.Value == 0.0).Select(kv => kv.Key);
		}
		
		public override int GetHashCode()
		{
			return UID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj))
			{
				return false;
			}

			if(ReferenceEquals(this, obj))
			{
				return true;
			}

			var other = obj as Template;
			return other != null && Equals(other);
		}

		public bool Equals(Template other)
		{
			if(ReferenceEquals(null, other))
			{
				return false;
			}

			if(ReferenceEquals(this, other))
			{
				return true;
			}

			return UID.Equals(other.UID);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			#region Critical Non-Versioned Values
			UID.Serialize(writer);
			#endregion

			int version = writer.SetVersion(0);

			switch(version)
			{
				case 0:
					{
						writer.Write(Name);
						writer.Write(Notes);
						writer.WriteBlockDictionary(
							Skills,
							(key, val) =>
							{
								writer.WriteFlag(key);
								writer.Write(val);
							});
						writer.WriteBlockDictionary(
							Stats,
							(key, val) =>
							{
								writer.WriteFlag(key);
								writer.Write(val);
							});
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			#region Critical Non-Versioned Values
			UID = new TemplateSerial(reader);
			#endregion

			int version = reader.GetVersion();

			switch(version)
			{
				case 0:
					{
						Name = reader.ReadString();
						Notes = reader.ReadString();
						Skills = reader.ReadBlockDictionary(
							() =>
							{
								var key = reader.ReadFlag<SkillName>();
								double val = reader.ReadDouble();
								return new KeyValuePair<SkillName, double>(key, val);
							},
							Skills);
						Stats = reader.ReadBlockDictionary(
							() =>
							{
								var key = reader.ReadFlag<StatType>();
								int val = reader.ReadInt();
								return new KeyValuePair<StatType, int>(key, val);
							},
							Stats);
					}
					break;
			}
		}
	}
}