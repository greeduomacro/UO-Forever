#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server.Mobiles;

using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
	public sealed class ConquestProfile : PropertyObject, IEnumerable<ConquestState>
	{
		public List<ConquestState> Registry { get; private set; }

		public ConquestState this[int index] { get { return Registry.InBounds(index) ? Registry[index] : null; } }

		[CommandProperty(Conquests.Access)]
		public int Count { get { return Registry.Count; } }

		[CommandProperty(Conquests.Access, true)]
		public PlayerMobile Owner { get; private set; }

		[CommandProperty(Conquests.Access)]
		public long Credit { get; set; }

		public ConquestProfile(PlayerMobile pm)
		{
			Registry = new List<ConquestState>();

			Owner = pm;

			Sync();
		}

		public ConquestProfile(GenericReader reader)
			: base(reader)
		{ }

		public void Sync()
		{
			Registry.RemoveAll(s => s == null || !s.ConquestExists || !Conquests.Validate(s.Conquest, Owner));

			Conquests.ConquestRegistry.Values.Where(c => Conquests.Validate(c, Owner)).ForEach(c => EnsureState(c));
		}

		public override void Reset()
		{
			Registry.ForEach(s => s.Reset());

			Registry.Clear();

            Registry.TrimExcess();

			if (Conquests.Profiles.ContainsValue(this))
			{
				Sync();
			}
		}

		public override void Clear()
		{
			Registry.ForEach(s => s.Clear());

			Registry.Clear();

            Registry.TrimExcess();

			if (Conquests.Profiles.ContainsValue(this))
			{
				Sync();
			}
		}
		
		public ConquestState EnsureState(Conquest c)
		{
			if (Owner == null || c == null)
			{
				return null;
			}

			ConquestState state;

			if (!TryGetState(c, out state))
			{
				Registry.Add(state = new ConquestState(Owner, c));
			}

			return state;
		}

		public bool TryGetState(Conquest c, out ConquestState state)
		{
			state = null;

			return c != null && (state = Registry.FirstOrDefault(s => s.Conquest == c)) != null;
		}

		public bool RemoveState(Conquest c)
		{
			return c != null && Registry.RemoveAll(s => s.Conquest == c) > 0;
		}

		public bool ContainsState(Conquest c)
		{
			return c != null && Registry.Exists(s => s.Conquest == c);
		}

		public long GetPointsTotal()
		{
			return Registry.AsParallel().Where(s => s.Completed && s.Conquest != null).Sum(s => (long)s.Conquest.Points);
		}

		public IEnumerator<ConquestState> GetEnumerator()
		{
			return Registry.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			writer.Write(Owner);

			switch (version)
			{
				case 0:
					{
						writer.Write(Credit);

						writer.WriteBlockList(Registry, s => s.Serialize(writer));
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			Owner = reader.ReadMobile<PlayerMobile>();

			switch (version)
			{
				case 0:
					{
						Credit = reader.ReadLong();

						Registry = reader.ReadBlockList(() => new ConquestState(reader));
					}
					break;
			}

			Sync();
		}
	}
}