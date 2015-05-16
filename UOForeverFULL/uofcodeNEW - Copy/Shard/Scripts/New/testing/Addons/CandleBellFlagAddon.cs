
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class CandleBellFlagAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {7186, 0, 1, 12}// 4	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new CandleBellFlagAddonDeed();
			}
		}

		[ Constructable ]
		public CandleBellFlagAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 5527, 0, 1, 0, 1, -1, "", 1);// 1
			AddComplexComponent( (BaseAddon) this, 5676, 0, 0, 0, 1, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 5677, 0, -1, 0, 1, -1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 3248, 0, 1, 10, 2003, -1, "", 1);// 5
			AddComplexComponent( (BaseAddon) this, 9070, 0, 0, 7, 38, 0, "", 1);// 6

		}

		public CandleBellFlagAddon( Serial serial ) : base( serial )
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

	public class CandleBellFlagAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new CandleBellFlagAddon();
			}
		}

		[Constructable]
		public CandleBellFlagAddonDeed()
		{
			Name = "CandleBellFlag";
		}

		public CandleBellFlagAddonDeed( Serial serial ) : base( serial )
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