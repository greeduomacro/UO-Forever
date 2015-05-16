using System;
using Server.Items;

namespace Server
{
	public class GemLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 25; } }

		private static Type[] m_GemTypes = new Type[]
			{
				typeof( Amber ),				typeof( Amethyst ),				typeof( Citrine ),
				typeof( Diamond ),				typeof( Emerald ),				typeof( Ruby ),
				typeof( Sapphire ),				typeof( StarSapphire ),			typeof( Tourmaline ),
                typeof( Gunpowder)
			};

		public GemLootSet( int amt ) : this( amt, amt )
		{
		}

		public GemLootSet( int min, int max ) : base( min, max, m_GemTypes )
		{
		}
	}
}