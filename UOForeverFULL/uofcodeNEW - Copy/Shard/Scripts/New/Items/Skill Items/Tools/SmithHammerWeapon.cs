using System;
using Server;
using Server.Engines.Craft;
using Server.Misc;

namespace Server.Items
{
	[FlipableAttribute( 0x13E3, 0x13E4 )]
	public class SmithHammerWeapon : BaseBashing, IUsesRemaining, IBaseTool
	{
		public CraftSystem CraftSystem{ get{ return DefBlacksmithy.CraftSystem; } }

		public bool ShowUsesRemaining{ get{ return true; } set{ } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ShadowStrike; } }

		public override int OldStrengthReq{ get{ return 30; } }
		public override int NewMinDamage{ get{ return 6; } }
		public override int NewMaxDamage{ get{ return 18; } }
		//public override int DiceDamage { get { return Utility.Dice( 6, 3, 0 ); } } // 6d3 + 0 (6-18)
		public override int OldSpeed{ get{ return 30; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 60; } }

		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get{ return m_UsesRemaining; }
			set{ m_UsesRemaining = value; InvalidateProperties(); }
		}

		//[CommandProperty( AccessLevel.GameMaster )]
		public ToolQuality ToolQuality
		{
			get{ return (ToolQuality)Quality; }
			set{ ScaleDurability(); Quality = (WeaponQuality)value; InvalidateProperties(); UnscaleDurability(); }
		}

		[Constructable]
		public SmithHammerWeapon() : this( Utility.RandomMinMax( 25, 75 ) )
		{
		}

		[Constructable]
		public SmithHammerWeapon( int uses ) : base( 0x13E3 )
		{
			m_UsesRemaining = uses;

			Weight = 8.0;
			Layer = Layer.OneHanded;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) || Parent == from )
			{
				CraftSystem system = DefBlacksmithy.CraftSystem;

				int num = system.CanCraft( from, this, null );

				if ( num > 0 )
					from.SendLocalizedMessage( num );
				else
					from.SendGump( new CraftGump( from, system, this, null ) );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}
		
		public override void UnscaleDurability()
		{
			base.UnscaleDurability();

			UnscaleUses();
		}

		public override void ScaleDurability()
		{
			base.ScaleDurability();

			ScaleUses();
		}

		public void ScaleUses()
		{
			m_UsesRemaining = ( m_UsesRemaining * GetUsesScalar() ) / 100;
		}

		public void UnscaleUses()
		{
			m_UsesRemaining = ( m_UsesRemaining * 100 ) / GetUsesScalar();
		}

		public int GetUsesScalar()
		{
			return Quality == WeaponQuality.Exceptional ? 200 : 100;
		}

		public SmithHammerWeapon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_UsesRemaining = reader.ReadInt();
		}
	}
}