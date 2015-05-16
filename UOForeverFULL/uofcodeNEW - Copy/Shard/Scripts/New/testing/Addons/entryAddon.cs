
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class entryAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1309, 0, 1, 22}, {12884, 0, 1, 0}, {12882, 0, 1, 23}// 1	2	3	
			, {209, 0, 1, 0}, {1309, 1, 1, 22}, {12882, 1, 1, 0}// 4	5	6	
			, {222, 0, 1, 20}, {1312, 2, 0, 23}, {12916, 1, 0, 24}// 8	9	10	
			, {12926, 2, 0, 20}, {1312, 2, 1, 23}, {221, -1, 1, 19}// 11	12	15	
			, {1309, 2, 1, 20}, {1309, 1, 0, 23}, {1309, 0, 0, 22}// 16	17	18	
			, {3350, 0, 1, 0}, {221, 2, 0, 21}, {7682, 0, 0, 0}// 19	20	22	
			, {221, -1, 0, 19}, {7682, 0, 1, 0}, {12927, 2, 1, 20}// 23	24	25	
			, {230, 1, 1, 20}, {3341, 0, 1, 0}, {1312, 1, 0, 22}// 27	28	29	
			, {14138, 1, 1, 0}, {206, -1, 1, 0}, {205, 2, 1, 0}// 33	35	36	
			, {12918, 2, 1, 20}, {3314, 0, 1, 0}, {1309, 2, 0, 22}// 37	38	39	
			, {230, 1, 1, 18}, {12930, 2, 1, 28}, {220, 2, 1, 21}// 40	41	42	
			, {12927, 2, 1, 20}, {12883, 0, 0, 23}, {4653, 1, 1, 0}// 44	45	46	
			, {12932, 2, 0, 23}// 47	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new entryAddonDeed();
			}
		}

		[ Constructable ]
		public entryAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 7107, 1, 0, 0, 1175, -1, " The Snake Pit", 1);// 7
			AddComplexComponent( (BaseAddon) this, 7107, 0, 0, 0, 1175, -1, " The Snake Pit", 1);// 13
			AddComplexComponent( (BaseAddon) this, 203, -1, 0, 0, 0, 1, "", 1);// 14
			AddComplexComponent( (BaseAddon) this, 4633, 1, 1, 23, 0, -1, "wall", 1);// 21
			AddComplexComponent( (BaseAddon) this, 203, 2, 0, 0, 0, 1, "", 1);// 26
			AddComplexComponent( (BaseAddon) this, 4635, 1, 1, 24, 0, -1, "wall", 1);// 30
			AddComplexComponent( (BaseAddon) this, 7979, 1, 1, 35, 1161, -1, "", 1);// 31
			AddComplexComponent( (BaseAddon) this, 3555, 1, 1, 37, 0, 1, "fire", 1);// 32
			AddComplexComponent( (BaseAddon) this, 2572, -2, 0, 0, 0, 1, "", 1);// 34
			AddComplexComponent( (BaseAddon) this, 4634, 1, 1, 24, 0, -1, "wall", 1);// 43

		}

		public entryAddon( Serial serial ) : base( serial )
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

	public class entryAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new entryAddon();
			}
		}

		[Constructable]
		public entryAddonDeed()
		{
			Name = "entry";
		}

		public entryAddonDeed( Serial serial ) : base( serial )
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