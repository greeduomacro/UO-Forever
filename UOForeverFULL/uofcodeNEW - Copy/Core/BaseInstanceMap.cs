#region References
using System;
using System.Linq;
#endregion

namespace Server
{
	[Parsable]
	public class BaseInstanceMap : Map, IComparable<BaseInstanceMap>, ISerializable
	{
		private readonly int m_TypeRef;

		public int TypeReference { get { return m_TypeRef; } }
		int ISerializable.SerialIdentity { get { return Serial; } }

		private bool m_Deleted;
		public bool Deleted { get { return m_Deleted; } set { m_Deleted = value; } }

		public Serial Serial { get { return MapIndex; } set { MapIndex = value; } }
		public virtual bool Decays { get { return false; } }

		//private Point3D m_EjectPoint; //Where do all the Players go?
		//public Point3D EjectPoint{ get{ return m_EjectPoint; } set{ m_EjectPoint = value; } }

		public BaseInstanceMap(Map model, string name, MapRules rules)
			: this(model.MapID, model.FileIndex, model.Width, model.Height, model.Season, name, rules)
		{ }

		public BaseInstanceMap(int mapID, int fileIndex, int width, int height, int season, string name, MapRules rules)
			: base(mapID, Serial.NewMap, fileIndex, width, height, season, Core.Expansion, name, rules)
		{
			Type ourType = GetType();

			m_TypeRef = World.m_InstanceMapTypes.IndexOf(ourType);

			if (m_TypeRef == -1)
			{
				World.m_InstanceMapTypes.Add(ourType);
				m_TypeRef = World.m_InstanceMapTypes.Count - 1;
			}

			Register();
		}

		public BaseInstanceMap(Serial serial)
			: base(serial)
		{
			Type ourType = GetType();
			m_TypeRef = World.m_InstanceMapTypes.IndexOf(ourType);

			if (m_TypeRef == -1)
			{
				World.m_InstanceMapTypes.Add(ourType);
				m_TypeRef = World.m_InstanceMapTypes.Count - 1;
			}

			Register();
		}

		public virtual void EjectPlayers()
		{
			foreach (Mobile m in World.Mobiles.Values.Where(m => m.Map == this).ToArray())
			{
				if (m.Player)
				{
					m.Map = Maps[MapID];
				}
				else
				{
					m.Delete();
				}
			}
		}

		public virtual void Register()
		{
			AllMaps.Add(this);
			World.AddMap(this);
		}

		public virtual void Delete()
		{
			if (m_Deleted || !World.OnDelete(this))
			{
				return;
			}

			AllMaps.Remove(this);

			EjectPlayers();

			OnDelete();

			m_Deleted = true;
			World.RemoveMap(this);

			OnAfterDelete();
		}

		public virtual bool OnDecay()
		{
			return true;
		}

		public virtual void OnDelete()
		{ }

		public virtual void OnAfterDelete()
		{ }

		public virtual void Serialize(GenericWriter writer)
		{
			writer.WriteEncodedInt(1); // version

			// 1
			writer.Write((int)Expansion);

			// 0
			writer.Write(MapID);
			//writer.Write( MapIndex ); - serialized in idx file
			writer.Write(FileIndex);
			writer.Write(Width);
			writer.Write(Height);
			writer.Write(Season);
			writer.Write(Name);
			writer.Write((int)Rules);
		}

		public virtual void Deserialize(GenericReader reader)
		{
			int version = reader.ReadEncodedInt();

			switch (version)
			{
				case 1:
					Expansion = (Expansion)reader.ReadInt();
					goto case 0;
				case 0:
					{
						MapID = reader.ReadInt();
						//MapIndex = reader.ReadInt(); - deserialized in ctor
						FileIndex = reader.ReadInt();
						Width = reader.ReadInt();
						Height = reader.ReadInt();
						Season = reader.ReadInt();
						Name = reader.ReadString();
						Rules = (MapRules)reader.ReadInt();
					}
					break;
			}

			if (version < 1)
			{
				Expansion = Core.Expansion;
			}

			InitializeSectors();
		}

		public int CompareTo(BaseInstanceMap other)
		{
			return other != null ? Serial.CompareTo(other.Serial) : -1;
		}

		public override string ToString()
		{
			return Name;
			//return String.Format( "{0}-{1:D}", Name, MapIndex );
		}
	}
}