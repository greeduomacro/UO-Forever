
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class PotCandleAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {9008, 0, 1, 14}, {14077, 0, 1, 13}, {14155, 0, 1, 2}// 3	4	5	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new PotCandleAddonDeed();
			}
		}

		[ Constructable ]
		public PotCandleAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 5527, 0, 2, 0, 1, -1, "", 1);// 1
			AddComplexComponent( (BaseAddon) this, 5676, 0, 1, 0, 1, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 5676, 0, 0, 0, 1, -1, "", 1);// 6
			AddComplexComponent( (BaseAddon) this, 5677, 0, -1, 0, 1, -1, "", 1);// 7
			AddComplexComponent( (BaseAddon) this, 2842, 0, 0, 13, 0, 1, "", 1);// 8

		}

		public PotCandleAddon( Serial serial ) : base( serial )
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

	public class PotCandleAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new PotCandleAddon();
			}
		}

		[Constructable]
		public PotCandleAddonDeed()
		{
			Name = "PotCandle";
		}

		public PotCandleAddonDeed( Serial serial ) : base( serial )
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