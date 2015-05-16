#region References
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Spells
{
	public class SpellRegistry
	{
		public static Type[] Types { get; private set; }

		public static string[] CircleNames { get; private set; }

		public static Dictionary<Type, Int32> TypeIDs { get; private set; }
		public static Dictionary<Type, SpellInfo> TypeInfos { get; private set; }

		static SpellRegistry()
		{
			Types = new Type[700];

			CircleNames = new[]
			{
				"First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth", "Necromancy", "Chivalry", "Bushido",
				"Ninjitsu", "Spellweaving"
			};

			TypeIDs = new Dictionary<Type, Int32>(Types.Length);
			TypeInfos = new Dictionary<Type, SpellInfo>(Types.Length);
		}

		public static int GetRegistryNumber(ISpell s)
		{
			return s != null ? GetRegistryNumber(s.GetType()) : -1;
		}

		public static int GetRegistryNumber(Type type)
		{
			int spellID;

			if (type == null || !TypeIDs.TryGetValue(type, out spellID))
			{
				spellID = -1;
			}

			return spellID;
		}

		public static SpellInfo GetSpellInfo(int spellID)
		{
			return Types.InBounds(spellID) ? GetSpellInfo(Types[spellID]) : null;
		}

		public static SpellInfo GetSpellInfo(ISpell s)
		{
			return s != null ? GetSpellInfo(s.GetType()) : null;
		}

		public static SpellInfo GetSpellInfo(Type type)
		{
			return type != null && TypeInfos.ContainsKey(type) ? TypeInfos[type] : null;
		}

		public static void Register(int spellID, Type type)
		{
			if (!Types.InBounds(spellID))
			{
				return;
			}

			var old = Types[spellID];

			if (old != null)
			{
				TypeIDs.Remove(old);
				TypeInfos.Remove(old);
			}

			Types[spellID] = type;

			//Console.WriteLine("Register: ID '{0}', replace '{1}' with '{2}'", spellID, old, type);

			if (type == null)
			{
				return;
			}

			TypeIDs.AddOrReplace(type, spellID);
			
			Spell s = type.CreateInstanceSafe<Spell>((Mobile)null, (Item)null);

			if (s != null && s.Info != null)
			{
				TypeInfos.AddOrReplace(type, s.Info);

				//Console.WriteLine("Register: Info resolved for '{0}'", s.Name);
			}
		}

        private static object[] m_Params = new object[2];

		public static Spell NewSpell(int spellID, Mobile caster, Item scroll)
		{
			//Console.WriteLine("NewSpell: '{0}' with caster '{1}', scroll '{2}'", spellID, caster, scroll);

			return Types.InBounds(spellID) && Types[spellID] != null
					   ? Types[spellID].CreateInstanceSafe<Spell>(caster, scroll)
					   : null;
		}

        public static Spell NewSpell(string name, Mobile caster, Item scroll)
        {
            for (int i = 0; i < CircleNames.Length; ++i)
            {
                Type t = ScriptCompiler.FindTypeByFullName(String.Format("Server.Spells.{0}.{1}", CircleNames[i], name));
                if (t != null)
                {
                    m_Params[0] = caster;
                    m_Params[1] = scroll;
                    try
                    {
                        return (Spell)Activator.CreateInstance(t, m_Params);
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }
	}
}