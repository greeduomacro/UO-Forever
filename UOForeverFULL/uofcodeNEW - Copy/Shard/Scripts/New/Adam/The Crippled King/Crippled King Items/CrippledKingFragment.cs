#region References
using System;

using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Items
{
	public sealed class CrippledKingFragment : Item
	{
	    private int _Contribution;
        private string _Owner;

		[Constructable]
		public CrippledKingFragment()
            : base(12253)
		{
            Name = "a refined stone fragment";
			Weight = 2;
			Hue = 2407;

		    LootType = LootType.Blessed;
		}

        public CrippledKingFragment(Serial serial)
			: base(serial)
		{ }

		public override void OnSingleClick(Mobile m)
		{
		    base.OnSingleClick(m);
            LabelTo(m, "Pulses with a strong alien energy.", 54);
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