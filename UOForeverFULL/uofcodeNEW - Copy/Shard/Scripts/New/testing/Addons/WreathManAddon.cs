
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class WreathManAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1006, 0, 0, 5}, {1011, -1, -1, 5}, {1012, 1, 1, 5}// 1	2	3	
			, {1013, 1, -1, 5}, {1014, -1, 1, 5}, {1011, -2, -2, 0}// 4	5	6	
			, {1012, 2, 2, 0}, {1013, 2, -2, 0}, {1014, -2, 2, 0}// 7	8	9	
			, {1007, -1, 2, 0}, {1008, 2, -1, 0}, {1009, -1, -2, 0}// 10	11	12	
			, {1010, -2, -1, 0}, {1007, 0, 2, 0}, {1008, 2, 0, 0}// 13	14	15	
			, {1009, 0, -2, 0}, {1010, -2, 0, 0}, {1007, 1, 2, 0}// 16	17	18	
			, {1008, 2, 1, 0}, {1009, 1, -2, 0}, {1010, -2, 1, 0}// 19	20	21	
			, {1007, 0, 1, 5}, {1008, 1, 0, 5}, {1009, 0, -1, 5}// 22	23	24	
			, {1010, -1, 0, 5}, {11539, 0, 0, 7}// 25	26	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new WreathManAddonDeed();
			}
		}

		[ Constructable ]
		public WreathManAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 9004, 0, 1, 7, 999, -1, "", 1);// 27

		}

		public WreathManAddon( Serial serial ) : base( serial )
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

	public class WreathManAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new WreathManAddon();
			}
		}

		[Constructable]
		public WreathManAddonDeed()
		{
			Name = "WreathMan";
		}

		public WreathManAddonDeed( Serial serial ) : base( serial )
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