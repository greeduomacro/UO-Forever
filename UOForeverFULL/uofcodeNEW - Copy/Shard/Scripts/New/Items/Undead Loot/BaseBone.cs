using System;
using Server;

namespace Server.Items
{
	public enum BoneType
	{
		Regular,
		Preserved,
		Ancient,
		Fossilized,
		Ethereal,
		Demonic
	}

	public class BaseBone : Item
	{
		private BoneType m_BoneType;

		[CommandProperty( AccessLevel.Administrator )]
		public BoneType BoneType{ get{ return m_BoneType; } set{ SetBoneType( value ); } }

		[Constructable]
		public BaseBone( int itemID ) : this( itemID, BoneType.Regular )
		{
		}

		[Constructable]
		public BaseBone( int itemID, BoneType type ) : base( itemID )
		{
			SetBoneType( type );
		}

		public void SetBoneType( BoneType type )
		{
			m_BoneType = type;

			switch ( type )
			{
				default:
				case BoneType.Regular: Hue = 0; break;
				case BoneType.Preserved: Hue = 2101; break;
				case BoneType.Ancient: Hue = 2105; break;
				case BoneType.Fossilized: Hue = 2112; break;
				case BoneType.Ethereal: Hue = 1409; break;
				case BoneType.Demonic: Hue = 2206; break;
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelToExpansion(from);

			string name = Name;

			if ( String.IsNullOrEmpty( name ) )
			{
				StringList stringlist = StringList.Default;

				if ( from.NetState != null && from.NetState.LTable != null )
					stringlist = from.NetState.LTable;

				if ( stringlist[LabelNumber] != null )
					name = stringlist[LabelNumber].Text;
			}

			if ( m_BoneType != BoneType.Regular )
			{
				if ( name.StartsWith( "a " ) && name.Length > 2 )
					name = name.Substring( 2, name.Length-2 );
				else if ( name.StartsWith( "an " ) && name.Length > 3 )
					name = name.Substring( 3, name.Length-3 );
			}

			string type = String.Empty;

			if ( m_BoneType != BoneType.Regular )
				type = String.Format( "{0} ", GetBoneTypeName() );

			LabelTo( from, String.Format( "{0}{1}", type, name ), 1153 );
		}

		private string GetBoneTypeName()
		{
			switch ( m_BoneType )
			{
				default:
				case BoneType.Regular: return String.Empty;
				case BoneType.Preserved: return "a preserved";
				case BoneType.Ancient: return "an ancient";
				case BoneType.Fossilized: return "a fossilized";
				case BoneType.Ethereal: return "an ethereal";
				case BoneType.Demonic: return "a demonic";
			}
		}

		public BaseBone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.WriteEncodedInt( (int)m_BoneType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_BoneType = (BoneType)reader.ReadEncodedInt();
		}
	}
}