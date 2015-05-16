
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class EntranceAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {222, 1, -1, 20}, {1309, 0, 1, 20}, {12884, 0, 1, 0}// 1	5	6	
			, {12882, 0, 1, 20}, {209, 0, 1, 0}, {12887, 3, 1, 0}// 7	8	9	
			, {1309, 1, 1, 20}, {3314, 3, 1, 0}, {12882, 0, 0, 0}// 10	11	12	
			, {222, 0, 1, 20}, {12916, 1, 0, 20}, {12925, 3, 0, 0}// 13	14	15	
			, {12926, 2, 0, 20}, {221, -1, 1, 20}, {1309, 2, 1, 20}// 16	18	19	
			, {1309, 1, 0, 20}, {1309, 0, 0, 20}, {3350, 0, 1, 0}// 21	23	24	
			, {221, 2, 0, 20}, {7682, 0, 0, 0}, {221, -1, 0, 20}// 25	27	28	
			, {7682, 3, 0, 0}, {7682, 0, 1, 0}, {12927, 2, 1, 20}// 29	30	31	
			, {230, 1, 1, 20}, {3310, 3, 0, 0}, {3341, 0, 1, 0}// 33	34	35	
			, {12922, 4, 0, 0}, {12922, -4, 0, 0}, {14138, 1, 1, 0}// 37	39	42	
			, {7687, 4, 0, 0}, {3342, -4, 0, 0}, {206, -1, 1, 0}// 43	44	47	
			, {205, 2, 1, 0}, {12928, -4, 1, 0}, {12918, 2, 1, 20}// 48	49	50	
			, {3314, 0, 1, 0}, {1309, 2, 0, 20}, {3310, -3, 0, 0}// 51	52	53	
			, {230, 1, 1, 18}, {12930, 2, 1, 28}, {220, 2, 1, 20}// 54	55	56	
			, {12927, 2, 1, 20}, {12883, 0, 0, 20}, {3314, 3, 0, 9}// 58	59	60	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new EntranceAddonDeed();
			}
		}

		[ Constructable ]
		public EntranceAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 200, 0, -1, 0, 1175, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 200, 1, -1, 0, 1175, -1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 200, 2, -1, 0, 1175, -1, "", 1);// 4
			AddComplexComponent( (BaseAddon) this, 203, -1, 0, 0, 0, 1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 3023, -3, 0, 1, 1157, -1, "Under Construction - Dungeon Perilous", 1);// 20
			AddComplexComponent( (BaseAddon) this, 2572, -4, 0, 13, 0, 1, "", 1);// 22
			AddComplexComponent( (BaseAddon) this, 4633, 1, 1, 20, 0, -1, "wall", 1);// 26
			AddComplexComponent( (BaseAddon) this, 203, 2, 0, 0, 0, 1, "", 1);// 32
			AddComplexComponent( (BaseAddon) this, 4635, 1, 1, 21, 0, -1, "wall", 1);// 36
			AddComplexComponent( (BaseAddon) this, 7979, 1, 1, 32, 1161, -1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 3555, 1, 1, 34, 0, 1, "fire", 1);// 40
			AddComplexComponent( (BaseAddon) this, 2572, 4, 0, 13, 0, 1, "", 1);// 41
			AddComplexComponent( (BaseAddon) this, 4758, -3, 0, 0, 1157, -1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 2572, -2, 0, 0, 0, 1, "", 1);// 46
			AddComplexComponent( (BaseAddon) this, 4634, 1, 1, 21, 0, -1, "wall", 1);// 57

		}

		public EntranceAddon( Serial serial ) : base( serial )
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

	public class EntranceAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new EntranceAddon();
			}
		}

		[Constructable]
		public EntranceAddonDeed()
		{
			Name = "Entrance";
		}

		public EntranceAddonDeed( Serial serial ) : base( serial )
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