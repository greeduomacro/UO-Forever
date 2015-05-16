using System;
using Server;

namespace Server
{
	public class PSerial
	{
		public static ulong ReadEncodedLong( GenericReader reader )
		{
			ulong v = 0;
			int shift = 0;
			byte b;

			do
			{
				b = reader.ReadByte();
				v |= (b & 0x7Ful) << shift;
				shift += 7;
			} while( b >= 0x80 );

			return v;
		}

		public static void WriteEncoded( GenericWriter writer, long value )
		{
			ulong v = (ulong)value;

			while( v >= 0x80 )
			{
				writer.Write( (byte)(v | 0x80) );
				v >>= 7;
			}
			writer.Write( (byte)v );
		}
	}
}