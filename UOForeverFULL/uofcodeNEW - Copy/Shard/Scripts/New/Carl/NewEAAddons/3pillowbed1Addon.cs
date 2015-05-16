

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class Pillowbed1Addon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {19566, 0, 1, 0}, {19567, -1, 1, 0}, {19568, 1, 1, 0}// 1	2	3	
			, {19569, 1, 0, 0}, {19570, 0, 0, 0}, {19571, -1, 0, 0}// 4	5	6	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new Pillowbed1AddonDeed();
			}
		}

		[ Constructable ]
		public Pillowbed1Addon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public Pillowbed1Addon( Serial serial ) : base( serial )
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

	public class Pillowbed1AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new Pillowbed1Addon();
			}
		}

		[Constructable]
		public Pillowbed1AddonDeed()
		{
			Name = "3pillowbed1";
		}

		public Pillowbed1AddonDeed( Serial serial ) : base( serial )
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