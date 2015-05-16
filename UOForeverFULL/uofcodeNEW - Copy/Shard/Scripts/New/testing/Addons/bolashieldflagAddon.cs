
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class bolashieldflagAddon : BaseAddon
	{
         
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new bolashieldflagAddonDeed();
			}
		}

		[ Constructable ]
		public bolashieldflagAddon()
		{



			AddComplexComponent( (BaseAddon) this, 5487, 1, 1, 11, 1161, -1, "", 1);// 1
			AddComplexComponent( (BaseAddon) this, 5677, 0, -1, 0, 1, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 5676, 0, 0, 0, 1, -1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 9900, 0, 1, 15, 1161, -1, "", 1);// 4
			AddComplexComponent( (BaseAddon) this, 5675, 0, 1, 0, 1, -1, "", 1);// 5

		}

		public bolashieldflagAddon( Serial serial ) : base( serial )
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

	public class bolashieldflagAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new bolashieldflagAddon();
			}
		}

		[Constructable]
		public bolashieldflagAddonDeed()
		{
			Name = "bolashieldflag";
		}

		public bolashieldflagAddonDeed( Serial serial ) : base( serial )
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