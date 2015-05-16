#define CLIENT6017

#region References
using Server.Items;
using Server.Network;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class PacketHandlerOverrides
	{
		public static PacketHandler ContentChangeParent { get; private set; }
		public static PacketHandler UseReqParent { get; private set; }
		public static PacketHandler LookReqParent { get; private set; }

		public static void InvokeContentChangeParent(NetState state, PacketReader pvSrc)
		{
			if (ContentChangeParent != null)
			{
				ContentChangeParent.OnReceive(state, pvSrc);
			}
			else
			{
				BaseBook.ContentChange(state, pvSrc);
			}
		}

		public static void InvokeUseReqParent(NetState state, PacketReader pvSrc)
		{
			if (UseReqParent != null)
			{
				UseReqParent.OnReceive(state, pvSrc);
			}
			else
			{
				PacketHandlers.UseReq(state, pvSrc);
			}
		}

		public static void InvokeLookReqParent(NetState state, PacketReader pvSrc)
		{
			if (LookReqParent != null)
			{
				LookReqParent.OnReceive(state, pvSrc);
			}
			else
			{
				PacketHandlers.LookReq(state, pvSrc);
			}
		}

		[CallPriority(-1)]
		public static void Configure()
		{
			EventSink.QuestGumpRequest += XmlQuest.QuestButton;

			ContentChangeParent = PacketHandlers.GetHandler(0x66);
			UseReqParent = PacketHandlers.GetHandler(0x06);
			LookReqParent = PacketHandlers.GetHandler(0x09);

			PacketHandlers.Register(0x66, 0, true, BaseEntryBook.ContentChange);
			PacketHandlers.Register(0x06, 5, true, XmlAttach.UseReq);
			PacketHandlers.Register(0x09, 5, true, XmlAttach.LookReq);

			#if CLIENT6017
			PacketHandlers.Register6017(0x66, 0, true, BaseEntryBook.ContentChange);
			PacketHandlers.Register6017(0x06, 5, true, XmlAttach.UseReq);
			PacketHandlers.Register6017(0x09, 5, true, XmlAttach.LookReq);
			#endif
		}
	}
}