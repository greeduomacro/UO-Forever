using System;
using System.Collections.Generic;
using System.Net;
using Server.Accounting;

namespace Server.Items
{
    public class NewGuildPersistence : Item
    {
        private static NewGuildPersistence _Instance;

        public static NewGuildPersistence Instance { get { return _Instance; } }

        public static List<IPAddress> JoinedIPs; 

        public override string DefaultName
        {
            get { return "New Guild - Instanced"; }
        }

        public NewGuildPersistence()
            : base(1)
        {
            if (_Instance == null || _Instance.Deleted)
                _Instance = this;
            else
                base.Delete();

            Movable = false;
            JoinedIPs = new List<IPAddress>();
        }

        public NewGuildPersistence(Serial serial)
            : base(serial)
        {
            _Instance = this;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            int count = JoinedIPs.Count;
            writer.Write(count);
            if (count > 0)
            {
                foreach (var ip in JoinedIPs)
                {
                    writer.Write(ip);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            JoinedIPs = new List<IPAddress>();

            int version = reader.ReadInt();

            int count = reader.ReadInt();
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var r = reader.ReadIPAddress();
                    JoinedIPs.Add(r);
                }
            }
            _Instance = this;
        }

        public override void Delete()
        {
        }
    }
}
