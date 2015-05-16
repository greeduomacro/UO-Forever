
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class FirepitsmallAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {6089, 1, -1, 2}, {7686, 0, -1, 2}, {6089, 0, -1, 2}// 1	2	3	
			, {6007, 1, -1, 0}, {6089, -1, -1, 2}, {4334, 1, -1, 2}// 4	5	6	
			, {6004, 0, 1, 3}, {4970, 0, 1, 0}, {3118, 0, 1, 2}// 7	8	9	
			, {6007, 1, 0, 7}, {7685, -1, 0, 2}, {3120, 0, 1, 2}// 12	13	14	
			, {4973, 1, 1, 4}, {7685, -1, 1, 2}, {14133, 0, 1, 7}// 15	16	17	
			, {14133, 1, 1, 10}, {6092, -1, 0, 2}, {3120, 1, 1, 7}// 18	19	21	
			, {3379, 0, 0, 14}, {3119, 0, 1, 2}, {6004, 1, 0, 2}// 22	23	26	
			, {4971, 1, 0, 2}, {6092, -1, 1, 2}, {6007, 0, 0, 2}// 28	29	30	
			, {6012, 0, 0, 2}, {3119, 0, 1, 2}// 31	33	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new FirepitsmallAddonDeed();
			}
		}

		[ Constructable ]
		public FirepitsmallAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 3561, 1, 0, 4, 0, 1, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 14742, 1, 1, 6, 0, 1, "", 1);// 11
			AddComplexComponent( (BaseAddon) this, 3555, 1, 1, 9, 0, 1, "", 1);// 20
			AddComplexComponent( (BaseAddon) this, 3555, 0, 1, 5, 0, 1, "", 1);// 24
			AddComplexComponent( (BaseAddon) this, 14732, 0, 1, 3, 0, 1, "", 1);// 25
			AddComplexComponent( (BaseAddon) this, 3561, 0, 1, 3, 0, 1, "", 1);// 27
			AddComplexComponent( (BaseAddon) this, 3561, 1, 1, 2, 0, 1, "", 1);// 32

		}

		public FirepitsmallAddon( Serial serial ) : base( serial )
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

	public class FirepitsmallAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new FirepitsmallAddon();
			}
		}

		[Constructable]
		public FirepitsmallAddonDeed()
		{
			Name = "Firepitsmall";
		}

		public FirepitsmallAddonDeed( Serial serial ) : base( serial )
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