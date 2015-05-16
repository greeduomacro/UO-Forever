
////////////////////////////////////////
//                                     //
//     //
// Addon Generator  //
//          //
//             //
//                                     //
////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SpiderWallCarvingAddon : BaseAddon
	{
         
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new SpiderWallCarvingAddonDeed();
			}
		}

		[ Constructable ]
		public SpiderWallCarvingAddon()
		{



			AddComplexComponent( (BaseAddon) this, 4668, 0, -2, 0, 942, -1, "", 1);// 1
			AddComplexComponent( (BaseAddon) this, 4667, 0, -1, 0, 942, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 4667, 0, 0, 0, 942, -1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 4667, 0, 1, 0, 942, -1, "", 1);// 4
			AddComplexComponent( (BaseAddon) this, 4667, 0, 2, 0, 942, -1, "", 1);// 5
			AddComplexComponent( (BaseAddon) this, 4666, 0, 3, 0, 942, -1, "", 1);// 6

		}

		public SpiderWallCarvingAddon( Serial serial ) : base( serial )
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

	public class SpiderWallCarvingAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new SpiderWallCarvingAddon();
			}
		}

		[Constructable]
		public SpiderWallCarvingAddonDeed()
		{
			Name = "a commemorative wall carving deed";
		}

		public SpiderWallCarvingAddonDeed( Serial serial ) : base( serial )
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