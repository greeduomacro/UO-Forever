#region References
using System.Collections;
using System.Collections.Generic;

using VitaNex;
#endregion

namespace Server.PvPTemplates
{
	public sealed class TemplateCollection : PropertyObject, IEnumerable<Template>
	{
		public List<Template> Entries { get; private set; }

		public Template this[int index]
		{
			get { return Entries[index]; }
		}

		public Template this[TemplateSerial uid]
		{
			get { return Find(uid); }
		}

		[CommandProperty(PvPTemplates.Access)]
		public int Count
		{
			get { return Entries.Count; }
		}

		[CommandProperty(PvPTemplates.Access)]
		public bool IsEmpty
		{
			get { return Count == 0; }
		}

		public TemplateCollection()
		{
			Entries = new List<Template>();
		}

		public TemplateCollection(IEnumerable<Template> collection)
		{
			Entries = new List<Template>(collection);
		}

		public TemplateCollection(GenericReader reader)
			: base(reader)
		{ }

		public override void Reset()
		{
			foreach(Template t in Entries)
			{
				t.Reset();
			}
		}

		public override void Clear()
		{
			foreach(Template t in Entries)
			{
				t.Clear();
			}

			Entries.Clear();
            Entries.TrimExcess();
		}

		public Template Create(
			string name,
			string notes = null,
			IDictionary<SkillName, double> skills = null,
			IDictionary<StatType, int> stats = null)
		{
			var template = new Template(name, notes, skills, stats);

			Add(template);

			return template;
		}

		public void Add(Template template)
		{
			if(template != null && !Contains(template))
			{
				Entries.Add(template);
			}
		}

		public bool Remove(Template template)
		{
			return template != null && Entries.Remove(template);
		}

		public bool Remove(TemplateSerial uid)
		{
			return Remove(Find(uid));
		}

		public bool Contains(Template template)
		{
			return template != null && Entries.Contains(template);
		}

		public bool Contains(TemplateSerial uid)
		{
			return Find(uid) != null;
		}

		public Template Find(TemplateSerial uid)
		{
			return uid != null ? Entries.Find(template => template.UID == uid) : null;
		}

		public bool TryFind(TemplateSerial uid, out Template template)
		{
			return (template = Find(uid)) != null;
		}

		public IEnumerator<Template> GetEnumerator()
		{
			return Entries.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch(version)
			{
				case 0:
					writer.WriteBlockList(Entries, template => template.Serialize(writer));
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch(version)
			{
				case 0:
					Entries = reader.ReadBlockList(() => new Template(reader));
					break;
			}
		}
	}
}