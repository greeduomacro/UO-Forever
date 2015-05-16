#region References
#endregion

namespace Server.Engines.Conquests
{
    public sealed class ConquestOneMonthVeteranStatue : Item
    {
	    public override bool ExpansionChangeAllowed { get { return true; } }

	    [Constructable]
        public ConquestOneMonthVeteranStatue()
            : base(4646)
        {
            Name = "one month veteran statue";
            LootType = LootType.Blessed;
            Weight = 2;
            Hue = 1165;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "We hope you enjoyed your first month on Ultima Online Forever! - UOF Staff", 1165);
        }

        public ConquestOneMonthVeteranStatue(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();
        }
    }
}