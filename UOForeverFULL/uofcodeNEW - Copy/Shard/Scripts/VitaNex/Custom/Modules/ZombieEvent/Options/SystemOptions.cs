#region References

using VitaNex;

#endregion

namespace Server.Engines.ZombieEvent
{
    public sealed class ZombieEventOptions : CoreServiceOptions
    {
        public ZombieEventOptions()
            : base(typeof(ZombieEvent))
        {
            EnsureDefaults();
        }

        public ZombieEventOptions(GenericReader reader)
            : base(reader)
        {}

        [CommandProperty(PlayerScores.Access)]
        public double MeleeMod { get; set; }

        [CommandProperty(PlayerScores.Access)]
        public int MaxZombiesQuadrant1 { get; set; }

        [CommandProperty(PlayerScores.Access)]
        public int MaxZombiesQuadrant2 { get; set; }

        [CommandProperty(PlayerScores.Access)]
        public int MaxZombiesQuadrant3 { get; set; }

        [CommandProperty(PlayerScores.Access)]
        public int MaxZombiesQuadrant4 { get; set; }

        public void EnsureDefaults()
        {
            MeleeMod = 3.0;

            MaxZombiesQuadrant1 = 3500;

            MaxZombiesQuadrant2 = 1400;

            MaxZombiesQuadrant3 = 1400;

            MaxZombiesQuadrant4 = 700;
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

            int version = writer.SetVersion(1);

            switch (version)
            {
                case 1:
                {
                    writer.Write(MaxZombiesQuadrant1);
                    writer.Write(MaxZombiesQuadrant2);
                    writer.Write(MaxZombiesQuadrant3);
                    writer.Write(MaxZombiesQuadrant4);
                }
                    goto case 0;
                case 0:
                    writer.Write(MeleeMod);
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                {
                    MaxZombiesQuadrant1 = reader.ReadInt();
                    MaxZombiesQuadrant2 = reader.ReadInt();
                    MaxZombiesQuadrant3 = reader.ReadInt();
                    MaxZombiesQuadrant4 = reader.ReadInt();
                }
                    goto case 0;
                case 0:
                    MeleeMod = reader.ReadDouble();
                    break;
            }
        }
    }
}