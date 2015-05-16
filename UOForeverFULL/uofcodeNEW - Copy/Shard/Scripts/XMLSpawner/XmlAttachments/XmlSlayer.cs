using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlSlayer : XmlAttachment
	{
        private SlayerName m_Slayer = SlayerName.None;

		[CommandProperty( AccessLevel.GameMaster )]
        public SlayerName Slayer { get { return m_Slayer; } set { m_Slayer = value; } }

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments
       
		// a serial constructor is REQUIRED
		public XmlSlayer(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlSlayer(string slayer)
		{
            try
            {
                m_Slayer = (SlayerName)Enum.Parse(typeof(SlayerName), slayer, true);
            }
            catch
            {
                m_Slayer = SlayerName.None;
            }
		}

		[Attachable]
        public XmlSlayer(string slayer, string name)
		{
			Name = name;
            try
            {
                m_Slayer = (SlayerName)Enum.Parse(typeof(SlayerName), slayer, true);
            }
            catch
            {
                m_Slayer = SlayerName.None;
            }
		}

		[Attachable]
        public XmlSlayer(string slayer, string name, double expiresin)
		{
			Name = name;
            try
            {
                m_Slayer = (SlayerName)Enum.Parse(typeof(SlayerName), slayer, true);
            }
            catch
            {
                m_Slayer = SlayerName.None;
            }
			Expiration = TimeSpan.FromMinutes(expiresin);

		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
			// version 0
            writer.Write((int)m_Slayer);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
            m_Slayer = (SlayerName)reader.ReadInt();
		}
	}
}
