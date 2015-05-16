

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class PillowbedAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {19560, 1, 0, 0}, {19561, 1, 1, 0}, {19563, 0, -1, 0}// 1	2	3	
			, {19562, 1, -1, 0}, {19565, 0, 1, 0}, {19564, 0, 0, 0}// 4	5	6	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new PillowbedAddonDeed();
			}
		}

		[ Constructable ]
		public PillowbedAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public PillowbedAddon( Serial serial ) : base( serial )
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

	public class PillowbedAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new PillowbedAddon();
			}
		}

		[Constructable]
		public PillowbedAddonDeed()
		{
			Name = "3pillowbed";
		}

		public PillowbedAddonDeed( Serial serial ) : base( serial )
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