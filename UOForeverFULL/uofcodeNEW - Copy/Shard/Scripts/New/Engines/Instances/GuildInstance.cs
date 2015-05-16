using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Regions;
using Server.Mobiles;
using Server.Guilds;

namespace Server.Engines.Instances
{
	public class GuildInstance : BaseInstance<BaseGuild>
	{
		public GuildInstance(Map map, string name, Point3D retloc, Map retmap) : this( map, name, retloc, retmap, DateTime.MaxValue )
		{
		}

		public GuildInstance(Map map, string name, Point3D retloc, Map retmap, DateTime exp) : base( map, name, retloc, retmap, exp )
		{
		}

		public GuildInstance( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 ); // version
		}

		public override void SerializeRefList( GenericWriter writer )
		{
			writer.Write( RefList, true );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		public override void DeserializeRefList( GenericReader reader )
		{
			RefList.AddRange( reader.ReadStrongGuildList() );
		}
	}
}
