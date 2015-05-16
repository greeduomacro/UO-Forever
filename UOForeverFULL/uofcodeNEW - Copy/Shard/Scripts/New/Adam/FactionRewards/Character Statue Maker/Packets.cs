namespace Server.Network
{
    public class UpdateStatueAnimationSA : Packet
    {
        public UpdateStatueAnimationSA(Mobile m, int animation, int frame)
            : base(0xBF)
        {
            EnsureCapacity(7);
            m_Stream.Write((short)0x2B);
            m_Stream.Write((short)m.Serial);
            m_Stream.Write((byte)animation);
            m_Stream.Write((byte)frame);
        }
    }
}