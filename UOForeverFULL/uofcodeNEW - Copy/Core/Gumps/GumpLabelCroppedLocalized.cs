/***************************************************************************
 *                            GumpLabelCropped.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: GumpLabelCropped.cs 4 2006-06-15 04:28:39Z mark $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using Server.Network;

namespace Server.Gumps
{
	public class GumpLabelCroppedLocalized : GumpEntry
	{
		private int m_X, m_Y;
		private int m_Width, m_Height;
		private int m_Hue;
		private int m_Number;
		private object[] m_Args;

		public int X
		{
			get
			{
				return m_X;
			}
			set
			{
				Delta( ref m_X, value );
			}
		}

		public int Y
		{
			get
			{
				return m_Y;
			}
			set
			{
				Delta( ref m_Y, value );
			}
		}

		public int Width
		{
			get
			{
				return m_Width;
			}
			set
			{
				Delta( ref m_Width, value );
			}
		}

		public int Height
		{
			get
			{
				return m_Height;
			}
			set
			{
				Delta( ref m_Height, value );
			}
		}

		public int Hue
		{
			get
			{
				return m_Hue;
			}
			set
			{
				Delta( ref m_Hue, value );
			}
		}

		public int Number
		{
			get
			{
				return m_Number;
			}
			set
			{
				Delta( ref m_Number, value );
			}
		}

		public object[] Args
		{
			get
			{
				return m_Args;
			}
			set
			{
				Delta( ref m_Args, value );
			}
		}

		public GumpLabelCroppedLocalized( int x, int y, int width, int height, int hue, int number ) : this( x, y, width, height, hue, number, null )
		{
		}

		public GumpLabelCroppedLocalized( int x, int y, int width, int height, int hue, int number, object[] args )
		{
			m_X = x;
			m_Y = y;
			m_Width = width;
			m_Height = height;
			m_Hue = hue;
			m_Number = number;
		}

		public override string Compile( NetState ns )
		{
			StringEntry entry = ns.Mobile.GetStringEntry( m_Number );
			if ( entry != null )
			{ //We don't care if its localized, since Labels are not localized...
				if ( m_Args != null && m_Args.Length > 0 )
					return String.Format( "{{ croppedtext {0} {1} {2} {3} {4} {5} }}", m_X, m_Y, m_Width, m_Height, m_Hue, Parent.Intern( entry.CombinedWithObjArguments( m_Args ) ) );
				else
					return String.Format( "{{ croppedtext {0} {1} {2} {3} {4} {5} }}", m_X, m_Y, m_Width, m_Height, m_Hue, Parent.Intern( entry.Text ) );
			}
			else
				return null;
		}

		private static byte[] m_LayoutName = Gump.StringToBuffer( "croppedtext" );

		public override void AppendTo( NetState ns, IGumpWriter disp )
		{
			disp.AppendLayout( m_LayoutName );
			disp.AppendLayout( m_X );
			disp.AppendLayout( m_Y );
			disp.AppendLayout( m_Width );
			disp.AppendLayout( m_Height );
			disp.AppendLayout( m_Hue );

			StringEntry entry = ns.Mobile.GetStringEntry( m_Number );
			if ( entry != null )
			{ //We don't care if its localized, since Labels are not localized...
				if ( m_Args != null && m_Args.Length > 0 )
					disp.AppendLayout( Parent.Intern( entry.CombinedWithObjArguments( m_Args ) ) );
				else
					disp.AppendLayout( Parent.Intern( entry.Text ) );
			}
		}
	}
}