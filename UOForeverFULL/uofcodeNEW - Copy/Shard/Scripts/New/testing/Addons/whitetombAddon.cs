
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class whitetombAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {7264, 1, 1, 0}, {7265, 0, 1, 0}, {7266, -1, 1, 0}// 1	2	3	
			, {7267, -1, 0, 0}, {7268, 0, 0, 0}, {7269, 1, 0, 0}// 4	5	6	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new whitetombAddonDeed();
			}
		}

		[ Constructable ]
		public whitetombAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public whitetombAddon( Serial serial ) : base( serial )
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

	public class whitetombAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new whitetombAddon();
			}
		}

		[Constructable]
		public whitetombAddonDeed()
		{
			Name = "whitetomb";
		}

		public whitetombAddonDeed( Serial serial ) : base( serial )
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