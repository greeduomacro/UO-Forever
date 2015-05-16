#region References

using System.Linq;
using Server.Network;

#endregion

namespace Server.Items
{
    public class MountedPixieArtifact : Item
    {
        [Constructable]
        public MountedPixieArtifact()
            : base(10867)
        {
            Name = "mounted pixie";
            Weight = 2;
            Movable = true;
        }

        public MountedPixieArtifact(Serial serial)
            : base(serial)
        {}

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);
            LabelTo(m,
                "[Champion Artifact]",
                134);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}