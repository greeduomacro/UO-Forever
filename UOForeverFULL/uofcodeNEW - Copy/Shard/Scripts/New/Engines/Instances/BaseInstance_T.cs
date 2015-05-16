using System;
using System.Collections;
using System.Collections.Generic;
using Server;

namespace Server.Engines.Instances
{
	public enum InstanceType
	{
		Dragon,		Blackthorne,	BloodKnight,
		None
	}

	public enum InstancePrivacy
	{
		Private,	Party,			Public
	}

	public abstract class BaseInstance<T> : BaseInstance
	{
		private List<T> m_RefList;
		public List<T> RefList{ get{ return m_RefList; } }

		public BaseInstance( Map map, string name, Point3D retloc, Map retmap ) : this( map, name, retloc, retmap, DateTime.MaxValue )
		{
		}

		public BaseInstance( Map map, string name, Point3D retloc, Map retmap, DateTime exp ) : base( map, name, retloc, retmap, exp )
		{
			m_RefList = new List<T>();
		}

		public BaseInstance( Serial serial ) : base( serial )
		{
			m_RefList = new List<T>();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public abstract void SerializeRefList( GenericWriter writer );

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		public abstract void DeserializeRefList( GenericReader reader );
	}
}
