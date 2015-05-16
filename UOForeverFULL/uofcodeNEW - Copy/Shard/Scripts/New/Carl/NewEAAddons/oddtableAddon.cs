

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class oddtableAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {19650, 1, 0, 0}, {19651, 1, 1, 0}, {19652, 1, -1, 0}// 1	2	3	
			, {19653, 0, -1, 0}, {19654, 0, 0, 0}// 4	5	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new oddtableAddonDeed();
			}
		}

		[ Constructable ]
		public oddtableAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public oddtableAddon( Serial serial ) : base( serial )
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

	public class oddtableAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new oddtableAddon();
			}
		}

		[Constructable]
		public oddtableAddonDeed()
		{
			Name = "oddtable";
		}

		public oddtableAddonDeed( Serial serial ) : base( serial )
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