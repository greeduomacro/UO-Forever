using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
	public class InfoTile : Item
	{
		#region Variables
		private string m_Message;
		private int m_Range;
		private bool m_Active;

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual string Message
		{
			get { return m_Message; }
			set { m_Message = value; }
		}


		[CommandProperty( AccessLevel.GameMaster )]
		public int Range
		{
			get { return m_Range; }
			set { m_Range = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get { return m_Active; }
			set { m_Active = value; InvalidateProperties(); }
		}
		#endregion

		public override string DefaultName{ get{ return "Help Trigger"; } }

		[Constructable]
		public InfoTile() : base( 0x1F1C )
		{
			ItemID = 1305;
			Hue = 1365;
			Visible = false;
			Movable = false;
			m_Range = 1;
			m_Active = false;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1060658, "Range\t{0}", m_Range);
			list.Add(1060659, "Active\t{0}", m_Active);
		}

		public override bool HandlesOnMovement { get { return true; } }

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if ( m_Active && m.Player )
			{
				bool oldrange = Utility.InRange( oldLocation, Location, m_Range );
				bool newrange = Utility.InRange( m.Location, Location, m_Range );

				if ( newrange && !oldrange )
					m.SendGump( new InfoTileGump( m, this ) );
				else if ( !newrange && oldrange )
					m.CloseGump( typeof( InfoTileGump ) );
			}
		}

		public InfoTile( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 ); // version

			writer.Write( (bool)m_Active );
			writer.Write( (int)m_Range );
			writer.Write( (string)m_Message );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();

			m_Active = reader.ReadBool();
			m_Range = reader.ReadInt();
			m_Message = reader.ReadString();
		}
	}

	public class InfoTileGump : Gump
	{
		private Mobile m_From;
		private InfoTile m_Tile;
		private Timer m_Timer;

		public virtual void StartTimer()
		{
			if ( m_Timer == null )
				m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ), new TimerCallback( Refresh ) );
		}

		public virtual void StopTimer()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;
		}

		public InfoTileGump( Mobile from, InfoTile tile ) : base( 100, 100 )
		{
			m_From = from;
			m_Tile = tile;

			StartTimer();

			AddPage( 0 );
			AddBackground(400, 30, 375, 400, 9380);
			AddBackground(430, 80, 315, 300, 9300);

			AddHtml( 440, 90, 295, 280, m_Tile.Message, false, true );
		}

		public virtual void Refresh()
		{
			if ( !Utility.InRange( m_From.Location, m_Tile.Location, m_Tile.Range ) )
			{
				StopTimer();
				m_From.CloseGump( typeof( InfoTileGump ) );
			}
		}
	}
}