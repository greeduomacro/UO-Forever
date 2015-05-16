
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class shorttableAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {16438, 1, 1, 0}, {16437, 0, 1, 0}, {16436, 1, 0, 0}// 1	2	3	
			, {16435, 0, 0, 0}// 4	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new shorttableAddonDeed();
			}
		}

		[ Constructable ]
		public shorttableAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public shorttableAddon( Serial serial ) : base( serial )
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

	public class shorttableAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new shorttableAddon();
			}
		}

		[Constructable]
		public shorttableAddonDeed()
		{
			Name = "shorttable";
		}

		public shorttableAddonDeed( Serial serial ) : base( serial )
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