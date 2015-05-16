#region References
using System.Globalization;

using VitaNex;
using VitaNex.Crypto;
#endregion

namespace Server.Poker
{
    [PropertyObject]
    public sealed class PokerPlayerSerial : CryptoHashCode
    {
        public static CryptoHashType Algorithm = CryptoHashType.MD5;

        public PokerPlayerSerial()
            : base(Algorithm, TimeStamp.UtcNow.Stamp.ToString(CultureInfo.InvariantCulture) + '+' + Utility.RandomDouble())
        { }

        public PokerPlayerSerial(GenericReader reader)
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
