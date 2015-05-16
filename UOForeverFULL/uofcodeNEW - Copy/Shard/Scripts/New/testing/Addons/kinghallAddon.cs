
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class kinghallAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {2910, 2, -1, 0}, {2729, 1, 0, 0}, {2729, 1, 3, 0}// 1	2	3	
			, {2729, 1, 1, 0}, {2729, 2, 1, 0}, {2729, 3, 2, 0}// 4	5	6	
			, {3751, 3, -2, 6}, {3748, -2, 0, 10}, {5429, 2, -2, 6}// 7	8	9	
			, {2729, 2, 2, 0}, {2729, 2, -2, 0}, {10115, -2, -1, 0}// 10	11	12	
			, {2729, 0, 2, 0}, {2729, 3, 1, 0}, {7643, -1, -1, 0}// 13	14	15	
			, {3589, 2, -2, 11}, {7641, -1, 2, 0}, {3130, -2, 3, 0}// 16	17	18	
			, {2729, 0, -2, 0}, {2729, -2, -1, 0}, {2729, 0, 3, 0}// 19	20	21	
			, {2729, 1, 2, 0}, {2729, 3, 3, 0}, {2729, -1, 3, 0}// 22	23	24	
			, {2729, -2, 1, 0}, {3745, -2, 2, 19}, {7637, 2, -1, 0}// 25	26	27	
			, {3589, -2, 2, 5}, {3743, 0, -2, 5}, {2729, 0, 0, 0}// 28	29	30	
			, {3815, -1, -2, 5}, {2729, 3, 0, 0}, {7643, 0, -1, 0}// 31	32	33	
			, {3997, -2, 0, 6}, {2729, 2, 0, 0}, {3750, 2, -2, 5}// 34	35	36	
			, {2729, -2, -2, 0}, {2729, -2, 2, 0}, {2910, 0, -1, 0}// 37	38	39	
			, {2620, 2, -2, 0}, {2620, 0, -2, 0}, {2621, 1, -2, 0}// 40	41	42	
			, {2621, -1, -2, 0}, {2628, -2, 2, 0}, {2629, -2, 1, 0}// 43	44	45	
			, {2729, -1, 1, 0}, {2729, 3, -2, 0}, {2729, 3, -1, 0}// 46	47	48	
			, {2910, -1, 0, 0}, {2910, -1, 2, 0}, {2729, -1, -2, 0}// 49	50	51	
			, {3782, -2, -2, 0}, {2729, -2, 3, 0}, {3130, -2, 0, 0}// 52	53	54	
			, {2729, 2, 0, 0}, {7644, -1, 0, 0}, {2729, 2, 3, 0}// 55	56	57	
			, {2729, 1, -1, 0}, {2729, -2, 0, 0}, {2729, 0, 1, 0}// 58	59	60	
			, {2729, 1, -2, 0}// 61	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new kinghallAddonDeed();
			}
		}

		[ Constructable ]
		public kinghallAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 5057, -2, 3, 0, 1348, -1, "", 1);// 62
			AddComplexComponent( (BaseAddon) this, 2567, -2, 2, 15, 0, 1, "", 1);// 63
			AddComplexComponent( (BaseAddon) this, 5058, -2, 0, 0, 1348, -1, "", 1);// 64
			AddComplexComponent( (BaseAddon) this, 2854, 3, 3, 0, 0, 1, "", 1);// 65
			AddComplexComponent( (BaseAddon) this, 3995, -2, -2, 0, 2003, -1, "", 1);// 66
			AddComplexComponent( (BaseAddon) this, 3992, -2, -2, 0, 38, -1, "", 1);// 67
			AddComplexComponent( (BaseAddon) this, 2574, 1, -2, 16, 0, 1, "", 1);// 68

		}

		public kinghallAddon( Serial serial ) : base( serial )
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

	public class kinghallAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new kinghallAddon();
			}
		}

		[Constructable]
		public kinghallAddonDeed()
		{
			Name = "kinghall";
		}

		public kinghallAddonDeed( Serial serial ) : base( serial )
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