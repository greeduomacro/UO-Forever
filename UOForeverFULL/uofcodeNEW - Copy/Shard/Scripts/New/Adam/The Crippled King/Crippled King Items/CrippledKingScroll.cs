#region References
using System;

using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Items
{
	public sealed class CrippledKingScroll : Item
	{
	    private int _Contribution;
        private string _Owner;

		[Constructable]
		public CrippledKingScroll(string owner, int contribution)
			: base(7989)
		{
            Name = "a commemorative scroll";
			Weight = 2;
			Hue = 1194;

		    _Owner = owner;
		    _Contribution = contribution;
		    LootType = LootType.Blessed;
		}

        public CrippledKingScroll(Serial serial)
			: base(serial)
		{ }

		public override void OnSingleClick(Mobile m)
		{
		    base.OnSingleClick(m);
            LabelTo(m, "This scroll is a commemoration to the decipherment of the Tome of Chaos.  The noble " + _Owner + " contributed " + _Contribution + " chaos scroll(s) to the endeavor.", 54);
		}

        public override void OnDoubleClick(Mobile m)
        {
            base.OnDoubleClick(m);
            LabelTo(m, "The scroll reads: Speak the words of power Ex Por Rel Uus to open the gates of the Marble Keep.", 54);
        }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

            writer.Write(_Owner);
            writer.Write(_Contribution);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

		    _Owner = reader.ReadString();
            _Contribution = reader.ReadInt();

		}
	}
}