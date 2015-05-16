using System.Collections;
using System.Collections.Generic;

namespace VitaNex.MySQL
{
	public class MySQLRow : IEnumerable<MySQLData>
	{
		public int ID { get; private set; }
		public Dictionary<string, MySQLData> Results { get; private set; }

		public MySQLData this[string key]
		{
			get
			{
				MySQLData value;

				if (!Results.TryGetValue(key, out value))
				{
					value = MySQLData.Empty;
				}

				return value;
			}
		}

		public MySQLRow(int id, List<MySQLData> results)
			: this(id, results.ToArray())
		{ }

		public MySQLRow(int id, MySQLData[] results)
		{
			ID = id;
			Results = new Dictionary<string, MySQLData>(results.Length);

			foreach (MySQLData result in results)
			{
				Results.Add(result.Key, result);
			}
		}

		public MySQLRow(MySQLRow row)
		{
			ID = row.ID;
			Results = row.Results;
		}

		public IEnumerator<MySQLData> GetEnumerator()
		{
			return Results.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}