using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Network
{
    /// <summary>
    /// This is Alan's mod for debugging (and being able to reproduce) specific packet issues.
    /// </summary>
    public class PacketSimulation
    {
        public DateTime ProcessedTime;
        public byte[] ProcessedBytes;
        public int PacketLength;
        public string Message;

        public PacketSimulation(byte[] processedBytes, int packetLength, string message)
        {
            ProcessedTime = DateTime.UtcNow;
            ProcessedBytes = processedBytes;
            PacketLength = packetLength;
            Message = message == null ? "" : message;
        }

        public virtual void Process(NetState ns)
        {
            throw new NotImplementedException();
        }
    }

    public class PacketReceiveSimulation : PacketSimulation
    {
        public Type ActualPacketType;
        public PacketHandler Handler;
       
        public PacketReceiveSimulation(byte[] sentBytes, int length, string message, PacketHandler handler) 
            : base(sentBytes, length, message)
        {
            Handler = handler;
        }

        public override void Process(NetState ns)
        {
            Console.WriteLine("Executing Logged Receive: " + Handler.OnReceive.Method.ToString());
            PacketReader r = new PacketReader(ProcessedBytes, PacketLength, Handler.Length != 0);
            Handler.OnReceive(ns, r);
        }
    }
    public class PacketSendSimulation : PacketSimulation
    {
        public Type ActualPacketType;
        public PacketSendSimulation(byte[] sentBytes, int length, string message, Type actualPacketType)
            : base(sentBytes, length, message)
        {
            ActualPacketType = actualPacketType;
        }

        public override void Process(NetState ns)
        {
            Console.WriteLine("Executing Logged Send: " + ActualPacketType);
            ns.SendSimulatedPacket(this);
        }
    }

    public class PacketSimulationLogger
    {
        public Queue<PacketSimulation> Packets = new Queue<PacketSimulation>();
        public void AddSendPacket(byte[] processedBytes, int length, string message, Type actualPacketType)
        {
            byte[] toAdd = new byte[processedBytes.Length];
            for (int i = 0; i < length; i++)
            {
                toAdd[i] = processedBytes[i];
            }
            Packets.Enqueue(new PacketSendSimulation(toAdd, length, message, actualPacketType));
            Console.WriteLine("Logging Sent Packet: " + actualPacketType);
        }

        public void AddReceivePacket(byte[] processedBytes, int length, string message, PacketHandler handler)
        {
            byte[] toAdd = new byte[processedBytes.Length];
            for (int i = 0; i < length; i++)
            {
                toAdd[i] = processedBytes[i];
            }
            Packets.Enqueue(new PacketReceiveSimulation(toAdd, length, message, handler));
            Console.WriteLine("Logging Received Packet: " + PacketHandlers.Handlers[handler.PacketID].OnReceive.Method.ToString());
        }
    }
}
