
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BloodyBowlFlagAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {3619, 0, 0, 13}, {7574, 0, 0, 6}, {3618, 0, 1, 15}// 4	5	6	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new BloodyBowlFlagAddonDeed();
			}
		}

		[ Constructable ]
		public BloodyBowlFlagAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 5527, 0, 1, 0, 1, -1, "", 1);// 1
			AddComplexComponent( (BaseAddon) this, 5676, 0, 0, 0, 1, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 5677, 0, -1, 0, 1, -1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 3676, 0, 1, 18, 0, 1, "", 1);// 7

		}

		public BloodyBowlFlagAddon( Serial serial ) : base( serial )
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

	public class BloodyBowlFlagAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new BloodyBowlFlagAddon();
			}
		}

		[Constructable]
		public BloodyBowlFlagAddonDeed()
		{
			Name = "BloodyBowlFlag";
		}

		public BloodyBowlFlagAddonDeed( Serial serial ) : base( serial )
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