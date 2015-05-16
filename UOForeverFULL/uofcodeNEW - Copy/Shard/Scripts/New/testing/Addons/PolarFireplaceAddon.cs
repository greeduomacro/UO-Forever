
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class PolarFireplaceAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1205, -1, 0, 1}, {1205, -1, 1, 1}, {1205, -1, 2, 1}// 7	8	9	
			, {7753, 2, 2, 1}, {7754, 1, 2, 1}, {7755, 0, 2, 1}// 13	14	15	
			, {7756, 0, 1, 1}, {7757, 1, 1, 1}, {7758, 2, 1, 1}// 16	17	18	
			, {7759, 2, 0, 1}, {7760, 1, 0, 1}, {7761, 0, 0, 1}// 19	20	21	
			, {7789, 0, 1, 5}, {11756, 1, 0, 1}, {7133, -1, 1, 1}// 22	23	24	
			, {7133, -1, 0, 1}, {7133, -1, 0, 1}, {3094, 2, 1, 1}// 25	26	27	
			, {7138, -1, 1, 1}, {7138, -1, 0, 1}// 31	32	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new PolarFireplaceAddonDeed();
			}
		}

		[ Constructable ]
		public PolarFireplaceAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 577, -1, 2, 1, 2052, -1, "", 1);// 1
			AddComplexComponent( (BaseAddon) this, 577, -1, -1, 1, 2052, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 578, -2, 0, 1, 2052, -1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 578, -2, 2, 1, 2052, -1, "", 1);// 4
			AddComplexComponent( (BaseAddon) this, 578, -2, 1, 1, 2052, -1, "", 1);// 5
			AddComplexComponent( (BaseAddon) this, 580, -2, -1, 1, 2052, -1, "", 1);// 6
			AddComplexComponent( (BaseAddon) this, 584, -1, 1, 1, 2052, -1, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 586, -1, 2, 1, 2052, -1, "", 1);// 11
			AddComplexComponent( (BaseAddon) this, 584, -1, 0, 1, 2052, -1, "", 1);// 12
			AddComplexComponent( (BaseAddon) this, 14732, -1, 0, 1, 0, 0, "", 1);// 28
			AddComplexComponent( (BaseAddon) this, 14732, -1, 1, 1, 0, 0, "", 1);// 29
			AddComplexComponent( (BaseAddon) this, 14732, -1, 2, 0, 0, 0, "", 1);// 30
			AddComplexComponent( (BaseAddon) this, 1205, -1, -1, 1, 2052, -1, "", 1);// 33
			AddComplexComponent( (BaseAddon) this, 1205, -1, 0, 1, 2052, -1, "", 1);// 34
			AddComplexComponent( (BaseAddon) this, 1205, -1, 1, 1, 2052, -1, "", 1);// 35
			AddComplexComponent( (BaseAddon) this, 1205, -1, 2, 1, 2052, -1, "", 1);// 36
			AddComplexComponent( (BaseAddon) this, 1205, 0, -1, 1, 2052, -1, "", 1);// 37
			AddComplexComponent( (BaseAddon) this, 1205, 0, 0, 1, 2052, -1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 1205, 0, 1, 1, 2052, -1, "", 1);// 39
			AddComplexComponent( (BaseAddon) this, 1205, 0, 2, 1, 2052, -1, "", 1);// 40
			AddComplexComponent( (BaseAddon) this, 1205, 1, -1, 1, 2052, -1, "", 1);// 41
			AddComplexComponent( (BaseAddon) this, 1205, 1, 0, 1, 2052, -1, "", 1);// 42
			AddComplexComponent( (BaseAddon) this, 1205, 1, 1, 1, 2052, -1, "", 1);// 43
			AddComplexComponent( (BaseAddon) this, 1205, 1, 2, 1, 2052, -1, "", 1);// 44

		}

		public PolarFireplaceAddon( Serial serial ) : base( serial )
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

	public class PolarFireplaceAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new PolarFireplaceAddon();
			}
		}

		[Constructable]
		public PolarFireplaceAddonDeed()
		{
			Name = "PolarFireplace";
		}

		public PolarFireplaceAddonDeed( Serial serial ) : base( serial )
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