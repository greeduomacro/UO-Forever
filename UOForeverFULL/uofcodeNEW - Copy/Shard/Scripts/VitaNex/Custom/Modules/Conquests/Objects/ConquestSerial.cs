#region References
using System.Globalization;

using VitaNex;
using VitaNex.Crypto;
#endregion

namespace Server.Engines.Conquests
{
	[PropertyObject]
	public sealed class ConquestSerial : CryptoHashCode
	{
		public static CryptoHashType Algorithm = CryptoHashType.MD5;

		public ConquestSerial()
			: base(Algorithm, TimeStamp.UtcNow.Stamp + "+" + Utility.RandomDouble())
		{ }

		public ConquestSerial(GenericReader reader)
			: base(reader)
		{ }
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}