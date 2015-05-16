using System;
using System.Collections.Generic;

namespace Server
{
	public enum MagicGrade
	{
		NonMagical,
		NonetoLowest,
		NonetoLower,
		NonetoMedium,
		NonetoHigh,
		NonetoHighest30,
		NonetoHighest40,
		NonetoHighest45,
		NonetoHighest49,
		LowesttoHighest,
		LowertoHighest60,
		LowertoHighest65,
		LowtoHighest70,
		LowtoHighest74,
		MediumtoHighest,
		HightoHighest
	}

/* For this class to work, the following must be inserted when using:

		public static void Configure()
		{
			GenerateLootSets();
		}
*/

	public class MagicLootSets<T> where T : BaseLootSet
	{
		private static Dictionary<MagicGrade, T> m_LootSets = new Dictionary<MagicGrade, T>();
        public static Dictionary<MagicGrade, T> LootSets { get { return m_LootSets; } }

		public static void GenerateLootSets()
		{
			for ( int i = 0; i < LootSystem.MagicGrades.Length; i++ )
			{
				int min;
				int max;
				LootSystem.GetMagicMinMax( LootSystem.MagicGrades[i], out min, out max );
				Add( LootSystem.MagicGrades[i], Activator.CreateInstance( typeof(T), min, max ) as T );
			}
		}

		public static T LootSet(MagicGrade index)
		{
			T tobj;
			m_LootSets.TryGetValue( index, out tobj );
			return tobj;
		}

        public static bool Exists(MagicGrade index)
        {
            return m_LootSets.ContainsKey(index);
        }

        private static void Add(MagicGrade index, T obj)
        {
            if (Exists(index))
                Console.WriteLine( String.Format("Loot: Attempted to add {0}.  This index already exists.", index) );
            else
                m_LootSets.Add(index, obj);
        }
	}
}