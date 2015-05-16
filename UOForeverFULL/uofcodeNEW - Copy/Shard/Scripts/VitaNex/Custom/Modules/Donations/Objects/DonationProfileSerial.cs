#region References
using System.Globalization;

using VitaNex;
using VitaNex.Crypto;
#endregion

namespace Server.Engines.DonationsTracker
{
	[PropertyObject]
	public sealed class DonationProfileSerial : CryptoHashCode
	{
		public static CryptoHashType Algorithm = CryptoHashType.MD5;

		public DonationProfileSerial()
			: base(Algorithm, TimeStamp.UtcNow.Stamp.ToString(CultureInfo.InvariantCulture) + '+' + Utility.RandomDouble())
		{ }

        public DonationProfileSerial(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return Value;
		}

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