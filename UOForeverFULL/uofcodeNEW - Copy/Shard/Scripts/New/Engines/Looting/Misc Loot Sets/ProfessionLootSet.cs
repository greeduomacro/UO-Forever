using System;
using Server.Items;

namespace Server
{
	public class ProfessionLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 25; } }

		private static Type[] m_ProfessionTypes = new Type[]
			{
				typeof( DovetailSaw ),		typeof( DrawKnife ),	typeof( FletcherTools ),
				typeof( FlourSifter ),		typeof( Froe ),			typeof( Hammer ),
				typeof( Inshave ),			typeof( JointingPlane ),	typeof( ShepherdsCrook ),
				typeof( MapmakersPen ),		typeof( MortarPestle ),	typeof( MouldingPlane ),
				typeof( Nails ),			typeof( RollingPin ),	typeof( Saw ),
				typeof( Scorp ),			typeof( ScribesPen ),	typeof( SewingKit ),
				typeof( Skillet ),			typeof( SledgeHammer ),	typeof( SmithHammerWeapon ),
				typeof( SmoothingPlane ),	typeof( TinkerTools ),	typeof( Tongs ),
				typeof( Pickaxe ),			typeof( Shovel ),		typeof( Hatchet ),
				typeof( FishingPole ),		typeof( ButcherKnife ),	typeof( SkinningKnife )
			};

		public ProfessionLootSet( int min, int max ) : base( min, max, m_ProfessionTypes )
		{
		}
	}
}