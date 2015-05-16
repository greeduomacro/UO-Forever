#region References

using Server;

#endregion

namespace Knives.TownHouses
{
    public class DecoreItemInfo
    {
        private Point3D c_Location;

        public string TypeString { get; private set; }
        public string Name { get; private set; }
        public int ItemID { get; private set; }
        public int Hue { get; private set; }

        public Point3D Location
        {
            get { return c_Location; }
        }

        public Map Map { get; private set; }

        public DecoreItemInfo()
        {
        }

        public DecoreItemInfo(string typestring, string name, int itemid, int hue, Point3D loc, Map map)
        {
            TypeString = typestring;
            ItemID = itemid;
            c_Location = loc;
            Map = map;
        }

        public void Save(GenericWriter writer)
        {
            writer.Write(1); // Version

            // Version 1
            writer.Write(Hue);
            writer.Write(Name);

            writer.Write(TypeString);
            writer.Write(ItemID);
            writer.Write(c_Location);
            writer.Write(Map);
        }

        public void Load(GenericReader reader)
        {
            int version = reader.ReadInt();

            if (version >= 1)
            {
                Hue = reader.ReadInt();
                Name = reader.ReadString();
            }

            TypeString = reader.ReadString();
            ItemID = reader.ReadInt();
            c_Location = reader.ReadPoint3D();
            Map = reader.ReadMap();
        }
    }
}