using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Server
{
	public class SFMT
	{
		private ISFMT m_Randomizer;

		public SFMT()
		{
			if ( Environment.Is64BitProcess )
				m_Randomizer = new SFMT64( (uint)DateTime.UtcNow.Ticks );
			else
				m_Randomizer = new SFMT32( (uint)DateTime.UtcNow.Ticks );
		}

		public double NextDouble()
		{
			return m_Randomizer.NextDouble();
		}

		public int Next()
		{
			return m_Randomizer.Next();
		}

		public int Next( int value )
		{
			return m_Randomizer.Next( value );
		}

		public int Next( int minValue, int maxValue )
		{
			return m_Randomizer.Next( minValue, maxValue );
		}

		private interface ISFMT
		{
			double NextDouble();
			int Next();
			int Next( int value );
			int Next( int minValue, int maxValue );
		}

		private class SFMT32 : ISFMT
		{
			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern UInt32 gen_rand32();

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern UInt64 gen_rand64();

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern int fill_array32( UInt32[] array, int size );

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern int fill_array64( UInt64[] array, int size );

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern void init_gen_rand( UInt32 seed );

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern double to_real1( UInt32 value );

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern double genrand_real1();

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern double to_real2( UInt32 value );

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern double genrand_real2();

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern double to_real3( UInt32 value );

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern double genrand_real3();

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern double to_res53( UInt64 value );

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern double to_res53_mix( UInt32 value1, UInt32 value2 );

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern double genrand_res53();

			[DllImport( "SFMT32", CallingConvention = CallingConvention.Cdecl )]
			private static extern double genrand_res53_mix();

			public SFMT32( uint seed )
			{
				init_gen_rand( seed );
			}

			public double NextDouble()
			{
				return genrand_res53_mix();
			}

			public int Next()
			{
				return (int)gen_rand32();
			}

			public int Next( int value )
			{
				return (int)( value * genrand_res53_mix() );
			}

			public int Next( int minValue, int maxValue )
			{
				return (int)( (maxValue - minValue) * genrand_res53_mix() + minValue );
			}
		}

		private class SFMT64 : ISFMT
		{
			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern UInt32 gen_rand32();

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern UInt64 gen_rand64();

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern int fill_array32( UInt32[] array, int size );

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern int fill_array64( UInt64[] array, int size );

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern void init_gen_rand( UInt32 seed );

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern double to_real1( UInt32 value );

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern double genrand_real1();

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern double to_real2( UInt32 value );

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern double genrand_real2();

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern double to_real3( UInt32 value );

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern double genrand_real3();

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern double to_res53( UInt64 value );

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern double to_res53_mix( UInt32 value1, UInt32 value2 );

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern double genrand_res53();

			[DllImport( "SFMT64", CallingConvention = CallingConvention.Cdecl )]
			private static extern double genrand_res53_mix();

			public SFMT64( uint seed  )
			{
				init_gen_rand( seed );
			}

			public double NextDouble()
			{
				return genrand_res53_mix();
			}

			public int Next()
			{
				return (int)gen_rand32();
			}

			public int Next( int value )
			{
				return (int)( value * genrand_res53_mix() );
			}

			public int Next( int minValue, int maxValue )
			{
				return (int)( (maxValue - minValue) * genrand_res53_mix() + minValue );
			}
		}
	}
}