#region References
using System;
using System.Collections.Generic;
using System.Linq;

using SQL = VitaNex.MySQL.MySQL;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public abstract class LegendState
	{
		public const string DataSep1 = "|";
		public const string DataSep2 = "¬";

		public static string JoinData(params object[] args)
		{
			return JoinData(args.Select(Convert.ToString));
		}

		public static string JoinData<T>(params T[] args)
		{
			return JoinData(args.Select(o => Convert.ToString(o)));
		}

		public static string JoinData<T>(IEnumerable<T> data)
		{
			return String.Join(DataSep1, data);
		}

		public static string JoinSubData(params object[] args)
		{
			return JoinSubData(args.Select(Convert.ToString));
		}

		public static string JoinSubData<T>(params T[] args)
		{
			return JoinSubData(args.Select(o => Convert.ToString(o)));
		}

		public static string JoinSubData<T>(IEnumerable<T> data)
		{
			return String.Join(DataSep2, data);
		}

		public abstract string TableName { get; }
		public abstract Type SupportedType { get; }

		public virtual bool IsSupported(object o)
		{
			return o != null && o.TypeEquals(SupportedType);
		}

		public bool Compile(object obj, out IDictionary<string, SimpleType> data)
		{
			data = null;

			if (!IsSupported(obj))
			{
				return false;
			}

			try
			{
				data = new Dictionary<string, SimpleType>();
				OnCompile(obj, data);

				if (data.Count > 0)
				{
					data.Add("updatestamp", DateTime.UtcNow);
				}
			}
			catch (Exception x)
			{
				UOFLegends.CMOptions.ToConsole("Compile failed on '{0}':", obj);
				UOFLegends.CMOptions.ToConsole(x);

				if (data != null)
				{
					data.Clear();
					data = null;
				}
			}

			return true;
		}

		protected abstract void OnCompile(object o, IDictionary<string, SimpleType> data);
	}

	public abstract class LegendState<T> : LegendState
	{
		public override Type SupportedType { get { return typeof(T); } }

		protected override sealed void OnCompile(object o, IDictionary<string, SimpleType> data)
		{
			OnCompile((T)o, data);
		}

		protected abstract void OnCompile(T o, IDictionary<string, SimpleType> data);
	}
}