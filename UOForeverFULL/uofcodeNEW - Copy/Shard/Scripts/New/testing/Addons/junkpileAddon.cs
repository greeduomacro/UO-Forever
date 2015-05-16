
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class junkpileAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {7077, -1, -1, 0}, {7074, -1, 2, 0}, {7076, -1, 0, 0}// 1	2	3	
			, {3379, -1, 2, 0}, {7075, -1, 1, 0}, {3378, -1, 0, 0}// 5	6	7	
			, {3378, 1, -2, 0}, {7071, 2, 2, 0}, {7080, 0, -1, 0}// 8	9	10	
			, {7073, 0, 2, 0}, {7085, 2, -2, 0}, {7089, 2, 1, 0}// 11	12	13	
			, {7079, 0, 0, 0}, {7087, 2, -1, 0}, {3378, 2, 1, 0}// 14	15	17	
			, {7084, 1, 0, 0}, {7081, 0, -2, 0}, {7101, 1, 1, 0}// 18	19	21	
			, {7082, 1, -2, 0}, {7088, 2, 0, 0}, {7078, 0, 1, 0}// 22	24	25	
			, {7072, 1, 2, 0}, {3378, 0, -2, 0}, {7083, 1, -1, 0}// 26	27	28	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new junkpileAddonDeed();
			}
		}

		[ Constructable ]
		public junkpileAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 3555, -1, 1, 2, 0, 1, "fire", 1);// 4
			AddComplexComponent( (BaseAddon) this, 3555, 0, -2, 0, 0, 1, "fire", 1);// 16
			AddComplexComponent( (BaseAddon) this, 3555, 0, 0, 0, 0, 1, "fire", 1);// 20
			AddComplexComponent( (BaseAddon) this, 3555, 1, 2, 2, 0, 1, "fire", 1);// 23
			AddComplexComponent( (BaseAddon) this, 3555, 1, -1, 2, 0, 1, "fire", 1);// 29
			AddComplexComponent( (BaseAddon) this, 14732, 0, 0, 2, 0, 1, "", 1);// 30
			AddComplexComponent( (BaseAddon) this, 14742, 0, 0, 2, 0, 1, "", 1);// 31

		}

		public junkpileAddon( Serial serial ) : base( serial )
		{
		}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (name != null && name.Length > 0)
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType) lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class junkpileAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new junkpileAddon();
			}
		}

		[Constructable]
		public junkpileAddonDeed()
		{
			Name = "junkpile";
		}

		public junkpileAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}