using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class testAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {3287, 0, 0, 0}, {3286, 0, 0, 0}, {3874, 0, 0, 2}// 1	2	3	
			, {3864, 0, 0, 2}, {3862, 0, 0, 2}, {3870, 0, 0, 2}// 4	5	6	
			, {3821, 0, 1, 20}, {3821, 0, 0, 20}, {3821, 0, 1, 20}// 7	8	9	
			, {3821, 1, 0, 20}, {3821, 1, 0, 20}, {3821, 1, 1, 20}// 10	11	12	
			, {3821, -1, 0, 20}, {3821, -1, 0, 30}, {3821, 0, 0, 20}// 13	14	15	
			, {3821, 0, 1, 20}, {3821, 1, 0, 20}, {3821, 1, 1, 20}// 16	17	18	
			, {3821, 0, 0, 20}// 19	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new testAddonDeed();
			}
		}

		[ Constructable ]
		public testAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public testAddon( Serial serial ) : base( serial )
		{
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

	public class testAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new testAddon();
			}
		}

		[Constructable]
		public testAddonDeed()
		{
			Name = "Tree";
		}

		public testAddonDeed( Serial serial ) : base( serial )
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