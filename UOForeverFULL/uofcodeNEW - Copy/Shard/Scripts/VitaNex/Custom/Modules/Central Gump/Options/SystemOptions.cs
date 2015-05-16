#region References

using VitaNex;

#endregion

namespace Server.Engines.CentralGump
{
    public sealed class CentralGumpOptions : CoreServiceOptions
    {
        public CentralGumpOptions()
            : base(typeof(CentralGump))
        {
            EnsureDefaults();
        }

        public CentralGumpOptions(GenericReader reader)
            : base(reader)
        {}

        public void EnsureDefaults()
        {
        }

        public override void Clear()
        {
            base.Clear();

            EnsureDefaults();
        }

        public override void Reset()
        {
            base.Reset();

            EnsureDefaults();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    break;
            }
        }
    }
}