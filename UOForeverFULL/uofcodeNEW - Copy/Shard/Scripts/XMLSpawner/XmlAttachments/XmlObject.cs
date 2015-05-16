#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using Server.Commands;
using Server.Guilds;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class XmlObject : XmlAttachment
	{
		private object m_DataValue;

		[CommandProperty(AccessLevel.GameMaster)]
		public object Value { get { return m_DataValue; } set { m_DataValue = value; } }

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlObject(ASerial serial)
			: base(serial)
		{ }

		[Attachable]
		public XmlObject(string name, object value)
		{
			Name = name;
			Value = value;
		}

		[Attachable]
		public XmlObject(string name, object value, double expiresin)
		{
			Name = name;
			Value = value;
			Expiration = TimeSpan.FromMinutes(expiresin);
		}

		public enum XmlObjectType
		{
			Other = 0,
			Mobile = 1,
			Item = 2,
			MobileList = 3,
			ItemList = 4,
			DateTime = 5,
			DateTimeOffset = 6,
			BaseGuild = 7,
			Boolean = 8,
			Map = 9,
			Point3D = 10,
			Point2D = 11,
			IPAddress = 12,
			TimeSpan = 13,
			Rectangle2D = 14,
			Rectangle3D = 15,
			ArrayList = 16,
			Double = 17,
			Integer = 18,
			UInt64 = 19,
			String = 20,
			Type = 21
		}

		public bool IsSerializable(object obj)
		{
			return obj is Mobile || obj is Item || obj is List<Mobile> || obj is List<Item> || obj is ArrayList ||
				   obj is DateTime || obj is DateTimeOffset || obj is BaseGuild || obj is Boolean || obj is Map || obj is Point3D ||
				   obj is Point2D || obj is IPAddress || obj is TimeSpan || obj is Rectangle2D || obj is Rectangle3D ||
				   obj is Type || obj is int || obj is double || obj is string || obj is ulong;
		}

		public void SerializeObject(GenericWriter writer, object obj)
		{
			if (obj is Mobile)
			{
				writer.Write((int)XmlObjectType.Mobile);
				writer.Write((Mobile)obj);
			}
			else if (obj is Item)
			{
				writer.Write((int)XmlObjectType.Item);
				writer.Write((Item)obj);
			}
			else if (obj is List<Mobile>)
			{
				writer.Write((int)XmlObjectType.MobileList);
				writer.WriteMobileList((List<Mobile>)obj, true);
			}
			else if (obj is List<Item>)
			{
				writer.Write((int)XmlObjectType.ItemList);
				writer.WriteItemList((List<Item>)obj, true);
			}
			else if (obj is ArrayList)
			{
				// guess at the type of arraylist
				ArrayList list = (ArrayList)obj;
				writer.Write((int)XmlObjectType.ArrayList);
				var toSerialize = new List<object>();
				for (int i = 0; i < list.Count; i++)
				{
					if (IsSerializable(list[i]))
					{
						toSerialize.Add(list[i]);
					}
				}
				writer.Write(toSerialize.Count);
				foreach (object thingy in toSerialize)
				{
					SerializeObject(writer, thingy);
				}
			}
			else if (obj is DateTime)
			{
				writer.Write((int)XmlObjectType.DateTime);
				writer.Write((DateTime)obj);
			}
			else if (obj is DateTimeOffset)
			{
				writer.Write((int)XmlObjectType.DateTimeOffset);
				writer.Write((DateTimeOffset)obj);
			}
			else if (obj is BaseGuild)
			{
				writer.Write((int)XmlObjectType.BaseGuild);
				writer.Write((BaseGuild)obj);
			}
			else if (obj is IPAddress)
			{
				writer.Write((int)XmlObjectType.IPAddress);
				writer.Write((IPAddress)obj);
			}
			else if (obj is Map)
			{
				writer.Write((int)XmlObjectType.Map);
				writer.Write((Map)obj);
			}
			else if (obj is Point3D)
			{
				writer.Write((int)XmlObjectType.Point3D);
				writer.Write((Point3D)obj);
			}
			else if (obj is Point2D)
			{
				writer.Write((int)XmlObjectType.Point2D);
				writer.Write((Point2D)obj);
			}
			else if (obj is TimeSpan)
			{
				writer.Write((int)XmlObjectType.TimeSpan);
				writer.Write((TimeSpan)obj);
			}
			else if (obj is Rectangle2D)
			{
				writer.Write((int)XmlObjectType.Rectangle2D);
				writer.Write((Rectangle2D)obj);
			}
			else if (obj is Rectangle3D)
			{
				writer.Write((int)XmlObjectType.Rectangle3D);
				writer.Write((Rectangle3D)obj);
			}
			else if (obj is double)
			{
				writer.Write((int)XmlObjectType.Double);
				writer.Write((double)obj);
			}
			else if (obj is int)
			{
				writer.Write((int)XmlObjectType.Integer);
				writer.Write((int)obj);
			}
			else if (obj is bool)
			{
				writer.Write((int)XmlObjectType.Boolean);
				writer.Write((bool)obj);
			}
			else if (obj is ulong)
			{
				writer.Write((int)XmlObjectType.UInt64);
				writer.Write((ulong)obj);
			}
			else if (obj is string)
			{
				writer.Write((int)XmlObjectType.String);
				writer.Write((string)obj);
			}
			else if (obj is Type)
			{
				writer.Write((int)XmlObjectType.Type);
				var splitType = obj.ToString().Split('.');
				writer.Write(splitType[splitType.Length - 1]);
			}
			else
			{
				writer.Write((int)XmlObjectType.Other);
				LoggingCustom.Log(
					"ERROR_Uberscript.txt",
					DateTime.Now + ": xmlobject: " + Name + " attached to " + AttachedTo +
					" with data that cannot be serialized: data = " + m_DataValue);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
			// version 0
			SerializeObject(writer, m_DataValue);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
			m_DataValue = DeserializeObject(reader);
            if (m_DataValue == null)
            {
                Delete();
            }
		}

		public object DeserializeObject(GenericReader reader)
		{
			object output = null;
			XmlObjectType xmlObjectType = (XmlObjectType)reader.ReadInt();

			if (xmlObjectType == XmlObjectType.Mobile)
			{
				output = reader.ReadMobile();
			}
			else if (xmlObjectType == XmlObjectType.Item)
			{
				output = reader.ReadItem();
			}
			else if (xmlObjectType == XmlObjectType.MobileList)
			{
				output = reader.ReadStrongMobileList();
			}
			else if (xmlObjectType == XmlObjectType.ItemList)
			{
				output = reader.ReadStrongItemList();
			}
			else if (xmlObjectType == XmlObjectType.ArrayList)
			{
				int elements = reader.ReadInt();
				ArrayList list = new ArrayList(elements);
				for (int i = 0; i < elements; i++)
				{
					list.Add(DeserializeObject(reader));
				}
				output = list;
			}
			else if (xmlObjectType == XmlObjectType.DateTime)
			{
				output = reader.ReadDateTime();
			}
			else if (xmlObjectType == XmlObjectType.DateTimeOffset)
			{
				output = reader.ReadDateTimeOffset();
			}
			else if (xmlObjectType == XmlObjectType.BaseGuild)
			{
				output = reader.ReadGuild();
			}
			else if (xmlObjectType == XmlObjectType.IPAddress)
			{
				output = reader.ReadIPAddress();
			}
			else if (xmlObjectType == XmlObjectType.Map)
			{
				output = reader.ReadMap();
			}
			else if (xmlObjectType == XmlObjectType.Point3D)
			{
				output = reader.ReadPoint3D();
			}
			else if (xmlObjectType == XmlObjectType.Point2D)
			{
				output = reader.ReadPoint2D();
			}
			else if (xmlObjectType == XmlObjectType.TimeSpan)
			{
				output = reader.ReadTimeSpan();
			}
			else if (xmlObjectType == XmlObjectType.Rectangle2D)
			{
				output = reader.ReadRect2D();
			}
			else if (xmlObjectType == XmlObjectType.Rectangle3D)
			{
				output = reader.ReadRect3D();
			}
			else if (xmlObjectType == XmlObjectType.Double)
			{
				output = reader.ReadDouble();
			}
			else if (xmlObjectType == XmlObjectType.Integer)
			{
				output = reader.ReadInt();
			}
			else if (xmlObjectType == XmlObjectType.Boolean)
			{
				output = reader.ReadBool();
			}
			else if (xmlObjectType == XmlObjectType.UInt64)
			{
				output = reader.ReadULong();
			}
			else if (xmlObjectType == XmlObjectType.String)
			{
				output = reader.ReadString();
			}
			else if (xmlObjectType == XmlObjectType.Type)
			{
				output = UberScriptFunctions.Methods.TYPE(null, reader.ReadString());
			}
			else if (xmlObjectType == XmlObjectType.Other)
			{
				LoggingCustom.Log(
					"ERROR_Uberscript.txt",
					DateTime.Now + ": xmlobject: " + Name + " attached to " + AttachedTo +
					" with data of type other was deserialized");
			}
			return output;
		}

		public override string OnIdentify(Mobile from)
		{
			if (from == null || from.AccessLevel == AccessLevel.Player)
			{
				return null;
			}

			if (Expiration > TimeSpan.Zero)
			{
				return String.Format("{2}: Value {0} expires in {1} mins", Value, Expiration.TotalMinutes, Name);
			}
			else
			{
				return String.Format("{1}: Value {0}", Value, Name);
			}
		}

		public override void OnDelete()
		{
			Value = null;
			base.OnDelete();
		}
	}
}