using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public enum PaintingQuality
	{
		Amatuer,
		Neophyte,
		Novice,
		Apprentice,
		Expert,
		Artisan,
		Master,
		Grandmaster,
		Legendary
	}

	public class BasePainting : Item
	{
		private PaintingQuality m_Quality;
		private string m_Artist;

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public PaintingQuality Quality{ get{ return m_Quality; } set{ m_Quality = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public string Artist{ get{ return m_Artist; } set{ m_Artist = value; } }

		public BasePainting( int itemid ) : this( itemid, PaintingQuality.Amatuer )
		{
		}

		public BasePainting( int itemid, PaintingQuality quality ) : base( itemid )
		{
			m_Quality = quality;
		}

		public void SetRandomArtist()
		{
			NameList list = NameList.GetNameList( Utility.RandomBool() ? "male" : "female" );
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			string name = Name;

			if ( String.IsNullOrEmpty( name ) )
				name = DefaultName;

			if ( String.IsNullOrEmpty( name ) )
				name = "painting";

			LabelTo( from, String.Format( "{0} {1}", GetQualityName(), name ) );
		}

		public virtual string GetQualityName()
		{
			switch ( m_Quality )
			{
				default:
				case PaintingQuality.Amatuer: return "an amatuer";
				case PaintingQuality.Neophyte: return "a neophyte";
				case PaintingQuality.Novice: return "a novice";
				case PaintingQuality.Apprentice: return "an apprentice";
				case PaintingQuality.Expert: return "an expert";
				case PaintingQuality.Artisan: return "an artisan";
				case PaintingQuality.Master: return "a master";
				case PaintingQuality.Grandmaster: return "a grandmaster";
				case PaintingQuality.Legendary: return "a legendary";
			}
		}

		public BasePainting( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.WriteEncodedInt( (int)m_Quality );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Quality = (PaintingQuality)reader.ReadEncodedInt();
		}
	}
}